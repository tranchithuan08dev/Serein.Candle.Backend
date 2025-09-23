using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using Serein.Candle.Domain.Entities;
using Serein.Candle.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductAttributeValueRepository _productAttributeValueRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IImageService _imageService;

        public ProductService(
        IProductRepository productRepository,
        IProductImageRepository productImageRepository,
        IProductAttributeValueRepository productAttributeValueRepository,
        IInventoryRepository inventoryRepository,
        IImageService imageService)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _productAttributeValueRepository = productAttributeValueRepository;
            _inventoryRepository = inventoryRepository;
            _imageService = imageService;
        }

        public async Task<PagedResult<ProductDetailDto>> GetAllProductsAsync(int pageNumber, int pageSize, string? sortBy)
        {
            var allProductsQuery = _productRepository.GetAllProducts();

            // 1. Áp dụng logic sắp xếp
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        allProductsQuery = allProductsQuery.OrderBy(p => p.Name);
                        break;
                    case "price":
                        allProductsQuery = allProductsQuery.OrderBy(p => p.Price);
                        break;
                    case "createdat":
                        allProductsQuery = allProductsQuery.OrderByDescending(p => p.CreatedAt);
                        break;
                    default:
                        allProductsQuery = allProductsQuery.OrderBy(p => p.Name); // Sắp xếp mặc định
                        break;
                }
            }
            else
            {
                allProductsQuery = allProductsQuery.OrderBy(p => p.Name);
            }

            // 2. Lấy tổng số sản phẩm (trước khi phân trang)
            var totalCount = await allProductsQuery.CountAsync();

            // 3. Phân trang nếu pageSize lớn hơn 0
            if (pageSize > 0)
            {
                allProductsQuery = allProductsQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);
            }

            // 4. Ánh xạ dữ liệu và thực thi truy vấn
            var products = await allProductsQuery
                .Select(p => new ProductDetailDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    SKU = p.Sku,
                    Description = p.Description,
                    Ingredients = p.Ingredients,
                    BurnTime = p.BurnTime,
                    Price = p.Price,
                    IsActive = p.IsActive,
                    CategoryName = p.Category.Name,
                    Images = p.ProductImages.Select(i => new ProductImageDto
                    {
                        ImageUrl = i.ImageUrl,
                        IsPrimary = i.IsPrimary
                    }).ToList(),
                    Attributes = p.ProductAttributeValues.Select(a => new ProductAttributeDto
                    {
                        AttributeName = a.Attribute.Name,
                        Value = a.Value
                    }).ToList()
                }).ToListAsync();

            return new PagedResult<ProductDetailDto>
            {
                Data = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProductDetailDto?> GetProductDetailAsync(int productId)
        {
            var product = await _productRepository.GetProductDetailAsync(productId);
            if (product == null)
            {
                return null;
            }

            var productDetailDto = new ProductDetailDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                SKU = product.Sku,
                Description = product.Description,
                Ingredients = product.Ingredients,
                BurnTime = product.BurnTime,
                Price = product.Price,
                CategoryName = product.Category.Name,
                IsActive = product.IsActive,
                Attributes = product.ProductAttributeValues.Select(pav => new ProductAttributeDto
                {
                    AttributeName = pav.Attribute.Name,
                    Value = pav.Value
                }).ToList(),
                Images = product.ProductImages.Select(pi => new ProductImageDto
                {
                    ImageUrl = pi.ImageUrl,
                    IsPrimary = pi.IsPrimary
                }).ToList()
            };

            return productDetailDto;
        }

        public async Task<bool> InsertProductAsync(InsertProductDto productDto, IFormFileCollection images)
        {        // 1. Tải ảnh lên dịch vụ cloud
            var imageUrls = await _imageService.UploadImagesAsync(images);
            if (imageUrls == null || imageUrls.Count == 0)
            {
                return false;
            }

            // 2. Tạo đối tượng Product
            var newProduct = new Product
            {
                CategoryId = productDto.CategoryId,
                Name = productDto.Name,
                Sku = productDto.SKU,
                ShortDescription = productDto.ShortDescription,
                Description = productDto.Description,
                Ingredients = productDto.Ingredients,
                BurnTime = productDto.BurnTime,
                Price = productDto.Price,
                Slug = productDto.Name.ToLower().Replace(" ", "-"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _productRepository.AddProductAsync(newProduct);
            await _productRepository.SaveChangesAsync(); // Lưu để có ProductId

            // 3. Thêm hình ảnh
            int sortOrder = 0;
            foreach (var url in imageUrls)
            {
                var productImage = new ProductImage
                {
                    ProductId = newProduct.ProductId,
                    ImageUrl = url,
                    IsPrimary = (sortOrder == 0),
                    SortOrder = sortOrder
                };
                await _productImageRepository.AddProductImageAsync(productImage);
                sortOrder++;
            }

            // 4. Thêm thuộc tính sản phẩm
            foreach (var attrDto in productDto.Attributes)
            {
                var attribute = new ProductAttributeValue
                {
                    ProductId = newProduct.ProductId,
                    AttributeId = attrDto.AttributeId,
                    Value = attrDto.Value
                };
                await _productAttributeValueRepository.AddProductAttributeValueAsync(attribute);
            }

            // 5. Thêm bản ghi vào bảng Inventory
            var newInventory = new Inventory
            {
                ProductId = newProduct.ProductId,
                Quantity = 0,
                ReorderThreshold = 10,
                LastUpdated = DateTime.UtcNow
            };
            await _inventoryRepository.AddInventoryAsync(newInventory);

            // Lưu tất cả các thay đổi còn lại cùng một lúc
            await _productRepository.SaveChangesAsync();

            return true;

        }

        public async Task<bool> SoftDeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return false;
            }

            product.IsActive = false;

            return await _productRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProductAsync(int productId, UpdateProductDto productDto)
        {
            // 1. Tìm sản phẩm trong DB
            var existingProduct = await _productRepository.GetProductDetailAsync(productId);
            if (existingProduct == null)
            {
                return false;
            }

            // 2. Cập nhật các trường chính của sản phẩm
            existingProduct.Name = productDto.Name;
            existingProduct.Sku = productDto.SKU;
            existingProduct.ShortDescription = productDto.ShortDescription;
            existingProduct.Description = productDto.Description;
            existingProduct.Ingredients = productDto.Ingredients;
            existingProduct.BurnTime = productDto.BurnTime;
            existingProduct.Price = productDto.Price;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            if (existingProduct.Name != productDto.Name)
            {
                existingProduct.Slug = productDto.Name.ToLower().Replace(" ", "-");
            }

            // 3. Cập nhật thuộc tính sản phẩm
            // Xóa thuộc tính cũ
            if (existingProduct.ProductAttributeValues.Any())
            {
                await _productAttributeValueRepository.RemoveRangeAsync(existingProduct.ProductAttributeValues);
            }

            // Thêm thuộc tính mới
            foreach (var attrDto in productDto.Attributes)
            {
                var newAttribute = new ProductAttributeValue
                {
                    ProductId = existingProduct.ProductId,
                    AttributeId = attrDto.AttributeId,
                    Value = attrDto.Value
                };
                await _productAttributeValueRepository.AddProductAttributeValueAsync(newAttribute);
            }

            // 4. Lưu tất cả các thay đổi
            await _productRepository.UpdateProductAsync(existingProduct);
            await _productRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateProductImagesAsync(int productId, IFormFileCollection images)
        {
            // 1. Tìm sản phẩm trong cơ sở dữ liệu
            var existingProduct = await _productRepository.GetProductDetailAsync(productId);
            if (existingProduct == null)
            {
                return false;
            }

            // 2. Tải các file ảnh mới lên dịch vụ Cloudinary
            var newImageUrls = await _imageService.UploadImagesAsync(images);
            if (newImageUrls == null || !newImageUrls.Any())
            {
                return false;
            }

            // 3. Xóa tất cả hình ảnh cũ của sản phẩm
            if (existingProduct.ProductImages.Any())
            {
                await _productImageRepository.RemoveRangeAsync(existingProduct.ProductImages);
            }

            // 4. Tạo các đối tượng ProductImage mới và thêm vào DB
            int sortOrder = 0;
            foreach (var url in newImageUrls)
            {
                var newImage = new ProductImage
                {
                    ProductId = existingProduct.ProductId,
                    ImageUrl = url,
                    IsPrimary = (sortOrder == 0),
                    SortOrder = sortOrder
                };
                await _productImageRepository.AddProductImageAsync(newImage);
                sortOrder++;
            }

            // 5. Lưu tất cả thay đổi vào cơ sở dữ liệu
            await _productRepository.SaveChangesAsync();

            return true;
        }
    }
}

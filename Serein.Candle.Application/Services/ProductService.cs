using Microsoft.AspNetCore.Http;
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
    }
}

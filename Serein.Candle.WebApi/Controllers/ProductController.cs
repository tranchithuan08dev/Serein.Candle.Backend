using Microsoft.AspNetCore.Mvc;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using Serein.Candle.WebApi.Responses;
using System.Text.Json;

namespace Serein.Candle.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> InsertProduct([FromForm] InsertProductDto productDto, [FromForm] IFormFileCollection images)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    success: false,
                    message: "Invalid product data.",
                    data: ModelState
                ));
            }
          

            var result = await _productService.InsertProductAsync(productDto, images);

            if (result)
            {
                return Ok(new ApiResponse<object>(
                    success: true,
                    message: "Product added successfully.",
                    data: null
                ));
            }

            return BadRequest(new ApiResponse<object>(
                success: false,
                message: "Failed to add product. Please check the data and try again.",
                data: null
            ));
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDetailDto>>> GetAllProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 0,
        [FromQuery] string? sortBy = null)
        {
            var products = await _productService.GetAllProductsAsync(pageNumber, pageSize, sortBy);

            if (products == null || !products.Data.Any())
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductDetail(int id)
        {
            var product = await _productService.GetProductDetailAsync(id);

            if (product == null)
            {
                return NotFound(new ApiResponse<object>(
                    success: false,
                    message: $"Product with id {id} not found.",
                    data: null
                ));
            }

            return Ok(new ApiResponse<ProductDetailDto>(
                success: true,
                message: "Product retrieved successfully.",
                data: product
            ));
        }

        [HttpPut("{id}/images")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProductImages(int id, [FromForm] IFormFileCollection images)
        {
            if (images == null || images.Count == 0)
            {
                return BadRequest(new ApiResponse<object>(
                    success: false,
                    message: "No images provided.",
                    data: null
                ));
            }

            var result = await _productService.UpdateProductImagesAsync(id, images);

            return result
                ? Ok(new ApiResponse<object>(true, "Product images updated successfully.", null))
                : NotFound(new ApiResponse<object>(false, $"Product with id {id} not found or failed to update images.", null));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    success: false,
                    message: "Invalid product data.",
                    data: ModelState
                ));
            }

            var result = await _productService.UpdateProductAsync(id, productDto);

            return result
                ? Ok(new ApiResponse<object>(true, "Product updated successfully.", null))
                : NotFound(new ApiResponse<object>(false, $"Product with id {id} not found or failed to update.", null));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteProduct(int id)
        {
            var result = await _productService.SoftDeleteProductAsync(id);

            return result
                ? Ok(new ApiResponse<object>(true, "Product was soft deleted successfully.", null))
                : NotFound(new ApiResponse<object>(false, $"Product with id {id} not found or failed to delete.", null));
        }
    }
}

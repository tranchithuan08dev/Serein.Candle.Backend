using Microsoft.AspNetCore.Mvc;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using Serein.Candle.WebApi.Responses;

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
    }
}

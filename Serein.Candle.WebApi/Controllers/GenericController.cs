using Microsoft.AspNetCore.Mvc;
using Serein.Candle.Application.Interfaces;

namespace Serein.Candle.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenericController<T, TDto> : ControllerBase
    where T : class
    where TDto : class
    {
        private readonly IGenericService<T, TDto> _service;

        public GenericController(IGenericService<T, TDto> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TDto dto)
        {
            var result = await _service.AddAsync(dto);
            if (result) return CreatedAtAction(nameof(GetById), new { id = result }, dto); // Cần trả về ID
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result) return NoContent();
            return NotFound();
        }

        
    }
}

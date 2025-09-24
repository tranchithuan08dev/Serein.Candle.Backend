using Microsoft.AspNetCore.Mvc;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Domain.DTOs;
using Serein.Candle.Domain.Entities;

namespace Serein.Candle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : GenericController<Category, CategoryCRUDDto>
    {
        public CategoryController(IGenericService<Category, CategoryCRUDDto> service) : base(service)
        {
        }
    }
}

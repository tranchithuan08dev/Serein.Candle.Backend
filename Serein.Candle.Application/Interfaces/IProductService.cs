using Microsoft.AspNetCore.Http;
using Serein.Candle.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Interfaces
{
    public interface IProductService
    {
        Task<bool> InsertProductAsync(InsertProductDto productDto, IFormFileCollection images);
    }
}

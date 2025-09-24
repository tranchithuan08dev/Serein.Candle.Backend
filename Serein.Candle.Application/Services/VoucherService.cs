using AutoMapper;
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
    public class VoucherService : GenericService<Voucher, VoucherCRUDDto>, IVoucherService
    {
        public VoucherService(IGenericRepository<Voucher> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}

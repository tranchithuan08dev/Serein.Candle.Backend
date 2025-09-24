using Serein.Candle.Domain.DTOs;
using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.Interfaces
{
    public interface IVoucherService : IGenericService<Voucher, VoucherCRUDDto>
    {
        // Thêm các phương thức đặc thù cho Voucher tại đây nếu cần
    }
}

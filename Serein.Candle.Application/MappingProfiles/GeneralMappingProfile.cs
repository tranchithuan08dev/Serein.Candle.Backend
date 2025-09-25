using AutoMapper;
using Serein.Candle.Domain.DTOs;
using Serein.Candle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serein.Candle.Application.MappingProfiles
{
    public class GeneralMappingProfile : Profile
    {
        public GeneralMappingProfile()
        {
            CreateMap<Category, CategoryCRUDDto>().ReverseMap();
            // ProductAttribute Mappings
            CreateMap<ProductAttribute, ProductAttributeCRUDDto>().ReverseMap();

            CreateMap<Voucher, VoucherCRUDDto>().ReverseMap();

            CreateMap<RoleType, RoleTypeCRUDDto>().ReverseMap();
        }
    }
}

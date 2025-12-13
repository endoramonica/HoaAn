using AutoMapper;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;

namespace VietCommerce.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for Voucher entity
    /// Requirements: 3.1
    /// </summary>
    public class VoucherMappingProfile : Profile
    {
        public VoucherMappingProfile()
        {
            // Voucher to VoucherDto
            CreateMap<Voucher, VoucherDto>()
                .ReverseMap();

            // GenerateVouchersDto (no mapping needed, used directly)
        }
    }
}

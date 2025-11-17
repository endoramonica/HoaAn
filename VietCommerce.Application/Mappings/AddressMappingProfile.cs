using AutoMapper;
using VietCommerce.Core.DTOs.Address;
using VietCommerce.Core.Entities.Customers;

namespace VietCommerce.Application.Mappings
{
    public class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            // ==========================================
            // CREATE DTO → ENTITY
            // ==========================================
            CreateMap<CreateAddressDto, CustomerAddress>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());

            // ==========================================
            // UPDATE DTO → ENTITY (Partial Update)
            // ==========================================
            CreateMap<UpdateAddressDto, CustomerAddress>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                // ✅ Chỉ map nếu source không null (partial update)
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ==========================================
            // ENTITY → RESPONSE DTO (Full)
            // ==========================================
            CreateMap<CustomerAddress, AddressResponseDto>()
                .ForMember(dest => dest.AddressTypeDisplay,
                    opt => opt.MapFrom(src => src.AddressType.ToString()))
                .ForMember(dest => dest.FullAddress,
                    opt => opt.MapFrom(src => FormatFullAddress(src)));

            // ==========================================
            // ENTITY → LIST RESPONSE DTO (Simplified)
            // ==========================================
            CreateMap<CustomerAddress, AddressListResponseDto>()
                .ForMember(dest => dest.AddressTypeDisplay,
                    opt => opt.MapFrom(src => src.AddressType.ToString()))
                .ForMember(dest => dest.FullAddress,
                    opt => opt.MapFrom(src => FormatFullAddress(src)));
        }

        /// <summary>
        /// Helper method để format địa chỉ đầy đủ
        /// </summary>
        private static string FormatFullAddress(CustomerAddress address)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(address.StreetAddress))
                parts.Add(address.StreetAddress);

            if (!string.IsNullOrWhiteSpace(address.City))
                parts.Add(address.City);

            if (!string.IsNullOrWhiteSpace(address.State))
                parts.Add(address.State);

            if (!string.IsNullOrWhiteSpace(address.PostalCode))
                parts.Add(address.PostalCode);

            if (!string.IsNullOrWhiteSpace(address.Country))
                parts.Add(address.Country);

            return string.Join(", ", parts);
        }
    }
}
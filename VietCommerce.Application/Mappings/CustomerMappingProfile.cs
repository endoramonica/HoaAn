using AutoMapper;
using VietCommerce.Core.DTOs.Address;
using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.DTOs.CRM;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.CRM;

namespace VietCommerce.Application.Mappings;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        // ========================================
        // CUSTOMER MAPPINGS
        // ========================================

        // Customer -> CustomerDetailDto
        CreateMap<Customer, CustomerDetailDto>()
            .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.Orders.Count))
            .ForMember(dest => dest.TotalSpent, opt => opt.MapFrom(src => src.Orders.Sum(o => o.TotalAmount)))
            .ForMember(dest => dest.TotalInteractions, opt => opt.MapFrom(src => src.Interactions.Count))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : null))
            .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses));

        // Customer -> CustomerListDto
        CreateMap<Customer, CustomerListDto>()
            .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.Orders.Count))
            .ForMember(dest => dest.TotalSpent, opt => opt.MapFrom(src => src.Orders.Sum(o => o.TotalAmount)));

        // CreateCustomerRequest -> Customer
        CreateMap<CreateCustomerRequest, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.LoyaltyPoints, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            //.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            //.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        // UpdateCustomerRequest -> Customer
        CreateMap<UpdateCustomerRequest, Customer>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // ========================================
        // CUSTOMER ADDRESS MAPPINGS
        // ========================================

        // CustomerAddress -> CustomerAddressDto
        CreateMap<CustomerAddress, CustomerAddressDto>()
            .ForMember(dest => dest.AddressTypeText, opt => opt.MapFrom(src => src.AddressType.ToString()));

        // CreateCustomerAddressRequest -> CustomerAddress
        CreateMap<CreateCustomerAddressRequest, CustomerAddress>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        // UpdateCustomerAddressRequest -> CustomerAddress
        CreateMap<UpdateCustomerAddressRequest, CustomerAddress>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // ========================================
        // CRM INTERACTION MAPPINGS
        // ========================================

        // CRMInteraction -> CRMInteractionDto
        CreateMap<CRMInteraction, CRMInteractionDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : null))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Email : null))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Phone : null))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Name : null))
            .ForMember(dest => dest.UpdatedByName, opt => opt.Ignore()); // Will need to be loaded separately if needed

        // CRMInteraction -> CRMInteractionListDto
        CreateMap<CRMInteraction, CRMInteractionListDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Name : null));

        // CreateInteractionRequest -> CRMInteraction
        CreateMap<CreateInteractionRequest, CRMInteraction>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        // UpdateInteractionRequest -> CRMInteraction
        CreateMap<UpdateInteractionRequest, CRMInteraction>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
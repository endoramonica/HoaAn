using VietCommerce.Core.Entities.Organization;
namespace VietCommerce.Core.Common
{
    public interface ITenantEntity
    {
        Guid TenantId { get; set; }
        Tenant Tenant { get; set; }
    }
}

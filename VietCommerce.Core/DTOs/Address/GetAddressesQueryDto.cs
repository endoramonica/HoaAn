using VietCommerce.Core.Enums.Common;

namespace VietCommerce.Core.DTOs.Address
{
    // ==========================================
    // ADMIN: GET ALL ADDRESSES QUERY DTO
    // ==========================================
    public class GetAddressesQueryDto
    {
        public Guid? CustomerId { get; set; }
        public AddressType? AddressType { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDefault { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // Sorting
        public string? SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "DESC"; // ASC or DESC
    }
}

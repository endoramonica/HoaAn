using VietCommerce.Core.Enums.Common;

namespace VietCommerce.Core.DTOs.Address
{
    // ==========================================
    // ADDRESS RESPONSE DTO
    // ==========================================
    public class AddressResponseDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string StreetAddress { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public AddressType AddressType { get; set; }
        public string AddressTypeDisplay { get; set; } = string.Empty; // "Home", "Office", "Other"
        public string? RecipientName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Full formatted address (computed)
        public string FullAddress =>
            $"{StreetAddress}, {City}, {State} {PostalCode}, {Country}".Trim();
    }
}

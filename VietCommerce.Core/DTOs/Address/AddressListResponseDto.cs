namespace VietCommerce.Core.DTOs.Address
{
    // ==========================================
    // ADDRESS LIST RESPONSE DTO (for GetAll)
    // ==========================================
    public class AddressListResponseDto
    {
        public Guid Id { get; set; }
        public string StreetAddress { get; set; } = string.Empty;
        public string? City { get; set; }
        public string AddressTypeDisplay { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public string FullAddress { get; set; } = string.Empty;
    }
}

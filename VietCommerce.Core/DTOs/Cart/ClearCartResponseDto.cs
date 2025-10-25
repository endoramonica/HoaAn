// VietCommerce.Core/DTOs/Cart/ClearCartResponseDto.cs
namespace VietCommerce.Core.DTOs.Cart
{
    public class ClearCartResponseDto
    {
        public Guid CartId { get; set; }
        public bool IsCleared { get; set; }
        public DateTime ClearedAt { get; set; }
    }
}
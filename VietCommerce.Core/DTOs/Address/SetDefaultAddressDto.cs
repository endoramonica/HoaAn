using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Address
{
    // ==========================================
    // SET DEFAULT ADDRESS DTO
    // ==========================================
    public class SetDefaultAddressDto
    {
        [Required(ErrorMessage = "Address ID is required")]
        public Guid AddressId { get; set; }
    }
}

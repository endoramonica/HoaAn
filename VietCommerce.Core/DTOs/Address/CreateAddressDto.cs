using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Enums.Common;

namespace VietCommerce.Core.DTOs.Address
{
    // DTO/CreateAddressDto.cs
    public class CreateAddressDto
    {
        [Required, MaxLength(500)] public string StreetAddress { get; set; } = string.Empty;
        [MaxLength(200)] public string? City { get; set; }
        [MaxLength(50)] public string? PostalCode { get; set; }
        [MaxLength(200)] public string? State { get; set; }
        [MaxLength(100)] public string? Country { get; set; } = "Vietnam";
        [Required] public AddressType AddressType { get; set; }
        [MaxLength(200)] public string? RecipientName { get; set; }
        [MaxLength(20)] public string? PhoneNumber { get; set; }
        [MaxLength(200)] public string? Email { get; set; }
        public bool IsDefault { get; set; } = false;
        public bool IsPrimary { get; set; } = false;
    }
}

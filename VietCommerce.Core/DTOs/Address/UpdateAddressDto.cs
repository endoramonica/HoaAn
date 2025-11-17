using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Enums.Common;

namespace VietCommerce.Core.DTOs.Address
{
    public class UpdateAddressDto
    {
        [MaxLength(500, ErrorMessage = "Street address cannot exceed 500 characters")]
        public string? StreetAddress { get; set; }

        [MaxLength(200, ErrorMessage = "City cannot exceed 200 characters")]
        public string? City { get; set; }

        [MaxLength(50, ErrorMessage = "Postal code cannot exceed 50 characters")]
        [RegularExpression(@"^\d{5,10}$", ErrorMessage = "Invalid postal code format")]
        public string? PostalCode { get; set; }
        [MaxLength(200, ErrorMessage = "State cannot exceed 200 characters")]
        public string? State { get; set; }

        [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string? Country { get; set; }
        public AddressType? AddressType { get; set; }
        [MaxLength(200, ErrorMessage = "Recipient name cannot exceed 200 characters")]
        public string? RecipientName { get; set; }
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }
        [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsPrimary { get; set; }
        public bool? IsActive { get; set; }
    }
}

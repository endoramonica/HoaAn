using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VietCommerce.Core.DTOs.Customers
{
    public class CustomerDto
    {
        public Guid CustomerId { get; set; }
        public string? CustomerAvatar { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public int LoyaltyPoints { get; set; }
        public string? Tier { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }

}

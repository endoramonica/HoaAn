using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VietCommerce.Core.DTOs.Products
{
    public class DisplayPriceResult
    {
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? PromotionName { get; set; }
    }
}


using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.Models
{
    public class PriceRange
    {
        [Range(0, double.MaxValue)]
        public decimal? MinPrice { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? MaxPrice { get; set; }
        public bool IsValid => !MinPrice.HasValue || !MaxPrice.HasValue || MinPrice <= MaxPrice;
        public bool InRange(decimal price)
        {
            return (!MinPrice.HasValue || price >= MinPrice.Value) &&
                   (!MaxPrice.HasValue || price <= MaxPrice.Value);
        }
    }
}

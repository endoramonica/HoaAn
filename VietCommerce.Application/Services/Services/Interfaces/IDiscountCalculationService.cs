using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Service interface for discount calculation and validation
    /// Handles percentage and fixed amount discount calculations
    /// Validates minimum order values and usage limits
    /// Requirements: 5.1, 5.2, 5.4, 5.5
    /// </summary>
    public interface IDiscountCalculationService
    {
        /// <summary>
        /// Calculate discount amount based on promotion type and value
        /// Supports percentage and fixed amount discounts
        /// Requirements: 5.1, 5.2
        /// </summary>
        /// <param name="promotion">The promotion to calculate discount for</param>
        /// <param name="originalPrice">The original price before discount</param>
        /// <returns>Calculated discount amount</returns>
        Task<decimal> CalculateDiscountAsync(PromotionDto promotion, decimal originalPrice);

        /// <summary>
        /// Validate if promotion can be applied to cart
        /// Checks minimum order value and usage limits
        /// Requirements: 5.4, 5.5
        /// </summary>
        /// <param name="promotion">The promotion to validate</param>
        /// <param name="cartTotal">The current cart total</param>
        /// <returns>Validation result with success flag and error message if any</returns>
        Task<ApiResponse<DiscountValidationResultDto>> ValidatePromotionAsync(PromotionDto promotion, decimal cartTotal);

        /// <summary>
        /// Calculate final discount amount with all validations
        /// Applies maximum discount cap if configured
        /// Requirements: 5.1, 5.2, 5.4, 5.5
        /// </summary>
        /// <param name="promotion">The promotion to apply</param>
        /// <param name="cartTotal">The current cart total</param>
        /// <returns>Final discount amount to apply</returns>
        Task<decimal> CalculateFinalDiscountAsync(PromotionDto promotion, decimal cartTotal);

        /// <summary>
        /// Get the best (highest discount) promotion from multiple applicable promotions
        /// Implements no-stacking rule by selecting only the promotion with highest discount
        /// Requirement: 5.3
        /// </summary>
        /// <param name="promotions">List of promotions to evaluate</param>
        /// <param name="cartTotal">The current cart total</param>
        /// <returns>The promotion with the highest discount, or null if none are valid</returns>
        Task<PromotionDto> GetBestPromotionAsync(List<PromotionDto> promotions, decimal cartTotal);
    }
}

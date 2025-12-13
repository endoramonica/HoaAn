using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Service implementation for discount calculation and validation
    /// Handles percentage and fixed amount discount calculations
    /// Validates minimum order values and usage limits
    /// Requirements: 5.1, 5.2, 5.4, 5.5
    /// </summary>
    public class DiscountCalculationService : BaseService, IDiscountCalculationService
    {
        public DiscountCalculationService(
            ILogger<DiscountCalculationService> logger,
            ICacheService cacheService)
            : base(logger, cacheService)
        {
        }

        /// <summary>
        /// Calculate discount amount based on promotion type and value
        /// Supports percentage and fixed amount discounts
        /// Requirement 5.1: percentage discount = price × (value / 100)
        /// Requirement 5.2: fixed discount = value
        /// </summary>
        public async Task<decimal> CalculateDiscountAsync(PromotionDto promotion, decimal originalPrice)
        {
            return await Task.Run(() =>
            {
                LogInfo($"💰 Calculating discount for promotion: {promotion.Id} - Type: {promotion.PromotionType}");

                // Validate inputs
                if (promotion == null)
                {
                    LogWarning("⚠️ Promotion is null");
                    throw new ArgumentNullException(nameof(promotion));
                }

                if (originalPrice < 0)
                {
                    LogWarning($"⚠️ Invalid original price: {originalPrice} < 0");
                    throw new ArgumentException("Original price cannot be negative", nameof(originalPrice));
                }

                decimal discount = 0;

                // Requirement 5.1: Percentage discount calculation
                if (promotion.PromotionType == PromotionType.PERCENTAGE)
                {
                    LogDebug($"📊 Calculating percentage discount: {promotion.DiscountValue}%");
                    discount = originalPrice * (promotion.DiscountValue / 100);
                    LogDebug($"✅ Percentage discount calculated: {discount}");
                }
                // Requirement 5.2: Fixed amount discount calculation
                else if (promotion.PromotionType == PromotionType.FIXED_AMOUNT)
                {
                    LogDebug($"💵 Calculating fixed amount discount: {promotion.DiscountValue}");
                    discount = promotion.DiscountValue;
                    LogDebug($"✅ Fixed amount discount calculated: {discount}");
                }
                else
                {
                    LogWarning($"⚠️ Unsupported promotion type: {promotion.PromotionType}");
                    throw new NotSupportedException($"Promotion type {promotion.PromotionType} is not supported for discount calculation");
                }

                // Ensure discount doesn't exceed original price
                if (discount > originalPrice)
                {
                    LogDebug($"⚠️ Discount ({discount}) exceeds original price ({originalPrice}), capping to original price");
                    discount = originalPrice;
                }

                // Ensure discount is not negative
                if (discount < 0)
                {
                    LogDebug($"⚠️ Discount is negative ({discount}), setting to 0");
                    discount = 0;
                }

                LogInfo($"✅ Discount calculated: {discount} (Original: {originalPrice})");
                return discount;
            });
        }

        /// <summary>
        /// Validate if promotion can be applied to cart
        /// Checks minimum order value and usage limits
        /// Requirement 5.4: minimum order value validation
        /// Requirement 5.5: usage limit validation
        /// </summary>
        public async Task<ApiResponse<DiscountValidationResultDto>> ValidatePromotionAsync(PromotionDto promotion, decimal cartTotal)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"🔍 Validating promotion: {promotion.Id} for cart total: {cartTotal}");

                // Validate inputs
                if (promotion == null)
                {
                    LogWarning("⚠️ Promotion is null");
                    throw new ArgumentNullException(nameof(promotion));
                }

                if (cartTotal < 0)
                {
                    LogWarning($"⚠️ Invalid cart total: {cartTotal} < 0");
                    throw new ArgumentException("Cart total cannot be negative", nameof(cartTotal));
                }

                var result = new DiscountValidationResultDto
                {
                    IsValid = true,
                    ErrorMessage = null,
                    ErrorCode = null,
                    Reason = null
                };

                // Requirement 5.4: Validate minimum order value
                if (promotion.MinOrderAmount.HasValue && promotion.MinOrderAmount.Value > 0)
                {
                    LogDebug($"💳 Checking minimum order value: {promotion.MinOrderAmount}");

                    if (cartTotal < promotion.MinOrderAmount.Value)
                    {
                        LogWarning($"⚠️ Cart total ({cartTotal}) is below minimum order amount ({promotion.MinOrderAmount})");
                        result.IsValid = false;
                        result.ErrorCode = "MIN_ORDER_VALUE_NOT_MET";
                        result.ErrorMessage = $"Minimum order value of {promotion.MinOrderAmount} is required";
                        result.Reason = $"Cart total {cartTotal} is below minimum {promotion.MinOrderAmount}";
                        return result;
                    }

                    LogDebug($"✅ Minimum order value check passed");
                }

                // Requirement 5.5: Validate usage limit
                if (promotion.UsageLimit.HasValue && promotion.UsageLimit.Value > 0)
                {
                    LogDebug($"📊 Checking usage limit: {promotion.UsageLimit} (Used: {promotion.UsedCount})");

                    if (promotion.UsedCount >= promotion.UsageLimit.Value)
                    {
                        LogWarning($"⚠️ Promotion usage limit exceeded: {promotion.UsedCount} >= {promotion.UsageLimit}");
                        result.IsValid = false;
                        result.ErrorCode = "USAGE_LIMIT_EXCEEDED";
                        result.ErrorMessage = "This promotion has reached its usage limit";
                        result.Reason = $"Usage limit {promotion.UsageLimit} has been reached";
                        return result;
                    }

                    LogDebug($"✅ Usage limit check passed");
                }

                // Validate promotion is active
                if (promotion.Status != PromotionStatus.ACTIVE)
                {
                    LogWarning($"⚠️ Promotion is not active: {promotion.Status}");
                    result.IsValid = false;
                    result.ErrorCode = "PROMOTION_NOT_ACTIVE";
                    result.ErrorMessage = "This promotion is not currently active";
                    result.Reason = $"Promotion status is {promotion.Status}";
                    return result;
                }

                // Validate promotion dates
                var now = DateTime.UtcNow;
                if (now < promotion.StartDate || now > promotion.EndDate)
                {
                    LogWarning($"⚠️ Promotion is outside valid date range: {promotion.StartDate} - {promotion.EndDate}");
                    result.IsValid = false;
                    result.ErrorCode = "PROMOTION_EXPIRED";
                    result.ErrorMessage = "This promotion is not valid at this time";
                    result.Reason = $"Current time {now} is outside promotion period";
                    return result;
                }

                LogInfo($"✅ Promotion validation passed");
                return result;

            }, "ValidatePromotionAsync", "Promotion validated successfully");
        }

        /// <summary>
        /// Calculate final discount amount with all validations
        /// Applies maximum discount cap if configured
        /// Requirements: 5.1, 5.2, 5.4, 5.5
        /// </summary>
        public async Task<decimal> CalculateFinalDiscountAsync(PromotionDto promotion, decimal cartTotal)
        {
            LogInfo($"🎯 Calculating final discount for promotion: {promotion.Id}");

            // Validate promotion first
            var validationResult = await ValidatePromotionAsync(promotion, cartTotal);

            if (!validationResult.Data.IsValid)
            {
                LogWarning($"⚠️ Promotion validation failed: {validationResult.Data.ErrorMessage}");
                throw new InvalidOperationException(validationResult.Data.ErrorMessage);
            }

            // Calculate base discount
            var baseDiscount = await CalculateDiscountAsync(promotion, cartTotal);

            // Apply maximum discount cap if configured
            decimal finalDiscount = baseDiscount;
            if (promotion.MaxDiscount.HasValue && promotion.MaxDiscount.Value > 0)
            {
                LogDebug($"💰 Applying maximum discount cap: {promotion.MaxDiscount}");

                if (finalDiscount > promotion.MaxDiscount.Value)
                {
                    LogDebug($"⚠️ Discount ({finalDiscount}) exceeds maximum ({promotion.MaxDiscount}), capping");
                    finalDiscount = promotion.MaxDiscount.Value;
                }
            }

            // Ensure final discount doesn't exceed cart total
            if (finalDiscount > cartTotal)
            {
                LogDebug($"⚠️ Final discount ({finalDiscount}) exceeds cart total ({cartTotal}), capping");
                finalDiscount = cartTotal;
            }

            LogInfo($"✅ Final discount calculated: {finalDiscount}");
            return finalDiscount;
        }

        /// <summary>
        /// Get the best (highest discount) promotion from multiple applicable promotions
        /// Implements no-stacking rule by selecting only the promotion with highest discount
        /// Requirement: 5.3
        /// </summary>
        public async Task<PromotionDto> GetBestPromotionAsync(List<PromotionDto> promotions, decimal cartTotal)
        {
            return await Task.Run(async () =>
            {
                LogInfo($"🏆 Finding best promotion from {promotions?.Count ?? 0} promotions for cart total: {cartTotal}");

                // Validate inputs
                if (promotions == null || promotions.Count == 0)
                {
                    LogDebug("⚠️ No promotions provided");
                    return null;
                }

                if (cartTotal < 0)
                {
                    LogWarning($"⚠️ Invalid cart total: {cartTotal} < 0");
                    throw new ArgumentException("Cart total cannot be negative", nameof(cartTotal));
                }

                PromotionDto bestPromotion = null;
                decimal highestDiscount = 0m;

                // Evaluate each promotion
                foreach (var promotion in promotions)
                {
                    if (promotion == null)
                    {
                        LogDebug("⚠️ Skipping null promotion");
                        continue;
                    }

                    LogDebug($"📊 Evaluating promotion: {promotion.Id} - {promotion.PromotionName}");

                    // Validate promotion
                    var validationResult = await ValidatePromotionAsync(promotion, cartTotal);

                    if (!validationResult.Data.IsValid)
                    {
                        LogDebug($"⚠️ Promotion {promotion.Id} failed validation: {validationResult.Data.ErrorMessage}");
                        continue;
                    }

                    // Calculate discount for this promotion
                    var discount = await CalculateDiscountAsync(promotion, cartTotal);

                    // Apply maximum discount cap if configured
                    if (promotion.MaxDiscount.HasValue && promotion.MaxDiscount.Value > 0)
                    {
                        if (discount > promotion.MaxDiscount.Value)
                        {
                            discount = promotion.MaxDiscount.Value;
                        }
                    }

                    // Ensure discount doesn't exceed cart total
                    if (discount > cartTotal)
                    {
                        discount = cartTotal;
                    }

                    LogDebug($"💰 Promotion {promotion.Id} discount: {discount}");

                    // Check if this is the best discount so far
                    if (discount > highestDiscount)
                    {
                        LogDebug($"✅ New best promotion found: {promotion.Id} with discount {discount}");
                        highestDiscount = discount;
                        bestPromotion = promotion;
                    }
                }

                if (bestPromotion != null)
                {
                    LogInfo($"🏆 Best promotion selected: {bestPromotion.Id} - {bestPromotion.PromotionName} (Discount: {highestDiscount})");
                }
                else
                {
                    LogInfo($"⚠️ No valid promotions found for cart total: {cartTotal}");
                }

                return bestPromotion;
            });
        }
    }
}

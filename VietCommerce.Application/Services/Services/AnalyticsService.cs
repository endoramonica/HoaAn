using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Service implementation for Campaign Analytics
    /// Handles tracking and reporting of campaign performance metrics
    /// Requirements: 7.1, 7.2, 7.3, 7.4, 7.5
    /// </summary>
    public class AnalyticsService : BaseService, IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Cache TTL constants
        private static readonly TimeSpan StatsCacheDuration = TimeSpan.FromMinutes(15);

        public AnalyticsService(
            IUnitOfWork unitOfWork,
            ILogger<AnalyticsService> logger,
            ICacheService cacheService)
            : base(logger, cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Track a campaign impression (when campaign is shown to user)
        /// Requirements: 7.1
        /// </summary>
        public async Task<ApiResponse<bool>> TrackImpressionAsync(Guid campaignId, TrackImpressionDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"📊 Tracking impression for campaign: {campaignId}");

                // Validate campaign ID
                ValidateId(campaignId, nameof(campaignId));

                // Validate input
                ValidateNotEmpty(dto.SessionId, nameof(dto.SessionId));
                ValidateNotEmpty(dto.Page, nameof(dto.Page));

                // Verify campaign exists
                var campaign = await _unitOfWork.Campaigns.GetByIdAsync(campaignId);
                if (campaign == null)
                {
                    LogWarning($"⚠️ Campaign not found: {campaignId}");
                    throw new KeyNotFoundException($"Campaign with ID '{campaignId}' not found");
                }

                // Create impression record
                var impression = new CampaignImpression
                {
                    Id = Guid.NewGuid(),
                    CampaignId = campaignId,
                    SessionId = dto.SessionId,
                    Page = dto.Page,
                    RecordedAt = dto.RecordedAt ?? DateTime.UtcNow
                };

                // Add to repository
                await _unitOfWork.CampaignImpressions.AddAsync(impression);
                await _unitOfWork.SaveChangesAsync();

                LogInfo($"✅ Impression tracked: {impression.Id} for campaign {campaignId}");

                // Invalidate stats cache
                await InvalidateCacheByPrefixAsync($"campaign_stats:{campaignId}");

                return true;

            }, "TrackImpressionAsync", "Impression tracked successfully");
        }

        /// <summary>
        /// Track a campaign click (when user clicks on campaign)
        /// Requirements: 7.2
        /// </summary>
        public async Task<ApiResponse<bool>> TrackClickAsync(Guid campaignId, TrackClickDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"📊 Tracking click for campaign: {campaignId}");

                // Validate campaign ID
                ValidateId(campaignId, nameof(campaignId));

                // Validate input
                ValidateNotEmpty(dto.SessionId, nameof(dto.SessionId));
                ValidateNotEmpty(dto.Page, nameof(dto.Page));

                // Verify campaign exists
                var campaign = await _unitOfWork.Campaigns.GetByIdAsync(campaignId);
                if (campaign == null)
                {
                    LogWarning($"⚠️ Campaign not found: {campaignId}");
                    throw new KeyNotFoundException($"Campaign with ID '{campaignId}' not found");
                }

                // Create click record
                var click = new CampaignClick
                {
                    Id = Guid.NewGuid(),
                    CampaignId = campaignId,
                    SessionId = dto.SessionId,
                    Page = dto.Page,
                    RecordedAt = dto.RecordedAt ?? DateTime.UtcNow
                };

                // Add to repository
                await _unitOfWork.CampaignClicks.AddAsync(click);
                await _unitOfWork.SaveChangesAsync();

                LogInfo($"✅ Click tracked: {click.Id} for campaign {campaignId}");

                // Invalidate stats cache
                await InvalidateCacheByPrefixAsync($"campaign_stats:{campaignId}");

                return true;

            }, "TrackClickAsync", "Click tracked successfully");
        }

        /// <summary>
        /// Track a voucher redemption (when voucher is applied to cart)
        /// Requirements: 7.3
        /// </summary>
        public async Task<ApiResponse<bool>> TrackRedemptionAsync(Guid voucherId, TrackRedemptionDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"📊 Tracking redemption for voucher: {voucherId}");

                // Validate voucher ID
                ValidateId(voucherId, nameof(voucherId));

                // Validate input
                ThrowIf(dto.DiscountAmount < 0, "Discount amount cannot be negative");

                // Verify voucher exists
                var voucher = await _unitOfWork.Vouchers.GetByIdAsync(voucherId);
                if (voucher == null)
                {
                    LogWarning($"⚠️ Voucher not found: {voucherId}");
                    throw new KeyNotFoundException($"Voucher with ID '{voucherId}' not found");
                }

                // Verify promotion exists (to get campaign ID)
                var promotion = await _unitOfWork.Promotions.GetByIdAsync(voucher.PromotionId);
                if (promotion == null)
                {
                    LogWarning($"⚠️ Promotion not found for voucher: {voucherId}");
                    throw new KeyNotFoundException($"Promotion not found for voucher '{voucherId}'");
                }

                // Create redemption record
                var redemption = new VoucherRedemption
                {
                    Id = Guid.NewGuid(),
                    VoucherId = voucherId,
                    OrderId = dto.OrderId,
                    DiscountAmount = dto.DiscountAmount,
                    RedeemedAt = dto.RedeemedAt ?? DateTime.UtcNow,
                    RedeemedBy = dto.RedeemedBy
                };

                // Add to repository
                await _unitOfWork.VoucherRedemptions.AddAsync(redemption);
                await _unitOfWork.SaveChangesAsync();

                LogInfo($"✅ Redemption tracked: {redemption.Id} for voucher {voucherId}. Discount: {dto.DiscountAmount}");

                // Invalidate stats cache for the campaign
                await InvalidateCacheByPrefixAsync($"campaign_stats:{promotion.CampaignId}");

                return true;

            }, "TrackRedemptionAsync", "Redemption tracked successfully");
        }

        /// <summary>
        /// Get campaign statistics including impressions, clicks, and redemptions
        /// Requirements: 7.4, 7.5
        /// </summary>
        public async Task<ApiResponse<CampaignStatsDto>> GetCampaignStatsAsync(Guid campaignId, GetCampaignStatsQueryDto query)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"📊 Getting campaign stats: {campaignId}");

                // Validate campaign ID
                ValidateId(campaignId, nameof(campaignId));

                // Verify campaign exists
                var campaign = await _unitOfWork.Campaigns.GetByIdAsync(campaignId);
                if (campaign == null)
                {
                    LogWarning($"⚠️ Campaign not found: {campaignId}");
                    throw new KeyNotFoundException($"Campaign with ID '{campaignId}' not found");
                }

                // Set default date range if not provided
                var fromDate = query.FromDate ?? campaign.StartDate;
                var toDate = query.ToDate ?? DateTime.UtcNow;

                // Validate date range
                if (toDate < fromDate)
                {
                    LogWarning($"⚠️ Invalid date range: ToDate < FromDate");
                    throw new ArgumentException("ToDate must be greater than or equal to FromDate", nameof(query.ToDate));
                }

                // Create cache key
                var cacheKey = CreateCacheKey("campaign_stats", campaignId, fromDate.Ticks, toDate.Ticks);

                // Try to get from cache
                var cachedStats = await GetFromCacheOrExecuteAsync(
                    cacheKey,
                    async () => await CalculateCampaignStatsAsync(campaignId, fromDate, toDate),
                    StatsCacheDuration);

                LogInfo($"✅ Campaign stats retrieved: {campaignId}");
                return cachedStats;

            }, "GetCampaignStatsAsync", "Campaign statistics retrieved successfully");
        }

        /// <summary>
        /// Calculate campaign statistics
        /// </summary>
        private async Task<CampaignStatsDto> CalculateCampaignStatsAsync(Guid campaignId, DateTime fromDate, DateTime toDate)
        {
            LogDebug($"🔢 Calculating stats for campaign {campaignId} from {fromDate} to {toDate}");

            // Get impression count
            var impressionCount = await _unitOfWork.CampaignImpressions.GetImpressionCountByDateRangeAsync(
                campaignId, fromDate, toDate);

            // Get click count
            var clickCount = await _unitOfWork.CampaignClicks.GetClickCountByDateRangeAsync(
                campaignId, fromDate, toDate);

            // Get redemption count and total discount
            var redemptions = await _unitOfWork.VoucherRedemptions.GetRedemptionsByDateRangeAsync(
                campaignId, fromDate, toDate);

            var redemptionCount = redemptions.Count();
            var totalDiscount = redemptions.Sum(r => r.DiscountAmount);

            // Calculate rates
            var clickThroughRate = impressionCount > 0 ? (double)clickCount / impressionCount * 100 : 0;
            var redemptionRate = clickCount > 0 ? (double)redemptionCount / clickCount * 100 : 0;
            var conversionRate = impressionCount > 0 ? (double)redemptionCount / impressionCount * 100 : 0;

            var stats = new CampaignStatsDto
            {
                CampaignId = campaignId,
                ImpressionCount = impressionCount,
                ClickCount = clickCount,
                ClickThroughRate = clickThroughRate,
                RedemptionCount = redemptionCount,
                RedemptionRate = redemptionRate,
                TotalRevenue = 0, // TODO: Calculate from orders
                TotalDiscount = totalDiscount,
                ConversionRate = conversionRate,
                FromDate = fromDate,
                ToDate = toDate
            };

            LogDebug($"✅ Stats calculated: Impressions={impressionCount}, Clicks={clickCount}, Redemptions={redemptionCount}");
            return stats;
        }
    }
}

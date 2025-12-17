/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * DTO for campaign statistics response
 * Requirements: 7.4, 7.5
 */
export type CampaignStatsDto = {
    /**
     * Campaign ID
     */
    campaignId?: string;
    /**
     * Total number of impressions
     */
    impressionCount?: number;
    /**
     * Total number of clicks
     */
    clickCount?: number;
    /**
     * Click-through rate (clicks / impressions * 100)
     */
    clickThroughRate?: number;
    /**
     * Total number of redemptions
     */
    redemptionCount?: number;
    /**
     * Redemption rate (redemptions / clicks * 100)
     */
    redemptionRate?: number;
    /**
     * Total revenue from redemptions
     */
    totalRevenue?: number;
    /**
     * Total discount amount
     */
    totalDiscount?: number;
    /**
     * Conversion rate (redemptions / impressions * 100)
     */
    conversionRate?: number;
    /**
     * Date range start for the statistics
     */
    fromDate?: string | null;
    /**
     * Date range end for the statistics
     */
    toDate?: string | null;
};


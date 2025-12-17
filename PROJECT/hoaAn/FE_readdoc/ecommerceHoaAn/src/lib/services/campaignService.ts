/**
 * Campaign & Promotion API Service
 * Handles all campaign and voucher operations
 */

import { apiClient } from '@/lib/api/orval-client';
import type {
    Campaign,
    CampaignStatus,
    Promotion,
    CampaignStats,
    VoucherApplicationResult,
    TrackingEvent,
    PaginatedResult,
    ApiResponse,
} from '@/lib/api/types';

// ============================================================================
// Campaign Operations
// ============================================================================

/**
 * Fetch campaigns with filtering and pagination
 */
export async function getCampaigns(params?: {
    status?: CampaignStatus;
    pageNumber?: number;
    pageSize?: number;
    fromDate?: string;
    toDate?: string;
}): Promise<PaginatedResult<Campaign>> {
    try {
        const response = await apiClient.get<ApiResponse<PaginatedResult<Campaign>>>(
            '/api/v1/campaigns',
            { params }
        );
        return response.data.data;
    } catch (error) {
        console.error('Failed to fetch campaigns:', error);
        throw error;
    }
}

/**
 * Get active campaigns only
 */
export async function getActiveCampaigns(pageSize = 10): Promise<Campaign[]> {
    const result = await getCampaigns({
        status: 'ACTIVE',
        pageSize,
    });
    return result.items;
}

/**
 * Get campaign by ID
 */
export async function getCampaignById(campaignId: string): Promise<Campaign> {
    try {
        const response = await apiClient.get<ApiResponse<Campaign>>(
            `/api/v1/campaigns/${campaignId}`
        );
        return response.data.data;
    } catch (error) {
        console.error(`Failed to fetch campaign ${campaignId}:`, error);
        throw error;
    }
}

// ============================================================================
// Campaign Analytics
// ============================================================================

/**
 * Track campaign impression (when campaign is shown)
 */
export async function trackCampaignImpression(
    campaignId: string,
    event: TrackingEvent
): Promise<void> {
    try {
        await apiClient.post(
            `/api/v1/campaigns/${campaignId}/track-impression`,
            event
        );
    } catch (error) {
        console.error(`Failed to track impression for campaign ${campaignId}:`, error);
        // Don't throw - tracking failures shouldn't break the app
    }
}

/**
 * Track campaign click
 */
export async function trackCampaignClick(
    campaignId: string,
    event: TrackingEvent
): Promise<void> {
    try {
        await apiClient.post(
            `/api/v1/campaigns/${campaignId}/track-click`,
            event
        );
    } catch (error) {
        console.error(`Failed to track click for campaign ${campaignId}:`, error);
        // Don't throw - tracking failures shouldn't break the app
    }
}

/**
 * Get campaign statistics
 */
export async function getCampaignStats(
    campaignId: string,
    params?: {
        fromDate?: string;
        toDate?: string;
    }
): Promise<CampaignStats> {
    try {
        const response = await apiClient.get<ApiResponse<CampaignStats>>(
            `/api/v1/campaigns/${campaignId}/stats`,
            { params }
        );
        return response.data.data;
    } catch (error) {
        console.error(`Failed to fetch stats for campaign ${campaignId}:`, error);
        throw error;
    }
}

// ============================================================================
// Voucher Operations
// ============================================================================

/**
 * Apply voucher code to cart
 */
export async function applyVoucher(
    couponCode: string
): Promise<VoucherApplicationResult> {
    try {
        const response = await apiClient.post<ApiResponse<VoucherApplicationResult>>(
            '/api/v1/cart/apply-voucher',
            { couponCode }
        );
        return response.data.data;
    } catch (error) {
        console.error('Failed to apply voucher:', error);
        throw error;
    }
}

/**
 * Remove voucher from cart
 */
export async function removeVoucher(): Promise<VoucherApplicationResult> {
    try {
        const response = await apiClient.post<ApiResponse<VoucherApplicationResult>>(
            '/api/v1/cart/remove-voucher',
            {}
        );
        return response.data.data;
    } catch (error) {
        console.error('Failed to remove voucher:', error);
        throw error;
    }
}

// ============================================================================
// Helper Functions
// ============================================================================

/**
 * Check if campaign should be shown based on targeting rules
 */
export function shouldShowCampaign(
    campaign: Campaign,
    currentPage: string,
    sessionKey: string
): boolean {
    // Check page targeting
    if (!campaign.targeting.pages.includes(currentPage)) {
        return false;
    }

    // Check frequency
    if (campaign.targeting.frequency === 'once-per-session') {
        if (sessionStorage.getItem(sessionKey)) {
            return false;
        }
    }

    // Check date range
    const now = new Date();
    if (now < new Date(campaign.startDate) || now > new Date(campaign.endDate)) {
        return false;
    }

    return true;
}

/**
 * Calculate discount amount based on promotion type
 */
export function calculateDiscount(
    promotion: Promotion,
    cartTotal: number
): number {
    if (promotion.discountType === 'percentage') {
        return cartTotal * (promotion.discountValue / 100);
    } else {
        return promotion.discountValue;
    }
}

/**
 * Format price with currency
 */
export function formatPrice(price: number, currency: string = 'VND'): string {
    return `${price.toLocaleString('vi-VN')} ${currency}`;
}

/**
 * Get discount badge text
 */
export function getDiscountBadgeText(promotion: Promotion): string {
    if (promotion.discountType === 'percentage') {
        return `-${promotion.discountValue}%`;
    } else {
        return `-${formatPrice(promotion.discountValue)}`;
    }
}

/**
 * Generate session ID for tracking
 */
export function getSessionId(): string {
    let sessionId = sessionStorage.getItem('sessionId');
    if (!sessionId) {
        sessionId = `session-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
        sessionStorage.setItem('sessionId', sessionId);
    }
    return sessionId;
}

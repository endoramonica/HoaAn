/**
 * Frontend TypeScript Types for Campaign & Promotion + Marketing Post APIs
 * 
 * Copy this file to your frontend project and use these types for type safety
 * 
 * Usage:
 * import { Campaign, MarketingPost, VoucherApplicationResult } from './frontend-types';
 */

// ============================================================================
// Campaign & Promotion API Types
// ============================================================================

export interface Campaign {
    campaignId: string;
    campaignName: string;
    budget: number;
    startDate: string; // ISO 8601
    endDate: string; // ISO 8601
    status: CampaignStatus;
    description: string;
    targeting: CampaignTargeting;
    createdAt: string;
    updatedAt: string;
}

export type CampaignStatus = 'DRAFT' | 'ACTIVE' | 'PAUSED' | 'COMPLETED' | 'ARCHIVED';

export interface CampaignTargeting {
    pages: string[]; // e.g., ["home", "products", "cart"]
    frequency: 'once-per-session' | 'once-per-page' | 'always';
    delayMs: number; // Milliseconds to wait before showing
    autoDismissMs: number; // Milliseconds before auto-closing
}

export interface Promotion {
    promotionId: string;
    campaignId: string;
    discountValue: number;
    discountType: 'percentage' | 'fixed';
    startDate: string;
    endDate: string;
    minOrderValue?: number;
    maxUsage?: number;
    currentUsage: number;
    description: string;
    createdAt: string;
}

export interface CampaignStats {
    campaignId: string;
    campaignName: string;
    impressions: number;
    clicks: number;
    clickThroughRate: number; // Percentage
    redemptions: number;
    redemptionRate: number; // Percentage
    totalRevenue: number;
    totalDiscount: number;
    conversionRate: number; // Percentage
    fromDate: string;
    toDate: string;
}

export interface VoucherApplicationResult {
    cartId: string;
    appliedCoupon: string;
    promotionId: string;
    discountValue: number;
    discountType: 'percentage' | 'fixed';
    discountAmount: number;
    subtotal: number;
    totalAmount: number;
}

export interface TrackingEvent {
    sessionId: string;
    page: string;
    timestamp: string; // ISO 8601
}

// ============================================================================
// Marketing Post API Types
// ============================================================================

export interface MarketingPost {
    id: string;
    title: string;
    content: string;
    shortDescription?: string;
    image?: string; // Primary image URL
    images?: string[]; // Multiple images

    // Product Integration - LIVE DATA
    productId?: string | null;
    productName?: string | null;
    taggedProduct?: TaggedProduct | null; // null if no product or product deleted

    // Content Metadata
    topic?: string;
    platform?: string;
    tone?: string;
    hashtags?: string[];

    // Priority & Display
    priorityScore: number; // 1-100
    displayLocation?: string[]; // e.g., ["homepage_banner", "featured_section"]
    isFeatured: boolean; // true if priorityScore > 80

    // SEO
    metaTitle?: string;
    metaDescription?: string;
    metaKeywords?: string[];

    // Social Media Variants
    socialPosts?: SocialMediaPosts;

    // Publishing
    status: MarketingPostStatus;
    scheduledDate?: string | null;
    publishedDate?: string | null;

    // Analytics
    views: number;
    clicks: number;
    shares: number;

    // Audit
    createdAt: string;
    updatedAt: string;
}

export type MarketingPostStatus = 'Draft' | 'Published' | 'Scheduled';

export interface TaggedProduct {
    id: string;
    name: string;
    price: number;
    currency: string; // e.g., "VND"
    formattedPrice: string; // e.g., "1,000,000 VND"
    thumbnailUrl: string;
    hasDiscount: boolean;
    discountPercentage: number;
}

export interface SocialMediaPosts {
    facebook?: string;
    instagram?: string;
    twitter?: string;
    linkedin?: string;
}

export interface MarketingPostListDto extends Omit<MarketingPost, 'content'> {
    // List view excludes full content
}

export interface MarketingPostDetailDto extends MarketingPost {
    createdBy?: string;
    updatedBy?: string;
}

export interface MarketingPostStatistics {
    total: number;
    draft: number;
    published: number;
    scheduled: number;
    totalViews: number;
    totalClicks: number;
    totalShares: number;
    averageViews: number;
    averageClicks: number;
    averageShares: number;
}

// ============================================================================
// Pagination Types
// ============================================================================

export interface PaginatedResult<T> {
    items: T[];
    pageNumber: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
}

export interface PaginatedCampaigns extends PaginatedResult<Campaign> { }
export interface PaginatedMarketingPosts extends PaginatedResult<MarketingPost> { }

// ============================================================================
// Query/Filter Types
// ============================================================================

export interface GetCampaignsQuery {
    status?: CampaignStatus;
    pageNumber?: number;
    pageSize?: number;
    fromDate?: string;
    toDate?: string;
}

export interface GetMarketingPostsQuery {
    pageNumber?: number;
    pageSize?: number;
    status?: MarketingPostStatus;
    productId?: string;
    platform?: string;
    searchTerm?: string;
    displayLocation?: string;
    isFeatured?: boolean;
    minPriorityScore?: number;
    fromDate?: string;
    toDate?: string;
    sortBy?: 'priority' | 'publishedDate' | 'views' | 'clicks' | 'shares' | 'updatedAt';
    sortOrder?: 'asc' | 'desc';
}

// ============================================================================
// Request/Response Types
// ============================================================================

export interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
    errors?: Record<string, string[]>;
    errorCode?: string;
}

export interface ApiErrorResponse {
    success: false;
    message: string;
    errorCode?: string;
    errors?: Record<string, string[]>;
}

export interface ApiSuccessResponse<T> {
    success: true;
    data: T;
    message?: string;
}

// ============================================================================
// Request Body Types
// ============================================================================

export interface ApplyVoucherRequest {
    couponCode: string;
}

export interface TrackImpressionRequest extends TrackingEvent { }
export interface TrackClickRequest extends TrackingEvent { }
export interface TrackViewRequest { }
export interface TrackShareRequest { }

// ============================================================================
// Error Types
// ============================================================================

export type ErrorCode =
    | 'INVALID_VOUCHER'
    | 'VOUCHER_LIMIT_EXCEEDED'
    | 'MIN_ORDER_VALUE_NOT_MET'
    | 'RESOURCE_NOT_FOUND'
    | 'INVALID_ARGUMENT'
    | 'UNAUTHORIZED_ACCESS'
    | 'INTERNAL_SERVER_ERROR';

export interface ApiError extends Error {
    statusCode: number;
    errorCode?: ErrorCode;
    errors?: Record<string, string[]>;
}

// ============================================================================
// Helper Types
// ============================================================================

export interface DiscountInfo {
    discountValue: number;
    discountType: 'percentage' | 'fixed';
    discountAmount: number;
    originalPrice: number;
    finalPrice: number;
}

export interface CartWithDiscount {
    subtotal: number;
    discount: number;
    total: number;
    appliedVoucher?: string;
}

// ============================================================================
// Utility Functions
// ============================================================================

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
 * Format price with currency
 */
export function formatPrice(price: number, currency: string = 'VND'): string {
    return `${price.toLocaleString('vi-VN')} ${currency}`;
}

/**
 * Check if post has valid product data
 */
export function hasValidProduct(post: MarketingPost): boolean {
    return post.productId !== null && post.productId !== undefined && post.taggedProduct !== null;
}

/**
 * Get product link from post
 */
export function getProductLink(post: MarketingPost): string | null {
    if (!hasValidProduct(post)) {
        return null;
    }
    return `/products/${post.taggedProduct?.id}`;
}

/**
 * Check if post is featured
 */
export function isFeaturedPost(post: MarketingPost): boolean {
    return post.isFeatured || post.priorityScore > 80;
}

/**
 * Format campaign status for display
 */
export function formatCampaignStatus(status: CampaignStatus): string {
    const statusMap: Record<CampaignStatus, string> = {
        DRAFT: 'Draft',
        ACTIVE: 'Active',
        PAUSED: 'Paused',
        COMPLETED: 'Completed',
        ARCHIVED: 'Archived'
    };
    return statusMap[status];
}

/**
 * Format post status for display
 */
export function formatPostStatus(status: MarketingPostStatus): string {
    const statusMap: Record<MarketingPostStatus, string> = {
        Draft: 'Draft',
        Published: 'Published',
        Scheduled: 'Scheduled'
    };
    return statusMap[status];
}

/**
 * Calculate engagement rate
 */
export function calculateEngagementRate(
    interactions: number,
    impressions: number
): number {
    if (impressions === 0) return 0;
    return (interactions / impressions) * 100;
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

// ============================================================================
// Type Guards
// ============================================================================

export function isApiError(error: unknown): error is ApiError {
    return (
        error instanceof Error &&
        'statusCode' in error &&
        typeof (error as any).statusCode === 'number'
    );
}

export function isApiErrorResponse(response: unknown): response is ApiErrorResponse {
    return (
        typeof response === 'object' &&
        response !== null &&
        'success' in response &&
        (response as any).success === false
    );
}

export function isApiSuccessResponse<T>(response: unknown): response is ApiSuccessResponse<T> {
    return (
        typeof response === 'object' &&
        response !== null &&
        'success' in response &&
        (response as any).success === true &&
        'data' in response
    );
}

// ============================================================================
// Constants
// ============================================================================

export const API_BASE_URLS = {
    CAMPAIGN: 'http://localhost:5000/api/v1',
    MARKETING_POST: 'http://localhost:5000/api/admin/marketing-posts'
};

export const CAMPAIGN_PAGES = [
    'home',
    'products',
    'cart',
    'profile',
    'services',
    'about'
] as const;

export const CAMPAIGN_FREQUENCIES = [
    'once-per-session',
    'once-per-page',
    'always'
] as const;

export const DISPLAY_LOCATIONS = [
    'homepage_banner',
    'product_section',
    'featured_section',
    'sidebar'
] as const;

export const PLATFORMS = [
    'facebook',
    'instagram',
    'twitter',
    'linkedin',
    'tiktok',
    'youtube'
] as const;

export const TONES = [
    'professional',
    'casual',
    'humorous',
    'inspirational',
    'urgent'
] as const;

// ============================================================================
// Export all types
// ============================================================================

export type {
    Campaign,
    CampaignStatus,
    CampaignTargeting,
    Promotion,
    CampaignStats,
    VoucherApplicationResult,
    TrackingEvent,
    MarketingPost,
    MarketingPostStatus,
    TaggedProduct,
    SocialMediaPosts,
    MarketingPostListDto,
    MarketingPostDetailDto,
    MarketingPostStatistics,
    PaginatedResult,
    PaginatedCampaigns,
    PaginatedMarketingPosts,
    GetCampaignsQuery,
    GetMarketingPostsQuery,
    ApiResponse,
    ApiErrorResponse,
    ApiSuccessResponse,
    ApplyVoucherRequest,
    TrackImpressionRequest,
    TrackClickRequest,
    TrackViewRequest,
    TrackShareRequest,
    ErrorCode,
    ApiError,
    DiscountInfo,
    CartWithDiscount
};

/**
 * Marketing Post API Service
 * Handles all marketing post operations and analytics
 */

import { apiClient } from '@/lib/api/orval-client';
import type {
    MarketingPost,
    MarketingPostStatus,
    TaggedProduct,
    PaginatedResult,
    ApiResponse,
    GetMarketingPostsQuery,
} from '@/lib/api/types';

// ============================================================================
// Post Fetching
// ============================================================================

/**
 * Fetch marketing posts with pagination and filtering
 */
export async function getMarketingPosts(
    params?: GetMarketingPostsQuery
): Promise<PaginatedResult<MarketingPost>> {
    try {
        const response = await apiClient.get<ApiResponse<PaginatedResult<MarketingPost>>>(
            '/api/admin/marketing-posts',
            { params }
        );
        return response.data.data;
    } catch (error) {
        console.error('Failed to fetch marketing posts:', error);
        throw error;
    }
}

/**
 * Get published posts only
 */
export async function getPublishedPosts(pageSize = 20): Promise<MarketingPost[]> {
    const result = await getMarketingPosts({
        status: 'Published',
        pageSize,
        sortBy: 'priority',
        sortOrder: 'desc',
    });
    return result.items;
}

/**
 * Get featured posts (priority > 80)
 */
export async function getFeaturedPosts(pageSize = 10): Promise<MarketingPost[]> {
    const result = await getMarketingPosts({
        status: 'Published',
        isFeatured: true,
        pageSize,
        sortBy: 'priority',
        sortOrder: 'desc',
    });
    return result.items;
}

/**
 * Get post by ID
 */
export async function getPostById(postId: string): Promise<MarketingPost> {
    try {
        const response = await apiClient.get<ApiResponse<MarketingPost>>(
            `/api/admin/marketing-posts/${postId}`
        );
        return response.data.data;
    } catch (error) {
        console.error(`Failed to fetch post ${postId}:`, error);
        throw error;
    }
}

/**
 * Search posts by keyword
 */
export async function searchPosts(
    searchTerm: string,
    pageSize = 20
): Promise<MarketingPost[]> {
    const result = await getMarketingPosts({
        status: 'Published',
        searchTerm,
        pageSize,
    });
    return result.items;
}

/**
 * Get posts by product ID
 */
export async function getPostsByProduct(
    productId: string,
    pageSize = 10
): Promise<MarketingPost[]> {
    const result = await getMarketingPosts({
        status: 'Published',
        productId,
        pageSize,
    });
    return result.items;
}

/**
 * Get posts by platform
 */
export async function getPostsByPlatform(
    platform: string,
    pageSize = 20
): Promise<MarketingPost[]> {
    const result = await getMarketingPosts({
        status: 'Published',
        platform,
        pageSize,
        sortBy: 'publishedDate',
        sortOrder: 'desc',
    });
    return result.items;
}

// ============================================================================
// Analytics Tracking
// ============================================================================

/**
 * Track post view
 */
export async function trackPostView(postId: string): Promise<void> {
    try {
        await apiClient.post(
            `/api/admin/marketing-posts/${postId}/analytics/views`,
            {}
        );
    } catch (error) {
        console.error(`Failed to track view for post ${postId}:`, error);
        // Don't throw - tracking failures shouldn't break the app
    }
}

/**
 * Track post click
 */
export async function trackPostClick(postId: string): Promise<void> {
    try {
        await apiClient.post(
            `/api/admin/marketing-posts/${postId}/analytics/clicks`,
            {}
        );
    } catch (error) {
        console.error(`Failed to track click for post ${postId}:`, error);
        // Don't throw - tracking failures shouldn't break the app
    }
}

/**
 * Track post share
 */
export async function trackPostShare(postId: string): Promise<void> {
    try {
        await apiClient.post(
            `/api/admin/marketing-posts/${postId}/analytics/shares`,
            {}
        );
    } catch (error) {
        console.error(`Failed to track share for post ${postId}:`, error);
        // Don't throw - tracking failures shouldn't break the app
    }
}

// ============================================================================
// Helper Functions
// ============================================================================

/**
 * Check if post has valid product data
 */
export function hasValidProduct(post: MarketingPost): boolean {
    return (
        post.productId !== null &&
        post.productId !== undefined &&
        post.taggedProduct !== null
    );
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
    return post.isFeatured || (post.priorityScore ?? 0) > 80;
}

/**
 * Format post status for display
 */
export function formatPostStatus(status: MarketingPostStatus): string {
    const statusMap: Record<MarketingPostStatus, string> = {
        Draft: 'Draft',
        Published: 'Published',
        Scheduled: 'Scheduled',
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
 * Format price with currency
 */
export function formatPrice(price: number, currency: string = 'VND'): string {
    return `${price.toLocaleString('vi-VN')} ${currency}`;
}

/**
 * Get discount badge text
 */
export function getDiscountBadgeText(product: TaggedProduct): string {
    if (product.hasDiscount) {
        return `-${product.discountPercentage}%`;
    }
    return '';
}

/**
 * Track post view with IntersectionObserver
 */
export function setupPostViewTracking(
    element: HTMLElement,
    postId: string
): IntersectionObserver {
    const observer = new IntersectionObserver(
        (entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    trackPostView(postId);
                    observer.unobserve(entry.target);
                }
            });
        },
        { threshold: 0.5 }
    );

    observer.observe(element);
    return observer;
}

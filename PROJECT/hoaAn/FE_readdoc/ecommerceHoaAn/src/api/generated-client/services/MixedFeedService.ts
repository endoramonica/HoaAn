/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { FeaturedPostsDtoApiResponse } from '../models/FeaturedPostsDtoApiResponse';
import type { LocationPostsDtoApiResponse } from '../models/LocationPostsDtoApiResponse';
import type { MixedFeedDtoPaginatedResultApiResponse } from '../models/MixedFeedDtoPaginatedResultApiResponse';
import type { PostInteractionDto } from '../models/PostInteractionDto';
import type { PostInteractionResultApiResponse } from '../models/PostInteractionResultApiResponse';
import type { RelatedPostsDtoApiResponse } from '../models/RelatedPostsDtoApiResponse';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class MixedFeedService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Lấy mixed feed với thuật toán trộn thông minh
     * Marketing posts xuất hiện mỗi N community posts (default: 1 marketing mỗi 4 community posts)
     * @param pageNumber
     * @param pageSize
     * @param marketingRatio Tỷ lệ marketing posts trong feed (1-10)
     * Ví dụ: 3 = 1 marketing post mỗi 3 community posts
     * @param location Vị trí hiển thị cụ thể (homepage_banner, product_section, featured_section)
     * @param featuredOnly Chỉ lấy featured posts (priority > 80)
     * @param productId Filter theo productId
     * @param minPriorityScore Minimum priority score cho marketing posts
     * ///
     * @returns MixedFeedDtoPaginatedResultApiResponse OK
     * @throws ApiError
     */
    public getApiV1MixedFeed(
        pageNumber?: number,
        pageSize?: number,
        marketingRatio?: number,
        location?: string,
        featuredOnly?: boolean,
        productId?: string,
        minPriorityScore?: number,
    ): CancelablePromise<MixedFeedDtoPaginatedResultApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/MixedFeed',
            query: {
                'PageNumber': pageNumber,
                'PageSize': pageSize,
                'MarketingRatio': marketingRatio,
                'Location': location,
                'FeaturedOnly': featuredOnly,
                'ProductId': productId,
                'MinPriorityScore': minPriorityScore,
            },
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Lấy posts theo vị trí hiển thị cụ thể
     * Ví dụ: homepage_banner, product_section, featured_section, sidebar
     * @param location Display location
     * @param pageNumber Page number (default: 1)
     * @param pageSize Page size (default: 20)
     * @returns LocationPostsDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1MixedFeedLocation(
        location: string,
        pageNumber: number = 1,
        pageSize: number = 20,
    ): CancelablePromise<LocationPostsDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/MixedFeed/location/{location}',
            path: {
                'location': location,
            },
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Lấy featured posts (priority score > 80)
     * Dành cho banner, highlight sections
     * @param pageNumber Page number (default: 1)
     * @param pageSize Page size (default: 10)
     * @returns FeaturedPostsDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1MixedFeedFeatured(
        pageNumber: number = 1,
        pageSize: number = 10,
    ): CancelablePromise<FeaturedPostsDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/MixedFeed/featured',
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Lấy posts liên quan đến một product
     * Bao gồm cả marketing posts và community posts mention product đó
     * @param productId Product ID
     * @param pageNumber Page number (default: 1)
     * @param pageSize Page size (default: 20)
     * @returns RelatedPostsDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1MixedFeedProduct(
        productId: string,
        pageNumber: number = 1,
        pageSize: number = 20,
    ): CancelablePromise<RelatedPostsDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/MixedFeed/product/{productId}',
            path: {
                'productId': productId,
            },
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Track interaction với post (view, click, share)
     * Public endpoint - không cần authentication
     * @param postId Post ID
     * @param postType Post type: "community" hoặc "marketing"
     * @param requestBody Interaction details
     * @returns PostInteractionResultApiResponse OK
     * @throws ApiError
     */
    public postApiV1MixedFeedTrack(
        postId: string,
        postType?: string,
        requestBody?: PostInteractionDto,
    ): CancelablePromise<PostInteractionResultApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/MixedFeed/{postId}/track',
            path: {
                'postId': postId,
            },
            query: {
                'postType': postType,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
}

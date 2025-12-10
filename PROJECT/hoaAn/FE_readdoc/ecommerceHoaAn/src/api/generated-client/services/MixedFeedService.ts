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
     * @param pageNumber
     * @param pageSize
     * @param marketingRatio
     * @param location
     * @param featuredOnly
     * @param productId
     * @param minPriorityScore
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
     * @param location
     * @param pageNumber
     * @param pageSize
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
     * @param pageNumber
     * @param pageSize
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
     * @param productId
     * @param pageNumber
     * @param pageSize
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
     * @param postId
     * @param postType
     * @param requestBody
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

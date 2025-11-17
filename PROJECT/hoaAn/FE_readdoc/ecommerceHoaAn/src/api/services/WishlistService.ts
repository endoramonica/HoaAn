/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AddToWishlistRequest } from '../models/AddToWishlistRequest';
import type { BooleanApiResponse } from '../models/BooleanApiResponse';
import type { IsInWishlistResponseApiResponse } from '../models/IsInWishlistResponseApiResponse';
import type { ToggleWishlistResponseApiResponse } from '../models/ToggleWishlistResponseApiResponse';
import type { WishlistItemDtoApiResponse } from '../models/WishlistItemDtoApiResponse';
import type { WishlistItemDtoListApiResponse } from '../models/WishlistItemDtoListApiResponse';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class WishlistService {
    /**
     * @returns WishlistItemDtoListApiResponse OK
     * @throws ApiError
     */
    public static getApiV1Wishlist(): CancelablePromise<WishlistItemDtoListApiResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Wishlist',
        });
    }
    /**
     * @param requestBody
     * @returns WishlistItemDtoApiResponse OK
     * @throws ApiError
     */
    public static postApiV1Wishlist(
        requestBody?: AddToWishlistRequest,
    ): CancelablePromise<WishlistItemDtoApiResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Wishlist',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public static deleteApiV1Wishlist(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/v1/Wishlist/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param productId
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public static deleteApiV1WishlistProduct(
        productId: string,
    ): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/v1/Wishlist/product/{productId}',
            path: {
                'productId': productId,
            },
        });
    }
    /**
     * @param productId
     * @returns IsInWishlistResponseApiResponse OK
     * @throws ApiError
     */
    public static getApiV1WishlistCheck(
        productId: string,
    ): CancelablePromise<IsInWishlistResponseApiResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Wishlist/check/{productId}',
            path: {
                'productId': productId,
            },
        });
    }
    /**
     * @param requestBody
     * @returns ToggleWishlistResponseApiResponse OK
     * @throws ApiError
     */
    public static postApiV1WishlistToggle(
        requestBody?: AddToWishlistRequest,
    ): CancelablePromise<ToggleWishlistResponseApiResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Wishlist/toggle',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public static deleteApiV1WishlistClear(): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/v1/Wishlist/clear',
        });
    }
    /**
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public static postApiV1WishlistMoveToCart(): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Wishlist/move-to-cart',
        });
    }
}

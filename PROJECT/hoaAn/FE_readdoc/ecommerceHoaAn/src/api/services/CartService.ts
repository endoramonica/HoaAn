/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AddToCartDto } from '../models/AddToCartDto';
import type { ApplyCouponDto } from '../models/ApplyCouponDto';
import type { MergeCartDto } from '../models/MergeCartDto';
import type { OrderShippingDto } from '../models/OrderShippingDto';
import type { UpdateCartItemDto } from '../models/UpdateCartItemDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class CartService {
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiV1Cart(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Cart',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiV1CartSummary(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Cart/summary',
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static postApiV1CartAdd(
        requestBody?: AddToCartDto,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Cart/add',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static putApiV1CartUpdateItem(
        requestBody?: UpdateCartItemDto,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/v1/Cart/update-item',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param cartItemId
     * @returns any OK
     * @throws ApiError
     */
    public static deleteApiV1CartItems(
        cartItemId: string,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/v1/Cart/items/{cartItemId}',
            path: {
                'cartItemId': cartItemId,
            },
        });
    }
    /**
     * @param cartItemId
     * @returns any OK
     * @throws ApiError
     */
    public static getApiV1CartItems(
        cartItemId: string,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Cart/items/{cartItemId}',
            path: {
                'cartItemId': cartItemId,
            },
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static deleteApiV1CartClear(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/v1/Cart/clear',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static postApiV1CartValidate(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Cart/validate',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiV1CartItemCount(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Cart/item-count',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiV1CartGuest(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Cart/guest',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiV1CartGuestSummary(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Cart/guest/summary',
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static postApiV1CartGuestAdd(
        requestBody?: AddToCartDto,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Cart/guest/add',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param cartItemId
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static putApiV1CartGuestItems(
        cartItemId: string,
        requestBody?: UpdateCartItemDto,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/v1/Cart/guest/items/{cartItemId}',
            path: {
                'cartItemId': cartItemId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param cartItemId
     * @returns any OK
     * @throws ApiError
     */
    public static deleteApiV1CartGuestItems(
        cartItemId: string,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/v1/Cart/guest/items/{cartItemId}',
            path: {
                'cartItemId': cartItemId,
            },
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static deleteApiV1CartGuestClear(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/v1/Cart/guest/clear',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static postApiV1CartGuestValidate(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Cart/guest/validate',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getApiV1CartGuestItemCount(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Cart/guest/item-count',
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static postApiV1CartMerge(
        requestBody?: MergeCartDto,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Cart/merge',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static postApiV1CartCouponApply(
        requestBody?: ApplyCouponDto,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Cart/coupon/apply',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public static postApiV1CartCouponRemove(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Cart/coupon/remove',
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public static putApiV1CartShipping(
        requestBody?: OrderShippingDto,
    ): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/v1/Cart/shipping',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
}

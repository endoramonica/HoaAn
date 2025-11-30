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
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class CartService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1Cart(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Cart',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1CartSummary(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Cart/summary',
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1CartAdd(
        requestBody?: AddToCartDto,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
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
    public putApiV1CartUpdateItem(
        requestBody?: UpdateCartItemDto,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
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
    public deleteApiV1CartItems(
        cartItemId: string,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
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
    public getApiV1CartItems(
        cartItemId: string,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
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
    public deleteApiV1CartClear(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/Cart/clear',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1CartValidate(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Cart/validate',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1CartItemCount(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Cart/item-count',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1CartGuest(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Cart/guest',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1CartGuestSummary(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Cart/guest/summary',
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1CartGuestAdd(
        requestBody?: AddToCartDto,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
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
    public putApiV1CartGuestItems(
        cartItemId: string,
        requestBody?: UpdateCartItemDto,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
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
    public deleteApiV1CartGuestItems(
        cartItemId: string,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
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
    public deleteApiV1CartGuestClear(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/Cart/guest/clear',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1CartGuestValidate(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Cart/guest/validate',
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1CartGuestItemCount(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Cart/guest/item-count',
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1CartMerge(
        requestBody?: MergeCartDto,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
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
    public postApiV1CartCouponApply(
        requestBody?: ApplyCouponDto,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
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
    public postApiV1CartCouponRemove(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Cart/coupon/remove',
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public putApiV1CartShipping(
        requestBody?: OrderShippingDto,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/Cart/shipping',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
}

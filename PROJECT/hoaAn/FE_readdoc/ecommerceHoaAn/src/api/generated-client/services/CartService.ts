/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AddToCartDto } from '../models/AddToCartDto';
import type { ApplyCouponDto } from '../models/ApplyCouponDto';
import type { ApplyVoucherDto } from '../models/ApplyVoucherDto';
import type { CartItemCustomizationDto } from '../models/CartItemCustomizationDto';
import type { GetCartResponseDtoApiResponse } from '../models/GetCartResponseDtoApiResponse';
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
     * Add a package product to cart with customizations.
     * Validates customization quantities against product constraints and calculates final price.
     * Requirements: 3.1, 3.3
     * - Validates customizations using ProductService.ValidateCustomizationsAsync
     * - Calculates final price: basePrice + sum(customization quantities × unit prices)
     * - Stores customizations as JSON in CartItem
     * - Returns CartItemDetailDto with BasePrice, CustomizationPrice, and FinalPrice breakdown
     * @param requestBody The add to cart request containing product ID and customizations
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1CartAddWithCustomizations(
        requestBody?: AddToCartDto,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Cart/add-with-customizations',
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
     * Update the customizations for an existing cart item.
     * Validates new customization quantities and recalculates the final price.
     * Requirements: 4.1, 4.2
     * - Validates new customizations against product constraints
     * - Recalculates final price based on new customizations
     * - Updates CartItem with new customizationsJson, customizationPrice, and finalPrice
     * - Returns CartItemDetailDto with updated price breakdown
     * @param cartItemId The unique identifier of the cart item to update
     * @param requestBody The new list of customizations to apply
     * @returns any OK
     * @throws ApiError
     */
    public putApiV1CartItemsCustomizations(
        cartItemId: string,
        requestBody?: Array<CartItemCustomizationDto>,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/Cart/items/{cartItemId}/customizations',
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
     * Get guest cart - session ID from cookie/header automatically
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
     * Get guest cart summary
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
     * Add item to guest cart
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
     * Update guest cart item
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
     * Remove item from guest cart
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
     * Clear all items from guest cart
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
     * Validate guest cart before checkout
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
     * Get guest cart item count
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
     * Manually merge guest cart to user cart (usually done automatically on login)
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
     * Apply a voucher code to user's cart
     * Requirements: 3.2, 3.3, 3.4, 3.5
     * @param requestBody Voucher code to apply
     * @returns GetCartResponseDtoApiResponse Voucher applied successfully
     * @throws ApiError
     */
    public postApiV1CartApplyVoucher(
        requestBody?: ApplyVoucherDto,
    ): CancelablePromise<GetCartResponseDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Cart/apply-voucher',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Validation error or invalid voucher`,
                401: `Unauthorized`,
                404: `Cart or voucher not found`,
                409: `Business rule violation (expired, usage limit exceeded, etc.)`,
            },
        });
    }
    /**
     * Remove applied voucher from user's cart
     * Requirements: 3.2
     * @returns GetCartResponseDtoApiResponse Voucher removed successfully
     * @throws ApiError
     */
    public postApiV1CartRemoveVoucher(): CancelablePromise<GetCartResponseDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Cart/remove-voucher',
            errors: {
                400: `Error removing voucher`,
                401: `Unauthorized`,
                404: `Cart not found`,
            },
        });
    }
    /**
     * Apply a voucher code to guest's cart
     * Requirements: 3.2, 3.3, 3.4, 3.5
     * @param requestBody Voucher code to apply
     * @returns GetCartResponseDtoApiResponse Voucher applied successfully
     * @throws ApiError
     */
    public postApiV1CartGuestApplyVoucher(
        requestBody?: ApplyVoucherDto,
    ): CancelablePromise<GetCartResponseDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Cart/guest/apply-voucher',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Validation error or invalid voucher`,
                404: `Cart or voucher not found`,
                409: `Business rule violation (expired, usage limit exceeded, etc.)`,
            },
        });
    }
    /**
     * Remove applied voucher from guest's cart
     * Requirements: 3.2
     * @returns GetCartResponseDtoApiResponse Voucher removed successfully
     * @throws ApiError
     */
    public postApiV1CartGuestRemoveVoucher(): CancelablePromise<GetCartResponseDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Cart/guest/remove-voucher',
            errors: {
                400: `Error removing voucher`,
                404: `Cart not found`,
            },
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

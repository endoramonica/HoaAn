/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AddToWishlistRequest } from '../models/AddToWishlistRequest';
import type { BooleanApiResponse } from '../models/BooleanApiResponse';
import type { Int32ApiResponse } from '../models/Int32ApiResponse';
import type { IsInWishlistResponseApiResponse } from '../models/IsInWishlistResponseApiResponse';
import type { ToggleWishlistResponseApiResponse } from '../models/ToggleWishlistResponseApiResponse';
import type { WishlistItemDtoApiResponse } from '../models/WishlistItemDtoApiResponse';
import type { WishlistItemDtoListApiResponse } from '../models/WishlistItemDtoListApiResponse';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class WishlistService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Lấy danh sách wishlist của user hiện tại
     * @returns WishlistItemDtoListApiResponse OK
     * @throws ApiError
     */
    public getApiV1Wishlist(): CancelablePromise<WishlistItemDtoListApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Wishlist',
        });
    }
    /**
     * Thêm sản phẩm vào wishlist
     * @param requestBody
     * @returns WishlistItemDtoApiResponse OK
     * @throws ApiError
     */
    public postApiV1Wishlist(
        requestBody?: AddToWishlistRequest,
    ): CancelablePromise<WishlistItemDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Wishlist',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * Xóa wishlist item theo ID
     * @param id
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public deleteApiV1Wishlist(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/Wishlist/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * Xóa sản phẩm khỏi wishlist theo ProductId
     * @param productId
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public deleteApiV1WishlistProduct(
        productId: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/Wishlist/product/{productId}',
            path: {
                'productId': productId,
            },
        });
    }
    /**
     * Kiểm tra sản phẩm có trong wishlist không
     * @param productId
     * @returns IsInWishlistResponseApiResponse OK
     * @throws ApiError
     */
    public getApiV1WishlistCheck(
        productId: string,
    ): CancelablePromise<IsInWishlistResponseApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Wishlist/check/{productId}',
            path: {
                'productId': productId,
            },
        });
    }
    /**
     * Toggle wishlist - thêm nếu chưa có, xóa nếu đã có
     * @param requestBody
     * @returns ToggleWishlistResponseApiResponse OK
     * @throws ApiError
     */
    public postApiV1WishlistToggle(
        requestBody?: AddToWishlistRequest,
    ): CancelablePromise<ToggleWishlistResponseApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Wishlist/toggle',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * Xóa toàn bộ wishlist
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public deleteApiV1WishlistClear(): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/Wishlist/clear',
        });
    }
    /**
     * Di chuyển tất cả wishlist items vào giỏ hàng
     * TODO: Implement khi cần
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public postApiV1WishlistMoveToCart(): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Wishlist/move-to-cart',
        });
    }
    /**
     * @returns Int32ApiResponse OK
     * @throws ApiError
     */
    public getApiV1WishlistCount(): CancelablePromise<Int32ApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Wishlist/count',
        });
    }
}

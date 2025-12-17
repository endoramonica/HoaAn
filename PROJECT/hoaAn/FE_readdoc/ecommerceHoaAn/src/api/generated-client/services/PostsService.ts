/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BooleanApiResponse } from '../models/BooleanApiResponse';
import type { PostBookmarkResultApiResponse } from '../models/PostBookmarkResultApiResponse';
import type { PostDetailDtoApiResponse } from '../models/PostDetailDtoApiResponse';
import type { PostFeedDtoPaginatedResultApiResponse } from '../models/PostFeedDtoPaginatedResultApiResponse';
import type { PostLikeResultApiResponse } from '../models/PostLikeResultApiResponse';
import type { PostResponseDtoApiResponse } from '../models/PostResponseDtoApiResponse';
import type { PostResponseDtoPaginatedResultApiResponse } from '../models/PostResponseDtoPaginatedResultApiResponse';
import type { UpdatePostDto } from '../models/UpdatePostDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class PostsService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Tạo post mới (yêu cầu authentication)
     * @param formData
     * @returns PostResponseDtoApiResponse Created
     * @throws ApiError
     */
    public postApiV1Posts(
        formData?: {
            Content?: string;
            PhotoFile?: Blob;
            NotificationOn?: string;
        },
    ): CancelablePromise<PostResponseDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Posts',
            formData: formData,
            mediaType: 'multipart/form-data',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Cập nhật post (chỉ owner được phép)
     * @param postId ID của post cần update
     * @param requestBody Dữ liệu cập nhật
     * @returns PostResponseDtoApiResponse OK
     * @throws ApiError
     */
    public putApiV1Posts(
        postId: string,
        requestBody?: UpdatePostDto,
    ): CancelablePromise<PostResponseDtoApiResponse> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/Posts/{postId}',
            path: {
                'postId': postId,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Xóa post (soft delete, chỉ owner được phép)
     * @param postId ID của post cần xóa
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public deleteApiV1Posts(
        postId: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/Posts/{postId}',
            path: {
                'postId': postId,
            },
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Lấy chi tiết một post (public, guest có thể xem)
     * @param postId ID của post
     * @returns PostDetailDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1Posts(
        postId: string,
    ): CancelablePromise<PostDetailDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Posts/{postId}',
            path: {
                'postId': postId,
            },
            errors: {
                404: `Not Found`,
            },
        });
    }
    /**
     * Lấy feed posts với pagination (public, guest có thể xem)
     * @param pageNumber Số trang (default: 1)
     * @param pageSize Số items mỗi trang (default: 20)
     * @returns PostFeedDtoPaginatedResultApiResponse OK
     * @throws ApiError
     */
    public getApiV1PostsFeed(
        pageNumber: number = 1,
        pageSize: number = 20,
    ): CancelablePromise<PostFeedDtoPaginatedResultApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Posts/feed',
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
     * Lấy danh sách posts của một customer (public profile)
     * @param customerId ID của customer
     * @param pageNumber Số trang (default: 1)
     * @param pageSize Số items mỗi trang (default: 20)
     * @returns PostResponseDtoPaginatedResultApiResponse OK
     * @throws ApiError
     */
    public getApiV1PostsCustomer(
        customerId: string,
        pageNumber: number = 1,
        pageSize: number = 20,
    ): CancelablePromise<PostResponseDtoPaginatedResultApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Posts/customer/{customerId}',
            path: {
                'customerId': customerId,
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
     * Tìm kiếm posts theo keyword và date range
     * @param keyword Từ khóa tìm kiếm
     * @param fromDate Từ ngày (nullable)
     * @param toDate Đến ngày (nullable)
     * @param pageNumber Số trang (default: 1)
     * @param pageSize Số items mỗi trang (default: 20)
     * @returns PostResponseDtoPaginatedResultApiResponse OK
     * @throws ApiError
     */
    public getApiV1PostsSearch(
        keyword?: string,
        fromDate?: string,
        toDate?: string,
        pageNumber: number = 1,
        pageSize: number = 20,
    ): CancelablePromise<PostResponseDtoPaginatedResultApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Posts/search',
            query: {
                'keyword': keyword,
                'fromDate': fromDate,
                'toDate': toDate,
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Toggle like/unlike post (yêu cầu authentication)
     * @param postId ID của post
     * @returns PostLikeResultApiResponse OK
     * @throws ApiError
     */
    public postApiV1PostsLike(
        postId: string,
    ): CancelablePromise<PostLikeResultApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Posts/{postId}/like',
            path: {
                'postId': postId,
            },
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Toggle bookmark/unbookmark post (yêu cầu authentication)
     * @param postId ID của post
     * @returns PostBookmarkResultApiResponse OK
     * @throws ApiError
     */
    public postApiV1PostsBookmark(
        postId: string,
    ): CancelablePromise<PostBookmarkResultApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Posts/{postId}/bookmark',
            path: {
                'postId': postId,
            },
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                404: `Not Found`,
            },
        });
    }
}

/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BooleanApiResponse } from '../models/BooleanApiResponse';
import type { CommentDetailDtoApiResponse } from '../models/CommentDetailDtoApiResponse';
import type { CommentDtoApiResponse } from '../models/CommentDtoApiResponse';
import type { CommentDtoListApiResponse } from '../models/CommentDtoListApiResponse';
import type { CommentDtoPaginatedResponseApiResponse } from '../models/CommentDtoPaginatedResponseApiResponse';
import type { CreateCommentDto } from '../models/CreateCommentDto';
import type { SaveCommentDto } from '../models/SaveCommentDto';
import type { UpdateCommentDto } from '../models/UpdateCommentDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class CommentsService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Tạo comment mới (root comment hoặc reply)
     * @param requestBody Thông tin comment cần tạo
     * @returns CommentDtoApiResponse OK
     * @throws ApiError
     */
    public postApiV1Comments(
        requestBody?: CreateCommentDto,
    ): CancelablePromise<CommentDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Comments',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
            },
        });
    }
    /**
     * Cập nhật comment (chỉ owner có quyền)
     * @param commentId ID của comment cần update
     * @param requestBody Thông tin cập nhật
     * @returns CommentDtoApiResponse OK
     * @throws ApiError
     */
    public putApiV1Comments(
        commentId: string,
        requestBody?: UpdateCommentDto,
    ): CancelablePromise<CommentDtoApiResponse> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/v1/Comments/{commentId}',
            path: {
                'commentId': commentId,
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
     * Xóa comment (soft delete, chỉ owner có quyền)
     * @param commentId ID của comment cần xóa
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public deleteApiV1Comments(
        commentId: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/v1/Comments/{commentId}',
            path: {
                'commentId': commentId,
            },
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Lấy chi tiết một comment kèm replies
     * @param commentId ID của comment
     * @returns CommentDetailDtoApiResponse OK
     * @throws ApiError
     */
    public getApiV1Comments(
        commentId: string,
    ): CancelablePromise<CommentDetailDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Comments/{commentId}',
            path: {
                'commentId': commentId,
            },
            errors: {
                404: `Not Found`,
            },
        });
    }
    /**
     * Lấy danh sách comments của một post với pagination
     * @param postId ID của post
     * @param pageNumber Số trang (default: 1)
     * @param pageSize Số lượng items trên mỗi trang (default: 20)
     * @returns CommentDtoPaginatedResponseApiResponse OK
     * @throws ApiError
     */
    public getApiV1CommentsPost(
        postId: string,
        pageNumber: number = 1,
        pageSize: number = 20,
    ): CancelablePromise<CommentDtoPaginatedResponseApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Comments/post/{postId}',
            path: {
                'postId': postId,
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
     * Lấy danh sách replies của một comment
     * @param commentId ID của comment cha
     * @returns CommentDtoListApiResponse OK
     * @throws ApiError
     */
    public getApiV1CommentsReplies(
        commentId: string,
    ): CancelablePromise<CommentDtoListApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Comments/{commentId}/replies',
            path: {
                'commentId': commentId,
            },
            errors: {
                404: `Not Found`,
            },
        });
    }
    /**
     * Save comment (Create hoặc Update dựa trên CommentId)
     * @param requestBody Thông tin comment
     * @returns CommentDtoApiResponse OK
     * @throws ApiError
     */
    public postApiV1CommentsSave(
        requestBody?: SaveCommentDto,
    ): CancelablePromise<CommentDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Comments/save',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
            },
        });
    }
}

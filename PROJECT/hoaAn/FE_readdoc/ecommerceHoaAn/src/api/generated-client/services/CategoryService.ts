/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BooleanApiResponse } from '../models/BooleanApiResponse';
import type { CategoryDetailDtoApiResponse } from '../models/CategoryDetailDtoApiResponse';
import type { CategoryDtoApiResponse } from '../models/CategoryDtoApiResponse';
import type { CategoryDtoIEnumerableApiResponse } from '../models/CategoryDtoIEnumerableApiResponse';
import type { CategoryDtoPaginatedResultApiResponse } from '../models/CategoryDtoPaginatedResultApiResponse';
import type { CreateCategoryDto } from '../models/CreateCategoryDto';
import type { Int32ApiResponse } from '../models/Int32ApiResponse';
import type { UpdateCategoryDto } from '../models/UpdateCategoryDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class CategoryService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @param id
     * @returns CategoryDtoApiResponse OK
     * @throws ApiError
     */
    public getApiCategory(
        id: string,
    ): CancelablePromise<CategoryDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/Category/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns CategoryDtoApiResponse OK
     * @throws ApiError
     */
    public putApiCategory(
        id: string,
        requestBody?: UpdateCategoryDto,
    ): CancelablePromise<CategoryDtoApiResponse> {
        return this.httpRequest.request({
            method: 'PUT',
            url: '/api/Category/{id}',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public deleteApiCategory(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'DELETE',
            url: '/api/Category/{id}',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @returns CategoryDtoIEnumerableApiResponse OK
     * @throws ApiError
     */
    public getApiCategory1(): CancelablePromise<CategoryDtoIEnumerableApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/Category',
        });
    }
    /**
     * @param requestBody
     * @returns CategoryDtoApiResponse OK
     * @throws ApiError
     */
    public postApiCategory(
        requestBody?: CreateCategoryDto,
    ): CancelablePromise<CategoryDtoApiResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/Category',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @param id
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public patchApiCategorySoftDelete(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'PATCH',
            url: '/api/Category/{id}/soft-delete',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param storeId
     * @returns CategoryDtoIEnumerableApiResponse OK
     * @throws ApiError
     */
    public getApiCategoryStore(
        storeId: string,
    ): CancelablePromise<CategoryDtoIEnumerableApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/Category/store/{storeId}',
            path: {
                'storeId': storeId,
            },
        });
    }
    /**
     * @returns CategoryDtoIEnumerableApiResponse OK
     * @throws ApiError
     */
    public getApiCategoryActiveList(): CancelablePromise<CategoryDtoIEnumerableApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/Category/active/list',
        });
    }
    /**
     * @param parentId
     * @returns CategoryDtoIEnumerableApiResponse OK
     * @throws ApiError
     */
    public getApiCategorySubcategories(
        parentId: string,
    ): CancelablePromise<CategoryDtoIEnumerableApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/Category/{parentId}/subcategories',
            path: {
                'parentId': parentId,
            },
        });
    }
    /**
     * @param id
     * @returns CategoryDetailDtoApiResponse OK
     * @throws ApiError
     */
    public getApiCategoryDetails(
        id: string,
    ): CancelablePromise<CategoryDetailDtoApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/Category/{id}/details',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param storeId
     * @returns CategoryDtoIEnumerableApiResponse OK
     * @throws ApiError
     */
    public getApiCategoryHierarchy(
        storeId: string,
    ): CancelablePromise<CategoryDtoIEnumerableApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/Category/hierarchy/{storeId}',
            path: {
                'storeId': storeId,
            },
        });
    }
    /**
     * @param pageNumber
     * @param pageSize
     * @param storeId
     * @param isActive
     * @param searchTerm
     * @returns CategoryDtoPaginatedResultApiResponse OK
     * @throws ApiError
     */
    public getApiCategoryPagedList(
        pageNumber: number = 1,
        pageSize: number = 10,
        storeId?: string,
        isActive?: boolean,
        searchTerm?: string,
    ): CancelablePromise<CategoryDtoPaginatedResultApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/Category/paged/list',
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
                'storeId': storeId,
                'isActive': isActive,
                'searchTerm': searchTerm,
            },
        });
    }
    /**
     * @param storeId
     * @returns Int32ApiResponse OK
     * @throws ApiError
     */
    public getApiCategoryCountByStore(
        storeId: string,
    ): CancelablePromise<Int32ApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/Category/count/by-store/{storeId}',
            path: {
                'storeId': storeId,
            },
        });
    }
    /**
     * @param storeId
     * @param name
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public getApiCategoryExists(
        storeId?: string,
        name?: string,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/Category/exists',
            query: {
                'storeId': storeId,
                'name': name,
            },
        });
    }
    /**
     * @param id
     * @param isActive
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public patchApiCategoryStatus(
        id: string,
        isActive?: boolean,
    ): CancelablePromise<BooleanApiResponse> {
        return this.httpRequest.request({
            method: 'PATCH',
            url: '/api/Category/{id}/status',
            path: {
                'id': id,
            },
            query: {
                'isActive': isActive,
            },
        });
    }
}

/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BooleanApiResponse } from '../models/BooleanApiResponse';
import type { Int32ApiResponse } from '../models/Int32ApiResponse';
import type { ProductCreateDto } from '../models/ProductCreateDto';
import type { ProductDetailDtoApiResponse } from '../models/ProductDetailDtoApiResponse';
import type { ProductListDtoListApiResponse } from '../models/ProductListDtoListApiResponse';
import type { ProductListDtoPaginatedResultApiResponse } from '../models/ProductListDtoPaginatedResultApiResponse';
import type { ProductUpdateDto } from '../models/ProductUpdateDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class ProductService {
    /**
     * @param requestBody
     * @returns ProductDetailDtoApiResponse OK
     * @throws ApiError
     */
    public static postApiV1Product(
        requestBody?: ProductCreateDto,
    ): CancelablePromise<ProductDetailDtoApiResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Product',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                403: `Forbidden`,
            },
        });
    }
    /**
     * @param page
     * @param pageSize
     * @param searchTerm
     * @param categoryId
     * @param storeId
     * @param isActive
     * @param isFeatured
     * @param minPrice
     * @param maxPrice
     * @param sortBy
     * @param isDescending
     * @returns ProductListDtoPaginatedResultApiResponse OK
     * @throws ApiError
     */
    public static getApiV1Product(
        page?: number,
        pageSize?: number,
        searchTerm?: string,
        categoryId?: string,
        storeId?: string,
        isActive?: boolean,
        isFeatured?: boolean,
        minPrice?: number,
        maxPrice?: number,
        sortBy?: string,
        isDescending?: boolean,
    ): CancelablePromise<ProductListDtoPaginatedResultApiResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Product',
            query: {
                'Page': page,
                'PageSize': pageSize,
                'SearchTerm': searchTerm,
                'CategoryId': categoryId,
                'StoreId': storeId,
                'IsActive': isActive,
                'IsFeatured': isFeatured,
                'MinPrice': minPrice,
                'MaxPrice': maxPrice,
                'SortBy': sortBy,
                'IsDescending': isDescending,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns ProductDetailDtoApiResponse OK
     * @throws ApiError
     */
    public static putApiV1Product(
        id: string,
        requestBody?: ProductUpdateDto,
    ): CancelablePromise<ProductDetailDtoApiResponse> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/v1/Product/{id}',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param id
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public static deleteApiV1Product(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/v1/Product/{id}',
            path: {
                'id': id,
            },
            errors: {
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * @param id
     * @returns ProductDetailDtoApiResponse OK
     * @throws ApiError
     */
    public static getApiV1Product1(
        id: string,
    ): CancelablePromise<ProductDetailDtoApiResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Product/{id}',
            path: {
                'id': id,
            },
            errors: {
                404: `Not Found`,
            },
        });
    }
    /**
     * @param slug
     * @returns ProductDetailDtoApiResponse OK
     * @throws ApiError
     */
    public static getApiV1ProductSlug(
        slug: string,
    ): CancelablePromise<ProductDetailDtoApiResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Product/slug/{slug}',
            path: {
                'slug': slug,
            },
            errors: {
                404: `Not Found`,
            },
        });
    }
    /**
     * @param storeId
     * @returns ProductListDtoListApiResponse OK
     * @throws ApiError
     */
    public static getApiV1ProductStore(
        storeId: string,
    ): CancelablePromise<ProductListDtoListApiResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Product/store/{storeId}',
            path: {
                'storeId': storeId,
            },
        });
    }
    /**
     * @param categoryId
     * @returns ProductListDtoListApiResponse OK
     * @throws ApiError
     */
    public static getApiV1ProductCategory(
        categoryId: string,
    ): CancelablePromise<ProductListDtoListApiResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Product/category/{categoryId}',
            path: {
                'categoryId': categoryId,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public static patchApiV1ProductStock(
        id: string,
        requestBody?: number,
    ): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
            method: 'PATCH',
            url: '/api/v1/Product/{id}/stock',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                403: `Forbidden`,
            },
        });
    }
    /**
     * @param id
     * @returns Int32ApiResponse OK
     * @throws ApiError
     */
    public static getApiV1ProductStock(
        id: string,
    ): CancelablePromise<Int32ApiResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Product/{id}/stock',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public static patchApiV1ProductActive(
        id: string,
        requestBody?: boolean,
    ): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
            method: 'PATCH',
            url: '/api/v1/Product/{id}/active',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                403: `Forbidden`,
            },
        });
    }
    /**
     * @param id
     * @param requestBody
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public static patchApiV1ProductFeatured(
        id: string,
        requestBody?: boolean,
    ): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
            method: 'PATCH',
            url: '/api/v1/Product/{id}/featured',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                403: `Forbidden`,
            },
        });
    }
    /**
     * @param id
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public static postApiV1ProductView(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Product/{id}/view',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @param id
     * @returns BooleanApiResponse OK
     * @throws ApiError
     */
    public static postApiV1ProductFavorite(
        id: string,
    ): CancelablePromise<BooleanApiResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Product/{id}/favorite',
            path: {
                'id': id,
            },
        });
    }
    /**
     * @returns ProductListDtoListApiResponse OK
     * @throws ApiError
     */
    public static getApiV1ProductFavorites(): CancelablePromise<ProductListDtoListApiResponse> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/v1/Product/favorites',
        });
    }
}

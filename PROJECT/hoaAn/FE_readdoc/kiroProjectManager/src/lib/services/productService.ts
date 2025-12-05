/**
 * Product Service
 * 
 * High-level service layer for product management operations.
 */

import type { AxiosError } from 'axios';
import {
    getApiProduct,
    getApiProductId,
    postApiProduct,
    putApiProductId,
    deleteApiProductId
} from '../../../api/generated-orval/product/product';

interface ErrorResponse {
    message?: string;
}

export const productService = {
    /**
     * Get paginated list of products
     */
    async getProducts(params?: {
        PageNumber?: number;
        PageSize?: number;
        SearchTerm?: string;
        CategoryId?: string;
        IsActive?: boolean;
    }) {
        try {
            const response = await getApiProduct(params);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to fetch products'
            );
        }
    },

    /**
     * Get product by ID
     */
    async getProductById(id: string) {
        try {
            const response = await getApiProductId(id);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to fetch product'
            );
        }
    },

    /**
     * Create new product
     */
    async createProduct(data: any) {
        try {
            const response = await postApiProduct(data);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to create product'
            );
        }
    },

    /**
     * Update product
     */
    async updateProduct(id: string, data: any) {
        try {
            const response = await putApiProductId(id, data);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to update product'
            );
        }
    },

    /**
     * Delete product
     */
    async deleteProduct(id: string) {
        try {
            const response = await deleteApiProductId(id);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to delete product'
            );
        }
    },

    /**
     * Search products by term
     */
    async searchProducts(term: string) {
        try {
            const response = await getApiProduct({ SearchTerm: term });
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to search products'
            );
        }
    },
};

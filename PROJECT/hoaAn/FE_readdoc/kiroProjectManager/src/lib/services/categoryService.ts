/**
 * Category Service
 * 
 * High-level service layer for category management operations.
 */

import type { AxiosError } from 'axios';
import {
    getApiCategory,
    getApiCategoryId,
    postApiCategory,
    putApiCategoryId,
    deleteApiCategoryId,
    getApiCategoryHierarchyStoreId,
    getApiCategoryParentIdSubcategories,
    getApiCategoryPagedList
} from '../../../api/generated-orval/category/category';

interface ErrorResponse {
    message?: string;
}

export const categoryService = {
    /**
     * Get list of categories
     */
    async getCategories(params?: {
        storeId?: string;
        isActive?: boolean;
        pageNumber?: number;
        pageSize?: number;
        searchTerm?: string;
    }) {
        try {
            const response = params ? await getApiCategoryPagedList(params) : await getApiCategory();
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to fetch categories'
            );
        }
    },

    /**
     * Get category by ID
     */
    async getCategoryById(id: string) {
        try {
            const response = await getApiCategoryId(id);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to fetch category'
            );
        }
    },

    /**
     * Create new category
     */
    async createCategory(data: any) {
        try {
            const response = await postApiCategory(data);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to create category'
            );
        }
    },

    /**
     * Update category
     */
    async updateCategory(id: string, data: any) {
        try {
            const response = await putApiCategoryId(id, data);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to update category'
            );
        }
    },

    /**
     * Delete category
     */
    async deleteCategory(id: string) {
        try {
            const response = await deleteApiCategoryId(id);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to delete category'
            );
        }
    },

    /**
     * Get category hierarchy for a store
     */
    async getCategoryHierarchy(storeId: string) {
        try {
            const response = await getApiCategoryHierarchyStoreId(storeId);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to fetch category hierarchy'
            );
        }
    },

    /**
     * Get subcategories of a parent category
     */
    async getSubcategories(parentId: string) {
        try {
            const response = await getApiCategoryParentIdSubcategories(parentId);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to fetch subcategories'
            );
        }
    },
};

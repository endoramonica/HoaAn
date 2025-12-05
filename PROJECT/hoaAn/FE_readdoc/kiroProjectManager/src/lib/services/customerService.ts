/**
 * Customer Service
 * 
 * High-level service layer for customer management operations.
 * Wraps Orval-generated API functions with additional business logic,
 * error handling, and data transformation.
 */

import type { AxiosError } from 'axios';
import {
    getApiAdminCustomer,
    getApiAdminCustomerId,
    postApiAdminCustomer,
    putApiAdminCustomerId,
    deleteApiAdminCustomerId,
    getApiAdminCustomerSearch,
    getApiAdminCustomerIdStatistics
} from '../../../api/generated-orval/admin-customer/admin-customer';

interface ErrorResponse {
    message?: string;
}

/**
 * Customer Service
 * Provides high-level methods for customer operations
 */
export const customerService = {
    /**
     * Get paginated list of customers
     * @param params - Query parameters for filtering and pagination
     * @returns Promise with customer list
     */
    async getCustomers(params?: {
        Page?: number;
        PageSize?: number;
        SearchTerm?: string;
        IsActive?: boolean;
        Tier?: string;
    }) {
        try {
            const response = await getApiAdminCustomer(params);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to fetch customers'
            );
        }
    },

    /**
     * Get customer by ID
     * @param id - Customer ID
     * @returns Promise with customer details
     */
    async getCustomerById(id: string) {
        try {
            const response = await getApiAdminCustomerId(id);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to fetch customer'
            );
        }
    },

    /**
     * Create new customer
     * @param data - Customer data
     * @returns Promise with created customer
     */
    async createCustomer(data: any) {
        try {
            const response = await postApiAdminCustomer(data);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to create customer'
            );
        }
    },

    /**
     * Update customer
     * @param id - Customer ID
     * @param data - Updated customer data
     * @returns Promise with updated customer
     */
    async updateCustomer(id: string, data: any) {
        try {
            const response = await putApiAdminCustomerId(id, data);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to update customer'
            );
        }
    },

    /**
     * Delete customer
     * @param id - Customer ID
     * @returns Promise with deletion result
     */
    async deleteCustomer(id: string) {
        try {
            const response = await deleteApiAdminCustomerId(id);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to delete customer'
            );
        }
    },

    /**
     * Search customers by term
     * @param term - Search term
     * @returns Promise with matching customers
     */
    async searchCustomers(term: string) {
        try {
            const response = await getApiAdminCustomerSearch({ term });
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to search customers'
            );
        }
    },

    /**
     * Get customer statistics
     * @param id - Customer ID
     * @returns Promise with customer statistics
     */
    async getCustomerStatistics(id: string) {
        try {
            const response = await getApiAdminCustomerIdStatistics(id);
            return response.data;
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponse>;
            throw new Error(
                axiosError.response?.data?.message || 'Failed to fetch customer statistics'
            );
        }
    },
};

/**
 * Customer Admin Service
 * Handles API calls to GET/PUT /api/v1/CustomerAdmin/{id}
 */

import { getVietCommerceAPI } from '../../../Api/generated-orval';

// Types from Orval generated API
type CustomerDetailDto = any;
type UpdateCustomerRequest = any;

const api = getVietCommerceAPI();

export const customerAdminService = {
    /**
     * Get customer details by ID
     * GET /api/v1/CustomerAdmin/{id}
     * 
     * Response format:
     * {
     *   success: true,
     *   data: {
     *     customer: { ... },
     *     user: { ... }
     *   },
     *   message: "..."
     * }
     */
    async getCustomerById(id: string): Promise<CustomerDetailDto> {
        try {
            const response = await api.getApiV1CustomerAdminId(id);
            console.log('[customerAdminService] Raw response:', response);

            // Extract customer data from nested structure
            const customerData = response.data?.customer || response.data;

            if (!customerData) {
                throw new Error('No customer data returned');
            }

            console.log('[customerAdminService] Extracted customer data:', customerData);
            return customerData;
        } catch (error) {
            console.error('[customerAdminService] Failed to get customer:', error);
            throw error;
        }
    },

    /**
     * Update customer details
     * PUT /api/v1/CustomerAdmin/{id}
     */
    async updateCustomer(
        id: string,
        data: UpdateCustomerRequest
    ): Promise<CustomerDetailDto> {
        try {
            const response = await api.putApiV1CustomerAdminId(id, data);
            console.log('[customerAdminService] Update response:', response);

            // Extract customer data from nested structure
            const customerData = response.data?.customer || response.data;

            if (!customerData) {
                throw new Error('No customer data returned');
            }

            console.log('[customerAdminService] Updated customer data:', customerData);
            return customerData;
        } catch (error) {
            console.error('[customerAdminService] Failed to update customer:', error);
            throw error;
        }
    },
};

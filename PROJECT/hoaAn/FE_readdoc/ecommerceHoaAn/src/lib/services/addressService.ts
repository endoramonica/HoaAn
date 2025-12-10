/**
 * Address Service
 * Handles API calls for customer addresses
 * Uses /api/v1/customer/addresses endpoints
 */

import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type { AddressResponseDto } from '../../../Api/generated-orval/schemas/addressResponseDto';
import type { CreateAddressDto } from '../../../Api/generated-orval/schemas/createAddressDto';
import type { UpdateAddressDto } from '../../../Api/generated-orval/schemas/updateAddressDto';

const api = getVietCommerceAPI();

export const addressService = {
    /**
     * Get all addresses for current customer
     * GET /api/v1/customer/addresses
     */
    async getAddresses(): Promise<AddressResponseDto[]> {
        try {
            const response = await api.getApiV1CustomerAddresses();
            console.log('[addressService] Get addresses response:', response);

            const addresses = response.data || [];
            return Array.isArray(addresses) ? addresses : [];
        } catch (error) {
            console.error('[addressService] Failed to get addresses:', error);
            throw error;
        }
    },

    /**
     * Get a single address by ID
     * GET /api/v1/customer/addresses/{id}
     */
    async getAddressById(id: string): Promise<AddressResponseDto> {
        try {
            const response = await api.getApiV1CustomerAddressesId(id);
            console.log('[addressService] Get address response:', response);

            const address = response.data;
            if (!address) {
                throw new Error('No address data returned');
            }

            return address;
        } catch (error) {
            console.error('[addressService] Failed to get address:', error);
            throw error;
        }
    },

    /**
     * Create a new address
     * POST /api/v1/customer/addresses
     */
    async createAddress(data: CreateAddressDto): Promise<AddressResponseDto> {
        try {
            const response = await api.postApiV1CustomerAddresses(data);
            console.log('[addressService] Create address response:', response);

            const address = response.data;
            if (!address) {
                throw new Error('No address data returned');
            }

            return address;
        } catch (error) {
            console.error('[addressService] Failed to create address:', error);
            throw error;
        }
    },

    /**
     * Update an address
     * PUT /api/v1/customer/addresses/{id}
     */
    async updateAddress(
        id: string,
        data: UpdateAddressDto
    ): Promise<AddressResponseDto> {
        try {
            const response = await api.putApiV1CustomerAddressesId(id, data);
            console.log('[addressService] Update address response:', response);

            const address = response.data;
            if (!address) {
                throw new Error('No address data returned');
            }

            return address;
        } catch (error) {
            console.error('[addressService] Failed to update address:', error);
            throw error;
        }
    },

    /**
     * Delete an address
     * DELETE /api/v1/customer/addresses/{id}
     */
    async deleteAddress(id: string): Promise<void> {
        try {
            await api.deleteApiV1CustomerAddressesId(id);
            console.log('[addressService] Address deleted successfully');
        } catch (error) {
            console.error('[addressService] Failed to delete address:', error);
            throw error;
        }
    },

    /**
     * Set an address as default
     * POST /api/v1/customer/addresses/{id}/set-default
     */
    async setDefaultAddress(id: string): Promise<boolean> {
        try {
            const response = await api.postApiV1CustomerAddressesIdSetDefault(id);
            console.log('[addressService] Set default address response:', response);

            const success = response.data || response.success;
            if (!success) {
                throw new Error('Failed to set default address');
            }

            return true;
        } catch (error) {
            console.error('[addressService] Failed to set default address:', error);
            throw error;
        }
    },
};

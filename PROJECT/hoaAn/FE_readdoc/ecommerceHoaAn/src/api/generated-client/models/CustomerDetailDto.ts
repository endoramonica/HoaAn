/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CustomerAddressDto } from './CustomerAddressDto';
/**
 * DTO for detailed customer information
 */
export type CustomerDetailDto = {
    id?: string;
    name?: string | null;
    email?: string | null;
    phone?: string | null;
    avatar?: string | null;
    loyaltyPoints?: number;
    tier?: string | null;
    isActive?: boolean;
    userId?: string | null;
    storeId?: string | null;
    storeName?: string | null;
    tenantId?: string;
    createdAt?: string;
    updatedAt?: string;
    createdBy?: string;
    updatedBy?: string | null;
    totalOrders?: number;
    totalSpent?: number;
    totalInteractions?: number;
    userProvider?: string | null;
    lastLogin?: string | null;
    userStatus?: string | null;
    addresses?: Array<CustomerAddressDto> | null;
};


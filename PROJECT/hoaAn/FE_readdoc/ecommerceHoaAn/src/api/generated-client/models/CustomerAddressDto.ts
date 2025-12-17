/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AddressType } from './AddressType';
/**
 * DTO for customer address details
 */
export type CustomerAddressDto = {
    id?: string;
    customerId?: string;
    streetAddress?: string | null;
    city?: string | null;
    postalCode?: string | null;
    state?: string | null;
    country?: string | null;
    addressType?: AddressType;
    addressTypeText?: string | null;
    recipientName?: string | null;
    phoneNumber?: string | null;
    email?: string | null;
    isDefault?: boolean;
    isPrimary?: boolean;
    isActive?: boolean;
    createdAt?: string;
    updatedAt?: string;
};


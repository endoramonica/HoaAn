/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AddressType } from './AddressType';
export type UpdateAddressDto = {
    streetAddress?: string | null;
    city?: string | null;
    postalCode?: string | null;
    state?: string | null;
    country?: string | null;
    addressType?: AddressType;
    recipientName?: string | null;
    phoneNumber?: string | null;
    email?: string | null;
    isDefault?: boolean | null;
    isPrimary?: boolean | null;
    isActive?: boolean | null;
};


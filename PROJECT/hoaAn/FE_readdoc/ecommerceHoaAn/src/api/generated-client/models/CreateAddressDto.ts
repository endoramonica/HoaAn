/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AddressType } from './AddressType';
export type CreateAddressDto = {
    streetAddress: string;
    city?: string | null;
    postalCode?: string | null;
    state?: string | null;
    country?: string | null;
    addressType: AddressType;
    recipientName?: string | null;
    phoneNumber?: string | null;
    email?: string | null;
    isDefault?: boolean;
    isPrimary?: boolean;
};


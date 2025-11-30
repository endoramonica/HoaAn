/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ShippingMethodEnum } from './ShippingMethodEnum';
export type OrderShippingInputDto = {
    recipientName: string;
    phoneNumber: string;
    address: string;
    ward: string;
    district: string;
    city: string;
    postalCode?: string | null;
    deliveryNote?: string | null;
    shippingMethod: ShippingMethodEnum;
};


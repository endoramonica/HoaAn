/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OrderItemDTO } from './OrderItemDTO';
import type { OrderShippingDto } from './OrderShippingDto';
import type { OrderStatus } from './OrderStatus';
export type CheckoutResponseDto = {
    orderId?: string;
    orderNumber?: string | null;
    status?: OrderStatus;
    readonly statusText?: string | null;
    totalAmount?: number;
    createdAt?: string;
    storeId?: string;
    storeName?: string | null;
    customerId?: string;
    customerName?: string | null;
    items?: Array<OrderItemDTO> | null;
    shipping?: OrderShippingDto;
    paymentUrl?: string | null;
    paymentMethodUsed?: string | null;
};


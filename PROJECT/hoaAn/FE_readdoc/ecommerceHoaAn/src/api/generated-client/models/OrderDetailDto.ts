/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OrderItemDTO } from './OrderItemDTO';
import type { OrderShippingDto } from './OrderShippingDto';
import type { OrderStatus } from './OrderStatus';
import type { OrderStatusHistoryDTO } from './OrderStatusHistoryDTO';
export type OrderDetailDto = {
    orderId?: string;
    orderNumber?: string | null;
    storeId?: string;
    storeName?: string | null;
    customerId?: string;
    customerName?: string | null;
    customerEmail?: string | null;
    customerPhone?: string | null;
    status?: OrderStatus;
    readonly statusText?: string | null;
    subTotal?: number;
    shippingFee?: number;
    taxAmount?: number;
    discountAmount?: number;
    totalAmount?: number;
    notes?: string | null;
    createdAt?: string;
    updatedAt?: string | null;
    completedAt?: string | null;
    createdById?: string | null;
    createdByName?: string | null;
    shipping?: OrderShippingDto;
    items?: Array<OrderItemDTO> | null;
    readonly itemsCount?: number;
    statusHistories?: Array<OrderStatusHistoryDTO> | null;
    isPaid?: boolean;
    paidAmount?: number | null;
    paymentMethod?: string | null;
    paidAt?: string | null;
    transactionId?: string | null;
    type?: string | null;
    serviceCategory?: string | null;
    serviceDuration?: string | null;
    serviceLocation?: string | null;
    serviceDate?: string | null;
    serviceNotes?: string | null;
};


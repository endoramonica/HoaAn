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
    /**
     * Type discriminator: "product" or "service"
     */
    type?: string | null;
    /**
     * Service category (only for type='service')
     */
    serviceCategory?: string | null;
    /**
     * Service duration (only for type='service')
     */
    serviceDuration?: string | null;
    /**
     * Service location/address (only for type='service')
     */
    serviceLocation?: string | null;
    /**
     * Scheduled service date (only for type='service')
     */
    serviceDate?: string | null;
    /**
     * Scheduled service time (only for type='service')
     */
    serviceNotes?: string | null;
};


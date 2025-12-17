/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
export type OrderItemDTO = {
    id?: string;
    orderId?: string;
    productId?: string;
    productName?: string | null;
    productSKU?: string | null;
    productImageUrl?: string | null;
    unitPrice?: number;
    quantity?: number;
    totalPrice?: number;
    readonly subtotal?: number;
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
};


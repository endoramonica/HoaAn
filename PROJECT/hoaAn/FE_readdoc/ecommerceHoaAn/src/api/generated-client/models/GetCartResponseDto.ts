/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CartItemDetailDto } from './CartItemDetailDto';
export type GetCartResponseDto = {
    cartId?: string;
    userId?: string;
    /**
     * List of cart items with detailed information including customizations.
     * Each item includes BasePrice, CustomizationPrice, and FinalPrice breakdown.
     */
    items?: Array<CartItemDetailDto> | null;
    totalItems?: number;
    subTotal?: number;
    taxAmount?: number;
    shippingFee?: number;
    totalAmount?: number;
    createdAt?: string;
    updatedAt?: string;
};


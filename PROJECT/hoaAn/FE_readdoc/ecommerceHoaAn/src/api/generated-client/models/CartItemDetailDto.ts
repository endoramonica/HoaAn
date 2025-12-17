/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CartItemCustomizationDto } from './CartItemCustomizationDto';
export type CartItemDetailDto = {
    cartItemId?: string;
    productId?: string;
    productName?: string | null;
    productSlug?: string | null;
    sku?: string | null;
    productImage?: string | null;
    unitPrice?: number;
    quantity?: number;
    totalPrice?: number;
    availableStock?: number;
    isProductActive?: boolean;
    purchaseCount?: number;
    avgRating?: number;
    reviewCount?: number;
    createdAt?: string;
    updatedAt?: string;
    /**
     * Gets or sets the base price of the package product before customizations.
     * This is the fixed price of the package itself.
     * Example: 3,500,000đ for a ritual package
     */
    basePrice?: number;
    /**
     * Gets or sets the total price of all customizations applied to this cart item.
     * Calculated as the sum of all customization option prices.
     * Example: 785,000đ (sum of customization surcharges)
     */
    customizationPrice?: number;
    /**
     * Gets or sets the final total price for this cart item.
     * Calculated as: BasePrice + CustomizationPrice
     * Example: 4,285,000đ (3,500,000 + 785,000)
     */
    finalPrice?: number;
    /**
     * Gets or sets the list of customizations applied to this cart item.
     * Contains details about which options were customized and their quantities.
     * Null if the product is not customizable or no customizations were applied.
     */
    customizations?: Array<CartItemCustomizationDto> | null;
};


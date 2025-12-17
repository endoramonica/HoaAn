/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CartItemCustomizationDto } from './CartItemCustomizationDto';
/**
 * Data transfer object for adding a product to the shopping cart.
 * Supports both regular products and customizable package products.
 */
export type AddToCartDto = {
    /**
     * Gets or sets the unique identifier of the product to add to cart.
     */
    productId?: string;
    /**
     * Gets or sets the quantity of the product to add.
     * Defaults to 1 if not specified.
     */
    quantity?: number;
    /**
     * Gets or sets the list of customizations for package products.
     * Nullable - only populated when adding customizable package products.
     * Each customization specifies an option ID and desired quantity.
     * Example: Customer adds "Mâm Cúng Khai Trương" with 10 dĩa xôi instead of 5.
     */
    customizations?: Array<CartItemCustomizationDto> | null;
};


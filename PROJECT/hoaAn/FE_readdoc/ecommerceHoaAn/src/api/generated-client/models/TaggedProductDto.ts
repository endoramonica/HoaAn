/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * Lightweight product information for marketing posts.
 * Contains live product data (price, discount, image) that is fetched at retrieval time.
 * Used to display current product details alongside marketing content.
 */
export type TaggedProductDto = {
    /**
     * Product unique identifier
     */
    id?: string;
    /**
     * Product name
     */
    name?: string | null;
    /**
     * Current product price
     */
    price?: number;
    /**
     * Currency code (default: VND)
     */
    currency?: string | null;
    /**
     * Formatted price string for display (e.g., "45,000 VND")
     */
    formattedPrice?: string | null;
    /**
     * Product thumbnail image URL
     */
    thumbnailUrl?: string | null;
    /**
     * Indicates if product has an active discount
     */
    hasDiscount?: boolean;
    /**
     * Discount percentage (0-100)
     */
    discountPercentage?: number;
};


/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * DTO for explanation of a single recommended item
 */
export type ItemExplanationDto = {
    /**
     * Product ID being explained
     */
    productId?: string;
    /**
     * Product name
     */
    productName?: string | null;
    /**
     * Why this item is needed for the ritual
     */
    whyNeeded?: string | null;
    /**
     * Traditional usage and cultural significance of the item
     */
    traditionalUsage?: string | null;
};


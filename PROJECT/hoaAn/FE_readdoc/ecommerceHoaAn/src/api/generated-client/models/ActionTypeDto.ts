/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * DTO for action type definitions used in ritual pattern matching
 */
export type ActionTypeDto = {
    /**
     * Type of action (e.g., "ViewProduct", "AddToCart", "BrowseCategory")
     */
    type?: string | null;
    /**
     * Optional product category ID associated with the action
     */
    productCategoryId?: string | null;
    /**
     * Additional metadata for the action type
     */
    metadata?: Record<string, any> | null;
};


/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CustomizableOptionDto } from './CustomizableOptionDto';
export type ProductUpdateDto = {
    name?: string | null;
    shortDescription?: string | null;
    description?: string | null;
    price?: number | null;
    compareAtPrice?: number | null;
    cost?: number | null;
    categoryId?: string | null;
    brandId?: string | null;
    sku?: string | null;
    barcode?: string | null;
    stockQuantity?: number | null;
    isActive?: boolean | null;
    isFeatured?: boolean | null;
    images?: Array<string> | null;
    tags?: Array<string> | null;
    metaTitle?: string | null;
    metaDescription?: string | null;
    metaKeywords?: string | null;
    /**
     * List of fixed items included in the package product (e.g., "Cá chép (3 con)", "Mũ giấy (3 cái)").
     * Only applicable for package products. Null for regular products.
     */
    details?: Array<string> | null;
    /**
     * List of customizable options for the package product.
     * Allows customers to modify quantities of specific items within the package.
     * Only applicable for package products. Null for regular products.
     */
    customizableOptions?: Array<CustomizableOptionDto> | null;
};


/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CustomizableOptionDto } from './CustomizableOptionDto';
export type ProductCreateDto = {
    name: string;
    shortDescription?: string | null;
    description?: string | null;
    code: string;
    price: number;
    compareAtPrice?: number | null;
    cost?: number | null;
    categoryId?: string | null;
    brandId?: string | null;
    sku?: string | null;
    barcode?: string | null;
    stockQuantity?: number;
    isActive?: boolean;
    isFeatured?: boolean;
    images?: Array<string> | null;
    tags?: Array<string> | null;
    metaTitle?: string | null;
    metaDescription?: string | null;
    metaKeywords?: string | null;
    /**
     * List of fixed items included in a package product (e.g., "Cá chép (3 con)", "Mũ giấy (3 cái)").
     * Used for package products to describe what's included.
     */
    details?: Array<string> | null;
    /**
     * List of customizable options for a package product.
     * Allows customers to modify quantities of specific items in the package.
     */
    customizableOptions?: Array<CustomizableOptionDto> | null;
};


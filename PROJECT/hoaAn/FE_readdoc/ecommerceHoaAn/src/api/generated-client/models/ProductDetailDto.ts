/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CustomizableOptionDto } from './CustomizableOptionDto';
import type { DisplayPriceResult } from './DisplayPriceResult';
import type { ProductImageDto } from './ProductImageDto';
export type ProductDetailDto = {
    id?: string;
    name?: string | null;
    shortDescription?: string | null;
    description?: string | null;
    code?: string | null;
    slug?: string | null;
    price?: number;
    compareAtPrice?: number | null;
    cost?: number | null;
    displayPrice?: DisplayPriceResult;
    readonly discountPercentage?: number | null;
    stockQuantity?: number;
    sku?: string | null;
    barcode?: string | null;
    categoryId?: string | null;
    categoryName?: string | null;
    brandId?: string | null;
    brandName?: string | null;
    primaryImage?: string | null;
    images?: Array<ProductImageDto> | null;
    tags?: Array<string> | null;
    metaTitle?: string | null;
    metaDescription?: string | null;
    metaKeywords?: string | null;
    isActive?: boolean;
    isFeatured?: boolean;
    viewCount?: number;
    favoriteCount?: number;
    averageRating?: number;
    reviewCount?: number;
    storeId?: string;
    storeName?: string | null;
    createdAt?: string;
    updatedAt?: string | null;
    createdBy?: string;
    createdByName?: string | null;
    /**
     * List of fixed items included in the package product (e.g., "Cá chép (3 con)", "Mũ giấy (3 cái)").
     * Null if this is not a package product.
     */
    details?: Array<string> | null;
    /**
     * List of customizable options available for this package product.
     * Customers can modify quantities of these options when adding to cart.
     * Null if this is not a customizable package product.
     */
    customizableOptions?: Array<CustomizableOptionDto> | null;
};


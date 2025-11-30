/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { DisplayPriceResult } from './DisplayPriceResult';
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
    images?: Array<string> | null;
    readonly primaryImage?: string | null;
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
};


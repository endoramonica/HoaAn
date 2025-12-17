/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { DisplayPriceResult } from './DisplayPriceResult';
import type { ProductImageDto } from './ProductImageDto';
export type ProductListDto = {
    id?: string;
    name?: string | null;
    shortDescription?: string | null;
    code?: string | null;
    slug?: string | null;
    price?: number;
    compareAtPrice?: number | null;
    displayPrice?: DisplayPriceResult;
    readonly discountPercentage?: number | null;
    stockQuantity?: number;
    readonly inStock?: boolean;
    primaryImage?: string | null;
    images?: Array<ProductImageDto> | null;
    isActive?: boolean;
    isFeatured?: boolean;
    categoryId?: string | null;
    categoryName?: string | null;
    viewCount?: number;
    favoriteCount?: number;
    averageRating?: number;
    createdAt?: string;
    /**
     * Type discriminator: "product" or "service"
     */
    type?: string | null;
    /**
     * Service category (only for type='service')
     */
    serviceCategory?: string | null;
    /**
     * Service duration (only for type='service')
     */
    serviceDuration?: string | null;
    /**
     * Service rating (0-5 stars)
     */
    serviceRating?: number | null;
};


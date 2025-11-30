/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
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
};


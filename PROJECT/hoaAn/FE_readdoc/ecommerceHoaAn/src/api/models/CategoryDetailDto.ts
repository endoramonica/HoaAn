/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CategoryDto } from './CategoryDto';
import type { ProductSummaryDto } from './ProductSummaryDto';
export type CategoryDetailDto = {
    id?: string;
    storeId?: string;
    name?: string | null;
    parentId?: string | null;
    parentName?: string | null;
    isActive?: boolean;
    createdAt?: string;
    updatedAt?: string | null;
    subCategoriesCount?: number;
    productsCount?: number;
    subCategories?: Array<CategoryDto> | null;
    products?: Array<ProductSummaryDto> | null;
};


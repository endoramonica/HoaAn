/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OrderDetailDto } from './OrderDetailDto';
export type OrderDetailDtoPaginatedResponse = {
    items?: Array<OrderDetailDto> | null;
    pageNumber?: number;
    pageSize?: number;
    totalItems?: number;
    readonly totalPages?: number;
    readonly hasPreviousPage?: boolean;
    readonly hasNextPage?: boolean;
};


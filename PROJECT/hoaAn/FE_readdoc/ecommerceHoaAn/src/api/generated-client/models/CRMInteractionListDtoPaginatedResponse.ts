/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CRMInteractionListDto } from './CRMInteractionListDto';
export type CRMInteractionListDtoPaginatedResponse = {
    items?: Array<CRMInteractionListDto> | null;
    pageNumber?: number;
    pageSize?: number;
    totalItems?: number;
    readonly totalPages?: number;
    readonly hasPreviousPage?: boolean;
    readonly hasNextPage?: boolean;
};


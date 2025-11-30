/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CommentDto } from './CommentDto';
export type CommentDtoPaginatedResponse = {
    items?: Array<CommentDto> | null;
    pageNumber?: number;
    pageSize?: number;
    totalItems?: number;
    readonly totalPages?: number;
    readonly hasPreviousPage?: boolean;
    readonly hasNextPage?: boolean;
};


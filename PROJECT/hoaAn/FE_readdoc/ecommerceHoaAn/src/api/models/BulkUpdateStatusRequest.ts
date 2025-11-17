/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OrderStatus } from './OrderStatus';
export type BulkUpdateStatusRequest = {
    orderIds?: Array<string> | null;
    newStatus?: OrderStatus;
    reason?: string | null;
};


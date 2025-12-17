/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OrderStatus } from './OrderStatus';
/**
 * Request to bulk update order statuses
 */
export type BulkUpdateStatusRequest = {
    /**
     * List of order IDs to update
     */
    orderIds?: Array<string> | null;
    newStatus?: OrderStatus;
    /**
     * Optional reason for the update
     */
    reason?: string | null;
};


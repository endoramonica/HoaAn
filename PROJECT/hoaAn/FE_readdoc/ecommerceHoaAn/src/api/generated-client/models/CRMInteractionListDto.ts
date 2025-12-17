/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CRMInteractionStatus } from './CRMInteractionStatus';
import type { CRMInteractionType } from './CRMInteractionType';
/**
 * DTO for CRM Interaction list view
 */
export type CRMInteractionListDto = {
    id?: string;
    customerId?: string;
    customerName?: string | null;
    type?: CRMInteractionType;
    readonly typeText?: string | null;
    title?: string | null;
    status?: CRMInteractionStatus;
    readonly statusText?: string | null;
    followUpDate?: string | null;
    createdAt?: string;
    createdByName?: string | null;
};


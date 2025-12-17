/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CRMInteractionStatus } from './CRMInteractionStatus';
import type { CRMInteractionType } from './CRMInteractionType';
/**
 * DTO for CRM Interaction details
 */
export type CRMInteractionDto = {
    id?: string;
    customerId?: string;
    customerName?: string | null;
    customerEmail?: string | null;
    customerPhone?: string | null;
    type?: CRMInteractionType;
    readonly typeText?: string | null;
    title?: string | null;
    description?: string | null;
    status?: CRMInteractionStatus;
    readonly statusText?: string | null;
    followUpDate?: string | null;
    createdAt?: string;
    updatedAt?: string;
    createdBy?: string;
    createdByName?: string | null;
    updatedBy?: string | null;
    updatedByName?: string | null;
};


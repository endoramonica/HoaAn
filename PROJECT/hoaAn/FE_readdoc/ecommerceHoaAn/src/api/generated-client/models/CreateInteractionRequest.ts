/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CRMInteractionStatus } from './CRMInteractionStatus';
import type { CRMInteractionType } from './CRMInteractionType';
export type CreateInteractionRequest = {
    customerId: string;
    type: CRMInteractionType;
    title: string;
    description: string;
    followUpDate?: string | null;
    status: CRMInteractionStatus;
};


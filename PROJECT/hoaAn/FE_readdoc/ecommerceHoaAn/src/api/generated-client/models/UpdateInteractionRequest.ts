/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CRMInteractionStatus } from './CRMInteractionStatus';
import type { CRMInteractionType } from './CRMInteractionType';
export type UpdateInteractionRequest = {
    type?: CRMInteractionType;
    title?: string | null;
    description?: string | null;
    followUpDate?: string | null;
    status?: CRMInteractionStatus;
};


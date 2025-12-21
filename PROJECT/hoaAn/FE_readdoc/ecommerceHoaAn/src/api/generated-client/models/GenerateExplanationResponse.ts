/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ExplanationPayloadDto } from './ExplanationPayloadDto';
/**
 * Response model for explanation generation
 */
export type GenerateExplanationResponse = {
    /**
     * Whether the request was successful
     */
    success?: boolean;
    /**
     * Message describing the result
     */
    message?: string | null;
    data?: ExplanationPayloadDto;
};


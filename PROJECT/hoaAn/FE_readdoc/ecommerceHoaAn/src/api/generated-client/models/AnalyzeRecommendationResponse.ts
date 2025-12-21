/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { RecommendationPayloadDto } from './RecommendationPayloadDto';
/**
 * Response model for recommendation analysis
 */
export type AnalyzeRecommendationResponse = {
    /**
     * Whether the request was successful
     */
    success?: boolean;
    /**
     * Message describing the result
     */
    message?: string | null;
    data?: RecommendationPayloadDto;
};


/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { GeminiStatusResponse } from '../models/GeminiStatusResponse';
import type { GenerateExplanationRequest } from '../models/GenerateExplanationRequest';
import type { GenerateExplanationResponse } from '../models/GenerateExplanationResponse';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class ExplanationService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Generates a natural language explanation for a recommendation using Gemini API
     *
     * Requirements: 2.1, 2.2, 2.3, 2.5
     * - Accepts RecommendationPayload from frontend
     * - Calls GeminiExplanationService to generate explanation
     * - Returns ExplanationPayload with human-readable text
     * - Includes fallback logic if Gemini API fails
     * @param requestBody Request containing the recommendation payload
     * @returns GenerateExplanationResponse Explanation generated successfully
     * @throws ApiError
     */
    public postApiV1ExplanationGenerate(
        requestBody?: GenerateExplanationRequest,
    ): CancelablePromise<GenerateExplanationResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Explanation/generate',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Invalid request or recommendation payload`,
                500: `Server error during explanation generation`,
            },
        });
    }
    /**
     * Gets the fallback explanation template for a recommendation
     * Useful for testing or when Gemini API is not available
     * @param requestBody Request containing the recommendation payload
     * @returns GenerateExplanationResponse Fallback explanation generated successfully
     * @throws ApiError
     */
    public postApiV1ExplanationFallback(
        requestBody?: GenerateExplanationRequest,
    ): CancelablePromise<GenerateExplanationResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Explanation/fallback',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Invalid request or recommendation payload`,
                500: `Server error`,
            },
        });
    }
    /**
     * Checks if the Gemini API is configured and available
     * @returns GeminiStatusResponse Status check successful
     * @throws ApiError
     */
    public getApiV1ExplanationStatus(): CancelablePromise<GeminiStatusResponse> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Explanation/status',
        });
    }
}

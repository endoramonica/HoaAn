/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { AnalyzeRecommendationRequest } from '../models/AnalyzeRecommendationRequest';
import type { AnalyzeRecommendationResponse } from '../models/AnalyzeRecommendationResponse';
import type { RecordInteractionRequest } from '../models/RecordInteractionRequest';
import type { SuccessResponse } from '../models/SuccessResponse';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class RecommendationService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Analyzes user action sequence to detect ritual patterns and generate recommendations
     *
     * Requirements: 1.1, 1.2, 1.3, 1.4, 1.5
     * - Accepts action sequence from frontend
     * - Calls RecommendationService to match patterns
     * - Returns RecommendationPayload with matched ritual and missing items
     * - Handles invalid sequences gracefully
     * @param requestBody Request containing action sequence and cart items
     * @returns AnalyzeRecommendationResponse Recommendation generated successfully (may be null if no pattern matched)
     * @throws ApiError
     */
    public postApiV1RecommendationAnalyze(
        requestBody?: AnalyzeRecommendationRequest,
    ): CancelablePromise<AnalyzeRecommendationResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Recommendation/analyze',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Invalid request or action sequence`,
                401: `Unauthorized (for authenticated endpoint)`,
                500: `Server error during recommendation generation`,
            },
        });
    }
    /**
     * Records user interaction with a recommendation (viewed, dismissed, clicked)
     * @param logId The recommendation log ID
     * @param requestBody Request containing interaction type
     * @returns SuccessResponse Interaction recorded successfully
     * @throws ApiError
     */
    public postApiV1RecommendationInteraction(
        logId: string,
        requestBody?: RecordInteractionRequest,
    ): CancelablePromise<SuccessResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Recommendation/{logId}/interaction',
            path: {
                'logId': logId,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Invalid request or interaction type`,
                404: `Recommendation log not found`,
                500: `Server error`,
            },
        });
    }
}

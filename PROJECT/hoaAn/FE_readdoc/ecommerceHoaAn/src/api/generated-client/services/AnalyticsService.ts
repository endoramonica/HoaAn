/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BooleanApiResponse } from '../models/BooleanApiResponse';
import type { CampaignStatsDtoApiResponse } from '../models/CampaignStatsDtoApiResponse';
import type { TrackClickDto } from '../models/TrackClickDto';
import type { TrackImpressionDto } from '../models/TrackImpressionDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class AnalyticsService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Track a campaign impression (when campaign is shown to user)
     *
     * Records an impression event when a campaign is displayed to a user. This endpoint is called
     * by the frontend when the AdPopup component renders a campaign. Multiple impressions from the
     * same session on the same page are tracked separately (frequency rules are enforced on frontend).
     *
     * **Validation Rules:**
     * - Campaign must exist and be ACTIVE
     * - SessionId: Required, max 100 characters (typically browser session ID)
     * - Page: Required, max 100 characters (e.g., 'home', 'products', 'checkout')
     *
     * **Requirements:** 7.1
     *
     * **Example Request:**
     * ```json
     * {
         * "sessionId": "sess_abc123def456",
         * "page": "home",
         * "timestamp": "2025-01-15T10:30:00Z"
         * }
         * ```
         *
         * **Note:** This endpoint is public (no authentication required) to allow frontend tracking.
         * @param campaignId Campaign ID (GUID format)
         * @param requestBody Impression tracking data (sessionId, page, timestamp)
         * @returns BooleanApiResponse Impression tracked successfully
         * @throws ApiError
         */
        public postApiV1AnalyticsTrackImpression(
            campaignId: string,
            requestBody?: TrackImpressionDto,
        ): CancelablePromise<BooleanApiResponse> {
            return this.httpRequest.request({
                method: 'POST',
                url: '/api/v1/Analytics/{campaignId}/track-impression',
                path: {
                    'campaignId': campaignId,
                },
                body: requestBody,
                mediaType: 'application/json',
                errors: {
                    400: `Validation error (missing sessionId, page, etc.)`,
                    404: `Campaign not found with the specified ID`,
                },
            });
        }
        /**
         * Track a campaign click (when user clicks on campaign)
         *
         * Records a click event when a user interacts with a campaign (e.g., clicks the CTA button).
         * This endpoint is called by the frontend when the user engages with the AdPopup component.
         *
         * **Validation Rules:**
         * - Campaign must exist and be ACTIVE
         * - SessionId: Required, max 100 characters (typically browser session ID)
         * - Page: Required, max 100 characters (e.g., 'home', 'products', 'checkout')
         *
         * **Requirements:** 7.2
         *
         * **Example Request:**
         * ```json
         * {
             * "sessionId": "sess_abc123def456",
             * "page": "home",
             * "timestamp": "2025-01-15T10:30:15Z"
             * }
             * ```
             *
             * **Note:** This endpoint is public (no authentication required) to allow frontend tracking.
             * @param campaignId Campaign ID (GUID format)
             * @param requestBody Click tracking data (sessionId, page, timestamp)
             * @returns BooleanApiResponse Click tracked successfully
             * @throws ApiError
             */
            public postApiV1AnalyticsTrackClick(
                campaignId: string,
                requestBody?: TrackClickDto,
            ): CancelablePromise<BooleanApiResponse> {
                return this.httpRequest.request({
                    method: 'POST',
                    url: '/api/v1/Analytics/{campaignId}/track-click',
                    path: {
                        'campaignId': campaignId,
                    },
                    body: requestBody,
                    mediaType: 'application/json',
                    errors: {
                        400: `Validation error (missing sessionId, page, etc.)`,
                        404: `Campaign not found with the specified ID`,
                    },
                });
            }
            /**
             * @param campaignId
             * @param fromDate
             * @param toDate
             * @returns CampaignStatsDtoApiResponse OK
             * @throws ApiError
             */
            public getApiV1AnalyticsStats(
                campaignId: string,
                fromDate?: string,
                toDate?: string,
            ): CancelablePromise<CampaignStatsDtoApiResponse> {
                return this.httpRequest.request({
                    method: 'GET',
                    url: '/api/v1/Analytics/{campaignId}/stats',
                    path: {
                        'campaignId': campaignId,
                    },
                    query: {
                        'fromDate': fromDate,
                        'toDate': toDate,
                    },
                    errors: {
                        400: `Bad Request`,
                        404: `Not Found`,
                    },
                });
            }
        }

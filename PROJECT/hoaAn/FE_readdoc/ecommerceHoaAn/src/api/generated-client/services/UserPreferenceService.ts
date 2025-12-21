/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { DisableRitualRequest } from '../models/DisableRitualRequest';
import type { DismissRitualRequest } from '../models/DismissRitualRequest';
import type { UserPreferenceResponse } from '../models/UserPreferenceResponse';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class UserPreferenceService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Records a dismissal for a ritual recommendation
     * Used when user temporarily hides a recommendation
     *
     * Requirements: 6.2
     * - Records dismissal for a user and ritual
     * - Reduces future recommendations of that type
     * - Allows user to dismiss without permanently disabling
     * @param requestBody Request containing ritual ID and optional reason
     * @returns UserPreferenceResponse Dismissal recorded successfully
     * @throws ApiError
     */
    public postApiV1UserPreferenceDismissRitual(
        requestBody?: DismissRitualRequest,
    ): CancelablePromise<UserPreferenceResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/UserPreference/dismiss-ritual',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Invalid request or missing required fields`,
                401: `Unauthorized (user not authenticated)`,
                500: `Server error during dismissal recording`,
            },
        });
    }
    /**
     * Disables a ritual for a user in the current session
     * Used when user explicitly indicates they're not interested in a ritual
     *
     * Requirements: 6.3
     * - Disables pattern detection for that ritual in the current session
     * - Prevents recommendations for the disabled ritual
     * - Allows user to explicitly opt-out of ritual detection
     * @param requestBody Request containing ritual ID
     * @returns UserPreferenceResponse Ritual disabled successfully
     * @throws ApiError
     */
    public postApiV1UserPreferenceDisableRitual(
        requestBody?: DisableRitualRequest,
    ): CancelablePromise<UserPreferenceResponse> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/UserPreference/disable-ritual',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Invalid request or missing required fields`,
                401: `Unauthorized (user not authenticated)`,
                500: `Server error during ritual disabling`,
            },
        });
    }
}

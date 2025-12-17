/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * DTO for tracking campaign impression
 * Requirements: 7.1
 */
export type TrackImpressionDto = {
    /**
     * Session ID of the user viewing the campaign
     */
    sessionId?: string | null;
    /**
     * Page where the campaign was shown
     */
    page?: string | null;
    /**
     * Timestamp when the impression was recorded
     */
    recordedAt?: string | null;
};


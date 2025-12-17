/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * DTO for tracking campaign click
 * Requirements: 7.2
 */
export type TrackClickDto = {
    /**
     * Session ID of the user clicking the campaign
     */
    sessionId?: string | null;
    /**
     * Page where the campaign was clicked
     */
    page?: string | null;
    /**
     * Timestamp when the click was recorded
     */
    recordedAt?: string | null;
};


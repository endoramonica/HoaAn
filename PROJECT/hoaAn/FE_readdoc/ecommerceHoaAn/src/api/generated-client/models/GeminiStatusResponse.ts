/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * Response model for Gemini API status check
 */
export type GeminiStatusResponse = {
    /**
     * Whether the request was successful
     */
    success?: boolean;
    /**
     * Whether Gemini API is configured
     */
    isConfigured?: boolean;
    /**
     * Status message
     */
    message?: string | null;
};


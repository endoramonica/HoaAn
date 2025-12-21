/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * Standardized error response format with error codes for programmatic handling
 * Requirements: 8.1, 8.2, 8.3, 8.4, 8.5
 */
export type ErrorResponse = {
    /**
     * Indicates if the request was successful
     */
    success?: boolean;
    /**
     * Human-readable error message
     */
    message?: string | null;
    /**
     * Machine-readable error code for programmatic handling
     * Examples: INVALID_DATE_RANGE, NEGATIVE_BUDGET, CAMPAIGN_NOT_DRAFT, etc.
     */
    errorCode?: string | null;
    /**
     * Field-level validation errors
     * Key: field name, Value: array of error messages for that field
     */
    errors?: Record<string, Array<string>> | null;
    /**
     * HTTP status code
     */
    statusCode?: number;
    /**
     * Timestamp when the error occurred
     */
    timestamp?: string;
};


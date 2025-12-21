/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * DTO for tracking user actions in the ritual recommendation system
 */
export type ActionDto = {
    /**
     * Unique identifier for the action
     */
    id?: string;
    /**
     * User ID who performed the action
     */
    userId?: string;
    /**
     * Session ID for tracking actions within a session
     */
    sessionId?: string | null;
    /**
     * Type of action performed
     */
    type?: string | null;
    /**
     * Timestamp when the action was performed
     */
    timestamp?: string;
    /**
     * Optional product ID associated with the action
     */
    productId?: string | null;
    /**
     * Optional category ID associated with the action
     */
    categoryId?: string | null;
    /**
     * Additional metadata for the action
     */
    metadata?: Record<string, any> | null;
};


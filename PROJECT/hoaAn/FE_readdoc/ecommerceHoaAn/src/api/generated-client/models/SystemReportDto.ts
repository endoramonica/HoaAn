/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ActionTypeDto } from './ActionTypeDto';
/**
 * DTO for system report containing detailed matching logic
 */
export type SystemReportDto = {
    /**
     * The action sequence pattern that was matched
     */
    matchedPattern?: Array<ActionTypeDto> | null;
    /**
     * Step-by-step explanation of the matching process
     */
    matchingSteps?: Array<string> | null;
    /**
     * Reasons why each item is missing/recommended
     */
    reasonsForMissingItems?: Record<string, string> | null;
};


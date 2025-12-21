/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { MatchingMetadataDto } from './MatchingMetadataDto';
import type { SystemReportDto } from './SystemReportDto';
/**
 * DTO for BE-AI recommendation output containing matched ritual and missing items
 */
export type RecommendationPayloadDto = {
    /**
     * ID of the detected ritual
     */
    ritualId?: string | null;
    /**
     * Name of the detected ritual
     */
    ritualName?: string | null;
    /**
     * Confidence score (0-1) for the recommendation
     */
    confidenceScore?: number;
    /**
     * Product IDs that are missing from the user's cart
     */
    missingItems?: Array<string> | null;
    matchingMetadata?: MatchingMetadataDto;
    systemReport?: SystemReportDto;
};


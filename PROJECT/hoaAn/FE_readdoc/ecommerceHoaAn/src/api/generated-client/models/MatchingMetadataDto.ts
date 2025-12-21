/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * DTO for metadata about the pattern matching process
 */
export type MatchingMetadataDto = {
    /**
     * Number of actions that matched the pattern
     */
    matchedSequenceLength?: number;
    /**
     * Total length of the action sequence analyzed
     */
    totalSequenceLength?: number;
    /**
     * Indices of matched actions in the original sequence
     */
    matchedActionIndices?: Array<number> | null;
};


/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { MixedFeedDto } from './MixedFeedDto';
/**
 * Result cho location-based posts
 */
export type LocationPostsDto = {
    location?: string | null;
    posts?: Array<MixedFeedDto> | null;
    totalCount?: number;
};


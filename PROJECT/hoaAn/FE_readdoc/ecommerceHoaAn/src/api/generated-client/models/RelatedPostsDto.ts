/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { MixedFeedDto } from './MixedFeedDto';
export type RelatedPostsDto = {
    productId?: string;
    productName?: string | null;
    marketingPosts?: Array<MixedFeedDto> | null;
    communityPosts?: Array<MixedFeedDto> | null;
    totalCount?: number;
};


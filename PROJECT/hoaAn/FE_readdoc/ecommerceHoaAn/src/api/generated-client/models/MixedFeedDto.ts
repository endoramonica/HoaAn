/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { TaggedProductDto } from './TaggedProductDto';
/**
 * DTO cho Mixed Feed - kết hợp Community Posts và Marketing Posts
 */
export type MixedFeedDto = {
    id?: string;
    postType?: string | null;
    content?: string | null;
    imageUrl?: string | null;
    imageUrls?: Array<string> | null;
    createdAt?: string;
    publishedDate?: string | null;
    customerId?: string | null;
    customerName?: string | null;
    customerAvatar?: string | null;
    title?: string | null;
    shortDescription?: string | null;
    image?: string | null;
    productId?: string | null;
    productName?: string | null;
    taggedProduct?: TaggedProductDto;
    platform?: string | null;
    hashtags?: Array<string> | null;
    priorityScore?: number | null;
    isFeatured?: boolean | null;
    status?: string | null;
    displayLocation?: Array<string> | null;
    likesCount?: number;
    commentsCount?: number;
    sharesCount?: number;
    viewsCount?: number;
    clicksCount?: number;
    isLikedByCurrentUser?: boolean;
    isBookmarkedByCurrentUser?: boolean;
    isOwnedByCurrentUser?: boolean;
};


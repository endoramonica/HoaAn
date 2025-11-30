/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CommentDtoPaginatedResult } from './CommentDtoPaginatedResult';
import type { CustomerDto } from './CustomerDto';
export type PostDetailDto = {
    postId?: string;
    customerId?: string;
    customerName?: string | null;
    customerAvatar?: string | null;
    content?: string | null;
    photoUrl?: string | null;
    thumbnailUrl?: string | null;
    postedOn?: string;
    createdAt?: string;
    updatedAt?: string | null;
    likesCount?: number;
    commentsCount?: number;
    bookmarksCount?: number;
    isLikedByCurrentUser?: boolean;
    isBookmarkedByCurrentUser?: boolean;
    isOwnedByCurrentUser?: boolean;
    comments?: CommentDtoPaginatedResult;
    recentLikes?: Array<CustomerDto> | null;
};


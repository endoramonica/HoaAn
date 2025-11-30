/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CommentPreviewDto } from './CommentPreviewDto';
export type PostFeedDto = {
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
    latestComments?: Array<CommentPreviewDto> | null;
};


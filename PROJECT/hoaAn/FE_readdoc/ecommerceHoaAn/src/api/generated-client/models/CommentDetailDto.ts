/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CommentDto } from './CommentDto';
/**
 * DTO chi tiết với replies
 */
export type CommentDetailDto = {
    commentId?: string;
    postId?: string;
    customerId?: string;
    customerName?: string | null;
    customerAvatar?: string | null;
    content?: string | null;
    addedOn?: string;
    parentCommentId?: string | null;
    repliesCount?: number;
    isOwnedByCurrentUser?: boolean;
    replies?: Array<CommentDto> | null;
};


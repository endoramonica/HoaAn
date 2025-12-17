/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * DTO response cơ bản cho comment
 */
export type CommentDto = {
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
};


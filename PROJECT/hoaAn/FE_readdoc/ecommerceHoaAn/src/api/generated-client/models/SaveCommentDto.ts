/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * DTO để tạo/update comment (Save operation)
 * Nếu CommentId = Guid.Empty → Tạo mới
 * Nếu CommentId có giá trị → Update
 */
export type SaveCommentDto = {
    commentId?: string;
    postId?: string;
    content?: string | null;
    parentCommentId?: string | null;
};


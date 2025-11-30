/**
 * React Query hooks for Comments API
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getVietCommerceAPI } from '../../../../Api/generated-orval';
import type {
    GetApiV1CommentsPostPostIdParams,
    CreateCommentDto,
} from '../../../../Api/generated-orval/schemas';

const api = getVietCommerceAPI();

// Query Keys
export const commentsKeys = {
    all: ['comments'] as const,
    post: (postId: string) => [...commentsKeys.all, 'post', postId] as const,
    postPaginated: (postId: string, page: number) => [...commentsKeys.post(postId), page] as const,
    replies: (commentId: string) => [...commentsKeys.all, 'replies', commentId] as const,
};

// Fetch Comments for a Post
export const usePostComments = (postId: string, page: number = 1, pageSize: number = 20) => {
    return useQuery({
        queryKey: commentsKeys.postPaginated(postId, page),
        queryFn: async () => {
            const params: GetApiV1CommentsPostPostIdParams = { pageNumber: page, pageSize };

            const data = await api.getApiV1CommentsPostPostId(postId, params);
            console.log(">>> Payload gửi lên:", data);

            return data;
        },
        staleTime: 1000 * 60 * 2, // 2 minutes
    });
};

// Fetch Replies for a Comment
export const useCommentReplies = (commentId: string) => {
    return useQuery({
        queryKey: commentsKeys.replies(commentId),
        queryFn: async () => {
            const data = await api.getApiV1CommentsCommentIdReplies(commentId);
            return data;
        },
        enabled: !!commentId,
        staleTime: 1000 * 60 * 2, // 2 minutes
    });
};

// Create Comment
export const useCreateComment = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (data: CreateCommentDto) => {

            // 🚀 LOG PAYLOAD GỬI LÊN API
            console.log(">>> Payload gửi từ FE lên API:", data);

            const result = await api.postApiV1Comments(data);
            return result;
        },
        onSuccess: (_, variables) => {
            if (variables.postId) {
                queryClient.invalidateQueries({ queryKey: commentsKeys.post(variables.postId) });
            }
            if (variables.parentCommentId) {
                queryClient.invalidateQueries({ queryKey: commentsKeys.replies(variables.parentCommentId) });
            }
        },
    });
};


// Update Comment
export const useUpdateComment = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({ commentId, data }: { commentId: string; data: any }) => {
            const result = await api.putApiV1CommentsCommentId(commentId, data);
            return result;
        },
        onSuccess: () => {
            // Invalidate all comments
            queryClient.invalidateQueries({ queryKey: commentsKeys.all });
        },
    });
};

// Delete Comment
export const useDeleteComment = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (commentId: string) => {
            const result = await api.deleteApiV1CommentsCommentId(commentId);
            return result;
        },
        onSuccess: () => {
            // Invalidate all comments
            queryClient.invalidateQueries({ queryKey: commentsKeys.all });
        },
    });
};

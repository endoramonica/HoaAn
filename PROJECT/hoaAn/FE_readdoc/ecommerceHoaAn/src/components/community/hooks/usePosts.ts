/**
 * React Query hooks for Posts API
 * Updated to use MixedFeed endpoint for feed data
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getVietCommerceAPI } from '../../../../Api/generated-orval';
import type {
    GetApiV1MixedFeedParams,
    GetApiV1PostsSearchParams,
    PostApiV1PostsBody,
} from '../../../../Api/generated-orval/schemas';

const api = getVietCommerceAPI();

// Query Keys
export const postsKeys = {
    all: ['posts'] as const,
    feeds: () => [...postsKeys.all, 'feed'] as const,
    feed: (page: number) => [...postsKeys.feeds(), page] as const,
    searches: () => [...postsKeys.all, 'search'] as const,
    search: (keyword: string, page: number) => [...postsKeys.searches(), keyword, page] as const,
};

// Fetch Posts Feed using MixedFeed endpoint
export const usePostsFeed = (page: number = 1, pageSize: number = 20) => {
    return useQuery({
        queryKey: postsKeys.feed(page),
        queryFn: async () => {
            const params: GetApiV1MixedFeedParams = { PageNumber: page, PageSize: pageSize };
            const data = await api.getApiV1MixedFeed(params);
            return data;
        },
        staleTime: 1000 * 60 * 5, // 5 minutes
    });
};

// Search Posts
export const useSearchPosts = (keyword: string, page: number = 1, pageSize: number = 20) => {
    return useQuery({
        queryKey: postsKeys.search(keyword, page),
        queryFn: async () => {
            const params: GetApiV1PostsSearchParams = {
                keyword,
                pageNumber: page,
                pageSize,
            };
            const data = await api.getApiV1PostsSearch(params);
            return data;
        },
        enabled: !!keyword.trim(),
        staleTime: 1000 * 60 * 2, // 2 minutes
    });
};

// Create Post
export const useCreatePost = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (data: PostApiV1PostsBody) => {
            const result = await api.postApiV1Posts(data);
            return result;
        },
        onSuccess: () => {
            // Invalidate all feed queries to refresh
            queryClient.invalidateQueries({ queryKey: postsKeys.feeds() });
        },
    });
};

// Helper to match post by id (supports both PostResponseDto.postId and MixedFeedDto.id)
const matchPostId = (post: any, targetId: string): boolean => {
    return post.postId === targetId || post.id === targetId;
};

// Like Post with Optimistic Updates
export const useLikePost = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (postId: string) => {
            const result = await api.postApiV1PostsPostIdLike(postId);
            return result;
        },
        onMutate: async (postId: string) => {
            // Cancel outgoing refetches
            await queryClient.cancelQueries({ queryKey: postsKeys.all });

            // Snapshot previous values
            const previousData: any[] = [];

            // Update all feed pages (supports both PostResponseDto and MixedFeedDto)
            queryClient.setQueriesData(
                { queryKey: postsKeys.feeds() },
                (old: any) => {
                    if (!old?.data?.items) return old;

                    previousData.push({ key: postsKeys.feeds(), data: old });

                    return {
                        ...old,
                        data: {
                            ...old.data,
                            items: old.data.items.map((post: any) =>
                                matchPostId(post, postId)
                                    ? {
                                        ...post,
                                        isLikedByCurrentUser: !post.isLikedByCurrentUser,
                                        likesCount: post.isLikedByCurrentUser
                                            ? (post.likesCount || 1) - 1
                                            : (post.likesCount || 0) + 1,
                                    }
                                    : post
                            ),
                        },
                    };
                }
            );

            // Update search results
            queryClient.setQueriesData(
                { queryKey: postsKeys.searches() },
                (old: any) => {
                    if (!old?.data?.items) return old;

                    previousData.push({ key: postsKeys.searches(), data: old });

                    return {
                        ...old,
                        data: {
                            ...old.data,
                            items: old.data.items.map((post: any) =>
                                matchPostId(post, postId)
                                    ? {
                                        ...post,
                                        isLikedByCurrentUser: !post.isLikedByCurrentUser,
                                        likesCount: post.isLikedByCurrentUser
                                            ? (post.likesCount || 1) - 1
                                            : (post.likesCount || 0) + 1,
                                    }
                                    : post
                            ),
                        },
                    };
                }
            );

            return { previousData };
        },
        onError: (err, postId, context: any) => {
            // Rollback on error
            if (context?.previousData) {
                context.previousData.forEach(({ key, data }: any) => {
                    queryClient.setQueryData(key, data);
                });
            }
        },
        onSettled: () => {
            // Optional: Refetch in background to sync with server
            queryClient.invalidateQueries({ queryKey: postsKeys.all });
        },
    });
};

// Bookmark Post with Optimistic Updates
export const useBookmarkPost = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (postId: string) => {
            const result = await api.postApiV1PostsPostIdBookmark(postId);
            return result;
        },
        onMutate: async (postId: string) => {
            // Cancel outgoing refetches
            await queryClient.cancelQueries({ queryKey: postsKeys.all });

            // Snapshot previous values
            const previousData: any[] = [];

            // Update all feed pages (supports both PostResponseDto and MixedFeedDto)
            queryClient.setQueriesData(
                { queryKey: postsKeys.feeds() },
                (old: any) => {
                    if (!old?.data?.items) return old;

                    previousData.push({ key: postsKeys.feeds(), data: old });

                    return {
                        ...old,
                        data: {
                            ...old.data,
                            items: old.data.items.map((post: any) =>
                                matchPostId(post, postId)
                                    ? {
                                        ...post,
                                        isBookmarkedByCurrentUser: !post.isBookmarkedByCurrentUser,
                                    }
                                    : post
                            ),
                        },
                    };
                }
            );

            // Update search results
            queryClient.setQueriesData(
                { queryKey: postsKeys.searches() },
                (old: any) => {
                    if (!old?.data?.items) return old;

                    previousData.push({ key: postsKeys.searches(), data: old });

                    return {
                        ...old,
                        data: {
                            ...old.data,
                            items: old.data.items.map((post: any) =>
                                matchPostId(post, postId)
                                    ? {
                                        ...post,
                                        isBookmarkedByCurrentUser: !post.isBookmarkedByCurrentUser,
                                    }
                                    : post
                            ),
                        },
                    };
                }
            );

            return { previousData };
        },
        onError: (err, postId, context: any) => {
            // Rollback on error
            if (context?.previousData) {
                context.previousData.forEach(({ key, data }: any) => {
                    queryClient.setQueryData(key, data);
                });
            }
        },
        onSettled: () => {
            // Optional: Refetch in background to sync with server
            queryClient.invalidateQueries({ queryKey: postsKeys.all });
        },
    });
};

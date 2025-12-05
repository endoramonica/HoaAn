/**
 * Marketing Hooks
 * React Query hooks for marketing posts - now using real API
 */

import { useQueryClient } from '@tanstack/react-query';
import {
    useGetApiAdminMarketingPosts,
    useGetApiAdminMarketingPostsId,
    useGetApiAdminMarketingPostsByProductProductId,
    useGetApiAdminMarketingPostsStatistics,
    usePostApiAdminMarketingPosts,
    usePutApiAdminMarketingPostsId,
    useDeleteApiAdminMarketingPostsId,
    usePostApiAdminMarketingPostsIdPublish,
    usePostApiAdminMarketingPostsIdSchedule,
    usePostApiAdminMarketingPostsIdDuplicate,
    usePostApiAdminMarketingPostsIdAnalyticsViews,
    usePostApiAdminMarketingPostsIdAnalyticsClicks,
    usePostApiAdminMarketingPostsIdAnalyticsShares,
    getGetApiAdminMarketingPostsQueryKey,
    getGetApiAdminMarketingPostsIdQueryKey,
    getGetApiAdminMarketingPostsStatisticsQueryKey,
} from '../../../api/generated-orval/admin-marketing-post/admin-marketing-post';
import type {
    MarketingCreateMarketingPostDto,
    MarketingUpdateMarketingPostDto,
    EnumsMarketingMarketingPostStatus
} from '../../../api/generated-orval/schemas';

// Re-export query keys for external use
export const marketingKeys = {
    all: ['marketing'] as const,
    lists: () => getGetApiAdminMarketingPostsQueryKey(),
    list: (filters?: any) => getGetApiAdminMarketingPostsQueryKey(filters),
    details: () => [...marketingKeys.all, 'detail'] as const,
    detail: (id: string) => getGetApiAdminMarketingPostsIdQueryKey(id),
    statistics: () => getGetApiAdminMarketingPostsStatisticsQueryKey(),
};

/**
 * Get all marketing posts with optional filters
 */
export function useMarketingPosts(filters?: {
    status?: 'draft' | 'published' | 'scheduled';
    productId?: string;
    platform?: string;
    searchTerm?: string;
}) {
    // Map status to API enum
    const statusMap: Record<string, EnumsMarketingMarketingPostStatus> = {
        'draft': 'Draft' as EnumsMarketingMarketingPostStatus,
        'published': 'Published' as EnumsMarketingMarketingPostStatus,
        'scheduled': 'Scheduled' as EnumsMarketingMarketingPostStatus,
    };

    const apiParams = {
        Status: filters?.status ? statusMap[filters.status] : undefined,
        ProductId: filters?.productId,
        Platform: filters?.platform,
        SearchTerm: filters?.searchTerm,
        PageNumber: 1,
        PageSize: 100,
    };

    const query = useGetApiAdminMarketingPosts(apiParams);

    // Debug: Log the response structure
    console.log('Marketing Posts Query Response:', query.data);
    console.log('Full data structure:', query.data?.data);
    console.log('Items (old path):', query.data?.data?.items);
    console.log('Items (new path):', query.data?.data?.data?.items);

    // Transform data to match old interface
    // API returns: { data: { success, data: { items, pageNumber, ... } } }
    const items = query.data?.data?.data?.items || [];
    console.log('Extracted items:', items);

    return {
        ...query,
        data: items,
    };
}

/**
 * Get single marketing post
 */
export function useMarketingPost(id: string) {
    const query = useGetApiAdminMarketingPostsId(id, {
        query: {
            enabled: !!id && id !== 'new',
        }
    });

    return {
        ...query,
        data: query.data?.data,
    };
}

/**
 * Get posts by product
 */
export function useMarketingPostsByProduct(productId: string) {
    const query = useGetApiAdminMarketingPostsByProductProductId(productId, {
        query: {
            enabled: !!productId,
        }
    });

    return {
        ...query,
        data: query.data?.data || [],
    };
}

/**
 * Get marketing statistics
 */
export function useMarketingStatistics() {
    const query = useGetApiAdminMarketingPostsStatistics();

    // Debug: Log the response structure
    console.log('Statistics Query Response:', query.data);
    console.log('Statistics Data (old path):', query.data?.data);
    console.log('Statistics Data (new path):', query.data?.data?.data);

    // API returns: { data: { success, data: { total, draft, ... } } }
    const stats = query.data?.data?.data || query.data?.data;
    console.log('Extracted stats:', stats);

    return {
        ...query,
        data: stats,
    };
}

/**
 * Create new marketing post
 */
export function useCreateMarketingPost() {
    const queryClient = useQueryClient();
    const mutation = usePostApiAdminMarketingPosts();

    return {
        ...mutation,
        mutate: (data: MarketingCreateMarketingPostDto, options?: any) => {
            return mutation.mutate({ data }, {
                ...options,
                onSuccess: (result: any, variables: any, context: any) => {
                    queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                    options?.onSuccess?.(result, variables, context);
                },
            });
        },
        mutateAsync: (data: MarketingCreateMarketingPostDto) => {
            return mutation.mutateAsync({ data }).then((result) => {
                queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                return result;
            });
        },
    };
}

/**
 * Update marketing post
 */
export function useUpdateMarketingPost() {
    const queryClient = useQueryClient();
    const mutation = usePutApiAdminMarketingPostsId();

    return {
        ...mutation,
        mutate: ({ id, data }: { id: string; data: MarketingUpdateMarketingPostDto }, options?: any) => {
            return mutation.mutate({ id, data }, {
                ...options,
                onSuccess: (result: any, variables: any, context: any) => {
                    queryClient.invalidateQueries({ queryKey: marketingKeys.detail(id) });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                    options?.onSuccess?.(result, variables, context);
                },
            });
        },
        mutateAsync: ({ id, data }: { id: string; data: MarketingUpdateMarketingPostDto }) => {
            return mutation.mutateAsync({ id, data }).then((result) => {
                queryClient.invalidateQueries({ queryKey: marketingKeys.detail(id) });
                queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                return result;
            });
        },
    };
}

/**
 * Delete marketing post
 */
export function useDeleteMarketingPost() {
    const queryClient = useQueryClient();
    const mutation = useDeleteApiAdminMarketingPostsId();

    return {
        ...mutation,
        mutate: (id: string, options?: any) => {
            return mutation.mutate({ id }, {
                ...options,
                onSuccess: (result: any, variables: any, context: any) => {
                    queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                    options?.onSuccess?.(result, variables, context);
                },
            });
        },
        mutateAsync: (id: string) => {
            return mutation.mutateAsync({ id }).then((result) => {
                queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                return result;
            });
        },
    };
}

/**
 * Publish marketing post
 */
export function usePublishMarketingPost() {
    const queryClient = useQueryClient();
    const mutation = usePostApiAdminMarketingPostsIdPublish();

    return {
        ...mutation,
        mutate: (id: string, options?: any) => {
            return mutation.mutate({ id }, {
                ...options,
                onSuccess: (result: any, variables: any, context: any) => {
                    queryClient.invalidateQueries({ queryKey: marketingKeys.detail(id) });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                    options?.onSuccess?.(result, variables, context);
                },
            });
        },
        mutateAsync: (id: string) => {
            return mutation.mutateAsync({ id }).then((result) => {
                queryClient.invalidateQueries({ queryKey: marketingKeys.detail(id) });
                queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                return result;
            });
        },
    };
}

/**
 * Schedule marketing post
 */
export function useScheduleMarketingPost() {
    const queryClient = useQueryClient();
    const mutation = usePostApiAdminMarketingPostsIdSchedule();

    return {
        ...mutation,
        mutate: ({ id, scheduledDate }: { id: string; scheduledDate: string }, options?: any) => {
            return mutation.mutate({
                id,
                data: { scheduledPublishDate: scheduledDate }
            }, {
                ...options,
                onSuccess: (result: any, variables: any, context: any) => {
                    queryClient.invalidateQueries({ queryKey: marketingKeys.detail(id) });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                    options?.onSuccess?.(result, variables, context);
                },
            });
        },
        mutateAsync: ({ id, scheduledDate }: { id: string; scheduledDate: string }) => {
            return mutation.mutateAsync({
                id,
                data: { scheduledPublishDate: scheduledDate }
            }).then((result) => {
                queryClient.invalidateQueries({ queryKey: marketingKeys.detail(id) });
                queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                return result;
            });
        },
    };
}

/**
 * Duplicate marketing post
 */
export function useDuplicateMarketingPost() {
    const queryClient = useQueryClient();
    const mutation = usePostApiAdminMarketingPostsIdDuplicate();

    return {
        ...mutation,
        mutate: (id: string, options?: any) => {
            return mutation.mutate({ id }, {
                ...options,
                onSuccess: (result: any, variables: any, context: any) => {
                    queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                    options?.onSuccess?.(result, variables, context);
                },
            });
        },
        mutateAsync: (id: string) => {
            return mutation.mutateAsync({ id }).then((result) => {
                queryClient.invalidateQueries({ queryKey: marketingKeys.lists() });
                queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                return result;
            });
        },
    };
}

/**
 * Increment post views
 */
export function useIncrementViews() {
    const queryClient = useQueryClient();
    const mutation = usePostApiAdminMarketingPostsIdAnalyticsViews();

    return {
        ...mutation,
        mutate: (id: string, options?: any) => {
            return mutation.mutate({ id }, {
                ...options,
                onSuccess: (result: any, variables: any, context: any) => {
                    queryClient.invalidateQueries({ queryKey: marketingKeys.detail(id) });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                    options?.onSuccess?.(result, variables, context);
                },
            });
        },
    };
}

/**
 * Increment post clicks
 */
export function useIncrementClicks() {
    const queryClient = useQueryClient();
    const mutation = usePostApiAdminMarketingPostsIdAnalyticsClicks();

    return {
        ...mutation,
        mutate: (id: string, options?: any) => {
            return mutation.mutate({ id }, {
                ...options,
                onSuccess: (result: any, variables: any, context: any) => {
                    queryClient.invalidateQueries({ queryKey: marketingKeys.detail(id) });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                    options?.onSuccess?.(result, variables, context);
                },
            });
        },
    };
}

/**
 * Increment post shares
 */
export function useIncrementShares() {
    const queryClient = useQueryClient();
    const mutation = usePostApiAdminMarketingPostsIdAnalyticsShares();

    return {
        ...mutation,
        mutate: (id: string, options?: any) => {
            return mutation.mutate({ id }, {
                ...options,
                onSuccess: (result: any, variables: any, context: any) => {
                    queryClient.invalidateQueries({ queryKey: marketingKeys.detail(id) });
                    queryClient.invalidateQueries({ queryKey: marketingKeys.statistics() });
                    options?.onSuccess?.(result, variables, context);
                },
            });
        },
    };
}

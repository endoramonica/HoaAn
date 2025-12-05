/**
 * Category Hooks
 * 
 * React Query hooks for category management.
 */

import { useMutation, useQuery, useQueryClient, type UseQueryOptions } from '@tanstack/react-query';
import type { AxiosError } from 'axios';
import { categoryService } from '../services/categoryService';

/**
 * Query keys for category-related queries
 */
export const categoryKeys = {
    all: ['categories'] as const,
    lists: () => [...categoryKeys.all, 'list'] as const,
    list: (params?: any) => [...categoryKeys.lists(), params] as const,
    details: () => [...categoryKeys.all, 'detail'] as const,
    detail: (id: string) => [...categoryKeys.details(), id] as const,
    hierarchy: (storeId: string) => [...categoryKeys.all, 'hierarchy', storeId] as const,
    subcategories: (parentId: string) => [...categoryKeys.all, 'subcategories', parentId] as const,
};

/**
 * Hook to fetch categories list
 */
export function useCategories(
    params?: {
        storeId?: string;
        isActive?: boolean;
        pageNumber?: number;
        pageSize?: number;
        searchTerm?: string;
    },
    options?: UseQueryOptions<any, AxiosError>
) {
    return useQuery({
        queryKey: categoryKeys.list(params),
        queryFn: () => categoryService.getCategories(params),
        ...options,
    });
}

/**
 * Hook to fetch single category by ID
 */
export function useCategory(
    id: string,
    options?: UseQueryOptions<any, AxiosError>
) {
    return useQuery({
        queryKey: categoryKeys.detail(id),
        queryFn: () => categoryService.getCategoryById(id),
        enabled: !!id,
        ...options,
    });
}

/**
 * Hook to fetch category hierarchy
 */
export function useCategoryHierarchy(
    storeId: string,
    options?: UseQueryOptions<any, AxiosError>
) {
    return useQuery({
        queryKey: categoryKeys.hierarchy(storeId),
        queryFn: () => categoryService.getCategoryHierarchy(storeId),
        enabled: !!storeId,
        ...options,
    });
}

/**
 * Hook to fetch subcategories
 */
export function useSubcategories(
    parentId: string,
    options?: UseQueryOptions<any, AxiosError>
) {
    return useQuery({
        queryKey: categoryKeys.subcategories(parentId),
        queryFn: () => categoryService.getSubcategories(parentId),
        enabled: !!parentId,
        ...options,
    });
}

/**
 * Hook to create new category
 */
export function useCreateCategory() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: any) => categoryService.createCategory(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: categoryKeys.lists() });
        },
    });
}

/**
 * Hook to update category
 */
export function useUpdateCategory() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: any }) =>
            categoryService.updateCategory(id, data),
        onSuccess: (_, variables) => {
            queryClient.invalidateQueries({ queryKey: categoryKeys.detail(variables.id) });
            queryClient.invalidateQueries({ queryKey: categoryKeys.lists() });
        },
    });
}

/**
 * Hook to delete category
 */
export function useDeleteCategory() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (id: string) => categoryService.deleteCategory(id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: categoryKeys.lists() });
        },
    });
}

/**
 * Product Hooks
 * 
 * Re-exports and enhances Orval-generated React Query hooks for product management.
 */

import { useQueryClient } from '@tanstack/react-query';
import {
    useGetApiProduct,
    useGetApiProductId,
    usePostApiProduct,
    usePutApiProductId,
    useDeleteApiProductId,
    usePatchApiProductIdStock,
    usePatchApiProductIdActive,
    usePatchApiProductIdFeatured,
    useGetApiProductStoreStoreId,
    useGetApiProductCategoryCategoryId,
    getGetApiProductQueryKey,
    getGetApiProductIdQueryKey,
} from '../../../api/generated-orval/product/product';

// Re-export generated hooks with simpler names
export const useProducts = useGetApiProduct;
export const useProduct = useGetApiProductId;
export const useProductsByStore = useGetApiProductStoreStoreId;
export const useProductsByCategory = useGetApiProductCategoryCategoryId;

/**
 * Hook to create new product with automatic cache invalidation
 */
export function useCreateProduct() {
    const queryClient = useQueryClient();

    return usePostApiProduct({
        mutation: {
            onSuccess: () => {
                queryClient.invalidateQueries({
                    queryKey: getGetApiProductQueryKey()
                });
            },
        },
    });
}

/**
 * Hook to update product with automatic cache invalidation
 */
export function useUpdateProduct() {
    const queryClient = useQueryClient();

    return usePutApiProductId({
        mutation: {
            onSuccess: (_, variables) => {
                queryClient.invalidateQueries({
                    queryKey: getGetApiProductIdQueryKey(variables.id)
                });
                queryClient.invalidateQueries({
                    queryKey: getGetApiProductQueryKey()
                });
            },
        },
    });
}

/**
 * Hook to delete product with automatic cache invalidation
 */
export function useDeleteProduct() {
    const queryClient = useQueryClient();

    return useDeleteApiProductId({
        mutation: {
            onSuccess: () => {
                queryClient.invalidateQueries({
                    queryKey: getGetApiProductQueryKey()
                });
            },
        },
    });
}

/**
 * Hook to update product stock quantity
 */
export function useUpdateProductStock() {
    const queryClient = useQueryClient();

    return usePatchApiProductIdStock({
        mutation: {
            onSuccess: (_, variables) => {
                queryClient.invalidateQueries({
                    queryKey: getGetApiProductIdQueryKey(variables.id)
                });
                queryClient.invalidateQueries({
                    queryKey: getGetApiProductQueryKey()
                });
            },
        },
    });
}

/**
 * Hook to toggle product active status
 */
export function useToggleProductActive() {
    const queryClient = useQueryClient();

    return usePatchApiProductIdActive({
        mutation: {
            onSuccess: (_, variables) => {
                queryClient.invalidateQueries({
                    queryKey: getGetApiProductIdQueryKey(variables.id)
                });
                queryClient.invalidateQueries({
                    queryKey: getGetApiProductQueryKey()
                });
            },
        },
    });
}

/**
 * Hook to toggle product featured status
 */
export function useToggleProductFeatured() {
    const queryClient = useQueryClient();

    return usePatchApiProductIdFeatured({
        mutation: {
            onSuccess: (_, variables) => {
                queryClient.invalidateQueries({
                    queryKey: getGetApiProductIdQueryKey(variables.id)
                });
                queryClient.invalidateQueries({
                    queryKey: getGetApiProductQueryKey()
                });
            },
        },
    });
}

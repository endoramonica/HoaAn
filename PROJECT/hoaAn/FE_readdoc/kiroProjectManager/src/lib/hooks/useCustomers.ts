/**
 * Customer Hooks
 * 
 * Re-exports and enhances Orval-generated React Query hooks for customer management.
 * Provides simplified API and additional business logic.
 */

import { useQueryClient } from '@tanstack/react-query';
import {
    useGetApiAdminCustomer,
    useGetApiAdminCustomerId,
    useGetApiAdminCustomerIdStatistics,
    useGetApiAdminCustomerSearch,
    usePostApiAdminCustomer,
    usePutApiAdminCustomerId,
    useDeleteApiAdminCustomerId,
    getGetApiAdminCustomerQueryKey,
    getGetApiAdminCustomerIdQueryKey,
} from '../../../api/generated-orval/admin-customer/admin-customer';

// Re-export generated hooks with simpler names
export const useCustomers = useGetApiAdminCustomer;
export const useCustomer = useGetApiAdminCustomerId;
export const useCustomerStatistics = useGetApiAdminCustomerIdStatistics;
export const useCustomerSearch = useGetApiAdminCustomerSearch;

/**
 * Hook to create new customer with automatic cache invalidation
 */
export function useCreateCustomer() {
    const queryClient = useQueryClient();

    return usePostApiAdminCustomer({
        mutation: {
            onSuccess: () => {
                // Invalidate customers list to refetch
                queryClient.invalidateQueries({
                    queryKey: getGetApiAdminCustomerQueryKey()
                });
            },
        },
    });
}

/**
 * Hook to update customer with automatic cache invalidation
 */
export function useUpdateCustomer() {
    const queryClient = useQueryClient();

    return usePutApiAdminCustomerId({
        mutation: {
            onSuccess: (_, variables) => {
                // Invalidate specific customer and list
                queryClient.invalidateQueries({
                    queryKey: getGetApiAdminCustomerIdQueryKey(variables.id)
                });
                queryClient.invalidateQueries({
                    queryKey: getGetApiAdminCustomerQueryKey()
                });
            },
        },
    });
}

/**
 * Hook to delete customer with automatic cache invalidation
 */
export function useDeleteCustomer() {
    const queryClient = useQueryClient();

    return useDeleteApiAdminCustomerId({
        mutation: {
            onSuccess: () => {
                // Invalidate customers list to refetch
                queryClient.invalidateQueries({
                    queryKey: getGetApiAdminCustomerQueryKey()
                });
            },
        },
    });
}

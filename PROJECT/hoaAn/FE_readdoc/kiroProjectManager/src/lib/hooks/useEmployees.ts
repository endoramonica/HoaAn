/**
 * Employee Hooks
 * 
 * Re-exports and enhances Orval-generated React Query hooks for employee management.
 */

import { useQueryClient } from '@tanstack/react-query';
import {
    useGetApiEmployees,
    useGetApiEmployeesUserId,
    usePostApiEmployees,
    usePutApiEmployeesUserId,
    useDeleteApiEmployeesUserId,
    getGetApiEmployeesQueryKey,
    getGetApiEmployeesUserIdQueryKey,
} from '../../../api/generated-orval/employees/employees';

// Re-export generated hooks with simpler names
export const useEmployees = useGetApiEmployees;
export const useEmployee = useGetApiEmployeesUserId;

/**
 * Hook to create new employee with automatic cache invalidation
 */
export function useCreateEmployee() {
    const queryClient = useQueryClient();

    return usePostApiEmployees({
        mutation: {
            onSuccess: () => {
                queryClient.invalidateQueries({
                    queryKey: getGetApiEmployeesQueryKey()
                });
            },
        },
    });
}

/**
 * Hook to update employee with automatic cache invalidation
 */
export function useUpdateEmployee() {
    const queryClient = useQueryClient();

    return usePutApiEmployeesUserId({
        mutation: {
            onSuccess: (_, variables) => {
                queryClient.invalidateQueries({
                    queryKey: getGetApiEmployeesUserIdQueryKey(variables.userId)
                });
                queryClient.invalidateQueries({
                    queryKey: getGetApiEmployeesQueryKey()
                });
            },
        },
    });
}

/**
 * Hook to delete employee with automatic cache invalidation
 */
export function useDeleteEmployee() {
    const queryClient = useQueryClient();

    return useDeleteApiEmployeesUserId({
        mutation: {
            onSuccess: () => {
                queryClient.invalidateQueries({
                    queryKey: getGetApiEmployeesQueryKey()
                });
            },
        },
    });
}

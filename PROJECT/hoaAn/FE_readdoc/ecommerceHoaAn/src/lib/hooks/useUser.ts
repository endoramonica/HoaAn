/**
 * Custom Hook cho User Management
 */

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  userService,
  UserDto,
  UserUpdateRequest,
  GetUsersParams,
} from '../services/userService';
import { toast } from 'sonner@2.0.3';

// ============================================================================
// Query Keys
// ============================================================================

export const userKeys = {
  all: ['users'] as const,
  lists: () => [...userKeys.all, 'list'] as const,
  list: (params?: GetUsersParams) => [...userKeys.lists(), params] as const,
  details: () => [...userKeys.all, 'detail'] as const,
  detail: (userId: string) => [...userKeys.details(), userId] as const,
};

// ============================================================================
// useUsers Hook - Lấy danh sách người dùng
// ============================================================================

export function useUsers(params?: GetUsersParams) {
  return useQuery({
    queryKey: userKeys.list(params),
    queryFn: () => userService.getUsers(params),
    staleTime: 60000, // 1 minute
  });
}

// ============================================================================
// useUser Hook - Lấy thông tin user theo ID
// ============================================================================

export function useUser(userId: string) {
  return useQuery({
    queryKey: userKeys.detail(userId),
    queryFn: () => userService.getUserById(userId),
    enabled: !!userId,
    staleTime: 60000,
  });
}

// ============================================================================
// useUpdateUser Hook - Cập nhật thông tin user
// ============================================================================

export function useUpdateUser() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ userId, data }: { userId: string; data: UserUpdateRequest }) =>
      userService.updateUser(userId, data),
    onMutate: async ({ userId, data }) => {
      // Cancel any outgoing refetches
      await queryClient.cancelQueries({ queryKey: userKeys.detail(userId) });

      // Snapshot the previous value
      const previousUser = queryClient.getQueryData<UserDto>(userKeys.detail(userId));

      // Optimistically update to the new value
      if (previousUser) {
        queryClient.setQueryData<UserDto>(userKeys.detail(userId), {
          ...previousUser,
          ...data,
        });
      }

      return { previousUser };
    },
    onSuccess: (data, variables) => {
      queryClient.setQueryData(userKeys.detail(variables.userId), data);
      queryClient.invalidateQueries({ queryKey: userKeys.lists() });
      toast.success('Đã cập nhật thông tin người dùng');
    },
    onError: (error: any, variables, context) => {
      // Rollback on error
      if (context?.previousUser) {
        queryClient.setQueryData(userKeys.detail(variables.userId), context.previousUser);
      }
      toast.error(error?.message || 'Không thể cập nhật thông tin người dùng');
    },
  });
}

// ============================================================================
// useDeactivateUser Hook - Vô hiệu hóa user
// ============================================================================

export function useDeactivateUser() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (userId: string) => userService.deactivateUser(userId),
    onSuccess: (_, userId) => {
      queryClient.invalidateQueries({ queryKey: userKeys.detail(userId) });
      queryClient.invalidateQueries({ queryKey: userKeys.lists() });
      toast.success('Đã vô hiệu hóa người dùng');
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể vô hiệu hóa người dùng');
    },
  });
}

// ============================================================================
// useActivateUser Hook - Kích hoạt lại user
// ============================================================================

export function useActivateUser() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (userId: string) => userService.activateUser(userId),
    onSuccess: (_, userId) => {
      queryClient.invalidateQueries({ queryKey: userKeys.detail(userId) });
      queryClient.invalidateQueries({ queryKey: userKeys.lists() });
      toast.success('Đã kích hoạt lại người dùng');
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể kích hoạt người dùng');
    },
  });
}

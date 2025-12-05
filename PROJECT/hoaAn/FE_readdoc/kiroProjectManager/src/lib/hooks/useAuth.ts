/**
 * Auth Hook
 * Hook for authentication operations
 */

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { authService } from '../services/authService';

export const authKeys = {
    currentUser: ['auth', 'currentUser'] as const,
};

export function useLogin() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (credentials: { email: string; password: string; rememberMe?: boolean }) =>
            authService.login(credentials.email, credentials.password, credentials.rememberMe),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: authKeys.currentUser });
        },
    });
}

export function useLogout() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () => authService.logout(),
        onSuccess: () => {
            queryClient.clear();
        },
    });
}

export function useCurrentUser() {
    return useQuery({
        queryKey: authKeys.currentUser,
        queryFn: () => authService.getCurrentUser(),
        retry: false,
    });
}

/**
 * Order Hooks
 * React Query hooks for order management
 */

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { orderService } from '../services/orderService';

export const orderKeys = {
    all: ['orders'] as const,
    lists: () => [...orderKeys.all, 'list'] as const,
    list: (params?: any) => [...orderKeys.lists(), params] as const,
    details: () => [...orderKeys.all, 'detail'] as const,
    detail: (id: string) => [...orderKeys.details(), id] as const,
};

export function useOrders(params?: {
    PageNumber?: number;
    PageSize?: number;
    Status?: string;
}) {
    return useQuery({
        queryKey: orderKeys.list(params),
        queryFn: () => orderService.getOrders(params),
    });
}

export function useOrder(id: string) {
    return useQuery({
        queryKey: orderKeys.detail(id),
        queryFn: () => orderService.getOrderById(id),
        enabled: !!id,
    });
}

export function useCreateOrder() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: any) => orderService.createOrder(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: orderKeys.lists() });
        },
    });
}

export function useUpdateOrderStatus() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ id, status }: { id: string; status: string }) =>
            orderService.updateOrderStatus(id, status),
        onSuccess: (_, variables) => {
            queryClient.invalidateQueries({ queryKey: orderKeys.detail(variables.id) });
            queryClient.invalidateQueries({ queryKey: orderKeys.lists() });
        },
    });
}

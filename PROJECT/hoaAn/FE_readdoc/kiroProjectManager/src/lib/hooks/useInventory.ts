/**
 * Inventory Hooks
 * React Query hooks for inventory management
 */

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { inventoryService } from '../services/inventoryService';

export const inventoryKeys = {
    all: ['inventory'] as const,
    lists: () => [...inventoryKeys.all, 'list'] as const,
    list: (params?: any) => [...inventoryKeys.lists(), params] as const,
    details: () => [...inventoryKeys.all, 'detail'] as const,
    detail: (id: string) => [...inventoryKeys.details(), id] as const,
};

export function useInventory(params?: {
    PageNumber?: number;
    PageSize?: number;
    LowStock?: boolean;
}) {
    return useQuery({
        queryKey: inventoryKeys.list(params),
        queryFn: () => inventoryService.getInventory(params),
    });
}

export function useInventoryItem(id: string) {
    return useQuery({
        queryKey: inventoryKeys.detail(id),
        queryFn: () => inventoryService.getInventoryItem(id),
        enabled: !!id,
    });
}

export function useUpdateInventory() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: any }) =>
            inventoryService.updateInventory(id, data),
        onSuccess: (_, variables) => {
            queryClient.invalidateQueries({ queryKey: inventoryKeys.detail(variables.id) });
            queryClient.invalidateQueries({ queryKey: inventoryKeys.lists() });
        },
    });
}

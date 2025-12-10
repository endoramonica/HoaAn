/**
 * useServiceOrders Hook
 * Custom hook để quản lý service orders state
 * Wrapper around useOrders hook với type='service'
 */

import { useState, useEffect, useCallback } from 'react';
import { orderService } from '../services/orderService';
import { ServiceCategory, type ServiceOrderFilterOptions } from '../../types/service';
import type { OrderDto, PagedResponse } from '../api/types';
import { toast } from 'sonner@2.0.3';
import { ApiError } from '../api/errors';

interface UseServiceOrdersOptions {
    autoLoad?: boolean;
    initialParams?: ServiceOrderFilterOptions;
    myOrdersOnly?: boolean;
}

interface UseServiceOrdersReturn {
    serviceOrders: OrderDto[];
    pagedData: PagedResponse<OrderDto> | null;
    isLoading: boolean;
    isRefreshing: boolean;
    error: string | null;
    params: ServiceOrderFilterOptions;
    setParams: (params: ServiceOrderFilterOptions) => void;
    loadServiceOrders: (params?: ServiceOrderFilterOptions) => Promise<void>;
    getServiceOrderById: (id: string) => Promise<OrderDto | null>;
    setServiceCategory: (category: ServiceCategory | string) => void;
    setStatus: (status: string) => void;
    refresh: () => Promise<void>;
    goToPage: (page: number) => void;
    nextPage: () => void;
    previousPage: () => void;
}

export const useServiceOrders = (options?: UseServiceOrdersOptions): UseServiceOrdersReturn => {
    const { autoLoad = true, initialParams = {}, myOrdersOnly = false } = options || {};

    const [serviceOrders, setServiceOrders] = useState<OrderDto[]>([]);
    const [pagedData, setPagedData] = useState<PagedResponse<OrderDto> | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [isRefreshing, setIsRefreshing] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [params, setParams] = useState<ServiceOrderFilterOptions>({
        type: 'service',
        ...initialParams,
    });

    /**
     * Load service orders
     */
    const loadServiceOrders = useCallback(async (loadParams?: ServiceOrderFilterOptions) => {
        try {
            const currentParams = loadParams || params;
            setIsLoading(true);
            setError(null);

            const response = myOrdersOnly
                ? await orderService.getMyServiceOrders({
                    pageNumber: currentParams.page,
                    pageSize: currentParams.pageSize,
                    status: currentParams.status as any,
                    serviceCategory: currentParams.serviceCategory as string,
                })
                : await orderService.getServiceOrders({
                    pageNumber: currentParams.page,
                    pageSize: currentParams.pageSize,
                    status: currentParams.status as any,
                    serviceCategory: currentParams.serviceCategory as string,
                });

            setServiceOrders(response.items);
            setPagedData(response);
        } catch (err) {
            const errorMessage = err instanceof ApiError
                ? err.getDisplayMessage()
                : 'Không thể tải danh sách đơn hàng dịch vụ';

            setError(errorMessage);
            console.error('Load service orders error:', err);
        } finally {
            setIsLoading(false);
        }
    }, [params, myOrdersOnly]);

    /**
     * Get service order by ID
     */
    const getServiceOrderById = useCallback(async (id: string): Promise<OrderDto | null> => {
        try {
            const order = await orderService.getOrderById(id);
            return order;
        } catch (err) {
            const errorMessage = err instanceof ApiError
                ? err.getDisplayMessage()
                : 'Không thể tải thông tin đơn hàng dịch vụ';

            toast.error(errorMessage);
            return null;
        }
    }, []);

    /**
     * Set service category filter
     */
    const setServiceCategory = useCallback((category: ServiceCategory | string) => {
        setParams(prev => ({
            ...prev,
            serviceCategory: category,
            page: 1, // Reset to first page
        }));
    }, []);

    /**
     * Set status filter
     */
    const setStatus = useCallback((status: string) => {
        setParams(prev => ({
            ...prev,
            status,
            page: 1, // Reset to first page
        }));
    }, []);

    /**
     * Refresh data
     */
    const refresh = useCallback(async () => {
        try {
            setIsRefreshing(true);
            await loadServiceOrders();
        } finally {
            setIsRefreshing(false);
        }
    }, [loadServiceOrders]);

    /**
     * Pagination helpers
     */
    const goToPage = useCallback((page: number) => {
        setParams(prev => ({ ...prev, page }));
    }, []);

    const nextPage = useCallback(() => {
        if (pagedData?.hasNextPage) {
            setParams(prev => ({ ...prev, page: (prev.page || 1) + 1 }));
        }
    }, [pagedData]);

    const previousPage = useCallback(() => {
        if (pagedData?.hasPreviousPage) {
            setParams(prev => ({ ...prev, page: Math.max((prev.page || 1) - 1, 1) }));
        }
    }, [pagedData]);

    /**
     * Auto load on mount và khi params thay đổi
     */
    useEffect(() => {
        if (autoLoad) {
            loadServiceOrders(params);
        }
    }, [params.page, params.serviceCategory, params.status]);

    return {
        serviceOrders,
        pagedData,
        isLoading,
        isRefreshing,
        error,
        params,
        setParams,
        loadServiceOrders,
        getServiceOrderById,
        setServiceCategory,
        setStatus,
        refresh,
        goToPage,
        nextPage,
        previousPage,
    };
};

export default useServiceOrders;

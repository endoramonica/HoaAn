/**
 * useServices Hook
 * Custom hook để quản lý services state với pagination, filter, search
 * Wrapper around useProducts hook với type='service'
 */

import { useState, useEffect, useCallback } from 'react';
import { productService } from '../services/productService';
import { ServiceCategory, type ServiceFilterOptions } from '../../types/service';
import type { ProductDto, ProductFilterParams, PagedResponse } from '../api/types';
import { toast } from 'sonner@2.0.3';
import { ApiError } from '../api/errors';

interface UseServicesOptions {
    autoLoad?: boolean;
    initialParams?: ServiceFilterOptions;
}

interface UseServicesReturn {
    services: ProductDto[];
    pagedData: PagedResponse<ProductDto> | null;
    isLoading: boolean;
    isRefreshing: boolean;
    error: string | null;
    params: ServiceFilterOptions;
    setParams: (params: ServiceFilterOptions) => void;
    loadServices: (params?: ServiceFilterOptions) => Promise<void>;
    getServiceById: (id: string) => Promise<ProductDto | null>;
    setServiceCategory: (category: ServiceCategory | string) => void;
    setSearchQuery: (query: string) => void;
    refresh: () => Promise<void>;
    goToPage: (page: number) => void;
    nextPage: () => void;
    previousPage: () => void;
}

export const useServices = (options?: UseServicesOptions): UseServicesReturn => {
    const { autoLoad = true, initialParams = {} } = options || {};

    const [services, setServices] = useState<ProductDto[]>([]);
    const [pagedData, setPagedData] = useState<PagedResponse<ProductDto> | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [isRefreshing, setIsRefreshing] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);
    const [params, setParams] = useState<ServiceFilterOptions>({
        type: 'service',
        ...initialParams,
    });

    /**
     * Load services
     */
    const loadServices = useCallback(async (loadParams?: ServiceFilterOptions) => {
        try {
            const currentParams = loadParams || params;
            setIsLoading(true);
            setError(null);

            // Convert to ProductFilterParams
            const productParams: ProductFilterParams = {
                pageNumber: currentParams.page,
                pageSize: currentParams.pageSize,
                search: currentParams.searchTerm,
                categoryId: currentParams.serviceCategory as string,
                sortBy: currentParams.sortBy,
                sortOrder: currentParams.isDescending ? 'desc' : 'asc',
            };

            const response = await productService.getServices({
                ...productParams,
                serviceCategory: currentParams.serviceCategory as string,
            });

            setServices(response.items);
            setPagedData(response);
        } catch (err) {
            const errorMessage = err instanceof ApiError
                ? err.getDisplayMessage()
                : 'Không thể tải danh sách dịch vụ';

            setError(errorMessage);
            console.error('Load services error:', err);
        } finally {
            setIsLoading(false);
        }
    }, [params]);

    /**
     * Get service by ID
     */
    const getServiceById = useCallback(async (id: string): Promise<ProductDto | null> => {
        try {
            const service = await productService.getProductById(id);
            return service;
        } catch (err) {
            const errorMessage = err instanceof ApiError
                ? err.getDisplayMessage()
                : 'Không thể tải thông tin dịch vụ';

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
     * Set search query
     */
    const setSearchQuery = useCallback((query: string) => {
        setParams(prev => ({
            ...prev,
            searchTerm: query,
            page: 1, // Reset to first page
        }));
    }, []);

    /**
     * Refresh data
     */
    const refresh = useCallback(async () => {
        try {
            setIsRefreshing(true);
            await loadServices();
        } finally {
            setIsRefreshing(false);
        }
    }, [loadServices]);

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
            loadServices(params);
        }
    }, [params.page, params.serviceCategory, params.searchTerm, params.sortBy]);

    return {
        services,
        pagedData,
        isLoading,
        isRefreshing,
        error,
        params,
        setParams,
        loadServices,
        getServiceById,
        setServiceCategory,
        setSearchQuery,
        refresh,
        goToPage,
        nextPage,
        previousPage,
    };
};

export default useServices;

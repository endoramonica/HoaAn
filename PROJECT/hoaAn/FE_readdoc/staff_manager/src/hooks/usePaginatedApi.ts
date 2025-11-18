import { useState, useEffect, useCallback } from 'react';
import { PaginatedResponse, PaginationParams } from '../types';

interface UsePaginatedApiOptions<T> {
  fetchFn: (params: PaginationParams) => Promise<PaginatedResponse<T>>;
  initialPage?: number;
  initialPageSize?: number;
  initialSortBy?: string;
  initialSortOrder?: 'asc' | 'desc';
  initialFilters?: Record<string, any>;
  autoFetch?: boolean;
}

interface UsePaginatedApiReturn<T> {
  data: T[];
  meta: PaginatedResponse<T>['meta'] | null;
  isLoading: boolean;
  error: string | null;
  page: number;
  pageSize: number;
  sortBy?: string;
  sortOrder: 'asc' | 'desc';
  filters: Record<string, any>;
  setPage: (page: number) => void;
  setPageSize: (pageSize: number) => void;
  setSortBy: (sortBy?: string) => void;
  setSortOrder: (sortOrder: 'asc' | 'desc') => void;
  setFilters: (filters: Record<string, any> | ((prev: Record<string, any>) => Record<string, any>)) => void;
  refetch: () => Promise<void>;
  goToNextPage: () => void;
  goToPreviousPage: () => void;
  goToFirstPage: () => void;
  goToLastPage: () => void;
}

export function usePaginatedApi<T>({
  fetchFn,
  initialPage = 1,
  initialPageSize = 10,
  initialSortBy,
  initialSortOrder = 'desc',
  initialFilters = {},
  autoFetch = true,
}: UsePaginatedApiOptions<T>): UsePaginatedApiReturn<T> {
  const [data, setData] = useState<T[]>([]);
  const [meta, setMeta] = useState<PaginatedResponse<T>['meta'] | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  
  const [page, setPage] = useState(initialPage);
  const [pageSize, setPageSize] = useState(initialPageSize);
  const [sortBy, setSortBy] = useState<string | undefined>(initialSortBy);
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>(initialSortOrder);
  const [filters, setFilters] = useState<Record<string, any>>(initialFilters);

  const fetchData = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const params: PaginationParams = {
        page,
        pageSize,
        sortBy,
        sortOrder,
        filters,
      };

      const response = await fetchFn(params);
      setData(response.data);
      setMeta(response.meta);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
      setData([]);
      setMeta(null);
    } finally {
      setIsLoading(false);
    }
  }, [fetchFn, page, pageSize, sortBy, sortOrder, filters]);

  useEffect(() => {
    if (autoFetch) {
      fetchData();
    }
  }, [fetchData, autoFetch]);

  const handleSetPageSize = useCallback((newPageSize: number) => {
    setPageSize(newPageSize);
    setPage(1); // Reset to first page when changing page size
  }, []);

  const goToNextPage = useCallback(() => {
    if (meta && meta.hasNextPage) {
      setPage(prev => prev + 1);
    }
  }, [meta]);

  const goToPreviousPage = useCallback(() => {
    if (meta && meta.hasPreviousPage) {
      setPage(prev => prev - 1);
    }
  }, [meta]);

  const goToFirstPage = useCallback(() => {
    setPage(1);
  }, []);

  const goToLastPage = useCallback(() => {
    if (meta) {
      setPage(meta.totalPages);
    }
  }, [meta]);

  return {
    data,
    meta,
    isLoading,
    error,
    page,
    pageSize,
    sortBy,
    sortOrder,
    filters,
    setPage,
    setPageSize: handleSetPageSize,
    setSortBy,
    setSortOrder,
    setFilters,
    refetch: fetchData,
    goToNextPage,
    goToPreviousPage,
    goToFirstPage,
    goToLastPage,
  };
}

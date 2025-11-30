/**
 * useOrder Hook
 * ✅ Chuẩn hóa hoàn toàn theo Orval-generated API
 * ✅ Xử lý Order Management (search, filter, update status, stats)
 * ✅ Logging tối ưu cho debugging
 * ✅ Exception handling thống nhất
 */

import { useState, useCallback } from 'react';
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type {
    OrderDetailDto,
    OrderStatus,
    OrderStatusUpdateDTO,
    BulkUpdateStatusRequest,
    GetApiV1OrderMyOrdersParams,
    GetApiV1OrderParams,
    GetApiV1OrderSearchParams,
    GetApiV1OrderRecentParams,
    GetApiV1OrderOrderIdCanChangeStatusParams,
    GetApiV1OrderStatsCountParams,
    GetApiV1OrderStatsRevenueParams
} from '../../../Api/generated-orval/schemas';
import { toast } from 'sonner';

const api = getVietCommerceAPI();

// ============================================================================
// Types
// ============================================================================

interface UseOrderReturn {
    isLoading: boolean;
    error: string | null;

    // Order queries
    getOrderById: (id: string) => Promise<OrderDetailDto | null>;
    getMyOrders: (params?: GetApiV1OrderMyOrdersParams) => Promise<OrderDetailDto[]>;
    getAllOrders: (params?: GetApiV1OrderParams) => Promise<OrderDetailDto[]>;
    searchOrders: (params?: GetApiV1OrderSearchParams) => Promise<OrderDetailDto[]>;
    getOrdersByStatus: (status: OrderStatus) => Promise<OrderDetailDto[]>;
    getRecentOrders: (params?: GetApiV1OrderRecentParams) => Promise<OrderDetailDto[]>;

    // Order status
    getStatusHistory: (orderId: string) => Promise<any[]>;
    updateOrderStatus: (orderId: string, status: OrderStatus, notes?: string) => Promise<boolean>;
    canChangeStatus: (orderId: string, params?: GetApiV1OrderOrderIdCanChangeStatusParams) => Promise<boolean>;
    bulkUpdateStatus: (orderIds: string[], status: OrderStatus, notes?: string) => Promise<boolean>;

    // Order stats
    getOrderCount: (params?: GetApiV1OrderStatsCountParams) => Promise<number>;
    getRevenue: (params?: GetApiV1OrderStatsRevenueParams) => Promise<number>;
}

// ============================================================================
// Helper Functions
// ============================================================================

/**
 * Extract user-friendly error message from API error
 */
const extractErrorMessage = (err: any): string => {
    if (err?.response?.data?.message) return err.response.data.message;
    if (err?.response?.data?.error) return err.response.data.error;
    if (err?.response?.data?.title) return err.response.data.title;
    if (err?.message) return err.message;
    return 'Đã xảy ra lỗi không xác định';
};

/**
 * Log error details for debugging
 */
const logError = (context: string, err: any) => {
    console.error(`[useOrder] ❌ ${context}:`, {
        message: err?.message,
        status: err?.response?.status,
        statusText: err?.response?.statusText,
        data: err?.response?.data,
        error: err
    });
};

// ============================================================================
// Hook Implementation
// ============================================================================

/**
 * Hook quản lý Orders (Admin/Store management)
 * 
 * @example
 * ```tsx
 * const { getMyOrders, updateOrderStatus, isLoading } = useOrder();
 * 
 * // Get orders
 * const orders = await getMyOrders({ page: 1, pageSize: 10 });
 * 
 * // Update status
 * await updateOrderStatus(orderId, 'confirmed', 'Order confirmed by admin');
 * ```
 */
export const useOrder = (): UseOrderReturn => {
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // ============================================================================
    // Order Queries
    // ============================================================================

    /**
     * Get order by ID
     */
    const getOrderById = useCallback(async (
        id: string
    ): Promise<OrderDetailDto | null> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 📋 Fetching order:', id);

            const response = await api.getApiV1OrderId(id);

            if (!response.data) {
                throw new Error('No order data returned');
            }

            console.log('[useOrder] ✅ Order loaded');
            return response.data;

        } catch (err: any) {
            logError('Get order by ID failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);

            return null;
        } finally {
            setIsLoading(false);
        }
    }, []);

    /**
     * Get my orders (customer view)
     */
    const getMyOrders = useCallback(async (
        params?: GetApiV1OrderMyOrdersParams
    ): Promise<OrderDetailDto[]> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 📋 Fetching my orders:', params);

            const response = await api.getApiV1OrderMyOrders(params);

            if (!response.data) {
                throw new Error('No orders data returned');
            }

            console.log('[useOrder] ✅ My orders loaded:', response.data.length);
            return response.data;

        } catch (err: any) {
            logError('Get my orders failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);

            return [];
        } finally {
            setIsLoading(false);
        }
    }, []);

    /**
     * Get all orders (admin view)
     */
    const getAllOrders = useCallback(async (
        params?: GetApiV1OrderParams
    ): Promise<OrderDetailDto[]> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 📋 Fetching all orders:', params);

            const response = await api.getApiV1Order(params);

            if (!response.data) {
                throw new Error('No orders data returned');
            }

            console.log('[useOrder] ✅ All orders loaded:', response.data.length);
            return response.data;

        } catch (err: any) {
            logError('Get all orders failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);

            return [];
        } finally {
            setIsLoading(false);
        }
    }, []);

    /**
     * Search orders
     */
    const searchOrders = useCallback(async (
        params?: GetApiV1OrderSearchParams
    ): Promise<OrderDetailDto[]> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 🔍 Searching orders:', params);

            const response = await api.getApiV1OrderSearch(params);

            if (!response.data) {
                throw new Error('No search results returned');
            }

            console.log('[useOrder] ✅ Search results:', response.data.length);
            return response.data;

        } catch (err: any) {
            logError('Search orders failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);

            return [];
        } finally {
            setIsLoading(false);
        }
    }, []);

    /**
     * Get orders by status
     */
    const getOrdersByStatus = useCallback(async (
        status: OrderStatus
    ): Promise<OrderDetailDto[]> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 📋 Fetching orders by status:', status);

            const response = await api.getApiV1OrderStatusStatus(status);

            if (!response.data) {
                throw new Error('No orders data returned');
            }

            console.log('[useOrder] ✅ Orders by status loaded:', response.data.length);
            return response.data;

        } catch (err: any) {
            logError('Get orders by status failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);

            return [];
        } finally {
            setIsLoading(false);
        }
    }, []);

    /**
     * Get recent orders
     */
    const getRecentOrders = useCallback(async (
        params?: GetApiV1OrderRecentParams
    ): Promise<OrderDetailDto[]> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 📋 Fetching recent orders:', params);

            const response = await api.getApiV1OrderRecent(params);

            if (!response.data) {
                throw new Error('No orders data returned');
            }

            console.log('[useOrder] ✅ Recent orders loaded:', response.data.length);
            return response.data;

        } catch (err: any) {
            logError('Get recent orders failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);

            return [];
        } finally {
            setIsLoading(false);
        }
    }, []);

    // ============================================================================
    // Order Status Management
    // ============================================================================

    /**
     * Get order status history
     */
    const getStatusHistory = useCallback(async (
        orderId: string
    ): Promise<any[]> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 📋 Fetching status history:', orderId);

            const response = await api.getApiV1OrderOrderIdStatusHistory(orderId);

            if (!response.data) {
                throw new Error('No status history returned');
            }

            console.log('[useOrder] ✅ Status history loaded');
            return response.data;

        } catch (err: any) {
            logError('Get status history failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);

            return [];
        } finally {
            setIsLoading(false);
        }
    }, []);

    /**
     * Update order status
     */
    const updateOrderStatus = useCallback(async (
        orderId: string,
        status: OrderStatus,
        notes?: string
    ): Promise<boolean> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 🔄 Updating order status:', { orderId, status, notes });

            const updateData: OrderStatusUpdateDTO = {
                status,
                notes: notes || undefined
            };

            const response = await api.putApiV1OrderOrderIdStatus(orderId, updateData);

            if (!response.data) {
                throw new Error('Update status failed');
            }

            console.log('[useOrder] ✅ Order status updated');
            toast.success('Cập nhật trạng thái đơn hàng thành công');

            return true;

        } catch (err: any) {
            logError('Update order status failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);
            toast.error(errorMessage);

            return false;
        } finally {
            setIsLoading(false);
        }
    }, []);

    /**
     * Check if can change order status
     */
    const canChangeStatus = useCallback(async (
        orderId: string,
        params?: GetApiV1OrderOrderIdCanChangeStatusParams
    ): Promise<boolean> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 🔍 Checking if can change status:', orderId);

            const response = await api.getApiV1OrderOrderIdCanChangeStatus(orderId, params);

            console.log('[useOrder] ✅ Can change status check complete');
            return response.data ?? false;

        } catch (err: any) {
            logError('Can change status check failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);

            return false;
        } finally {
            setIsLoading(false);
        }
    }, []);

    /**
     * Bulk update order status
     */
    const bulkUpdateStatus = useCallback(async (
        orderIds: string[],
        status: OrderStatus,
        notes?: string
    ): Promise<boolean> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 🔄 Bulk updating order status:', { count: orderIds.length, status });

            const bulkRequest: BulkUpdateStatusRequest = {
                orderIds,
                status,
                notes: notes || undefined
            };

            const response = await api.postApiV1OrderBulkUpdateStatus(bulkRequest);

            if (!response.data) {
                throw new Error('Bulk update failed');
            }

            console.log('[useOrder] ✅ Bulk status update complete');
            toast.success(`Đã cập nhật ${orderIds.length} đơn hàng`);

            return true;

        } catch (err: any) {
            logError('Bulk update status failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);
            toast.error(errorMessage);

            return false;
        } finally {
            setIsLoading(false);
        }
    }, []);

    // ============================================================================
    // Order Statistics
    // ============================================================================

    /**
     * Get order count
     */
    const getOrderCount = useCallback(async (
        params?: GetApiV1OrderStatsCountParams
    ): Promise<number> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 📊 Fetching order count:', params);

            const response = await api.getApiV1OrderStatsCount(params);

            console.log('[useOrder] ✅ Order count loaded:', response.data);
            return response.data ?? 0;

        } catch (err: any) {
            logError('Get order count failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);

            return 0;
        } finally {
            setIsLoading(false);
        }
    }, []);

    /**
     * Get revenue statistics
     */
    const getRevenue = useCallback(async (
        params?: GetApiV1OrderStatsRevenueParams
    ): Promise<number> => {
        try {
            setIsLoading(true);
            setError(null);

            console.log('[useOrder] 📊 Fetching revenue:', params);

            const response = await api.getApiV1OrderStatsRevenue(params);

            console.log('[useOrder] ✅ Revenue loaded:', response.data);
            return response.data ?? 0;

        } catch (err: any) {
            logError('Get revenue failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);

            return 0;
        } finally {
            setIsLoading(false);
        }
    }, []);

    // ============================================================================
    // Return
    // ============================================================================

    return {
        isLoading,
        error,

        // Queries
        getOrderById,
        getMyOrders,
        getAllOrders,
        searchOrders,
        getOrdersByStatus,
        getRecentOrders,

        // Status management
        getStatusHistory,
        updateOrderStatus,
        canChangeStatus,
        bulkUpdateStatus,

        // Statistics
        getOrderCount,
        getRevenue,
    };
};

export default useOrder;

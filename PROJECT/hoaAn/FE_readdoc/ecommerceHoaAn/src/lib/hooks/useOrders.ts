/**
 * useOrders Hook
 * Custom hook để quản lý orders state với pagination, filter, tracking
 */

import { useState, useEffect, useCallback } from 'react';
import { orderService } from '../services/orderService';
import {
  OrderDto,
  CreateOrderRequest,
  OrderFilterParams,
  PagedResponse,
  OrderStatus,
} from '../api/types';
import { toast } from 'sonner@2.0.3';
import { ApiError } from '../api/errors';

interface UseOrdersOptions {
  autoLoad?: boolean;
  initialParams?: OrderFilterParams;
  myOrdersOnly?: boolean;
}

interface UseOrdersReturn {
  orders: OrderDto[];
  pagedData: PagedResponse<OrderDto> | null;
  isLoading: boolean;
  isRefreshing: boolean;
  error: string | null;
  params: OrderFilterParams;
  setParams: (params: OrderFilterParams) => void;
  loadOrders: (params?: OrderFilterParams) => Promise<void>;
  getOrderById: (id: string) => Promise<OrderDto | null>;
  getOrderByNumber: (orderNumber: string) => Promise<OrderDto | null>;
  createOrder: (request: CreateOrderRequest) => Promise<OrderDto | null>;
  cancelOrder: (id: string, reason?: string) => Promise<boolean>;
  trackOrder: (trackingNumber: string) => Promise<any>;
  rateOrder: (orderId: string, rating: number, review?: string) => Promise<boolean>;
  updateOrderStatus: (orderId: string, status: OrderStatus) => Promise<OrderDto | null>;
  refresh: () => Promise<void>;
  goToPage: (page: number) => void;
  nextPage: () => void;
  previousPage: () => void;
}

export const useOrders = (options?: UseOrdersOptions): UseOrdersReturn => {
  const { autoLoad = true, initialParams = {}, myOrdersOnly = false } = options || {};

  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [pagedData, setPagedData] = useState<PagedResponse<OrderDto> | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [isRefreshing, setIsRefreshing] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [params, setParams] = useState<OrderFilterParams>(initialParams);

  /**
   * Load orders
   */
  const loadOrders = useCallback(async (loadParams?: OrderFilterParams) => {
    try {
      const currentParams = loadParams || params;
      setIsLoading(true);
      setError(null);

      const response = myOrdersOnly
        ? await orderService.getMyOrders(currentParams)
        : await orderService.getOrders(currentParams);
      
      setOrders(response.items);
      setPagedData(response);
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể tải danh sách đơn hàng';
      
      setError(errorMessage);
      console.error('Load orders error:', err);
    } finally {
      setIsLoading(false);
    }
  }, [params, myOrdersOnly]);

  /**
   * Get order by ID
   */
  const getOrderById = useCallback(async (id: string): Promise<OrderDto | null> => {
    try {
      const order = await orderService.getOrderById(id);
      return order;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể tải thông tin đơn hàng';
      
      toast.error(errorMessage);
      return null;
    }
  }, []);

  /**
   * Get order by number
   */
  const getOrderByNumber = useCallback(async (orderNumber: string): Promise<OrderDto | null> => {
    try {
      const order = await orderService.getOrderByNumber(orderNumber);
      return order;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể tải thông tin đơn hàng';
      
      toast.error(errorMessage);
      return null;
    }
  }, []);

  /**
   * Create order
   */
  const createOrder = useCallback(async (request: CreateOrderRequest): Promise<OrderDto | null> => {
    try {
      setIsLoading(true);
      const order = await orderService.createOrder(request);
      
      toast.success('Đặt hàng thành công!', {
        description: `Mã đơn hàng: ${order.orderNumber}`,
      });
      
      // Refresh danh sách
      await loadOrders();
      
      return order;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể tạo đơn hàng';
      
      toast.error(errorMessage);
      return null;
    } finally {
      setIsLoading(false);
    }
  }, [loadOrders]);

  /**
   * Cancel order
   */
  const cancelOrder = useCallback(async (id: string, reason?: string): Promise<boolean> => {
    try {
      setIsLoading(true);
      await orderService.cancelOrder(id, reason);
      
      toast.success('Hủy đơn hàng thành công!');
      
      // Refresh danh sách
      await loadOrders();
      
      return true;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể hủy đơn hàng';
      
      toast.error(errorMessage);
      return false;
    } finally {
      setIsLoading(false);
    }
  }, [loadOrders]);

  /**
   * Track order
   */
  const trackOrder = useCallback(async (trackingNumber: string) => {
    try {
      setIsLoading(true);
      const result = await orderService.trackOrder(trackingNumber);
      return result;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể theo dõi đơn hàng';
      
      toast.error(errorMessage);
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Rate order
   */
  const rateOrder = useCallback(async (
    orderId: string, 
    rating: number, 
    review?: string
  ): Promise<boolean> => {
    try {
      setIsLoading(true);
      await orderService.rateOrder(orderId, rating, review);
      
      toast.success('Cảm ơn bạn đã đánh giá!');
      
      return true;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể gửi đánh giá';
      
      toast.error(errorMessage);
      return false;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Update order status (Admin)
   */
  const updateOrderStatus = useCallback(async (
    orderId: string, 
    status: OrderStatus
  ): Promise<OrderDto | null> => {
    try {
      setIsLoading(true);
      const order = await orderService.updateOrderStatus(orderId, status);
      
      toast.success('Cập nhật trạng thái đơn hàng thành công!');
      
      // Refresh danh sách
      await loadOrders();
      
      return order;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể cập nhật trạng thái đơn hàng';
      
      toast.error(errorMessage);
      return null;
    } finally {
      setIsLoading(false);
    }
  }, [loadOrders]);

  /**
   * Refresh data
   */
  const refresh = useCallback(async () => {
    try {
      setIsRefreshing(true);
      await loadOrders();
    } finally {
      setIsRefreshing(false);
    }
  }, [loadOrders]);

  /**
   * Pagination helpers
   */
  const goToPage = useCallback((page: number) => {
    setParams(prev => ({ ...prev, pageNumber: page }));
  }, []);

  const nextPage = useCallback(() => {
    if (pagedData?.hasNextPage) {
      setParams(prev => ({ ...prev, pageNumber: (prev.pageNumber || 1) + 1 }));
    }
  }, [pagedData]);

  const previousPage = useCallback(() => {
    if (pagedData?.hasPreviousPage) {
      setParams(prev => ({ ...prev, pageNumber: Math.max((prev.pageNumber || 1) - 1, 1) }));
    }
  }, [pagedData]);

  /**
   * Auto load on mount và khi params thay đổi
   */
  useEffect(() => {
    if (autoLoad) {
      loadOrders(params);
    }
  }, [params.pageNumber, params.status, params.paymentStatus, params.fromDate, params.toDate]);

  return {
    orders,
    pagedData,
    isLoading,
    isRefreshing,
    error,
    params,
    setParams,
    loadOrders,
    getOrderById,
    getOrderByNumber,
    createOrder,
    cancelOrder,
    trackOrder,
    rateOrder,
    updateOrderStatus,
    refresh,
    goToPage,
    nextPage,
    previousPage,
  };
};

export default useOrders;

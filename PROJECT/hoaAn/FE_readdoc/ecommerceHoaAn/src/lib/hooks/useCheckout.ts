/**
 * useCheckout Hook
 * Custom hook để quản lý checkout & orders
 * ✅ Tích hợp với CheckoutService
 * ✅ FIX: Không dùng getDisplayMessage() - dùng helper function
 * ✅ FIX: Request format đúng theo backend API
 */

import { useState, useCallback } from 'react';
import { CheckoutService } from '@/api/services/CheckoutService';
import { ApiError } from '@/api/core/ApiError';
import type { 
  CheckoutDto, 
  OrderDetailDto,
  CancelOrderRequest,
  OrderStatus 
} from '@/api';
import { toast } from 'sonner';

/**
 * ✅ Helper: Extract error message từ ApiError
 * Vì ApiError từ openapi-typescript-codegen không có method getDisplayMessage()
 */
const getErrorMessage = (err: any): string => {
  if (err instanceof ApiError) {
    // Thử lấy message từ response body
    if (err.body?.message) {
      return err.body.message;
    }
    if (err.body?.error) {
      return err.body.error;
    }
    if (err.body?.title) {
      return err.body.title;
    }
    // Fallback về statusText
    return err.statusText || `Lỗi ${err.status}`;
  }
  
  // Fallback cho errors khác
  return err.message || 'Đã xảy ra lỗi';
};

interface UseCheckoutReturn {
  isProcessing: boolean;
  error: string | null;
  processCheckout: (checkoutData: CheckoutDto) => Promise<OrderDetailDto | null>;
  getOrderDetails: (orderId: string) => Promise<OrderDetailDto | null>;
  getMyOrders: (filters?: OrderFilters) => Promise<OrderDetailDto[]>;
  cancelOrder: (orderId: string, reason: string) => Promise<boolean>;
}

interface OrderFilters {
  page?: number;
  pageSize?: number;
  keyword?: string;
  status?: OrderStatus;
  fromDate?: string;
  toDate?: string;
  minAmount?: number;
  maxAmount?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

/**
 * ✅ Hook quản lý checkout & orders
 * 
 * @example
 * ```tsx
 * const { processCheckout, getOrderDetails, isProcessing } = useCheckout();
 * 
 * // Process checkout
 * const result = await processCheckout({
 *   items: [...],
 *   shippingAddress: {...},
 *   paymentMethod: 'COD',
 * });
 * 
 * if (result) {
 *   console.log('Order created:', result.orderId);
 * }
 * ```
 */
export const useCheckout = (): UseCheckoutReturn => {
  const [isProcessing, setIsProcessing] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  /**
   * ✅ Xử lý checkout - Tạo đơn hàng
   * Backend returns OrderDetailDto trong response.data
   */
  const processCheckout = useCallback(async (
    checkoutData: CheckoutDto
  ): Promise<OrderDetailDto | null> => {
    try {
      setIsProcessing(true);
      setError(null);

      console.log('[useCheckout] 🛒 Processing checkout...', checkoutData);

      const response = await CheckoutService.postApiV1CheckoutProcess(checkoutData);

      if (!response.success || !response.data) {
        throw new Error('Không thể xử lý đơn hàng');
      }

      console.log('[useCheckout] ✅ Checkout successful:', response.data);
      toast.success('Đặt hàng thành công!');
      
      return response.data;
    } catch (err: any) {
      console.error('[useCheckout] ❌ Checkout error:', err);
      
      const errorMessage = getErrorMessage(err);
      setError(errorMessage);
      toast.error(errorMessage);
      
      return null;
    } finally {
      setIsProcessing(false);
    }
  }, []);

  /**
   * ✅ Lấy chi tiết đơn hàng
   */
  const getOrderDetails = useCallback(async (
    orderId: string
  ): Promise<OrderDetailDto | null> => {
    try {
      setIsProcessing(true);
      setError(null);

      const response = await CheckoutService.getApiV1Checkout(orderId);

      if (!response.success || !response.data) {
        throw new Error('Không thể tải thông tin đơn hàng');
      }

      console.log('[useCheckout] ✅ Order details loaded:', orderId);
      
      return response.data;
    } catch (err: any) {
      console.error('[useCheckout] ❌ Get order details error:', err);
      
      const errorMessage = getErrorMessage(err);
      setError(errorMessage);
      
      return null;
    } finally {
      setIsProcessing(false);
    }
  }, []);

  /**
   * ✅ Lấy danh sách đơn hàng của tôi
   */
  const getMyOrders = useCallback(async (
    filters: OrderFilters = {}
  ): Promise<OrderDetailDto[]> => {
    try {
      setIsProcessing(true);
      setError(null);

      const response = await CheckoutService.getApiV1CheckoutMyOrders(
        filters.page,
        filters.pageSize,
        filters.keyword,
        undefined, // customerId - backend tự lấy từ token
        undefined, // storeId
        filters.status,
        filters.fromDate,
        filters.toDate,
        filters.minAmount,
        filters.maxAmount,
        filters.sortBy,
        filters.sortDescending
      );

      if (!response.success || !response.data) {
        throw new Error('Không thể tải danh sách đơn hàng');
      }

      console.log('[useCheckout] ✅ My orders loaded:', response.data.length);
      
      return response.data;
    } catch (err: any) {
      console.error('[useCheckout] ❌ Get my orders error:', err);
      
      const errorMessage = getErrorMessage(err);
      setError(errorMessage);
      toast.error(errorMessage);
      
      return [];
    } finally {
      setIsProcessing(false);
    }
  }, []);

  /**
   * ✅ Hủy đơn hàng
   */
  const cancelOrder = useCallback(async (
    orderId: string,
    reason: string
  ): Promise<boolean> => {
    try {
      setIsProcessing(true);
      setError(null);

      const cancelRequest: CancelOrderRequest = {
        reason,
      };

      const response = await CheckoutService.postApiV1CheckoutCancel(
        orderId,
        cancelRequest
      );

      if (!response.success) {
        throw new Error('Không thể hủy đơn hàng');
      }

      console.log('[useCheckout] ✅ Order cancelled:', orderId);
      toast.success('Đã hủy đơn hàng');
      
      return true;
    } catch (err: any) {
      console.error('[useCheckout] ❌ Cancel order error:', err);
      
      const errorMessage = getErrorMessage(err);
      setError(errorMessage);
      toast.error(errorMessage);
      
      return false;
    } finally {
      setIsProcessing(false);
    }
  }, []);

  return {
    isProcessing,
    error,
    processCheckout,
    getOrderDetails,
    getMyOrders,
    cancelOrder,
  };
};

export default useCheckout;
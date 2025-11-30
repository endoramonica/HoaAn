/**
 * useCheckout Hook
 * ✅ Chuẩn hóa hoàn toàn theo Orval-generated API
 * ✅ Chỉ xử lý Checkout flow (process, get order, my orders, cancel)
 * ✅ Logging tối ưu cho debugging
 * ✅ Exception handling thống nhất
 */

import { useState, useCallback } from 'react';
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type {
  CheckoutDto,
  OrderDetailDto,
  CancelOrderRequest,
  GetApiV1CheckoutMyOrdersParams
} from '../../../Api/generated-orval/schemas';
import { toast } from 'sonner';

const api = getVietCommerceAPI();

// ============================================================================
// Types
// ============================================================================

interface UseCheckoutReturn {
  isProcessing: boolean;
  error: string | null;
  processCheckout: (checkoutData: CheckoutDto) => Promise<OrderDetailDto | null>;
  getOrderDetails: (orderId: string) => Promise<OrderDetailDto | null>;
  getMyOrders: (params?: GetApiV1CheckoutMyOrdersParams) => Promise<OrderDetailDto[]>;
  cancelOrder: (orderId: string, reason: string) => Promise<boolean>;
}

// ============================================================================
// Helper Functions
// ============================================================================

/**
 * Extract user-friendly error message from API error
 */
const extractErrorMessage = (err: any): string => {
  // Try response data first
  if (err?.response?.data?.message) return err.response.data.message;
  if (err?.response?.data?.error) return err.response.data.error;
  if (err?.response?.data?.title) return err.response.data.title;

  // Try error message
  if (err?.message) return err.message;

  // Fallback
  return 'Đã xảy ra lỗi không xác định';
};

/**
 * Log error details for debugging
 */
const logError = (context: string, err: any) => {
  console.error(`[useCheckout] ❌ ${context}:`, {
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
 * Hook quản lý Checkout flow
 * 
 * @example
 * ```tsx
 * const { processCheckout, isProcessing } = useCheckout();
 * 
 * const handleCheckout = async () => {
 *   const result = await processCheckout({
 *     cartId: '...',
 *     shippingInfo: { ... },
 *     paymentMethod: 'cod'
 *   });
 *   
 *   if (result) {
 *     navigate(`/order/${result.orderId}`);
 *   }
 * };
 * ```
 */
export const useCheckout = (): UseCheckoutReturn => {
  const [isProcessing, setIsProcessing] = useState(false);
  const [error, setError] = useState<string | null>(null);

  /**
   * Process checkout - Create order from cart
   * ✅ Validates cart before checkout to prevent EMPTY_CART error
   */
  const processCheckout = useCallback(async (
    checkoutData: CheckoutDto
  ): Promise<OrderDetailDto | null> => {
    try {
      setIsProcessing(true);
      setError(null);

      console.log('[useCheckout] � Proceassing checkout...');
      console.log('[useCheckout] 📦 CartId:', checkoutData.cartId);

      // ✅ STEP 1: Validate cart exists and has items
      console.log('[useCheckout] 🔍 Validating cart before checkout...');

      let cartData;
      try {
        const cartResponse = await api.getApiV1Cart();
        cartData = cartResponse.data;

        console.log('[useCheckout] 📋 Cart validation:', {
          cartId: cartData?.cartId,
          userId: cartData?.userId,
          itemCount: cartData?.totalItems,
          totalAmount: cartData?.totalAmount
        });

        // Check if cart exists
        if (!cartData || !cartData.cartId) {
          throw new Error('CART_NOT_FOUND: Không tìm thấy giỏ hàng');
        }

        // Check if cart has items
        if (!cartData.items || cartData.items.length === 0) {
          throw new Error('EMPTY_CART: Giỏ hàng trống');
        }

        // Check if cartId matches
        if (cartData.cartId !== checkoutData.cartId) {
          console.warn('[useCheckout] ⚠️ CartId mismatch:', {
            expected: checkoutData.cartId,
            actual: cartData.cartId
          });
          throw new Error('CART_MISMATCH: CartId không khớp');
        }

        console.log('[useCheckout] ✅ Cart validation passed');

      } catch (validationErr: any) {
        console.error('[useCheckout] ❌ Cart validation failed:', validationErr);

        // If it's our custom error, throw it
        if (validationErr.message?.startsWith('CART_') || validationErr.message?.startsWith('EMPTY_')) {
          throw validationErr;
        }

        // If it's API error, log and throw custom error
        logError('Cart validation API error', validationErr);
        throw new Error('CART_VALIDATION_FAILED: Không thể xác thực giỏ hàng');
      }

      // ✅ STEP 2: Proceed with checkout
      console.log('[useCheckout] 💳 Proceeding with checkout...');
      console.log('[useCheckout] 📦 Checkout Payload:', {
        cartId: checkoutData.cartId,
        paymentMethod: checkoutData.paymentMethod,
        shippingInfo: {
          recipientName: checkoutData.shippingInfo.recipientName,
          phoneNumber: checkoutData.shippingInfo.phoneNumber,
          address: checkoutData.shippingInfo.address,
          city: checkoutData.shippingInfo.city
        },
        couponCode: checkoutData.couponCode,
        itemCount: cartData.totalItems,
        totalAmount: cartData.totalAmount
      });

      const response = await api.postApiV1CheckoutProcess(checkoutData);

      if (!response.data) {
        throw new Error('No data returned from checkout API');
      }

      console.log('[useCheckout] ✅ Checkout successful:', {
        orderId: response.data.orderId,
        orderNumber: response.data.orderNumber,
        totalAmount: response.data.totalAmount
      });

      toast.success('Đặt hàng thành công!');
      return response.data;

    } catch (err: any) {
      logError('Process checkout failed', err);

      const errorMessage = extractErrorMessage(err);
      setError(errorMessage);

      // Show user-friendly error messages
      if (errorMessage.includes('EMPTY_CART')) {
        toast.error('Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi thanh toán.');
      } else if (errorMessage.includes('CART_NOT_FOUND')) {
        toast.error('Không tìm thấy giỏ hàng. Vui lòng thử lại.');
      } else if (errorMessage.includes('CART_MISMATCH')) {
        toast.error('Giỏ hàng không hợp lệ. Vui lòng tải lại trang.');
      } else if (errorMessage.includes('CART_VALIDATION_FAILED')) {
        toast.error('Không thể xác thực giỏ hàng. Vui lòng đăng nhập lại.');
      } else {
        toast.error(errorMessage);
      }

      return null;
    } finally {
      setIsProcessing(false);
    }
  }, []);

  /**
   * Get order details by orderId
   */
  const getOrderDetails = useCallback(async (
    orderId: string
  ): Promise<OrderDetailDto | null> => {
    try {
      setIsProcessing(true);
      setError(null);

      console.log('[useCheckout] 📋 Fetching order details:', orderId);

      const response = await api.getApiV1CheckoutOrderId(orderId);

      if (!response.data) {
        throw new Error('No order data returned');
      }

      console.log('[useCheckout] ✅ Order details loaded');
      return response.data;

    } catch (err: any) {
      logError('Get order details failed', err);

      const errorMessage = extractErrorMessage(err);
      setError(errorMessage);

      return null;
    } finally {
      setIsProcessing(false);
    }
  }, []);

  /**
   * Get my orders with optional filters
   */
  const getMyOrders = useCallback(async (
    params?: GetApiV1CheckoutMyOrdersParams
  ): Promise<OrderDetailDto[]> => {
    try {
      setIsProcessing(true);
      setError(null);

      console.log('[useCheckout] 📋 Fetching my orders:', params);

      const response = await api.getApiV1CheckoutMyOrders(params);

      if (!response.data) {
        throw new Error('No orders data returned');
      }

      console.log('[useCheckout] ✅ My orders loaded:', response.data.length);
      return response.data;

    } catch (err: any) {
      logError('Get my orders failed', err);

      const errorMessage = extractErrorMessage(err);
      setError(errorMessage);
      toast.error(errorMessage);

      return [];
    } finally {
      setIsProcessing(false);
    }
  }, []);

  /**
   * Cancel order
   */
  const cancelOrder = useCallback(async (
    orderId: string,
    reason: string
  ): Promise<boolean> => {
    try {
      setIsProcessing(true);
      setError(null);

      console.log('[useCheckout] 🚫 Cancelling order:', orderId);

      const cancelRequest: CancelOrderRequest = { reason };
      const response = await api.postApiV1CheckoutOrderIdCancel(orderId, cancelRequest);

      if (!response.data) {
        throw new Error('Cancel order failed');
      }

      console.log('[useCheckout] ✅ Order cancelled successfully');
      toast.success('Đã hủy đơn hàng');

      return true;

    } catch (err: any) {
      logError('Cancel order failed', err);

      const errorMessage = extractErrorMessage(err);
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

/**
 * Custom Hook cho Checkout - Quản lý checkout và đơn hàng
 */

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  checkoutService,
  orderService,
  CheckoutRequest,
  CheckoutResponseDto,
  OrderDetailDto,
  CancelOrderRequest,
  OrderFilterParams,
  OrderStatus,
} from '../services/checkoutService';
import { toast } from 'sonner@2.0.3';
import { cartKeys } from './useCart';

// ============================================================================
// Query Keys
// ============================================================================

export const checkoutKeys = {
  all: ['checkout'] as const,
  orders: () => [...checkoutKeys.all, 'orders'] as const,
  myOrders: (params?: OrderFilterParams) => [...checkoutKeys.all, 'my-orders', params] as const,
  order: (orderId: string) => [...checkoutKeys.all, 'order', orderId] as const,
  statusHistory: (orderId: string) => [...checkoutKeys.all, 'status-history', orderId] as const,
  byStatus: (status: OrderStatus) => [...checkoutKeys.all, 'by-status', status] as const,
  recent: (count: number) => [...checkoutKeys.all, 'recent', count] as const,
  stats: () => [...checkoutKeys.all, 'stats'] as const,
};

// ============================================================================
// useProcessCheckout Hook - Xử lý checkout
// ============================================================================

export function useProcessCheckout() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CheckoutRequest) => checkoutService.processCheckout(request),
    onSuccess: (data: CheckoutResponseDto) => {
      // Clear cart sau khi checkout thành công
      queryClient.invalidateQueries({ queryKey: cartKeys.all });
      queryClient.invalidateQueries({ queryKey: checkoutKeys.orders() });
      
      toast.success(`Đặt hàng thành công! Mã đơn: ${data.orderNumber}`);
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể đặt hàng. Vui lòng thử lại');
    },
  });
}

// ============================================================================
// useOrderDetails Hook - Lấy chi tiết đơn hàng
// ============================================================================

export function useOrderDetails(orderId: string) {
  return useQuery({
    queryKey: checkoutKeys.order(orderId),
    queryFn: () => orderService.getOrderById(orderId),
    enabled: !!orderId,
    staleTime: 30000, // 30 seconds
  });
}

// ============================================================================
// useMyOrders Hook - Lấy danh sách đơn hàng của user
// ============================================================================

export function useMyOrders(params?: OrderFilterParams) {
  return useQuery({
    queryKey: checkoutKeys.myOrders(params),
    queryFn: () => orderService.getMyOrders(params),
    staleTime: 60000, // 1 minute
  });
}

// ============================================================================
// useMyOrdersList Hook - Lấy danh sách đơn hàng (không phân trang)
// ============================================================================

export function useMyOrdersList(params?: OrderFilterParams) {
  return useQuery({
    queryKey: checkoutKeys.myOrders(params),
    queryFn: () => checkoutService.getMyOrders(params),
    staleTime: 60000,
  });
}

// ============================================================================
// useAllOrders Hook - Lấy tất cả đơn hàng (Admin)
// ============================================================================

export function useAllOrders(params?: OrderFilterParams) {
  return useQuery({
    queryKey: checkoutKeys.orders(),
    queryFn: () => orderService.getAllOrders(params),
    staleTime: 60000,
  });
}

// ============================================================================
// useOrdersByStatus Hook - Lấy đơn hàng theo trạng thái
// ============================================================================

export function useOrdersByStatus(status: OrderStatus) {
  return useQuery({
    queryKey: checkoutKeys.byStatus(status),
    queryFn: () => orderService.getOrdersByStatus(status),
    enabled: !!status,
    staleTime: 30000,
  });
}

// ============================================================================
// useRecentOrders Hook - Lấy đơn hàng gần đây
// ============================================================================

export function useRecentOrders(count: number = 10, storeId?: string) {
  return useQuery({
    queryKey: checkoutKeys.recent(count),
    queryFn: () => orderService.getRecentOrders(count, storeId),
    staleTime: 30000,
  });
}

// ============================================================================
// useOrderStatusHistory Hook - Lấy lịch sử trạng thái
// ============================================================================

export function useOrderStatusHistory(orderId: string) {
  return useQuery({
    queryKey: checkoutKeys.statusHistory(orderId),
    queryFn: () => orderService.getOrderStatusHistory(orderId),
    enabled: !!orderId,
    staleTime: 30000,
  });
}

// ============================================================================
// useSearchOrders Hook - Tìm kiếm đơn hàng
// ============================================================================

export function useSearchOrders(query: string) {
  return useQuery({
    queryKey: [...checkoutKeys.orders(), 'search', query],
    queryFn: () => orderService.searchOrders(query),
    enabled: !!query && query.length > 2,
    staleTime: 30000,
  });
}

// ============================================================================
// useCancelOrder Hook - Hủy đơn hàng
// ============================================================================

export function useCancelOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ orderId, request }: { orderId: string; request: CancelOrderRequest }) => 
      checkoutService.cancelOrder(orderId, request),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: checkoutKeys.order(variables.orderId) });
      queryClient.invalidateQueries({ queryKey: checkoutKeys.orders() });
      toast.success('Đã hủy đơn hàng');
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể hủy đơn hàng');
    },
  });
}

// ============================================================================
// useUpdateOrderStatus Hook - Cập nhật trạng thái (Admin/Staff)
// ============================================================================

export function useUpdateOrderStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ 
      orderId, 
      status, 
      notes 
    }: { 
      orderId: string; 
      status: OrderStatus; 
      notes?: string 
    }) => orderService.updateOrderStatus(orderId, status, notes),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: checkoutKeys.order(variables.orderId) });
      queryClient.invalidateQueries({ queryKey: checkoutKeys.orders() });
      toast.success('Đã cập nhật trạng thái đơn hàng');
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể cập nhật trạng thái');
    },
  });
}

// ============================================================================
// useCanChangeStatus Hook - Kiểm tra có thể thay đổi trạng thái
// ============================================================================

export function useCanChangeStatus(orderId: string, newStatus: OrderStatus) {
  return useQuery({
    queryKey: [...checkoutKeys.order(orderId), 'can-change', newStatus],
    queryFn: () => orderService.canChangeStatus(orderId, newStatus),
    enabled: !!orderId && !!newStatus,
  });
}

// ============================================================================
// useOrderStats Hook - Thống kê đơn hàng
// ============================================================================

export function useOrderStats(
  storeId?: string,
  fromDate?: string,
  toDate?: string
) {
  return useQuery({
    queryKey: [...checkoutKeys.stats(), storeId, fromDate, toDate],
    queryFn: async () => {
      const [count, revenue] = await Promise.all([
        orderService.getOrderCount(storeId, fromDate, toDate),
        orderService.getRevenue(storeId, fromDate, toDate),
      ]);

      return {
        count,
        revenue,
      };
    },
    staleTime: 60000,
  });
}

// ============================================================================
// useBulkUpdateOrderStatus Hook - Cập nhật nhiều đơn hàng
// ============================================================================

export function useBulkUpdateOrderStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ 
      orderIds, 
      newStatus, 
      reason 
    }: { 
      orderIds: string[]; 
      newStatus: OrderStatus; 
      reason?: string 
    }) => orderService.bulkUpdateStatus(orderIds, newStatus, reason),
    onSuccess: (updatedCount) => {
      queryClient.invalidateQueries({ queryKey: checkoutKeys.orders() });
      toast.success(`Đã cập nhật ${updatedCount} đơn hàng`);
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể cập nhật đơn hàng');
    },
  });
}

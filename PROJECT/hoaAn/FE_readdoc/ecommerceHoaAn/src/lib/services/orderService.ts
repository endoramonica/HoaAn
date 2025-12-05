/**
 * Order Service
 * ✅ UPDATED: Uses Orval generated API
 * Xử lý tạo đơn hàng, theo dõi đơn hàng, lịch sử đơn hàng
 */

import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type {
  GetApiV1OrderMyOrdersParams,
  GetApiV1OrderParams,
  OrderStatusUpdateDTO,
} from '../../../Api/generated-orval/schemas';

const api = getVietCommerceAPI();

// Types for backward compatibility
export type OrderStatus =
  | 'Pending'
  | 'Confirmed'
  | 'Processing'
  | 'Shipping'
  | 'Delivered'
  | 'Cancelled'
  | 'Refunded';

export type PaymentStatus = 'Pending' | 'Paid' | 'Failed' | 'Refunded';
export type PaymentMethod = 'COD' | 'BankTransfer' | 'VNPay' | 'Momo';

export interface OrderFilterParams {
  pageNumber?: number;
  pageSize?: number;
  status?: OrderStatus;
  paymentStatus?: PaymentStatus;
  search?: string;
  fromDate?: string;
  toDate?: string;
}

export interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

class OrderService {
  /**
   * Lấy chi tiết đơn hàng theo ID
   */
  async getOrderById(id: string): Promise<any> {
    try {
      const response = await api.getApiV1OrderId(id);
      return response.data;
    } catch (error) {
      console.error('Get order by id error:', error);
      throw error;
    }
  }

  /**
   * Lấy lịch sử đơn hàng của user hiện tại
   */
  async getMyOrders(params?: OrderFilterParams): Promise<PagedResponse<any>> {
    try {
      const orvalParams: GetApiV1OrderMyOrdersParams = {
        pageNumber: params?.pageNumber,
        pageSize: params?.pageSize,
        status: params?.status as any,
      };

      const response = await api.getApiV1OrderMyOrders(orvalParams);
      const data = response.data;

      return {
        items: data?.items || [],
        pageNumber: data?.pageNumber || 1,
        pageSize: data?.pageSize || 10,
        totalPages: data?.totalPages || 0,
        totalCount: data?.totalItems || 0,
        hasPreviousPage: (data?.pageNumber || 1) > 1,
        hasNextPage: (data?.pageNumber || 1) < (data?.totalPages || 0),
      };
    } catch (error) {
      console.error('Get my orders error:', error);
      throw error;
    }
  }

  /**
   * Lấy danh sách đơn hàng (Admin)
   */
  async getOrders(params?: OrderFilterParams): Promise<PagedResponse<any>> {
    try {
      const orvalParams: GetApiV1OrderParams = {
        pageNumber: params?.pageNumber,
        pageSize: params?.pageSize,
        status: params?.status as any,
      };

      const response = await api.getApiV1Order(orvalParams);
      const data = response.data;

      return {
        items: data?.items || [],
        pageNumber: data?.pageNumber || 1,
        pageSize: data?.pageSize || 10,
        totalPages: data?.totalPages || 0,
        totalCount: data?.totalItems || 0,
        hasPreviousPage: (data?.pageNumber || 1) > 1,
        hasNextPage: (data?.pageNumber || 1) < (data?.totalPages || 0),
      };
    } catch (error) {
      console.error('Get orders error:', error);
      throw error;
    }
  }

  /**
   * Lấy đơn hàng theo trạng thái
   */
  async getOrdersByStatus(status: OrderStatus): Promise<any[]> {
    try {
      const response = await api.getApiV1OrderStatusStatus(status as any);
      return response.data || [];
    } catch (error) {
      console.error('Get orders by status error:', error);
      throw error;
    }
  }

  /**
   * Lấy lịch sử trạng thái đơn hàng
   */
  async getOrderStatusHistory(orderId: string): Promise<any[]> {
    try {
      const response = await api.getApiV1OrderOrderIdStatusHistory(orderId);
      return response.data || [];
    } catch (error) {
      console.error('Get order status history error:', error);
      throw error;
    }
  }

  /**
   * Cập nhật trạng thái đơn hàng (Admin)
   */
  async updateOrderStatus(orderId: string, status: OrderStatus, note?: string): Promise<any> {
    try {
      const dto: OrderStatusUpdateDTO = {
        status: status as any,
        note,
      };
      const response = await api.putApiV1OrderOrderIdStatus(orderId, dto);
      return response.data;
    } catch (error) {
      console.error('Update order status error:', error);
      throw error;
    }
  }

  /**
   * Hủy đơn hàng
   */
  async cancelOrder(orderId: string): Promise<any> {
    try {
      const response = await api.postApiV1OrderOrderIdCancel(orderId);
      return response.data;
    } catch (error) {
      console.error('Cancel order error:', error);
      throw error;
    }
  }

  /**
   * Kiểm tra có thể thay đổi trạng thái không
   */
  async canChangeStatus(orderId: string, newStatus: OrderStatus): Promise<boolean> {
    try {
      const response = await api.getApiV1OrderOrderIdCanChangeStatus(orderId, { newStatus: newStatus as any });
      return response.data || false;
    } catch (error) {
      console.error('Can change status error:', error);
      return false;
    }
  }

  /**
   * Lấy thống kê số lượng đơn hàng
   */
  async getOrderCountStats(): Promise<any> {
    try {
      const response = await api.getApiV1OrderStatsCount();
      return response.data;
    } catch (error) {
      console.error('Get order count stats error:', error);
      throw error;
    }
  }

  /**
   * Lấy thống kê doanh thu
   */
  async getRevenueStats(params?: { fromDate?: string; toDate?: string }): Promise<any> {
    try {
      const response = await api.getApiV1OrderStatsRevenue(params);
      return response.data;
    } catch (error) {
      console.error('Get revenue stats error:', error);
      throw error;
    }
  }

  /**
   * Cập nhật hàng loạt trạng thái đơn hàng (Admin)
   */
  async bulkUpdateStatus(orderIds: string[], status: OrderStatus): Promise<any> {
    try {
      const response = await api.postApiV1OrderBulkUpdateStatus({
        orderIds,
        status: status as any,
      });
      return response.data;
    } catch (error) {
      console.error('Bulk update status error:', error);
      throw error;
    }
  }
}

export const orderService = new OrderService();
export default orderService;

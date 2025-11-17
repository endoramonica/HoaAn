/**
 * Checkout Service Layer - Xử lý checkout và thanh toán
 * Tích hợp với ASP.NET Core Checkout API endpoints
 */

import { apiRequest } from '../api/client';
import { OrderShippingDto } from './cartService';

// ============================================================================
// Checkout Types (dựa trên OpenAPI spec)
// ============================================================================

export enum OrderStatus {
  Pending = 'pending',
  Confirmed = 'confirmed',
  Shipped = 'shipped',
  Completed = 'completed',
  Cancelled = 'cancelled',
}

export enum ShippingMethod {
  Standard = 'standard',
  Express = 'express',
  SameDay = 'sameDay',
  Overnight = 'overnight',
}

export interface OrderShippingInputDto {
  recipientName: string;
  phoneNumber: string;
  address: string;
  ward: string;
  district: string;
  city: string;
  postalCode?: string;
  deliveryNote?: string;
  shippingMethod: ShippingMethod;
}

export interface CheckoutRequest {
  cartId: string;
  shippingInfo: OrderShippingInputDto;
  couponCode?: string;
  notes?: string;
}

export interface OrderItemDto {
  id: string;
  orderId: string;
  productId: string;
  productName: string;
  productSKU?: string;
  productImageUrl?: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  subtotal: number;
}

export interface CheckoutResponseDto {
  orderId: string;
  orderNumber: string;
  status: OrderStatus;
  statusText: string;
  totalAmount: number;
  createdAt: string;
  storeId: string;
  storeName?: string;
  customerId: string;
  customerName?: string;
  items?: OrderItemDto[];
  shipping?: OrderShippingDto;
}

export interface OrderDetailDto {
  orderId: string;
  orderNumber: string;
  storeId: string;
  storeName?: string;
  customerId: string;
  customerName?: string;
  customerEmail?: string;
  customerPhone?: string;
  status: OrderStatus;
  statusText: string;
  subTotal: number;
  shippingFee: number;
  taxAmount: number;
  discountAmount: number;
  totalAmount: number;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
  completedAt?: string;
  createdById?: string;
  createdByName?: string;
  shipping?: OrderShippingDto;
  items?: OrderItemDto[];
  itemsCount: number;
  statusHistories?: OrderStatusHistoryDto[];
  isPaid: boolean;
  paidAmount?: number;
  paymentMethod?: string;
  paidAt?: string;
  transactionId?: string;
}

export interface OrderStatusHistoryDto {
  status: OrderStatus;
  statusText: string;
  changedAt: string;
  changedByName?: string;
  notes?: string;
}

export interface CancelOrderRequest {
  reason: string;
}

export interface OrderFilterParams {
  Page?: number;
  PageSize?: number;
  Keyword?: string;
  CustomerId?: string;
  StoreId?: string;
  Status?: OrderStatus;
  FromDate?: string;
  ToDate?: string;
  MinAmount?: number;
  MaxAmount?: number;
  SortBy?: string;
  SortDescending?: boolean;
}

export interface PaginatedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

// ============================================================================
// Checkout Service
// ============================================================================

export const checkoutService = {
  /**
   * Xử lý checkout và tạo đơn hàng
   */
  processCheckout: async (request: CheckoutRequest): Promise<CheckoutResponseDto> => {
    return apiRequest.post<CheckoutResponseDto>('/Checkout/process', request);
  },

  /**
   * Lấy thông tin chi tiết đơn hàng sau checkout
   */
  getOrderDetails: async (orderId: string): Promise<OrderDetailDto> => {
    return apiRequest.get<OrderDetailDto>(`/Checkout/${orderId}`);
  },

  /**
   * Lấy danh sách đơn hàng của user hiện tại
   */
  getMyOrders: async (params?: OrderFilterParams): Promise<OrderDetailDto[]> => {
    const queryParams = new URLSearchParams();
    
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          queryParams.append(key, String(value));
        }
      });
    }

    const queryString = queryParams.toString();
    const url = queryString ? `/Checkout/my-orders?${queryString}` : '/Checkout/my-orders';
    
    return apiRequest.get<OrderDetailDto[]>(url);
  },

  /**
   * Hủy đơn hàng
   */
  cancelOrder: async (orderId: string, request: CancelOrderRequest): Promise<boolean> => {
    return apiRequest.post<boolean>(`/Checkout/${orderId}/cancel`, request);
  },
};

// ============================================================================
// Order Service (Advanced)
// ============================================================================

export const orderService = {
  /**
   * Lấy chi tiết đơn hàng theo ID
   */
  getOrderById: async (orderId: string): Promise<OrderDetailDto> => {
    return apiRequest.get<OrderDetailDto>(`/Order/${orderId}`);
  },

  /**
   * Lấy danh sách đơn hàng của user (có phân trang)
   */
  getMyOrders: async (params?: OrderFilterParams): Promise<PaginatedResult<OrderDetailDto>> => {
    const queryParams = new URLSearchParams();
    
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          queryParams.append(key, String(value));
        }
      });
    }

    const queryString = queryParams.toString();
    const url = queryString ? `/Order/my-orders?${queryString}` : '/Order/my-orders';
    
    return apiRequest.get<PaginatedResult<OrderDetailDto>>(url);
  },

  /**
   * Lấy tất cả đơn hàng (Admin only - có phân trang)
   */
  getAllOrders: async (params?: OrderFilterParams): Promise<PaginatedResult<OrderDetailDto>> => {
    const queryParams = new URLSearchParams();
    
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null) {
          queryParams.append(key, String(value));
        }
      });
    }

    const queryString = queryParams.toString();
    const url = queryString ? `/Order?${queryString}` : '/Order';
    
    return apiRequest.get<PaginatedResult<OrderDetailDto>>(url);
  },

  /**
   * Tìm kiếm đơn hàng
   */
  searchOrders: async (query: string): Promise<OrderDetailDto[]> => {
    return apiRequest.get<OrderDetailDto[]>(`/Order/search?q=${encodeURIComponent(query)}`);
  },

  /**
   * Lấy đơn hàng theo trạng thái
   */
  getOrdersByStatus: async (status: OrderStatus): Promise<OrderDetailDto[]> => {
    return apiRequest.get<OrderDetailDto[]>(`/Order/status/${status}`);
  },

  /**
   * Lấy đơn hàng gần đây
   */
  getRecentOrders: async (count: number = 10, storeId?: string): Promise<OrderDetailDto[]> => {
    const params = new URLSearchParams({ count: String(count) });
    if (storeId) {
      params.append('storeId', storeId);
    }
    return apiRequest.get<OrderDetailDto[]>(`/Order/recent?${params.toString()}`);
  },

  /**
   * Lấy lịch sử trạng thái đơn hàng
   */
  getOrderStatusHistory: async (orderId: string): Promise<OrderStatusHistoryDto[]> => {
    return apiRequest.get<OrderStatusHistoryDto[]>(`/Order/${orderId}/status-history`);
  },

  /**
   * Cập nhật trạng thái đơn hàng (Admin/Staff only)
   */
  updateOrderStatus: async (
    orderId: string,
    status: OrderStatus,
    notes?: string
  ): Promise<boolean> => {
    return apiRequest.put<boolean>(`/Order/${orderId}/status`, {
      status,
      notes,
    });
  },

  /**
   * Kiểm tra có thể thay đổi trạng thái không
   */
  canChangeStatus: async (orderId: string, newStatus: OrderStatus): Promise<boolean> => {
    return apiRequest.get<boolean>(
      `/Order/${orderId}/can-change-status?newStatus=${newStatus}`
    );
  },

  /**
   * Lấy số lượng đơn hàng (thống kê)
   */
  getOrderCount: async (
    storeId?: string,
    fromDate?: string,
    toDate?: string
  ): Promise<number> => {
    const params = new URLSearchParams();
    if (storeId) params.append('storeId', storeId);
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);

    const queryString = params.toString();
    const url = queryString ? `/Order/stats/count?${queryString}` : '/Order/stats/count';
    
    return apiRequest.get<number>(url);
  },

  /**
   * Lấy tổng doanh thu (thống kê)
   */
  getRevenue: async (
    storeId?: string,
    fromDate?: string,
    toDate?: string
  ): Promise<number> => {
    const params = new URLSearchParams();
    if (storeId) params.append('storeId', storeId);
    if (fromDate) params.append('fromDate', fromDate);
    if (toDate) params.append('toDate', toDate);

    const queryString = params.toString();
    const url = queryString ? `/Order/stats/revenue?${queryString}` : '/Order/stats/revenue';
    
    return apiRequest.get<number>(url);
  },

  /**
   * Cập nhật trạng thái nhiều đơn hàng (Bulk update - Admin only)
   */
  bulkUpdateStatus: async (
    orderIds: string[],
    newStatus: OrderStatus,
    reason?: string
  ): Promise<number> => {
    return apiRequest.post<number>('/Order/bulk-update-status', {
      orderIds,
      newStatus,
      reason,
    });
  },
};

export default checkoutService;

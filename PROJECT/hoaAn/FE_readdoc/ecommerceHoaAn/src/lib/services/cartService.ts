/**
 * Cart Service Layer - Quản lý giỏ hàng (cả user và guest)
 * Tích hợp với ASP.NET Core Cart API endpoints
 */

import { apiRequest } from '../api/client';

// ============================================================================
// Cart Types (dựa trên OpenAPI spec)
// ============================================================================

export interface CartItemDto {
  id: string;
  cartId: string;
  productId: string;
  productName: string;
  productCode: string;
  productImageUrl?: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
  isAvailable: boolean;
  stockQuantity: number;
}

export interface CartDto {
  id: string;
  userId?: string;
  sessionId?: string;
  items: CartItemDto[];
  itemCount: number;
  subtotal: number;
  shippingFee: number;
  taxAmount: number;
  discountAmount: number;
  couponCode?: string;
  totalAmount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CartSummaryDto {
  itemCount: number;
  subtotal: number;
  shippingFee: number;
  taxAmount: number;
  discountAmount: number;
  totalAmount: number;
}

export interface AddToCartRequest {
  productId: string;
  quantity: number;
}

export interface UpdateCartItemRequest {
  cartItemId: string;
  quantity: number;
}

export interface ApplyCouponRequest {
  couponCode: string;
}

export interface MergeCartRequest {
  sessionId: string;
}

export interface OrderShippingDto {
  recipientName: string;
  phoneNumber: string;
  address: string;
  ward: string;
  district: string;
  city: string;
  postalCode?: string;
  shippingMethod: 'standard' | 'express' | 'sameDay' | 'overnight';
  shippingFee?: number;
  deliveryNote?: string;
}

// ============================================================================
// Cart Service - Authenticated User
// ============================================================================

export const cartService = {
  /**
   * Lấy giỏ hàng của user hiện tại
   */
  getCart: async (): Promise<CartDto> => {
    return apiRequest.get<CartDto>('/Cart');
  },

  /**
   * Lấy tóm tắt giỏ hàng
   */
  getSummary: async (): Promise<CartSummaryDto> => {
    return apiRequest.get<CartSummaryDto>('/Cart/summary');
  },

  /**
   * Thêm sản phẩm vào giỏ hàng
   */
  addItem: async (request: AddToCartRequest): Promise<CartDto> => {
    return apiRequest.post<CartDto>('/Cart/add', request);
  },

  /**
   * Cập nhật số lượng item trong giỏ
   */
  updateItem: async (request: UpdateCartItemRequest): Promise<CartDto> => {
    return apiRequest.put<CartDto>('/Cart/update-item', request);
  },

  /**
   * Xóa item khỏi giỏ hàng
   */
  removeItem: async (cartItemId: string): Promise<CartDto> => {
    return apiRequest.delete<CartDto>(`/Cart/items/${cartItemId}`);
  },

  /**
   * Lấy thông tin một cart item cụ thể
   */
  getCartItem: async (cartItemId: string): Promise<CartItemDto> => {
    return apiRequest.get<CartItemDto>(`/Cart/items/${cartItemId}`);
  },

  /**
   * Xóa toàn bộ giỏ hàng
   */
  clearCart: async (): Promise<void> => {
    return apiRequest.delete<void>('/Cart/clear');
  },

  /**
   * Validate giỏ hàng (kiểm tra tồn kho, giá...)
   */
  validateCart: async (): Promise<CartDto> => {
    return apiRequest.post<CartDto>('/Cart/validate');
  },

  /**
   * Lấy số lượng items trong giỏ
   */
  getItemCount: async (): Promise<number> => {
    return apiRequest.get<number>('/Cart/item-count');
  },

  /**
   * Áp dụng mã giảm giá
   */
  applyCoupon: async (request: ApplyCouponRequest): Promise<CartDto> => {
    return apiRequest.post<CartDto>('/Cart/coupon/apply', request);
  },

  /**
   * Xóa mã giảm giá
   */
  removeCoupon: async (): Promise<CartDto> => {
    return apiRequest.post<CartDto>('/Cart/coupon/remove');
  },

  /**
   * Cập nhật thông tin giao hàng
   */
  updateShipping: async (shipping: OrderShippingDto): Promise<CartDto> => {
    return apiRequest.put<CartDto>('/Cart/shipping', shipping);
  },

  /**
   * Merge guest cart với user cart sau khi login
   */
  mergeCart: async (request: MergeCartRequest): Promise<CartDto> => {
    return apiRequest.post<CartDto>('/Cart/merge', request);
  },
};

// ============================================================================
// Guest Cart Service - Không cần authentication
// ============================================================================

export const guestCartService = {
  /**
   * Lấy giỏ hàng guest (dùng sessionId trong localStorage)
   */
  getCart: async (): Promise<CartDto> => {
    return apiRequest.get<CartDto>('/Cart/guest');
  },

  /**
   * Lấy tóm tắt giỏ hàng guest
   */
  getSummary: async (): Promise<CartSummaryDto> => {
    return apiRequest.get<CartSummaryDto>('/Cart/guest/summary');
  },

  /**
   * Thêm sản phẩm vào giỏ hàng guest
   */
  addItem: async (request: AddToCartRequest): Promise<CartDto> => {
    return apiRequest.post<CartDto>('/Cart/guest/add', request);
  },

  /**
   * Cập nhật item trong giỏ guest
   */
  updateItem: async (cartItemId: string, request: UpdateCartItemRequest): Promise<CartDto> => {
    return apiRequest.put<CartDto>(`/Cart/guest/items/${cartItemId}`, request);
  },

  /**
   * Xóa item khỏi giỏ guest
   */
  removeItem: async (cartItemId: string): Promise<CartDto> => {
    return apiRequest.delete<CartDto>(`/Cart/guest/items/${cartItemId}`);
  },

  /**
   * Xóa toàn bộ giỏ hàng guest
   */
  clearCart: async (): Promise<void> => {
    return apiRequest.delete<void>('/Cart/guest/clear');
  },

  /**
   * Validate giỏ hàng guest
   */
  validateCart: async (): Promise<CartDto> => {
    return apiRequest.post<CartDto>('/Cart/guest/validate');
  },

  /**
   * Lấy số lượng items trong giỏ guest
   */
  getItemCount: async (): Promise<number> => {
    return apiRequest.get<number>('/Cart/guest/item-count');
  },
};

// ============================================================================
// Unified Cart Service - Tự động chọn user/guest cart
// ============================================================================

export const unifiedCartService = {
  /**
   * Kiểm tra user có đang đăng nhập không
   */
  isAuthenticated: (): boolean => {
    return !!localStorage.getItem('access_token');
  },

  /**
   * Lấy giỏ hàng (tự động chọn user/guest)
   */
  getCart: async (): Promise<CartDto> => {
    return unifiedCartService.isAuthenticated()
      ? cartService.getCart()
      : guestCartService.getCart();
  },

  /**
   * Thêm sản phẩm vào giỏ
   */
  addItem: async (request: AddToCartRequest): Promise<CartDto> => {
    return unifiedCartService.isAuthenticated()
      ? cartService.addItem(request)
      : guestCartService.addItem(request);
  },

  /**
   * Xóa item
   */
  removeItem: async (cartItemId: string): Promise<CartDto> => {
    return unifiedCartService.isAuthenticated()
      ? cartService.removeItem(cartItemId)
      : guestCartService.removeItem(cartItemId);
  },

  /**
   * Lấy số lượng items
   */
  getItemCount: async (): Promise<number> => {
    return unifiedCartService.isAuthenticated()
      ? cartService.getItemCount()
      : guestCartService.getItemCount();
  },

  /**
   * Clear cart
   */
  clearCart: async (): Promise<void> => {
    return unifiedCartService.isAuthenticated()
      ? cartService.clearCart()
      : guestCartService.clearCart();
  },
};

export default cartService;

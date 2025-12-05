/**
 * Cart Service Layer - Quản lý giỏ hàng (cả user và guest)
 * ✅ UPDATED: Uses Orval generated API
 */

import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type {
  AddToCartDto,
  UpdateCartItemDto,
  ApplyCouponDto,
  MergeCartDto,
  OrderShippingDto,
} from '../../../Api/generated-orval/schemas';

const api = getVietCommerceAPI();

// ============================================================================
// Cart Types (for backward compatibility)
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

export { OrderShippingDto };

// ============================================================================
// Cart Service - Authenticated User
// ============================================================================

export const cartService = {
  /**
   * Lấy giỏ hàng của user hiện tại
   */
  getCart: async (): Promise<any> => {
    return api.getApiV1Cart();
  },

  /**
   * Lấy tóm tắt giỏ hàng
   */
  getSummary: async (): Promise<any> => {
    return api.getApiV1CartSummary();
  },

  /**
   * Thêm sản phẩm vào giỏ hàng
   */
  addItem: async (request: AddToCartRequest): Promise<any> => {
    const dto: AddToCartDto = {
      productId: request.productId,
      quantity: request.quantity,
    };
    return api.postApiV1CartAdd(dto);
  },

  /**
   * Cập nhật số lượng item trong giỏ
   */
  updateItem: async (request: UpdateCartItemRequest): Promise<any> => {
    const dto: UpdateCartItemDto = {
      cartItemId: request.cartItemId,
      quantity: request.quantity,
    };
    return api.putApiV1CartUpdateItem(dto);
  },

  /**
   * Xóa item khỏi giỏ hàng
   */
  removeItem: async (cartItemId: string): Promise<any> => {
    return api.deleteApiV1CartItemsCartItemId(cartItemId);
  },

  /**
   * Lấy thông tin một cart item cụ thể
   */
  getCartItem: async (cartItemId: string): Promise<any> => {
    return api.getApiV1CartItemsCartItemId(cartItemId);
  },

  /**
   * Xóa toàn bộ giỏ hàng
   */
  clearCart: async (): Promise<any> => {
    return api.deleteApiV1CartClear();
  },

  /**
   * Validate giỏ hàng (kiểm tra tồn kho, giá...)
   */
  validateCart: async (): Promise<any> => {
    return api.postApiV1CartValidate();
  },

  /**
   * Lấy số lượng items trong giỏ
   */
  getItemCount: async (): Promise<any> => {
    return api.getApiV1CartItemCount();
  },

  /**
   * Áp dụng mã giảm giá
   */
  applyCoupon: async (request: ApplyCouponRequest): Promise<any> => {
    const dto: ApplyCouponDto = {
      couponCode: request.couponCode,
    };
    return api.postApiV1CartCouponApply(dto);
  },

  /**
   * Xóa mã giảm giá
   */
  removeCoupon: async (): Promise<any> => {
    return api.postApiV1CartCouponRemove();
  },

  /**
   * Cập nhật thông tin giao hàng
   */
  updateShipping: async (shipping: OrderShippingDto): Promise<any> => {
    return api.putApiV1CartShipping(shipping);
  },

  /**
   * Merge guest cart với user cart sau khi login
   */
  mergeCart: async (request: MergeCartRequest): Promise<any> => {
    const dto: MergeCartDto = {
      sessionId: request.sessionId,
    };
    return api.postApiV1CartMerge(dto);
  },
};

// ============================================================================
// Guest Cart Service - Không cần authentication
// ============================================================================

export const guestCartService = {
  /**
   * Lấy giỏ hàng guest (dùng sessionId trong localStorage)
   */
  getCart: async (): Promise<any> => {
    return api.getApiV1CartGuest();
  },

  /**
   * Lấy tóm tắt giỏ hàng guest
   */
  getSummary: async (): Promise<any> => {
    return api.getApiV1CartGuestSummary();
  },

  /**
   * Thêm sản phẩm vào giỏ hàng guest
   */
  addItem: async (request: AddToCartRequest): Promise<any> => {
    const dto: AddToCartDto = {
      productId: request.productId,
      quantity: request.quantity,
    };
    return api.postApiV1CartGuestAdd(dto);
  },

  /**
   * Cập nhật item trong giỏ guest
   */
  updateItem: async (cartItemId: string, request: UpdateCartItemRequest): Promise<any> => {
    const dto: UpdateCartItemDto = {
      cartItemId: request.cartItemId,
      quantity: request.quantity,
    };
    return api.putApiV1CartGuestItemsCartItemId(cartItemId, dto);
  },

  /**
   * Xóa item khỏi giỏ guest
   */
  removeItem: async (cartItemId: string): Promise<any> => {
    return api.deleteApiV1CartGuestItemsCartItemId(cartItemId);
  },

  /**
   * Xóa toàn bộ giỏ hàng guest
   */
  clearCart: async (): Promise<any> => {
    return api.deleteApiV1CartGuestClear();
  },

  /**
   * Validate giỏ hàng guest
   */
  validateCart: async (): Promise<any> => {
    return api.postApiV1CartGuestValidate();
  },

  /**
   * Lấy số lượng items trong giỏ guest
   */
  getItemCount: async (): Promise<any> => {
    return api.getApiV1CartGuestItemCount();
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
    return !!localStorage.getItem('authToken') || !!sessionStorage.getItem('authToken');
  },

  /**
   * Lấy giỏ hàng (tự động chọn user/guest)
   */
  getCart: async (): Promise<any> => {
    return unifiedCartService.isAuthenticated()
      ? cartService.getCart()
      : guestCartService.getCart();
  },

  /**
   * Thêm sản phẩm vào giỏ
   */
  addItem: async (request: AddToCartRequest): Promise<any> => {
    return unifiedCartService.isAuthenticated()
      ? cartService.addItem(request)
      : guestCartService.addItem(request);
  },

  /**
   * Xóa item
   */
  removeItem: async (cartItemId: string): Promise<any> => {
    return unifiedCartService.isAuthenticated()
      ? cartService.removeItem(cartItemId)
      : guestCartService.removeItem(cartItemId);
  },

  /**
   * Lấy số lượng items
   */
  getItemCount: async (): Promise<any> => {
    return unifiedCartService.isAuthenticated()
      ? cartService.getItemCount()
      : guestCartService.getItemCount();
  },

  /**
   * Clear cart
   */
  clearCart: async (): Promise<any> => {
    return unifiedCartService.isAuthenticated()
      ? cartService.clearCart()
      : guestCartService.clearCart();
  },
};

export default cartService;

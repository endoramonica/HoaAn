import { useState, useEffect, useCallback } from "react";
import { CartService } from "@/api/services/CartService";
import type { UpdateCartItemDto } from "@/api/models/UpdateCartItemDto";
import { type  ApplyCouponDto } from "@/api/models/ApplyCouponDto";
import { useAuth } from "./useAuth";

interface CartItem {
  id: string;
  productId: string;
  name: string;
  price: number;
  originalPrice?: number;
  quantity: number;
  image: string;
  category: string;
  inStock: boolean;
  maxQuantity: number;
}

interface CartSummary {
  subtotal: number;
  discount: number;
  shippingFee: number;
  total: number;
  itemCount: number;
  appliedCoupon?: string;
}

interface UseCartReturn {
  cartItems: CartItem[];
  summary: CartSummary | null;
  isLoading: boolean;
  error: string | null;
  isGuest: boolean;
  updateQuantity: (cartItemId: string, quantity: number) => Promise<void>;
  removeItem: (cartItemId: string) => Promise<void>;
  clearCart: () => Promise<void>;
  applyCoupon: (couponCode: string) => Promise<void>;
  removeCoupon: () => Promise<void>;
  refreshCart: () => Promise<void>;
  validateCart: () => Promise<void>;
}

/**
 * useCart Hook
 * Quản lý giỏ hàng cho cả user và guest
 * - Tự động phát hiện user/guest từ useAuth
 * - Backend xử lý sessionId qua HTTP-only cookie
 * - Tự động refresh khi auth status thay đổi
 */
export function useCart(): UseCartReturn {
  const { isAuthenticated } = useAuth();
  const isGuest = !isAuthenticated;

  const [cartItems, setCartItems] = useState<CartItem[]>([]);
  const [summary, setSummary] = useState<CartSummary | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch cart data
  const fetchCart = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      // Backend tự động xử lý sessionId qua cookie
      // Không cần truyền sessionId từ frontend
      const cartResponse = isGuest
        ? await CartService.getApiV1CartGuest()
        : await CartService.getApiV1Cart();

      const summaryResponse = isGuest
        ? await CartService.getApiV1CartGuestSummary()
        : await CartService.getApiV1CartSummary();

      // Transform API response to CartItem format
      if (cartResponse?.data?.items) {
        const transformedItems: CartItem[] = cartResponse.data.items.map((item: any) => ({
          id: item.id || item.cartItemId,
          productId: item.productId,
          name: item.productName || item.name,
          price: item.unitPrice || item.price,
          originalPrice: item.originalPrice,
          quantity: item.quantity,
          image: item.productImage || item.imageUrl || item.image,
          category: item.category || "",
          inStock: item.stockAvailable > 0,
          maxQuantity: item.stockAvailable || 99,
        }));
        setCartItems(transformedItems);
      } else {
        setCartItems([]);
      }

      // Transform summary response
      if (summaryResponse?.data) {
        const data = summaryResponse.data;
        setSummary({
          subtotal: data.subTotal || data.subtotal || 0,
          discount: data.discount || 0,
          shippingFee: data.shippingFee || 0,
          total: data.totalAmount || data.total || 0,
          itemCount: data.totalItems || data.itemCount || 0,
          appliedCoupon: data.appliedCoupon || data.couponCode,
        });
      }
    } catch (err: any) {
      // Nếu cart không tồn tại (404), không coi là lỗi
      if (err?.status === 404 || err?.message?.includes('Not Found')) {
        setCartItems([]);
        setSummary(null);
        setError(null); // ✅ Clear error
        console.log(`📦 ${isGuest ? 'Guest' : 'User'} cart is empty`);
      } else {
        setError(err.message || "Không thể tải giỏ hàng");
        console.error("Error fetching cart:", err);
      }
    } finally {
      setIsLoading(false);
    }
  }, [isGuest]);

  // Update item quantity
  const updateQuantity = useCallback(
    async (cartItemId: string, quantity: number) => {
      try {
        const updateDto: UpdateCartItemDto = { 
          cartItemId,
          quantity 
        };

        if (isGuest) {
          // Backend tự lấy sessionId từ cookie
          await CartService.putApiV1CartGuestItems(cartItemId, updateDto);
        } else {
          await CartService.putApiV1CartUpdateItem(updateDto);
        }

        // Refresh cart after update
        await fetchCart();
      } catch (err: any) {
        setError(err.message || "Không thể cập nhật số lượng");
        throw err;
      }
    },
    [isGuest, fetchCart]
  );

  // Remove item from cart
  const removeItem = useCallback(
    async (cartItemId: string) => {
      try {
        if (isGuest) {
          await CartService.deleteApiV1CartGuestItems(cartItemId);
        } else {
          await CartService.deleteApiV1CartItems(cartItemId);
        }

        // Refresh cart after removal
        await fetchCart();
      } catch (err: any) {
        setError(err.message || "Không thể xóa sản phẩm");
        throw err;
      }
    },
    [isGuest, fetchCart]
  );

  // Clear entire cart
  const clearCart = useCallback(async () => {
    try {
      if (isGuest) {
        await CartService.deleteApiV1CartGuestClear();
      } else {
        await CartService.deleteApiV1CartClear();
      }

      setCartItems([]);
      setSummary(null);
    } catch (err: any) {
      setError(err.message || "Không thể xóa giỏ hàng");
      throw err;
    }
  }, [isGuest]);

  // Apply coupon
  const applyCoupon = useCallback(
    async (couponCode: string) => {
      try {
        const couponDto: ApplyCouponDto = { couponCode };
        await CartService.postApiV1CartCouponApply(couponDto);

        // Refresh cart to get updated pricing
        await fetchCart();
      } catch (err: any) {
        setError(err.message || "Mã giảm giá không hợp lệ");
        throw err;
      }
    },
    [fetchCart]
  );

  // Remove coupon
  const removeCoupon = useCallback(async () => {
    try {
      await CartService.postApiV1CartCouponRemove();

      // Refresh cart to get updated pricing
      await fetchCart();
    } catch (err: any) {
      setError(err.message || "Không thể xóa mã giảm giá");
      throw err;
    }
  }, [fetchCart]);

  // Validate cart
  const validateCart = useCallback(async () => {
    try {
      if (isGuest) {
        await CartService.postApiV1CartGuestValidate();
      } else {
        await CartService.postApiV1CartValidate();
      }
    } catch (err: any) {
      setError(err.message || "Giỏ hàng có sản phẩm không hợp lệ");
      throw err;
    }
  }, [isGuest]);

  // Load cart on mount and when auth status changes
  useEffect(() => {
    fetchCart();
  }, [fetchCart, isAuthenticated]);

  return {
    cartItems,
    summary,
    isLoading,
    error,
    isGuest,
    updateQuantity,
    removeItem,
    clearCart,
    applyCoupon,
    removeCoupon,
    refreshCart: fetchCart,
    validateCart,
  };
}
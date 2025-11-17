import { useState, useEffect, useCallback } from "react";
import { CartService } from "@/api/services/CartService";
import type { UpdateCartItemDto } from "@/api/models/UpdateCartItemDto";
import { type ApplyCouponDto } from "@/api/models/ApplyCouponDto";
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
  cartItemCount: number; // ✅ ADDED: Quick count for badge
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
  const [cartItemCount, setCartItemCount] = useState<number>(0); // ✅ ADDED
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // ✅ NEW: Fetch cart item count (lightweight for badge)
  const fetchCartCount = useCallback(async () => {
    try {
      const response = isGuest
        ? await CartService.getApiV1CartGuestItemCount()
        : await CartService.getApiV1CartItemCount();

      // Backend có thể trả về: { count: 5 } hoặc { data: 5 } hoặc trực tiếp 5
      const count = response?.data?.count ?? response?.data ?? response ?? 0;
      setCartItemCount(count);
    } catch (err: any) {
      // Silent fail - không hiển thị error cho count
      if (err?.status !== 404 && err?.status !== 400) {
        console.warn('Failed to fetch cart count:', err.message);
      }
      setCartItemCount(0);
    }
  }, [isGuest]);

  // Fetch cart data
  const fetchCart = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

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
        
        // ✅ Update count from summary
        setCartItemCount(data.totalItems || data.itemCount || 0);
      }
    } catch (err: any) {
      if (err?.status === 404 || err?.message?.includes('Not Found')) {
        setCartItems([]);
        setSummary(null);
        setCartItemCount(0); // ✅ Reset count
        setError(null);
        console.log(`📦 ${isGuest ? 'Guest' : 'User'} cart is empty`);
      } else if (err?.status === 400) {
        setCartItems([]);
        setSummary(null);
        setCartItemCount(0); // ✅ Reset count
        setError(null);
        console.warn(`⚠️ ${isGuest ? 'Guest' : 'User'} cart not initialized yet`);
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
        if (isGuest) {
          const updateDto: UpdateCartItemDto = { quantity };
          await CartService.putApiV1CartGuestItems(cartItemId, updateDto);
        } else {
          const updateDto: UpdateCartItemDto = {
            cartItemId,
            quantity,
          };
          await CartService.putApiV1CartUpdateItem(updateDto);
        }

        // ✅ Refresh both count and cart
        await Promise.all([fetchCartCount(), fetchCart()]);
      } catch (err: any) {
        setError(err.message || "Không thể cập nhật số lượng");
        throw err;
      }
    },
    [isGuest, fetchCart, fetchCartCount]
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

        // ✅ Refresh both count and cart
        await Promise.all([fetchCartCount(), fetchCart()]);
      } catch (err: any) {
        setError(err.message || "Không thể xóa sản phẩm");
        throw err;
      }
    },
    [isGuest, fetchCart, fetchCartCount]
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
      setCartItemCount(0); // ✅ Reset count
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

  // ✅ Load cart and count on mount and when auth status changes
  useEffect(() => {
    fetchCartCount(); // Lightweight count first
    fetchCart();       // Full cart data
  }, [fetchCartCount, fetchCart, isAuthenticated]);

  return {
    cartItems,
    summary,
    cartItemCount, // ✅ EXPOSED
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
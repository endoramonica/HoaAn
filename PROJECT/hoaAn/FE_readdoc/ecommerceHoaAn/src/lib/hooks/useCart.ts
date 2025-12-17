/**
 * useCart Hook - UPDATED for Checkout Flow
 * ✅ Returns proper cart object with id for checkout
 * ✅ Handles both guest and authenticated users
 * ✅ Auto-detects user status from useAuth
 */

import { useState, useEffect, useCallback } from "react";
import { CartService } from "@/api/services/CartService";
import type { UpdateCartItemDto } from "@/api/models/UpdateCartItemDto";
import { type ApplyCouponDto } from "@/api/models/ApplyCouponDto";
import type { CartResponseDto, CartSummaryResponseDto, CartItemDto } from "@/lib/api/types";
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
  basePrice?: number;
  customizationPrice?: number;
  finalPrice?: number;
  customizations?: Array<{
    optionId?: string;
    quantity?: number;
    unitPrice?: number;
    totalPrice?: number;
  }>;
}

interface Cart {
  id: string;
  items: CartItem[];
  subtotal: number;
  discount: number;
  shippingFee: number;
  totalAmount: number;
  itemCount: number;
  couponCode?: string;
}

interface UseCartReturn {
  // ✅ NEW: Return full cart object for checkout
  cart: Cart | null;

  // Legacy support (deprecated but kept for compatibility)
  cartItems: CartItem[];
  summary: {
    subtotal: number;
    discount: number;
    shippingFee: number;
    total: number;
    itemCount: number;
    appliedCoupon?: string;
  } | null;

  cartItemCount: number;
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
  const { isAuthenticated, isLoading: authLoading } = useAuth();
  const isGuest = !isAuthenticated;

  const [cart, setCart] = useState<Cart | null>(null);
  const [cartItemCount, setCartItemCount] = useState<number>(0);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // ✅ Fetch cart item count (lightweight for badge)
  const fetchCartCount = useCallback(async () => {
    try {
      const response = isGuest
        ? await CartService.getApiV1CartGuestItemCount()
        : await CartService.getApiV1CartItemCount();

      const count = response?.data?.count ?? response?.data ?? response ?? 0;
      setCartItemCount(count);
      return count;
    } catch (err: any) {
      if (err?.status !== 404 && err?.status !== 400) {
        console.warn('Failed to fetch cart count:', err.message);
      }
      setCartItemCount(0);
      return 0;
    }
  }, [isGuest]);

  // ✅ Fetch full cart data
  const fetchCart = useCallback(async () => {
    // For guest users, ensure we have a sessionId
    if (isGuest) {
      let sessionId = localStorage.getItem('guest_cart_session_id');
      if (!sessionId) {
        sessionId = `guest_${Date.now()}_${Math.random().toString(36).substring(2, 11)}`;
        localStorage.setItem('guest_cart_session_id', sessionId);
        console.log('[useCart] 🆕 Created guest cart sessionId:', sessionId);
      }
    }

    try {
      // ✅ ADD THIS: Log before API call
      console.log('[useCart] 🔍 Fetching cart...');
      console.log('[useCart] 📋 Auth Token:', localStorage.getItem('auth_token')?.substring(0, 20) + '...');
      console.log('[useCart] 👤 Is Guest:', isGuest);

      const cartResponse = (isGuest
        ? await CartService.getApiV1CartGuest()
        : await CartService.getApiV1Cart()) as CartResponseDto;

      // ✅ ADD THIS: Log response
      console.log('[useCart] ✅ Cart Response:', {
        cartId: cartResponse?.data?.cartId,
        userId: cartResponse?.data?.userId,
        itemCount: cartResponse?.data?.totalItems
      });

      const summaryResponse = (isGuest
        ? await CartService.getApiV1CartGuestSummary()
        : await CartService.getApiV1CartSummary()) as CartSummaryResponseDto;

      // ✅ Transform API response to CartItem format
      const transformedItems: CartItem[] = cartResponse?.data?.items
        ? cartResponse.data.items.map((item: CartItemDto) => ({
          id: item.cartItemId,
          productId: item.productId,
          name: item.productName,
          price: item.unitPrice,
          originalPrice: undefined,
          quantity: item.quantity,
          image: item.productImage,
          category: "",
          inStock: item.availableStock > 0,
          maxQuantity: item.availableStock,
          basePrice: item.basePrice,
          customizationPrice: item.customizationPrice,
          finalPrice: item.finalPrice,
          customizations: item.customizations,
        }))
        : [];

      // ✅ Build complete cart object
      const summaryData = summaryResponse?.data;
      const cartData = cartResponse?.data;

      if (transformedItems.length > 0 || summaryData || cartData) {
        const newCart = {
          id: cartData?.cartId || 'guest-cart',
          items: transformedItems,
          subtotal: cartData?.subTotal || summaryData?.subTotal || 0,
          discount: cartData?.taxAmount || summaryData?.taxAmount || 0,
          shippingFee: cartData?.shippingFee || summaryData?.shippingFee || 0,
          totalAmount: cartData?.totalAmount || summaryData?.totalAmount || 0,
          itemCount: cartData?.totalItems || summaryData?.totalItems || transformedItems.length,
          couponCode: summaryData?.appliedCoupon,
        };

        setCart(newCart);
        setCartItemCount(newCart.itemCount);
      } else {
        setCart(null);
        setCartItemCount(0);
      }

      setError(null);
    } catch (err: any) {
      if (err?.status === 404 || err?.message?.includes('Not Found')) {
        setCart(null);
        setCartItemCount(0);
        setError(null);
        console.log(`📦 ${isGuest ? 'Guest' : 'User'} cart is empty`);
      } else if (err?.status === 400) {
        setCart(null);
        setCartItemCount(0);
        setError(null);
        console.warn(`⚠️ ${isGuest ? 'Guest' : 'User'} cart not initialized yet`);
      } else {
        setError(err.message || "Không thể tải giỏ hàng");
        console.error("Error fetching cart:", err);
      }
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
        console.log('[useCart] 🗑️ Removing item:', cartItemId);

        if (isGuest) {
          await CartService.deleteApiV1CartGuestItems(cartItemId);
        } else {
          await CartService.deleteApiV1CartItems(cartItemId);
        }

        console.log('[useCart] ✅ Item removed successfully');
        await Promise.all([fetchCartCount(), fetchCart()]);
      } catch (err: any) {
        console.error('[useCart] ❌ Error removing item:', err);
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

      setCart(null);
      setCartItemCount(0);
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

  // ✅ Load cart on mount and when auth status changes
  // CRITICAL: Wait for auth to finish loading before fetching cart
  useEffect(() => {
    // Don't fetch cart while auth is still loading
    if (authLoading) {
      console.log('[useCart] ⏳ Waiting for auth to finish loading...');
      return;
    }

    let isMounted = true;

    const loadCart = async () => {
      setIsLoading(true);
      setError(null);

      console.log(`[useCart] 🔄 Loading cart for ${isGuest ? 'guest' : 'user'}...`);

      try {
        // Fetch both cart data and count in parallel
        await Promise.all([
          fetchCart(),
          fetchCartCount()
        ]);

        console.log('[useCart] ✅ Cart loaded successfully');
      } catch (err: any) {
        console.error('[useCart] ❌ Error loading cart:', err);
      } finally {
        if (isMounted) {
          setIsLoading(false);
        }
      }
    };

    loadCart();

    return () => {
      isMounted = false;
    };
  }, [authLoading, isAuthenticated, isGuest]);

  return {
    // ✅ NEW: Full cart object for checkout
    cart,

    // Legacy support (deprecated)
    cartItems: cart?.items || [],
    summary: cart ? {
      subtotal: cart.subtotal,
      discount: cart.discount,
      shippingFee: cart.shippingFee,
      total: cart.totalAmount,
      itemCount: cart.itemCount,
      appliedCoupon: cart.couponCode,
    } : null,

    cartItemCount,
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

import { useCallback, useEffect, useState } from "react";
import { toast } from "sonner";
import wishlistService from "../services/wishlistService"; // Import service tự viết
import type { WishlistItemDto } from "../api/types";

export const useWishlist = () => {
  const [wishlistItems, setWishlistItems] = useState<WishlistItemDto[]>([]);
  const [wishlistCount, setWishlistCount] = useState<number>(0); // Badge count
  const [loading, setLoading] = useState(false);

  // ===============================
  // Load count nhanh (dùng cho badge)
  // ===============================
  const loadWishlistCount = useCallback(async () => {
    try {
      const count = await wishlistService.getWishlistCount();
      setWishlistCount(count);
    } catch (error) {
      console.error("Load wishlist count error:", error);
    }
  }, []);

  // ===============================
  // Load full list (khi cần hiển thị danh sách)
  // ===============================
  const loadWishlist = useCallback(async () => {
    setLoading(true);
    try {
      const items = await wishlistService.getWishlist();
      setWishlistItems(items);
      setWishlistCount(items.length); // Đồng bộ badge
    } catch (error) {
      toast.error("Không thể tải danh sách yêu thích");
    } finally {
      setLoading(false);
    }
  }, []);

  // ===============================
  // Refresh count sau mỗi thay đổi
  // ===============================
  const refreshCount = useCallback(() => {
    loadWishlistCount();
  }, [loadWishlistCount]);

  // ===============================
  // Actions
  // ===============================
  const addToWishlist = useCallback(
    async (productId: string) => {
      try {
        const newItem = await wishlistService.addToWishlist(productId);
        toast.success("Đã thêm vào danh sách yêu thích");
        setWishlistItems((prev) => [...prev, newItem]);
        refreshCount();
        return newItem;
      } catch (error: any) {
        toast.error(error.message || "Không thể thêm sản phẩm");
        return null;
      }
    },
    [refreshCount]
  );

  const removeFromWishlist = useCallback(
    async (id: string) => {
      try {
        await wishlistService.removeFromWishlist(id);
        setWishlistItems((prev) => prev.filter((x) => x.id !== id));
        toast.success("Đã xóa khỏi danh sách yêu thích");
        refreshCount();
      } catch (error) {
        toast.error("Không thể xóa sản phẩm");
      }
    },
    [refreshCount]
  );

  const removeByProductId = useCallback(
    async (productId: string) => {
      try {
        await wishlistService.removeFromWishlistByProductId(productId);
        setWishlistItems((prev) => prev.filter((x) => x.productId !== productId));
        toast.success("Đã xóa khỏi danh sách yêu thích");
        refreshCount();
      } catch (error) {
        toast.error("Không thể xóa sản phẩm");
      }
    },
    [refreshCount]
  );

  const clearWishlist = useCallback(async () => {
    try {
      await wishlistService.clearWishlist();
      setWishlistItems([]);
      setWishlistCount(0);
      toast.success("Đã xóa toàn bộ danh sách yêu thích");
    } catch (error) {
      toast.error("Không thể xóa toàn bộ");
    }
  }, []);

  const toggleWishlist = useCallback(
    async (productId: string): Promise<boolean> => {
      try {
        const result = await wishlistService.toggleWishlist(productId);
        if (result.isInWishlist) {
          toast.success("Đã thêm vào danh sách yêu thích");
          setWishlistCount((prev) => prev + 1);
        } else {
          toast.success("Đã xóa khỏi danh sách yêu thích");
          setWishlistCount((prev) => Math.max(0, prev - 1));
          setWishlistItems((prev) => prev.filter((x) => x.productId !== productId));
        }
        return result.isInWishlist;
      } catch (error: any) {
        toast.error(error.message || "Lỗi toggle wishlist");
        return false;
      }
    },
    []
  );

  const isInWishlist = useCallback(
    (productId: string) => wishlistItems.some((x) => x.productId === productId),
    [wishlistItems]
  );

  // ===============================
  // Mount: load count trước, list sau
  // ===============================
  useEffect(() => {
    loadWishlistCount(); // Badge hiển thị ngay
    loadWishlist(); // Load list chi tiết
  }, [loadWishlistCount, loadWishlist]);

  return {
    // State
    wishlistItems,
    wishlistCount,
    loading,
    isEmpty: wishlistCount === 0,

    // Actions
    loadWishlist,
    loadWishlistCount,
    addToWishlist,
    removeFromWishlist,
    removeByProductId,
    clearWishlist,
    toggleWishlist,
    isInWishlist,
  };
};

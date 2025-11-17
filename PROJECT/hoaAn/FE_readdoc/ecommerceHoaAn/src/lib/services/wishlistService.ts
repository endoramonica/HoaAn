/**
 * Wishlist Service - Quản lý danh sách yêu thích
 * Tương thích với ASP.NET Core backend API
 * Route: /api/v1/wishlist
 */

import apiClient from '../api/client';
import type {
  WishlistItemDto,
  WishlistSummaryDto,
  AddToWishlistRequest,
  ApiResponse,
} from '../api/types';

// Mock data cho development khi backend chưa sẵn sàng
const getMockMode = () => {
  if (typeof import.meta !== 'undefined' && import.meta.env) {
    return import.meta.env.VITE_USE_MOCK_DATA === 'true';
  }
  return true; // Default to mock mode trong Figma Make
};

const MOCK_MODE = getMockMode();

// Mock wishlist data
const mockWishlistItems: WishlistItemDto[] = [];

/**
 * Lấy danh sách wishlist của user hiện tại
 * GET /api/v1/wishlist
 */
export const getWishlist = async (): Promise<WishlistItemDto[]> => {
  if (MOCK_MODE) {
    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 500));
    return [...mockWishlistItems];
  }

  // ✅ Fixed: Bỏ /api/v1 vì đã có trong baseURL
  const response = await apiClient.get<ApiResponse<WishlistItemDto[]>>('/wishlist');
  return response.data.data;
};

/**
 * Lấy tổng quan wishlist (tổng số item, items)
 * GET /api/v1/wishlist/summary
 */
export const getWishlistSummary = async (): Promise<WishlistSummaryDto> => {
  if (MOCK_MODE) {
    await new Promise(resolve => setTimeout(resolve, 300));
    return {
      totalItems: mockWishlistItems.length,
      items: [...mockWishlistItems]
    };
  }

  // ✅ Fixed
  const response = await apiClient.get<ApiResponse<WishlistSummaryDto>>('/wishlist/summary');
  return response.data.data;
};

/**
 * Thêm sản phẩm vào wishlist
 * POST /api/v1/wishlist
 * @param productId - ID của sản phẩm cần thêm
 */
export const addToWishlist = async (productId: string): Promise<WishlistItemDto> => {
  if (MOCK_MODE) {
    await new Promise(resolve => setTimeout(resolve, 400));
    
    // Kiểm tra xem sản phẩm đã có trong wishlist chưa
    const existingItem = mockWishlistItems.find(item => item.productId === productId);
    if (existingItem) {
      throw new Error('Sản phẩm đã có trong danh sách yêu thích');
    }

    // Tạo mock wishlist item
    const newItem: WishlistItemDto = {
      id: `wishlist-${Date.now()}`,
      userId: 'mock-user-id',
      productId,
      product: {
        id: productId,
        name: 'Sản phẩm mock',
        slug: 'san-pham-mock',
        description: 'Mô tả sản phẩm mock',
        price: 100000,
        categoryId: 'cat-1',
        category: {
          id: 'cat-1',
          name: 'Danh mục mock',
          slug: 'danh-muc-mock',
          sortOrder: 1,
          isActive: true
        },
        images: [],
        stock: 100,
        isActive: true,
        isFeatured: false,
        rating: 4.5,
        reviewCount: 10,
        tags: [],
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      },
      createdAt: new Date().toISOString()
    };

    mockWishlistItems.push(newItem);
    return newItem;
  }

  // ✅ Fixed
  const request: AddToWishlistRequest = { productId };
  const response = await apiClient.post<ApiResponse<WishlistItemDto>>('/wishlist', request);
  return response.data.data;
};

/**
 * Xóa sản phẩm khỏi wishlist
 * DELETE /api/v1/wishlist/{id}
 * @param wishlistItemId - ID của wishlist item cần xóa
 */
export const removeFromWishlist = async (wishlistItemId: string): Promise<void> => {
  if (MOCK_MODE) {
    await new Promise(resolve => setTimeout(resolve, 400));
    const index = mockWishlistItems.findIndex(item => item.id === wishlistItemId);
    if (index > -1) {
      mockWishlistItems.splice(index, 1);
    }
    return;
  }

  // ✅ Fixed
  await apiClient.delete(`/wishlist/${wishlistItemId}`);
};

/**
 * Xóa sản phẩm khỏi wishlist theo productId
 * DELETE /api/v1/wishlist/product/{productId}
 * @param productId - ID của sản phẩm cần xóa khỏi wishlist
 */
export const removeFromWishlistByProductId = async (productId: string): Promise<void> => {
  if (MOCK_MODE) {
    await new Promise(resolve => setTimeout(resolve, 400));
    const index = mockWishlistItems.findIndex(item => item.productId === productId);
    if (index > -1) {
      mockWishlistItems.splice(index, 1);
    }
    return;
  }

  // ✅ Fixed
  await apiClient.delete(`/wishlist/product/${productId}`);
};

/**
 * Kiểm tra sản phẩm có trong wishlist không
 * GET /api/v1/wishlist/check/{productId}
 * @param productId - ID của sản phẩm cần kiểm tra
 */
export const isInWishlist = async (productId: string): Promise<boolean> => {
  if (MOCK_MODE) {
    await new Promise(resolve => setTimeout(resolve, 200));
    return mockWishlistItems.some(item => item.productId === productId);
  }

  // ✅ Fixed
  const response = await apiClient.get<ApiResponse<{ isInWishlist: boolean }>>(
    `/wishlist/check/${productId}`
  );
  return response.data.data.isInWishlist;
};

/**
 * Xóa toàn bộ wishlist
 * DELETE /api/v1/wishlist/clear
 */
export const clearWishlist = async (): Promise<void> => {
  if (MOCK_MODE) {
    await new Promise(resolve => setTimeout(resolve, 400));
    mockWishlistItems.length = 0;
    return;
  }

  // ✅ Fixed
  await apiClient.delete('/wishlist/clear');
};

/**
 * Di chuyển tất cả wishlist items vào giỏ hàng
 * POST /api/v1/wishlist/move-to-cart
 */
export const moveAllToCart = async (): Promise<void> => {
  if (MOCK_MODE) {
    await new Promise(resolve => setTimeout(resolve, 600));
    // Mock: xóa toàn bộ wishlist sau khi di chuyển vào giỏ hàng
    mockWishlistItems.length = 0;
    return;
  }

  // ✅ Fixed
  await apiClient.post('/wishlist/move-to-cart');
};

/**
 * Toggle wishlist - thêm nếu chưa có, xóa nếu đã có
 * POST /api/v1/wishlist/toggle
 * @param productId - ID của sản phẩm
 */
export const toggleWishlist = async (productId: string): Promise<{ isInWishlist: boolean }> => {
  if (MOCK_MODE) {
    await new Promise(resolve => setTimeout(resolve, 400));
    const index = mockWishlistItems.findIndex(item => item.productId === productId);
    
    if (index > -1) {
      // Đã có trong wishlist -> xóa
      mockWishlistItems.splice(index, 1);
      return { isInWishlist: false };
    } else {
      // Chưa có trong wishlist -> thêm
      const newItem: WishlistItemDto = {
        id: `wishlist-${Date.now()}`,
        userId: 'mock-user-id',
        productId,
        product: {
          id: productId,
          name: 'Sản phẩm mock',
          slug: 'san-pham-mock',
          description: 'Mô tả sản phẩm mock',
          price: 100000,
          categoryId: 'cat-1',
          category: {
            id: 'cat-1',
            name: 'Danh mục mock',
            slug: 'danh-muc-mock',
            sortOrder: 1,
            isActive: true
          },
          images: [],
          stock: 100,
          isActive: true,
          isFeatured: false,
          rating: 4.5,
          reviewCount: 10,
          tags: [],
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        },
        createdAt: new Date().toISOString()
      };
      mockWishlistItems.push(newItem);
      return { isInWishlist: true };
    }
  }
 

  // ✅ Fixed
  const response = await apiClient.post<ApiResponse<{ isInWishlist: boolean }>>(
    '/wishlist/toggle',
    { productId }
  );
  return response.data.data;
};
 /**
 * Lấy số lượng wishlist items (cho badge count)
 * GET /api/v1/wishlist/count
 */
 export const getWishlistCount = async (): Promise<number> => {
  if (MOCK_MODE) {
    await new Promise(resolve => setTimeout(resolve, 200));
    return mockWishlistItems.length;
  }

  const response = await apiClient.get<ApiResponse<number>>('/wishlist/count');
  return response.data.data; // ✅ FIX: response.data.data
};
  
const wishlistService = {
  getWishlist,
  getWishlistSummary,
  addToWishlist,
  removeFromWishlist,
  removeFromWishlistByProductId,
  isInWishlist,
  clearWishlist,
  moveAllToCart,
  getWishlistCount,
  toggleWishlist
};

export default wishlistService;
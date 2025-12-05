/**
 * VietCommerce Product Service
 * ✅ UPDATED: Uses Orval generated API
 * Base route: /api/v1/Product
 */

import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type {
  GetApiV1ProductParams,
  ProductCreateDto,
  ProductUpdateDto,
  PatchApiV1ProductIdStockBody,
  PatchApiV1ProductIdActiveBody,
  PatchApiV1ProductIdFeaturedBody,
} from '../../../Api/generated-orval/schemas';

// Types for backward compatibility
export interface ProductListDto {
  id?: string;
  name?: string;
  slug?: string;
  price?: number;
  stock?: number;
  stockQuantity?: number;
  thumbnailUrl?: string;
  primaryImage?: string;
  categoryName?: string;
  storeName?: string;
  inStock?: boolean;
}

export interface ProductDetailDto extends ProductListDto {
  code?: string;
  categoryId?: string;
  storeId?: string;
  sku?: string;
  isActive?: boolean;
  description?: string;
  images?: string[];
  createdAt?: string;
  updatedAt?: string;
}

export interface ProductFilterDto {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  categoryId?: string;
  storeId?: string;
  minPrice?: number;
  maxPrice?: number;
  isActive?: boolean;
  isFeatured?: boolean;
  sortBy?: string;
  isDescending?: boolean;
}

export interface PaginatedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

const api = getVietCommerceAPI();

class VietCommerceProductService {
  /**
   * GET /api/v1/Product
   * Lấy danh sách sản phẩm với filter/pagination
   */
  async getProducts(filter?: ProductFilterDto): Promise<PaginatedResult<ProductListDto>> {
    try {
      // Map filter to Orval params
      const params: GetApiV1ProductParams = {
        pageNumber: filter?.pageNumber,
        pageSize: filter?.pageSize,
        searchTerm: filter?.searchTerm,
        categoryId: filter?.categoryId,
        storeId: filter?.storeId,
        minPrice: filter?.minPrice,
        maxPrice: filter?.maxPrice,
        isActive: filter?.isActive,
        isFeatured: filter?.isFeatured,
        sortBy: filter?.sortBy,
        isDescending: filter?.isDescending,
      };

      const response = await api.getApiV1Product(params);

      // Debug log
      console.log('[vietCommerceProductService] Raw response:', JSON.stringify(response, null, 2));

      // Handle nested response structure: response.data.data (backend wraps twice)
      // Structure: { success, data: { success, data: { items, pageNumber, ... } } }
      const outerData = response.data;
      const innerData = (outerData as any)?.data || outerData;

      console.log('[vietCommerceProductService] Parsed data:', {
        hasOuterData: !!outerData,
        hasInnerData: !!innerData,
        itemsCount: innerData?.items?.length,
      });

      return {
        items: (innerData?.items || []).map((item: any) => ({
          id: item.id,
          name: item.name,
          slug: item.slug,
          price: item.price || item.displayPrice?.discountedPrice || 0,
          stock: item.stockQuantity,
          stockQuantity: item.stockQuantity,
          thumbnailUrl: item.primaryImage,
          primaryImage: item.primaryImage,
          categoryName: item.categoryName,
          storeName: item.storeName,
          inStock: item.inStock ?? (item.stockQuantity || 0) > 0,
        })),
        pageNumber: innerData?.pageNumber || 1,
        pageSize: innerData?.pageSize || 12,
        totalItems: innerData?.totalItems || 0,
        totalPages: innerData?.totalPages || 0,
      };
    } catch (error) {
      console.error('Get products error:', error);
      throw error;
    }
  }

  /**
   * GET /api/v1/Product/{id}
   * Lấy chi tiết sản phẩm theo ID
   */
  async getProductById(id: string): Promise<ProductDetailDto> {
    try {
      const response = await api.getApiV1ProductId(id);
      const data = response.data;

      return {
        id: data?.id,
        name: data?.name,
        slug: data?.slug,
        code: data?.code,
        categoryId: data?.categoryId,
        storeId: data?.storeId,
        sku: data?.sku,
        price: data?.price,
        stock: data?.stockQuantity,
        stockQuantity: data?.stockQuantity,
        isActive: data?.isActive,
        description: data?.description,
        thumbnailUrl: data?.primaryImage,
        primaryImage: data?.primaryImage,
        images: data?.images?.map(img => img.imageUrl || '') || [],
        categoryName: data?.categoryName,
        storeName: data?.storeName,
        createdAt: data?.createdAt,
        updatedAt: data?.updatedAt,
        inStock: (data?.stockQuantity || 0) > 0,
      };
    } catch (error) {
      console.error('Get product by id error:', error);
      throw error;
    }
  }

  /**
   * GET /api/v1/Product/slug/{slug}
   * Lấy sản phẩm theo slug
   */
  async getProductBySlug(slug: string): Promise<ProductDetailDto> {
    try {
      const response = await api.getApiV1ProductSlugSlug(slug);
      const data = response.data;

      return {
        id: data?.id,
        name: data?.name,
        slug: data?.slug,
        code: data?.code,
        categoryId: data?.categoryId,
        storeId: data?.storeId,
        sku: data?.sku,
        price: data?.price,
        stock: data?.stockQuantity,
        stockQuantity: data?.stockQuantity,
        isActive: data?.isActive,
        description: data?.description,
        thumbnailUrl: data?.primaryImage,
        primaryImage: data?.primaryImage,
        images: data?.images?.map(img => img.imageUrl || '') || [],
        categoryName: data?.categoryName,
        storeName: data?.storeName,
        createdAt: data?.createdAt,
        updatedAt: data?.updatedAt,
        inStock: (data?.stockQuantity || 0) > 0,
      };
    } catch (error) {
      console.error('Get product by slug error:', error);
      throw error;
    }
  }

  /**
   * GET /api/v1/Product/category/{categoryId}
   * Lọc sản phẩm theo danh mục
   */
  async getProductsByCategory(categoryId: string): Promise<ProductListDto[]> {
    try {
      const response = await api.getApiV1ProductCategoryCategoryId(categoryId);
      const items = response.data || [];

      return items.map(item => ({
        id: item.id,
        name: item.name,
        slug: item.slug,
        price: item.price,
        stock: item.stockQuantity,
        stockQuantity: item.stockQuantity,
        thumbnailUrl: item.primaryImage,
        primaryImage: item.primaryImage,
        categoryName: item.categoryName,
        storeName: item.storeName,
        inStock: (item.stockQuantity || 0) > 0,
      }));
    } catch (error) {
      console.error('Get products by category error:', error);
      throw error;
    }
  }

  /**
   * GET /api/v1/Product/store/{storeId}
   * Lọc sản phẩm theo cửa hàng
   */
  async getProductsByStore(storeId: string): Promise<ProductListDto[]> {
    try {
      const response = await api.getApiV1ProductStoreStoreId(storeId);
      const items = response.data || [];

      return items.map(item => ({
        id: item.id,
        name: item.name,
        slug: item.slug,
        price: item.price,
        stock: item.stockQuantity,
        stockQuantity: item.stockQuantity,
        thumbnailUrl: item.primaryImage,
        primaryImage: item.primaryImage,
        categoryName: item.categoryName,
        storeName: item.storeName,
        inStock: (item.stockQuantity || 0) > 0,
      }));
    } catch (error) {
      console.error('Get products by store error:', error);
      throw error;
    }
  }

  /**
   * POST /api/v1/Product
   * Tạo sản phẩm mới (Requires Auth: product.create)
   */
  async createProduct(data: ProductCreateDto): Promise<ProductDetailDto> {
    try {
      const response = await api.postApiV1Product(data);
      const result = response.data;

      return {
        id: result?.id,
        name: result?.name,
        slug: result?.slug,
        code: result?.code,
        categoryId: result?.categoryId,
        storeId: result?.storeId,
        sku: result?.sku,
        price: result?.price,
        stock: result?.stockQuantity,
        stockQuantity: result?.stockQuantity,
        isActive: result?.isActive,
        description: result?.description,
        thumbnailUrl: result?.primaryImage,
        primaryImage: result?.primaryImage,
        images: result?.images?.map(img => img.imageUrl || '') || [],
        createdAt: result?.createdAt,
        updatedAt: result?.updatedAt,
      };
    } catch (error) {
      console.error('Create product error:', error);
      throw error;
    }
  }

  /**
   * PUT /api/v1/Product/{id}
   * Cập nhật sản phẩm (Requires Auth: product.update)
   */
  async updateProduct(id: string, data: ProductUpdateDto): Promise<ProductDetailDto> {
    try {
      const response = await api.putApiV1ProductId(id, data);
      const result = response.data;

      return {
        id: result?.id,
        name: result?.name,
        slug: result?.slug,
        code: result?.code,
        categoryId: result?.categoryId,
        storeId: result?.storeId,
        sku: result?.sku,
        price: result?.price,
        stock: result?.stockQuantity,
        stockQuantity: result?.stockQuantity,
        isActive: result?.isActive,
        description: result?.description,
        thumbnailUrl: result?.primaryImage,
        primaryImage: result?.primaryImage,
        images: result?.images?.map(img => img.imageUrl || '') || [],
        createdAt: result?.createdAt,
        updatedAt: result?.updatedAt,
      };
    } catch (error) {
      console.error('Update product error:', error);
      throw error;
    }
  }

  /**
   * DELETE /api/v1/Product/{id}
   * Xóa mềm sản phẩm (Requires Auth: product.delete)
   */
  async deleteProduct(id: string): Promise<boolean> {
    try {
      const response = await api.deleteApiV1ProductId(id);
      return response.success || false;
    } catch (error) {
      console.error('Delete product error:', error);
      throw error;
    }
  }

  /**
   * PATCH /api/v1/Product/{id}/stock
   * Cập nhật tồn kho (Requires Auth: product.update_stock)
   */
  async updateStock(id: string, quantity: number): Promise<boolean> {
    try {
      const body: PatchApiV1ProductIdStockBody = { quantity };
      const response = await api.patchApiV1ProductIdStock(id, body);
      return response.success || false;
    } catch (error) {
      console.error('Update stock error:', error);
      throw error;
    }
  }

  /**
   * GET /api/v1/Product/{id}/stock
   * Lấy tồn kho hiện tại
   */
  async getStock(id: string): Promise<number> {
    try {
      const response = await api.getApiV1ProductIdStock(id);
      return response.data || 0;
    } catch (error) {
      console.error('Get stock error:', error);
      throw error;
    }
  }

  /**
   * PATCH /api/v1/Product/{id}/active
   * Kích hoạt/vô hiệu hóa sản phẩm (Requires Auth)
   */
  async toggleActive(id: string, isActive: boolean): Promise<boolean> {
    try {
      const body: PatchApiV1ProductIdActiveBody = { isActive };
      const response = await api.patchApiV1ProductIdActive(id, body);
      return response.success || false;
    } catch (error) {
      console.error('Toggle active error:', error);
      throw error;
    }
  }

  /**
   * PATCH /api/v1/Product/{id}/featured
   * Đặt sản phẩm nổi bật (Requires Auth)
   */
  async toggleFeatured(id: string, isFeatured: boolean): Promise<boolean> {
    try {
      const body: PatchApiV1ProductIdFeaturedBody = { isFeatured };
      const response = await api.patchApiV1ProductIdFeatured(id, body);
      return response.success || false;
    } catch (error) {
      console.error('Toggle featured error:', error);
      throw error;
    }
  }

  /**
   * POST /api/v1/Product/{id}/view
   * Tăng lượt xem sản phẩm
   */
  async incrementView(id: string): Promise<boolean> {
    try {
      const response = await api.postApiV1ProductIdView(id);
      return response.success || false;
    } catch (error) {
      console.error('Increment view error:', error);
      // Don't throw error for view tracking
      return false;
    }
  }

  /**
   * POST /api/v1/Product/{id}/favorite
   * Thêm/gỡ yêu thích (Requires Auth)
   */
  async toggleFavorite(id: string): Promise<boolean> {
    try {
      const response = await api.postApiV1ProductIdFavorite(id);
      return response.success || false;
    } catch (error) {
      console.error('Toggle favorite error:', error);
      throw error;
    }
  }
}

export const vietCommerceProductService = new VietCommerceProductService();
export default vietCommerceProductService;

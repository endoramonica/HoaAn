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
} from '../../../Api/generated-orval/schemas';

// Types for backward compatibility
export interface ProductListDto {
  id?: string;
  name?: string;
  slug?: string;
  shortDescription?: string;
  price?: number;
  stock?: number;
  stockQuantity?: number;
  thumbnailUrl?: string;
  primaryImage?: string;
  categoryName?: string;
  categoryId?: string;
  storeName?: string;
  type?: string;
  serviceCategory?: string;
  serviceDuration?: string;
  serviceRating?: number;
  isFeatured?: boolean;
  inStock?: boolean;
}

export interface ImageObject {
  id?: string;
  url?: string;
  thumbnailUrl?: string;
  displayOrder?: number;
  mediaType?: string;
  isMain?: boolean;
  createdAt?: string;
  updatedAt?: string;
}

export interface CustomizableOption {
  id?: string;
  name?: string;
  baseQuantity?: number;
  unitPrice?: number;
  minQuantity?: number;
  maxQuantity?: number;
  unit?: string;
}

export interface ProductDetailDto extends ProductListDto {
  code?: string;
  categoryId?: string;
  storeId?: string;
  sku?: string;
  isActive?: boolean;
  description?: string;
  images?: ImageObject[];
  tags?: string[];
  viewCount?: number;
  favoriteCount?: number;
  averageRating?: number;
  reviewCount?: number;
  details?: string[];
  customizableOptions?: CustomizableOption[];
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
  type?: string; // 'product' or 'service'
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
      // Map filter to Orval params (note: Orval uses PascalCase)
      // NOTE: Do NOT send Type parameter to backend - it doesn't filter correctly
      // We'll filter by type on frontend instead
      const params: GetApiV1ProductParams = {
        Page: filter?.pageNumber,
        PageSize: filter?.pageSize,
        SearchTerm: filter?.searchTerm,
        CategoryId: filter?.categoryId,
        StoreId: filter?.storeId,
        MinPrice: filter?.minPrice,
        MaxPrice: filter?.maxPrice,
        IsActive: filter?.isActive,
        IsFeatured: filter?.isFeatured,
        SortBy: filter?.sortBy,
        IsDescending: filter?.isDescending,
        // Type: filter?.type, // Don't send to backend - filter on frontend instead
      };

      const response = await api.getApiV1Product(params);

      // Debug log
      console.log('[vietCommerceProductService] Raw response:', JSON.stringify(response, null, 2));

      // Handle nested response structure: response.data.data.data (backend wraps 3 times)
      // Structure: { success, data: { success, data: { items, pageNumber, ... }, message } }
      const firstLevel = (response as any)?.data;
      const secondLevel = firstLevel?.data;
      const paginatedData = secondLevel?.data || secondLevel;

      console.log('[vietCommerceProductService] Parsed data:', {
        firstLevel: !!firstLevel,
        secondLevel: !!secondLevel,
        paginatedData: !!paginatedData,
        itemsCount: paginatedData?.items?.length,
        filterType: filter?.type,
        params: params,
      });

      let items = paginatedData?.items || [];

      // Filter by type on frontend since backend doesn't support Type parameter filtering
      if (filter?.type) {
        items = items.filter((item: any) => {
          // Backend returns empty string "" for products, "service" for services
          const itemType = item.type || '';

          // Map filter type to backend type value
          // 'product' filter → match empty string ""
          // 'service' filter → match "service"
          if (filter.type === 'product') {
            return itemType === '';
          } else if (filter.type === 'service') {
            return itemType === 'service';
          }
          return true;
        });
        console.log('[vietCommerceProductService] Filtered items by type:', {
          filterType: filter.type,
          beforeFilter: paginatedData?.items?.length,
          afterFilter: items.length,
        });
      }

      return {
        items: items.map((item: any) => ({
          id: item.id,
          name: item.name,
          slug: item.slug,
          shortDescription: item.shortDescription,
          price: item.price || item.displayPrice?.discountedPrice || 0,
          stock: item.stockQuantity,
          stockQuantity: item.stockQuantity,
          thumbnailUrl: item.primaryImage,
          primaryImage: item.primaryImage,
          categoryName: item.categoryName,
          categoryId: item.categoryId,
          storeName: item.storeName,
          type: item.type,
          serviceCategory: item.serviceCategory,
          serviceDuration: item.serviceDuration,
          serviceRating: item.serviceRating,
          isFeatured: item.isFeatured,
          inStock: item.inStock ?? (item.stockQuantity || 0) > 0,
        })),
        pageNumber: paginatedData?.pageNumber || 1,
        pageSize: paginatedData?.pageSize || 12,
        totalItems: paginatedData?.totalItems || 0,
        totalPages: paginatedData?.totalPages || 0,
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
      const firstLevel = (response as any)?.data;
      const data = firstLevel?.data || firstLevel;

      console.log('[getProductById] Raw data:', data);

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
        images: data?.images?.map((img: any) => ({
          id: img.id,
          url: img.url,
          thumbnailUrl: img.thumbnailUrl,
          displayOrder: img.displayOrder,
          mediaType: img.mediaType,
          isMain: img.isMain,
          createdAt: img.createdAt,
          updatedAt: img.updatedAt,
        })) || [],
        tags: data?.tags || [],
        viewCount: data?.viewCount,
        favoriteCount: data?.favoriteCount,
        averageRating: data?.averageRating,
        reviewCount: data?.reviewCount,
        details: data?.details || [],
        customizableOptions: data?.customizableOptions?.map((opt: any) => ({
          id: opt.id,
          name: opt.name,
          baseQuantity: opt.baseQuantity,
          unitPrice: opt.unitPrice,
          minQuantity: opt.minQuantity,
          maxQuantity: opt.maxQuantity,
          unit: opt.unit,
        })) || [],
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
      const firstLevel = (response as any)?.data;
      const data = firstLevel?.data || firstLevel;

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
        images: data?.images?.map((img: any) => ({
          id: img.id,
          url: img.url,
          thumbnailUrl: img.thumbnailUrl,
          displayOrder: img.displayOrder,
          mediaType: img.mediaType,
          isMain: img.isMain,
          createdAt: img.createdAt,
          updatedAt: img.updatedAt,
        })) || [],
        tags: data?.tags || [],
        viewCount: data?.viewCount,
        favoriteCount: data?.favoriteCount,
        averageRating: data?.averageRating,
        reviewCount: data?.reviewCount,
        details: data?.details || [],
        customizableOptions: data?.customizableOptions?.map((opt: any) => ({
          id: opt.id,
          name: opt.name,
          baseQuantity: opt.baseQuantity,
          unitPrice: opt.unitPrice,
          minQuantity: opt.minQuantity,
          maxQuantity: opt.maxQuantity,
          unit: opt.unit,
        })) || [],
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
      const firstLevel = (response as any)?.data;
      const items = firstLevel?.data || firstLevel || [];

      return items.map((item: any) => ({
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
      const firstLevel = (response as any)?.data;
      const items = firstLevel?.data || firstLevel || [];

      return items.map((item: any) => ({
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
      const firstLevel = (response as any)?.data;
      const result = firstLevel?.data || firstLevel;

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
        images: result?.images?.map((img: any) => ({
          id: img.id,
          url: img.url,
          thumbnailUrl: img.thumbnailUrl,
          displayOrder: img.displayOrder,
          mediaType: img.mediaType,
          isMain: img.isMain,
          createdAt: img.createdAt,
          updatedAt: img.updatedAt,
        })) || [],
        tags: result?.tags || [],
        viewCount: result?.viewCount,
        favoriteCount: result?.favoriteCount,
        averageRating: result?.averageRating,
        reviewCount: result?.reviewCount,
        details: result?.details || [],
        customizableOptions: result?.customizableOptions?.map((opt: any) => ({
          id: opt.id,
          name: opt.name,
          baseQuantity: opt.baseQuantity,
          unitPrice: opt.unitPrice,
          minQuantity: opt.minQuantity,
          maxQuantity: opt.maxQuantity,
          unit: opt.unit,
        })) || [],
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
      const firstLevel = (response as any)?.data;
      const result = firstLevel?.data || firstLevel;

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
        images: result?.images?.map((img: any) => ({
          id: img.id,
          url: img.url,
          thumbnailUrl: img.thumbnailUrl,
          displayOrder: img.displayOrder,
          mediaType: img.mediaType,
          isMain: img.isMain,
          createdAt: img.createdAt,
          updatedAt: img.updatedAt,
        })) || [],
        tags: result?.tags || [],
        viewCount: result?.viewCount,
        favoriteCount: result?.favoriteCount,
        averageRating: result?.averageRating,
        reviewCount: result?.reviewCount,
        details: result?.details || [],
        customizableOptions: result?.customizableOptions?.map((opt: any) => ({
          id: opt.id,
          name: opt.name,
          baseQuantity: opt.baseQuantity,
          unitPrice: opt.unitPrice,
          minQuantity: opt.minQuantity,
          maxQuantity: opt.maxQuantity,
          unit: opt.unit,
        })) || [],
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
      const response = await api.patchApiV1ProductIdStock(id, quantity);
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
      const response = await api.patchApiV1ProductIdActive(id, isActive);
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
      const response = await api.patchApiV1ProductIdFeatured(id, isFeatured);
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

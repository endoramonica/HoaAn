/**
 * Product Service
 * ✅ UPDATED: Uses Orval generated API
 * Xử lý CRUD sản phẩm, filter, search, pagination
 */

import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type {
  GetApiV1ProductParams,
  ProductCreateDto,
  ProductUpdateDto,
} from '../../../Api/generated-orval/schemas';

// Re-export types from vietCommerceProductService for backward compatibility
export type {
  ProductListDto,
  ProductDetailDto,
  ProductFilterDto,
  PaginatedResult,
} from './vietCommerceProductService';

// Import types for internal use
import type {
  ProductDto,
  CreateProductRequest,
  UpdateProductRequest,
  ProductFilterParams,
  PagedResponse,
  CategoryDto,
  FileUploadResponse,
} from '../api/types';

const api = getVietCommerceAPI();

class ProductService {
  /**
   * Lấy danh sách sản phẩm với filter, search, pagination
   */
  async getProducts(params?: ProductFilterParams): Promise<PagedResponse<ProductDto>> {
    try {
      // Map params to Orval format
      const orvalParams: GetApiV1ProductParams = {
        pageNumber: params?.pageNumber,
        pageSize: params?.pageSize,
        searchTerm: params?.search,
        categoryId: params?.categoryId,
        minPrice: params?.minPrice,
        maxPrice: params?.maxPrice,
        isFeatured: params?.isFeatured,
        sortBy: params?.sortBy,
        isDescending: params?.sortOrder === 'desc',
      };

      const response = await api.getApiV1Product(orvalParams);
      const data = response.data;

      // Map response to expected format
      const items: ProductDto[] = (data?.items || []).map(item => ({
        id: item.id || '',
        name: item.name || '',
        slug: item.slug || '',
        description: item.description || '',
        shortDescription: item.shortDescription || '',
        price: item.price || 0,
        originalPrice: item.originalPrice,
        discount: item.discountPercentage,
        categoryId: item.categoryId || '',
        category: item.categoryName ? {
          id: item.categoryId || '',
          name: item.categoryName,
          slug: item.categoryName.toLowerCase().replace(/\s+/g, '-'),
          sortOrder: 0,
          isActive: true,
        } : undefined,
        images: item.images?.map((img, idx) => ({
          id: img.id || `img-${idx}`,
          url: img.imageUrl || '',
          altText: item.name || '',
          isPrimary: img.isPrimary || idx === 0,
          sortOrder: img.sortOrder || idx,
        })) || [],
        thumbnailUrl: item.primaryImage,
        stock: item.stockQuantity || 0,
        isActive: item.isActive ?? true,
        isFeatured: item.isFeatured ?? false,
        rating: item.averageRating || 0,
        reviewCount: item.reviewCount || 0,
        tags: [],
        specifications: {},
        createdAt: item.createdAt || new Date().toISOString(),
        updatedAt: item.updatedAt || new Date().toISOString(),
      }));

      return {
        items,
        pageNumber: data?.pageNumber || 1,
        pageSize: data?.pageSize || 12,
        totalPages: data?.totalPages || 0,
        totalCount: data?.totalItems || 0,
        hasPreviousPage: (data?.pageNumber || 1) > 1,
        hasNextPage: (data?.pageNumber || 1) < (data?.totalPages || 0),
      };
    } catch (error) {
      console.error('Get products error:', error);
      throw error;
    }
  }

  /**
   * Lấy chi tiết sản phẩm theo ID
   */
  async getProductById(id: string): Promise<ProductDto> {
    try {
      const response = await api.getApiV1ProductId(id);
      const data = response.data;

      return {
        id: data?.id || '',
        name: data?.name || '',
        slug: data?.slug || '',
        description: data?.description || '',
        shortDescription: data?.shortDescription || '',
        price: data?.price || 0,
        originalPrice: data?.originalPrice,
        discount: data?.discountPercentage,
        categoryId: data?.categoryId || '',
        category: data?.categoryName ? {
          id: data.categoryId || '',
          name: data.categoryName,
          slug: data.categoryName.toLowerCase().replace(/\s+/g, '-'),
          sortOrder: 0,
          isActive: true,
        } : undefined,
        images: data?.images?.map((img, idx) => ({
          id: img.id || `img-${idx}`,
          url: img.imageUrl || '',
          altText: data?.name || '',
          isPrimary: img.isPrimary || idx === 0,
          sortOrder: img.sortOrder || idx,
        })) || [],
        thumbnailUrl: data?.primaryImage,
        stock: data?.stockQuantity || 0,
        isActive: data?.isActive ?? true,
        isFeatured: data?.isFeatured ?? false,
        rating: data?.averageRating || 0,
        reviewCount: data?.reviewCount || 0,
        tags: [],
        specifications: {},
        createdAt: data?.createdAt || new Date().toISOString(),
        updatedAt: data?.updatedAt || new Date().toISOString(),
      };
    } catch (error) {
      console.error('Get product by id error:', error);
      throw error;
    }
  }

  /**
   * Lấy chi tiết sản phẩm theo slug
   */
  async getProductBySlug(slug: string): Promise<ProductDto> {
    try {
      const response = await api.getApiV1ProductSlugSlug(slug);
      const data = response.data;

      return {
        id: data?.id || '',
        name: data?.name || '',
        slug: data?.slug || '',
        description: data?.description || '',
        shortDescription: data?.shortDescription || '',
        price: data?.price || 0,
        originalPrice: data?.originalPrice,
        discount: data?.discountPercentage,
        categoryId: data?.categoryId || '',
        category: data?.categoryName ? {
          id: data.categoryId || '',
          name: data.categoryName,
          slug: data.categoryName.toLowerCase().replace(/\s+/g, '-'),
          sortOrder: 0,
          isActive: true,
        } : undefined,
        images: data?.images?.map((img, idx) => ({
          id: img.id || `img-${idx}`,
          url: img.imageUrl || '',
          altText: data?.name || '',
          isPrimary: img.isPrimary || idx === 0,
          sortOrder: img.sortOrder || idx,
        })) || [],
        thumbnailUrl: data?.primaryImage,
        stock: data?.stockQuantity || 0,
        isActive: data?.isActive ?? true,
        isFeatured: data?.isFeatured ?? false,
        rating: data?.averageRating || 0,
        reviewCount: data?.reviewCount || 0,
        tags: [],
        specifications: {},
        createdAt: data?.createdAt || new Date().toISOString(),
        updatedAt: data?.updatedAt || new Date().toISOString(),
      };
    } catch (error) {
      console.error('Get product by slug error:', error);
      throw error;
    }
  }

  /**
   * Tạo sản phẩm mới (Admin)
   */
  async createProduct(request: CreateProductRequest): Promise<ProductDto> {
    try {
      const createDto: ProductCreateDto = {
        name: request.name,
        code: request.name.substring(0, 10).toUpperCase(),
        description: request.description,
        shortDescription: request.shortDescription,
        price: request.price,
        originalPrice: request.originalPrice,
        categoryId: request.categoryId,
        stockQuantity: request.stock,
        sku: request.name.toLowerCase().replace(/\s+/g, '-'),
        isActive: true,
        isFeatured: false,
      };

      const response = await api.postApiV1Product(createDto);
      const data = response.data;

      return {
        id: data?.id || '',
        name: data?.name || '',
        slug: data?.slug || '',
        description: data?.description || '',
        shortDescription: data?.shortDescription || '',
        price: data?.price || 0,
        originalPrice: data?.originalPrice,
        categoryId: data?.categoryId || '',
        images: [],
        stock: data?.stockQuantity || 0,
        isActive: data?.isActive ?? true,
        isFeatured: data?.isFeatured ?? false,
        rating: 0,
        reviewCount: 0,
        tags: [],
        specifications: {},
        createdAt: data?.createdAt || new Date().toISOString(),
        updatedAt: data?.updatedAt || new Date().toISOString(),
      };
    } catch (error) {
      console.error('Create product error:', error);
      throw error;
    }
  }

  /**
   * Cập nhật sản phẩm (Admin)
   */
  async updateProduct(id: string, request: UpdateProductRequest): Promise<ProductDto> {
    try {
      const updateDto: ProductUpdateDto = {
        name: request.name,
        description: request.description,
        shortDescription: request.shortDescription,
        price: request.price,
        originalPrice: request.originalPrice,
        categoryId: request.categoryId,
        stockQuantity: request.stock,
        isActive: request.isActive,
        isFeatured: request.isFeatured,
      };

      const response = await api.putApiV1ProductId(id, updateDto);
      const data = response.data;

      return {
        id: data?.id || '',
        name: data?.name || '',
        slug: data?.slug || '',
        description: data?.description || '',
        shortDescription: data?.shortDescription || '',
        price: data?.price || 0,
        originalPrice: data?.originalPrice,
        categoryId: data?.categoryId || '',
        images: data?.images?.map((img, idx) => ({
          id: img.id || `img-${idx}`,
          url: img.imageUrl || '',
          altText: data?.name || '',
          isPrimary: img.isPrimary || idx === 0,
          sortOrder: img.sortOrder || idx,
        })) || [],
        stock: data?.stockQuantity || 0,
        isActive: data?.isActive ?? true,
        isFeatured: data?.isFeatured ?? false,
        rating: data?.averageRating || 0,
        reviewCount: data?.reviewCount || 0,
        tags: [],
        specifications: {},
        createdAt: data?.createdAt || new Date().toISOString(),
        updatedAt: data?.updatedAt || new Date().toISOString(),
      };
    } catch (error) {
      console.error('Update product error:', error);
      throw error;
    }
  }

  /**
   * Xóa sản phẩm (Admin)
   */
  async deleteProduct(id: string): Promise<void> {
    try {
      await api.deleteApiV1ProductId(id);
    } catch (error) {
      console.error('Delete product error:', error);
      throw error;
    }
  }

  /**
   * Lấy danh sách categories
   */
  async getCategories(): Promise<CategoryDto[]> {
    try {
      const response = await api.getApiV1Category();
      const items = response.data || [];

      return items.map(item => ({
        id: item.id || '',
        name: item.name || '',
        slug: item.slug || '',
        description: item.description,
        icon: item.icon,
        imageUrl: item.imageUrl,
        sortOrder: item.sortOrder || 0,
        isActive: item.isActive ?? true,
        parentId: item.parentId,
      }));
    } catch (error) {
      console.error('Get categories error:', error);
      throw error;
    }
  }

  /**
   * Lấy category theo ID
   */
  async getCategoryById(id: string): Promise<CategoryDto> {
    try {
      const response = await api.getApiV1CategoryId(id);
      const data = response.data;

      return {
        id: data?.id || '',
        name: data?.name || '',
        slug: data?.slug || '',
        description: data?.description,
        icon: data?.icon,
        imageUrl: data?.imageUrl,
        sortOrder: data?.sortOrder || 0,
        isActive: data?.isActive ?? true,
        parentId: data?.parentId,
      };
    } catch (error) {
      console.error('Get category by id error:', error);
      throw error;
    }
  }

  /**
   * Upload ảnh sản phẩm
   * Note: This endpoint may need to be implemented separately
   */
  async uploadProductImage(productId: string, file: File): Promise<FileUploadResponse> {
    try {
      // For now, return a placeholder response
      // The actual implementation depends on the backend API
      console.warn('uploadProductImage: Not implemented with Orval yet');
      return {
        url: URL.createObjectURL(file),
        fileName: file.name,
        fileSize: file.size,
        contentType: file.type,
      };
    } catch (error) {
      console.error('Upload product image error:', error);
      throw error;
    }
  }
}

export const productService = new ProductService();
export default productService;

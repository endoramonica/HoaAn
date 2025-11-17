/**
 * VietCommerce Product Service
 * Tích hợp với VietCommerce .NET 8 Web API
 * Base route: /api/v1/product
 */

import { apiRequest, buildQueryString } from "../api/client";
import type {
  ProductListDto,
  ProductDetailDto,
  ProductCreateDto,
  ProductUpdateDto,
  ProductFilterDto,
  PaginatedResult,
  ApiResponse,
} from "../api/types";

const getMockMode = () => {
  if (typeof import.meta !== "undefined" && import.meta.env) {
    return import.meta.env.VITE_USE_MOCK_DATA === "true";
  }
  return false; // Mặc định sử dụng API thật
};

const USE_MOCK = getMockMode();

/**
 * Mock data cho development (fallback)
 */
const generateMockProducts = (): ProductListDto[] => {
  const products: ProductListDto[] = [];
  const names = [
    "Hương Trầm Cao Cấp",
    "Nến Thờ Đỏ",
    "Bộ Đồ Thờ Ngũ Sự",
    "Tượng Phật Quan Âm",
    "Chuỗi Hạt Gỗ Trầm",
    "Bình Hương Đồng",
    "Lọ Hoa Sen Gốm",
    "Đèn Thờ LED",
  ];

  for (let i = 1; i <= 50; i++) {
    products.push({
      id: `product-${i}`,
      name: `${names[i % names.length]} ${i}`,
      slug: `product-${i}`,
      price: Math.floor(Math.random() * 500000) + 50000,
      stock: Math.floor(Math.random() * 100) + 10,
      thumbnailUrl: `https://images.unsplash.com/photo-${1600000000000 + i}?w=400`,
      categoryName: [
        "Hương & Nến",
        "Đồ Thờ Cúng",
        "Vật Phẩm Phong Thủy",
      ][i % 3],
      storeName: "VietCommerce Store",
    });
  }

  return products;
};

const MOCK_PRODUCTS = generateMockProducts();

class VietCommerceProductService {
  /**
   * GET /api/v1/product
   * Lấy danh sách sản phẩm với filter/pagination
   */
  async getProducts(
    filter?: ProductFilterDto,
  ): Promise<PaginatedResult<ProductListDto>> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 800),
        );

        let filtered = [...MOCK_PRODUCTS];

        // Apply filters
        if (filter?.searchTerm) {
          const search = filter.searchTerm.toLowerCase();
          filtered = filtered.filter((p) =>
            p.name.toLowerCase().includes(search),
          );
        }

        if (filter?.categoryId) {
          filtered = filtered.filter(
            (p) => p.categoryName === filter.categoryId,
          );
        }

        if (filter?.minPrice !== undefined) {
          filtered = filtered.filter(
            (p) => p.price >= filter.minPrice!,
          );
        }

        if (filter?.maxPrice !== undefined) {
          filtered = filtered.filter(
            (p) => p.price <= filter.maxPrice!,
          );
        }

        // Apply sorting
        if (filter?.sortBy) {
          filtered.sort((a, b) => {
            let comparison = 0;
            switch (filter.sortBy) {
              case "name":
                comparison = a.name.localeCompare(b.name);
                break;
              case "price":
                comparison = a.price - b.price;
                break;
              default:
                comparison = 0;
            }
            return filter.isDescending
              ? -comparison
              : comparison;
          });
        }

        // Apply pagination
        const page = filter?.pageNumber || 1;
        const pageSize = filter?.pageSize || 12;
        const totalCount = filtered.length;
        const totalPages = Math.ceil(totalCount / pageSize);
        const startIndex = (page - 1) * pageSize;
        const items = filtered.slice(
          startIndex,
          startIndex + pageSize,
        );

        return {
          items,
          pageNumber: page,
          pageSize,
          totalItems: totalCount,
          totalPages,
        };
      }
      const queryString = buildQueryString(filter || {});
      // ✅ Xác định type response đúng: ApiResponse wrapper
      const response = await apiRequest.get<
        ApiResponse<PaginatedResult<ProductListDto>>
      >(`/product${queryString}`);

      // ✅ Extract data từ ApiResponse wrapper
      // response.data là object ApiResponse<PaginatedResult<ProductListDto>>
      // response.data.data là PaginatedResult<ProductListDto>
      return response.data.data;
    } catch (error) {
      console.error("Get products error:", error);
      throw error;
    }
  }

  /**
   * GET /api/v1/product/{id}
   * Lấy chi tiết sản phẩm theo ID
   */
  async getProductById(id: string): Promise<ProductDetailDto> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 500),
        );

        const product = MOCK_PRODUCTS.find((p) => p.id === id);
        if (!product) {
          throw new Error("Không tìm thấy sản phẩm");
        }

        return {
          ...product,
          code: `CODE-${id}`,
          categoryId: "cat-1",
          storeId: "store-1",
          sku: `SKU-${id}`,
          isActive: true,
          description: `Mô tả chi tiết cho ${product.name}`,
          images: [product.thumbnailUrl || ""],
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        };
      }

      const response = await apiRequest.get<
        ApiResponse<ProductDetailDto>
      >(`/product/${id}`);
      return response.data;
    } catch (error) {
      console.error("Get product by id error:", error);
      throw error;
    }
  }

  /**
   * GET /api/v1/product/slug/{slug}
   * Lấy sản phẩm theo slug
   */
  async getProductBySlug(
    slug: string,
  ): Promise<ProductDetailDto> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 500),
        );

        const product = MOCK_PRODUCTS.find(
          (p) => p.slug === slug,
        );
        if (!product) {
          throw new Error("Không tìm thấy sản phẩm");
        }

        return {
          ...product,
          code: `CODE-${product.id}`,
          categoryId: "cat-1",
          storeId: "store-1",
          sku: `SKU-${product.id}`,
          isActive: true,
          description: `Mô tả chi tiết cho ${product.name}`,
          images: [product.thumbnailUrl || ""],
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        };
      }

      const response = await apiRequest.get<
        ApiResponse<ProductDetailDto>
      >(`/product/slug/${slug}`);
      return response.data;
    } catch (error) {
      console.error("Get product by slug error:", error);
      throw error;
    }
  }

  /**
   * GET /api/v1/product/category/{categoryId}
   * Lọc sản phẩm theo danh mục
   */
  async getProductsByCategory(
    categoryId: string,
  ): Promise<ProductListDto[]> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 500),
        );
        return MOCK_PRODUCTS.filter((p) =>
          p.categoryName?.includes(categoryId),
        );
      }

      const response = await apiRequest.get<
        ApiResponse<ProductListDto[]>
      >(`/product/category/${categoryId}`);
      return response.data;
    } catch (error) {
      console.error("Get products by category error:", error);
      throw error;
    }
  }

  /**
   * GET /api/v1/product/store/{storeId}
   * Lọc sản phẩm theo cửa hàng
   */
  async getProductsByStore(
    storeId: string,
  ): Promise<ProductListDto[]> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 500),
        );
        return MOCK_PRODUCTS;
      }

      const response = await apiRequest.get<
        ApiResponse<ProductListDto[]>
      >(`/product/store/${storeId}`);
      return response.data;
    } catch (error) {
      console.error("Get products by store error:", error);
      throw error;
    }
  }

  /**
   * POST /api/v1/product
   * Tạo sản phẩm mới (Requires Auth: product.create)
   */
  async createProduct(
    data: ProductCreateDto,
  ): Promise<ProductDetailDto> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 1000),
        );

        const newProduct: ProductDetailDto = {
          id: `product-new-${Date.now()}`,
          name: data.name,
          slug: data.name.toLowerCase().replace(/\s+/g, "-"),
          code: data.code,
          categoryId: data.categoryId || "cat-1",
          storeId: "store-1",
          sku: data.sku || `SKU-${Date.now()}`,
          price: data.price || 0,
          stock: data.stockQuantity || 0,
          isActive: data.isActive ?? true,
          description: data.description,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        };

        return newProduct;
      }

      const response = await apiRequest.post<
        ApiResponse<ProductDetailDto>
      >("/product", data);
      return response.data;
    } catch (error) {
      console.error("Create product error:", error);
      throw error;
    }
  }

  /**
   * PUT /api/v1/product/{id}
   * Cập nhật sản phẩm (Requires Auth: product.update)
   */
  async updateProduct(
    id: string,
    data: ProductUpdateDto,
  ): Promise<ProductDetailDto> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 1000),
        );

        const product = MOCK_PRODUCTS.find((p) => p.id === id);
        if (!product) {
          throw new Error("Không tìm thấy sản phẩm");
        }

        return {
          ...product,
          ...data,
          code: `CODE-${id}`,
          categoryId: data.categoryId || "cat-1",
          storeId: "store-1",
          sku: data.sku || `SKU-${id}`,
          stock: data.stockQuantity || product.stock,
          isActive: data.isActive ?? true,
          description: data.description,
          images: [product.thumbnailUrl || ""],
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        };
      }

      const response = await apiRequest.put<
        ApiResponse<ProductDetailDto>
      >(`/product/${id}`, data);
      return response.data;
    } catch (error) {
      console.error("Update product error:", error);
      throw error;
    }
  }

  /**
   * DELETE /api/v1/product/{id}
   * Xóa mềm sản phẩm (Requires Auth: product.delete)
   */
  async deleteProduct(id: string): Promise<boolean> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 500),
        );
        return true;
      }

      const response = await apiRequest.delete<
        ApiResponse<boolean>
      >(`/product/${id}`);
      return response.data;
    } catch (error) {
      console.error("Delete product error:", error);
      throw error;
    }
  }

  /**
   * PATCH /api/v1/product/{id}/stock
   * Cập nhật tồn kho (Requires Auth: product.update_stock)
   */
  async updateStock(
    id: string,
    quantity: number,
  ): Promise<boolean> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 500),
        );
        return true;
      }

      const response = await apiRequest.patch<
        ApiResponse<boolean>
      >(`/product/${id}/stock`, { quantity });
      return response.data;
    } catch (error) {
      console.error("Update stock error:", error);
      throw error;
    }
  }

  /**
   * GET /api/v1/product/{id}/stock
   * Lấy tồn kho hiện tại
   */
  async getStock(id: string): Promise<number> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 300),
        );
        const product = MOCK_PRODUCTS.find((p) => p.id === id);
        return product?.stock || 0;
      }

      const response = await apiRequest.get<
        ApiResponse<number>
      >(`/product/${id}/stock`);
      return response.data;
    } catch (error) {
      console.error("Get stock error:", error);
      throw error;
    }
  }

  /**
   * PATCH /api/v1/product/{id}/active
   * Kích hoạt/vô hiệu hóa sản phẩm (Requires Auth)
   */
  async toggleActive(
    id: string,
    isActive: boolean,
  ): Promise<boolean> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 500),
        );
        return true;
      }

      const response = await apiRequest.patch<
        ApiResponse<boolean>
      >(`/product/${id}/active`, { isActive });
      return response.data;
    } catch (error) {
      console.error("Toggle active error:", error);
      throw error;
    }
  }

  /**
   * PATCH /api/v1/product/{id}/featured
   * Đặt sản phẩm nổi bật (Requires Auth)
   */
  async toggleFeatured(
    id: string,
    isFeatured: boolean,
  ): Promise<boolean> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 500),
        );
        return true;
      }

      const response = await apiRequest.patch<
        ApiResponse<boolean>
      >(`/product/${id}/featured`, { isFeatured });
      return response.data;
    } catch (error) {
      console.error("Toggle featured error:", error);
      throw error;
    }
  }

  /**
   * POST /api/v1/product/{id}/view
   * Tăng lượt xem sản phẩm
   */
  async incrementView(id: string): Promise<boolean> {
    try {
      if (USE_MOCK) {
        return true; // No delay for view tracking
      }

      const response = await apiRequest.post<
        ApiResponse<boolean>
      >(`/product/${id}/view`);
      return response.data;
    } catch (error) {
      console.error("Increment view error:", error);
      // Don't throw error for view tracking
      return false;
    }
  }

  /**
   * POST /api/v1/product/{id}/favorite
   * Thêm/gỡ yêu thích (Requires Auth)
   */
  async toggleFavorite(id: string): Promise<boolean> {
    try {
      if (USE_MOCK) {
        await new Promise((resolve) =>
          setTimeout(resolve, 500),
        );
        return true;
      }

      const response = await apiRequest.post<
        ApiResponse<boolean>
      >(`/product/${id}/favorite`);
      return response.data;
    } catch (error) {
      console.error("Toggle favorite error:", error);
      throw error;
    }
  }
}

export const vietCommerceProductService =
  new VietCommerceProductService();
export default vietCommerceProductService;
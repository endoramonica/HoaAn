/**
 * Product Service
 * Xử lý CRUD sản phẩm, filter, search, pagination
 */

import { apiRequest, buildQueryString } from '../api/client';
import {
  ProductDto,
  CreateProductRequest,
  UpdateProductRequest,
  ProductFilterParams,
  PagedResponse,
  CategoryDto,
  FileUploadRequest,
  FileUploadResponse,
} from '../api/types';

/**
 * Mock data cho development
 */
const MOCK_CATEGORIES: CategoryDto[] = [
  {
    id: 'cat-1',
    name: 'Hương & Nến',
    slug: 'huong-nen',
    description: 'Hương trầm, nến thơm các loại',
    icon: '🕯️',
    imageUrl: 'https://images.unsplash.com/photo-1602874801006-84c78b4e9dcc',
    sortOrder: 1,
    isActive: true,
  },
  {
    id: 'cat-2',
    name: 'Đồ Thờ Cúng',
    slug: 'do-tho-cung',
    description: 'Bộ đồ thờ cúng đầy đủ',
    icon: '🙏',
    imageUrl: 'https://images.unsplash.com/photo-1544980919-e17526d4ed0a',
    sortOrder: 2,
    isActive: true,
  },
  {
    id: 'cat-3',
    name: 'Vật Phẩm Phong Thủy',
    slug: 'vat-pham-phong-thuy',
    description: 'Đồ phong thủy, may mắn',
    icon: '🧧',
    imageUrl: 'https://images.unsplash.com/photo-1518709268805-4e9042af9f23',
    sortOrder: 3,
    isActive: true,
  },
];

const generateMockProducts = (count: number = 20): ProductDto[] => {
  const products: ProductDto[] = [];
  const names = [
    'Hương Trầm Cao Cấp',
    'Nến Thờ Đỏ',
    'Bộ Đồ Thờ Ngũ Sự',
    'Tượng Phật Quan Âm',
    'Chuỗi Hạt Gỗ Trầm',
    'Bình Hương Đồng',
    'Lọ Hoa Sen Gốm',
    'Đèn Thờ LED',
  ];

  for (let i = 1; i <= count; i++) {
    const category = MOCK_CATEGORIES[i % MOCK_CATEGORIES.length];
    const name = names[i % names.length] + ` ${i}`;
    const price = Math.floor(Math.random() * 500000) + 50000;
    const originalPrice = price + Math.floor(Math.random() * 100000);
    
    products.push({
      id: `product-${i}`,
      name,
      slug: name.toLowerCase().replace(/\s+/g, '-'),
      description: `Mô tả chi tiết cho ${name}. Sản phẩm chất lượng cao, được tuyển chọn kỹ càng.`,
      shortDescription: `${name} - Sản phẩm cao cấp`,
      price,
      originalPrice,
      discount: Math.round(((originalPrice - price) / originalPrice) * 100),
      categoryId: category.id,
      category,
      images: [
        {
          id: `img-${i}-1`,
          url: `https://images.unsplash.com/photo-${1600000000000 + i}?w=800`,
          altText: name,
          isPrimary: true,
          sortOrder: 1,
        },
      ],
      thumbnailUrl: `https://images.unsplash.com/photo-${1600000000000 + i}?w=400`,
      stock: Math.floor(Math.random() * 100) + 10,
      isActive: true,
      isFeatured: i % 3 === 0,
      rating: 4 + Math.random(),
      reviewCount: Math.floor(Math.random() * 100),
      tags: ['mới', 'bán chạy', 'cao cấp'].filter(() => Math.random() > 0.5),
      specifications: {
        'Xuất xứ': 'Việt Nam',
        'Chất liệu': 'Gỗ trầm hương',
        'Kích thước': '20cm x 5cm',
      },
      createdAt: new Date(Date.now() - i * 86400000).toISOString(),
      updatedAt: new Date(Date.now() - i * 3600000).toISOString(),
    });
  }

  return products;
};

const MOCK_PRODUCTS = generateMockProducts(50);

const getMockMode = () => {
  if (typeof import.meta !== 'undefined' && import.meta.env) {
    return import.meta.env.VITE_USE_MOCK_DATA === 'true';
  }
  return true; // Default to mock mode
};

const USE_MOCK = getMockMode();

class ProductService {
  /**
   * Lấy danh sách sản phẩm với filter, search, pagination
   */
  async getProducts(params?: ProductFilterParams): Promise<PagedResponse<ProductDto>> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 800));

        // Apply filters
        let filtered = [...MOCK_PRODUCTS];

        if (params?.search) {
          const search = params.search.toLowerCase();
          filtered = filtered.filter(p =>
            p.name.toLowerCase().includes(search) ||
            p.description.toLowerCase().includes(search)
          );
        }

        if (params?.categoryId) {
          filtered = filtered.filter(p => p.categoryId === params.categoryId);
        }

        if (params?.minPrice !== undefined) {
          filtered = filtered.filter(p => p.price >= params.minPrice!);
        }

        if (params?.maxPrice !== undefined) {
          filtered = filtered.filter(p => p.price <= params.maxPrice!);
        }

        if (params?.isFeatured !== undefined) {
          filtered = filtered.filter(p => p.isFeatured === params.isFeatured);
        }

        if (params?.tags && params.tags.length > 0) {
          filtered = filtered.filter(p =>
            params.tags!.some(tag => p.tags.includes(tag))
          );
        }

        // Apply sorting
        if (params?.sortBy) {
          filtered.sort((a, b) => {
            let comparison = 0;
            switch (params.sortBy) {
              case 'name':
                comparison = a.name.localeCompare(b.name);
                break;
              case 'price':
                comparison = a.price - b.price;
                break;
              case 'rating':
                comparison = a.rating - b.rating;
                break;
              case 'createdAt':
                comparison = new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
                break;
            }
            return params.sortOrder === 'desc' ? -comparison : comparison;
          });
        }

        // Apply pagination
        const pageNumber = params?.pageNumber || 1;
        const pageSize = params?.pageSize || 12;
        const totalCount = filtered.length;
        const totalPages = Math.ceil(totalCount / pageSize);
        const startIndex = (pageNumber - 1) * pageSize;
        const items = filtered.slice(startIndex, startIndex + pageSize);

        return {
          items,
          pageNumber,
          pageSize,
          totalPages,
          totalCount,
          hasPreviousPage: pageNumber > 1,
          hasNextPage: pageNumber < totalPages,
        };
      }

      const queryString = buildQueryString(params || {});
      return await apiRequest.get<PagedResponse<ProductDto>>(`/products${queryString}`);
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
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 500));
        
        const product = MOCK_PRODUCTS.find(p => p.id === id);
        if (!product) {
          throw new Error('Không tìm thấy sản phẩm');
        }
        return product;
      }

      return await apiRequest.get<ProductDto>(`/products/${id}`);
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
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 500));
        
        const product = MOCK_PRODUCTS.find(p => p.slug === slug);
        if (!product) {
          throw new Error('Không tìm thấy sản phẩm');
        }
        return product;
      }

      return await apiRequest.get<ProductDto>(`/products/slug/${slug}`);
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
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        const category = MOCK_CATEGORIES.find(c => c.id === request.categoryId);
        if (!category) {
          throw new Error('Danh mục không tồn tại');
        }

        const newProduct: ProductDto = {
          id: `product-new-${Date.now()}`,
          name: request.name,
          slug: request.name.toLowerCase().replace(/\s+/g, '-'),
          description: request.description,
          shortDescription: request.shortDescription,
          price: request.price,
          originalPrice: request.originalPrice,
          discount: request.originalPrice 
            ? Math.round(((request.originalPrice - request.price) / request.originalPrice) * 100)
            : undefined,
          categoryId: request.categoryId,
          category,
          images: [],
          stock: request.stock,
          isActive: true,
          isFeatured: false,
          rating: 0,
          reviewCount: 0,
          tags: request.tags || [],
          specifications: request.specifications,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        };

        return newProduct;
      }

      return await apiRequest.post<ProductDto>('/products', request);
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
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        const product = MOCK_PRODUCTS.find(p => p.id === id);
        if (!product) {
          throw new Error('Không tìm thấy sản phẩm');
        }

        return {
          ...product,
          ...request,
          updatedAt: new Date().toISOString(),
        };
      }

      return await apiRequest.put<ProductDto>(`/products/${id}`, request);
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
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 500));
        return;
      }

      await apiRequest.delete(`/products/${id}`);
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
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 300));
        return MOCK_CATEGORIES;
      }

      return await apiRequest.get<CategoryDto[]>('/categories');
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
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 300));
        
        const category = MOCK_CATEGORIES.find(c => c.id === id);
        if (!category) {
          throw new Error('Không tìm thấy danh mục');
        }
        return category;
      }

      return await apiRequest.get<CategoryDto>(`/categories/${id}`);
    } catch (error) {
      console.error('Get category by id error:', error);
      throw error;
    }
  }

  /**
   * Upload ảnh sản phẩm
   */
  async uploadProductImage(productId: string, file: File): Promise<FileUploadResponse> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1500));
        
        return {
          url: URL.createObjectURL(file),
          fileName: file.name,
          fileSize: file.size,
          contentType: file.type,
        };
      }

      const formData = new FormData();
      formData.append('file', file);

      return await apiRequest.post<FileUploadResponse>(
        `/products/${productId}/images`,
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        }
      );
    } catch (error) {
      console.error('Upload product image error:', error);
      throw error;
    }
  }
}

export const productService = new ProductService();
export default productService;

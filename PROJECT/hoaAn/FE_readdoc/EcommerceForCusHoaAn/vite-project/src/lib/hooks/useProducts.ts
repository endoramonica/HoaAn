/**
 * useProducts Hook
 * Custom hook để quản lý products state với pagination, filter, search
 */

import { useState, useEffect, useCallback } from 'react';
import { productService } from '../services/productService';
import type {
  ProductDto,
  ProductFilterParams,
  PagedResponse,
  CategoryDto,
  CreateProductRequest,
  UpdateProductRequest,
} from '../api/types';
import { toast } from 'sonner';
import { ApiError } from '../api/errors';

interface UseProductsOptions {
  autoLoad?: boolean;
  initialParams?: ProductFilterParams;
}

interface UseProductsReturn {
  products: ProductDto[];
  pagedData: PagedResponse<ProductDto> | null;
  categories: CategoryDto[];
  isLoading: boolean;
  isRefreshing: boolean;
  error: string | null;
  params: ProductFilterParams;
  setParams: (params: ProductFilterParams) => void;
  loadProducts: (params?: ProductFilterParams) => Promise<void>;
  loadCategories: () => Promise<void>;
  getProductById: (id: string) => Promise<ProductDto | null>;
  createProduct: (request: CreateProductRequest) => Promise<ProductDto | null>;
  updateProduct: (id: string, request: UpdateProductRequest) => Promise<ProductDto | null>;
  deleteProduct: (id: string) => Promise<boolean>;
  refresh: () => Promise<void>;
  goToPage: (page: number) => void;
  nextPage: () => void;
  previousPage: () => void;
}

export const useProducts = (options?: UseProductsOptions): UseProductsReturn => {
  const { autoLoad = true, initialParams = {} } = options || {};

  const [products, setProducts] = useState<ProductDto[]>([]);
  const [pagedData, setPagedData] = useState<PagedResponse<ProductDto> | null>(null);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [isRefreshing, setIsRefreshing] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [params, setParams] = useState<ProductFilterParams>(initialParams);

  /**
   * Load products
   */
  const loadProducts = useCallback(async (loadParams?: ProductFilterParams) => {
    try {
      const currentParams = loadParams || params;
      setIsLoading(true);
      setError(null);

      const response = await productService.getProducts(currentParams);
      
      setProducts(response.items);
      setPagedData(response);
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể tải danh sách sản phẩm';
      
      setError(errorMessage);
      console.error('Load products error:', err);
    } finally {
      setIsLoading(false);
    }
  }, [params]);

  /**
   * Load categories
   */
  const loadCategories = useCallback(async () => {
    try {
      const response = await productService.getCategories();
      setCategories(response);
    } catch (err) {
      console.error('Load categories error:', err);
    }
  }, []);

  /**
   * Get product by ID
   */
  const getProductById = useCallback(async (id: string): Promise<ProductDto | null> => {
    try {
      const product = await productService.getProductById(id);
      return product;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể tải thông tin sản phẩm';
      
      toast.error(errorMessage);
      return null;
    }
  }, []);

  /**
   * Create product
   */
  const createProduct = useCallback(async (request: CreateProductRequest): Promise<ProductDto | null> => {
    try {
      setIsLoading(true);
      const product = await productService.createProduct(request);
      
      toast.success('Tạo sản phẩm thành công!');
      
      // Refresh danh sách
      await loadProducts();
      
      return product;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể tạo sản phẩm';
      
      toast.error(errorMessage);
      return null;
    } finally {
      setIsLoading(false);
    }
  }, [loadProducts]);

  /**
   * Update product
   */
  const updateProduct = useCallback(async (
    id: string, 
    request: UpdateProductRequest
  ): Promise<ProductDto | null> => {
    try {
      setIsLoading(true);
      const product = await productService.updateProduct(id, request);
      
      toast.success('Cập nhật sản phẩm thành công!');
      
      // Refresh danh sách
      await loadProducts();
      
      return product;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể cập nhật sản phẩm';
      
      toast.error(errorMessage);
      return null;
    } finally {
      setIsLoading(false);
    }
  }, [loadProducts]);

  /**
   * Delete product
   */
  const deleteProduct = useCallback(async (id: string): Promise<boolean> => {
    try {
      setIsLoading(true);
      await productService.deleteProduct(id);
      
      toast.success('Xóa sản phẩm thành công!');
      
      // Refresh danh sách
      await loadProducts();
      
      return true;
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Không thể xóa sản phẩm';
      
      toast.error(errorMessage);
      return false;
    } finally {
      setIsLoading(false);
    }
  }, [loadProducts]);

  /**
   * Refresh data
   */
  const refresh = useCallback(async () => {
    try {
      setIsRefreshing(true);
      await Promise.all([
        loadProducts(),
        loadCategories(),
      ]);
    } finally {
      setIsRefreshing(false);
    }
  }, [loadProducts, loadCategories]);

  /**
   * Pagination helpers
   */
  const goToPage = useCallback((page: number) => {
    setParams(prev => ({ ...prev, pageNumber: page }));
  }, []);

  const nextPage = useCallback(() => {
    if (pagedData?.hasNextPage) {
      setParams(prev => ({ ...prev, pageNumber: (prev.pageNumber || 1) + 1 }));
    }
  }, [pagedData]);

  const previousPage = useCallback(() => {
    if (pagedData?.hasPreviousPage) {
      setParams(prev => ({ ...prev, pageNumber: Math.max((prev.pageNumber || 1) - 1, 1) }));
    }
  }, [pagedData]);

  /**
   * Auto load on mount và khi params thay đổi
   */
  useEffect(() => {
    if (autoLoad) {
      loadProducts(params);
    }
  }, [params.pageNumber, params.categoryId, params.sortBy, params.sortOrder, params.search]);

  /**
   * Load categories on mount
   */
  useEffect(() => {
    if (autoLoad) {
      loadCategories();
    }
  }, []);

  return {
    products,
    pagedData,
    categories,
    isLoading,
    isRefreshing,
    error,
    params,
    setParams,
    loadProducts,
    loadCategories,
    getProductById,
    createProduct,
    updateProduct,
    deleteProduct,
    refresh,
    goToPage,
    nextPage,
    previousPage,
  };
};

export default useProducts;

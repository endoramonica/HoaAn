import { useState, useEffect, useCallback } from 'react';
import { ProductService } from '@/api/services/ProductService';
import type { ProductListDtoPaginatedResultApiResponse, ProductDetailDtoApiResponse, ProductListDtoPaginatedResult } from '../../api';
import { toast } from 'sonner';

export function useProduct() {
  const [products, setProducts] = useState<ProductListDtoPaginatedResult | null>(null);
  const [productDetail, setProductDetail] = useState<ProductDetailDtoApiResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Fetch danh sách sản phẩm
  const fetchProducts = useCallback(async (
    page = 1,
    pageSize = 10,
    searchTerm?: string,
    categoryId?: string,
    storeId?: string
  ) => {
    setLoading(true);
    setError(null);
    try {
      const res = await ProductService.getApiV1Product(page, pageSize, searchTerm, categoryId, storeId);
      setProducts(res.data ?? null);
    } catch (err: any) {
      setError(err?.message || 'Lỗi khi lấy sản phẩm');
      toast.error(err?.message || 'Lỗi khi lấy sản phẩm');
    } finally {
      setLoading(false);
    }
  }, []);

  // Fetch chi tiết sản phẩm
  const fetchProductDetail = useCallback(async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      const res = await ProductService.getApiV1Product1(id);
      setProductDetail({ data: res.data }); // res.data là ProductDetailDto
    } catch (err: any) {
      setError(err?.message || 'Lỗi khi lấy chi tiết sản phẩm');
      toast.error(err?.message || 'Lỗi khi lấy chi tiết sản phẩm');
    } finally {
      setLoading(false);
    }
  }, []);

  // Cập nhật stock
  const updateStock = useCallback(async (id: string, stock: number) => {
    try {
      await ProductService.patchApiV1ProductStock(id, stock);
      toast.success('Cập nhật tồn kho thành công');
    } catch (err: any) {
      toast.error(err?.message || 'Lỗi khi cập nhật tồn kho');
    }
  }, []);

  // Cập nhật active / featured
  const updateActive = useCallback(async (id: string, active: boolean) => {
    try {
      await ProductService.patchApiV1ProductActive(id, active);
      toast.success('Cập nhật trạng thái thành công');
    } catch (err: any) {
      toast.error(err?.message || 'Lỗi khi cập nhật trạng thái');
    }
  }, []);

  const updateFeatured = useCallback(async (id: string, featured: boolean) => {
    try {
      await ProductService.patchApiV1ProductFeatured(id, featured);
      toast.success('Cập nhật nổi bật thành công');
    } catch (err: any) {
      toast.error(err?.message || 'Lỗi khi cập nhật nổi bật');
    }
  }, []);

  // Favorite / view
  const favoriteProduct = useCallback(async (id: string) => {
    try {
      await ProductService.postApiV1ProductFavorite(id);
      toast.success('Đã thêm vào yêu thích');
    } catch (err: any) {
      toast.error(err?.message || 'Lỗi khi thêm yêu thích');
    }
  }, []);

  const viewProduct = useCallback(async (id: string) => {
    try {
      await ProductService.postApiV1ProductView(id);
    } catch (err: any) {
      console.error('Error view product', err);
    }
  }, []);

  return {
    products,
    productDetail,
    loading,
    error,
    fetchProducts,
    fetchProductDetail,
    updateStock,
    updateActive,
    updateFeatured,
    favoriteProduct,
    viewProduct,
  };
}

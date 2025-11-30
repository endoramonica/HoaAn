/**
 * useProducts Hook - React Query Integration
 * ✅ UPDATED: Uses Orval generated API
 * ✅ React Query for caching and state management
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type {
  GetApiV1ProductParams,
  ProductCreateDto,
  ProductUpdateDto,
  PatchApiV1ProductIdStockBody,
  PatchApiV1ProductIdActiveBody,
  PatchApiV1ProductIdFeaturedBody,
} from '../../../Api/generated-orval/schemas';
import { toast } from 'sonner';

const api = getVietCommerceAPI();

// Query Keys
export const productsKeys = {
  all: ['products'] as const,
  lists: () => [...productsKeys.all, 'list'] as const,
  list: (params: GetApiV1ProductParams) => [...productsKeys.lists(), params] as const,
  details: () => [...productsKeys.all, 'detail'] as const,
  detail: (id: string) => [...productsKeys.details(), id] as const,
  slug: (slug: string) => [...productsKeys.details(), 'slug', slug] as const,
  category: (categoryId: string) => [...productsKeys.all, 'category', categoryId] as const,
  store: (storeId: string) => [...productsKeys.all, 'store', storeId] as const,
  favorites: () => [...productsKeys.all, 'favorites'] as const,
};

// Fetch Products List with Filters
export const useProducts = (params?: GetApiV1ProductParams) => {
  return useQuery({
    queryKey: productsKeys.list(params || {}),
    queryFn: async () => {
      const data = await api.getApiV1Product(params);
      return data;
    },
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
};

// Fetch Product Detail by ID
export const useProductDetail = (id: string) => {
  return useQuery({
    queryKey: productsKeys.detail(id),
    queryFn: async () => {
      const data = await api.getApiV1ProductId(id);
      return data;
    },
    enabled: !!id,
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
};

// Fetch Product by Slug
export const useProductBySlug = (slug: string) => {
  return useQuery({
    queryKey: productsKeys.slug(slug),
    queryFn: async () => {
      const data = await api.getApiV1ProductSlugSlug(slug);
      return data;
    },
    enabled: !!slug,
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
};

// Fetch Products by Category
export const useProductsByCategory = (categoryId: string) => {
  return useQuery({
    queryKey: productsKeys.category(categoryId),
    queryFn: async () => {
      const data = await api.getApiV1ProductCategoryCategoryId(categoryId);
      return data;
    },
    enabled: !!categoryId,
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
};

// Fetch Products by Store
export const useProductsByStore = (storeId: string) => {
  return useQuery({
    queryKey: productsKeys.store(storeId),
    queryFn: async () => {
      const data = await api.getApiV1ProductStoreStoreId(storeId);
      return data;
    },
    enabled: !!storeId,
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
};

// Fetch Favorite Products
export const useFavoriteProducts = () => {
  return useQuery({
    queryKey: productsKeys.favorites(),
    queryFn: async () => {
      const data = await api.getApiV1ProductFavorites();
      return data;
    },
    staleTime: 1000 * 60 * 2, // 2 minutes
  });
};

// Get Product Stock
export const useProductStock = (id: string) => {
  return useQuery({
    queryKey: [...productsKeys.detail(id), 'stock'],
    queryFn: async () => {
      const data = await api.getApiV1ProductIdStock(id);
      return data;
    },
    enabled: !!id,
    staleTime: 1000 * 30, // 30 seconds (stock changes frequently)
  });
};

// Create Product
export const useCreateProduct = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: ProductCreateDto) => {
      const result = await api.postApiV1Product(data);
      return result;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: productsKeys.lists() });
      toast.success('Tạo sản phẩm thành công!');
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể tạo sản phẩm');
    },
  });
};

// Update Product
export const useUpdateProduct = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, data }: { id: string; data: ProductUpdateDto }) => {
      const result = await api.putApiV1ProductId(id, data);
      return result;
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: productsKeys.detail(variables.id) });
      queryClient.invalidateQueries({ queryKey: productsKeys.lists() });
      toast.success('Cập nhật sản phẩm thành công!');
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể cập nhật sản phẩm');
    },
  });
};

// Delete Product
export const useDeleteProduct = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      const result = await api.deleteApiV1ProductId(id);
      return result;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: productsKeys.lists() });
      toast.success('Xóa sản phẩm thành công!');
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể xóa sản phẩm');
    },
  });
};

// Update Stock
export const useUpdateStock = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, quantity }: { id: string; quantity: number }) => {
      const body: PatchApiV1ProductIdStockBody = { quantity };
      const result = await api.patchApiV1ProductIdStock(id, body);
      return result;
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: productsKeys.detail(variables.id) });
      toast.success('Cập nhật tồn kho thành công!');
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể cập nhật tồn kho');
    },
  });
};

// Toggle Active Status
export const useToggleActive = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, isActive }: { id: string; isActive: boolean }) => {
      const body: PatchApiV1ProductIdActiveBody = { isActive };
      const result = await api.patchApiV1ProductIdActive(id, body);
      return result;
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: productsKeys.detail(variables.id) });
      queryClient.invalidateQueries({ queryKey: productsKeys.lists() });
      toast.success('Cập nhật trạng thái thành công!');
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể cập nhật trạng thái');
    },
  });
};

// Toggle Featured Status
export const useToggleFeatured = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, isFeatured }: { id: string; isFeatured: boolean }) => {
      const body: PatchApiV1ProductIdFeaturedBody = { isFeatured };
      const result = await api.patchApiV1ProductIdFeatured(id, body);
      return result;
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: productsKeys.detail(variables.id) });
      queryClient.invalidateQueries({ queryKey: productsKeys.lists() });
      toast.success('Cập nhật nổi bật thành công!');
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể cập nhật nổi bật');
    },
  });
};

// Track Product View
export const useTrackView = () => {
  return useMutation({
    mutationFn: async (id: string) => {
      const result = await api.postApiV1ProductIdView(id);
      return result;
    },
    // No toast for view tracking (silent)
  });
};

// Toggle Favorite
export const useToggleFavorite = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      const result = await api.postApiV1ProductIdFavorite(id);
      return result;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: productsKeys.favorites() });
      toast.success('Đã cập nhật yêu thích!');
    },
    onError: (error: any) => {
      toast.error(error?.message || 'Không thể cập nhật yêu thích');
    },
  });
};

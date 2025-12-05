/**
 * useCategories Hook - React Query Integration
 * ✅ Uses Orval generated API for Category endpoints
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type {
    GetApiV1CategoryPagedListParams,
    CreateCategoryDto,
    UpdateCategoryDto,
} from '../../../Api/generated-orval/schemas';
import { toast } from 'sonner';

const api = getVietCommerceAPI();

// Query Keys
export const categoryKeys = {
    all: ['categories'] as const,
    lists: () => [...categoryKeys.all, 'list'] as const,
    list: (params?: any) => [...categoryKeys.lists(), params] as const,
    active: () => [...categoryKeys.all, 'active'] as const,
    details: () => [...categoryKeys.all, 'detail'] as const,
    detail: (id: string) => [...categoryKeys.details(), id] as const,
    store: (storeId: string) => [...categoryKeys.all, 'store', storeId] as const,
    hierarchy: (storeId: string) => [...categoryKeys.all, 'hierarchy', storeId] as const,
    subcategories: (parentId: string) => [...categoryKeys.all, 'subcategories', parentId] as const,
};

// Category type for UI
export interface CategoryItem {
    id: string;
    name: string;
    slug?: string;
    description?: string;
    icon?: string;
    imageUrl?: string;
    parentId?: string;
    sortOrder?: number;
    isActive?: boolean;
    productCount?: number;
}

// Helper to extract data from nested response
const extractData = (response: any): any => {
    // Handle nested response: { success, data: { success, data: actualData } }
    if (response?.data?.data) {
        return response.data.data;
    }
    if (response?.data) {
        return response.data;
    }
    return response;
};

// Fetch All Categories
export const useCategories = () => {
    return useQuery({
        queryKey: categoryKeys.lists(),
        queryFn: async () => {
            const response = await api.getApiV1Category();
            const data = extractData(response);

            console.log('[useCategories] Raw response:', response);
            console.log('[useCategories] Extracted data:', data);

            // Map to CategoryItem array
            // Note: API returns productsCount, we map to productCount for consistency
            const items: CategoryItem[] = (Array.isArray(data) ? data : []).map((item: any) => ({
                id: item.id || '',
                name: item.name || '',
                slug: item.slug,
                description: item.description,
                icon: item.icon,
                imageUrl: item.imageUrl,
                parentId: item.parentId,
                sortOrder: item.sortOrder,
                isActive: item.isActive,
                productCount: item.productsCount || item.productCount || 0,
            }));

            return items;
        },
        staleTime: 1000 * 60 * 10, // 10 minutes (categories don't change often)
    });
};

// Fetch Active Categories Only
export const useActiveCategories = () => {
    return useQuery({
        queryKey: categoryKeys.active(),
        queryFn: async () => {
            const response = await api.getApiV1CategoryActiveList();
            const data = extractData(response);

            console.log('[useActiveCategories] Raw response:', response);
            console.log('[useActiveCategories] Extracted data:', data);

            // Note: API returns productsCount, we map to productCount for consistency
            const items: CategoryItem[] = (Array.isArray(data) ? data : []).map((item: any) => ({
                id: item.id || '',
                name: item.name || '',
                slug: item.slug,
                description: item.description,
                icon: item.icon,
                imageUrl: item.imageUrl,
                parentId: item.parentId,
                sortOrder: item.sortOrder,
                isActive: item.isActive,
                productCount: item.productsCount || item.productCount || 0,
            }));

            return items;
        },
        staleTime: 1000 * 60 * 10, // 10 minutes
    });
};

// Fetch Category by ID
export const useCategoryDetail = (id: string) => {
    return useQuery({
        queryKey: categoryKeys.detail(id),
        queryFn: async () => {
            const response = await api.getApiV1CategoryId(id);
            const data = extractData(response);

            return {
                id: data?.id || '',
                name: data?.name || '',
                slug: data?.slug,
                description: data?.description,
                icon: data?.icon,
                imageUrl: data?.imageUrl,
                parentId: data?.parentId,
                sortOrder: data?.sortOrder,
                isActive: data?.isActive,
                productCount: data?.productsCount || data?.productCount || 0,
            } as CategoryItem;
        },
        enabled: !!id,
        staleTime: 1000 * 60 * 10,
    });
};

// Fetch Category Details (with more info)
export const useCategoryDetails = (id: string) => {
    return useQuery({
        queryKey: [...categoryKeys.detail(id), 'details'],
        queryFn: async () => {
            const response = await api.getApiV1CategoryIdDetails(id);
            return extractData(response);
        },
        enabled: !!id,
        staleTime: 1000 * 60 * 10,
    });
};

// Fetch Categories by Store
export const useCategoriesByStore = (storeId: string) => {
    return useQuery({
        queryKey: categoryKeys.store(storeId),
        queryFn: async () => {
            const response = await api.getApiV1CategoryStoreStoreId(storeId);
            const data = extractData(response);

            const items: CategoryItem[] = (Array.isArray(data) ? data : []).map((item: any) => ({
                id: item.id || '',
                name: item.name || '',
                slug: item.slug,
                description: item.description,
                icon: item.icon,
                imageUrl: item.imageUrl,
                parentId: item.parentId,
                sortOrder: item.sortOrder,
                isActive: item.isActive,
                productCount: item.productsCount || item.productCount || 0,
            }));

            return items;
        },
        enabled: !!storeId,
        staleTime: 1000 * 60 * 10,
    });
};

// Fetch Category Hierarchy by Store
export const useCategoryHierarchy = (storeId: string) => {
    return useQuery({
        queryKey: categoryKeys.hierarchy(storeId),
        queryFn: async () => {
            const response = await api.getApiV1CategoryHierarchyStoreId(storeId);
            return extractData(response);
        },
        enabled: !!storeId,
        staleTime: 1000 * 60 * 10,
    });
};

// Fetch Subcategories
export const useSubcategories = (parentId: string) => {
    return useQuery({
        queryKey: categoryKeys.subcategories(parentId),
        queryFn: async () => {
            const response = await api.getApiV1CategoryParentIdSubcategories(parentId);
            const data = extractData(response);

            const items: CategoryItem[] = (Array.isArray(data) ? data : []).map((item: any) => ({
                id: item.id || '',
                name: item.name || '',
                slug: item.slug,
                description: item.description,
                icon: item.icon,
                imageUrl: item.imageUrl,
                parentId: item.parentId,
                sortOrder: item.sortOrder,
                isActive: item.isActive,
                productCount: item.productsCount || item.productCount || 0,
            }));

            return items;
        },
        enabled: !!parentId,
        staleTime: 1000 * 60 * 10,
    });
};

// Fetch Paged Categories (Admin)
export const usePagedCategories = (params?: GetApiV1CategoryPagedListParams) => {
    return useQuery({
        queryKey: categoryKeys.list(params),
        queryFn: async () => {
            const response = await api.getApiV1CategoryPagedList(params);
            const data = extractData(response);

            return {
                items: (data?.items || []).map((item: any) => ({
                    id: item.id || '',
                    name: item.name || '',
                    slug: item.slug,
                    description: item.description,
                    icon: item.icon,
                    imageUrl: item.imageUrl,
                    parentId: item.parentId,
                    sortOrder: item.sortOrder,
                    isActive: item.isActive,
                    productCount: item.productsCount || item.productCount || 0,
                })) as CategoryItem[],
                pageNumber: data?.pageNumber || 1,
                pageSize: data?.pageSize || 10,
                totalItems: data?.totalItems || 0,
                totalPages: data?.totalPages || 0,
            };
        },
        staleTime: 1000 * 60 * 5,
    });
};

// Create Category (Admin)
export const useCreateCategory = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (data: CreateCategoryDto) => {
            const response = await api.postApiV1Category(data);
            return extractData(response);
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: categoryKeys.all });
            toast.success('Tạo danh mục thành công!');
        },
        onError: (error: any) => {
            toast.error(error?.message || 'Không thể tạo danh mục');
        },
    });
};

// Update Category (Admin)
export const useUpdateCategory = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({ id, data }: { id: string; data: UpdateCategoryDto }) => {
            const response = await api.putApiV1CategoryId(id, data);
            return extractData(response);
        },
        onSuccess: (_, variables) => {
            queryClient.invalidateQueries({ queryKey: categoryKeys.detail(variables.id) });
            queryClient.invalidateQueries({ queryKey: categoryKeys.lists() });
            toast.success('Cập nhật danh mục thành công!');
        },
        onError: (error: any) => {
            toast.error(error?.message || 'Không thể cập nhật danh mục');
        },
    });
};

// Delete Category (Admin)
export const useDeleteCategory = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (id: string) => {
            const response = await api.deleteApiV1CategoryId(id);
            return extractData(response);
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: categoryKeys.all });
            toast.success('Xóa danh mục thành công!');
        },
        onError: (error: any) => {
            toast.error(error?.message || 'Không thể xóa danh mục');
        },
    });
};

// Soft Delete Category (Admin)
export const useSoftDeleteCategory = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (id: string) => {
            const response = await api.patchApiV1CategoryIdSoftDelete(id);
            return extractData(response);
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: categoryKeys.all });
            toast.success('Đã ẩn danh mục!');
        },
        onError: (error: any) => {
            toast.error(error?.message || 'Không thể ẩn danh mục');
        },
    });
};

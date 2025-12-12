/**
 * useProductsByCategory Hook - Fetch products by category
 */

import { useQuery } from '@tanstack/react-query';
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import { useActiveCategories } from './useCategories';

const api = getVietCommerceAPI();

// Query Keys
export const productCategoryKeys = {
    all: ['products-by-category'] as const,
    byCategory: (categoryId: string) => [...productCategoryKeys.all, categoryId] as const,
    top: (categoryId: string, limit: number) => [...productCategoryKeys.byCategory(categoryId), 'top', limit] as const,
};

// Product interface matching API response
interface ProductResponse {
    id: string;
    name: string;
    code?: string;
    slug?: string;
    price: number;
    compareAtPrice?: number;
    displayPrice?: {
        originalPrice: number;
        discountedPrice: number;
        discountAmount: number;
    };
    stockQuantity: number;
    inStock: boolean;
    primaryImage?: string;
    images?: Array<{
        id: string;
        url: string;
        thumbnailUrl?: string;
        isMain?: boolean;
    }>;
    categoryId: string;
    categoryName?: string;
    viewCount?: number;
    favoriteCount?: number;
    averageRating?: number;
}

// Mapped product for UI
export interface MappedProduct {
    id: string;
    name: string;
    slug?: string;
    price: number;
    originalPrice: number;
    discount: number;
    imageUrl: string;
    rating: number;
    reviewCount: number;
    inStock: boolean;
    categoryId: string;
}

// Helper to extract data from nested response
const extractData = (response: any): any => {
    if (response?.data?.data) {
        return response.data.data;
    }
    if (response?.data) {
        return response.data;
    }
    return response;
};

// Helper to get main image from product
const getMainImage = (product: ProductResponse): string => {
    // Try to get main image from images array
    if (product.images && product.images.length > 0) {
        const mainImage = product.images.find(img => img.isMain);
        if (mainImage?.url) return mainImage.url;
        // Fallback to first image
        if (product.images[0]?.url) return product.images[0].url;
    }
    // Fallback to primaryImage
    if (product.primaryImage) return product.primaryImage;
    // Default fallback
    return 'https://via.placeholder.com/300x300?text=No+Image';
};

// Helper to calculate discount percentage
const calculateDiscount = (product: ProductResponse): number => {
    if (product.displayPrice?.discountAmount && product.displayPrice?.originalPrice) {
        return Math.round((product.displayPrice.discountAmount / product.displayPrice.originalPrice) * 100);
    }
    return 0;
};

// Fetch products by category
export const useProductsByCategory = (categoryId: string, limit?: number) => {
    return useQuery({
        queryKey: limit ? productCategoryKeys.top(categoryId, limit) : productCategoryKeys.byCategory(categoryId),
        queryFn: async () => {
            const response = await api.getApiV1ProductCategoryCategoryId(categoryId);
            const data = extractData(response);

            console.log('[useProductsByCategory] Raw response:', response);
            console.log('[useProductsByCategory] Extracted data:', data);

            // Map to product array - handle both array and paginated response
            let products: ProductResponse[] = Array.isArray(data) ? data : data?.items || [];

            // Limit results if specified
            if (limit) {
                products = products.slice(0, limit);
            }

            return products.map((item: ProductResponse): MappedProduct => {
                const originalPrice = item.displayPrice?.originalPrice || item.compareAtPrice || item.price;
                const discountedPrice = item.displayPrice?.discountedPrice || item.price;
                const discount = calculateDiscount(item);

                return {
                    id: item.id || '',
                    name: item.name || '',
                    slug: item.slug,
                    price: discountedPrice,
                    originalPrice: originalPrice,
                    discount: discount,
                    imageUrl: getMainImage(item),
                    rating: item.averageRating || 0,
                    reviewCount: item.viewCount || 0, // Using viewCount as review count
                    inStock: item.inStock && item.stockQuantity > 0,
                    categoryId: item.categoryId,
                };
            });
        },
        enabled: !!categoryId,
        staleTime: 1000 * 60 * 5, // 5 minutes
    });
};

// Fetch top N products by category
export const useTopProductsByCategory = (categoryId: string, limit: number = 5) => {
    return useProductsByCategory(categoryId, limit);
};

// Get category name by ID
export const useCategoryNameById = (categoryId: string) => {
    const { data: categories } = useActiveCategories();

    const categoryName = categories?.find(cat => cat.id === categoryId)?.name || '';

    return categoryName;
};

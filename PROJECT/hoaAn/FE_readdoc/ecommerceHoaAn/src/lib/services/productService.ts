/**
 * Product Service
 * Handles API calls for product operations
 */

import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type { ProductDetailDto } from '../../../Api/generated-orval/schemas/productDetailDto';
import type { ProductListDto } from '../../../Api/generated-orval/schemas/productListDto';

const api = getVietCommerceAPI();

export const productService = {
  /**
   * Get product detail by ID
   * GET /api/v1/Product/{id}
   */
  async getProductById(id: string): Promise<ProductDetailDto> {
    try {
      const response = await api.getApiV1ProductId(id);
      console.log('[productService] Get product detail response:', response);

      const product = response.data;
      if (!product) {
        throw new Error('No product data returned');
      }

      return product;
    } catch (error) {
      console.error('[productService] Failed to get product:', error);
      throw error;
    }
  },

  /**
   * Get product by slug
   * GET /api/v1/Product/slug/{slug}
   */
  async getProductBySlug(slug: string): Promise<ProductDetailDto> {
    try {
      const response = await api.getApiV1ProductSlugSlug(slug);
      console.log('[productService] Get product by slug response:', response);

      const product = response.data;
      if (!product) {
        throw new Error('No product data returned');
      }

      return product;
    } catch (error) {
      console.error('[productService] Failed to get product by slug:', error);
      throw error;
    }
  },

  /**
   * Get products by category
   * GET /api/v1/Product/category/{categoryId}
   */
  async getProductsByCategory(categoryId: string): Promise<ProductListDto[]> {
    try {
      const response = await api.getApiV1ProductCategoryCategoryId(categoryId);
      console.log('[productService] Get products by category response:', response);

      const products = response.data || [];
      return Array.isArray(products) ? products : [];
    } catch (error) {
      console.error('[productService] Failed to get products by category:', error);
      throw error;
    }
  },

  /**
   * Record product view
   * POST /api/v1/Product/{id}/view
   */
  async recordProductView(id: string): Promise<void> {
    try {
      await api.postApiV1ProductIdView(id);
      console.log('[productService] Product view recorded:', id);
    } catch (error) {
      console.error('[productService] Failed to record product view:', error);
      // Don't throw - this is not critical
    }
  },

  /**
   * Toggle product favorite
   * POST /api/v1/Product/{id}/favorite
   */
  async toggleProductFavorite(id: string): Promise<boolean> {
    try {
      const response = await api.postApiV1ProductIdFavorite(id);
      console.log('[productService] Toggle favorite response:', response);

      const success = response.data || response.success;
      return !!success;
    } catch (error) {
      console.error('[productService] Failed to toggle favorite:', error);
      throw error;
    }
  },

  /**
   * Get favorite products
   * GET /api/v1/Product/favorites
   */
  async getFavoriteProducts(): Promise<ProductDetailDto[]> {
    try {
      const response = await api.getApiV1ProductFavorites();
      console.log('[productService] Get favorites response:', response);

      const products = response.data || [];
      return Array.isArray(products) ? products : [];
    } catch (error) {
      console.error('[productService] Failed to get favorites:', error);
      throw error;
    }
  },
};

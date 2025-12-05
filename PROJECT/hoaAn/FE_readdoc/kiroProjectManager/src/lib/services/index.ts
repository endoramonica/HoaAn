/**
 * Services Index
 * 
 * Central export point for all service layers.
 * Import services from here for cleaner imports.
 * 
 * @example
 * import { customerService, productService } from '@/lib/services';
 */

// Auth service
export * from './authService';

// Customer service
export { customerService } from './customerService';

// Product service
export { productService } from './productService';

// Category service
export { categoryService } from './categoryService';

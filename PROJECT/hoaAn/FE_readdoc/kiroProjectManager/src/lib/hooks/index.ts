/**
 * Hooks Index
 * 
 * Central export point for all React Query hooks.
 * Import hooks from here for cleaner imports.
 * 
 * @example
 * // Custom hooks (recommended - includes cache invalidation)
 * import { useCustomers, useProducts } from '@/lib/hooks';
 * 
 * // Generated hooks (direct access)
 * import { useGetApiAdminCustomer } from '@/lib/hooks/generated';
 */

// Custom hooks with enhanced functionality
export * from './useCustomers';
export * from './useProducts';
export * from './useCategories';

// All generated hooks (for direct access)
export * from './generated';

/**
 * Service-related types and enums
 * Used for Services-Product Unification feature
 */

/**
 * Service categories available in the system
 */
export enum ServiceCategory {
    ANCESTOR_WORSHIP = 'ancestor-worship',
    OPENING_CEREMONY = 'opening-ceremony',
    WEDDING = 'wedding',
    BUDDHA_WORSHIP = 'buddha-worship',
    NEW_HOUSE = 'new-house',
    FENG_SHUI_CONSULTATION = 'feng-shui-consultation',
}

/**
 * Human-readable labels for service categories
 */
export const SERVICE_CATEGORY_LABELS: Record<ServiceCategory, string> = {
    [ServiceCategory.ANCESTOR_WORSHIP]: 'Cúng Gia Tiên',
    [ServiceCategory.OPENING_CEREMONY]: 'Lễ Khai Trương',
    [ServiceCategory.WEDDING]: 'Lễ Cưới Hỏi',
    [ServiceCategory.BUDDHA_WORSHIP]: 'Cúng Phật',
    [ServiceCategory.NEW_HOUSE]: 'Lễ Tân Gia',
    [ServiceCategory.FENG_SHUI_CONSULTATION]: 'Tư Vấn Phong Thủy',
};

/**
 * Product/Service type discriminator
 */
export type ItemType = 'product' | 'service';

/**
 * Service-specific product information
 */
export interface ServiceInfo {
    type: 'service';
    serviceCategory: ServiceCategory;
    serviceDuration: string; // e.g., "2-3 giờ"
    serviceRating?: number; // 0-5 stars
}

/**
 * Service-specific order information
 */
export interface ServiceOrderInfo {
    type: 'service';
    serviceCategory: ServiceCategory;
    serviceDuration: string;
    serviceLocation: string; // Address where service will be performed
    serviceDate: Date; // Scheduled date
    serviceTime?: string; // Scheduled time (e.g., "09:00")
    serviceNotes?: string; // Additional notes/requirements
}

/**
 * Service-specific order item information
 */
export interface ServiceOrderItemInfo {
    type: 'service';
    serviceCategory: ServiceCategory;
    serviceDuration: string;
}

/**
 * Filter options for services
 */
export interface ServiceFilterOptions {
    type?: ItemType;
    serviceCategory?: ServiceCategory | string;
    searchTerm?: string;
    page?: number;
    pageSize?: number;
    sortBy?: string;
    isDescending?: boolean;
}

/**
 * Filter options for service orders
 */
export interface ServiceOrderFilterOptions {
    type?: ItemType;
    serviceCategory?: ServiceCategory | string;
    status?: string;
    page?: number;
    pageSize?: number;
    sortBy?: string;
    sortDescending?: boolean;
}

/**
 * Get label for a service category
 */
export function getServiceCategoryLabel(category: ServiceCategory | string): string {
    return SERVICE_CATEGORY_LABELS[category as ServiceCategory] || category;
}

/**
 * Check if a category is a valid service category
 */
export function isValidServiceCategory(category: string): category is ServiceCategory {
    return Object.values(ServiceCategory).includes(category as ServiceCategory);
}

/**
 * Get all available service categories
 */
export function getAllServiceCategories(): ServiceCategory[] {
    return Object.values(ServiceCategory);
}

/**
 * Get all service categories with labels
 */
export function getServiceCategoriesWithLabels(): Array<{
    value: ServiceCategory;
    label: string;
}> {
    return getAllServiceCategories().map((category) => ({
        value: category,
        label: SERVICE_CATEGORY_LABELS[category],
    }));
}

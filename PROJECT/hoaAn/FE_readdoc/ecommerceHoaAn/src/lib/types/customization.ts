/**
 * Customization Types for Frontend Integration
 * Sử dụng cho cart operations với customizable products
 */

// ============================================
// CUSTOMIZABLE OPTION (từ Product API)
// ============================================
export interface CustomizableOption {
    /** Unique identifier: "opt-incense-burner", "opt-fruit-tray", etc */
    id: string;
    /** Display name: "Lư hương", "Mâm trái cây", etc */
    name: string;
    /** Default quantity included in package */
    baseQuantity: number;
    /** Price per unit */
    unitPrice: number;
    /** Minimum quantity customer can order */
    minQuantity: number;
    /** Maximum quantity customer can order (null = unlimited) */
    maxQuantity: number | null;
    /** Unit of measurement: "cái", "mâm", "dĩa", etc */
    unit: string;
}

// ============================================
// CUSTOMIZATION (khi add to cart)
// ============================================
export interface CartItemCustomization {
    /** Option ID */
    optionId: string;
    /** Quantity selected by customer */
    quantity: number;
    /** Unit price at time of adding to cart */
    unitPrice: number;
    /** Total price: quantity * unitPrice */
    totalPrice: number;
}

// ============================================
// ADD TO CART REQUEST
// ============================================
export interface AddToCartRequest {
    /** Product ID */
    productId: string;
    /** Quantity of product */
    quantity: number;
    /** Customizations (optional, only for customizable products) */
    customizations?: CartItemCustomization[];
}

// ============================================
// ADD TO CART RESPONSE
// ============================================
export interface AddToCartResponse {
    /** Cart item ID */
    cartItemId: string;
    /** Product ID */
    productId: string;
    /** Quantity added */
    quantity: number;
    /** Final unit price (basePrice + customizationPrice) */
    unitPrice: number;
    /** Total items in cart */
    cartItemCount: number;
    /** Total cart amount */
    cartTotalAmount: number;
}

// ============================================
// CART ITEM DETAIL (in cart response)
// ============================================
export interface CartItemDetail {
    /** Cart item ID */
    cartItemId: string;
    /** Product ID */
    productId: string;
    /** Product name */
    productName: string;
    /** Product slug for URL */
    productSlug: string;
    /** Product SKU */
    sku: string;
    /** Product image URL */
    productImage?: string;
    /** Base product price (without customizations) */
    unitPrice: number;
    /** Quantity in cart */
    quantity: number;
    /** Total price: quantity * unitPrice (without customizations) */
    totalPrice: number;
    /** Available stock */
    availableStock: number;
    /** Is product active */
    isProductActive: boolean;
    /** Number of purchases */
    purchaseCount: number;
    /** Average rating (1-5) */
    avgRating: number;
    /** Number of reviews */
    reviewCount: number;
    /** When added to cart */
    createdAt: string;
    /** Last updated */
    updatedAt: string;

    // ========== CUSTOMIZATION FIELDS ==========
    /** Base price of product (before customizations) */
    basePrice: number;
    /** Total price of all customizations */
    customizationPrice: number;
    /** Final price: basePrice + customizationPrice */
    finalPrice: number;
    /** Customizations applied to this item */
    customizations?: CartItemCustomization[];
}

// ============================================
// GET CART RESPONSE
// ============================================
export interface GetCartResponse {
    /** Cart ID */
    cartId: string;
    /** User ID (if logged in) */
    userId?: string;
    /** Cart items */
    items: CartItemDetail[];
    /** Total items count */
    totalItems: number;
    /** Subtotal (sum of all finalPrice * quantity) */
    subTotal: number;
    /** Tax amount */
    taxAmount: number;
    /** Shipping fee */
    shippingFee: number;
    /** Total amount */
    totalAmount: number;
    /** When cart was created */
    createdAt: string;
    /** Last updated */
    updatedAt: string;
}

// ============================================
// HELPER FUNCTIONS
// ============================================

/**
 * Calculate customization price for a set of customizations
 */
export function calculateCustomizationPrice(customizations: CartItemCustomization[]): number {
    return customizations.reduce((sum, c) => sum + c.totalPrice, 0);
}

/**
 * Calculate final price for a cart item
 */
export function calculateFinalPrice(
    basePrice: number,
    customizations?: CartItemCustomization[]
): number {
    const customizationPrice = customizations
        ? calculateCustomizationPrice(customizations)
        : 0;
    return basePrice + customizationPrice;
}

/**
 * Validate customization quantity against option bounds
 */
export function isValidCustomizationQuantity(
    quantity: number,
    option: CustomizableOption
): boolean {
    return (
        quantity >= option.minQuantity &&
        (option.maxQuantity === null || quantity <= option.maxQuantity)
    );
}

/**
 * Build customization from user input
 */
export function buildCustomization(
    optionId: string,
    quantity: number,
    unitPrice: number
): CartItemCustomization {
    return {
        optionId,
        quantity,
        unitPrice,
        totalPrice: quantity * unitPrice,
    };
}

/**
 * Filter out zero-quantity customizations
 */
export function filterActiveCustomizations(
    customizations: CartItemCustomization[]
): CartItemCustomization[] {
    return customizations.filter(c => c.quantity > 0);
}

/**
 * Format price for display (Vietnamese Dong)
 */
export function formatPrice(price: number): string {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    }).format(price);
}

/**
 * Calculate price breakdown for display
 */
export interface PriceBreakdown {
    basePrice: number;
    customizationPrice: number;
    finalPrice: number;
    quantity: number;
    totalLinePrice: number;
}

export function calculatePriceBreakdown(
    basePrice: number,
    customizations: CartItemCustomization[] | undefined,
    quantity: number
): PriceBreakdown {
    const customizationPrice = customizations
        ? calculateCustomizationPrice(customizations)
        : 0;
    const finalPrice = basePrice + customizationPrice;
    return {
        basePrice,
        customizationPrice,
        finalPrice,
        quantity,
        totalLinePrice: finalPrice * quantity,
    };
}

/**
 * Customization Service
 * Xử lý việc thêm customizations vào cart
 * Gửi request với customization options đến backend
 */

import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type { AddToCartDto } from '../../../Api/generated-orval/schemas';
import type { CustomizationOption } from '../hooks/useCustomizationState';

const api = getVietCommerceAPI();

export interface AddToCartWithCustomizationRequest {
    productId: string;
    quantity: number;
    customizations: CustomizationOption[];
}

export interface AddToCartResponse {
    success: boolean;
    data: {
        cartItemId: string;
        cartId: string;
        productId: string;
        quantity: number;
        basePrice: number;
        customizationPrice: number;
        finalPrice: number;
    };
    message: string;
}

/**
 * Customization Service
 */
export const customizationService = {
    /**
     * Thêm sản phẩm vào giỏ hàng với customizations
     */
    addToCartWithCustomizations: async (
        request: AddToCartWithCustomizationRequest
    ): Promise<AddToCartResponse> => {
        try {
            console.log('[customizationService] 🛒 Adding to cart with customizations:', {
                productId: request.productId,
                quantity: request.quantity,
                customizationCount: request.customizations.length
            });

            // Log each customization
            request.customizations.forEach(custom => {
                console.log('[customizationService]   - Option:', {
                    optionId: custom.optionId,
                    selectedQuantity: custom.selectedQuantity,
                    baseQuantity: custom.baseQuantity,
                    additionalQuantity: custom.selectedQuantity - custom.baseQuantity,
                    unitPrice: custom.unitPrice,
                    totalPrice: (custom.selectedQuantity - custom.baseQuantity) * custom.unitPrice
                });
            });

            // Build AddToCartDto
            const addToCartDto: AddToCartDto = {
                productId: request.productId,
                quantity: request.quantity,
                // Note: Backend should handle customizations separately
                // or we need to extend AddToCartDto to include customizations
            };

            console.log('[customizationService] 📦 Sending request to backend');

            // Call backend API
            const response = await api.postApiV1CartAdd(addToCartDto);

            console.log('[customizationService] ✅ Item added to cart:', {
                cartItemId: response.data?.cartItemId,
                finalPrice: response.data?.finalPrice
            });

            return {
                success: true,
                data: response.data as any,
                message: 'Item added to cart with customizations'
            };
        } catch (err: any) {
            console.error('[customizationService] ❌ Failed to add to cart:', err);
            throw err;
        }
    },

    /**
     * Lưu customization state tạm thời
     * Để user có thể quay lại sau
     */
    saveCustomizationState: (
        productId: string,
        customizations: CustomizationOption[]
    ): void => {
        try {
            const state = {
                productId,
                customizations,
                savedAt: new Date().toISOString()
            };

            sessionStorage.setItem(
                `customization_${productId}`,
                JSON.stringify(state)
            );

            console.log('[customizationService] 💾 Customization state saved:', {
                productId,
                optionCount: customizations.length
            });
        } catch (err) {
            console.error('[customizationService] ❌ Failed to save state:', err);
        }
    },

    /**
     * Lấy customization state đã lưu
     */
    loadCustomizationState: (productId: string): CustomizationOption[] | null => {
        try {
            const stateStr = sessionStorage.getItem(`customization_${productId}`);
            if (!stateStr) return null;

            const state = JSON.parse(stateStr);
            console.log('[customizationService] 📂 Customization state loaded:', {
                productId,
                optionCount: state.customizations.length
            });

            return state.customizations;
        } catch (err) {
            console.error('[customizationService] ❌ Failed to load state:', err);
            return null;
        }
    },

    /**
     * Xóa customization state
     */
    clearCustomizationState: (productId: string): void => {
        try {
            sessionStorage.removeItem(`customization_${productId}`);
            console.log('[customizationService] 🗑️ Customization state cleared:', productId);
        } catch (err) {
            console.error('[customizationService] ❌ Failed to clear state:', err);
        }
    },

    /**
     * Tính tổng giá customizations
     */
    calculateCustomizationPrice: (customizations: CustomizationOption[]): number => {
        return customizations.reduce((sum, custom) => {
            const additionalQuantity = custom.selectedQuantity - custom.baseQuantity;
            const additionalPrice = Math.max(0, additionalQuantity) * custom.unitPrice;
            return sum + additionalPrice;
        }, 0);
    },

    /**
     * Tính tổng giá cuối cùng (base + customization)
     */
    calculateFinalPrice: (
        basePrice: number,
        customizations: CustomizationOption[]
    ): number => {
        const customizationPrice = customizationService.calculateCustomizationPrice(customizations);
        return basePrice + customizationPrice;
    },

    /**
     * Validate customizations
     */
    validateCustomizations: (customizations: CustomizationOption[]): { valid: boolean; errors: string[] } => {
        const errors: string[] = [];

        customizations.forEach(custom => {
            if (custom.selectedQuantity < custom.minQuantity) {
                errors.push(
                    `${custom.name}: Minimum quantity is ${custom.minQuantity}`
                );
            }

            if (custom.selectedQuantity > custom.maxQuantity) {
                errors.push(
                    `${custom.name}: Maximum quantity is ${custom.maxQuantity}`
                );
            }
        });

        return {
            valid: errors.length === 0,
            errors
        };
    }
};

export default customizationService;

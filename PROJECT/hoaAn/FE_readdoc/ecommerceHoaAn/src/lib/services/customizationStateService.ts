/**
 * Customization State Service
 * Quản lý lưu trữ trạng thái lựa chọn customization cho service products
 */

import type { CustomizableOptionDto } from '../../Api/generated-orval/schemas';

export interface SavedCustomizationState {
    productId: string;
    productName: string;
    productType?: string;
    customizations: Array<{
        optionId: string;
        optionName: string;
        quantity: number;
        unitPrice: number;
        unit: string;
    }>;
    savedAt: string; // ISO timestamp
}

const STORAGE_KEY = 'vietcommerce_customization_state';

class CustomizationStateService {
    /**
     * Lưu trạng thái customization cho sản phẩm
     */
    saveCustomizationState(
        productId: string,
        productName: string,
        productType: string | undefined,
        customizableOptions: CustomizableOptionDto[],
        customizationQuantities: Record<string, number>
    ): SavedCustomizationState {
        const customizations = customizableOptions.map(option => ({
            optionId: option.id,
            optionName: option.name,
            quantity: customizationQuantities[option.id] || 0,
            unitPrice: option.unitPrice,
            unit: option.unit,
        }));

        const state: SavedCustomizationState = {
            productId,
            productName,
            productType,
            customizations,
            savedAt: new Date().toISOString(),
        };

        // Lưu vào localStorage
        const allStates = this.getAllSavedStates();
        const existingIndex = allStates.findIndex(s => s.productId === productId);

        if (existingIndex >= 0) {
            allStates[existingIndex] = state;
        } else {
            allStates.push(state);
        }

        localStorage.setItem(STORAGE_KEY, JSON.stringify(allStates));
        console.log('[CustomizationStateService] Saved state for product:', productId, state);

        return state;
    }

    /**
     * Lấy trạng thái customization đã lưu cho sản phẩm
     */
    getCustomizationState(productId: string): SavedCustomizationState | null {
        const allStates = this.getAllSavedStates();
        const state = allStates.find(s => s.productId === productId);

        if (state) {
            console.log('[CustomizationStateService] Retrieved state for product:', productId, state);
        }

        return state || null;
    }

    /**
     * Lấy tất cả trạng thái customization đã lưu
     */
    getAllSavedStates(): SavedCustomizationState[] {
        try {
            const data = localStorage.getItem(STORAGE_KEY);
            return data ? JSON.parse(data) : [];
        } catch (error) {
            console.error('[CustomizationStateService] Error reading saved states:', error);
            return [];
        }
    }

    /**
     * Xóa trạng thái customization cho sản phẩm
     */
    clearCustomizationState(productId: string): void {
        const allStates = this.getAllSavedStates();
        const filtered = allStates.filter(s => s.productId !== productId);
        localStorage.setItem(STORAGE_KEY, JSON.stringify(filtered));
        console.log('[CustomizationStateService] Cleared state for product:', productId);
    }

    /**
     * Xóa tất cả trạng thái customization
     */
    clearAllCustomizationStates(): void {
        localStorage.removeItem(STORAGE_KEY);
        console.log('[CustomizationStateService] Cleared all saved states');
    }

    /**
     * Chuyển đổi SavedCustomizationState thành Record<string, number> cho component
     */
    stateToQuantitiesMap(state: SavedCustomizationState): Record<string, number> {
        const map: Record<string, number> = {};
        state.customizations.forEach(custom => {
            map[custom.optionId] = custom.quantity;
        });
        return map;
    }

    /**
     * Tính tổng giá trị customization
     */
    calculateTotalCustomizationPrice(state: SavedCustomizationState): number {
        return state.customizations.reduce((total, custom) => {
            return total + (custom.quantity * custom.unitPrice);
        }, 0);
    }
}

export const customizationStateService = new CustomizationStateService();
export default customizationStateService;

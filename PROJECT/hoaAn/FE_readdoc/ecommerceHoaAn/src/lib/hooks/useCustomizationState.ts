/**
 * useCustomizationState Hook
 * Quản lý state tạm thời cho customizable options
 * Lưu lựa chọn của user trước khi add to cart hoặc booking
 */

import { useState, useCallback, useEffect } from 'react';

export interface CustomizationOption {
    optionId: string;
    name: string;
    baseQuantity: number;
    unitPrice: number;
    minQuantity: number;
    maxQuantity: number;
    unit: string;
    selectedQuantity: number; // User's selection
}

export interface CustomizationState {
    productId: string;
    productName: string;
    options: CustomizationOption[];
    totalCustomizationPrice: number;
    savedAt: string;
}

interface UseCustomizationStateReturn {
    // State
    customizationState: CustomizationState | null;
    selectedOptions: Map<string, number>;
    totalPrice: number;

    // Actions
    initializeCustomization: (productId: string, productName: string, options: CustomizationOption[]) => void;
    updateOptionQuantity: (optionId: string, quantity: number) => void;
    getSelectedOptions: () => CustomizationOption[];
    getTotalPrice: () => number;
    saveState: () => void;
    loadState: (productId: string) => CustomizationState | null;
    clearState: (productId: string) => void;
    resetOptions: () => void;
}

const STORAGE_KEY = 'customization_state';

/**
 * Hook để quản lý customization state
 * Lưu trữ ở localStorage để persist khi user navigate
 */
export function useCustomizationState(): UseCustomizationStateReturn {
    const [customizationState, setCustomizationState] = useState<CustomizationState | null>(null);
    const [selectedOptions, setSelectedOptions] = useState<Map<string, number>>(new Map());

    // ============================================================================
    // Initialize Customization
    // ============================================================================
    const initializeCustomization = useCallback(
        (productId: string, productName: string, options: CustomizationOption[]) => {
            console.log('[useCustomizationState] 🎯 Initializing customization:', {
                productId,
                productName,
                optionCount: options.length
            });

            // Try to load existing state
            const existingState = loadState(productId);
            if (existingState) {
                console.log('[useCustomizationState] ✅ Loaded existing state');
                setCustomizationState(existingState);

                // Restore selected quantities
                const newMap = new Map<string, number>();
                existingState.options.forEach(opt => {
                    newMap.set(opt.optionId, opt.selectedQuantity);
                });
                setSelectedOptions(newMap);
                return;
            }

            // Create new state
            const newState: CustomizationState = {
                productId,
                productName,
                options: options.map(opt => ({
                    ...opt,
                    selectedQuantity: opt.baseQuantity // Default to base quantity
                })),
                totalCustomizationPrice: 0,
                savedAt: new Date().toISOString()
            };

            setCustomizationState(newState);

            // Initialize selected options map
            const newMap = new Map<string, number>();
            options.forEach(opt => {
                newMap.set(opt.optionId, opt.baseQuantity);
            });
            setSelectedOptions(newMap);

            console.log('[useCustomizationState] ✅ Customization initialized');
        },
        []
    );

    // ============================================================================
    // Update Option Quantity
    // ============================================================================
    const updateOptionQuantity = useCallback((optionId: string, quantity: number) => {
        console.log('[useCustomizationState] 📝 Updating option:', {
            optionId,
            quantity
        });

        setSelectedOptions(prev => {
            const newMap = new Map(prev);
            newMap.set(optionId, quantity);
            return newMap;
        });

        // Update customization state
        setCustomizationState(prev => {
            if (!prev) return null;

            const updatedOptions = prev.options.map(opt =>
                opt.optionId === optionId
                    ? { ...opt, selectedQuantity: quantity }
                    : opt
            );

            return {
                ...prev,
                options: updatedOptions,
                totalCustomizationPrice: calculateTotalPrice(updatedOptions)
            };
        });
    }, []);

    // ============================================================================
    // Calculate Total Price
    // ============================================================================
    const calculateTotalPrice = (options: CustomizationOption[]): number => {
        return options.reduce((sum, opt) => {
            const additionalQuantity = opt.selectedQuantity - opt.baseQuantity;
            const additionalPrice = Math.max(0, additionalQuantity) * opt.unitPrice;
            return sum + additionalPrice;
        }, 0);
    };

    // ============================================================================
    // Get Selected Options
    // ============================================================================
    const getSelectedOptions = useCallback((): CustomizationOption[] => {
        if (!customizationState) return [];
        return customizationState.options.filter(opt => opt.selectedQuantity > 0);
    }, [customizationState]);

    // ============================================================================
    // Get Total Price
    // ============================================================================
    const getTotalPrice = useCallback((): number => {
        if (!customizationState) return 0;
        return customizationState.totalCustomizationPrice;
    }, [customizationState]);

    // ============================================================================
    // Save State to LocalStorage
    // ============================================================================
    const saveState = useCallback(() => {
        if (!customizationState) {
            console.warn('[useCustomizationState] ⚠️ No customization state to save');
            return;
        }

        try {
            const states = JSON.parse(localStorage.getItem(STORAGE_KEY) || '{}');
            states[customizationState.productId] = {
                ...customizationState,
                savedAt: new Date().toISOString()
            };

            localStorage.setItem(STORAGE_KEY, JSON.stringify(states));

            console.log('[useCustomizationState] 💾 State saved:', {
                productId: customizationState.productId,
                options: customizationState.options.length,
                totalPrice: customizationState.totalCustomizationPrice
            });
        } catch (err) {
            console.error('[useCustomizationState] ❌ Failed to save state:', err);
        }
    }, [customizationState]);

    // ============================================================================
    // Load State from LocalStorage
    // ============================================================================
    const loadState = useCallback((productId: string): CustomizationState | null => {
        try {
            const states = JSON.parse(localStorage.getItem(STORAGE_KEY) || '{}');
            const state = states[productId];

            if (state) {
                console.log('[useCustomizationState] 📂 State loaded:', {
                    productId,
                    options: state.options.length,
                    savedAt: state.savedAt
                });
                return state;
            }

            return null;
        } catch (err) {
            console.error('[useCustomizationState] ❌ Failed to load state:', err);
            return null;
        }
    }, []);

    // ============================================================================
    // Clear State
    // ============================================================================
    const clearState = useCallback((productId: string) => {
        try {
            const states = JSON.parse(localStorage.getItem(STORAGE_KEY) || '{}');
            delete states[productId];
            localStorage.setItem(STORAGE_KEY, JSON.stringify(states));

            console.log('[useCustomizationState] 🗑️ State cleared:', productId);
        } catch (err) {
            console.error('[useCustomizationState] ❌ Failed to clear state:', err);
        }
    }, []);

    // ============================================================================
    // Reset Options
    // ============================================================================
    const resetOptions = useCallback(() => {
        if (!customizationState) return;

        console.log('[useCustomizationState] 🔄 Resetting options');

        const resetOptions = customizationState.options.map(opt => ({
            ...opt,
            selectedQuantity: opt.baseQuantity
        }));

        setCustomizationState(prev => {
            if (!prev) return null;
            return {
                ...prev,
                options: resetOptions,
                totalCustomizationPrice: calculateTotalPrice(resetOptions)
            };
        });

        const newMap = new Map<string, number>();
        resetOptions.forEach(opt => {
            newMap.set(opt.optionId, opt.baseQuantity);
        });
        setSelectedOptions(newMap);
    }, [customizationState]);

    return {
        customizationState,
        selectedOptions,
        totalPrice: customizationState?.totalCustomizationPrice || 0,
        initializeCustomization,
        updateOptionQuantity,
        getSelectedOptions,
        getTotalPrice,
        saveState,
        loadState,
        clearState,
        resetOptions
    };
}

export default useCustomizationState;

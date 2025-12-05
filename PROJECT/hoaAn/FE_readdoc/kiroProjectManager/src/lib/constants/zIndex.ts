/**
 * Z-Index Constants
 * 
 * Centralized z-index values to prevent layering conflicts
 * Higher values appear on top
 */

export const Z_INDEX = {
    // Base layers
    BASE: 0,

    // Layout
    SIDEBAR: 40,
    SIDEBAR_BACKDROP: 30,
    TOPBAR: 20,

    // Dropdowns and popovers
    DROPDOWN: 50,
    DROPDOWN_BACKDROP: 45,

    // Modals
    MODAL: 100,
    MODAL_BACKDROP: 90,

    // Toast notifications
    TOAST: 200,
} as const;

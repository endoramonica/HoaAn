/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * DTO representing a customizable option within a package product.
 * Customizable options allow customers to modify quantities of specific items in a package.
 */
export type CustomizableOptionDto = {
    /**
     * Unique identifier for the customizable option (e.g., "opt-xoi", "opt-che").
     */
    id: string;
    /**
     * Display name of the customizable option (e.g., "Xôi gấc đậu xanh", "Chè trôi nước").
     */
    name: string;
    /**
     * Default quantity included in the package for this option.
     */
    baseQuantity: number;
    /**
     * Price per unit of this option (e.g., 45,000đ per dĩa of xôi).
     */
    unitPrice: number;
    /**
     * Minimum quantity a customer can order for this option.
     */
    minQuantity: number;
    /**
     * Maximum quantity a customer can order for this option. Null means unlimited.
     */
    maxQuantity?: number | null;
    /**
     * Unit of measurement for this option (e.g., "dĩa", "chén", "bộ").
     */
    unit: string;
};


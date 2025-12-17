/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * Represents a customization option selected for a cart item.
 * Used when customers customize package products by adjusting quantities of optional items.
 */
export type CartItemCustomizationDto = {
    /**
     * Gets or sets the unique identifier of the customizable option.
     * Example: "opt-xoi" for "Xôi gấc đậu xanh"
     */
    optionId?: string | null;
    /**
     * Gets or sets the quantity of this option selected by the customer.
     * Must be within the min/max bounds defined in the product's customizable options.
     */
    quantity?: number;
    /**
     * Gets or sets the unit price of this option at the time of cart addition.
     * Example: 45,000đ per dĩa of xôi
     */
    unitPrice?: number;
    /**
     * Gets or sets the total price for this customization.
     * Calculated as: Quantity × UnitPrice
     * Example: 10 dĩa × 45,000đ = 450,000đ
     */
    totalPrice?: number;
};


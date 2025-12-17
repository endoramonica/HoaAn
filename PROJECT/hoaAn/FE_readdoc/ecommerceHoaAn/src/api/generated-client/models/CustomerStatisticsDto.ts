/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
/**
 * Customer statistics summary
 */
export type CustomerStatisticsDto = {
    customerId?: string;
    totalOrders?: number;
    totalSpent?: number;
    averageOrderValue?: number;
    totalInteractions?: number;
    lastOrderDate?: string | null;
    lastInteractionDate?: string | null;
    loyaltyPoints?: number;
    tier?: string | null;
};


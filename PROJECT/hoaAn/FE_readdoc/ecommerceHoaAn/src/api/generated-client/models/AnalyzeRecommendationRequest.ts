/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ActionDto } from './ActionDto';
import type { CartItemDto } from './CartItemDto';
/**
 * Request model for analyzing action sequences
 */
export type AnalyzeRecommendationRequest = {
    /**
     * Sequence of user actions to analyze
     */
    actionSequence?: Array<ActionDto> | null;
    /**
     * Items currently in the user's cart
     */
    cartItems?: Array<CartItemDto> | null;
};


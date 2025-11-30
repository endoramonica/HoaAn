/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { OrderShippingInputDto } from './OrderShippingInputDto';
import type { PaymentMethodType } from './PaymentMethodType';
export type CheckoutDto = {
    cartId: string;
    shippingInfo: OrderShippingInputDto;
    couponCode?: string | null;
    notes?: string | null;
    paymentMethod?: PaymentMethodType;
};


/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class PaymentWebhookService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * Handles VNPay IPN (Instant Payment Notification) callback.
     * Validates secure hash signature and processes payment callback.
     * Requirements: 1.3, 1.4, 1.5, 5.1, 5.5
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1PaymentVnpayIpn(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/payment/vnpay/ipn',
        });
    }
    /**
     * Retrieves the current payment status for an order.
     * Extracts orderId from route parameter and returns PaymentStatusDto with current status.
     * Handles non-existent orders gracefully.
     * Requirements: 4.4, 4.5
     * @param orderId
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1PaymentStatus(
        orderId: string,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/payment/status/{orderId}',
            path: {
                'orderId': orderId,
            },
        });
    }
}

/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class PaymentWebhookService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1PaymentVnpayIpn(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/payment/vnpay/ipn',
        });
    }
}

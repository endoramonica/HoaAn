/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { StringDateTime_f__AnonymousType11 } from '../models/StringDateTime_f__AnonymousType11';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class OtherService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @returns StringDateTime_f__AnonymousType11 OK
     * @throws ApiError
     */
    public getHealth(): CancelablePromise<StringDateTime_f__AnonymousType11> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/health',
        });
    }
}

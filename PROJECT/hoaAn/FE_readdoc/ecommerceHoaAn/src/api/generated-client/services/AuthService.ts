/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ChangePasswordRequestDTO } from '../models/ChangePasswordRequestDTO';
import type { ForgotPasswordRequestDTO } from '../models/ForgotPasswordRequestDTO';
import type { LoginDTO } from '../models/LoginDTO';
import type { RefreshTokenRequestDTO } from '../models/RefreshTokenRequestDTO';
import type { RegisterRequestDTO } from '../models/RegisterRequestDTO';
import type { ResetPasswordRequestDTO } from '../models/ResetPasswordRequestDTO';
import type { SocialLoginRequestDTO } from '../models/SocialLoginRequestDTO';
import type { CancelablePromise } from '../core/CancelablePromise';
import type { BaseHttpRequest } from '../core/BaseHttpRequest';
export class AuthService {
    constructor(public readonly httpRequest: BaseHttpRequest) {}
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthRegister(
        requestBody?: RegisterRequestDTO,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Auth/register',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                409: `Conflict`,
            },
        });
    }
    /**
     * @param token
     * @returns any OK
     * @throws ApiError
     */
    public getApiV1AuthVerifyEmail(
        token?: string,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'GET',
            url: '/api/v1/Auth/verify-email',
            query: {
                'token': token,
            },
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthLogin(
        requestBody?: LoginDTO,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Auth/login',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
            },
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthLoginGoogle(
        requestBody?: SocialLoginRequestDTO,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Auth/login/google',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthRefreshToken(
        requestBody?: RefreshTokenRequestDTO,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Auth/refresh-token',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthLogout(): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Auth/logout',
            errors: {
                401: `Unauthorized`,
            },
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthChangePassword(
        requestBody?: ChangePasswordRequestDTO,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Auth/change-password',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
            },
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthForgotPassword(
        requestBody?: ForgotPasswordRequestDTO,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Auth/forgot-password',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * @param requestBody
     * @returns any OK
     * @throws ApiError
     */
    public postApiV1AuthResetPassword(
        requestBody?: ResetPasswordRequestDTO,
    ): CancelablePromise<any> {
        return this.httpRequest.request({
            method: 'POST',
            url: '/api/v1/Auth/reset-password',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
}

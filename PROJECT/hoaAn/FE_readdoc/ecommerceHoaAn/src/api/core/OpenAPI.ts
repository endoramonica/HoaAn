/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ApiRequestOptions } from './ApiRequestOptions';

type Resolver<T> = (options: ApiRequestOptions) => Promise<T>;
type Headers = Record<string, string>;

export type OpenAPIConfig = {
    BASE: string;
    VERSION: string;
    WITH_CREDENTIALS: boolean;
    CREDENTIALS: 'include' | 'omit' | 'same-origin';
    TOKEN?: string | Resolver<string> | undefined;
    USERNAME?: string | Resolver<string> | undefined;
    PASSWORD?: string | Resolver<string> | undefined;
    HEADERS?: Headers | Resolver<Headers> | undefined;
    ENCODE_PATH?: ((path: string) => string) | undefined;
};

export const OpenAPI: OpenAPIConfig = {
    BASE: 'https://hbh1z72d-7131.asse.devtunnels.ms',
    VERSION: '1',
    WITH_CREDENTIALS: true,  // ✅ FIXED: Enable credentials
    CREDENTIALS: 'include',
    
    // ✅ FIXED: Dynamic token resolver - Đọc từ sessionStorage hoặc localStorage
    TOKEN: async () => {
        // Check sessionStorage first (current session)
        const sessionToken = sessionStorage.getItem('authToken');
        if (sessionToken) {
            console.log('[OpenAPI] ✅ Using token from sessionStorage');
            return sessionToken;
        }
        
        // Fallback to localStorage (remember me)
        const localToken = localStorage.getItem('authToken');
        if (localToken) {
            console.log('[OpenAPI] ✅ Using token from localStorage');
            return localToken;
        }
        
        console.log('[OpenAPI] ⚠️ No token found');
        return '';
    },
    
    USERNAME: undefined,
    PASSWORD: undefined,
    HEADERS: undefined,
    ENCODE_PATH: undefined,
};
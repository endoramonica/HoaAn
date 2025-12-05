import axios from 'axios';
import type { AxiosRequestConfig, AxiosResponse } from 'axios';
import { tokenStorage } from './client';

/**
 * Custom Axios instance for Orval-generated API functions
 * 
 * This is a separate instance from the main apiClient to avoid
 * circular dependencies and provide a clean interface for Orval.
 * 
 * Features:
 * - Automatic JWT token attachment
 * - Base URL configuration from environment
 * - Timeout configuration
 * - Standard headers
 * 
 * Note: This instance does NOT include token refresh logic.
 * Token refresh is handled by the main apiClient instance.
 */

/**
 * Create Axios instance for Orval
 * Configured with base URL, timeout, and headers
 */
const orvalAxiosInstance = axios.create({
    baseURL: import.meta.env.VITE_API_URL || 'http://localhost:3000/api',
    timeout: 30000, // 30 seconds
    headers: {
        'Content-Type': 'application/json',
    },
});

/**
 * Request interceptor to attach JWT tokens
 * Automatically adds Authorization header if access token is available
 */
orvalAxiosInstance.interceptors.request.use(
    (config) => {
        // Attach JWT token if available
        const accessToken = tokenStorage.getAccessToken();
        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`;
        }

        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

/**
 * Custom mutator function for Orval
 * This function is used by all Orval-generated API functions
 * 
 * @param config - Axios request configuration
 * @returns Promise with the response data (unwrapped from Axios response)
 * 
 * Note: We return the full AxiosResponse to maintain compatibility with Orval's
 * generated code which expects status, headers, etc.
 */
export function apiClient<T = any>(config: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return orvalAxiosInstance.request<T>(config);
}

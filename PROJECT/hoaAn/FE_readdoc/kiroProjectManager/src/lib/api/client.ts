import axios from 'axios';
import type { AxiosInstance, AxiosRequestConfig, AxiosResponse, InternalAxiosRequestConfig } from 'axios';
import { STORAGE_KEYS } from './types';
import type { TokenStorage } from './types';

/**
 * Queue for failed requests during token refresh
 */
interface FailedRequestQueue {
    resolve: (value?: any) => void;
    reject: (reason?: any) => void;
}

/**
 * Flag to track if token refresh is in progress
 */
let isRefreshing = false;

/**
 * Queue of failed requests waiting for token refresh
 */
let failedQueue: FailedRequestQueue[] = [];

/**
 * Token storage implementation
 * Manages JWT tokens in localStorage (remember me) or sessionStorage
 */
export const tokenStorage: TokenStorage = {
    /**
     * Check if "remember me" is enabled
     * @returns true if remember me is enabled
     */
    isRememberMe(): boolean {
        // Check localStorage first (persistent)
        const rememberMe = localStorage.getItem(STORAGE_KEYS.REMEMBER_ME);
        return rememberMe === 'true';
    },

    /**
     * Set "remember me" preference
     * @param remember - true to use localStorage, false to use sessionStorage
     */
    setRememberMe(remember: boolean): void {
        if (remember) {
            localStorage.setItem(STORAGE_KEYS.REMEMBER_ME, 'true');
        } else {
            localStorage.removeItem(STORAGE_KEYS.REMEMBER_ME);
        }
    },

    /**
     * Get the appropriate storage based on "remember me" preference
     * @returns localStorage if remember me is enabled, otherwise sessionStorage
     */
    getStorage(): Storage {
        return this.isRememberMe() ? localStorage : sessionStorage;
    },

    /**
     * Get access token from storage
     * Checks both localStorage and sessionStorage for migration support
     * @returns access token or null if not found
     */
    getAccessToken(): string | null {
        // Check preferred storage first
        const storage = this.getStorage();
        const token = storage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
        if (token) {
            return token;
        }

        // Check the other storage for migration
        const otherStorage = storage === localStorage ? sessionStorage : localStorage;
        return otherStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
    },

    /**
     * Set access token in storage
     * @param token - access token to store
     */
    setAccessToken(token: string): void {
        const storage = this.getStorage();
        storage.setItem(STORAGE_KEYS.ACCESS_TOKEN, token);
    },

    /**
     * Get refresh token from storage
     * Checks both localStorage and sessionStorage for migration support
     * @returns refresh token or null if not found
     */
    getRefreshToken(): string | null {
        // Check preferred storage first
        const storage = this.getStorage();
        const token = storage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
        if (token) {
            return token;
        }

        // Check the other storage for migration
        const otherStorage = storage === localStorage ? sessionStorage : localStorage;
        return otherStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
    },

    /**
     * Set refresh token in storage
     * @param token - refresh token to store
     */
    setRefreshToken(token: string): void {
        const storage = this.getStorage();
        storage.setItem(STORAGE_KEYS.REFRESH_TOKEN, token);
    },

    /**
     * Set both access and refresh tokens
     * Optionally update remember me preference
     * @param accessToken - access token to store
     * @param refreshToken - refresh token to store
     * @param rememberMe - optional remember me preference
     */
    setTokens(accessToken: string, refreshToken: string, rememberMe?: boolean): void {
        // Update remember me preference if provided
        if (rememberMe !== undefined) {
            this.setRememberMe(rememberMe);
        }

        // Store tokens in appropriate storage
        this.setAccessToken(accessToken);
        this.setRefreshToken(refreshToken);
    },

    /**
     * Clear all tokens from both localStorage and sessionStorage
     * Also clears remember me preference
     */
    clearTokens(): void {
        // Clear from both storages for security
        localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
        localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
        localStorage.removeItem(STORAGE_KEYS.REMEMBER_ME);

        sessionStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
        sessionStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
    },
};

/**
 * Process queued requests after token refresh
 * @param error - Error if token refresh failed, null if successful
 * @param token - New access token if refresh was successful
 */
function processQueue(error: any, token: string | null = null): void {
    failedQueue.forEach((promise) => {
        if (error) {
            promise.reject(error);
        } else {
            promise.resolve(token);
        }
    });

    failedQueue = [];
}

/**
 * Refresh the access token using the refresh token
 * @returns Promise with the new access token
 */
async function refreshAccessToken(): Promise<string> {
    const refreshToken = tokenStorage.getRefreshToken();

    if (!refreshToken) {
        throw new Error('No refresh token available');
    }

    // Get API base URL from environment variables
    const baseURL = import.meta.env.VITE_API_URL || 'http://localhost:3000/api';

    // Make refresh token request without interceptors to avoid infinite loop
    const response = await axios.post(`${baseURL}/auth/refresh`, {
        refreshToken,
    });

    const { accessToken, refreshToken: newRefreshToken } = response.data;

    // Update stored tokens
    tokenStorage.setTokens(accessToken, newRefreshToken);

    return accessToken;
}

/**
 * Create Axios instance with base configuration and interceptors
 * @returns Configured Axios instance
 */
export function createAxiosInstance(): AxiosInstance {
    // Get API base URL from environment variables
    const baseURL = import.meta.env.VITE_API_URL || 'http://localhost:3000/api';
    const isDevelopment = import.meta.env.VITE_ENV === 'development' || import.meta.env.DEV;

    // Create Axios instance with base configuration
    const instance = axios.create({
        baseURL,
        timeout: 30000, // 30 seconds
        headers: {
            'Content-Type': 'application/json',
        },
    });

    // Request interceptor: attach JWT tokens and log in development
    instance.interceptors.request.use(
        (config) => {
            // Attach JWT token if available
            const accessToken = tokenStorage.getAccessToken();
            if (accessToken) {
                config.headers.Authorization = `Bearer ${accessToken}`;
            }

            // Development logging
            if (isDevelopment) {
                console.log(`[API Request] ${config.method?.toUpperCase()} ${config.url}`, {
                    params: config.params,
                    data: config.data,
                });
            }

            return config;
        },
        (error) => {
            // Development logging for request errors
            if (isDevelopment) {
                console.error('[API Request Error]', error);
            }
            return Promise.reject(error);
        }
    );

    // Response interceptor: handle token refresh and log in development
    instance.interceptors.response.use(
        (response) => {
            // Development logging for successful responses
            if (isDevelopment) {
                console.log(`[API Response] ${response.config.method?.toUpperCase()} ${response.config.url}`, {
                    status: response.status,
                    data: response.data,
                });
            }

            return response;
        },
        async (error) => {
            const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

            // Development logging for errors
            if (isDevelopment) {
                console.error('[API Error]', {
                    url: error.config?.url,
                    method: error.config?.method,
                    status: error.response?.status,
                    message: error.message,
                    data: error.response?.data,
                });
            }

            // Handle 401 Unauthorized - attempt token refresh
            if (error.response?.status === 401 && !originalRequest._retry) {
                if (isRefreshing) {
                    // If refresh is already in progress, queue this request
                    return new Promise((resolve, reject) => {
                        failedQueue.push({ resolve, reject });
                    })
                        .then((token) => {
                            // Update the authorization header with new token
                            originalRequest.headers.Authorization = `Bearer ${token}`;
                            return instance(originalRequest);
                        })
                        .catch((err) => {
                            return Promise.reject(err);
                        });
                }

                // Mark this request as retried to prevent infinite loops
                originalRequest._retry = true;
                isRefreshing = true;

                try {
                    // Attempt to refresh the token
                    const newAccessToken = await refreshAccessToken();

                    // Process queued requests with the new token
                    processQueue(null, newAccessToken);

                    // Update the authorization header with new token
                    originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

                    // Retry the original request
                    return instance(originalRequest);
                } catch (refreshError) {
                    // Token refresh failed - clear tokens and redirect to login
                    processQueue(refreshError, null);
                    tokenStorage.clearTokens();

                    // Redirect to login page with current location
                    if (typeof window !== 'undefined') {
                        const currentPath = window.location.hash.replace('#', '');
                        window.location.href = `#/login?redirect=${encodeURIComponent(currentPath)}`;
                    }

                    return Promise.reject(refreshError);
                } finally {
                    isRefreshing = false;
                }
            }

            return Promise.reject(error);
        }
    );

    return instance;
}

/**
 * Main Axios instance for API requests
 * Configured with base URL, timeout, and request interceptors
 */
export const apiClient = createAxiosInstance();

/**
 * Wrapper function for making API requests with the configured client
 * @param config - Axios request configuration
 * @returns Promise with the response
 */
export async function apiRequest<T = any>(config: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return apiClient.request<T>(config);
}

/**
 * Convenience wrapper for GET requests
 * @param url - Request URL
 * @param config - Optional Axios request configuration
 * @returns Promise with the response
 */
export async function apiGet<T = any>(url: string, config?: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return apiClient.get<T>(url, config);
}

/**
 * Convenience wrapper for POST requests
 * @param url - Request URL
 * @param data - Request body data
 * @param config - Optional Axios request configuration
 * @returns Promise with the response
 */
export async function apiPost<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return apiClient.post<T>(url, data, config);
}

/**
 * Convenience wrapper for PUT requests
 * @param url - Request URL
 * @param data - Request body data
 * @param config - Optional Axios request configuration
 * @returns Promise with the response
 */
export async function apiPut<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return apiClient.put<T>(url, data, config);
}

/**
 * Convenience wrapper for PATCH requests
 * @param url - Request URL
 * @param data - Request body data
 * @param config - Optional Axios request configuration
 * @returns Promise with the response
 */
export async function apiPatch<T = any>(url: string, data?: any, config?: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return apiClient.patch<T>(url, data, config);
}

/**
 * Convenience wrapper for DELETE requests
 * @param url - Request URL
 * @param config - Optional Axios request configuration
 * @returns Promise with the response
 */
export async function apiDelete<T = any>(url: string, config?: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return apiClient.delete<T>(url, config);
}

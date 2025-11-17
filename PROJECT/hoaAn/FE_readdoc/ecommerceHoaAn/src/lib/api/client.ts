/**
 * API Client Layer - Axios instance với interceptors, JWT, logging
 * Tương thích với ASP.NET Core (.NET 8) backend
 */

import axios, { type AxiosInstance, type AxiosRequestConfig, type AxiosResponse, type AxiosError } from 'axios';
import { createApiError, getErrorMessage, ErrorMessages } from './errors';
import { type RefreshTokenRequest, type RefreshTokenResponse } from './types';

// Lấy API URL từ environment variable
const getApiUrl = () => {
  if (typeof import.meta !== 'undefined' && import.meta.env) {
    return import.meta.env.VITE_API_URL || 'https://hbh1z72d-7131.asse.devtunnels.ms/api/v1';
  }
  // Fallback cho môi trường không hỗ trợ import.meta
  return 'https://hbh1z72d-7131.asse.devtunnels.ms/api/v1';
};

const API_BASE_URL = getApiUrl();

// Token storage keys
const ACCESS_TOKEN_KEY = 'access_token';
const REFRESH_TOKEN_KEY = 'refresh_token';
const REMEMBER_ME_KEY = 'remember_me';

/**
 * Token storage utilities
 * Hỗ trợ "Remember me" - dùng localStorage hoặc sessionStorage
 */
export const tokenStorage = {
  /**
   * Kiểm tra xem user có chọn "Remember me" không
   */
  isRememberMe: (): boolean => {
    return localStorage.getItem(REMEMBER_ME_KEY) === 'true';
  },

  /**
   * Set Remember me preference
   */
  setRememberMe: (remember: boolean): void => {
    if (remember) {
      localStorage.setItem(REMEMBER_ME_KEY, 'true');
    } else {
      localStorage.removeItem(REMEMBER_ME_KEY);
    }
  },

  /**
   * Get storage - localStorage nếu remember me, sessionStorage nếu không
   */
  getStorage: (): Storage => {
    return tokenStorage.isRememberMe() ? localStorage : sessionStorage;
  },

  getAccessToken: (): string | null => {
    // Check localStorage first (remember me)
    const rememberToken = localStorage.getItem(ACCESS_TOKEN_KEY);
    if (rememberToken) return rememberToken;
    
    // Then check sessionStorage (current session)
    return sessionStorage.getItem(ACCESS_TOKEN_KEY);
  },

  setAccessToken: (token: string): void => {
    const storage = tokenStorage.getStorage();
    storage.setItem(ACCESS_TOKEN_KEY, token);
  },

  getRefreshToken: (): string | null => {
    // Check localStorage first (remember me)
    const rememberToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    if (rememberToken) return rememberToken;
    
    // Then check sessionStorage (current session)
    return sessionStorage.getItem(REFRESH_TOKEN_KEY);
  },

  setRefreshToken: (token: string): void => {
    const storage = tokenStorage.getStorage();
    storage.setItem(REFRESH_TOKEN_KEY, token);
  },

  clearTokens: (): void => {
    // Clear from both storages
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(REMEMBER_ME_KEY);
    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
    sessionStorage.removeItem(REFRESH_TOKEN_KEY);
  },

  setTokens: (accessToken: string, refreshToken: string, rememberMe?: boolean): void => {
    // Set remember me preference if provided
    if (rememberMe !== undefined) {
      tokenStorage.setRememberMe(rememberMe);
    }
    
    tokenStorage.setAccessToken(accessToken);
    tokenStorage.setRefreshToken(refreshToken);
  }
};

/**
 * Tạo Axios instance
 */
const createAxiosInstance = (): AxiosInstance => {
  const instance = axios.create({
    baseURL: API_BASE_URL,
    timeout: 30000, // 30 seconds
    headers: {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    },
  });

  return instance;
};

/**
 * Main API client instance
 */
export const apiClient = createAxiosInstance();

/**
 * Flag để tránh refresh token loop
 */
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (value?: any) => void;
  reject: (reason?: any) => void;
}> = [];

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach(promise => {
    if (error) {
      promise.reject(error);
    } else {
      promise.resolve(token);
    }
  });

  failedQueue = [];
};

/**
 * Request Interceptor - Tự động attach JWT token
 */
apiClient.interceptors.request.use(
  (config) => {
    const token = tokenStorage.getAccessToken();
    
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    // Logging request (dev only)
    if (typeof import.meta !== 'undefined' && import.meta.env && import.meta.env.DEV) {
      console.log(`[API Request] ${config.method?.toUpperCase()} ${config.url}`, config.data);
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

/**
 * Response Interceptor - Handle errors, auto refresh token
 */
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    // Logging response (dev only)
    if (typeof import.meta !== 'undefined' && import.meta.env && import.meta.env.DEV) {
      console.log(`[API Response] ${response.config.method?.toUpperCase()} ${response.config.url}`, response.data);
    }

    return response;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as AxiosRequestConfig & { _retry?: boolean };

    // Logging error (dev only)
    if (typeof import.meta !== 'undefined' && import.meta.env && import.meta.env.DEV) {
      console.error(`[API Error] ${originalRequest?.method?.toUpperCase()} ${originalRequest?.url}`, error.response?.data);
    }

    // Network error
    if (!error.response) {
      return Promise.reject(createApiError(0, { message: ErrorMessages.NETWORK_ERROR }));
    }

    const status = error.response.status;

    // Auto refresh token khi 401 Unauthorized
    if (status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        // Đang refresh token, đợi kết quả
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then(token => {
            if (originalRequest.headers) {
              originalRequest.headers.Authorization = `Bearer ${token}`;
            }
            return apiClient(originalRequest);
          })
          .catch(err => {
            return Promise.reject(err);
          });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const refreshToken = tokenStorage.getRefreshToken();

      if (!refreshToken) {
        // Không có refresh token, logout
        tokenStorage.clearTokens();
        window.location.href = '/login';
        return Promise.reject(createApiError(401, { message: ErrorMessages.UNAUTHORIZED }));
      }

      try {
        // Gọi API refresh token
        const response = await axios.post<RefreshTokenResponse>(
          `${API_BASE_URL}/auth/refresh-token`,
          { refreshToken } as RefreshTokenRequest,
          {
            headers: {
              'Content-Type': 'application/json',
            },
          }
        );

        const { accessToken, refreshToken: newRefreshToken } = response.data;

        // Lưu token mới
        tokenStorage.setTokens(accessToken, newRefreshToken);

        // Process queued requests
        processQueue(null, accessToken);

        // Retry original request
        if (originalRequest.headers) {
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        }
        return apiClient(originalRequest);
      } catch (refreshError) {
        // Refresh token failed, logout
        processQueue(refreshError, null);
        tokenStorage.clearTokens();
        window.location.href = '/login';
        return Promise.reject(createApiError(401, { message: ErrorMessages.UNAUTHORIZED }));
      } finally {
        isRefreshing = false;
      }
    }

    // Parse error theo format ASP.NET Core ProblemDetails
    const apiError = createApiError(status, error.response.data);
    return Promise.reject(apiError);
  }
);

/**
 * File upload client với progress tracking
 */
export const createFileUploadClient = (
  onUploadProgress?: (progressEvent: any) => void
): AxiosInstance => {
  const instance = createAxiosInstance();

  // Copy interceptors từ main client
  instance.interceptors.request = apiClient.interceptors.request;
  instance.interceptors.response = apiClient.interceptors.response;

  // Override config cho file upload
  instance.defaults.headers['Content-Type'] = 'multipart/form-data';
  instance.defaults.timeout = 300000; // 5 minutes cho upload

  if (onUploadProgress) {
    instance.defaults.onUploadProgress = onUploadProgress;
  }

  return instance;
};

/**
 * Generic API request wrapper
 */
export const apiRequest = {
  get: function<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return apiClient.get<T>(url, config).then(response => response.data);
  },

  post: function<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return apiClient.post<T>(url, data, config).then(response => response.data);
  },

  put: function<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return apiClient.put<T>(url, data, config).then(response => response.data);
  },

  patch: function<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return apiClient.patch<T>(url, data, config).then(response => response.data);
  },

  delete: function<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return apiClient.delete<T>(url, config).then(response => response.data);
  },
};

/**
 * Build query string từ object
 */
export const buildQueryString = (params: Record<string, any>): string => {
  const searchParams = new URLSearchParams();

  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      if (Array.isArray(value)) {
        value.forEach(v => searchParams.append(key, String(v)));
      } else {
        searchParams.append(key, String(value));
      }
    }
  });

  const queryString = searchParams.toString();
  return queryString ? `?${queryString}` : '';
};

export default apiClient;
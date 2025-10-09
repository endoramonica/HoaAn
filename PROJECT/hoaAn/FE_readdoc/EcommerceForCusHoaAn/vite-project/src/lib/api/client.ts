/**
 * API Client Layer - Axios instance với interceptors, JWT, logging
 * Tương thích với ASP.NET Core (.NET 8) backend
 */

import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse, AxiosError } from 'axios';
import { createApiError, getErrorMessage, ErrorMessages } from './errors';
import { RefreshTokenRequest, RefreshTokenResponse } from './types';

// Lấy API URL từ environment variable
const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api/v1';

// Token storage keys
const ACCESS_TOKEN_KEY = 'access_token';
const REFRESH_TOKEN_KEY = 'refresh_token';

/**
 * Token storage utilities
 */
export const tokenStorage = {
  getAccessToken: (): string | null => {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  },

  setAccessToken: (token: string): void => {
    localStorage.setItem(ACCESS_TOKEN_KEY, token);
  },

  getRefreshToken: (): string | null => {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  },

  setRefreshToken: (token: string): void => {
    localStorage.setItem(REFRESH_TOKEN_KEY, token);
  },

  clearTokens: (): void => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
  },

  setTokens: (accessToken: string, refreshToken: string): void => {
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
    if (process.env.NODE_ENV === 'development') {
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
    if (process.env.NODE_ENV === 'development') {
      console.log(`[API Response] ${response.config.method?.toUpperCase()} ${response.config.url}`, response.data);
    }

    return response;
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as AxiosRequestConfig & { _retry?: boolean };

    // Logging error (dev only)
    if (process.env.NODE_ENV === 'development') {
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
  get: <T>(url: string, config?: AxiosRequestConfig): Promise<T> => {
    return apiClient.get<T>(url, config).then(response => response.data);
  },

  post: <T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> => {
    return apiClient.post<T>(url, data, config).then(response => response.data);
  },

  put: <T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> => {
    return apiClient.put<T>(url, data, config).then(response => response.data);
  },

  patch: <T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> => {
    return apiClient.patch<T>(url, data, config).then(response => response.data);
  },

  delete: <T>(url: string, config?: AxiosRequestConfig): Promise<T> => {
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

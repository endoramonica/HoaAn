/**
 * API Client Layer - Axios instance với interceptors, JWT, logging
 * ✅ FIXED: Token keys phải khớp với authService
 */

import axios, { type AxiosInstance, type AxiosRequestConfig, type AxiosResponse, type AxiosError } from 'axios';
import { createApiError, getErrorMessage, ErrorMessages } from './errors';
import { type RefreshTokenRequest, type RefreshTokenResponse } from './types';

// Lấy API URL từ environment variable
const getApiUrl = () => {
  return import.meta.env.VITE_API_URL ?? "https://localhost:7131/api/v1";
};



const API_BASE_URL = getApiUrl();

// ✅ FIXED: Token storage keys - PHẢI KHỚP với authService
const ACCESS_TOKEN_KEY = 'authToken';      // ← FIXED từ 'accessToken'
const REFRESH_TOKEN_KEY = 'refreshToken';  // ← Giữ nguyên (đã đúng)
const REMEMBER_ME_KEY = 'remember_me';

/**
 * Token storage utilities
 * ✅ FIXED: Thêm debug logs để verify
 */
export const tokenStorage = {
  isRememberMe: (): boolean => {
    return localStorage.getItem(REMEMBER_ME_KEY) === 'true';
  },

  setRememberMe: (remember: boolean): void => {
    if (remember) {
      localStorage.setItem(REMEMBER_ME_KEY, 'true');
    } else {
      localStorage.removeItem(REMEMBER_ME_KEY);
    }
  },

  getStorage: (): Storage => {
    return tokenStorage.isRememberMe() ? localStorage : sessionStorage;
  },

  getAccessToken: (): string | null => {
    // Check localStorage first (remember me)
    const rememberToken = localStorage.getItem(ACCESS_TOKEN_KEY);
    if (rememberToken) {
      console.log('[TokenStorage] ✅ Found access token in localStorage');
      return rememberToken;
    }

    // Then check sessionStorage (current session)
    const sessionToken = sessionStorage.getItem(ACCESS_TOKEN_KEY);
    if (sessionToken) {
      console.log('[TokenStorage] ✅ Found access token in sessionStorage');
      return sessionToken;
    }

    console.warn('[TokenStorage] ⚠️ No access token found');
    return null;
  },

  setAccessToken: (token: string): void => {
    const storage = tokenStorage.getStorage();
    storage.setItem(ACCESS_TOKEN_KEY, token);

    console.log('[TokenStorage] ✅ Access token saved to',
      storage === localStorage ? 'localStorage' : 'sessionStorage',
      '- Key:', ACCESS_TOKEN_KEY
    );
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

    console.log('[TokenStorage] ✅ Refresh token saved to',
      storage === localStorage ? 'localStorage' : 'sessionStorage'
    );
  },

  clearTokens: (): void => {
    // Clear from both storages
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(REMEMBER_ME_KEY);
    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
    sessionStorage.removeItem(REFRESH_TOKEN_KEY);

    console.log('[TokenStorage] 🗑️ All tokens cleared');
  },

  setTokens: (accessToken: string, refreshToken: string, rememberMe?: boolean): void => {
    console.log('[TokenStorage] 🔄 setTokens called:', {
      accessTokenLength: accessToken.length,
      refreshTokenLength: refreshToken.length,
      rememberMe,
      willUseStorage: rememberMe ? 'localStorage' : 'sessionStorage'
    });

    // Set remember me preference if provided
    if (rememberMe !== undefined) {
      tokenStorage.setRememberMe(rememberMe);
    }

    tokenStorage.setAccessToken(accessToken);
    tokenStorage.setRefreshToken(refreshToken);

    // ✅ VERIFY NGAY
    const storage = tokenStorage.getStorage();
    const savedAccess = storage.getItem(ACCESS_TOKEN_KEY);
    const savedRefresh = storage.getItem(REFRESH_TOKEN_KEY);

    console.log('[TokenStorage] ✅ Verification:', {
      accessTokenSaved: !!savedAccess,
      refreshTokenSaved: !!savedRefresh,
      accessTokenPreview: savedAccess?.substring(0, 30) + '...',
      refreshTokenPreview: savedRefresh?.substring(0, 30) + '...',
      storageType: storage === localStorage ? 'localStorage' : 'sessionStorage'
    });
  }
};

/**
 * Tạo Axios instance
 */
const createAxiosInstance = (): AxiosInstance => {
  const instance = axios.create({
    baseURL: API_BASE_URL,
    timeout: 30000,
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
      console.log('[API Request] ✅ Token attached to', config.method?.toUpperCase(), config.url);
    } else {
      console.warn('[API Request] ⚠️ No token available for', config.method?.toUpperCase(), config.url);
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
        tokenStorage.clearTokens();
        window.location.href = '/login';
        return Promise.reject(createApiError(401, { message: ErrorMessages.UNAUTHORIZED }));
      }

      try {
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

        tokenStorage.setTokens(accessToken, newRefreshToken);
        processQueue(null, accessToken);

        if (originalRequest.headers) {
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        }
        return apiClient(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError, null);
        tokenStorage.clearTokens();
        window.location.href = '/login';
        return Promise.reject(createApiError(401, { message: ErrorMessages.UNAUTHORIZED }));
      } finally {
        isRefreshing = false;
      }
    }

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

  instance.interceptors.request = apiClient.interceptors.request;
  instance.interceptors.response = apiClient.interceptors.response;

  instance.defaults.headers['Content-Type'] = 'multipart/form-data';
  instance.defaults.timeout = 300000;

  if (onUploadProgress) {
    instance.defaults.onUploadProgress = onUploadProgress;
  }

  return instance;
};

/**
 * Generic API request wrapper
 */
export const apiRequest = {
  get: function <T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return apiClient.get<T>(url, config).then(response => response.data);
  },

  post: function <T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return apiClient.post<T>(url, data, config).then(response => response.data);
  },

  put: function <T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return apiClient.put<T>(url, data, config).then(response => response.data);
  },

  patch: function <T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    return apiClient.patch<T>(url, data, config).then(response => response.data);
  },

  delete: function <T>(url: string, config?: AxiosRequestConfig): Promise<T> {
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
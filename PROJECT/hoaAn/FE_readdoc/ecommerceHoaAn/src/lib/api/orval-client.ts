// src/lib/api/orval-client.ts
import axios, { type AxiosRequestConfig } from 'axios';
import { tokenStorage } from './client';

const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:7131',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
  withCredentials: true,
});

// Add auth token to requests
// ✅ FIXED: Chỉ gửi Authorization header khi có token hợp lệ
axiosInstance.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();

  // ✅ CRITICAL: Kiểm tra token hợp lệ trước khi gửi
  if (token && token.trim() && token !== 'null' && token !== 'undefined') {
    config.headers.Authorization = `Bearer ${token}`;
    console.log('[Orval Client] ✅ Token attached');
  } else {
    // ❌ Không gửi Authorization header nếu token không hợp lệ
    delete config.headers.Authorization;
    console.warn('[Orval Client] ⚠️ No valid token - skipping Authorization header');
  }

  return config;
});

export function apiClient<T>(config: AxiosRequestConfig): Promise<T> {
  return axiosInstance.request<T>(config).then((response) => response.data as T);
}

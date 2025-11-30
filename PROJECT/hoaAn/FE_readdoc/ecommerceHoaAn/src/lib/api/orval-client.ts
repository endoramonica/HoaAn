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
axiosInstance.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export function apiClient<T>(config: AxiosRequestConfig): Promise<T> {
  return axiosInstance.request<T>(config).then((response) => response.data as T);
}

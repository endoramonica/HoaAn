/**
 * useAuth Hook
 * Custom hook để quản lý authentication state
 */

import { useState, useEffect, useCallback } from 'react';
import { authService } from '../services/authService';
import { tokenStorage } from '../api/client';
import type {
  LoginRequest,
  RegisterRequest,
  UserDto,
  UpdateProfileRequest,
} from '../api/types';
import { toast } from 'sonner';
import { ApiError } from '../api/errors';

interface UseAuthReturn {
  user: UserDto | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
  updateProfile: (request: UpdateProfileRequest) => Promise<void>;
  changePassword: (currentPassword: string, newPassword: string) => Promise<void>;
  refreshUser: () => Promise<void>;
}

export const useAuth = (): UseAuthReturn => {
  const [user, setUser] = useState<UserDto | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  /**
   * Load user khi component mount
   */
  useEffect(() => {
    const loadUser = async () => {
      try {
        if (authService.isAuthenticated()) {
          const currentUser = await authService.getCurrentUser();
          setUser(currentUser);
        }
      } catch (err) {
        console.error('Load user error:', err);
        // Nếu token không hợp lệ, clear tokens
        tokenStorage.clearTokens();
      } finally {
        setIsLoading(false);
      }
    };

    loadUser();
  }, []);

  /**
   * Login
   */
  const login = useCallback(async (request: LoginRequest) => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await authService.login(request);
      setUser(response.user);

      toast.success('Đăng nhập thành công!');
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Đăng nhập thất bại';
      
      setError(errorMessage);
      toast.error(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Register
   */
  const register = useCallback(async (request: RegisterRequest) => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await authService.register(request);
      setUser(response.user);

      toast.success('Đăng ký thành công!');
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Đăng ký thất bại';
      
      setError(errorMessage);
      toast.error(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Logout
   */
  const logout = useCallback(async () => {
    try {
      setIsLoading(true);
      await authService.logout();
      setUser(null);
      toast.success('Đăng xuất thành công!');
    } catch (err) {
      console.error('Logout error:', err);
      // Vẫn clear user dù có lỗi
      setUser(null);
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Update profile
   */
  const updateProfile = useCallback(async (request: UpdateProfileRequest) => {
    try {
      setIsLoading(true);
      setError(null);

      const updatedUser = await authService.updateProfile(request);
      setUser(updatedUser);

      toast.success('Cập nhật thông tin thành công!');
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Cập nhật thông tin thất bại';
      
      setError(errorMessage);
      toast.error(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Change password
   */
  const changePassword = useCallback(async (currentPassword: string, newPassword: string) => {
    try {
      setIsLoading(true);
      setError(null);

      await authService.changePassword(currentPassword, newPassword);

      toast.success('Đổi mật khẩu thành công!');
    } catch (err) {
      const errorMessage = err instanceof ApiError 
        ? err.getDisplayMessage() 
        : 'Đổi mật khẩu thất bại';
      
      setError(errorMessage);
      toast.error(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Refresh user data
   */
  const refreshUser = useCallback(async () => {
    try {
      setIsLoading(true);
      const currentUser = await authService.getCurrentUser();
      setUser(currentUser);
    } catch (err) {
      console.error('Refresh user error:', err);
      setError('Không thể tải thông tin người dùng');
    } finally {
      setIsLoading(false);
    }
  }, []);

  return {
    user,
    isAuthenticated: !!user,
    isLoading,
    error,
    login,
    register,
    logout,
    updateProfile,
    changePassword,
    refreshUser,
  };
};

export default useAuth;

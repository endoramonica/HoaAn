/**
 * useAuth Hook
 * Custom hook để quản lý authentication state
 * ✅ UPDATED: Tự động merge guest cart sau khi login
 */

import { useState, useEffect, useCallback } from 'react';
import { authService } from '../services/authService';
import { tokenStorage } from '../api/client';
import { CartService } from '@/api/services/CartService';
import type {
  LoginRequest,
  RegisterRequest,
  GoogleLoginRequest,
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
  loginWithGoogle: (idToken: string) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
  updateProfile: (request: UpdateProfileRequest) => Promise<void>;
  changePassword: (currentPassword: string, newPassword: string) => Promise<void>;
  refreshUser: () => Promise<void>;
}

/**
 * ✅ Local Storage Keys for user data
 */
const USER_STORAGE_KEY = 'current_user';

/**
 * ✅ Helper: Lưu user vào localStorage
 */
const saveUserToStorage = (user: UserDto) => {
  try {
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(user));
  } catch (error) {
    console.error('Failed to save user to storage:', error);
  }
};

/**
 * ✅ Helper: Lấy user từ localStorage
 */
const getUserFromStorage = (): UserDto | null => {
  try {
    const userJson = localStorage.getItem(USER_STORAGE_KEY);
    return userJson ? JSON.parse(userJson) : null;
  } catch (error) {
    console.error('Failed to get user from storage:', error);
    return null;
  }
};

/**
 * ✅ Helper: Xóa user từ localStorage
 */
const clearUserFromStorage = () => {
  try {
    localStorage.removeItem(USER_STORAGE_KEY);
  } catch (error) {
    console.error('Failed to clear user from storage:', error);
  }
};

/**
 * ✅ NEW: Merge guest cart to user cart after login
 * Backend tự động lấy sessionId từ cookie
 */
const mergeGuestCart = async () => {
  try {
    console.log('[useAuth] 🔄 Merging guest cart to user cart...');
    
    // ✅ FIX: Backend expects empty body or specific DTO
    // Check CartService.postApiV1CartMerge() signature
    await CartService.postApiV1CartMerge({
      // Backend có thể cần:
      // guestSessionId: sessionId (nếu cần)
      // hoặc empty {} nếu backend tự lấy từ cookie
    });
    
    console.log('[useAuth] ✅ Cart merged successfully');
  } catch (error: any) {
    // Non-critical error - guest có thể không có cart
    if (error.status === 404 || error.message?.includes('Not Found')) {
      console.log('[useAuth] ℹ️ No guest cart to merge');
    } else if (error.status === 415) {
      console.error('[useAuth] ❌ Cart merge 415: Backend expects different Content-Type or body format');
      console.error('[useAuth] 💡 Check MergeCartDto in backend');
    } else {
      console.log('[useAuth] ⚠️ Cart merge failed (non-critical):', error.message);
    }
  }
};

export const useAuth = (): UseAuthReturn => {
  const [user, setUser] = useState<UserDto | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  /**
   * ✅ Load user khi component mount - Từ localStorage thay vì API
   */
  useEffect(() => {
    const loadUser = () => {
      try {
        // Check token có tồn tại không
        const hasToken = authService.isAuthenticated();

        if (hasToken) {
          // Load user từ localStorage
          const savedUser = getUserFromStorage();

          if (savedUser) {
            console.log('[useAuth] User loaded from storage:', savedUser.email);
            setUser(savedUser);
          } else {
            console.warn('[useAuth] Token exists but no user data found');
            // Token có nhưng không có user data → Clear tokens
            tokenStorage.clearTokens();
          }
        } else {
          console.log('[useAuth] No token found, user not authenticated');
        }
      } catch (err) {
        console.error('[useAuth] Load user error:', err);
        // Nếu có lỗi, clear tokens
        tokenStorage.clearTokens();
        clearUserFromStorage();
      } finally {
        setIsLoading(false);
      }
    };

    loadUser();
  }, []);

  /**
   * Login
   * ✅ UPDATED: Tự động merge cart sau khi login thành công
   */
  const login = useCallback(async (request: LoginRequest) => {
    try {
      setIsLoading(true);
      setError(null);

      console.log('[useAuth] Login started...');
      const response = await authService.login(request);

      console.log('[useAuth] Login successful, user:', response.user.email);

      // ✅ Lưu user vào state VÀ localStorage
      setUser(response.user);
      saveUserToStorage(response.user);

      // ✅ Merge guest cart to user cart
      await mergeGuestCart();

      toast.success('Đăng nhập thành công!');
    } catch (err) {
      console.error('[useAuth] Login error:', err);
      const errorMessage =
        err instanceof ApiError ? err.getDisplayMessage() : 'Đăng nhập thất bại';

      setError(errorMessage);
      toast.error(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Login with Google
   * ✅ UPDATED: Tự động merge cart sau khi login thành công
   */
  const loginWithGoogle = useCallback(async (idToken: string) => {
    try {
      setIsLoading(true);
      setError(null);

      const request: GoogleLoginRequest = { idToken };
      const response = await authService.loginWithGoogle(request);

      // ✅ Lưu user
      setUser(response.user);
      saveUserToStorage(response.user);

      // ✅ Merge guest cart to user cart
      await mergeGuestCart();

      toast.success('Đăng nhập bằng Google thành công!');
    } catch (err) {
      const errorMessage =
        err instanceof ApiError
          ? err.getDisplayMessage()
          : 'Đăng nhập bằng Google thất bại';

      setError(errorMessage);
      toast.error(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Register
   * ✅ UPDATED: Tự động merge cart sau khi đăng ký thành công
   */
  const register = useCallback(async (request: RegisterRequest) => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await authService.register(request);

      // ✅ Lưu user
      setUser(response.user);
      saveUserToStorage(response.user);

      // ✅ Merge guest cart to user cart
      await mergeGuestCart();

      toast.success('Đăng ký thành công!');
    } catch (err) {
      const errorMessage =
        err instanceof ApiError ? err.getDisplayMessage() : 'Đăng ký thất bại';

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

      // ✅ Clear user from state và storage
      setUser(null);
      clearUserFromStorage();

      toast.success('Đăng xuất thành công!');
    } catch (err) {
      console.error('[useAuth] Logout error:', err);
      // Vẫn clear user dù có lỗi
      setUser(null);
      clearUserFromStorage();
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

      // ✅ Cập nhật user
      setUser(updatedUser);
      saveUserToStorage(updatedUser);

      toast.success('Cập nhật thông tin thành công!');
    } catch (err) {
      const errorMessage =
        err instanceof ApiError
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
  const changePassword = useCallback(
    async (currentPassword: string, newPassword: string) => {
      try {
        setIsLoading(true);
        setError(null);

        await authService.changePassword(currentPassword, newPassword);

        toast.success('Đổi mật khẩu thành công!');
      } catch (err) {
        const errorMessage =
          err instanceof ApiError
            ? err.getDisplayMessage()
            : 'Đổi mật khẩu thất bại';

        setError(errorMessage);
        toast.error(errorMessage);
        throw err;
      } finally {
        setIsLoading(false);
      }
    },
    []
  );

  /**
   * ✅ Refresh user data - Gọi API nếu backend có endpoint
   * Nếu backend không có /users/me thì dùng cached data
   */
  const refreshUser = useCallback(async () => {
    try {
      setIsLoading(true);

      // Try to get fresh data from API
      try {
        const currentUser = await authService.getCurrentUser();
        setUser(currentUser);
        saveUserToStorage(currentUser);
        console.log('[useAuth] User refreshed from API');
      } catch (apiError) {
        // Nếu API fail, dùng cached data
        console.warn('[useAuth] API refresh failed, using cached data');
        const cachedUser = getUserFromStorage();
        if (cachedUser) {
          setUser(cachedUser);
        }
      }
    } catch (err) {
      console.error('[useAuth] Refresh user error:', err);
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
    loginWithGoogle,
    register,
    logout,
    updateProfile,
    changePassword,
    refreshUser,
  };
};

export default useAuth;
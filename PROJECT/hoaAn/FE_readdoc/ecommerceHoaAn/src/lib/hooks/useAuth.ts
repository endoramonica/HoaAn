/**
 * useAuth Hook
 * ✅ FIXED: Properly extract error messages from authService
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
  requireAuth: () => void;

}

const USER_STORAGE_KEY = 'current_user';

const saveUserToStorage = (user: UserDto) => {
  try {
    localStorage.setItem(USER_STORAGE_KEY, JSON.stringify(user));
  } catch (error) {
    console.error('Failed to save user to storage:', error);
  }
};

const getUserFromStorage = (): UserDto | null => {
  try {
    const userJson = localStorage.getItem(USER_STORAGE_KEY);
    return userJson ? JSON.parse(userJson) : null;
  } catch (error) {
    console.error('Failed to get user from storage:', error);
    return null;
  }
};

const clearUserFromStorage = () => {
  try {
    localStorage.removeItem(USER_STORAGE_KEY);
  } catch (error) {
    console.error('Failed to clear user from storage:', error);
  }
};

const mergeGuestCart = async () => {
  try {
    // Get guest cart sessionId from localStorage
    const guestSessionId = localStorage.getItem('guest_cart_session_id');

    if (!guestSessionId) {
      console.log('[useAuth] ℹ️ No guest cart sessionId found, skipping merge');
      return;
    }

    console.log('[useAuth] 🔄 Merging guest cart to user cart...', { guestSessionId });

    // Call merge API with sessionId
    await CartService.postApiV1CartMerge({ sessionId: guestSessionId });

    console.log('[useAuth] ✅ Cart merged successfully');

    // Clear guest cart sessionId after successful merge
    localStorage.removeItem('guest_cart_session_id');
    console.log('[useAuth] 🗑️ Cleared guest cart sessionId');
  } catch (error: any) {
    if (error.status === 404 || error.message?.includes('Not Found')) {
      console.log('[useAuth] ℹ️ No guest cart to merge (404)');
      // Clear sessionId anyway
      localStorage.removeItem('guest_cart_session_id');
    } else if (error.status === 415) {
      console.error('[useAuth] ❌ Cart merge 415: Backend expects different Content-Type');
    } else {
      console.log('[useAuth] ⚠️ Cart merge failed (non-critical):', error.message);
    }
  }
};

export const useAuth = (): UseAuthReturn => {
  const [user, setUser] = useState<UserDto | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadUser = () => {
      try {
        const hasToken = authService.isAuthenticated();

        if (hasToken) {
          const savedUser = getUserFromStorage();

          if (savedUser) {
            console.log('[useAuth] User loaded from storage:', savedUser.email);
            setUser(savedUser);
          } else {
            console.warn('[useAuth] Token exists but no user data found');
            tokenStorage.clearTokens();
          }
        } else {
          console.log('[useAuth] No token found, user not authenticated');
        }
      } catch (err) {
        console.error('[useAuth] Load user error:', err);
        tokenStorage.clearTokens();
        clearUserFromStorage();
      } finally {
        setIsLoading(false);
      }
    };

    loadUser();
  }, []);

  /**
   * ✅ FIXED: Login with proper error message extraction
   */
  const login = useCallback(async (request: LoginRequest) => {
    try {
      setIsLoading(true);
      setError(null);

      console.log('[useAuth] Login started...');
      const response = await authService.login(request);

      console.log('[useAuth] Login successful, user:', response.user.email);

      setUser(response.user);
      saveUserToStorage(response.user);

      await mergeGuestCart();

      toast.success('Đăng nhập thành công!');
    } catch (err: any) {
      console.error('[useAuth] Login error:', err);

      // ✅ FIXED: Ưu tiên message từ Error.message (đã được authService xử lý)
      const errorMessage = (err as Error).message || 'Đăng nhập thất bại';

      console.log('[useAuth] 🎯 Error message to display:', errorMessage);

      setError(errorMessage);
      toast.error(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * ✅ FIXED: Google login with proper error handling
   */
  const loginWithGoogle = useCallback(async (idToken: string) => {
    try {
      setIsLoading(true);
      setError(null);

      const request: GoogleLoginRequest = { idToken };
      const response = await authService.loginWithGoogle(request);

      setUser(response.user);
      saveUserToStorage(response.user);

      await mergeGuestCart();

      toast.success('Đăng nhập bằng Google thành công!');
    } catch (err: any) {
      console.error('[useAuth] Google login error:', err);

      // ✅ FIXED: Use Error.message directly
      const errorMessage = (err as Error).message || 'Đăng nhập bằng Google thất bại';

      setError(errorMessage);
      toast.error(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * ✅ FIXED: Register with proper error handling
   */
  const register = useCallback(async (request: RegisterRequest) => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await authService.register(request);

      setUser(response.user);
      saveUserToStorage(response.user);

      await mergeGuestCart();

      toast.success('Đăng ký thành công!');
    } catch (err: any) {
      console.error('[useAuth] Register error:', err);

      // ✅ FIXED: Use Error.message directly
      const errorMessage = (err as Error).message || 'Đăng ký thất bại';

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
      clearUserFromStorage();

      toast.success('Đăng xuất thành công!');
    } catch (err) {
      console.error('[useAuth] Logout error:', err);
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

      setUser(updatedUser);
      saveUserToStorage(updatedUser);

      toast.success('Cập nhật thông tin thành công!');
    } catch (err: any) {
      console.error('[useAuth] Update profile error:', err);

      // ✅ FIXED: Use Error.message directly
      const errorMessage = (err as Error).message || 'Cập nhật thông tin thất bại';

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
      } catch (err: any) {
        console.error('[useAuth] Change password error:', err);

        // ✅ FIXED: Use Error.message directly
        const errorMessage = (err as Error).message || 'Đổi mật khẩu thất bại';

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
 * Require user to be logged in before performing an action
 * - Shows toast
 * - Throws typed error "AUTH_REQUIRED"
 * - Auto-redirects to /login
 */
  const requireAuth = () => {
    if (!user) {
      const msg = 'Vui lòng đăng nhập để tiếp tục.';
      toast.error(msg);

      // 👇 Redirect only if running in browser (avoid SSR issues)
      if (typeof window !== 'undefined') {
        window.location.href = '/login';
      }

      const error = new Error('AUTH_REQUIRED');
      (error as any).code = 'AUTH_REQUIRED';
      throw error;
    }
  };

  /**
   * Refresh user data
   */
  const refreshUser = useCallback(async () => {
    try {
      setIsLoading(true);

      try {
        const currentUser = await authService.getCurrentUser();
        setUser(currentUser);
        saveUserToStorage(currentUser);
        console.log('[useAuth] User refreshed from API');
      } catch (apiError) {
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

    requireAuth,
  };
};

export default useAuth;
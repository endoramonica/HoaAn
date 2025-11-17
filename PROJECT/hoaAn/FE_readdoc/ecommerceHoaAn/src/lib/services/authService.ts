/**
 * Authentication Service
 * Xử lý login, register, logout, refresh token
 */

import { apiRequest, tokenStorage } from '../api/client';
import type {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  GoogleLoginRequest,
  RefreshTokenRequest,
  RefreshTokenResponse,
  UserDto,
  UpdateProfileRequest,
} from '../api/types';

/**
 * ✅ Backend Response Type - Actual structure từ API
 */
interface BackendLoginResponse {
  success: boolean;
  data: {
    token: string;           // ← Backend dùng "token" thay vì "accessToken"
    refreshToken: string;
    expires: string;
    user: {
      id: string;
      email: string;
      name: string;
      avatarUrl: string;
      provider: string;
      roles: string[];
    };
  };
  message: string;
}

/**
 * Mock data cho development khi backend chưa sẵn sàng
 */
const MOCK_USER: UserDto = {
  id: 'user-123',
  email: 'test@example.com',
  fullName: 'Nguyễn Văn A',
  phoneNumber: '0901234567',
  avatar: 'https://i.pravatar.cc/150?img=1',
  role: 'Customer' as any,
  isEmailConfirmed: true,
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString(),
};

const MOCK_LOGIN_RESPONSE: LoginResponse = {
  accessToken: 'mock_access_token_' + Date.now(),
  refreshToken: 'mock_refresh_token_' + Date.now(),
  expiresIn: 3600,
  user: MOCK_USER,
};

/**
 * Kiểm tra xem có sử dụng mock data không
 */
const getMockMode = () => {
  if (typeof import.meta !== 'undefined') {
    const env = (import.meta as any).env;
    if (env) {
      return env.VITE_USE_MOCK_DATA === 'true';
    }
  }
  return false; // ✅ Default to real API
};

const USE_MOCK = getMockMode();

class AuthService {
  /**
   * Đăng nhập
   */
  async login(request: LoginRequest): Promise<LoginResponse> {
    try {
      if (USE_MOCK) {
        // Mock delay
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        // Mock validation
        if (!request.email || !request.password) {
          throw new Error('Email và mật khẩu không được để trống');
        }

        // Save tokens with remember me
        tokenStorage.setTokens(
          MOCK_LOGIN_RESPONSE.accessToken, 
          MOCK_LOGIN_RESPONSE.refreshToken,
          request.rememberMe
        );
        
        return MOCK_LOGIN_RESPONSE;
      }

      // ✅ Gọi API với type chính xác từ backend
      const backendResponse = await apiRequest.post<BackendLoginResponse>('/auth/login', request);
      
      console.log('[AuthService] Login response:', backendResponse);

      // ✅ Extract data từ nested structure
      const { token, refreshToken, user } = backendResponse.data;

      // ✅ Lưu tokens với remember me preference
      tokenStorage.setTokens(token, refreshToken, request.rememberMe);
      
      console.log('[AuthService] Tokens saved:', {
        accessToken: token.substring(0, 20) + '...',
        refreshToken: refreshToken.substring(0, 20) + '...',
        rememberMe: request.rememberMe
      });

      // ✅ Transform backend user format sang frontend format
      const transformedResponse: LoginResponse = {
        accessToken: token,
        refreshToken: refreshToken,
        expiresIn: 7200, // 2 hours default
        user: {
          id: user.id,
          email: user.email,
          fullName: user.name,
          avatar: user.avatarUrl || undefined,
          role: user.roles[0] as any, // Take first role
          isEmailConfirmed: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        }
      };
      
      return transformedResponse;
    } catch (error) {
      console.error('[AuthService] Login error:', error);
      throw error;
    }
  }

  /**
   * Đăng nhập bằng Google
   */
  async loginWithGoogle(request: GoogleLoginRequest): Promise<LoginResponse> {
    try {
      if (USE_MOCK) {
        // Mock delay
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        // Mock validation
        if (!request.idToken) {
          throw new Error('Token Google không hợp lệ');
        }

        // Save tokens
        tokenStorage.setTokens(MOCK_LOGIN_RESPONSE.accessToken, MOCK_LOGIN_RESPONSE.refreshToken);
        
        return {
          ...MOCK_LOGIN_RESPONSE,
          user: {
            ...MOCK_USER,
            email: 'google.user@gmail.com',
            fullName: 'Google User',
          },
        };
      }

      const backendResponse = await apiRequest.post<BackendLoginResponse>('/auth/google-login', request);
      const { token, refreshToken, user } = backendResponse.data;
      
      // Lưu tokens
      tokenStorage.setTokens(token, refreshToken);
      
      return {
        accessToken: token,
        refreshToken: refreshToken,
        expiresIn: 7200,
        user: {
          id: user.id,
          email: user.email,
          fullName: user.name,
          avatar: user.avatarUrl || undefined,
          role: user.roles[0] as any,
          isEmailConfirmed: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        }
      };
    } catch (error) {
      console.error('[AuthService] Google login error:', error);
      throw error;
    }
  }

  /**
   * Đăng ký
   */
  async register(request: RegisterRequest): Promise<LoginResponse> {
    try {
      if (USE_MOCK) {
        // Mock delay
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        // Mock validation
        if (!request.email || !request.password || !request.fullName) {
          throw new Error('Vui lòng điền đầy đủ thông tin');
        }

        if (request.password !== request.confirmPassword) {
          throw new Error('Mật khẩu xác nhận không khớp');
        }

        // Save tokens
        tokenStorage.setTokens(MOCK_LOGIN_RESPONSE.accessToken, MOCK_LOGIN_RESPONSE.refreshToken);
        
        return {
          ...MOCK_LOGIN_RESPONSE,
          user: {
            ...MOCK_USER,
            email: request.email,
            fullName: request.fullName,
            phoneNumber: request.phoneNumber,
          },
        };
      }

      const backendResponse = await apiRequest.post<BackendLoginResponse>('/auth/register', request);
      const { token, refreshToken, user } = backendResponse.data;
      
      // Lưu tokens
      tokenStorage.setTokens(token, refreshToken);
      
      return {
        accessToken: token,
        refreshToken: refreshToken,
        expiresIn: 7200,
        user: {
          id: user.id,
          email: user.email,
          fullName: user.name,
          avatar: user.avatarUrl || undefined,
          role: user.roles[0] as any,
          isEmailConfirmed: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        }
      };
    } catch (error) {
      console.error('[AuthService] Register error:', error);
      throw error;
    }
  }

  /**
   * Đăng xuất
   */
  async logout(): Promise<void> {
    try {
      if (USE_MOCK) {
        // Mock delay
        await new Promise(resolve => setTimeout(resolve, 500));
        tokenStorage.clearTokens();
        return;
      }

      // Gọi API logout (optional - có thể chỉ clear tokens local)
      try {
        await apiRequest.post('/auth/logout');
      } catch (error) {
        // Ignore logout API errors
        console.warn('[AuthService] Logout API error (ignored):', error);
      }
      
      // Clear tokens
      tokenStorage.clearTokens();
    } catch (error) {
      // Vẫn clear tokens dù có lỗi
      tokenStorage.clearTokens();
      console.error('[AuthService] Logout error:', error);
    }
  }

  /**
   * Refresh access token
   */
  async refreshToken(): Promise<RefreshTokenResponse> {
    try {
      const refreshToken = tokenStorage.getRefreshToken();
      
      if (!refreshToken) {
        throw new Error('No refresh token available');
      }

      if (USE_MOCK) {
        // Mock delay
        await new Promise(resolve => setTimeout(resolve, 500));
        
        const mockResponse: RefreshTokenResponse = {
          accessToken: 'mock_access_token_refreshed_' + Date.now(),
          refreshToken: 'mock_refresh_token_refreshed_' + Date.now(),
          expiresIn: 3600,
        };

        tokenStorage.setTokens(mockResponse.accessToken, mockResponse.refreshToken);
        
        return mockResponse;
      }

      const request: RefreshTokenRequest = { refreshToken };
      const response = await apiRequest.post<RefreshTokenResponse>('/auth/refresh-token', request);
      
      // Lưu tokens mới
      tokenStorage.setTokens(response.accessToken, response.refreshToken);
      
      return response;
    } catch (error) {
      // Clear tokens nếu refresh failed
      tokenStorage.clearTokens();
      console.error('[AuthService] Refresh token error:', error);
      throw error;
    }
  }

  /**
   * Lấy thông tin user hiện tại
   */
  async getCurrentUser(): Promise<UserDto> {
    try {
      if (USE_MOCK) {
        // Mock delay
        await new Promise(resolve => setTimeout(resolve, 500));
        return MOCK_USER;
      }

      // ✅ Backend endpoint là /users/me chứ không phải /auth/me
      return await apiRequest.get<UserDto>('/users/me');
    } catch (error) {
      console.error('[AuthService] Get current user error:', error);
      throw error;
    }
  }

  /**
   * Cập nhật profile
   */
  async updateProfile(request: UpdateProfileRequest): Promise<UserDto> {
    try {
      if (USE_MOCK) {
        // Mock delay
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        return {
          ...MOCK_USER,
          ...request,
          updatedAt: new Date().toISOString(),
        };
      }

      return await apiRequest.put<UserDto>('/users/profile', request);
    } catch (error) {
      console.error('[AuthService] Update profile error:', error);
      throw error;
    }
  }

  /**
   * Đổi mật khẩu
   */
  async changePassword(currentPassword: string, newPassword: string): Promise<void> {
    try {
      if (USE_MOCK) {
        // Mock delay
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        if (!currentPassword || !newPassword) {
          throw new Error('Vui lòng điền đầy đủ thông tin');
        }
        
        return;
      }

      await apiRequest.post('/auth/change-password', {
        currentPassword,
        newPassword,
      });
    } catch (error) {
      console.error('[AuthService] Change password error:', error);
      throw error;
    }
  }

  /**
   * Kiểm tra xem user đã đăng nhập chưa
   */
  isAuthenticated(): boolean {
    return !!tokenStorage.getAccessToken();
  }

  /**
   * Gửi email xác thực
   */
  async sendVerificationEmail(): Promise<void> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        return;
      }

      await apiRequest.post('/auth/send-verification-email');
    } catch (error) {
      console.error('[AuthService] Send verification email error:', error);
      throw error;
    }
  }

  /**
   * Xác thực email
   */
  async verifyEmail(token: string): Promise<void> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        return;
      }

      await apiRequest.post('/auth/verify-email', { token });
    } catch (error) {
      console.error('[AuthService] Verify email error:', error);
      throw error;
    }
  }

  /**
   * Quên mật khẩu - Gửi email reset
   */
  async forgotPassword(email: string): Promise<void> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        return;
      }

      await apiRequest.post('/auth/forgot-password', { email });
    } catch (error) {
      console.error('[AuthService] Forgot password error:', error);
      throw error;
    }
  }

  /**
   * Reset mật khẩu
   */
  async resetPassword(token: string, newPassword: string): Promise<void> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        return;
      }

      await apiRequest.post('/auth/reset-password', { token, newPassword });
    } catch (error) {
      console.error('[AuthService] Reset password error:', error);
      throw error;
    }
  }
}

export const authService = new AuthService();
export default authService;
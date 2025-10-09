/**
 * Authentication Service
 * Xử lý login, register, logout, refresh token
 */

import { apiRequest, tokenStorage } from '../api/client';
import type {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RefreshTokenRequest,
  RefreshTokenResponse,
  UserDto,
  UpdateProfileRequest,
} from '../api/types';

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
const USE_MOCK = process.env.REACT_APP_USE_MOCK === 'true';

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

        // Save tokens
        tokenStorage.setTokens(MOCK_LOGIN_RESPONSE.accessToken, MOCK_LOGIN_RESPONSE.refreshToken);
        
        return MOCK_LOGIN_RESPONSE;
      }

      const response = await apiRequest.post<LoginResponse>('/auth/login', request);
      
      // Lưu tokens
      tokenStorage.setTokens(response.accessToken, response.refreshToken);
      
      return response;
    } catch (error) {
      console.error('Login error:', error);
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

      const response = await apiRequest.post<LoginResponse>('/auth/register', request);
      
      // Lưu tokens
      tokenStorage.setTokens(response.accessToken, response.refreshToken);
      
      return response;
    } catch (error) {
      console.error('Register error:', error);
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
      await apiRequest.post('/auth/logout');
      
      // Clear tokens
      tokenStorage.clearTokens();
    } catch (error) {
      // Vẫn clear tokens dù có lỗi
      tokenStorage.clearTokens();
      console.error('Logout error:', error);
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
      console.error('Refresh token error:', error);
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

      return await apiRequest.get<UserDto>('/auth/me');
    } catch (error) {
      console.error('Get current user error:', error);
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

      return await apiRequest.put<UserDto>('/auth/profile', request);
    } catch (error) {
      console.error('Update profile error:', error);
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
      console.error('Change password error:', error);
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
      console.error('Send verification email error:', error);
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
      console.error('Verify email error:', error);
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
      console.error('Forgot password error:', error);
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
      console.error('Reset password error:', error);
      throw error;
    }
  }
}

export const authService = new AuthService();
export default authService;

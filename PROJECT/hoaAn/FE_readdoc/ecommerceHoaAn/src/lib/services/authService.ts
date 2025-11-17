/**
 * Authentication Service
 * ✅ FIXED: Sử dụng OpenAPI generated client thay vì custom apiRequest
 */

import { AuthService as ApiAuthService } from '@/api/services/AuthService';
import { tokenStorage } from '../api/client';
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
 * ✅ OpenAPI Response Structure - Trả về raw response
 */
interface OpenAPIResponse {
  success: boolean;
  data: {
    token: string;           // ← Backend có thể dùng "token" hoặc "accessToken"
    accessToken?: string;    // ← Hỗ trợ cả 2
    refreshToken: string;
    expires?: string;
    user: {
      id: string;
      email: string;
      name?: string;
      fullName?: string;
      avatarUrl?: string;
      provider?: string;
      roles?: string[];
    };
  };
  message?: string;
}

/**
 * Mock data cho development
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

const getMockMode = () => {
  if (typeof import.meta !== 'undefined') {
    const env = (import.meta as any).env;
    if (env) {
      return env.VITE_USE_MOCK_DATA === 'true';
    }
  }
  return false;
};

const USE_MOCK = getMockMode();

class AuthService {
  /**
   * ✅ FIXED: Đăng nhập sử dụng OpenAPI client
   */
  async login(request: LoginRequest): Promise<LoginResponse> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        if (!request.email || !request.password) {
          throw new Error('Email và mật khẩu không được để trống');
        }

        tokenStorage.setTokens(
          MOCK_LOGIN_RESPONSE.accessToken, 
          MOCK_LOGIN_RESPONSE.refreshToken,
          request.rememberMe
        );
        
        return MOCK_LOGIN_RESPONSE;
      }

      console.log('[AuthService] Calling login API via OpenAPI client...');
      
      // ✅ FIXED: Dùng OpenAPI generated AuthService
      const apiResponse = await ApiAuthService.postApiV1AuthLogin({
        email: request.email,
        password: request.password,
      });
      
      console.log('[AuthService] 🔍 Raw API Response:', apiResponse);
      
      // ✅ Parse response - OpenAPI client trả về raw object
      const backendResponse = apiResponse as OpenAPIResponse;
      
      console.log('[AuthService] Parsed response structure:', {
        hasSuccess: backendResponse.success !== undefined,
        hasData: backendResponse.data !== undefined,
        dataKeys: backendResponse.data ? Object.keys(backendResponse.data) : [],
      });

      // ✅ Extract token - Hỗ trợ cả "token" và "accessToken"
      const accessToken = backendResponse.data?.accessToken || backendResponse.data?.token;
      const refreshToken = backendResponse.data?.refreshToken;
      const user = backendResponse.data?.user;

      // ✅ Validate response data
      if (!accessToken) {
        console.error('[AuthService] ❌ Missing accessToken/token in response:', backendResponse);
        console.error('[AuthService] Available keys:', Object.keys(backendResponse.data || {}));
        throw new Error('Backend không trả về access token');
      }

      if (!refreshToken) {
        console.error('[AuthService] ❌ Missing refreshToken in response');
        throw new Error('Backend không trả về refresh token');
      }

      if (!user) {
        console.error('[AuthService] ❌ Missing user in response');
        throw new Error('Backend không trả về thông tin user');
      }

      // ✅ Lưu tokens
      tokenStorage.setTokens(accessToken, refreshToken, request.rememberMe);
      
      console.log('[AuthService] ✅ Tokens saved successfully:', {
        accessToken: accessToken.substring(0, 30) + '...',
        refreshToken: refreshToken.substring(0, 30) + '...',
        rememberMe: request.rememberMe,
      });

      // ✅ Transform backend user sang frontend format
      const transformedResponse: LoginResponse = {
        accessToken,
        refreshToken,
        expiresIn: 7200,
        user: {
          id: user.id,
          email: user.email,
          fullName: user.fullName || user.name || user.email,
          avatar: user.avatarUrl,
          role: (user.roles && user.roles.length > 0 ? user.roles[0] : 'Customer') as any,
          isEmailConfirmed: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString(),
        }
      };
      
      console.log('[AuthService] ✅ Login successful:', {
        userId: transformedResponse.user.id,
        email: transformedResponse.user.email,
        role: transformedResponse.user.role,
      });

      return transformedResponse;
    } catch (error: any) {
      console.error('[AuthService] ❌ Login error:', error);
      console.error('[AuthService] Error details:', {
        message: error.message,
        body: error.body,
        status: error.status,
      });
      throw error;
    }
  }

  /**
   * ✅ FIXED: Google login
   */
  async loginWithGoogle(request: GoogleLoginRequest): Promise<LoginResponse> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        if (!request.idToken) {
          throw new Error('Token Google không hợp lệ');
        }

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

      const apiResponse = await ApiAuthService.postApiV1AuthLoginGoogle({
        idToken: request.idToken,
      });
      
      const backendResponse = apiResponse as OpenAPIResponse;
      const accessToken = backendResponse.data?.accessToken || backendResponse.data?.token;
      const refreshToken = backendResponse.data?.refreshToken;
      const user = backendResponse.data?.user;
      
      if (!accessToken || !refreshToken || !user) {
        throw new Error('Invalid response from Google login');
      }

      tokenStorage.setTokens(accessToken, refreshToken);
      
      return {
        accessToken,
        refreshToken,
        expiresIn: 7200,
        user: {
          id: user.id,
          email: user.email,
          fullName: user.fullName || user.name || user.email,
          avatar: user.avatarUrl,
          role: (user.roles && user.roles.length > 0 ? user.roles[0] : 'Customer') as any,
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
   * ✅ FIXED: Register
   */
  async register(request: RegisterRequest): Promise<LoginResponse> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        if (!request.email || !request.password || !request.fullName) {
          throw new Error('Vui lòng điền đầy đủ thông tin');
        }

        if (request.password !== request.confirmPassword) {
          throw new Error('Mật khẩu xác nhận không khớp');
        }

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

      const apiResponse = await ApiAuthService.postApiV1AuthRegister({
        email: request.email,
        password: request.password,
        confirmPassword: request.confirmPassword,
        fullName: request.fullName,
        phoneNumber: request.phoneNumber,
      });
      
      const backendResponse = apiResponse as OpenAPIResponse;
      const accessToken = backendResponse.data?.accessToken || backendResponse.data?.token;
      const refreshToken = backendResponse.data?.refreshToken;
      const user = backendResponse.data?.user;
      
      if (!accessToken || !refreshToken || !user) {
        throw new Error('Invalid response from register');
      }

      tokenStorage.setTokens(accessToken, refreshToken);
      
      return {
        accessToken,
        refreshToken,
        expiresIn: 7200,
        user: {
          id: user.id,
          email: user.email,
          fullName: user.fullName || user.name || user.email,
          avatar: user.avatarUrl,
          role: (user.roles && user.roles.length > 0 ? user.roles[0] : 'Customer') as any,
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
        await new Promise(resolve => setTimeout(resolve, 500));
        tokenStorage.clearTokens();
        return;
      }

      try {
        await ApiAuthService.postApiV1AuthLogout();
      } catch (error) {
        console.warn('[AuthService] Logout API error (ignored):', error);
      }
      
      tokenStorage.clearTokens();
      console.log('[AuthService] ✅ Logged out successfully');
    } catch (error) {
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
        await new Promise(resolve => setTimeout(resolve, 500));
        
        const mockResponse: RefreshTokenResponse = {
          accessToken: 'mock_access_token_refreshed_' + Date.now(),
          refreshToken: 'mock_refresh_token_refreshed_' + Date.now(),
          expiresIn: 3600,
        };

        tokenStorage.setTokens(mockResponse.accessToken, mockResponse.refreshToken);
        
        return mockResponse;
      }

      const apiResponse = await ApiAuthService.postApiV1AuthRefreshToken({
        refreshToken,
      });
      
      const response = apiResponse as any;
      const newAccessToken = response.data?.accessToken || response.data?.token;
      const newRefreshToken = response.data?.refreshToken;
      
      if (!newAccessToken || !newRefreshToken) {
        throw new Error('Invalid refresh token response');
      }
      
      tokenStorage.setTokens(newAccessToken, newRefreshToken);
      
      return {
        accessToken: newAccessToken,
        refreshToken: newRefreshToken,
        expiresIn: 7200,
      };
    } catch (error) {
      tokenStorage.clearTokens();
      console.error('[AuthService] Refresh token error:', error);
      throw error;
    }
  }

  /**
   * Kiểm tra authentication
   */
  isAuthenticated(): boolean {
    const hasToken = !!tokenStorage.getAccessToken();
    return hasToken;
  }

  // Placeholder methods - Implement nếu backend có endpoints
  async getCurrentUser(): Promise<UserDto> {
    throw new Error('Not implemented - Backend endpoint /users/me needed');
  }

  async updateProfile(request: UpdateProfileRequest): Promise<UserDto> {
    throw new Error('Not implemented - Backend endpoint /users/profile needed');
  }

  async changePassword(currentPassword: string, newPassword: string): Promise<void> {
    await ApiAuthService.postApiV1AuthChangePassword({
      currentPassword,
      newPassword,
    });
  }

  async sendVerificationEmail(): Promise<void> {
    throw new Error('Not implemented');
  }

  async verifyEmail(token: string): Promise<void> {
    await ApiAuthService.getApiV1AuthVerifyEmail(token);
  }

  async forgotPassword(email: string): Promise<void> {
    await ApiAuthService.postApiV1AuthForgotPassword({ email });
  }

  async resetPassword(token: string, newPassword: string): Promise<void> {
    await ApiAuthService.postApiV1AuthResetPassword({ token, newPassword });
  }
}

export const authService = new AuthService();
export default authService;
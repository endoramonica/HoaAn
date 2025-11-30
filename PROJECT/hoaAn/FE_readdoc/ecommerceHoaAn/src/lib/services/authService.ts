/**
 * Authentication Service
 * ✅ FIXED: Extract error messages from backend response body
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
 * ✅ OpenAPI Response Structure
 */
interface OpenAPIResponse {
  success: boolean;
  data: {
    token: string;
    accessToken?: string;
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
 * ✅ Helper: Extract error message from OpenAPI client error
 * Priority:
 * 1. error.body.message (backend custom message)
 * 2. error.body.title
 * 3. error.message
 * 4. Default message
 */
function extractErrorMessage(error: any, defaultMessage: string): string {
  console.log('[AuthService] 🔍 Extracting error message...');
  console.log('[AuthService] Error structure:', {
    hasBody: !!error.body,
    bodyType: typeof error.body,
    hasMessage: !!error.message,
    status: error.status,
  });

  // Case 1: error.body is an object with message
  if (error.body && typeof error.body === 'object') {
    console.log('[AuthService] 📦 error.body content:', error.body);
    
    if (error.body.message) {
      console.log('[AuthService] ✅ Found message in error.body.message');
      return error.body.message;
    }
    
    if (error.body.title) {
      console.log('[AuthService] ✅ Found message in error.body.title');
      return error.body.title;
    }

    // Case 2: error.body is a string (rare but possible)
    if (typeof error.body === 'string') {
      console.log('[AuthService] ✅ error.body is string');
      return error.body;
    }
  }

  // Case 3: Axios-style error.response.data.message
  if (error.response?.data?.message) {
    console.log('[AuthService] ✅ Found message in error.response.data.message');
    return error.response.data.message;
  }

  // Case 4: Direct error.message (but avoid generic ones)
  if (error.message && error.message !== 'Unauthorized' && error.message !== 'Request failed') {
    console.log('[AuthService] ✅ Using error.message');
    return error.message;
  }

  // Case 5: String error
  if (typeof error === 'string') {
    console.log('[AuthService] ✅ Error is string');
    return error;
  }

  console.log('[AuthService] ⚠️ Using default message:', defaultMessage);
  return defaultMessage;
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
   * ✅ FIXED: Login with proper error message extraction
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
      
      const apiResponse = await ApiAuthService.postApiV1AuthLogin({
        email: request.email,
        password: request.password,
      });
      
      console.log('[AuthService] 🔍 Raw API Response:', apiResponse);
      
      const backendResponse = apiResponse as OpenAPIResponse;
      
      const accessToken = backendResponse.data?.accessToken || backendResponse.data?.token;
      const refreshToken = backendResponse.data?.refreshToken;
      const user = backendResponse.data?.user;

      if (!accessToken) {
        console.error('[AuthService] ❌ Missing accessToken/token in response');
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

      tokenStorage.setTokens(accessToken, refreshToken, request.rememberMe);
      
      console.log('[AuthService] ✅ Tokens saved successfully');

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
      
      console.log('[AuthService] ✅ Login successful:', transformedResponse.user.email);

      return transformedResponse;
      
    } catch (error: any) {
      console.error('[AuthService] ❌ Login error:', error);
      console.error('[AuthService] Full error object:', JSON.stringify(error, null, 2));
      
      // ✅ CRITICAL: Extract message from error.body
      const errorMessage = extractErrorMessage(error, 'Đăng nhập thất bại');
      
      console.error('[AuthService] 🎯 Final error message to display:', errorMessage);
      
      // ✅ Create new error with extracted message
      const customError = new Error(errorMessage);
      (customError as any).status = error.status;
      (customError as any).originalError = error;
      
      throw customError;
    }
  }

  /**
   * ✅ FIXED: Google login with error extraction
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
      
    } catch (error: any) {
      console.error('[AuthService] Google login error:', error);
      
      const errorMessage = extractErrorMessage(error, 'Đăng nhập bằng Google thất bại');
      
      const customError = new Error(errorMessage);
      (customError as any).status = error.status;
      throw customError;
    }
  }

  /**
   * ✅ FIXED: Register with error extraction
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
      
    } catch (error: any) {
      console.error('[AuthService] Register error:', error);
      
      const errorMessage = extractErrorMessage(error, 'Đăng ký thất bại');
      
      const customError = new Error(errorMessage);
      (customError as any).status = error.status;
      throw customError;
    }
  }

  /**
   * Logout
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
   * Refresh token
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
   * Check authentication
   */
  isAuthenticated(): boolean {
    return !!tokenStorage.getAccessToken();
  }

  /**
   * Change password
   */
  async changePassword(currentPassword: string, newPassword: string): Promise<void> {
    try {
      await ApiAuthService.postApiV1AuthChangePassword({
        currentPassword,
        newPassword,
      });
    } catch (error: any) {
      const errorMessage = extractErrorMessage(error, 'Đổi mật khẩu thất bại');
      const customError = new Error(errorMessage);
      throw customError;
    }
  }

  /**
   * Placeholder methods
   */
  async getCurrentUser(): Promise<UserDto> {
    throw new Error('Not implemented - Backend endpoint /users/me needed');
  }

  async updateProfile(request: UpdateProfileRequest): Promise<UserDto> {
    throw new Error('Not implemented - Backend endpoint /users/profile needed');
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
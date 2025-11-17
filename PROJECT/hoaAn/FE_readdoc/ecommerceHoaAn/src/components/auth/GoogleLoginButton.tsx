/**
 * GoogleLoginButton - Nút đăng nhập/đăng ký bằng Google
 * Sử dụng @react-oauth/google hoặc mock cho development
 */

import { useState } from 'react';
import { useAuth } from '../../lib/hooks/useAuth';
import { useHybridNavigate } from '../../lib/hooks/useHybridNavigate';
import { Button } from '../ui/button';
import { Loader2 } from 'lucide-react';

interface GoogleLoginButtonProps {
  onNavigate: (page: string) => void;
  isSignUp?: boolean;
}

export const GoogleLoginButton = ({ onNavigate, isSignUp = false }: GoogleLoginButtonProps) => {
  const navigate = useHybridNavigate(onNavigate);
  const { loginWithGoogle } = useAuth();
  const [isLoading, setIsLoading] = useState(false);

  /**
   * Xử lý đăng nhập bằng Google
   * TODO: Tích hợp thực tế với @react-oauth/google khi có Google Client ID
   */
  const handleGoogleLogin = async () => {
    try {
      setIsLoading(true);

      // MOCK MODE: Simulate Google OAuth flow
      // Trong production, sử dụng Google OAuth SDK
      if (typeof import.meta !== 'undefined' && import.meta.env && import.meta.env.VITE_USE_MOCK_DATA === 'true') {
        // Mock delay để giả lập OAuth flow
        await new Promise(resolve => setTimeout(resolve, 1500));
        
        // Mock Google ID token
        const mockGoogleIdToken = 'mock_google_id_token_' + Date.now();
        
        await loginWithGoogle(mockGoogleIdToken);
        
        // Chuyển hướng sau khi đăng nhập thành công
        navigate('home');
      } else {
        // PRODUCTION MODE: Real Google OAuth
        // Kiểm tra xem có Google Client ID không
        const googleClientId = import.meta.env?.VITE_GOOGLE_CLIENT_ID;
        
        if (!googleClientId) {
          throw new Error('Google Client ID chưa được cấu hình');
        }

        // TODO: Tích hợp thực tế với Google OAuth
        // Sử dụng thư viện @react-oauth/google
        // import { useGoogleLogin } from '@react-oauth/google';
        
        // Hiện tại show error message
        alert('Tính năng đăng nhập Google đang được phát triển.\nVui lòng sử dụng form đăng nhập thông thường.');
      }
    } catch (error) {
      console.error('Google login error:', error);
      // Error được xử lý bởi useAuth hook với toast notification
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Button
      type="button"
      variant="outline"
      className="w-full border-[#92400E]/30 hover:border-[#92400E] hover:bg-[#92400E]/5 text-[#92400E] shadow-sm"
      onClick={handleGoogleLogin}
      disabled={isLoading}
    >
      {isLoading ? (
        <>
          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
          Đang xử lý...
        </>
      ) : (
        <>
          <svg className="mr-2 h-4 w-4" viewBox="0 0 24 24">
            <path
              fill="currentColor"
              d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
            />
            <path
              fill="#34A853"
              d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
            />
            <path
              fill="#FBBC05"
              d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
            />
            <path
              fill="#EA4335"
              d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
            />
          </svg>
          {isSignUp ? 'Đăng ký bằng Google' : 'Đăng nhập với Google'}
        </>
      )}
    </Button>
  );
};

export default GoogleLoginButton;

/**
 * HƯỚNG DẪN TÍCH HỢP GOOGLE OAUTH THỰC TẾ
 * ========================================
 * 
 * 1. Cài đặt thư viện Google OAuth:
 *    npm install @react-oauth/google
 * 
 * 2. Lấy Google Client ID từ Google Cloud Console:
 *    - Truy cập https://console.cloud.google.com/
 *    - Tạo project mới hoặc chọn project hiện có
 *    - Enable Google+ API
 *    - Tạo OAuth 2.0 Client ID (Web application)
 *    - Thêm authorized redirect URIs
 *    - Copy Client ID
 * 
 * 3. Thêm Google Client ID vào .env:
 *    VITE_GOOGLE_CLIENT_ID=your_google_client_id_here
 * 
 * 4. Wrap App với GoogleOAuthProvider trong App.tsx:
 *    import { GoogleOAuthProvider } from '@react-oauth/google';
 *    
 *    <GoogleOAuthProvider clientId={import.meta.env.VITE_GOOGLE_CLIENT_ID}>
 *      <App />
 *    </GoogleOAuthProvider>
 * 
 * 5. Sử dụng hook useGoogleLogin trong component này:
 *    import { useGoogleLogin } from '@react-oauth/google';
 *    
 *    const login = useGoogleLogin({
 *      onSuccess: async (tokenResponse) => {
 *        await loginWithGoogle(tokenResponse.access_token);
 *        onNavigate('home');
 *      },
 *      onError: (error) => console.error('Google login error:', error),
 *    });
 *    
 *    // Trong button onClick:
 *    onClick={() => login()}
 * 
 * 6. Backend (.NET Core) cần có endpoint POST /api/auth/google-login
 *    Nhận GoogleLoginRequest { idToken: string }
 *    Verify token với Google API
 *    Tạo hoặc update user
 *    Trả về LoginResponse với JWT tokens
 */

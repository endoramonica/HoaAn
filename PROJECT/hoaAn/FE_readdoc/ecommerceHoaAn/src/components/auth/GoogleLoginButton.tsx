/**
 * GoogleLoginButton - Nút đăng nhập/đăng ký bằng Google
 * Tích hợp với @react-oauth/google
 * ✅ FIXED: Dùng GoogleLogin component để nhận JWT credential (id_token)
 * BE verify JWT với GoogleJsonWebSignature.ValidateAsync
 */

import { GoogleLogin } from '@react-oauth/google';
import { useAuth } from '../../lib/hooks/useAuth';
import { useHybridNavigate } from '../../lib/hooks/useHybridNavigate';
import { useState } from 'react';

interface GoogleLoginButtonProps {
  onNavigate: (page: string) => void;
  isSignUp?: boolean;
}

export const GoogleLoginButton = ({ onNavigate, isSignUp = false }: GoogleLoginButtonProps) => {
  const navigate = useHybridNavigate(onNavigate);
  const { loginWithGoogle } = useAuth();
  const [isLoading, setIsLoading] = useState(false);

  return (
    <GoogleLogin
      onSuccess={async (credentialResponse) => {
        try {
          setIsLoading(true);
          console.log('[GoogleLoginButton] ✅ Google Sign-In successful');
          console.log('[GoogleLoginButton] 🔍 credentialResponse type:', typeof credentialResponse);
          console.log('[GoogleLoginButton] 🔍 credentialResponse:', credentialResponse);
          console.log('[GoogleLoginButton] 🔍 credentialResponse keys:', Object.keys(credentialResponse || {}));
          console.log('[GoogleLoginButton] 🔍 credentialResponse.credential:', credentialResponse?.credential);
          console.log('[GoogleLoginButton] 🔍 credentialResponse.credential type:', typeof credentialResponse?.credential);
          
          // ✅ FIXED: GoogleLogin trả về object với credential property
          const idToken = credentialResponse?.credential;

          console.log('[GoogleLoginButton] 🔍 idToken:', {
            exists: !!idToken,
            type: typeof idToken,
            length: idToken?.length,
            preview: idToken?.substring(0, 50),
            isString: typeof idToken === 'string',
          });

          if (!idToken || typeof idToken !== 'string') {
            console.error('[GoogleLoginButton] ❌ idToken is not a string:', {
              idToken,
              type: typeof idToken,
            });
            throw new Error('Không nhận được JWT từ Google. Vui lòng thử lại.');
          }

          console.log('[GoogleLoginButton] 📤 Calling loginWithGoogle with idToken...');
          
          // Gửi JWT credential (id_token) tới backend
          // Backend sẽ verify JWT với GoogleJsonWebSignature.ValidateAsync
          await loginWithGoogle(idToken);

          // Chuyển hướng sau khi đăng nhập thành công
          navigate('home');
        } catch (error) {
          console.error('[GoogleLoginButton] ❌ Login error:', error);
          // Error được xử lý bởi useAuth hook với toast notification
        } finally {
          setIsLoading(false);
        }
      }}
      onError={() => {
        console.error('[GoogleLoginButton] ❌ Google Sign-In failed');
      }}
      text={isSignUp ? 'signup_with' : 'signin_with'}
      size="large"
      width="100%"
    />
  );
};

export default GoogleLoginButton;

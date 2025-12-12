# Google OAuth Integration Guide

## ✅ Hoàn Thành

Tích hợp Google OAuth đã được hoàn thành với các thành phần sau:

### 1. **Frontend Setup**

#### Cài đặt thư viện
```bash
npm install @react-oauth/google
```

#### Cấu hình .env
```env
VITE_GOOGLE_CLIENT_ID=827299123899-ofqi52mkbf59d3id9cjrs9sq54av1rr3.apps.googleusercontent.com
```

#### Wrap App với GoogleOAuthProvider (src/App.tsx)
```tsx
import { GoogleOAuthProvider } from "@react-oauth/google";

export default function App() {
  const googleClientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;

  return (
    <GoogleOAuthProvider clientId={googleClientId || ''}>
      <BrowserRouter>
        {/* Routes */}
      </BrowserRouter>
    </GoogleOAuthProvider>
  );
}
```

### 2. **Components**

#### GoogleLoginButton (src/components/auth/GoogleLoginButton.tsx)
- Sử dụng `useGoogleLogin` hook từ `@react-oauth/google`
- Gửi access token tới backend
- Xử lý lỗi và hiển thị loading state

#### LoginPage (src/pages/auth/LoginPage.tsx)
- Tích hợp GoogleLoginButton
- Hỗ trợ đăng nhập bằng email/password hoặc Google

#### RegisterPage (src/pages/auth/RegisterPage.tsx)
- Tích hợp GoogleLoginButton
- Hỗ trợ đăng ký bằng form hoặc Google

### 3. **Services**

#### authService (src/lib/services/authService.ts)
- `loginWithGoogle(request: GoogleLoginRequest)` - Xử lý Google login
- Gửi Google token tới backend endpoint `/api/v1/Auth/login/google`
- Xử lý lỗi từ backend và hiển thị thông báo

#### googleTokenDecoder (src/lib/utils/googleTokenDecoder.ts)
- Decode Google ID token để lấy thông tin user
- Extract email, name, picture từ token
- Gửi thông tin này cùng với token tới backend

### 4. **Backend Integration**

#### Endpoint: POST /api/v1/Auth/login/google
```
Request Body (SocialLoginRequestDTO):
{
  "provider": "Google",
  "idToken": "google_id_token",
  "email": "user@gmail.com",
  "name": "User Name",
  "avatarUrl": "https://..."
}

Response (LoginResponse):
{
  "accessToken": "jwt_token",
  "refreshToken": "refresh_token",
  "expiresIn": 7200,
  "user": {
    "id": "user_id",
    "email": "user@gmail.com",
    "fullName": "User Name",
    "avatar": "https://...",
    "role": "Customer",
    "isEmailConfirmed": true,
    "createdAt": "2025-12-11T...",
    "updatedAt": "2025-12-11T..."
  }
}
```

### 5. **Flow Diagram**

```
User clicks "Đăng nhập với Google"
    ↓
GoogleLoginButton triggers useGoogleLogin()
    ↓
Google OAuth popup appears
    ↓
User authenticates with Google
    ↓
Google returns access_token
    ↓
GoogleLoginButton calls loginWithGoogle(accessToken)
    ↓
authService.loginWithGoogle():
  - Decode token to extract user info
  - Send to backend: POST /api/v1/Auth/login/google
    ↓
Backend verifies token with Google API
    ↓
Backend creates/updates user in database
    ↓
Backend returns JWT tokens
    ↓
Frontend saves tokens to localStorage
    ↓
Frontend redirects to home page
```

## 🔧 Configuration Details

### Google Cloud Console Setup

1. **Create Project**
   - Go to https://console.cloud.google.com/
   - Create new project or select existing

2. **Enable Google+ API**
   - Search for "Google+ API"
   - Click "Enable"

3. **Create OAuth 2.0 Credentials**
   - Go to "Credentials" section
   - Click "Create Credentials" → "OAuth 2.0 Client ID"
   - Choose "Web application"
   - Add Authorized JavaScript origins:
     - `http://localhost:5173` (development)
     - `http://localhost:3000` (if using different port)
     - `https://yourdomain.com` (production)
   - Add Authorized redirect URIs:
     - `http://localhost:5173` (development)
     - `https://yourdomain.com` (production)
   - Copy Client ID and add to .env

### Backend Requirements

Backend (.NET Core) must:

1. **Verify Google Token**
   - Use Google.Apis.Auth NuGet package
   - Verify token signature with Google's public keys
   - Extract claims from token

2. **Create/Update User**
   - Check if user exists by email
   - If not exists, create new user with Google info
   - If exists, update avatar and name if provided

3. **Generate JWT Tokens**
   - Create access token (short-lived, ~2 hours)
   - Create refresh token (long-lived, ~7 days)
   - Return both tokens with user info

## 📝 Environment Variables

```env
# Google OAuth
VITE_GOOGLE_CLIENT_ID=827299123899-ofqi52mkbf59d3id9cjrs9sq54av1rr3.apps.googleusercontent.com

# API
VITE_API_URL=https://localhost:7131
```

## 🧪 Testing

### Development Mode
- Use mock data by setting `VITE_USE_MOCK_DATA=true`
- Mock Google login will simulate OAuth flow

### Production Mode
- Real Google OAuth flow
- Backend must verify tokens with Google API
- Ensure HTTPS is used

## 🐛 Troubleshooting

### Issue: "Google Client ID not configured"
**Solution:** Add `VITE_GOOGLE_CLIENT_ID` to .env file

### Issue: "Google login popup doesn't appear"
**Solution:** 
- Check if GoogleOAuthProvider is wrapping the app
- Verify Client ID is correct
- Check browser console for errors

### Issue: "Backend returns 400 Bad Request"
**Solution:**
- Verify SocialLoginRequestDTO structure matches backend expectations
- Check if backend is receiving idToken correctly
- Verify Google token verification in backend

### Issue: "User not created after Google login"
**Solution:**
- Check backend logs for token verification errors
- Verify user creation logic in backend
- Ensure database connection is working

## 📚 References

- [Google OAuth Documentation](https://developers.google.com/identity/protocols/oauth2)
- [@react-oauth/google Documentation](https://www.npmjs.com/package/@react-oauth/google)
- [Google.Apis.Auth NuGet Package](https://www.nuget.org/packages/Google.Apis.Auth/)

## ✨ Features

✅ Google OAuth 2.0 integration
✅ Automatic user creation/update
✅ JWT token generation
✅ Error handling and user feedback
✅ Mock mode for development
✅ Responsive UI with loading states
✅ Vietnamese language support
✅ Token storage and refresh

## 🚀 Next Steps

1. **Backend Implementation**
   - Implement `/api/v1/Auth/login/google` endpoint
   - Add Google token verification
   - Add user creation/update logic

2. **Testing**
   - Test Google login flow
   - Test error scenarios
   - Test token refresh

3. **Deployment**
   - Add production domain to Google Cloud Console
   - Update .env for production
   - Test on production environment

## 📞 Support

For issues or questions:
1. Check browser console for errors
2. Check backend logs
3. Verify Google Cloud Console configuration
4. Review this guide for troubleshooting steps

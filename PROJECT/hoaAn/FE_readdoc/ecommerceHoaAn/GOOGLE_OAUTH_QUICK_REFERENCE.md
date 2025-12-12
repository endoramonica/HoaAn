# Google OAuth Quick Reference

## 🚀 Quick Start

### Frontend (Already Done ✅)

1. **Install dependency**
   ```bash
   npm install @react-oauth/google
   ```

2. **Add to .env**
   ```env
   VITE_GOOGLE_CLIENT_ID=827299123899-ofqi52mkbf59d3id9cjrs9sq54av1rr3.apps.googleusercontent.com
   ```

3. **Wrap App with GoogleOAuthProvider** (src/App.tsx)
   ```tsx
   import { GoogleOAuthProvider } from "@react-oauth/google";
   
   <GoogleOAuthProvider clientId={import.meta.env.VITE_GOOGLE_CLIENT_ID}>
     <BrowserRouter>
       {/* Routes */}
     </BrowserRouter>
   </GoogleOAuthProvider>
   ```

4. **Use GoogleLoginButton** (Already in LoginPage & RegisterPage)
   ```tsx
   <GoogleLoginButton onNavigate={navigate} />
   ```

### Backend (TODO)

1. **Install NuGet packages**
   ```bash
   dotnet add package Google.Apis.Auth
   ```

2. **Create GoogleAuthService**
   - Verify Google token
   - Extract user info

3. **Update AuthController**
   - Add POST `/api/v1/Auth/login/google` endpoint
   - Accept SocialLoginRequestDTO
   - Return LoginResponse

4. **Update AuthService**
   - Implement FindOrCreateGoogleUserAsync
   - Create/update user in database

## 📋 Request/Response Format

### Request
```json
POST /api/v1/Auth/login/google

{
  "provider": "Google",
  "idToken": "google_id_token_here",
  "email": "user@gmail.com",
  "name": "User Name",
  "avatarUrl": "https://..."
}
```

### Response
```json
{
  "success": true,
  "data": {
    "accessToken": "jwt_token_here",
    "refreshToken": "refresh_token_here",
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
}
```

## 🔑 Key Files

### Frontend
- `src/components/auth/GoogleLoginButton.tsx` - Google login button
- `src/lib/services/authService.ts` - Auth service with loginWithGoogle
- `src/lib/utils/googleTokenDecoder.ts` - Token decoder utility
- `src/pages/auth/LoginPage.tsx` - Login page with Google button
- `src/pages/auth/RegisterPage.tsx` - Register page with Google button

### Backend (To be implemented)
- `Services/GoogleAuthService.cs` - Google token verification
- `Controllers/AuthController.cs` - Auth endpoints
- `DTOs/SocialLoginRequestDTO.cs` - Request DTO
- `DTOs/GoogleUserInfo.cs` - Google user info

## 🔄 Flow

```
User clicks "Đăng nhập với Google"
    ↓
Google OAuth popup
    ↓
User authenticates
    ↓
Frontend receives access_token
    ↓
Frontend sends to backend: POST /api/v1/Auth/login/google
    ↓
Backend verifies token with Google
    ↓
Backend creates/updates user
    ↓
Backend returns JWT tokens
    ↓
Frontend saves tokens
    ↓
Frontend redirects to home
```

## 🧪 Testing

### Frontend
```bash
npm run dev
# Navigate to /auth/login
# Click "Đăng nhập với Google"
# Authenticate with Google account
```

### Backend
```bash
curl -X POST https://localhost:7131/api/v1/Auth/login/google \
  -H "Content-Type: application/json" \
  -d '{
    "provider": "Google",
    "idToken": "your_token",
    "email": "user@gmail.com",
    "name": "User Name",
    "avatarUrl": "https://..."
  }'
```

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| Google button not showing | Check GoogleOAuthProvider wraps app |
| Popup doesn't appear | Verify Client ID in .env |
| Backend returns 400 | Check SocialLoginRequestDTO structure |
| User not created | Check backend logs for token verification |
| Token verification fails | Verify Client ID matches Google Cloud Console |

## 📚 Full Documentation

- **GOOGLE_OAUTH_INTEGRATION_GUIDE.md** - Complete frontend guide
- **GOOGLE_OAUTH_BACKEND_GUIDE.md** - Complete backend guide
- **GOOGLE_OAUTH_IMPLEMENTATION_SUMMARY.md** - Implementation overview

## ✅ Checklist

### Frontend (Done ✅)
- [x] Install @react-oauth/google
- [x] Add Google Client ID to .env
- [x] Wrap App with GoogleOAuthProvider
- [x] Implement GoogleLoginButton
- [x] Integrate with LoginPage
- [x] Integrate with RegisterPage
- [x] Create token decoder utility
- [x] Update authService

### Backend (TODO)
- [ ] Install Google.Apis.Auth
- [ ] Create GoogleAuthService
- [ ] Implement token verification
- [ ] Create FindOrCreateGoogleUserAsync
- [ ] Add /login/google endpoint
- [ ] Test with frontend
- [ ] Add error handling
- [ ] Deploy to production

## 🔐 Security

- ✅ Google token verification (backend)
- ✅ JWT token generation
- ✅ Refresh token support
- ✅ HTTPS recommended
- ✅ CORS configured
- ✅ Error handling

## 📞 Support

For detailed information, see:
1. GOOGLE_OAUTH_INTEGRATION_GUIDE.md (Frontend)
2. GOOGLE_OAUTH_BACKEND_GUIDE.md (Backend)
3. GOOGLE_OAUTH_IMPLEMENTATION_SUMMARY.md (Overview)

## 🎯 Next Steps

1. Backend team implements services
2. Test full flow
3. Deploy to production
4. Monitor authentication logs

---

**Status:** Frontend ✅ | Backend 🔄 | Integration 🔄

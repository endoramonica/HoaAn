# 🔐 Hướng Dẫn Tích Hợp Authentication

## 📋 Tổng Quan

Tài liệu này hướng dẫn chi tiết về hệ thống authentication đã được tích hợp vào website thương mại điện tử đồ cúng Việt Nam, bao gồm:

- ✅ Đăng nhập/đăng ký truyền thống với email & password
- ✅ Đăng nhập bằng Google OAuth
- ✅ Quản lý JWT tokens với auto-refresh
- ✅ Protected routes và authorization
- ✅ Profile management
- ✅ Password management

---

## 🏗️ Kiến Trúc Hệ Thống

### 1. API Client Layer (`/lib/api/`)

**File: `/lib/api/client.ts`**
- Axios instance với base URL từ environment variable
- Request interceptor: Tự động attach JWT token
- Response interceptor: Xử lý errors, auto-refresh token khi 401
- Token storage utilities (localStorage)
- File upload client với progress tracking

**File: `/lib/api/types.ts`**
- TypeScript interfaces/types đồng bộ với backend .NET Core
- Authentication types: LoginRequest, RegisterRequest, GoogleLoginRequest, LoginResponse
- User types: UserDto, UserRole, UpdateProfileRequest
- Các types khác: Product, Order, Delivery, Spiritual Services, etc.

**File: `/lib/api/errors.ts`**
- ApiError class với methods helpers
- Parse ProblemDetails từ ASP.NET Core
- Error messages tiếng Việt
- Validation error handling

### 2. Service Layer (`/lib/services/`)

**File: `/lib/services/authService.ts`**

Các methods chính:

```typescript
// Đăng nhập truyền thống
login(request: LoginRequest): Promise<LoginResponse>

// Đăng nhập bằng Google
loginWithGoogle(request: GoogleLoginRequest): Promise<LoginResponse>

// Đăng ký
register(request: RegisterRequest): Promise<LoginResponse>

// Đăng xuất
logout(): Promise<void>

// Refresh access token
refreshToken(): Promise<RefreshTokenResponse>

// Lấy thông tin user hiện tại
getCurrentUser(): Promise<UserDto>

// Cập nhật profile
updateProfile(request: UpdateProfileRequest): Promise<UserDto>

// Đổi mật khẩu
changePassword(currentPassword: string, newPassword: string): Promise<void>

// Kiểm tra authentication status
isAuthenticated(): boolean

// Email verification
sendVerificationEmail(): Promise<void>
verifyEmail(token: string): Promise<void>

// Password reset
forgotPassword(email: string): Promise<void>
resetPassword(token: string, newPassword: string): Promise<void>
```

**Mock Mode Support:**
- Service hỗ trợ mock mode cho development (VITE_USE_MOCK_DATA=true)
- Không cần backend để test UI/UX
- Mock data realistic với delays

### 3. Custom Hooks Layer (`/lib/hooks/`)

**File: `/lib/hooks/useAuth.ts`**

React hook để quản lý authentication state:

```typescript
const {
  user,              // UserDto | null
  isAuthenticated,   // boolean
  isLoading,         // boolean
  error,             // string | null
  login,             // (request: LoginRequest) => Promise<void>
  loginWithGoogle,   // (idToken: string) => Promise<void>
  register,          // (request: RegisterRequest) => Promise<void>
  logout,            // () => Promise<void>
  updateProfile,     // (request: UpdateProfileRequest) => Promise<void>
  changePassword,    // (current, new) => Promise<void>
  refreshUser,       // () => Promise<void>
} = useAuth();
```

**Features:**
- Auto-load user on mount
- Toast notifications với Sonner
- Error handling tập trung
- State management với useState

### 4. UI Components (`/pages/auth/` và `/components/auth/`)

#### `/pages/auth/LoginPage.tsx`
- Form đăng nhập với validation
- Remember me checkbox
- Forgot password link
- Google login button
- Link đến register page
- Thiết kế văn hóa Việt (màu nâu ấm, vàng, đỏ nghi lễ)

#### `/pages/auth/RegisterPage.tsx`
- Form đăng ký với validation chi tiết
- Password strength indicator
- Confirm password validation
- Google sign up button
- Success screen sau khi đăng ký
- Link đến login page

#### `/components/auth/GoogleLoginButton.tsx`
- Component tái sử dụng cho Google OAuth
- Hỗ trợ cả login và sign up mode
- Mock mode cho development
- Hướng dẫn tích hợp thực tế

---

## 🚀 Cách Sử Dụng

### 1. Setup Environment Variables

Tạo file `.env` từ `.env.example`:

```bash
cp .env.example .env
```

Cấu hình `.env`:

```env
# API Backend URL
VITE_API_URL=http://localhost:5000/api/v1

# Development mode (sử dụng mock data)
VITE_USE_MOCK_DATA=true

# Google OAuth (optional)
# VITE_GOOGLE_CLIENT_ID=your_google_client_id_here
```

### 2. Navigation từ App.tsx

```typescript
// Chuyển đến trang login
onNavigate('login')

// Chuyển đến trang register
onNavigate('register')

// Chuyển đến trang forgot password (cần implement)
onNavigate('forgot-password')
```

### 3. Sử dụng Authentication trong Components

```typescript
import { useAuth } from '../lib/hooks/useAuth';

function MyComponent() {
  const { user, isAuthenticated, login, logout } = useAuth();

  // Kiểm tra đã đăng nhập chưa
  if (!isAuthenticated) {
    return <div>Vui lòng đăng nhập</div>;
  }

  return (
    <div>
      <h1>Xin chào, {user?.fullName}</h1>
      <button onClick={logout}>Đăng xuất</button>
    </div>
  );
}
```

### 4. Protected Routes Pattern

```typescript
function ProtectedRoute({ children, requiredRole }: ProtectedRouteProps) {
  const { user, isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return <LoadingSpinner />;
  }

  if (!isAuthenticated) {
    // Chuyển hướng đến login
    onNavigate('login');
    return null;
  }

  if (requiredRole && user?.role !== requiredRole) {
    return <div>Bạn không có quyền truy cập</div>;
  }

  return <>{children}</>;
}
```

### 5. Form Validation với React Hook Form (Optional)

Để validation mạnh mẽ hơn, có thể tích hợp react-hook-form:

```typescript
import { useForm } from 'react-hook-form@7.55.0';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';

const loginSchema = z.object({
  email: z.string().email('Email không hợp lệ'),
  password: z.string().min(6, 'Mật khẩu phải có ít nhất 6 ký tự'),
});

function LoginForm() {
  const { login } = useAuth();
  const { register, handleSubmit, formState: { errors } } = useForm({
    resolver: zodResolver(loginSchema)
  });

  const onSubmit = async (data) => {
    await login(data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <input {...register('email')} />
      {errors.email && <span>{errors.email.message}</span>}
      
      <input type="password" {...register('password')} />
      {errors.password && <span>{errors.password.message}</span>}
      
      <button type="submit">Đăng nhập</button>
    </form>
  );
}
```

---

## 🔌 Tích Hợp Backend (.NET Core 8)

### Required Endpoints

Backend cần implement các endpoints sau:

#### 1. Authentication Endpoints

```csharp
// POST /api/v1/auth/login
[HttpPost("login")]
public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)

// POST /api/v1/auth/google-login
[HttpPost("google-login")]
public async Task<ActionResult<LoginResponse>> GoogleLogin([FromBody] GoogleLoginRequest request)

// POST /api/v1/auth/register
[HttpPost("register")]
public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)

// POST /api/v1/auth/logout
[HttpPost("logout")]
[Authorize]
public async Task<IActionResult> Logout()

// POST /api/v1/auth/refresh-token
[HttpPost("refresh-token")]
public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)

// GET /api/v1/auth/me
[HttpGet("me")]
[Authorize]
public async Task<ActionResult<UserDto>> GetCurrentUser()

// PUT /api/v1/auth/profile
[HttpPut("profile")]
[Authorize]
public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateProfileRequest request)

// POST /api/v1/auth/change-password
[HttpPost("change-password")]
[Authorize]
public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)

// POST /api/v1/auth/forgot-password
[HttpPost("forgot-password")]
public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)

// POST /api/v1/auth/reset-password
[HttpPost("reset-password")]
public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)

// POST /api/v1/auth/send-verification-email
[HttpPost("send-verification-email")]
[Authorize]
public async Task<IActionResult> SendVerificationEmail()

// POST /api/v1/auth/verify-email
[HttpPost("verify-email")]
public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
```

### DTOs Example (C#)

```csharp
public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    public bool RememberMe { get; set; }
}

public class GoogleLoginRequest
{
    [Required]
    public string IdToken { get; set; }
    
    public string AccessToken { get; set; }
}

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    
    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
    
    [Required]
    public string FullName { get; set; }
    
    [Phone]
    public string PhoneNumber { get; set; }
}

public class LoginResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
    public UserDto User { get; set; }
}

public class UserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Avatar { get; set; }
    public UserRole Role { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public enum UserRole
{
    Customer,
    Driver,
    Admin,
    SpiritualAdvisor
}
```

### JWT Configuration (.NET Core)

```csharp
// appsettings.json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-here-min-32-chars",
    "Issuer": "YourAppName",
    "Audience": "YourAppName",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 30
  }
}

// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])
            )
        };
    });

builder.Services.AddAuthorization();
```

### Google OAuth Backend Verification

```csharp
using Google.Apis.Auth;

public async Task<LoginResponse> GoogleLogin(GoogleLoginRequest request)
{
    try
    {
        // Verify Google ID token
        var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
        
        // Check if user exists
        var user = await _userRepository.GetByEmailAsync(payload.Email);
        
        if (user == null)
        {
            // Create new user from Google data
            user = new User
            {
                Email = payload.Email,
                FullName = payload.Name,
                Avatar = payload.Picture,
                IsEmailConfirmed = true,
                Role = UserRole.Customer,
                AuthProvider = "Google"
            };
            
            await _userRepository.CreateAsync(user);
        }
        
        // Generate JWT tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user);
        
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            User = _mapper.Map<UserDto>(user)
        };
    }
    catch (Exception ex)
    {
        throw new UnauthorizedException("Invalid Google token");
    }
}
```

---

## 🔐 Google OAuth Setup

### 1. Lấy Google Client ID

1. Truy cập [Google Cloud Console](https://console.cloud.google.com/)
2. Tạo hoặc chọn project
3. Enable Google+ API:
   - Vào "APIs & Services" > "Library"
   - Tìm "Google+ API" và enable
4. Tạo OAuth 2.0 Client ID:
   - Vào "APIs & Services" > "Credentials"
   - Click "Create Credentials" > "OAuth 2.0 Client ID"
   - Chọn "Web application"
   - Điền thông tin:
     * Name: Your App Name
     * Authorized JavaScript origins:
       - `http://localhost:5173` (development)
       - `https://your-domain.com` (production)
     * Authorized redirect URIs:
       - `http://localhost:5173` (development)
       - `https://your-domain.com` (production)
5. Copy Client ID

### 2. Cài Đặt Google OAuth Library

```bash
npm install @react-oauth/google
```

### 3. Wrap App với GoogleOAuthProvider

Cập nhật `/App.tsx`:

```typescript
import { GoogleOAuthProvider } from '@react-oauth/google';

export default function App() {
  const googleClientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;

  return (
    <GoogleOAuthProvider clientId={googleClientId}>
      <QueryClientProvider client={queryClient}>
        <AppContent />
        <Toaster position="top-right" richColors />
        <ReactQueryDevtools initialIsOpen={false} />
      </QueryClientProvider>
    </GoogleOAuthProvider>
  );
}
```

### 4. Sử dụng useGoogleLogin Hook

Cập nhật `/components/auth/GoogleLoginButton.tsx`:

```typescript
import { useGoogleLogin } from '@react-oauth/google';

export const GoogleLoginButton = ({ onNavigate, isSignUp = false }) => {
  const { loginWithGoogle } = useAuth();
  const [isLoading, setIsLoading] = useState(false);

  const login = useGoogleLogin({
    onSuccess: async (tokenResponse) => {
      try {
        setIsLoading(true);
        await loginWithGoogle(tokenResponse.access_token);
        onNavigate('home');
      } catch (error) {
        console.error('Google login error:', error);
      } finally {
        setIsLoading(false);
      }
    },
    onError: (error) => {
      console.error('Google login error:', error);
      toast.error('Đăng nhập Google thất bại');
    },
  });

  return (
    <Button onClick={() => login()} disabled={isLoading}>
      {/* ... */}
    </Button>
  );
};
```

---

## 📊 Testing

### 1. Mock Mode Testing

Set `VITE_USE_MOCK_DATA=true` trong `.env`:

```bash
# Test login
Email: test@example.com
Password: (bất kỳ - sẽ mock success)

# Test register
Điền form đầy đủ - sẽ mock success
```

### 2. Real API Testing

Set `VITE_USE_MOCK_DATA=false` và đảm bảo backend đang chạy:

```bash
# Start backend
cd YourBackend
dotnet run

# Start frontend
npm run dev
```

### 3. Test Cases

- ✅ Login với email/password hợp lệ
- ✅ Login với credentials không hợp lệ (hiển thị error)
- ✅ Register với thông tin hợp lệ
- ✅ Register với email đã tồn tại (hiển thị error)
- ✅ Validation errors (email không hợp lệ, password quá ngắn, etc.)
- ✅ Google login flow
- ✅ Auto refresh token khi expired
- ✅ Logout và clear tokens
- ✅ Protected routes redirect đến login
- ✅ Remember me functionality
- ✅ Password strength validation

---

## 🎨 UI/UX Features

### Thiết Kế Văn Hóa Việt Nam

- **Màu sắc:**
  - Nâu ấm (#92400E) - Màu chủ đạo, tượng trưng đất Việt
  - Vàng (#F59E0B) - Màu vàng kim, may mắn thịnh vượng
  - Đỏ nghi lễ (#DC2626) - Màu đỏ truyền thống, tài lộc
  - Nền kem (#FFFBEB) - Nền sáng, thanh tịnh

- **Họa tiết:**
  - Icon hoa sen (Flower2 từ Lucide)
  - Icon ngọn lửa hương (Flame từ Lucide)
  - Background decorative elements

- **Typography:**
  - Font chữ tiếng Việt dễ đọc
  - Các trường bắt buộc đánh dấu sao đỏ (*)
  - Labels và messages đều bằng tiếng Việt

### Form Validation

- Realtime validation khi user typing
- Error messages rõ ràng bằng tiếng Việt
- Visual feedback (border đỏ khi error)
- Password strength indicator
- Confirm password matching

### Loading States

- Loading spinner khi submit form
- Disabled state cho buttons và inputs
- Skeleton loaders cho protected pages

### Toast Notifications

- Success: "Đăng nhập thành công!"
- Error: Hiển thị error message từ backend
- Auto dismiss sau 3-5 giây

---

## 🔒 Security Best Practices

### Frontend

1. **Never store sensitive data in localStorage**
   - Chỉ lưu JWT tokens (accessToken, refreshToken)
   - Không lưu password

2. **HTTPS only in production**
   - JWT tokens chỉ nên gửi qua HTTPS

3. **XSS Protection**
   - React tự động escape outputs
   - Không sử dụng `dangerouslySetInnerHTML` với user input

4. **CSRF Protection**
   - Backend sử dụng JWT instead of cookies
   - Hoặc implement CSRF tokens nếu dùng cookies

### Backend

1. **Password Hashing**
   ```csharp
   using Microsoft.AspNetCore.Identity;
   
   var hashedPassword = _passwordHasher.HashPassword(user, password);
   ```

2. **JWT Best Practices**
   - Short-lived access tokens (15-60 minutes)
   - Long-lived refresh tokens (7-30 days)
   - Rotate refresh tokens on use
   - Store refresh tokens in database với user_id

3. **Rate Limiting**
   ```csharp
   services.AddRateLimiter(options => {
       options.AddFixedWindowLimiter("auth", opt => {
           opt.Window = TimeSpan.FromMinutes(1);
           opt.PermitLimit = 5;
       });
   });
   ```

4. **Email Verification**
   - Gửi email verification sau register
   - Không cho phép login nếu email chưa verify (tùy business logic)

---

## 📁 File Structure Summary

```
├── .env.example                          # Environment variables template
├── AUTH_INTEGRATION_GUIDE.md            # This file
├── /lib
│   ├── /api
│   │   ├── client.ts                    # Axios client, interceptors, token mgmt
│   │   ├── types.ts                     # TypeScript types/interfaces
│   │   └── errors.ts                    # Error handling utilities
│   ├── /services
│   │   ├── authService.ts               # Auth API calls, Google login
│   │   ├── userService.ts               # User management
│   │   ├── productService.ts            # Product API
│   │   ├── orderService.ts              # Order API
│   │   ├── cartService.ts               # Cart API
│   │   ├── checkoutService.ts           # Checkout API
│   │   └── wishlistService.ts           # Wishlist API
│   └── /hooks
│       ├── useAuth.ts                   # Auth state management hook (updated)
│       ├── useUser.ts                   # User management hook
│       ├── useProducts.ts               # Products hook
│       ├── useOrders.ts                 # Orders hook
│       ├── useCart.ts                   # Cart hook
│       ├── useCheckout.ts               # Checkout hook
│       └── useWishlist.ts               # Wishlist hook
├── /pages
│   └── /auth
│       ├── LoginPage.tsx                # Login page component (NEW)
│       └── RegisterPage.tsx             # Register page component (NEW)
├── /components
│   └── /auth
│       └── GoogleLoginButton.tsx        # Google OAuth button (NEW)
└── App.tsx                              # Main app (updated with auth routes)
```

---

## 🚧 TODO: Features Chưa Implement

### 1. Forgot Password Flow

**File cần tạo:** `/pages/auth/ForgotPasswordPage.tsx`

```typescript
// Flow:
// 1. User nhập email
// 2. Backend gửi email với reset token
// 3. User click link trong email → đến ResetPasswordPage
// 4. User nhập password mới
// 5. Backend verify token và update password
```

**Endpoints:**
- POST `/api/v1/auth/forgot-password` - Gửi email
- POST `/api/v1/auth/reset-password` - Reset password

### 2. Email Verification Flow

**File cần tạo:** `/pages/auth/VerifyEmailPage.tsx`

```typescript
// Flow:
// 1. Sau register, backend gửi email verification
// 2. User click link trong email → đến VerifyEmailPage
// 3. Frontend gọi API verify với token từ URL
// 4. Hiển thị success/error message
```

**Endpoints:**
- POST `/api/v1/auth/send-verification-email` - Gửi lại email
- POST `/api/v1/auth/verify-email` - Verify token

### 3. Social Login khác

- Facebook Login
- Apple Login
- Zalo Login (phổ biến ở Việt Nam)

### 4. Two-Factor Authentication (2FA)

- SMS OTP
- Authenticator app (Google Authenticator, etc.)

### 5. Session Management

- Hiển thị danh sách devices/sessions đang active
- Logout từ tất cả devices
- Logout từ device khác

---

## 🐛 Troubleshooting

### Issue 1: "Network Error" khi call API

**Solution:**
```bash
# Check backend đang chạy
curl http://localhost:5000/api/v1/health

# Check CORS settings trong backend
services.AddCors(options => {
    options.AddPolicy("AllowFrontend", builder => {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
```

### Issue 2: Token refresh loop

**Solution:**
- Check refresh token endpoint không yêu cầu valid access token
- Đảm bảo không có race condition trong interceptor

### Issue 3: Google Login không hoạt động

**Solution:**
```bash
# Check Google Client ID
echo $VITE_GOOGLE_CLIENT_ID

# Check authorized origins trong Google Console
# Check network tab để xem request/response
```

---

## 📞 Support

Nếu có vấn đề hoặc câu hỏi:

1. Xem lại tài liệu này
2. Check console logs (browser devtools)
3. Check network tab để xem API responses
4. Verify environment variables
5. Test với mock mode trước

---

## 📝 Changelog

### v1.0.0 (2025-10-31)
- ✅ Initial authentication system setup
- ✅ Login/Register pages với thiết kế văn hóa Việt
- ✅ Google OAuth integration (mock + real)
- ✅ JWT token management với auto-refresh
- ✅ Error handling và validation
- ✅ Toast notifications
- ✅ useAuth custom hook
- ✅ Mock mode support cho development

---

**Happy Coding! 🎉**

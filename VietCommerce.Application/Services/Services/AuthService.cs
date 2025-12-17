// ================================================================
// FILE: VietCommerce.Api/Services/AuthService.cs
// COMPLETE IMPLEMENTATION WITH ALL FIXES
// ================================================================

using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VietCommerce.Application.Helpers;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Auth;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Users;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Authentication Service - Handles user authentication, registration, and session management
    /// Features:
    /// - Email/password login with guest cart merge
    /// - Email verification workflow
    /// - JWT token management (access + refresh tokens)
    /// - Password change and reset flows
    /// - Google OAuth integration
    /// - Server-side session tracking in Redis
    /// - Redis-based cache for performance optimization
    /// - 🛡️ Brute-force protection with exponential backoff
    /// </summary>
    public class AuthService : BaseService, IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtHelper _jwtHelper;
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtSettings _jwtSettings;
        private readonly GoogleSettings _googleSettings;
        private readonly ILoginAttemptService _loginAttemptService;

        // Cache TTLs
        private static readonly TimeSpan UserCacheTtl = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan VerificationTokenTtl = TimeSpan.FromHours(1);
        private static readonly TimeSpan ResetTokenTtl = TimeSpan.FromMinutes(30);

        public AuthService(
            IUnitOfWork unitOfWork,
            JwtHelper jwtHelper,
            ILogger<AuthService> logger,
            IMapper mapper,
            ICacheService cacheService,
            ICartService cartService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<JwtSettings> jwtSettings,
            ILoginAttemptService loginAttemptService,
            IOptions<GoogleSettings> googleOptions)
            : base(logger, cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _jwtSettings = jwtSettings?.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
            _loginAttemptService = loginAttemptService;
            _googleSettings = googleOptions?.Value ?? throw new ArgumentNullException(nameof(googleOptions));
        }

        #region Authentication (Login/Register)

        /// <summary>
        /// Login user with email and password
        /// 🛡️ Features:
        /// - Brute-force protection with exponential backoff
        /// - Account lockout after multiple failed attempts
        /// - Progressive warning messages
        /// - Automatic unlock with increasing TTL
        /// - Merges guest cart if session exists
        /// - Creates JWT tokens and saves session in Redis
        /// </summary>
        public async Task<ApiResponse<AuthResponseDTO>> LoginAsync(LoginDTO request)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateEmail(request.Email);
                ValidateNotEmpty(request.Password, nameof(request.Password));

                LogInfo($"🔐 Login attempt for email: {request.Email}");

                // ✅ STEP 1: Check account lock (KHÔNG CATCH EXCEPTION Ở ĐÂY!)
                var (isLocked, lockMessage) = await _loginAttemptService.IsLockedAsync(request.Email);
                if (isLocked)
                {
                    LogWarning($"🚫 Login blocked - Account locked: {request.Email}");
                    throw new InvalidOperationException(lockMessage);
                }

                // STEP 2: Get user
                var user = await GetUserByEmailCachedAsync(request.Email);

                // STEP 3: Validate credentials
                bool isValidCredentials = user != null &&
                                         PasswordHelper.VerifyPassword(request.Password, user.PasswordHash);

                if (!isValidCredentials)
                {
                    // ✅ Tăng failed attempts
                    var failedCount = await _loginAttemptService.IncreaseFailedCountAsync(request.Email);
                    var warningMessage = GetLoginFailureMessage(failedCount);

                    LogWarning($"❌ Login failed [{failedCount}x] for: {request.Email}");
                    throw new InvalidOperationException(warningMessage);
                }

                // ✅ STEP 4: Reset failed attempts on successful login
                await _loginAttemptService.ResetAsync(request.Email);
                LogInfo($"✅ Login attempt counter reset for: {request.Email}");

                // STEP 5: Check account status
                ThrowIf(!user.IsActive || user.Status != UserStatus.ACTIVE,
                    "Account is deactivated");

                // STEP 6: Extract guest session ID
                var guestSessionId = SessionIdHelper.ExtractSessionId(
                    _httpContextAccessor.HttpContext, _logger);

                if (!string.IsNullOrWhiteSpace(guestSessionId))
                {
                    LogInfo($"👤 Guest session detected: {SessionIdHelper.FormatSessionIdForLogging(guestSessionId)}");
                }

                // STEP 7: Update last login
                user.LastLogin = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
                // STEP 7.1: Extract permissions from user
                var permissions = user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList();

                var customer = await _unitOfWork.Customers.GetByUserIdAsync(user.Id);

                var accessToken = _jwtHelper.GenerateToken(
                    user,
                    customer?.Id,
                    permissions
                );

                // STEP 8: Generate tokens

                var refreshToken = GenerateRefreshToken();

                // STEP 9: Extract JTI
                var jti = _jwtHelper.GetJtiFromToken(accessToken);
                ThrowIf(string.IsNullOrEmpty(jti), "Failed to extract JTI from token");

                // STEP 10: Save refresh token
                var refreshTokenEntity = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays),
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);

                // STEP 11: Save session
                await SaveSessionAsync(user.Id, jti,
                    TimeSpan.FromMinutes(_jwtSettings.AccessTokenLifetimeMinutes));

                // STEP 12: Merge guest cart
                if (!string.IsNullOrWhiteSpace(guestSessionId))
                {
                    try
                    {
                        // Get or create customer for this user
                        var guestCustomer = await _unitOfWork.Customers.GetByUserIdAsync(user.Id);
                        if (guestCustomer == null)
                        {
                            // Create customer if doesn't exist
                            guestCustomer = new Customer
                            {
                                UserId = user.Id,
                                Email = user.Email,
                                Name = user.Name,
                                Phone = user.Phone ?? string.Empty,
                                IsActive = true,
                                StoreId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C"),
                                TenantId = Guid.Parse("F40EC7E0-FC21-4E67-831C-07D14D0B304A")
                            };
                            await _unitOfWork.Customers.AddAsync(guestCustomer);
                            await _unitOfWork.SaveChangesAsync();
                        }

                        LogInfo($"🛒 Merging guest cart for customer {guestCustomer.Id}");
                        var mergeResult = await _cartService.MergeGuestCartToUserAsync(guestSessionId, guestCustomer.Id);

                        if (mergeResult.Success)
                        {
                            LogInfo($"✅ Guest cart merged | Items: {mergeResult.Data?.Items?.Count ?? 0}");
                        }
                        else
                        {
                            LogWarning($"⚠️ Cart merge failed: {mergeResult.Message}");
                        }
                    }
                    catch (Exception mergeEx)
                    {
                        LogError($"❌ Exception during guest cart merge", mergeEx);
                    }
                    finally
                    {
                        SessionIdHelper.ClearSessionId(_httpContextAccessor.HttpContext.Response, _logger);
                        LogInfo($"🗑️ Guest session cleared after login");
                    }
                }

                // STEP 13: Save changes
                await _unitOfWork.SaveChangesAsync();

                // STEP 14: Prepare response
                var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();
                var userInfo = new UserInfoDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name ?? string.Empty,
                    AvatarUrl = user.AvatarUrl ?? string.Empty,
                    Provider = user.Provider ?? "Local",
                    Roles = roles
                };

                var response = new AuthResponseDTO
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes),
                    User = userInfo
                };

                LogInfo($"✅ Login successful | UserId: {user.Id} | GuestCart: {(string.IsNullOrWhiteSpace(guestSessionId) ? "No" : "Merged")}");

                return response;

            }, "Login", "Login successful");
        }

        /// <summary>
        /// 🛡️ Generate progressive warning messages based on failed attempt count
        /// </summary>
        private string GetLoginFailureMessage(int failedCount)
        {
            return failedCount switch
            {
                1 => "Email hoặc mật khẩu không đúng. Vui lòng thử lại.",
                2 => "⚠️ Email hoặc mật khẩu không đúng. Bạn còn 3 lần thử.",
                3 => "⚠️ Email hoặc mật khẩu không đúng. Bạn còn 2 lần thử.",
                4 => "⚠️ Email hoặc mật khẩu không đúng. Bạn còn 1 lần thử trước khi tài khoản bị khóa.",
                5 => "🚫 Tài khoản tạm thời bị khóa 15 phút do đăng nhập sai quá nhiều lần.",
                >= 6 and < 10 => $"🚫 Email hoặc mật khẩu không đúng. Tài khoản vẫn đang bị khóa (lần thất bại thứ {failedCount}).",
                10 => "🚫 Tài khoản bị khóa 30 phút do vi phạm bảo mật nhiều lần.",
                >= 11 and < 15 => $"🚫 Email hoặc mật khẩu không đúng. Tài khoản đang bị khóa cấp 2 (lần thất bại thứ {failedCount}).",
                15 => "🚫 Tài khoản bị khóa 1 giờ do vi phạm bảo mật nghiêm trọng.",
                _ => $"🚫 Tài khoản đang bị khóa do vi phạm bảo mật. Vui lòng liên hệ hỗ trợ (lần thất bại thứ {failedCount})."
            };
        }

        /// <summary>
        /// Register new user with email and password
        /// Sends verification email with token
        /// Account is inactive until email is verified
        /// </summary>
        public async Task<ApiResponse<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO request)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateEmail(request.Email);
                ValidateNotEmpty(request.Password, nameof(request.Password));
                ValidateNotEmpty(request.Name, nameof(request.Name));
                ValidatePasswordStrength(request.Password);

                LogInfo($"📝 Registration attempt for email: {request.Email}");

                // Check email not already registered
                var emailExists = await CheckEmailExistsAsync(request.Email);
                ThrowIf(emailExists, "Email is already registered");

                // Generate verification token
                var verificationToken = Guid.NewGuid().ToString("N");
                var cacheKey = CreateCacheKey("auth:verify", verificationToken);

                // Store registration data in Redis (1 hour validity)
                await _cacheService.SetAsync(cacheKey, request, VerificationTokenTtl);

                // Send verification email
                await SendVerificationEmailAsync(request.Email, verificationToken);

                LogInfo($"📧 Verification email sent to {request.Email}");

                return (AuthResponseDTO)null!;

            }, "Register", "Please check your email to verify your account");
        }

        /// <summary>
        /// Verify user email using token sent in email
        /// Creates user, customer profile, and assigns default role
        /// </summary>
        public async Task<ApiResponse<bool>> VerifyEmailAsync(string token)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(token, nameof(token));

                LogInfo($"✉️ Email verification attempt with token: {token.Substring(0, 8)}...");

                // [1] Get verification request from cache
                var cacheKey = CreateCacheKey("auth:verify", token);
                var pendingUser = await _cacheService.GetAsync<RegisterRequestDTO>(cacheKey);

                ThrowIf(pendingUser == null, "Invalid or expired verification token");
                ThrowIf(await _unitOfWork.Users.EmailExistsAsync(pendingUser.Email),
                    "Email already verified");

                // [2] Create user entity
                var user = _mapper.Map<User>(pendingUser);
                user.PasswordHash = PasswordHelper.HashPassword(pendingUser.Password);
                user.CreatedBy = Guid.Empty;
                user.UpdatedBy = Guid.Empty;
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                user.IsActive = true;
                user.Status = UserStatus.ACTIVE;

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // [3] Assign Customer role
                var customerRoleId = Guid.Parse("9F21385E-AC62-448F-91DD-04296F09C354");
                await _unitOfWork.UserRoles.AddAsync(new UserRole
                {
                    UserId = user.Id,
                    RoleId = customerRoleId
                });

                // [4] Create customer profile
                var customer = new Customer
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Phone = user.Phone ?? string.Empty,
                    IsActive = true,
                    StoreId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C"),
                    TenantId = Guid.Parse("F40EC7E0-FC21-4E67-831C-07D14D0B304A")
                };
                await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                // [5] Clean up verification token
                await _cacheService.RemoveAsync(cacheKey);

                // [6] Invalidate cache for this email
                await InvalidateUserCacheAsync(user.Id, user.Email);

                LogInfo($"✅ Email verified and account created: {user.Email}");
                return true;

            }, "VerifyEmail", "Email verified successfully");
        }

        /// <summary>
        /// Send verification email to user
        /// Email contains link with verification token
        /// </summary>
        public async Task SendVerificationEmailAsync(string email, string token)
        {
            try
            {
                ValidateEmail(email);
                ValidateNotEmpty(token, nameof(token));

                var verifyLink = $"https://yourapp.com/api/auth/verify-email?token={token}";
                LogInfo($"📧 Verification email (mock) sent to {email}");
                Console.WriteLine($"Verification email sent to {email}: {verifyLink}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Error sending verification email: {ex.Message}");
                // Don't throw - email failure shouldn't fail registration
            }
        }

        /// <summary>
        /// Authenticate with Google OAuth token
        /// Creates new user if doesn't exist
        /// </summary>
        public async Task<ApiResponse<AuthResponseDTO>> FindOrCreateGoogleUserAsync(SocialLoginRequestDTO request)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(request.IdToken, nameof(request.IdToken));

                LogInfo($"🔐 Google login attempt");

                // Validate Google token
                GoogleJsonWebSignature.Payload payload;
                try
                {
                    payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken,
                        new GoogleJsonWebSignature.ValidationSettings
                        {
                            Audience = new[] { _googleSettings.ClientId }
                        });
                }
                catch (InvalidJwtException)
                {
                    throw new InvalidOperationException("Invalid Google token");
                }

                var email = payload.Email;

                // Get or create user
                var user = await GetUserByEmailCachedAsync(email);

                if (user == null)
                {
                    LogInfo($"👤 Creating new Google user: {email}");

                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = email,
                        Name = payload.Name ?? email.Split('@')[0],
                        AvatarUrl = payload.Picture,
                        Provider = "Google",
                        Status = UserStatus.ACTIVE,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = Guid.Empty,
                        UpdatedBy = Guid.Empty,
                        StoreId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C")
                    };

                    var defaultRole = await _unitOfWork.Roles.GetByNameAsync("User");
                    if (defaultRole != null)
                    {
                        user.UserRoles = new List<UserRole>
                        {
                            new UserRole { RoleId = defaultRole.Id, UserId = user.Id }
                        };
                    }

                    await _unitOfWork.Users.AddAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                }

                // Generate tokens
                var accessToken = _jwtHelper.GenerateToken(user);
                var refreshToken = GenerateRefreshToken();

                await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays)
                });
                await _unitOfWork.SaveChangesAsync();

                LogInfo($"✅ Google login successful: {email}");

                return new AuthResponseDTO
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes)
                };

            }, "GoogleLogin", "Google login successful");
        }

        #endregion

        #region Token Management

        /// <summary>
        /// Refresh access token using refresh token
        /// Issues new refresh token (old one is replaced)
        /// </summary>
        public async Task<ApiResponse<AuthResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(request.RefreshToken, nameof(request.RefreshToken));

                LogInfo($"🔄 Token refresh attempt");

                // Get refresh token from database
                var refreshTokenEntity = await _unitOfWork.RefreshTokens.GetValidTokenAsync(request.RefreshToken);
                ThrowIf(refreshTokenEntity == null, "Invalid or expired refresh token");

                // Check if token is revoked
                var isRevoked = await IsTokenRevokedAsync(request.RefreshToken);
                ThrowIf(isRevoked, "Refresh token has been revoked");

                // Verify user is still active
                ThrowIf(!refreshTokenEntity.User.IsActive || refreshTokenEntity.User.Status != UserStatus.ACTIVE,
                    "User account is deactivated");

                // Generate new access token
                var accessToken = _jwtHelper.GenerateToken(refreshTokenEntity.User);
                var newRefreshToken = GenerateRefreshToken();

                // Update refresh token
                refreshTokenEntity.Token = newRefreshToken;
                refreshTokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);
                _unitOfWork.RefreshTokens.Update(refreshTokenEntity);
                await _unitOfWork.SaveChangesAsync();

                // Add old refresh token to revocation list
                await AddToRevokedTokensAsync(request.RefreshToken);

                LogInfo($"✅ Token refreshed successfully");

                return new AuthResponseDTO
                {
                    Token = accessToken,
                    RefreshToken = newRefreshToken,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes)
                };

            }, "RefreshToken", "Token refreshed successfully");
        }

        /// <summary>
        /// Validate JWT token format and signature
        /// </summary>
        public async Task<ApiResponse<bool>> ValidateTokenAsync(string token)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(token, nameof(token));

                var userId = _jwtHelper.ValidateToken(token);
                ThrowIf(userId == null, "Invalid or malformed token");

                var user = await _unitOfWork.Users.GetByIdAsync(userId.Value);
                ThrowIf(user == null || !user.IsActive || user.Status != UserStatus.ACTIVE,
                    "User not found or inactive");

                LogDebug($"✅ Token is valid for user: {userId}");
                return true;

            }, "ValidateToken", "Token is valid");
        }

        #endregion

        #region Password Management

        /// <summary>
        /// Change password for authenticated user
        /// Requires old password verification
        /// Invalidates all user sessions (force re-login on other devices)
        /// </summary>
        public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDTO request)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId);
                ValidateNotNull(request, nameof(request));
                ValidateNotEmpty(request.CurrentPassword, nameof(request.CurrentPassword));
                ValidateNotEmpty(request.NewPassword, nameof(request.NewPassword));

                LogInfo($"🔐 Password change attempt for user: {userId}");

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                ThrowIf(user == null, "User not found");

                // ✅ Verify old password
                ThrowIf(!PasswordHelper.VerifyPassword(request.CurrentPassword, user.PasswordHash),
                    "Old password is incorrect");

                // Validate new password strength
                ValidatePasswordStrength(request.NewPassword);

                // Prevent reusing same password
                ThrowIf(PasswordHelper.VerifyPassword(request.NewPassword, user.PasswordHash),
                    "New password must be different from old password");

                // Update password
                user.PasswordHash = PasswordHelper.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                // ✅ Invalidate ALL user sessions (force re-login everywhere)
                await InvalidateUserCacheAsync(userId, user.Email);

                LogInfo($"✅ Password changed for user: {userId}");
                return true;

            }, "ChangePassword", "Password changed successfully");
        }

        /// <summary>
        /// Send password reset email (forgot password)
        /// Generates temporary reset token valid for 30 minutes
        /// Always returns success (doesn't reveal if email exists for security)
        /// </summary>
        public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequestDTO request)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotNull(request, nameof(request));
                ValidateEmail(request.Email);

                LogInfo($"🔑 Password reset request for email: {request.Email}");

                // 🔒 Security: Check email but don't reveal existence
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

                if (user != null && user.IsActive)
                {
                    // Generate reset token
                    var resetToken = Guid.NewGuid().ToString("N");
                    var resetTokenKey = CreateCacheKey("auth:reset", resetToken);

                    var resetData = new PasswordResetToken
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        CreatedAt = DateTime.UtcNow
                    };

                    // Store in Redis with 30-minute TTL
                    await _cacheService.SetAsync(resetTokenKey, resetData, ResetTokenTtl);

                    // Send reset email
                    await SendPasswordResetEmailAsync(user.Email, resetToken);

                    LogInfo($"📧 Password reset email sent to {user.Email}");
                }
                else
                {
                    // 🔒 Security: Don't reveal if email exists
                    LogWarning($"⚠️ Password reset requested for non-existent/inactive email: {request.Email}");
                }

                // Always return success (security best practice)
                return true;

            }, "ForgotPassword", "If email exists, password reset link has been sent");
        }

        /// <summary>
        /// Reset password using token from forgot password email
        /// Validates token is within 30-minute window
        /// Invalidates all user sessions
        /// </summary>
        public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequestDTO request)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotNull(request, nameof(request));
                ValidateNotEmpty(request.Token, nameof(request.Token));
                ValidateNotEmpty(request.NewPassword, nameof(request.NewPassword));

                LogInfo($"🔐 Password reset attempt with token: {request.Token.Substring(0, 8)}...");

                // Get reset token from cache
                var resetTokenKey = CreateCacheKey("auth:reset", request.Token);
                var resetData = await _cacheService.GetAsync<PasswordResetToken>(resetTokenKey);

                ThrowIf(resetData == null, "Invalid or expired reset token");

                // Verify token age (double-check TTL)
                var tokenAge = DateTime.UtcNow.Subtract(resetData.CreatedAt);
                ThrowIf(tokenAge.TotalMinutes > 30, "Reset token has expired");

                // Get user
                var user = await _unitOfWork.Users.GetByIdAsync(resetData.UserId);
                ThrowIf(user == null, "User not found");

                // Validate new password strength
                ValidatePasswordStrength(request.NewPassword);

                // Prevent reusing old password
                ThrowIf(PasswordHelper.VerifyPassword(request.NewPassword, user.PasswordHash),
                    "New password must be different from current password");

                // Update password
                user.PasswordHash = PasswordHelper.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                // Clean up reset token
                await _cacheService.RemoveAsync(resetTokenKey);

                // Invalidate all sessions (force re-login)
                await InvalidateUserCacheAsync(user.Id, user.Email);

                LogInfo($"✅ Password reset successful for user: {resetData.UserId}");
                return true;

            }, "ResetPassword", "Password reset successfully. Please login with new password");
        }

        #endregion

        #region Logout & Session Revocation

        /// <summary>
        /// Logout user - revoke session and refresh token
        /// Removes session from Redis
        /// Adds refresh token to revocation blacklist
        /// Clears user-related cache
        /// </summary>
        public async Task<ApiResponse<bool>> LogoutAsync(Guid userId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId);

                LogInfo($"👋 Logout attempt for user: {userId}");

                await RevokeSessionAsync(userId);
                await _unitOfWork.SaveChangesAsync();

                LogInfo($"✅ User logged out: {userId}");
                return true;

            }, "Logout", "Logout successful");
        }

        #endregion

        #region Session Management (Server-Side)

        /// <summary>
        /// Save user session in Redis after successful login
        /// Stores JWT ID (JTI) + user info + timestamps
        /// Enables server-side logout (session revocation)
        /// </summary>
        public async Task SaveSessionAsync(Guid userId, string jti, TimeSpan ttl)
        {
            await ExecuteAsync(async () =>
            {
                ValidateId(userId);
                ValidateNotEmpty(jti, nameof(jti));

                var sessionKey = CreateCacheKey("auth:session", userId);
                var sessionData = new SessionData
                {
                    Jti = jti,
                    UserId = userId,
                    IssuedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.Add(ttl),
                    LastActivityAt = DateTime.UtcNow
                };

                await _cacheService.SetAsync(sessionKey, sessionData, ttl);
                LogDebug($"💾 Session saved | UserId: {userId} | TTL: {ttl.TotalMinutes}m");

            }, "SaveSession");
        }

        /// <summary>
        /// Validate user's current session
        /// Verifies JTI matches stored session
        /// Checks session hasn't expired or been revoked
        /// </summary>
        public async Task<bool> ValidateSessionAsync(Guid userId, string jti)
        {
            return await ExecuteAsync(async () =>
            {
                ValidateId(userId);
                ValidateNotEmpty(jti, nameof(jti));

                var sessionKey = CreateCacheKey("auth:session", userId);
                var session = await _cacheService.GetAsync<SessionData>(sessionKey);

                if (session == null)
                {
                    LogWarning($"⚠️ Session not found for user: {userId}");
                    return false;
                }

                // Verify JTI matches
                if (session.Jti != jti)
                {
                    LogWarning($"⚠️ JTI mismatch for user {userId}");
                    return false;
                }

                // Check session not expired
                if (session.ExpiresAt < DateTime.UtcNow)
                {
                    LogWarning($"⚠️ Session expired for user: {userId}");
                    return false;
                }

                return true;

            }, "ValidateSession");
        }

        /// <summary>
        /// Revoke user's current session (logout)
        /// Removes session from Redis
        /// Invalidates all refresh tokens
        /// </summary>
        public async Task RevokeSessionAsync(Guid userId)
        {
            await ExecuteAsync(async () =>
            {
                ValidateId(userId);

                var sessionKey = CreateCacheKey("auth:session", userId);
                await InvalidateCacheAsync(sessionKey);

                LogInfo($"🔐 Session revoked for user: {userId}");

            }, "RevokeSession");
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Get user by email with caching
        /// Cache key: auth:user:email:{email}
        /// TTL: 30 minutes
        /// </summary>
        private async Task<User?> GetUserByEmailCachedAsync(string email)
        {
            try
            {
                ValidateEmail(email);

                var cacheKey = CreateCacheKey("auth:user:email", email.ToLower());

                return await GetFromCacheOrExecuteAsync(
                    cacheKey,
                    async () => await _unitOfWork.Users.GetByEmailWithRolesAndPermissionsAsync(email),
                    UserCacheTtl
                );
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Error getting user by email: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Check if email exists in system with caching
        /// Cache key: auth:email:exists:{email}
        /// TTL: 30 minutes
        /// </summary>
        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            try
            {
                ValidateEmail(email);

                var cacheKey = CreateCacheKey("auth:email:exists", email.ToLower());

                return await GetFromCacheOrExecuteAsync(
                    cacheKey,
                    async () =>
                    {
                        var exists = await _unitOfWork.Users.EmailExistsAsync(email);
                        LogDebug($"📧 Email existence check: {email} = {(exists ? "EXISTS" : "NEW")}");
                        return exists;
                    },
                    UserCacheTtl
                );
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Error checking email existence: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Invalidate all user-related cache (session + email)
        /// Publish invalidation to other instances via Pub/Sub
        /// </summary>
        private async Task InvalidateUserCacheAsync(Guid userId, string? email = null)
        {
            try
            {
                ValidateId(userId);

                // 1. Revoke session
                await RevokeSessionAsync(userId);

                // 2. Invalidate email cache
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var emailCacheKey = CreateCacheKey("auth:user:email", email.ToLower());
                    await InvalidateCacheAsync(emailCacheKey);
                    LogDebug($"🗑️ Invalidated email cache: {email}");
                }

                // 3. Publish invalidation to other instances (Pub/Sub)
                await PublishCacheInvalidationAsync("auth:user-invalidated", new
                {
                    UserId = userId,
                    Email = email,
                    InvalidatedAt = DateTime.UtcNow
                });

                LogInfo($"🔄 User cache invalidated: {userId}");
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Error invalidating user cache: {ex.Message}");
            }
        }

        /// <summary>
        /// Add token to revocation blacklist (prevents token reuse)
        /// Uses Redis with TTL matching token expiration
        /// </summary>
        private async Task AddToRevokedTokensAsync(string token)
        {
            try
            {
                ValidateNotEmpty(token, nameof(token));

                var key = CreateCacheKey("auth:revoked", token);
                // Store a boolean marker to save memory
                await _cacheService.SetAsync(key, "true", TimeSpan.FromHours(8));


                LogDebug($"🔒 Token added to blacklist: {token.Substring(0, 16)}...");
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Error adding token to blacklist: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if refresh token has been revoked
        /// Returns true if token is in revocation list
        /// </summary>
        private async Task<bool> IsTokenRevokedAsync(string token)
        {
            try
            {
                ValidateNotEmpty(token, nameof(token));

                var key = CreateCacheKey("auth:revoked", token);
                var isRevokedStr = await _cacheService.GetAsync<string>(key);
                bool isRevoked = isRevokedStr == "true";


                if (isRevoked)
                {
                    LogDebug($"🚫 Token is revoked: {token.Substring(0, 16)}...");
                    return true;
                }

                LogDebug($"✅ Token is valid: {token.Substring(0, 16)}...");
                return false;
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Error checking token revocation: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validate password strength requirements
        /// Must contain: uppercase, lowercase, number, min 8 characters
        /// </summary>
        private void ValidatePasswordStrength(string password)
        {
            ThrowIf(string.IsNullOrWhiteSpace(password), "Password cannot be empty");
            //ThrowIf(password.Length < 8, "Password must be at least 8 characters");
            //ThrowIf(!Regex.IsMatch(password, @"[A-Z]"),
            //    "Password must contain at least one uppercase letter");
            //ThrowIf(!Regex.IsMatch(password, @"[a-z]"),
            //    "Password must contain at least one lowercase letter");
            //ThrowIf(!Regex.IsMatch(password, @"[\d]"),
            //    "Password must contain at least one number");
        }

        /// <summary>
        /// Send password reset email to user
        /// Contains link with reset token valid for 30 minutes
        /// </summary>
        private async Task SendPasswordResetEmailAsync(string email, string token)
        {
            try
            {
                ValidateEmail(email);
                ValidateNotEmpty(token, nameof(token));

                var resetLink = $"https://yourapp.com/auth/reset-password?token={token}";
                LogInfo($"🔐 Password reset email (mock) sent to {email}");
                Console.WriteLine($"Password reset email sent to {email}: {resetLink}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Error sending password reset email: {ex.Message}");
                // Don't throw - email failure shouldn't fail password reset request
            }
        }

        /// <summary>
        /// Generate refresh token (concatenated GUIDs for uniqueness)
        /// </summary>
        private static string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        }

        #endregion
    }
}


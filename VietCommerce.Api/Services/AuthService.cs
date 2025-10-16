using AutoMapper;
using Google.Apis.Auth;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Auth;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Users;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<AuthService> _logger;
        private readonly IMemoryCache _cache;
        private readonly JwtSettings _jwtSettings;
        private readonly GoogleSettings _googleSettings;
        private readonly IMapper _mapper;

        public AuthService(
            IUnitOfWork unitOfWork,
            JwtHelper jwtHelper,
            ILogger<AuthService> logger,
            IOptions<GoogleSettings> googleOptions,
            IMemoryCache cache,
            IMapper mapper,
            IOptions<JwtSettings> jwtSettings)
        {
            _unitOfWork = unitOfWork;
            _jwtHelper = jwtHelper;
            _logger = logger;
            _cache = cache;
            _googleSettings = googleOptions.Value;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
        }

        // 🔹 LOGIN
        public async Task<ApiResponse<AuthResponseDTO>> LoginAsync(LoginDTO request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailWithRolesAsync(request.Email);
                if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
                    return ApiResponse<AuthResponseDTO>.FailureResponse("Invalid email or password");

                if (!user.IsActive || user.Status != UserStatus.ACTIVE)
                    return ApiResponse<AuthResponseDTO>.FailureResponse("Account is deactivated");

                // ✅ Update last login
                user.LastLogin = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);

                // ✅ Generate tokens
                var accessToken = _jwtHelper.GenerateToken(user);
                var refreshToken = GenerateRefreshToken();

                // ✅ Save refresh token
                var refreshTokenEntity = new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays)
                };

                await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
                await _unitOfWork.SaveChangesAsync();

                var response = new AuthResponseDTO
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes)
                };

                _logger.LogInformation("User {Email} logged in successfully", request.Email);
                return ApiResponse<AuthResponseDTO>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email {Email}", request.Email);
                return ApiResponse<AuthResponseDTO>.FailureResponse("An error occurred during login");
            }
        }

        // 🔹 REGISTER
        public async Task<ApiResponse<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO request)
        {
            try
            {
                if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
                    return ApiResponse<AuthResponseDTO>.FailureResponse("Email is already registered");

                // ✅ Generate verification token
                var verificationToken = Guid.NewGuid().ToString();

                // ✅ Cache registration info (valid 1 hour)
                _cache.Set(verificationToken, request, TimeSpan.FromHours(1));

                // ✅ Send verification email
                await SendVerificationEmailAsync(request.Email, verificationToken);

                _logger.LogInformation("Verification email sent to {Email}", request.Email);
                return ApiResponse<AuthResponseDTO>.SuccessResponse(null!, "Please check your email to verify your account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email {Email}", request.Email);
                return ApiResponse<AuthResponseDTO>.FailureResponse("An error occurred during registration");
            }
        }

        // 🔹 MOCK EMAIL (placeholder)
        public Task SendVerificationEmailAsync(string email, string token)
        {
            var verificationLink = $"https://yourapp.com/api/auth/verify-email?token={token}";
            _logger.LogInformation("Mock Email Sent to {Email}: {Link}", email, verificationLink);
            Console.WriteLine($"📧 Verification email (mock) sent to {email}: {verificationLink}");
            return Task.CompletedTask;
        }

        // 🔹 REFRESH TOKEN
        public async Task<ApiResponse<AuthResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request)
        {
            try
            {
                var refreshToken = await _unitOfWork.RefreshTokens.GetValidTokenAsync(request.RefreshToken);
                if (refreshToken == null)
                    return ApiResponse<AuthResponseDTO>.FailureResponse("Invalid or expired refresh token");

                if (!refreshToken.User.IsActive || refreshToken.User.Status != UserStatus.ACTIVE)
                    return ApiResponse<AuthResponseDTO>.FailureResponse("Account is deactivated");

                var accessToken = _jwtHelper.GenerateToken(refreshToken.User);
                var newRefreshToken = GenerateRefreshToken();

                refreshToken.Token = newRefreshToken;
                refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);

                await _unitOfWork.SaveChangesAsync();

                var response = new AuthResponseDTO
                {
                    Token = accessToken,
                    RefreshToken = newRefreshToken,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes)
                };

                return ApiResponse<AuthResponseDTO>.SuccessResponse(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return ApiResponse<AuthResponseDTO>.FailureResponse("An error occurred during token refresh");
            }
        }

        // 🔹 LOGOUT
        public async Task<ApiResponse<bool>> LogoutAsync(Guid userId)
        {
            try
            {
                // await _unitOfWork.RefreshTokens.InvalidateTokensAsync(userId);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("User {UserId} logged out successfully", userId);
                return ApiResponse<bool>.SuccessResponse(true, "Logout successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", userId);
                return ApiResponse<bool>.FailureResponse("An error occurred during logout");
            }
        }

        // 🔹 CHANGE PASSWORD
        public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDTO request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<bool>.FailureResponse("User not found");

                if (!PasswordHelper.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                    return ApiResponse<bool>.FailureResponse("Current password is incorrect");

                user.PasswordHash = PasswordHelper.HashPassword(request.NewPassword);
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Password changed successfully for user {UserId}", userId);
                return ApiResponse<bool>.SuccessResponse(true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return ApiResponse<bool>.FailureResponse("An error occurred while changing password");
            }
        }

        // 🔹 FORGOT PASSWORD
        public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequestDTO request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (user == null)
                    return ApiResponse<bool>.SuccessResponse(true, "If the email exists, a password reset link has been sent");

                var resetToken = PasswordHelper.GenerateResetToken();
                _logger.LogInformation("Password reset requested for {Email}. Token: {Token}", request.Email, resetToken);
                return ApiResponse<bool>.SuccessResponse(true, "If the email exists, a password reset link has been sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for email {Email}", request.Email);
                return ApiResponse<bool>.FailureResponse("An error occurred while processing password reset request");
            }
        }

        // 🔹 RESET PASSWORD
        public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequestDTO request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (user == null)
                    return ApiResponse<bool>.FailureResponse("Invalid reset request");

                user.PasswordHash = PasswordHelper.HashPassword(request.NewPassword);
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Password reset successfully for user {Email}", request.Email);
                return ApiResponse<bool>.SuccessResponse(true, "Password reset successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for email {Email}", request.Email);
                return ApiResponse<bool>.FailureResponse("An error occurred during password reset");
            }
        }

        // 🔹 VALIDATE TOKEN
        public async Task<ApiResponse<bool>> ValidateTokenAsync(string token)
        {
            try
            {
                var userId = _jwtHelper.ValidateToken(token);
                if (userId == null)
                    return ApiResponse<bool>.FailureResponse("Invalid token");

                var user = await _unitOfWork.Users.GetByIdAsync(userId.Value);
                if (user == null || !user.IsActive || user.Status != UserStatus.ACTIVE)
                    return ApiResponse<bool>.FailureResponse("User not found or inactive");

                return ApiResponse<bool>.SuccessResponse(true, "Token is valid");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return ApiResponse<bool>.FailureResponse("Token validation failed");
            }
        }

        // 🔹 VERIFY EMAIL
        public async Task<ApiResponse<bool>> VerifyEmailAsync(string token)
        {
            try
            {
                if (!_cache.TryGetValue(token, out RegisterRequestDTO? pendingUser))
                    return ApiResponse<bool>.FailureResponse("Invalid or expired verification token");

                if (await _unitOfWork.Users.EmailExistsAsync(pendingUser.Email))
                    return ApiResponse<bool>.FailureResponse("Email already verified");

                var user = _mapper.Map<User>(pendingUser);
                user.PasswordHash = PasswordHelper.HashPassword(pendingUser.Password);

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                _cache.Remove(token);

                return ApiResponse<bool>.SuccessResponse(true, "Email verified successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email for token {Token}", token);
                return ApiResponse<bool>.FailureResponse("Error verifying email");
            }
        }

        // 🔹 Helper
        private static string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        }
        // 🔹 GOOGLE LOGIN
        public async Task<ApiResponse<AuthResponseDTO>> FindOrCreateGoogleUserAsync(SocialLoginRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.IdToken))
                    return ApiResponse<AuthResponseDTO>.FailureResponse("Google ID token is required");

                // 🔹 Validate idToken với Google
                GoogleJsonWebSignature.Payload payload;
                try
                {
                    payload = await GoogleJsonWebSignature.ValidateAsync(
                        request.IdToken,
                        new GoogleJsonWebSignature.ValidationSettings
                        {
                            Audience = new[] { _googleSettings.ClientId }
                        });
                }
                catch (InvalidJwtException)
                {
                    return ApiResponse<AuthResponseDTO>.FailureResponse("Invalid Google token");
                }

                // ✅ Lấy thông tin user từ payload
                var email = payload.Email;
                var name = payload.Name;
                var picture = payload.Picture;

                // ⚡ Ghi đè dữ liệu từ payload
                request.Email = email;
                request.Name = name;
                request.AvatarUrl = picture;

                var user = await _unitOfWork.Users.GetByEmailWithRolesAsync(email);
                if (user == null)
                {
                    user = _mapper.Map<User>(request);
                    user.Id = Guid.NewGuid();
                    user.Status = UserStatus.ACTIVE;
                    user.Provider = "Google";
                    user.CreatedAt = DateTime.UtcNow;
                    user.UpdatedAt = DateTime.UtcNow;

                    user.CreatedBy = Guid.Empty;
                    user.UpdatedBy = Guid.Empty;
                    user.StoreId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C");



                    // Gán role mặc định "User"
                    var defaultRole = await _unitOfWork.Roles.GetByNameAsync("User");
                    if (defaultRole != null)
                    {
                        user.UserRoles = new List<UserRole>
                    {
                        new UserRole { RoleId = defaultRole.Id, UserId = user.Id }
                    };
                    }
                    // Gán cửa hàng mặc định nếu cần
                    //var defaultStore = await _unitOfWork.Stores.FirstOrDefaultAsync();
                    //if (defaultStore != null)
                    //{
                    //    user.StoreId = defaultStore.Id;
                    //}

                    await _unitOfWork.Users.AddAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                }

                var accessToken = _jwtHelper.GenerateToken(user);
                var refreshToken = GenerateRefreshToken();

                await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays)
                });
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<AuthResponseDTO>.SuccessResponse(new AuthResponseDTO
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes)
                }, "Google login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google login for email {Email}", request.Email);
                return ApiResponse<AuthResponseDTO>.FailureResponse("An error occurred during Google login");
            }
        }

    }
}




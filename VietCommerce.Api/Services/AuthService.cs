using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.DTOs.Auth;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Users;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;
using VietCommerce.Api.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
namespace VietCommerce.Api.Services;
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtHelper _jwtHelper;
    private readonly ILogger<AuthService> _logger;
    private readonly IMemoryCache _cache;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;
    public AuthService(
        IUnitOfWork unitOfWork,
        JwtHelper jwtHelper,
        ILogger<AuthService> logger,
        IMemoryCache cache,
        IMapper mapper,
        IOptions<JwtSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _jwtHelper = jwtHelper;
        _logger = logger;
        _cache = cache;
        _mapper = mapper;
        _jwtSettings = jwtSettings.Value;
    }
    public async Task<ApiResponse<AuthResponseDTO>> LoginAsync(LoginDTO request)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailWithRolesAsync(request.Email);
            if (user == null || !PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
                return ApiResponse<AuthResponseDTO>.FailureResponse("Invalid email or password");
            if (!user.IsActive || user.Status != UserStatus.ACTIVE)
                return ApiResponse<AuthResponseDTO>.FailureResponse("Account is deactivated");
            // Update last login
            user.LastLogin = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            // Generate tokens
            var accessToken = _jwtHelper.GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            // Save refresh token
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays),
               // IsActive = true
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
    // public async Task<ApiResponse<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO request)
    // {  
    //     try
    //     {
    //         if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
    //             return ApiResponse<AuthResponseDTO>.FailureResponse("Email is already registered");
    //         var user = new User
    //         {
    //             Email = request.Email.ToLower(),
    //             PasswordHash = PasswordHelper.HashPassword(request.Password),
    //             Name = request.Name,
    //             Phone = request.Phone,
    //             StoreId = request.StoreId,
    //             IsActive = true,
    //             Status = UserStatus.ACTIVE
    //         };
    //         await _unitOfWork.Users.AddAsync(user);
    //         await _unitOfWork.SaveChangesAsync();
    //         var createdUser = await _unitOfWork.Users.GetByIdWithRolesAsync(user.Id);
    //         if (createdUser == null)
    //             return ApiResponse<AuthResponseDTO>.FailureResponse("Failed to create user");
    //         var accessToken = _jwtHelper.GenerateToken(createdUser);
    //         var refreshToken = GenerateRefreshToken();
    //         var refreshTokenEntity = new RefreshToken
    //         {
    //             UserId = user.Id,
    //             Token = refreshToken,
    //             ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays),
    //            // IsActive = true
    //         };
    //         await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
    //         await _unitOfWork.SaveChangesAsync();
    //         var response = new AuthResponseDTO
    //         {
    //             Token = accessToken,
    //             RefreshToken = refreshToken,
    //             Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes)
    //         };
    //         _logger.LogInformation("User {Email} registered successfully", request.Email);
    //         return ApiResponse<AuthResponseDTO>.SuccessResponse(response, "Registration successful");
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error during registration for email {Email}", request.Email);
    //         //return ApiResponse<AuthResponseDTO>.FailureResponse("An error occurred during registration");
    //         throw;
    //     }
    // }
public async Task<ApiResponse<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO request)
{
    try
    {
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
            return ApiResponse<AuthResponseDTO>.FailureResponse("Email is already registered");
        // ✅ Tạo token xác minh
        var verificationToken = Guid.NewGuid().ToString();
        // ✅ Lưu tạm vào cache trong 1 giờ
        _cache.Set(verificationToken, request, TimeSpan.FromHours(1));
        // ✅ Gửi email xác minh (tuỳ bạn có EmailService hay chưa)
        await SendVerificationEmailAsync(request.Email, verificationToken);
        _logger.LogInformation("Verification email sent to {Email}", request.Email);
        return ApiResponse<AuthResponseDTO>.SuccessResponse(
             null!, 
            "Please check your email to verify your account"
        );
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error during registration for email {Email}", request.Email);
        return ApiResponse<AuthResponseDTO>.FailureResponse("An error occurred during registration");
    }
}
public Task SendVerificationEmailAsync(string email, string token)
        {
            var verificationLink = $"https://yourapp.com/api/auth/verify-email?token={token}";
            _logger.LogInformation("Mock Email Sent to {Email}: {Link}", email, verificationLink);
            // Có thể in ra console nếu bạn muốn xem trực tiếp:
            Console.WriteLine($"📧 Verification email (mock) sent to {email}: {verificationLink}");
            return Task.CompletedTask;
        }
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
            //refreshToken.IsActive = true;
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
    public async Task<ApiResponse<bool>> LogoutAsync(Guid userId)
    {
        try
        {
            //await _unitOfWork.RefreshTokens.InvalidateTokensAsync(userId);
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
           // await _unitOfWork.RefreshTokens.InvalidateTokensAsync(userId);
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
    public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequestDTO request)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("Invalid reset request");
            user.PasswordHash = PasswordHelper.HashPassword(request.NewPassword);
            _unitOfWork.Users.Update(user);
           // await _unitOfWork.RefreshTokens.InvalidateTokensAsync(user.Id);
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
   public async Task<ApiResponse<bool>> VerifyEmailAsync(string token)
{
    try
    {
        if (!_cache.TryGetValue(token, out RegisterRequestDTO? pendingUser))
            return ApiResponse<bool>.FailureResponse("Invalid or expired verification token");
        if (await _unitOfWork.Users.EmailExistsAsync(pendingUser.Email))
            return ApiResponse<bool>.FailureResponse("Email already verified");
        // ✅ Map DTO → Entity bằng AutoMapper
        var user = _mapper.Map<User>(pendingUser);
        // ✅ Hash mật khẩu thủ công
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
    private static string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
    }
}

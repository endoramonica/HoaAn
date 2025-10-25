// Core/Services/Interfaces/IAuthService.cs
using VietCommerce.Core.DTOs.Auth;
using VietCommerce.Core.Models;
namespace VietCommerce.Api.Services.Interfaces;
public interface IAuthService
{
    Task<ApiResponse<AuthResponseDTO>> LoginAsync(LoginDTO request);
    Task<ApiResponse<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO request);
    Task<ApiResponse<AuthResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request);
    Task<ApiResponse<bool>> LogoutAsync(Guid userId);
    Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequestDTO request);
    Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequestDTO request);
    Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequestDTO request);
    Task<ApiResponse<bool>> ValidateTokenAsync(string token);
    Task<ApiResponse<bool>> VerifyEmailAsync(string token);
    Task SendVerificationEmailAsync(string email, string token);
    Task<ApiResponse<AuthResponseDTO>> FindOrCreateGoogleUserAsync(SocialLoginRequestDTO request);
    Task SaveSessionAsync(Guid userId, string jti, TimeSpan ttl);
    Task<bool> ValidateSessionAsync(Guid userId, string jti);
    Task RevokeSessionAsync(Guid userId);
}

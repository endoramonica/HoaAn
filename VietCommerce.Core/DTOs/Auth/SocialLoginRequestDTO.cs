namespace VietCommerce.Core.DTOs.Auth
{
    public class SocialLoginRequestDTO
    {
        public string Provider { get; set; } = "Google"; // hoặc Facebook, Apple nếu mở rộng sau này
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public string? IdToken { get; set; } // token từ Google
    }
}

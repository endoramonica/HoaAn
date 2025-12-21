using System.Text.Json.Serialization;

namespace VietCommerce.Core.DTOs.Auth
{
    public class SocialLoginRequestDTO
    {
        [JsonPropertyName("provider")]
        public string Provider { get; set; } = "Google"; // hoặc Facebook, Apple nếu mở rộng sau này

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("avatarUrl")]
        public string? AvatarUrl { get; set; }

        [JsonPropertyName("idToken")]
        public string? IdToken { get; set; } // token từ Google
    }
}

# Google OAuth Backend Implementation Guide (.NET Core)

## 📋 Overview

Backend cần implement endpoint `/api/v1/Auth/login/google` để xử lý Google OAuth login.

## 🔧 Setup

### 1. Install NuGet Packages

```bash
dotnet add package Google.Apis.Auth
dotnet add package Google.Apis.Core
```

### 2. Configure Google Client ID

Add to `appsettings.json`:

```json
{
  "GoogleAuth": {
    "ClientId": "827299123899-ofqi52mkbf59d3id9cjrs9sq54av1rr3.apps.googleusercontent.com"
  }
}
```

## 📝 Implementation

### 1. Create SocialLoginRequestDTO

```csharp
public class SocialLoginRequestDTO
{
    public string Provider { get; set; } // "Google", "Facebook", etc.
    public string IdToken { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
}
```

### 2. Create Google Token Verification Service

```csharp
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;

public interface IGoogleAuthService
{
    Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken);
}

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(IConfiguration configuration, ILogger<GoogleAuthService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            var clientId = _configuration["GoogleAuth:ClientId"];
            
            _logger.LogInformation("[GoogleAuthService] Verifying Google token with Client ID: {ClientId}", clientId);

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { clientId }
            });

            _logger.LogInformation("[GoogleAuthService] ✅ Token verified successfully for email: {Email}", payload.Email);

            return payload;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError("[GoogleAuthService] ❌ Token verification failed: {Message}", ex.Message);
            throw new UnauthorizedAccessException("Invalid Google token", ex);
        }
    }
}
```

### 3. Update AuthController

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IGoogleAuthService googleAuthService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _googleAuthService = googleAuthService;
        _logger = logger;
    }

    [HttpPost("login/google")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] SocialLoginRequestDTO request)
    {
        try
        {
            _logger.LogInformation("[AuthController] Google login request received for email: {Email}", request.Email);

            // Verify Google token
            var payload = await _googleAuthService.VerifyGoogleTokenAsync(request.IdToken);

            _logger.LogInformation("[AuthController] 📦 Google payload: Email={Email}, Name={Name}", 
                payload.Email, payload.Name);

            // Find or create user
            var user = await _authService.FindOrCreateGoogleUserAsync(new GoogleUserInfo
            {
                Email = payload.Email,
                FullName = payload.Name,
                AvatarUrl = payload.Picture,
                GoogleId = payload.Subject
            });

            _logger.LogInformation("[AuthController] ✅ User found/created: {UserId}", user.Id);

            // Generate tokens
            var accessToken = _authService.GenerateAccessToken(user);
            var refreshToken = _authService.GenerateRefreshToken(user);

            // Save refresh token
            await _authService.SaveRefreshTokenAsync(user.Id, refreshToken);

            _logger.LogInformation("[AuthController] ✅ Tokens generated successfully");

            return Ok(new
            {
                success = true,
                data = new
                {
                    accessToken = accessToken,
                    refreshToken = refreshToken,
                    expiresIn = 7200,
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        fullName = user.FullName,
                        avatarUrl = user.AvatarUrl,
                        roles = user.Roles.Select(r => r.Name).ToList()
                    }
                }
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError("[AuthController] ❌ Unauthorized: {Message}", ex.Message);
            return Unauthorized(new { message = "Invalid Google token" });
        }
        catch (Exception ex)
        {
            _logger.LogError("[AuthController] ❌ Error: {Message}", ex.Message);
            return BadRequest(new { message = "Google login failed" });
        }
    }
}
```

### 4. Update AuthService

```csharp
public interface IAuthService
{
    Task<User> FindOrCreateGoogleUserAsync(GoogleUserInfo googleUserInfo);
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    Task SaveRefreshTokenAsync(string userId, string refreshToken);
}

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<User> FindOrCreateGoogleUserAsync(GoogleUserInfo googleUserInfo)
    {
        _logger.LogInformation("[AuthService] Finding or creating Google user: {Email}", googleUserInfo.Email);

        // Find existing user by email
        var user = await _userManager.FindByEmailAsync(googleUserInfo.Email);

        if (user != null)
        {
            _logger.LogInformation("[AuthService] ✅ User found: {UserId}", user.Id);
            
            // Update avatar if provided
            if (!string.IsNullOrEmpty(googleUserInfo.AvatarUrl))
            {
                user.AvatarUrl = googleUserInfo.AvatarUrl;
                await _userManager.UpdateAsync(user);
            }

            return user;
        }

        // Create new user
        _logger.LogInformation("[AuthService] Creating new user from Google: {Email}", googleUserInfo.Email);

        user = new User
        {
            UserName = googleUserInfo.Email,
            Email = googleUserInfo.Email,
            FullName = googleUserInfo.FullName,
            AvatarUrl = googleUserInfo.AvatarUrl,
            EmailConfirmed = true, // Google email is verified
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            _logger.LogError("[AuthService] ❌ Failed to create user: {Errors}", 
                string.Join(", ", result.Errors.Select(e => e.Description)));
            throw new Exception("Failed to create user");
        }

        // Assign Customer role
        await _userManager.AddToRoleAsync(user, "Customer");

        _logger.LogInformation("[AuthService] ✅ User created: {UserId}", user.Id);

        return user;
    }

    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        // Add roles
        var roles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken(User user)
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }

    public async Task SaveRefreshTokenAsync(string userId, string refreshToken)
    {
        // Save to database (implement based on your schema)
        _logger.LogInformation("[AuthService] Saving refresh token for user: {UserId}", userId);
    }
}
```

### 5. Create GoogleUserInfo DTO

```csharp
public class GoogleUserInfo
{
    public string Email { get; set; }
    public string FullName { get; set; }
    public string AvatarUrl { get; set; }
    public string GoogleId { get; set; }
}
```

### 6. Register Services in Startup

```csharp
// In Program.cs or Startup.cs
services.AddScoped<IGoogleAuthService, GoogleAuthService>();
services.AddScoped<IAuthService, AuthService>();
```

## 🔐 Security Considerations

1. **Token Verification**
   - Always verify Google token signature
   - Check token expiration
   - Validate audience (Client ID)

2. **HTTPS Only**
   - Use HTTPS in production
   - Never send tokens over HTTP

3. **Token Storage**
   - Store refresh tokens securely in database
   - Hash refresh tokens before storing
   - Implement token rotation

4. **CORS Configuration**
   - Allow only trusted origins
   - Add frontend domain to CORS policy

```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder
            .WithOrigins("http://localhost:5173", "https://yourdomain.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

## 📊 Database Schema

Add to User model:

```csharp
public class User : IdentityUser
{
    public string FullName { get; set; }
    public string AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // For OAuth
    public string GoogleId { get; set; }
    public string FacebookId { get; set; }
}
```

## 🧪 Testing

### Test with cURL

```bash
curl -X POST https://localhost:7131/api/v1/Auth/login/google \
  -H "Content-Type: application/json" \
  -d '{
    "provider": "Google",
    "idToken": "your_google_id_token",
    "email": "user@gmail.com",
    "name": "User Name",
    "avatarUrl": "https://..."
  }'
```

### Expected Response

```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "base64_encoded_refresh_token",
    "expiresIn": 7200,
    "user": {
      "id": "user_id",
      "email": "user@gmail.com",
      "fullName": "User Name",
      "avatarUrl": "https://...",
      "roles": ["Customer"]
    }
  }
}
```

## 🐛 Troubleshooting

### Issue: "Invalid Google token"
**Solution:**
- Verify Client ID matches Google Cloud Console
- Check token expiration
- Ensure token is not tampered with

### Issue: "User creation failed"
**Solution:**
- Check database connection
- Verify User model has required fields
- Check email uniqueness constraint

### Issue: "Token generation failed"
**Solution:**
- Verify JWT configuration in appsettings.json
- Check secret key length (minimum 32 characters)
- Ensure issuer and audience are configured

## 📚 References

- [Google OAuth 2.0 Documentation](https://developers.google.com/identity/protocols/oauth2)
- [Google.Apis.Auth NuGet Package](https://www.nuget.org/packages/Google.Apis.Auth/)
- [ASP.NET Core JWT Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/jwt)
- [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)

## ✨ Features

✅ Google token verification
✅ Automatic user creation
✅ JWT token generation
✅ Refresh token support
✅ Error handling
✅ Logging
✅ Security best practices

## 🚀 Next Steps

1. Implement the services above
2. Test with frontend
3. Add error handling
4. Implement refresh token rotation
5. Add rate limiting
6. Monitor and log authentication events

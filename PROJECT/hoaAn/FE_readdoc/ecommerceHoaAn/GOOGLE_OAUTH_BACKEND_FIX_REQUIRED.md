# 🔴 BE CẦN SỬA: Google OAuth Login

## 📋 Vấn đề hiện tại

FE đang gửi `access_token` từ Google, nhưng BE đang cố verify nó như `id_token` → **Invalid Google token**

## ✅ Cách fix BE

### 1️⃣ Endpoint: `POST /api/v1/Auth/login/google`

**FE gửi:**
```json
{
  "provider": "Google",
  "idToken": "ya29.A0Aa7pCA_...",
  "email": null,
  "name": null,
  "avatarUrl": null
}
```

⚠️ **Lưu ý:** `idToken` ở đây là **access_token** từ Google (không phải JWT id_token)

### 2️⃣ BE cần làm:

```csharp
[HttpPost("login/google")]
public async Task<IActionResult> LoginGoogle([FromBody] SocialLoginRequestDTO request)
{
    try
    {
        if (string.IsNullOrEmpty(request.IdToken))
            return BadRequest(new { message = "IdToken không được rỗng" });

        // ✅ STEP 1: Call Google API để lấy user info
        var userInfo = await GetGoogleUserInfo(request.IdToken);
        
        if (userInfo == null)
            return BadRequest(new { message = "Invalid Google token" });

        // ✅ STEP 2: Extract user info
        var email = userInfo.Email;
        var name = userInfo.Name;
        var avatarUrl = userInfo.Picture;

        // ✅ STEP 3: Tìm hoặc tạo user
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null)
        {
            user = new User
            {
                UserName = email,
                Email = email,
                FullName = name,
                AvatarUrl = avatarUrl,
                Provider = "Google",
                EmailConfirmed = true
            };
            
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { message = "Không thể tạo user" });
        }
        else
        {
            // Update user info nếu cần
            user.FullName = name;
            user.AvatarUrl = avatarUrl;
            await _userManager.UpdateAsync(user);
        }

        // ✅ STEP 4: Tạo JWT tokens
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        // Save refresh token
        user.RefreshToken = refreshToken;
        await _userManager.UpdateAsync(user);

        return Ok(new
        {
            success = true,
            data = new
            {
                token = accessToken,
                accessToken = accessToken,
                refreshToken = refreshToken,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.FullName,
                    avatarUrl = user.AvatarUrl
                }
            }
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}

// ✅ Helper: Call Google API để lấy user info
private async Task<GoogleUserInfo> GetGoogleUserInfo(string accessToken)
{
    try
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
            
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GoogleUserInfo>(json);
        }
    }
    catch
    {
        return null;
    }
}

// ✅ Model
public class GoogleUserInfo
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("picture")]
    public string Picture { get; set; }
}
```

### 3️⃣ NuGet packages cần:
```
Newtonsoft.Json
System.Net.Http
```

## 📊 Flow hoàn chỉnh

```
FE (Google Sign-In)
    ↓
    Nhận access_token từ Google
    ↓
POST /api/v1/Auth/login/google
{
  "provider": "Google",
  "idToken": "ya29.A0Aa7pCA_..."
}
    ↓
BE
    ↓
    Call: GET https://www.googleapis.com/oauth2/v2/userinfo
    Header: Authorization: Bearer ya29.A0Aa7pCA_...
    ↓
    Nhận: { email, name, picture }
    ↓
    Tìm/tạo user
    ↓
    Tạo JWT tokens
    ↓
    Return: { accessToken, refreshToken, user }
    ↓
FE
    ↓
    Save tokens
    ↓
    Redirect to home
```

## 🔗 Google API Reference
- Endpoint: `https://www.googleapis.com/oauth2/v2/userinfo`
- Auth: Bearer token (access_token)
- Response: `{ id, email, name, picture, ... }`

## ⚠️ Lưu ý
- Access token từ Google có thời gian sống ~1 giờ
- Nếu token expire, FE sẽ phải login lại
- Không cần verify JWT signature vì gọi trực tiếp Google API

# 🔴 DEBUG: BE không nhận được idToken

## 📋 Vấn đề

FE gửi JWT đúng:
```json
{
  "provider": "Google",
  "idToken": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjZhOTA2ZWMxMTlkN2JhND..."
}
```

Nhưng BE log nói:
```
⚠️ Tham số không hợp lệ (GoogleLogin): IdToken không được rỗng
```

## ✅ Cách debug

### 1️⃣ Thêm log vào Controller

```csharp
[HttpPost("login/google")]
public async Task<IActionResult> LoginGoogle([FromBody] SocialLoginRequestDTO request)
{
    // 🔍 DEBUG: Log request nhận được
    Console.WriteLine($"[DEBUG] Request received:");
    Console.WriteLine($"[DEBUG] Provider: {request.Provider}");
    Console.WriteLine($"[DEBUG] IdToken: {request.IdToken}");
    Console.WriteLine($"[DEBUG] IdToken length: {request.IdToken?.Length}");
    Console.WriteLine($"[DEBUG] IdToken is null: {request.IdToken == null}");
    Console.WriteLine($"[DEBUG] IdToken is empty: {string.IsNullOrEmpty(request.IdToken)}");
    
    // Gọi service
    var result = await _authService.FindOrCreateGoogleUserAsync(request);
    return Ok(result);
}
```

### 2️⃣ Kiểm tra SocialLoginRequestDTO

```csharp
public class SocialLoginRequestDTO
{
    public string Provider { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public string IdToken { get; set; }  // ✅ Phải có property này
}
```

### 3️⃣ Kiểm tra JSON serialization

Nếu BE không nhận được `idToken`, có thể là:

**❌ Sai:** Property name không khớp
```csharp
public string Id_Token { get; set; }  // ❌ Sai - FE gửi idToken
```

**✅ Đúng:** Property name khớp
```csharp
public string IdToken { get; set; }  // ✅ Đúng - FE gửi idToken
```

**Hoặc thêm JsonProperty:**
```csharp
[JsonProperty("idToken")]
public string IdToken { get; set; }
```

### 4️⃣ Kiểm tra Model Binding

Nếu vẫn không nhận được, thêm `[FromBody]`:

```csharp
[HttpPost("login/google")]
public async Task<IActionResult> LoginGoogle([FromBody] SocialLoginRequestDTO request)
{
    // ...
}
```

### 5️⃣ Kiểm tra Content-Type

FE gửi `Content-Type: application/json`, BE phải accept nó:

```csharp
[Consumes("application/json")]
[HttpPost("login/google")]
public async Task<IActionResult> LoginGoogle([FromBody] SocialLoginRequestDTO request)
{
    // ...
}
```

## 🔍 Hãy thử:

1. Thêm log ở trên vào Controller
2. Chạy lại login từ FE
3. Xem BE console output
4. Chia sẻ output

## 📊 Expected output

```
[DEBUG] Request received:
[DEBUG] Provider: Google
[DEBUG] IdToken: eyJhbGciOiJSUzI1NiIsImtpZCI6IjZhOTA2ZWMxMTlkN2JhND...
[DEBUG] IdToken length: 1234
[DEBUG] IdToken is null: False
[DEBUG] IdToken is empty: False
```

Nếu `IdToken is null: True` hoặc `IdToken is empty: True`, thì vấn đề là model binding.

## 🎯 Khả năng cao nhất

**Property name không khớp** - BE DTO có property `Id_Token` hoặc `id_token` thay vì `IdToken`

Hãy kiểm tra `SocialLoginRequestDTO` class definition.

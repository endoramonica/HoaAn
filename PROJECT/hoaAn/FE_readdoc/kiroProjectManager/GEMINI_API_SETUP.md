# Hướng Dẫn Cấu Hình Gemini API

## Bước 1: Lấy Gemini API Key

### Cách 1: Sử dụng Google AI Studio (Khuyến nghị)

1. **Truy cập Google AI Studio:**
   ```
   https://aistudio.google.com/app/apikey
   ```

2. **Đăng nhập:**
   - Sử dụng tài khoản Google của bạn
   - Nếu chưa có, tạo tài khoản Google mới

3. **Tạo API Key:**
   - Click nút **"Create API Key"** hoặc **"Get API Key"**
   - Chọn Google Cloud project (hoặc tạo mới)
   - Click **"Create API key in new project"** nếu chưa có project

4. **Copy API Key:**
   - API key sẽ hiển thị dạng: `AIzaSy...`
   - Click icon **Copy** để copy key
   - **LƯU Ý:** Lưu key này an toàn, không chia sẻ công khai

### Cách 2: Sử dụng Google Cloud Console

1. **Truy cập Google Cloud Console:**
   ```
   https://console.cloud.google.com/
   ```

2. **Tạo hoặc chọn Project:**
   - Click dropdown project ở góc trên
   - Chọn project hiện có hoặc **"New Project"**

3. **Enable Gemini API:**
   - Vào **"APIs & Services"** → **"Library"**
   - Tìm **"Generative Language API"**
   - Click **"Enable"**

4. **Tạo Credentials:**
   - Vào **"APIs & Services"** → **"Credentials"**
   - Click **"Create Credentials"** → **"API Key"**
   - Copy API key được tạo

## Bước 2: Cấu Hình Trong Project

### 1. Tạo file `.env`

Nếu chưa có file `.env`, copy từ `.env.example`:

```bash
cp .env.example .env
```

### 2. Thêm API Key vào `.env`

Mở file `.env` và thêm/cập nhật:

```env
# Gemini API Key
VITE_GEMINI_API_KEY=AIzaSyBCAUM9DHJjeILypYzSEtUY76W-jmXQsqU
```

**Thay thế** `AIzaSyBCAUM9DHJjeILypYzSEtUY76W-jmXQsqU` bằng API key của bạn.

### 3. Restart Development Server

Sau khi thêm API key, restart server:

```bash
# Dừng server hiện tại (Ctrl + C)
# Chạy lại
npm run dev
```

## Bước 3: Kiểm Tra Cấu Hình

### Test API Key

Tạo file test đơn giản:

```typescript
// test-gemini.ts
const GEMINI_API_KEY = import.meta.env.VITE_GEMINI_API_KEY;

async function testGemini() {
    const response = await fetch(
        `https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=${GEMINI_API_KEY}`,
        {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                contents: [{
                    parts: [{ text: 'Hello, Gemini!' }]
                }]
            })
        }
    );
    
    const data = await response.json();
    console.log('Response:', data);
}

testGemini();
```

### Hoặc Test Trực Tiếp Trong App

1. Vào trang **Marketing**
2. Click **"Tạo Bài Viết"**
3. Chọn một sản phẩm
4. Click **"Tạo Nội Dung"**
5. Nếu thành công → API key đã hoạt động ✅

## Thông Tin API Models

### Models Có Sẵn

| Model | Mô Tả | Use Case | Max Tokens |
|-------|-------|----------|------------|
| `gemini-2.5-flash` | **Mới nhất**, nhanh nhất, thông minh | Marketing, SEO, content generation | 8,192 |
| `gemini-1.5-flash` | Nhanh, hiệu quả | Marketing content, mô tả ngắn | 8,192 |
| `gemini-1.5-pro` | Chất lượng cao | Nội dung phức tạp, dài | 32,768 |
| `gemini-pro` | Cũ (deprecated) | Không khuyến nghị | 2,048 |

### Cấu Hình Mặc Định

```typescript
{
    model: 'gemini-2.5-flash',  // Model mới nhất
    temperature: 0.7,            // Độ sáng tạo (0-1)
    maxOutputTokens: 8192,       // Độ dài tối đa
    topK: 40,
    topP: 0.95
}
```

## API Limits & Pricing

### Free Tier (Miễn Phí)

**Gemini 1.5 Flash:**
- ✅ 15 requests/phút
- ✅ 1 triệu tokens/phút
- ✅ 1,500 requests/ngày

**Gemini 1.5 Pro:**
- ✅ 2 requests/phút
- ✅ 32,000 tokens/phút
- ✅ 50 requests/ngày

### Paid Tier (Trả Phí)

**Gemini 1.5 Flash:**
- $0.075 / 1 triệu input tokens
- $0.30 / 1 triệu output tokens

**Gemini 1.5 Pro:**
- $1.25 / 1 triệu input tokens
- $5.00 / 1 triệu output tokens

## Xử Lý Lỗi Thường Gặp

### Lỗi 404: Model Not Found

**Nguyên nhân:** Model name không đúng hoặc API endpoint cũ

**Giải pháp:**
```typescript
// ❌ Sai (model cũ)
model: 'gemini-pro'

// ✅ Đúng (model mới nhất)
model: 'gemini-2.5-flash'

// ✅ Cũng OK (model cũ hơn nhưng vẫn hoạt động)
model: 'gemini-1.5-flash'
```

### Lỗi 400: Invalid API Key

**Nguyên nhân:** API key không hợp lệ

**Giải pháp:**
1. Kiểm tra API key trong `.env`
2. Đảm bảo không có khoảng trắng thừa
3. Tạo API key mới nếu cần

### Lỗi 429: Rate Limit Exceeded

**Nguyên nhân:** Vượt quá giới hạn requests

**Giải pháp:**
1. Đợi 1 phút rồi thử lại
2. Giảm số lượng requests
3. Implement rate limiting trong code
4. Nâng cấp lên paid tier

### Lỗi 403: Permission Denied

**Nguyên nhân:** API chưa được enable trong project

**Giải pháp:**
1. Vào Google Cloud Console
2. Enable "Generative Language API"
3. Đợi vài phút để API active

## Best Practices

### 1. Bảo Mật API Key

**Nên:**
- ✅ Lưu trong `.env` file
- ✅ Thêm `.env` vào `.gitignore`
- ✅ Sử dụng environment variables
- ✅ Rotate key định kỳ

**Không nên:**
- ❌ Commit API key lên Git
- ❌ Hardcode trong source code
- ❌ Chia sẻ key công khai
- ❌ Sử dụng key trong client-side code production

### 2. Tối Ưu Sử Dụng

**Cache Results:**
```typescript
// Cache kết quả để tránh gọi API lại
const cache = new Map();

async function generateWithCache(prompt: string) {
    if (cache.has(prompt)) {
        return cache.get(prompt);
    }
    
    const result = await generateText({ prompt });
    cache.set(prompt, result);
    return result;
}
```

**Rate Limiting:**
```typescript
// Giới hạn số requests
let requestCount = 0;
const MAX_REQUESTS_PER_MINUTE = 15;

async function generateWithLimit(prompt: string) {
    if (requestCount >= MAX_REQUESTS_PER_MINUTE) {
        throw new Error('Đã đạt giới hạn requests. Vui lòng đợi.');
    }
    
    requestCount++;
    setTimeout(() => requestCount--, 60000);
    
    return generateText({ prompt });
}
```

### 3. Error Handling

```typescript
async function generateSafely(prompt: string) {
    try {
        return await generateText({ prompt });
    } catch (error: any) {
        // Log error
        console.error('Gemini Error:', error);
        
        // Fallback
        if (error.message.includes('429')) {
            return 'Đã đạt giới hạn API. Vui lòng thử lại sau.';
        }
        
        // Re-throw
        throw error;
    }
}
```

## Monitoring & Analytics

### Theo Dõi Usage

1. **Google Cloud Console:**
   - Vào **"APIs & Services"** → **"Dashboard"**
   - Xem biểu đồ requests, errors, latency

2. **Quotas:**
   - Vào **"IAM & Admin"** → **"Quotas"**
   - Xem và request tăng quota

### Metrics Quan Trọng

- **Requests/day**: Số lượng requests mỗi ngày
- **Tokens/request**: Trung bình tokens mỗi request
- **Error rate**: Tỷ lệ lỗi
- **Latency**: Thời gian phản hồi

## Nâng Cấp Lên Paid Tier

### Khi Nào Nên Nâng Cấp?

- Vượt quá 1,500 requests/ngày
- Cần response nhanh hơn
- Sử dụng cho production
- Cần support chính thức

### Cách Nâng Cấp

1. Vào Google Cloud Console
2. Enable billing cho project
3. Thêm payment method
4. Quota sẽ tự động tăng

## Troubleshooting

### Debug Mode

Bật debug để xem chi tiết requests:

```typescript
// geminiService.ts
const DEBUG = true;

async function generateText(options: GenerateTextOptions) {
    if (DEBUG) {
        console.log('Request:', {
            model: options.model,
            prompt: options.prompt.substring(0, 100) + '...',
        });
    }
    
    const response = await fetch(/* ... */);
    
    if (DEBUG) {
        console.log('Response status:', response.status);
        const data = await response.json();
        console.log('Response data:', data);
    }
    
    // ...
}
```

### Test API Trực Tiếp

Sử dụng curl để test với model mới nhất:

```bash
curl -X POST \
  "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key=YOUR_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "contents": [{
      "parts": [{"text": "Hello, Gemini! Hãy giới thiệu về bạn bằng tiếng Việt."}]
    }]
  }'
```

## Resources

### Official Documentation
- **Gemini API Docs**: https://ai.google.dev/docs
- **API Reference**: https://ai.google.dev/api
- **Quickstart**: https://ai.google.dev/tutorials/quickstart

### Community
- **GitHub Issues**: https://github.com/google/generative-ai-js/issues
- **Stack Overflow**: Tag `google-gemini`
- **Discord**: Google AI Discord server

### Tools
- **AI Studio**: https://aistudio.google.com/
- **Cloud Console**: https://console.cloud.google.com/
- **API Explorer**: https://developers.google.com/apis-explorer

## Support

Nếu gặp vấn đề:
1. Kiểm tra lại các bước trên
2. Xem phần Troubleshooting
3. Kiểm tra console logs
4. Liên hệ team support

# Hướng Dẫn Sử Dụng Marketing AI

## Cấu Hình API Keys

### 1. Gemini API Key (Google)

Trong file `.env`, thêm:
```env
VITE_GEMINI_API_KEY=AIzaSyBCAUM9DHJjeILypYzSEtUY76W-jmXQsqU
```

**Cách lấy Gemini API Key:**
1. Truy cập: https://makersuite.google.com/app/apikey
2. Đăng nhập bằng tài khoản Google
3. Click "Create API Key"
4. Copy key và paste vào `.env`

### 2. OpenAI API Key (Tùy chọn)

```env
VITE_OPENAI_API_KEY=your_openai_api_key_here
```

**Cách lấy OpenAI API Key:**
1. Truy cập: https://platform.openai.com/api-keys
2. Đăng nhập/Đăng ký
3. Click "Create new secret key"
4. Copy key và paste vào `.env`

## Tính Năng Marketing AI

### 1. Tạo Nội Dung Marketing Tự Động

**Cách sử dụng:**
1. Vào trang **Marketing** → Click **"Tạo Bài Viết"**
2. Chọn **Sản Phẩm** từ dropdown
3. Chọn **Giọng Điệu**:
   - **Thân thiện**: Ấm áp, gần gũi với khách hàng
   - **Chuyên nghiệp**: Trang trọng, đáng tin cậy
   - **Gần gũi**: Thoải mái, dễ tiếp cận
   - **Nhiệt tình**: Hào hứng, năng động
4. Click **"Tạo Nội Dung"**

**AI sẽ tự động tạo:**
- ✅ Tiêu đề hấp dẫn (tối đa 100 ký tự)
- ✅ Nội dung bài viết (200-300 từ)
- ✅ 5-7 hashtags phù hợp
- ✅ Kêu gọi hành động rõ ràng

### 2. Cải Thiện Nội Dung

**Khi nào sử dụng:**
- Nội dung chưa đủ hấp dẫn
- Cần thêm kêu gọi hành động
- Muốn tối ưu cho SEO

**Cách sử dụng:**
1. Sau khi có nội dung (tự viết hoặc AI tạo)
2. Click nút **"Cải Thiện"**
3. AI sẽ tự động:
   - Làm nội dung hấp dẫn hơn
   - Thêm call-to-action mạnh mẽ
   - Tối ưu từ khóa SEO

### 3. Tạo Bài Viết Cho Nhiều Nền Tảng

**Tính năng:**
Tự động tạo phiên bản phù hợp cho từng mạng xã hội:
- **Facebook**: Dài, chi tiết, có emoji
- **Instagram**: Ngắn gọn, nhiều emoji, hashtags
- **Twitter/X**: Ngắn gọn, tối đa 280 ký tự
- **LinkedIn**: Chuyên nghiệp, trang trọng

**Cách sử dụng:**
1. Tạo nội dung chính trước
2. Click **"Social Posts"**
3. AI tạo 4 phiên bản cho 4 nền tảng
4. Click icon **Copy** để copy từng phiên bản

### 4. Tạo SEO Metadata

**Tính năng:**
Tự động tạo metadata tối ưu cho SEO:
- **Meta Title**: Tối đa 60 ký tự, có từ khóa chính
- **Meta Description**: 150-160 ký tự, mô tả ngắn gọn
- **Meta Keywords**: 5-10 từ khóa liên quan

**Cách sử dụng:**
1. Nhập tiêu đề và nội dung
2. Click **"Tạo SEO"** trong phần SEO Metadata
3. AI tự động tạo metadata tối ưu
4. Có thể chỉnh sửa thủ công nếu cần

## Quy Trình Làm Việc Đề Xuất

### Quy Trình Nhanh (5 phút)
```
1. Chọn sản phẩm
2. Chọn giọng điệu
3. Click "Tạo Nội Dung"
4. Review và chỉnh sửa nhẹ
5. Lưu và đăng
```

### Quy Trình Chuyên Nghiệp (15 phút)
```
1. Chọn sản phẩm và giọng điệu
2. Tạo nội dung với AI
3. Click "Cải Thiện" để tối ưu
4. Tạo SEO metadata
5. Tạo Social Posts cho các nền tảng
6. Review toàn bộ
7. Lưu và lên lịch đăng
```

## Các Service AI Có Sẵn

### 1. `generateMarketingContent()`
Tạo nội dung marketing hoàn chỉnh từ thông tin sản phẩm.

**Input:**
```typescript
{
    name: string;              // Tên sản phẩm
    description?: string;      // Mô tả sản phẩm
    price?: number;           // Giá
    category?: string;        // Danh mục
    targetAudience?: string;  // Đối tượng khách hàng
    tone?: 'professional' | 'casual' | 'enthusiastic' | 'friendly';
}
```

**Output:**
```typescript
{
    title: string;        // Tiêu đề
    content: string;      // Nội dung
    hashtags: string[];   // Danh sách hashtags
}
```

### 2. `generateSocialMediaPosts()`
Tạo phiên bản cho nhiều nền tảng mạng xã hội.

**Input:**
```typescript
baseContent: string;  // Nội dung gốc
platforms: ('facebook' | 'instagram' | 'twitter' | 'linkedin')[];
```

**Output:**
```typescript
{
    facebook: string;
    instagram: string;
    twitter: string;
    linkedin: string;
}
```

### 3. `improveMarketingContent()`
Cải thiện nội dung marketing hiện có.

**Input:**
```typescript
currentContent: string;     // Nội dung hiện tại
improvements: string[];     // Danh sách yêu cầu cải thiện
```

**Output:**
```typescript
string  // Nội dung đã được cải thiện
```

### 4. `generateSEOContent()`
Tạo SEO metadata tối ưu.

**Input:**
```typescript
{
    name: string;           // Tên sản phẩm/bài viết
    description: string;    // Mô tả
    keywords?: string[];    // Từ khóa gợi ý
}
```

**Output:**
```typescript
{
    metaTitle: string;
    metaDescription: string;
    metaKeywords: string[];
}
```

### 5. `generateProductDescription()`
Tạo mô tả sản phẩm hấp dẫn.

**Input:**
```typescript
{
    name: string;
    category?: string;
    features?: string[];   // Tính năng
    benefits?: string[];   // Lợi ích
}
```

**Output:**
```typescript
string  // Mô tả sản phẩm (100-150 từ)
```

## Tips & Best Practices

### 1. Chọn Giọng Điệu Phù Hợp

| Sản Phẩm | Giọng Điệu Đề Xuất |
|----------|-------------------|
| Công nghệ, B2B | Chuyên nghiệp |
| Thời trang, Lifestyle | Thân thiện |
| Đồ gia dụng | Gần gũi |
| Sự kiện, Khuyến mãi | Nhiệt tình |

### 2. Tối Ưu Nội Dung

**Nên:**
- ✅ Review và chỉnh sửa nội dung AI tạo
- ✅ Thêm thông tin cụ thể về sản phẩm
- ✅ Kiểm tra chính tả và ngữ pháp
- ✅ Thêm emoji phù hợp (đặc biệt cho Facebook, Instagram)
- ✅ Đảm bảo có call-to-action rõ ràng

**Không nên:**
- ❌ Copy 100% nội dung AI mà không review
- ❌ Sử dụng quá nhiều hashtags (tối đa 5-7)
- ❌ Nội dung quá dài (tối đa 300 từ)
- ❌ Thiếu thông tin liên hệ/mua hàng

### 3. SEO Best Practices

**Meta Title:**
- Độ dài: 50-60 ký tự
- Có từ khóa chính ở đầu
- Hấp dẫn, kêu gọi click

**Meta Description:**
- Độ dài: 150-160 ký tự
- Mô tả ngắn gọn, súc tích
- Có call-to-action
- Chứa từ khóa chính

**Meta Keywords:**
- 5-10 từ khóa
- Liên quan trực tiếp đến sản phẩm
- Bao gồm cả từ khóa dài (long-tail)

### 4. Lên Lịch Đăng Bài

**Thời gian đăng tốt nhất:**

| Nền Tảng | Thời Gian Tốt Nhất |
|----------|-------------------|
| Facebook | 13:00 - 16:00, 19:00 - 21:00 |
| Instagram | 11:00 - 13:00, 19:00 - 21:00 |
| LinkedIn | 7:00 - 9:00, 17:00 - 18:00 |
| Twitter | 12:00 - 13:00, 17:00 - 18:00 |

**Tần suất đăng:**
- Facebook: 1-2 bài/ngày
- Instagram: 1-3 bài/ngày
- LinkedIn: 1 bài/ngày
- Twitter: 3-5 bài/ngày

## Xử Lý Lỗi

### Lỗi: "VITE_GEMINI_API_KEY không được cấu hình"

**Nguyên nhân:** Chưa thêm API key vào `.env`

**Giải pháp:**
1. Mở file `.env`
2. Thêm dòng: `VITE_GEMINI_API_KEY=your_api_key_here`
3. Restart dev server

### Lỗi: "Lỗi khi gọi Gemini API"

**Nguyên nhân:** 
- API key không hợp lệ
- Hết quota
- Lỗi mạng

**Giải pháp:**
1. Kiểm tra API key có đúng không
2. Kiểm tra quota tại: https://makersuite.google.com/
3. Thử lại sau vài phút

### Lỗi: "Không nhận được nội dung từ Gemini"

**Nguyên nhân:** Response từ API không đúng format

**Giải pháp:**
1. Thử tạo lại với prompt khác
2. Giảm độ dài yêu cầu
3. Kiểm tra log console để debug

## Giới Hạn & Lưu Ý

### Gemini API Free Tier
- **Requests**: 60 requests/phút
- **Tokens**: 32,000 tokens/phút
- **Daily limit**: 1,500 requests/ngày

### Best Practices
1. **Cache kết quả**: Lưu nội dung đã tạo để tránh gọi API lại
2. **Batch processing**: Tạo nhiều nội dung cùng lúc
3. **Error handling**: Luôn có fallback khi API lỗi
4. **Rate limiting**: Không gọi quá nhiều requests liên tục

## Ví Dụ Thực Tế

### Ví Dụ 1: Tạo Bài Viết Cho Sản Phẩm Mới

**Input:**
- Sản phẩm: "Đèn LED Thông Minh"
- Giá: 299,000 VND
- Giọng điệu: Nhiệt tình

**Output:**
```
Tiêu đề: 🌟 Đèn LED Thông Minh - Chiếu Sáng Ngôi Nhà Bạn! 🏠

Nội dung:
Bạn đang tìm kiếm giải pháp chiếu sáng hiện đại cho ngôi nhà? 
Đèn LED Thông Minh chính là lựa chọn hoàn hảo! 💡

✨ Điều khiển bằng điện thoại
🎨 16 triệu màu sắc
⚡ Tiết kiệm điện đến 80%
🔊 Kết nối với trợ lý ảo

Chỉ với 299,000đ, bạn sẽ có một ngôi nhà thông minh, hiện đại!

👉 Đặt hàng ngay hôm nay để nhận ưu đãi đặc biệt!

Hashtags: #denled #denthongminh #smarthome #tietkiemdien #nhamodern
```

### Ví Dụ 2: Tạo SEO Cho Bài Blog

**Input:**
- Tiêu đề: "Top 10 Mẹo Trang Trí Nhà Đẹp"
- Nội dung: [Bài viết về trang trí nội thất]

**Output:**
```
Meta Title: Top 10 Mẹo Trang Trí Nhà Đẹp 2024 | Nội Thất Hiện Đại

Meta Description: Khám phá 10 mẹo trang trí nhà đẹp, hiện đại và tiết kiệm chi phí. Hướng dẫn chi tiết từ chuyên gia nội thất. Xem ngay!

Meta Keywords: trang trí nhà, nội thất đẹp, mẹo trang trí, thiết kế nhà, nội thất hiện đại, trang trí tiết kiệm, decor nhà
```

## Tích Hợp Với Workflow

### Tích Hợp Với Products
- Tự động lấy thông tin sản phẩm
- Sử dụng tên, mô tả, giá để tạo nội dung
- Đồng bộ với danh mục sản phẩm

### Tích Hợp Với Calendar
- Lên lịch đăng bài tự động
- Quản lý content calendar
- Nhắc nhở đăng bài

### Tích Hợp Với Analytics
- Theo dõi hiệu quả bài viết
- A/B testing các phiên bản
- Tối ưu dựa trên dữ liệu

## Roadmap

### Sắp Tới
- [ ] Tích hợp tạo ảnh với DALL-E/Midjourney
- [ ] Phân tích sentiment của nội dung
- [ ] Gợi ý thời gian đăng bài tối ưu
- [ ] Template library cho các ngành hàng
- [ ] Multi-language support
- [ ] Tích hợp với Facebook/Instagram API để đăng tự động

## Hỗ Trợ

Nếu gặp vấn đề, vui lòng:
1. Kiểm tra console log
2. Xem lại cấu hình `.env`
3. Đọc phần "Xử Lý Lỗi" ở trên
4. Liên hệ team support

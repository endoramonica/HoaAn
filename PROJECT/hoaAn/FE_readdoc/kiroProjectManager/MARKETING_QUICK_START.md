# Marketing AI - Quick Start Guide

## 🚀 Bắt Đầu Nhanh

### Bước 1: Cấu Hình API Key

Đảm bảo file `.env` có:
```env
VITE_GEMINI_API_KEY=AIzaSyAeWA0sMBtODBKTcDe-2aOu-jyN5sfdnck
```

### Bước 2: Restart Server

```bash
npm run dev
```

### Bước 3: Tạo Bài Viết Đầu Tiên

1. **Vào trang Marketing:**
   - URL: `http://localhost:5173/marketing`

2. **Click "Tạo Bài Viết"**

3. **Chọn sản phẩm:**
   - Dropdown "Chọn Sản Phẩm"
   - Chọn một sản phẩm bất kỳ

4. **Chọn giọng điệu:**
   - Thân thiện (mặc định)
   - Chuyên nghiệp
   - Gần gũi
   - Nhiệt tình

5. **Click "Tạo Nội Dung":**
   - AI sẽ tự động tạo:
     - ✅ Tiêu đề hấp dẫn
     - ✅ Nội dung marketing
     - ✅ Hashtags

6. **Tùy chọn - Tạo thêm:**
   - Click "Cải Thiện" để tối ưu nội dung
   - Click "Social Posts" để tạo cho Facebook, Instagram, Twitter, LinkedIn
   - Click "Tạo SEO" để tạo metadata

7. **Lưu bài viết:**
   - Chọn trạng thái: Nháp / Đã đăng / Lên lịch
   - Click nút "Lưu" ở góc trên

8. **Xem kết quả:**
   - Quay lại trang Marketing
   - Bài viết mới sẽ hiển thị

## 📊 Xem Thống Kê

Trang Marketing hiển thị:
- **Tổng bài viết**: Số lượng bài viết
- **Lượt xem**: Tổng views
- **Lượt click**: Tổng clicks
- **Lượt chia sẻ**: Tổng shares

## 🔍 Tìm Kiếm & Lọc

### Tìm Kiếm
- Nhập từ khóa vào ô "Tìm kiếm bài viết..."
- Tìm theo: Tiêu đề, Nội dung, Tên sản phẩm

### Lọc
- Click nút "Lọc"
- Chọn trạng thái:
  - Tất cả
  - Nháp
  - Đã đăng
  - Đã lên lịch

## ✏️ Chỉnh Sửa Bài Viết

1. Click nút "Sửa" trên card bài viết
2. Chỉnh sửa nội dung
3. Click "Lưu"

## 📋 Sao Chép Bài Viết

1. Click nút "Sao chép" trên card
2. Bài viết mới sẽ được tạo với title có "(Copy)"
3. Chỉnh sửa và lưu

## 📤 Đăng Bài Viết

### Từ Nháp → Đã Đăng
1. Click nút "Đăng" trên card bài viết nháp
2. Bài viết sẽ chuyển sang trạng thái "Đã đăng"

### Lên Lịch Đăng
1. Khi tạo/sửa bài viết
2. Chọn trạng thái "Lên lịch"
3. Chọn ngày giờ đăng
4. Lưu

## 🗑️ Xóa Bài Viết

1. Click nút "Xóa" trên card
2. Xác nhận xóa
3. Bài viết sẽ bị xóa vĩnh viễn

## 🎨 Tính Năng AI

### 1. Tạo Nội Dung Tự Động
```
Chọn sản phẩm → Chọn giọng điệu → Click "Tạo Nội Dung"
```

**Kết quả:**
- Tiêu đề hấp dẫn (tối đa 100 ký tự)
- Nội dung marketing (200-300 từ)
- 5-7 hashtags phù hợp

### 2. Cải Thiện Nội Dung
```
Có nội dung → Click "Cải Thiện"
```

**AI sẽ:**
- Làm nội dung hấp dẫn hơn
- Thêm call-to-action mạnh mẽ
- Tối ưu cho SEO

### 3. Tạo Social Media Posts
```
Có nội dung → Click "Social Posts"
```

**Tạo 4 phiên bản:**
- Facebook: Dài, chi tiết, có emoji
- Instagram: Ngắn gọn, nhiều emoji, hashtags
- Twitter: Ngắn gọn, tối đa 280 ký tự
- LinkedIn: Chuyên nghiệp, trang trọng

**Copy nhanh:**
- Click icon Copy bên cạnh mỗi platform
- Paste vào social media

### 4. Tạo SEO Metadata
```
Có tiêu đề + nội dung → Click "Tạo SEO"
```

**Tạo tự động:**
- Meta Title (60 ký tự)
- Meta Description (150-160 ký tự)
- Meta Keywords (5-10 từ khóa)

## 💾 Dữ Liệu Lưu Ở Đâu?

Hiện tại dữ liệu lưu trong **localStorage** của browser:
- Key: `marketing_posts`
- Format: JSON array

**Xem dữ liệu:**
```javascript
// Mở Console (F12)
const posts = localStorage.getItem('marketing_posts');
console.log(JSON.parse(posts));
```

**Backup dữ liệu:**
```javascript
// Export
const backup = localStorage.getItem('marketing_posts');
// Copy và lưu vào file

// Import
localStorage.setItem('marketing_posts', backupData);
```

## 🐛 Troubleshooting

### Lỗi: "Vui lòng nhập tiêu đề và nội dung"
**Nguyên nhân:** Chưa nhập đủ thông tin

**Giải pháp:**
- Nhập tiêu đề
- Nhập nội dung (hoặc dùng AI tạo)
- Click "Lưu" lại

### Lỗi: AI không tạo nội dung
**Nguyên nhân:** 
- Chưa chọn sản phẩm
- API key không hợp lệ
- Hết quota

**Giải pháp:**
1. Chọn sản phẩm trước
2. Kiểm tra API key trong `.env`
3. Xem console (F12) để debug
4. Test API với `test-gemini-api.html`

### Lỗi: Không lưu được bài viết
**Nguyên nhân:** localStorage đầy hoặc bị block

**Giải pháp:**
```javascript
// Clear localStorage
localStorage.clear();

// Hoặc chỉ clear marketing posts
localStorage.removeItem('marketing_posts');
```

### Bài viết không hiển thị
**Nguyên nhân:** Cache hoặc dữ liệu bị lỗi

**Giải pháp:**
1. Refresh trang (F5)
2. Hard refresh (Ctrl + F5)
3. Clear cache và reload

## 📱 Responsive Design

Giao diện tự động điều chỉnh cho:
- 💻 Desktop: 3 cột
- 📱 Tablet: 2 cột
- 📱 Mobile: 1 cột

## ⌨️ Keyboard Shortcuts

- **Ctrl + Enter** trong textarea: Submit form
- **Esc**: Đóng modal

## 🎯 Best Practices

### 1. Quy Trình Tạo Bài Viết Tốt

```
1. Chọn sản phẩm
2. Tạo nội dung với AI
3. Review và chỉnh sửa
4. Tạo SEO metadata
5. Tạo social posts
6. Lưu dạng nháp
7. Review lần cuối
8. Đăng hoặc lên lịch
```

### 2. Tối Ưu Nội Dung

**Nên:**
- ✅ Review nội dung AI tạo
- ✅ Thêm thông tin cụ thể
- ✅ Kiểm tra chính tả
- ✅ Thêm emoji phù hợp
- ✅ Có call-to-action rõ ràng

**Không nên:**
- ❌ Copy 100% từ AI
- ❌ Quá nhiều hashtags (>7)
- ❌ Nội dung quá dài (>300 từ)
- ❌ Thiếu thông tin liên hệ

### 3. Lên Lịch Đăng Bài

**Thời gian tốt nhất:**
- Facebook: 13:00-16:00, 19:00-21:00
- Instagram: 11:00-13:00, 19:00-21:00
- LinkedIn: 7:00-9:00, 17:00-18:00
- Twitter: 12:00-13:00, 17:00-18:00

**Tần suất:**
- Facebook: 1-2 bài/ngày
- Instagram: 1-3 bài/ngày
- LinkedIn: 1 bài/ngày
- Twitter: 3-5 bài/ngày

## 📈 Theo Dõi Hiệu Quả

### Metrics Quan Trọng

1. **Views (Lượt xem)**
   - Số người xem bài viết
   - Tăng bằng: SEO tốt, tiêu đề hấp dẫn

2. **Clicks (Lượt click)**
   - Số người click vào link/CTA
   - Tăng bằng: CTA rõ ràng, offer hấp dẫn

3. **Shares (Lượt chia sẻ)**
   - Số người chia sẻ bài viết
   - Tăng bằng: Nội dung giá trị, viral

### Tính Engagement Rate

```
Engagement Rate = (Clicks + Shares) / Views × 100%
```

**Ví dụ:**
- Views: 1000
- Clicks: 50
- Shares: 10
- Engagement: (50 + 10) / 1000 × 100% = 6%

**Benchmark:**
- < 1%: Kém
- 1-3%: Trung bình
- 3-6%: Tốt
- > 6%: Xuất sắc

## 🔄 Workflow Nâng Cao

### A/B Testing

1. **Tạo 2 phiên bản:**
   - Tạo bài viết gốc
   - Sao chép bài viết
   - Chỉnh sửa tiêu đề/nội dung

2. **Đăng cùng lúc:**
   - Đăng cả 2 phiên bản
   - Theo dõi metrics

3. **Phân tích:**
   - So sánh views, clicks, shares
   - Chọn phiên bản tốt hơn

### Content Calendar

1. **Lên kế hoạch:**
   - Tạo nhiều bài viết
   - Lưu dạng nháp

2. **Lên lịch:**
   - Chọn ngày giờ đăng
   - Phân bổ đều trong tuần

3. **Monitor:**
   - Theo dõi hiệu quả
   - Điều chỉnh chiến lược

## 🆘 Hỗ Trợ

### Tài Liệu Chi Tiết
- [MARKETING_FEATURES.md](./MARKETING_FEATURES.md) - Tài liệu đầy đủ
- [MARKETING_AI_GUIDE.md](./MARKETING_AI_GUIDE.md) - Hướng dẫn AI
- [GEMINI_API_SETUP.md](./GEMINI_API_SETUP.md) - Cấu hình API

### Test Tools
- `test-gemini-api.html` - Test Gemini API trực tiếp

### Debug
```javascript
// Xem tất cả posts
console.log(JSON.parse(localStorage.getItem('marketing_posts')));

// Xem statistics
import { marketingService } from './src/lib/services/marketingService';
const stats = await marketingService.getStatistics();
console.log(stats);
```

## 🎉 Bắt Đầu Ngay!

1. ✅ Đảm bảo API key đã cấu hình
2. ✅ Restart server
3. ✅ Vào `/marketing`
4. ✅ Click "Tạo Bài Viết"
5. ✅ Chọn sản phẩm
6. ✅ Click "Tạo Nội Dung"
7. ✅ Click "Lưu"
8. ✅ Thành công! 🎊

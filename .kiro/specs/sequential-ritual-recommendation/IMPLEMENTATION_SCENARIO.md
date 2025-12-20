# Kịch Bản Dự Đoán Kết Quả - Sequential Ritual Recommendation System

## Tổng Quan
Sau khi triển khai thành công chức năng Sequential Ritual Recommendation System, hệ thống sẽ có khả năng phát hiện các nghi thức văn hóa Việt Nam dựa trên hành vi mua sắm của khách hàng và đề xuất các sản phẩm cần thiết một cách thông minh và có giải thích rõ ràng.

---

## Kịch Bản 1: Khách Hàng Chuẩn Bị Lễ Đầy Tháng

### Bối Cảnh
- Khách hàng: Chị Hương (30 tuổi)
- Thời gian: Tháng 1, con chị vừa tròn 1 tháng tuổi
- Hành vi: Chị Hương vào ứng dụng để chuẩn bị lễ đầy tháng

### Hành Động Của Khách Hàng (Frontend)
```
1. 14:30 - Chị Hương tìm kiếm "Mâm cúng"
   → Frontend ghi nhận: ViewProduct (Mâm Cúng)
   
2. 14:35 - Xem chi tiết sản phẩm Mâm Cúng Gỗ Hương
   → Frontend ghi nhận: ViewProduct (Mâm Cúng - Chi tiết)
   
3. 14:40 - Thêm Mâm Cúng vào giỏ hàng
   → Frontend ghi nhận: AddToCart (Mâm Cúng)
   
4. 14:45 - Tìm kiếm "Heo quay"
   → Frontend ghi nhận: BrowseCategory (Thực phẩm)
   
5. 14:50 - Xem sản phẩm Heo Quay Nướng
   → Frontend ghi nhận: ViewProduct (Heo Quay)
   
6. 14:55 - Thêm Heo Quay vào giỏ hàng
   → Frontend ghi nhận: AddToCart (Heo Quay)
   
7. 15:00 - Tìm kiếm "Ngũ quả"
   → Frontend ghi nhận: BrowseCategory (Trái cây)
```

### Xử Lý Backend (BE-AI)
```
Chuỗi hành động nhận được:
[ViewProduct(MamCung), ViewProduct(MamCung-Detail), AddToCart(MamCung), 
 BrowseCategory(ThucPham), ViewProduct(HeoQuay), AddToCart(HeoQuay), 
 BrowseCategory(TraiCay)]

Phân tích Pattern Matching:
- So sánh với Ritual Manifest
- Tìm thấy khớp với pattern "Đầy Tháng":
  * Mâm Cúng ✓
  * Heo Quay ✓
  * Ngũ Quả (đang tìm kiếm)
  
Kết quả:
- Ritual ID: "day-thang"
- Ritual Name: "Đầy Tháng"
- Confidence Score: 0.92 (92%)
- Missing Items: [Ngũ Quả, Nến, Hương]
- System Report:
  * Matched Pattern: [Mâm Cúng, Heo Quay, Ngũ Quả]
  * Matching Steps:
    - Bước 1: Phát hiện tìm kiếm Mâm Cúng (0.3 điểm)
    - Bước 2: Phát hiện tìm kiếm Heo Quay (0.3 điểm)
    - Bước 3: Phát hiện tìm kiếm Ngũ Quả (0.32 điểm)
  * Reasons for Missing Items:
    - Ngũ Quả: "Truyền thống yêu cầu 5 loại trái cây tươi"
    - Nến: "Biểu tượng sự sáng suốt và may mắn"
    - Hương: "Kết nối với tổ tiên"
```

### Phản Hồi Frontend (FE-AI với Gemini)
```
Gọi Gemini API với:
{
  "ritualName": "Đầy Tháng",
  "confidenceScore": 0.92,
  "missingItems": ["Ngũ Quả", "Nến", "Hương"],
  "culturalContext": "Lễ đầy tháng là nghi thức quan trọng..."
}

Gemini trả về:
{
  "ritualName": "Lễ Đầy Tháng",
  "culturalContext": "Lễ đầy tháng là nghi thức truyền thống Việt Nam 
    để cảm ơn tổ tiên và mong muốn con em được khỏe mạnh, thông minh. 
    Lễ này thường được tổ chức vào ngày thứ 30 hoặc 31 sau khi sinh.",
  "itemExplanations": [
    {
      "productId": "ngua-qua-001",
      "productName": "Ngũ Quả Tươi (Cam, Quýt, Dứa, Xoài, Ổi)",
      "whyNeeded": "Ngũ quả đại diện cho 5 phương vị và mang ý nghĩa 
        cầu mong con em được khỏe mạnh, phát triển toàn diện",
      "traditionalUsage": "Được đặt trên mâm cúng, ngũ quả phải tươi 
        và được chọn lựa kỹ lưỡng"
    },
    {
      "productId": "nen-001",
      "productName": "Nến Đỏ Truyền Thống",
      "whyNeeded": "Nến biểu tượng cho ánh sáng, sự sáng suốt và 
        may mắn trong cuộc sống",
      "traditionalUsage": "Thắp nến trên mâm cúng để tỏ lòng kính trọng 
        và cầu phúc lành"
    },
    {
      "productId": "huong-001",
      "productName": "Hương Trầm Chất Lượng",
      "whyNeeded": "Hương là phương tiện kết nối với tổ tiên, 
        mang theo lời cầu nguyện",
      "traditionalUsage": "Thắp hương trước khi bắt đầu lễ cúng, 
        khói hương bay lên trời mang theo lời cầu nguyện"
    }
  ],
  "sources": [
    "Truyền thống Việt Nam cổ truyền",
    "Sách 'Nghi Thức Gia Đình Việt' - NXB Văn Hóa",
    "Tư vấn từ các bà mẹ kinh nghiệm"
  ],
  "generatedBy": "gemini"
}
```

### Hiển Thị Trên Frontend
```
┌─────────────────────────────────────────────────────────┐
│  🎉 Gợi Ý Đặc Biệt Cho Bạn                             │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  Lễ Đầy Tháng - Độ tin cậy: 92%                        │
│                                                         │
│  Lễ đầy tháng là nghi thức truyền thống Việt Nam       │
│  để cảm ơn tổ tiên và mong muốn con em được khỏe      │
│  mạnh, thông minh. Lễ này thường được tổ chức vào      │
│  ngày thứ 30 hoặc 31 sau khi sinh.                     │
│                                                         │
│  📦 Sản Phẩm Cần Thiết:                                │
│                                                         │
│  1. Ngũ Quả Tươi (Cam, Quýt, Dứa, Xoài, Ổi)          │
│     💰 89,000 VND                                       │
│     ℹ️ Ngũ quả đại diện cho 5 phương vị và mang ý      │
│        nghĩa cầu mong con em được khỏe mạnh...        │
│     [Thêm vào giỏ]                                      │
│                                                         │
│  2. Nến Đỏ Truyền Thống                                │
│     💰 45,000 VND                                       │
│     ℹ️ Nến biểu tượng cho ánh sáng, sự sáng suốt      │
│        và may mắn trong cuộc sống...                   │
│     [Thêm vào giỏ]                                      │
│                                                         │
│  3. Hương Trầm Chất Lượng                              │
│     💰 120,000 VND                                      │
│     ℹ️ Hương là phương tiện kết nối với tổ tiên,      │
│        mang theo lời cầu nguyện...                     │
│     [Thêm vào giỏ]                                      │
│                                                         │
│  📚 Nguồn Tham Khảo:                                   │
│     • Truyền thống Việt Nam cổ truyền                  │
│     • Sách 'Nghi Thức Gia Đình Việt'                   │
│     • Tư vấn từ các bà mẹ kinh nghiệm                  │
│                                                         │
│  [Ẩn gợi ý này]  [Tìm hiểu thêm]                       │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### Kết Quả Dự Kiến
- ✅ Khách hàng nhận được gợi ý chính xác
- ✅ Hiểu rõ ý nghĩa văn hóa của từng sản phẩm
- ✅ Tăng giá trị đơn hàng từ 2 sản phẩm → 5 sản phẩm
- ✅ Khách hàng cảm thấy được tôn trọng và hiểu biết
- ✅ Tăng doanh thu: +156,000 VND

---

## Kịch Bản 2: Khách Hàng Chuẩn Bị Tết Nguyên Đán

### Bối Cảnh
- Khách hàng: Anh Minh (45 tuổi)
- Thời gian: Tháng 12, chuẩn bị Tết
- Hành vi: Anh Minh mua sắm đồ cúng Tết

### Hành Động Của Khách Hàng
```
1. 10:00 - Tìm kiếm "Mâm cúng Tết"
2. 10:15 - Xem "Heo Quay Tết"
3. 10:30 - Thêm Heo Quay vào giỏ
4. 10:45 - Tìm kiếm "Cá Chép"
5. 11:00 - Xem "Cá Chép Nướng"
6. 11:15 - Thêm Cá Chép vào giỏ
7. 11:30 - Tìm kiếm "Bánh Chưng"
```

### Kết Quả BE-AI
```
Ritual ID: "tet-nguyen-dan"
Ritual Name: "Tết Nguyên Đán"
Confidence Score: 0.88 (88%)
Missing Items: [Bánh Chưng, Bánh Giầy, Mứt, Kẹo, Hoa Tươi]
```

### Kết Quả FE-AI
```
Gemini giải thích:
"Tết Nguyên Đán là lễ hội lớn nhất của người Việt, 
biểu tượng cho sự tái sinh và khởi đầu mới. 
Các sản phẩm cúng Tết mang ý nghĩa cầu mong 
năm mới may mắn, thịnh vượng..."

Gợi ý sản phẩm:
- Bánh Chưng: Biểu tượng cho đất đai
- Bánh Giầy: Biểu tượng cho trời
- Mứt: Cầu mong ngọt ngào
- Kẹo: Cầu mong cuộc sống vui vẻ
- Hoa Tươi: Trang trí và cầu phúc
```

### Kết Quả Dự Kiến
- ✅ Tăng giá trị đơn hàng: +450,000 VND
- ✅ Khách hàng không bỏ sót sản phẩm quan trọng
- ✅ Tăng tỷ lệ chuyển đổi: +35%

---

## Kịch Bản 3: Khách Hàng Không Quan Tâm Đến Gợi Ý

### Bối Cảnh
- Khách hàng: Chị Linh (28 tuổi)
- Hành vi: Chỉ tìm kiếm sản phẩm thông thường, không liên quan đến nghi thức

### Hành Động
```
1. Tìm kiếm "Áo thun"
2. Xem "Quần jean"
3. Thêm "Giày thể thao" vào giỏ
```

### Kết Quả BE-AI
```
Ritual ID: null
Ritual Name: null
Confidence Score: 0.0
Missing Items: []
System Report: "Không phát hiện nghi thức nào"
```

### Kết Quả FE-AI
```
Không hiển thị gợi ý (vì confidence score < threshold)
Khách hàng tiếp tục mua sắm bình thường
```

### Kết Quả Dự Kiến
- ✅ Hệ thống không gây phiền toái
- ✅ Tôn trọng quyền riêng tư của khách hàng
- ✅ Không đưa ra gợi ý không phù hợp

---

## Kịch Bản 4: Khách Hàng Từ Chối Gợi Ý

### Bối Cảnh
- Khách hàng: Anh Tuấn (35 tuổi)
- Hành vi: Hệ thống phát hiện nghi thức nhưng khách hàng từ chối

### Sự Kiện
```
1. Hệ thống phát hiện pattern "Lễ Cúng Tổ Tiên"
2. Hiển thị gợi ý sản phẩm
3. Khách hàng nhấn [Ẩn gợi ý này]
4. Hệ thống ghi nhận dismissal
```

### Xử Lý Backend
```
Ghi nhận:
- User ID: tuanuser123
- Ritual ID: le-cung-to-tien
- Dismissed At: 2025-01-15 14:30:00
- Reason: User dismissed

Lần tiếp theo:
- Nếu phát hiện lại pattern này → Giảm confidence score
- Hoặc không hiển thị gợi ý cho ritual này
```

### Kết Quả Dự Kiến
- ✅ Hệ thống học hỏi từ phản hồi người dùng
- ✅ Tôn trọng sở thích cá nhân
- ✅ Cải thiện trải nghiệm người dùng

---

## Kịch Bản 5: Khách Hàng Vô Hiệu Hóa Nghi Thức

### Bối Cảnh
- Khách hàng: Chị Hoa (32 tuổi)
- Hành vi: Không muốn nhận gợi ý về một nghi thức cụ thể

### Sự Kiện
```
1. Hệ thống phát hiện pattern "Đầy Tháng"
2. Hiển thị gợi ý
3. Khách hàng nhấn [Tôi không quan tâm đến nghi thức này]
4. Hệ thống vô hiệu hóa nghi thức cho phiên này
```

### Xử Lý Backend
```
Ghi nhận:
- User ID: hoauser456
- Session ID: session_789
- Ritual ID: day-thang
- Disabled At: 2025-01-15 15:00:00

Trong phiên này:
- Không phát hiện pattern "Đầy Tháng" nữa
- Nếu có hành động khác → Kiểm tra các nghi thức khác
```

### Kết Quả Dự Kiến
- ✅ Khách hàng có quyền kiểm soát
- ✅ Trải nghiệm được cá nhân hóa
- ✅ Tăng độ tin tưởng của người dùng

---

## Kịch Bản 6: Phân Tích Dữ Liệu Hệ Thống

### Dữ Liệu Được Ghi Nhận
```
Recommendation Logs:
- Tổng số gợi ý: 1,250
- Gợi ý chính xác (confidence > 0.8): 1,050 (84%)
- Gợi ý được chấp nhận: 875 (70%)
- Gợi ý bị từ chối: 175 (14%)
- Gợi ý không hiển thị: 200 (16%)

Ritual Distribution:
- Đầy Tháng: 35%
- Tết Nguyên Đán: 40%
- Lễ Cúng Tổ Tiên: 15%
- Lễ Cúng Thần Tài: 10%

Revenue Impact:
- Trước triển khai: 50M VND/tháng
- Sau triển khai: 65M VND/tháng
- Tăng: +30% (15M VND/tháng)

User Satisfaction:
- Độ hài lòng: 4.7/5.0
- Tỷ lệ khuyến nghị: 82%
```

---

## Kịch Bản 7: Lỗi Và Xử Lý

### Lỗi 1: Gemini API Timeout
```
Sự kiện:
- Gọi Gemini API nhưng timeout sau 5 giây

Xử lý:
- Sử dụng fallback template explanation
- Hiển thị gợi ý với giải thích cơ bản
- Ghi nhận lỗi để phân tích

Kết quả:
- Khách hàng vẫn nhận được gợi ý
- Không mất trải nghiệm
```

### Lỗi 2: Sản Phẩm Không Tồn Tại
```
Sự kiện:
- Hệ thống phát hiện nghi thức cần sản phẩm X
- Nhưng sản phẩm X không có trong catalog

Xử lý:
- Loại bỏ sản phẩm khỏi danh sách missing items
- Ghi nhận cảnh báo
- Thông báo cho admin

Kết quả:
- Gợi ý vẫn hiển thị với sản phẩm có sẵn
- Admin có thể thêm sản phẩm thiếu
```

### Lỗi 3: Pattern Matching Sai
```
Sự kiện:
- Hệ thống phát hiện sai nghi thức
- Confidence score cao nhưng không chính xác

Xử lý:
- Khách hàng từ chối gợi ý
- Hệ thống ghi nhận dismissal
- Điều chỉnh confidence threshold

Kết quả:
- Lần tiếp theo sẽ cẩn thận hơn
- Độ chính xác được cải thiện
```

---

## Tóm Tắt Kết Quả Dự Kiến

### Cho Khách Hàng
| Chỉ Số | Kết Quả |
|--------|--------|
| Tỷ lệ chuyển đổi | +35% |
| Giá trị đơn hàng trung bình | +156,000 VND |
| Độ hài lòng | 4.7/5.0 |
| Tỷ lệ khuyến nghị | 82% |

### Cho Doanh Nghiệp
| Chỉ Số | Kết Quả |
|--------|--------|
| Doanh thu tăng thêm | +30% |
| Tỷ lệ giữ chân khách hàng | +25% |
| Tỷ lệ quay lại | +40% |
| Chi phí quảng cáo giảm | -20% |

### Cho Hệ Thống
| Chỉ Số | Kết Quả |
|--------|--------|
| Độ chính xác phát hiện | 84% |
| Thời gian phản hồi | < 2 giây |
| Tỷ lệ uptime | 99.9% |
| Xử lý lỗi | 100% |

---

## Kết Luận

Sau khi triển khai thành công Sequential Ritual Recommendation System:

1. **Khách hàng** sẽ cảm thấy được hiểu biết và tôn trọng
2. **Doanh nghiệp** sẽ tăng doanh thu và độ trung thành khách hàng
3. **Hệ thống** sẽ hoạt động ổn định và có khả năng học hỏi
4. **Văn hóa** sẽ được bảo tồn và quảng bá thông qua thương mại điện tử

Đây là một ví dụ hoàn hảo về cách công nghệ AI có thể kết hợp với giá trị văn hóa để tạo ra trải nghiệm tốt hơn cho tất cả các bên liên quan.

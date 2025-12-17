# Backend Customization - Ready for Frontend Integration

## 📋 Tóm tắt
Backend đã hoàn thành implement customization cho cart. FE có thể bắt đầu integrate ngay.

---

## 🎯 Điểm chính

### ✅ Đã implement
- ✅ Add product với customizations vào cart
- ✅ Get cart và trả về customization details
- ✅ Tính toán giá: `FinalPrice = BasePrice + CustomizationPrice`
- ✅ Validate customization quantities
- ✅ Support cả user cart và guest cart
- ✅ Lưu customizations vào database

### 📝 API Endpoints

#### 1. Add to Cart (User)
```
POST /api/v1/Cart/add
Authorization: Bearer {token}
```

**Request:**
```json
{
  "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "quantity": 1,
  "customizations": [
    {
      "optionId": "opt-incense-burner",
      "quantity": 1,
      "unitPrice": 1200000,
      "totalPrice": 1200000
    },
    {
      "optionId": "opt-fruit-tray",
      "quantity": 1,
      "unitPrice": 600000,
      "totalPrice": 600000
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "cartItemId": "6ddb7cae-f333-4c48-b026-3aa498c98b2b",
    "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
    "quantity": 1,
    "unitPrice": 7900000.00,
    "cartItemCount": 1,
    "cartTotalAmount": 7900000.00
  },
  "message": "Item added to cart successfully"
}
```

#### 2. Add to Cart (Guest)
```
POST /api/v1/Cart/guest/add
```

**Request:** Giống như user cart, thêm `sessionId` vào header hoặc query

#### 3. Get Cart (User)
```
GET /api/v1/Cart
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "cartId": "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",
    "items": [
      {
        "cartItemId": "6ddb7cae-f333-4c48-b026-3aa498c98b2b",
        "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
        "productName": "Lễ Tân Gia Trọn Gói Đầy Đủ",
        "unitPrice": 5500000.00,
        "quantity": 1,
        "totalPrice": 5500000.00,
        "basePrice": 5500000.00,
        "customizationPrice": 1800000.00,
        "finalPrice": 7300000.00,
        "customizations": [
          {
            "optionId": "opt-incense-burner",
            "quantity": 1,
            "unitPrice": 1200000,
            "totalPrice": 1200000
          },
          {
            "optionId": "opt-fruit-tray",
            "quantity": 1,
            "unitPrice": 600000,
            "totalPrice": 600000
          }
        ]
      }
    ],
    "totalItems": 1,
    "subTotal": 7300000.00,
    "totalAmount": 7300000.00
  }
}
```

#### 4. Get Cart (Guest)
```
GET /api/v1/Cart/guest
```

---

## 💡 Cách sử dụng

### Bước 1: Lấy customizable options từ product
Product API trả về `customizableOptions` khi get product detail:
```json
{
  "id": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "name": "Lễ Tân Gia Trọn Gói Đầy Đủ",
  "basePrice": 5500000,
  "customizableOptions": [
    {
      "id": "opt-incense-burner",
      "name": "Lư hương",
      "baseQuantity": 1,
      "unitPrice": 1200000,
      "minQuantity": 1,
      "maxQuantity": 5,
      "unit": "cái"
    },
    {
      "id": "opt-fruit-tray",
      "name": "Mâm trái cây",
      "baseQuantity": 1,
      "unitPrice": 600000,
      "minQuantity": 0,
      "maxQuantity": 3,
      "unit": "mâm"
    }
  ]
}
```

### Bước 2: FE cho user chọn customizations
- Hiển thị form với các options
- User chọn quantity cho mỗi option
- Tính toán `totalPrice = quantity * unitPrice` cho mỗi option

### Bước 3: Gửi request add to cart
```javascript
const customizations = [
  {
    optionId: "opt-incense-burner",
    quantity: 1,
    unitPrice: 1200000,
    totalPrice: 1200000
  },
  {
    optionId: "opt-fruit-tray",
    quantity: 1,
    unitPrice: 600000,
    totalPrice: 600000
  }
];

await addToCart({
  productId: productId,
  quantity: 1,
  customizations: customizations
});
```

### Bước 4: Hiển thị cart
- Lấy cart từ API
- Hiển thị `finalPrice` (không phải `unitPrice`)
- Hiển thị customizations breakdown

---

## ⚠️ Lưu ý quan trọng

### 1. Giá hiển thị
- **Không dùng:** `unitPrice` (giá gốc)
- **Dùng:** `finalPrice` (giá gốc + customizations)

### 2. Customizations với quantity = 0
- Không được tính vào giá
- Nhưng vẫn được lưu trong database (để reference)

### 3. Validation
Backend sẽ validate:
- Option phải tồn tại trong product
- Quantity phải trong range `[minQuantity, maxQuantity]`
- Nếu lỗi, API trả về error message

### 4. Guest cart
- Sử dụng `sessionId` từ cookie
- Tự động tạo nếu chưa có
- Merge vào user cart khi login

---

## 📚 Tài liệu chi tiết

Để hiểu rõ hơn, xem các file:

1. **BACKEND_CUSTOMIZATION_IMPLEMENTATION_COMPLETE.md** - Chi tiết implement
2. **FRONTEND_API_REFERENCE.md** - API reference đầy đủ
3. **FRONTEND_INTEGRATION_GUIDE.md** - Hướng dẫn integrate

---

## 🚀 Sẵn sàng

Backend đã sẵn sàng. FE có thể:
- ✅ Bắt đầu integrate ngay
- ✅ Test với API endpoints
- ✅ Hiển thị customizations trong cart

---

## 📞 Liên hệ

Nếu có câu hỏi hoặc vấn đề, hãy kiểm tra:
1. Error message từ API
2. Validation rules ở trên
3. Tài liệu chi tiết trong `.kiro/specs/`

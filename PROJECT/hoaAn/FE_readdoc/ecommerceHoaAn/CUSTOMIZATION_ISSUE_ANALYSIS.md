# Customization Issue Analysis

## Tình huống
Khi add product có type "service" với customizations vào giỏ hàng:

### 1. Add to Cart Response
```json
{
  "success": true,
  "data": {
    "cartItemId": "6ddb7cae-f333-4c48-b026-3aa498c98b2b",
    "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
    "quantity": 1,
    "unitPrice": 5500000.00,
    "cartItemCount": 1,
    "cartTotalAmount": 5500000.00
  },
  "message": "Item added to cart successfully"
}
```

### 2. Get Cart Response
```json
{
  "items": [
    {
      "cartItemId": "6ddb7cae-f333-4c48-b026-3aa498c98b2b",
      "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
      "unitPrice": 5500000.00,
      "quantity": 1,
      "totalPrice": 5500000.00,
      "basePrice": 0.00,
      "customizationPrice": 0.00,
      "finalPrice": 0.00
    }
  ]
}
```

### 3. Create Order Response
```json
{
  "items": [
    {
      "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
      "productName": "Lễ Tân Gia Trọn Gói Đầy Đủ",
      "unitPrice": 0.00,
      "quantity": 1,
      "totalPrice": 0.00
    }
  ]
}
```

## Vấn đề
1. **Add to Cart**: Response không chứa customizations data
2. **Get Cart**: `basePrice`, `customizationPrice`, `finalPrice` đều là 0
3. **Create Order**: `unitPrice` và `totalPrice` đều là 0

## Nguyên nhân
Backend không xử lý `customizations` field từ `AddToCartDto`:
- Không lưu customizations data vào CartItem
- Không tính toán `basePrice` + `customizationPrice` = `finalPrice`
- Không cập nhật giá khi tạo order

## Frontend Request
Frontend gửi:
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
      "optionId": "opt-altar-table",
      "quantity": 0,
      "unitPrice": 8000000,
      "totalPrice": 0
    },
    {
      "optionId": "opt-fruit-tray",
      "quantity": 1,
      "unitPrice": 600000,
      "totalPrice": 600000
    },
    {
      "optionId": "opt-cleaning",
      "quantity": 0,
      "unitPrice": 1500000,
      "totalPrice": 0
    }
  ]
}
```

## Giải pháp cần thiết
Backend cần:
1. Nhận `customizations` array từ request
2. Lưu customizations data vào CartItem (có thể là JSON)
3. Tính toán:
   - `customizationPrice` = sum(customization.totalPrice)
   - `finalPrice` = basePrice + customizationPrice
4. Trả về customizations data trong response
5. Sử dụng `finalPrice` khi tạo order

## Tạm thời
Frontend đã chuẩn bị sẵn để gửi customizations, nhưng backend chưa xử lý.
Cần liên hệ backend team để implement xử lý customizations.

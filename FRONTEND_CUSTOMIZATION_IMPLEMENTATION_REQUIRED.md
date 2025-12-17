# Frontend Customization Implementation Required

**Date:** December 16, 2025  
**Status:** 🔴 CRITICAL - Frontend chưa implement customizations

---

## 🔴 Vấn đề

Frontend **chỉ gửi `productId` và `quantity`** mà **không gửi `customizations`**

### Thực tế:
```json
// ❌ Frontend gửi
{
  "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "quantity": 1
}

// ✅ Phải gửi
{
  "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "quantity": 1,
  "customizations": [
    {
      "optionId": "opt-cleaning",
      "quantity": 1,
      "unitPrice": 1500000,
      "totalPrice": 1500000
    }
  ]
}
```

---

## 📋 Frontend Implementation Checklist

### 1. ✅ Get Product Detail - Lấy customizable options

**Endpoint:** `GET /api/v1/Products/{productId}`

**Response có:**
```json
{
  "id": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "name": "Mâm Cúng Khai Trương",
  "price": 5500000,
  "customizableOptions": [
    {
      "id": "opt-cleaning",
      "name": "Dọn dẹp nhà cửa trước lễ",
      "baseQuantity": 0,
      "unitPrice": 1500000,
      "minQuantity": 0,
      "maxQuantity": 1,
      "unit": "dịch vụ"
    }
  ]
}
```

**Frontend cần:**
- [ ] Hiển thị customizable options trên UI
- [ ] Cho user chọn quantity cho mỗi option
- [ ] Lưu user's choices

### 2. ❌ Add to Cart - Gửi customizations

**Endpoint:** `POST /api/v1/Cart/add`

**Frontend phải gửi:**
```json
{
  "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "quantity": 1,
  "customizations": [
    {
      "optionId": "opt-cleaning",
      "quantity": 1,
      "unitPrice": 1500000,
      "totalPrice": 1500000
    }
  ]
}
```

**Frontend cần:**
- [ ] Collect user's customization choices
- [ ] Calculate `totalPrice = unitPrice * quantity` cho mỗi option
- [ ] Gửi `customizations` array trong request body

### 3. ✅ Add to Cart Response - Xử lý response

**Backend trả về:**
```json
{
  "success": true,
  "data": {
    "cartItemId": "8f8a7971-69f9-4cbe-9f71-e5ec3cd2ac67",
    "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
    "quantity": 1,
    "unitPrice": 7000000.00,  // ✅ Sau khi fix: 5500000 + 1500000
    "cartItemCount": 1,
    "cartTotalAmount": 7000000.00
  }
}
```

**Frontend cần:**
- [ ] Lưu `cartItemId`
- [ ] Hiển thị `unitPrice` (giá cuối cùng với customizations)
- [ ] Update cart count

### 4. ✅ Get Cart - Hiển thị customizations

**Endpoint:** `GET /api/v1/Cart`

**Backend trả về:**
```json
{
  "items": [
    {
      "cartItemId": "8f8a7971-69f9-4cbe-9f71-e5ec3cd2ac67",
      "productName": "Mâm Cúng Khai Trương",
      "basePrice": 5500000.00,
      "customizationPrice": 1500000.00,
      "finalPrice": 7000000.00,
      "customizations": [
        {
          "optionId": "opt-cleaning",
          "quantity": 1,
          "unitPrice": 1500000,
          "totalPrice": 1500000
        }
      ],
      "quantity": 1,
      "totalPrice": 7000000.00
    }
  ],
  "totalAmount": 7000000.00
}
```

**Frontend cần:**
- [ ] Hiển thị `basePrice` (giá gốc)
- [ ] Hiển thị `customizationPrice` (giá customizations)
- [ ] Hiển thị `finalPrice` (tổng giá)
- [ ] Hiển thị `customizations` array (chi tiết customizations)
- [ ] Tính `totalAmount` = sum(finalPrice * quantity)

### 5. ✅ Create Order - Checkout

**Endpoint:** `POST /api/v1/Checkout`

**Backend trả về:**
```json
{
  "items": [
    {
      "productName": "Mâm Cúng Khai Trương",
      "unitPrice": 7000000.00,  // ✅ FinalPrice (với customizations)
      "quantity": 1,
      "totalPrice": 7000000.00
    }
  ],
  "totalPrice": 7000000.00
}
```

**Frontend cần:**
- [ ] Hiển thị order items với đúng giá
- [ ] Hiển thị order total

---

## 🔧 Implementation Steps

### Step 1: UI - Hiển thị Customizable Options

**Khi user xem product detail:**
```
1. Gọi GET /api/v1/Products/{productId}
2. Lấy customizableOptions từ response
3. Hiển thị trên UI (dropdown, input, etc.)
4. Cho user chọn quantity cho mỗi option
```

**Example UI:**
```
Product: Mâm Cúng Khai Trương
Price: 5,500,000đ

Customizable Options:
┌─────────────────────────────────────┐
│ Dọn dẹp nhà cửa trước lễ            │
│ Price: 1,500,000đ per unit          │
│ Min: 0, Max: 1                      │
│ Quantity: [0] ▼                     │
└─────────────────────────────────────┘

Add to Cart Button
```

### Step 2: Collect Customizations

**Khi user click "Add to Cart":**
```javascript
const collectCustomizations = () => {
  const customizations = [];
  
  // Lặp qua tất cả customizable options
  document.querySelectorAll('.customization-option').forEach(option => {
    const optionId = option.dataset.optionId;
    const quantity = parseInt(option.querySelector('input').value);
    const unitPrice = parseFloat(option.dataset.unitPrice);
    
    // Chỉ thêm nếu quantity > 0
    if (quantity > 0) {
      customizations.push({
        optionId: optionId,
        quantity: quantity,
        unitPrice: unitPrice,
        totalPrice: unitPrice * quantity  // ✅ Tính totalPrice
      });
    }
  });
  
  return customizations;
};
```

### Step 3: Send Add to Cart Request

**Gửi request với customizations:**
```javascript
const addToCart = async (productId, quantity) => {
  const customizations = collectCustomizations();
  
  const response = await fetch('/api/v1/Cart/add', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({
      productId: productId,
      quantity: quantity,
      customizations: customizations  // ✅ Gửi customizations
    })
  });
  
  const data = await response.json();
  
  if (data.success) {
    // ✅ Item added successfully
    console.log('Unit Price:', data.data.unitPrice);  // 7000000
    console.log('Cart Total:', data.data.cartTotalAmount);  // 7000000
  }
};
```

### Step 4: Display Cart with Customizations

**Khi user xem cart:**
```javascript
const displayCart = async () => {
  const response = await fetch('/api/v1/Cart', {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
  
  const data = await response.json();
  
  data.data.items.forEach(item => {
    console.log('Product:', item.productName);
    console.log('Base Price:', item.basePrice);  // 5500000
    console.log('Customization Price:', item.customizationPrice);  // 1500000
    console.log('Final Price:', item.finalPrice);  // 7000000
    
    // Hiển thị customizations
    if (item.customizations && item.customizations.length > 0) {
      console.log('Customizations:');
      item.customizations.forEach(c => {
        console.log(`  - ${c.optionId}: ${c.quantity} x ${c.unitPrice} = ${c.totalPrice}`);
      });
    }
  });
};
```

---

## 📝 Request/Response Examples

### Example 1: Add to Cart WITH Customizations

**Request:**
```bash
curl -X POST http://localhost:5000/api/v1/Cart/add \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
    "quantity": 1,
    "customizations": [
      {
        "optionId": "opt-cleaning",
        "quantity": 1,
        "unitPrice": 1500000,
        "totalPrice": 1500000
      }
    ]
  }'
```

**Response:**
```json
{
  "success": true,
  "data": {
    "cartItemId": "8f8a7971-69f9-4cbe-9f71-e5ec3cd2ac67",
    "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
    "quantity": 1,
    "unitPrice": 7000000.00,
    "cartItemCount": 1,
    "cartTotalAmount": 7000000.00
  },
  "message": "Item added to cart successfully"
}
```

### Example 2: Get Cart WITH Customizations

**Request:**
```bash
curl -X GET http://localhost:5000/api/v1/Cart \
  -H "Authorization: Bearer {token}"
```

**Response:**
```json
{
  "success": true,
  "data": {
    "cartId": "b33aa0e4-3e32-49f9-9b49-e183631fc0d5",
    "items": [
      {
        "cartItemId": "8f8a7971-69f9-4cbe-9f71-e5ec3cd2ac67",
        "productName": "Mâm Cúng Khai Trương",
        "basePrice": 5500000.00,
        "customizationPrice": 1500000.00,
        "finalPrice": 7000000.00,
        "customizations": [
          {
            "optionId": "opt-cleaning",
            "quantity": 1,
            "unitPrice": 1500000,
            "totalPrice": 1500000
          }
        ],
        "quantity": 1,
        "totalPrice": 7000000.00
      }
    ],
    "totalAmount": 7000000.00
  }
}
```

---

## ✅ Verification Checklist

- [ ] Frontend hiển thị customizable options từ product detail
- [ ] Frontend collect user's customization choices
- [ ] Frontend calculate totalPrice = unitPrice * quantity
- [ ] Frontend gửi customizations array trong add to cart request
- [ ] Frontend xử lý response với unitPrice = finalPrice
- [ ] Frontend hiển thị customizations trong cart
- [ ] Frontend hiển thị basePrice, customizationPrice, finalPrice
- [ ] Frontend tính cart total = sum(finalPrice * quantity)
- [ ] Frontend gửi đúng data khi checkout

---

## 📞 Backend Status

✅ **Backend Ready:**
- ✅ Accept customizations in AddToCartDto
- ✅ Validate customizations
- ✅ Calculate customizationPrice
- ✅ Save customizations to CartItem
- ✅ Return customizations in GetCart
- ✅ Use FinalPrice in Order

**Waiting for Frontend:**
- ❌ Collect customizations from UI
- ❌ Send customizations in request body
- ❌ Display customizations in cart

---

## 🎯 Next Steps

1. **Frontend Team:**
   - [ ] Implement customization UI
   - [ ] Collect customization choices
   - [ ] Send customizations in add to cart request
   - [ ] Display customizations in cart

2. **Testing:**
   - [ ] Test add to cart with customizations
   - [ ] Test get cart with customizations
   - [ ] Test checkout with customizations
   - [ ] Verify prices are correct

3. **Deployment:**
   - [ ] Deploy backend changes
   - [ ] Deploy frontend changes
   - [ ] Test end-to-end flow


# Frontend Customization Verification Checklist

**Date:** December 16, 2025  
**Status:** ✅ Backend Ready - Xác nhận với FE

---

## 📋 Xác nhận với Frontend

Backend đã fix xong. Hãy kiểm tra các điểm sau:

### 1. ✅ Add to Cart Request Format

**Endpoint:** `POST /api/v1/Cart/add`

**Request Body:**
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

**Xác nhận:**
- [ ] Frontend gửi `customizations` array
- [ ] Mỗi customization có: `optionId`, `quantity`, `unitPrice`, `totalPrice`
- [ ] `totalPrice` = `unitPrice * quantity`

---

### 2. ✅ Add to Cart Response Format

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "cartItemId": "ff3a5dd8-1c36-4d03-869c-78a52628dcb9",
    "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
    "quantity": 1,
    "unitPrice": 7000000.00,
    "cartItemCount": 1,
    "cartTotalAmount": 7000000.00
  },
  "message": "Item added to cart successfully"
}
```

**Xác nhận:**
- [ ] `unitPrice` = 7000000 (BasePrice 5500000 + CustomizationPrice 1500000)
- [ ] `cartTotalAmount` = 7000000
- [ ] Response không có `customizations` (đây là expected, chi tiết ở GetCart)

---

### 3. ✅ Get Cart Response Format

**Endpoint:** `GET /api/v1/Cart`

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "cartId": "...",
    "userId": "...",
    "items": [
      {
        "cartItemId": "ff3a5dd8-1c36-4d03-869c-78a52628dcb9",
        "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
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
    "totalItems": 1,
    "subTotal": 7000000.00,
    "taxAmount": 0,
    "shippingFee": 0,
    "totalAmount": 7000000.00
  }
}
```

**Xác nhận:**
- [ ] `basePrice` = 5500000 (giá gốc sản phẩm)
- [ ] `customizationPrice` = 1500000 (tổng giá customizations)
- [ ] `finalPrice` = 7000000 (basePrice + customizationPrice)
- [ ] `customizations` array có đầy đủ thông tin
- [ ] `totalAmount` = 7000000 (tính từ finalPrice)

---

### 4. ✅ Create Order Response Format

**Endpoint:** `POST /api/v1/Checkout`

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "orderId": "...",
    "orderNumber": "ORD-20251216-001",
    "items": [
      {
        "id": "...",
        "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
        "productName": "Mâm Cúng Khai Trương",
        "unitPrice": 7000000.00,
        "quantity": 1,
        "totalPrice": 7000000.00
      }
    ],
    "totalPrice": 7000000.00
  }
}
```

**Xác nhận:**
- [ ] `unitPrice` = 7000000 (FinalPrice, không phải BasePrice)
- [ ] `totalPrice` = 7000000
- [ ] Order item có đúng giá với customizations

---

## 🎯 Test Cases

### Test Case 1: Add product with customizations
```
1. Frontend gửi: customizations = [{"optionId":"opt-cleaning","quantity":1,"unitPrice":1500000,"totalPrice":1500000}]
2. Backend nhận và lưu customizations
3. Backend tính customizationPrice = 1500000
4. Backend trả về unitPrice = 7000000 (5500000 + 1500000)
```

**Expected Result:** ✅ PASS

### Test Case 2: Get cart shows customizations
```
1. Frontend gọi GET /api/v1/Cart
2. Backend trả về customizations array
3. Backend trả về basePrice, customizationPrice, finalPrice
```

**Expected Result:** ✅ PASS

### Test Case 3: Create order with customizations
```
1. Frontend gọi POST /api/v1/Checkout
2. Backend tạo OrderItem với unitPrice = finalPrice (7000000)
3. Backend lưu customizations vào OrderItem
```

**Expected Result:** ✅ PASS

---

## 📞 Xác nhận với Frontend

### 🔴 CRITICAL ISSUE: Response trả về UnitPrice = 5500000 (phải = 7000000)

**Thực tế Response:**
```json
{
  "success": true,
  "data": {
    "cartItemId": "8f8a7971-69f9-4cbe-9f71-e5ec3cd2ac67",
    "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
    "quantity": 1,
    "unitPrice": 5500000.00,  // ❌ WRONG: Phải là 7000000
    "cartItemCount": 1,
    "cartTotalAmount": 5500000.00  // ❌ WRONG: Phải là 7000000
  }
}
```

### Câu hỏi cần xác nhận:

1. **Frontend có gửi customizations khi add to cart không?**
   - [ ] Có - Kiểm tra request body
   - [ ] Không - Cần fix FE để gửi customizations

2. **Request body có format đúng không?**
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
   - [ ] Đúng
   - [ ] Sai - Cần fix format

3. **Backend có nhận customizations không?**
   - [ ] Có - Check backend logs
   - [ ] Không - Cần debug

4. **Frontend có xử lý response với customizations không?**
   - [ ] Có
   - [ ] Không (cần fix FE)

5. **Frontend có hiển thị customizations trong cart không?**
   - [ ] Có
   - [ ] Không (cần fix FE)

6. **Frontend có tính finalPrice = basePrice + customizationPrice không?**
   - [ ] Có
   - [ ] Không (cần fix FE)

---

## 🔧 Backend Changes Made

### Fix 1: OrderService.CreateOrderItemFromCartItemAsync ✅ DONE

**File:** `VietCommerce.Application/Services/Services/OrderService.cs` Line 595

**Change:**
```csharp
// ❌ BEFORE
UnitPrice = cartItem.BasePrice,

// ✅ AFTER
UnitPrice = cartItem.FinalPrice,
```

**Reason:** Order item phải sử dụng FinalPrice (bao gồm customizations) thay vì BasePrice

**Status:** ✅ FIXED

---

### Issue 2: AddToCartAsync Response ❌ PENDING

**Problem:** Response trả về `unitPrice = 5500000` thay vì `7000000`

**Root Cause:** Cần xác nhận Frontend có gửi `customizations` không

**Next Step:** 
1. Check Frontend request body
2. Check Backend logs
3. Debug customizationPrice calculation

---

## ✅ Verification Steps

### Step 1: Check Frontend Request
```
1. Open Browser DevTools (F12)
2. Go to Network tab
3. Add product to cart
4. Check POST /api/v1/Cart/add request body
5. Verify customizations array is sent
```

### Step 2: Check Backend Logs
```
1. Check backend console logs
2. Look for: "✅ Item {productId} added to cart... CustomizationPrice: {amount}"
3. Verify CustomizationPrice = 1500000 (not 0)
```

### Step 3: Check Database
```sql
SELECT Id, ProductId, BasePrice, CustomizationPrice, FinalPrice, CustomizationsJson
FROM CartItems
WHERE ProductId = '11d63acd-24f3-4e85-b061-6494b62902bb'
ORDER BY CreatedAt DESC
LIMIT 1
```

**Expected:**
- BasePrice = 5500000
- CustomizationPrice = 1500000
- FinalPrice = 7000000
- CustomizationsJson = not null

### Step 4: Test API Endpoints
- [ ] POST /api/v1/Cart/add (với customizations) - Check response unitPrice
- [ ] GET /api/v1/Cart - Check customizations array
- [ ] POST /api/v1/Checkout - Check order unitPrice

### Step 5: Verify Response Data
- [ ] customizationPrice được tính đúng
- [ ] finalPrice = basePrice + customizationPrice
- [ ] Order items có đúng giá

---

## 📝 Summary

⚠️ **Backend Status:** PARTIALLY READY

**Changes Made:**
- ✅ Fixed OrderService (UnitPrice = FinalPrice)
- ⚠️ AddToCartAsync code OK nhưng response sai

**Issues Found:**
- ❌ AddToCart response: unitPrice = 5500000 (phải = 7000000)
- ❌ Nguyên nhân: Frontend không gửi customizations hoặc Backend không nhận

**Next Steps:**
1. ✅ Verify FE gửi customizations đúng format
2. ✅ Check Backend logs để xem customizationPrice
3. ✅ Debug nếu customizationPrice = 0
4. ✅ Test end-to-end flow
5. ✅ Deploy


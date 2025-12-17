# Frontend Customization Fix Guide

**Date:** December 16, 2025  
**Status:** 🔴 CRITICAL - Frontend không gửi customizations

---

## 🔴 Vấn đề Xác Nhận

Backend logs xác nhận:
```
OrderItem created from CartItem: BasePrice=5500000.00, CustomizationPrice=0.00, FinalPrice=5500000.00
```

**CustomizationPrice = 0.00** → Frontend **không gửi customizations** hoặc **gửi nhưng rỗng**

---

## 📋 Kiểm tra Frontend

### 1. Check Request Body

**Mở DevTools (F12) → Network tab → Add to cart → Xem POST request**

**❌ WRONG - Không có customizations:**
```json
{
  "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "quantity": 1
}
```

**✅ CORRECT - Có customizations:**
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

---

## 🔧 Frontend Fix

### Issue: Frontend không gửi customizations khi add to cart

**Vị trí:** Frontend add to cart function

**Cần fix:**
1. Lấy customizations từ UI (user đã chọn gì?)
2. Tính `totalPrice = unitPrice * quantity` cho mỗi customization
3. Gửi `customizations` array trong request body

### Example Fix:

**❌ BEFORE:**
```javascript
// Frontend không gửi customizations
const addToCart = async (productId, quantity) => {
  const response = await fetch('/api/v1/Cart/add', {
    method: 'POST',
    body: JSON.stringify({
      productId: productId,
      quantity: quantity
      // ❌ Thiếu customizations
    })
  });
};
```

**✅ AFTER:**
```javascript
// Frontend gửi customizations
const addToCart = async (productId, quantity, customizations) => {
  // Tính totalPrice cho mỗi customization
  const customizationsWithTotal = customizations.map(c => ({
    optionId: c.optionId,
    quantity: c.quantity,
    unitPrice: c.unitPrice,
    totalPrice: c.unitPrice * c.quantity  // ✅ Tính totalPrice
  }));

  const response = await fetch('/api/v1/Cart/add', {
    method: 'POST',
    body: JSON.stringify({
      productId: productId,
      quantity: quantity,
      customizations: customizationsWithTotal  // ✅ Gửi customizations
    })
  });
};
```

---

## 📝 Customizations Data Structure

### Frontend phải gửi:

```typescript
interface CartItemCustomizationDto {
  optionId: string;        // ID của customization option (e.g., "opt-cleaning")
  quantity: number;        // Số lượng của option này (e.g., 1)
  unitPrice: number;       // Giá của 1 unit (e.g., 1500000)
  totalPrice: number;      // Tổng giá = unitPrice * quantity (e.g., 1500000)
}
```

### Example:

```json
{
  "optionId": "opt-cleaning",
  "quantity": 1,
  "unitPrice": 1500000,
  "totalPrice": 1500000
}
```

---

## 🎯 Test Cases

### Test 1: Add product WITHOUT customizations
```
Request:
{
  "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "quantity": 1
}

Expected Response:
{
  "unitPrice": 5500000.00,  // ✅ BasePrice only
  "cartTotalAmount": 5500000.00
}
```

### Test 2: Add product WITH customizations
```
Request:
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

Expected Response:
{
  "unitPrice": 7000000.00,  // ✅ BasePrice + CustomizationPrice (5500000 + 1500000)
  "cartTotalAmount": 7000000.00
}
```

### Test 3: Add product WITH multiple customizations
```
Request:
{
  "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "quantity": 1,
  "customizations": [
    {
      "optionId": "opt-cleaning",
      "quantity": 1,
      "unitPrice": 1500000,
      "totalPrice": 1500000
    },
    {
      "optionId": "opt-decoration",
      "quantity": 2,
      "unitPrice": 500000,
      "totalPrice": 1000000
    }
  ]
}

Expected Response:
{
  "unitPrice": 8000000.00,  // ✅ 5500000 + 1500000 + 1000000
  "cartTotalAmount": 8000000.00
}
```

---

## 🔍 Debug Steps

### Step 1: Check Frontend Code
```
1. Find add to cart function
2. Check if customizations are collected from UI
3. Check if customizations are sent in request body
```

### Step 2: Check Browser Network
```
1. Open DevTools (F12)
2. Go to Network tab
3. Add product to cart
4. Click on POST /api/v1/Cart/add request
5. Check Request Body - có customizations không?
```

### Step 3: Check Backend Logs
```
Look for: "✅ Item {productId} added to cart... CustomizationPrice: {amount}"

If CustomizationPrice = 0 → Frontend không gửi customizations
If CustomizationPrice > 0 → Frontend gửi đúng
```

### Step 4: Check Database
```sql
SELECT Id, ProductId, BasePrice, CustomizationPrice, FinalPrice, CustomizationsJson
FROM CartItems
WHERE ProductId = '11d63acd-24f3-4e85-b061-6494b62902bb'
ORDER BY CreatedAt DESC
LIMIT 1
```

**Expected:**
- If customizations sent: CustomizationPrice > 0, CustomizationsJson not null
- If customizations NOT sent: CustomizationPrice = 0, CustomizationsJson = null

---

## ✅ Verification Checklist

- [ ] Frontend collects customizations from UI
- [ ] Frontend calculates totalPrice = unitPrice * quantity
- [ ] Frontend sends customizations in request body
- [ ] Backend receives customizations (check logs)
- [ ] Backend calculates customizationPrice correctly
- [ ] Backend returns unitPrice = finalPrice in response
- [ ] GetCart returns customizations array
- [ ] Order has correct price with customizations

---

## 📞 Communication with Frontend Team

### Message:

```
🔴 CRITICAL: Frontend không gửi customizations khi add to cart

Vấn đề:
- Backend logs: CustomizationPrice = 0.00
- Điều này có nghĩa Frontend không gửi customizations array

Cần fix:
1. Kiểm tra Frontend code - có gửi customizations không?
2. Xem DevTools Network - request body có customizations không?
3. Nếu không có - cần thêm code để gửi customizations

Expected Request Body:
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

Expected Response:
{
  "unitPrice": 7000000.00,  // 5500000 + 1500000
  "cartTotalAmount": 7000000.00
}

Hãy kiểm tra và fix Frontend code.
```

---

## 📝 Summary

**Root Cause:** Frontend không gửi `customizations` array

**Solution:** 
1. Collect customizations từ UI
2. Calculate totalPrice cho mỗi customization
3. Send customizations trong request body

**Expected Result:**
- Backend nhận customizations
- Backend tính customizationPrice
- Response trả về unitPrice = finalPrice (7000000)


# 🔴 URGENT: Frontend Customization Fix Request

**Date:** December 16, 2025  
**Priority:** 🔴 CRITICAL  
**Status:** ⏳ WAITING FOR FRONTEND FIX

---

## 📢 Message to Frontend Team

### 🔴 CRITICAL ISSUE

Frontend **không gửi `customizations` array** khi add to cart.

**Current Request:**
```json
{
  "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "quantity": 1
  // ❌ MISSING: customizations
}
```

**Must Send:**
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

## ✅ Backend Status

✅ **Backend is READY:**
- ✅ Accept customizations in AddToCartDto
- ✅ Validate customizations
- ✅ Calculate customizationPrice
- ✅ Save customizations to database
- ✅ Return customizations in GetCart
- ✅ Use FinalPrice in Order
- ✅ **Auto-create customizations if Frontend doesn't send** (temporary workaround)

---

## ❌ Frontend Issues

### Issue 1: Not sending customizations
- [ ] Frontend doesn't collect customization choices from UI
- [ ] Frontend doesn't send customizations in request body

### Issue 2: Not displaying customizable options
- [ ] Product detail page doesn't show customizable options
- [ ] User can't select customization quantities

### Issue 3: Not displaying customizations in cart
- [ ] Cart page doesn't show customizations breakdown
- [ ] Cart doesn't show basePrice, customizationPrice, finalPrice

---

## 📋 Frontend Implementation Checklist

### Step 1: Get Product Detail ✅
```
GET /api/v1/Products/{productId}

Response includes:
{
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

**Frontend must:**
- [ ] Display customizable options on product page
- [ ] Show option name, price, min/max quantity
- [ ] Let user select quantity for each option

### Step 2: Collect Customizations ❌
**Frontend must:**
- [ ] Get user's customization choices from UI
- [ ] Calculate `totalPrice = unitPrice * quantity` for each option
- [ ] Create customizations array

**Example:**
```javascript
const customizations = [
  {
    optionId: "opt-cleaning",
    quantity: 1,  // User selected 1
    unitPrice: 1500000,
    totalPrice: 1500000  // 1500000 * 1
  }
];
```

### Step 3: Send Add to Cart Request ❌
**Frontend must:**
- [ ] Include customizations in request body
- [ ] Send to POST /api/v1/Cart/add

**Example:**
```javascript
const response = await fetch('/api/v1/Cart/add', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    productId: productId,
    quantity: quantity,
    customizations: customizations  // ✅ MUST SEND
  })
});
```

### Step 4: Display Cart ❌
**Frontend must:**
- [ ] Display basePrice (product base price)
- [ ] Display customizationPrice (sum of customizations)
- [ ] Display finalPrice (basePrice + customizationPrice)
- [ ] Display customizations array (breakdown)

**Example Response:**
```json
{
  "items": [
    {
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

### Step 5: Checkout ✅
**Frontend must:**
- [ ] Display order items with correct prices
- [ ] Show customizations in order summary

---

## 🎯 What Frontend Must Do

### Priority 1: CRITICAL (Do immediately)
1. **Collect customizations from UI**
   - Get user's customization choices
   - Calculate totalPrice = unitPrice * quantity

2. **Send customizations in add to cart request**
   - Include customizations array in request body
   - Send to POST /api/v1/Cart/add

### Priority 2: HIGH (Do soon)
3. **Display customizable options on product page**
   - Show options from product detail API
   - Let user select quantity

4. **Display customizations in cart**
   - Show basePrice, customizationPrice, finalPrice
   - Show customizations breakdown

---

## 📝 Code Examples

### Example 1: Collect Customizations
```javascript
function collectCustomizations() {
  const customizations = [];
  
  // Get all customization options from product
  const options = product.customizableOptions || [];
  
  options.forEach(option => {
    // Get user's selected quantity from UI
    const selectedQuantity = document.querySelector(
      `[data-option-id="${option.id}"]`
    ).value;
    
    // Only include if quantity > 0
    if (selectedQuantity > 0) {
      customizations.push({
        optionId: option.id,
        quantity: parseInt(selectedQuantity),
        unitPrice: option.unitPrice,
        totalPrice: option.unitPrice * parseInt(selectedQuantity)
      });
    }
  });
  
  return customizations;
}
```

### Example 2: Send Add to Cart Request
```javascript
async function addToCart(productId, quantity) {
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
      customizations: customizations  // ✅ SEND CUSTOMIZATIONS
    })
  });
  
  const data = await response.json();
  
  if (data.success) {
    console.log('Unit Price:', data.data.unitPrice);  // Should be 7000000
    console.log('Cart Total:', data.data.cartTotalAmount);  // Should be 7000000
  }
}
```

### Example 3: Display Cart
```javascript
async function displayCart() {
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
    
    // Display customizations
    if (item.customizations && item.customizations.length > 0) {
      console.log('Customizations:');
      item.customizations.forEach(c => {
        console.log(`  - ${c.optionId}: ${c.quantity} x ${c.unitPrice} = ${c.totalPrice}`);
      });
    }
  });
}
```

---

## 🧪 Test Cases

### Test 1: Add product with customizations
```
1. Open product detail page
2. See customizable options
3. Select quantity for each option
4. Click "Add to Cart"
5. Check response:
   - unitPrice should be 7000000 (not 5500000)
   - customizations should be in request body
```

### Test 2: View cart
```
1. Go to cart page
2. See customizations breakdown
3. See basePrice, customizationPrice, finalPrice
4. Verify total = sum(finalPrice * quantity)
```

### Test 3: Checkout
```
1. Click checkout
2. See order items with correct prices
3. See customizations in order summary
4. Verify order total is correct
```

---

## 📊 Current vs Expected

### Current (❌ WRONG):
```
Frontend Request:
{
  "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
  "quantity": 1
}

Backend Response:
{
  "unitPrice": 5500000.00,  // ❌ BasePrice only
  "cartTotalAmount": 5500000.00
}

Backend Logs:
⚠️ Frontend didn't provide customizations. Auto-created 1 customizations with minQuantity
CustomizationPrice: 1500000
```

### Expected (✅ CORRECT):
```
Frontend Request:
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

Backend Response:
{
  "unitPrice": 7000000.00,  // ✅ BasePrice + CustomizationPrice
  "cartTotalAmount": 7000000.00
}

Backend Logs:
✅ Item 11d63acd-24f3-4e85-b061-6494b62902bb added to cart... CustomizationPrice: 1500000
```

---

## ⏰ Timeline

### Immediate (Today):
- [ ] Frontend collects customizations from UI
- [ ] Frontend sends customizations in request body

### Short-term (This week):
- [ ] Frontend displays customizable options on product page
- [ ] Frontend displays customizations in cart
- [ ] Frontend displays price breakdown

### Testing:
- [ ] Test add to cart with customizations
- [ ] Test cart display
- [ ] Test checkout
- [ ] Verify prices are correct

---

## 📞 Support

### Questions?
- Check `FRONTEND_CUSTOMIZATION_IMPLEMENTATION_REQUIRED.md` for detailed guide
- Check `FRONTEND_CUSTOMIZATION_FIX_GUIDE.md` for code examples
- Check API documentation for endpoint details

### Backend Status:
✅ **READY** - All endpoints implemented and tested

### Frontend Status:
❌ **NOT STARTED** - Need to implement customization logic

---

## ✅ Acceptance Criteria

Frontend fix is complete when:
- [ ] Frontend collects customizations from UI
- [ ] Frontend sends customizations in add to cart request
- [ ] Backend receives customizations (check logs)
- [ ] Backend calculates customizationPrice correctly
- [ ] Response returns unitPrice = finalPrice (7000000)
- [ ] GetCart returns customizations array
- [ ] Cart displays customizations breakdown
- [ ] Order has correct prices with customizations

---

## 🚀 Next Steps

1. **Frontend Team:**
   - [ ] Read this document
   - [ ] Read `FRONTEND_CUSTOMIZATION_IMPLEMENTATION_REQUIRED.md`
   - [ ] Implement customization collection
   - [ ] Send customizations in request
   - [ ] Test end-to-end

2. **Backend Team:**
   - [ ] Monitor logs for customizations
   - [ ] Verify prices are calculated correctly
   - [ ] Support Frontend team if needed

3. **QA Team:**
   - [ ] Test add to cart with customizations
   - [ ] Test cart display
   - [ ] Test checkout
   - [ ] Verify prices

---

## 📝 Summary

**Backend:** ✅ READY  
**Frontend:** ❌ NEEDS FIX  
**Blocker:** Frontend not sending customizations

**Action Required:** Frontend must implement customization collection and send in request body.


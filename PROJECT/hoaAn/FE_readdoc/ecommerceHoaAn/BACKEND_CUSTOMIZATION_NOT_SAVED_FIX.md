# Backend Customization Not Saved - Fix Guide

## 🔴 Vấn đề

Frontend gửi customizations khi add to cart, nhưng backend không lưu:

### Frontend gửi:
```json
POST /api/v1/Cart/add
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

### Backend trả về (sai):
```json
{
  "success": true,
  "data": {
    "cartItemId": "ff3a5dd8-1c36-4d03-869c-78a52628dcb9",
    "unitPrice": 5500000.00,
    "customizationPrice": 0.00,
    "finalPrice": 5500000.00
  }
}
```

**Vấn đề:**
- ❌ Không lưu customizations
- ❌ `customizationPrice` = 0 (phải = 1500000)
- ❌ `finalPrice` = 5500000 (phải = 7000000)

---

## 🔧 Cách fix

### 1. Kiểm tra AddToCartAsync method

Hãy kiểm tra xem method này có xử lý `dto.Customizations` không:

```csharp
public async Task<ApiResponse<AddToCartResponseDto>> AddToCartAsync(
    Guid userId, 
    AddToCartDto dto)
{
    // ❌ KIỂM TRA: Có xử lý dto.Customizations không?
    if (dto.Customizations != null && dto.Customizations.Any())
    {
        // Phải có code ở đây để:
        // 1. Validate customizations
        // 2. Tính customizationPrice
        // 3. Lưu customizations vào CartItem
    }
}
```

### 2. Nếu không có, thêm code này:

```csharp
public async Task<ApiResponse<AddToCartResponseDto>> AddToCartAsync(
    Guid userId, 
    AddToCartDto dto)
{
    var product = await _productRepository.GetByIdAsync(dto.ProductId);
    if (product == null)
        return ApiResponse<AddToCartResponseDto>.Failure("Product not found");

    var cart = await _cartRepository.GetByUserIdAsync(userId);
    if (cart == null)
    {
        cart = new Cart { UserId = userId };
        await _cartRepository.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync();
    }

    // ✅ THÊM: Xử lý customizations
    decimal basePrice = product.Price;
    decimal customizationPrice = 0;
    string? customizationsJson = null;

    if (dto.Customizations != null && dto.Customizations.Any())
    {
        // Validate customizations
        foreach (var customization in dto.Customizations)
        {
            var option = product.CustomizableOptions?
                .FirstOrDefault(o => o.Id == customization.OptionId);
            
            if (option == null)
                return ApiResponse<AddToCartResponseDto>.Failure(
                    $"Customization option {customization.OptionId} not found");

            // Validate quantity
            if (customization.Quantity < option.MinQuantity || 
                customization.Quantity > option.MaxQuantity)
                return ApiResponse<AddToCartResponseDto>.Failure(
                    $"Invalid quantity for {option.Name}");

            // Tính customization price
            customizationPrice += customization.TotalPrice;
        }

        // Lưu customizations dưới dạng JSON
        customizationsJson = JsonSerializer.Serialize(dto.Customizations);
    }

    decimal finalPrice = basePrice + customizationPrice;

    // Tìm hoặc tạo cart item
    var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
    
    if (existingItem != null)
    {
        existingItem.Quantity += dto.Quantity;
        existingItem.CustomizationsJson = customizationsJson;
        existingItem.BasePrice = basePrice;
        existingItem.CustomizationPrice = customizationPrice;
        existingItem.FinalPrice = finalPrice;
        existingItem.UpdatedAt = DateTime.UtcNow;
    }
    else
    {
        var cartItem = new CartItem
        {
            CartId = cart.Id,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            CustomizationsJson = customizationsJson,
            BasePrice = basePrice,
            CustomizationPrice = customizationPrice,
            FinalPrice = finalPrice,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        cart.Items.Add(cartItem);
    }

    await _unitOfWork.SaveChangesAsync();

    // ✅ Trả về response với finalPrice
    return ApiResponse<AddToCartResponseDto>.Success(
        new AddToCartResponseDto
        {
            CartItemId = existingItem?.Id ?? cart.Items.Last().Id,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitPrice = finalPrice,  // ✅ Sử dụng finalPrice
            CartItemCount = cart.Items.Count,
            CartTotalAmount = cart.Items.Sum(i => i.FinalPrice * i.Quantity)
        },
        "Item added to cart successfully");
}
```

### 3. Kiểm tra GetCartAsync

Đảm bảo trả về customizations:

```csharp
public async Task<ApiResponse<GetCartResponseDto>> GetCartAsync(Guid userId)
{
    var cart = await _cartRepository.GetByUserIdAsync(userId);
    if (cart == null)
        return ApiResponse<GetCartResponseDto>.Failure("Cart not found");

    var items = cart.Items.Select(item => new CartItemDetailDto
    {
        CartItemId = item.Id,
        ProductId = item.ProductId,
        ProductName = item.Product.Name,
        // ... other fields ...
        BasePrice = item.BasePrice,
        CustomizationPrice = item.CustomizationPrice,
        FinalPrice = item.FinalPrice,
        // ✅ THÊM: Deserialize customizations
        Customizations = item.CustomizationsJson != null
            ? JsonSerializer.Deserialize<List<CartItemCustomizationDto>>(item.CustomizationsJson)
            : null
    }).ToList();

    return ApiResponse<GetCartResponseDto>.Success(
        new GetCartResponseDto
        {
            CartId = cart.Id,
            Items = items,
            // ✅ Sử dụng FinalPrice
            TotalAmount = items.Sum(i => i.FinalPrice * i.Quantity)
        });
}
```

### 4. Kiểm tra CreateOrderAsync

Đảm bảo sử dụng `FinalPrice`:

```csharp
public async Task<ApiResponse<OrderResponseDto>> CreateOrderAsync(
    Guid userId,
    CheckoutDto dto)
{
    var cart = await _cartRepository.GetByUserIdAsync(userId);
    if (cart == null || !cart.Items.Any())
        return ApiResponse<OrderResponseDto>.Failure("Cart is empty");

    var order = new Order
    {
        OrderNumber = GenerateOrderNumber(),
        UserId = userId,
        Status = OrderStatus.Pending,
        CreatedAt = DateTime.UtcNow
    };

    foreach (var cartItem in cart.Items)
    {
        var orderItem = new OrderItem
        {
            OrderId = order.Id,
            ProductId = cartItem.ProductId,
            ProductName = cartItem.Product.Name,
            // ✅ Sử dụng FinalPrice thay vì BasePrice
            UnitPrice = cartItem.FinalPrice,
            Quantity = cartItem.Quantity,
            TotalPrice = cartItem.FinalPrice * cartItem.Quantity,
            // ✅ Lưu customizations
            Customizations = cartItem.CustomizationsJson
        };
        order.Items.Add(orderItem);
    }

    // ... rest of order creation ...
}
```

---

## 📋 Checklist

- [ ] Kiểm tra AddToCartAsync có xử lý customizations
- [ ] Thêm code tính customizationPrice
- [ ] Lưu customizations vào CartItem.CustomizationsJson
- [ ] Cập nhật GetCartAsync để trả về customizations
- [ ] Cập nhật CreateOrderAsync để sử dụng FinalPrice
- [ ] Test add to cart với customizations
- [ ] Verify customizationPrice được tính đúng
- [ ] Verify finalPrice = basePrice + customizationPrice
- [ ] Verify order items có đúng giá

---

## 🧪 Test

### Test 1: Add to cart với customizations
```
POST /api/v1/Cart/add
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

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "cartItemId": "...",
    "unitPrice": 7000000.00,  // ✅ 5500000 + 1500000
    "cartTotalAmount": 7000000.00
  }
}
```

### Test 2: Get cart
```
GET /api/v1/Cart
```

**Expected Response:**
```json
{
  "items": [
    {
      "basePrice": 5500000.00,
      "customizationPrice": 1500000.00,  // ✅ Tính đúng
      "finalPrice": 7000000.00,  // ✅ 5500000 + 1500000
      "customizations": [
        {
          "optionId": "opt-cleaning",
          "quantity": 1,
          "unitPrice": 1500000,
          "totalPrice": 1500000
        }
      ]
    }
  ],
  "totalAmount": 7000000.00
}
```

### Test 3: Create order
```
POST /api/v1/Checkout
```

**Expected Response:**
```json
{
  "items": [
    {
      "unitPrice": 7000000.00,  // ✅ Sử dụng finalPrice
      "totalPrice": 7000000.00
    }
  ]
}
```

---

## 📞 Support

Nếu cần giúp, hãy:
1. Kiểm tra AddToCartAsync method
2. Xem có xử lý customizations không
3. Nếu không, thêm code ở trên
4. Test lại

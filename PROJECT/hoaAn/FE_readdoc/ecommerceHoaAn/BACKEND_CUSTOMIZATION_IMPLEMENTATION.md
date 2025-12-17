# Backend Customization Implementation Guide

## Tóm tắt
Frontend đã chuẩn bị gửi customizations khi add product vào cart, nhưng backend chưa xử lý. Document này hướng dẫn cách implement.

## Frontend Request Format

```json
POST /api/v1/Cart/add
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

## Backend Implementation Steps

### 1. Update CartItem Entity
Thêm field để lưu customizations:

```csharp
public class CartItem
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    
    // Thêm các field này:
    public string? CustomizationsJson { get; set; } // Lưu customizations dưới dạng JSON
    public decimal BasePrice { get; set; } // Giá gốc của product
    public decimal CustomizationPrice { get; set; } // Tổng giá customizations
    public decimal FinalPrice { get; set; } // BasePrice + CustomizationPrice
    
    // Existing fields...
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 2. Update AddToCartAsync Method

```csharp
public async Task<ApiResponse<AddToCartResponseDto>> AddToCartAsync(
    Guid userId, 
    AddToCartDto dto)
{
    // Validate product exists
    var product = await _productRepository.GetByIdAsync(dto.ProductId);
    if (product == null)
        return ApiResponse<AddToCartResponseDto>.Failure("Product not found");

    // Get or create cart
    var cart = await _cartRepository.GetByUserIdAsync(userId);
    if (cart == null)
    {
        cart = new Cart { UserId = userId };
        await _cartRepository.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync();
    }

    // Calculate prices
    decimal basePrice = product.Price;
    decimal customizationPrice = 0;
    string? customizationsJson = null;

    // Process customizations if provided
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

            // Validate quantity within bounds
            if (customization.Quantity < option.MinQuantity || 
                customization.Quantity > option.MaxQuantity)
                return ApiResponse<AddToCartResponseDto>.Failure(
                    $"Invalid quantity for {option.Name}. Min: {option.MinQuantity}, Max: {option.MaxQuantity}");

            // Add to customization price
            customizationPrice += customization.TotalPrice;
        }

        // Serialize customizations to JSON
        customizationsJson = JsonSerializer.Serialize(dto.Customizations);
    }

    decimal finalPrice = basePrice + customizationPrice;

    // Check if item already exists
    var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
    
    if (existingItem != null)
    {
        // Update existing item
        existingItem.Quantity += dto.Quantity;
        existingItem.CustomizationsJson = customizationsJson;
        existingItem.BasePrice = basePrice;
        existingItem.CustomizationPrice = customizationPrice;
        existingItem.FinalPrice = finalPrice;
        existingItem.UpdatedAt = DateTime.UtcNow;
    }
    else
    {
        // Create new item
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

    // Return response
    return ApiResponse<AddToCartResponseDto>.Success(
        new AddToCartResponseDto
        {
            CartItemId = existingItem?.Id ?? cart.Items.Last().Id,
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitPrice = basePrice,
            CartItemCount = cart.Items.Count,
            CartTotalAmount = cart.Items.Sum(i => i.FinalPrice * i.Quantity)
        },
        "Item added to cart successfully");
}
```

### 3. Update GetCartAsync Method

Khi trả về cart items, cần include customizations:

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
        Customizations = item.CustomizationsJson != null
            ? JsonSerializer.Deserialize<List<CartItemCustomizationDto>>(item.CustomizationsJson)
            : null
    }).ToList();

    return ApiResponse<GetCartResponseDto>.Success(
        new GetCartResponseDto
        {
            CartId = cart.Id,
            Items = items,
            TotalAmount = items.Sum(i => i.FinalPrice * i.Quantity)
        });
}
```

### 4. Update CreateOrderAsync Method

Khi tạo order, sử dụng `FinalPrice` thay vì `UnitPrice`:

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
            UnitPrice = cartItem.FinalPrice, // Use FinalPrice instead of BasePrice
            Quantity = cartItem.Quantity,
            TotalPrice = cartItem.FinalPrice * cartItem.Quantity,
            Customizations = cartItem.CustomizationsJson // Include customizations
        };
        order.Items.Add(orderItem);
    }

    // ... rest of order creation logic ...
}
```

## Response Format

### Add to Cart Response
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

### Get Cart Response
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
        "customizationPrice": 2400000.00,
        "finalPrice": 7900000.00,
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
    "totalAmount": 7900000.00
  }
}
```

## Testing Checklist

- [ ] Add product with customizations to cart
- [ ] Verify customizations are saved in CartItem
- [ ] Get cart and verify customizations are returned
- [ ] Verify FinalPrice = BasePrice + CustomizationPrice
- [ ] Create order and verify order items use FinalPrice
- [ ] Test with multiple customizations
- [ ] Test with zero quantity customizations (should be ignored)
- [ ] Test validation of customization quantities

## Notes

- Customizations được lưu dưới dạng JSON trong `CustomizationsJson` field
- `FinalPrice` được tính khi add to cart và được sử dụng cho order
- Frontend tạm thời lưu customizations vào localStorage để hiển thị, nhưng backend là source of truth

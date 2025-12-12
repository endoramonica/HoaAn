# Design Document: Package & Customizable Products

## Overview

This design document outlines the architecture for implementing package products with customizable options in VietCommerce. The system enables merchants to create pre-configured ritual packages that customers can customize by adjusting quantities of optional items. The design preserves historical accuracy through snapshots and maintains data integrity through validation.

**Key Design Principles:**
- **Immutability of Orders**: Once an order is placed, customization details are frozen (snapshots)
- **Flexibility in Products**: Merchants can update package options anytime without affecting existing orders
- **Price Transparency**: Customers see base price + customization surcharges separately
- **Data Integrity**: All JSON data is validated before storage and deserialization

## Architecture

### High-Level Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    MERCHANT WORKFLOW                         │
├─────────────────────────────────────────────────────────────┤
│ 1. Create Package Product                                    │
│    - Set base price                                          │
│    - Add fixed items (details)                               │
│    - Add customizable options (with min/max/price)           │
│    - Store as JSON in Product entity                         │
│                                                              │
│ 2. Update Package Options                                    │
│    - Modify option prices                                    │
│    - Add/remove options                                      │
│    - Changes apply only to new orders                        │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                   CUSTOMER WORKFLOW                          │
├─────────────────────────────────────────────────────────────┤
│ 1. View Package Product                                      │
│    - See base price                                          │
│    - See included items (details)                            │
│    - See customizable options with constraints               │
│                                                              │
│ 2. Add to Cart with Customizations                           │
│    - Validate customization quantities                       │
│    - Calculate: finalPrice = basePrice + customizationPrice  │
│    - Store customizations in CartItem                        │
│                                                              │
│ 3. Update Cart Item                                          │
│    - Modify customization quantities                         │
│    - Recalculate final price                                 │
│                                                              │
│ 4. Checkout                                                  │
│    - Snapshot customizations to OrderItem                    │
│    - Preserve prices for historical accuracy                 │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                  FULFILLMENT WORKFLOW                        │
├─────────────────────────────────────────────────────────────┤
│ 1. View Order Item                                           │
│    - See customizations snapshot                             │
│    - See exact quantities ordered                            │
│    - See prices at order time                                │
│                                                              │
│ 2. Generate Delivery Note                                    │
│    - Print customization details                             │
│    - Use snapshot data (not current product data)            │
└─────────────────────────────────────────────────────────────┘
```

## Components and Interfaces

### 1. Data Models

#### Product Entity (Extended)
```csharp
public class Product : AuditableEntity, ISoftDelete
{
    // Existing fields...
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    // NEW: Package fields
    public string? DetailsJson { get; set; }           // ["Cá chép (3 con)", "Mũ giấy (3 cái)", ...]
    public string? CustomizableOptionsJson { get; set; } // [{"id":"opt-xoi","name":"Xôi",...}]
}
```

#### CartItem Entity (Extended)
```csharp
public class CartItem : AuditableEntity, ISoftDelete
{
    // Existing fields...
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    
    // NEW: Customization fields
    public string? CustomizationsJson { get; set; }    // [{"optionId":"opt-xoi","quantity":10,...}]
    public decimal BasePrice { get; set; }             // 3,500,000đ
    public decimal CustomizationPrice { get; set; }    // 785,000đ (sum of customization surcharges)
    public decimal FinalPrice { get; set; }            // 4,285,000đ (BasePrice + CustomizationPrice)
}
```

#### OrderItem Entity (Extended)
```csharp
public class OrderItem : AuditableEntity, ISoftDelete
{
    // Existing fields...
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    
    // NEW: Customization snapshot
    public string? CustomizationsJson { get; set; }    // Snapshot from CartItem
    public decimal BasePrice { get; set; }             // Snapshot from CartItem
    public decimal CustomizationPrice { get; set; }    // Snapshot from CartItem
}
```

### 2. DTOs

#### CustomizableOptionDto
```csharp
public class CustomizableOptionDto
{
    public string Id { get; set; }                  // "opt-xoi"
    public string Name { get; set; }                // "Xôi gấc đậu xanh"
    public int BaseQuantity { get; set; }           // 5 (default quantity)
    public decimal UnitPrice { get; set; }          // 45,000đ per unit
    public int MinQuantity { get; set; }            // 5 (minimum)
    public int? MaxQuantity { get; set; }           // 100 (maximum, null = unlimited)
    public string Unit { get; set; }                // "dĩa"
}
```

#### CartItemCustomizationDto
```csharp
public class CartItemCustomizationDto
{
    public string OptionId { get; set; }            // "opt-xoi"
    public int Quantity { get; set; }               // 10
    public decimal UnitPrice { get; set; }          // 45,000đ
    public decimal TotalPrice { get; set; }         // 450,000đ (10 × 45,000)
}
```

#### Updated AddToCartDto
```csharp
public class AddToCartDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    
    // NEW: Customizations
    public List<CartItemCustomizationDto>? Customizations { get; set; }
}
```

#### Updated CartItemDetailDto
```csharp
public class CartItemDetailDto
{
    // Existing fields...
    public Guid CartItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    
    // NEW: Customization fields
    public decimal BasePrice { get; set; }
    public decimal CustomizationPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public List<CartItemCustomizationDto>? Customizations { get; set; }
}
```

#### Updated ProductDetailDto
```csharp
public class ProductDetailDto
{
    // Existing fields...
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    // NEW: Package fields
    public List<string>? Details { get; set; }                          // ["Cá chép (3 con)", ...]
    public List<CustomizableOptionDto>? CustomizableOptions { get; set; } // [{"id":"opt-xoi",...}]
}
```

### 3. Service Interfaces

#### IProductService (Extended)
```csharp
public interface IProductService
{
    // Existing methods...
    
    // NEW: Package product methods
    Task<ApiResponse<ProductDetailDto>> CreatePackageProductAsync(
        Guid actorUserId, 
        ProductCreateDto dto);
    
    Task<ApiResponse<ProductDetailDto>> UpdatePackageProductAsync(
        Guid actorUserId, 
        Guid productId, 
        ProductUpdateDto dto);
    
    Task<ValidationResult> ValidateCustomizableOptionsAsync(
        List<CustomizableOptionDto> options);
    
    Task<ValidationResult> ValidateCustomizationsAsync(
        Guid productId, 
        List<CartItemCustomizationDto> customizations);
}
```

#### ICartService (Extended)
```csharp
public interface ICartService
{
    // Existing methods...
    
    // NEW: Customization support
    Task<ApiResponse<CartItemDetailDto>> AddToCartWithCustomizationsAsync(
        Guid userId, 
        AddToCartDto dto);
    
    Task<ApiResponse<CartItemDetailDto>> UpdateCartItemCustomizationsAsync(
        Guid userId, 
        Guid cartItemId, 
        List<CartItemCustomizationDto> customizations);
    
    decimal CalculateFinalPrice(
        decimal basePrice, 
        List<CartItemCustomizationDto> customizations);
}
```

#### IOrderService (Extended)
```csharp
public interface IOrderService
{
    // Existing methods...
    
    // NEW: Customization snapshot
    Task<OrderItem> CreateOrderItemFromCartItemAsync(
        CartItem cartItem, 
        Guid orderId);
}
```

## Data Models

### Database Schema Changes

```sql
-- Product table additions
ALTER TABLE Products ADD DetailsJson NVARCHAR(MAX) NULL;
ALTER TABLE Products ADD CustomizableOptionsJson NVARCHAR(MAX) NULL;

-- CartItem table additions
ALTER TABLE CartItems ADD CustomizationsJson NVARCHAR(MAX) NULL;
ALTER TABLE CartItems ADD BasePrice DECIMAL(18,2) NOT NULL DEFAULT 0;
ALTER TABLE CartItems ADD CustomizationPrice DECIMAL(18,2) NOT NULL DEFAULT 0;
ALTER TABLE CartItems ADD FinalPrice DECIMAL(18,2) NOT NULL DEFAULT 0;

-- OrderItem table additions
ALTER TABLE OrderItems ADD CustomizationsJson NVARCHAR(MAX) NULL;
ALTER TABLE OrderItems ADD BasePrice DECIMAL(18,2) NOT NULL DEFAULT 0;
ALTER TABLE OrderItems ADD CustomizationPrice DECIMAL(18,2) NOT NULL DEFAULT 0;
```

### JSON Structure Examples

**Product.DetailsJson:**
```json
[
  "Cá chép giấy (3 con)",
  "Mũ giấy (3 cái)",
  "Vàng mã (1 bộ)",
  "Hương đèn (1 bộ)",
  "Hoa quả (1 mâm)",
  "Bánh kẹo (1 hộp)"
]
```

**Product.CustomizableOptionsJson:**
```json
[
  {
    "id": "opt-xoi",
    "name": "Xôi gấc đậu xanh",
    "baseQuantity": 5,
    "unitPrice": 45000,
    "minQuantity": 5,
    "maxQuantity": 100,
    "unit": "dĩa"
  },
  {
    "id": "opt-che",
    "name": "Chè trôi nước",
    "baseQuantity": 5,
    "unitPrice": 35000,
    "minQuantity": 5,
    "maxQuantity": null,
    "unit": "chén"
  }
]
```

**CartItem.CustomizationsJson:**
```json
[
  {
    "optionId": "opt-xoi",
    "quantity": 10,
    "unitPrice": 45000,
    "totalPrice": 450000
  },
  {
    "optionId": "opt-che",
    "quantity": 5,
    "unitPrice": 35000,
    "totalPrice": 175000
  }
]
```

## Error Handling

### Validation Errors
- Invalid JSON structure → 400 Bad Request
- Missing required fields → 400 Bad Request
- Quantity out of bounds → 400 Bad Request
- Invalid option ID → 400 Bad Request

### Authorization Errors
- Missing product.create permission → 403 Forbidden
- Missing product.update permission → 403 Forbidden

### Not Found Errors
- Product not found → 404 Not Found
- Option not found → 404 Not Found

### Serialization Errors
- JSON deserialization failure → Log error, return 500 Internal Server Error
- JSON serialization failure → Log error, return 500 Internal Server Error

## Testing Strategy

### Unit Testing
- Validate CustomizableOptionDto structure
- Validate quantity constraints (min/max)
- Calculate final price correctly
- Deserialize JSON without errors
- Handle null customizations gracefully

### Property-Based Testing
- **Property 1:** For any valid customizable options, deserializing then serializing produces equivalent data
- **Property 2:** For any customization request, final price = base price + sum(customization quantities × unit prices)
- **Property 3:** For any quantity within bounds, validation passes; outside bounds, validation fails
- **Property 4:** For any cart item with customizations, snapshot to order item preserves all data
- **Property 5:** For any product update, existing orders retain original customization data

### Integration Testing
- Create package product with options
- Add to cart with customizations
- Update cart item customizations
- Create order from cart
- Verify order item snapshots
- Query analytics from order items


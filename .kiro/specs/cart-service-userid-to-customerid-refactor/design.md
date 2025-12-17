# Design Document: CartService userId → customerId Refactor

## Overview

Refactor CartService to use `customerId` instead of `userId` for all cart operations. This aligns with the correct business logic separation:
- **User** = Authentication/Authorization (Identity)
- **Customer** = Business Logic (Purchasing, Cart, Orders)

The refactor is a systematic replacement of parameter names and internal references, with careful attention to:
1. Method signatures in CartService
2. Internal implementation logic
3. Cache key generation
4. Repository method calls
5. Controller integration
6. CheckoutService integration

## Architecture

### Current Architecture (Before Refactor)
```
CartController (userId)
    ↓
CartService (userId)
    ↓
ICartRepository (userId)
    ↓
Cart Entity (UserId field)
```

### Target Architecture (After Refactor)
```
CartController (customerId extracted from User context)
    ↓
CartService (customerId)
    ↓
ICartRepository (customerId)
    ↓
Cart Entity (UserId field - still exists for backward compatibility)
```

**Key Point**: The Cart entity's `UserId` field will remain unchanged in the database. We're only changing the parameter names and logic flow to use `customerId` semantically.

## Components and Interfaces

### 1. CartService Changes

#### Method Signature Changes
All public methods will change from `Guid userId` to `Guid customerId`:

```csharp
// Before
public async Task<ApiResponse<GetCartResponseDto>> GetCartAsync(Guid userId)

// After
public async Task<ApiResponse<GetCartResponseDto>> GetCartAsync(Guid customerId)
```

#### Internal Helper Method Changes
```csharp
// Before
private async Task<Cart> GetActiveUserCartWithItemsAsync(Guid userId)

// After
private async Task<Cart> GetActiveUserCartWithItemsAsync(Guid customerId)
```

#### Cache Key Generation
```csharp
// Before
var cacheKey = CreateCacheKey(USER_CART_CACHE_PREFIX, userId);

// After
var cacheKey = CreateCacheKey(USER_CART_CACHE_PREFIX, customerId);
```

### 2. ICartRepository Changes

```csharp
// Before
Task<Cart> GetUserCartWithItemsAsync(Guid userId);
Task<Cart> GetOrCreateCartByUserIdAsync(Guid userId);
Task MergeGuestCartToUserCartAsync(string sessionId, Guid userId);

// After
Task<Cart> GetUserCartWithItemsAsync(Guid customerId);
Task<Cart> GetOrCreateCartByCustomerIdAsync(Guid customerId);
Task MergeGuestCartToUserCartAsync(string sessionId, Guid customerId);
```

### 3. CartRepository Implementation Changes

Update CartRepository to accept `customerId` and map it to `UserId` field in Cart entity:

```csharp
public async Task<Cart> GetUserCartWithItemsAsync(Guid customerId)
{
    // Query by customerId (which maps to UserId in database)
    return await _context.Carts
        .Where(c => c.UserId == customerId && c.IsActive && !c.IsDeleted)
        .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
        .FirstOrDefaultAsync();
}
```

### 4. CartController Changes

Extract `customerId` from current user context and pass to CartService:

```csharp
[HttpGet("cart")]
public async Task<IActionResult> GetCart()
{
    var customerId = GetCurrentCustomerId(); // Extract from User context
    var result = await _cartService.GetCartAsync(customerId);
    return Ok(result);
}
```

### 5. CheckoutService Integration

Update CheckoutService to pass `customerId` to CartService:

```csharp
// Before
var cartValidation = await _cartService.ValidateCartForCheckoutAsync(userId);

// After
var cartValidation = await _cartService.ValidateCartForCheckoutAsync(customerId);
```

## Data Models

### Cart Entity (No Changes)
```csharp
public class Cart : BaseEntity
{
    public Guid? UserId { get; set; }  // Still maps to customerId semantically
    public string? SessionId { get; set; }  // For guest carts
    public Guid? AppliedVoucherId { get; set; }
    public string? AppliedVoucherCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public bool IsActive { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
}
```

### DTOs (No Changes)
- GetCartResponseDto
- CartSummaryDto
- AddToCartResponseDto
- UpdateCartItemResponseDto
- ClearCartResponseDto
- CartItemDetailDto

## Correctness Properties

A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.

### Property 1: CustomerId Parameter Acceptance
*For any* CartService method, the method SHALL accept a Guid customerId parameter and reject calls without this parameter.

**Validates: Requirements 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 1.10, 1.11, 1.12, 1.13, 1.14, 1.15, 1.16, 1.17**

### Property 2: Internal Helper Method Signature
*For any* call to GetActiveUserCartWithItemsAsync, the method SHALL accept Guid customerId parameter and retrieve the cart for that customerId.

**Validates: Requirements 2.1, 2.2**

### Property 3: Cache Key Uniqueness
*For any* two different customerIds, the generated cache keys SHALL be different and SHALL not collide.

**Validates: Requirements 2.3**

### Property 4: Permission Check Preservation
*For any* cart operation, permission checks SHALL still validate against userId (from User entity) for authentication, even though the cart operation uses customerId.

**Validates: Requirements 2.4**

### Property 5: Repository Method Mapping
*For any* customerId passed to CartService, the repository method SHALL correctly map it to the UserId field in the Cart entity and retrieve the correct cart.

**Validates: Requirements 2.5, 4.1, 4.2, 4.3**

### Property 6: Controller Parameter Extraction
*For any* authenticated request to CartController, the extracted customerId SHALL be passed to CartService methods without modification.

**Validates: Requirements 3.1, 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8**

### Property 7: CheckoutService Integration
*For any* CheckoutService call to CartService, the service SHALL pass customerId instead of userId to all CartService methods.

**Validates: Requirements 5.1, 5.2**

### Property 8: Cart Item Isolation
*For any* two different customerIds, cart items from one customer SHALL never be accessible or modifiable by another customer.

**Validates: Requirements 6.1, 6.2**

### Property 9: Cart Clear Operation
*For any* cart clear operation using a valid customerId, all items in that customer's cart SHALL be removed and no items from other customers' carts SHALL be affected.

**Validates: Requirements 6.3**

### Property 10: Guest Cart Merge Consistency
*For any* guest cart merge operation with a valid sessionId and customerId, all guest cart items SHALL be merged to the correct customer's cart and the guest cart SHALL be cleared.

**Validates: Requirements 6.4**

### Property 11: Voucher Application Isolation
*For any* voucher application to a customer's cart, the discount SHALL be applied only to that customer's cart and SHALL not affect other customers' carts.

**Validates: Requirements 6.5**

## Error Handling

### Validation Errors
- Invalid customerId (empty or null) → throw ArgumentNullException
- Customer not found → throw InvalidOperationException
- Unauthorized access (customerId mismatch) → throw UnauthorizedAccessException

### Business Logic Errors
- Cart not found for customerId → throw InvalidOperationException
- Cart is empty → throw InvalidOperationException
- Insufficient stock → throw InvalidOperationException

### Cache Errors
- Cache invalidation failures → log warning, continue with DB query

## Testing Strategy

### Unit Testing
- Test each CartService method with valid customerId
- Test cache key generation with different customerIds
- Test repository method calls with customerId parameter
- Test error handling for invalid customerId
- Test controller parameter extraction

### Property-Based Testing
- **Property 1**: Generate random customerIds and verify cart isolation
- **Property 2**: Generate random customerIds and verify cache key uniqueness
- **Property 3**: Generate random customerIds and verify repository mapping
- **Property 4**: Generate random authenticated requests and verify parameter extraction
- **Property 5**: Generate random guest carts and verify merge consistency
- **Property 6**: Generate random cart operations and verify no breaking changes

### Integration Testing
- Test full flow: GetCart → AddToCart → UpdateCart → RemoveCart → ClearCart
- Test guest cart merge flow
- Test voucher application flow
- Test checkout validation flow

### Testing Framework
- **Unit Tests**: xUnit with Moq
- **Property-Based Tests**: FsCheck (F# property testing library for .NET)
- **Integration Tests**: xUnit with test database

### Test Configuration
- Minimum 100 iterations for each property-based test
- Each property-based test tagged with requirement reference
- Each property-based test tagged with format: `**Feature: cart-service-userid-to-customerid-refactor, Property {number}: {property_text}**`

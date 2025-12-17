# Requirements Document: CartService userId → customerId Refactor

## Introduction

CartService hiện tại sử dụng `userId` (Identity/Auth) để quản lý giỏ hàng, nhưng theo đúng nguyên tắc backend, **Cart là nghiệp vụ của Customer (mua hàng), không phải User (auth)**. 

Spec này hướng dẫn refactor CartService từ `userId` sang `customerId` một cách an toàn, không vỡ luồng, và đảm bảo tất cả các điểm gọi được cập nhật đúng.

## Glossary

- **User**: Entity đại diện cho tài khoản auth/login (Identity)
- **Customer**: Entity đại diện cho người mua hàng (Business Logic)
- **UserId**: Guid của User (dùng cho auth, phân quyền)
- **CustomerId**: Guid của Customer (dùng cho cart, order, mua hàng)
- **Cart**: Giỏ hàng của Customer, chứa CartItems
- **CartService**: Service quản lý cart operations
- **ICartRepository**: Repository interface cho Cart entity
- **CartController**: API endpoint cho cart operations

## Requirements

### Requirement 1: Refactor CartService Method Signatures

**User Story:** As a backend developer, I want CartService methods to accept `customerId` instead of `userId`, so that the service correctly reflects that cart is a customer business operation, not a user authentication operation.

#### Acceptance Criteria

1. WHEN CartService method is called with `customerId` parameter THEN the method SHALL accept Guid customerId instead of Guid userId
2. WHEN GetCartAsync is called THEN the method signature SHALL be `Task<ApiResponse<GetCartResponseDto>> GetCartAsync(Guid customerId)`
3. WHEN GetCartSummaryAsync is called THEN the method signature SHALL be `Task<ApiResponse<CartSummaryDto>> GetCartSummaryAsync(Guid customerId)`
4. WHEN AddToCartAsync is called THEN the method signature SHALL be `Task<ApiResponse<AddToCartResponseDto>> AddToCartAsync(Guid customerId, AddToCartDto dto)`
5. WHEN UpdateCartItemAsync is called THEN the method signature SHALL be `Task<ApiResponse<UpdateCartItemResponseDto>> UpdateCartItemAsync(Guid customerId, UpdateCartItemDto dto)`
6. WHEN RemoveFromCartAsync is called THEN the method signature SHALL be `Task<ApiResponse<bool>> RemoveFromCartAsync(Guid customerId, Guid cartItemId)`
7. WHEN ClearCartAsync is called THEN the method signature SHALL be `Task<ApiResponse<ClearCartResponseDto>> ClearCartAsync(Guid customerId)`
8. WHEN ValidateCartForCheckoutAsync is called THEN the method signature SHALL be `Task<ApiResponse<bool>> ValidateCartForCheckoutAsync(Guid customerId)`
9. WHEN GetCartItemCountAsync is called THEN the method signature SHALL be `Task<ApiResponse<int>> GetCartItemCountAsync(Guid customerId)`
10. WHEN GetCartItemDetailAsync is called THEN the method signature SHALL be `Task<ApiResponse<CartItemDetailDto>> GetCartItemDetailAsync(Guid customerId, Guid cartItemId)`
11. WHEN ApplyCouponAsync is called THEN the method signature SHALL be `Task<ApiResponse<CartSummaryDto>> ApplyCouponAsync(Guid customerId, string couponCode)`
12. WHEN RemoveCouponAsync is called THEN the method signature SHALL be `Task<ApiResponse<CartSummaryDto>> RemoveCouponAsync(Guid customerId)`
13. WHEN AddToCartWithCustomizationsAsync is called THEN the method signature SHALL be `Task<ApiResponse<CartItemDetailDto>> AddToCartWithCustomizationsAsync(Guid customerId, AddToCartDto dto)`
14. WHEN UpdateCartItemCustomizationsAsync is called THEN the method signature SHALL be `Task<ApiResponse<CartItemDetailDto>> UpdateCartItemCustomizationsAsync(Guid customerId, Guid cartItemId, List<CartItemCustomizationDto> customizations)`
15. WHEN ApplyVoucherAsync is called THEN the method signature SHALL be `Task<ApiResponse<GetCartResponseDto>> ApplyVoucherAsync(Guid customerId, string voucherCode)`
16. WHEN RemoveVoucherAsync is called THEN the method signature SHALL be `Task<ApiResponse<GetCartResponseDto>> RemoveVoucherAsync(Guid customerId)`
17. WHEN MergeGuestCartToUserAsync is called THEN the method signature SHALL be `Task<ApiResponse<GetCartResponseDto>> MergeGuestCartToUserAsync(string sessionId, Guid customerId)`

### Requirement 2: Update CartService Implementation

**User Story:** As a backend developer, I want CartService implementation to use `customerId` internally, so that all internal logic correctly references customer business operations.

#### Acceptance Criteria

1. WHEN GetActiveUserCartWithItemsAsync is called THEN the method SHALL accept Guid customerId parameter instead of Guid userId
2. WHEN GetActiveUserCartWithItemsAsync is called THEN the method SHALL query cart by customerId instead of userId
3. WHEN cache keys are created THEN the cache keys SHALL use customerId instead of userId
4. WHEN permission checks are performed THEN the permission checks SHALL still use userId (from User entity) for auth validation
5. WHEN cart is retrieved from repository THEN the repository method SHALL accept customerId parameter

### Requirement 3: Update CartController Endpoints

**User Story:** As a backend developer, I want CartController endpoints to pass `customerId` to CartService, so that the API layer correctly provides customer ID instead of user ID.

#### Acceptance Criteria

1. WHEN CartController.GetCart endpoint is called THEN the controller SHALL extract customerId from current user context
2. WHEN CartController.AddToCart endpoint is called THEN the controller SHALL pass customerId to CartService.AddToCartAsync
3. WHEN CartController.UpdateCartItem endpoint is called THEN the controller SHALL pass customerId to CartService.UpdateCartItemAsync
4. WHEN CartController.RemoveFromCart endpoint is called THEN the controller SHALL pass customerId to CartService.RemoveFromCartAsync
5. WHEN CartController.ClearCart endpoint is called THEN the controller SHALL pass customerId to CartService.ClearCartAsync
6. WHEN CartController.ValidateCart endpoint is called THEN the controller SHALL pass customerId to CartService.ValidateCartForCheckoutAsync
7. WHEN CartController.ApplyVoucher endpoint is called THEN the controller SHALL pass customerId to CartService.ApplyVoucherAsync
8. WHEN CartController.RemoveVoucher endpoint is called THEN the controller SHALL pass customerId to CartService.RemoveVoucherAsync

### Requirement 4: Update ICartRepository Interface

**User Story:** As a backend developer, I want ICartRepository methods to accept `customerId` instead of `userId`, so that the data access layer correctly reflects customer business operations.

#### Acceptance Criteria

1. WHEN GetUserCartWithItemsAsync is called THEN the method signature SHALL be `Task<Cart> GetUserCartWithItemsAsync(Guid customerId)`
2. WHEN GetOrCreateCartByUserIdAsync is called THEN the method signature SHALL be `Task<Cart> GetOrCreateCartByCustomerIdAsync(Guid customerId)`
3. WHEN MergeGuestCartToUserCartAsync is called THEN the method signature SHALL be `Task MergeGuestCartToUserCartAsync(string sessionId, Guid customerId)`

### Requirement 5: Update CheckoutService Integration

**User Story:** As a backend developer, I want CheckoutService to pass `customerId` to CartService, so that checkout flow correctly uses customer ID.

#### Acceptance Criteria

1. WHEN CheckoutService calls CartService methods THEN the service SHALL pass customerId instead of userId
2. WHEN CheckoutService validates cart THEN the service SHALL call ValidateCartForCheckoutAsync with customerId

### Requirement 6: Ensure No Breaking Changes

**User Story:** As a backend developer, I want to ensure the refactor doesn't break existing functionality, so that all cart operations continue to work correctly.

#### Acceptance Criteria

1. WHEN cart is retrieved THEN the cart SHALL still contain all items correctly
2. WHEN cart item is added THEN the item SHALL be added to the correct customer's cart
3. WHEN cart is cleared THEN all items SHALL be removed from the correct customer's cart
4. WHEN guest cart is merged THEN items SHALL be merged to the correct customer's cart
5. WHEN voucher is applied THEN the discount SHALL be applied to the correct customer's cart

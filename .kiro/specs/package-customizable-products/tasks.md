# Implementation Plan: Package & Customizable Products

## Overview
This implementation plan breaks down the feature into discrete, manageable coding tasks. Each task builds incrementally on previous tasks, with testing integrated throughout.

---

## Phase 1: Database & Entity Models

- [x] 1. Create database migration for Product entity extensions




  - Add `DetailsJson` column (NVARCHAR(MAX), nullable)
  - Add `CustomizableOptionsJson` column (NVARCHAR(MAX), nullable)
  - Create index on ProductId for query performance
  - _Requirements: 1.4, 2.1_

- [x] 2. Create database migration for CartItem entity extensions




  - Add `CustomizationsJson` column (NVARCHAR(MAX), nullable)
  - Add `BasePrice` column (DECIMAL(18,2), default 0)
  - Add `CustomizationPrice` column (DECIMAL(18,2), default 0)
  - Add `FinalPrice` column (DECIMAL(18,2), default 0)
  - _Requirements: 3.4, 4.2_

- [x] 3. Create database migration for OrderItem entity extensions




  - Add `CustomizationsJson` column (NVARCHAR(MAX), nullable)
  - Add `BasePrice` column (DECIMAL(18,2), default 0)
  - Add `CustomizationPrice` column (DECIMAL(18,2), default 0)
  - _Requirements: 5.1, 5.2_

- [x] 4. Update Product entity class




  - Add `DetailsJson` property (string, nullable)
  - Add `CustomizableOptionsJson` property (string, nullable)
  - Add XML documentation
  - _Requirements: 1.4_

- [x] 5. Update CartItem entity class




  - Add `CustomizationsJson` property (string, nullable)
  - Add `BasePrice` property (decimal)
  - Add `CustomizationPrice` property (decimal)
  - Add `FinalPrice` property (decimal)
  - Add XML documentation
  - _Requirements: 3.4, 4.2_

- [x] 6. Update OrderItem entity class




  - Add `CustomizationsJson` property (string, nullable)
  - Add `BasePrice` property (decimal)
  - Add `CustomizationPrice` property (decimal)
  - Add XML documentation
  - _Requirements: 5.1, 5.2_

---

## Phase 2: DTOs & Validators

- [x] 7. Create CustomizableOptionDto class




  - Properties: Id, Name, BaseQuantity, UnitPrice, MinQuantity, MaxQuantity, Unit
  - Add XML documentation
  - _Requirements: 1.2, 2.2_

- [x] 8. Create CartItemCustomizationDto class




  - Properties: OptionId, Quantity, UnitPrice, TotalPrice
  - Add XML documentation
  - _Requirements: 3.1, 4.1_

- [x] 9. Update ProductCreateDto class




  - Add `Details` property (List<string>, nullable)
  - Add `CustomizableOptions` property (List<CustomizableOptionDto>, nullable)
  
  - _Requirements: 1.1_

- [x] 10. Update ProductUpdateDto class




  - Add `Details` property (List<string>, nullable)
  - Add `CustomizableOptions` property (List<CustomizableOptionDto>, nullable)
  
  - _Requirements: 6.1_

- [x] 11. Update AddToCartDto class




  - Add `Customizations` property (List<CartItemCustomizationDto>, nullable)
  
  - _Requirements: 3.1_

- [x] 12. Update CartItemDetailDto class




  - Add `BasePrice` property (decimal)
  - Add `CustomizationPrice` property (decimal)
  - Add `FinalPrice` property (decimal)
  - Add `Customizations` property (List<CartItemCustomizationDto>, nullable)
  
  - _Requirements: 8.1, 8.2_

- [x] 13. Update ProductDetailDto class




  - Add `Details` property (List<string>, nullable)
  - Add `CustomizableOptions` property (List<CustomizableOptionDto>, nullable)

  - _Requirements: 2.1, 2.2_

- [x] 14. Create FluentValidation validator for CustomizableOptionDto





  - Validate Id is not empty
  - Validate Name is not empty
  - Validate UnitPrice > 0
  - Validate MinQuantity > 0
  - Validate MinQuantity ≤ MaxQuantity (if MaxQuantity exists)
  - _Requirements: 1.2, 1.3_

- [x] 15. Create FluentValidation validator for ProductCreateDto (package fields)




  - Validate Details list is not empty (if package product)
  - Validate CustomizableOptions structure (if present)
  - Validate each option using CustomizableOptionDto validator
  - _Requirements: 1.1, 1.2_

- [x] 16. Create FluentValidation validator for AddToCartDto (customizations)




  - Validate Customizations structure (if present)
  - Validate each customization has valid OptionId and Quantity
  - _Requirements: 3.1_

- [ ]* 17. Write unit tests for DTO validators
  - Test valid CustomizableOptionDto
  - Test invalid CustomizableOptionDto (missing fields, invalid values)
  - Test valid ProductCreateDto with package
  - Test invalid ProductCreateDto
  - Test valid AddToCartDto with customizations
  - Test invalid AddToCartDto
  - _Requirements: 1.2, 1.3, 3.1_

---

## Phase 3: Service Layer - Product Management

- [x] 18. Create CustomizableOptionValidator service class





  - Method: ValidateCustomizableOptions(List<CustomizableOptionDto>) → ValidationResult
  - Validate JSON structure compliance
  - Validate all required fields present and valid
  - Validate quantity constraints
  - _Requirements: 1.2, 1.3, 7.2_

- [x] 19. Create JSON serialization/deserialization utilities




  - Method: SerializeCustomizableOptions(List<CustomizableOptionDto>) → string
  - Method: DeserializeCustomizableOptions(string) → List<CustomizableOptionDto>
  - Handle serialization errors gracefully
  - _Requirements: 1.4, 7.4_

- [x] 20. Update IProductService interface





  - Add method: ValidateCustomizableOptionsAsync(List<CustomizableOptionDto>) → Task<ValidationResult>
  - Add method: ValidateCustomizationsAsync(Guid productId, List<CartItemCustomizationDto>) → Task<ValidationResult>
  - _Requirements: 1.2, 3.1_

- [x] 21. Update ProductService implementation - Create package product





  - Implement CreateProductAsync to handle package products
  - Serialize Details and CustomizableOptions to JSON
  - Validate using CustomizableOptionValidator
  - Store in Product entity
  - _Requirements: 1.1, 1.2, 1.4_

- [x] 22. Update ProductService implementation - Update package product







  - Implement UpdateProductAsync to handle package updates
  - Validate new options structure
  - Update CustomizableOptionsJson
  - Ensure existing orders not affected (snapshots)
  - _Requirements: 6.1, 6.2_

- [x] 23. Update ProductService implementation - Get product with customizations





  - Implement GetProductByIdAsync to deserialize and return Details and CustomizableOptions
  - Handle null/empty JSON gracefully
  - Return structured CustomizableOptionDto objects
  - _Requirements: 2.1, 2.2, 2.4_

- [x] 24. Implement ValidateCustomizableOptionsAsync in ProductService








  - Validate option structure (id, name, unitPrice, minQuantity)
  - Validate quantity constraints
  - Return ValidationResult with error messages
  - _Requirements: 1.2, 1.3, 7.2_

- [x] 25. Implement ValidateCustomizationsAsync in ProductService





  - Fetch product and deserialize CustomizableOptionsJson
  - For each customization, verify option exists
  - Verify quantity within min/max bounds
  - Return ValidationResult
  - _Requirements: 3.1, 7.3_

- [ ]* 26. Write unit tests for ProductService package methods
  - Test CreateProductAsync with valid package
  - Test CreateProductAsync with invalid options
  - Test UpdateProductAsync with new options
  - Test GetProductByIdAsync returns customizations
  - Test ValidateCustomizableOptionsAsync
  - Test ValidateCustomizationsAsync
  - _Requirements: 1.1, 1.2, 2.1, 3.1_

---

## Phase 4: Service Layer - Cart Management

- [x] 27. Create price calculation utility





  - Method: CalculateFinalPrice(decimal basePrice, List<CartItemCustomizationDto> customizations) → decimal
  - Formula: basePrice + sum(customization quantities × unit prices)
  - Handle null customizations
  - _Requirements: 3.3_

- [x] 28. Update ICartService interface





  - Add method: AddToCartWithCustomizationsAsync(Guid userId, AddToCartDto dto) → Task<ApiResponse<CartItemDetailDto>>
  - Add method: UpdateCartItemCustomizationsAsync(Guid userId, Guid cartItemId, List<CartItemCustomizationDto> customizations) → Task<ApiResponse<CartItemDetailDto>>
  - _Requirements: 3.1, 4.1_

- [x] 29. Update CartService - Add to cart with customizations








  - Implement AddToCartWithCustomizationsAsync
  - Validate customizations using ProductService.ValidateCustomizationsAsync
  - Calculate final price using price calculation utility
  - Serialize customizations to JSON
  - Store BasePrice, CustomizationPrice, FinalPrice in CartItem
  - _Requirements: 3.1, 3.3, 3.4_

- [x] 30. Update CartService - Update cart item customizations





  - Implement UpdateCartItemCustomizationsAsync
  - Validate new customizations
  - Recalculate final price
  - Update CartItem with new customizations and prices
  - _Requirements: 4.1, 4.2_


- [x] 31. Update CartService - Get cart with customizations




  - Implement GetCartAsync to deserialize customizations
  - Return CartItemDetailDto with Customizations array
  - Show BasePrice, CustomizationPrice, FinalPrice breakdown
  - _Requirements: 8.1, 8.2_

- [ ]* 32. Write unit tests for CartService customization methods
  - Test AddToCartWithCustomizationsAsync with valid customizations
  - Test AddToCartWithCustomizationsAsync with invalid customizations
  - Test price calculation accuracy
  - Test UpdateCartItemCustomizationsAsync
  - Test GetCartAsync returns customizations
  - _Requirements: 3.1, 3.3, 4.1, 8.1_

---

## Phase 5: Service Layer - Order Processing

- [x] 33. Update IOrderService interface





  - Add method: CreateOrderItemFromCartItemAsync(CartItem cartItem, Guid orderId) → Task<OrderItem>
  - _Requirements: 5.1, 5.2_

- [x] 34. Update OrderService - Snapshot customizations to order
  - Implement CreateOrderItemFromCartItemAsync
  - Deserialize CartItem.CustomizationsJson
  - Snapshot to OrderItem.CustomizationsJson
  - Snapshot BasePrice and CustomizationPrice
  - Preserve historical accuracy
  - _Requirements: 5.1, 5.2, 5.3_

- [x] 35. Update OrderService - Create order from cart





  - Update CreateOrderAsync to use CreateOrderItemFromCartItemAsync
  - Ensure all customization data is preserved
  - Verify snapshot data completeness
  - _Requirements: 5.1, 5.2_

- [ ]* 36. Write unit tests for OrderService snapshot methods
  - Test CreateOrderItemFromCartItemAsync preserves customizations
  - Test snapshot preserves prices
  - Test null customizations handled
  - Test order items have complete data
  - _Requirements: 5.1, 5.2, 5.3_

---

## Phase 6: API Controllers

- [x] 37. Update AdminProductController - Create package product
  - Update CreateProduct endpoint to accept Details and CustomizableOptions
  - Call ProductService.CreateProductAsync
  - Return ProductDetailDto with customizations
  - _Requirements: 1.1, 1.2_

- [x] 38. Update AdminProductController - Update package product





  - Update UpdateProduct endpoint to accept Details and CustomizableOptions
  - Call ProductService.UpdateProductAsync
  - Return ProductDetailDto with updated customizations
  - _Requirements: 6.1_

- [x] 39. Update AdminProductController - Get product with customizations





  - Update GetProductById endpoint
  - Return ProductDetailDto with Details and CustomizableOptions
  - _Requirements: 2.1, 2.2_

- [x] 40. Create/Update CartController - Add to cart with customizations




  - Create AddToCartWithCustomizations endpoint
  - Accept AddToCartDto with Customizations
  - Call CartService.AddToCartWithCustomizationsAsync
  - Return CartItemDetailDto
  - _Requirements: 3.1, 3.3_

- [x] 41. Create/Update CartController - Update cart item customizations




  - Create UpdateCartItemCustomizations endpoint
  - Accept CartItemId and List<CartItemCustomizationDto>
  - Call CartService.UpdateCartItemCustomizationsAsync
  - Return CartItemDetailDto
  - _Requirements: 4.1, 4.2_

- [x] 42. Create/Update CartController - Get cart with customizations





  - Update GetCart endpoint
  - Return cart items with Customizations array
  - Show price breakdown (BasePrice, CustomizationPrice, FinalPrice)
  - _Requirements: 8.1, 8.2_

- [ ]* 43. Write integration tests for API endpoints
  - Test POST /api/cart/add-with-customizations
  - Test PUT /api/cart/items/{id}/customizations
  - Test GET /api/cart
  - Test error responses (400, 401, 403, 404)
  - _Requirements: 3.1, 4.1, 8.1_

---

## Phase 7: Analytics & Reporting

- [x] 44. Create analytics service for customization patterns





  - Method: GetMostPopularOptionsAsync() → Task<Dictionary<string, int>>
  - Query OrderItem.CustomizationsJson
  - Aggregate option usage across all orders
  - Calculate total quantity per option
  - _Requirements: 9.1, 9.2_

- [ ] 45. Create analytics endpoint
  - GET /api/analytics/customization-patterns
  - Return option name, total quantity, percentage of orders
  - Handle division by zero
  - _Requirements: 9.3_

- [ ]* 46. Write unit tests for analytics
  - Test GetMostPopularOptionsAsync with sample data
  - Test aggregation accuracy
  - Test percentage calculation
  - Test empty results handling
  - _Requirements: 9.1, 9.2, 9.3_

---

## Phase 8: Property-Based Testing

- [ ]* 47. Write property test: JSON round-trip for customizable options
  - **Feature: package-customizable-products, Property 1: JSON serialization round-trip**
  - Generate random CustomizableOptionDto
  - Serialize to JSON
  - Deserialize back
  - Verify equivalent to original
  - **Validates: Requirements 1.4, 7.1**

- [ ]* 48. Write property test: Price calculation accuracy
  - **Feature: package-customizable-products, Property 2: Final price calculation**
  - Generate random basePrice and customizations
  - Calculate finalPrice
  - Verify: finalPrice = basePrice + sum(customization quantities × unit prices)
  - **Validates: Requirements 3.3**

- [ ]* 49. Write property test: Quantity validation
  - **Feature: package-customizable-products, Property 3: Quantity constraint validation**
  - Generate random quantities and constraints
  - Verify: quantity within bounds → validation passes
  - Verify: quantity outside bounds → validation fails
  - **Validates: Requirements 1.3, 3.1**

- [ ]* 50. Write property test: Customization snapshot preservation
  - **Feature: package-customizable-products, Property 4: Order item snapshot**
  - Generate random CartItem with customizations
  - Create OrderItem from CartItem
  - Verify: all customization data preserved
  - Verify: prices preserved
  - **Validates: Requirements 5.1, 5.2**

- [ ]* 51. Write property test: Product update isolation
  - **Feature: package-customizable-products, Property 5: Product update isolation**
  - Create product with options
  - Create order with customizations
  - Update product options
  - Verify: order item retains original customizations
  - **Validates: Requirements 6.2**

---

## Phase 9: Integration & Validation

- [ ] 52. Checkpoint - Ensure all tests pass
  - Run all unit tests
  - Run all property-based tests
  - Run all integration tests
  - Fix any failures
  - Verify code coverage > 80%

- [ ] 53. End-to-end test: Complete package product workflow
  - Create package product with options
  - Add to cart with customizations
  - Update cart item customizations
  - Create order from cart
  - Verify order item snapshots
  - Verify analytics data

- [ ] 54. Performance testing
  - Test JSON serialization/deserialization performance
  - Test cart operations with large customization lists
  - Test analytics queries on large order datasets
  - Optimize if needed

- [ ] 55. Documentation
  - Add API documentation for new endpoints
  - Add code comments for complex logic
  - Create user guide for merchants (creating packages)
  - Create user guide for customers (customizing packages)


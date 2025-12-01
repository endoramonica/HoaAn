# Implementation Plan: Checkout CustomerId Fix

## Overview
This plan addresses critical issues in CheckoutService related to CustomerId handling, authorization, and code compilation errors. The current implementation has a broken GetOrdersAsync method and needs consistent CustomerId retrieval logic.

---

## Tasks

- [x] 1. Fix Critical Syntax Errors in CheckoutService


  - Fix the broken GetOrdersAsync method that has incomplete code and syntax errors
  - Remove orphaned code fragments between GetOrderByIdAsync and CancelOrderAsync
  - Ensure all methods are properly enclosed within the CheckoutService class
  - Verify code compiles without syntax errors
  - _Requirements: 6.1, 6.2, 6.3, 6.4_

- [-] 2. Implement Helper Method for Unified CustomerId Retrieval




  - Create private async method `GetCustomerIdAsync()` in CheckoutService
  - Implement logic to:
    1. Check if `_currentUser.CustomerId` is available and not `Guid.Empty`, return it
    2. If not available, retrieve `UserId` from `_currentUser.UserId`
    3. Query `_customerRepository.GetByUserIdAsync(userId)` to get Customer entity
    4. If customer is null, throw `UnauthorizedAccessException` with message "Customer not found for user"
    5. Return valid `CustomerId`
  - Add proper logging for each step
  - _Requirements: 1.1, 1.2, 1.3, 1.4_

- [x] 3. Implement Complete GetOrdersAsync Method


  - Create complete method signature: `Task<ApiResponse<List<OrderDetailDto>>> GetOrdersAsync(Guid userId, OrderFilterDTO filter)`
  - Use `GetCustomerIdAsync()` helper to retrieve CustomerId
  - Call `_orderRepository.GetOrdersByCustomerIdAsync(customerId, filter)` to get paginated orders
  - Map Order entities to OrderDetailDto using `MapOrderToDetailDto` helper
  - Return ApiResponse with list of OrderDetailDto
  - Add logging for order retrieval count
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [x] 4. Refactor GetOrderByIdAsync to Use Helper Method


  - Replace direct `_currentUser.CustomerId` usage with `GetCustomerIdAsync()` call
  - Update authorization check to use CustomerId from helper
  - Ensure proper error handling for unauthorized access
  - Keep existing order not found logic
  - _Requirements: 2.1, 2.2, 2.3, 2.4_

- [x] 5. Refactor CancelOrderAsync to Use Helper Method


  - Replace direct `_currentUser.CustomerId` usage with `GetCustomerIdAsync()` call
  - Update authorization check to use CustomerId from helper
  - Ensure cancellation reason and timestamp are recorded
  - Keep existing status validation logic
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_

- [x] 6. Refactor CheckoutAsync to Use Helper Method


  - Replace existing CustomerId logic with `GetCustomerIdAsync()` call
  - Remove the conditional check for `_currentUser.CustomerId != Guid.Empty`
  - Let the helper method handle Customer creation if needed (via EnsureCustomerExistsAsync)
  - Ensure order is created with correct CustomerId from helper
  - Keep existing cart validation and order creation logic
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7_

- [x] 7. Add Missing ICustomerRepository Dependency


  - Add `ICustomerRepository _customerRepository` field to CheckoutService
  - Update constructor to inject ICustomerRepository
  - Assign the injected repository to the field
  - _Requirements: 1.2, 1.5_

- [x] 8. Final Verification and Testing



  - Run build to ensure no compilation errors
  - Verify all ICheckoutService interface methods are implemented
  - Check that no UserId is used for authorization (only CustomerId)
  - Ensure all methods use the GetCustomerIdAsync helper consistently
  - Verify proper error handling and logging throughout
  - _Requirements: 6.1, 6.2, 6.3, 6.4_

---

## Notes
- Each task builds incrementally on previous tasks
- Task 1 must be completed first to fix syntax errors
- Tasks 2-6 implement the core CustomerId logic
- Task 7 adds the missing dependency
- Task 8 provides final verification

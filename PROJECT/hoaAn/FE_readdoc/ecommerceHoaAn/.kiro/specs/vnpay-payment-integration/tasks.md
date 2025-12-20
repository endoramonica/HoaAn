# VNPay Payment Integration - Implementation Plan

## Overview

This implementation plan converts the VNPay Payment Integration design into a series of actionable coding tasks. Each task builds incrementally on previous tasks, with no orphaned code. Tasks are sequenced to validate core functionality early through tests.

---

## Implementation Tasks

- [x] 1. Backend: Set up Payment entity and database schema


  - Create Payment entity with all required fields (Id, OrderId, MethodId, Amount, Status, TransactionRef, ResponseCode, PaidAt, ErrorMessage)
  - Create PaymentStatus enum (Pending, Paid, Failed, Cancelled)
  - Add DbSet<Payment> to DbContext
  - Create database migration
  - _Requirements: 4.1, 4.2, 4.3_

- [x] 2. Backend: Create PaymentWebhookController



  - Create PaymentWebhookController with [AllowAnonymous] attribute
  - Implement GET /api/v1/payment/vnpay/ipn endpoint
  - Implement GET /api/v1/payment/status/{orderId} endpoint
  - Add basic error handling and logging
  - _Requirements: 1.3, 4.5_

- [x] 3. Backend: Enhance VNPayService with callback handling





  - Add GetAmount() method to extract amount from callback data
  - Add GetTransactionDate() method to extract transaction date
  - Add method to convert VND amount (in 100s) to decimal
  - Add logging for all signature validation attempts
  - _Requirements: 1.3, 3.2, 3.3_

- [ ] 4. Backend: Create PaymentService for callback processing
  - Create IPaymentService interface with ProcessVNPayCallbackAsync, GetPaymentStatusAsync, UpdateOrderPaymentStatusAsync
  - Implement ProcessVNPayCallbackAsync to:
    - Validate secure hash signature
    - Extract callback data (TxnRef, TransactionNo, ResponseCode, Amount)
    - Validate amount matches order amount
    - Check for duplicate callbacks (idempotency)
    - Update Payment record based on response code
    - Update Order status based on payment result
  - Implement GetPaymentStatusAsync to retrieve Payment record
  - Implement UpdateOrderPaymentStatusAsync to update Order status
  - Add comprehensive logging and error handling
  - _Requirements: 1.3, 1.4, 1.5, 3.4, 4.2, 4.3, 5.2, 5.4, 5.5_

- [ ] 5. Backend: Implement VNPay IPN callback handler
  - In PaymentWebhookController.VnpayIpn():
    - Extract query parameters from request
    - Call VnpayService.ValidateSignature()
    - If invalid: log security alert, return HTTP 400
    - If valid: call PaymentService.ProcessVNPayCallbackAsync()
    - Handle edge cases (non-existent order, database errors)
    - Return HTTP 200 "OK" for successful processing
  - _Requirements: 1.3, 1.4, 1.5, 5.1, 5.5_

- [ ] 6. Backend: Implement payment status retrieval endpoint
  - In PaymentWebhookController.GetPaymentStatus():
    - Extract orderId from route parameter
    - Call PaymentService.GetPaymentStatusAsync()
    - Return PaymentStatusDto with current status
    - Handle non-existent orders gracefully
  - _Requirements: 4.4, 4.5_

- [ ]* 6.1 Backend: Write property tests for VNPayService
  - **Property 1: Payment URL Contains All Required Parameters**
  - **Validates: Requirements 1.1, 3.1**
  - **Property 2: Secure Hash Validation**
  - **Validates: Requirements 1.3, 3.2, 3.3**

- [ ]* 6.2 Backend: Write property tests for PaymentService
  - **Property 3: Successful Payment Updates Order Status**
  - **Validates: Requirements 1.4, 4.2**
  - **Property 4: Failed Payment Updates Order Status**
  - **Validates: Requirements 1.5, 4.3**
  - **Property 5: Payment Amount Validation**
  - **Validates: Requirements 3.4, 5.3**
  - **Property 6: Idempotent Callback Processing**
  - **Validates: Requirements 5.2, 5.4**
  - **Property 7: Invalid Signature Prevents Updates**
  - **Validates: Requirements 1.5, 5.5**
  - **Property 8: Payment Record Creation**
  - **Validates: Requirements 4.1**
  - **Property 9: Payment Status Retrieval**
  - **Validates: Requirements 4.4, 4.5**

- [ ] 7. Backend: Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 8. Frontend: Create PaymentStatusPoller hook
  - Create usePaymentStatusPoller hook
  - Implement pollPaymentStatus() function that:
    - Calls GET /api/v1/payment/status/{orderId} repeatedly
    - Waits 2 seconds between attempts
    - Stops when status changes to Paid or Failed
    - Supports configurable max attempts (default 30)
  - Implement cancelPolling() to stop polling
  - Add error handling and logging
  - _Requirements: 4.5_

- [ ] 9. Frontend: Enhance CheckoutPage for VNPay redirect
  - In handlePlaceOrder():
    - After successful checkout, check if paymentMethod is VNPay
    - If VNPay: call paymentService.createPayment() with order details
    - If paymentResponse.paymentUrl exists: redirect with window.location.href
    - Save orderId and orderNumber to sessionStorage for later retrieval
  - Add logging for payment redirect
  - _Requirements: 1.1, 2.1_

- [ ] 10. Frontend: Create OrderResultPage component
  - Create OrderResultPage component that:
    - Extracts VNPay response parameters from URL (vnp_ResponseCode, vnp_TxnRef, etc.)
    - Displays success page if response code is "00"
    - Displays failed page if response code is not "00"
    - Shows appropriate error message based on response code
    - Provides retry button for failed payments
  - Implement error message mapping for all VNPay response codes
  - Add logging for payment results
  - _Requirements: 2.1, 2.2, 2.3, 6.1, 6.2, 6.3, 6.4, 6.5_

- [ ] 11. Frontend: Implement payment status polling in OrderResultPage
  - In OrderResultPage:
    - Use usePaymentStatusPoller hook to poll payment status
    - Start polling when component mounts
    - Update UI when status changes
    - Show loading state while polling
    - Handle polling timeout gracefully
  - _Requirements: 4.5_

- [ ] 12. Frontend: Implement retry functionality
  - In OrderResultPage failed page:
    - Add retry button that calls handleRetry()
    - handleRetry() should:
      - Call paymentService.createPayment() again with same order
      - Redirect to VNPay payment gateway
      - NOT create a duplicate order
  - _Requirements: 2.4_

- [ ]* 12.1 Frontend: Write property tests for payment flow
  - **Property 10: Response Code Parameter Extraction**
  - **Validates: Requirements 2.1**
  - **Property 11: Retry Functionality**
  - **Validates: Requirements 2.4**

- [ ] 13. Frontend: Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 14. Integration: Update CheckoutService to create Payment record
  - In CheckoutService.CheckoutAsync():
    - After creating Order, create Payment record with status Pending
    - Store Payment.OrderId = order.Id
    - Store Payment.Amount = order.TotalAmount
    - Store Payment.MethodId from payment method lookup
  - _Requirements: 4.1_

- [ ] 15. Integration: Update CheckoutPage to handle all payment methods
  - Ensure CheckoutPage correctly handles:
    - COD: Navigate to success immediately
    - BankTransfer: Navigate to success with banking info
    - VNPay/Momo/ZaloPay: Redirect to payment gateway
  - Add comprehensive logging for each payment method
  - _Requirements: 1.1, 2.1_

- [ ] 16. Integration: Test complete payment flow
  - Test checkout → VNPay redirect → callback → order update flow
  - Test payment status polling
  - Test retry after failed payment
  - Test concurrent callbacks for same order
  - Verify order status updates correctly
  - Verify Payment record is created and updated
  - _Requirements: 1.1, 1.3, 1.4, 1.5, 2.1, 2.2, 2.3, 2.4, 4.1, 4.2, 4.3, 4.4, 4.5_

- [ ] 17. Final Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.


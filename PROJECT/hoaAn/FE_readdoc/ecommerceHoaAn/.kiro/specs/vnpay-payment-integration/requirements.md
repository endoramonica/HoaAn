# VNPay Payment Integration - Requirements Document

## Introduction

VNPay Payment Integration enables customers to pay for orders using VNPay gateway, which supports multiple payment methods including ATM, credit cards, and e-wallets. This feature completes the checkout flow by handling payment processing, callback validation, and order status updates based on payment results.

The system must securely handle payment transactions, validate VNPay signatures, update order status based on payment outcomes, and provide clear feedback to users about payment success or failure.

## Glossary

- **VNPay**: Vietnamese payment gateway supporting multiple payment methods
- **IPN (Instant Payment Notification)**: Server-to-server callback from VNPay to notify payment result
- **Return URL**: Client-side redirect URL after payment completion
- **Secure Hash**: HMAC-SHA512 signature for validating VNPay requests
- **Transaction Reference (TxnRef)**: Unique order identifier sent to VNPay
- **Response Code**: VNPay status code indicating payment result (00=success, others=failure)
- **Payment Status**: Order payment state (Pending, Paid, Failed, Cancelled)
- **Merchant Code (TmnCode)**: VNPay merchant identifier
- **Hash Secret**: Secret key for generating secure hash

## Requirements

### Requirement 1

**User Story:** As a customer, I want to pay for my order using VNPay, so that I can use my preferred payment method (ATM, credit card, e-wallet).

#### Acceptance Criteria

1. WHEN a customer selects VNPay as payment method during checkout THEN the system SHALL generate a secure payment URL with order details and redirect the customer to VNPay gateway
2. WHEN the customer completes payment on VNPay THEN VNPay SHALL send an IPN callback to the backend with payment result
3. WHEN the backend receives VNPay IPN callback THEN the system SHALL validate the secure hash signature to ensure authenticity
4. WHEN the secure hash is valid and response code is "00" THEN the system SHALL update the order status to "Paid" and record the transaction ID
5. WHEN the secure hash is invalid or response code indicates failure THEN the system SHALL update the order status to "Failed" and log the error

### Requirement 2

**User Story:** As a customer, I want to see the payment result after returning from VNPay, so that I know whether my payment succeeded or failed.

#### Acceptance Criteria

1. WHEN the customer is redirected back from VNPay THEN the system SHALL extract VNPay response parameters from the URL
2. WHEN the response code is "00" THEN the system SHALL display the order success page with order details and confirmation
3. WHEN the response code indicates failure THEN the system SHALL display the order failed page with error reason and retry option
4. WHEN a customer clicks retry on the failed page THEN the system SHALL allow the customer to attempt payment again with the same order

### Requirement 3

**User Story:** As a system administrator, I want to ensure payment security, so that customer payment data is protected and transactions are verified.

#### Acceptance Criteria

1. WHEN generating a payment URL THEN the system SHALL include all required VNPay parameters (version, command, merchant code, amount, order info, return URL, etc.)
2. WHEN creating the secure hash THEN the system SHALL use HMAC-SHA512 algorithm with the configured hash secret
3. WHEN receiving VNPay callback THEN the system SHALL validate that the secure hash matches the calculated hash from callback parameters
4. WHEN the payment amount in callback differs from the order amount THEN the system SHALL reject the payment and log a security alert
5. WHEN processing payment THEN the system SHALL never store sensitive payment card information in the database

### Requirement 4

**User Story:** As a customer, I want to track my payment status, so that I can verify if my payment was processed correctly.

#### Acceptance Criteria

1. WHEN a payment is initiated THEN the system SHALL create a Payment record with status "Pending" and store the order reference
2. WHEN VNPay IPN callback is received with success code THEN the system SHALL update the Payment record with transaction ID, response code, and paid timestamp
3. WHEN VNPay IPN callback is received with failure code THEN the system SHALL update the Payment record with failure response code and error details
4. WHEN a customer views their order THEN the system SHALL display the payment status (Pending, Paid, Failed) based on the Payment record
5. WHEN the frontend polls for payment status THEN the system SHALL return the current payment status from the Payment record

### Requirement 5

**User Story:** As a developer, I want to handle edge cases in payment processing, so that the system remains stable under various scenarios.

#### Acceptance Criteria

1. WHEN VNPay callback is received for a non-existent order THEN the system SHALL log the error and return HTTP 400 without crashing
2. WHEN VNPay callback is received multiple times for the same transaction THEN the system SHALL process it idempotently (only update once)
3. WHEN the payment amount in callback is zero or negative THEN the system SHALL reject the payment and log a security alert
4. WHEN VNPay callback is received but the order is already paid THEN the system SHALL log the duplicate callback and return success without re-processing
5. WHEN the secure hash validation fails THEN the system SHALL return HTTP 400 and NOT update any order or payment status

### Requirement 6

**User Story:** As a customer, I want to receive clear feedback about payment failures, so that I understand what went wrong and how to resolve it.

#### Acceptance Criteria

1. WHEN payment fails with response code "01" (bank system error) THEN the system SHALL display "Bank system error. Please try again later."
2. WHEN payment fails with response code "02" (card locked) THEN the system SHALL display "Your card is locked. Please contact your bank."
3. WHEN payment fails with response code "10" (user cancelled) THEN the system SHALL display "You cancelled the payment. Click retry to try again."
4. WHEN payment fails with response code "07" (timeout) THEN the system SHALL display "Payment timeout. Please try again."
5. WHEN payment fails with unknown response code THEN the system SHALL display "Payment failed. Please contact support." with the response code for debugging


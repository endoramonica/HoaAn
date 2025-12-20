# VNPay Payment Integration - Design Document

## Overview

The VNPay Payment Integration system handles the complete payment flow from order checkout through payment confirmation. The system securely communicates with VNPay gateway, validates payment callbacks, updates order status, and provides clear user feedback.

The architecture separates concerns into:
- **Payment URL Generation**: Creating secure VNPay payment links
- **Callback Validation**: Verifying VNPay IPN signatures
- **Order Status Management**: Updating order and payment records
- **User Feedback**: Displaying payment results to customers

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         Frontend (React)                        │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────┐  │
│  │  CheckoutPage    │  │  PaymentPage     │  │  OrderResult │  │
│  │  - Select method │  │  - Show status   │  │  - Success/  │  │
│  │  - Redirect      │  │  - Poll status   │  │    Failed    │  │
│  └──────────────────┘  └──────────────────┘  └──────────────┘  │
└────────────────────────────┬────────────────────────────────────┘
                             │
                    ┌────────▼────────┐
                    │   VNPay Gateway │
                    │  - Payment form │
                    │  - Processing   │
                    └────────┬────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│                      Backend (ASP.NET Core)                     │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              PaymentWebhookController                    │  │
│  │  - POST /api/v1/payment/vnpay/ipn (IPN callback)        │  │
│  │  - GET /api/v1/payment/status/{orderId} (Status check)  │  │
│  └──────────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              VNPayService                                │  │
│  │  - CreatePaymentUrl()                                    │  │
│  │  - ValidateSignature()                                   │  │
│  │  - ExtractResponseData()                                 │  │
│  └──────────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              PaymentService                              │  │
│  │  - ProcessVNPayCallback()                                │  │
│  │  - UpdateOrderPaymentStatus()                            │  │
│  │  - GetPaymentStatus()                                    │  │
│  └──────────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │              Database                                    │  │
│  │  - Order (status: Pending/Paid/Failed)                  │  │
│  │  - Payment (transactionId, responseCode, paidAt)        │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

## Components and Interfaces

### Backend Components

#### 1. PaymentWebhookController
```csharp
[ApiController]
[Route("api/v1/payment")]
[AllowAnonymous] // VNPay IPN doesn't require auth
public class PaymentWebhookController : ControllerBase
{
    [HttpGet("vnpay/ipn")]
    public async Task<IActionResult> VnpayIpn()
    
    [HttpGet("status/{orderId}")]
    public async Task<IActionResult> GetPaymentStatus(string orderId)
}
```

#### 2. VNPayService (Enhanced)
```csharp
public interface IVnpayService
{
    string CreatePaymentUrl(string orderId, decimal amount, string ipAddress);
    bool ValidateSignature(IDictionary<string, string> data, string secureHash);
    string GetResponseCode(IDictionary<string, string> data);
    string GetTransactionRef(IDictionary<string, string> data);
    decimal GetAmount(IDictionary<string, string> data);
}
```

#### 3. PaymentService (New)
```csharp
public interface IPaymentService
{
    Task<PaymentCallbackResult> ProcessVNPayCallbackAsync(
        IDictionary<string, string> callbackData);
    Task<PaymentStatusDto> GetPaymentStatusAsync(string orderId);
    Task UpdateOrderPaymentStatusAsync(string orderId, string status);
}
```

### Frontend Components

#### 1. CheckoutPage (Enhanced)
- Handles payment method selection
- Calls checkout API
- Redirects to VNPay for online payments

#### 2. PaymentStatusPoller (New)
```typescript
interface PaymentStatusPoller {
  pollPaymentStatus(orderId: string, maxAttempts?: number): Promise<PaymentStatus>;
  cancelPolling(): void;
}
```

#### 3. OrderResultPage (Enhanced)
- Displays success/failure based on VNPay response code
- Shows error messages mapped to response codes
- Provides retry option for failed payments

## Data Models

### Backend

#### Payment Entity (Enhanced)
```csharp
public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid MethodId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } // Pending, Paid, Failed
    public string? TransactionRef { get; set; } // VNPay transaction ID
    public string? ResponseCode { get; set; }   // VNPay response code
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Failed = 2,
    Cancelled = 3
}
```

#### VNPay Callback Data
```csharp
public class VNPayCallbackData
{
    public string TxnRef { get; set; }           // Order ID
    public string TransactionNo { get; set; }    // VNPay transaction ID
    public string ResponseCode { get; set; }     // Payment result code
    public decimal Amount { get; set; }          // Payment amount (in VND * 100)
    public string OrderInfo { get; set; }        // Order description
    public DateTime TransactionDate { get; set; } // Transaction timestamp
    public string SecureHash { get; set; }       // HMAC-SHA512 signature
}
```

### Frontend

#### PaymentStatus DTO
```typescript
interface PaymentStatusDto {
  orderId: string;
  status: 'pending' | 'paid' | 'failed' | 'cancelled';
  transactionId?: string;
  responseCode?: string;
  paidAt?: string;
  errorMessage?: string;
}

interface VNPayResponseParams {
  vnp_ResponseCode: string;
  vnp_TxnRef: string;
  vnp_TransactionNo: string;
  vnp_Amount: string;
  vnp_OrderInfo: string;
  vnp_TransactionDate: string;
  vnp_SecureHash: string;
}
```

## Correctness Properties

A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.

### Property 1: Payment URL Contains All Required Parameters
*For any* order with valid amount and order ID, the generated VNPay payment URL SHALL contain all required parameters: version, command, merchant code, amount, order info, return URL, and secure hash.

**Validates: Requirements 1.1, 3.1**

### Property 2: Secure Hash Validation
*For any* VNPay callback data, the system SHALL accept callbacks with valid HMAC-SHA512 signatures and reject callbacks with invalid signatures.

**Validates: Requirements 1.3, 3.2, 3.3**

### Property 3: Successful Payment Updates Order Status
*For any* order with a valid VNPay callback containing response code "00", the system SHALL update the order status to "Paid" and record the transaction ID.

**Validates: Requirements 1.4, 4.2**

### Property 4: Failed Payment Updates Order Status
*For any* order with a valid VNPay callback containing a non-"00" response code, the system SHALL update the order status to "Failed" and record the response code.

**Validates: Requirements 1.5, 4.3**

### Property 5: Payment Amount Validation
*For any* VNPay callback, if the callback amount differs from the order amount, the system SHALL reject the payment and NOT update the order status.

**Validates: Requirements 3.4, 5.3**

### Property 6: Idempotent Callback Processing
*For any* VNPay callback received multiple times with the same transaction reference, the system SHALL process it idempotently—only the first callback updates the order status, subsequent callbacks are logged but don't cause re-processing.

**Validates: Requirements 5.2, 5.4**

### Property 7: Invalid Signature Prevents Updates
*For any* VNPay callback with an invalid secure hash, the system SHALL return HTTP 400 and NOT update any order or payment status.

**Validates: Requirements 1.5, 5.5**

### Property 8: Payment Record Creation
*For any* order initiated for payment, the system SHALL create a Payment record with status "Pending" and store the order reference.

**Validates: Requirements 4.1**

### Property 9: Payment Status Retrieval
*For any* order with a Payment record, the system SHALL return the current payment status (Pending, Paid, Failed) when queried.

**Validates: Requirements 4.4, 4.5**

### Property 10: Response Code Parameter Extraction
*For any* VNPay return URL with response parameters, the system SHALL correctly extract the response code and other parameters from the URL query string.

**Validates: Requirements 2.1**

### Property 11: Retry Functionality
*For any* failed payment, the system SHALL allow the customer to retry payment with the same order, generating a new payment URL without creating a duplicate order.

**Validates: Requirements 2.4**

## Error Handling

### VNPay Response Codes

| Code | Meaning | Action | User Message |
|------|---------|--------|--------------|
| 00 | Success | Update to Paid | "Payment successful!" |
| 01 | Bank system error | Update to Failed | "Bank system error. Please try again later." |
| 02 | Card/Account locked | Update to Failed | "Your card is locked. Please contact your bank." |
| 03 | Card/Account expired | Update to Failed | "Your card has expired. Please use another card." |
| 04 | Transaction declined | Update to Failed | "Transaction declined. Please try another card." |
| 05 | Insufficient funds | Update to Failed | "Insufficient funds. Please check your balance." |
| 06 | Incorrect OTP | Update to Failed | "Incorrect OTP. Please try again." |
| 07 | Transaction timeout | Update to Failed | "Payment timeout. Please try again." |
| 09 | Card not registered | Update to Failed | "Card not registered for online payment." |
| 10 | Cancelled by user | Update to Cancelled | "You cancelled the payment. Click retry to try again." |
| 11 | Invalid amount | Update to Failed | "Invalid payment amount. Please contact support." |
| 12 | Merchant not found | Log error | "Payment gateway error. Please contact support." |
| Other | Unknown error | Update to Failed | "Payment failed. Please contact support. (Code: {code})" |

### Error Scenarios

1. **Non-existent Order**: Log error, return HTTP 400, don't crash
2. **Invalid Signature**: Log security alert, return HTTP 400, don't update status
3. **Amount Mismatch**: Log security alert, reject payment, don't update status
4. **Duplicate Callback**: Log as duplicate, return success, don't re-process
5. **Database Error**: Log error, return HTTP 500, retry mechanism in place

## Testing Strategy

### Unit Tests

- Test VNPay URL generation with various order amounts
- Test signature validation with valid and invalid signatures
- Test response code extraction from callback data
- Test amount validation logic
- Test error message mapping for all response codes
- Test Payment record creation and updates
- Test idempotency logic for duplicate callbacks

### Property-Based Tests

Each correctness property SHALL be implemented as a property-based test using a PBT library:

**For C# Backend**: Use FsCheck or QuickCheck.NET
- Generate random order IDs, amounts, and callback data
- Run 100+ iterations per property
- Verify properties hold across all generated inputs

**For TypeScript Frontend**: Use fast-check
- Generate random VNPay response parameters
- Generate random payment statuses
- Run 100+ iterations per property
- Verify UI renders correctly for all scenarios

### Test Annotation Format

Each property-based test SHALL be tagged with:
```
**Feature: vnpay-payment-integration, Property {number}: {property_text}**
**Validates: Requirements {requirement_numbers}**
```

Example:
```csharp
[Property]
public void Property1_PaymentUrlContainsAllRequiredParameters()
{
    // **Feature: vnpay-payment-integration, Property 1: Payment URL Contains All Required Parameters**
    // **Validates: Requirements 1.1, 3.1**
}
```

### Integration Tests

- Test complete checkout → payment → callback → order update flow
- Test payment status polling
- Test retry after failed payment
- Test concurrent callbacks for same order


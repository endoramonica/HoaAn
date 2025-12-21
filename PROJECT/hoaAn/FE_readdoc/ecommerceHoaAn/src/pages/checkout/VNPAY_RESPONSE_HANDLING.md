# VNPay Response Handling Implementation

## Overview

This document describes the implementation of VNPay payment response handling in the checkout flow. The system extracts VNPay response parameters from the URL, maps response codes to user-friendly messages, and routes users to appropriate success or failed pages.

## Architecture

### Flow Diagram

```
VNPay Gateway
    ↓
Return URL with parameters
    ↓
CheckoutFlow (extracts & routes)
    ↓
    ├─→ Response Code "00" → OrderSuccessPage
    └─→ Other codes → OrderFailedPage
```

## Components

### 1. CheckoutFlow.tsx

**Responsibility**: Extract VNPay response parameters and route to appropriate page

**Key Functions**:
- `extractVNPayResponseParams()`: Extracts all VNPay parameters from URL query string
- Routes based on `vnp_ResponseCode`:
  - "00" → Success page
  - Other codes → Failed page

**VNPay Parameters Extracted**:
- `vnp_ResponseCode`: Payment result code
- `vnp_TxnRef`: Order ID (transaction reference)
- `vnp_TransactionNo`: VNPay transaction ID
- `vnp_Amount`: Payment amount (in VND * 100)
- `vnp_OrderInfo`: Order description
- `vnp_TransactionDate`: Transaction timestamp (YYYYMMDDHHmmss)

**Logging**:
- Logs all extracted parameters for audit trail
- Logs transaction details with timestamp
- Logs routing decision (success/failed)

### 2. OrderSuccessPage.tsx

**Responsibility**: Display successful payment confirmation with transaction details

**New Features**:
- Displays VNPay transaction details section when available
- Shows transaction ID, reference, amount, and date
- Formats VNPay amount (divides by 100 to convert from VND * 100)
- Formats transaction date from YYYYMMDDHHmmss format

**VNPay Transaction Details Display**:
```
Chi Tiết Giao Dịch VNPay
├─ Mã giao dịch VNPay: VNP-TXN-123456789
├─ Mã tham chiếu: ORD-2024-001
├─ Số tiền thanh toán: 1.000.000₫
└─ Thời gian giao dịch: 15/01/2024 14:30:25
```

### 3. OrderFailedPage.tsx

**Responsibility**: Display payment failure with error details and retry option

**New Features**:
- Displays VNPay transaction details including error code
- Shows user-friendly error message based on response code
- Provides retry button to attempt payment again
- Logs payment failure for audit trail

**VNPay Transaction Details Display**:
```
Chi Tiết Giao Dịch VNPay
├─ Mã giao dịch VNPay: VNP-TXN-123456789
├─ Mã tham chiếu: ORD-2024-001
├─ Mã lỗi VNPay: 10
├─ Số tiền thanh toán: 1.000.000₫
└─ Thời gian giao dịch: 15/01/2024 14:30:25
```

## VNPay Response Code Mapping

| Code | Message | User Message | Action |
|------|---------|--------------|--------|
| 00 | Giao dịch thành công | Thanh toán thành công! | Success |
| 01 | Lỗi hệ thống ngân hàng | Lỗi hệ thống ngân hàng. Vui lòng thử lại sau. | Failed |
| 02 | Thẻ/Tài khoản bị khóa | Thẻ hoặc tài khoản của bạn bị khóa. Vui lòng liên hệ ngân hàng. | Failed |
| 03 | Thẻ/Tài khoản hết hạn | Thẻ hoặc tài khoản của bạn đã hết hạn. Vui lòng sử dụng thẻ khác. | Failed |
| 04 | Giao dịch bị từ chối | Giao dịch bị từ chối. Vui lòng thử lại hoặc sử dụng thẻ khác. | Failed |
| 05 | Số dư không đủ | Số dư tài khoản không đủ. Vui lòng kiểm tra lại số dư. | Failed |
| 06 | Nhập sai OTP | Bạn nhập sai mã OTP. Vui lòng thử lại. | Failed |
| 07 | Hết thời gian chờ | Giao dịch hết thời gian chờ. Vui lòng thử lại. | Failed |
| 09 | Thẻ chưa đăng ký dịch vụ | Thẻ của bạn chưa được đăng ký dịch vụ thanh toán online. Vui lòng liên hệ ngân hàng. | Failed |
| 10 | Khách hàng hủy giao dịch | Bạn đã hủy giao dịch. Nhấp vào "Thử lại" để thanh toán lại. | Failed |
| 11 | Số tiền không hợp lệ | Số tiền thanh toán không hợp lệ. Vui lòng liên hệ hỗ trợ. | Failed |
| 12 | Merchant không tồn tại | Lỗi cấu hình cổng thanh toán. Vui lòng liên hệ hỗ trợ. | Failed |
| Other | Lỗi không xác định | Thanh toán thất bại. Mã lỗi: {code}. Vui lòng liên hệ hỗ trợ. | Failed |

## Data Flow

### Success Flow

```
1. Customer completes payment on VNPay
2. VNPay redirects to: /checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-001&...
3. CheckoutFlow extracts parameters
4. Response code "00" detected
5. Route to OrderSuccessPage
6. Display success message with transaction details
7. Log success event for audit trail
```

### Failure Flow

```
1. Customer's payment fails on VNPay
2. VNPay redirects to: /checkout?vnp_ResponseCode=10&vnp_TxnRef=ORD-001&...
3. CheckoutFlow extracts parameters
4. Response code "10" detected (user cancelled)
5. Route to OrderFailedPage
6. Display error message: "Bạn đã hủy giao dịch..."
7. Show transaction details for reference
8. Provide retry button
9. Log failure event for audit trail
```

## Helper Functions

### formatVNPayAmount(amountStr?: string): string

Converts VNPay amount format (VND * 100) to user-friendly format.

**Example**:
```typescript
formatVNPayAmount('100000000') // Returns: "1.000.000₫"
formatVNPayAmount('50000000')  // Returns: "500.000₫"
```

### formatTransactionDate(dateStr?: string): string

Converts VNPay date format (YYYYMMDDHHmmss) to user-friendly format.

**Example**:
```typescript
formatTransactionDate('20240115143025') // Returns: "15/01/2024 14:30:25"
formatTransactionDate('20240101000000') // Returns: "01/01/2024 00:00:00"
```

## Logging and Tracking

### Success Logging

```typescript
console.log('[CheckoutFlow] ✅ VNPay payment successful - routing to success page');
console.log('[CheckoutFlow] 📊 Response code info:', responseCodeInfo);
console.log('[OrderSuccessPage] ✅ Order loaded:', {
  orderId: orderDetails.orderId,
  orderNumber: orderDetails.orderNumber,
  status: orderDetails.status
});
```

### Failure Logging

```typescript
console.log('[CheckoutFlow] ❌ VNPay payment failed - routing to failed page');
console.log('[OrderFailedPage] ❌ Payment failed:', {
  timestamp: new Date().toISOString(),
  orderId: errorData?.orderId,
  responseCode: errorData?.vnpayResponseCode,
  transactionId: errorData?.vnpayTransactionNo,
  reason: errorData?.reason,
});
```

### Audit Trail

All payment transactions are logged with:
- Timestamp (ISO format)
- Order ID
- Response code
- Transaction ID
- Amount
- Payment method
- Success/failure status

## Testing

### Test Files

1. **CheckoutFlow.vnpay.test.tsx**
   - Tests VNPay response parameter extraction
   - Tests routing based on response codes
   - Tests all response code mappings
   - Tests legacy parameter support
   - Tests edge cases

2. **OrderResultPages.vnpay.test.tsx**
   - Tests amount formatting
   - Tests date formatting
   - Tests response code mapping
   - Tests transaction details display
   - Tests retry functionality
   - Tests logging and tracking

### Running Tests

```bash
# Run all tests
npm test

# Run specific test file
npm test CheckoutFlow.vnpay.test.tsx

# Run with coverage
npm test -- --coverage
```

## URL Examples

### Success Response

```
/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_TransactionNo=VNP-TXN-123456&vnp_Amount=100000000&vnp_OrderInfo=Order%20for%20customer&vnp_TransactionDate=20240115143025
```

### Failure Response (User Cancelled)

```
/checkout?vnp_ResponseCode=10&vnp_TxnRef=ORD-2024-001&vnp_TransactionNo=VNP-TXN-123456&vnp_Amount=100000000&vnp_TransactionDate=20240115143025
```

### Failure Response (Bank Error)

```
/checkout?vnp_ResponseCode=01&vnp_TxnRef=ORD-2024-001&vnp_TransactionNo=VNP-TXN-123456&vnp_Amount=100000000&vnp_TransactionDate=20240115143025
```

## Retry Functionality

When a payment fails, users can click "Thử lại thanh toán" (Retry Payment) button:

1. Button calls `handleRetry()` function
2. Navigates back to checkout page
3. Same order ID is preserved
4. New payment URL is generated
5. User is redirected to VNPay again
6. No duplicate order is created

## Edge Cases Handled

1. **Missing Response Code**: Shows checkout page
2. **Unknown Response Code**: Treats as failure with generic error message
3. **Empty Parameters**: Handled gracefully with defaults
4. **Special Characters**: Properly URL-encoded and decoded
5. **Large Amounts**: Formatted correctly regardless of size
6. **Invalid Date Format**: Falls back to original string

## Requirements Coverage

This implementation satisfies the following requirements:

- **Requirement 2.1**: Extract VNPay response parameters from URL ✅
- **Requirement 2.2**: Display success page when response code is "00" ✅
- **Requirement 2.3**: Display failed page with error message for other codes ✅
- **Requirement 2.4**: Provide retry functionality ✅
- **Requirement 6.1-6.5**: Map error codes to user-friendly messages ✅

## Future Enhancements

1. Add email notification with transaction details
2. Add SMS notification for payment status
3. Add payment history page with all transactions
4. Add transaction receipt download
5. Add payment status polling for real-time updates
6. Add webhook support for server-side payment confirmation

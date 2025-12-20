# VNPay Payment Flow Analysis - FE & BE

## 📋 Tóm Tắt Hiện Tại

### ✅ Những gì đã được implement:

#### **Backend (BE)**
1. **VnpayService** (`BE/VietCommerce.Application/Services/Payments/VnpayConfig.cs`)
   - Tạo payment URL cho VNPay
   - Validate signature từ VNPay callback
   - Extract response code và transaction ref

2. **CheckoutService** (`BE/VietCommerce.Application/Services/Services/CheckoutService.cs`)
   - Khi user chọn payment method khác COD, gọi `_vnpayService.CreatePaymentUrl()`
   - Trả về `PaymentUrl` trong `CheckoutResponseDto`
   - Redirect FE đến VNPay payment gateway

3. **PaymentController** (`BE/BE/Controllers/PaymentController.cs`)
   - Endpoints: `/api/payment/process`, `/api/payment/process-cash`, `/api/payment/process-card`
   - Nhưng **KHÔNG có endpoint xử lý VNPay IPN callback**

#### **Frontend (FE)**
1. **CheckoutPage** (`ecommerceHoaAn/src/pages/checkout/CheckoutPage.tsx`)
   - User chọn payment method (COD, VNPay, Momo, ZaloPay, BankTransfer)
   - Gọi `processCheckout()` từ `useCheckout` hook
   - Nếu payment method không phải COD/BankTransfer:
     - Gọi `paymentService.createPayment()` để lấy payment URL
     - Redirect: `window.location.href = paymentResponse.paymentUrl`

2. **PaymentService** (`ecommerceHoaAn/src/lib/services/paymentService.ts`)
   - `createPayment()`: Gọi `/payments/create` API (MOCK mode)
   - `handlePaymentCallback()`: Xử lý callback từ payment gateway
   - `checkPaymentStatus()`: Kiểm tra trạng thái thanh toán

3. **OrderSuccessPage** (`ecommerceHoaAn/src/pages/checkout/OrderSuccessPage.tsx`)
   - Hiển thị thông tin đơn hàng sau khi thanh toán thành công
   - Nếu BankTransfer: hiển thị thông tin banking

---

## ⚠️ Vấn Đề Hiện Tại

### **1. Backend - Thiếu VNPay IPN Callback Handler**
- Swagger có endpoint `/api/v1/payment/vnpay/ipn` nhưng **không có implementation**
- Không có controller xử lý callback từ VNPay
- Khi VNPay gọi callback, BE không biết xử lý → **Order không được cập nhật trạng thái**

### **2. Frontend - Xử lý Callback Không Rõ Ràng**
- Sau khi redirect từ VNPay, FE không có cơ chế xử lý callback params
- Không có logic kiểm tra `vnp_ResponseCode` từ VNPay
- Không có retry mechanism nếu payment thất bại

### **3. Thiếu Payment Status Tracking**
- Không có cơ chế polling để kiểm tra trạng thái thanh toán
- Order status không được cập nhật sau khi VNPay callback

---

## 🔄 VNPay Payment Flow Hiện Tại

```
┌─────────────────────────────────────────────────────────────────┐
│ 1. USER CHECKOUT                                                │
│    - Chọn payment method: VNPay                                 │
│    - Click "Place Order"                                        │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│ 2. FE: CheckoutPage.handlePlaceOrder()                          │
│    - Gọi processCheckout(checkoutData)                          │
│    - BE tạo Order (status: Pending)                             │
│    - BE gọi VnpayService.CreatePaymentUrl()                     │
│    - Trả về CheckoutResponseDto với PaymentUrl                 │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│ 3. FE: Redirect to VNPay                                        │
│    - window.location.href = paymentResponse.paymentUrl          │
│    - User nhập thông tin thẻ/ATM                                │
│    - VNPay xử lý thanh toán                                     │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│ 4. VNPay Callback (2 cách)                                      │
│    a) Server-to-Server (IPN):                                   │
│       POST /api/v1/payment/vnpay/ipn                            │
│       ❌ KHÔNG CÓ HANDLER                                        │
│                                                                 │
│    b) Client Redirect:                                          │
│       GET returnUrl?vnp_ResponseCode=00&vnp_TxnRef=xxx          │
│       ❌ FE KHÔNG XỬ LÝ                                          │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│ 5. ❌ PROBLEM: Order Status NOT Updated                         │
│    - Order vẫn ở status: Pending                                │
│    - Payment record không được cập nhật                         │
│    - User không biết thanh toán thành công hay thất bại         │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🛠️ Cần Implement

### **Backend**

#### 1. Tạo PaymentWebhookController
```csharp
[ApiController]
[Route("api/v1/payment")]
[AllowAnonymous] // VNPay IPN không có auth
public class PaymentWebhookController : ControllerBase
{
    private readonly IVnpayService _vnpayService;
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;

    [HttpGet("vnpay/ipn")]
    public async Task<IActionResult> VnpayIpn()
    {
        // 1. Lấy query params từ VNPay
        var vnpayData = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        
        // 2. Validate signature
        var secureHash = vnpayData["vnp_SecureHash"];
        if (!_vnpayService.ValidateSignature(vnpayData, secureHash))
            return BadRequest("Invalid signature");
        
        // 3. Extract order info
        var orderId = vnpayData["vnp_TxnRef"];
        var responseCode = vnpayData["vnp_ResponseCode"];
        var transactionId = vnpayData["vnp_TransactionNo"];
        
        // 4. Update order status based on response code
        if (responseCode == "00") // Success
        {
            await _orderService.UpdateOrderPaymentStatusAsync(orderId, "Paid");
            await _paymentService.RecordPaymentAsync(orderId, transactionId);
        }
        else // Failed
        {
            await _orderService.UpdateOrderPaymentStatusAsync(orderId, "Failed");
        }
        
        return Ok("OK");
    }
}
```

#### 2. Cập nhật CheckoutService
```csharp
// Trong CheckoutAsync, khi tạo Payment:
var payment = new Payment
{
    Id = Guid.NewGuid(),
    OrderId = order.Id,
    MethodId = paymentMethodId,
    Amount = order.TotalAmount,
    Status = PaymentMethodType.PENDING,
    TransactionRef = null, // Sẽ được cập nhật từ VNPay callback
    CreatedAt = DateTime.UtcNow
};
```

#### 3. Cập nhật Payment Entity
```csharp
public class Payment
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid MethodId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethodType Status { get; set; }
    public string? TransactionRef { get; set; } // VNPay transaction ID
    public string? ResponseCode { get; set; }   // VNPay response code
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}
```

### **Frontend**

#### 1. Cập nhật CheckoutPage - Xử lý Callback
```typescript
// Sau khi redirect từ VNPay, FE sẽ nhận được:
// /checkout?step=success&orderId=xxx&orderNumber=xxx&vnp_ResponseCode=00&vnp_TxnRef=xxx

useEffect(() => {
  const params = new URLSearchParams(window.location.search);
  const vnpResponseCode = params.get('vnp_ResponseCode');
  const vnpTxnRef = params.get('vnp_TxnRef');
  
  if (vnpResponseCode) {
    if (vnpResponseCode === '00') {
      // Success
      onNavigate('success', { orderId, orderNumber });
    } else {
      // Failed
      onNavigate('failed', { reason: `Payment failed: ${vnpResponseCode}` });
    }
  }
}, []);
```

#### 2. Cập nhật PaymentService
```typescript
async createPayment(request: PaymentRequest): Promise<PaymentResponse> {
  // Gọi BE endpoint để tạo payment
  const response = await apiRequest.post<PaymentResponse>(
    '/api/v1/payment/create',
    request
  );
  
  // Nếu có paymentUrl, redirect
  if (response.paymentUrl) {
    window.location.href = response.paymentUrl;
  }
  
  return response;
}
```

#### 3. Thêm Payment Status Polling
```typescript
// Polling để kiểm tra trạng thái thanh toán
async function pollPaymentStatus(orderId: string, maxAttempts = 30) {
  for (let i = 0; i < maxAttempts; i++) {
    const status = await paymentService.checkPaymentStatus(orderId);
    
    if (status.status === 'completed') {
      return { success: true, status };
    }
    
    if (status.status === 'failed') {
      return { success: false, status };
    }
    
    // Wait 2 seconds before next attempt
    await new Promise(resolve => setTimeout(resolve, 2000));
  }
  
  return { success: false, reason: 'Timeout' };
}
```

---

## 📝 VNPay Response Codes

| Code | Meaning | Action |
|------|---------|--------|
| 00 | Success | Update order to "Paid" |
| 01 | Bank system error | Retry |
| 02 | Card/Account locked | Show error |
| 03 | Card/Account expired | Show error |
| 04 | Transaction declined | Show error |
| 05 | Insufficient funds | Show error |
| 06 | Incorrect OTP | Retry |
| 07 | Transaction timeout | Retry |
| 09 | Card not registered | Show error |
| 10 | Cancelled by user | Show cancelled message |
| 11 | Invalid amount | Show error |
| 12 | Merchant not found | Show error |

---

## 🔐 VNPay Configuration

Cần setup trong `appsettings.json`:

```json
{
  "VnpayConfig": {
    "TmnCode": "your_merchant_code",
    "HashSecret": "your_hash_secret",
    "PaymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "ReturnUrl": "https://yourdomain.com/checkout?step=success",
    "IpnUrl": "https://yourdomain.com/api/v1/payment/vnpay/ipn"
  }
}
```

---

## ✅ Checklist

- [ ] Tạo PaymentWebhookController với VnpayIpn endpoint
- [ ] Implement VNPay signature validation
- [ ] Cập nhật Order status khi nhận VNPay callback
- [ ] Cập nhật Payment record với transaction ID
- [ ] Xử lý VNPay callback params ở FE
- [ ] Thêm payment status polling
- [ ] Thêm error handling cho failed payments
- [ ] Test với VNPay sandbox
- [ ] Setup IPN URL trong VNPay merchant dashboard


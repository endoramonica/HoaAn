# 🛒 Hướng Dẫn Triển Khai Checkout - Hybrid Navigation

## 📋 Tổng Quan

Hệ thống checkout đã được triển khai với **Hybrid Navigation Approach**:
- **React Router DOM**: Quản lý routes chính (`/`, `/checkout`, `/products`, v.v.)
- **useState**: Quản lý các bước nội bộ trong checkout flow (checkout → success/failed)

## 🏗️ Kiến Trúc

### 1. Cấu Trúc Thư Mục

```
├── pages/
│   └── checkout/
│       ├── CheckoutFlow.tsx        # Quản lý luồng checkout với useState
│       ├── CheckoutPage.tsx        # Trang thanh toán chính
│       ├── OrderSuccessPage.tsx    # Trang thành công
│       └── OrderFailedPage.tsx     # Trang thất bại
├── lib/
│   └── services/
│       ├── paymentService.ts       # Service xử lý thanh toán
│       ├── userService.ts          # Thêm quản lý địa chỉ
│       ├── orderService.ts         # Service đơn hàng
│       └── cartService.ts          # Service giỏ hàng
```

### 2. Luồng Navigation

```
Main App (React Router)
    ├── / (HomePage)
    ├── /products (ProductsPage)
    ├── /cart (CartPage) → Click "Tiến hành thanh toán"
    │                        ↓
    └── /checkout (CheckoutFlow) ← React Router Route
            │
            └── Internal Navigation (useState)
                ├── Step 1: CheckoutPage (chọn địa chỉ, phương thức thanh toán)
                ├── Step 2: OrderSuccessPage (đặt hàng thành công)
                └── Step 3: OrderFailedPage (thanh toán thất bại)
```

## 🔧 Implementation Details

### 0. useHybridNavigate Hook

Custom hook để thống nhất navigation giữa useState và React Router:

```tsx
// lib/hooks/useHybridNavigate.ts
import { useCallback } from "react";
import { useNavigate } from "react-router-dom";

export function useHybridNavigate(onNavigate?: (page: string) => void) {
  const routerNavigate = useNavigate();

  // Nếu có onNavigate từ cha => dùng nó, không thì fallback sang routerNavigate
  const navigateTo = useCallback(
    (page: string) => {
      if (onNavigate) {
        onNavigate(page);
      } else {
        routerNavigate(page.startsWith("/") ? page : `/${page}`);
      }
    },
    [onNavigate, routerNavigate]
  );

  return navigateTo;
}
```

**Cách sử dụng:**

```tsx
// Trong component nhận onNavigate prop
const MyComponent = ({ onNavigate }: { onNavigate?: (page: string) => void }) => {
  const navigate = useHybridNavigate(onNavigate);
  
  return (
    <button onClick={() => navigate('home')}>Go Home</button>
  );
};

// Component hoạt động tốt cả khi:
// 1. Có onNavigate prop (useState-based navigation)
// 2. Không có onNavigate (React Router navigation)
```

### 1. App.tsx - Router Setup

```tsx
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { CheckoutFlow } from './pages/checkout/CheckoutFlow';

export default function App() {
  return (
    <BrowserRouter>
      <QueryClientProvider client={queryClient}>
        <Routes>
          <Route path="/" element={<AppContent />} />
          <Route path="/checkout" element={<CheckoutFlow />} />
          <Route path="*" element={<AppContent />} />
        </Routes>
        <Toaster />
      </QueryClientProvider>
    </BrowserRouter>
  );
}
```

### 2. CheckoutFlow.tsx - State Management

```tsx
type CheckoutStep = 'checkout' | 'success' | 'failed';

export const CheckoutFlow = () => {
  const [currentStep, setCurrentStep] = useState<CheckoutStep>('checkout');
  const [flowData, setFlowData] = useState<CheckoutFlowData>({});

  const handleNavigate = (step: CheckoutStep, data?: CheckoutFlowData) => {
    setCurrentStep(step);
    if (data) {
      setFlowData(prev => ({ ...prev, ...data }));
    }
  };

  switch (currentStep) {
    case 'success':
      return <OrderSuccessPage orderData={flowData} />;
    case 'failed':
      return <OrderFailedPage errorData={flowData} />;
    case 'checkout':
    default:
      return <CheckoutPage onNavigate={handleNavigate} />;
  }
};
```

### 3. CartPage.tsx - Navigation to Checkout

```tsx
import { useHybridNavigate } from '../lib/hooks/useHybridNavigate';

export function CartPage() {
  const navigate = useHybridNavigate();

  return (
    <Button onClick={() => navigate('checkout')}>
      Tiến hành thanh toán
    </Button>
  );
}
```

**Lợi ích của useHybridNavigate:**
- ✅ Tự động detect có onNavigate prop hay không
- ✅ Fallback sang React Router nếu không có onNavigate
- ✅ Code nhất quán trên toàn bộ app
- ✅ Dễ refactor và maintain

## 💳 Payment Flow

### 1. Phương Thức Thanh Toán Hỗ Trợ

- **COD**: Thanh toán khi nhận hàng
- **VNPay**: Cổng thanh toán VNPay
- **Momo**: Ví điện tử MoMo
- **ZaloPay**: Ví điện tử ZaloPay
- **BankTransfer**: Chuyển khoản ngân hàng

### 2. Luồng Thanh Toán

#### a. COD (Cash On Delivery)
```typescript
// Checkout → Create Order → Success (không cần payment URL)
const order = await orderService.createOrder(orderRequest);
onNavigate('success', { orderId: order.id, orderNumber: order.orderNumber });
```

#### b. Bank Transfer
```typescript
// Checkout → Create Order → Success (hiển thị thông tin banking)
const order = await orderService.createOrder(orderRequest);
onNavigate('success', { 
  orderId: order.id, 
  orderNumber: order.orderNumber,
  paymentMethod: 'BankTransfer'
});
```

#### c. Online Payment (VNPay, Momo, ZaloPay)
```typescript
// Checkout → Create Order → Create Payment → Redirect
const order = await orderService.createOrder(orderRequest);
const paymentResponse = await paymentService.createPayment({
  orderId: order.id,
  amount: order.total,
  paymentMethod: selectedPaymentMethod,
  returnUrl: `${window.location.origin}/checkout?payment=success&orderId=${order.id}`,
  cancelUrl: `${window.location.origin}/checkout?payment=failed&orderId=${order.id}`,
});

// Lưu thông tin vào sessionStorage trước khi redirect
sessionStorage.setItem('pendingOrderId', order.id);
sessionStorage.setItem('pendingOrderNumber', order.orderNumber);

// Redirect đến cổng thanh toán
window.location.href = paymentResponse.paymentUrl;
```

## 🔄 CheckoutFlow với URL Params

CheckoutFlow hỗ trợ navigation thông qua URL parameters:

```tsx
// Hỗ trợ các URL sau:
/checkout                                    // Default - hiển thị CheckoutPage
/checkout?step=success&orderId=xxx           // Hiển thị OrderSuccessPage
/checkout?step=failed&reason=xxx             // Hiển thị OrderFailedPage
/checkout?payment=success&orderId=xxx        // Legacy support - success
/checkout?payment=failed&orderId=xxx         // Legacy support - failed
```

**Implementation:**

```tsx
import { useSearchParams } from 'react-router-dom';

export const CheckoutFlow = () => {
  const [searchParams] = useSearchParams();
  
  useEffect(() => {
    const stepParam = searchParams.get('step');
    const paymentParam = searchParams.get('payment');
    const orderId = searchParams.get('orderId');
    
    // Auto-navigate based on URL params
    if (stepParam === 'success') {
      setCurrentStep('success');
      setFlowData({ orderId });
    }
  }, [searchParams]);
  
  // ...
};
```

## 📦 Services

### 1. PaymentService

```typescript
// lib/services/paymentService.ts
class PaymentService {
  async createPayment(request: PaymentRequest): Promise<PaymentResponse>;
  async handlePaymentCallback(paymentMethod: PaymentMethod, callbackData: any): Promise<PaymentCallbackData>;
  async checkPaymentStatus(orderId: string): Promise<PaymentStatusResponse>;
  async getBankingInfo(): Promise<BankingInfo>;
  async getAvailablePaymentMethods(): Promise<PaymentMethod[]>;
}
```

### 2. UserService (Updated)

```typescript
// Thêm quản lý địa chỉ
userService.getAddresses(): Promise<AddressDto[]>
userService.addAddress(address: Omit<AddressDto, 'id'>): Promise<AddressDto>
userService.updateAddress(addressId: string, address: Partial<AddressDto>): Promise<AddressDto>
userService.deleteAddress(addressId: string): Promise<void>
userService.setDefaultAddress(addressId: string): Promise<void>
```

## 🎨 UI Components

### 1. CheckoutPage Features

- ✅ Chọn/thêm địa chỉ giao hàng
- ✅ Chọn phương thức thanh toán (COD, VNPay, Momo, ZaloPay, BankTransfer)
- ✅ Nhập mã giảm giá
- ✅ Ghi chú đơn hàng
- ✅ Tóm tắt đơn hàng (items, giá, phí ship, thuế, giảm giá)
- ✅ Responsive design
- ✅ Thiết kế văn hóa Việt Nam

### 2. OrderSuccessPage Features

- ✅ Hiển thị mã đơn hàng (có nút copy)
- ✅ Thông tin chuyển khoản (nếu chọn BankTransfer)
- ✅ Chi tiết đơn hàng
- ✅ Địa chỉ giao hàng
- ✅ Nút "Xem đơn hàng của tôi" và "Về trang chủ"

### 3. OrderFailedPage Features

- ✅ Thông báo lỗi chi tiết
- ✅ Nguyên nhân có thể
- ✅ Thông tin liên hệ hỗ trợ
- ✅ Gợi ý phương thức thanh toán khác
- ✅ Nút "Thử lại" và "Về trang chủ"

## 🔄 Mock Data Mode

Tất cả services đều hỗ trợ mock mode thông qua biến môi trường:

```env
VITE_USE_MOCK_DATA=true
```

Mock mode features:
- ✅ Địa chỉ mẫu (2 addresses)
- ✅ Phương thức thanh toán mẫu
- ✅ Thông tin banking mẫu
- ✅ Payment response mẫu
- ✅ Order creation simulation

## 🚀 Cách Sử Dụng

### 1. Từ CartPage

```tsx
// User click "Tiến hành thanh toán"
navigate('/checkout');
// → CheckoutFlow được mount với step 'checkout'
```

### 2. Trong CheckoutPage

```tsx
// User điền thông tin và click "Đặt hàng"
const order = await orderService.createOrder(orderRequest);

// Nếu COD hoặc BankTransfer
onNavigate('success', { orderId, orderNumber, paymentMethod });

// Nếu online payment
const paymentResponse = await paymentService.createPayment({...});
window.location.href = paymentResponse.paymentUrl;
```

### 3. Payment Callback

```tsx
// Khi user quay về từ cổng thanh toán
// URL: /checkout?payment=success&orderId=xxx
// CheckoutFlow sẽ:
// 1. Đọc orderId từ URL params hoặc sessionStorage
// 2. Load order details
// 3. Hiển thị OrderSuccessPage
```

## 🔐 Security & Best Practices

1. **Session Management**: 
   - Dùng `sessionStorage` cho pending order khi redirect
   - Clear sessionStorage sau khi load xong

2. **Error Handling**:
   - Try-catch cho tất cả API calls
   - Toast notifications cho user feedback
   - Fallback UI cho loading/error states

3. **Validation**:
   - Validate địa chỉ trước khi đặt hàng
   - Validate giỏ hàng không trống
   - Validate payment method được chọn

4. **Responsive Design**:
   - Mobile-first approach
   - Grid layout responsive
   - Sticky order summary trên desktop

## 📝 Integration với Backend

Khi kết nối với backend .NET Core 8:

1. **Cập nhật API Base URL**:
```typescript
// lib/api/client.ts
const API_BASE_URL = process.env.VITE_API_BASE_URL || 'https://api.yourdomain.com';
```

2. **Tắt Mock Mode**:
```env
VITE_USE_MOCK_DATA=false
```

3. **Payment Gateway Config**:
```env
VITE_VNPAY_MERCHANT_ID=your_merchant_id
VITE_MOMO_PARTNER_CODE=your_partner_code
VITE_ZALOPAY_APP_ID=your_app_id
```

## 🎯 Testing Checklist

- [ ] Navigate từ cart → checkout
- [ ] Chọn địa chỉ có sẵn
- [ ] Thêm địa chỉ mới
- [ ] Chọn từng phương thức thanh toán
- [ ] Áp dụng mã giảm giá
- [ ] Nhập ghi chú đơn hàng
- [ ] Đặt hàng với COD → Success
- [ ] Đặt hàng với BankTransfer → Success (banking info)
- [ ] Đặt hàng với VNPay → Redirect (mock)
- [ ] Cancel payment → Failed page
- [ ] Copy order number
- [ ] Navigate từ success/failed về home/profile

## 🔄 Components Đã Áp Dụng useHybridNavigate

Các components sau đã được cập nhật để sử dụng `useHybridNavigate`:

1. **CheckoutPage** - Navigate về cart, products
2. **OrderSuccessPage** - Navigate về profile, home
3. **OrderFailedPage** - Navigate về checkout retry, home
4. **CartPage** - Navigate đến checkout
5. **LoginPage** - Navigate giữa login/register/home
6. **RegisterPage** - Navigate giữa register/login/home
7. **GoogleLoginButton** - Navigate sau khi login thành công

**Pattern:**

```tsx
// OLD - Direct useNavigate
import { useNavigate } from 'react-router-dom';
const navigate = useNavigate();
onClick={() => navigate('/home')}

// NEW - useHybridNavigate
import { useHybridNavigate } from '../lib/hooks/useHybridNavigate';
const navigate = useHybridNavigate(onNavigate); // onNavigate là optional
onClick={() => navigate('home')} // Không cần '/'
```

## 🆘 Troubleshooting

### Issue: Cannot navigate to /checkout
**Solution**: Đảm bảo BrowserRouter đã được wrap và route đã được định nghĩa

### Issue: useHybridNavigate not working
**Solution**: 
- Check import path đúng: `import { useHybridNavigate } from '../lib/hooks/useHybridNavigate'`
- Đảm bảo component được wrap trong BrowserRouter
- Nếu có onNavigate prop, truyền vào: `useHybridNavigate(onNavigate)`

### Issue: Order data không hiển thị
**Solution**: Check sessionStorage và order ID từ props/URL params

### Issue: Payment redirect không hoạt động
**Solution**: Check CORS settings và payment gateway URLs

## 📚 Tài Liệu Liên Quan

- **[HYBRID_NAVIGATION_GUIDE.md](./HYBRID_NAVIGATION_GUIDE.md)** - Chi tiết về useHybridNavigate hook ⭐
- [API_INTEGRATION_GUIDE.md](./API_INTEGRATION_GUIDE.md)
- [CART_CHECKOUT_INTEGRATION_GUIDE.md](./CART_CHECKOUT_INTEGRATION_GUIDE.md)
- [AUTH_INTEGRATION_GUIDE.md](./AUTH_INTEGRATION_GUIDE.md)
- [BE_DATA_FLOW_SPECIFICATION.md](./BE_DATA_FLOW_SPECIFICATION.md)

---

**Version**: 1.0  
**Last Updated**: 2025-01-03  
**Status**: ✅ Hoàn thiện

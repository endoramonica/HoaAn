# Order Detail Page - Integration Complete

## Overview
Added complete order detail page functionality with navigation from ProfilePage. Users can now click "Xem chi tiết" button to view full order details including items, shipping, and payment information.

## Changes Made

### 1. Created OrderDetailPage (`src/pages/OrderDetailPage.tsx`)
- **Route Parameter**: `/orders/:orderId`
- **API Call**: `GET /api/v1/Order/{id}` via `orderService.getOrderById(orderId)`
- **Features**:
  - Displays order header with order number, date, and status badge
  - Shows all order items with product images, SKU, quantity, unit price, and total
  - Displays order summary with subtotal, discounts, shipping fees, taxes, and total amount
  - Shows shipping information (recipient name, phone, address, ward, district, city, tracking number)
  - Displays payment information (method, status, paid amount, payment date)
  - Loading state with spinner
  - Error handling with user-friendly messages
  - Back button to return to previous page

### 2. Updated ProfilePage (`src/components/ProfilePage.tsx`)
- Added `useNavigate` hook from react-router-dom
- Updated "Xem chi tiết" button with onClick handler:
  ```typescript
  onClick={() => navigate(`/orders/${order.orderId}`)}
  ```
- Button now navigates to `/orders/{orderId}` when clicked

### 3. Updated App.tsx
- Added import for `OrderDetailPage`
- Added new route:
  ```typescript
  <Route path="/orders/:orderId" element={<OrderDetailPage />} />
  ```
- Route is placed in MainLayout so it has header/footer

## Data Structure

### OrderDetailDto (from API)
```typescript
{
  orderId: string
  orderNumber: string
  status: OrderStatus (Pending, Confirmed, Processing, Shipping, Delivered, Cancelled, Refunded)
  createdAt: string (ISO date)
  totalAmount: number
  subTotal: number
  discountAmount: number
  shippingFee: number
  taxAmount: number
  items: OrderItemDTO[]
  shipping: OrderShippingDto
  isPaid: boolean
  paymentMethod: string
  paidAmount: number
  paidAt: string
}
```

### OrderItemDTO
```typescript
{
  productName: string
  productImageUrl: string
  productSKU: string
  quantity: number
  unitPrice: number
  totalPrice: number
}
```

### OrderShippingDto
```typescript
{
  recipientName: string
  phoneNumber: string
  address: string
  ward: string
  district: string
  city: string
  postalCode: string
  trackingNumber: string
  shippingStatus: string
  estimatedDelivery: string
}
```

## User Flow

1. User navigates to Profile page
2. Clicks on Orders tab
3. Orders list loads from API
4. User clicks "Xem chi tiết" button on any order
5. Navigates to `/orders/{orderId}`
6. OrderDetailPage loads order data from API
7. Full order details are displayed
8. User can click back button to return to profile

## Status Mapping

API Status → Display Text:
- `Pending` → "Chờ xác nhận"
- `Confirmed` → "Đã xác nhận"
- `Processing` → "Đang xử lý"
- `Shipping` → "Đang giao"
- `Delivered` → "Đã giao"
- `Cancelled` → "Đã hủy"
- `Refunded` → "Đã hoàn tiền"

## Error Handling

- Missing orderId: Shows error message "Không tìm thấy ID đơn hàng"
- API failure: Shows error message "Không thể tải chi tiết đơn hàng"
- No order found: Shows error message "Không tìm thấy đơn hàng"
- Missing data: Uses "N/A" as fallback

## Testing

1. Navigate to Profile page
2. Go to Orders tab
3. Wait for orders to load
4. Click "Xem chi tiết" on any order
5. Verify order details load correctly
6. Check all sections display properly:
   - Order header with status
   - Order items with images
   - Order summary with calculations
   - Shipping information
   - Payment information
7. Test back button navigation

## Notes

- Component uses `useParams` to extract orderId from URL
- Component uses `useNavigate` for back button navigation
- All prices formatted in Vietnamese format with ₫ symbol
- Loading spinner shown while fetching data
- Responsive design for mobile and desktop
- Consistent styling with rest of application (amber/red theme)

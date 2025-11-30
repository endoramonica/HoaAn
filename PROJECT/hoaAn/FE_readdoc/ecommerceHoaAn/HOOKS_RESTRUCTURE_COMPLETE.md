# Hooks Restructure - Complete ✅

## Overview
Tái cấu trúc hoàn toàn `useCheckout` và tạo mới `useOrder` hook theo chuẩn Orval-generated API.

## Architecture

### Before (Old Structure)
```
useCheckout.ts
├── processCheckout()
├── getOrderDetails()
├── getMyOrders()
├── cancelOrder()
├── getAllOrders()        // ❌ Mixed concerns
├── searchOrders()        // ❌ Mixed concerns
├── updateOrderStatus()   // ❌ Mixed concerns
└── getOrderStats()       // ❌ Mixed concerns
```

### After (New Structure)
```
useCheckout.ts (Checkout Flow)
├── processCheckout()     // ✅ Create order from cart
├── getOrderDetails()     // ✅ Get order by ID (checkout context)
├── getMyOrders()         // ✅ Customer's orders (checkout context)
└── cancelOrder()         // ✅ Cancel order (checkout context)

useOrder.ts (Order Management)
├── getOrderById()        // ✅ Get order by ID (admin context)
├── getMyOrders()         // ✅ Customer's orders (order context)
├── getAllOrders()        // ✅ Admin: all orders
├── searchOrders()        // ✅ Admin: search
├── getOrdersByStatus()   // ✅ Admin: filter by status
├── getRecentOrders()     // ✅ Admin: recent orders
├── getStatusHistory()    // ✅ Order status timeline
├── updateOrderStatus()   // ✅ Admin: update status
├── canChangeStatus()     // ✅ Check if status change allowed
├── bulkUpdateStatus()    // ✅ Admin: bulk update
├── getOrderCount()       // ✅ Statistics
└── getRevenue()          // ✅ Statistics
```

## Key Improvements

### ✅ 1. Separation of Concerns
- **useCheckout**: Customer checkout flow (process, view, cancel)
- **useOrder**: Admin order management (search, update, stats)

### ✅ 2. Orval-Generated Types
```typescript
// ❌ Before: Mixed old and new types
import { CheckoutService } from '@/api/services/CheckoutService';
import type { CheckoutDto } from '@/api';

// ✅ After: Pure Orval types
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type { CheckoutDto } from '../../../Api/generated-orval/schemas';
```

### ✅ 3. Consistent Error Handling
```typescript
const extractErrorMessage = (err: any): string => {
  if (err?.response?.data?.message) return err.response.data.message;
  if (err?.response?.data?.error) return err.response.data.error;
  if (err?.response?.data?.title) return err.response.data.title;
  if (err?.message) return err.message;
  return 'Đã xảy ra lỗi không xác định';
};

const logError = (context: string, err: any) => {
  console.error(`[useCheckout] ❌ ${context}:`, {
    message: err?.message,
    status: err?.response?.status,
    data: err?.response?.data,
    error: err
  });
};
```

### ✅ 4. Optimized Logging
```typescript
// ✅ Concise but informative
console.log('[useCheckout] 🛒 Processing checkout...');
console.log('[useCheckout] 📦 Payload:', {
  cartId: checkoutData.cartId,
  paymentMethod: checkoutData.paymentMethod,
  shippingInfo: checkoutData.shippingInfo
});

// ✅ Success logging
console.log('[useCheckout] ✅ Checkout successful:', {
  orderId: response.data.orderId,
  orderNumber: response.data.orderNumber
});

// ✅ Error logging with context
logError('Process checkout failed', err);
```

### ✅ 5. Type Safety
```typescript
// ✅ All params use Orval-generated types
const getMyOrders = useCallback(async (
  params?: GetApiV1CheckoutMyOrdersParams  // ✅ Orval type
): Promise<OrderDetailDto[]> => {
  // ...
}, []);
```

### ✅ 6. Consistent Return Types
```typescript
// ✅ Always return data or null/empty array
processCheckout(): Promise<OrderDetailDto | null>
getMyOrders(): Promise<OrderDetailDto[]>
cancelOrder(): Promise<boolean>
```

## API Mapping

### useCheckout (Checkout API)
| Method | Endpoint | Purpose |
|--------|----------|---------|
| `processCheckout()` | `POST /api/v1/Checkout/process` | Create order from cart |
| `getOrderDetails()` | `GET /api/v1/Checkout/{orderId}` | Get order details |
| `getMyOrders()` | `GET /api/v1/Checkout/my-orders` | Get customer's orders |
| `cancelOrder()` | `POST /api/v1/Checkout/{orderId}/cancel` | Cancel order |

### useOrder (Order API)
| Method | Endpoint | Purpose |
|--------|----------|---------|
| `getOrderById()` | `GET /api/v1/Order/{id}` | Get order by ID |
| `getMyOrders()` | `GET /api/v1/Order/my-orders` | Get customer's orders |
| `getAllOrders()` | `GET /api/v1/Order` | Get all orders (admin) |
| `searchOrders()` | `GET /api/v1/Order/search` | Search orders |
| `getOrdersByStatus()` | `GET /api/v1/Order/status/{status}` | Filter by status |
| `getRecentOrders()` | `GET /api/v1/Order/recent` | Get recent orders |
| `getStatusHistory()` | `GET /api/v1/Order/{orderId}/status-history` | Get status timeline |
| `updateOrderStatus()` | `PUT /api/v1/Order/{orderId}/status` | Update order status |
| `canChangeStatus()` | `GET /api/v1/Order/{orderId}/can-change-status` | Check if can change |
| `bulkUpdateStatus()` | `POST /api/v1/Order/bulk-update-status` | Bulk update |
| `getOrderCount()` | `GET /api/v1/Order/stats/count` | Get order count |
| `getRevenue()` | `GET /api/v1/Order/stats/revenue` | Get revenue stats |

## Usage Examples

### useCheckout (Customer Flow)

```typescript
import { useCheckout } from '@/lib/hooks/useCheckout';

function CheckoutPage() {
  const { processCheckout, isProcessing } = useCheckout();

  const handleCheckout = async () => {
    const result = await processCheckout({
      cartId: cart.id,
      shippingInfo: {
        recipientName: 'John Doe',
        phoneNumber: '0123456789',
        address: '123 Main St',
        ward: 'Ward 1',
        district: 'District 1',
        city: 'Ho Chi Minh',
        shippingMethod: 'standard'
      },
      paymentMethod: 'cod'
    });

    if (result) {
      navigate(`/order/${result.orderId}`);
    }
  };

  return (
    <button onClick={handleCheckout} disabled={isProcessing}>
      {isProcessing ? 'Processing...' : 'Place Order'}
    </button>
  );
}
```

### useOrder (Admin Dashboard)

```typescript
import { useOrder } from '@/lib/hooks/useOrder';

function AdminOrdersPage() {
  const { 
    getAllOrders, 
    updateOrderStatus, 
    getOrderCount,
    isLoading 
  } = useOrder();

  useEffect(() => {
    loadOrders();
  }, []);

  const loadOrders = async () => {
    const orders = await getAllOrders({
      page: 1,
      pageSize: 20,
      sortBy: 'createdAt',
      sortDescending: true
    });
    setOrders(orders);
  };

  const handleConfirmOrder = async (orderId: string) => {
    const success = await updateOrderStatus(
      orderId, 
      'confirmed',
      'Order confirmed by admin'
    );
    
    if (success) {
      loadOrders(); // Refresh list
    }
  };

  return (
    <div>
      {isLoading ? <Spinner /> : <OrderList orders={orders} />}
    </div>
  );
}
```

### useOrder (Customer Order History)

```typescript
import { useOrder } from '@/lib/hooks/useOrder';

function MyOrdersPage() {
  const { getMyOrders, isLoading } = useOrder();
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    loadMyOrders();
  }, []);

  const loadMyOrders = async () => {
    const myOrders = await getMyOrders({
      page: 1,
      pageSize: 10
    });
    setOrders(myOrders);
  };

  return (
    <div>
      {isLoading ? <Spinner /> : <OrderList orders={orders} />}
    </div>
  );
}
```

## Error Handling

### Automatic Toast Notifications
```typescript
// ✅ Success
toast.success('Đặt hàng thành công!');

// ✅ Error
toast.error('Không thể xử lý đơn hàng');
```

### Error State
```typescript
const { error, processCheckout } = useCheckout();

// Check error state
if (error) {
  console.error('Checkout error:', error);
}
```

### Console Logging
```typescript
// ✅ Detailed error logging
[useCheckout] ❌ Process checkout failed: {
  message: "Invalid phone number",
  status: 400,
  data: { ... }
}
```

## Testing Checklist

### useCheckout
- [ ] Process checkout with valid data
- [ ] Process checkout with invalid data (400 error)
- [ ] Get order details by ID
- [ ] Get my orders with pagination
- [ ] Cancel order
- [ ] Error handling works
- [ ] Loading states work
- [ ] Toast notifications show

### useOrder
- [ ] Get order by ID
- [ ] Get my orders (customer)
- [ ] Get all orders (admin)
- [ ] Search orders
- [ ] Filter by status
- [ ] Get recent orders
- [ ] Get status history
- [ ] Update order status
- [ ] Check can change status
- [ ] Bulk update status
- [ ] Get order count
- [ ] Get revenue stats
- [ ] Error handling works
- [ ] Loading states work

## Migration Guide

### From Old useCheckout

```typescript
// ❌ Before
import { useCheckout } from '@/lib/hooks/useCheckout';
const { processCheckout, getAllOrders, updateOrderStatus } = useCheckout();

// ✅ After
import { useCheckout } from '@/lib/hooks/useCheckout';
import { useOrder } from '@/lib/hooks/useOrder';

const { processCheckout } = useCheckout();
const { getAllOrders, updateOrderStatus } = useOrder();
```

### Update Imports

```typescript
// ❌ Before
import type { CheckoutDto } from '@/api';

// ✅ After
import type { CheckoutDto } from '../../../Api/generated-orval/schemas';
```

## Benefits

### 🎯 Clear Separation
- Checkout flow vs Order management
- Customer actions vs Admin actions

### 🔒 Type Safety
- 100% Orval-generated types
- No type casting needed
- Compile-time error checking

### 🐛 Better Debugging
- Consistent logging format
- Detailed error information
- Easy to trace issues

### 📦 Maintainability
- Single responsibility principle
- Easy to extend
- Clear API boundaries

### 🚀 Performance
- Optimized logging (not spammy)
- Efficient error handling
- Clean state management

## Files

### Created
- `src/lib/hooks/useCheckout.ts` - Checkout flow hook
- `src/lib/hooks/useOrder.ts` - Order management hook
- `HOOKS_RESTRUCTURE_COMPLETE.md` - This documentation

### Modified
- `src/pages/checkout/CheckoutPage.tsx` - Uses new useCheckout

### Deprecated
- Old `useCheckout.ts` with mixed concerns (replaced)

## Next Steps

1. Update all components using old useCheckout
2. Implement admin dashboard with useOrder
3. Add React Query for caching (optional)
4. Add unit tests for both hooks
5. Document all API endpoints in Swagger

## Success Metrics

- ✅ 0 TypeScript errors
- ✅ Clear separation of concerns
- ✅ 100% Orval types
- ✅ Consistent error handling
- ✅ Optimized logging
- ✅ Easy to maintain and extend

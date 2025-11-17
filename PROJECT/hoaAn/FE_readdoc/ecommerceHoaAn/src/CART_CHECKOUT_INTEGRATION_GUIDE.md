# Hướng dẫn tích hợp Cart & Checkout API

## Tổng quan

Hệ thống Cart & Checkout đã được tích hợp hoàn chỉnh với ASP.NET Core backend sử dụng kiến trúc 3 lớp:
- **API Client Layer**: `/lib/api/client.ts` - Axios instance với interceptors, JWT, auto refresh token
- **Service Layer**: `/lib/services/` - Business logic, API calls
- **Hooks Layer**: `/lib/hooks/` - React Query hooks cho state management

## 🛒 Cart Service

### Services được tạo

#### 1. `cartService.ts` - Quản lý giỏ hàng cho user đã đăng nhập
- ✅ `getCart()` - Lấy giỏ hàng
- ✅ `getSummary()` - Lấy tóm tắt giỏ hàng
- ✅ `addItem()` - Thêm sản phẩm
- ✅ `updateItem()` - Cập nhật số lượng
- ✅ `removeItem()` - Xóa item
- ✅ `clearCart()` - Xóa toàn bộ giỏ
- ✅ `validateCart()` - Validate giỏ hàng (tồn kho, giá...)
- ✅ `getItemCount()` - Lấy số lượng items
- ✅ `applyCoupon()` - Áp dụng mã giảm giá
- ✅ `removeCoupon()` - Xóa mã giảm giá
- ✅ `updateShipping()` - Cập nhật thông tin giao hàng
- ✅ `mergeCart()` - Merge guest cart sau khi login

#### 2. `guestCartService.ts` - Quản lý giỏ hàng cho guest (không cần login)
- ✅ Tương tự cartService nhưng dùng endpoints `/Cart/guest/*`
- ✅ Sử dụng sessionId trong localStorage

#### 3. `unifiedCartService.ts` - Tự động chọn user/guest cart
- ✅ Tự động phát hiện trạng thái authentication
- ✅ Route đến service phù hợp

### Hooks được tạo

File: `/lib/hooks/useCart.ts`

```tsx
import {
  useUnifiedCart,
  useCartItemCount,
  useAddToCart,
  useUpdateCartItem,
  useRemoveCartItem,
  useClearCart,
  useValidateCart,
  useApplyCoupon,
  useRemoveCoupon,
  useUpdateShipping,
  useMergeCart,
} from './lib/hooks/useCart';
```

### Ví dụ sử dụng

#### 1. Hiển thị giỏ hàng

```tsx
function CartPage() {
  const { data: cart, isLoading, error } = useUnifiedCart();
  const { mutate: removeItem } = useRemoveCartItem();
  const { mutate: updateItem } = useUpdateCartItem();

  if (isLoading) return <div>Đang tải...</div>;
  if (error) return <div>Lỗi: {error.message}</div>;

  return (
    <div>
      <h1>Giỏ hàng ({cart?.itemCount || 0} sản phẩm)</h1>
      {cart?.items.map(item => (
        <div key={item.id}>
          <h3>{item.productName}</h3>
          <p>Giá: {item.unitPrice.toLocaleString('vi-VN')}đ</p>
          <input
            type="number"
            value={item.quantity}
            onChange={(e) => updateItem({
              cartItemId: item.id,
              quantity: parseInt(e.target.value)
            })}
          />
          <button onClick={() => removeItem(item.id)}>Xóa</button>
        </div>
      ))}
      <p>Tổng: {cart?.totalAmount.toLocaleString('vi-VN')}đ</p>
    </div>
  );
}
```

#### 2. Thêm vào giỏ hàng

```tsx
function ProductCard({ product }) {
  const { mutate: addToCart, isPending } = useAddToCart();

  const handleAddToCart = () => {
    addToCart({
      productId: product.id,
      quantity: 1
    });
  };

  return (
    <button onClick={handleAddToCart} disabled={isPending}>
      {isPending ? 'Đang thêm...' : 'Thêm vào giỏ'}
    </button>
  );
}
```

#### 3. Hiển thị số lượng items trong header

```tsx
function CartIcon() {
  const { data: itemCount = 0 } = useCartItemCount();

  return (
    <div className="relative">
      <ShoppingCart />
      {itemCount > 0 && (
        <span className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full px-2">
          {itemCount}
        </span>
      )}
    </div>
  );
}
```

#### 4. Áp dụng mã giảm giá

```tsx
function CouponSection() {
  const [code, setCode] = useState('');
  const { mutate: applyCoupon, isPending } = useApplyCoupon();
  const { mutate: removeCoupon } = useRemoveCoupon();
  const { data: cart } = useUnifiedCart();

  return (
    <div>
      {cart?.couponCode ? (
        <div>
          <span>Mã: {cart.couponCode}</span>
          <button onClick={() => removeCoupon()}>Xóa</button>
        </div>
      ) : (
        <div>
          <input
            value={code}
            onChange={(e) => setCode(e.target.value)}
            placeholder="Nhập mã giảm giá"
          />
          <button
            onClick={() => applyCoupon({ couponCode: code })}
            disabled={isPending}
          >
            Áp dụng
          </button>
        </div>
      )}
    </div>
  );
}
```

## 💳 Checkout Service

### Services được tạo

File: `/lib/services/checkoutService.ts`

#### 1. `checkoutService` - Xử lý checkout
- ✅ `processCheckout()` - Tạo đơn hàng từ giỏ hàng
- ✅ `getOrderDetails()` - Lấy chi tiết đơn hàng
- ✅ `getMyOrders()` - Lấy danh sách đơn hàng (không phân trang)
- ✅ `cancelOrder()` - Hủy đơn hàng

#### 2. `orderService` - Quản lý đơn hàng nâng cao
- ✅ `getOrderById()` - Chi tiết đơn hàng
- ✅ `getMyOrders()` - Đơn hàng của user (có phân trang)
- ✅ `getAllOrders()` - Tất cả đơn hàng (Admin)
- ✅ `searchOrders()` - Tìm kiếm
- ✅ `getOrdersByStatus()` - Lọc theo trạng thái
- ✅ `getRecentOrders()` - Đơn hàng gần đây
- ✅ `getOrderStatusHistory()` - Lịch sử trạng thái
- ✅ `updateOrderStatus()` - Cập nhật trạng thái (Admin)
- ✅ `canChangeStatus()` - Kiểm tra có thể thay đổi trạng thái
- ✅ `getOrderCount()` - Thống kê số lượng đơn
- ✅ `getRevenue()` - Thống kê doanh thu
- ✅ `bulkUpdateStatus()` - Cập nhật nhiều đơn

### Hooks được tạo

File: `/lib/hooks/useCheckout.ts`

```tsx
import {
  useProcessCheckout,
  useOrderDetails,
  useMyOrders,
  useMyOrdersList,
  useCancelOrder,
  useOrderStatusHistory,
  useOrderStats,
  useUpdateOrderStatus,
  useBulkUpdateOrderStatus,
} from './lib/hooks/useCheckout';
```

### Ví dụ sử dụng

#### 1. Xử lý checkout

```tsx
function CheckoutPage() {
  const { data: cart } = useUnifiedCart();
  const { mutate: processCheckout, isPending } = useProcessCheckout();
  const [shippingInfo, setShippingInfo] = useState({
    recipientName: '',
    phoneNumber: '',
    address: '',
    ward: '',
    district: '',
    city: '',
    shippingMethod: 'standard',
  });

  const handleCheckout = () => {
    if (!cart) return;

    processCheckout({
      cartId: cart.id,
      shippingInfo: shippingInfo,
      notes: 'Giao giờ hành chính',
    }, {
      onSuccess: (data) => {
        // Redirect to order success page
        window.location.href = `/order/${data.orderId}`;
      }
    });
  };

  return (
    <div>
      <h1>Thanh toán</h1>
      {/* Form nhập thông tin giao hàng */}
      <input
        placeholder="Họ tên"
        value={shippingInfo.recipientName}
        onChange={(e) => setShippingInfo({
          ...shippingInfo,
          recipientName: e.target.value
        })}
      />
      {/* ... các field khác */}
      
      <button onClick={handleCheckout} disabled={isPending}>
        {isPending ? 'Đang xử lý...' : 'Đặt hàng'}
      </button>
    </div>
  );
}
```

#### 2. Hiển thị danh sách đơn hàng

```tsx
function MyOrdersPage() {
  const [status, setStatus] = useState<OrderStatus | undefined>();
  const { data, isLoading } = useMyOrders({
    Page: 1,
    PageSize: 10,
    Status: status,
  });

  return (
    <div>
      <h1>Đơn hàng của tôi</h1>
      
      <select onChange={(e) => setStatus(e.target.value as OrderStatus)}>
        <option value="">Tất cả</option>
        <option value="pending">Chờ xác nhận</option>
        <option value="confirmed">Đã xác nhận</option>
        <option value="shipped">Đang giao</option>
        <option value="completed">Hoàn thành</option>
        <option value="cancelled">Đã hủy</option>
      </select>

      {isLoading ? (
        <div>Đang tải...</div>
      ) : (
        <div>
          {data?.items.map(order => (
            <div key={order.orderId}>
              <h3>Đơn hàng #{order.orderNumber}</h3>
              <p>Trạng thái: {order.statusText}</p>
              <p>Tổng: {order.totalAmount.toLocaleString('vi-VN')}đ</p>
              <p>Ngày đặt: {new Date(order.createdAt).toLocaleDateString('vi-VN')}</p>
            </div>
          ))}
          
          <Pagination
            currentPage={data?.pageNumber || 1}
            totalPages={data?.totalPages || 1}
          />
        </div>
      )}
    </div>
  );
}
```

#### 3. Chi tiết đơn hàng

```tsx
function OrderDetailsPage({ orderId }: { orderId: string }) {
  const { data: order, isLoading } = useOrderDetails(orderId);
  const { data: statusHistory } = useOrderStatusHistory(orderId);
  const { mutate: cancelOrder } = useCancelOrder();

  const handleCancel = () => {
    if (confirm('Bạn có chắc muốn hủy đơn hàng?')) {
      cancelOrder({
        orderId,
        request: { reason: 'Đổi ý không mua nữa' }
      });
    }
  };

  if (isLoading) return <div>Đang tải...</div>;
  if (!order) return <div>Không tìm thấy đơn hàng</div>;

  return (
    <div>
      <h1>Đơn hàng #{order.orderNumber}</h1>
      
      <div>
        <h2>Thông tin giao hàng</h2>
        <p>{order.shipping?.recipientName}</p>
        <p>{order.shipping?.phoneNumber}</p>
        <p>{order.shipping?.address}</p>
      </div>

      <div>
        <h2>Sản phẩm</h2>
        {order.items?.map(item => (
          <div key={item.id}>
            <p>{item.productName} x {item.quantity}</p>
            <p>{item.totalPrice.toLocaleString('vi-VN')}đ</p>
          </div>
        ))}
      </div>

      <div>
        <h2>Lịch sử trạng thái</h2>
        {statusHistory?.map((history, idx) => (
          <div key={idx}>
            <p>{history.statusText}</p>
            <p>{new Date(history.changedAt).toLocaleString('vi-VN')}</p>
            {history.notes && <p>{history.notes}</p>}
          </div>
        ))}
      </div>

      {order.status === 'pending' && (
        <button onClick={handleCancel}>Hủy đơn hàng</button>
      )}
    </div>
  );
}
```

#### 4. Thống kê đơn hàng (Admin)

```tsx
function OrderStatsWidget() {
  const { data: stats } = useOrderStats();

  return (
    <div className="grid grid-cols-2 gap-4">
      <div className="p-4 bg-white rounded-lg shadow">
        <h3>Tổng đơn hàng</h3>
        <p className="text-3xl">{stats?.count || 0}</p>
      </div>
      <div className="p-4 bg-white rounded-lg shadow">
        <h3>Doanh thu</h3>
        <p className="text-3xl">
          {(stats?.revenue || 0).toLocaleString('vi-VN')}đ
        </p>
      </div>
    </div>
  );
}
```

## 👤 User Service

### Services & Hooks

File: `/lib/services/userService.ts` & `/lib/hooks/useUser.ts`

```tsx
import {
  useUsers,
  useUser,
  useUpdateUser,
  useActivateUser,
  useDeactivateUser,
} from './lib/hooks/useUser';
```

### Ví dụ sử dụng

```tsx
function UserManagementPage() {
  const [page, setPage] = useState(1);
  const { data: users } = useUsers({ pageNumber: page, pageSize: 10 });
  const { mutate: deactivateUser } = useDeactivateUser();

  return (
    <div>
      <h1>Quản lý người dùng</h1>
      {users?.items.map(user => (
        <div key={user.id}>
          <p>{user.fullName} ({user.email})</p>
          <button onClick={() => deactivateUser(user.id)}>
            Vô hiệu hóa
          </button>
        </div>
      ))}
    </div>
  );
}
```

## 🔧 Cấu hình

### Environment Variables

Tạo file `.env` hoặc `.env.local`:

```env
VITE_API_URL=http://localhost:5000/api/v1
```

### React Query Configuration

Đã được cấu hình trong `App.tsx`:

```tsx
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
      staleTime: 30000, // 30 seconds
    },
    mutations: {
      retry: 0,
    },
  },
});
```

## 🎯 Tính năng nổi bật

### 1. Automatic Token Management
- ✅ Tự động attach JWT token vào mọi request
- ✅ Auto refresh token khi hết hạn
- ✅ Auto redirect đến login khi unauthorized

### 2. Optimistic Updates
- ✅ UI cập nhật ngay lập tức
- ✅ Rollback nếu có lỗi
- ✅ UX mượt mà, không lag

### 3. Smart Caching
- ✅ React Query cache tự động
- ✅ Invalidate cache khi cần
- ✅ Giảm số lần gọi API

### 4. Error Handling
- ✅ Toast notification tự động
- ✅ Error messages rõ ràng
- ✅ Tương thích ASP.NET Core ProblemDetails

### 5. Guest Cart Support
- ✅ Giỏ hàng cho khách chưa đăng nhập
- ✅ Tự động merge khi login
- ✅ SessionId tracking

## 📝 Notes

### Khi nào dùng cartService vs unifiedCartService?

- **`unifiedCartService`**: Dùng khi bạn muốn hệ thống tự động detect user/guest
- **`cartService`**: Dùng khi bạn chắc chắn user đã đăng nhập
- **`guestCartService`**: Dùng khi bạn muốn force guest cart

### Order Status Flow

```
pending → confirmed → shipped → completed
   ↓
cancelled
```

### Shipping Methods

- `standard`: Giao hàng tiêu chuẩn (3-5 ngày)
- `express`: Giao hàng nhanh (1-2 ngày)
- `sameDay`: Giao trong ngày
- `overnight`: Giao qua đêm

## 🚀 Next Steps

Bạn có thể:

1. ✅ Cập nhật `CartPage.tsx` để sử dụng hooks mới
2. ✅ Tạo `CheckoutPage.tsx` với form thanh toán
3. ✅ Cập nhật `Header.tsx` để hiển thị cart count
4. ✅ Tạo `OrdersPage.tsx` để hiển thị lịch sử đơn hàng
5. ✅ Tạo `OrderDetailPage.tsx` để xem chi tiết đơn

## 📚 Tài liệu tham khảo

- [React Query Documentation](https://tanstack.com/query/latest)
- [Axios Documentation](https://axios-http.com/)
- [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)

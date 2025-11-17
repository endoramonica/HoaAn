# 🗺️ Routing Architecture - URL-Based Navigation with React Router

## 📌 Tổng Quan

Kiến trúc routing mới của VietCommerce sử dụng **react-router-dom v6** với **nested routes** và **layouts** để tối ưu hóa cấu trúc code và user experience.

## 🎯 Lý do Chuyển Đổi

### Trước đây (State-based):
- ❌ URL không thay đổi khi navigate
- ❌ Không support browser back/forward
- ❌ Khó share deep links
- ❌ State management phức tạp
- ❌ Khó debug và test

### Hiện tại (URL-based):
- ✅ URL phản ánh đúng page hiện tại
- ✅ Browser navigation hoạt động tự nhiên
- ✅ Deep linking và bookmarking
- ✅ State management đơn giản hơn
- ✅ SEO-friendly (quan trọng cho future SSR)

## 🏗️ Cấu Trúc Routes

```
/
├── / (MainLayout - Header + Footer)
│   ├── /                      → HomePage
│   ├── /products              → ProductsPage  
│   ├── /services              → ServicesPage
│   ├── /about                 → AboutPage
│   ├── /contact               → ContactPage
│   ├── /cart                  → CartPage
│   ├── /wishlist              → WishlistPage
│   ├── /profile               → ProfilePage
│   ├── /calendar              → CalendarPage
│   ├── /community             → CommunityPage
│   └── /qa                    → QAPage
│
├── /auth (AuthLayout - No Header/Footer)
│   ├── /login                 → LoginPage
│   ├── /register              → RegisterPage
│   └── /forgot-password       → ForgotPasswordPage
│
├── /delivery (DeliveryLayout - Nested Routes)
│   ├── /                      → DeliveryHomePage
│   ├── /booking               → DeliveryBookingPage
│   ├── /price-estimation      → PriceEstimationPage
│   ├── /tracking              → OrderTrackingPage
│   ├── /order/:orderId        → OrderDetailsPage
│   └── /rating/:orderId       → DriverRatingPage
│
├── /spiritual (SpiritualLayout - Nested Routes + AI Chat)
│   ├── /                      → SpiritualHomePage
│   ├── /virtual-incense       → VirtualIncensePage
│   ├── /audio-chanting        → AudioChantingPage
│   └── /fengshui-consultation → FengShuiConsultationPage
│
└── /checkout                  → CheckoutFlow (Internal state navigation)
```

## 📦 Layout Components

### 1. MainLayout (`/layouts/MainLayout.tsx`)

Wrapper chính cho tất cả trang thông thường với Header + Footer.

```tsx
<MainLayout>
  <Header />
  <main><Outlet /></main>
  <Footer />
  <AdPopup />
  <PrayerFeed />
  <SupportChatModal />
</MainLayout>
```

**Trang sử dụng:**
- HomePage, ProductsPage, ServicesPage
- AboutPage, ContactPage
- CartPage, WishlistPage, ProfilePage
- CalendarPage, CommunityPage, QAPage

---

### 2. AuthLayout (`/layouts/AuthLayout.tsx`)

Layout clean cho authentication pages (không có header/footer).

```tsx
<AuthLayout>
  <Outlet />
</AuthLayout>
```

**Trang sử dụng:**
- LoginPage
- RegisterPage
- ForgotPasswordPage

---

### 3. DeliveryLayout (`/layouts/DeliveryLayout.tsx`)

Layout cho VietDelivery service với welcome toast.

```tsx
<DeliveryLayout>
  {/* Welcome toast on first visit */}
  <Outlet />
</DeliveryLayout>
```

**Trang sử dụng:**
- DeliveryHomePage (index)
- DeliveryBookingPage
- PriceEstimationPage
- OrderTrackingPage
- OrderDetailsPage (with :orderId param)
- DriverRatingPage (with :orderId param)

---

### 4. SpiritualLayout (`/layouts/SpiritualLayout.tsx`)

Layout cho Spiritual Service với AI Chatbox.

```tsx
<SpiritualLayout>
  <Outlet />
  <SpiritualAIChatBox />
</SpiritualLayout>
```

**Trang sử dụng:**
- SpiritualHomePage (index)
- VirtualIncensePage
- AudioChantingPage
- FengShuiConsultationPage

## 🧩 Context & State Management

### AppContext (`/lib/contexts/AppContext.tsx`)

Global state được quản lý tập trung qua React Context:

```tsx
interface AppContextType {
  // UI State
  showSpiritualChat: boolean;
  setShowSpiritualChat: (show: boolean) => void;
  showSupportChat: boolean;
  setShowSupportChat: (show: boolean) => void;
  
  // Prayer State
  prayers: Prayer[];
  addPrayer: (prayer: Prayer) => void;
}
```

**Sử dụng:**
```tsx
const { showSupportChat, setShowSupportChat, prayers, addPrayer } = useApp();
```

## 🚀 Navigation Patterns

### 1. Basic Navigation

```tsx
import { useNavigate } from 'react-router-dom';

const navigate = useNavigate();

// Navigate to page
navigate('/products');
navigate('/delivery/booking');
navigate('/auth/login');

// Navigate with params
navigate(`/delivery/order/${orderId}`);
navigate(`/delivery/rating/${orderId}`);

// Navigate with state
navigate('/cart', { state: { from: 'products' } });

// Navigate and replace history
navigate('/home', { replace: true });
```

### 2. Link Component

```tsx
import { Link } from 'react-router-dom';

<Link to="/products">Sản phẩm</Link>
<Link to="/delivery">Giao hàng</Link>
<Link to={`/delivery/order/${orderId}`}>Chi tiết</Link>
```

### 3. Get Current Location

```tsx
import { useLocation } from 'react-router-dom';

const location = useLocation();
const currentPath = location.pathname; // "/products"
const currentPage = location.pathname.split('/')[1]; // "products"
```

### 4. URL Parameters

```tsx
import { useParams } from 'react-router-dom';

// Route: /delivery/order/:orderId
const { orderId } = useParams(); // "12345"
```

### 5. Query Parameters

```tsx
import { useSearchParams } from 'react-router-dom';

const [searchParams, setSearchParams] = useSearchParams();

// Read: /products?category=huong&sort=price
const category = searchParams.get('category'); // "huong"
const sort = searchParams.get('sort'); // "price"

// Set: /products → /products?category=nen
setSearchParams({ category: 'nen' });
```

## 🔄 Migration Examples

### Before: State-based Navigation

```tsx
// Old way
<Button onClick={() => onNavigate('products')}>
  Sản phẩm
</Button>
```

### After: URL-based Navigation

```tsx
// New way
import { useNavigate } from 'react-router-dom';

const navigate = useNavigate();

<Button onClick={() => navigate('/products')}>
  Sản phẩm
</Button>
```

---

### Before: Props Drilling

```tsx
// Old way - Props drilling
function App() {
  const [currentPage, setCurrentPage] = useState('home');
  return <Header onNavigate={setCurrentPage} />;
}

function Header({ onNavigate }) {
  return <button onClick={() => onNavigate('products')}>Products</button>;
}
```

### After: Direct Navigation

```tsx
// New way - Direct navigation
function Header() {
  const navigate = useNavigate();
  return <button onClick={() => navigate('/products')}>Products</button>;
}
```

---

### Before: Manual Page Rendering

```tsx
// Old way
function App() {
  const [page, setPage] = useState('home');
  
  const renderPage = () => {
    switch (page) {
      case 'home': return <HomePage />;
      case 'products': return <ProductsPage />;
      // ... 20+ cases
    }
  };
  
  return renderPage();
}
```

### After: React Router

```tsx
// New way
<Routes>
  <Route element={<MainLayout />}>
    <Route path="/" element={<HomePage />} />
    <Route path="/products" element={<ProductsPage />} />
  </Route>
</Routes>
```

## 📚 Best Practices

### ✅ DO

```tsx
// 1. Use absolute paths
navigate('/products');
navigate('/delivery/booking');

// 2. Use Link for static links
<Link to="/about">Về chúng tôi</Link>

// 3. Use useNavigate for dynamic navigation
const handleSuccess = () => {
  navigate('/checkout/success');
};

// 4. Use layouts for shared components
<Route element={<MainLayout />}>
  <Route path="/products" element={<ProductsPage />} />
</Route>

// 5. Use context for global state
const { showSupportChat } = useApp();
```

### ❌ DON'T

```tsx
// 1. Don't use relative paths without context
navigate('products'); // ❌ Could navigate to wrong place

// 2. Don't pass navigation callbacks as props
<Component onNavigate={handleNavigate} /> // ❌ Props drilling

// 3. Don't use state for routing
const [currentPage, setCurrentPage] = useState(); // ❌ Use URL instead

// 4. Don't duplicate layout components
// ❌ Duplicate header in every page
<ProductsPage>
  <Header />
  {/* content */}
</ProductsPage>

// 5. Don't store navigation state in useState
const [page, setPage] = useState('home'); // ❌ Use react-router
```

## 🎨 Advanced Patterns

### Protected Routes

```tsx
function ProtectedRoute({ children }: { children: JSX.Element }) {
  const { isAuthenticated } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/auth/login" state={{ from: location }} replace />;
  }

  return children;
}

// Usage
<Route
  path="/profile"
  element={
    <ProtectedRoute>
      <ProfilePage />
    </ProtectedRoute>
  }
/>
```

### Redirect After Login

```tsx
// LoginPage.tsx
const location = useLocation();
const from = location.state?.from?.pathname || '/';

const handleLogin = async () => {
  await login();
  navigate(from, { replace: true });
};
```

### 404 Not Found

```tsx
<Routes>
  {/* All routes */}
  <Route path="*" element={<Navigate to="/" replace />} />
</Routes>
```

## 🧪 Testing

```tsx
import { BrowserRouter } from 'react-router-dom';
import { render, screen } from '@testing-library/react';

test('navigates to products page', () => {
  render(
    <BrowserRouter>
      <App />
    </BrowserRouter>
  );
  
  const link = screen.getByText('Sản phẩm');
  fireEvent.click(link);
  
  expect(window.location.pathname).toBe('/products');
});
```

## 🔍 Debugging

### DevTools

```tsx
// Hiển thị current route
const location = useLocation();
console.log('Current path:', location.pathname);

// Hiển thị params
const params = useParams();
console.log('Route params:', params);

// Hiển thị search params
const [searchParams] = useSearchParams();
console.log('Query params:', Object.fromEntries(searchParams));
```

### React DevTools

- Xem Router state trong Components tree
- Check context values
- Inspect route matches

## 🚨 Common Issues & Solutions

### Issue 1: Navigate không hoạt động

```tsx
// ❌ Problem: useNavigate outside Router context
function Component() {
  const navigate = useNavigate(); // Error!
}

// ✅ Solution: Wrap with BrowserRouter
<BrowserRouter>
  <Component />
</BrowserRouter>
```

### Issue 2: Layout hiển thị 2 lần

```tsx
// ❌ Problem: Duplicate layout
<MainLayout>
  <Routes>
    <Route path="/" element={<MainLayout><HomePage /></MainLayout>} />
  </Routes>
</MainLayout>

// ✅ Solution: Use Outlet
<Routes>
  <Route element={<MainLayout />}>
    <Route path="/" element={<HomePage />} />
  </Route>
</Routes>
```

### Issue 3: Nested route không render

```tsx
// ❌ Problem: Missing Outlet
function DeliveryLayout() {
  return <div>Delivery</div>; // No Outlet!
}

// ✅ Solution: Add Outlet
function DeliveryLayout() {
  return (
    <div>
      <Outlet /> {/* Renders child routes */}
    </div>
  );
}
```

## 📊 Performance Optimization

### Code Splitting

```tsx
import { lazy, Suspense } from 'react';

const ProductsPage = lazy(() => import('./components/ProductsPage'));

<Route
  path="/products"
  element={
    <Suspense fallback={<Loading />}>
      <ProductsPage />
    </Suspense>
  }
/>
```

### Prefetching

```tsx
// Prefetch route on hover
<Link 
  to="/products"
  onMouseEnter={() => {
    // Prefetch data
    queryClient.prefetchQuery(['products']);
  }}
>
  Sản phẩm
</Link>
```

## 🔗 Related Documentation

- [React Router v6 Documentation](https://reactrouter.com/)
- [CHECKOUT_IMPLEMENTATION_GUIDE.md](./CHECKOUT_IMPLEMENTATION_GUIDE.md)
- [API_INTEGRATION_GUIDE.md](./API_INTEGRATION_GUIDE.md)
- [AUTH_INTEGRATION_GUIDE.md](./AUTH_INTEGRATION_GUIDE.md)

---

**Version**: 2.0  
**Last Updated**: 2025-01-08  
**Status**: ✅ Production Ready
**Migration**: ✅ Completed from state-based to URL-based routing

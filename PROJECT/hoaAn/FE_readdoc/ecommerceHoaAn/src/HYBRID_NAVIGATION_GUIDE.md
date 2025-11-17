# 🧭 Hybrid Navigation Pattern - useHybridNavigate

## 📌 Tổng Quan

`useHybridNavigate` là custom hook thống nhất navigation giữa **useState-based** và **React Router**, cho phép components hoạt động linh hoạt trong cả hai môi trường.

## 🎯 Vấn Đề Giải Quyết

### Trước khi có useHybridNavigate:

```tsx
// Component A - Dùng onNavigate callback
const ComponentA = ({ onNavigate }: { onNavigate: (page: string) => void }) => {
  return <button onClick={() => onNavigate('home')}>Home</button>;
};

// Component B - Dùng React Router
const ComponentB = () => {
  const navigate = useNavigate();
  return <button onClick={() => navigate('/home')}>Home</button>;
};

// ❌ Problem: Hai cách khác nhau, khó maintain và không thể tái sử dụng
```

### Sau khi có useHybridNavigate:

```tsx
// Component - Hoạt động ở mọi nơi
const UniversalComponent = ({ onNavigate }: { onNavigate?: (page: string) => void }) => {
  const navigate = useHybridNavigate(onNavigate);
  return <button onClick={() => navigate('home')}>Home</button>;
};

// ✅ Solution: Một cách duy nhất, tự động adapt
```

## 📝 Implementation

### Hook Definition

```typescript
// lib/hooks/useHybridNavigate.ts
import { useCallback } from "react";
import { useNavigate } from "react-router-dom";

export function useHybridNavigate(onNavigate?: (page: string) => void) {
  const routerNavigate = useNavigate();

  const navigateTo = useCallback(
    (page: string) => {
      if (onNavigate) {
        // Nếu có onNavigate callback từ parent → dùng nó
        onNavigate(page);
      } else {
        // Không có → fallback sang React Router
        routerNavigate(page.startsWith("/") ? page : `/${page}`);
      }
    },
    [onNavigate, routerNavigate]
  );

  return navigateTo;
}
```

## 🚀 Cách Sử Dụng

### 1. Component Nhận onNavigate Prop

```tsx
import { useHybridNavigate } from '../lib/hooks/useHybridNavigate';

interface MyComponentProps {
  onNavigate?: (page: string) => void; // Optional!
}

export const MyComponent = ({ onNavigate }: MyComponentProps) => {
  const navigate = useHybridNavigate(onNavigate);
  
  return (
    <div>
      <button onClick={() => navigate('home')}>Home</button>
      <button onClick={() => navigate('products')}>Products</button>
      <button onClick={() => navigate('cart')}>Cart</button>
    </div>
  );
};
```

### 2. Component Không Nhận onNavigate

```tsx
import { useHybridNavigate } from '../lib/hooks/useHybridNavigate';

export const MyComponent = () => {
  const navigate = useHybridNavigate(); // Không truyền gì
  
  return (
    <button onClick={() => navigate('checkout')}>
      Checkout
    </button>
  );
};
```

### 3. Trong Context của useState Navigation

```tsx
// App.tsx - useState navigation
const [currentPage, setCurrentPage] = useState('home');

<MyComponent onNavigate={setCurrentPage} />

// MyComponent sẽ dùng setCurrentPage để navigate
```

### 4. Trong Context của React Router

```tsx
// App.tsx - React Router
<BrowserRouter>
  <Routes>
    <Route path="/" element={<MyComponent />} />
  </Routes>
</BrowserRouter>

// MyComponent sẽ tự động dùng React Router để navigate
```

## ✨ Tính Năng

### 1. Auto Path Normalization

```tsx
navigate('home')    // → Tự động thêm '/' nếu cần: '/home'
navigate('/home')   // → Giữ nguyên: '/home'
navigate('cart')    // → '/cart'
```

### 2. Backward Compatible

```tsx
// Component cũ vẫn hoạt động bình thường
<LoginPage onNavigate={handleNavigate} />

// Component mới không cần onNavigate
<CheckoutPage />
```

### 3. TypeScript Support

```tsx
interface Props {
  onNavigate?: (page: string) => void; // Always optional
}
```

## 📦 Components Đã Áp Dụng

### Checkout Flow
- ✅ `CheckoutPage`
- ✅ `OrderSuccessPage`
- ✅ `OrderFailedPage`
- ✅ `CartPage`

### Authentication
- ✅ `LoginPage`
- ✅ `RegisterPage`
- ✅ `GoogleLoginButton`

## 🎨 Best Practices

### ✅ DO

```tsx
// 1. Always make onNavigate optional
interface Props {
  onNavigate?: (page: string) => void;
}

// 2. Use without leading slash
navigate('home');
navigate('products');
navigate('cart');

// 3. Pass onNavigate prop when using hybrid hook
const navigate = useHybridNavigate(onNavigate);
```

### ❌ DON'T

```tsx
// 1. Don't require onNavigate
interface Props {
  onNavigate: (page: string) => void; // ❌ Not flexible
}

// 2. Don't mix navigation methods
const navigate = useNavigate(); // ❌
navigate('home'); // ❌ Use hybrid instead

// 3. Don't call both onNavigate and navigate
if (onNavigate) {
  onNavigate('home'); // ❌
} else {
  navigate('home'); // ❌
}
// Use: navigate('home'); // ✅ Hybrid handles it
```

## 🔄 Migration Guide

### From useState Navigation

**Before:**
```tsx
const MyComponent = ({ onNavigate }: { onNavigate: (page: string) => void }) => {
  return <button onClick={() => onNavigate('home')}>Home</button>;
};
```

**After:**
```tsx
const MyComponent = ({ onNavigate }: { onNavigate?: (page: string) => void }) => {
  const navigate = useHybridNavigate(onNavigate);
  return <button onClick={() => navigate('home')}>Home</button>;
};
```

### From React Router

**Before:**
```tsx
import { useNavigate } from 'react-router-dom';

const MyComponent = () => {
  const navigate = useNavigate();
  return <button onClick={() => navigate('/home')}>Home</button>;
};
```

**After:**
```tsx
import { useHybridNavigate } from '../lib/hooks/useHybridNavigate';

const MyComponent = () => {
  const navigate = useHybridNavigate();
  return <button onClick={() => navigate('home')}>Home</button>;
};
```

## 🧪 Testing

```tsx
import { render, fireEvent } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { MyComponent } from './MyComponent';

// Test 1: With onNavigate callback
test('navigates using callback', () => {
  const mockNavigate = jest.fn();
  const { getByText } = render(
    <MyComponent onNavigate={mockNavigate} />
  );
  
  fireEvent.click(getByText('Home'));
  expect(mockNavigate).toHaveBeenCalledWith('home');
});

// Test 2: With React Router
test('navigates using router', () => {
  const { getByText } = render(
    <BrowserRouter>
      <MyComponent />
    </BrowserRouter>
  );
  
  fireEvent.click(getByText('Home'));
  // Router navigation happens
});
```

## 🎯 Use Cases

### 1. Gradual Migration
Migrate từ useState sang React Router từng bước mà không phá vỡ code hiện tại.

### 2. Reusable Components
Tạo components có thể dùng ở mọi nơi trong app.

### 3. Library Components
Tạo component library không bị lock-in vào một navigation method.

### 4. Legacy Support
Hỗ trợ cả code cũ và code mới song song.

## 📊 Architecture Diagram

```
┌─────────────────────────────────────────┐
│         useHybridNavigate Hook          │
│                                         │
│  ┌───────────────────────────────────┐  │
│  │  onNavigate prop có giá trị?     │  │
│  └───────────────┬───────────────────┘  │
│                  │                      │
│         ┌────────┴────────┐             │
│         │                 │             │
│      ✅ YES            ❌ NO            │
│         │                 │             │
│         ▼                 ▼             │
│  ┌──────────────┐  ┌──────────────┐    │
│  │  onNavigate  │  │ useNavigate  │    │
│  │  (useState)  │  │ (Router)     │    │
│  └──────────────┘  └──────────────┘    │
│         │                 │             │
│         └────────┬────────┘             │
│                  ▼                      │
│            navigateTo()                 │
└─────────────────────────────────────────┘
```

## 🔗 Related Documentation

- [CHECKOUT_IMPLEMENTATION_GUIDE.md](./CHECKOUT_IMPLEMENTATION_GUIDE.md)
- [React Router Documentation](https://reactrouter.com/)

---

**Version**: 1.0  
**Last Updated**: 2025-01-03  
**Status**: ✅ Production Ready

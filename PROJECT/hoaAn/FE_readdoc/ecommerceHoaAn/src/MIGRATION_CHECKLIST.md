# 🔄 Migration Checklist - State-based to URL-based Navigation

## 📊 Progress Overview

**Status**: 🚧 In Progress  
**Completed**: 8/35 components  
**Progress**: 23%

---

## ✅ Completed

### Core Architecture
- [x] `/App.tsx` - Refactored with react-router-dom routes
- [x] `/lib/contexts/AppContext.tsx` - Created global state context
- [x] `/layouts/MainLayout.tsx` - Created main layout wrapper
- [x] `/layouts/AuthLayout.tsx` - Created auth layout
- [x] `/layouts/DeliveryLayout.tsx` - Created delivery layout
- [x] `/layouts/SpiritualLayout.tsx` - Created spiritual layout

### Components
- [x] `/components/Header.tsx` - Removed onNavigate prop, using useNavigate + useApp
- [x] `/components/AdPopup.tsx` - Removed onNavigate prop, using useNavigate
- [x] `/components/HomePage.tsx` - Removed onNavigate prop, using useNavigate
- [x] `/components/delivery/DeliveryHomePage.tsx` - Removed props, using useNavigate

---

## 🚧 In Progress

### Delivery Components (4/6)
- [x] `DeliveryHomePage.tsx` - ✅ Updated
- [ ] `DeliveryBookingPage.tsx` - ⏳ Remove onBack, onNavigate props
- [ ] `PriceEstimationPage.tsx` - ⏳ Remove onBack, onNavigate props
- [ ] `OrderTrackingPage.tsx` - ⏳ Remove onBack, onNavigate props
- [ ] `OrderDetailsPage.tsx` - ⏳ Remove onBack, onNavigate props, add useParams
- [ ] `DriverRatingPage.tsx` - ⏳ Remove onBack, onComplete props, add useParams

### Spiritual Components (0/4)
- [ ] `SpiritualHomePage.tsx` - ⏳ Remove all navigation props, use useApp for prayers
- [ ] `VirtualIncensePage.tsx` - ⏳ Remove onBack prop
- [ ] `AudioChantingPage.tsx` - ⏳ Remove onBack prop
- [ ] `FengShuiConsultationPage.tsx` - ⏳ Remove onBack prop

### Main Pages (1/10)
- [x] `HomePage.tsx` - ✅ Updated
- [ ] `ProductsPage.tsx` - ⏳ Check if needs updates
- [ ] `ServicesPage.tsx` - ⏳ Check if needs updates
- [ ] `AboutPage.tsx` - ⏳ Check if needs updates
- [ ] `ContactPage.tsx` - ⏳ Check if needs updates
- [ ] `CartPage.tsx` - ⏳ Remove onNavigate if exists
- [ ] `WishlistPage.tsx` - ⏳ Remove onNavigate if exists
- [ ] `ProfilePage.tsx` - ⏳ Check if needs updates
- [ ] `CalendarPage.tsx` - ⏳ Remove onBack prop
- [ ] `CommunityPage.tsx` - ⏳ Remove onBack prop
- [ ] `QAPage.tsx` - ⏳ Remove onBack prop

### Auth Pages (0/3)
- [ ] `LoginPage.tsx` - ⏳ Remove onNavigate prop, use useNavigate
- [ ] `RegisterPage.tsx` - ⏳ Remove onNavigate prop, use useNavigate
- [ ] `ForgotPasswordPage.tsx` - ⏳ Remove onNavigate prop, use useNavigate

### Checkout Pages (0/4)
- [ ] `CheckoutFlow.tsx` - ⏳ Review internal navigation (keep state-based)
- [ ] `CheckoutPage.tsx` - ⏳ Review useHybridNavigate usage
- [ ] `OrderSuccessPage.tsx` - ⏳ Review useHybridNavigate usage
- [ ] `OrderFailedPage.tsx` - ⏳ Review useHybridNavigate usage

---

## 📝 Migration Pattern

### Step 1: Remove Props

**Before:**
```tsx
interface ComponentProps {
  onNavigate: (page: string) => void;
  onBack?: () => void;
}

export function Component({ onNavigate, onBack }: ComponentProps) {
  return (
    <button onClick={() => onNavigate('products')}>
      Products
    </button>
  );
}
```

**After:**
```tsx
import { useNavigate } from 'react-router-dom';

export function Component() {
  const navigate = useNavigate();
  
  return (
    <button onClick={() => navigate('/products')}>
      Products
    </button>
  );
}
```

### Step 2: Replace Navigation Calls

| Old Pattern | New Pattern |
|------------|-------------|
| `onNavigate('home')` | `navigate('/')` |
| `onNavigate('products')` | `navigate('/products')` |
| `onNavigate('delivery')` | `navigate('/delivery')` |
| `onDeliveryNavigate('booking')` | `navigate('/delivery/booking')` |
| `onSpiritualNavigate('virtual-incense')` | `navigate('/spiritual/virtual-incense')` |
| `onBack()` | `navigate(-1)` or `navigate('/parent-path')` |

### Step 3: Add URL Parameters

**Before:**
```tsx
// State passed through props
<OrderDetailsPage orderId="12345" />
```

**After:**
```tsx
// URL parameter
// Route: /delivery/order/:orderId
import { useParams } from 'react-router-dom';

export function OrderDetailsPage() {
  const { orderId } = useParams();
  // orderId = "12345"
}

// Navigate with param
navigate(`/delivery/order/${orderId}`);
```

### Step 4: Use Global Context

**Before:**
```tsx
// Props drilling
<Component 
  prayers={prayers} 
  onSubmitPrayer={handleSubmit}
  onToggleChat={setShowChat}
/>
```

**After:**
```tsx
// Context
import { useApp } from '../lib/contexts/AppContext';

export function Component() {
  const { prayers, addPrayer, showSpiritualChat, setShowSpiritualChat } = useApp();
}
```

---

## 🎯 Priority Tasks

### High Priority (Core Navigation)
1. ✅ App.tsx routing structure
2. ✅ Header navigation
3. ✅ HomePage navigation
4. ⏳ Auth pages (LoginPage, RegisterPage, ForgotPasswordPage)
5. ⏳ Main pages (Products, Services, Cart, Wishlist)

### Medium Priority (Service Pages)
1. ⏳ Delivery service components (6 files)
2. ⏳ Spiritual service components (4 files)
3. ⏳ Community and Q&A pages

### Low Priority (Review)
1. ⏳ Checkout flow (already using useHybridNavigate)
2. ⏳ Profile and settings
3. ⏳ Footer links

---

## 🧪 Testing Checklist

After migration, verify:

- [ ] All navigation links work correctly
- [ ] Browser back/forward buttons work
- [ ] URL updates when navigating
- [ ] Deep links work (e.g., `/delivery/booking`)
- [ ] Nested routes render correctly
- [ ] URL parameters work (e.g., `/delivery/order/:orderId`)
- [ ] Layouts render with correct header/footer
- [ ] Auth layout has no header/footer
- [ ] Global state (prayers, chat modals) works
- [ ] Toast notifications work
- [ ] 404 redirects to home
- [ ] Protected routes redirect to login

---

## 📚 Component Update Guide

### Template for Component Update

```tsx
// 1. Import useNavigate
import { useNavigate } from 'react-router-dom';

// 2. Remove navigation props from interface
interface ComponentProps {
  // Remove: onNavigate, onBack, onDeliveryNavigate, etc.
  // Keep: other props like data, loading, etc.
}

// 3. Use navigate hook
export function Component(props: ComponentProps) {
  const navigate = useNavigate();
  
  // 4. Replace all navigation calls
  const handleClick = () => {
    // Old: onNavigate('products')
    navigate('/products'); // New
  };
  
  const handleBack = () => {
    // Old: onBack()
    navigate(-1); // New - Go back in history
    // OR
    navigate('/parent-path'); // Navigate to specific parent
  };
  
  return (
    // Your component JSX
  );
}
```

---

## 🔍 Find and Replace Patterns

Use these patterns to quickly identify components that need updates:

### Search for navigation props:
```typescript
onNavigate: (page: string) => void
onDeliveryNavigate: (page: string) => void
onSpiritualNavigate: (page: string) => void
onBack?: () => void
onComplete?: () => void
```

### Search for navigation calls:
```typescript
onNavigate('
onDeliveryNavigate('
onSpiritualNavigate('
onBack()
onComplete()
```

---

## 📊 Files to Update

### Components (27 files)
```
✅ /components/Header.tsx
✅ /components/AdPopup.tsx
✅ /components/HomePage.tsx
⏳ /components/ProductsPage.tsx
⏳ /components/ServicesPage.tsx
⏳ /components/AboutPage.tsx
⏳ /components/ContactPage.tsx
⏳ /components/CartPage.tsx
⏳ /components/ProfilePage.tsx
⏳ /components/CalendarPage.tsx
⏳ /components/WishlistPage.tsx
⏳ /components/CommunityPage.tsx
⏳ /components/QAPage.tsx
```

### Delivery (6 files)
```
✅ /components/delivery/DeliveryHomePage.tsx
⏳ /components/delivery/DeliveryBookingPage.tsx
⏳ /components/delivery/PriceEstimationPage.tsx
⏳ /components/delivery/OrderTrackingPage.tsx
⏳ /components/delivery/OrderDetailsPage.tsx
⏳ /components/delivery/DriverRatingPage.tsx
```

### Spiritual (4 files)
```
⏳ /components/spiritual/SpiritualHomePage.tsx
⏳ /components/spiritual/VirtualIncensePage.tsx
⏳ /components/spiritual/AudioChantingPage.tsx
⏳ /components/spiritual/FengShuiConsultationPage.tsx
```

### Auth (3 files)
```
⏳ /pages/auth/LoginPage.tsx
⏳ /pages/auth/RegisterPage.tsx
⏳ /pages/auth/ForgotPasswordPage.tsx
```

### Checkout (4 files) - Review Only
```
⏳ /pages/checkout/CheckoutFlow.tsx
⏳ /pages/checkout/CheckoutPage.tsx
⏳ /pages/checkout/OrderSuccessPage.tsx
⏳ /pages/checkout/OrderFailedPage.tsx
```

---

## 🚀 Next Steps

1. **Phase 1**: Update Auth pages (Login, Register, ForgotPassword)
2. **Phase 2**: Update Delivery service components
3. **Phase 3**: Update Spiritual service components
4. **Phase 4**: Update remaining main pages
5. **Phase 5**: Review and test Checkout flow
6. **Phase 6**: Full application testing
7. **Phase 7**: Update documentation and guides

---

## 📖 Related Documentation

- [ROUTING_ARCHITECTURE.md](./ROUTING_ARCHITECTURE.md) - New routing structure
- [HYBRID_NAVIGATION_GUIDE.md](./HYBRID_NAVIGATION_GUIDE.md) - Old navigation (deprecated)
- [CHECKOUT_IMPLEMENTATION_GUIDE.md](./CHECKOUT_IMPLEMENTATION_GUIDE.md) - Checkout specifics

---

**Last Updated**: 2025-01-08  
**Maintainer**: Development Team  
**Status**: 🚧 Active Migration

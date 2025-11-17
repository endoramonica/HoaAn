# 🚀 Migration Quick Reference

## Common Replacements

### Navigation Calls

```typescript
// Old → New
onNavigate('home')                → navigate('/')
onNavigate('products')            → navigate('/products')
onNavigate('services')            → navigate('/services')
onNavigate('about')               → navigate('/about')
onNavigate('contact')             → navigate('/contact')
onNavigate('cart')                → navigate('/cart')
onNavigate('wishlist')            → navigate('/wishlist')
onNavigate('profile')             → navigate('/profile')
onNavigate('calendar')            → navigate('/calendar')
onNavigate('community')           → navigate('/community')
onNavigate('qa')                  → navigate('/qa')
onNavigate('login')               → navigate('/auth/login')
onNavigate('register')            → navigate('/auth/register')
onNavigate('forgot-password')     → navigate('/auth/forgot-password')
onNavigate('checkout')            → navigate('/checkout')

// Delivery
onNavigate('delivery')                        → navigate('/delivery')
onDeliveryNavigate('delivery-booking')        → navigate('/delivery/booking')
onDeliveryNavigate('price-estimation')        → navigate('/delivery/price-estimation')
onDeliveryNavigate('order-tracking')          → navigate('/delivery/tracking')
onDeliveryNavigate('order-details')           → navigate(`/delivery/order/${orderId}`)
onDeliveryNavigate('driver-rating')           → navigate(`/delivery/rating/${orderId}`)

// Spiritual
onNavigate('spiritual')                       → navigate('/spiritual')
onSpiritualNavigate('virtual-incense')        → navigate('/spiritual/virtual-incense')
onSpiritualNavigate('audio-chanting')         → navigate('/spiritual/audio-chanting')
onSpiritualNavigate('fengshui-consultation')  → navigate('/spiritual/fengshui-consultation')

// Back navigation
onBack()                          → navigate(-1)  // Go back in history
onBack()                          → navigate('/parent-path')  // Specific parent
```

### Import Statements

```typescript
// Add this import
import { useNavigate } from 'react-router-dom';

// If using params
import { useParams } from 'react-router-dom';

// If using location
import { useLocation } from 'react-router-dom';

// If using global state
import { useApp } from '../lib/contexts/AppContext';
```

### Props Interface Changes

```typescript
// REMOVE these props
interface ComponentProps {
  onNavigate: (page: string) => void;          // ❌ Remove
  onDeliveryNavigate: (page: string) => void;  // ❌ Remove
  onSpiritualNavigate: (page: string) => void; // ❌ Remove
  onBack?: () => void;                          // ❌ Remove
  onComplete?: () => void;                      // ❌ Remove
  onToggleSupport?: () => void;                 // ❌ Remove (use context)
  prayers?: Prayer[];                           // ❌ Remove (use context)
  onSubmitPrayer?: (prayer: Prayer) => void;   // ❌ Remove (use context)
  onAddToast?: (toast: Toast) => void;         // ❌ Remove (use sonner)
}

// KEEP these props
interface ComponentProps {
  // Data props - OK to keep
  data?: any;
  loading?: boolean;
  error?: Error;
  
  // Callback props for events (not navigation) - OK to keep
  onSubmit?: (data: any) => void;
  onChange?: (value: any) => void;
  onClose?: () => void;
}
```

### Hook Usage

```typescript
export function Component() {
  // Navigation
  const navigate = useNavigate();
  
  // URL params
  const { orderId, userId } = useParams();
  
  // Current location
  const location = useLocation();
  
  // Global state
  const { 
    showSpiritualChat, 
    setShowSpiritualChat,
    showSupportChat,
    setShowSupportChat,
    prayers,
    addPrayer 
  } = useApp();
  
  // Rest of component...
}
```

## Pattern Examples

### Simple Navigation

```typescript
// Before
<Button onClick={() => onNavigate('products')}>
  Sản phẩm
</Button>

// After
<Button onClick={() => navigate('/products')}>
  Sản phẩm
</Button>
```

### Back Button

```typescript
// Before
<Button onClick={onBack}>
  ← Quay lại
</Button>

// After - Option 1: Browser back
<Button onClick={() => navigate(-1)}>
  ← Quay lại
</Button>

// After - Option 2: Specific parent
<Button onClick={() => navigate('/delivery')}>
  ← Quay lại
</Button>
```

### Navigation with Parameters

```typescript
// Before
<Button onClick={() => onDeliveryNavigate('order-details')}>
  Chi tiết
</Button>

// After
<Button onClick={() => navigate(`/delivery/order/${order.id}`)}>
  Chi tiết
</Button>

// In target component
const { orderId } = useParams();
```

### Conditional Navigation

```typescript
// Before
const handleSubmit = async () => {
  const success = await submitOrder();
  if (success) {
    onNavigate('order-success');
  } else {
    onNavigate('order-failed');
  }
};

// After
const handleSubmit = async () => {
  const success = await submitOrder();
  if (success) {
    navigate('/checkout/success');
  } else {
    navigate('/checkout/failed');
  }
};
```

### Navigation with State

```typescript
// Before
onNavigate('login');

// After - Pass state for redirect after login
navigate('/auth/login', { 
  state: { from: location.pathname } 
});

// In LoginPage
const location = useLocation();
const from = location.state?.from || '/';

const handleLogin = async () => {
  await login();
  navigate(from, { replace: true });
};
```

### Global State Instead of Props

```typescript
// Before
interface Props {
  prayers: Prayer[];
  onSubmitPrayer: (prayer: Prayer) => void;
}

export function Component({ prayers, onSubmitPrayer }: Props) {
  const handleSubmit = (prayer: Prayer) => {
    onSubmitPrayer(prayer);
  };
}

// After
export function Component() {
  const { prayers, addPrayer } = useApp();
  
  const handleSubmit = (prayer: Prayer) => {
    addPrayer(prayer);
  };
}
```

### Toast Notifications

```typescript
// Before
onAddToast({
  type: 'success',
  title: 'Thành công',
  description: 'Đơn hàng đã được tạo'
});

// After
import { toast } from 'sonner@2.0.3';

toast.success('Thành công', {
  description: 'Đơn hàng đã được tạo'
});
```

## Component Checklist

When updating a component:

- [ ] Remove navigation props from interface
- [ ] Add `import { useNavigate } from 'react-router-dom'`
- [ ] Add `const navigate = useNavigate()` in component body
- [ ] Replace all `onNavigate()` calls with `navigate()`
- [ ] Replace all `onBack()` calls with `navigate(-1)` or `navigate('/path')`
- [ ] Add `useParams()` if component needs URL parameters
- [ ] Add `useApp()` if component needs global state
- [ ] Replace toast calls with `sonner` toast
- [ ] Update parent component calls to remove props
- [ ] Test navigation flow

## Testing Commands

```bash
# Check for remaining onNavigate props
grep -r "onNavigate:" components/ pages/

# Check for navigation callback usage
grep -r "onNavigate(" components/ pages/

# Check for onBack usage
grep -r "onBack" components/ pages/

# Check for onDeliveryNavigate
grep -r "onDeliveryNavigate" components/

# Check for onSpiritualNavigate
grep -r "onSpiritualNavigate" components/
```

## Common Mistakes

### ❌ DON'T

```typescript
// Don't use relative paths without context
navigate('products'); // Where does this go?

// Don't keep navigation props
interface Props {
  onNavigate: (page: string) => void; // Remove this!
}

// Don't use window.location
window.location.href = '/products'; // Use navigate()

// Don't forget to remove unused props
<Component onNavigate={handleNav} /> // Remove if Component doesn't need it
```

### ✅ DO

```typescript
// Use absolute paths
navigate('/products');
navigate('/delivery/booking');

// Remove navigation props
interface Props {
  // Only keep non-navigation props
  data: Product[];
}

// Use navigate hook
const navigate = useNavigate();
navigate('/products');

// Clean up parent calls
<Component /> // No navigation props
```

## Quick Win Components

These components are easy to update (no complex logic):

1. `CalendarPage.tsx` - Just has onBack
2. `CommunityPage.tsx` - Just has onBack
3. `QAPage.tsx` - Just has onBack
4. `VirtualIncensePage.tsx` - Just has onBack
5. `AudioChantingPage.tsx` - Just has onBack
6. `FengShuiConsultationPage.tsx` - Just has onBack

Template for these:
```typescript
// Before
interface PageProps {
  onBack: () => void;
}

export function Page({ onBack }: PageProps) {
  return (
    <button onClick={onBack}>Back</button>
  );
}

// After
import { useNavigate } from 'react-router-dom';

export function Page() {
  const navigate = useNavigate();
  
  return (
    <button onClick={() => navigate(-1)}>Back</button>
  );
}
```

---

**Pro Tip**: Start with "Quick Win" components first to build momentum, then tackle complex components with multiple navigation points.

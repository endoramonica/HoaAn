# Frontend Page Integration Guide

**Date**: December 21, 2025  
**Status**: Ready for Implementation

---

## 📋 Overview

Hướng dẫn chi tiết cách integrate ActionTrackingService vào các pages và RitualRecommendation vào CartPage.

---

## 🎯 Integration Points

### 1. HomePage
**File**: `src/pages/HomePage.tsx`

**Actions to Track**:
- BrowseCategory (when user views featured categories)
- ViewProduct (when user views featured products)

**Implementation**:
```typescript
import { getActionTrackingService } from '@/lib/services/actionTrackingService';

export const HomePage: React.FC = () => {
  const actionTracking = getActionTrackingService();

  // Track category browse
  const handleCategoryClick = (categoryId: string) => {
    actionTracking.trackAction('BrowseCategory', {}, undefined, categoryId);
    // Navigate to category
  };

  // Track product view
  const handleProductClick = (productId: string, categoryId: string) => {
    actionTracking.trackAction('ViewProduct', {}, productId, categoryId);
    // Navigate to product
  };

  return (
    <div>
      {/* Featured Categories */}
      <section>
        {categories.map((cat) => (
          <div key={cat.id} onClick={() => handleCategoryClick(cat.id)}>
            {cat.name}
          </div>
        ))}
      </section>

      {/* Featured Products */}
      <section>
        {products.map((prod) => (
          <div key={prod.id} onClick={() => handleProductClick(prod.id, prod.categoryId)}>
            {prod.name}
          </div>
        ))}
      </section>
    </div>
  );
};
```

---

### 2. ServicePage
**File**: `src/pages/ServicePage.tsx`

**Actions to Track**:
- BrowseCategory (when user views service categories)
- ViewProduct (when user views services)

**Implementation**:
```typescript
import { getActionTrackingService } from '@/lib/services/actionTrackingService';

export const ServicePage: React.FC = () => {
  const actionTracking = getActionTrackingService();

  // Track category browse
  const handleCategoryFilter = (categoryId: string) => {
    actionTracking.trackAction('BrowseCategory', {}, undefined, categoryId);
    // Filter services
  };

  // Track service view
  const handleServiceClick = (serviceId: string, categoryId: string) => {
    actionTracking.trackAction('ViewProduct', {}, serviceId, categoryId);
    // Navigate to service detail
  };

  return (
    <div>
      {/* Category Filter */}
      <aside>
        {categories.map((cat) => (
          <button key={cat.id} onClick={() => handleCategoryFilter(cat.id)}>
            {cat.name}
          </button>
        ))}
      </aside>

      {/* Services List */}
      <main>
        {services.map((svc) => (
          <div key={svc.id} onClick={() => handleServiceClick(svc.id, svc.categoryId)}>
            {svc.name}
          </div>
        ))}
      </main>
    </div>
  );
};
```

---

### 3. ProductPage
**File**: `src/pages/ProductPage.tsx`

**Actions to Track**:
- ViewProduct (when product details load)
- AddToCart (when user adds to cart)
- BrowseCategory (when user views related products)

**Implementation**:
```typescript
import { getActionTrackingService } from '@/lib/services/actionTrackingService';

export const ProductPage: React.FC = () => {
  const actionTracking = getActionTrackingService();
  const { productId } = useParams();
  const [product, setProduct] = useState<Product | null>(null);

  // Track product view on mount
  useEffect(() => {
    if (product) {
      actionTracking.trackAction('ViewProduct', {}, product.id, product.categoryId);
    }
  }, [product?.id]);

  // Track add to cart
  const handleAddToCart = (quantity: number) => {
    actionTracking.trackAction('AddToCart', { quantity }, product?.id, product?.categoryId);
    // Add to cart logic
  };

  // Track related product view
  const handleRelatedProductClick = (relatedProductId: string, categoryId: string) => {
    actionTracking.trackAction('ViewProduct', {}, relatedProductId, categoryId);
    // Navigate to related product
  };

  return (
    <div>
      {/* Product Details */}
      <section>
        <h1>{product?.name}</h1>
        <p>{product?.description}</p>
        <button onClick={() => handleAddToCart(1)}>Thêm vào giỏ</button>
      </section>

      {/* Related Products */}
      <section>
        <h2>Sản phẩm liên quan</h2>
        {relatedProducts.map((prod) => (
          <div key={prod.id} onClick={() => handleRelatedProductClick(prod.id, prod.categoryId)}>
            {prod.name}
          </div>
        ))}
      </section>
    </div>
  );
};
```

---

### 4. CommunityPage (TaggedProduct)
**File**: `src/pages/CommunityPage.tsx` or `src/components/TaggedProduct.tsx`

**Actions to Track**:
- ViewProduct (when user views tagged product)
- AddToCart (when user adds tagged product to cart)

**Implementation**:
```typescript
import { getActionTrackingService } from '@/lib/services/actionTrackingService';

export const TaggedProduct: React.FC<{ product: Product }> = ({ product }) => {
  const actionTracking = getActionTrackingService();

  // Track product view
  const handleProductClick = () => {
    actionTracking.trackAction('ViewProduct', {}, product.id, product.categoryId);
    // Navigate to product
  };

  // Track add to cart
  const handleAddToCart = () => {
    actionTracking.trackAction('AddToCart', {}, product.id, product.categoryId);
    // Add to cart logic
  };

  return (
    <div onClick={handleProductClick}>
      <img src={product.image} alt={product.name} />
      <h3>{product.name}</h3>
      <p>{product.price}</p>
      <button onClick={(e) => {
        e.stopPropagation();
        handleAddToCart();
      }}>
        Thêm vào giỏ
      </button>
    </div>
  );
};
```

---

### 5. CartPage
**File**: `src/pages/CartPage.tsx`

**Actions to Track**:
- AddToCart (when user adds recommended items)

**Components to Add**:
- RitualRecommendation component
- Real-time updates with SignalR

**Implementation**:
```typescript
import { useEffect, useState, useCallback } from 'react';
import { getActionTrackingService } from '@/lib/services/actionTrackingService';
import { getRecommendationService, RecommendationPayload, ExplanationPayload } from '@/lib/services/recommendationService';
import { getUserPreferenceService } from '@/lib/services/userPreferenceService';
import { RitualRecommendation } from '@/components/RitualRecommendation';

export const CartPage: React.FC = () => {
  const actionTracking = getActionTrackingService();
  const recommendationService = getRecommendationService();
  const userPreferenceService = getUserPreferenceService();

  const [recommendation, setRecommendation] = useState<RecommendationPayload | null>(null);
  const [explanation, setExplanation] = useState<ExplanationPayload | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [cartItems, setCartItems] = useState<CartItem[]>([]);

  // Generate recommendation
  const generateRecommendation = useCallback(async () => {
    const actionSequence = actionTracking.getActionSequence();
    if (actionSequence.length === 0) {
      setRecommendation(null);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const { payload, explanation: exp } = await recommendationService.generateRecommendation(
        actionSequence,
        userPreferenceService.getDebugInfo().userId,
        actionTracking.getSessionId()
      );

      // Check if ritual is disabled
      if (payload.matched && payload.ritualId && userPreferenceService.isRitualDisabled(payload.ritualId)) {
        setRecommendation(null);
        return;
      }

      setRecommendation(payload);
      setExplanation(exp);
    } catch (err) {
      console.error('Error generating recommendation:', err);
      setError('Failed to generate recommendation');
    } finally {
      setIsLoading(false);
    }
  }, [actionTracking, recommendationService, userPreferenceService]);

  // Generate recommendation on mount and when cart changes
  useEffect(() => {
    generateRecommendation();
  }, [generateRecommendation, cartItems]);

  // Subscribe to action changes
  useEffect(() => {
    const unsubscribe = actionTracking.subscribe(() => {
      generateRecommendation();
    });
    return unsubscribe;
  }, [actionTracking, generateRecommendation]);

  // Handle dismiss
  const handleDismiss = useCallback(() => {
    if (recommendation?.ritualId) {
      userPreferenceService.recordDismissal(
        'current-user-id', // Get from auth context
        recommendation.ritualId
      );
    }
  }, [recommendation, userPreferenceService]);

  // Handle disable ritual
  const handleDisableRitual = useCallback(() => {
    if (recommendation?.ritualId) {
      userPreferenceService.disableRitual(
        'current-user-id', // Get from auth context
        actionTracking.getSessionId(),
        recommendation.ritualId
      );
    }
  }, [recommendation, userPreferenceService, actionTracking]);

  // Handle add to cart from recommendation
  const handleAddToCart = useCallback((productId: string) => {
    actionTracking.trackAction('AddToCart', {}, productId);
    // Add to cart logic
    // Refresh cart items
  }, [actionTracking]);

  return (
    <div className="space-y-6">
      {/* Section 1: Cart Items */}
      <section>
        <h2>Giỏ hàng của bạn</h2>
        {cartItems.map((item) => (
          <CartItemRow key={item.id} item={item} />
        ))}
      </section>

      {/* Section 2: Order Summary */}
      <section>
        <h2>Tóm tắt đơn hàng</h2>
        <OrderSummary items={cartItems} />
      </section>

      {/* Section 3: Ritual Recommendations */}
      <section>
        <h2>Gợi ý từ AI</h2>
        <RitualRecommendation
          recommendation={recommendation}
          explanation={explanation}
          isLoading={isLoading}
          error={error}
          onDismiss={handleDismiss}
          onDisableRitual={handleDisableRitual}
          onAddToCart={handleAddToCart}
        />
      </section>

      {/* Section 4: Checkout */}
      <section>
        <CheckoutButton />
      </section>
    </div>
  );
};
```

---

## 🔄 Real-time Updates with SignalR

**File**: `src/lib/services/signalRService.ts` (to be created)

**Implementation**:
```typescript
import * as signalR from '@microsoft/signalr';

class SignalRService {
  private connection: signalR.HubConnection | null = null;

  async connect(): Promise<void> {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(import.meta.env.VITE_SIGNALR_HUB_URL || 'http://localhost:5000/hubs/realtime')
      .withAutomaticReconnect()
      .build();

    this.connection.on('RecommendationUpdated', (payload) => {
      // Handle recommendation update
      console.log('Recommendation updated:', payload);
    });

    this.connection.on('PreferenceUpdated', (preference) => {
      // Handle preference update
      console.log('Preference updated:', preference);
    });

    await this.connection.start();
  }

  disconnect(): void {
    this.connection?.stop();
  }

  subscribe(event: string, callback: (data: any) => void): void {
    this.connection?.on(event, callback);
  }
}

export const signalRService = new SignalRService();
```

---

## 📊 Data Flow Diagram

```
User Action (HomePage, ProductPage, etc.)
    ↓
ActionTrackingService.trackAction()
    ↓
Action stored in session storage
    ↓
[On CartPage Load]
    ↓
RecommendationService.generateRecommendation()
    ↓
BE-AI Analysis (POST /api/recommendations/analyze)
    ↓
FE-AI Explanation (POST /api/explanations/generate)
    ↓
RitualRecommendation Component displays
    ↓
User can:
  - Add items to cart → trackAction('AddToCart')
  - Dismiss → UserPreferenceService.recordDismissal()
  - Disable ritual → UserPreferenceService.disableRitual()
```

---

## ✅ Integration Checklist

### HomePage
- [ ] Import ActionTrackingService
- [ ] Track BrowseCategory on category click
- [ ] Track ViewProduct on product click
- [ ] Test action tracking

### ServicePage
- [ ] Import ActionTrackingService
- [ ] Track BrowseCategory on filter
- [ ] Track ViewProduct on service click
- [ ] Test action tracking

### ProductPage
- [ ] Import ActionTrackingService
- [ ] Track ViewProduct on mount
- [ ] Track AddToCart on add button
- [ ] Track ViewProduct on related product click
- [ ] Test action tracking

### CommunityPage
- [ ] Import ActionTrackingService
- [ ] Track ViewProduct on product click
- [ ] Track AddToCart on add button
- [ ] Test action tracking

### CartPage
- [ ] Import all services
- [ ] Import RitualRecommendation component
- [ ] Add recommendation section
- [ ] Implement generateRecommendation()
- [ ] Subscribe to action changes
- [ ] Handle dismiss
- [ ] Handle disable ritual
- [ ] Handle add to cart
- [ ] Test end-to-end flow

### SignalR (Optional)
- [ ] Create SignalRService
- [ ] Connect on app load
- [ ] Subscribe to events
- [ ] Handle real-time updates

---

## 🧪 Testing

### Unit Tests
```typescript
// Test ActionTrackingService
describe('ActionTrackingService', () => {
  it('should track action', () => {
    const service = getActionTrackingService();
    service.trackAction('ViewProduct', {}, 'prod-1', 'cat-1');
    expect(service.getActionCount()).toBe(1);
  });
});

// Test RecommendationService
describe('RecommendationService', () => {
  it('should call BE-AI endpoint', async () => {
    const service = getRecommendationService();
    const result = await service.callBeAiAnalysis([]);
    expect(result).toBeDefined();
  });
});

// Test UserPreferenceService
describe('UserPreferenceService', () => {
  it('should record dismissal', async () => {
    const service = getUserPreferenceService();
    await service.recordDismissal('user-1', 'ritual-1');
    expect(service.getDismissalCount('ritual-1')).toBe(1);
  });
});
```

### Component Tests
```typescript
// Test RitualRecommendation
describe('RitualRecommendation', () => {
  it('should render recommendation', () => {
    const recommendation = {
      matched: true,
      ritualId: 'ritual-1',
      ritualName: 'Tết',
      confidenceScore: 0.8,
      missingItems: [],
    };
    const { getByText } = render(
      <RitualRecommendation
        recommendation={recommendation}
        explanation={null}
        isLoading={false}
        error={null}
        onDismiss={() => {}}
        onDisableRitual={() => {}}
        onAddToCart={() => {}}
      />
    );
    expect(getByText('Tết')).toBeInTheDocument();
  });
});
```

---

## 🚀 Implementation Order

1. **HomePage** (30 min)
2. **ServicePage** (30 min)
3. **ProductPage** (45 min)
4. **CommunityPage** (30 min)
5. **CartPage** (1.5 hours)
6. **SignalR** (1 hour)
7. **Testing** (2 hours)

**Total**: ~6-7 hours

---

## 📞 Support

### Common Issues

**Issue**: Action not being tracked
- Check if ActionTrackingService is imported
- Check if trackAction() is called with correct parameters
- Check browser console for errors

**Issue**: Recommendation not showing
- Check if BE-AI endpoint is running
- Check if action sequence has items
- Check browser console for API errors

**Issue**: Add to cart not working
- Check if onAddToCart callback is implemented
- Check if cart service is working
- Check browser console for errors

---

**Status**: Ready for Implementation  
**Last Updated**: December 21, 2025  
**Prepared By**: Kiro AI Assistant

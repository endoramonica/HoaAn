# Phase 2: Page Integration Plan

**Date**: December 21, 2025  
**Status**: ✅ **READY FOR PHASE 2**

---

## 📊 Current Status

| Component | Status | Completion |
|-----------|--------|-----------|
| Backend | ✅ Complete | 100% |
| Frontend Services | ✅ Complete | 100% |
| Frontend Components | ✅ Complete | 100% |
| HomePage Integration | ✅ Complete | 100% |
| **Overall** | ✅ In Progress | **~80%** |

---

## 🎯 Phase 2: Page Integration

### Pages to Integrate (in order)

1. **ProductsPage** (45 min)
   - Track "BrowseCategory" on category filter
   - Track "ViewProduct" on product click
   - Track "AddToCart" on add button

2. **ProductDetailPage** (45 min)
   - Track "ViewProduct" on mount
   - Track "AddToCart" on add button
   - Track "ViewProduct" on related products

3. **ServicesPage** (30 min)
   - Track "BrowseCategory" on service category
   - Track "ViewProduct" on service click

4. **ServiceDetailPage** (30 min)
   - Track "ViewProduct" on mount
   - Track "AddToCart" on booking/add button

5. **CommunityPage** (30 min)
   - Track "ViewProduct" on product click
   - Track "AddToCart" on add button

6. **CartPage** (1.5 hours)
   - Add RitualRecommendation component
   - Integrate all services
   - Handle user interactions

---

## 📁 Files to Modify

### 1. ProductsPage
**File**: `src/components/ProductsPage.tsx`

**Actions to Track**:
- BrowseCategory (on category filter)
- ViewProduct (on product click)
- AddToCart (on add button)

**Integration Points**:
```typescript
// Import
import { getActionTrackingService } from '@/lib/services/actionTrackingService';

// In component
const actionTracking = getActionTrackingService();

// On category filter
const handleCategoryFilter = (categoryId: string) => {
  actionTracking.trackAction('BrowseCategory', {}, undefined, categoryId);
  // Filter logic
};

// On product click
const handleProductClick = (productId: string, categoryId: string) => {
  actionTracking.trackAction('ViewProduct', {}, productId, categoryId);
  // Navigate to product detail
};

// On add to cart
const handleAddToCart = (productId: string, categoryId: string) => {
  actionTracking.trackAction('AddToCart', {}, productId, categoryId);
  // Add to cart logic
};
```

---

### 2. ProductDetailPage
**File**: `src/pages/ProductDetailPage.tsx`

**Actions to Track**:
- ViewProduct (on mount)
- AddToCart (on add button)
- ViewProduct (on related products)

**Integration Points**:
```typescript
// Import
import { getActionTrackingService } from '@/lib/services/actionTrackingService';

// In component
const actionTracking = getActionTrackingService();
const { productId } = useParams();

// On mount - track product view
useEffect(() => {
  if (product) {
    actionTracking.trackAction('ViewProduct', {}, product.id, product.categoryId);
  }
}, [product?.id]);

// On add to cart
const handleAddToCart = (quantity: number) => {
  actionTracking.trackAction('AddToCart', { quantity }, product?.id, product?.categoryId);
  // Add to cart logic
};

// On related product click
const handleRelatedProductClick = (relatedProductId: string, categoryId: string) => {
  actionTracking.trackAction('ViewProduct', {}, relatedProductId, categoryId);
  // Navigate to related product
};
```

---

### 3. ServicesPage
**File**: `src/components/ServicesPage.tsx`

**Actions to Track**:
- BrowseCategory (on service category)
- ViewProduct (on service click)

**Integration Points**:
```typescript
// Import
import { getActionTrackingService } from '@/lib/services/actionTrackingService';

// In component
const actionTracking = getActionTrackingService();

// On category filter
const handleCategoryFilter = (categoryId: string) => {
  actionTracking.trackAction('BrowseCategory', {}, undefined, categoryId);
  // Filter logic
};

// On service click
const handleServiceClick = (serviceId: string, categoryId: string) => {
  actionTracking.trackAction('ViewProduct', {}, serviceId, categoryId);
  // Navigate to service detail
};
```

---

### 4. ServiceDetailPage
**File**: `src/components/ServiceDetailPage.tsx`

**Actions to Track**:
- ViewProduct (on mount)
- AddToCart (on booking/add button)

**Integration Points**:
```typescript
// Import
import { getActionTrackingService } from '@/lib/services/actionTrackingService';

// In component
const actionTracking = getActionTrackingService();

// On mount - track service view
useEffect(() => {
  if (service) {
    actionTracking.trackAction('ViewProduct', {}, service.id, service.categoryId);
  }
}, [service?.id]);

// On booking/add
const handleBooking = () => {
  actionTracking.trackAction('AddToCart', {}, service?.id, service?.categoryId);
  // Booking logic
};
```

---

### 5. CommunityPage
**File**: `src/components/CommunityPage.tsx`

**Actions to Track**:
- ViewProduct (on product click)
- AddToCart (on add button)

**Integration Points**:
```typescript
// Import
import { getActionTrackingService } from '@/lib/services/actionTrackingService';

// In component
const actionTracking = getActionTrackingService();

// On product click
const handleProductClick = (productId: string, categoryId: string) => {
  actionTracking.trackAction('ViewProduct', {}, productId, categoryId);
  // Navigate to product detail
};

// On add to cart
const handleAddToCart = (productId: string, categoryId: string) => {
  actionTracking.trackAction('AddToCart', {}, productId, categoryId);
  // Add to cart logic
};
```

---

### 6. CartPage
**File**: `src/components/CartPage.tsx`

**Components to Add**:
- RitualRecommendation component
- Real-time updates

**Integration Points**:
```typescript
// Imports
import { useEffect, useState, useCallback } from 'react';
import { getActionTrackingService } from '@/lib/services/actionTrackingService';
import { getRecommendationService } from '@/lib/services/recommendationService';
import { getUserPreferenceService } from '@/lib/services/userPreferenceService';
import { RitualRecommendation } from '@/components/RitualRecommendation';

export const CartPage: React.FC = () => {
  const actionTracking = getActionTrackingService();
  const recommendationService = getRecommendationService();
  const userPreferenceService = getUserPreferenceService();

  const [recommendation, setRecommendation] = useState(null);
  const [explanation, setExplanation] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  // Generate recommendation
  const generateRecommendation = useCallback(async () => {
    const actionSequence = actionTracking.getActionSequence();
    if (actionSequence.length === 0) {
      setRecommendation(null);
      return;
    }

    setIsLoading(true);
    try {
      const { payload, explanation: exp } = await recommendationService.generateRecommendation(
        actionSequence,
        'current-user-id',
        actionTracking.getSessionId()
      );

      // Check if ritual is disabled
      if (payload.matched && payload.ritualId && userPreferenceService.isRitualDisabled(payload.ritualId)) {
        setRecommendation(null);
        return;
      }

      setRecommendation(payload);
      setExplanation(exp);
    } finally {
      setIsLoading(false);
    }
  }, [actionTracking, recommendationService, userPreferenceService]);

  // Generate on mount and when cart changes
  useEffect(() => {
    generateRecommendation();
  }, [generateRecommendation]);

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
      userPreferenceService.recordDismissal('current-user-id', recommendation.ritualId);
    }
  }, [recommendation, userPreferenceService]);

  // Handle disable ritual
  const handleDisableRitual = useCallback(() => {
    if (recommendation?.ritualId) {
      userPreferenceService.disableRitual(
        'current-user-id',
        actionTracking.getSessionId(),
        recommendation.ritualId
      );
    }
  }, [recommendation, userPreferenceService, actionTracking]);

  // Handle add to cart from recommendation
  const handleAddToCart = useCallback((productId: string) => {
    actionTracking.trackAction('AddToCart', {}, productId);
    // Add to cart logic
  }, [actionTracking]);

  return (
    <div className="space-y-6">
      {/* Existing cart sections */}
      
      {/* Ritual Recommendations Section */}
      <section>
        <h2>Gợi ý từ AI</h2>
        <RitualRecommendation
          recommendation={recommendation}
          explanation={explanation}
          isLoading={isLoading}
          error={null}
          onDismiss={handleDismiss}
          onDisableRitual={handleDisableRitual}
          onAddToCart={handleAddToCart}
        />
      </section>
    </div>
  );
};
```

---

## 📋 Integration Checklist

### ProductsPage
- [ ] Import ActionTrackingService
- [ ] Track BrowseCategory on filter
- [ ] Track ViewProduct on click
- [ ] Track AddToCart on button
- [ ] Test

### ProductDetailPage
- [ ] Import ActionTrackingService
- [ ] Track ViewProduct on mount
- [ ] Track AddToCart on button
- [ ] Track ViewProduct on related
- [ ] Test

### ServicesPage
- [ ] Import ActionTrackingService
- [ ] Track BrowseCategory on filter
- [ ] Track ViewProduct on click
- [ ] Test

### ServiceDetailPage
- [ ] Import ActionTrackingService
- [ ] Track ViewProduct on mount
- [ ] Track AddToCart on button
- [ ] Test

### CommunityPage
- [ ] Import ActionTrackingService
- [ ] Track ViewProduct on click
- [ ] Track AddToCart on button
- [ ] Test

### CartPage
- [ ] Import all services
- [ ] Import RitualRecommendation
- [ ] Add recommendation section
- [ ] Implement generateRecommendation
- [ ] Subscribe to actions
- [ ] Handle dismiss/disable
- [ ] Handle add to cart
- [ ] Test end-to-end

---

## ⏱️ Time Estimate

| Page | Time | Status |
|------|------|--------|
| ProductsPage | 45 min | ⏳ |
| ProductDetailPage | 45 min | ⏳ |
| ServicesPage | 30 min | ⏳ |
| ServiceDetailPage | 30 min | ⏳ |
| CommunityPage | 30 min | ⏳ |
| CartPage | 1.5 hours | ⏳ |
| **Total** | **~4 hours** | **⏳** |

---

## 🚀 Next Steps

1. **ProductsPage** - Start here
2. **ProductDetailPage** - Follow
3. **ServicesPage** - Continue
4. **ServiceDetailPage** - Continue
5. **CommunityPage** - Continue
6. **CartPage** - Final integration

---

## 📊 Overall Progress After Phase 2

| Component | Status | Completion |
|-----------|--------|-----------|
| Backend | ✅ Complete | 100% |
| Frontend Services | ✅ Complete | 100% |
| Frontend Components | ✅ Complete | 100% |
| HomePage | ✅ Complete | 100% |
| ProductsPage | ⏳ Ready | 0% |
| ProductDetailPage | ⏳ Ready | 0% |
| ServicesPage | ⏳ Ready | 0% |
| ServiceDetailPage | ⏳ Ready | 0% |
| CommunityPage | ⏳ Ready | 0% |
| CartPage | ⏳ Ready | 0% |
| Testing | ⏳ Ready | 0% |
| **Overall** | ✅ In Progress | **~80%** |

---

## 🎯 Success Criteria

Phase 2 is complete when:
1. ✅ All pages have action tracking
2. ✅ CartPage displays recommendations
3. ✅ User preferences work
4. ✅ End-to-end flow works
5. ✅ No console errors

---

**Status**: ✅ PHASE 2 READY  
**Completion**: ~80%  
**Time Remaining**: ~4 hours (integration) + 2-3 hours (testing)  
**Last Updated**: December 21, 2025  
**Prepared By**: Kiro AI Assistant

---

## 🚀 Ready to Continue?

**Next**: ProductsPage Integration

Start with ProductsPage and follow the integration pattern provided above.


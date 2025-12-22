# Frontend Implementation Tasks - Sequential Ritual Recommendation System

**Status**: Ready for Implementation  
**Priority**: HIGH - Critical for system completion  
**Estimated Effort**: 12-16 hours  
**Target Pages**: HomePage, ServicePage, ProductPage, TaggedProduct (CommunityPage), CartPage

---

## Overview

This document outlines all frontend implementation tasks required to complete the Sequential Ritual Recommendation System. The frontend is responsible for:

1. **Action Tracking** - Capture user interactions on specified pages
2. **Recommendation Display** - Show recommendations in CartPage as carousel slider
3. **User Preferences** - Handle dismissals and ritual disabling
4. **Real-time Updates** - Use SignalR for real-time recommendation updates

---

## Phase 1: Action Tracking Service

### Task 1.1: Create ActionTrackingService

**File**: `src/services/actionTrackingService.ts`

**Responsibilities**:
- Track user actions (ViewProduct, AddToCart, BrowseCategory)
- Maintain action sequence in session
- Send actions to backend for analysis
- Manage session ID

**Implementation Details**:

```typescript
interface Action {
  type: 'ViewProduct' | 'AddToCart' | 'BrowseCategory'
  productId?: string
  categoryId?: string
  timestamp: number
  metadata?: Record<string, any>
}

class ActionTrackingService {
  // Track a user action
  trackAction(actionType: string, metadata?: any): void
  
  // Get current action sequence
  getActionSequence(): Action[]
  
  // Clear sequence (on navigation)
  clearActionSequence(): void
  
  // Get session ID
  getSessionId(): string
  
  // Send actions to backend
  sendActionsToBackend(): Promise<void>
}
```

**Integration Points**:
- [ ] Hook into product view events (ProductPage)
- [ ] Hook into add-to-cart events (ProductPage, CartPage)
- [ ] Hook into category browse events (ServicePage, HomePage)
- [ ] Hook into tagged product view (CommunityPage)
- [ ] Clear sequence on page navigation

**Requirements**: 1.1, 5.3, 5.4

---

### Task 1.2: Integrate Action Tracking into HomePage

**File**: `src/pages/HomePage.tsx`

**Changes**:
- [ ] Import ActionTrackingService
- [ ] Track "BrowseCategory" when user views category sections
- [ ] Track "ViewProduct" when user clicks on featured products
- [ ] Initialize service on component mount

**Requirements**: 1.1, 5.3

---

### Task 1.3: Integrate Action Tracking into ServicePage

**File**: `src/pages/ServicePage.tsx`

**Changes**:
- [ ] Import ActionTrackingService
- [ ] Track "BrowseCategory" when user browses services
- [ ] Track "ViewProduct" when user views service details
- [ ] Initialize service on component mount

**Requirements**: 1.1, 5.3

---

### Task 1.4: Integrate Action Tracking into ProductPage

**File**: `src/pages/ProductPage.tsx`

**Changes**:
- [ ] Import ActionTrackingService
- [ ] Track "ViewProduct" when product details load
- [ ] Track "AddToCart" when user adds product to cart
- [ ] Track "BrowseCategory" when user browses related products
- [ ] Initialize service on component mount

**Requirements**: 1.1, 5.3

---

### Task 1.5: Integrate Action Tracking into CommunityPage (TaggedProduct)

**File**: `src/pages/CommunityPage.tsx` or `src/components/TaggedProduct.tsx`

**Changes**:
- [ ] Import ActionTrackingService
- [ ] Track "ViewProduct" when user views tagged products
- [ ] Track "AddToCart" when user adds tagged product to cart
- [ ] Initialize service on component mount

**Requirements**: 1.1, 5.3

---

## Phase 2: Recommendation Service

### Task 2.1: Create RecommendationService

**File**: `src/services/recommendationService.ts`

**Responsibilities**:
- Call BE-AI `/api/recommendations/analyze` endpoint
- Call FE-AI `/api/explanations/generate` endpoint
- Cache recommendations for 15 minutes
- Handle errors and retries
- Manage recommendation state

**Implementation Details**:

```typescript
interface RecommendationPayload {
  ritualId: string
  ritualName: string
  confidenceScore: number
  missingItems: Product[]
  matchingMetadata: any
  systemReport: string
}

interface ExplanationPayload {
  ritualName: string
  culturalContext: string
  itemExplanations: Record<string, string>
  sources: string[]
  generatedBy: string
}

class RecommendationService {
  // Call BE-AI to analyze action sequence
  async callBeAiAnalysis(
    actionSequence: Action[]
  ): Promise<RecommendationPayload>
  
  // Call FE-AI to generate explanation
  async callFeAiExplanation(
    payload: RecommendationPayload
  ): Promise<ExplanationPayload>
  
  // Get cached recommendation
  getCachedRecommendation(): RecommendationPayload | null
  
  // Clear cached recommendation
  clearCachedRecommendation(): void
  
  // Check if cache is still valid (15 min TTL)
  isCacheValid(): boolean
}
```

**API Endpoints**:
- `POST /api/recommendations/analyze` - BE-AI analysis
- `POST /api/explanations/generate` - FE-AI explanation

**Error Handling**:
- Timeout handling (5 second timeout)
- Retry logic with exponential backoff (max 3 retries)
- Fallback to cached data if available
- User-friendly error messages
- No error display to user (silent fail)

**Caching Strategy**:
- Cache TTL: 15 minutes
- Cache key: hash of action sequence
- Clear cache on: navigation, cart update, new recommendation

**Requirements**: 1.3, 1.4, 2.1, 2.2, 2.3

---

### Task 2.2: Integrate RecommendationService with ActionTrackingService

**File**: `src/services/recommendationService.ts`

**Changes**:
- [ ] Listen to action tracking events
- [ ] Trigger recommendation analysis when action sequence changes
- [ ] Implement real-time analysis (not batch)
- [ ] Use SignalR for real-time updates if available

**Requirements**: 1.1, 5.3

---

## Phase 3: Recommendation UI Component

### Task 3.1: Create RitualRecommendation Component

**File**: `src/components/RitualRecommendation.tsx`

**Responsibilities**:
- Display ritual name and confidence score
- Show cultural explanation
- List missing items with reasons
- Add "Add to Cart" buttons for missing items
- Handle dismiss action
- Handle disable ritual action
- Display as carousel slider

**Component Structure**:

```typescript
interface RitualRecommendationProps {
  recommendation: RecommendationPayload
  explanation: ExplanationPayload
  onDismiss: () => void
  onDisable: () => void
  onAddToCart: (productId: string) => void
}

export const RitualRecommendation: React.FC<RitualRecommendationProps> = ({
  recommendation,
  explanation,
  onDismiss,
  onDisable,
  onAddToCart
}) => {
  // Component implementation
}
```

**UI Elements**:
- [ ] Ritual name header with confidence score badge
- [ ] Cultural explanation section
- [ ] Missing items carousel slider
- [ ] Item cards with:
  - Product image
  - Product name
  - Product price
  - Item reason/explanation
  - "Add to Cart" button
- [ ] Action buttons:
  - "Dismiss" button
  - "Disable Ritual" button
  - "Learn More" button (optional)

**Styling**:
- Use existing design system
- Responsive design for mobile/tablet/desktop
- Smooth animations for carousel
- Accessible color contrast

**Requirements**: 2.1, 2.2, 2.3, 2.5

---

### Task 3.2: Create Carousel Slider Component (if not exists)

**File**: `src/components/CarouselSlider.tsx` (if needed)

**Responsibilities**:
- Display items in carousel format
- Support touch/mouse navigation
- Auto-scroll capability
- Responsive to screen size

**Requirements**: 2.1

---

## Phase 4: CartPage Integration

### Task 4.1: Integrate RitualRecommendation into CartPage

**File**: `src/pages/CartPage.tsx`

**Changes**:
- [ ] Import RitualRecommendation component
- [ ] Import RecommendationService
- [ ] Add recommendation section below cart items
- [ ] Display as carousel slider (Section 3)
- [ ] Handle "Add to Cart" for recommended items
- [ ] Handle dismiss and disable actions
- [ ] Show loading state while fetching recommendations
- [ ] Show error state if recommendation fails (silent)

**Layout**:
```
┌─────────────────────────────────────┐
│ Cart Items Section                  │
├─────────────────────────────────────┤
│ Cart Summary (Subtotal, Tax, etc.)  │
├─────────────────────────────────────┤
│ SECTION 3: Ritual Recommendations   │
│ ┌─────────────────────────────────┐ │
│ │ Carousel Slider with Items      │ │
│ │ [Item 1] [Item 2] [Item 3] ...  │ │
│ └─────────────────────────────────┘ │
├─────────────────────────────────────┤
│ Checkout Button                     │
└─────────────────────────────────────┘
```

**Requirements**: 2.1, 2.2, 2.3, 2.5

---

### Task 4.2: Handle Real-time Updates with SignalR

**File**: `src/pages/CartPage.tsx`

**Changes**:
- [ ] Connect to SignalR hub for real-time updates
- [ ] Listen for recommendation updates
- [ ] Update UI when new recommendations arrive
- [ ] Handle connection errors gracefully

**SignalR Integration**:
- Use existing SignalR connection from workspace
- Listen to recommendation update events
- Update component state on new recommendations

**Requirements**: 1.1, 5.3

---

## Phase 5: User Preferences UI

### Task 5.1: Create UserPreferenceService

**File**: `src/services/userPreferenceService.ts`

**Responsibilities**:
- Track dismissed rituals (per session)
- Track disabled rituals (per session)
- Call backend API to persist preferences
- Manage preference state

**Implementation Details**:

```typescript
class UserPreferenceService {
  // Record dismissal
  async recordDismissal(ritualId: string): Promise<void>
  
  // Disable ritual
  async disableRitual(ritualId: string): Promise<void>
  
  // Check if ritual is disabled
  isRitualDisabled(ritualId: string): boolean
  
  // Get dismissal count
  getDismissalCount(ritualId: string): number
  
  // Get dismissal history
  getDismissalHistory(): Record<string, number>
}
```

**API Endpoints**:
- `POST /api/preferences/dismiss-ritual` - Record dismissal
- `POST /api/preferences/disable-ritual` - Disable ritual

**Storage**:
- Store in session storage (per session)
- Sync with backend for persistence

**Requirements**: 6.2, 6.3

---

### Task 5.2: Integrate Dismissal Handling

**File**: `src/components/RitualRecommendation.tsx`

**Changes**:
- [ ] Import UserPreferenceService
- [ ] Handle "Dismiss" button click
- [ ] Call `recordDismissal()` API
- [ ] Remove recommendation from display
- [ ] Show confirmation message

**Requirements**: 6.2

---

### Task 5.3: Integrate Disable Ritual Handling

**File**: `src/components/RitualRecommendation.tsx`

**Changes**:
- [ ] Import UserPreferenceService
- [ ] Handle "Disable Ritual" button click
- [ ] Call `disableRitual()` API
- [ ] Stop showing recommendations for this ritual
- [ ] Show confirmation message

**Requirements**: 6.3

---

### Task 5.4: Create Dismissal History UI (Optional)

**File**: `src/components/DismissalHistory.tsx`

**Responsibilities**:
- Display list of dismissed rituals
- Show dismissal count for each ritual
- Allow re-enabling rituals
- Show dismissal history

**Requirements**: 6.3

---

## Phase 6: Testing and Validation

### Task 6.1: Write Unit Tests for Services

**Files**:
- `src/services/__tests__/actionTrackingService.test.ts`
- `src/services/__tests__/recommendationService.test.ts`
- `src/services/__tests__/userPreferenceService.test.ts`

**Test Coverage**:
- [ ] ActionTrackingService: action tracking, sequence management, session ID
- [ ] RecommendationService: API calls, caching, error handling
- [ ] UserPreferenceService: dismissal tracking, preference management

**Requirements**: All frontend services

---

### Task 6.2: Write Component Tests

**Files**:
- `src/components/__tests__/RitualRecommendation.test.tsx`
- `src/components/__tests__/CarouselSlider.test.tsx`

**Test Coverage**:
- [ ] RitualRecommendation: rendering, user interactions, callbacks
- [ ] CarouselSlider: navigation, responsiveness, accessibility

**Requirements**: 2.1, 2.2, 2.3

---

### Task 6.3: Integration Testing

**File**: `src/__tests__/integration/ritualRecommendation.integration.test.ts`

**Test Scenarios**:
- [ ] User views product → action tracked → recommendation generated
- [ ] User adds to cart → recommendation updated
- [ ] User dismisses recommendation → not shown again
- [ ] User disables ritual → no recommendations for that ritual
- [ ] Real-time updates via SignalR

**Requirements**: All integration requirements

---

## Phase 7: Deployment and Documentation

### Task 7.1: Create Frontend Implementation Documentation

**File**: `src/docs/RITUAL_RECOMMENDATION_FRONTEND.md`

**Content**:
- [ ] Architecture overview
- [ ] Service descriptions
- [ ] Component descriptions
- [ ] Integration points
- [ ] Configuration guide
- [ ] Troubleshooting guide

**Requirements**: All frontend requirements

---

### Task 7.2: Update README with Frontend Setup

**File**: `README.md`

**Changes**:
- [ ] Add frontend setup instructions
- [ ] Add environment variables needed
- [ ] Add how to run frontend
- [ ] Add troubleshooting section

**Requirements**: All frontend requirements

---

## Implementation Checklist

### Before Starting
- [ ] Review design document
- [ ] Review requirements document
- [ ] Understand action tracking flow
- [ ] Understand recommendation flow
- [ ] Understand user preference flow

### Phase 1: Action Tracking
- [ ] Task 1.1: Create ActionTrackingService
- [ ] Task 1.2: Integrate into HomePage
- [ ] Task 1.3: Integrate into ServicePage
- [ ] Task 1.4: Integrate into ProductPage
- [ ] Task 1.5: Integrate into CommunityPage

### Phase 2: Recommendation Service
- [ ] Task 2.1: Create RecommendationService
- [ ] Task 2.2: Integrate with ActionTrackingService

### Phase 3: UI Components
- [ ] Task 3.1: Create RitualRecommendation component
- [ ] Task 3.2: Create CarouselSlider component (if needed)

### Phase 4: CartPage Integration
- [ ] Task 4.1: Integrate into CartPage
- [ ] Task 4.2: Handle real-time updates with SignalR

### Phase 5: User Preferences
- [ ] Task 5.1: Create UserPreferenceService
- [ ] Task 5.2: Integrate dismissal handling
- [ ] Task 5.3: Integrate disable ritual handling
- [ ] Task 5.4: Create dismissal history UI (optional)

### Phase 6: Testing
- [ ] Task 6.1: Write unit tests
- [ ] Task 6.2: Write component tests
- [ ] Task 6.3: Integration testing

### Phase 7: Documentation
- [ ] Task 7.1: Create documentation
- [ ] Task 7.2: Update README

---

## Key Configuration

### Environment Variables
```
VITE_API_BASE_URL=http://localhost:5000
VITE_GEMINI_API_KEY=<your-gemini-api-key>
VITE_SIGNALR_HUB_URL=http://localhost:5000/hubs/realtime
```

### Cache Configuration
- Cache TTL: 15 minutes
- Max actions per session: 10
- Recommendation timeout: 5 seconds

### SignalR Configuration
- Hub URL: `/hubs/realtime`
- Events: `RecommendationUpdated`, `PreferenceUpdated`

---

## Notes

1. **Action Tracking**: Capture actions on HomePage, ServicePage, ProductPage, TaggedProduct (CommunityPage)
2. **Recommendations Display**: Show in CartPage as carousel slider (Section 3)
3. **Caching**: 15 minutes TTL
4. **Max Actions**: 10 per session
5. **Real-time**: Use SignalR for updates
6. **Error Handling**: Silent fail (no error display to user)
7. **Dismissals**: Remember per session only
8. **Re-enable**: Users cannot re-enable disabled rituals
9. **History**: Dismissal history is visible to users

---

**Status**: Ready for Implementation  
**Next Step**: Start with Phase 1 - Action Tracking Service

# Frontend - Backend Compatibility Check

**Date**: December 21, 2025  
**Status**: ⚠️ **PARTIALLY COMPATIBLE** - FE chưa hoàn thiện, BE sẵn sàng

---

## 📊 Tình Trạng Hiện Tại

### Backend (BE-AI) ✅
- ✅ Tất cả services đã implement
- ✅ Tất cả API endpoints đã tạo
- ✅ Database entities đã tạo
- ✅ Ritual manifest đã cấu hình
- ✅ Sẵn sàng nhận request từ FE

### Frontend (FE-AI) ❌
- ⚠️ ActionTrackingService: Bắt đầu nhưng chưa hoàn thiện (có lỗi import)
- ❌ RecommendationService: Chưa tạo
- ❌ RitualRecommendation component: Chưa tạo
- ❌ CarouselSlider component: Chưa tạo (chỉ có UI carousel)
- ❌ UserPreferenceService: Chưa tạo
- ❌ Integration vào pages: Chưa làm

---

## 🔍 Chi Tiết Kiểm Tra

### 1. ActionTrackingService ⚠️

**Status**: Bắt đầu nhưng chưa hoàn thiện

**Vấn đề**:
```
❌ Cannot find module 'uuid'
❌ Cannot find namespace 'NodeJS'
```

**Cần sửa**:
1. Cài đặt package `uuid`
2. Cài đặt type definitions `@types/node`
3. Hoặc thay thế bằng `crypto.randomUUID()`

**Tính năng đã có**:
- ✅ trackAction() method
- ✅ getActionSequence() method
- ✅ clearActionSequence() method
- ✅ getSessionId() method
- ✅ Session storage
- ✅ Debouncing
- ✅ Listeners/subscribers

**Tính năng cần thêm**:
- ❌ Integration vào HomePage
- ❌ Integration vào ServicePage
- ❌ Integration vào ProductPage
- ❌ Integration vào CommunityPage
- ❌ Integration vào CartPage

---

### 2. RecommendationService ❌

**Status**: Chưa tạo

**Cần implement**:
```typescript
class RecommendationService {
  // Call BE-AI endpoint
  async callBeAiAnalysis(actionSequence: Action[]): Promise<RecommendationPayload>
  
  // Call FE-AI endpoint
  async callFeAiExplanation(payload: RecommendationPayload): Promise<ExplanationPayload>
  
  // Caching
  getCachedRecommendation(): RecommendationPayload | null
  clearCachedRecommendation(): void
}
```

**API Endpoints cần gọi**:
- `POST /api/recommendations/analyze` (BE-AI)
- `POST /api/explanations/generate` (FE-AI)

**Cấu hình**:
- Cache TTL: 15 minutes
- Timeout: 5 seconds
- Retry: 3 times with exponential backoff
- Error handling: Silent fail

---

### 3. RitualRecommendation Component ❌

**Status**: Chưa tạo

**Cần implement**:
```typescript
interface RitualRecommendationProps {
  recommendation: RecommendationPayload | null
  explanation: ExplanationPayload | null
  isLoading: boolean
  error: string | null
  onDismiss: () => void
  onDisableRitual: () => void
  onAddToCart: (productId: string) => void
}

export const RitualRecommendation: React.FC<RitualRecommendationProps> = (props) => {
  // Display:
  // - Ritual name with confidence score badge
  // - Cultural explanation section
  // - Missing items as carousel slider
  // - Item cards with image, name, price, reason, "Add to Cart" button
  // - "Dismiss" button
  // - "Disable Ritual" button
}
```

**Tính năng**:
- Display ritual name & confidence score
- Show cultural explanation
- List missing items with reasons
- Add to cart buttons
- Dismiss/disable actions
- Loading state
- Error state
- Empty state

---

### 4. CarouselSlider Component ⚠️

**Status**: Có UI carousel nhưng chưa customize cho ritual items

**Hiện tại**:
- ✅ Có `src/components/ui/carousel.tsx` (embla-carousel)
- ❌ Chưa tạo `RitualCarouselSlider` component

**Cần tạo**:
```typescript
interface RitualCarouselSliderProps {
  items: Product[]
  onAddToCart: (productId: string) => void
}

export const RitualCarouselSlider: React.FC<RitualCarouselSliderProps> = (props) => {
  // Display items in carousel
  // Support touch/mouse navigation
  // Responsive design
  // Smooth transitions
}
```

---

### 5. UserPreferenceService ❌

**Status**: Chưa tạo

**Cần implement**:
```typescript
class UserPreferenceService {
  // Record dismissal
  async recordDismissal(ritualId: string): Promise<void>
  
  // Disable ritual
  async disableRitual(ritualId: string): Promise<void>
  
  // Check if disabled
  isRitualDisabled(ritualId: string): boolean
  
  // Get disabled rituals
  getDisabledRituals(): string[]
  
  // Get dismissal history
  getDismissalHistory(): DismissalRecord[]
}
```

**API Endpoints cần gọi**:
- `POST /api/preferences/dismiss-ritual`
- `POST /api/preferences/disable-ritual`

**Storage**:
- Session storage (per session only)
- No persistence across sessions

---

### 6. Page Integration ❌

**Status**: Chưa làm

**Cần integrate vào**:
1. **HomePage** - Track "BrowseCategory" events
2. **ServicePage** - Track "BrowseCategory" events
3. **ProductPage** - Track "ViewProduct" and "AddToCart" events
4. **CommunityPage** - Track "ViewProduct" events
5. **CartPage** - Display recommendations, track "AddToCart" events

**Cách integrate**:
```typescript
// Trong component
const actionTracking = getActionTrackingService();

// Track action
actionTracking.trackAction('ViewProduct', {}, productId, categoryId);

// Get sequence
const sequence = actionTracking.getActionSequence();

// Subscribe to changes
const unsubscribe = actionTracking.subscribe(() => {
  // Re-analyze recommendations
});
```

---

## 🔗 BE-FE Integration Points

### API Endpoints

#### 1. BE-AI Analysis Endpoint
```
POST /api/recommendations/analyze

Request:
{
  userId: string
  sessionId: string
  actionSequence: Action[]
}

Response:
{
  matched: boolean
  ritualId: string
  ritualName: string
  confidenceScore: number (0-1)
  missingItems: Product[]
  matchingMetadata: {
    matchedSequenceLength: number
    totalSequenceLength: number
    matchedActionIndices: number[]
  }
  systemReport: {
    matchedPattern: ActionType[]
    matchingSteps: string[]
    reasonsForMissingItems: { productId: string, reason: string }[]
  }
}
```

#### 2. FE-AI Explanation Endpoint
```
POST /api/explanations/generate

Request:
{
  ritualId: string
  ritualName: string
  confidenceScore: number
  missingItems: Product[]
  matchingMetadata: object
  systemReport: object
}

Response:
{
  ritualName: string
  culturalContext: string
  itemExplanations: {
    productId: string
    productName: string
    whyNeeded: string
    traditionalUsage: string
  }[]
  sources: string[]
  generatedBy: "gemini" | "fallback"
}
```

#### 3. User Preference Endpoints
```
POST /api/preferences/dismiss-ritual
Request: { userId: string, ritualId: string }
Response: { success: boolean }

POST /api/preferences/disable-ritual
Request: { userId: string, sessionId: string, ritualId: string }
Response: { success: boolean }
```

### Data Flow

```
User Action (HomePage, ProductPage, etc.)
    ↓
ActionTrackingService.trackAction()
    ↓
Action stored in session storage
    ↓
RecommendationService.callBeAiAnalysis()
    ↓
POST /api/recommendations/analyze
    ↓
BE-AI returns RecommendationPayload
    ↓
RecommendationService.callFeAiExplanation()
    ↓
POST /api/explanations/generate
    ↓
FE-AI returns ExplanationPayload
    ↓
RitualRecommendation component displays
    ↓
User can dismiss or disable ritual
    ↓
UserPreferenceService records preference
    ↓
POST /api/preferences/dismiss-ritual or disable-ritual
```

---

## ✅ Compatibility Checklist

### Backend Ready ✅
- [x] All DTOs defined
- [x] All services implemented
- [x] All API endpoints created
- [x] Database entities created
- [x] Ritual manifest configured
- [x] Error handling implemented
- [x] Logging implemented

### Frontend Needed ❌
- [ ] Fix ActionTrackingService imports
- [ ] Create RecommendationService
- [ ] Create RitualRecommendation component
- [ ] Create RitualCarouselSlider component
- [ ] Create UserPreferenceService
- [ ] Integrate into HomePage
- [ ] Integrate into ServicePage
- [ ] Integrate into ProductPage
- [ ] Integrate into CommunityPage
- [ ] Integrate into CartPage
- [ ] Implement SignalR integration
- [ ] Write tests

---

## 🚀 Next Steps

### Immediate (Fix ActionTrackingService)
1. Install dependencies:
   ```bash
   npm install uuid
   npm install --save-dev @types/node
   ```

2. Or replace uuid with crypto:
   ```typescript
   // Replace: import { v4 as uuidv4 } from 'uuid';
   // With: const uuidv4 = () => crypto.randomUUID();
   ```

### Phase 1: Complete ActionTrackingService
- [ ] Fix import errors
- [ ] Test trackAction() method
- [ ] Test getActionSequence() method
- [ ] Integrate into HomePage
- [ ] Integrate into ServicePage
- [ ] Integrate into ProductPage
- [ ] Integrate into CommunityPage
- [ ] Integrate into CartPage

### Phase 2: Create RecommendationService
- [ ] Create service class
- [ ] Implement callBeAiAnalysis()
- [ ] Implement callFeAiExplanation()
- [ ] Implement caching
- [ ] Implement error handling
- [ ] Test with backend

### Phase 3: Create UI Components
- [ ] Create RitualRecommendation component
- [ ] Create RitualCarouselSlider component
- [ ] Integrate into CartPage
- [ ] Test rendering

### Phase 4: User Preferences
- [ ] Create UserPreferenceService
- [ ] Implement dismissal handling
- [ ] Implement disable ritual handling
- [ ] Create dismissal history UI

### Phase 5: Real-time Updates
- [ ] Implement SignalR integration
- [ ] Test real-time updates

### Phase 6: Testing & Documentation
- [ ] Write unit tests
- [ ] Write component tests
- [ ] Write integration tests
- [ ] Create documentation

---

## 📋 Estimated Effort

| Task | Effort | Status |
|------|--------|--------|
| Fix ActionTrackingService | 30 min | ⏳ Pending |
| Create RecommendationService | 2 hours | ❌ Not Started |
| Create UI Components | 3 hours | ❌ Not Started |
| Page Integration | 2 hours | ❌ Not Started |
| User Preferences | 1.5 hours | ❌ Not Started |
| Real-time Updates | 1 hour | ❌ Not Started |
| Testing | 2 hours | ❌ Not Started |
| Documentation | 1 hour | ❌ Not Started |
| **Total** | **~13 hours** | **⏳ Ready** |

---

## 🎯 Recommendation

**FE và BE chưa hoàn toàn phù hợp vì:**

1. ✅ BE đã sẵn sàng (85% complete)
2. ❌ FE chưa hoàn thiện (0% complete)
3. ⚠️ ActionTrackingService có lỗi import cần sửa

**Hành động tiếp theo:**
1. Sửa lỗi import trong ActionTrackingService
2. Hoàn thiện FE implementation theo plan
3. Test integration giữa FE và BE
4. Deploy khi tất cả đều ready

---

**Status**: ⚠️ PARTIALLY COMPATIBLE  
**Last Updated**: December 21, 2025  
**Prepared By**: Kiro AI Assistant

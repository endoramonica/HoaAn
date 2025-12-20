# Frontend Implementation Guide: Sequential Ritual Recommendation System

## Overview

This guide outlines all frontend jobs required to integrate the Sequential Ritual Recommendation System into the VietCommerce application. The frontend is responsible for:

1. **Action Tracking**: Capturing user interactions and sending them to BE-AI
2. **Recommendation Display**: Showing recommendations with explanations to users
3. **User Preferences**: Handling dismissals and ritual disabling
4. **Real-time Updates**: Updating recommendations as users interact with products

---

## Frontend Architecture

```
┌─────────────────────────────────────────────────────────────┐
│ FRONTEND LAYER                                              │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ 1. ACTION TRACKING SERVICE                           │  │
│  │    - Capture user interactions                       │  │
│  │    - Send to BE-AI API                               │  │
│  │    - Manage session state                            │  │
│  └──────────────────────────────────────────────────────┘  │
│                          ↓                                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ 2. RECOMMENDATION SERVICE                            │  │
│  │    - Call BE-AI /api/recommendations/analyze         │  │
│  │    - Call FE-AI /api/explanations/generate           │  │
│  │    - Cache recommendations                           │  │
│  └──────────────────────────────────────────────────────┘  │
│                          ↓                                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ 3. RECOMMENDATION UI COMPONENT                       │  │
│  │    - Display ritual name & confidence score          │  │
│  │    - Show cultural explanation                       │  │
│  │    - List missing items with reasons                 │  │
│  │    - Handle user interactions                        │  │
│  └──────────────────────────────────────────────────────┘  │
│                          ↓                                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ 4. USER PREFERENCE SERVICE                           │  │
│  │    - Track dismissals                                │  │
│  │    - Disable rituals                                 │  │
│  │    - Persist preferences                             │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Frontend Jobs Checklist

### Phase 1: Action Tracking Service

**File**: `src/services/actionTrackingService.ts`

**Responsibilities**:
- [ ] Create ActionTrackingService class
- [ ] Implement trackAction() method
- [ ] Implement getActionSequence() method
- [ ] Implement clearActionSequence() method
- [ ] Handle session management
- [ ] Store actions in local state/storage

**Key Methods**:
```typescript
class ActionTrackingService {
  // Track a user action
  trackAction(actionType: string, metadata?: any): void
  
  // Get current action sequence
  getActionSequence(): Action[]
  
  // Clear sequence (on navigation)
  clearActionSequence(): void
  
  // Get session ID
  getSessionId(): string
}
```

**Integration Points**:
- Hook into product view events
- Hook into add-to-cart events
- Hook into category browse events
- Hook into navigation events

---

### Phase 2: Recommendation Service

**File**: `src/services/recommendationService.ts`

**Responsibilities**:
- [ ] Create RecommendationService class
- [ ] Implement callBeAiAnalysis() method
- [ ] Implement callFeAiExplanation() method
- [ ] Handle API errors and retries
- [ ] Cache recommendations
- [ ] Manage recommendation state

**Key Methods**:
```typescript
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
}
```

**API Endpoints**:
- `POST /api/recommendations/analyze` - BE-AI analysis
- `POST /api/explanations/generate` - FE-AI explanation

**Error Handling**:
- Timeout handling (5 second timeout)
- Retry logic with exponential backoff
- Fallback to cached data
- User-friendly error messages

---

### Phase 3: Recommendation UI Component

**File**: `src/components/RitualRecommendation.tsx`

**Responsibilities**:
- [ ] Create RitualRecommendation component
- [ ] Display ritual name and confidence score
- [ ] Show cultural explanation
- [ ] List missing items with reasons
- [ ] Add "Add to Cart" buttons for missing items
- [ ] Handle dismiss action
- [ ] Handle disable ritual action
- [ ] Show loading state
- [ ] Show error state
- [ ] Show empty state (no recommendation)

**Component Structure**:
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
  // Component implementation
}
```

**UI Elements**:
- Ritual name header
- Confidence score badge
- Cultural context section
- Missing items list
- Item explanation cards
- Action buttons (Add to Cart, Dismiss, Disable)
- Sources/references section

**Styling**:
- Responsive design (mobile, tablet, desktop)
- Calm aesthetic (per requirements)
- Accessible color contrast
- Clear typography hierarchy

---

### Phase 4: User Preference Service

**File**: `src/services/userPreferenceService.ts`

**Responsibilities**:
- [ ] Create UserPreferenceService class
- [ ] Implement recordDismissal() method
- [ ] Implement disableRitual() method
- [ ] Implement isRitualDisabled() method
- [ ] Persist preferences to backend
- [ ] Manage local preference state

**Key Methods**:
```typescript
class UserPreferenceService {
  // Record user dismissal
  async recordDismissal(ritualId: string): Promise<void>
  
  // Disable ritual for current session
  async disableRitual(ritualId: string): Promise<void>
  
  // Check if ritual is disabled
  isRitualDisabled(ritualId: string): boolean
  
  // Get all disabled rituals
  getDisabledRituals(): string[]
  
  // Clear all preferences
  clearPreferences(): void
}
```

**API Endpoints**:
- `POST /api/preferences/dismiss-ritual` - Record dismissal
- `POST /api/preferences/disable-ritual` - Disable ritual

**Storage**:
- Session storage for disabled rituals
- Backend persistence for dismissals
- Local cache for performance

---

### Phase 5: Integration with Existing Components

**Product Page Integration**:
- [ ] Hook ActionTrackingService into product view
- [ ] Trigger recommendation analysis on product view
- [ ] Display RitualRecommendation component

**Cart Page Integration**:
- [ ] Hook ActionTrackingService into add-to-cart
- [ ] Re-analyze recommendations on cart update
- [ ] Update missing items list

**Category Page Integration**:
- [ ] Hook ActionTrackingService into category browse
- [ ] Trigger recommendation analysis on category view

**Navigation Integration**:
- [ ] Clear recommendations on navigation away
- [ ] Clear action sequence on page change
- [ ] Reset session state

---

### Phase 6: State Management

**File**: `src/store/ritualRecommendationSlice.ts` (if using Redux)

**State Structure**:
```typescript
interface RitualRecommendationState {
  // Recommendation data
  recommendation: RecommendationPayload | null
  explanation: ExplanationPayload | null
  
  // UI state
  isLoading: boolean
  error: string | null
  isVisible: boolean
  
  // User preferences
  dismissedRituals: string[]
  disabledRituals: string[]
  
  // Session data
  sessionId: string
  actionSequence: Action[]
}
```

**Actions**:
- `setRecommendation(payload)` - Set recommendation
- `setExplanation(payload)` - Set explanation
- `setLoading(isLoading)` - Set loading state
- `setError(error)` - Set error state
- `dismissRitual(ritualId)` - Dismiss ritual
- `disableRitual(ritualId)` - Disable ritual
- `clearRecommendation()` - Clear recommendation
- `addAction(action)` - Add action to sequence
- `clearActionSequence()` - Clear action sequence

---

### Phase 7: Real-time Updates

**Responsibilities**:
- [ ] Implement debounced recommendation analysis
- [ ] Update recommendations on cart changes
- [ ] Update recommendations on product view
- [ ] Handle concurrent requests
- [ ] Prevent duplicate API calls

**Implementation**:
- Use debounce (500ms) for action tracking
- Use throttle (1s) for recommendation updates
- Cancel previous requests on new action
- Queue requests if API is busy

---

### Phase 8: Error Handling & Fallbacks

**Responsibilities**:
- [ ] Handle API timeouts
- [ ] Handle network errors
- [ ] Handle invalid responses
- [ ] Show user-friendly error messages
- [ ] Provide fallback UI

**Error Scenarios**:
- BE-AI API timeout → Show cached recommendation or nothing
- FE-AI API timeout → Show fallback explanation
- Network error → Show retry button
- Invalid response → Log error and show generic message
- Missing data → Show partial recommendation

---

### Phase 9: Performance Optimization

**Responsibilities**:
- [ ] Implement recommendation caching
- [ ] Lazy load explanation component
- [ ] Optimize re-renders
- [ ] Minimize API calls
- [ ] Optimize bundle size

**Techniques**:
- Memoize components (React.memo)
- Use useCallback for event handlers
- Debounce action tracking
- Cache recommendations in localStorage
- Lazy load Gemini explanation

---

### Phase 10: Testing

**Unit Tests**:
- [ ] Test ActionTrackingService
- [ ] Test RecommendationService
- [ ] Test UserPreferenceService
- [ ] Test RitualRecommendation component
- [ ] Test error handling

**Integration Tests**:
- [ ] Test end-to-end flow
- [ ] Test API integration
- [ ] Test state management
- [ ] Test user interactions

**E2E Tests**:
- [ ] Test complete user journey
- [ ] Test with real API
- [ ] Test error scenarios
- [ ] Test performance

---

### Phase 11: Accessibility

**Responsibilities**:
- [ ] Ensure WCAG 2.1 AA compliance
- [ ] Add ARIA labels
- [ ] Ensure keyboard navigation
- [ ] Test with screen readers
- [ ] Ensure color contrast

**Checklist**:
- [ ] All interactive elements are keyboard accessible
- [ ] ARIA labels for all buttons
- [ ] Semantic HTML structure
- [ ] Color contrast ratio ≥ 4.5:1
- [ ] Focus indicators visible
- [ ] Screen reader tested

---

### Phase 12: Documentation

**Responsibilities**:
- [ ] Document ActionTrackingService API
- [ ] Document RecommendationService API
- [ ] Document RitualRecommendation component props
- [ ] Document UserPreferenceService API
- [ ] Create integration guide for developers
- [ ] Create user guide for end users

**Documentation Files**:
- [ ] API documentation
- [ ] Component documentation
- [ ] Integration guide
- [ ] Troubleshooting guide
- [ ] FAQ

---

## Frontend File Structure

```
src/
├── services/
│   ├── actionTrackingService.ts
│   ├── recommendationService.ts
│   └── userPreferenceService.ts
├── components/
│   ├── RitualRecommendation.tsx
│   ├── RitualRecommendation.module.css
│   ├── RecommendationCard.tsx
│   ├── ItemExplanation.tsx
│   └── ConfidenceScore.tsx
├── store/
│   └── ritualRecommendationSlice.ts (if using Redux)
├── hooks/
│   ├── useRitualRecommendation.ts
│   ├── useActionTracking.ts
│   └── useUserPreferences.ts
├── types/
│   ├── ritual.ts
│   ├── recommendation.ts
│   └── explanation.ts
└── utils/
    ├── apiClient.ts
    └── errorHandler.ts
```

---

## Frontend Dependencies

**Required Packages**:
- `react` - UI framework
- `axios` or `fetch` - HTTP client
- `zustand` or `redux` - State management
- `react-query` - Data fetching
- `lodash` - Utility functions (debounce, throttle)

**Optional Packages**:
- `react-i18n` - Internationalization
- `react-testing-library` - Testing
- `jest` - Test runner
- `cypress` - E2E testing

---

## Frontend Environment Variables

**`.env.local`**:
```
VITE_API_BASE_URL=http://localhost:5000/api
VITE_GEMINI_API_KEY=your_gemini_api_key
VITE_SESSION_TIMEOUT=30
VITE_DEBOUNCE_DELAY=500
VITE_RECOMMENDATION_CACHE_TTL=300
```

---

## Frontend Integration Checklist

### Setup Phase
- [ ] Create service files
- [ ] Create component files
- [ ] Set up state management
- [ ] Configure API client
- [ ] Set up environment variables

### Implementation Phase
- [ ] Implement ActionTrackingService
- [ ] Implement RecommendationService
- [ ] Implement UserPreferenceService
- [ ] Implement RitualRecommendation component
- [ ] Integrate with existing components
- [ ] Implement error handling
- [ ] Implement caching

### Testing Phase
- [ ] Write unit tests
- [ ] Write integration tests
- [ ] Write E2E tests
- [ ] Test error scenarios
- [ ] Test performance
- [ ] Test accessibility

### Optimization Phase
- [ ] Optimize bundle size
- [ ] Optimize re-renders
- [ ] Optimize API calls
- [ ] Implement caching
- [ ] Lazy load components

### Documentation Phase
- [ ] Document APIs
- [ ] Document components
- [ ] Create integration guide
- [ ] Create user guide
- [ ] Create troubleshooting guide

### Deployment Phase
- [ ] Code review
- [ ] QA testing
- [ ] Performance testing
- [ ] Security review
- [ ] Deploy to staging
- [ ] Deploy to production

---

## Frontend Success Criteria

✅ Action tracking works correctly
✅ Recommendations display properly
✅ Explanations are generated
✅ User preferences are respected
✅ Error handling works
✅ Performance is acceptable
✅ Accessibility is compliant
✅ Tests pass
✅ Documentation is complete
✅ Users are satisfied

---

## Frontend Support & Troubleshooting

### Common Issues

**Issue**: Recommendations not showing
- Check if action sequence is being tracked
- Check if BE-AI API is responding
- Check browser console for errors
- Verify API endpoint is correct

**Issue**: Explanations not generating
- Check if FE-AI API is responding
- Check Gemini API key is valid
- Check network connectivity
- Verify API endpoint is correct

**Issue**: Performance is slow
- Check if debounce is working
- Check if caching is enabled
- Check API response times
- Profile React components

**Issue**: Accessibility issues
- Check ARIA labels
- Check keyboard navigation
- Check color contrast
- Test with screen reader

---

## Related Documentation

- [BE-AI Integration Guide](./BE_AI_INTEGRATION_GUIDE.md)
- [FE-AI Integration Guide](./FE_AI_INTEGRATION_GUIDE.md)
- [API Documentation](./API_DOCUMENTATION.md)
- [Developer Guide](./DEVELOPER_GUIDE_RITUAL_PATTERNS.md)
- [Design Document](./design.md)
- [Requirements Document](./requirements.md)


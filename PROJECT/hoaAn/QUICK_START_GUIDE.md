# Sequential Ritual Recommendation System - Quick Start Guide

**Status**: ✅ Ready for Implementation  
**Last Updated**: December 21, 2025

---

## 🎯 What You Need to Know

### System Overview
The Sequential Ritual Recommendation System detects Vietnamese cultural rituals through user behavior and suggests missing ritual items. It has two main components:

1. **BE-AI (Backend)**: Analyzes user actions and matches them against ritual patterns
2. **FE-AI (Frontend)**: Displays recommendations with cultural explanations

### Current Status
- ✅ **Backend**: ~85% complete (core logic done)
- ❌ **Frontend**: 0% complete (ready to start)
- ✅ **Documentation**: 100% complete

---

## 📋 Key Information You Provided

### Rituals to Support (5 total)
1. Tết (Lunar New Year)
2. Tết Nguyên Tiêu (Lantern Festival)
3. Đầy Tháng (Baby's First Month)
4. Cổ Truyền (Traditional Customs)
5. Ông Công Ông Táo (Kitchen Gods)

### Action Tracking Pages
- HomePage
- ServicePage
- ProductPage
- TaggedProduct (in CommunityPage)
- CartPage

### Recommendation Display
- **Location**: CartPage (Section 3)
- **Format**: Carousel slider
- **Cache**: 15 minutes
- **Max Actions**: 10 per session

### User Preferences
- **Dismissals**: Per session only
- **Disabled Rituals**: Per session only
- **Re-enable**: Not allowed
- **History**: Visible to users

### Real-time Updates
- **Technology**: SignalR (already in workspace)
- **Hub**: `/hubs/realtime`

---

## 📁 Important Files

### Specification Documents
```
FE_readdoc/ecommerceHoaAn/.kiro/specs/
├── requirements.md                          # What the system should do
├── sequential-ritual-recommendation/
│   └── design.md                           # How it should work
├── tasks.md                                # Implementation tasks (52 total)
├── FRONTEND_IMPLEMENTATION_TASKS.md        # Detailed frontend tasks
├── FRONTEND_IMPLEMENTATION_GUIDE.md        # Frontend architecture
└── BE_AI_INTEGRATION_GUIDE.md             # Backend architecture
```

### Backend Files (Already Created)
```
BackEnd/VietCommerce.Api/
├── Controllers/
│   ├── RecommendationController.cs         # BE-AI endpoint
│   ├── ExplanationController.cs            # FE-AI endpoint
│   └── UserPreferenceController.cs         # Preferences endpoints
├── Services/
│   ├── RecommendationService.cs
│   ├── GeminiExplanationService.cs
│   ├── ActionTrackingService.cs
│   └── UserPreferenceService.cs
└── wwwroot/data/
    └── ritual-manifest.json                # Ritual patterns (5 rituals)
```

### Frontend Files (To Be Created)
```
FE_readdoc/ecommerceHoaAn/src/
├── services/
│   ├── actionTrackingService.ts            # Track user actions
│   ├── recommendationService.ts            # Call BE-AI & FE-AI
│   └── userPreferenceService.ts            # Handle preferences
├── components/
│   ├── RitualRecommendation.tsx            # Display recommendations
│   ├── CarouselSlider.tsx                  # Carousel for items
│   └── DismissalHistory.tsx                # Show history
└── pages/
    ├── HomePage.tsx                        # Add action tracking
    ├── ServicePage.tsx                     # Add action tracking
    ├── ProductPage.tsx                     # Add action tracking
    ├── CommunityPage.tsx                   # Add action tracking
    └── CartPage.tsx                        # Add recommendations
```

---

## 🚀 How to Get Started

### Step 1: Review the Specification
1. Read `requirements.md` to understand what the system should do
2. Read `design.md` to understand how it works
3. Read `FRONTEND_IMPLEMENTATION_GUIDE.md` for architecture

### Step 2: Understand the Flow
```
User Action (HomePage, ProductPage, etc.)
    ↓
ActionTrackingService captures action
    ↓
RecommendationService calls BE-AI
    ↓
BE-AI analyzes action sequence
    ↓
BE-AI returns recommendation payload
    ↓
RecommendationService calls FE-AI (Gemini)
    ↓
FE-AI returns explanation payload
    ↓
RitualRecommendation component displays in CartPage
    ↓
User can dismiss or disable ritual
```

### Step 3: Start Implementation
Follow the tasks in order:

**Phase 1: Action Tracking (Tasks 27-31)**
- Create ActionTrackingService
- Integrate into 5 pages

**Phase 2: Recommendation Service (Tasks 32-33)**
- Create RecommendationService
- Integrate with ActionTrackingService

**Phase 3: UI Components (Tasks 34-36)**
- Create RitualRecommendation component
- Create CarouselSlider component
- Integrate into CartPage

**Phase 4: Real-time & Preferences (Tasks 37-41)**
- Implement SignalR integration
- Implement user preferences

**Phase 5: Testing (Tasks 43-48)**
- Write unit tests
- Write integration tests

**Phase 6: Documentation (Tasks 50-51)**
- Create documentation
- Update README

---

## 🔧 API Endpoints

### Backend Endpoints (Already Created)

**1. Analyze Action Sequence (BE-AI)**
```
POST /api/recommendations/analyze
Content-Type: application/json

{
  "actionSequence": [
    {
      "type": "ViewProduct",
      "productId": "123",
      "categoryId": "456",
      "timestamp": 1234567890,
      "metadata": {}
    },
    ...
  ]
}

Response:
{
  "ritualId": "tet",
  "ritualName": "Tết Nguyên Đán",
  "confidenceScore": 0.85,
  "missingItems": [
    {
      "id": "product-123",
      "name": "Hoa Đào",
      "price": 50000,
      "categoryId": "flowers"
    },
    ...
  ],
  "matchingMetadata": {...},
  "systemReport": "..."
}
```

**2. Generate Explanation (FE-AI)**
```
POST /api/explanations/generate
Content-Type: application/json

{
  "ritualId": "tet",
  "ritualName": "Tết Nguyên Đán",
  "confidenceScore": 0.85,
  "missingItems": [...],
  "matchingMetadata": {...},
  "systemReport": "..."
}

Response:
{
  "ritualName": "Tết Nguyên Đán",
  "culturalContext": "Tết Nguyên Đán là lễ hội truyền thống...",
  "itemExplanations": {
    "product-123": "Hoa Đào là biểu tượng của..."
  },
  "sources": ["Vietnamese Lunar New Year Traditions"],
  "generatedBy": "Gemini API"
}
```

**3. Record Dismissal**
```
POST /api/preferences/dismiss-ritual
Content-Type: application/json

{
  "ritualId": "tet"
}
```

**4. Disable Ritual**
```
POST /api/preferences/disable-ritual
Content-Type: application/json

{
  "ritualId": "tet"
}
```

---

## 📊 Task Summary

### Total Tasks: 52
- **Backend**: 26 tasks (mostly complete)
- **Frontend**: 26 tasks (ready to start)

### Task Breakdown by Phase

| Phase | Tasks | Status | Effort |
|-------|-------|--------|--------|
| 1. Core Data Models | 1-3 | ✅ Complete | - |
| 2. Pattern Matching | 4-8 | ✅ Complete | - |
| 3. Gemini Integration | 9-11 | ✅ Complete | - |
| 4. Action Tracking | 12-15 | ✅ Complete | - |
| 5. Pattern Prioritization | 16-17 | ✅ Complete | - |
| 6. Integration Tests | 20-23 | ✅ Complete | - |
| 7. Documentation | 24-26 | ✅ Complete | - |
| **8. Frontend Implementation** | **27-41** | **❌ Not Started** | **12-16h** |
| 9. Frontend Testing | 43-48 | ❌ Not Started | 2-3h |
| 10. Documentation | 50-51 | ❌ Not Started | 1-2h |

---

## 💡 Key Implementation Tips

### 1. Action Tracking
- Use a simple array to store actions in session state
- Clear actions on page navigation
- Send to backend when user reaches CartPage

### 2. Recommendation Service
- Implement caching with 15-minute TTL
- Use exponential backoff for retries (max 3)
- Silent fail on errors (don't show error to user)
- 5-second timeout for API calls

### 3. UI Components
- Use existing design system for consistency
- Make carousel responsive (mobile/tablet/desktop)
- Smooth animations for better UX
- Show loading state while fetching

### 4. Real-time Updates
- Use existing SignalR connection from workspace
- Listen for `RecommendationUpdated` events
- Update component state on new recommendations

### 5. User Preferences
- Store dismissals in session storage (per session)
- Call backend API to persist
- Don't allow re-enabling disabled rituals
- Show dismissal history to users

---

## 🧪 Testing Strategy

### Unit Tests
- Test each service independently
- Mock API calls
- Test error handling

### Component Tests
- Test rendering
- Test user interactions
- Test callbacks

### Integration Tests
- Test end-to-end flow
- Test with real API calls
- Test real-time updates

---

## 📞 Quick Reference

### Environment Variables
```
VITE_API_BASE_URL=http://localhost:5000
VITE_GEMINI_API_KEY=<your-gemini-api-key>
VITE_SIGNALR_HUB_URL=http://localhost:5000/hubs/realtime
```

### Ritual Manifest Location
```
BackEnd/VietCommerce.Api/wwwroot/data/ritual-manifest.json
```

### SignalR Hub
```
URL: /hubs/realtime
Events: RecommendationUpdated, PreferenceUpdated
```

### Cache Configuration
- TTL: 15 minutes
- Max actions: 10 per session
- Timeout: 5 seconds

---

## ✅ Verification Checklist

Before starting implementation, verify:

- [ ] Backend is running and accessible
- [ ] Gemini API key is configured
- [ ] SignalR hub is available
- [ ] ritual-manifest.json has correct product IDs
- [ ] Database migrations are applied
- [ ] All specification documents are reviewed

---

## 🎯 Success Criteria

The system is complete when:

1. ✅ Action tracking works on all 5 pages
2. ✅ Recommendations display in CartPage
3. ✅ Real-time updates work via SignalR
4. ✅ User preferences work correctly
5. ✅ All tests pass
6. ✅ Documentation is complete
7. ✅ End-to-end flow works

---

## 📚 Additional Resources

### Documentation Files
- `SEQUENTIAL_RITUAL_STATUS_REPORT.md` - Detailed status report
- `SEQUENTIAL_RITUAL_IMPLEMENTATION_SUMMARY.md` - Complete summary
- `FRONTEND_IMPLEMENTATION_TASKS.md` - Detailed frontend tasks

### Specification Files
- `requirements.md` - Full requirements
- `design.md` - Complete design
- `tasks.md` - All 52 tasks

### Guide Files
- `FRONTEND_IMPLEMENTATION_GUIDE.md` - Frontend architecture
- `BE_AI_INTEGRATION_GUIDE.md` - Backend architecture
- `DEVELOPER_GUIDE_RITUAL_PATTERNS.md` - How to add rituals

---

## 🚀 Ready to Start?

1. **Review** this quick start guide
2. **Read** the specification documents
3. **Open** `tasks.md` in your IDE
4. **Start** with Task 27: Create ActionTrackingService
5. **Follow** the sequential order
6. **Test** each phase before moving to the next

---

**Good luck! 🎉**

For detailed information, see:
- `SEQUENTIAL_RITUAL_IMPLEMENTATION_SUMMARY.md` - Complete overview
- `FRONTEND_IMPLEMENTATION_TASKS.md` - Detailed frontend tasks
- `FE_readdoc/ecommerceHoaAn/.kiro/specs/tasks.md` - All tasks

**Last Updated**: December 21, 2025

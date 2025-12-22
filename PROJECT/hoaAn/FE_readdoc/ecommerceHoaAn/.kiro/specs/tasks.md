# Implementation Plan: Sequential Ritual Recommendation System

## Overview
This implementation plan converts the Sequential Ritual Recommendation System design into actionable coding tasks. Tasks are sequenced to build incrementally, with core functionality implemented first, followed by testing and integration.

---

## Phase 1: Core Data Models and Entities

- [x] 1. Create data models and DTOs for ritual system



  - Create `RitualDto.cs` with ritual pattern structure (id, name, actionSequencePattern, requiredItems, confidenceThreshold, culturalSignificance, sources)
  - Create `ActionTypeDto.cs` for action type definitions
  - Create `ActionDto.cs` for user action tracking (userId, sessionId, type, timestamp, productId, categoryId, metadata)
  - Create `RecommendationPayloadDto.cs` for BE-AI output (ritualId, ritualName, confidenceScore, missingItems, matchingMetadata, systemReport)
  - Create `ExplanationPayloadDto.cs` for FE-AI output (ritualName, culturalContext, itemExplanations, sources, generatedBy)
  - Create `MatchResultDto.cs` for pattern matching results
  - _Requirements: 7.1, 7.2, 7.3_

- [x]* 1.1 Write property test for DTO serialization round-trip

  - **Property 1: DTO Serialization Round Trip**
  - **Validates: Requirements 7.1**

- [x] 2. Create database entities for ritual tracking




  - Create `RitualEntity.cs` in VietCommerce.Core/Entities
  - Create `ActionEntity.cs` for tracking user actions
  - Create `RitualDismissalEntity.cs` for tracking user dismissals
  - Create `RecommendationLogEntity.cs` for logging recommendations
  - Add relationships and foreign keys
  - _Requirements: 3.1, 3.2, 3.3_

- [ ]* 2.1 Write unit tests for entity relationships
  - Test RitualEntity structure and properties
  - Test ActionEntity timestamp and metadata
  - Test RitualDismissalEntity foreign key relationships
  - _Requirements: 3.1, 3.2, 3.3_

- [x] 3. Create database migrations for ritual entities





  - Add migration for RitualEntity, ActionEntity, RitualDismissalEntity, RecommendationLogEntity
  - Update AppDBContext to include DbSets for new entities
  - _Requirements: 3.1, 3.5_

---

## Phase 2: Backend Pattern Matching Engine (BE-AI)

- [x] 4. Create Ritual Manifest loader and data structure






  - Create `RitualManifest.cs` class to hold all ritual patterns
  - Create `RitualManifestLoader.cs` to load manifest from JSON file
  - Create sample `ritual-manifest.json` with 3-5 Vietnamese rituals (Đầy Tháng, Tết, Lễ Cúng Tổ Tiên, etc.)
  - Implement manifest validation (all required items exist in catalog)
  - _Requirements: 7.1, 7.2, 7.3_

- [ ]* 4.1 Write property test for manifest loading
  - **Property 24: Ritual Manifest Loading**
  - **Validates: Requirements 7.1**

- [ ]* 4.2 Write property test for manifest structure
  - **Property 25: Action Sequence Storage in Manifest**
  - **Property 26: Required Items Specification**
  - **Validates: Requirements 7.2, 7.3**

- [x] 5. Implement sequential pattern matching algorithm




  - Create `ISequentialPatternMatcher.cs` interface
  - Create `SequentialPatternMatcher.cs` implementing PrefixSpan-inspired algorithm
  - Implement `MatchPattern(actionSequence)` method that:
    - Iterates through all rituals in manifest
    - Matches action sequence against ritual patterns
    - Calculates confidence score based on matched actions
    - Returns MatchResult with matched ritual and confidence
  - _Requirements: 7.4_

- [ ]* 5.1 Write property test for pattern matching
  - **Property 27: PrefixSpan-Inspired Matching Algorithm**
  - **Validates: Requirements 7.4**


- [ ]* 5.2 Write property test for pattern matching accuracy
  - **Property 2: Ritual Type Identification Accuracy**
  - **Validates: Requirements 1.2**

- [x] 6. Implement recommendation generation service





  - Create `IRecommendationService.cs` interface
  - Create `RecommendationService.cs` implementing:
    - `GenerateRecommendation(actionSequence, cartItems)` method
    - `GetMissingItems(ritual, cartItems)` method to identify missing items
    - `LogRecommendation(payload)` method for audit logging
  - Return RecommendationPayload with all required fields
  - _Requirements: 1.3, 1.4, 4.1, 4.2, 4.3, 4.4, 4.5_

- [ ]* 6.1 Write property test for missing items identification
  - **Property 3: Ritual Requirements Comparison**
  - **Property 4: Missing Items Catalog Lookup**
  - **Validates: Requirements 1.3, 1.4**

- [ ]* 6.2 Write property test for recommendation logging
  - **Property 14: Recommendation Logging Completeness**
  - **Property 15: Action Metadata Logging**
  - **Property 16: Intermediate Matching Steps Recording**
  - **Property 17: System Report Inclusion**
  - **Property 18: Failure Logging**
  - **Validates: Requirements 4.1, 4.2, 4.3, 4.4, 4.5**


- [x] 7. Create BE-AI API endpoint




  - Create `RecommendationController.cs` in VietCommerce.Api/Controllers
  - Implement `POST /api/recommendations/analyze` endpoint that:
    - Accepts action sequence from frontend
    - Calls RecommendationService
    - Returns RecommendationPayload
  - Add error handling for invalid sequences
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_

- [ ]* 7.1 Write property test for no recommendations on non-matching sequences
  - **Property 5: No Recommendations for Non-Matching Sequences**
  - **Validates: Requirements 1.5**


- [x] 8. Checkpoint - Ensure all BE-AI tests pass




  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 2.5: Ritual Manifest Data Verification

- [ ] 8.5 Verify and update ritual-manifest.json with actual product IDs
  - Location: `BackEnd/VietCommerce.Api/wwwroot/data/ritual-manifest.json`
  - Verify product IDs for each ritual match actual catalog
  - Rituals to include: Tết, Tết Nguyên Tiêu, Đầy Tháng, Cổ Truyền, Ông Công Ông Táo
  - Update categoryIds and productIds with real data from database
  - _Requirements: 7.1, 7.2, 7.3_

---

## Phase 3: Frontend Explanation Engine (FE-AI with Gemini)


- [x] 9. Create Gemini explanation service



  - Create `IGeminiExplanationService.cs` interface
  - Create `GeminiExplanationService.cs` implementing:
    - `GenerateExplanation(payload)` method that calls Gemini API
    - `GetFallbackExplanation(payload)` method for fallback template
    - Proper error handling and retry logic
  - Configure Gemini API key from environment variable (VITE_GEMINI_API_KEY)
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [ ]* 9.1 Write property test for Gemini API invocation
  - **Property 29: Gemini API Invocation**
  - **Property 30: Gemini Context Completeness**
  - **Validates: Requirements 8.1, 8.2**

- [ ]* 9.2 Write property test for explanation payload formatting
  - **Property 31: Explanation Payload Formatting**
  - **Property 32: Item Reason Inclusion in Explanations**
  - **Validates: Requirements 8.3, 8.4**

- [ ]* 9.3 Write property test for Gemini API fallback
  - **Property 33: Gemini API Fallback**
  - **Validates: Requirements 8.5**

- [x] 10. Create FE-AI API endpoint




  - Create `ExplanationController.cs` in VietCommerce.Api/Controllers
  - Implement `POST /api/explanations/generate` endpoint that:
    - Accepts RecommendationPayload from frontend
    - Calls GeminiExplanationService
    - Returns ExplanationPayload
  - Add error handling and fallback logic
  - _Requirements: 2.1, 2.2, 2.3, 2.5_

- [ ]* 10.1 Write property test for explanation inclusion
  - **Property 6: Recommendation Includes Explanation**
  - **Property 7: Explanation Contains Required Information**
  - **Property 8: Confidence Score Presence**
  - **Property 9: Recommendations Include Cultural References**
  - **Validates: Requirements 2.1, 2.2, 2.3, 2.5**

- [x] 11. Checkpoint - Ensure all FE-AI tests pass





  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 4: Action Tracking and Session Management

- [x] 12. Create action tracking service





  - Create `IActionTrackingService.cs` interface
  - Create `ActionTrackingService.cs` implementing:
    - `TrackAction(userId, sessionId, actionType, metadata)` method
    - `GetActionSequence(userId, sessionId)` method to retrieve recent actions
    - `ClearActionSequence(sessionId)` method to reset on navigation
  - Store actions in database via ActionRepository
  - _Requirements: 1.1, 5.3, 5.4_

- [ ]* 12.1 Write property test for action sequence analysis
  - **Property 1: Pattern Matching Completeness**
  - **Validates: Requirements 1.1**

- [ ]* 12.2 Write property test for re-analysis on cart update
  - **Property 19: Re-analysis on Cart Update**
  - **Validates: Requirements 5.3**

- [ ]* 12.3 Write property test for recommendations cleared on navigation
  - **Property 20: Recommendations Cleared on Navigation**
  - **Validates: Requirements 5.4**

- [] 13. Create dismissal and preference tracking service





  - Create `IUserPreferenceService.cs` interface
  - Create `UserPreferenceService.cs` implementing:
    - `RecordDismissal(userId, ritualId)` method
    - `DisableRitual(userId, sessionId, ritualId)` method
    - `IsRitualDisabled(userId, sessionId, ritualId)` method
    - `GetDismissalCount(userId, ritualId)` method
  - Store dismissals in RitualDismissalEntity
  - _Requirements: 6.2, 6.3_

- [ ]* 13.1 Write property test for dismissal recording
  - **Property 22: Dismissal Recording**
  - **Validates: Requirements 6.2**

- [ ]* 13.2 Write property test for ritual detection disabling
  - **Property 23: Ritual Detection Disabling**
  - **Validates: Requirements 6.3**




- [x] 14. Create API endpoint for user preferences

  - Create `UserPreferenceController.cs` in VietCommerce.Api/Controllers




  - Implement `POST /api/preferences/dismiss-ritual` endpoint
  - Implement `POST /api/preferences/disable-ritual` endpoint
  - _Requirements: 6.2, 6.3_





- [x] 15. Checkpoint - Ensure all action tracking tests pass

  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 5: Pattern Prioritization and Filtering





- [x] 16. Implement pattern prioritization logic

  - Update RecommendationService to handle multiple matching patterns




  - Implement logic to select highest confidence pattern

  - Filter out disabled rituals based on user preferences
  - _Requirements: 5.5, 6.3_

- [ ]* 16.1 Write property test for highest confidence prioritization
  - **Property 21: Highest Confidence Pattern Prioritization**
  - **Validates: Requirements 5.5**





- [x] 17. Implement confidence threshold filtering

  - Update SequentialPatternMatcher to respect confidence thresholds
  - Ensure patterns below threshold are not matched
  - _Requirements: 1.2, 3.2_


- [ ]* 17.1 Write property test for confidence threshold
  - **Property 11: Confidence Threshold Assignment**
  - **Validates: Requirements 3.2**

- [x] 18. Implement pattern storage format optimization

  - Update RitualManifest to index patterns by action types
  - Optimize SequentialPatternMatcher for efficient lookup
  - _Requirements: 3.5, 7.5_

- [ ]* 18.1 Write property test for pattern storage efficiency
  - **Property 13: Pattern Storage Format Efficiency**
  - **Property 28: Recommendation Payload Structure**
  - **Validates: Requirements 3.5, 7.5**

- [x] 19. Checkpoint - Ensure all prioritization tests pass

  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 6: Integration and End-to-End Testing


- [x] 20. Create integration test suite



  - Create `SequentialRitualRecommendationIntegrationTests.cs`
  - Test end-to-end flow: Action → BE-AI → FE-AI → Display
  - Test with multiple concurrent user sessions
  - Test with various ritual patterns and action sequences
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 2.1, 2.2, 2.3, 2.5_

- [ ]* 20.1 Write integration test for complete recommendation flow
  - Test action tracking → pattern matching → explanation generation
  - Verify all data flows correctly through the system
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_

- [ ]* 20.2 Write integration test for Gemini API integration
  - Test with mock Gemini responses
  - Test fallback behavior on API failure
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [x] 21. Create sample ritual manifest with Vietnamese rituals




  - Add Đầy Tháng (1-month celebration) ritual
  - Add Tết (Lunar New Year) ritual
  - Add Lễ Cúng Tổ Tiên (Ancestor worship) ritual
  - Add Lễ Cúng Thần Tài (Wealth god worship) ritual
  - Include action sequences and required items for each
  - _Requirements: 7.1, 7.2, 7.3_




- [ ] 22. Create sample product catalog entries for ritual items


  - Add products for each ritual (Mâm Cúng, Heo Quay, Bộ Tam Sên, Ngũ Quả, etc.)

  - Ensure all required items in manifest exist in catalog
  - _Requirements: 1.4, 3.3_

- [x] 23. Checkpoint - Ensure all integration tests pass






  - Ensure all tests pass, ask the user if questions arise.


---

## Phase 7: Documentation and Deployment

- [ ] 24. Create API documentation


  
  - Document `POST /api/recommendations/analyze` endpoint
  - Document `POST /api/explanations/generate` endpoint
  - Document `POST /api/preferences/dismiss-ritual` endpoint
  - Document `POST /api/preferences/disable-ritual` endpoint
  - Include request/response examples
  - _Requirements: 1.1, 2.1, 6.2, 6.3_


- [x] 25. Create developer guide for ritual pattern definition




  - Document how to add new rituals to manifest
  - Document action sequence syntax
  - Document required items specification
  - Document confidence threshold tuning
  - Document include what fe have to do 
  - _Requirements: 3.1, 3.2, 3.3, 7.1, 7.2, 7.3_

- [x] 26. Final checkpoint - fe jobs
  - Created FRONTEND_IMPLEMENTATION_GUIDE.md with comprehensive frontend implementation guide
  - Created FRONTEND_JOBS_BREAKDOWN.md with detailed breakdown of all 12 frontend jobs
  - Created FRONTEND_QUICK_START.md with quick reference guide for developers
  - Documented all frontend responsibilities and integration points
  - Provided code examples and best practices
  - _Requirements: All frontend integration requirements_ 

---

## Phase 8: Frontend Implementation (Action Tracking & Recommendation Display)

- [ ] 27. Create ActionTrackingService
  - File: `src/services/actionTrackingService.ts`
  - Implement trackAction() method to capture user interactions
  - Implement getActionSequence() to retrieve recent actions
  - Implement clearActionSequence() for navigation
  - Implement getSessionId() for session management
  - Store actions in session state
  - _Requirements: 1.1, 5.3, 5.4_

- [ ] 28. Integrate action tracking into HomePage
  - File: `src/pages/HomePage.tsx`
  - Track "BrowseCategory" events
  - Track "ViewProduct" events for featured products
  - Initialize ActionTrackingService on mount
  - _Requirements: 1.1, 5.3_

- [ ] 29. Integrate action tracking into ServicePage
  - File: `src/pages/ServicePage.tsx`
  - Track "BrowseCategory" events
  - Track "ViewProduct" events
  - Initialize ActionTrackingService on mount
  - _Requirements: 1.1, 5.3_

- [ ] 30. Integrate action tracking into ProductPage
  - File: `src/pages/ProductPage.tsx`
  - Track "ViewProduct" when product details load
  - Track "AddToCart" when user adds to cart
  - Track "BrowseCategory" for related products
  - Initialize ActionTrackingService on mount
  - _Requirements: 1.1, 5.3_

- [ ] 31. Integrate action tracking into CommunityPage (TaggedProduct)
  - File: `src/pages/CommunityPage.tsx` or `src/components/TaggedProduct.tsx`
  - Track "ViewProduct" for tagged products
  - Track "AddToCart" for tagged products
  - Initialize ActionTrackingService on mount
  - _Requirements: 1.1, 5.3_

- [ ] 32. Create RecommendationService
  - File: `src/services/recommendationService.ts`
  - Implement callBeAiAnalysis() to call `POST /api/recommendations/analyze`
  - Implement callFeAiExplanation() to call `POST /api/explanations/generate`
  - Implement caching with 15-minute TTL
  - Implement error handling with retry logic (max 3 retries)
  - Implement timeout handling (5 second timeout)
  - Silent fail on errors (no user-facing error display)
  - _Requirements: 1.3, 1.4, 2.1, 2.2, 2.3_

- [ ] 33. Integrate RecommendationService with ActionTrackingService
  - File: `src/services/recommendationService.ts`
  - Listen to action tracking events
  - Trigger recommendation analysis on action sequence changes
  - Implement real-time analysis (not batch)
  - Use SignalR for real-time updates
  - _Requirements: 1.1, 5.3_

- [ ] 34. Create RitualRecommendation component
  - File: `src/components/RitualRecommendation.tsx`
  - Display ritual name with confidence score badge
  - Display cultural explanation section
  - Display missing items as carousel slider
  - Show item cards with: image, name, price, reason, "Add to Cart" button
  - Implement "Dismiss" button
  - Implement "Disable Ritual" button
  - Responsive design for mobile/tablet/desktop
  - Smooth carousel animations
  - _Requirements: 2.1, 2.2, 2.3, 2.5_

- [ ] 35. Create CarouselSlider component (if not exists)
  - File: `src/components/CarouselSlider.tsx`
  - Support touch/mouse navigation
  - Auto-scroll capability
  - Responsive to screen size
  - Smooth transitions
  - _Requirements: 2.1_

- [ ] 36. Integrate RitualRecommendation into CartPage
  - File: `src/pages/CartPage.tsx`
  - Add recommendation section below cart items (Section 3)
  - Display as carousel slider
  - Handle "Add to Cart" for recommended items
  - Handle dismiss and disable actions
  - Show loading state while fetching
  - Silent fail on errors
  - _Requirements: 2.1, 2.2, 2.3, 2.5_

- [ ] 37. Implement real-time updates with SignalR
  - File: `src/pages/CartPage.tsx`
  - Connect to SignalR hub for real-time updates
  - Listen for recommendation update events
  - Update UI when new recommendations arrive
  - Handle connection errors gracefully
  - _Requirements: 1.1, 5.3_

- [ ] 38. Create UserPreferenceService
  - File: `src/services/userPreferenceService.ts`
  - Implement recordDismissal() to call `POST /api/preferences/dismiss-ritual`
  - Implement disableRitual() to call `POST /api/preferences/disable-ritual`
  - Implement isRitualDisabled() to check if ritual is disabled
  - Implement getDismissalCount() to get dismissal count
  - Implement getDismissalHistory() to get history
  - Store preferences in session storage (per session only)
  - _Requirements: 6.2, 6.3_

- [ ] 39. Integrate dismissal handling
  - File: `src/components/RitualRecommendation.tsx`
  - Handle "Dismiss" button click
  - Call recordDismissal() API
  - Remove recommendation from display
  - Show confirmation message
  - _Requirements: 6.2_

- [ ] 40. Integrate disable ritual handling
  - File: `src/components/RitualRecommendation.tsx`
  - Handle "Disable Ritual" button click
  - Call disableRitual() API
  - Stop showing recommendations for this ritual
  - Show confirmation message
  - _Requirements: 6.3_

- [ ] 41. Create dismissal history UI
  - File: `src/components/DismissalHistory.tsx`
  - Display list of dismissed rituals
  - Show dismissal count for each ritual
  - Display dismissal history
  - _Requirements: 6.3_

- [ ] 42. Checkpoint - Ensure all frontend services work
  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 9: Frontend Testing

- [ ]* 43. Write unit tests for ActionTrackingService
  - File: `src/services/__tests__/actionTrackingService.test.ts`
  - Test action tracking functionality
  - Test sequence management
  - Test session ID generation
  - _Requirements: 1.1, 5.3_

- [ ]* 44. Write unit tests for RecommendationService
  - File: `src/services/__tests__/recommendationService.test.ts`
  - Test API calls
  - Test caching logic
  - Test error handling and retries
  - Test timeout handling
  - _Requirements: 1.3, 1.4, 2.1_

- [ ]* 45. Write unit tests for UserPreferenceService
  - File: `src/services/__tests__/userPreferenceService.test.ts`
  - Test dismissal recording
  - Test ritual disabling
  - Test preference retrieval
  - _Requirements: 6.2, 6.3_

- [ ]* 46. Write component tests for RitualRecommendation
  - File: `src/components/__tests__/RitualRecommendation.test.tsx`
  - Test rendering
  - Test user interactions
  - Test callbacks
  - _Requirements: 2.1, 2.2, 2.3_

- [ ]* 47. Write component tests for CarouselSlider
  - File: `src/components/__tests__/CarouselSlider.test.tsx`
  - Test navigation
  - Test responsiveness
  - Test accessibility
  - _Requirements: 2.1_

- [ ]* 48. Write integration tests for ritual recommendation flow
  - File: `src/__tests__/integration/ritualRecommendation.integration.test.ts`
  - Test end-to-end flow: Action → BE-AI → FE-AI → Display
  - Test with multiple concurrent user sessions
  - Test real-time updates
  - _Requirements: All integration requirements_

- [ ] 49. Checkpoint - Ensure all frontend tests pass
  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 10: Frontend Documentation and Deployment

- [ ] 50. Create frontend implementation documentation
  - File: `src/docs/RITUAL_RECOMMENDATION_FRONTEND.md`
  - Document architecture overview
  - Document service descriptions
  - Document component descriptions
  - Document integration points
  - Document configuration guide
  - Document troubleshooting guide
  - _Requirements: All frontend requirements_

- [ ] 51. Update README with frontend setup
  - File: `README.md`
  - Add frontend setup instructions
  - Add environment variables needed
  - Add how to run frontend
  - Add troubleshooting section
  - _Requirements: All frontend requirements_

- [ ] 52. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.
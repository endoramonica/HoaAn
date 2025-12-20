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


# Design Document: Sequential Ritual Recommendation System

## Overview

The Sequential Ritual Recommendation System is an intelligent recommendation engine that detects Vietnamese cultural rituals through user behavior analysis and suggests missing ritual items. The system operates in a dual-AI architecture:

- **BE-AI (Backend Strategist)**: Defines ritual patterns, performs sequential pattern matching, and returns structured recommendation data
- **FE-AI (Frontend Advisor)**: Calls Gemini API to generate natural language explanations and cultural context

The system uses a PrefixSpan-inspired algorithm to match user action sequences against predefined ritual patterns, then leverages Gemini API to create explainable, culturally-informed recommendations.

## Architecture

### High-Level Flow

```
User Action → Action Tracker → BE-AI Pattern Matcher → Recommendation Payload
                                                              ↓
                                                        FE-AI (Gemini Call)
                                                              ↓
                                                        Explanation Payload
                                                              ↓
                                                        Display to User
```

### Component Layers

1. **Action Tracking Layer** (Frontend)
   - Captures user interactions (view product, add to cart, browse category)
   - Sends action sequence to BE-AI

2. **Pattern Matching Layer** (Backend)
   - Loads Ritual Manifest on startup
   - Implements PrefixSpan-inspired sequential pattern matching
   - Returns Recommendation Payload with matched ritual and missing items

3. **Explanation Layer** (Frontend)
   - Receives Recommendation Payload from BE-AI
   - Calls Gemini API with ritual context
   - Formats response as Explanation Payload

4. **Presentation Layer** (Frontend)
   - Displays recommendations with explanations
   - Handles user interactions (dismiss, request details)

## Components and Interfaces

### BE-AI Components

#### 1. RitualManifest (Data Structure)
```
RitualManifest {
  rituals: Ritual[]
}

Ritual {
  id: string
  name: string (e.g., "Đầy Tháng", "Tết", "Lễ Cúng Tổ Tiên")
  actionSequencePattern: ActionType[]
  requiredItems: {
    categoryId: string
    productIds: string[]
  }[]
  confidenceThreshold: number (0-1)
  culturalSignificance: string
  sources: string[]
}

ActionType {
  type: string (e.g., "ViewProduct", "AddToCart", "BrowseCategory")
  productCategoryId?: string
  metadata?: object
}
```

#### 2. SequentialPatternMatcher (Service)
```
Interface ISequentialPatternMatcher {
  LoadManifest(manifestPath: string): void
  MatchPattern(actionSequence: Action[]): MatchResult
  GetRitualById(ritualId: string): Ritual
}

MatchResult {
  matched: boolean
  ritualId: string
  ritualName: string
  confidenceScore: number (0-1)
  matchedActions: Action[]
  missingItems: Product[]
  matchingMetadata: {
    matchedSequenceLength: number
    totalSequenceLength: number
    matchedActionIndices: number[]
  }
}
```

#### 3. RecommendationService (Service)
```
Interface IRecommendationService {
  GenerateRecommendation(actionSequence: Action[], cartItems: CartItem[]): RecommendationPayload
  GetMissingItems(ritual: Ritual, cartItems: CartItem[]): Product[]
  LogRecommendation(payload: RecommendationPayload): void
}

RecommendationPayload {
  ritualId: string
  ritualName: string
  confidenceScore: number
  missingItems: Product[]
  matchingMetadata: object
  systemReport: {
    matchedPattern: ActionType[]
    matchingSteps: string[]
    reasonsForMissingItems: { productId: string, reason: string }[]
  }
}
```

### FE-AI Components

#### 1. GeminiExplanationService (Service)
```
Interface IGeminiExplanationService {
  GenerateExplanation(payload: RecommendationPayload): Promise<ExplanationPayload>
  GetFallbackExplanation(payload: RecommendationPayload): ExplanationPayload
}

ExplanationPayload {
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

#### 2. RecommendationUIService (Service)
```
Interface IRecommendationUIService {
  DisplayRecommendation(payload: RecommendationPayload, explanation: ExplanationPayload): void
  HandleDismissal(ritualId: string): void
  HandleDisableRitual(ritualId: string): void
  ClearRecommendations(): void
}
```

## Data Models

### Action Entity
```
Action {
  id: string
  userId: string
  sessionId: string
  type: ActionType
  timestamp: DateTime
  productId?: string
  categoryId?: string
  metadata?: object
}
```

### RitualDismissal Entity
```
RitualDismissal {
  id: string
  userId: string
  ritualId: string
  dismissedAt: DateTime
  reason?: string
}
```

### RecommendationLog Entity
```
RecommendationLog {
  id: string
  userId: string
  sessionId: string
  ritualId: string
  confidenceScore: number
  missingItems: string[] (productIds)
  matchingMetadata: object
  displayedAt: DateTime
  userInteraction: "viewed" | "dismissed" | "clicked" | null
}
```

## Correctness Properties

A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.

### Property 1: Pattern Matching Completeness
*For any* action sequence and ritual manifest, the system SHALL analyze the sequence against all patterns without errors or exceptions.
**Validates: Requirements 1.1**

### Property 2: Ritual Type Identification Accuracy
*For any* action sequence that matches a ritual pattern above the confidence threshold, the system SHALL identify the correct ritual type.
**Validates: Requirements 1.2**

### Property 3: Ritual Requirements Comparison
*For any* detected ritual and user cart, the system SHALL compare ritual requirements against cart items and identify all missing items.
**Validates: Requirements 1.3**

### Property 4: Missing Items Catalog Lookup
*For any* set of missing items identified by the system, all items SHALL exist in the product catalog.
**Validates: Requirements 1.4**

### Property 5: No Recommendations for Non-Matching Sequences
*For any* action sequence that does not match any ritual pattern above the confidence threshold, the system SHALL not generate recommendations.
**Validates: Requirements 1.5**

### Property 6: Recommendation Includes Explanation
*For any* recommendation generated, the system SHALL provide a human-readable explanation of the ritual context.
**Validates: Requirements 2.1**

### Property 7: Explanation Contains Required Information
*For any* explanation provided, it SHALL include ritual name, cultural significance, and reasons why each item is traditionally required.
**Validates: Requirements 2.2**

### Property 8: Confidence Score Presence
*For any* recommendation displayed, a confidence score (0-1) SHALL be included.
**Validates: Requirements 2.3**

### Property 9: Recommendations Include Cultural References
*For any* recommendation displayed, it SHALL include citations or cultural references supporting the recommendations.
**Validates: Requirements 2.5**

### Property 10: Ritual Pattern Storage Structure
*For any* ritual pattern stored in the manifest, it SHALL include action sequence and required products in a structured format.
**Validates: Requirements 3.1**

### Property 11: Confidence Threshold Assignment
*For any* ritual created, a confidence threshold value SHALL be assigned.
**Validates: Requirements 3.2**

### Property 12: Ritual Item Validation
*For any* ritual updated with required items, all items SHALL exist in the product catalog.
**Validates: Requirements 3.3**

### Property 13: Pattern Storage Format Efficiency
*For any* ritual patterns stored, they SHALL be persisted in a format that supports efficient sequential matching (e.g., indexed by action types).
**Validates: Requirements 3.5**

### Property 14: Recommendation Logging Completeness
*For any* recommendation generated, the system SHALL log the matched pattern, confidence score, and reasoning.
**Validates: Requirements 4.1**

### Property 15: Action Metadata Logging
*For any* recommendation generated, the system SHALL include metadata about which actions triggered the pattern match.
**Validates: Requirements 4.2**

### Property 16: Intermediate Matching Steps Recording
*For any* action sequence analyzed, the system SHALL record intermediate matching steps for debugging.
**Validates: Requirements 4.3**

### Property 17: System Report Inclusion
*For any* recommendation displayed to users, a system report SHALL be included showing the matching logic.
**Validates: Requirements 4.4**

### Property 18: Failure Logging
*For any* action sequence that does not match any pattern, the system SHALL log why the sequence did not match.
**Validates: Requirements 4.5**

### Property 19: Re-analysis on Cart Update
*For any* item added to cart, the system SHALL re-analyze the action sequence and update recommendations if ritual context changes.
**Validates: Requirements 5.3**

### Property 20: Recommendations Cleared on Navigation
*For any* user navigation away from ritual-related products, recommendations SHALL be cleared and pattern detection reset.
**Validates: Requirements 5.4**

### Property 21: Highest Confidence Pattern Prioritization
*For any* action sequence matching multiple ritual patterns, the system SHALL prioritize the pattern with the highest confidence score.
**Validates: Requirements 5.5**

### Property 22: Dismissal Recording
*For any* user dismissal of a recommendation, the system SHALL record the dismissal and reduce future recommendations of that type.
**Validates: Requirements 6.2**

### Property 23: Ritual Detection Disabling
*For any* user indication of disinterest in a ritual, the system SHALL disable pattern detection for that ritual in the current session.
**Validates: Requirements 6.3**

### Property 24: Ritual Manifest Loading
*For any* system startup, the BE-AI SHALL load the Ritual Manifest containing all ritual patterns, action sequences, and required items.
**Validates: Requirements 7.1**

### Property 25: Action Sequence Storage in Manifest
*For any* ritual pattern defined, the manifest SHALL include action type sequences.
**Validates: Requirements 7.2**

### Property 26: Required Items Specification
*For any* ritual configured, the manifest SHALL specify required product categories and specific items.
**Validates: Requirements 7.3**

### Property 27: PrefixSpan-Inspired Matching Algorithm
*For any* action sequence received by BE-AI, the system SHALL match it against patterns using PrefixSpan-inspired algorithm.
**Validates: Requirements 7.4**

### Property 28: Recommendation Payload Structure
*For any* pattern match, the BE-AI SHALL return a Recommendation Payload containing ritual name, missing items, confidence score, and matching metadata.
**Validates: Requirements 7.5**

### Property 29: Gemini API Invocation
*For any* Recommendation Payload received by FE-AI, the system SHALL call Gemini API with ritual context and missing items.
**Validates: Requirements 8.1**

### Property 30: Gemini Context Completeness
*For any* Gemini API call, the system SHALL provide ritual name, cultural significance, and list of missing items as context.
**Validates: Requirements 8.2**

### Property 31: Explanation Payload Formatting
*For any* Gemini response, the FE-AI SHALL format it as an Explanation Payload with human-readable text.
**Validates: Requirements 8.3**

### Property 32: Item Reason Inclusion in Explanations
*For any* Explanation Payload generated, it SHALL include why each item is traditionally required for the ritual.
**Validates: Requirements 8.4**

### Property 33: Gemini API Fallback
*For any* Gemini API failure, the FE-AI SHALL fall back to a predefined template explanation without blocking recommendation display.
**Validates: Requirements 8.5**

## Error Handling

### Pattern Matching Errors
- **Invalid Action Sequence**: Log error and skip pattern matching
- **Manifest Load Failure**: Prevent system startup and alert administrator
- **No Matching Pattern**: Return empty recommendation (no error)

### Gemini API Errors
- **API Timeout**: Use fallback template explanation
- **API Rate Limit**: Queue request and retry with exponential backoff
- **Invalid Response**: Log error and use fallback template

### Data Validation Errors
- **Missing Required Items in Catalog**: Log warning and exclude from recommendations
- **Invalid Ritual Configuration**: Prevent ritual from being loaded into manifest
- **Corrupted Action Data**: Log error and skip action in sequence analysis

## Testing Strategy

### Unit Testing
- Test SequentialPatternMatcher with predefined action sequences and ritual patterns
- Test RecommendationService missing item identification logic
- Test GeminiExplanationService fallback behavior
- Test RitualDismissal and DisabledRitual tracking

### Property-Based Testing
- **Property 1-33**: Each property SHALL be implemented as a separate property-based test
- Use xUnit with custom generators for:
  - Random action sequences
  - Random ritual manifests
  - Random cart states
  - Random Gemini API responses
- Minimum 100 iterations per property test
- Tag each test with format: `**Feature: sequential-ritual-recommendation, Property {number}: {property_text}**`

### Integration Testing
- Test end-to-end flow: Action → BE-AI → FE-AI → Display
- Test Gemini API integration with mock responses
- Test database persistence of recommendations and dismissals
- Test concurrent user sessions with different rituals

### Test Coverage Goals
- Core pattern matching logic: 100% coverage
- Recommendation generation: 100% coverage
- Error handling paths: 100% coverage
- Gemini integration: 95% coverage (excluding external API)


# Frontend Quick Start Guide: Sequential Ritual Recommendation System

## Overview

This is a quick reference guide for frontend developers implementing the Sequential Ritual Recommendation System. For detailed information, see the [Frontend Implementation Guide](./FRONTEND_IMPLEMENTATION_GUIDE.md) and [Frontend Jobs Breakdown](./FRONTEND_JOBS_BREAKDOWN.md).

---

## 5-Minute Setup

### 1. Create Services

**`src/services/actionTrackingService.ts`**:
```typescript
export interface Action {
  type: string
  productId?: string
  categoryId?: string
  timestamp: Date
  metadata?: any
}

export class ActionTrackingService {
  private actions: Action[] = []
  private sessionId: string

  constructor() {
    this.sessionId = this.getOrCreateSessionId()
  }

  trackAction(type: string, metadata?: any): void {
    this.actions.push({
      type,
      timestamp: new Date(),
      metadata
    })
  }

  getActionSequence(): Action[] {
    return this.actions.slice(-10)
  }

  clearActionSequence(): void {
    this.actions = []
  }

  private getOrCreateSessionId(): string {
    let sessionId = localStorage.getItem('sessionId')
    if (!sessionId) {
      sessionId = `session_${Date.now()}`
      localStorage.setItem('sessionId', sessionId)
    }
    return sessionId
  }
}
```

**`src/services/recommendationService.ts`**:
```typescript
export class RecommendationService {
  private apiBase = process.env.VITE_API_BASE_URL

  async callBeAiAnalysis(actionSequence: Action[]): Promise<any> {
    const response = await fetch(`${this.apiBase}/recommendations/analyze`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ actionSequence })
    })
    return response.json()
  }

  async callFeAiExplanation(payload: any): Promise<any> {
    const response = await fetch(`${this.apiBase}/explanations/generate`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })
    return response.json()
  }
}
```

### 2. Create Component

**`src/components/RitualRecommendation.tsx`**:
```typescript
import React, { useState, useEffect } from 'react'
import { RecommendationService } from '../services/recommendationService'

export const RitualRecommendation: React.FC<{ recommendation: any }> = ({ recommendation }) => {
  if (!recommendation) return null

  return (
    <div className="ritual-recommendation">
      <h3>{recommendation.ritualName}</h3>
      <p>Confidence: {(recommendation.confidenceScore * 100).toFixed(0)}%</p>
      <p>{recommendation.explanation?.culturalContext}</p>
      
      <div className="missing-items">
        {recommendation.missingItems?.map((item: any) => (
          <div key={item.id} className="item">
            <h4>{item.name}</h4>
            <button>Add to Cart</button>
          </div>
        ))}
      </div>
    </div>
  )
}
```

### 3. Integrate with Product Page

**`src/pages/ProductDetail.tsx`**:
```typescript
import { useEffect, useState } from 'react'
import { ActionTrackingService } from '../services/actionTrackingService'
import { RecommendationService } from '../services/recommendationService'
import { RitualRecommendation } from '../components/RitualRecommendation'

export const ProductDetail = () => {
  const [recommendation, setRecommendation] = useState(null)
  const actionTracker = new ActionTrackingService()
  const recommendationService = new RecommendationService()

  useEffect(() => {
    // Track product view
    actionTracker.trackAction('ViewProduct', { productId: 'product-123' })

    // Get recommendations
    const analyze = async () => {
      const payload = await recommendationService.callBeAiAnalysis(
        actionTracker.getActionSequence()
      )
      if (payload.matched) {
        const explanation = await recommendationService.callFeAiExplanation(payload)
        setRecommendation({ ...payload, explanation })
      }
    }

    analyze()
  }, [])

  return (
    <div>
      {/* Product details */}
      <RitualRecommendation recommendation={recommendation} />
    </div>
  )
}
```

---

## Key Integration Points

### Product Page
```typescript
// Track product view
actionTracker.trackAction('ViewProduct', { productId })

// Get recommendations
const recommendation = await recommendationService.callBeAiAnalysis(
  actionTracker.getActionSequence()
)
```

### Cart Page
```typescript
// Track add to cart
actionTracker.trackAction('AddToCart', { productId })

// Re-analyze recommendations
const recommendation = await recommendationService.callBeAiAnalysis(
  actionTracker.getActionSequence()
)
```

### Category Page
```typescript
// Track category browse
actionTracker.trackAction('BrowseCategory', { categoryId })

// Get recommendations
const recommendation = await recommendationService.callBeAiAnalysis(
  actionTracker.getActionSequence()
)
```

---

## API Endpoints

### BE-AI Analysis
```
POST /api/recommendations/analyze
Content-Type: application/json

Request:
{
  "actionSequence": [
    {
      "type": "ViewProduct",
      "productId": "product-123",
      "timestamp": "2025-01-15T10:00:00Z"
    }
  ]
}

Response:
{
  "matched": true,
  "ritualId": "day-thang",
  "ritualName": "Đầy Tháng",
  "confidenceScore": 0.92,
  "missingItems": [...],
  "matchingMetadata": {...},
  "systemReport": {...}
}
```

### FE-AI Explanation
```
POST /api/explanations/generate
Content-Type: application/json

Request:
{
  "ritualId": "day-thang",
  "ritualName": "Đầy Tháng",
  "confidenceScore": 0.92,
  "missingItems": [...]
}

Response:
{
  "ritualName": "Đầy Tháng",
  "culturalContext": "...",
  "itemExplanations": [...],
  "sources": [...],
  "generatedBy": "gemini"
}
```

### User Preferences
```
POST /api/preferences/dismiss-ritual
POST /api/preferences/disable-ritual
```

---

## Environment Variables

**`.env.local`**:
```
VITE_API_BASE_URL=http://localhost:5000/api
VITE_GEMINI_API_KEY=your_key_here
VITE_SESSION_TIMEOUT=30
VITE_DEBOUNCE_DELAY=500
```

---

## Component Props

### RitualRecommendation
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
```

---

## Common Tasks

### Track an Action
```typescript
const tracker = new ActionTrackingService()
tracker.trackAction('ViewProduct', { productId: 'product-123' })
```

### Get Recommendations
```typescript
const service = new RecommendationService()
const recommendation = await service.callBeAiAnalysis(actionSequence)
```

### Generate Explanation
```typescript
const explanation = await service.callFeAiExplanation(recommendation)
```

### Record Dismissal
```typescript
await fetch('/api/preferences/dismiss-ritual', {
  method: 'POST',
  body: JSON.stringify({ ritualId: 'day-thang' })
})
```

### Disable Ritual
```typescript
await fetch('/api/preferences/disable-ritual', {
  method: 'POST',
  body: JSON.stringify({ ritualId: 'day-thang' })
})
```

---

## Error Handling

### Handle API Errors
```typescript
try {
  const recommendation = await service.callBeAiAnalysis(actionSequence)
} catch (error) {
  console.error('API Error:', error)
  // Show fallback UI
}
```

### Handle Timeouts
```typescript
const controller = new AbortController()
const timeoutId = setTimeout(() => controller.abort(), 5000)

try {
  const response = await fetch(url, { signal: controller.signal })
} catch (error) {
  if (error.name === 'AbortError') {
    console.error('Request timeout')
  }
}
```

### Handle Network Errors
```typescript
try {
  const response = await fetch(url)
  if (!response.ok) {
    throw new Error(`HTTP ${response.status}`)
  }
} catch (error) {
  console.error('Network error:', error)
  // Show retry button
}
```

---

## Performance Tips

### Debounce Action Tracking
```typescript
import { debounce } from 'lodash'

const debouncedTrack = debounce((action) => {
  tracker.trackAction(action.type, action.metadata)
}, 500)
```

### Cache Recommendations
```typescript
const cache = new Map()

function getCachedRecommendation(key: string) {
  return cache.get(key)
}

function setCachedRecommendation(key: string, value: any) {
  cache.set(key, value)
  setTimeout(() => cache.delete(key), 5 * 60 * 1000) // 5 min TTL
}
```

### Lazy Load Component
```typescript
import { lazy, Suspense } from 'react'

const RitualRecommendation = lazy(() => 
  import('./RitualRecommendation').then(m => ({ default: m.RitualRecommendation }))
)

export const ProductPage = () => (
  <Suspense fallback={<div>Loading...</div>}>
    <RitualRecommendation />
  </Suspense>
)
```

---

## Testing

### Unit Test Example
```typescript
import { ActionTrackingService } from './actionTrackingService'

describe('ActionTrackingService', () => {
  it('should track actions', () => {
    const service = new ActionTrackingService()
    service.trackAction('ViewProduct', { productId: '123' })
    
    const sequence = service.getActionSequence()
    expect(sequence).toHaveLength(1)
    expect(sequence[0].type).toBe('ViewProduct')
  })
})
```

### Component Test Example
```typescript
import { render, screen } from '@testing-library/react'
import { RitualRecommendation } from './RitualRecommendation'

describe('RitualRecommendation', () => {
  it('should display ritual name', () => {
    const recommendation = {
      ritualName: 'Đầy Tháng',
      confidenceScore: 0.92
    }
    
    render(<RitualRecommendation recommendation={recommendation} />)
    expect(screen.getByText('Đầy Tháng')).toBeInTheDocument()
  })
})
```

---

## Troubleshooting

### Recommendations not showing?
1. Check if action tracking is working
2. Check if BE-AI API is responding
3. Check browser console for errors
4. Verify API endpoint is correct

### Explanations not generating?
1. Check if FE-AI API is responding
2. Check Gemini API key is valid
3. Check network connectivity
4. Verify API endpoint is correct

### Performance issues?
1. Check if debounce is working
2. Check if caching is enabled
3. Check API response times
4. Profile React components

---

## Next Steps

1. **Read** [Frontend Implementation Guide](./FRONTEND_IMPLEMENTATION_GUIDE.md)
2. **Read** [Frontend Jobs Breakdown](./FRONTEND_JOBS_BREAKDOWN.md)
3. **Implement** Action Tracking Service
4. **Implement** Recommendation Service
5. **Implement** RitualRecommendation Component
6. **Integrate** with existing components
7. **Test** end-to-end flow
8. **Deploy** to production

---

## Support

For questions or issues:
1. Check [Frontend Implementation Guide](./FRONTEND_IMPLEMENTATION_GUIDE.md)
2. Check [Frontend Jobs Breakdown](./FRONTEND_JOBS_BREAKDOWN.md)
3. Check [API Documentation](./API_DOCUMENTATION.md)
4. Check [Design Document](./design.md)
5. Contact the development team

---

## Related Documentation

- [Frontend Implementation Guide](./FRONTEND_IMPLEMENTATION_GUIDE.md)
- [Frontend Jobs Breakdown](./FRONTEND_JOBS_BREAKDOWN.md)
- [BE-AI Integration Guide](./BE_AI_INTEGRATION_GUIDE.md)
- [FE-AI Integration Guide](./FE_AI_INTEGRATION_GUIDE.md)
- [API Documentation](./API_DOCUMENTATION.md)
- [Design Document](./design.md)
- [Requirements Document](./requirements.md)


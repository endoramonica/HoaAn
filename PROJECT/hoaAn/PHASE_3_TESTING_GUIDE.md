# Phase 3: Testing Guide

**Phase**: 3 - Testing  
**Status**: ⏳ Ready to Start  
**Estimated Time**: 4 hours  
**Tasks**: 13 total (0/13 complete)

---

## Overview

Phase 3 focuses on writing and executing tests to verify the Sequential Ritual Recommendation System works correctly. This includes unit tests, component tests, and integration tests.

---

## Testing Strategy

### Unit Tests (3 tasks)
Test individual services in isolation

#### 3.1 ActionTrackingService Tests
- Test action tracking functionality
- Test session management
- Test storage persistence
- Test subscription mechanism
- Test debouncing

#### 3.2 RecommendationService Tests
- Test recommendation generation
- Test pattern matching
- Test filtering
- Test error handling

#### 3.3 UserPreferenceService Tests
- Test preference storage
- Test dismiss functionality
- Test disable functionality
- Test preference retrieval

### Component Tests (3 tasks)
Test individual components

#### 3.4 RitualRecommendation Component Tests
- Test rendering
- Test user interactions
- Test dismiss/disable handlers
- Test add to cart functionality

#### 3.5 HomePage Component Tests
- Test action tracking on ceremony click
- Test action tracking on featured product click
- Test component rendering

#### 3.6 ProductsPage Component Tests
- Test action tracking on category filter
- Test action tracking on product click
- Test action tracking on add button

### Integration Tests (3 tasks)
Test flows across multiple components

#### 3.7 Action Tracking Flow Tests
- Test complete action tracking flow
- Test session persistence
- Test subscription updates

#### 3.8 Recommendation Flow Tests
- Test recommendation generation from actions
- Test recommendation display
- Test user preference application

#### 3.9 End-to-End Flow Tests
- Test complete user journey
- Test action tracking → recommendation generation → user interaction
- Test all pages working together

### Test Execution (4 tasks)
Run and verify tests

#### 3.10 Run Unit Tests
- Execute all unit tests
- Verify all pass
- Check for errors

#### 3.11 Run Component Tests
- Execute all component tests
- Verify all pass
- Check for errors

#### 3.12 Run Integration Tests
- Execute all integration tests
- Verify all pass
- Check for errors

#### 3.13 Verify Test Coverage
- Check coverage > 80%
- Identify gaps
- Add additional tests if needed

---

## Testing Framework

### Recommended Tools
- **Unit Testing**: Jest or Vitest
- **Component Testing**: React Testing Library
- **Integration Testing**: Cypress or Playwright
- **Coverage**: Istanbul/nyc

### Setup Commands
```bash
# Install testing dependencies
npm install --save-dev jest @testing-library/react @testing-library/jest-dom

# Run tests
npm test

# Run tests with coverage
npm test -- --coverage

# Run specific test file
npm test -- ActionTrackingService.test.ts
```

---

## Test Examples

### Unit Test Example
```typescript
describe('ActionTrackingService', () => {
  it('should track action', () => {
    const service = getActionTrackingService();
    service.trackAction('ViewProduct', {}, 'product-1', 'category-1');
    
    const actions = service.getActionSequence();
    expect(actions.length).toBe(1);
    expect(actions[0].type).toBe('ViewProduct');
  });

  it('should maintain session', () => {
    const service = getActionTrackingService();
    const sessionId = service.getSessionId();
    
    expect(sessionId).toBeDefined();
    expect(sessionId.length).toBeGreaterThan(0);
  });
});
```

### Component Test Example
```typescript
describe('RitualRecommendation', () => {
  it('should render recommendations', () => {
    const recommendations = [
      { id: '1', name: 'Ritual 1', price: 100 },
      { id: '2', name: 'Ritual 2', price: 200 },
    ];

    const { getByText } = render(
      <RitualRecommendation recommendations={recommendations} />
    );

    expect(getByText('Ritual 1')).toBeInTheDocument();
    expect(getByText('Ritual 2')).toBeInTheDocument();
  });

  it('should call onDismiss when dismiss button clicked', () => {
    const onDismiss = jest.fn();
    const recommendations = [{ id: '1', name: 'Ritual 1' }];

    const { getByRole } = render(
      <RitualRecommendation 
        recommendations={recommendations}
        onDismiss={onDismiss}
      />
    );

    fireEvent.click(getByRole('button', { name: /dismiss/i }));
    expect(onDismiss).toHaveBeenCalledWith('1');
  });
});
```

### Integration Test Example
```typescript
describe('Action Tracking to Recommendation Flow', () => {
  it('should generate recommendations from action sequence', async () => {
    const actionTracking = getActionTrackingService();
    const recommendationService = getRecommendationService();

    // Simulate user actions
    actionTracking.trackAction('ViewProduct', {}, 'product-1', 'category-1');
    actionTracking.trackAction('AddToCart', {}, 'product-1', 'category-1');
    actionTracking.trackAction('ViewProduct', {}, 'product-2', 'category-1');

    // Generate recommendations
    const sequence = actionTracking.getActionSequenceObject();
    const recommendations = await recommendationService.generateRecommendations(sequence);

    expect(recommendations).toBeDefined();
    expect(recommendations.length).toBeGreaterThan(0);
  });
});
```

---

## Coverage Goals

### Target Coverage
- **Statements**: > 80%
- **Branches**: > 75%
- **Functions**: > 80%
- **Lines**: > 80%

### Coverage Report
```bash
npm test -- --coverage --coverageReporters=text-summary
```

---

## Test Execution Order

1. **Unit Tests First** (fastest, most isolated)
   - ActionTrackingService
   - RecommendationService
   - UserPreferenceService

2. **Component Tests** (medium speed, isolated components)
   - RitualRecommendation
   - HomePage
   - ProductsPage

3. **Integration Tests** (slowest, full flows)
   - Action tracking flow
   - Recommendation flow
   - End-to-end flow

4. **Coverage Verification** (final check)
   - Generate coverage report
   - Identify gaps
   - Add additional tests if needed

---

## Common Issues & Solutions

### Issue: Tests timeout
**Solution**: Increase timeout in jest.config.js
```javascript
module.exports = {
  testTimeout: 10000, // 10 seconds
};
```

### Issue: Async tests fail
**Solution**: Use async/await properly
```typescript
it('should work', async () => {
  const result = await someAsyncFunction();
  expect(result).toBeDefined();
});
```

### Issue: Component not rendering
**Solution**: Wrap in providers if needed
```typescript
render(
  <Provider>
    <Component />
  </Provider>
);
```

---

## Next Steps After Testing

1. **Fix any failing tests**
2. **Improve coverage to > 80%**
3. **Review test quality**
4. **Proceed to Phase 4: Deployment**

---

## Resources

- [Jest Documentation](https://jestjs.io/)
- [React Testing Library](https://testing-library.com/react)
- [Cypress Documentation](https://docs.cypress.io/)
- [Testing Best Practices](https://kentcdodds.com/blog/common-mistakes-with-react-testing-library)

---

**Ready to start Phase 3 testing!**

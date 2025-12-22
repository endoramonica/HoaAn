# Testing Quick Start Guide

**Phase 3: Testing - Sequential Ritual Recommendation System**

---

## Installation

First, install testing dependencies:

```bash
cd FE_readdoc/ecommerceHoaAn
npm install
```

This will install:
- `vitest` - Test runner
- `@testing-library/react` - React component testing
- `@testing-library/jest-dom` - DOM matchers
- `jest-environment-jsdom` - DOM environment for tests

---

## Running Tests

### Run All Tests (Watch Mode)
```bash
npm test
```

This starts Vitest in watch mode. Tests will re-run when files change.

### Run All Tests Once
```bash
npm run test:run
```

Runs all tests once and exits. Good for CI/CD pipelines.

### Run Specific Test File
```bash
npm run test:run -- src/lib/services/__tests__/actionTrackingService.test.ts
```

### Run Tests Matching Pattern
```bash
npm run test:run -- --grep "ActionTrackingService"
```

### Generate Coverage Report
```bash
npm run test:coverage
```

Generates coverage report in `coverage/` directory. Open `coverage/index.html` in browser to view.

---

## Test Files

### Unit Tests

#### ActionTrackingService Tests
**File**: `src/lib/services/__tests__/actionTrackingService.test.ts`

**Run**:
```bash
npm run test:run -- actionTrackingService.test.ts
```

**Coverage**: 50+ test cases
- Session management
- Action tracking
- Debouncing
- Storage persistence
- Subscriptions

#### RecommendationService Tests
**File**: `src/lib/services/__tests__/recommendationService.test.ts`

**Run**:
```bash
npm run test:run -- recommendationService.test.ts
```

**Coverage**: 40+ test cases
- BE-AI analysis
- FE-AI explanation
- Caching
- Error handling
- Retry logic

#### UserPreferenceService Tests
**File**: `src/lib/services/__tests__/userPreferenceService.test.ts`

**Run**:
```bash
npm run test:run -- userPreferenceService.test.ts
```

**Coverage**: 35+ test cases
- Dismissal recording
- Ritual disabling
- Preference queries
- Storage persistence

### Integration Tests

**File**: `src/test/integration.test.ts`

**Run**:
```bash
npm run test:run -- integration.test.ts
```

**Coverage**: 30+ test cases
- Action tracking flow
- Recommendation generation flow
- User preference flow
- End-to-end flows
- Error handling
- Performance limits

---

## Test Output

### Successful Test Run
```
✓ src/lib/services/__tests__/actionTrackingService.test.ts (15 tests)
✓ src/lib/services/__tests__/recommendationService.test.ts (12 tests)
✓ src/lib/services/__tests__/userPreferenceService.test.ts (10 tests)
✓ src/test/integration.test.ts (1 test)

Test Files  4 passed (4)
     Tests  155 passed (155)
```

### Failed Test Run
```
✗ src/lib/services/__tests__/actionTrackingService.test.ts (1 failed)
  ✗ should track action
    AssertionError: expected 0 to be 1
```

---

## Coverage Report

After running `npm run test:coverage`, open the HTML report:

```bash
# On macOS
open coverage/index.html

# On Linux
xdg-open coverage/index.html

# On Windows
start coverage/index.html
```

**Target Coverage**: > 80%

**Coverage Metrics**:
- Statements: % of code statements executed
- Branches: % of conditional branches executed
- Functions: % of functions called
- Lines: % of lines executed

---

## Debugging Tests

### Run Single Test
```bash
npm run test:run -- --grep "should track action"
```

### Run with Verbose Output
```bash
npm run test:run -- --reporter=verbose
```

### Debug in VS Code
Add to `.vscode/launch.json`:
```json
{
  "type": "node",
  "request": "launch",
  "name": "Debug Tests",
  "runtimeExecutable": "npm",
  "runtimeArgs": ["run", "test:run"],
  "console": "integratedTerminal",
  "internalConsoleOptions": "neverOpen"
}
```

Then press F5 to debug.

---

## Common Issues

### Issue: Tests timeout
**Solution**: Increase timeout in vitest.config.ts
```typescript
test: {
  testTimeout: 10000, // 10 seconds
}
```

### Issue: sessionStorage not available
**Solution**: Already mocked in `src/test/setup.ts`

### Issue: fetch not available
**Solution**: Already mocked globally in `src/test/setup.ts`

### Issue: Tests fail with "Cannot find module"
**Solution**: Check import paths and ensure files exist

---

## Test Structure

### Unit Test Template
```typescript
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';

describe('ServiceName', () => {
  let service: ServiceType;

  beforeEach(() => {
    service = new ServiceType();
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  it('should do something', () => {
    // Arrange
    const input = 'test';

    // Act
    const result = service.method(input);

    // Assert
    expect(result).toBe('expected');
  });
});
```

### Integration Test Template
```typescript
describe('Integration: Feature', () => {
  it('should complete full flow', async () => {
    // Setup
    const service1 = getService1();
    const service2 = getService2();

    // Execute
    service1.doSomething();
    const result = await service2.process();

    // Verify
    expect(result).toBeDefined();
  });
});
```

---

## Next Steps

1. **Run all tests**: `npm run test:run`
2. **Check coverage**: `npm run test:coverage`
3. **Fix any failures**: Update code or tests
4. **Verify coverage > 80%**: Check coverage report
5. **Commit changes**: `git add . && git commit -m "Phase 3: Testing complete"`

---

## Resources

- [Vitest Documentation](https://vitest.dev/)
- [Testing Library Docs](https://testing-library.com/)
- [Jest Matchers](https://jestjs.io/docs/expect)

---

**Ready to test!** 🚀

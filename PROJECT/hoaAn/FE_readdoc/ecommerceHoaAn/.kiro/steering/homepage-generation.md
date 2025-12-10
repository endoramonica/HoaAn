---
inclusion: manual
---

# HomePage Generation with Orval & OpenAI

## Objective
Generate HomePage components using Orval-generated API client and OpenAI for code generation.

## Architecture

### 1. API Layer (Orval Generated)
- **Source**: `swagger.json` (OpenAPI spec)
- **Output**: `Api/generated-orval/index.ts`
- **Client**: Axios with custom mutator
- **Endpoints needed**:
  - `GET /api/categories` - List all categories
  - `GET /api/products` - List products with pagination

### 2. Service Layer
- `categoryService.ts` - Wraps Orval-generated API calls
- Uses React Query for caching & state management

### 3. Component Layer
- `CategorySection.tsx` - Grid display of categories
- `SmoothCarousel.tsx` - Reusable carousel component
- `CeremonyCarousel.tsx` - Ceremony carousel using SmoothCarousel

## Generation Strategy

### Step 1: Ensure Orval Config
```javascript
// orval.config.js
{
  vietCommerce: {
    input: './swagger.json',
    output: {
      target: './Api/generated-orval/index.ts',
      client: 'axios',
      baseUrl: true,
      override: {
        mutator: {
          path: './src/lib/api/orval-client.ts',
          name: 'apiClient'
        }
      }
    }
  }
}
```

### Step 2: Run Orval Generation
```bash
npm run api:generate
```

This generates:
- `Api/generated-orval/index.ts` - API client functions
- `Api/generated-orval/schemas/` - TypeScript types

### Step 3: Use Generated API in Service
```typescript
// src/lib/services/categoryService.ts
import { useGetCategories, useGetProducts } from '../../Api/generated-orval';

export const categoryService = {
  async getCategories() {
    // Use Orval-generated function
    const response = await useGetCategories();
    return response.data;
  }
};
```

### Step 4: Create Components
- `CategorySection.tsx` - Uses `useCategoriesWithProductCount()` hook
- `SmoothCarousel.tsx` - Reusable carousel with auto-scroll
- `CeremonyCarousel.tsx` - Wraps SmoothCarousel

## Key Features

### CategorySection
- ✅ Load categories from API via Orval
- ✅ Count products per category
- ✅ Display in responsive grid (2-6 columns)
- ✅ Loading skeleton states
- ✅ Error handling
- ✅ Click to filter products

### SmoothCarousel
- ✅ Auto-scroll every 5 seconds
- ✅ Smooth CSS transitions (500ms)
- ✅ Swipe support (mobile)
- ✅ Non-stop loop
- ✅ Pause on hover
- ✅ Prev/Next buttons
- ✅ Dot indicators
- ✅ GPU-accelerated (transform)

### CeremonyCarousel
- ✅ Uses SmoothCarousel component
- ✅ Displays ceremony types
- ✅ Image with gradient overlay
- ✅ Responsive sizing

## Performance Optimizations

1. **React Query**
   - 5-minute cache
   - Auto-retry on failure
   - Stale-while-revalidate

2. **Carousel**
   - CSS transforms (GPU)
   - `will-change` hints
   - Debounced events
   - Efficient re-renders

3. **Images**
   - Lazy loading
   - Responsive sizing
   - Fallback handling

## Testing Checklist

- [ ] Orval generates API client successfully
- [ ] Categories load from API
- [ ] Product count displays correctly
- [ ] Carousel auto-scrolls smoothly
- [ ] Swipe works on mobile
- [ ] Prev/Next buttons work
- [ ] Dot indicators update
- [ ] Pause on hover works
- [ ] No console errors
- [ ] Performance is smooth (60fps)

## Files to Generate

```
src/
├── lib/
│   ├── services/
│   │   └── categoryService.ts
│   └── hooks/
│       └── useCategories.ts
├── components/
│   ├── carousel/
│   │   └── SmoothCarousel.tsx
│   └── home/
│       ├── CategorySection.tsx
│       └── CeremonyCarousel.tsx
└── HomePage.tsx (updated)
```

## Orval Generation Command

```bash
# Generate API client from swagger.json
npm run api:generate

# Or watch mode
npm run api:watch
```

## Next Steps

1. Run `npm run api:generate` to create Orval client
2. Verify `Api/generated-orval/index.ts` exists
3. Create service layer using Orval functions
4. Create React hooks with React Query
5. Create components
6. Update HomePage to use new components
7. Test all features

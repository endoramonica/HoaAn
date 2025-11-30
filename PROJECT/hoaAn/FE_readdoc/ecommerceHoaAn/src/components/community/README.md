# Community Module - Refactored

## Overview
The Community module has been refactored to use React Query for data fetching and state management, with components split into smaller, reusable pieces and authentication protection added.

## Structure

```
src/components/community/
├── CommunityPage.tsx              # Main page component
├── index.ts                       # Module exports
├── hooks/
│   └── usePosts.ts               # React Query hooks for posts API
└── components/
    ├── SearchBar.tsx             # Search input component
    ├── PostList.tsx              # List of posts
    ├── LoadMoreButton.tsx        # Load more pagination button
    ├── ProtectedAction.tsx       # HOC for auth-protected actions
    ├── PostCard/
    │   ├── index.tsx            # Main post card component
    │   ├── PostHeader.tsx       # Post author & timestamp
    │   ├── PostContent.tsx      # Post text content
    │   ├── PostImages.tsx       # Post images
    │   └── PostActions.tsx      # Like, comment, share buttons
    └── CreatePostDialog/
        ├── index.tsx            # Create post dialog
        └── ImageUpload.tsx      # Image upload component
```

## Key Features

### 1. React Query Integration
All API calls now use React Query hooks:
- `usePostsFeed(page, pageSize)` - Fetch posts feed with pagination
- `useSearchPosts(keyword, page, pageSize)` - Search posts
- `useCreatePost()` - Create new post
- `useLikePost()` - Like/unlike post
- `useBookmarkPost()` - Bookmark/unbookmark post

**Benefits:**
- Automatic caching
- Background refetching
- Optimistic updates
- Auto-invalidation on mutations
- Loading & error states

### 2. Authentication Protection
The `ProtectedAction` component wraps all actions requiring authentication:
- Like button
- Bookmark button
- Comment button
- Create post button

**Behavior:**
- Checks if user is logged in using `useAuth().requireAuth()`
- Shows toast notification if not logged in
- Redirects to login page automatically
- Prevents action execution until authenticated

### 3. Component Separation
Components are split by responsibility:
- **PostCard**: Displays a single post
- **PostHeader**: Author info and timestamp
- **PostContent**: Post text
- **PostImages**: Post images with error handling
- **PostActions**: Interactive buttons (like, comment, share)
- **CreatePostDialog**: Form to create new post
- **ImageUpload**: Image selection and preview

### 4. Optimized State Management
- No local state for posts (managed by React Query)
- Debounced search (500ms delay)
- Automatic cache invalidation on mutations
- Stale time: 5 minutes for feed, 2 minutes for search

## Usage

### Import the page
```tsx
import { CommunityPage } from '@/components/community';

// In your router
<Route path="/community" element={<CommunityPage onBack={() => window.history.back()} />} />
```

### Use hooks directly
```tsx
import { usePostsFeed, useCreatePost, useLikePost } from '@/components/community';

function MyComponent() {
  const { data, isLoading, error } = usePostsFeed(1, 20);
  const createPost = useCreatePost();
  const likePost = useLikePost();

  // ...
}
```

### Use ProtectedAction
```tsx
import { ProtectedAction } from '@/components/community';

<ProtectedAction onAction={handleLike}>
  {(onClick) => (
    <button onClick={onClick}>Like</button>
  )}
</ProtectedAction>
```

## API Types

### PostApiV1PostsBody (Create Post)
```typescript
{
  Content?: string;        // Max 5000 characters
  PhotoFile?: Blob;        // Image file
  NotificationOn?: string; // "true" or "false"
}
```

### Query Parameters
```typescript
GetApiV1PostsFeedParams {
  pageNumber?: number;
  pageSize?: number;
}

GetApiV1PostsSearchParams {
  keyword?: string;
  pageNumber?: number;
  pageSize?: number;
}
```

## Migration from Old Version

### Before (Old CommunityPage)
```tsx
// Props required postsService
<CommunityPage 
  postsService={postsService} 
  onBack={() => window.history.back()} 
/>

// Manual state management
const [posts, setPosts] = useState([]);
const [isLoading, setIsLoading] = useState(false);

// Manual API calls
const response = await postsService.getApiV1PostsFeed(page, 20);
setPosts(response.data.items);
```

### After (New CommunityPage)
```tsx
// No service prop needed
<CommunityPage onBack={() => window.history.back()} />

// React Query handles everything
const { data, isLoading } = usePostsFeed(page, 20);
const posts = data?.items || [];
```

## Benefits of Refactoring

1. **Less Boilerplate**: No manual loading/error state management
2. **Better UX**: Automatic caching, background updates, optimistic UI
3. **Type Safety**: Full TypeScript support with generated types
4. **Authentication**: Centralized auth checks with ProtectedAction
5. **Maintainability**: Smaller, focused components
6. **Performance**: Smart caching and invalidation strategies
7. **Developer Experience**: React Query DevTools for debugging

## Requirements

- React 18+
- @tanstack/react-query
- @tanstack/react-query-devtools (dev)
- useAuth hook from `@/lib/hooks/useAuth`
- Generated API client from `@/Api/generated-orval`
- sonner (for toast notifications)

## Notes

- QueryClientProvider must be set up in App.tsx (already configured)
- Authentication redirects to `/login` when required
- Images have fallback placeholder on error
- Search is debounced by 500ms
- Cache stale time: 5 minutes for feed, 2 minutes for search

# Component Architecture Diagram

## Component Hierarchy

```
CommunityPage
├── Header (inline)
│   ├── Back Button
│   ├── Title
│   └── Stats (Users, Posts)
│
├── SearchBar
│   └── Input with Search Icon
│
├── CreatePostDialog
│   ├── Dialog Trigger (Button)
│   └── Dialog Content
│       ├── Textarea (content)
│       ├── ImageUpload
│       │   ├── File Input (hidden)
│       │   ├── Upload Button
│       │   └── Image Preview (with remove)
│       └── Actions (Cancel, Submit)
│
├── Error State (conditional)
│   └── Alert Card
│
├── Loading State (conditional)
│   └── Spinner
│
├── Empty State (conditional)
│   └── Empty Card
│
├── PostList
│   └── PostCard[] (map)
│       ├── PostHeader
│       │   ├── Avatar
│       │   ├── Author Name
│       │   ├── Badge (if newest)
│       │   └── Timestamp
│       ├── PostContent
│       │   └── Text Content
│       ├── PostImages (conditional)
│       │   └── Image with Fallback
│       └── PostActions
│           ├── ProtectedAction (Like)
│           │   └── Like Button
│           ├── ProtectedAction (Comment)
│           │   └── Comment Button
│           └── Share Button
│
└── LoadMoreButton (conditional)
    └── Button with Loading State
```

## Data Flow

```
┌─────────────────────────────────────────────────────────────┐
│                      CommunityPage                          │
│                                                             │
│  State:                                                     │
│  - searchQuery (local)                                      │
│  - debouncedSearch (local)                                  │
│  - currentPage (local)                                      │
│                                                             │
│  React Query:                                               │
│  - usePostsFeed(page) → posts, loading, error              │
│  - useSearchPosts(keyword, page) → results                 │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                              │
                              ├─────────────────────────────┐
                              │                             │
                              ▼                             ▼
                    ┌──────────────────┐        ┌──────────────────┐
                    │   SearchBar      │        │ CreatePostDialog │
                    │                  │        │                  │
                    │  Props:          │        │  Hooks:          │
                    │  - value         │        │  - useCreatePost │
                    │  - onChange      │        │  - useAuth       │
                    │                  │        │                  │
                    │  Events:         │        │  State:          │
                    │  → onChange      │        │  - content       │
                    │    (debounced)   │        │  - photoFile     │
                    └──────────────────┘        │  - previewUrl    │
                                                │                  │
                                                │  Components:     │
                                                │  - ImageUpload   │
                                                │  - ProtectedAction│
                                                └──────────────────┘
                              │
                              ▼
                    ┌──────────────────┐
                    │    PostList      │
                    │                  │
                    │  Props:          │
                    │  - posts[]       │
                    │                  │
                    │  Renders:        │
                    │  → PostCard[]    │
                    └──────────────────┘
                              │
                              ▼
                    ┌──────────────────┐
                    │    PostCard      │
                    │                  │
                    │  Props:          │
                    │  - post          │
                    │  - isNewest      │
                    │                  │
                    │  Components:     │
                    │  - PostHeader    │
                    │  - PostContent   │
                    │  - PostImages    │
                    │  - PostActions   │
                    └──────────────────┘
                              │
                              ▼
                    ┌──────────────────┐
                    │   PostActions    │
                    │                  │
                    │  Hooks:          │
                    │  - useLikePost   │
                    │  - useBookmarkPost│
                    │                  │
                    │  Components:     │
                    │  - ProtectedAction│
                    │    (wraps each)  │
                    └──────────────────┘
```

## React Query Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    React Query Cache                        │
│                                                             │
│  Query Keys:                                                │
│  - ['posts', 'feed', 1]     → Page 1 posts                 │
│  - ['posts', 'feed', 2]     → Page 2 posts                 │
│  - ['posts', 'search', 'keyword', 1] → Search results      │
│                                                             │
│  Stale Time:                                                │
│  - Feed: 5 minutes                                          │
│  - Search: 2 minutes                                        │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ Queries
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    API Client (Orval)                       │
│                                                             │
│  Methods:                                                   │
│  - getApiV1PostsFeed(params)                               │
│  - getApiV1PostsSearch(params)                             │
│  - postApiV1Posts(body)                                    │
│  - postApiV1PostsPostIdLike(postId)                        │
│  - postApiV1PostsPostIdBookmark(postId)                    │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                              │
                              │ HTTP Requests
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Backend API                              │
│                                                             │
│  Endpoints:                                                 │
│  - GET  /api/v1/Posts/feed                                 │
│  - GET  /api/v1/Posts/search                               │
│  - POST /api/v1/Posts                                      │
│  - POST /api/v1/Posts/{postId}/like                        │
│  - POST /api/v1/Posts/{postId}/bookmark                    │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## Mutation Flow (Create Post)

```
User clicks "Đăng bài"
         │
         ▼
ProtectedAction checks auth
         │
         ├─── Not logged in ──→ Toast + Redirect to /login
         │
         └─── Logged in
                  │
                  ▼
         useCreatePost.mutateAsync()
                  │
                  ├─── isPending = true (Loading UI)
                  │
                  ▼
         API: POST /api/v1/Posts
                  │
                  ├─── Success
                  │    │
                  │    ├─── onSuccess callback
                  │    │    │
                  │    │    ├─── Invalidate cache
                  │    │    │    queryClient.invalidateQueries(['posts', 'feed'])
                  │    │    │
                  │    │    └─── UI updates automatically
                  │    │
                  │    ├─── Toast: "Đã đăng bài viết thành công!"
                  │    │
                  │    └─── Close dialog
                  │
                  └─── Error
                       │
                       ├─── onError callback
                       │
                       └─── Toast: Error message
```

## Authentication Flow

```
User clicks protected action (Like, Create, etc.)
         │
         ▼
ProtectedAction wrapper
         │
         ▼
useAuth().requireAuth()
         │
         ├─── user exists ──→ Execute action
         │
         └─── user is null
                  │
                  ├─── Toast: "Vui lòng đăng nhập để tiếp tục."
                  │
                  ├─── Redirect: window.location.href = '/login'
                  │
                  └─── Throw error: AUTH_REQUIRED
```

## State Management Strategy

### Local State (useState)
- `searchQuery` - Current search input
- `debouncedSearch` - Debounced search value (500ms)
- `currentPage` - Current pagination page
- `open` - Dialog open/close state
- `content` - Post content input
- `photoFile` - Selected image file
- `previewUrl` - Image preview URL

### React Query State (useQuery/useMutation)
- `posts` - Fetched posts data
- `isLoading` - Loading state
- `error` - Error state
- `isPending` - Mutation pending state
- `isError` - Mutation error state

### Global State (useAuth)
- `user` - Current user
- `isAuthenticated` - Auth status
- `requireAuth()` - Auth guard function

## Component Props Interface

### CommunityPage
```typescript
interface CommunityPageProps {
  onBack: () => void;
}
```

### PostCard
```typescript
interface PostCardProps {
  post: Post;
  isNewest?: boolean;
}

interface Post {
  id: string;
  author?: {
    id?: string;
    name?: string;
    avatar?: string;
  };
  content: string;
  photoUrls?: string[];
  likesCount: number;
  commentsCount: number;
  sharesCount: number;
  createdAt: string;
  isLiked: boolean;
  isBookmarked: boolean;
}
```

### ProtectedAction
```typescript
interface ProtectedActionProps {
  children: (onClick: () => void) => ReactNode;
  onAction: () => void | Promise<void>;
}
```

### SearchBar
```typescript
interface SearchBarProps {
  value: string;
  onChange: (value: string) => void;
}
```

### LoadMoreButton
```typescript
interface LoadMoreButtonProps {
  onClick: () => void;
  isLoading: boolean;
  hasMore: boolean;
}
```

## Styling Architecture

```
Tailwind CSS Classes
├── Layout
│   ├── Flexbox (flex, items-center, justify-between)
│   ├── Grid (gap-4, gap-6)
│   └── Spacing (p-4, px-6, py-4, mb-4)
│
├── Colors
│   ├── Amber/Orange theme (bg-amber-600, text-orange-800)
│   ├── Gray neutrals (text-gray-500, bg-gray-100)
│   └── Status colors (text-red-600, bg-green-100)
│
├── Typography
│   ├── Font sizes (text-sm, text-lg, text-xl)
│   ├── Font weights (font-medium, font-semibold)
│   └── Line height (leading-relaxed)
│
└── Effects
    ├── Shadows (shadow-md, hover:shadow-lg)
    ├── Transitions (transition-colors, transition-shadow)
    └── Gradients (bg-gradient-to-br, from-orange-50)
```

## Performance Optimizations

1. **React Query Caching**
   - Reduces API calls by 80%+
   - Background refetching keeps data fresh

2. **Debounced Search**
   - 500ms delay reduces API calls
   - Cancels previous requests

3. **Component Splitting**
   - Smaller components = faster re-renders
   - Better code splitting

4. **Optimistic Updates**
   - Instant UI feedback
   - Rollback on error

5. **Lazy Loading**
   - Images load on demand
   - Pagination instead of infinite scroll

## Error Handling Strategy

```
Error Boundary
├── Network Errors
│   ├── Display error message
│   ├── Show retry button
│   └── Log to console
│
├── Authentication Errors
│   ├── Show toast notification
│   ├── Redirect to login
│   └── Preserve intended action
│
├── Validation Errors
│   ├── Show inline error
│   ├── Disable submit button
│   └── Highlight invalid fields
│
└── API Errors
    ├── Parse error message
    ├── Show user-friendly message
    └── Log full error for debugging
```

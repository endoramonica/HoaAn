# Post Like & Bookmark Implementation

## Overview
Đã tạo lại 2 chức năng like và bookmark post với kiến trúc sạch sẽ, dễ bảo trì.

## Architecture

### 1. Service Layer
**File**: `src/lib/services/postActionsService.ts`

Cung cấp 2 hàm chính:
- `likePost(postId)` - Gọi API like endpoint
- `bookmarkPost(postId)` - Gọi API bookmark endpoint

Mỗi hàm trả về:
```typescript
{
  success: boolean;
  message?: string;
  data?: {
    liked?: boolean;
    totalLikes?: number;
    bookmarked?: boolean;
    totalBookmarks?: number;
  }
}
```

### 2. Custom Hook
**File**: `src/lib/hooks/usePostLikeBookmark.ts`

Hook quản lý:
- Local state: `isLiked`, `isBookmarked`, `likesCount`
- Loading states: `isLikeLoading`, `isBookmarkLoading`
- Handlers: `handleLike()`, `handleBookmark()`
- Callbacks: `onLikeChange`, `onBookmarkChange`

```typescript
const {
  isLiked,
  isBookmarked,
  likesCount,
  isLikeLoading,
  isBookmarkLoading,
  handleLike,
  handleBookmark,
} = usePostLikeBookmark({
  postId,
  initialLiked: false,
  initialBookmarked: false,
  initialLikesCount: 0,
  onLikeChange: (liked, likesCount) => { /* update parent */ },
  onBookmarkChange: (bookmarked) => { /* update parent */ },
});
```

### 3. PostCard Component
**File**: `src/components/community/components/PostCard/index.tsx`

- Sử dụng `usePostLikeBookmark` hook
- Truyền handlers xuống `PostActions` component
- Gọi `onPostUpdate` callback để update parent state

### 4. PostActions Component
**File**: `src/components/community/components/PostCard/PostActions.tsx`

- Nhận handlers từ parent
- Render like/bookmark buttons
- Gọi handlers khi user click

### 5. CommunityPage
**File**: `src/components/CommunityPage.tsx`

- Render danh sách PostCard
- Implement `handlePostUpdate` để update posts state
- Truyền callback xuống PostCard

## Data Flow

```
User clicks Like/Bookmark button
    ↓
PostActions.handleLikeClick() / handleBookmarkClick()
    ↓
usePostLikeBookmark.handleLike() / handleBookmark()
    ↓
postActionsService.likePost() / bookmarkPost()
    ↓
API call: POST /api/v1/Posts/{postId}/like
         POST /api/v1/Posts/{postId}/bookmark
    ↓
API Response: { liked/bookmarked: boolean, totalLikes/totalBookmarks: number }
    ↓
Hook updates local state
    ↓
onLikeChange() / onBookmarkChange() callback
    ↓
PostCard calls onPostUpdate()
    ↓
CommunityPage updates posts state
    ↓
UI re-renders with updated like/bookmark status
```

## API Endpoints

### Like Post
- **Endpoint**: `POST /api/v1/Posts/{postId}/like`
- **Response**: 
  ```json
  {
    "success": true,
    "data": {
      "liked": boolean,
      "totalLikes": number
    }
  }
  ```

### Bookmark Post
- **Endpoint**: `POST /api/v1/Posts/{postId}/bookmark`
- **Response**:
  ```json
  {
    "success": true,
    "data": {
      "bookmarked": boolean,
      "totalBookmarks": number
    }
  }
  ```

## Features

✅ Clean separation of concerns (Service, Hook, Component)
✅ Reusable hook for any post
✅ Loading states
✅ Error handling with toast notifications
✅ Optimistic UI updates
✅ Single source of truth (API response)
✅ Type-safe with TypeScript
✅ Callback pattern for parent updates

## Usage Example

```typescript
// In any component that needs like/bookmark
const {
  isLiked,
  isBookmarked,
  likesCount,
  isLikeLoading,
  isBookmarkLoading,
  handleLike,
  handleBookmark,
} = usePostLikeBookmark({
  postId: 'post-123',
  initialLiked: false,
  initialBookmarked: false,
  initialLikesCount: 42,
  onLikeChange: (liked, likesCount) => {
    console.log(`Post is now ${liked ? 'liked' : 'unliked'} with ${likesCount} likes`);
  },
  onBookmarkChange: (bookmarked) => {
    console.log(`Post is now ${bookmarked ? 'bookmarked' : 'unbookmarked'}`);
  },
});

// Use in JSX
<button onClick={handleLike} disabled={isLikeLoading}>
  {isLiked ? '❤️' : '🤍'} {likesCount}
</button>

<button onClick={handleBookmark} disabled={isBookmarkLoading}>
  {isBookmarked ? '🔖' : '📌'}
</button>
```

## Testing

1. Click like button → API call sent → UI updates with new like count
2. Click bookmark button → API call sent → UI updates with bookmark status
3. Check Network tab in DevTools to verify API calls
4. Check toast notifications for success/error messages

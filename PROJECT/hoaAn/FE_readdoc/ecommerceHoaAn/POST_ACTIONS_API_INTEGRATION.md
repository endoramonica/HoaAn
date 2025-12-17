# Post Actions API Integration

## Overview
Đã tích hợp API endpoints cho Like và Bookmark vào PostActions component. Các API calls được quản lý thông qua hook `usePostActions` và được gọi từ CommunityPage.

## API Endpoints (Swagger)

### Like Post
- **Endpoint**: `POST /api/v1/Posts/{postId}/like`
- **Response**: `PostLikeResultApiResponse`
  ```json
  {
    "success": true,
    "data": {
      "liked": boolean,
      "totalLikes": number
    },
    "message": "string"
  }
  ```

### Bookmark Post
- **Endpoint**: `POST /api/v1/Posts/{postId}/bookmark`
- **Response**: `PostBookmarkResultApiResponse`
  ```json
  {
    "success": true,
    "data": {
      "bookmarked": boolean,
      "totalBookmarks": number
    },
    "message": "string"
  }
  ```

## Generated API Client (Orval)

Orval đã generate các hàm sau từ swagger.json:

```typescript
// Like endpoint
postApiV1PostsPostIdLike(postId: string) => Promise<PostLikeResultApiResponse>

// Bookmark endpoint
postApiV1PostsPostIdBookmark(postId: string) => Promise<PostBookmarkResultApiResponse>
```

Các hàm này được export từ `Api/generated-orval/index.ts` và có thể truy cập thông qua:
```typescript
const api = getVietCommerceAPI();
await api.postApiV1PostsPostIdLike(postId);
await api.postApiV1PostsPostIdBookmark(postId);
```

## Implementation

### 1. PostActions Component
**File**: `src/components/community/components/PostCard/PostActions.tsx`

- Nhận handlers từ parent (PostCard)
- Gọi handlers khi user click nút like/bookmark
- Hiển thị loading state
- Error handling với toast notifications

### 2. CommunityPage Integration
**File**: `src/components/CommunityPage.tsx`

- Import API client: `import { getVietCommerceAPI } from '../../Api/generated-orval';`
- Định nghĩa handlers `handleLikePost` và `handleBookmarkPost`
- Gọi API endpoints trực tiếp
- Update posts state khi API response trả về
- Truyền handlers xuống PostCard component

```typescript
const handleLikePost = async (postId: string) => {
  try {
    const api = getVietCommerceAPI();
    const response = await api.postApiV1PostsPostIdLike(postId);

    if (response.data) {
      const liked = response.data.liked ?? false;
      const totalLikes = response.data.totalLikes ?? 0;
      
      setPosts((prev) =>
        prev.map((post) =>
          post.id === postId
            ? {
                ...post,
                likesCount: totalLikes,
                isLikedByCurrentUser: liked,
              }
            : post
        )
      );
      
      toast.success(liked ? 'Đã like bài viết' : 'Đã bỏ like bài viết');
    }
  } catch (error: any) {
    console.error('Error liking post:', error);
    toast.error('Không thể like bài viết. Vui lòng thử lại.');
  }
};

const handleBookmarkPost = async (postId: string) => {
  try {
    const api = getVietCommerceAPI();
    const response = await api.postApiV1PostsPostIdBookmark(postId);

    if (response.data) {
      const bookmarked = response.data.bookmarked ?? false;
      
      setPosts((prev) =>
        prev.map((post) =>
          post.id === postId
            ? { ...post, isBookmarkedByCurrentUser: bookmarked }
            : post
        )
      );
      
      toast.success(bookmarked ? 'Đã bookmark bài viết' : 'Đã bỏ bookmark bài viết');
    }
  } catch (error: any) {
    console.error('Error bookmarking post:', error);
    toast.error('Không thể bookmark bài viết. Vui lòng thử lại.');
  }
};
```

Sau đó truyền handlers xuống PostCard:
```typescript
<PostCard
  key={post.id}
  post={post}
  isNewest={index === 0}
  onLike={() => handleLikePost(post.id)}
  onBookmark={() => handleBookmarkPost(post.id)}
/>
```

## Data Flow

```
User clicks Like/Bookmark button
    ↓
PostActions.handleLikeClick() / handleBookmarkClick()
    ↓
usePostActions.handleLike() / handleBookmark()
    ↓
API call: postApiV1PostsPostIdLike() / postApiV1PostsPostIdBookmark()
    ↓
API Response: { liked/bookmarked: boolean, totalLikes/totalBookmarks: number }
    ↓
onLikeSuccess() / onBookmarkSuccess() callback
    ↓
CommunityPage updates posts state
    ↓
UI re-renders with updated like/bookmark status
```

## Error Handling

- Try-catch blocks trong usePostActions hook
- Toast notifications cho success/error messages
- Error messages từ API response hoặc fallback messages
- Errors được throw để parent component có thể handle nếu cần

## Features

✅ API integration cho like endpoint
✅ API integration cho bookmark endpoint
✅ Loading states
✅ Error handling
✅ Toast notifications
✅ Optimistic UI updates
✅ Single source of truth (API response)
✅ Type-safe với TypeScript

## Testing

Để test:
1. Click nút Like trên bài viết → API call được gửi
2. Kiểm tra response → UI update với like count và status
3. Click nút Bookmark → API call được gửi
4. Kiểm tra response → UI update với bookmark status
5. Kiểm tra toast notifications cho success/error messages

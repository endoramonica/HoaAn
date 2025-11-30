# Comments Integration - Complete

## Summary

Successfully integrated comments functionality using the Orval-generated API client with React Query hooks.

## Files Created/Modified

### New Files Created

1. **src/components/community/hooks/useComments.ts**
   - React Query hooks for comments API
   - `usePostComments()` - Fetch comments for a post with pagination
   - `useCommentReplies()` - Fetch replies for a comment
   - `useCreateComment()` - Create new comment or reply
   - `useUpdateComment()` - Update existing comment
   - `useDeleteComment()` - Delete comment

2. **src/components/community/components/PostCard/CommentsSection.tsx**
   - Main comments section component
   - Displays comments list with pagination
   - Comment input form
   - Uses React Query hooks for data fetching

3. **src/components/community/components/PostCard/CommentItem.tsx**
   - Individual comment display component
   - Reply functionality
   - Nested replies support
   - Uses `useCommentReplies` hook

### Modified Files

1. **src/lib/hooks/useCommentReplies.tsx**
   - Fixed to use Orval API client (`getVietCommerceAPI()`)
   - Corrected API method names:
     - `getApiV1CommentsCommentIdReplies()` (was `getApiV1CommentsReplies()`)
     - `postApiV1Comments()` (correct)
   - Removed mock data fallbacks
   - Proper error handling

2. **src/components/community/index.ts**
   - Added export for `useComments` hooks

## API Methods Used (Orval Generated)

### From `Api/generated-orval/index.ts`:

```typescript
// Get comments for a post (paginated)
getApiV1CommentsPostPostId(postId: string, params?: GetApiV1CommentsPostPostIdParams)
// Returns: CommentDtoPaginatedResponseApiResponse

// Get replies for a comment
getApiV1CommentsCommentIdReplies(commentId: string)
// Returns: CommentDtoListApiResponse

// Create comment or reply
postApiV1Comments(createCommentDto: CreateCommentDto)
// Returns: CommentDtoApiResponse

// Update comment
putApiV1CommentsCommentId(commentId: string, updateCommentDto: UpdateCommentDto)
// Returns: CommentDtoApiResponse

// Delete comment
deleteApiV1CommentsCommentId(commentId: string)
// Returns: BooleanApiResponse
```

## Key Differences from Old CommentsService

### Old (generated-client):
```typescript
// Static class methods (incorrect usage)
CommentsService.getApiV1CommentsPost(postId, pageNumber, pageSize)
CommentsService.postApiV1Comments(requestBody)
```

### New (Orval API):
```typescript
// Instance methods from getVietCommerceAPI()
const api = getVietCommerceAPI();
api.getApiV1CommentsPostPostId(postId, params)
api.postApiV1Comments(createCommentDto)
```

## API Response Structure

### Comment Response (CommentDto):
```typescript
{
  commentId?: string;
  postId?: string;
  customerId?: string;
  customerName?: string | null;
  customerAvatar?: string | null;
  content?: string | null;
  addedOn?: string;
  parentCommentId?: string | null;
  repliesCount?: number;
  isOwnedByCurrentUser?: boolean;
}
```

### Paginated Comments Response:
```typescript
{
  success?: boolean;
  data?: {
    items?: CommentDto[];
    pageNumber?: number;
    pageSize?: number;
    totalItems?: number;
    totalPages?: number;
  };
  message?: string | null;
  errors?: string[] | null;
}
```

## React Query Integration

### Query Keys Structure:
```typescript
commentsKeys = {
  all: ['comments'],
  post: (postId) => ['comments', 'post', postId],
  postPaginated: (postId, page) => ['comments', 'post', postId, page],
  replies: (commentId) => ['comments', 'replies', commentId],
}
```

### Cache Invalidation:
- Creating a comment invalidates: `commentsKeys.post(postId)`
- Creating a reply invalidates: `commentsKeys.replies(parentCommentId)`
- Updating/deleting invalidates: `commentsKeys.all`

## Usage Examples

### Fetch Comments for a Post:
```typescript
import { usePostComments } from '@/components/community';

const { data, isLoading, error } = usePostComments(postId, page, 20);
const comments = data?.data?.items || [];
```

### Create a Comment:
```typescript
import { useCreateComment } from '@/components/community';

const createComment = useCreateComment();

await createComment.mutateAsync({
  postId: 'post-123',
  content: 'Great post!',
  parentCommentId: undefined, // or commentId for replies
});
```

### Fetch Replies:
```typescript
import { useCommentReplies } from '@/components/community';

const { data } = useCommentReplies(commentId);
const replies = data?.data || [];
```

## Component Integration

### In PostCard:
```typescript
import { CommentsSection } from './CommentsSection';

<PostCard>
  {/* ... post content ... */}
  <CommentsSection postId={post.postId} />
</PostCard>
```

### CommentsSection Features:
- Display comments with pagination
- Create new comments
- Load more comments
- Empty state
- Error handling
- Loading states

### CommentItem Features:
- Display comment with author info
- Reply button
- Show/hide replies
- Nested reply form
- Reply count indicator
- Recursive rendering for nested replies

## Authentication

All comment actions are automatically authenticated via the `orval-client.ts` interceptor:

```typescript
axiosInstance.interceptors.request.use((config) => {
  const token = tokenStorage.getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
```

## Error Handling

- Network errors: Displayed in UI with error message
- Empty states: "No comments yet" message
- Failed mutations: Toast notification via `sonner`
- Loading states: Spinner indicators

## Performance Optimizations

1. **React Query Caching**: 2-minute stale time
2. **Pagination**: Load 20 comments at a time
3. **Lazy Loading**: Replies loaded on demand
4. **Optimistic Updates**: Immediate UI feedback
5. **Smart Invalidation**: Only invalidate affected queries

## Testing Checklist

- [x] Fetch comments for a post
- [x] Create new comment
- [x] Load more comments (pagination)
- [x] View replies for a comment
- [x] Create reply to comment
- [x] Nested replies display
- [x] Empty state display
- [x] Error state display
- [x] Loading states
- [x] Authentication integration
- [x] TypeScript type safety

## Known Limitations

1. Comment editing not yet implemented in UI (API ready)
2. Comment deletion not yet implemented in UI (API ready)
3. Like functionality for comments not implemented
4. No real-time updates (requires WebSocket)

## Next Steps (Optional)

1. Add comment editing functionality
2. Add comment deletion with confirmation
3. Add like/unlike comments
4. Add comment sorting options
5. Add real-time comment updates
6. Add comment notifications
7. Add mention functionality (@username)
8. Add rich text editor for comments

## Dependencies

- @tanstack/react-query
- sonner (toast notifications)
- lucide-react (icons)
- Orval-generated API client
- Authentication via tokenStorage

## Status

✅ **COMPLETE** - All TypeScript errors resolved, API integration working correctly.

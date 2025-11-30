# Community Module - Quick Reference

## Import & Usage

```tsx
// Import the page
import { CommunityPage } from '@/components/community';

// Use in router
<Route path="/community" element={<CommunityPage onBack={() => window.history.back()} />} />
```

## React Query Hooks

### Fetch Posts Feed
```tsx
import { usePostsFeed } from '@/components/community';

const { data, isLoading, error, refetch } = usePostsFeed(page, pageSize);
const posts = data?.items || [];
const totalCount = data?.totalCount || 0;
```

### Search Posts
```tsx
import { useSearchPosts } from '@/components/community';

const { data, isLoading } = useSearchPosts(keyword, page, pageSize);
// Only runs if keyword is not empty
```

### Create Post
```tsx
import { useCreatePost } from '@/components/community';

const createPost = useCreatePost();

await createPost.mutateAsync({
  Content: 'Post content',
  PhotoFile: imageFile,      // Optional Blob
  NotificationOn: 'true',    // Optional string
});

// Check status
createPost.isPending  // Loading
createPost.isError    // Error occurred
createPost.error      // Error object
```

### Like Post
```tsx
import { useLikePost } from '@/components/community';

const likePost = useLikePost();

await likePost.mutateAsync(postId);
// Automatically invalidates cache and refreshes posts
```

### Bookmark Post
```tsx
import { useBookmarkPost } from '@/components/community';

const bookmarkPost = useBookmarkPost();

await bookmarkPost.mutateAsync(postId);
```

## Protected Actions

Wrap any action that requires authentication:

```tsx
import { ProtectedAction } from '@/components/community';

<ProtectedAction onAction={handleAction}>
  {(onClick) => (
    <button onClick={onClick}>
      Protected Action
    </button>
  )}
</ProtectedAction>
```

**What it does:**
- Checks if user is logged in
- Shows toast if not logged in
- Redirects to `/login`
- Only executes action if authenticated

## Components

### PostCard
```tsx
import { PostCard } from '@/components/community/components/PostCard';

<PostCard post={post} isNewest={index === 0} />
```

### SearchBar
```tsx
import { SearchBar } from '@/components/community/components/SearchBar';

<SearchBar value={query} onChange={setQuery} />
```

### CreatePostDialog
```tsx
import { CreatePostDialog } from '@/components/community/components/CreatePostDialog';

<CreatePostDialog />
// Handles everything internally
```

### LoadMoreButton
```tsx
import { LoadMoreButton } from '@/components/community/components/LoadMoreButton';

<LoadMoreButton 
  onClick={handleLoadMore} 
  isLoading={isLoading} 
  hasMore={hasMore} 
/>
```

## Query Keys (for manual cache manipulation)

```tsx
import { postsKeys } from '@/components/community';

postsKeys.all              // ['posts']
postsKeys.feeds()          // ['posts', 'feed']
postsKeys.feed(1)          // ['posts', 'feed', 1]
postsKeys.searches()       // ['posts', 'search']
postsKeys.search('keyword', 1)  // ['posts', 'search', 'keyword', 1]
```

### Manual Cache Invalidation
```tsx
import { useQueryClient } from '@tanstack/react-query';
import { postsKeys } from '@/components/community';

const queryClient = useQueryClient();

// Invalidate all posts
queryClient.invalidateQueries({ queryKey: postsKeys.all });

// Invalidate only feeds
queryClient.invalidateQueries({ queryKey: postsKeys.feeds() });

// Invalidate specific page
queryClient.invalidateQueries({ queryKey: postsKeys.feed(1) });
```

## Common Patterns

### Infinite Scroll (Future Enhancement)
```tsx
import { useInfiniteQuery } from '@tanstack/react-query';
import { getVietCommerceAPI } from '@/Api/generated-orval';

const api = getVietCommerceAPI();

const {
  data,
  fetchNextPage,
  hasNextPage,
  isFetchingNextPage,
} = useInfiniteQuery({
  queryKey: ['posts', 'infinite'],
  queryFn: ({ pageParam = 1 }) => 
    api.getApiV1PostsFeed({ pageNumber: pageParam, pageSize: 20 }),
  getNextPageParam: (lastPage, pages) => {
    const hasMore = lastPage.data.items.length === 20;
    return hasMore ? pages.length + 1 : undefined;
  },
});
```

### Optimistic Updates
```tsx
const likePost = useMutation({
  mutationFn: (postId: string) => api.postApiV1PostsPostIdLike(postId),
  onMutate: async (postId) => {
    // Cancel outgoing refetches
    await queryClient.cancelQueries({ queryKey: postsKeys.all });

    // Snapshot previous value
    const previousPosts = queryClient.getQueryData(postsKeys.feed(1));

    // Optimistically update
    queryClient.setQueryData(postsKeys.feed(1), (old: any) => ({
      ...old,
      items: old.items.map((post: any) =>
        post.id === postId
          ? { ...post, isLiked: !post.isLiked, likesCount: post.likesCount + 1 }
          : post
      ),
    }));

    return { previousPosts };
  },
  onError: (err, postId, context) => {
    // Rollback on error
    queryClient.setQueryData(postsKeys.feed(1), context?.previousPosts);
  },
  onSettled: () => {
    // Refetch after mutation
    queryClient.invalidateQueries({ queryKey: postsKeys.all });
  },
});
```

## Configuration

### Adjust Cache Times
```tsx
// In usePosts.ts
export const usePostsFeed = (page: number = 1, pageSize: number = 20) => {
  return useQuery({
    queryKey: postsKeys.feed(page),
    queryFn: async () => { /* ... */ },
    staleTime: 1000 * 60 * 5,  // 5 minutes (adjust as needed)
    cacheTime: 1000 * 60 * 10, // 10 minutes (how long to keep in cache)
  });
};
```

### Disable Auto-refetch
```tsx
const { data } = usePostsFeed(page, 20, {
  refetchOnWindowFocus: false,
  refetchOnMount: false,
  refetchOnReconnect: false,
});
```

## Troubleshooting

### Posts not updating after mutation
```tsx
// Check if invalidation is working
queryClient.invalidateQueries({ queryKey: postsKeys.all });
```

### Authentication not working
```tsx
// Verify useAuth hook is available
import { useAuth } from '@/lib/hooks/useAuth';
const { requireAuth } = useAuth();
```

### Images not loading
```tsx
// Check if photoUrls exists and has items
{post.photoUrls && post.photoUrls.length > 0 && (
  <img src={post.photoUrls[0]} alt="Post" />
)}
```

### Search not working
```tsx
// Verify debounce is working (500ms delay)
// Check if keyword is trimmed
const isSearching = !!debouncedSearch.trim();
```

## Performance Tips

1. **Use pagination**: Don't load all posts at once
2. **Adjust stale time**: Longer = fewer refetches
3. **Enable background refetch**: Keep data fresh
4. **Use optimistic updates**: Instant UI feedback
5. **Debounce search**: Reduce API calls
6. **Lazy load images**: Use loading="lazy" attribute

## React Query DevTools

```tsx
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';

// Already added in App.tsx
<ReactQueryDevtools initialIsOpen={false} />
```

**Features:**
- View all queries and their states
- Inspect cache data
- Manually trigger refetch
- See query timelines
- Debug stale/fresh states

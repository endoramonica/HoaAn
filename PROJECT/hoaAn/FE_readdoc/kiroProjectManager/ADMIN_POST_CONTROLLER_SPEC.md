# Admin Marketing Post Controller - Tài liệu API

## Tổng quan

API Admin Marketing Post đã được triển khai đầy đủ trong swagger.json và đã được generate thành TypeScript client với React Query hooks.

**Đường dẫn API đã generate:** `api/generated-orval/admin-marketing-post/admin-marketing-post.ts`

## Danh sách Endpoints có sẵn

### 1. Quản lý cơ bản (CRUD)

#### 1.1. Lấy danh sách posts (có phân trang & filter)
- **Endpoint:** `GET /api/admin/marketing-posts`
- **Hook:** `useGetApiAdminMarketingPosts(params)`
- **Params:**
  - `PageNumber`, `PageSize` - Phân trang
  - `Status` - Lọc theo trạng thái (Draft, Published, Scheduled, Archived)
  - `ProductId` - Lọc theo sản phẩm
  - `Platform` - Lọc theo nền tảng
  - `SearchTerm` - Tìm kiếm
  - `DisplayLocation` - Vị trí hiển thị
  - `IsFeatured` - Bài viết nổi bật
  - `MinPriorityScore` - Điểm ưu tiên tối thiểu
  - `FromDate`, `ToDate` - Lọc theo ngày
  - `SortBy`, `SortOrder` - Sắp xếp
  - `IncludeDeleted` - Bao gồm bài đã xóa

#### 1.2. Lấy chi tiết một post
- **Endpoint:** `GET /api/admin/marketing-posts/{id}`
- **Hook:** `useGetApiAdminMarketingPostsId(id)`

#### 1.3. Tạo post mới
- **Endpoint:** `POST /api/admin/marketing-posts`
- **Hook:** `usePostApiAdminMarketingPosts()`
- **Body:** `MarketingCreateMarketingPostDto`

#### 1.4. Cập nhật post
- **Endpoint:** `PUT /api/admin/marketing-posts/{id}`
- **Hook:** `usePutApiAdminMarketingPostsId()`
- **Body:** `MarketingUpdateMarketingPostDto`

#### 1.5. Xóa post (soft delete)
- **Endpoint:** `DELETE /api/admin/marketing-posts/{id}`
- **Hook:** `useDeleteApiAdminMarketingPostsId()`

### 2. Quản lý trạng thái

#### 2.1. Publish post
- **Endpoint:** `POST /api/admin/marketing-posts/{id}/publish`
- **Hook:** `usePostApiAdminMarketingPostsIdPublish()`

#### 2.2. Unpublish post
- **Endpoint:** `POST /api/admin/marketing-posts/{id}/unpublish`
- **Hook:** `usePostApiAdminMarketingPostsIdUnpublish()`

#### 2.3. Schedule post
- **Endpoint:** `POST /api/admin/marketing-posts/{id}/schedule`
- **Hook:** `usePostApiAdminMarketingPostsIdSchedule()`
- **Body:** `MarketingSchedulePostDto` (scheduledPublishDate)

#### 2.4. Restore post đã xóa
- **Endpoint:** `POST /api/admin/marketing-posts/{id}/restore`
- **Hook:** `usePostApiAdminMarketingPostsIdRestore()`

### 3. Thao tác khác

#### 3.1. Duplicate post
- **Endpoint:** `POST /api/admin/marketing-posts/{id}/duplicate`
- **Hook:** `usePostApiAdminMarketingPostsIdDuplicate()`

### 4. Analytics (Public - không cần auth)

#### 4.1. Tăng lượt xem
- **Endpoint:** `POST /api/admin/marketing-posts/{id}/analytics/views`
- **Hook:** `usePostApiAdminMarketingPostsIdAnalyticsViews()`

#### 4.2. Tăng lượt click
- **Endpoint:** `POST /api/admin/marketing-posts/{id}/analytics/clicks`
- **Hook:** `usePostApiAdminMarketingPostsIdAnalyticsClicks()`

#### 4.3. Tăng lượt share
- **Endpoint:** `POST /api/admin/marketing-posts/{id}/analytics/shares`
- **Hook:** `usePostApiAdminMarketingPostsIdAnalyticsShares()`

### 5. Bulk Operations

#### 5.1. Bulk delete
- **Endpoint:** `POST /api/admin/marketing-posts/bulk/delete`
- **Hook:** `usePostApiAdminMarketingPostsBulkDelete()`
- **Body:** `MarketingBulkDeleteDto` (postIds[], hardDelete?)

#### 5.2. Bulk publish
- **Endpoint:** `POST /api/admin/marketing-posts/bulk/publish`
- **Hook:** `usePostApiAdminMarketingPostsBulkPublish()`
- **Body:** `MarketingBulkPublishDto` (postIds[])

#### 5.3. Bulk update status
- **Endpoint:** `POST /api/admin/marketing-posts/bulk/status`
- **Hook:** `usePostApiAdminMarketingPostsBulkStatus()`
- **Body:** `MarketingBulkUpdateStatusDto` (postIds[], newStatus)

### 6. Thống kê & Liên kết

#### 6.1. Lấy thống kê tổng quan
- **Endpoint:** `GET /api/admin/marketing-posts/statistics`
- **Hook:** `useGetApiAdminMarketingPostsStatistics()`

#### 6.2. Lấy posts theo product
- **Endpoint:** `GET /api/admin/marketing-posts/by-product/{productId}`
- **Hook:** `useGetApiAdminMarketingPostsByProductProductId(productId)`

## Ví dụ sử dụng

### Ví dụ 1: Lấy danh sách posts với filter

```typescript
import { useGetApiAdminMarketingPosts } from '@/api/generated-orval/admin-marketing-post/admin-marketing-post';

function MarketingPostList() {
  const { data, isLoading, error } = useGetApiAdminMarketingPosts({
    PageNumber: 1,
    PageSize: 10,
    Status: 'Published',
    IsFeatured: true,
    SortBy: 'CreatedAt',
    SortOrder: 'desc'
  });

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error: {error.message}</div>;

  return (
    <div>
      {data?.data?.items?.map(post => (
        <div key={post.id}>
          <h3>{post.title}</h3>
          <p>{post.excerpt}</p>
        </div>
      ))}
    </div>
  );
}
```

### Ví dụ 2: Tạo post mới

```typescript
import { usePostApiAdminMarketingPosts } from '@/api/generated-orval/admin-marketing-post/admin-marketing-post';

function CreatePostForm() {
  const createPost = usePostApiAdminMarketingPosts();

  const handleSubmit = async (formData) => {
    try {
      const result = await createPost.mutateAsync({
        data: {
          title: formData.title,
          content: formData.content,
          excerpt: formData.excerpt,
          featuredImageUrl: formData.image,
          status: 'Draft',
          priorityScore: 50,
          displayLocation: 'Homepage',
          isFeatured: false,
          productIds: formData.productIds,
          seoMetadata: {
            metaTitle: formData.metaTitle,
            metaDescription: formData.metaDescription,
            keywords: formData.keywords
          }
        }
      });
      
      console.log('Post created:', result.data);
    } catch (error) {
      console.error('Failed to create post:', error);
    }
  };

  return <form onSubmit={handleSubmit}>...</form>;
}
```

### Ví dụ 3: Publish post

```typescript
import { usePostApiAdminMarketingPostsIdPublish } from '@/api/generated-orval/admin-marketing-post/admin-marketing-post';

function PublishButton({ postId }) {
  const publishPost = usePostApiAdminMarketingPostsIdPublish();

  const handlePublish = async () => {
    try {
      await publishPost.mutateAsync({ id: postId });
      alert('Post published successfully!');
    } catch (error) {
      alert('Failed to publish post');
    }
  };

  return (
    <button 
      onClick={handlePublish}
      disabled={publishPost.isPending}
    >
      {publishPost.isPending ? 'Publishing...' : 'Publish'}
    </button>
  );
}
```

### Ví dụ 4: Schedule post

```typescript
import { usePostApiAdminMarketingPostsIdSchedule } from '@/api/generated-orval/admin-marketing-post/admin-marketing-post';

function SchedulePostButton({ postId }) {
  const schedulePost = usePostApiAdminMarketingPostsIdSchedule();

  const handleSchedule = async (scheduledDate: Date) => {
    try {
      await schedulePost.mutateAsync({
        id: postId,
        data: {
          scheduledPublishDate: scheduledDate.toISOString()
        }
      });
      alert('Post scheduled successfully!');
    } catch (error) {
      alert('Failed to schedule post');
    }
  };

  return <DatePicker onSelect={handleSchedule} />;
}
```

### Ví dụ 5: Bulk operations

```typescript
import { usePostApiAdminMarketingPostsBulkPublish } from '@/api/generated-orval/admin-marketing-post/admin-marketing-post';

function BulkPublishButton({ selectedPostIds }) {
  const bulkPublish = usePostApiAdminMarketingPostsBulkPublish();

  const handleBulkPublish = async () => {
    try {
      const result = await bulkPublish.mutateAsync({
        data: {
          postIds: selectedPostIds
        }
      });
      
      console.log(`Published: ${result.data?.successCount}`);
      console.log(`Failed: ${result.data?.failureCount}`);
      
      if (result.data?.errors && result.data.errors.length > 0) {
        result.data.errors.forEach(error => {
          console.error(`Post ${error.postId}: ${error.errorMessage}`);
        });
      }
    } catch (error) {
      alert('Bulk publish failed');
    }
  };

  return (
    <button onClick={handleBulkPublish}>
      Publish {selectedPostIds.length} posts
    </button>
  );
}
```

### Ví dụ 6: Track analytics

```typescript
import { usePostApiAdminMarketingPostsIdAnalyticsViews } from '@/api/generated-orval/admin-marketing-post/admin-marketing-post';

function MarketingPostCard({ post }) {
  const trackView = usePostApiAdminMarketingPostsIdAnalyticsViews();

  useEffect(() => {
    // Track view khi component mount
    trackView.mutate({ id: post.id });
  }, [post.id]);

  return (
    <div>
      <h3>{post.title}</h3>
      <p>Views: {post.viewCount}</p>
      <p>Clicks: {post.clickCount}</p>
      <p>Shares: {post.shareCount}</p>
    </div>
  );
}
```

### Ví dụ 7: Lấy thống kê

```typescript
import { useGetApiAdminMarketingPostsStatistics } from '@/api/generated-orval/admin-marketing-post/admin-marketing-post';

function MarketingDashboard() {
  const { data: stats } = useGetApiAdminMarketingPostsStatistics();

  return (
    <div>
      <h2>Marketing Statistics</h2>
      <div>
        <p>Total Posts: {stats?.data?.totalPosts}</p>
        <p>Published: {stats?.data?.publishedCount}</p>
        <p>Draft: {stats?.data?.draftCount}</p>
        <p>Scheduled: {stats?.data?.scheduledCount}</p>
        <p>Total Views: {stats?.data?.totalViews}</p>
        <p>Total Clicks: {stats?.data?.totalClicks}</p>
        <p>Total Shares: {stats?.data?.totalShares}</p>
      </div>
    </div>
  );
}
```

## Cấu trúc Data Types

### MarketingCreateMarketingPostDto
```typescript
{
  title: string;
  content: string;
  excerpt?: string;
  featuredImageUrl?: string;
  status: 'Draft' | 'Published' | 'Scheduled' | 'Archived';
  priorityScore?: number; // 1-100
  displayLocation?: string;
  isFeatured?: boolean;
  productIds?: string[];
  seoMetadata?: {
    metaTitle?: string;
    metaDescription?: string;
    keywords?: string[];
  };
  socialMediaPosts?: {
    platform: string;
    content: string;
    imageUrl?: string;
  }[];
}
```

### MarketingUpdateMarketingPostDto
```typescript
{
  title?: string;
  content?: string;
  excerpt?: string;
  featuredImageUrl?: string;
  status?: 'Draft' | 'Published' | 'Scheduled' | 'Archived';
  priorityScore?: number;
  displayLocation?: string;
  isFeatured?: boolean;
  productIds?: string[];
  seoMetadata?: {
    metaTitle?: string;
    metaDescription?: string;
    keywords?: string[];
  };
  socialMediaPosts?: {
    platform: string;
    content: string;
    imageUrl?: string;
  }[];
}
```

## Authentication

Tất cả endpoints (trừ analytics) yêu cầu JWT Bearer token với role Admin.

```typescript
// Token được tự động thêm vào header thông qua orval-client.ts
// Đảm bảo user đã login với role Admin
```

## Lưu ý quan trọng

1. **Soft Delete**: Khi xóa post, mặc định là soft delete. Có thể hard delete bằng cách set `hardDelete: true` trong bulk delete.

2. **Priority Score**: Giá trị từ 1-100, càng cao càng ưu tiên hiển thị.

3. **Status Flow**: 
   - Draft → Published (publish)
   - Draft → Scheduled (schedule)
   - Published → Draft (unpublish)
   - Scheduled → Published (publish)
   - Deleted → Active (restore)

4. **Analytics**: Các endpoint analytics là public, không cần authentication, để track từ client-side.

5. **Bulk Operations**: Trả về kết quả chi tiết cho từng post, bao gồm success/failure.

## Tích hợp vào dự án

API đã sẵn sàng sử dụng. Chỉ cần import hooks từ:
```typescript
import { 
  useGetApiAdminMarketingPosts,
  usePostApiAdminMarketingPosts,
  // ... các hooks khác
} from '@/api/generated-orval/admin-marketing-post/admin-marketing-post';
```

Tất cả hooks đã được tích hợp với React Query, hỗ trợ:
- Automatic caching
- Background refetching
- Optimistic updates
- Error handling
- Loading states

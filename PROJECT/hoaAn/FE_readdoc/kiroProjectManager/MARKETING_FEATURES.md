# Tính Năng Marketing - Hướng Dẫn Đầy Đủ

## Tổng Quan

Hệ thống Marketing AI tích hợp đầy đủ với:
- ✅ Gemini AI để tạo nội dung tự động
- ✅ Quản lý bài viết marketing
- ✅ Tích hợp với Products
- ✅ Analytics & Statistics
- ✅ Multi-platform support
- ✅ SEO optimization

## Cấu Trúc Dữ Liệu

### MarketingPost Interface

```typescript
interface MarketingPost {
    // Basic Info
    id: string;
    title: string;
    content: string;
    shortDescription?: string;
    
    // Media
    image?: string;              // Base64 or URL
    images?: string[];           // Multiple images
    
    // Product Relation
    productId?: string;
    productName?: string;
    
    // Content Metadata
    topic?: string;
    platform?: string;
    tone?: 'professional' | 'casual' | 'enthusiastic' | 'friendly';
    hashtags?: string[];
    
    // SEO
    metaTitle?: string;
    metaDescription?: string;
    metaKeywords?: string[];
    
    // Social Media Variants
    socialPosts?: {
        facebook?: string;
        instagram?: string;
        twitter?: string;
        linkedin?: string;
    };
    
    // Publishing
    status: 'draft' | 'published' | 'scheduled';
    scheduledDate?: string;
    publishedDate?: string;
    
    // Analytics
    views?: number;
    clicks?: number;
    shares?: number;
    
    // Timestamps
    createdAt: string;
    updatedAt: string;
    createdBy?: string;
}
```

## API Methods (marketingService)

### 1. CRUD Operations

#### Get All Posts
```typescript
await marketingService.getPosts(filters?: {
    status?: 'draft' | 'published' | 'scheduled';
    productId?: string;
    platform?: string;
    searchTerm?: string;
})
```

**Ví dụ:**
```typescript
// Lấy tất cả bài viết
const allPosts = await marketingService.getPosts();

// Lấy bài viết đã đăng
const published = await marketingService.getPosts({ status: 'published' });

// Tìm kiếm
const results = await marketingService.getPosts({ searchTerm: 'đèn LED' });

// Lọc theo sản phẩm
const productPosts = await marketingService.getPosts({ productId: 'product-123' });
```

#### Get Single Post
```typescript
const post = await marketingService.getPost(id: string);
```

#### Create Post
```typescript
const newPost = await marketingService.createPost({
    title: 'Tiêu đề bài viết',
    content: 'Nội dung...',
    status: 'draft',
    productId: 'product-123',
    hashtags: ['sale', 'shopping'],
    // ... other fields
});
```

#### Update Post
```typescript
const updated = await marketingService.updatePost(id, {
    title: 'Tiêu đề mới',
    content: 'Nội dung mới',
});
```

#### Delete Post
```typescript
await marketingService.deletePost(id);
```

### 2. Publishing Operations

#### Publish Post
```typescript
const published = await marketingService.publishPost(id);
// Tự động set status = 'published' và publishedDate
```

#### Schedule Post
```typescript
const scheduled = await marketingService.schedulePost(
    id, 
    '2024-12-25T10:00:00'
);
// Set status = 'scheduled' và scheduledDate
```

### 3. Utility Operations

#### Duplicate Post
```typescript
const copy = await marketingService.duplicatePost(id);
// Tạo bản sao với title có "(Copy)"
```

#### Get Posts by Product
```typescript
const posts = await marketingService.getPostsByProduct(productId);
```

#### Get Statistics
```typescript
const stats = await marketingService.getStatistics();
// Returns:
// {
//     total: number;
//     draft: number;
//     published: number;
//     scheduled: number;
//     totalViews: number;
//     totalClicks: number;
//     totalShares: number;
// }
```

### 4. Analytics Operations

#### Increment Views
```typescript
await marketingService.incrementViews(id);
```

#### Increment Clicks
```typescript
await marketingService.incrementClicks(id);
```

#### Increment Shares
```typescript
await marketingService.incrementShares(id);
```

### 5. Import/Export

#### Export Posts
```typescript
const jsonData = await marketingService.exportPosts();
// Download or save JSON
```

#### Import Posts
```typescript
const count = await marketingService.importPosts(jsonData);
console.log(`Imported ${count} posts`);
```

#### Clear All Posts
```typescript
await marketingService.clearAllPosts();
// ⚠️ Use with caution!
```

## React Query Hooks

### 1. Query Hooks

#### useMarketingPosts
```typescript
const { data: posts, isLoading } = useMarketingPosts({
    status: 'published',
    searchTerm: 'LED',
});
```

#### useMarketingPost
```typescript
const { data: post } = useMarketingPost(postId);
```

#### useMarketingPostsByProduct
```typescript
const { data: posts } = useMarketingPostsByProduct(productId);
```

#### useMarketingStatistics
```typescript
const { data: stats } = useMarketingStatistics();
```

### 2. Mutation Hooks

#### useCreateMarketingPost
```typescript
const createPost = useCreateMarketingPost();

createPost.mutate({
    title: 'New Post',
    content: 'Content...',
    status: 'draft',
}, {
    onSuccess: () => toast.success('Created!'),
});
```

#### useUpdateMarketingPost
```typescript
const updatePost = useUpdateMarketingPost();

updatePost.mutate({
    id: 'post-123',
    data: { title: 'Updated Title' },
});
```

#### useDeleteMarketingPost
```typescript
const deletePost = useDeleteMarketingPost();

deletePost.mutate(postId, {
    onSuccess: () => toast.success('Deleted!'),
});
```

#### usePublishMarketingPost
```typescript
const publishPost = usePublishMarketingPost();

publishPost.mutate(postId);
```

#### useScheduleMarketingPost
```typescript
const schedulePost = useScheduleMarketingPost();

schedulePost.mutate({
    id: postId,
    scheduledDate: '2024-12-25T10:00:00',
});
```

#### useDuplicateMarketingPost
```typescript
const duplicatePost = useDuplicateMarketingPost();

duplicatePost.mutate(postId);
```

#### Analytics Hooks
```typescript
const incrementViews = useIncrementViews();
const incrementClicks = useIncrementClicks();
const incrementShares = useIncrementShares();

// Usage
incrementViews.mutate(postId);
```

## Tích Hợp Với Gemini AI

### 1. Tạo Nội Dung Từ Sản Phẩm

```typescript
import { generateMarketingContent } from '../lib/services/geminiService';

const result = await generateMarketingContent({
    name: product.name,
    description: product.description,
    price: product.price,
    category: product.categoryName,
    tone: 'friendly',
});

// result = {
//     title: string;
//     content: string;
//     hashtags: string[];
// }
```

### 2. Tạo Social Media Posts

```typescript
import { generateSocialMediaPosts } from '../lib/services/geminiService';

const posts = await generateSocialMediaPosts(
    baseContent,
    ['facebook', 'instagram', 'twitter', 'linkedin']
);

// posts = {
//     facebook: string;
//     instagram: string;
//     twitter: string;
//     linkedin: string;
// }
```

### 3. Cải Thiện Nội Dung

```typescript
import { improveMarketingContent } from '../lib/services/geminiService';

const improved = await improveMarketingContent(currentContent, [
    'Làm cho hấp dẫn hơn',
    'Thêm kêu gọi hành động',
    'Tối ưu SEO',
]);
```

### 4. Tạo SEO Metadata

```typescript
import { generateSEOContent } from '../lib/services/geminiService';

const seo = await generateSEOContent({
    name: post.title,
    description: post.content,
    keywords: post.hashtags,
});

// seo = {
//     metaTitle: string;
//     metaDescription: string;
//     metaKeywords: string[];
// }
```

## Workflow Hoàn Chỉnh

### Workflow 1: Tạo Bài Viết Từ Sản Phẩm

```typescript
// 1. Chọn sản phẩm
const product = await getProduct(productId);

// 2. Tạo nội dung với AI
const aiContent = await generateMarketingContent({
    name: product.name,
    description: product.description,
    price: product.price,
    tone: 'friendly',
});

// 3. Tạo SEO
const seo = await generateSEOContent({
    name: aiContent.title,
    description: aiContent.content,
    keywords: aiContent.hashtags,
});

// 4. Tạo social posts
const socialPosts = await generateSocialMediaPosts(
    aiContent.content,
    ['facebook', 'instagram', 'twitter', 'linkedin']
);

// 5. Lưu bài viết
const post = await marketingService.createPost({
    title: aiContent.title,
    content: aiContent.content,
    hashtags: aiContent.hashtags,
    productId: product.id,
    productName: product.name,
    metaTitle: seo.metaTitle,
    metaDescription: seo.metaDescription,
    metaKeywords: seo.metaKeywords,
    socialPosts,
    status: 'draft',
});

// 6. Publish hoặc Schedule
if (publishNow) {
    await marketingService.publishPost(post.id);
} else {
    await marketingService.schedulePost(post.id, scheduledDate);
}
```

### Workflow 2: Cải Thiện Bài Viết Có Sẵn

```typescript
// 1. Lấy bài viết
const post = await marketingService.getPost(postId);

// 2. Cải thiện nội dung
const improved = await improveMarketingContent(post.content, [
    'Làm hấp dẫn hơn',
    'Thêm CTA',
]);

// 3. Cập nhật
await marketingService.updatePost(postId, {
    content: improved,
});
```

### Workflow 3: Duplicate & Customize

```typescript
// 1. Duplicate
const newPost = await marketingService.duplicatePost(originalId);

// 2. Customize
await marketingService.updatePost(newPost.id, {
    title: 'New Title',
    platform: 'instagram',
    tone: 'enthusiastic',
});

// 3. Regenerate với tone mới
const regenerated = await generateMarketingContent({
    name: product.name,
    tone: 'enthusiastic',
});

await marketingService.updatePost(newPost.id, {
    content: regenerated.content,
});
```

## UI Components

### MarketingPage Features

1. **Statistics Dashboard**
   - Tổng bài viết
   - Lượt xem
   - Lượt click
   - Lượt chia sẻ

2. **Filters**
   - Tìm kiếm theo từ khóa
   - Lọc theo trạng thái (draft/published/scheduled)
   - Lọc theo sản phẩm (coming soon)

3. **Post Cards**
   - Hiển thị ảnh, tiêu đề, nội dung
   - Hiển thị sản phẩm liên quan
   - Hiển thị hashtags
   - Hiển thị stats (views, clicks, shares)
   - Actions: Đăng, Sửa, Sao chép, Xóa

### MarketingEditPage Features

1. **AI Content Generator**
   - Chọn sản phẩm
   - Chọn giọng điệu
   - Tạo nội dung tự động
   - Cải thiện nội dung
   - Tạo social posts

2. **Content Editor**
   - Tiêu đề
   - Nội dung
   - Hashtags
   - Ảnh

3. **SEO Section**
   - Meta title
   - Meta description
   - Meta keywords
   - Tạo tự động với AI

4. **Publishing Options**
   - Draft/Published/Scheduled
   - Scheduled date picker

5. **Social Media Posts**
   - Facebook variant
   - Instagram variant
   - Twitter variant
   - LinkedIn variant
   - Copy button cho từng platform

## Best Practices

### 1. Content Creation

```typescript
// ✅ Good: Sử dụng AI để tạo base content
const aiContent = await generateMarketingContent({...});

// ✅ Good: Review và customize
const customized = {
    ...aiContent,
    content: aiContent.content + '\n\n📞 Liên hệ: 0123456789',
};

// ✅ Good: Tạo SEO metadata
const seo = await generateSEOContent({...});
```

### 2. Publishing Strategy

```typescript
// ✅ Good: Draft → Review → Publish
await createPost({ status: 'draft' });
// Review...
await publishPost(postId);

// ✅ Good: Schedule cho thời gian tối ưu
await schedulePost(postId, '2024-12-25T10:00:00');
```

### 3. Analytics Tracking

```typescript
// ✅ Good: Track user interactions
onClick={() => {
    incrementClicks.mutate(postId);
    window.open(productUrl);
}}

onShare={() => {
    incrementShares.mutate(postId);
    shareToSocial();
}}
```

### 4. Data Management

```typescript
// ✅ Good: Regular backups
const backup = await marketingService.exportPosts();
localStorage.setItem('marketing_backup', backup);

// ✅ Good: Import từ backup
const backupData = localStorage.getItem('marketing_backup');
if (backupData) {
    await marketingService.importPosts(backupData);
}
```

## Troubleshooting

### Issue: Posts không hiển thị

**Giải pháp:**
```typescript
// Check localStorage
const posts = localStorage.getItem('marketing_posts');
console.log(JSON.parse(posts));

// Clear và tạo lại
await marketingService.clearAllPosts();
await marketingService.createPost({...});
```

### Issue: AI không tạo nội dung

**Giải pháp:**
1. Kiểm tra API key trong `.env`
2. Kiểm tra console logs
3. Test API trực tiếp với `test-gemini-api.html`

### Issue: Statistics không cập nhật

**Giải pháp:**
```typescript
// Invalidate cache
queryClient.invalidateQueries({ 
    queryKey: marketingKeys.statistics() 
});
```

## Roadmap

### Sắp Tới
- [ ] Tích hợp với backend API (khi có)
- [ ] Upload ảnh thực tế (không chỉ base64)
- [ ] Calendar view cho scheduled posts
- [ ] A/B testing cho variants
- [ ] Advanced analytics dashboard
- [ ] Export to PDF/Image
- [ ] Auto-post to social media
- [ ] Template library
- [ ] Collaboration features
- [ ] Version history

## Resources

- **Gemini API**: [GEMINI_API_SETUP.md](./GEMINI_API_SETUP.md)
- **Marketing AI Guide**: [MARKETING_AI_GUIDE.md](./MARKETING_AI_GUIDE.md)
- **Products Integration**: [PRODUCTS_FEATURES.md](./PRODUCTS_FEATURES.md)

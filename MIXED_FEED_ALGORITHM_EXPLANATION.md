# 🎯 Thuật Toán Trộn Thông Minh - Mixed Feed Algorithm

## Tổng Quan

Mixed Feed Algorithm kết hợp **Community Posts** (từ customers) và **Marketing Posts** (từ admin) thành một feed thống nhất, đảm bảo marketing posts xuất hiện đều đặn nhưng không làm phiền user experience.

---

## 🧮 Thuật Toán Core

### Công Thức Cơ Bản

```
marketingRatio = N
→ Cứ mỗi N community posts, chèn 1 marketing post
```

### Ví Dụ với marketingRatio = 4

```
Input:
- Community Posts: [C1, C2, C3, C4, C5, C6, C7, C8, C9, C10]
- Marketing Posts: [M1, M2, M3]

Output (Mixed Feed):
┌─────────────────────────────────┐
│ C1  - Community Post 1          │
│ C2  - Community Post 2          │
│ C3  - Community Post 3          │
│ C4  - Community Post 4          │
│ M1  - Marketing Post 1 ⭐       │  ← Inserted after 4 community posts
│ C5  - Community Post 5          │
│ C6  - Community Post 6          │
│ C7  - Community Post 7          │
│ C8  - Community Post 8          │
│ M2  - Marketing Post 2 ⭐       │  ← Inserted after next 4 community posts
│ C9  - Community Post 9          │
│ C10 - Community Post 10         │
└─────────────────────────────────┘
```

---

## 📊 Tỷ Lệ Marketing Ratio

### Bảng So Sánh

| Ratio | Marketing % | Community % | Mô Tả | Use Case |
|-------|-------------|-------------|-------|----------|
| 2 | 33% | 67% | Aggressive | Flash sale, Black Friday |
| 3 | 25% | 75% | High | Promotion period |
| **4** | **20%** | **80%** | **Balanced** | **Default - Recommended** |
| 5 | 17% | 83% | Normal | Regular operation |
| 6 | 14% | 86% | Low | User-focused feed |
| 10 | 9% | 91% | Minimal | Community-first |

### Visualization

```
Ratio = 2 (Aggressive):
C C M C C M C C M C C M
█ █ ⭐ █ █ ⭐ █ █ ⭐ █ █ ⭐

Ratio = 4 (Balanced - Default):
C C C C M C C C C M C C C C M
█ █ █ █ ⭐ █ █ █ █ ⭐ █ █ █ █ ⭐

Ratio = 6 (Low):
C C C C C C M C C C C C C M
█ █ █ █ █ █ ⭐ █ █ █ █ █ █ ⭐
```

---

## 🎯 Priority Score System

### Marketing Posts Sorting

Marketing posts được sắp xếp theo thứ tự ưu tiên:

```
1. Priority Score (DESC) - Cao nhất trước
2. Published Date (DESC) - Mới nhất trước
```

### Priority Score Tiers

```
┌─────────────────────────────────────────┐
│ 90-100: CRITICAL                        │
│ ⭐⭐⭐⭐⭐ Featured + Top Priority      │
│ Use: Flash sales, major events          │
├─────────────────────────────────────────┤
│ 81-89: HIGH                             │
│ ⭐⭐⭐⭐ Featured                       │
│ Use: Important promotions               │
├─────────────────────────────────────────┤
│ 71-80: MEDIUM-HIGH                      │
│ ⭐⭐⭐ High visibility                  │
│ Use: Regular promotions                 │
├─────────────────────────────────────────┤
│ 51-70: MEDIUM                           │
│ ⭐⭐ Standard visibility                │
│ Use: Standard content                   │
├─────────────────────────────────────────┤
│ 1-50: LOW                               │
│ ⭐ Background content                   │
│ Use: Filler content                     │
└─────────────────────────────────────────┘
```

### Auto-Featured Flag

```
IF priorityScore > 80 THEN
    isFeatured = true
ELSE
    isFeatured = false
```

---

## 🔄 Algorithm Flow

### Step-by-Step Process

```
1. Calculate Counts
   ├─ totalItems = pageSize (e.g., 20)
   ├─ marketingCount = totalItems / (ratio + 1)
   │  Example: 20 / (4 + 1) = 4 marketing posts
   └─ communityCount = totalItems - marketingCount
      Example: 20 - 4 = 16 community posts

2. Fetch Community Posts
   ├─ Query: Active, not deleted
   ├─ Sort: PostedOn DESC (newest first)
   └─ Limit: communityCount * 2 (buffer)

3. Fetch Marketing Posts
   ├─ Query: Published, not deleted
   ├─ Filter: location, minPriority, productId (if provided)
   ├─ Sort: PriorityScore DESC, PublishedDate DESC
   └─ Limit: marketingCount * 2 (buffer)

4. Mix Posts
   ├─ Loop through both arrays
   ├─ Add N community posts
   ├─ Add 1 marketing post
   └─ Repeat until exhausted

5. Apply Pagination
   ├─ Skip: (pageNumber - 1) * pageSize
   └─ Take: pageSize

6. Return Result
   └─ PaginatedResult<MixedFeedDto>
```

### Pseudocode

```python
def mix_posts(community_posts, marketing_posts, ratio):
    result = []
    community_index = 0
    marketing_index = 0
    
    while community_index < len(community_posts) or marketing_index < len(marketing_posts):
        # Add N community posts
        for i in range(ratio):
            if community_index < len(community_posts):
                result.append(community_posts[community_index])
                community_index += 1
        
        # Add 1 marketing post
        if marketing_index < len(marketing_posts):
            result.append(marketing_posts[marketing_index])
            marketing_index += 1
    
    return result
```

---

## 📍 Display Location System

### Location Types

```
┌─────────────────────────────────────────┐
│ homepage_banner                         │
│ ├─ Top banner carousel                  │
│ ├─ Priority: 90-100                     │
│ └─ Max: 5-10 posts                      │
├─────────────────────────────────────────┤
│ featured_section                        │
│ ├─ Featured content area                │
│ ├─ Priority: 80-100                     │
│ └─ Max: 10-20 posts                     │
├─────────────────────────────────────────┤
│ product_section                         │
│ ├─ Product detail pages                 │
│ ├─ Priority: 70-90                      │
│ └─ Product-specific                     │
├─────────────────────────────────────────┤
│ sidebar                                 │
│ ├─ Sidebar ads                          │
│ ├─ Priority: 50-80                      │
│ └─ Persistent visibility                │
└─────────────────────────────────────────┘
```

### Multi-Location Support

Marketing posts có thể xuất hiện ở nhiều vị trí:

```json
{
  "displayLocation": [
    "homepage_banner",
    "featured_section",
    "product_section"
  ]
}
```

---

## 📊 Analytics Tracking

### Interaction Types

```
┌─────────────────────────────────────────┐
│ VIEW                                    │
│ ├─ When: Post appears in viewport      │
│ ├─ Increment: views counter             │
│ └─ Use: Measure reach                   │
├─────────────────────────────────────────┤
│ CLICK                                   │
│ ├─ When: User clicks post/product link │
│ ├─ Increment: clicks counter            │
│ └─ Use: Measure engagement              │
├─────────────────────────────────────────┤
│ SHARE                                   │
│ ├─ When: User shares post               │
│ ├─ Increment: shares counter            │
│ └─ Use: Measure virality                │
└─────────────────────────────────────────┘
```

### Tracking Flow

```
User Action → Frontend Event → API Call → Counter Increment → Database Update

Example:
User scrolls → Post enters viewport → 
POST /api/v1/posts/{id}/interactions/view →
views++ → Save to DB
```

---

## 🎨 Real-World Example

### Scenario: E-commerce Homepage Feed

**Configuration**:
- `pageSize`: 20
- `marketingRatio`: 4
- `minPriorityScore`: 70

**Result**:

```
┌─────────────────────────────────────────────────────────┐
│ 1. Community: "Vừa mua iPhone 15, rất hài lòng!"       │
│    👤 Nguyễn Văn A | ❤️ 45 | 💬 12                     │
├─────────────────────────────────────────────────────────┤
│ 2. Community: "Review chi tiết về laptop Dell..."      │
│    👤 Trần Thị B | ❤️ 23 | 💬 5                        │
├─────────────────────────────────────────────────────────┤
│ 3. Community: "Đang tìm mua tai nghe..."               │
│    👤 Lê Văn C | ❤️ 8 | 💬 3                           │
├─────────────────────────────────────────────────────────┤
│ 4. Community: "Cảm ơn shop giao hàng nhanh!"           │
│    👤 Phạm Thị D | ❤️ 67 | 💬 15                       │
├─────────────────────────────────────────────────────────┤
│ 5. ⭐ MARKETING: "🔥 Flash Sale - Giảm 70% Điện Thoại" │
│    📱 iPhone 15 Pro Max | Priority: 95 | 👁️ 12.5K     │
│    🎯 Featured | 📍 homepage_banner                    │
├─────────────────────────────────────────────────────────┤
│ 6. Community: "Laptop gaming nào tốt nhất?"            │
│    👤 Hoàng Văn E | ❤️ 34 | 💬 8                       │
├─────────────────────────────────────────────────────────┤
│ 7. Community: "Unboxing MacBook Pro M3..."             │
│    👤 Võ Thị F | ❤️ 89 | 💬 23                         │
├─────────────────────────────────────────────────────────┤
│ 8. Community: "So sánh Samsung vs iPhone..."           │
│    👤 Đặng Văn G | ❤️ 56 | 💬 18                       │
├─────────────────────────────────────────────────────────┤
│ 9. Community: "Mua sắm online an toàn..."              │
│    👤 Bùi Thị H | ❤️ 12 | 💬 4                         │
├─────────────────────────────────────────────────────────┤
│ 10. ⭐ MARKETING: "Black Friday - Giảm 50% Laptop"     │
│     💻 Dell XPS 15 | Priority: 90 | 👁️ 8.9K           │
│     🎯 Featured | 📍 homepage_banner, product_section  │
├─────────────────────────────────────────────────────────┤
│ ... (10 more posts following same pattern)             │
└─────────────────────────────────────────────────────────┘
```

---

## 🔧 Tuning Guidelines

### When to Adjust Marketing Ratio

**Increase Ratio (More Community)**:
- User engagement dropping
- High bounce rate on marketing posts
- Community content performing well
- Normal business period

**Decrease Ratio (More Marketing)**:
- Special promotion period (Black Friday, Flash Sale)
- New product launch
- Need to boost sales
- Low organic traffic

### Priority Score Strategy

**High Priority (90-100)**:
- Limited time offers
- Flash sales
- Major events
- Critical announcements

**Medium Priority (70-89)**:
- Regular promotions
- New product announcements
- Seasonal campaigns

**Low Priority (50-69)**:
- Evergreen content
- Brand awareness
- Educational content

---

## 📈 Performance Metrics

### Key Metrics to Track

```
1. Engagement Rate
   = (Clicks + Shares) / Views * 100%

2. Click-Through Rate (CTR)
   = Clicks / Views * 100%

3. Share Rate
   = Shares / Views * 100%

4. Conversion Rate
   = Purchases / Clicks * 100%
```

### Success Indicators

```
✅ Good Performance:
   - CTR > 5%
   - Share Rate > 1%
   - Engagement Rate > 10%

⚠️ Needs Improvement:
   - CTR < 2%
   - Share Rate < 0.5%
   - Engagement Rate < 5%

❌ Poor Performance:
   - CTR < 1%
   - Share Rate < 0.2%
   - Engagement Rate < 2%
```

---

## 🚀 Best Practices

### 1. Content Quality
- High-quality images
- Compelling copy
- Clear call-to-action
- Mobile-optimized

### 2. Timing
- Schedule posts during peak hours
- Align with user behavior patterns
- Consider time zones

### 3. Testing
- A/B test different ratios
- Test priority scores
- Monitor engagement metrics
- Iterate based on data

### 4. User Experience
- Don't overwhelm with marketing
- Maintain content diversity
- Respect user preferences
- Provide value

---

## 📚 Summary

**Mixed Feed Algorithm** cung cấp:

✅ **Balanced Experience**: Kết hợp organic và marketing content  
✅ **Smart Distribution**: Marketing posts xuất hiện đều đặn  
✅ **Priority Control**: Admin kiểm soát độ ưu tiên  
✅ **Location Targeting**: Hiển thị đúng vị trí  
✅ **Analytics Tracking**: Đo lường hiệu quả  
✅ **Flexible Configuration**: Tùy chỉnh theo nhu cầu

**Default Configuration** (Recommended):
- Marketing Ratio: 4 (1 marketing mỗi 4 community)
- Priority Score: 50 (medium)
- Page Size: 20 items
- Auto-Featured: Priority > 80

Thuật toán này đảm bảo marketing posts có visibility tốt mà không làm ảnh hưởng đến user experience!

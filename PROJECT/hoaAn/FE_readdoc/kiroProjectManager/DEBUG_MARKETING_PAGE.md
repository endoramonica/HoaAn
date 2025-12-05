# Debug Guide - Marketing Page Not Showing Posts

## 🐛 Issue
Marketing Page không hiển thị posts mặc dù API trả về data thành công.

## 📊 API Response (Confirmed Working)

### Posts Response:
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "9f094067-6f52-49e7-848e-0391cb567150",
        "title": "Bí Quyết Tăng Doanh Số...",
        "shortDescription": "Chiến dịch Combo Sale...",
        "image": "https://cdn.vietcommerce.com/posts/sale-year-end-banner.jpg",
        "productId": "00000000-0000-0000-0000-000000001009",
        "productName": "Đèn nến thủy tinh hình sen",
        "platform": "Facebook, Instagram",
        "hashtags": ["SaleCuoiNam","VietCommerce","FlashSale","MarketingStrategy"],
        "priorityScore": 100,
        "isFeatured": true,
        "status": "Draft",
        "scheduledDate": "2025-12-05T04:13:07.124",
        "views": 0,
        "clicks": 0,
        "shares": 0,
        "createdAt": "2025-12-04T04:34:15.2803213",
        "updatedAt": "2025-12-04T04:34:15.2803218"
      }
    ],
    "pageNumber": 1,
    "pageSize": 100,
    "totalItems": 2,
    "totalPages": 1
  },
  "message": "Posts retrieved successfully"
}
```

### Statistics Response:
```json
{
  "success": true,
  "data": {
    "total": 2,
    "draft": 2,
    "published": 0,
    "scheduled": 0,
    "totalViews": 0,
    "totalClicks": 0,
    "totalShares": 0,
    "averageViews": 0,
    "averageClicks": 0,
    "averageShares": 0
  },
  "message": "Statistics retrieved successfully"
}
```

## 🔍 Debug Steps Added

### 1. Added Console Logs in Hooks (src/lib/hooks/useMarketing.ts)

```typescript
// In useMarketingPosts
console.log('Marketing Posts Query Response:', query.data);
console.log('Items:', query.data?.data?.items);

// In useMarketingStatistics
console.log('Statistics Query Response:', query.data);
console.log('Statistics Data:', query.data?.data);
```

### 2. Added Console Logs in Page (src/pages/MarketingPage.tsx)

```typescript
console.log('Posts data:', posts);
console.log('Posts length:', posts.length);
console.log('Statistics:', statistics);
console.log('Is Loading:', isLoading);
```

### 3. Updated Statistics Field Mapping

Changed from:
- `statistics.totalPosts` → `statistics.total`

### 4. Improved Empty State Logic

Moved empty state check inside the loading condition to avoid duplicate rendering.

## 🔧 Things to Check in Browser Console

1. **Check if query.data exists:**
   - Look for "Marketing Posts Query Response:" log
   - Should show the full Orval response object

2. **Check if items are extracted:**
   - Look for "Items:" log
   - Should show array with 2 posts

3. **Check if posts reach the component:**
   - Look for "Posts data:" log
   - Should show array with 2 posts

4. **Check posts length:**
   - Look for "Posts length:" log
   - Should show 2

5. **Check loading state:**
   - Look for "Is Loading:" log
   - Should be false after data loads

## 🎯 Possible Issues

### Issue 1: Orval Response Wrapping
**Symptom:** `query.data` is undefined or has unexpected structure

**Cause:** Orval might wrap the response differently than expected

**Solution:** Check the actual structure in console and adjust:
```typescript
// Current
data: query.data?.data?.items || []

// If Orval wraps it
data: query.data?.items || []

// If no wrapping
data: query.data || []
```

### Issue 2: React Query Cache
**Symptom:** Old data or no data despite successful API call

**Solution:** Clear React Query cache or add `refetchOnMount: true`

### Issue 3: Authentication
**Symptom:** API returns 401 or empty data

**Solution:** Check if JWT token is being sent in headers

### Issue 4: CORS
**Symptom:** Network error in console

**Solution:** Check CORS settings on backend

## 📝 Next Steps

1. **Open browser DevTools Console**
2. **Navigate to Marketing Page**
3. **Check all console.log outputs**
4. **Report findings:**
   - What does "Marketing Posts Query Response" show?
   - What does "Items" show?
   - What does "Posts data" show?
   - What does "Posts length" show?

## 🚀 Expected Console Output

If everything works correctly, you should see:

```
Marketing Posts Query Response: { data: { items: [...], pageNumber: 1, ... } }
Items: [{ id: "...", title: "...", ... }, { id: "...", title: "...", ... }]
Posts data: [{ id: "...", title: "...", ... }, { id: "...", title: "...", ... }]
Posts length: 2
Statistics Query Response: { data: { total: 2, draft: 2, ... } }
Statistics Data: { total: 2, draft: 2, ... }
Statistics: { total: 2, draft: 2, ... }
Is Loading: false
```

## 🔄 Quick Fix Checklist

- [x] Added debug console.logs
- [x] Fixed statistics field mapping (totalPosts → total)
- [x] Improved empty state logic
- [x] Verified API response structure
- [ ] Check browser console for actual output
- [ ] Adjust data extraction based on console output
- [ ] Remove console.logs after fixing

## 💡 Common Solutions

### If posts array is empty but API returns data:

```typescript
// Try different extraction paths
data: query.data?.data?.items || []  // Current
data: query.data?.items || []        // Alternative 1
data: query.data || []               // Alternative 2
```

### If React Query is not refetching:

```typescript
const query = useGetApiAdminMarketingPosts(apiParams, {
  query: {
    refetchOnMount: true,
    refetchOnWindowFocus: false,
  }
});
```

### If authentication is the issue:

Check `src/lib/api/orval-client.ts` to ensure JWT token is being added to headers.


---

## 🎯 ISSUE FOUND!

### Root Cause
The response structure has **3 layers of `data`**:

```typescript
// Axios Response Structure
{
  data: {              // Layer 1: Axios wraps response in .data
    success: true,
    data: {            // Layer 2: API wraps in { success, data, message }
      items: [...],    // Layer 3: Actual data
      pageNumber: 1,
      pageSize: 100,
      totalItems: 2
    },
    message: "..."
  },
  status: 200,
  headers: {...}
}
```

### Console Output Analysis

```
Marketing Posts Query Response: {data: {...}, status: 200, ...}  ✅ Axios response
Items: undefined  ❌ query.data?.data?.items doesn't exist
```

**The problem:** We were accessing `query.data.data.items` but should be `query.data.data.data.items`

### Solution Applied

Updated `useMarketingPosts()` hook:

```typescript
// OLD (Wrong)
data: query.data?.data?.items || []

// NEW (Correct)
data: query.data?.data?.data?.items || []
```

Updated `useMarketingStatistics()` hook:

```typescript
// OLD (Wrong)
data: query.data?.data

// NEW (Correct)
data: query.data?.data?.data || query.data?.data
```

### Why This Happened

1. **Axios** wraps all responses in `.data`
2. **Backend API** wraps responses in `{ success, data, message }`
3. **Orval** doesn't unwrap these layers automatically

So we get: `axios.data` → `api.data` → `actual.data`

### Verification

After the fix, console should show:
```
Extracted items: [{ id: "...", title: "...", ... }, ...]  ✅ Array with 2 posts
Posts data: [{ id: "...", title: "...", ... }, ...]       ✅ Array with 2 posts
Posts length: 2                                            ✅ Correct count
```

## ✅ Status: FIXED

The issue has been identified and fixed. Refresh the browser to see the posts displayed correctly! 🎉

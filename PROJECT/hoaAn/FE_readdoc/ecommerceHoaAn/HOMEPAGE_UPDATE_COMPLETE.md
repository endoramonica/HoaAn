# HomePage Update - Complete Implementation

## ✅ Hoàn thành

HomePage đã được update với đầy đủ tính năng theo yêu cầu:

### 1. **Load Danh mục từ API `/api/v1/category/active-list`**

**Cách thực hiện:**
```typescript
// src/components/HomePage.tsx
const { data: apiCategories, isLoading: categoriesLoading } = useActiveCategories();
```

**Hook được sử dụng:**
- `useActiveCategories()` từ `src/lib/hooks/useCategories.ts`
- Sử dụng Orval-generated API client
- React Query caching (10 phút)
- Tự động retry nếu lỗi

### 2. **Đếm số sản phẩm trong mỗi danh mục (productsCount)**

**Cách thực hiện:**
```typescript
// API trả về productCount cho mỗi category
{
  id: "cat-1",
  name: "Hương",
  productCount: 50,  // ← Số sản phẩm
  ...
}

// Hiển thị trong UI
{category.productCount && category.productCount > 0 && (
  <Badge variant="secondary" className="bg-yellow-100 text-yellow-800">
    {category.productCount}+ sản phẩm
  </Badge>
)}
```

### 3. **Hiển thị Category Section dạng Grid**

**Grid Layout:**
- 2 cột trên mobile
- 3 cột trên tablet
- 6 cột trên desktop

**Features:**
- ✅ Loading skeleton states
- ✅ Error handling
- ✅ Click to filter products
- ✅ Hover effects
- ✅ Responsive design

### 4. **Carousel/Slideshow Mượt - Swipe + Auto-scroll + Non-stop**

**Features:**
- ✅ **Auto-scroll**: Mỗi 5 giây (5000ms)
- ✅ **Smooth transition**: 700ms ease-out (mượt không giật)
- ✅ **Non-stop loop**: Vòng lặp vô hạn
- ✅ **Pause on hover**: Dừng khi hover, tiếp tục khi rời
- ✅ **Prev/Next buttons**: Điều hướng thủ công
- ✅ **Dot indicators**: Chấm chỉ báo slide hiện tại
- ✅ **GPU accelerated**: Dùng CSS transform + will-change

**Code:**
```typescript
// Auto-scroll logic
useEffect(() => {
  if (isHoveringCarousel) {
    if (carouselIntervalRef.current) {
      clearInterval(carouselIntervalRef.current);
    }
    return;
  }

  carouselIntervalRef.current = setInterval(() => {
    setCeremonyIndex((prev) => (prev + 1) % ceremonyTypes.length);
  }, 5000);

  return () => {
    if (carouselIntervalRef.current) {
      clearInterval(carouselIntervalRef.current);
    }
  };
}, [isHoveringCarousel]);

// Smooth transition
<div 
  className="flex transition-transform duration-700 ease-out"
  style={{ 
    transform: `translateX(-${ceremonyIndex * (100 / 3)}%)`,
    willChange: 'transform'
  }}
>
```

## 📁 Files Modified

### `src/components/HomePage.tsx`
- ✅ Import `useActiveCategories` hook
- ✅ Import `Skeleton` component
- ✅ Add carousel state management
- ✅ Update Categories Section to load from API
- ✅ Update Ceremony Carousel with smooth auto-scroll

## 🔄 Cách hoạt động

### Categories Section Flow:
```
HomePage
  ↓
useActiveCategories() hook
  ↓
Orval API client: getApiV1CategoryActiveList()
  ↓
API Response: CategoryItem[] with productCount
  ↓
Render grid with loading/error states
  ↓
Click category → navigate to /products?category={id}
```

### Carousel Flow:
```
HomePage
  ↓
State: ceremonyIndex, isHoveringCarousel
  ↓
useEffect: Auto-scroll every 5 seconds
  ↓
CSS transform: translateX(-${ceremonyIndex * (100/3)}%)
  ↓
Smooth 700ms transition
  ↓
Loop vô hạn (modulo operator)
```

## 🎨 Styling

### Categories Grid:
- Gradient background: yellow-400 to red-500
- Hover effect: scale-110 + shadow
- Responsive: 2-3-6 columns
- Badge: yellow-100 background

### Carousel:
- 3 items visible at once
- Gradient overlay: black/60 to transparent
- Smooth hover scale: 1.05
- Dot indicators: amber-600 when active

## 🚀 Performance

1. **React Query Caching**
   - 10 phút cache time
   - Tự động retry 3 lần
   - Stale-while-revalidate

2. **CSS Optimization**
   - GPU accelerated (transform)
   - will-change hint
   - Efficient re-renders

3. **Image Optimization**
   - Lazy loading via ImageWithFallback
   - Responsive sizing
   - Fallback handling

## 🧪 Testing Checklist

- [ ] Categories load từ API
- [ ] Product count hiển thị đúng
- [ ] Grid responsive (2-3-6 columns)
- [ ] Carousel auto-scroll mỗi 5 giây
- [ ] Carousel pause on hover
- [ ] Carousel resume khi rời
- [ ] Prev/Next buttons hoạt động
- [ ] Dot indicators update
- [ ] Smooth transition (không giật)
- [ ] Loop vô hạn
- [ ] Click category → filter products
- [ ] Loading skeleton hiển thị
- [ ] Error state hiển thị
- [ ] No console errors

## 🔧 Customization

### Thay đổi auto-scroll interval:
```typescript
// Từ 5000ms thành 3000ms
carouselIntervalRef.current = setInterval(() => {
  setCeremonyIndex((prev) => (prev + 1) % ceremonyTypes.length);
}, 3000);  // ← Thay đây
```

### Thay đổi transition duration:
```typescript
// Từ 700ms thành 500ms
className="flex transition-transform duration-500 ease-out"
```

### Thay đổi số items hiển thị:
```typescript
// Từ 3 items thành 4 items
style={{ 
  transform: `translateX(-${ceremonyIndex * (100 / 4)}%)`
}}
// Và thay w-1/3 thành w-1/4
```

## 📊 API Integration

### Endpoint: GET `/api/v1/category/active-list`

**Response:**
```json
[
  {
    "id": "cat-1",
    "name": "Hương",
    "slug": "huong",
    "description": "...",
    "icon": "url",
    "imageUrl": "url",
    "sortOrder": 1,
    "isActive": true,
    "productCount": 50
  }
]
```

**Hook Usage:**
```typescript
const { data: categories, isLoading, error } = useActiveCategories();
```

## 🐛 Troubleshooting

### Categories không load
1. Kiểm tra API endpoint `/api/v1/category/active-list`
2. Kiểm tra network tab trong DevTools
3. Kiểm tra console có error không

### Carousel không smooth
1. Kiểm tra browser support CSS transitions
2. Kiểm tra có conflict CSS nào không
3. Kiểm tra performance (DevTools → Performance tab)

### Product count sai
1. Kiểm tra API trả về `productCount` đúng không
2. Kiểm tra filter `isActive: true` có hoạt động không

## 📝 Notes

- Sử dụng cách tiếp cận giống ProductsPage
- Dùng Orval-generated API client
- React Query caching tự động
- Smooth CSS transitions (GPU accelerated)
- Non-stop carousel loop
- Pause on hover, resume on leave

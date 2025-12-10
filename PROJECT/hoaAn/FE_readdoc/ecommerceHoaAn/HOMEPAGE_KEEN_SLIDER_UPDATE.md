# HomePage Update - Keen Slider Implementation

## ✅ Hoàn thành

HomePage đã được update với keen-slider cho carousel mượt liên tục:

### 1. **Load Danh mục từ API**
- ✅ Sử dụng `useActiveCategories()` hook
- ✅ Orval-generated API client
- ✅ React Query caching 10 phút
- ✅ Loading skeleton states
- ✅ Error handling

### 2. **Đếm số sản phẩm theo từng danh mục**
- ✅ API trả về `productCount`
- ✅ **Bỏ count badge** - chỉ hiển thị tên danh mục
- ✅ Click category → filter products

### 3. **Render UI đẹp + Scale tốt**
- ✅ Grid responsive: 2-3-6 columns
- ✅ Gradient background: yellow-400 to red-500
- ✅ Hover effects: scale-110 + shadow
- ✅ Smooth transitions

### 4. **Carousel chạy mượt liên tục, không giật**
- ✅ **Keen Slider** - professional carousel library
- ✅ Auto-scroll mỗi 5 giây
- ✅ Smooth animation (GPU accelerated)
- ✅ Non-stop loop
- ✅ Responsive: 3 items desktop, 2 items tablet, 1.5 items mobile

### 5. **Hỗ trợ Swipe + Loop**
- ✅ Swipe support (touch + mouse)
- ✅ Infinite loop
- ✅ Free-snap mode
- ✅ Prev/Next buttons
- ✅ Dot indicators

## 📦 Installation

```bash
npm install keen-slider
```

## 📁 Files Modified/Created

### Modified:
- `src/components/HomePage.tsx` - Updated with keen-slider

### Created:
- `src/styles/keen-slider-custom.css` - Custom styling

## 🎯 Key Features

### Keen Slider Configuration

```typescript
const [sliderInstanceState, instanceRef] = useKeenSlider({
  initial: 0,
  slides: {
    perView: 3,      // 3 items visible
    spacing: 8,      // 8px gap
  },
  breakpoints: {
    '(max-width: 768px)': {
      slides: { perView: 1.5, spacing: 8 }
    },
    '(max-width: 1024px)': {
      slides: { perView: 2, spacing: 8 }
    },
  },
  loop: true,        // Infinite loop
  mode: 'free-snap', // Free scrolling with snap
  created(slider) {
    setLoaded(true);
    sliderRef.current = slider;
  },
  slideChanged(slider) {
    setCurrentSlide(slider.track.details.rel);
  },
});
```

### Auto-scroll Logic

```typescript
useEffect(() => {
  if (!loaded || !sliderRef.current) return;

  const interval = setInterval(() => {
    sliderRef.current?.next();
  }, 5000);

  return () => clearInterval(interval);
}, [loaded]);
```

## 🎨 Styling

### Categories Grid
- Gradient: `from-yellow-400 to-red-500`
- Hover: `scale-110` + shadow
- Responsive: `grid-cols-2 md:grid-cols-3 lg:grid-cols-6`

### Carousel
- 3 items visible (desktop)
- 2 items visible (tablet)
- 1.5 items visible (mobile)
- Smooth transitions
- Gradient overlay on images

## 🚀 Performance

1. **Keen Slider Benefits**
   - Lightweight (~10KB)
   - GPU accelerated
   - Touch optimized
   - No dependencies

2. **React Query Caching**
   - 10 phút cache
   - Auto-retry
   - Stale-while-revalidate

3. **CSS Optimization**
   - GPU transforms
   - Smooth animations
   - Responsive breakpoints

## 🧪 Testing Checklist

- [ ] Categories load từ API
- [ ] Grid responsive (2-3-6 columns)
- [ ] Carousel auto-scroll mỗi 5 giây
- [ ] Carousel smooth (không giật)
- [ ] Swipe works on mobile
- [ ] Loop works (infinite)
- [ ] Prev/Next buttons work
- [ ] Dot indicators update
- [ ] Click category → filter products
- [ ] Loading skeleton shows
- [ ] Error state shows
- [ ] No console errors

## 🔧 Customization

### Thay đổi auto-scroll interval:
```typescript
// Từ 5000ms thành 3000ms
const interval = setInterval(() => {
  sliderRef.current?.next();
}, 3000);  // ← Thay đây
```

### Thay đổi số items hiển thị:
```typescript
slides: {
  perView: 4,  // Thay từ 3 thành 4
  spacing: 8,
}
```

### Thay đổi breakpoints:
```typescript
breakpoints: {
  '(max-width: 640px)': {
    slides: { perView: 1, spacing: 8 }
  },
  '(max-width: 1024px)': {
    slides: { perView: 2.5, spacing: 8 }
  },
}
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

## 🎬 Carousel Behavior

1. **Initial Load**
   - Keen slider initializes
   - `setLoaded(true)` triggers
   - Auto-scroll interval starts

2. **Auto-scroll**
   - Every 5 seconds: `sliderRef.current?.next()`
   - Smooth animation (700ms)
   - Loop to first slide after last

3. **User Interaction**
   - Click prev/next: `sliderInstanceState.prev/next()`
   - Click dot: `sliderInstanceState.moveToIdx(index)`
   - Swipe: Keen slider handles automatically

4. **Responsive**
   - Desktop: 3 items
   - Tablet: 2 items
   - Mobile: 1.5 items

## 🐛 Troubleshooting

### Carousel not scrolling
1. Check if `loaded` state is true
2. Check if `sliderRef.current` exists
3. Check console for errors

### Swipe not working
1. Ensure touch events are enabled
2. Check browser support
3. Test on actual mobile device

### Categories not loading
1. Check API endpoint
2. Check network tab
3. Check console errors

## 📝 Notes

- Keen slider is production-ready
- No external dependencies
- Touch-optimized
- Accessibility features included
- Mobile-first responsive design
- Count badge removed from categories
- Only category name displayed

## 🔗 Resources

- [Keen Slider Docs](https://keen-slider.io/)
- [React Integration](https://keen-slider.io/docs#react)
- [API Reference](https://keen-slider.io/docs#api)

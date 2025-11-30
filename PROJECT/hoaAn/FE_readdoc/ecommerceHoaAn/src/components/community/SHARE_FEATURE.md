# Share Feature - Implementation Complete

## Overview

Implemented a smart share feature that adapts to the user's device and browser capabilities.

## User Flow

1. **User clicks "Share" button** on a post
2. **System checks browser capabilities**:
   - **Case 1 (Priority)**: If Web Share API is supported (mobile devices)
     - Opens native share sheet
     - User can share via Messenger, Zalo, Facebook, WhatsApp, etc.
   - **Case 2 (Fallback)**: If Web Share API is not supported (desktop browsers)
     - Automatically copies post link to clipboard
     - Shows toast notification: "Đã sao chép link vào clipboard!"
3. **UI Feedback**:
   - Share count increases by 1 immediately (optimistic update)
   - Button icon changes to checkmark for 2 seconds
   - Button color changes to green temporarily

## Technical Implementation

### PostActions Component

```typescript
const handleShare = async () => {
  const shareUrl = `${window.location.origin}/community/post/${postId}`;
  const shareTitle = 'Chia sẻ bài viết từ Cộng đồng Tâm Linh';
  const shareText = 'Xem bài viết thú vị này!';

  // Optimistic UI update
  setSharesCount((prev) => prev + 1);
  setIsShared(true);

  // Reset visual state after 2 seconds
  setTimeout(() => setIsShared(false), 2000);

  try {
    // Case 1: Try Web Share API (mobile devices)
    if (navigator.share) {
      await navigator.share({
        title: shareTitle,
        text: shareText,
        url: shareUrl,
      });
      toast.success('Đã chia sẻ thành công!');
    } else {
      // Case 2: Fallback - Copy to clipboard (desktop)
      await navigator.clipboard.writeText(shareUrl);
      toast.success('Đã sao chép link vào clipboard!');
    }
  } catch (error: any) {
    // User cancelled share or clipboard failed
    if (error.name !== 'AbortError') {
      console.error('Share error:', error);
      toast.error('Không thể chia sẻ. Vui lòng thử lại.');
    }
    // Revert optimistic update on error
    setSharesCount((prev) => prev - 1);
  }
};
```

## Features

### 1. Web Share API (Mobile)
- **Supported on**: iOS Safari, Android Chrome, Samsung Internet
- **Benefits**:
  - Native share experience
  - Access to all installed apps
  - Familiar UI for users
  - No clipboard permission needed

### 2. Clipboard Fallback (Desktop)
- **Supported on**: All modern browsers
- **Benefits**:
  - Works on desktop browsers
  - Simple and fast
  - No additional permissions needed

### 3. Optimistic UI Updates
- Share count increases immediately
- Visual feedback with checkmark icon
- Green color indication
- Reverts on error

### 4. Error Handling
- Handles user cancellation gracefully
- Reverts optimistic updates on failure
- Shows appropriate error messages
- Logs errors for debugging

## Browser Support

### Web Share API Support:
✅ iOS Safari 12.2+
✅ Android Chrome 61+
✅ Samsung Internet 8.2+
✅ Edge 93+
❌ Desktop Chrome (requires HTTPS + user gesture)
❌ Firefox Desktop
❌ Safari Desktop

### Clipboard API Support:
✅ All modern browsers
✅ Chrome 63+
✅ Firefox 53+
✅ Safari 13.1+
✅ Edge 79+

## Share URL Format

```
https://yourdomain.com/community/post/{postId}
```

Example:
```
https://yourdomain.com/community/post/abc123xyz
```

## UI States

### Normal State:
- Icon: Share2 (arrow icon)
- Color: Gray
- Text: "Chia sẻ" or share count

### Shared State (2 seconds):
- Icon: Check (checkmark)
- Color: Green
- Text: Share count

### Hover State:
- Color: Green (lighter)

## Toast Notifications

1. **Web Share Success**: "Đã chia sẻ thành công!"
2. **Clipboard Success**: "Đã sao chép link vào clipboard!"
3. **Error**: "Không thể chia sẻ. Vui lòng thử lại."

## PostCard Integration

### Updated PostCard Component:
- Added `useState` for `showComments`
- Added `handleToggleComments` function
- Passes `showComments` and `onToggleComments` to PostActions
- Conditionally renders CommentsSection when toggled

```typescript
const [showComments, setShowComments] = useState(false);

const handleToggleComments = () => {
  setShowComments((prev) => !prev);
};

// In JSX:
<PostActions
  showComments={showComments}
  onToggleComments={handleToggleComments}
  // ... other props
/>

{showComments && <CommentsSection postId={post.postId || ''} />}
```

## PostActions Props

```typescript
interface PostActionsProps {
  postId: string;
  likesCount: number;
  commentsCount: number;
  sharesCount: number;
  isLiked: boolean;
  isBookmarked?: boolean;
  showComments: boolean;
  onToggleComments: () => void;
}
```

## Additional Features

### 1. Like Button
- Protected action (requires login)
- Optimistic updates via React Query
- Heart icon fills when liked
- Red color when liked

### 2. Comment Button
- Toggles comments section
- Shows comment count
- Amber color when active
- No login required to view

### 3. Bookmark Button
- Protected action (requires login)
- Saves post for later
- Bookmark icon fills when bookmarked
- Amber color when bookmarked

## Testing Checklist

- [x] Share on iOS Safari (Web Share API)
- [x] Share on Android Chrome (Web Share API)
- [x] Share on Desktop Chrome (Clipboard fallback)
- [x] Share on Desktop Firefox (Clipboard fallback)
- [x] User cancels share dialog
- [x] Clipboard permission denied
- [x] Optimistic UI update
- [x] Error handling and revert
- [x] Toast notifications
- [x] Visual feedback (checkmark, color)
- [x] Comments toggle functionality
- [x] Like button with authentication
- [x] Bookmark button with authentication

## Security Considerations

1. **HTTPS Required**: Web Share API requires secure context
2. **User Gesture**: Share must be triggered by user action
3. **Clipboard Permissions**: Handled automatically by browser
4. **URL Validation**: Share URL is constructed safely

## Performance

- **Optimistic Updates**: Instant UI feedback
- **No Backend Call**: Share count is client-side only
- **Minimal Re-renders**: State updates are localized
- **Async Operations**: Non-blocking share actions

## Future Enhancements

1. **Track Shares**: Send share event to backend analytics
2. **Share Targets**: Custom share targets for PWA
3. **Social Media Previews**: Open Graph meta tags
4. **Share Statistics**: Track which platforms users share to
5. **Deep Linking**: Handle shared links in app
6. **QR Code**: Generate QR code for sharing
7. **Email Share**: Add email option for desktop

## Dependencies

- `lucide-react`: Icons (Share2, Check, Heart, MessageCircle, Bookmark)
- `sonner`: Toast notifications
- `@tanstack/react-query`: State management for like/bookmark
- Web Share API: Native browser API
- Clipboard API: Native browser API

## Status

✅ **COMPLETE** - All features implemented and tested

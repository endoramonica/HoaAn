/**
 * PostActions Component
 */
import { useState } from 'react';
import { Heart, MessageCircle, Share2, Check, Bookmark } from 'lucide-react';
import { useLikePost, useBookmarkPost } from '../../hooks/usePosts';
import { ProtectedAction } from '../ProtectedAction';
import { toast } from 'sonner';

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

export const PostActions = ({
  postId,
  likesCount,
  commentsCount,
  sharesCount: initialShares,
  isLiked,
  isBookmarked = false,
  showComments,
  onToggleComments,
}: PostActionsProps) => {
  const likePostMutation = useLikePost();
  const bookmarkPostMutation = useBookmarkPost();
  const [sharesCount, setSharesCount] = useState(initialShares);
  const [isShared, setIsShared] = useState(false);

  const handleLike = async () => {
    await likePostMutation.mutateAsync(postId);
  };

  const handleBookmark = async () => {
    await bookmarkPostMutation.mutateAsync(postId);
  };

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

  return (
    <div className="flex items-center justify-between pt-3 border-t border-gray-100">
      <div className="flex gap-6">
        {/* Like Button */}
        <ProtectedAction onAction={handleLike}>
          {(onClick) => (
            <button
              onClick={onClick}
              disabled={likePostMutation.isPending}
              className={`flex items-center gap-2 text-sm font-medium transition-colors ${
                isLiked ? 'text-red-600' : 'text-gray-500 hover:text-red-600'
              }`}
            >
              <Heart className={`w-5 h-5 ${isLiked ? 'fill-current' : ''}`} />
              <span>{likesCount}</span>
            </button>
          )}
        </ProtectedAction>

        {/* Comment Button */}
        <button
          onClick={onToggleComments}
          className={`flex items-center gap-2 text-sm font-medium transition-colors ${
            showComments ? 'text-amber-600' : 'text-gray-500 hover:text-amber-600'
          }`}
        >
          <MessageCircle className="w-5 h-5" />
          <span>{commentsCount}</span>
        </button>

        {/* Share Button */}
        <button
          onClick={handleShare}
          className={`flex items-center gap-2 text-sm font-medium transition-colors ${
            isShared ? 'text-green-600' : 'text-gray-500 hover:text-green-600'
          }`}
        >
          {isShared ? <Check className="w-5 h-5" /> : <Share2 className="w-5 h-5" />}
          <span>{sharesCount > 0 ? sharesCount : 'Chia sẻ'}</span>
        </button>
      </div>

      {/* Bookmark Button */}
      <ProtectedAction onAction={handleBookmark}>
        {(onClick) => (
          <button
            onClick={onClick}
            disabled={bookmarkPostMutation.isPending}
            className={`transition-colors ${
              isBookmarked ? 'text-amber-600' : 'text-gray-400 hover:text-gray-700'
            }`}
          >
            <Bookmark className={`w-5 h-5 ${isBookmarked ? 'fill-current' : ''}`} />
          </button>
        )}
      </ProtectedAction>
    </div>
  );
};

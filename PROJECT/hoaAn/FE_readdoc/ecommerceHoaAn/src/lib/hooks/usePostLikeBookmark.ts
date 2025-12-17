/**
 * usePostLikeBookmark Hook
 * Manages like and bookmark state and API calls for a single post
 */

import { useState, useCallback } from 'react';
import { likePost, bookmarkPost } from '../services/postActionsService';
import { toast } from 'sonner';

interface UsePostLikeBookmarkProps {
    postId: string;
    initialLiked?: boolean;
    initialBookmarked?: boolean;
    initialLikesCount?: number;
    onLikeChange?: (liked: boolean, likesCount: number) => void;
    onBookmarkChange?: (bookmarked: boolean) => void;
}

export const usePostLikeBookmark = ({
    postId,
    initialLiked = false,
    initialBookmarked = false,
    initialLikesCount = 0,
    onLikeChange,
    onBookmarkChange,
}: UsePostLikeBookmarkProps) => {
    const [isLiked, setIsLiked] = useState(initialLiked);
    const [isBookmarked, setIsBookmarked] = useState(initialBookmarked);
    const [likesCount, setLikesCount] = useState(initialLikesCount);
    const [isLikeLoading, setIsLikeLoading] = useState(false);
    const [isBookmarkLoading, setIsBookmarkLoading] = useState(false);

    const handleLike = useCallback(async () => {
        setIsLikeLoading(true);
        try {
            const result = await likePost(postId);

            if (result.success && result.data) {
                const newLiked = result.data.liked ?? false;
                const newLikesCount = result.data.totalLikes ?? 0;

                setIsLiked(newLiked);
                setLikesCount(newLikesCount);

                onLikeChange?.(newLiked, newLikesCount);
                toast.success(newLiked ? 'Đã like bài viết' : 'Đã bỏ like bài viết');
            } else {
                toast.error(result.message || 'Không thể like bài viết');
            }
        } catch (error) {
            console.error('Error in handleLike:', error);
            toast.error('Lỗi khi like bài viết');
        } finally {
            setIsLikeLoading(false);
        }
    }, [postId, onLikeChange]);

    const handleBookmark = useCallback(async () => {
        setIsBookmarkLoading(true);
        try {
            const result = await bookmarkPost(postId);

            if (result.success && result.data) {
                const newBookmarked = result.data.bookmarked ?? false;

                setIsBookmarked(newBookmarked);
                onBookmarkChange?.(newBookmarked);
                toast.success(newBookmarked ? 'Đã bookmark bài viết' : 'Đã bỏ bookmark bài viết');
            } else {
                toast.error(result.message || 'Không thể bookmark bài viết');
            }
        } catch (error) {
            console.error('Error in handleBookmark:', error);
            toast.error('Lỗi khi bookmark bài viết');
        } finally {
            setIsBookmarkLoading(false);
        }
    }, [postId, onBookmarkChange]);

    return {
        isLiked,
        isBookmarked,
        likesCount,
        isLikeLoading,
        isBookmarkLoading,
        handleLike,
        handleBookmark,
    };
};

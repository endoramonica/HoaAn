/**
 * usePostActions Hook
 * Manages like and bookmark API calls for posts
 * Integrates with generated Orval API client
 */

import { useState, useCallback } from 'react';
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import { toast } from 'sonner';

interface UsePostActionsProps {
    postId: string;
    onLikeSuccess?: (liked: boolean, totalLikes: number) => void;
    onBookmarkSuccess?: (bookmarked: boolean) => void;
}

export const usePostActions = ({
    postId,
    onLikeSuccess,
    onBookmarkSuccess,
}: UsePostActionsProps) => {
    const [isLikeLoading, setIsLikeLoading] = useState(false);
    const [isBookmarkLoading, setIsBookmarkLoading] = useState(false);

    const api = getVietCommerceAPI();

    const handleLike = useCallback(async () => {
        if (!postId) {
            toast.error('Post ID không hợp lệ');
            return;
        }

        setIsLikeLoading(true);
        try {
            const response = await api.postApiV1PostsPostIdLike(postId);

            if (response.data) {
                const liked = response.data.liked ?? false;
                const totalLikes = response.data.totalLikes ?? 0;
                onLikeSuccess?.(liked, totalLikes);
                toast.success(liked ? 'Đã like bài viết' : 'Đã bỏ like bài viết');
            }
        } catch (error: any) {
            console.error('Error liking post:', error);
            const errorMessage = error?.response?.data?.message || 'Không thể like bài viết';
            toast.error(errorMessage);
            throw error;
        } finally {
            setIsLikeLoading(false);
        }
    }, [postId, onLikeSuccess]);

    const handleBookmark = useCallback(async () => {
        if (!postId) {
            toast.error('Post ID không hợp lệ');
            return;
        }

        setIsBookmarkLoading(true);
        try {
            const response = await api.postApiV1PostsPostIdBookmark(postId);

            if (response.data) {
                const bookmarked = response.data.bookmarked ?? false;
                onBookmarkSuccess?.(bookmarked);
                toast.success(bookmarked ? 'Đã bookmark bài viết' : 'Đã bỏ bookmark bài viết');
            }
        } catch (error: any) {
            console.error('Error bookmarking post:', error);
            const errorMessage = error?.response?.data?.message || 'Không thể bookmark bài viết';
            toast.error(errorMessage);
            throw error;
        } finally {
            setIsBookmarkLoading(false);
        }
    }, [postId, onBookmarkSuccess]);

    return {
        handleLike,
        handleBookmark,
        isLikeLoading,
        isBookmarkLoading,
    };
};

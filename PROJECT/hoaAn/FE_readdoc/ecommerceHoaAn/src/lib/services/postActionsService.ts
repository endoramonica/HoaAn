/**
 * Post Actions Service
 * Handles like and bookmark API calls for posts
 */

import { getVietCommerceAPI } from '../../../Api/generated-orval';

const api = getVietCommerceAPI();

export interface PostActionResult {
    success: boolean;
    message?: string;
    data?: {
        liked?: boolean;
        totalLikes?: number;
        bookmarked?: boolean;
        totalBookmarks?: number;
    };
}

/**
 * Like a post
 * @param postId - The ID of the post to like
 * @returns Promise with like result
 */
export const likePost = async (postId: string): Promise<PostActionResult> => {
    try {
        if (!postId) {
            throw new Error('Post ID is required');
        }

        const response = await api.postApiV1PostsPostIdLike(postId);

        if (response.data) {
            return {
                success: true,
                data: {
                    liked: response.data.liked ?? false,
                    totalLikes: response.data.totalLikes ?? 0,
                },
            };
        }

        return {
            success: false,
            message: 'No data returned from server',
        };
    } catch (error: any) {
        console.error('Error liking post:', error);
        const errorMessage = error?.response?.data?.message || error?.message || 'Failed to like post';
        return {
            success: false,
            message: errorMessage,
        };
    }
};

/**
 * Bookmark a post
 * @param postId - The ID of the post to bookmark
 * @returns Promise with bookmark result
 */
export const bookmarkPost = async (postId: string): Promise<PostActionResult> => {
    try {
        if (!postId) {
            throw new Error('Post ID is required');
        }

        const response = await api.postApiV1PostsPostIdBookmark(postId);

        if (response.data) {
            return {
                success: true,
                data: {
                    bookmarked: response.data.bookmarked ?? false,
                    totalBookmarks: response.data.totalBookmarks ?? 0,
                },
            };
        }

        return {
            success: false,
            message: 'No data returned from server',
        };
    } catch (error: any) {
        console.error('Error bookmarking post:', error);
        const errorMessage = error?.response?.data?.message || error?.message || 'Failed to bookmark post';
        return {
            success: false,
            message: errorMessage,
        };
    }
};

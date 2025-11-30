
import { useState } from 'react';
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type { CommentDto } from '../../../Api/generated-orval/schemas';

const api = getVietCommerceAPI();

interface UseCommentRepliesProps {
  postId: string;
  commentId: string;
  initialRepliesCount?: number;
}

export const useCommentReplies = ({ postId, commentId, initialRepliesCount = 0 }: UseCommentRepliesProps) => {
  const [replies, setReplies] = useState<CommentDto[]>([]);
  const [repliesCount, setRepliesCount] = useState(initialRepliesCount);
  const [isLoading, setIsLoading] = useState(false);
  const [isPosting, setIsPosting] = useState(false);
  const [showReplies, setShowReplies] = useState(false);
  const [areRepliesLoaded, setAreRepliesLoaded] = useState(false);

  const toggleReplies = async () => {
    const nextState = !showReplies;
    setShowReplies(nextState);

    if (nextState && !areRepliesLoaded && repliesCount > 0) {
      await loadReplies();
    }
  };

  const loadReplies = async () => {
    setIsLoading(true);
    try {
      const response = await api.getApiV1CommentsCommentIdReplies(commentId);
      if (response.success && response.data) {
        setReplies(response.data);
        setAreRepliesLoaded(true);
      } else {
        throw new Error("API returned no data");
      }
    } catch (error) {
      console.error('Error loading replies:', error);
      setReplies([]);
      setAreRepliesLoaded(true);
    } finally {
      setIsLoading(false);
    }
  };

  const replyToComment = async (content: string): Promise<boolean> => {
  if (!content.trim()) return false;
  
  setIsPosting(true);
  try {
    const payload = {
      postId,
      content: content.trim(),
      parentCommentId: commentId
    };

    // 🔍 Debug logs
    console.log('🔍 Reply payload:', {
      postId,
      content: content.trim(),
      parentCommentId: commentId,
      types: {
        postId: typeof postId,
        content: typeof content,
        parentCommentId: typeof commentId
      }
    });

    const response = await api.postApiV1Comments(payload);

    console.log('✅ Response:', response);

    if (response.success && response.data) {
      setReplies(prev => [...prev, response.data!]);
      setRepliesCount(prev => prev + 1);
      setShowReplies(true);
      return true;
    }
    throw new Error("API Failure");
  } catch (error: any) {
    console.error('❌ Error posting reply:', error);
    console.error('🔍 Error details:', {
      status: error.response?.status,
      data: error.response?.data,
      message: error.message
    });
    return false;
  } finally {
    setIsPosting(false);
  }
};

  return {
    replies,
    repliesCount,
    isLoading,
    isPosting,
    showReplies,
    toggleReplies,
    replyToComment
  };
};

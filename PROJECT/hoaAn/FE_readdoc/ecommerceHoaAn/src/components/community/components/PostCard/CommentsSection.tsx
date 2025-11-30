
import React, { useState } from 'react';
import { Send, Loader2, MessageSquare } from 'lucide-react';
import { CommentItem } from './CommentItem';
import { usePostComments, useCreateComment } from '../../hooks/useComments';
import type { CommentDto } from '../../../../../Api/generated-orval/schemas';
import { toast } from 'sonner';

interface CommentsSectionProps {
  postId: string;
}

export const CommentsSection = ({ postId }: CommentsSectionProps) => {
  const [newComment, setNewComment] = useState('');
  const [page, setPage] = useState(1);

  const { data, isLoading, error } = usePostComments(postId, page, 20);
  const createCommentMutation = useCreateComment();

  const comments = data?.data?.items || [];
  const totalCount = data?.data?.totalItems || 0;
  const hasMore = (data?.data?.pageNumber || 0) < (data?.data?.totalPages || 0);
  const isPosting = createCommentMutation.isPending;
  const [parentId, setParentId] = useState<string | null>(null);


  const handleLoadMore = () => {
    setPage((prev) => prev + 1);
  };

  const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();
  if (!newComment.trim()) return;
  
  // Build payload conditionally
  const payload: any = {
    postId,
    content: newComment,
  };
  
  // Only add parentCommentId if it actually exists
  if (parentId) {
    payload.parentCommentId = parentId;
  }
  // If no parentId, the field is simply not included in payload

  try {
    await createCommentMutation.mutateAsync(payload);
    setNewComment('');
    toast.success('Comment posted successfully!');
  } catch (error: any) {
    console.error('Error posting comment:', error);
    console.error('Server response:', error.response?.data);
    toast.error(error?.message || 'Failed to post comment');
  }
};

  return (
    <div className="pt-4 border-t border-gray-100 bg-gray-50/50 -mx-6 px-6 pb-2">
      <div className="flex items-center gap-2 mb-4 text-sm text-gray-500">
        <MessageSquare className="w-4 h-4" />
        <span>{totalCount} Comments</span>
      </div>

      <form onSubmit={handleSubmit} className="flex items-start gap-3 mb-6">
        <img 
          src="https://api.dicebear.com/7.x/avataaars/svg?seed=CurrentUser" 
          alt="Current user" 
          className="w-8 h-8 rounded-full border border-gray-200"
        />
        <div className="flex-1 relative">
          <input
            type="text"
            value={newComment}
            onChange={(e) => setNewComment(e.target.value)}
            placeholder="Write a comment..."
            className="w-full bg-white border border-gray-200 rounded-xl px-4 py-2.5 text-sm focus:ring-2 focus:ring-blue-100 focus:border-blue-400 outline-none transition-all pr-10"
            disabled={isPosting}
          />
          <button 
            type="submit"
            disabled={!newComment.trim() || isPosting}
            className="absolute right-2 top-1.5 p-1 text-blue-600 hover:bg-blue-50 rounded-lg disabled:opacity-30 disabled:hover:bg-transparent transition-colors"
          >
            {isPosting ? <Loader2 className="w-4 h-4 animate-spin" /> : <Send className="w-4 h-4" />}
          </button>
        </div>
      </form>

      {error && (
        <div className="text-sm text-red-600 mb-4">
          Failed to load comments. Please try again.
        </div>
      )}

      <div className="space-y-6">
        {comments.map((comment: CommentDto, idx: number) => (
          <CommentItem 
            key={comment.commentId || idx} 
            comment={comment} 
            postId={postId} 
          />
        ))}
        
        {isLoading && (
          <div className="flex justify-center py-4">
            <Loader2 className="w-6 h-6 animate-spin text-blue-500" />
          </div>
        )}

        {!isLoading && comments.length === 0 && (
          <div className="text-center py-8 text-gray-500 text-sm">
            No comments yet. Be the first to comment!
          </div>
        )}

        {hasMore && !isLoading && (
          <button 
            onClick={handleLoadMore}
            className="w-full py-2 text-sm text-blue-600 font-medium hover:bg-blue-50 rounded-lg transition-colors"
          >
            Load more comments
          </button>
        )}
      </div>
    </div>
  );
};


import React, { useState } from 'react';
import { Loader2, Send, ChevronDown, ChevronUp } from 'lucide-react';
import type { CommentDto } from '../../../../../Api/generated-orval/schemas';
import { useCommentReplies } from '../../../../lib/hooks/useCommentReplies';

interface CommentItemProps {
  comment: CommentDto;
  postId: string;
}

export const CommentItem = ({ comment, postId }: CommentItemProps) => {
  // 🔍 Debug the actual comment object
  console.log('Full comment object:', comment);
  console.log('Comment keys:', Object.keys(comment));
  const [isReplying, setIsReplying] = useState(false);
  const [replyContent, setReplyContent] = useState('');

  const {
    replies,
    repliesCount,
    isLoading,
    isPosting,
    showReplies,
    toggleReplies,
    replyToComment
  } = useCommentReplies({
    postId,
    commentId: comment.commentId!,
    initialRepliesCount: comment.repliesCount || 0
  });

  const handleReplySubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    console.log('CommentItem props:', { postId, commentId: comment.commentId });
    const success = await replyToComment(replyContent);
    if (success) {
      setReplyContent('');
      setIsReplying(false);
    }
  };

  return (
    <div className="flex gap-3 animate-in fade-in slide-in-from-top-1 duration-300">
      {/* Avatar */}
      <img 
        src={comment.customerAvatar || `https://ui-avatars.com/api/?name=${comment.customerName}`} 
        alt={comment.customerName || 'User'} 
        className="w-8 h-8 rounded-full border border-gray-200 mt-1 flex-shrink-0"
      />
      
      <div className="flex-1 min-w-0">
        {/* Comment Bubble */}
        <div className="bg-white p-3 rounded-2xl border border-gray-100 shadow-sm inline-block min-w-[200px]">
          <div className="flex items-center justify-between mb-1 gap-4">
            <span className="font-semibold text-sm text-gray-900">{comment.customerName}</span>
            <span className="text-xs text-gray-400">
              {comment.addedOn ? new Date(comment.addedOn).toLocaleDateString() : 'Just now'}
            </span>
          </div>
          <p className="text-gray-700 text-sm whitespace-pre-wrap break-words">{comment.content}</p>
        </div>

        {/* Action Buttons */}
        <div className="flex items-center gap-4 mt-1 ml-2">
          <button className="text-xs font-medium text-gray-500 hover:text-blue-600 transition-colors">
            Like
          </button>
          <button 
            onClick={() => setIsReplying(!isReplying)}
            className="text-xs font-medium text-gray-500 hover:text-blue-600 transition-colors"
          >
            Reply
          </button>
          {repliesCount > 0 && (
             <button 
                onClick={toggleReplies}
                className="flex items-center gap-1 text-xs font-medium text-gray-500 hover:text-blue-600 transition-colors ml-2"
             >
                {isLoading ? (
                    <Loader2 className="w-3 h-3 animate-spin" />
                ) : (
                    <>
                        {showReplies ? <ChevronUp className="w-3 h-3" /> : <ChevronDown className="w-3 h-3" />}
                        {repliesCount} {repliesCount === 1 ? 'reply' : 'replies'}
                    </>
                )}
             </button>
          )}
        </div>

        {/* Reply Input Form */}
        {isReplying && (
          <form onSubmit={handleReplySubmit} className="mt-3 flex gap-2 items-start">
            <div className="relative flex-1">
              <input
                type="text"
                value={replyContent}
                onChange={(e) => setReplyContent(e.target.value)}
                placeholder={`Reply to ${comment.customerName}...`}
                autoFocus
                className="w-full bg-gray-50 border border-gray-200 rounded-xl px-3 py-2 text-sm focus:ring-2 focus:ring-blue-100 focus:border-blue-400 outline-none transition-all pr-9"
                disabled={isPosting}
              />
              <button 
                type="submit"
                disabled={!replyContent.trim() || isPosting}
                className="absolute right-1.5 top-1.5 p-1 text-blue-600 hover:bg-blue-100 rounded-lg disabled:opacity-30 disabled:hover:bg-transparent transition-colors"
              >
                {isPosting ? <Loader2 className="w-3 h-3 animate-spin" /> : <Send className="w-3 h-3" />}
              </button>
            </div>
          </form>
        )}

        {/* Nested Replies List */}
        {showReplies && (
            <div className="mt-3 space-y-3">
                {replies.map((reply, index) => (
                    <div key={reply.commentId || index} className="relative pl-2">
                        {/* Connecting line visually */}
                        <div className="absolute left-[-10px] top-[-10px] bottom-4 w-4 border-l-2 border-b-2 border-gray-200 rounded-bl-xl opacity-50 pointer-events-none"></div>
                        
                        <CommentItem 
                            comment={reply} 
                            postId={postId} 
                        />
                    </div>
                ))}
            </div>
        )}
      </div>
    </div>
  );
};

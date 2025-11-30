/**
 * PostCard Component
 * // dùng như một thẻ cha : gọi đến các thẻ con
 */

import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { PostHeader } from './PostHeader';
import { PostContent } from './PostContent';
import { PostImages } from './PostImages';
import { PostActions } from './PostActions';
import { CommentsSection } from './CommentsSection';

import type { PostResponseDto } from '../../../../../Api/generated-orval/schemas';

type Post = PostResponseDto;

interface PostCardProps {
  post: Post;
  isNewest?: boolean;
}

export const PostCard = ({ post, isNewest }: PostCardProps) => {
  const [showComments, setShowComments] = useState(false);

  const author = {
    id: post.customerId,
    name: post.customerName || undefined,
    avatar: post.customerAvatar || undefined,
  };

  const photoUrls = post.photoUrl ? [post.photoUrl] : undefined;

  const handleToggleComments = () => {
    setShowComments((prev) => !prev);
  };

  return (
    <Card className="p-6 bg-white shadow-md hover:shadow-lg transition-shadow">
      <PostHeader
        author={author}
        createdAt={post.createdAt || ''}
        isNewest={isNewest}
      />

      <PostContent content={post.content || ''} />

      <PostImages photoUrls={photoUrls} />

      <PostActions
        postId={post.postId || ''}
        likesCount={post.likesCount || 0}
        commentsCount={post.commentsCount || 0}
        sharesCount={0}
        isLiked={post.isLikedByCurrentUser || false}
        isBookmarked={post.isBookmarkedByCurrentUser || false}
        showComments={showComments}
        onToggleComments={handleToggleComments}
      />

      {showComments && <CommentsSection postId={post.postId || ''} />}
    </Card>
  );
};

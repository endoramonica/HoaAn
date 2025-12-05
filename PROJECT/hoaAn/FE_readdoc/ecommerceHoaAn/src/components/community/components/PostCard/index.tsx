/**
 * PostCard Component
 * // dùng như một thẻ cha : gọi đến các thẻ con
 * Supports both PostResponseDto and MixedFeedDto
 */

import { useState } from 'react';
import { Card } from '@/components/ui/card';
import { PostHeader } from './PostHeader';
import { PostContent } from './PostContent';
import { PostImages } from './PostImages';
import { PostActions } from './PostActions';
import { CommentsSection } from './CommentsSection';

import type { PostResponseDto, MixedFeedDto } from '../../../../../Api/generated-orval/schemas';

// Union type to support both PostResponseDto and MixedFeedDto
type Post = PostResponseDto | MixedFeedDto;

interface PostCardProps {
  post: Post;
  isNewest?: boolean;
}

// Helper to get postId from either DTO type
const getPostId = (post: Post): string => {
  return (post as PostResponseDto).postId || (post as MixedFeedDto).id || '';
};

// Helper to get photo URLs from either DTO type
const getPhotoUrls = (post: Post): string[] | undefined => {
  const mixedFeed = post as MixedFeedDto;
  const postResponse = post as PostResponseDto;
  
  // MixedFeedDto uses imageUrls or imageUrl
  if (mixedFeed.imageUrls && mixedFeed.imageUrls.length > 0) {
    return mixedFeed.imageUrls;
  }
  if (mixedFeed.imageUrl) {
    return [mixedFeed.imageUrl];
  }
  // PostResponseDto uses photoUrl
  if (postResponse.photoUrl) {
    return [postResponse.photoUrl];
  }
  return undefined;
};

export const PostCard = ({ post, isNewest }: PostCardProps) => {
  const [showComments, setShowComments] = useState(false);

  const author = {
    id: post.customerId || undefined,
    name: post.customerName || undefined,
    avatar: post.customerAvatar || undefined,
  };

  const postId = getPostId(post);
  const photoUrls = getPhotoUrls(post);

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
        postId={postId}
        likesCount={post.likesCount || 0}
        commentsCount={post.commentsCount || 0}
        sharesCount={(post as MixedFeedDto).sharesCount || 0}
        isLiked={post.isLikedByCurrentUser || false}
        isBookmarked={post.isBookmarkedByCurrentUser || false}
        showComments={showComments}
        onToggleComments={handleToggleComments}
      />

      {showComments && <CommentsSection postId={postId} />}
    </Card>
  );
};

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
import { ProductShowcase } from '@/components/marketing';
import { usePostLikeBookmark } from '@/lib/hooks/usePostLikeBookmark';

import type { PostResponseDto, MixedFeedDto } from '../../../../../Api/generated-orval/schemas';

// Union type to support both PostResponseDto and MixedFeedDto
type Post = PostResponseDto | MixedFeedDto;

interface PostCardProps {
  post: Post;
  isNewest?: boolean;
  onProductClick?: (productId: string) => void;
  onPostUpdate?: (postId: string, updates: any) => void;
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

export const PostCard = ({ 
  post, 
  isNewest, 
  onProductClick,
  onPostUpdate,
}: PostCardProps) => {
  const [showComments, setShowComments] = useState(false);

  const postId = getPostId(post);
  const photoUrls = getPhotoUrls(post);

  // Use hook for like and bookmark
  const {
    isLiked,
    isBookmarked,
    likesCount,
    isLikeLoading,
    isBookmarkLoading,
    handleLike,
    handleBookmark,
  } = usePostLikeBookmark({
    postId,
    initialLiked: post.isLikedByCurrentUser || false,
    initialBookmarked: post.isBookmarkedByCurrentUser || false,
    initialLikesCount: post.likesCount || 0,
    onLikeChange: (liked, likesCount) => {
      onPostUpdate?.(postId, { isLikedByCurrentUser: liked, likesCount });
    },
    onBookmarkChange: (bookmarked) => {
      onPostUpdate?.(postId, { isBookmarkedByCurrentUser: bookmarked });
    },
  });

  const author = {
    id: post.customerId || undefined,
    name: post.customerName || undefined,
    avatar: post.customerAvatar || undefined,
  };

  const handleToggleComments = () => {
    setShowComments((prev) => !prev);
  };

  const handleProductClick = () => {
    const product = (post as MixedFeedDto).taggedProduct;
    if (product && product.id && onProductClick) {
      onProductClick(product.id);
    }
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

      {/* Tagged Product */}
      {(() => {
        const product = (post as MixedFeedDto).taggedProduct;
        if (
          product &&
          product.id &&
          product.name &&
          product.price !== undefined &&
          product.currency &&
          product.thumbnailUrl &&
          product.hasDiscount !== undefined &&
          product.discountPercentage !== undefined
        ) {
          return (
            <div className="mb-4">
              <ProductShowcase
                product={{
                  id: product.id,
                  name: product.name,
                  price: product.price,
                  currency: product.currency,
                  formattedPrice: product.formattedPrice,
                  thumbnailUrl: product.thumbnailUrl,
                  hasDiscount: product.hasDiscount,
                  discountPercentage: product.discountPercentage,
                }}
                onViewDetails={handleProductClick}
              />
            </div>
          );
        }
        return null;
      })()}

      <PostActions
        postId={postId}
        likesCount={likesCount}
        commentsCount={post.commentsCount || 0}
        sharesCount={(post as MixedFeedDto).sharesCount || 0}
        isLiked={isLiked}
        isBookmarked={isBookmarked}
        showComments={showComments}
        onToggleComments={handleToggleComments}
        onLike={handleLike}
        onBookmark={handleBookmark}
        isLikeLoading={isLikeLoading}
        isBookmarkLoading={isBookmarkLoading}
      />

      {showComments && <CommentsSection postId={postId} />}
    </Card>
  );
};

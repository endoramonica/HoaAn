/**
 * Marketing Post Card Component
 * Displays marketing post with product information
 */

import { useEffect, useRef } from 'react';
import { Heart, Share2, Eye, MessageCircle } from 'lucide-react';
import { ProductShowcase } from './ProductShowcase';
import type { MarketingPost } from '@/lib/api/types';
import {
  hasValidProduct,
  isFeaturedPost,
  setupPostViewTracking,
  trackPostClick,
  trackPostShare,
} from '@/lib/services/marketingPostService';

interface PostCardProps {
  post: MarketingPost;
  onProductClick?: (productId: string) => void;
}

export function PostCard({ post, onProductClick }: PostCardProps) {
  const cardRef = useRef<HTMLDivElement>(null);
  const observerRef = useRef<IntersectionObserver | null>(null);

  useEffect(() => {
    if (cardRef.current) {
      observerRef.current = setupPostViewTracking(cardRef.current, post.id);
    }

    return () => {
      if (observerRef.current) {
        observerRef.current.disconnect();
      }
    };
  }, [post.id]);

  const handleProductClick = () => {
    trackPostClick(post.id);
    const productId = post.taggedProduct?.id;
    if (productId) {
      onProductClick?.(productId);
    }
  };

  const handleShare = async () => {
    await trackPostShare(post.id);

    if (navigator.share) {
      try {
        await navigator.share({
          title: post.title,
          text: post.shortDescription,
          url: window.location.href,
        });
      } catch (err) {
        console.log('Share cancelled');
      }
    }
  };



  const isFeatured = isFeaturedPost(post);
  const hasProduct = hasValidProduct(post);

  return (
    <div
      ref={cardRef}
      className={`bg-white rounded-lg shadow-md hover:shadow-lg transition overflow-hidden ${
        isFeatured ? 'ring-2 ring-yellow-400' : ''
      }`}
    >
      {/* Featured Badge */}
      {isFeatured && (
        <div className="absolute top-2 right-2 bg-yellow-400 text-yellow-900 px-3 py-1 rounded-full text-xs font-bold z-10">
          ⭐ Featured
        </div>
      )}

      {/* Image */}
      <div className="relative h-48 bg-gray-200 overflow-hidden">
        <img
          src={post.image}
          alt={post.title}
          className="w-full h-full object-cover hover:scale-105 transition"
        />
      </div>

      {/* Content */}
      <div className="p-4 space-y-3">
        {/* Title */}
        <h3 className="font-bold text-lg text-gray-800 line-clamp-2">
          {post.title}
        </h3>

        {/* Description */}
        <p className="text-sm text-gray-600 line-clamp-2">
          {post.shortDescription}
        </p>

        {/* Product Section */}
        {hasProduct && post.taggedProduct && (
          <div className="mt-4">
            <ProductShowcase
              product={post.taggedProduct}
              onViewDetails={handleProductClick}
            />
          </div>
        )}


        


        {/* No Product Message */}
        {!hasProduct && post.productId && (
          <div className="bg-gray-100 rounded-lg p-3 text-sm text-gray-600">
            Product no longer available
          </div>
        )}

        {/* Hashtags */}
        {post.hashtags && post.hashtags.length > 0 && (
          <div className="flex flex-wrap gap-2">
            {post.hashtags.slice(0, 3).map((tag: string) => (
              <span
                key={tag}
                className="text-xs bg-gray-100 text-gray-700 px-2 py-1 rounded"
              >
                #{tag}
              </span>
            ))}
          </div>
        )}

        {/* Analytics */}
        <div className="flex justify-between items-center pt-3 border-t border-gray-200 text-xs text-gray-600">
          <div className="flex items-center gap-1">
            <Eye size={16} />
            <span>{post.views}</span>
          </div>
          <div className="flex items-center gap-1">
            <MessageCircle size={16} />
            <span>{post.clicks}</span>
          </div>
          <div className="flex items-center gap-1">
            <Share2 size={16} />
            <span>{post.shares}</span>
          </div>
        </div>

        {/* Actions */}
        <div className="flex gap-2 pt-3 border-t border-gray-200">
          <button className="flex-1 flex items-center justify-center gap-2 py-2 text-gray-600 hover:bg-gray-100 rounded transition text-sm font-medium">
            <Heart size={16} />
            Thích
          </button>
          <button
            onClick={handleShare}
            className="flex-1 flex items-center justify-center gap-2 py-2 text-gray-600 hover:bg-gray-100 rounded transition text-sm font-medium"
          >
            <Share2 size={16} />
            Chia sẻ
          </button>
        </div>
      </div>
    </div>
  );
}

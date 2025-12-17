/**
 * Marketing Posts Feed Component
 * Displays a feed of marketing posts with pagination
 */

import { useState, useEffect } from 'react';
import { Loader2, AlertCircle } from 'lucide-react';
import { PostCard } from './PostCard';
import { getMarketingPosts } from '@/lib/services/marketingPostService';
import type { MarketingPost, GetMarketingPostsQuery } from '@/lib/api/types';

interface PostsFeedProps {
  query?: GetMarketingPostsQuery;
  onProductClick?: (productId: string) => void;
}

export function PostsFeed({ query = {}, onProductClick }: PostsFeedProps) {
  const [posts, setPosts] = useState<MarketingPost[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  const pageSize = query.pageSize || 12;

  useEffect(() => {
    loadPosts();
  }, [pageNumber, query]);

  const loadPosts = async () => {
    setIsLoading(true);
    setError(null);

    try {
      const result = await getMarketingPosts({
        ...query,
        pageNumber,
        pageSize,
      });

      setPosts(result.items);
      setTotalPages(result.totalPages);
    } catch (err: any) {
      setError(err.message || 'Failed to load posts');
      console.error('Error loading posts:', err);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading && posts.length === 0) {
    return (
      <div className="flex justify-center items-center py-12">
        <Loader2 className="animate-spin text-blue-600" size={32} />
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-4 flex items-center gap-3">
        <AlertCircle className="text-red-600" size={24} />
        <div>
          <p className="font-semibold text-red-800">Error loading posts</p>
          <p className="text-red-700 text-sm">{error}</p>
        </div>
      </div>
    );
  }

  if (posts.length === 0) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500 text-lg">No posts found</p>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      {/* Posts Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {posts.map((post) => (
          <PostCard
            key={post.id}
            post={post}
            onProductClick={onProductClick}
          />
        ))}
      </div>

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex justify-center items-center gap-2 py-8">
          <button
            onClick={() => setPageNumber(Math.max(1, pageNumber - 1))}
            disabled={pageNumber === 1}
            className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed transition"
          >
            Previous
          </button>

          <div className="flex items-center gap-2">
            {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
              const page = i + 1;
              return (
                <button
                  key={page}
                  onClick={() => setPageNumber(page)}
                  className={`px-3 py-2 rounded-lg transition ${
                    pageNumber === page
                      ? 'bg-blue-600 text-white'
                      : 'border border-gray-300 hover:bg-gray-100'
                  }`}
                >
                  {page}
                </button>
              );
            })}
          </div>

          <button
            onClick={() => setPageNumber(Math.min(totalPages, pageNumber + 1))}
            disabled={pageNumber === totalPages}
            className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed transition"
          >
            Next
          </button>
        </div>
      )}

      {/* Loading indicator for pagination */}
      {isLoading && (
        <div className="flex justify-center py-4">
          <Loader2 className="animate-spin text-blue-600" size={24} />
        </div>
      )}
    </div>
  );
}

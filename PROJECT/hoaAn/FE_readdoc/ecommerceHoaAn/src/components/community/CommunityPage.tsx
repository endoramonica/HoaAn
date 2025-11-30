/**
 * CommunityPage Component - Refactored with React Query
 */

import { useState, useEffect } from 'react';
import { Card } from '@/components/ui/card';
import { MessageCircle, Users, Loader2, AlertCircle } from 'lucide-react';
import { SearchBar } from './components/SearchBar';
import { CreatePostDialog } from './components/CreatePostDialog';
import { PostList } from './components/PostList';
import { LoadMoreButton } from './components/LoadMoreButton';
import { usePostsFeed, useSearchPosts } from './hooks/usePosts';

interface CommunityPageProps {
  onBack: () => void;
}

export default function CommunityPage({ onBack }: CommunityPageProps) {
   const [searchQuery, setSearchQuery] = useState('');
  const [debouncedSearch, setDebouncedSearch] = useState('');
  const [currentPage, setCurrentPage] = useState(1);

  // Debounce search query
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(searchQuery);
      setCurrentPage(1); // Reset to page 1 on new search
    }, 500);

    return () => clearTimeout(timer);
  }, [searchQuery]);

  // Fetch posts based on search or feed
  const isSearching = !!debouncedSearch.trim();
  const feedQuery = usePostsFeed(currentPage, 20);
  const searchQueryResult = useSearchPosts(debouncedSearch, currentPage, 20);

  const activeQuery = isSearching ? searchQueryResult : feedQuery;
  const posts = activeQuery.data?.data?.items || [];
  const totalPosts = activeQuery.data?.data?.totalItems || 0;
  const hasMore = posts.length < totalPosts;

  const handleLoadMore = () => {
    if (!activeQuery.isLoading && hasMore) {
      setCurrentPage((prev) => prev + 1);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-orange-50 to-amber-50">
      {/* Header */}
      <div className="bg-gradient-to-r from-amber-800 to-orange-800 text-white sticky top-0 z-40">
        <div className="max-w-2xl mx-auto px-4 py-4">
          <div className="flex items-center justify-between mb-4">
            <button onClick={onBack} className="text-amber-100 hover:text-white">
              ← Quay lại
            </button>
            <h1 className="text-xl font-semibold">Cộng đồng Tâm Linh</h1>
            <div className="w-20"></div>
          </div>

          <div className="text-center">
            <p className="text-amber-100 text-sm mb-2">Chia sẻ - Kết nối - Học hỏi</p>
            <div className="flex items-center justify-center gap-6 text-sm">
              <span className="flex items-center gap-1">
                <Users className="w-4 h-4" />
                {totalPosts * 142} thành viên
              </span>
              <span className="flex items-center gap-1">
                <MessageCircle className="w-4 h-4" />
                {totalPosts} bài viết
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="max-w-2xl mx-auto px-4 py-6">
        {/* Search & Create Post */}
        <div className="mb-6 space-y-4">
          <SearchBar value={searchQuery} onChange={setSearchQuery} />
          <CreatePostDialog />
        </div>

        {/* Error State */}
        {activeQuery.isError && (
          <Card className="p-4 mb-6 bg-red-50 border-red-200">
            <div className="flex items-center gap-2 text-red-600">
              <AlertCircle className="w-5 h-5" />
              <p>
                {activeQuery.error?.message ||
                  'Không thể tải bài viết. Vui lòng thử lại.'}
              </p>
            </div>
          </Card>
        )}

        {/* Loading State */}
        {activeQuery.isLoading && posts.length === 0 && (
          <div className="flex justify-center items-center py-12">
            <Loader2 className="w-8 h-8 animate-spin text-amber-600" />
          </div>
        )}

        {/* Empty State */}
        {!activeQuery.isLoading && posts.length === 0 && (
          <Card className="p-12 text-center">
            <MessageCircle className="w-16 h-16 mx-auto mb-4 text-gray-300" />
            <h3 className="text-lg font-semibold text-gray-700 mb-2">
              Chưa có bài viết nào
            </h3>
            <p className="text-gray-500">
              {isSearching
                ? 'Không tìm thấy bài viết phù hợp'
                : 'Hãy là người đầu tiên chia sẻ câu chuyện của bạn!'}
            </p>
          </Card>
        )}

        {/* Posts List */}
        {posts.length > 0 && <PostList posts={posts} />}

        {/* Load More */}
        <LoadMoreButton
          onClick={handleLoadMore}
          isLoading={activeQuery.isLoading}
          hasMore={hasMore}
        />
      </div>
    </div>
  );
}

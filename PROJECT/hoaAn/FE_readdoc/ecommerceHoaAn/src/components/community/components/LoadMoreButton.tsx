/**
 * LoadMoreButton Component
 */

import { Button } from '@/components/ui/button';
import { Loader2 } from 'lucide-react';

interface LoadMoreButtonProps {
  onClick: () => void;
  isLoading: boolean;
  hasMore: boolean;
}

export const LoadMoreButton = ({ onClick, isLoading, hasMore }: LoadMoreButtonProps) => {
  if (!hasMore) return null;

  return (
    <div className="mt-6 text-center">
      <Button
        onClick={onClick}
        disabled={isLoading}
        variant="outline"
        className="min-w-[200px]"
      >
        {isLoading ? (
          <>
            <Loader2 className="w-4 h-4 mr-2 animate-spin" />
            Đang tải...
          </>
        ) : (
          'Xem thêm bài viết'
        )}
      </Button>
    </div>
  );
};

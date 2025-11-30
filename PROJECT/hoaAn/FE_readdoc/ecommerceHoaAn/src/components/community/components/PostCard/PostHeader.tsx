/**
 * PostHeader Component
 */

import { Badge } from "@/components/ui/badge";
import { Clock, MoreHorizontal, ShieldCheck } from "lucide-react";

interface PostHeaderProps {
  author?: {
    id?: string;
    name?: string;
    avatar?: string;
  };
  createdAt: string;
  isNewest?: boolean;
}

export const PostHeader = ({
  author,
  createdAt,
  isNewest,
}: PostHeaderProps) => {
  const formatTime = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const diff = now.getTime() - date.getTime();
    const hours = Math.floor(diff / (1000 * 60 * 60));
    const days = Math.floor(hours / 24);

    if (hours < 1) return "Vừa xong";
    if (hours < 24) return `${hours} giờ trước`;
    if (days < 7) return `${days} ngày trước`;
    return date.toLocaleDateString("vi-VN");
  };

  return (
    <div className="flex items-start gap-3 mb-4">
      {/* Avatar */}
      <div className="w-12 h-12 rounded-full overflow-hidden bg-gray-200 flex items-center justify-center">
        {author?.avatar ? (
          <img
            src={author.avatar}
            alt="avatar"
            className="w-full h-full object-cover"
          />
        ) : (
          <span className="text-xl">👤</span>
        )}
      </div>

      {/* Info */}
      <div className="flex-1">
        <div className="flex items-start justify-between">
          <div>
            <div className="flex items-center gap-2 mb-1">
              <span className="font-medium text-gray-800">
                {author?.name || "Người dùng ẩn danh"}
              </span>
              {isNewest && (
                <Badge className="bg-green-100 text-green-800 text-xs">
                  Mới nhất
                </Badge>
              )}
            </div>

            <div className="flex items-center gap-2 text-sm text-gray-500">
              <Clock className="w-3 h-3" />
              <span>{formatTime(createdAt)}</span>
            </div>

            {/* Verified */}
            <div className="flex items-center gap-1 mt-1">
              <ShieldCheck className="w-4 h-4 text-blue-600" />
              <span className="text-xs text-gray-600">
                {author?.name || "Người dùng"}
              </span>
            </div>
          </div>

          {/* More button */}
          <button className="text-gray-400 hover:text-gray-600 p-1 rounded-full hover:bg-gray-50 transition-colors">
            <MoreHorizontal className="w-5 h-5" />
          </button>
        </div>
      </div>
    </div>
  );
};

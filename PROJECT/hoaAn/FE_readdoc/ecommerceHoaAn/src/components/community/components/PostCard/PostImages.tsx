// Đề xuất mới: PostImages.tsx (phiên bản tốt hơn rất nhiều)
import { useState } from "react";

interface PostImagesProps {
  photoUrls?: string[]; // Bắt buộc có, không optional
  alt?: string;
}

const normalizeUrl = (url: string): string => {
  if (!url) return "";
  return url
    .replace(/\\/g, "/")
    .replace(/^wwwroot\//, "/")
    .replace(/^\/wwwroot\//, "/")
    .replace(/^\/+/, "/");
};

export const PostImages = ({
  photoUrls = [],
  alt = "Post image",
}: PostImagesProps) => {
  const [failedUrls, setFailedUrls] = useState<Set<string>>(new Set());

  if (!photoUrls || photoUrls.length === 0) {
    return (
      <div className="w-full h-64 bg-gray-100 rounded-lg flex items-center justify-center text-gray-400">
        <span>Không có hình ảnh</span>
      </div>
    );
  }

  const validImages = photoUrls
    .map((url) => ({
      original: url,
      normalized: normalizeUrl(url),
      full: normalizeUrl(url).startsWith("http")
        ? normalizeUrl(url)
        : `http://localhost:3000${normalizeUrl(url)}`,
    }))
    .filter((img) => !failedUrls.has(img.full));

  if (validImages.length === 0) {
    return (
      <div className="w-full h-64 bg-gray-200 rounded-lg flex items-center justify-center">
        <div className="text-center">
          <svg
            className="w-12 h-12 mx-auto text-gray-400"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <p className="text-sm text-gray-500 mt-2">Không tải được hình ảnh</p>
        </div>
      </div>
    );
  }

  // Hiển thị grid nếu nhiều ảnh, single nếu 1 ảnh
  return (
    <div
      className={`grid gap-2 mb-4 ${
        validImages.length === 1 ? "" : "grid-cols-2"
      }`}
    >
      {validImages.slice(0, 4).map((img, i) => (
        <div
          key={i}
          className={`relative overflow-hidden rounded-lg ${
            validImages.length > 1 ? "aspect-square" : "h-80"
          }`}
        >
          <img
            src={img.full}
            alt={`${alt} ${i + 1}`}
            className="w-full h-full object-cover"
            onError={() => setFailedUrls((prev) => new Set(prev).add(img.full))}
          />
          {validImages.length > 4 && i === 3 && (
            <div className="absolute inset-0 bg-black/50 flex items-center justify-center">
              <span className="text-white text-2xl font-semibold">
                +{validImages.length - 4}
              </span>
            </div>
          )}
        </div>
      ))}
    </div>
  );
};

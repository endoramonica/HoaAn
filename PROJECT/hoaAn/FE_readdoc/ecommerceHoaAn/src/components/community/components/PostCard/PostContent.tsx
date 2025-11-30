import React, { useState } from "react";

interface PostContentProps {
  content: string;
}

export const PostContent = ({ content }: PostContentProps) => {
  const [isExpanded, setIsExpanded] = useState(false);
  const maxLength = 280;
  const shouldTruncate = content.length > maxLength;

  const displayContent = isExpanded ? content : content.slice(0, maxLength);

  return (
    <div className="mb-4">
      <p className="text-gray-700 leading-relaxed whitespace-pre-wrap text-[15px]">
        {displayContent}
        {shouldTruncate && !isExpanded && "..."}
      </p>
      {shouldTruncate && (
        <button
          onClick={() => setIsExpanded(!isExpanded)}
          className="text-blue-600 text-sm font-medium mt-1 hover:underline"
        >
          {isExpanded ? "Show less" : "Read more"}
        </button>
      )}
    </div>
  );
};

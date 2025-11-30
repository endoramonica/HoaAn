import React, { useState } from "react";
import { Sparkles, Loader2, PlusCircle } from "lucide-react";

interface PostGeneratorProps {
  onGenerate: (topic: string) => void;
  isGenerating: boolean;
}

export const PostGenerator = ({
  onGenerate,
  isGenerating,
}: PostGeneratorProps) => {
  const [topic, setTopic] = useState("");

  const handleGenerate = () => {
    if (!topic.trim()) {
      // Fallback to a default if empty, or handle validation
      onGenerate("Trending topics");
    } else {
      onGenerate(topic);
    }
    setTopic(""); // Clear input after submitting
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter") {
      handleGenerate();
    }
  };

  return (
    <div className="mb-8 bg-white p-4 rounded-2xl shadow-sm border border-blue-100">
      <div className="flex flex-col sm:flex-row gap-3">
        <div className="flex-1 relative">
          <input
            type="text"
            placeholder="What would you like to see posts about?"
            value={topic}
            onChange={(e) => setTopic(e.target.value)}
            onKeyDown={handleKeyDown}
            className="w-full pl-4 pr-10 py-2.5 bg-gray-50 border-gray-200 rounded-xl focus:ring-2 focus:ring-blue-100 focus:border-blue-400 transition-all outline-none text-sm"
          />
          <Sparkles className="absolute right-3 top-2.5 w-5 h-5 text-blue-400" />
        </div>
        <button
          onClick={handleGenerate}
          disabled={isGenerating}
          className="flex items-center justify-center gap-2 px-6 py-2.5 bg-gray-900 hover:bg-black text-white rounded-xl text-sm font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {isGenerating ? (
            <>
              <Loader2 className="w-4 h-4 animate-spin" />
              Generating...
            </>
          ) : (
            <>
              <PlusCircle className="w-4 h-4" />
              Generate Posts
            </>
          )}
        </button>
      </div>
    </div>
  );
};

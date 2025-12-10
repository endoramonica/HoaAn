/**
 * AiSuggestionsModal Component
 * Modal hiển thị gợi ý bài viết từ AI
 */

import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Sparkles, Loader2, RefreshCw, Check, AlertCircle } from "lucide-react";
import { generateText } from "@/lib/api/geminiService";
import type { PostResponseDto } from "../../../../../Api/generated-orval/schemas";

// Generate AI post suggestions
const generateAiPosts = async (topic: string): Promise<PostResponseDto[]> => {
  const prompt = `Tạo 3 gợi ý bài viết cộng đồng về chủ đề: "${topic}"

Yêu cầu:
- Mỗi bài viết 100-150 từ
- Viết bằng tiếng Việt
- Thân thiện, hấp dẫn
- Phù hợp cho cộng đồng

Định dạng:
POST 1: [nội dung]
POST 2: [nội dung]
POST 3: [nội dung]`;

  const response = await generateText({ prompt, temperature: 0.8 });
  
  // Parse response
  const posts: PostResponseDto[] = [];
  const postMatches = response.match(/POST \d+:\s*(.+?)(?=POST \d+:|$)/gs);
  
  if (postMatches) {
    postMatches.forEach((match, index) => {
      const content = match.replace(/POST \d+:\s*/i, '').trim();
      posts.push({
        id: `ai-${index}`,
        content,
        createdAt: new Date().toISOString(),
      } as PostResponseDto);
    });
  }
  
  return posts;
};

interface AiSuggestionsModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSelectContent: (content: string) => void;
}

export const AiSuggestionsModal = ({
  open,
  onOpenChange,
  onSelectContent,
}: AiSuggestionsModalProps) => {
  const [topic, setTopic] = useState("");
  const [suggestions, setSuggestions] = useState<PostResponseDto[]>([]);
  const [isGenerating, setIsGenerating] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedIndex, setSelectedIndex] = useState<number | null>(null);

  const handleGenerate = async () => {
    setIsGenerating(true);
    setError(null);
    setSuggestions([]);
    setSelectedIndex(null);

    try {
      const topicToUse = topic.trim() || "tâm linh và phát triển bản thân";
      const generated = await generateAiPosts(topicToUse, 5);
      setSuggestions(generated);
    } catch (err) {
      setError("Không thể tạo gợi ý. Vui lòng thử lại.");
      console.error("Generation error:", err);
    } finally {
      setIsGenerating(false);
    }
  };

  const handleSelectSuggestion = (index: number) => {
    setSelectedIndex(index);
    const content = suggestions[index].content || "";
    onSelectContent(content);
    onOpenChange(false);
    // Reset state
    setTimeout(() => {
      setTopic("");
      setSuggestions([]);
      setSelectedIndex(null);
      setError(null);
    }, 300);
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" && !isGenerating) {
      handleGenerate();
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-3xl max-h-[85vh] overflow-hidden flex flex-col">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Sparkles className="w-5 h-5 text-amber-600" />
            Gợi ý nội dung từ AI
          </DialogTitle>
        </DialogHeader>

        <div className="flex-1 overflow-y-auto space-y-4 pr-2">
          {/* Topic Input */}
          <div className="bg-amber-50 p-4 rounded-lg border border-amber-200">
            <div className="flex flex-col sm:flex-row gap-3">
              <div className="flex-1 relative">
                <input
                  type="text"
                  placeholder="Chủ đề bạn muốn viết về... (ví dụ: thiền định, biết ơn)"
                  value={topic}
                  onChange={(e) => setTopic(e.target.value)}
                  onKeyDown={handleKeyDown}
                  disabled={isGenerating}
                  className="w-full px-4 py-2.5 bg-white border border-amber-300 rounded-lg focus:ring-2 focus:ring-amber-400 focus:border-amber-400 transition-all outline-none text-sm disabled:opacity-50"
                />
                <Sparkles className="absolute right-3 top-3 w-5 h-5 text-amber-500" />
              </div>
              <Button
                onClick={handleGenerate}
                disabled={isGenerating}
                className="bg-amber-600 hover:bg-amber-700 text-white px-6"
              >
                {isGenerating ? (
                  <>
                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                    Đang tạo...
                  </>
                ) : (
                  <>
                    <Sparkles className="w-4 h-4 mr-2" />
                    Tạo gợi ý
                  </>
                )}
              </Button>
            </div>
            <p className="text-xs text-amber-700 mt-2">
              💡 Để trống để nhận gợi ý ngẫu nhiên về tâm linh & mindfulness
            </p>
          </div>

          {/* Error State */}
          {error && (
            <Card className="p-4 bg-red-50 border-red-200">
              <div className="flex items-center gap-2 text-red-600 text-sm">
                <AlertCircle className="w-5 h-5" />
                {error}
              </div>
            </Card>
          )}

          {/* Loading State */}
          {isGenerating && (
            <div className="flex flex-col items-center justify-center py-12 gap-3">
              <Loader2 className="w-10 h-10 animate-spin text-amber-600" />
              <p className="text-gray-600 text-sm">
                AI đang tạo gợi ý cho bạn...
              </p>
            </div>
          )}

          {/* Suggestions Grid */}
          {!isGenerating && suggestions.length > 0 && (
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <p className="text-sm font-medium text-gray-700">
                  {suggestions.length} gợi ý được tạo
                </p>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={handleGenerate}
                  className="text-xs"
                >
                  <RefreshCw className="w-3 h-3 mr-1" />
                  Tạo lại
                </Button>
              </div>

              <div className="grid gap-3">
                {suggestions.map((suggestion, index) => (
                  <Card
                    key={suggestion.postId}
                    className={`p-4 cursor-pointer transition-all hover:shadow-md border-2 ${
                      selectedIndex === index
                        ? "border-amber-500 bg-amber-50"
                        : "border-gray-200 hover:border-amber-300"
                    }`}
                    onClick={() => handleSelectSuggestion(index)}
                  >
                    <div className="flex items-start gap-3">
                      <div className="flex-shrink-0 mt-1">
                        {selectedIndex === index ? (
                          <div className="w-6 h-6 rounded-full bg-amber-600 flex items-center justify-center">
                            <Check className="w-4 h-4 text-white" />
                          </div>
                        ) : (
                          <div className="w-6 h-6 rounded-full border-2 border-gray-300" />
                        )}
                      </div>
                      <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 mb-2">
                          <img
                            src={suggestion.customerAvatar || ""}
                            alt={suggestion.customerName || "Avatar"}
                            className="w-6 h-6 rounded-full"
                          />
                          <span className="text-sm font-medium text-gray-700">
                            {suggestion.customerName}
                          </span>
                        </div>
                        <p className="text-sm text-gray-800 whitespace-pre-wrap leading-relaxed">
                          {suggestion.content}
                        </p>
                        <div className="flex items-center gap-4 mt-3 text-xs text-gray-500">
                          <span>❤️ {suggestion.likesCount}</span>
                          <span>💬 {suggestion.commentsCount}</span>
                        </div>
                      </div>
                    </div>
                  </Card>
                ))}
              </div>
            </div>
          )}

          {/* Empty State */}
          {!isGenerating && suggestions.length === 0 && !error && (
            <div className="text-center py-12 text-gray-500">
              <Sparkles className="w-16 h-16 mx-auto mb-4 text-gray-300" />
              <p className="text-sm">
                Nhập chủ đề và nhấn "Tạo gợi ý" để bắt đầu
              </p>
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
};

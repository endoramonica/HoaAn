/**
 * CreatePostDialog Component - With AI Suggestions
 */

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Plus, Send, Loader2, AlertCircle, Sparkles } from "lucide-react";
import { ImageUpload } from "./ImageUpload";
import { AiSuggestionsModal } from "./AiSuggestionsModal";
import { useCreatePost } from "../../hooks/usePosts";
import { ProtectedAction } from "../ProtectedAction";
import { toast } from "sonner";

export const CreatePostDialog = () => {
  const [open, setOpen] = useState(false);
  const [content, setContent] = useState("");
  const [photoFile, setPhotoFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [aiModalOpen, setAiModalOpen] = useState(false);

  const createPostMutation = useCreatePost();

  const handleImageSelect = (file: File) => {
    setPhotoFile(file);
    const reader = new FileReader();
    reader.onload = (e) => {
      setPreviewUrl(e.target?.result as string);
    };
    reader.readAsDataURL(file);
  };

  const handleImageRemove = () => {
    setPhotoFile(null);
    setPreviewUrl(null);
  };

  const handleAiContentSelect = (aiContent: string) => {
    setContent(aiContent);
    toast.success("Đã áp dụng gợi ý từ AI");
  };

  const handleSubmit = async () => {
    if (!content.trim()) return;

    try {
      await createPostMutation.mutateAsync({
        Content: content,
        PhotoFile: photoFile || undefined,
        NotificationOn: "true",
      });

      toast.success("Đã đăng bài viết thành công!");
      setContent("");
      setPhotoFile(null);
      setPreviewUrl(null);
      setOpen(false);
    } catch (error: any) {
      toast.error(error?.message || "Không thể đăng bài. Vui lòng thử lại.");
    }
  };

  return (
    <>
      <Dialog open={open} onOpenChange={setOpen}>
        <ProtectedAction onAction={() => setOpen(true)}>
          {(onClick) => (
            <DialogTrigger asChild>
              <Button
                onClick={onClick}
                className="w-full bg-amber-600 hover:bg-amber-700 text-white"
              >
                <Plus className="w-4 h-4 mr-2" />
                Chia sẻ bài viết mới
              </Button>
            </DialogTrigger>
          )}
        </ProtectedAction>

        <DialogContent className="max-w-lg">
          <DialogHeader>
            <DialogTitle>Đăng bài viết mới</DialogTitle>
          </DialogHeader>

          <div className="space-y-4">
            {/* AI Suggestion Button */}
            <div className="flex items-center justify-between">
              <Label htmlFor="content">Bạn đang nghĩ gì?</Label>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={() => setAiModalOpen(true)}
                className="text-amber-600 border-amber-300 hover:bg-amber-50"
              >
                <Sparkles className="w-4 h-4 mr-1" />
                Gợi ý AI
              </Button>
            </div>

            {/* Content Textarea */}
            <Textarea
              id="content"
              placeholder="Chia sẻ suy nghĩ, kinh nghiệm tâm linh của bạn..."
              rows={6}
              value={content}
              onChange={(e) => setContent(e.target.value)}
              className="resize-none"
            />

            {/* Character Count */}
            <div className="flex justify-between items-center text-xs text-gray-500">
              <span>{content.length > 0 && `${content.length} ký tự`}</span>
              {content.length > 4500 && (
                <span className="text-red-500">Tối đa 5000 ký tự</span>
              )}
            </div>

            {/* Image Upload */}
            <ImageUpload
              images={photoFile ? [photoFile] : []}
              previewUrls={previewUrl ? [previewUrl] : []}
              onImagesChange={(files) => {
                const file = files[0] ?? null;
                setPhotoFile(file);
                setPreviewUrl(file ? URL.createObjectURL(file) : null);
              }}
              disabled={createPostMutation.isPending}
            />

            {/* Error Message */}
            {createPostMutation.isError && (
              <div className="flex items-center gap-2 text-red-600 text-sm">
                <AlertCircle className="w-4 h-4" />
                {createPostMutation.error?.message || "Đã xảy ra lỗi"}
              </div>
            )}

            {/* Action Buttons */}
            <div className="flex justify-end gap-2">
              <Button
                variant="outline"
                onClick={() => setOpen(false)}
                disabled={createPostMutation.isPending}
              >
                Hủy
              </Button>
              <Button
                onClick={handleSubmit}
                disabled={
                  !content.trim() ||
                  content.length > 5000 ||
                  createPostMutation.isPending
                }
                className="bg-amber-600 hover:bg-amber-700"
              >
                {createPostMutation.isPending ? (
                  <>
                    <Loader2 className="w-4 h-4 mr-1 animate-spin" />
                    Đang đăng...
                  </>
                ) : (
                  <>
                    <Send className="w-4 h-4 mr-1" />
                    Đăng bài
                  </>
                )}
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>

      {/* AI Suggestions Modal */}
      <AiSuggestionsModal
        open={aiModalOpen}
        onOpenChange={setAiModalOpen}
        onSelectContent={handleAiContentSelect}
      />
    </>
  );
};

import { useState, useRef, useEffect } from "react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import {
  MessageCircle,
  Plus,
  Search,
  Users,
  Send,
  X,
  Camera,
  Loader2,
  AlertCircle,
} from "lucide-react";
import { PostCard } from "@/components/community/components/PostCard";
import { getActionTrackingService } from "@/lib/services/actionTrackingService";

interface CommunityPageProps {
  onBack: () => void;
  postsService: any;
}
// [X]TODO FIXED: Added optional properties to Post interface
// [ ] add comments service into props 
// [ ] TODO FIXED: Added error handling for image loading - there are no images in the current posts
// [ ]  add token and autheticated for comunity. 
// [ ]  fix the errors when we post the comments. 


// Updated Post interface to match MixedFeedDto structure
interface Post {
  id: string;
  postType?: string;
  customerId?: string;
  customerName?: string;
  customerAvatar?: string;
  content: string;
  imageUrl?: string;
  imageUrls?: string[];
  likesCount: number;
  commentsCount: number;
  sharesCount: number;
  createdAt: string;
  isLikedByCurrentUser: boolean;
  isBookmarkedByCurrentUser: boolean;
}

export default function CommunityPage({
  onBack,
  postsService,
}: CommunityPageProps) {
  const [searchQuery, setSearchQuery] = useState("");
  const [showNewPostDialog, setShowNewPostDialog] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const actionTracking = getActionTrackingService();

  const [posts, setPosts] = useState<Post[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);

  const [newPost, setNewPost] = useState({
    content: "",
    photoFile: null as File | null,
    previewUrl: null as string | null,
  });

  useEffect(() => {
    fetchPosts();
  }, []);

  const fetchPosts = async (page = 1, append = false) => {
    try {
      setIsLoading(true);
      setError(null);

      // Use MixedFeed endpoint instead of PostsFeed
      const response = await postsService.getApiV1MixedFeed({ PageNumber: page, PageSize: 20 });

      if (response.data) {
        const newPosts = response.data.items || [];

        if (append) {
          setPosts((prev) => [...prev, ...newPosts]);
        } else {
          setPosts(newPosts);
        }

        setHasMore(newPosts.length === 20);
        setCurrentPage(page);
      }
    } catch (err: any) {
      setError(err?.message || "Không thể tải bài viết. Vui lòng thử lại.");
      console.error("Error fetching posts:", err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSearch = async (keyword: string) => {
    if (!keyword.trim()) {
      fetchPosts();
      return;
    }

    try {
      setIsLoading(true);
      setError(null);

      const response = await postsService.getApiV1PostsSearch(
        keyword,
        undefined,
        undefined,
        1,
        20
      );

      if (response.data) {
        setPosts(response.data.items || []);
      }
    } catch (err: any) {
      setError("Không thể tìm kiếm. Vui lòng thử lại.");
      console.error("Error searching posts:", err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    const timer = setTimeout(() => {
      if (searchQuery) {
        handleSearch(searchQuery);
      } else {
        fetchPosts();
      }
    }, 500);

    return () => clearTimeout(timer);
  }, [searchQuery]);

  // Handle post updates from PostCard
  const handlePostUpdate = (postId: string, updates: any) => {
    // Track ViewProduct action when post is viewed/updated
    actionTracking.trackAction('ViewProduct', { postType: 'community-post' }, postId, 'community');
    
    setPosts((prev) =>
      prev.map((post) =>
        post.id === postId ? { ...post, ...updates } : post
      )
    );
  };

  const handleImageUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    if (!file.type.startsWith("image/")) {
      alert("Vui lòng chọn file hình ảnh");
      return;
    }

    const reader = new FileReader();
    reader.onload = (e) => {
      setNewPost((prev) => ({
        ...prev,
        photoFile: file,
        previewUrl: e.target?.result as string,
      }));
    };
    reader.readAsDataURL(file);
  };

  const removeImage = () => {
    setNewPost((prev) => ({
      ...prev,
      photoFile: null,
      previewUrl: null,
    }));
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleSubmitPost = async () => {
    if (!newPost.content.trim()) return;

    try {
      setIsSubmitting(true);
      setError(null);

      const formData = {
        Content: newPost.content,
        PhotoFile: newPost.photoFile || undefined,
        NotificationOn: new Date().toISOString(),
      };

      const response = await postsService.postApiV1Posts(formData);

      if (response.data) {
        // Track AddToCart action for post submission (engagement)
        actionTracking.trackAction('AddToCart', { action: 'submit-post' }, response.data.id, 'community');
        
        setPosts((prev) => [response.data, ...prev]);
        setNewPost({ content: "", photoFile: null, previewUrl: null });
        setShowNewPostDialog(false);
        alert("Đã đăng bài viết thành công!");
      }
    } catch (err: any) {
      setError(err?.message || "Không thể đăng bài. Vui lòng thử lại.");
      console.error("Error creating post:", err);
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleLoadMore = () => {
    if (!isLoading && hasMore) {
      fetchPosts(currentPage + 1, true);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-orange-50 to-amber-50">
      <div className="bg-gradient-to-r from-amber-800 to-orange-800 text-white sticky top-0 z-40">
        <div className="max-w-2xl mx-auto px-4 py-4">
          <div className="flex items-center justify-between mb-4">
            <button
              onClick={onBack}
              className="text-amber-100 hover:text-white"
            >
              ← Quay lại
            </button>
            <h1 className="text-xl font-semibold">Cộng đồng Tâm Linh</h1>
            <div className="w-20"></div>
          </div>

          <div className="text-center">
            <p className="text-amber-100 text-sm mb-2">
              Chia sẻ - Kết nối - Học hỏi
            </p>
            <div className="flex items-center justify-center gap-6 text-sm">
              <span className="flex items-center gap-1">
                <Users className="w-4 h-4" />
                {posts.length * 142} thành viên
              </span>
              <span className="flex items-center gap-1">
                <MessageCircle className="w-4 h-4" />
                {posts.length} bài viết
              </span>
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-2xl mx-auto px-4 py-6">
        <div className="mb-6 space-y-4">
          <div className="relative">
            <Search className="absolute left-3 top-3 w-4 h-4 text-gray-400" />
            <Input
              placeholder="Tìm kiếm bài viết..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="pl-10"
            />
          </div>

          <Dialog open={showNewPostDialog} onOpenChange={setShowNewPostDialog}>
            <DialogTrigger asChild>
              <Button className="w-full bg-amber-600 hover:bg-amber-700 text-white">
                <Plus className="w-4 h-4 mr-2" />
                Chia sẻ bài viết mới
              </Button>
            </DialogTrigger>
            <DialogContent className="max-w-lg">
              <DialogHeader>
                <DialogTitle>Đăng bài viết mới</DialogTitle>
              </DialogHeader>
              <div className="space-y-4">
                <div>
                  <Label htmlFor="content">Bạn đang nghĩ gì?</Label>
                  <Textarea
                    id="content"
                    placeholder="Chia sẻ suy nghĩ, kinh nghiệm tâm linh của bạn..."
                    rows={4}
                    value={newPost.content}
                    onChange={(e) =>
                      setNewPost((prev) => ({
                        ...prev,
                        content: e.target.value,
                      }))
                    }
                  />
                </div>

                <div>
                  <div className="flex items-center gap-2 mb-3">
                    <Label>Hình ảnh</Label>
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={() => fileInputRef.current?.click()}
                      disabled={!!newPost.photoFile}
                    >
                      <Camera className="w-4 h-4 mr-1" />
                      Thêm ảnh
                    </Button>
                  </div>

                  <input
                    ref={fileInputRef}
                    type="file"
                    accept="image/*"
                    onChange={handleImageUpload}
                    className="hidden"
                  />

                  {newPost.previewUrl && (
                    <div className="relative">
                      <img
                        src={newPost.previewUrl}
                        alt="Preview"
                        className="w-full h-48 object-cover rounded-lg"
                      />
                      <button
                        onClick={removeImage}
                        className="absolute -top-2 -right-2 w-6 h-6 bg-red-500 text-white rounded-full flex items-center justify-center hover:bg-red-600"
                      >
                        <X className="w-3 h-3" />
                      </button>
                    </div>
                  )}
                </div>

                {error && (
                  <div className="flex items-center gap-2 text-red-600 text-sm">
                    <AlertCircle className="w-4 h-4" />
                    {error}
                  </div>
                )}

                <div className="flex justify-end gap-2">
                  <Button
                    variant="outline"
                    onClick={() => setShowNewPostDialog(false)}
                    disabled={isSubmitting}
                  >
                    Hủy
                  </Button>
                  <Button
                    onClick={handleSubmitPost}
                    disabled={!newPost.content.trim() || isSubmitting}
                    className="bg-amber-600 hover:bg-amber-700"
                  >
                    {isSubmitting ? (
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
        </div>

        {error && !isSubmitting && (
          <Card className="p-4 mb-6 bg-red-50 border-red-200">
            <div className="flex items-center gap-2 text-red-600">
              <AlertCircle className="w-5 h-5" />
              <p>{error}</p>
            </div>
          </Card>
        )}

        {isLoading && posts.length === 0 && (
          <div className="flex justify-center items-center py-12">
            <Loader2 className="w-8 h-8 animate-spin text-amber-600" />
          </div>
        )}

        {!isLoading && posts.length === 0 && (
          <Card className="p-12 text-center">
            <MessageCircle className="w-16 h-16 mx-auto mb-4 text-gray-300" />
            <h3 className="text-lg font-semibold text-gray-700 mb-2">
              Chưa có bài viết nào
            </h3>
            <p className="text-gray-500">
              Hãy là người đầu tiên chia sẻ câu chuyện của bạn!
            </p>
          </Card>
        )}

        <div className="space-y-6">
          {posts.map((post, index) => (
            <PostCard
              key={post.id}
              post={post}
              isNewest={index === 0}
              onPostUpdate={handlePostUpdate}
            />
          ))}
        </div>

        {hasMore && posts.length > 0 && (
          <div className="mt-6 text-center">
            <Button
              onClick={handleLoadMore}
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
                "Xem thêm bài viết"
              )}
            </Button>
          </div>
        )}
      </div>
    </div>
  );
}

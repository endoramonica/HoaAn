import { useState, useRef } from 'react';
import { Card } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Textarea } from './ui/textarea';
import { Badge } from './ui/badge';
import { Avatar } from './ui/avatar';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from './ui/dialog';
import { Label } from './ui/label';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { 
  MessageCircle, 
  Heart, 
  Share, 
  Plus, 
  Search, 
  Clock,
  Users,
  Send,
  Image as ImageIcon,
  X,
  Camera
} from 'lucide-react';

interface CommunityPageProps {
  onBack: () => void;
}

interface Post {
  id: number;
  author: string;
  avatar: string;
  content: string;
  images: string[];
  likes: number;
  comments: number;
  shares: number;
  timestamp: Date;
  isLiked: boolean;
}

interface Comment {
  id: number;
  postId: number;
  author: string;
  avatar: string;
  content: string;
  likes: number;
  timestamp: Date;
  isLiked: boolean;
}

export function CommunityPage({ onBack }: CommunityPageProps) {
  const [searchQuery, setSearchQuery] = useState('');
  const [showNewPostDialog, setShowNewPostDialog] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  
  // New post form state
  const [newPost, setNewPost] = useState({
    content: '',
    images: [] as string[]
  });

  const [posts, setPosts] = useState<Post[]>([
    {
      id: 1,
      author: "Minh Châu",
      avatar: "👩‍🦳",
      content: "Hôm nay đi chùa cầu an cho gia đình, cảm thấy tâm hồn thật bình yên. Ai có kinh nghiệm về việc bày trí bàn thờ gia tiên cho đúng phong thủy không ạ? Gia đình mình sắp chuyển nhà mới.",
      images: [],
      likes: 24,
      comments: 8,
      shares: 5,
      timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000), // 2 hours ago
      isLiked: false
    },
    {
      id: 2,
      author: "Thanh Hương",
      avatar: "🧑‍💼",
      content: "Chia sẻ với mọi người về chuyến đi cầu duyên tại chùa Hà. Quy trình rất trang nghiêm và ý nghĩa. Mình đã chuẩn bị hoa quả, hương nến như hướng dẫn và thật sự cảm nhận được năng lượng tích cực.",
      images: ["https://images.unsplash.com/photo-1570129477492-45c003edd2be?w=500", "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=500"],
      likes: 67,
      comments: 23,
      shares: 12,
      timestamp: new Date(Date.now() - 5 * 60 * 60 * 1000), // 5 hours ago
      isLiked: true
    },
    {
      id: 3,
      author: "Đức Minh",
      avatar: "👨‍🎓",
      content: "Mọi người cho mình hỏi, tại sao khi thắp hương cầu nguyện thường thắp 3 nén? Có ý nghĩa tâm linh gì đặc biệt không ạ? Mình mới tìm hiểu về văn hóa tâm linh nên chưa rõ lắm.",
      images: ["https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=500"],
      likes: 18,
      comments: 12,
      shares: 3,
      timestamp: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000), // 1 day ago
      isLiked: false
    }
  ]);

  const [comments] = useState<Comment[]>([
    {
      id: 1,
      postId: 1,
      author: "Bà Lan",
      avatar: "👵",
      content: "Bàn thờ nên đặt ở vị trí cao nhất trong nhà, hướng ra cửa chính nhé con.",
      likes: 15,
      timestamp: new Date(Date.now() - 1 * 60 * 60 * 1000),
      isLiked: false
    },
    {
      id: 2,
      postId: 2,
      author: "Thầy Minh",
      avatar: "🧙‍♂️",
      content: "Cảm ơn bạn đã chia sẻ. Năng lượng tâm linh thật sự có thể cảm nhận được khi ta có tâm thành.",
      likes: 12,
      timestamp: new Date(Date.now() - 30 * 60 * 1000),
      isLiked: true
    }
  ]);

  const filteredPosts = posts.filter(post => 
    post.content.toLowerCase().includes(searchQuery.toLowerCase()) ||
    post.author.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const handleLikePost = (postId: number) => {
    setPosts(prev => prev.map(post => 
      post.id === postId 
        ? { ...post, likes: post.isLiked ? post.likes - 1 : post.likes + 1, isLiked: !post.isLiked }
        : post
    ));
  };

  const handleImageUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = event.target.files;
    if (!files) return;

    Array.from(files).forEach(file => {
      if (file.type.startsWith('image/')) {
        const reader = new FileReader();
        reader.onload = (e) => {
          const result = e.target?.result as string;
          setNewPost(prev => ({
            ...prev,
            images: [...prev.images, result]
          }));
        };
        reader.readAsDataURL(file);
      }
    });
  };

  const removeImage = (index: number) => {
    setNewPost(prev => ({
      ...prev,
      images: prev.images.filter((_, i) => i !== index)
    }));
  };

  const handleSubmitPost = () => {
    if (!newPost.content.trim()) return;

    const post: Post = {
      id: Date.now(),
      author: "Bạn",
      avatar: "😊",
      content: newPost.content,
      images: newPost.images,
      likes: 0,
      comments: 0,
      shares: 0,
      timestamp: new Date(),
      isLiked: false
    };

    // Add to top of feed (state update, no reload needed)
    setPosts(prev => [post, ...prev]);
    setNewPost({ content: '', images: [] });
    setShowNewPostDialog(false);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-orange-50 to-amber-50">
      {/* Header */}
      <div className="bg-gradient-to-r from-amber-800 to-orange-800 text-white sticky top-0 z-40">
        <div className="max-w-2xl mx-auto px-4 py-4">
          <div className="flex items-center justify-between mb-4">
            <button
              onClick={onBack}
              className="text-amber-100 hover:text-white"
            >
              ← Quay lại
            </button>
            <h1 className="text-xl">Cộng đồng Tâm Linh</h1>
            <div className="w-6"></div>
          </div>
          
          <div className="text-center">
            <p className="text-amber-100 text-sm mb-2">Chia sẻ - Kết nối - Học hỏi</p>
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
        {/* Search and New Post */}
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
                    onChange={(e) => setNewPost(prev => ({ ...prev, content: e.target.value }))}
                  />
                </div>

                {/* Image Upload */}
                <div>
                  <div className="flex items-center gap-2 mb-3">
                    <Label>Hình ảnh</Label>
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={() => fileInputRef.current?.click()}
                    >
                      <Camera className="w-4 h-4 mr-1" />
                      Thêm ảnh
                    </Button>
                  </div>
                  
                  <input
                    ref={fileInputRef}
                    type="file"
                    accept="image/*"
                    multiple
                    onChange={handleImageUpload}
                    className="hidden"
                  />

                  {/* Image Preview */}
                  {newPost.images.length > 0 && (
                    <div className="grid grid-cols-2 gap-2 mb-3">
                      {newPost.images.map((image, index) => (
                        <div key={index} className="relative">
                          <img
                            src={image}
                            alt={`Preview ${index + 1}`}
                            className="w-full h-24 object-cover rounded-lg"
                          />
                          <button
                            onClick={() => removeImage(index)}
                            className="absolute -top-2 -right-2 w-6 h-6 bg-red-500 text-white rounded-full flex items-center justify-center hover:bg-red-600"
                          >
                            <X className="w-3 h-3" />
                          </button>
                        </div>
                      ))}
                    </div>
                  )}
                </div>

                <div className="flex justify-end gap-2">
                  <Button 
                    variant="outline" 
                    onClick={() => setShowNewPostDialog(false)}
                  >
                    Hủy
                  </Button>
                  <Button 
                    onClick={handleSubmitPost}
                    disabled={!newPost.content.trim()}
                    className="bg-amber-600 hover:bg-amber-700"
                  >
                    <Send className="w-4 h-4 mr-1" />
                    Đăng bài
                  </Button>
                </div>
              </div>
            </DialogContent>
          </Dialog>
        </div>

        {/* Posts Feed */}
        <div className="space-y-6">
          {filteredPosts.map((post) => (
            <Card key={post.id} className="p-6 bg-white shadow-md hover:shadow-lg transition-shadow">
              {/* Post Header */}
              <div className="flex items-start gap-3 mb-4">
                <div className="w-12 h-12 bg-gradient-to-br from-amber-200 to-orange-300 rounded-full flex items-center justify-center text-xl">
                  {post.avatar}
                </div>
                <div className="flex-1">
                  <div className="flex items-center gap-2 mb-1">
                    <span className="text-gray-800">{post.author}</span>
                    {post.id === posts[0]?.id && (
                      <Badge className="bg-green-100 text-green-800 text-xs">
                        Mới đăng
                      </Badge>
                    )}
                  </div>
                  <div className="flex items-center gap-2 text-sm text-gray-500">
                    <Clock className="w-3 h-3" />
                    <span>{post.timestamp.toLocaleString('vi-VN')}</span>
                  </div>
                </div>
              </div>

              {/* Post Content */}
              <div className="mb-4">
                <p className="text-gray-700 leading-relaxed">{post.content}</p>
              </div>

              {/* Post Images */}
              {post.images.length > 0 && (
                <div className={`mb-4 ${
                  post.images.length === 1 ? 'grid grid-cols-1' :
                  post.images.length === 2 ? 'grid grid-cols-2 gap-2' :
                  'grid grid-cols-2 gap-2'
                }`}>
                  {post.images.slice(0, 4).map((image, index) => (
                    <div key={index} className="relative">
                      <ImageWithFallback
                        src={image}
                        alt={`Post image ${index + 1}`}
                        className={`w-full object-cover rounded-lg ${
                          post.images.length === 1 ? 'h-64' : 'h-32'
                        }`}
                      />
                      {index === 3 && post.images.length > 4 && (
                        <div className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center">
                          <span className="text-white">+{post.images.length - 4}</span>
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              )}

              {/* Post Actions */}
              <div className="flex items-center justify-between pt-3 border-t border-gray-100">
                <div className="flex gap-6">
                  <button
                    onClick={() => handleLikePost(post.id)}
                    className={`flex items-center gap-2 text-sm transition-colors ${
                      post.isLiked ? 'text-red-600' : 'text-gray-500 hover:text-red-600'
                    }`}
                  >
                    <Heart className={`w-5 h-5 ${post.isLiked ? 'fill-current' : ''}`} />
                    {post.likes}
                  </button>
                  <button className="flex items-center gap-2 text-sm text-gray-500 hover:text-amber-600">
                    <MessageCircle className="w-5 h-5" />
                    {post.comments}
                  </button>
                  <button className="flex items-center gap-2 text-sm text-gray-500 hover:text-blue-600">
                    <Share className="w-5 h-5" />
                    {post.shares}
                  </button>
                </div>
              </div>

              {/* Comments Preview */}
              {comments.filter(c => c.postId === post.id).length > 0 && (
                <div className="mt-4 pt-4 border-t border-gray-100">
                  <div className="space-y-3">
                    {comments.filter(c => c.postId === post.id).slice(0, 2).map((comment) => (
                      <div key={comment.id} className="flex gap-3">
                        <div className="w-8 h-8 bg-gradient-to-br from-gray-200 to-gray-300 rounded-full flex items-center justify-center text-sm">
                          {comment.avatar}
                        </div>
                        <div className="flex-1">
                          <div className="bg-gray-50 rounded-lg p-3">
                            <div className="flex items-center gap-2 mb-1">
                              <span className="text-sm text-gray-800">{comment.author}</span>
                              <span className="text-xs text-gray-500">
                                {comment.timestamp.toLocaleString('vi-VN')}
                              </span>
                            </div>
                            <p className="text-sm text-gray-600">{comment.content}</p>
                          </div>
                          <div className="flex items-center gap-2 mt-1">
                            <button className="flex items-center gap-1 text-xs text-gray-500 hover:text-red-600">
                              <Heart className="w-3 h-3" />
                              {comment.likes}
                            </button>
                            <button className="text-xs text-gray-500 hover:text-amber-600">
                              Trả lời
                            </button>
                          </div>
                        </div>
                      </div>
                    ))}
                    {comments.filter(c => c.postId === post.id).length > 2 && (
                      <button className="text-sm text-amber-600 hover:underline ml-11">
                        Xem thêm bình luận...
                      </button>
                    )}
                  </div>
                </div>
              )}
            </Card>
          ))}
        </div>
      </div>
    </div>
  );
}

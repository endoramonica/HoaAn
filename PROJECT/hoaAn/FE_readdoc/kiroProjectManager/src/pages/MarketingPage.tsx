import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { Modal } from '../components/ui/Modal';
import { Input } from '../components/ui/Input';
import { 
    useMarketingPosts, 
    useDeleteMarketingPost,
    usePublishMarketingPost,
    useDuplicateMarketingPost,
    useMarketingStatistics 
} from '../lib/hooks/useMarketing';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';
import { 
    Plus, 
    Edit, 
    Trash2, 
    Calendar, 
    Search,
    Copy,
    Eye,
    MousePointerClick,
    Share2,
    Filter,
    BarChart3,
    Send
} from 'lucide-react';
import { useState } from 'react';

export const MarketingPage = () => {
    const navigate = useNavigate();
    
    // Filters
    const [searchTerm, setSearchTerm] = useState('');
    const [statusFilter, setStatusFilter] = useState<'draft' | 'published' | 'scheduled' | ''>('');
    const [showFilters, setShowFilters] = useState(false);
    
    const { data: posts = [], isLoading } = useMarketingPosts({
        searchTerm: searchTerm || undefined,
        status: statusFilter || undefined,
    });
    const { data: statistics } = useMarketingStatistics();

    // Debug logs
    console.log('Posts data:', posts);
    console.log('Posts length:', posts.length);
    console.log('Statistics:', statistics);
    console.log('Is Loading:', isLoading);
    
    const deletePost = useDeleteMarketingPost();
    const publishPost = usePublishMarketingPost();
    const duplicatePost = useDuplicateMarketingPost();
    
    const [deleteId, setDeleteId] = useState<string | null>(null);

    const handleDelete = () => {
        if (!deleteId) return;

        deletePost.mutate(deleteId, {
            onSuccess: () => {
                toast.success('Xóa bài viết thành công');
                setDeleteId(null);
            },
            onError: () => {
                toast.error('Xóa bài viết thất bại');
            },
        });
    };

    const handlePublish = (id: string) => {
        publishPost.mutate(id, {
            onSuccess: () => {
                toast.success('Đã đăng bài viết');
            },
            onError: () => {
                toast.error('Đăng bài viết thất bại');
            },
        });
    };

    const handleDuplicate = (id: string) => {
        duplicatePost.mutate(id, {
            onSuccess: () => {
                toast.success('Đã sao chép bài viết');
            },
            onError: () => {
                toast.error('Sao chép bài viết thất bại');
            },
        });
    };

    const getStatusBadge = (status: string) => {
        const statusLower = status?.toLowerCase();
        switch (statusLower) {
            case 'published': return <Badge variant="success">Đã đăng</Badge>;
            case 'scheduled': return <Badge variant="info">Đã lên lịch</Badge>;
            case 'archived': return <Badge variant="warning">Lưu trữ</Badge>;
            default: return <Badge variant="default">Nháp</Badge>;
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">
                    Marketing AI
                </h1>
                <Button
                    variant="primary"
                    icon={<Plus className="w-5 h-5" />}
                    onClick={() => navigate('/marketing/edit/new')}
                >
                    Tạo Bài Viết
                </Button>
            </div>

            {/* Statistics */}
            {statistics && (
                <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                    <Card>
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-gray-600">Tổng Bài Viết</p>
                                <p className="text-2xl font-bold text-gray-900">{(statistics as any).total || 0}</p>
                            </div>
                            <BarChart3 className="w-8 h-8 text-blue-500" />
                        </div>
                    </Card>
                    <Card>
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-gray-600">Lượt Xem</p>
                                <p className="text-2xl font-bold text-gray-900">{(statistics as any).totalViews || 0}</p>
                            </div>
                            <Eye className="w-8 h-8 text-green-500" />
                        </div>
                    </Card>
                    <Card>
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-gray-600">Lượt Click</p>
                                <p className="text-2xl font-bold text-gray-900">{(statistics as any).totalClicks || 0}</p>
                            </div>
                            <MousePointerClick className="w-8 h-8 text-purple-500" />
                        </div>
                    </Card>
                    <Card>
                        <div className="flex items-center justify-between">
                            <div>
                                <p className="text-sm text-gray-600">Lượt Chia Sẻ</p>
                                <p className="text-2xl font-bold text-gray-900">{(statistics as any).totalShares || 0}</p>
                            </div>
                            <Share2 className="w-8 h-8 text-orange-500" />
                        </div>
                    </Card>
                </div>
            )}

            {/* Filters */}
            <Card>
                <div className="flex gap-4 mb-4">
                    <Input
                        placeholder="Tìm kiếm bài viết..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        icon={<Search className="w-5 h-5" />}
                        className="flex-1"
                    />
                    <Button
                        variant="ghost"
                        icon={<Filter className="w-5 h-5" />}
                        onClick={() => setShowFilters(!showFilters)}
                    >
                        Lọc
                    </Button>
                </div>

                {showFilters && (
                    <div className="mb-4 p-4 bg-gray-50 rounded-lg">
                        <div className="grid grid-cols-3 gap-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Trạng Thái
                                </label>
                                <select
                                    value={statusFilter}
                                    onChange={(e) => setStatusFilter(e.target.value as any)}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                >
                                    <option value="">Tất cả</option>
                                    <option value="draft">Nháp</option>
                                    <option value="published">Đã đăng</option>
                                    <option value="scheduled">Đã lên lịch</option>
                                </select>
                            </div>
                        </div>
                    </div>
                )}
            </Card>

            {isLoading ? (
                <div className="text-center py-12">
                    <p className="text-gray-500">Đang tải...</p>
                </div>
            ) : posts.length === 0 ? (
                <Card>
                    <div className="text-center py-12">
                        <p className="text-gray-500 mb-4">
                            Chưa có bài viết nào
                        </p>
                        <Button
                            variant="primary"
                            onClick={() => navigate('/marketing/edit/new')}
                        >
                            Tạo Bài Viết Đầu Tiên
                        </Button>
                    </div>
                </Card>
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    {posts.map((post: any) => (
                        <Card key={post.id}>
                            {post.image && (
                                <img
                                    src={post.image}
                                    alt={post.title}
                                    className="w-full h-48 object-cover rounded-lg mb-4"
                                />
                            )}
                            <h3 className="text-lg font-semibold text-gray-900 mb-2">
                                {post.title}
                            </h3>
                            {post.productName && (
                                <p className="text-xs text-blue-600 mb-2">
                                    📦 {post.productName}
                                </p>
                            )}
                            <p className="text-gray-600 text-sm mb-3 line-clamp-3">
                                {post.shortDescription}
                            </p>
                            
                            {/* Hashtags */}
                            {post.hashtags && post.hashtags.length > 0 && (
                                <div className="flex flex-wrap gap-1 mb-3">
                                    {post.hashtags.slice(0, 3).map((tag: string, i: number) => (
                                        <span key={i} className="text-xs text-blue-600">
                                            #{tag}
                                        </span>
                                    ))}
                                    {post.hashtags.length > 3 && (
                                        <span className="text-xs text-gray-500">
                                            +{post.hashtags.length - 3}
                                        </span>
                                    )}
                                </div>
                            )}

                            {/* Stats */}
                            <div className="flex items-center gap-3 mb-3 text-xs text-gray-500">
                                <span className="flex items-center gap-1">
                                    <Eye className="w-3 h-3" />
                                    {post.views || 0}
                                </span>
                                <span className="flex items-center gap-1">
                                    <MousePointerClick className="w-3 h-3" />
                                    {post.clicks || 0}
                                </span>
                                <span className="flex items-center gap-1">
                                    <Share2 className="w-3 h-3" />
                                    {post.shares || 0}
                                </span>
                            </div>

                            <div className="flex items-center justify-between mb-3">
                                {getStatusBadge(post.status || 'draft')}
                                {post.scheduledDate && (
                                    <span className="text-xs text-gray-500 flex items-center gap-1">
                                        <Calendar className="w-3 h-3" />
                                        {new Date(post.scheduledDate).toLocaleDateString('vi-VN')}
                                    </span>
                                )}
                            </div>

                            <div className="flex items-center gap-2 flex-wrap">
                                {post.status?.toLowerCase() === 'draft' && (
                                    <Button
                                        variant="primary"
                                        size="sm"
                                        icon={<Send className="w-4 h-4" />}
                                        onClick={() => handlePublish(post.id)}
                                        isLoading={publishPost.isPending}
                                    >
                                        Đăng
                                    </Button>
                                )}
                                <Button
                                    variant="ghost"
                                    size="sm"
                                    icon={<Edit className="w-4 h-4" />}
                                    onClick={() => navigate(`/marketing/edit/${post.id}`)}
                                >
                                    Sửa
                                </Button>
                                <Button
                                    variant="ghost"
                                    size="sm"
                                    icon={<Copy className="w-4 h-4" />}
                                    onClick={() => handleDuplicate(post.id)}
                                    isLoading={duplicatePost.isPending}
                                >
                                    Sao chép
                                </Button>
                                <Button
                                    variant="danger"
                                    size="sm"
                                    icon={<Trash2 className="w-4 h-4" />}
                                    onClick={() => setDeleteId(post.id)}
                                >
                                    Xóa
                                </Button>
                            </div>
                        </Card>
                    ))}
                </div>
            )}

            <Modal
                isOpen={!!deleteId}
                onClose={() => setDeleteId(null)}
                title="Xác Nhận Xóa"
                footer={
                    <>
                        <Button variant="ghost" onClick={() => setDeleteId(null)}>
                            Hủy
                        </Button>
                        <Button
                            variant="danger"
                            onClick={handleDelete}
                            isLoading={deletePost.isPending}
                        >
                            Xóa
                        </Button>
                    </>
                }
            >
                <p className="text-gray-700">
                    Bạn có chắc chắn muốn xóa bài viết này?
                </p>
            </Modal>
        </div>
    );
};

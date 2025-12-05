import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Select } from '../components/ui/Select';
import { useMarketingPost, useCreateMarketingPost, useUpdateMarketingPost } from '../lib/hooks/useMarketing';
import { 
    generateMarketingContent, 
    generateSocialMediaPosts,
    improveMarketingContent,
    generateSEOContent 
} from '../lib/services/geminiService';
import { useProducts } from '../lib/hooks/useProducts';
import { toast } from 'sonner';
import { Sparkles, Image as ImageIcon, Save, RefreshCw, Copy, Wand2 } from 'lucide-react';

export const MarketingEditPage = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const isNew = id === 'new';

    const { data: post } = useMarketingPost(id || '');
    const createPost = useCreateMarketingPost();
    const updatePost = useUpdateMarketingPost();

    const [title, setTitle] = useState('');
    const [content, setContent] = useState('');
    const [hashtags, setHashtags] = useState<string[]>([]);
    const [selectedProduct, setSelectedProduct] = useState('');
    const [platform, setPlatform] = useState('facebook');
    const [tone, setTone] = useState<'professional' | 'casual' | 'enthusiastic' | 'friendly'>('friendly');
    const [image, setImage] = useState('');
    const [status, setStatus] = useState<'draft' | 'published' | 'scheduled'>('draft');
    const [scheduledDate, setScheduledDate] = useState('');
    const [isGenerating, setIsGenerating] = useState(false);
    const [socialPosts, setSocialPosts] = useState<Record<string, string>>({});
    const [metaTitle, setMetaTitle] = useState('');
    const [metaDescription, setMetaDescription] = useState('');
    const [metaKeywords, setMetaKeywords] = useState<string[]>([]);

    const { data: productsData } = useProducts({});
    const products = (productsData?.data?.data as any)?.data?.items || [];

    useEffect(() => {
        if (post && !isNew) {
            setTitle(post.title || '');
            setContent(post.content || '');
            setHashtags(post.seoKeywords || []);
            setSelectedProduct(post.linkedProductIds?.[0] || '');
            setPlatform(post.platform || 'facebook');
            setTone(post.tone || 'friendly');
            setImage(post.featuredImageUrl || '');
            
            // Map API status to local status
            const statusLower = post.status?.toLowerCase();
            setStatus((statusLower === 'draft' || statusLower === 'published' || statusLower === 'scheduled' 
                ? statusLower 
                : 'draft') as any);
            
            setScheduledDate(post.scheduledPublishDate || '');
            
            // Convert social posts array to object
            const socialPostsObj: Record<string, string> = {};
            if (post.socialMediaPosts) {
                post.socialMediaPosts.forEach((sp: any) => {
                    socialPostsObj[sp.platform] = sp.content;
                });
            }
            setSocialPosts(socialPostsObj);
            
            setMetaTitle(post.seoMetaTitle || '');
            setMetaDescription(post.seoMetaDescription || '');
            setMetaKeywords(post.seoKeywords || []);
        }
    }, [post, isNew]);

    const handleGenerateContent = async () => {
        if (!selectedProduct) {
            toast.error('Vui lòng chọn sản phẩm');
            return;
        }

        const product = products.find((p: any) => p.id === selectedProduct);
        if (!product) return;

        setIsGenerating(true);
        try {
            const result = await generateMarketingContent({
                name: product.name,
                description: product.shortDescription || product.description,
                price: product.price,
                category: product.categoryName,
                tone,
            });

            setTitle(result.title);
            setContent(result.content);
            setHashtags(result.hashtags);

            toast.success('Đã tạo nội dung marketing!');
        } catch (error: any) {
            toast.error(error.message || 'Lỗi khi tạo nội dung');
        } finally {
            setIsGenerating(false);
        }
    };

    const handleGenerateSocialPosts = async () => {
        if (!content) {
            toast.error('Vui lòng tạo nội dung trước');
            return;
        }

        setIsGenerating(true);
        try {
            const posts = await generateSocialMediaPosts(
                content,
                ['facebook', 'instagram', 'twitter', 'linkedin']
            );
            setSocialPosts(posts);
            toast.success('Đã tạo bài viết cho các nền tảng!');
        } catch (error: any) {
            toast.error(error.message || 'Lỗi khi tạo bài viết');
        } finally {
            setIsGenerating(false);
        }
    };

    const handleImproveContent = async () => {
        if (!content) {
            toast.error('Chưa có nội dung để cải thiện');
            return;
        }

        setIsGenerating(true);
        try {
            const improved = await improveMarketingContent(content, [
                'Làm cho hấp dẫn hơn',
                'Thêm kêu gọi hành động mạnh mẽ',
                'Tối ưu cho SEO',
            ]);

            setContent(improved);
            toast.success('Đã cải thiện nội dung!');
        } catch (error: any) {
            toast.error(error.message || 'Lỗi khi cải thiện nội dung');
        } finally {
            setIsGenerating(false);
        }
    };

    const handleGenerateSEO = async () => {
        if (!title || !content) {
            toast.error('Vui lòng nhập tiêu đề và nội dung trước');
            return;
        }

        setIsGenerating(true);
        try {
            const seo = await generateSEOContent({
                name: title,
                description: content,
                keywords: hashtags,
            });

            setMetaTitle(seo.metaTitle);
            setMetaDescription(seo.metaDescription);
            setMetaKeywords(seo.metaKeywords);

            toast.success('Đã tạo SEO metadata!');
        } catch (error: any) {
            toast.error(error.message || 'Lỗi khi tạo SEO');
        } finally {
            setIsGenerating(false);
        }
    };

    const handleCopyToClipboard = (text: string, label: string) => {
        navigator.clipboard.writeText(text);
        toast.success(`Đã copy ${label}!`);
    };

    const handleSave = () => {
        if (!title || !content) {
            toast.error('Vui lòng nhập tiêu đề và nội dung');
            return;
        }

        // Map status to API enum
        const statusMap: Record<string, any> = {
            'draft': 'Draft',
            'published': 'Published',
            'scheduled': 'Scheduled',
        };

        // Convert social posts to API format
        const socialMediaPosts = Object.keys(socialPosts).length > 0 
            ? Object.entries(socialPosts).map(([platform, content]) => ({
                platform,
                content,
                imageUrl: image || null,
            }))
            : undefined;

        const data = {
            title,
            content,
            shortDescription: content.substring(0, 200), // Use first 200 chars as excerpt
            imageUrl: image || null,
            imageData: null,
            productId: selectedProduct || null,
            platform: platform || null,
            tone: tone || null,
            hashtags: hashtags.length > 0 ? hashtags : null,
            priorityScore: 50, // Default priority
            displayLocation: ['Homepage'], // Default display location
            metaTitle: metaTitle || null,
            metaDescription: metaDescription || null,
            metaKeywords: metaKeywords.length > 0 ? metaKeywords : null,
            socialPosts: socialMediaPosts ? { posts: socialMediaPosts } : undefined,
            status: statusMap[status] || 'Draft',
            scheduledDate: scheduledDate || null,
        };

        if (isNew) {
            createPost.mutate(data as any, {
                onSuccess: () => {
                    toast.success('Tạo bài viết thành công');
                    navigate('/marketing');
                },
                onError: (error: any) => {
                    toast.error(error?.response?.data?.message || 'Tạo bài viết thất bại');
                },
            });
        } else {
            updatePost.mutate(
                { id: id!, data: data as any },
                {
                    onSuccess: () => {
                        toast.success('Cập nhật bài viết thành công');
                        navigate('/marketing');
                    },
                    onError: (error: any) => {
                        toast.error(error?.response?.data?.message || 'Cập nhật bài viết thất bại');
                    },
                }
            );
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">
                    {isNew ? 'Tạo Bài Viết Mới' : 'Chỉnh Sửa Bài Viết'}
                </h1>
                <Button
                    variant="primary"
                    icon={<Save className="w-5 h-5" />}
                    onClick={handleSave}
                    isLoading={createPost.isPending || updatePost.isPending}
                >
                    Lưu
                </Button>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className="lg:col-span-2 space-y-6">
                    {/* AI Generator */}
                    <Card>
                        <h2 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
                            <Sparkles className="w-5 h-5 text-purple-500" />
                            AI Content Generator
                        </h2>

                        <div className="space-y-4">
                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">
                                        Chọn Sản Phẩm
                                    </label>
                                    <select
                                        value={selectedProduct}
                                        onChange={(e) => setSelectedProduct(e.target.value)}
                                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    >
                                        <option value="">-- Chọn sản phẩm --</option>
                                        {products.map((product: any) => (
                                            <option key={product.id} value={product.id}>
                                                {product.name}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">
                                        Giọng Điệu
                                    </label>
                                    <select
                                        value={tone}
                                        onChange={(e) => setTone(e.target.value as any)}
                                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    >
                                        <option value="friendly">Thân thiện</option>
                                        <option value="professional">Chuyên nghiệp</option>
                                        <option value="casual">Gần gũi</option>
                                        <option value="enthusiastic">Nhiệt tình</option>
                                    </select>
                                </div>
                            </div>

                            <div className="flex gap-2">
                                <Button
                                    variant="primary"
                                    icon={<Wand2 className="w-4 h-4" />}
                                    onClick={handleGenerateContent}
                                    isLoading={isGenerating}
                                    disabled={!selectedProduct}
                                >
                                    Tạo Nội Dung
                                </Button>
                                <Button
                                    variant="ghost"
                                    icon={<RefreshCw className="w-4 h-4" />}
                                    onClick={handleImproveContent}
                                    isLoading={isGenerating}
                                    disabled={!content}
                                >
                                    Cải Thiện
                                </Button>
                                <Button
                                    variant="ghost"
                                    icon={<Sparkles className="w-4 h-4" />}
                                    onClick={handleGenerateSocialPosts}
                                    isLoading={isGenerating}
                                    disabled={!content}
                                >
                                    Social Posts
                                </Button>
                            </div>
                        </div>
                    </Card>

                    {/* Content Editor */}
                    <Card title="Nội Dung">
                        <div className="space-y-4">
                            <Input
                                label="Tiêu đề"
                                value={title}
                                onChange={(e) => setTitle(e.target.value)}
                                placeholder="Nhập tiêu đề bài viết"
                            />

                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Nội dung
                                </label>
                                <textarea
                                    value={content}
                                    onChange={(e) => setContent(e.target.value)}
                                    rows={12}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    placeholder="Nhập nội dung hoặc dùng AI để tạo"
                                />
                            </div>

                            <Input
                                label="Hashtags (phân cách bằng dấu phẩy)"
                                value={hashtags.join(', ')}
                                onChange={(e) => setHashtags(e.target.value.split(',').map(t => t.trim()).filter(Boolean))}
                                placeholder="#sale, #shopping, #deal"
                            />

                            {image && (
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">
                                        Ảnh
                                    </label>
                                    <img
                                        src={`data:image/png;base64,${image}`}
                                        alt="Generated"
                                        className="w-full rounded-lg"
                                    />
                                </div>
                            )}
                        </div>
                    </Card>

                    {/* SEO Section */}
                    <Card>
                        <div className="flex items-center justify-between mb-4">
                            <h2 className="text-lg font-semibold text-gray-900">
                                SEO Metadata
                            </h2>
                            <Button
                                variant="ghost"
                                size="sm"
                                icon={<Sparkles className="w-4 h-4" />}
                                onClick={handleGenerateSEO}
                                isLoading={isGenerating}
                            >
                                Tạo SEO
                            </Button>
                        </div>

                        <div className="space-y-4">
                            <Input
                                label="Meta Title"
                                value={metaTitle}
                                onChange={(e) => setMetaTitle(e.target.value)}
                                placeholder="Tiêu đề SEO..."
                            />

                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Meta Description
                                </label>
                                <textarea
                                    value={metaDescription}
                                    onChange={(e) => setMetaDescription(e.target.value)}
                                    placeholder="Mô tả SEO..."
                                    rows={3}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                />
                            </div>

                            <Input
                                label="Meta Keywords"
                                value={metaKeywords.join(', ')}
                                onChange={(e) => setMetaKeywords(e.target.value.split(',').map(k => k.trim()).filter(Boolean))}
                                placeholder="keyword1, keyword2, keyword3"
                            />
                        </div>
                    </Card>
                </div>

                <div className="space-y-6">
                    {/* Publishing Options */}
                    <Card title="Xuất Bản">
                        <div className="space-y-4">
                            <Select
                                label="Trạng thái"
                                options={[
                                    { value: 'draft', label: 'Nháp' },
                                    { value: 'published', label: 'Đã đăng' },
                                    { value: 'scheduled', label: 'Lên lịch' },
                                ]}
                                value={status}
                                onChange={(val) => setStatus(val as any)}
                            />

                            {status === 'scheduled' && (
                                <Input
                                    label="Ngày đăng"
                                    type="datetime-local"
                                    value={scheduledDate}
                                    onChange={(e) => setScheduledDate(e.target.value)}
                                />
                            )}
                        </div>
                    </Card>

                    {/* Social Media Posts */}
                    {Object.keys(socialPosts).length > 0 && (
                        <Card title="Social Media Posts">
                            <div className="space-y-3">
                                {Object.entries(socialPosts).map(([platform, content]) => (
                                    <div key={platform} className="p-3 bg-gray-50 rounded-lg">
                                        <div className="flex items-center justify-between mb-2">
                                            <span className="text-sm font-medium text-gray-700 capitalize">
                                                {platform}
                                            </span>
                                            <Button
                                                variant="ghost"
                                                size="sm"
                                                icon={<Copy className="w-3 h-3" />}
                                                onClick={() => handleCopyToClipboard(content, platform)}
                                            />
                                        </div>
                                        <p className="text-xs text-gray-600 line-clamp-4">
                                            {content}
                                        </p>
                                    </div>
                                ))}
                            </div>
                        </Card>
                    )}

                    {/* Tips */}
                    <Card>
                        <h3 className="text-sm font-semibold text-gray-900 mb-2">
                            💡 Mẹo Sử Dụng
                        </h3>
                        <ul className="text-xs text-gray-600 space-y-1">
                            <li>• Chọn sản phẩm để AI tạo nội dung tự động</li>
                            <li>• Sử dụng "Cải Thiện" để tối ưu nội dung</li>
                            <li>• Tạo SEO metadata để tăng khả năng tìm kiếm</li>
                            <li>• Tạo Social Posts cho nhiều nền tảng</li>
                        </ul>
                    </Card>
                </div>
            </div>
        </div>
    );
};

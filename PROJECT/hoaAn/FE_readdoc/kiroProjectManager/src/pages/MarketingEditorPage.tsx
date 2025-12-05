import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { toast } from 'sonner';
import { 
    Save, 
    ArrowLeft, 
    Sparkles, 
    RefreshCw, 
    Copy,
    Wand2
} from 'lucide-react';
import { 
    generateMarketingContent, 
    generateSocialMediaPosts,
    improveMarketingContent,
    generateSEOContent
} from '../lib/services/geminiService';
import { useProducts } from '../lib/hooks/useProducts';

export const MarketingEditorPage = () => {
    const navigate = useNavigate();
    const { id } = useParams();
    const isNew = id === 'new';

    const [isGenerating, setIsGenerating] = useState(false);
    const [selectedProduct, setSelectedProduct] = useState('');
    const [formData, setFormData] = useState({
        title: '',
        content: '',
        hashtags: [] as string[],
        status: 'draft' as 'draft' | 'published' | 'scheduled',
        scheduledDate: '',
        metaTitle: '',
        metaDescription: '',
        metaKeywords: [] as string[],
    });

    const [socialPosts, setSocialPosts] = useState<Record<string, string>>({});
    const [tone, setTone] = useState<'professional' | 'casual' | 'enthusiastic' | 'friendly'>('friendly');

    const { data: productsData } = useProducts({});
    const products = (productsData?.data?.data as any)?.data?.items || [];

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

            setFormData({
                ...formData,
                title: result.title,
                content: result.content,
                hashtags: result.hashtags,
            });

            toast.success('Đã tạo nội dung marketing!');
        } catch (error: any) {
            toast.error(error.message || 'Lỗi khi tạo nội dung');
        } finally {
            setIsGenerating(false);
        }
    };

    const handleGenerateSocialPosts = async () => {
        if (!formData.content) {
            toast.error('Vui lòng tạo nội dung trước');
            return;
        }

        setIsGenerating(true);
        try {
            const posts = await generateSocialMediaPosts(
                formData.content,
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
        if (!formData.content) {
            toast.error('Chưa có nội dung để cải thiện');
            return;
        }

        setIsGenerating(true);
        try {
            const improved = await improveMarketingContent(formData.content, [
                'Làm cho hấp dẫn hơn',
                'Thêm kêu gọi hành động mạnh mẽ',
                'Tối ưu cho SEO',
            ]);

            setFormData({ ...formData, content: improved });
            toast.success('Đã cải thiện nội dung!');
        } catch (error: any) {
            toast.error(error.message || 'Lỗi khi cải thiện nội dung');
        } finally {
            setIsGenerating(false);
        }
    };

    const handleGenerateSEO = async () => {
        if (!formData.title || !formData.content) {
            toast.error('Vui lòng nhập tiêu đề và nội dung trước');
            return;
        }

        setIsGenerating(true);
        try {
            const seo = await generateSEOContent({
                name: formData.title,
                description: formData.content,
                keywords: formData.hashtags,
            });

            setFormData({
                ...formData,
                metaTitle: seo.metaTitle,
                metaDescription: seo.metaDescription,
                metaKeywords: seo.metaKeywords,
            });

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
        if (!formData.title || !formData.content) {
            toast.error('Vui lòng nhập tiêu đề và nội dung');
            return;
        }

        // TODO: Implement save to backend
        toast.success('Đã lưu bài viết!');
        navigate('/marketing');
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div className="flex items-center gap-4">
                    <Button
                        variant="ghost"
                        icon={<ArrowLeft className="w-5 h-5" />}
                        onClick={() => navigate('/marketing')}
                    >
                        Quay lại
                    </Button>
                    <h1 className="text-2xl font-bold text-gray-900">
                        {isNew ? 'Tạo Bài Viết Mới' : 'Chỉnh Sửa Bài Viết'}
                    </h1>
                </div>
                <Button
                    variant="primary"
                    icon={<Save className="w-5 h-5" />}
                    onClick={handleSave}
                >
                    Lưu
                </Button>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Main Editor */}
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
                                    disabled={!formData.content}
                                >
                                    Cải Thiện
                                </Button>
                            </div>
                        </div>
                    </Card>

                    {/* Content Editor */}
                    <Card>
                        <h2 className="text-lg font-semibold text-gray-900 mb-4">
                            Nội Dung
                        </h2>

                        <div className="space-y-4">
                            <Input
                                label="Tiêu Đề"
                                value={formData.title}
                                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                                placeholder="Nhập tiêu đề bài viết..."
                            />

                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Nội Dung
                                </label>
                                <textarea
                                    value={formData.content}
                                    onChange={(e) => setFormData({ ...formData, content: e.target.value })}
                                    placeholder="Nhập nội dung bài viết..."
                                    rows={12}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                />
                            </div>

                            <Input
                                label="Hashtags (phân cách bằng dấu phẩy)"
                                value={formData.hashtags.join(', ')}
                                onChange={(e) => setFormData({ 
                                    ...formData, 
                                    hashtags: e.target.value.split(',').map(t => t.trim()).filter(Boolean)
                                })}
                                placeholder="#sale, #shopping, #deal"
                            />
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
                                value={formData.metaTitle}
                                onChange={(e) => setFormData({ ...formData, metaTitle: e.target.value })}
                                placeholder="Tiêu đề SEO..."
                            />

                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Meta Description
                                </label>
                                <textarea
                                    value={formData.metaDescription}
                                    onChange={(e) => setFormData({ ...formData, metaDescription: e.target.value })}
                                    placeholder="Mô tả SEO..."
                                    rows={3}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                />
                            </div>

                            <Input
                                label="Meta Keywords"
                                value={formData.metaKeywords.join(', ')}
                                onChange={(e) => setFormData({ 
                                    ...formData, 
                                    metaKeywords: e.target.value.split(',').map(k => k.trim()).filter(Boolean)
                                })}
                                placeholder="keyword1, keyword2, keyword3"
                            />
                        </div>
                    </Card>
                </div>

                {/* Sidebar */}
                <div className="space-y-6">
                    {/* Publishing Options */}
                    <Card>
                        <h2 className="text-lg font-semibold text-gray-900 mb-4">
                            Xuất Bản
                        </h2>

                        <div className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Trạng Thái
                                </label>
                                <select
                                    value={formData.status}
                                    onChange={(e) => setFormData({ ...formData, status: e.target.value as any })}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                >
                                    <option value="draft">Nháp</option>
                                    <option value="published">Đã đăng</option>
                                    <option value="scheduled">Lên lịch</option>
                                </select>
                            </div>

                            {formData.status === 'scheduled' && (
                                <Input
                                    label="Ngày Đăng"
                                    type="datetime-local"
                                    value={formData.scheduledDate}
                                    onChange={(e) => setFormData({ ...formData, scheduledDate: e.target.value })}
                                />
                            )}
                        </div>
                    </Card>

                    {/* Social Media Posts */}
                    <Card>
                        <div className="flex items-center justify-between mb-4">
                            <h2 className="text-lg font-semibold text-gray-900">
                                Social Media
                            </h2>
                            <Button
                                variant="ghost"
                                size="sm"
                                icon={<Sparkles className="w-4 h-4" />}
                                onClick={handleGenerateSocialPosts}
                                isLoading={isGenerating}
                                disabled={!formData.content}
                            >
                                Tạo
                            </Button>
                        </div>

                        {Object.keys(socialPosts).length > 0 && (
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
                                        <p className="text-xs text-gray-600 line-clamp-3">
                                            {content}
                                        </p>
                                    </div>
                                ))}
                            </div>
                        )}
                    </Card>
                </div>
            </div>
        </div>
    );
};

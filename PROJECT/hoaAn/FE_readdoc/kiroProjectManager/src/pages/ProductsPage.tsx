import { useState } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Modal } from '../components/ui/Modal';
import { Badge } from '../components/ui/Badge';
import { ProductImageModal } from '../components/product/ProductImageModal';
import { ProductFormModal } from '../components/product/ProductFormModal';
import { 
    useProducts, 
    useDeleteProduct, 
    useUpdateProduct, 
    useCreateProduct,
    useToggleProductActive,
    useToggleProductFeatured 
} from '../lib/hooks/useProducts';
import { toast } from 'sonner';
import { Plus, Edit, Trash2, Search, Image as ImageIcon, Filter, Star } from 'lucide-react';

export const ProductsPage = () => {
    const [search, setSearch] = useState('');
    const [deleteId, setDeleteId] = useState<string | null>(null);
    const [editImageProduct, setEditImageProduct] = useState<any | null>(null);
    const [editProduct, setEditProduct] = useState<any | null>(null);
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
    const [showFilters, setShowFilters] = useState(false);
    
    // Filters
    const [filters, setFilters] = useState({
        IsActive: undefined as boolean | undefined,
        IsFeatured: undefined as boolean | undefined,
        MinPrice: undefined as number | undefined,
        MaxPrice: undefined as number | undefined,
    });

    const { data: productsData } = useProducts({ 
        SearchTerm: search,
        ...filters
    });
    const deleteProduct = useDeleteProduct();
    const updateProduct = useUpdateProduct();
    const createProduct = useCreateProduct();
    const toggleActive = useToggleProductActive();
    const toggleFeatured = useToggleProductFeatured();

    // Extract products from paginated response
    // Response structure: productsData.data.data.data.items
    const products = (productsData?.data?.data as any)?.data?.items || [];

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND',
        }).format(value);
    };

    const handleDelete = () => {
        if (!deleteId) return;

        deleteProduct.mutate({ id: deleteId }, {
            onSuccess: () => {
                toast.success('Xóa sản phẩm thành công');
                setDeleteId(null);
            },
            onError: () => {
                toast.error('Xóa sản phẩm thất bại');
            },
        });
    };

    const handleUpdateImages = (images: string[]) => {
        if (!editImageProduct) return;

        updateProduct.mutate(
            {
                id: editImageProduct.id,
                data: { images },
            },
            {
                onSuccess: () => {
                    toast.success('Cập nhật ảnh thành công');
                    setEditImageProduct(null);
                },
                onError: () => {
                    toast.error('Cập nhật ảnh thất bại');
                },
            }
        );
    };

    const handleCreateProduct = (data: any) => {
        createProduct.mutate(
            { data },
            {
                onSuccess: () => {
                    toast.success('Thêm sản phẩm thành công');
                    setIsCreateModalOpen(false);
                },
                onError: () => {
                    toast.error('Thêm sản phẩm thất bại');
                },
            }
        );
    };

    const handleUpdateProduct = (data: any) => {
        if (!editProduct) return;

        updateProduct.mutate(
            {
                id: editProduct.id,
                data,
            },
            {
                onSuccess: () => {
                    toast.success('Cập nhật sản phẩm thành công');
                    setEditProduct(null);
                },
                onError: () => {
                    toast.error('Cập nhật sản phẩm thất bại');
                },
            }
        );
    };

    const handleToggleActive = (id: string, currentStatus: boolean) => {
        toggleActive.mutate(
            { id, data: !currentStatus },
            {
                onSuccess: () => {
                    toast.success(`${!currentStatus ? 'Kích hoạt' : 'Vô hiệu hóa'} sản phẩm thành công`);
                },
                onError: () => {
                    toast.error('Cập nhật trạng thái thất bại');
                },
            }
        );
    };

    const handleToggleFeatured = (id: string, currentStatus: boolean) => {
        toggleFeatured.mutate(
            { id, data: !currentStatus },
            {
                onSuccess: () => {
                    toast.success(`${!currentStatus ? 'Đánh dấu' : 'Bỏ đánh dấu'} nổi bật thành công`);
                },
                onError: () => {
                    toast.error('Cập nhật trạng thái nổi bật thất bại');
                },
            }
        );
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">
                    Quản Lý Sản Phẩm
                </h1>
                <Button 
                    variant="primary" 
                    icon={<Plus className="w-5 h-5" />}
                    onClick={() => setIsCreateModalOpen(true)}
                >
                    Thêm Sản Phẩm
                </Button>
            </div>

            <Card>
                <div className="flex gap-4 mb-4">
                    <Input
                        placeholder="Tìm kiếm sản phẩm..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
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
                    <div className="mb-4 p-4 bg-gray-50 rounded-lg space-y-3">
                        <div className="grid grid-cols-4 gap-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Trạng Thái
                                </label>
                                <select
                                    value={filters.IsActive === undefined ? '' : filters.IsActive.toString()}
                                    onChange={(e) => setFilters({
                                        ...filters,
                                        IsActive: e.target.value === '' ? undefined : e.target.value === 'true'
                                    })}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                >
                                    <option value="">Tất cả</option>
                                    <option value="true">Hoạt động</option>
                                    <option value="false">Ngừng</option>
                                </select>
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Nổi Bật
                                </label>
                                <select
                                    value={filters.IsFeatured === undefined ? '' : filters.IsFeatured.toString()}
                                    onChange={(e) => setFilters({
                                        ...filters,
                                        IsFeatured: e.target.value === '' ? undefined : e.target.value === 'true'
                                    })}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                >
                                    <option value="">Tất cả</option>
                                    <option value="true">Có</option>
                                    <option value="false">Không</option>
                                </select>
                            </div>
                            <Input
                                label="Giá Tối Thiểu"
                                type="number"
                                value={filters.MinPrice || ''}
                                onChange={(e) => setFilters({
                                    ...filters,
                                    MinPrice: e.target.value ? Number(e.target.value) : undefined
                                })}
                                placeholder="0"
                            />
                            <Input
                                label="Giá Tối Đa"
                                type="number"
                                value={filters.MaxPrice || ''}
                                onChange={(e) => setFilters({
                                    ...filters,
                                    MaxPrice: e.target.value ? Number(e.target.value) : undefined
                                })}
                                placeholder="0"
                            />
                        </div>
                    </div>
                )}

                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">
                                    Ảnh
                                </th>
                                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">
                                    Tên Sản Phẩm
                                </th>
                                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">
                                    Giá
                                </th>
                                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">
                                    Kho
                                </th>
                                <th className="px-4 py-3 text-left text-sm font-semibold text-gray-900">
                                    Trạng Thái
                                </th>
                                <th className="px-4 py-3 text-right text-sm font-semibold text-gray-900">
                                    Thao Tác
                                </th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {products.map((product: any) => (
                                <tr key={product.id} className="hover:bg-gray-50:bg-gray-700">
                                    <td className="px-4 py-3">
                                        <div className="w-12 h-12 rounded-lg overflow-hidden bg-gray-100 flex items-center justify-center">
                                            {product.primaryImage ? (
                                                <img
                                                    src={product.primaryImage}
                                                    alt={product.name}
                                                    className="w-full h-full object-cover"
                                                />
                                            ) : (
                                                <span className="text-gray-400 text-xs">
                                                    No img
                                                </span>
                                            )}
                                        </div>
                                    </td>
                                    <td className="px-4 py-3 text-gray-900">
                                        {product.name}
                                    </td>
                                    <td className="px-4 py-3 text-gray-900">
                                        {formatCurrency(product.price)}
                                    </td>
                                    <td className="px-4 py-3 text-gray-900">
                                        {product.stockQuantity || 0}
                                    </td>
                                    <td className="px-4 py-3">
                                        <div className="flex items-center gap-2">
                                            <div 
                                                className="cursor-pointer"
                                                onClick={() => handleToggleActive(product.id, product.isActive)}
                                                title="Click để thay đổi trạng thái"
                                            >
                                                <Badge variant={product.isActive ? 'success' : 'danger'}>
                                                    {product.isActive ? 'Hoạt động' : 'Ngừng'}
                                                </Badge>
                                            </div>
                                            {product.isFeatured && (
                                                <Star className="w-4 h-4 text-yellow-500 fill-yellow-500" />
                                            )}
                                        </div>
                                    </td>
                                    <td className="px-4 py-3 text-right">
                                        <div className="flex items-center justify-end gap-2">
                                            <Button
                                                variant="ghost"
                                                size="sm"
                                                icon={<Star className="w-4 h-4" />}
                                                onClick={() => handleToggleFeatured(product.id, product.isFeatured)}
                                                title={product.isFeatured ? 'Bỏ nổi bật' : 'Đánh dấu nổi bật'}
                                            />
                                            <Button
                                                variant="ghost"
                                                size="sm"
                                                icon={<ImageIcon className="w-4 h-4" />}
                                                onClick={() => setEditImageProduct(product)}
                                            >
                                                Ảnh
                                            </Button>
                                            <Button
                                                variant="ghost"
                                                size="sm"
                                                icon={<Edit className="w-4 h-4" />}
                                                onClick={() => setEditProduct(product)}
                                            >
                                                Sửa
                                            </Button>
                                            <Button
                                                variant="danger"
                                                size="sm"
                                                icon={<Trash2 className="w-4 h-4" />}
                                                onClick={() => setDeleteId(product.id)}
                                            >
                                                Xóa
                                            </Button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </Card>

            {/* Delete Confirmation Modal */}
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
                            isLoading={deleteProduct.isPending}
                        >
                            Xóa
                        </Button>
                    </>
                }
            >
                <p className="text-gray-700">
                    Bạn có chắc chắn muốn xóa sản phẩm này? Hành động này không thể hoàn tác.
                </p>
            </Modal>

            {/* Product Image Modal */}
            <ProductImageModal
                isOpen={!!editImageProduct}
                onClose={() => setEditImageProduct(null)}
                currentImages={editImageProduct?.images || []}
                onSave={handleUpdateImages}
                isLoading={updateProduct.isPending}
            />

            {/* Create Product Modal */}
            <ProductFormModal
                isOpen={isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
                onSave={handleCreateProduct}
                isLoading={createProduct.isPending}
                mode="create"
            />

            {/* Edit Product Modal */}
            <ProductFormModal
                isOpen={!!editProduct}
                onClose={() => setEditProduct(null)}
                onSave={handleUpdateProduct}
                isLoading={updateProduct.isPending}
                initialData={editProduct}
                mode="edit"
            />
        </div>
    );
};

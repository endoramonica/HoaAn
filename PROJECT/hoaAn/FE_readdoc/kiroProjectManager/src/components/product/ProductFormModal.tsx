import { useState, useEffect } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { toast } from 'sonner';

interface ProductFormData {
    name: string;
    code: string;
    price: number;
    compareAtPrice?: number;
    cost?: number;
    shortDescription?: string;
    description?: string;
    categoryId?: string;
    sku?: string;
    barcode?: string;
    stockQuantity: number;
    isActive: boolean;
    isFeatured: boolean;
    images?: string[];
}

interface ProductFormModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSave: (data: ProductFormData) => void;
    isLoading?: boolean;
    initialData?: Partial<ProductFormData>;
    mode: 'create' | 'edit';
}

export const ProductFormModal = ({
    isOpen,
    onClose,
    onSave,
    isLoading = false,
    initialData,
    mode,
}: ProductFormModalProps) => {
    const [formData, setFormData] = useState<ProductFormData>({
        name: '',
        code: '',
        price: 0,
        stockQuantity: 0,
        isActive: true,
        isFeatured: false,
    });

    useEffect(() => {
        if (initialData) {
            setFormData({
                name: initialData.name || '',
                code: initialData.code || '',
                price: initialData.price || 0,
                compareAtPrice: initialData.compareAtPrice,
                cost: initialData.cost,
                shortDescription: initialData.shortDescription,
                description: initialData.description,
                categoryId: initialData.categoryId,
                sku: initialData.sku,
                barcode: initialData.barcode,
                stockQuantity: initialData.stockQuantity || 0,
                isActive: initialData.isActive ?? true,
                isFeatured: initialData.isFeatured ?? false,
                images: initialData.images,
            });
        }
    }, [initialData]);

    const handleSubmit = () => {
        if (!formData.name.trim()) {
            toast.error('Vui lòng nhập tên sản phẩm');
            return;
        }
        if (mode === 'create' && !formData.code.trim()) {
            toast.error('Vui lòng nhập mã sản phẩm');
            return;
        }
        if (formData.price <= 0) {
            toast.error('Giá sản phẩm phải lớn hơn 0');
            return;
        }

        onSave(formData);
    };

    const handleClose = () => {
        setFormData({
            name: '',
            code: '',
            price: 0,
            stockQuantity: 0,
            isActive: true,
            isFeatured: false,
        });
        onClose();
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={handleClose}
            title={mode === 'create' ? 'Thêm Sản Phẩm' : 'Chỉnh Sửa Sản Phẩm'}
            footer={
                <>
                    <Button variant="ghost" onClick={handleClose} disabled={isLoading}>
                        Hủy
                    </Button>
                    <Button
                        variant="primary"
                        onClick={handleSubmit}
                        isLoading={isLoading}
                    >
                        {mode === 'create' ? 'Thêm' : 'Cập Nhật'}
                    </Button>
                </>
            }
        >
            <div className="space-y-4 max-h-[60vh] overflow-y-auto">
                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Tên Sản Phẩm *"
                        value={formData.name}
                        onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                        placeholder="Nhập tên sản phẩm"
                    />
                    <Input
                        label="Mã Sản Phẩm *"
                        value={formData.code}
                        onChange={(e) => setFormData({ ...formData, code: e.target.value })}
                        placeholder="Nhập mã sản phẩm"
                        disabled={mode === 'edit'}
                    />
                </div>

                <div className="grid grid-cols-3 gap-4">
                    <Input
                        label="Giá Bán *"
                        type="number"
                        value={formData.price}
                        onChange={(e) => setFormData({ ...formData, price: Number(e.target.value) })}
                        placeholder="0"
                    />
                    <Input
                        label="Giá So Sánh"
                        type="number"
                        value={formData.compareAtPrice || ''}
                        onChange={(e) => setFormData({ ...formData, compareAtPrice: e.target.value ? Number(e.target.value) : undefined })}
                        placeholder="0"
                    />
                    <Input
                        label="Giá Vốn"
                        type="number"
                        value={formData.cost || ''}
                        onChange={(e) => setFormData({ ...formData, cost: e.target.value ? Number(e.target.value) : undefined })}
                        placeholder="0"
                    />
                </div>

                <div className="grid grid-cols-3 gap-4">
                    <Input
                        label="SKU"
                        value={formData.sku || ''}
                        onChange={(e) => setFormData({ ...formData, sku: e.target.value })}
                        placeholder="Nhập SKU"
                    />
                    <Input
                        label="Barcode"
                        value={formData.barcode || ''}
                        onChange={(e) => setFormData({ ...formData, barcode: e.target.value })}
                        placeholder="Nhập barcode"
                    />
                    <Input
                        label="Số Lượng Kho"
                        type="number"
                        value={formData.stockQuantity}
                        onChange={(e) => setFormData({ ...formData, stockQuantity: Number(e.target.value) })}
                        placeholder="0"
                    />
                </div>

                <Input
                    label="Mô Tả Ngắn"
                    value={formData.shortDescription || ''}
                    onChange={(e) => setFormData({ ...formData, shortDescription: e.target.value })}
                    placeholder="Nhập mô tả ngắn"
                />

                <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                        Mô Tả Chi Tiết
                    </label>
                    <textarea
                        value={formData.description || ''}
                        onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                        placeholder="Nhập mô tả chi tiết"
                        rows={4}
                        className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                </div>

                <div className="flex gap-4">
                    <label className="flex items-center gap-2">
                        <input
                            type="checkbox"
                            checked={formData.isActive}
                            onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                            className="w-4 h-4 text-blue-600 rounded focus:ring-blue-500"
                        />
                        <span className="text-sm text-gray-700">Hoạt động</span>
                    </label>
                    <label className="flex items-center gap-2">
                        <input
                            type="checkbox"
                            checked={formData.isFeatured}
                            onChange={(e) => setFormData({ ...formData, isFeatured: e.target.checked })}
                            className="w-4 h-4 text-blue-600 rounded focus:ring-blue-500"
                        />
                        <span className="text-sm text-gray-700">Nổi bật</span>
                    </label>
                </div>
            </div>
        </Modal>
    );
};

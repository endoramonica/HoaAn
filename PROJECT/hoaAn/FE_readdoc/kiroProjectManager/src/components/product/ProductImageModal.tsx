import { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { Plus, X, Image as ImageIcon } from 'lucide-react';

interface ProductImageModalProps {
    isOpen: boolean;
    onClose: () => void;
    currentImages: string[];
    onSave: (images: string[]) => void;
    isLoading?: boolean;
}

export const ProductImageModal = ({
    isOpen,
    onClose,
    currentImages,
    onSave,
    isLoading = false,
}: ProductImageModalProps) => {
    const [images, setImages] = useState<string[]>(currentImages);
    const [newImageUrl, setNewImageUrl] = useState('');

    const handleAddImage = () => {
        if (newImageUrl.trim()) {
            setImages([...images, newImageUrl.trim()]);
            setNewImageUrl('');
        }
    };

    const handleRemoveImage = (index: number) => {
        setImages(images.filter((_, i) => i !== index));
    };

    const handleSave = () => {
        onSave(images);
    };

    const handleClose = () => {
        setImages(currentImages);
        setNewImageUrl('');
        onClose();
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={handleClose}
            title="Cập Nhật Ảnh Sản Phẩm"
            footer={
                <>
                    <Button variant="ghost" onClick={handleClose} disabled={isLoading}>
                        Hủy
                    </Button>
                    <Button
                        variant="primary"
                        onClick={handleSave}
                        isLoading={isLoading}
                    >
                        Lưu
                    </Button>
                </>
            }
        >
            <div className="space-y-4">
                {/* Add new image URL */}
                <div className="flex gap-2">
                    <Input
                        placeholder="Nhập URL ảnh..."
                        value={newImageUrl}
                        onChange={(e) => setNewImageUrl(e.target.value)}
                        onKeyPress={(e) => e.key === 'Enter' && handleAddImage()}
                        icon={<ImageIcon className="w-5 h-5" />}
                    />
                    <Button
                        variant="primary"
                        icon={<Plus className="w-5 h-5" />}
                        onClick={handleAddImage}
                        disabled={!newImageUrl.trim()}
                    >
                        Thêm
                    </Button>
                </div>

                {/* Image list */}
                <div className="space-y-2">
                    <p className="text-sm font-medium text-gray-700">
                        Danh sách ảnh ({images.length})
                        {images.length > 0 && (
                            <span className="text-xs text-gray-500 ml-2">
                                (Ảnh đầu tiên sẽ là ảnh chính)
                            </span>
                        )}
                    </p>

                    {images.length === 0 ? (
                        <div className="text-center py-8 text-gray-500">
                            Chưa có ảnh nào
                        </div>
                    ) : (
                        <div className="space-y-2 max-h-96 overflow-y-auto">
                            {images.map((url, index) => (
                                <div
                                    key={index}
                                    className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg"
                                >
                                    {/* Image preview */}
                                    <div className="w-16 h-16 rounded-lg overflow-hidden bg-gray-200 flex-shrink-0">
                                        <img
                                            src={url}
                                            alt={`Product ${index + 1}`}
                                            className="w-full h-full object-cover"
                                            onError={(e) => {
                                                e.currentTarget.src = '';
                                                e.currentTarget.className = 'hidden';
                                                e.currentTarget.parentElement!.innerHTML = `
                                                    <div class="w-full h-full flex items-center justify-center text-gray-400 text-xs">
                                                        Error
                                                    </div>
                                                `;
                                            }}
                                        />
                                    </div>

                                    {/* URL text */}
                                    <div className="flex-1 min-w-0">
                                        <p className="text-xs text-gray-600 truncate">
                                            {url}
                                        </p>
                                        {index === 0 && (
                                            <span className="inline-block mt-1 px-2 py-0.5 text-xs bg-blue-100 text-blue-700 rounded">
                                                Ảnh chính
                                            </span>
                                        )}
                                    </div>

                                    {/* Remove button */}
                                    <Button
                                        variant="ghost"
                                        size="sm"
                                        icon={<X className="w-4 h-4" />}
                                        onClick={() => handleRemoveImage(index)}
                                    />
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </Modal>
    );
};

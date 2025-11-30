// ImageUpload.tsx – phiên bản nâng cấp
import { useRef } from "react";
import { Button } from "@/components/ui/button";
import { Camera, X } from "lucide-react";

interface ImageUploadProps {
  images: File[]; // Đổi thành mảng
  previewUrls: string[]; // Mảng URL preview
  onImagesChange: (files: File[]) => void;
  maxImages?: number; // Giới hạn số ảnh (mặc định 4)
  disabled?: boolean;
}

export const ImageUpload = ({
  images,
  previewUrls,
  onImagesChange,
  maxImages = 4,
  disabled,
}: ImageUploadProps) => {
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files || []);
    const validFiles = files.filter((f) => f.type.startsWith("image/"));

    if (validFiles.length === 0) {
      alert("Vui lòng chọn file hình ảnh hợp lệ");
      return;
    }

    const newImages = [...images, ...validFiles].slice(0, maxImages);
    onImagesChange(newImages);

    // Reset input
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const removeImage = (index: number) => {
    onImagesChange(images.filter((_, i) => i !== index));
  };

  const canAddMore = images.length < maxImages;

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3">
        <Button
          type="button"
          variant="outline"
          size="sm"
          onClick={() => fileInputRef.current?.click()}
          disabled={disabled || !canAddMore}
        >
          <Camera className="w-4 h-4 mr-1" />
          Thêm ảnh {images.length > 0 && `(${images.length}/${maxImages})`}
        </Button>
        {images.length > 0 && (
          <span className="text-sm text-gray-500">
            Đã chọn {images.length} ảnh
          </span>
        )}
      </div>

      <input
        ref={fileInputRef}
        type="file"
        accept="image/*"
        multiple
        onChange={handleFileChange}
        className="hidden"
      />

      {previewUrls.length > 0 && (
        <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
          {previewUrls.map((url, i) => (
            <div key={i} className="relative group">
              <img
                src={url}
                alt={`Preview ${i + 1}`}
                className="w-full h-40 object-cover rounded-lg"
              />
              <button
                onClick={() => removeImage(i)}
                className="absolute top-2 right-2 w-8 h-8 bg-red-500 text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center hover:bg-red-600"
                type="button"
              >
                <X className="w-5 h-5" />
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

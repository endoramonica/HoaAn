/**
 * CustomizationEditor Component
 * Cho phép chỉnh sửa số lượng các tùy chọn customization cho một cart item
 * Hiển thị trong modal khi người dùng click "Chỉnh sửa tùy chọn"
 */

import { useState, useEffect } from 'react';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogHeader,
  AlertDialogTitle,
} from './ui/alert-dialog';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Loader2, AlertCircle } from 'lucide-react';
import { toast } from 'sonner';

interface CartItemCustomization {
  optionId?: string;
  quantity?: number;
  unitPrice?: number;
  totalPrice?: number;
}

interface CustomizationEditorProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  customizations: CartItemCustomization[];
  productName?: string;
  onSave: (updatedCustomizations: CartItemCustomization[]) => Promise<void>;
  minQuantities?: Record<string, number>;
  maxQuantities?: Record<string, number>;
}

const formatPrice = (price: number) => {
  return new Intl.NumberFormat('vi-VN').format(price) + '₫';
};

export function CustomizationEditor({
  open,
  onOpenChange,
  customizations,
  productName = 'Sản phẩm',
  onSave,
  minQuantities = {},
  maxQuantities = {},
}: CustomizationEditorProps) {
  const [editedCustomizations, setEditedCustomizations] = useState<CartItemCustomization[]>([]);
  const [isSaving, setIsSaving] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (open) {
      setEditedCustomizations(JSON.parse(JSON.stringify(customizations)));
      setErrors({});
    }
  }, [open, customizations]);

  const handleQuantityChange = (optionId: string | undefined, newQuantity: number) => {
    if (!optionId) return;

    const min = minQuantities[optionId] || 0;
    const max = maxQuantities[optionId];

    // Validate quantity
    const newErrors = { ...errors };
    if (newQuantity < min) {
      newErrors[optionId] = `Tối thiểu ${min}`;
    } else if (max && newQuantity > max) {
      newErrors[optionId] = `Tối đa ${max}`;
    } else {
      delete newErrors[optionId];
    }
    setErrors(newErrors);

    // Update customization
    setEditedCustomizations(prev =>
      prev.map(custom => {
        if (custom.optionId === optionId) {
          return {
            ...custom,
            quantity: newQuantity,
            totalPrice: newQuantity * (custom.unitPrice || 0),
          };
        }
        return custom;
      })
    );
  };

  const handleSave = async () => {
    // Validate all quantities
    const newErrors: Record<string, string> = {};
    let hasErrors = false;

    editedCustomizations.forEach(custom => {
      if (!custom.optionId) return;
      const min = minQuantities[custom.optionId] || 0;
      const max = maxQuantities[custom.optionId];

      if ((custom.quantity || 0) < min) {
        newErrors[custom.optionId] = `Tối thiểu ${min}`;
        hasErrors = true;
      } else if (max && (custom.quantity || 0) > max) {
        newErrors[custom.optionId] = `Tối đa ${max}`;
        hasErrors = true;
      }
    });

    if (hasErrors) {
      setErrors(newErrors);
      toast.error('Vui lòng kiểm tra các tùy chọn');
      return;
    }

    try {
      setIsSaving(true);
      await onSave(editedCustomizations);
      onOpenChange(false);
      toast.success('Đã cập nhật tùy chọn');
    } catch (error: any) {
      console.error('Error saving customizations:', error);
      toast.error(error.message || 'Không thể cập nhật tùy chọn');
    } finally {
      setIsSaving(false);
    }
  };

  const totalCustomizationPrice = editedCustomizations.reduce(
    (sum, c) => sum + (c.totalPrice || 0),
    0
  );

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent className="max-w-md">
        <AlertDialogHeader>
          <AlertDialogTitle>Chỉnh sửa tùy chọn</AlertDialogTitle>
          <AlertDialogDescription>
            {productName}
          </AlertDialogDescription>
        </AlertDialogHeader>

        <div className="space-y-4 py-4">
          {editedCustomizations.map(custom => {
            if ((custom.quantity || 0) <= 0) return null;
            const hasError = errors[custom.optionId || ''];

            return (
              <div key={custom.optionId} className="space-y-2">
                <div className="flex justify-between items-center">
                  <Label className="text-sm font-medium">
                    {custom.optionId}
                  </Label>
                  <span className="text-xs text-gray-600">
                    {formatPrice(custom.unitPrice || 0)}/đơn vị
                  </span>
                </div>

                <div className="flex gap-2 items-center">
                  <Input
                    type="number"
                    value={custom.quantity || 0}
                    onChange={(e) =>
                      handleQuantityChange(custom.optionId, parseInt(e.target.value) || 0)
                    }
                    className={`flex-1 ${hasError ? 'border-red-500' : ''}`}
                    min={minQuantities[custom.optionId || ''] || 0}
                    max={maxQuantities[custom.optionId || ''] || undefined}
                  />
                  <span className="text-sm font-medium text-gray-900 min-w-fit">
                    {formatPrice(custom.totalPrice || 0)}
                  </span>
                </div>

                {hasError && (
                  <div className="flex items-center gap-1 text-red-600 text-xs">
                    <AlertCircle className="w-3 h-3" />
                    {hasError}
                  </div>
                )}
              </div>
            );
          })}

          <div className="pt-4 border-t border-gray-200">
            <div className="flex justify-between font-medium">
              <span>Tổng tùy chọn:</span>
              <span className="text-rose-600">{formatPrice(totalCustomizationPrice)}</span>
            </div>
          </div>
        </div>

        <div className="flex gap-2">
          <AlertDialogCancel disabled={isSaving}>
            Hủy
          </AlertDialogCancel>
          <AlertDialogAction
            onClick={handleSave}
            disabled={isSaving}
            className="bg-amber-600 hover:bg-amber-700"
          >
            {isSaving ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                Đang lưu...
              </>
            ) : (
              'Lưu thay đổi'
            )}
          </AlertDialogAction>
        </div>
      </AlertDialogContent>
    </AlertDialog>
  );
}

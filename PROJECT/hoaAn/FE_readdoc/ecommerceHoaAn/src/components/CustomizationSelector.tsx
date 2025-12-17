/**
 * CustomizationSelector Component
 * Hiển thị customizable options và cho phép user chọn
 * Có nút "Lưu tạm thời" để lưu state
 */

import { useState, useEffect } from 'react';
import { Button } from './ui/button';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Separator } from './ui/separator';
import { Plus, Minus, Save, RotateCcw, AlertCircle } from 'lucide-react';
import { useCustomizationState, type CustomizationOption } from '../lib/hooks/useCustomizationState';
import { customizationService } from '../lib/services/customizationService';
import { toast } from 'sonner';

interface CustomizationSelectorProps {
  productId: string;
  productName: string;
  basePrice: number;
  options: CustomizationOption[];
  onCustomizationsChange?: (options: CustomizationOption[], totalPrice: number) => void;
  onSave?: (options: CustomizationOption[]) => void;
  compact?: boolean; // Compact mode for modal
}

export function CustomizationSelector({
  productId,
  productName,
  basePrice,
  options,
  onCustomizationsChange,
  onSave,
  compact = false
}: CustomizationSelectorProps) {
  const {
    customizationState,
    selectedOptions,
    totalPrice,
    initializeCustomization,
    updateOptionQuantity,
    getSelectedOptions,
    getTotalPrice,
    saveState,
    resetOptions
  } = useCustomizationState();

  const [validationErrors, setValidationErrors] = useState<string[]>([]);

  // Initialize on mount
  useEffect(() => {
    initializeCustomization(productId, productName, options);
  }, [productId, productName, options, initializeCustomization]);

  // Notify parent of changes
  useEffect(() => {
    if (customizationState && onCustomizationsChange) {
      onCustomizationsChange(customizationState.options, getTotalPrice());
    }
  }, [customizationState, getTotalPrice, onCustomizationsChange]);

  const handleQuantityChange = (optionId: string, newQuantity: number) => {
    const option = options.find(o => o.optionId === optionId);
    if (!option) return;

    // Validate bounds
    if (newQuantity < option.minQuantity) {
      toast.error(`Minimum quantity: ${option.minQuantity}`);
      return;
    }

    if (newQuantity > option.maxQuantity) {
      toast.error(`Maximum quantity: ${option.maxQuantity}`);
      return;
    }

    updateOptionQuantity(optionId, newQuantity);
    setValidationErrors([]);
  };

  const handleSaveState = () => {
    // Validate
    const validation = customizationService.validateCustomizations(
      customizationState?.options || []
    );

    if (!validation.valid) {
      setValidationErrors(validation.errors);
      toast.error('Vui lòng kiểm tra lại các lựa chọn');
      return;
    }

    // Save to localStorage
    saveState();

    // Call parent callback
    if (onSave && customizationState) {
      onSave(customizationState.options);
    }

    toast.success('Đã lưu lựa chọn của bạn');
  };

  const handleReset = () => {
    resetOptions();
    setValidationErrors([]);
    toast.info('Đã đặt lại các lựa chọn');
  };

  if (!customizationState) {
    return null;
  }

  const finalPrice = basePrice + totalPrice;

  if (compact) {
    // Compact mode for modal/dialog
    return (
      <div className="space-y-4">
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-3 flex items-start gap-2">
          <AlertCircle className="w-5 h-5 text-blue-600 mt-0.5 flex-shrink-0" />
          <p className="text-sm text-blue-800">
            Chọn số lượng cho mỗi tùy chọn. Giá sẽ được cập nhật tự động.
          </p>
        </div>

        <div className="space-y-3">
          {customizationState.options.map(option => (
            <div key={option.optionId} className="border rounded-lg p-3">
              <div className="flex items-center justify-between mb-2">
                <div>
                  <p className="font-medium text-sm">{option.name}</p>
                  <p className="text-xs text-gray-500">
                    {option.unitPrice.toLocaleString('vi-VN')}₫ / {option.unit}
                  </p>
                </div>
                <Badge variant="outline" className="text-xs">
                  {option.minQuantity}-{option.maxQuantity}
                </Badge>
              </div>

              <div className="flex items-center gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  className="h-8 w-8 p-0"
                  onClick={() =>
                    handleQuantityChange(
                      option.optionId,
                      option.selectedQuantity - 1
                    )
                  }
                  disabled={option.selectedQuantity <= option.minQuantity}
                >
                  <Minus className="w-3 h-3" />
                </Button>

                <Input
                  type="number"
                  value={option.selectedQuantity}
                  onChange={e =>
                    handleQuantityChange(option.optionId, parseInt(e.target.value) || 0)
                  }
                  className="w-16 h-8 text-center text-sm"
                  min={option.minQuantity}
                  max={option.maxQuantity}
                />

                <Button
                  variant="outline"
                  size="sm"
                  className="h-8 w-8 p-0"
                  onClick={() =>
                    handleQuantityChange(
                      option.optionId,
                      option.selectedQuantity + 1
                    )
                  }
                  disabled={option.selectedQuantity >= option.maxQuantity}
                >
                  <Plus className="w-3 h-3" />
                </Button>

                <span className="text-sm font-medium ml-auto">
                  {(
                    (option.selectedQuantity - option.baseQuantity) *
                    option.unitPrice
                  ).toLocaleString('vi-VN')}
                  ₫
                </span>
              </div>
            </div>
          ))}
        </div>

        {validationErrors.length > 0 && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-3">
            <p className="text-sm text-red-800 font-medium mb-2">Lỗi:</p>
            <ul className="text-sm text-red-700 space-y-1">
              {validationErrors.map((error, idx) => (
                <li key={idx}>• {error}</li>
              ))}
            </ul>
          </div>
        )}

        <Separator />

        <div className="space-y-2">
          <div className="flex justify-between text-sm">
            <span>Giá cơ bản:</span>
            <span>{basePrice.toLocaleString('vi-VN')}₫</span>
          </div>
          <div className="flex justify-between text-sm">
            <span>Tùy chọn thêm:</span>
            <span className="text-orange-600">{totalPrice.toLocaleString('vi-VN')}₫</span>
          </div>
          <Separator />
          <div className="flex justify-between font-bold">
            <span>Tổng cộng:</span>
            <span className="text-lg text-red-600">
              {finalPrice.toLocaleString('vi-VN')}₫
            </span>
          </div>
        </div>

        <div className="flex gap-2">
          <Button
            onClick={handleSaveState}
            className="flex-1 bg-blue-600 hover:bg-blue-700"
          >
            <Save className="w-4 h-4 mr-2" />
            Lưu tạm thời
          </Button>
          <Button
            variant="outline"
            onClick={handleReset}
            className="flex-1"
          >
            <RotateCcw className="w-4 h-4 mr-2" />
            Đặt lại
          </Button>
        </div>
      </div>
    );
  }

  // Full mode for product detail page
  return (
    <Card className="border-2 border-amber-200">
      <CardHeader className="bg-gradient-to-r from-amber-50 to-orange-50">
        <CardTitle className="text-amber-900">Tùy Chọn Dịch Vụ</CardTitle>
      </CardHeader>
      <CardContent className="pt-6">
        <div className="space-y-4">
          {customizationState.options.map(option => (
            <div key={option.optionId} className="border rounded-lg p-4">
              <div className="flex items-center justify-between mb-3">
                <div>
                  <h4 className="font-semibold text-amber-900">{option.name}</h4>
                  <p className="text-sm text-amber-700">
                    {option.unitPrice.toLocaleString('vi-VN')}₫ / {option.unit}
                  </p>
                </div>
                <Badge className="bg-amber-100 text-amber-800">
                  {option.minQuantity}-{option.maxQuantity}
                </Badge>
              </div>

              <div className="flex items-center gap-3">
                <Button
                  variant="outline"
                  size="icon"
                  className="h-10 w-10 border-amber-300"
                  onClick={() =>
                    handleQuantityChange(
                      option.optionId,
                      option.selectedQuantity - 1
                    )
                  }
                  disabled={option.selectedQuantity <= option.minQuantity}
                >
                  <Minus className="w-4 h-4" />
                </Button>

                <Input
                  type="number"
                  value={option.selectedQuantity}
                  onChange={e =>
                    handleQuantityChange(option.optionId, parseInt(e.target.value) || 0)
                  }
                  className="w-20 h-10 text-center border-amber-300"
                  min={option.minQuantity}
                  max={option.maxQuantity}
                />

                <Button
                  variant="outline"
                  size="icon"
                  className="h-10 w-10 border-amber-300"
                  onClick={() =>
                    handleQuantityChange(
                      option.optionId,
                      option.selectedQuantity + 1
                    )
                  }
                  disabled={option.selectedQuantity >= option.maxQuantity}
                >
                  <Plus className="w-4 h-4" />
                </Button>

                <span className="text-lg font-bold text-red-600 ml-auto">
                  {(
                    (option.selectedQuantity - option.baseQuantity) *
                    option.unitPrice
                  ).toLocaleString('vi-VN')}
                  ₫
                </span>
              </div>
            </div>
          ))}
        </div>

        {validationErrors.length > 0 && (
          <div className="mt-4 bg-red-50 border border-red-200 rounded-lg p-4">
            <p className="text-sm text-red-800 font-semibold mb-2">Lỗi:</p>
            <ul className="text-sm text-red-700 space-y-1">
              {validationErrors.map((error, idx) => (
                <li key={idx}>• {error}</li>
              ))}
            </ul>
          </div>
        )}

        <Separator className="my-4" />

        <div className="space-y-2 mb-4">
          <div className="flex justify-between text-amber-900">
            <span>Giá cơ bản:</span>
            <span>{basePrice.toLocaleString('vi-VN')}₫</span>
          </div>
          <div className="flex justify-between text-orange-600">
            <span>Tùy chọn thêm:</span>
            <span className="font-semibold">{totalPrice.toLocaleString('vi-VN')}₫</span>
          </div>
          <Separator />
          <div className="flex justify-between text-lg font-bold text-amber-900">
            <span>Tổng cộng:</span>
            <span className="text-red-600">
              {finalPrice.toLocaleString('vi-VN')}₫
            </span>
          </div>
        </div>

        <div className="flex gap-2">
          <Button
            onClick={handleSaveState}
            className="flex-1 bg-amber-600 hover:bg-amber-700 text-white"
          >
            <Save className="w-4 h-4 mr-2" />
            Lưu Tạm Thời
          </Button>
          <Button
            variant="outline"
            onClick={handleReset}
            className="flex-1 border-amber-300 text-amber-700 hover:bg-amber-50"
          >
            <RotateCcw className="w-4 h-4 mr-2" />
            Đặt Lại
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}

export default CustomizationSelector;

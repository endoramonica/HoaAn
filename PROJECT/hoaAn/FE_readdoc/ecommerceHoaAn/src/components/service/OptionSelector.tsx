import React from 'react';
import { Minus, Plus, Check } from 'lucide-react';
import type { CustomizableOption } from '../../lib/services/vietCommerceProductService';

interface OptionSelectorProps {
  option: CustomizableOption;
  quantity: number;
  onChange: (id: string, newQuantity: number) => void;
}

export const OptionSelector: React.FC<OptionSelectorProps> = ({
  option,
  quantity,
  onChange,
}) => {
  const isCheckboxStyle = option.minQuantity === 0 && option.maxQuantity === 1;

  const handleIncrement = () => {
    if (quantity < (option.maxQuantity || 0)) {
      onChange(option.id || '', quantity + 1);
    }
  };

  const handleDecrement = () => {
    if (quantity > (option.minQuantity || 0)) {
      onChange(option.id || '', quantity - 1);
    }
  };

  const toggleCheckbox = () => {
    onChange(option.id || '', quantity === 0 ? 1 : 0);
  };

  const isSelected = quantity > 0;

  return (
    <div
      className={`flex flex-col md:flex-row md:items-center md:justify-between p-4 rounded-xl border transition-all duration-200 ${
        isSelected
          ? 'border-amber-500 bg-amber-50/50'
          : 'border-amber-200 hover:border-amber-300 bg-white'
      }`}
    >
      {/* Option Info */}
      <div className="flex-1 mb-4 md:mb-0 md:pr-4">
        <h4 className="font-semibold text-amber-900">{option.name}</h4>
        <p className="text-sm text-amber-700 font-semibold mt-1">
          {new Intl.NumberFormat('vi-VN').format(option.unitPrice || 0)}₫
          <span className="text-gray-500 font-normal ml-1">/ {option.unit}</span>
        </p>
        <p className="text-xs text-gray-600 mt-2">
          Số lượng: {option.minQuantity} - {option.maxQuantity}
          {option.baseQuantity > 0 && ` (cơ bản: ${option.baseQuantity})`}
        </p>
      </div>

      {/* Selector Controls */}
      <div className="flex items-center gap-3">
        {isCheckboxStyle ? (
          <button
            onClick={toggleCheckbox}
            className={`flex h-6 w-6 items-center justify-center rounded-md border transition-colors ${
              isSelected
                ? 'border-amber-600 bg-amber-600 text-white'
                : 'border-amber-300 bg-white hover:border-amber-400'
            }`}
          >
            {isSelected && <Check className="h-4 w-4" />}
          </button>
        ) : (
          <div className="flex items-center gap-2 bg-amber-50 rounded-lg p-1 border border-amber-200">
            <button
              onClick={handleDecrement}
              disabled={quantity <= (option.minQuantity || 0)}
              className="p-1.5 rounded-md hover:bg-white text-amber-700 disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
            >
              <Minus className="h-4 w-4" />
            </button>
            <span className="w-6 text-center text-sm font-semibold text-amber-900 tabular-nums">
              {quantity}
            </span>
            <button
              onClick={handleIncrement}
              disabled={quantity >= (option.maxQuantity || 0)}
              className="p-1.5 rounded-md hover:bg-white text-amber-700 disabled:opacity-30 disabled:cursor-not-allowed transition-colors"
            >
              <Plus className="h-4 w-4" />
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

/**
 * CartItemCustomizations Component
 * Hiển thị customizations đã chọn cho một cart item
 * Lấy data từ backend response
 * Hỗ trợ cả Cart Page và Checkout Page
 */

interface CartItemCustomization {
  optionId?: string;
  quantity?: number;
  unitPrice?: number;
  totalPrice?: number;
}

interface CartItemCustomizationsProps {
  customizations?: CartItemCustomization[];
  productName?: string;
  compact?: boolean; // Cho checkout page
}

const formatPrice = (price: number) => {
  return new Intl.NumberFormat("vi-VN").format(price) + "₫";
};

export function CartItemCustomizations({
  customizations,
  productName,
  compact = false,
}: CartItemCustomizationsProps) {
  if (!customizations || customizations.length === 0) {
    return null;
  }

  const hasCustomizations = customizations.some(c => (c.quantity || 0) > 0);

  if (!hasCustomizations) {
    return null;
  }

  const customizationPrice = customizations.reduce((sum, c) => sum + (c.totalPrice || 0), 0);

  if (compact) {
    // Compact version for checkout
    return (
      <div className="mt-2 pt-2 border-t border-[#92400E]/10">
        <div className="space-y-1">
          {customizations.map(custom => {
            if ((custom.quantity || 0) <= 0) return null;
            return (
              <div key={custom.optionId} className="text-xs text-[#92400E]/70 flex justify-between">
                <span>
                  {custom.optionId} × {custom.quantity}
                </span>
                <span className="font-medium text-[#92400E]">
                  {formatPrice(custom.totalPrice || 0)}
                </span>
              </div>
            );
          })}
        </div>
        {customizationPrice > 0 && (
          <div className="mt-1 pt-1 border-t border-[#92400E]/10 flex justify-between font-medium text-xs text-[#DC2626]">
            <span>Tùy chọn:</span>
            <span>{formatPrice(customizationPrice)}</span>
          </div>
        )}
      </div>
    );
  }

  // Full version for cart page
  return (
    <div className="mt-3 pt-3 border-t border-gray-200">
      <p className="text-sm font-medium text-gray-700 mb-2">Tùy chọn thêm:</p>
      <div className="space-y-1">
        {customizations.map(custom => {
          if ((custom.quantity || 0) <= 0) return null;
          return (
            <div key={custom.optionId} className="text-sm text-gray-600 flex justify-between">
              <span>
                {custom.optionId} × {custom.quantity}
              </span>
              <span className="font-medium text-gray-900">
                {formatPrice(custom.totalPrice || 0)}
              </span>
            </div>
          );
        })}
      </div>
      {customizationPrice > 0 && (
        <div className="mt-2 pt-2 border-t border-gray-100 flex justify-between font-medium text-gray-900">
          <span>Tổng tùy chọn:</span>
          <span className="text-rose-600">{formatPrice(customizationPrice)}</span>
        </div>
      )}
    </div>
  );
}

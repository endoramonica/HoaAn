import React, { useState, useMemo } from 'react';
import {
  ShoppingCart,
  Star,
  TrendingUp,
  ChevronRight,
  Loader2,
  Tag,
} from 'lucide-react';
import { toast } from 'sonner';
import { useCart } from '@/lib/hooks/useCart';
import { useAuth } from '@/lib/hooks/useAuth';
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type { AddToCartDto } from '../../../Api/generated-orval/schemas';

const api = getVietCommerceAPI();

interface Product {
  id: string;
  name: string;
  price: number;
  currency: string;
  formattedPrice?: string;
  imageUrl?: string;
  thumbnailUrl?: string;
  hasDiscount?: boolean;
  discountPercentage?: number;
  rating?: number;
  reviewCount?: number;
  soldCount?: number;
}

interface ProductShowcaseProps {
  product: Product;
  onViewDetails?: () => void;
  className?: string;
}

export function ProductShowcase({
  product,
  onViewDetails,
  className = '',
}: ProductShowcaseProps) {
  const [imageError, setImageError] = useState(false);
  const [isAddingToCart, setIsAddingToCart] = useState(false);
  const { refreshCart } = useCart();
  const { isAuthenticated } = useAuth();

  const originalPrice = useMemo(() => {
    if (!product.discountPercentage || product.discountPercentage <= 0) return null;
    return Math.round(product.price / (1 - product.discountPercentage / 100));
  }, [product.price, product.discountPercentage]);

  const formatPrice = (price: number) =>
    new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND',
    }).format(price);

  const handleAddToCart = async (e: React.MouseEvent) => {
    e.stopPropagation();
    e.preventDefault();

    if (isAddingToCart) return;

    try {
      setIsAddingToCart(true);

      const dto: AddToCartDto = {
        productId: product.id,
        quantity: 1,
      };

      if (isAuthenticated) {
        await api.postApiV1CartAdd(dto);
      } else {
        await api.postApiV1CartGuestAdd(dto);
      }

      await refreshCart();

      toast.success('Đã thêm vào giỏ hàng!', {
        description: product.name,
      });
    } catch (error: any) {
      toast.error(error?.message || 'Không thể thêm vào giỏ hàng');
    } finally {
      setIsAddingToCart(false);
    }
  };

  return (
    <div
      onClick={onViewDetails}
      className={`group relative cursor-pointer overflow-hidden rounded-2xl
        border border-amber-100 bg-white
        shadow-sm transition-all duration-300
        hover:-translate-y-1 hover:shadow-xl ${className}`}
    >
      {/* Background gradient */}
      <div className="absolute inset-0 bg-gradient-to-br from-amber-50/50 via-white to-orange-50/30" />

      <div className="relative z-10 flex gap-3 sm:gap-4 p-3 sm:p-4">
        {/* IMAGE */}
        <div className="relative w-24 h-24 sm:w-28 sm:h-28 shrink-0">
          <div className="w-full h-full overflow-hidden rounded-xl border bg-gray-50">
            <img
              src={
                imageError
                  ? 'https://via.placeholder.com/150?text=No+Image'
                  : product.imageUrl || product.thumbnailUrl
              }
              alt={product.name}
              onError={() => setImageError(true)}
              className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110"
            />
          </div>

          {product.discountPercentage && (
            <div className="absolute -top-2 -left-2 z-10 flex h-10 w-10 items-center justify-center rounded-full bg-red-500 text-white text-xs font-bold shadow">
              -{product.discountPercentage}%
            </div>
          )}

          <div className="absolute -bottom-2 -right-2 z-10 rounded-full bg-amber-400 p-1.5 text-white shadow">
            <TrendingUp size={12} />
          </div>
        </div>

        {/* INFO */}
        <div className="flex min-w-0 flex-1 flex-col justify-between">
          <div>
            <div className="mb-1 flex items-center gap-1 text-[10px] font-bold uppercase tracking-wider text-amber-600">
              <Tag size={10} /> Sản phẩm nổi bật
            </div>

            <h4 className="mb-2 line-clamp-2 text-sm font-bold text-gray-900 group-hover:text-amber-700">
              {product.name}
            </h4>

            <div className="flex items-baseline gap-2">
              <span className="text-lg font-extrabold text-red-600">
                {product.formattedPrice || formatPrice(product.price)}
              </span>
              {originalPrice && (
                <span className="text-xs text-gray-400 line-through">
                  {formatPrice(originalPrice)}
                </span>
              )}
            </div>

            <div className="mt-1.5 flex items-center gap-2 text-xs text-gray-500">
              <span className="flex items-center gap-0.5 rounded bg-amber-50 px-1.5 py-0.5 font-medium text-amber-700">
                <Star size={10} className="fill-amber-500 text-amber-500" />
                {product.rating || 4.8}
              </span>
              <span>Đã bán {product.soldCount || 100}+</span>
            </div>
          </div>

          {/* ACTIONS */}
          <div className="mt-2 flex items-center gap-2 border-t border-dashed border-gray-200 pt-2 sm:mt-3 sm:pt-3">
            <span className="mr-auto flex items-center gap-1 text-xs font-semibold text-gray-500 group-hover:text-amber-600">
              Xem chi tiết <ChevronRight size={12} />
            </span>

            {/* ✅ FIXED BUTTON */}
            <button
              onClick={handleAddToCart}
              disabled={isAddingToCart}
              className="
                relative z-10
                flex items-center gap-2
                rounded-lg px-4 py-2
                text-xs font-semibold text-white
                bg-amber-600
                hover:bg-amber-700
                shadow-md hover:shadow-amber-300
                transition-all duration-200
                disabled:opacity-60 disabled:cursor-not-allowed
              "
            >
              {isAddingToCart ? (
                <Loader2 size={14} className="animate-spin" />
              ) : (
                <>
                  <ShoppingCart size={14} />
                  Mua ngay
                </>
              )}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

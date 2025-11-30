/**
 * ProductCard - Component hiển thị card sản phẩm với wishlist toggle
 * Tái sử dụng được cho HomePage, ProductsPage, và các trang khác
 */

import { useState } from 'react';
import { Card, CardContent } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { useWishlist } from '../lib/hooks/useWishlist';
import { useCart } from '../lib/hooks/useCart';
import { useAuth } from '../lib/hooks/useAuth';
import { getVietCommerceAPI } from '../../Api/generated-orval';
import type { AddToCartDto } from '../../Api/generated-orval/schemas';
import { Heart, ShoppingCart, Eye, Loader2 } from 'lucide-react';
import type { ProductListDto } from '../lib/api/types';
import { toast } from 'sonner';

const api = getVietCommerceAPI();

interface ProductCardProps {
  product: ProductListDto;
  onQuickView?: (product: ProductListDto) => void;
  viewMode?: 'grid' | 'list';
  showWishlistButton?: boolean;
}

export function ProductCard({ 
  product, 
  onQuickView, 
  viewMode = 'grid',
  showWishlistButton = true 
}: ProductCardProps) {
  const { toggleWishlist, isInWishlist } = useWishlist();
  const { refreshCart } = useCart();
  const { isAuthenticated } = useAuth();
  const [isTogglingWishlist, setIsTogglingWishlist] = useState(false);
  const [isAddingToCart, setIsAddingToCart] = useState(false);

  const isInWishlistState = isInWishlist(product.id);

  const handleToggleWishlist = async (e: React.MouseEvent) => {
    e.stopPropagation();
    setIsTogglingWishlist(true);
    try {
      await toggleWishlist(product.id);
    } catch (error) {
      console.error('Error toggling wishlist:', error);
    } finally {
      setIsTogglingWishlist(false);
    }
  };

  const handleAddToCart = async (e: React.MouseEvent) => {
    e.stopPropagation();
    
    if (!product?.id) {
      toast.error('Không tìm thấy sản phẩm');
      return;
    }

    try {
      setIsAddingToCart(true);
      
      const dto: AddToCartDto = { 
        productId: product.id, 
        quantity: 1
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
      console.error('Add to cart error:', error);
      
      if (error.message?.includes('Insufficient stock')) {
        toast.error('Sản phẩm không đủ số lượng trong kho');
      } else if (error.status === 401) {
        toast.error('Vui lòng đăng nhập để thêm vào giỏ hàng');
      } else {
        toast.error(error.message || 'Không thể thêm vào giỏ hàng');
      }
    } finally {
      setIsAddingToCart(false);
    }
  };

  const handleQuickView = (e: React.MouseEvent) => {
    e.stopPropagation();
    if (onQuickView) {
      onQuickView(product);
    }
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('vi-VN').format(price) + '₫';
  };

  const hasDiscount = product.compareAtPrice > product.price;
  const discountPercent = hasDiscount
    ? Math.round(((product.compareAtPrice - product.price) / product.compareAtPrice) * 100)
    : 0;

  return (
    <Card 
      className="group hover:shadow-xl transition-all duration-300 border-2 hover:border-amber-300 cursor-pointer"
      onClick={() => onQuickView?.(product)}
    >
      <div className={`${viewMode === 'list' ? 'flex' : 'block'}`}>
        {/* Product Image */}
        <div className={`relative overflow-hidden ${viewMode === 'list' ? 'w-48 flex-shrink-0' : ''}`}>
          <ImageWithFallback
            src={product.primaryImage || 'https://images.unsplash.com/photo-1602874801006-84c78b4e9dcc?w=400'}
            alt={product.name}
            className={`w-full object-cover group-hover:scale-105 transition-transform duration-300 ${
              viewMode === 'list' ? 'h-48' : 'h-64'
            }`}
          />
          
          {/* Badges */}
          <div className="absolute top-2 left-2 flex flex-col gap-2">
            {hasDiscount && (
              <Badge className="bg-red-600 text-white">
                -{discountPercent}%
              </Badge>
            )}
            {product.isFeatured && (
              <Badge className="bg-amber-600 text-white">
                Nổi bật
              </Badge>
            )}
            {!product.inStock && (
              <Badge variant="destructive">
                Hết hàng
              </Badge>
            )}
          </div>

          {/* Hover Actions */}
          <div className="absolute inset-0 bg-black/20 opacity-0 group-hover:opacity-100 transition-opacity duration-300 flex items-center justify-center gap-2">
            {onQuickView && (
              <Button 
                size="icon" 
                variant="secondary" 
                className="bg-white/90 hover:bg-white"
                onClick={handleQuickView}
                title="Xem nhanh"
              >
                <Eye className="w-4 h-4" />
              </Button>
            )}
            
            {showWishlistButton && (
              <Button 
                size="icon" 
                variant="secondary" 
                className={`bg-white/90 hover:bg-white ${
                  isInWishlistState ? 'text-pink-600' : ''
                }`}
                onClick={handleToggleWishlist}
                disabled={isTogglingWishlist}
                title={isInWishlistState ? 'Xóa khỏi yêu thích' : 'Thêm vào yêu thích'}
              >
                {isTogglingWishlist ? (
                  <div className="animate-spin h-4 w-4 border-2 border-current border-t-transparent rounded-full" />
                ) : (
                  <Heart className={`w-4 h-4 ${
                    isInWishlistState ? 'fill-current' : ''
                  }`} />
                )}
              </Button>
            )}
          </div>

          {/* Favorite Count */}
          {product.favoriteCount > 0 && (
            <div className="absolute bottom-2 right-2">
              <Badge className="bg-white/90 text-gray-700 text-xs">
                <Heart className="w-3 h-3 mr-1 fill-current text-pink-600" />
                {product.favoriteCount}
              </Badge>
            </div>
          )}
        </div>
        
        {/* Product Info */}
        <CardContent className={`p-4 ${viewMode === 'list' ? 'flex-1' : ''}`}>
          <h3 className="text-lg text-amber-900 mb-2 line-clamp-2">
            {product.name}
          </h3>
          
          {product.categoryName && (
            <Badge variant="outline" className="mb-2 text-amber-700 border-amber-300">
              {product.categoryName}
            </Badge>
          )}
          
          {/* Price */}
          <div className="flex items-center gap-2 mb-4">
            <span className="text-2xl text-red-600">
              {formatPrice(product.displayPrice.discountedPrice)}
            </span>
            {hasDiscount && (
              <span className="text-sm text-gray-400 line-through">
                {formatPrice(product.displayPrice.originalPrice)}
              </span>
            )}
          </div>

          {/* Stock */}
          {product.inStock ? (
            <div className="text-sm text-gray-600 mb-4">
              Còn lại: {product.stockQuantity} sản phẩm
            </div>
          ) : (
            <div className="text-sm text-red-600 mb-4">
              Tạm hết hàng
            </div>
          )}
          
          {/* Add to Cart Button */}
          <Button 
            className="w-full bg-red-600 hover:bg-red-700 text-white"
            disabled={!product.inStock || isAddingToCart}
            onClick={handleAddToCart}
          >
            {isAddingToCart ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                Đang thêm...
              </>
            ) : (
              <>
                <ShoppingCart className="w-4 h-4 mr-2" />
                {product.inStock ? 'Thêm vào giỏ hàng' : 'Hết hàng'}
              </>
            )}
          </Button>
        </CardContent>
      </div>
    </Card>
  );
}

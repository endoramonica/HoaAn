/**
 * WishlistPage - Trang hiển thị danh sách sản phẩm yêu thích
 * Màu sắc văn hóa: nâu ấm (#92400E), vàng (#F59E0B), đỏ nghi lễ (#DC2626), nền kem (#FFFBEB)
 */

import { useState } from 'react';
import { useWishlist } from '../lib/hooks/useWishlist';
import { Button } from './ui/button';
import { Card, CardContent } from './ui/card';
// import { Alert, AlertDescription } from './ui/alert'; // Reserved for future use
import { Badge } from './ui/badge';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from './ui/alert-dialog';
import { Heart, ShoppingCart, Trash2, X, Package, RefreshCcw } from 'lucide-react';
import { ImageWithFallback } from './figma/ImageWithFallback';

export default function WishlistPage() {
  const {
    wishlistItems,
    loading,
    totalItems,
    isEmpty,
    removeFromWishlist,
    clearWishlist,
    loadWishlist,
    // moveAllToCart sẽ được thêm vào hook sau
  } = useWishlist();

  const [showClearDialog, setShowClearDialog] = useState(false);
  const [removingItemId, setRemovingItemId] = useState<string | null>(null);

  const handleRemoveItem = async (itemId: string) => {
    setRemovingItemId(itemId);
    try {
      await removeFromWishlist(itemId);
    } finally {
      setRemovingItemId(null);
    }
  };

  const handleClearWishlist = async () => {
    await clearWishlist();
    setShowClearDialog(false);
  };

  // TODO: Thêm moveAllToCart vào hook nếu backend hỗ trợ
  const handleMoveAllToCart = async () => {
    // await moveAllToCart();
    // toast.success("Đã thêm tất cả vào giỏ hàng");
    alert("Chức năng đang phát triển");
  };

  // Format giá tiền VND
  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(price);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-[#FFFBEB] via-[#FEF3C7] to-[#FFFBEB]">
      {/* Header */}
      <div className="bg-gradient-to-r from-[#92400E] via-[#B45309] to-[#92400E] text-white py-12">
        <div className="container mx-auto px-4">
          <div className="flex flex-col md:flex-row items-center justify-between gap-4">
            <div>
              <div className="flex items-center gap-3 mb-2">
                <Heart className="h-8 w-8 fill-current" />
                <h1 className="text-2xl md:text-3xl font-bold">Danh Sách Yêu Thích</h1>
              </div>
              <p className="text-amber-100">
                {totalItems > 0
                  ? `Bạn có ${totalItems} sản phẩm trong danh sách yêu thích`
                  : 'Danh sách yêu thích của bạn đang trống'}
              </p>
            </div>

            {!isEmpty && (
              <div className="flex flex-wrap gap-3">
                <Button
                  onClick={loadWishlist}
                  disabled={loading}
                  variant="outline"
                  className="bg-white/10 border-white/20 text-white hover:bg-white/20"
                >
                  <RefreshCcw className={`h-4 w-4 mr-2 ${loading ? 'animate-spin' : ''}`} />
                  Làm mới
                </Button>
                <Button
                  onClick={handleMoveAllToCart}
                  disabled={loading}
                  className="bg-[#F59E0B] hover:bg-[#D97706] text-white"
                >
                  <ShoppingCart className="h-4 w-4 mr-2" />
                  Thêm tất cả vào giỏ
                </Button>
                <Button
                  onClick={() => setShowClearDialog(true)}
                  disabled={loading}
                  variant="destructive"
                  className="bg-[#DC2626] hover:bg-[#B91C1C]"
                >
                  <Trash2 className="h-4 w-4 mr-2" />
                  Xóa tất cả
                </Button>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="container mx-auto px-4 py-8">
        {/* Loading State */}
        {loading && isEmpty && (
          <div className="text-center py-12">
            <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-[#92400E]" />
            <p className="mt-4 text-[#92400E]">Đang tải danh sách yêu thích...</p>
          </div>
        )}

        {/* Empty State */}
        {!loading && isEmpty && (
          <Card className="border-2 border-[#92400E]/20 bg-white/80 backdrop-blur">
            <CardContent className="py-12 text-center">
              <div className="flex justify-center mb-4">
                <div className="p-4 bg-amber-100 rounded-full">
                  <Heart className="h-12 w-12 text-[#92400E]" />
                </div>
              </div>
              <h3 className="mb-2 text-[#92400E] text-xl font-semibold">Danh sách yêu thích trống</h3>
              <p className="text-amber-800 mb-6">
                Hãy thêm các sản phẩm yêu thích để dễ dàng mua sắm sau này
              </p>
              <Button
                onClick={() => window.location.href = '/products'}
                className="bg-[#92400E] hover:bg-[#7C2D12] text-white"
              >
                <Package className="h-4 w-4 mr-2" />
                Khám phá sản phẩm
              </Button>
            </CardContent>
          </Card>
        )}

        {/* Wishlist Items Grid */}
        {!isEmpty && (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {wishlistItems.map((item) => {
              // Safety checks
              if (!item?.product) {
                console.warn('[WishlistPage] Item missing product:', item);
                return null;
              }

              const product = item.product;
              const productId = product.id || item.productId || item.id;
              const productName = product.name || 'Sản phẩm không tên';
              const productPrice = product.price ?? 0;
              const productStock = product.stock ?? 0;
              const productRating = product.rating ?? 0;
              const productReviewCount = product.reviewCount ?? 0;
              
              // Safe image access - fallback chain
              const productImage = product.images?.[0]?.url 
                || product.thumbnailUrl 
                || '/placeholder-product.png';

              return (
                <Card
                  key={item.id}
                  className="group relative overflow-hidden border-2 border-[#92400E]/20 bg-white/90 backdrop-blur hover:border-[#F59E0B] transition-all duration-300 hover:shadow-xl"
                >
                  {/* Remove Button */}
                  <button
                    onClick={() => handleRemoveItem(item.id)}
                    disabled={removingItemId === item.id}
                    className="absolute top-2 right-2 z-10 p-2 bg-white/90 rounded-full shadow-lg hover:bg-[#DC2626] hover:text-white transition-colors"
                    title="Xóa khỏi danh sách yêu thích"
                  >
                    {removingItemId === item.id ? (
                      <div className="animate-spin h-4 w-4 border-2 border-current border-t-transparent rounded-full" />
                    ) : (
                      <X className="h-4 w-4" />
                    )}
                  </button>

                  <CardContent className="p-0">
                    {/* Product Image */}
                    <div className="relative aspect-square overflow-hidden bg-gradient-to-br from-amber-50 to-orange-50">
                      <ImageWithFallback
                        src={productImage}
                        alt={productName}
                        className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                      />

                      {/* Out of Stock Overlay */}
                      {productStock === 0 && (
                        <div className="absolute inset-0 bg-black/60 flex items-center justify-center">
                          <Badge variant="destructive" className="text-sm">
                            Hết hàng
                          </Badge>
                        </div>
                      )}
                    </div>

                    {/* Product Info */}
                    <div className="p-4">
                      {/* Category */}
                      {product.category && (
                        <div className="mb-2">
                          <Badge variant="outline" className="text-xs border-[#92400E] text-[#92400E]">
                            {product.category.name || 'Uncategorized'}
                          </Badge>
                        </div>
                      )}

                      {/* Product Name */}
                      <h3 className="mb-2 text-[#92400E] line-clamp-2 group-hover:text-[#F59E0B] transition-colors font-medium">
                        {productName}
                      </h3>

                      {/* Rating */}
                      {productReviewCount > 0 && (
                        <div className="flex items-center gap-2 mb-3">
                          <div className="flex items-center">
                            {Array.from({ length: 5 }).map((_, i) => (
                              <span
                                key={i}
                                className={`text-sm ${
                                  i < Math.floor(productRating)
                                    ? 'text-[#F59E0B]'
                                    : 'text-gray-300'
                                }`}
                              >
                                ★
                              </span>
                            ))}
                          </div>
                          <span className="text-sm text-gray-600">
                            ({productReviewCount})
                          </span>
                        </div>
                      )}

                      {/* Price */}
                      <div className="mb-4">
                        <span className="text-lg font-bold text-[#DC2626]">
                          {formatPrice(productPrice)}
                        </span>
                      </div>

                      {/* Stock Status */}
                      {productStock > 0 && productStock <= 10 && (
                        <p className="text-sm text-[#DC2626] mb-3">
                          Chỉ còn {productStock} sản phẩm
                        </p>
                      )}

                      {/* Actions */}
                      <div className="flex gap-2">
                        <Button
                          className="flex-1 bg-[#92400E] hover:bg-[#7C2D12] text-white text-sm"
                          disabled={productStock === 0}
                          onClick={() => {
                            console.log('Add to cart:', productId);
                            // TODO: Gọi addToCart từ useCart
                          }}
                        >
                          <ShoppingCart className="h-4 w-4 mr-2" />
                          Thêm vào giỏ
                        </Button>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              );
            })}
          </div>
        )}
      </div>

      {/* Clear Wishlist Confirmation Dialog */}
      <AlertDialog open={showClearDialog} onOpenChange={setShowClearDialog}>
        <AlertDialogContent className="border-2 border-[#92400E]/20">
          <AlertDialogHeader>
            <AlertDialogTitle className="text-[#92400E]">
              Xóa toàn bộ danh sách yêu thích?
            </AlertDialogTitle>
            <AlertDialogDescription>
              Bạn có chắc chắn muốn xóa tất cả {totalItems} sản phẩm khỏi danh sách yêu thích?
              Hành động này không thể hoàn tác.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel className="border-[#92400E]/20">
              Hủy
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={handleClearWishlist}
              className="bg-[#DC2626] hover:bg-[#B91C1C] text-white"
            >
              Xóa tất cả
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
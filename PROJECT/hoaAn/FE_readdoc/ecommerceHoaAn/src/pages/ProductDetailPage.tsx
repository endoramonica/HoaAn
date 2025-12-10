import { useState, useEffect, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { ImageWithFallback } from '../components/figma/ImageWithFallback';
import { Star, ShoppingCart, Heart, Minus, Plus, Loader2, ChevronLeft, ChevronRight, ArrowLeft } from 'lucide-react';
import { toast } from 'sonner';
import { productService } from '../lib/services/productService';
import { useCart } from '../lib/hooks/useCart';
import { useWishlist } from '../lib/hooks/useWishlist';
import { useAuth } from '../lib/hooks/useAuth';
import type { ProductDetailDto } from '../../Api/generated-orval/schemas/productDetailDto';
import type { AddToCartDto } from '../../Api/generated-orval/schemas';
import { getVietCommerceAPI } from '../../Api/generated-orval';

const api = getVietCommerceAPI();

export function ProductDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  
  const [product, setProduct] = useState<ProductDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [quantity, setQuantity] = useState(1);
  const [isAddingToCart, setIsAddingToCart] = useState(false);
  const [isTogglingWishlist, setIsTogglingWishlist] = useState(false);
  const [relatedProducts, setRelatedProducts] = useState<any[]>([]);
  const [loadingRelated, setLoadingRelated] = useState(false);

  const { isInWishlist, toggleWishlist, loadWishlist } = useWishlist();
  const { isAuthenticated } = useAuth();
  const { refreshCart } = useCart();

  // Load product detail
  useEffect(() => {
    const loadProduct = async () => {
      if (!id) {
        setError('Không tìm thấy sản phẩm');
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError(null);
        const data = await productService.getProductById(id);
        setProduct(data);
        setCurrentImageIndex(0);

        // Record view
        await productService.recordProductView(id);

        // Load related products
        if (data.categoryId) {
          loadRelatedProducts(data.categoryId);
        }
      } catch (err) {
        console.error('Failed to load product:', err);
        setError('Không thể tải thông tin sản phẩm');
        toast.error('Không thể tải thông tin sản phẩm');
      } finally {
        setLoading(false);
      }
    };

    loadProduct();
  }, [id]);

  // Load related products
  const loadRelatedProducts = async (categoryId: string) => {
    try {
      setLoadingRelated(true);
      const products = await productService.getProductsByCategory(categoryId);
      // Filter out current product
      setRelatedProducts(products.filter(p => p.id !== id).slice(0, 6));
    } catch (err) {
      console.error('Failed to load related products:', err);
    } finally {
      setLoadingRelated(false);
    }
  };

  // Check wishlist
  useEffect(() => {
    if (product?.id) {
      loadWishlist();
    }
  }, [product?.id, loadWishlist]);

  const handleAddToCart = async () => {
    if (!product?.id) {
      toast.error('Không tìm thấy sản phẩm');
      return;
    }

    try {
      setIsAddingToCart(true);

      const dto: AddToCartDto = {
        productId: product.id,
        quantity
      };

      if (isAuthenticated) {
        await api.postApiV1CartAdd(dto);
      } else {
        await api.postApiV1CartGuestAdd(dto);
      }

      await refreshCart();
      toast.success(`Đã thêm ${quantity} sản phẩm vào giỏ hàng!`);
      setQuantity(1);
    } catch (error: any) {
      console.error('Add to cart error:', error);
      toast.error(error.message || 'Không thể thêm vào giỏ hàng');
    } finally {
      setIsAddingToCart(false);
    }
  };

  const handleToggleWishlist = async () => {
    if (!product?.id) return;

    setIsTogglingWishlist(true);
    try {
      await toggleWishlist(product.id);
    } catch (error) {
      console.error('Error toggling wishlist:', error);
    } finally {
      setIsTogglingWishlist(false);
    }
  };

  const handlePrevImage = () => {
    const imageUrls = getImageUrls();
    if (imageUrls.length > 0) {
      setCurrentImageIndex(prev => (prev - 1 + imageUrls.length) % imageUrls.length);
    }
  };

  const handleNextImage = () => {
    const imageUrls = getImageUrls();
    if (imageUrls.length > 0) {
      setCurrentImageIndex(prev => (prev + 1) % imageUrls.length);
    }
  };

  // Extract image URLs from product images array
  const getImageUrls = (): string[] => {
    if (!product) return [];
    
    // If images is array of objects with url property
    if (Array.isArray(product.images) && product.images.length > 0) {
      if (typeof product.images[0] === 'object' && 'url' in product.images[0]) {
        return (product.images as any[]).map(img => img.url).filter(Boolean);
      }
      // If images is array of strings
      if (typeof product.images[0] === 'string') {
        return product.images as string[];
      }
    }
    
    // Fallback to primaryImage
    return product.primaryImage ? [product.primaryImage] : [];
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('vi-VN').format(price) + '₫';
  };

  const isProductInWishlist = product?.id ? isInWishlist(product.id) : false;

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-amber-600" />
      </div>
    );
  }

  if (error || !product) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center gap-4">
        <p className="text-red-600">{error || 'Không tìm thấy sản phẩm'}</p>
        <Button onClick={() => navigate(-1)} className="bg-amber-600 hover:bg-amber-700">
          <ArrowLeft className="w-4 h-4 mr-2" />
          Quay lại
        </Button>
      </div>
    );
  }

  const images = getImageUrls();
  const currentImage = images[currentImageIndex] || product.primaryImage || '';
  const discount = product.displayPrice?.discountPercentage || 0;

  return (
    <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50">
      {/* Header */}
      <div className="bg-white border-b border-amber-200 sticky top-0 z-10">
        <div className="max-w-6xl mx-auto px-4 py-4 flex items-center gap-4">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => navigate(-1)}
            className="text-amber-700 hover:bg-amber-50"
          >
            <ArrowLeft className="w-5 h-5" />
          </Button>
          <h1 className="text-2xl text-amber-900 font-semibold">{product.name}</h1>
        </div>
      </div>

      <div className="max-w-6xl mx-auto px-4 py-8">
        <div className="grid md:grid-cols-2 gap-8">
          {/* Image Gallery */}
          <div className="space-y-4">
            {/* Main Image */}
            <div className="relative bg-gray-100 rounded-lg overflow-hidden">
              <ImageWithFallback
                src={currentImage}
                alt={product.name}
                className="w-full h-96 md:h-[500px] object-cover transition-opacity duration-300"
                key={currentImageIndex}
              />
              {product.isFeatured && (
                <Badge className="absolute top-3 left-3 bg-red-600 text-white">
                  Nổi bật
                </Badge>
              )}
              {discount > 0 && (
                <Badge className="absolute top-3 right-3 bg-green-600 text-white">
                  -{Math.round(discount)}%
                </Badge>
              )}

              {/* Navigation */}
              {images.length > 1 && (
                <>
                  <Button
                    size="icon"
                    variant="outline"
                    className="absolute left-2 top-1/2 -translate-y-1/2 bg-white/80 hover:bg-white"
                    onClick={handlePrevImage}
                  >
                    <ChevronLeft className="w-4 h-4" />
                  </Button>
                  <Button
                    size="icon"
                    variant="outline"
                    className="absolute right-2 top-1/2 -translate-y-1/2 bg-white/80 hover:bg-white"
                    onClick={handleNextImage}
                  >
                    <ChevronRight className="w-4 h-4" />
                  </Button>
                </>
              )}
            </div>

            {/* Thumbnails */}
            {images.length > 1 && (
              <div className="flex gap-2 overflow-x-auto pb-2">
                {images.map((image: string, index: number) => (
                  <button
                    key={index}
                    onClick={() => setCurrentImageIndex(index)}
                    className={`flex-shrink-0 w-20 h-20 rounded border-2 overflow-hidden transition-all ${
                      currentImageIndex === index
                        ? 'border-amber-600'
                        : 'border-gray-200 hover:border-amber-300'
                    }`}
                  >
                    <ImageWithFallback
                      src={image}
                      alt={`${product.name} ${index + 1}`}
                      className="w-full h-full object-cover"
                    />
                  </button>
                ))}
              </div>
            )}
          </div>

          {/* Product Info */}
          <div className="space-y-6">
            {/* Rating */}
            <div className="flex items-center gap-2">
              <div className="flex items-center">
                {[...Array(5)].map((_, i) => (
                  <Star
                    key={i}
                    className={`w-5 h-5 ${
                      i < Math.floor(product.averageRating || 0)
                        ? 'fill-yellow-400 text-yellow-400'
                        : 'text-gray-300'
                    }`}
                  />
                ))}
              </div>
              <span className="text-sm text-gray-600">
                {(product.averageRating || 0).toFixed(1)} ({product.reviewCount || 0} đánh giá)
              </span>
            </div>

            {/* Price */}
            <div className="space-y-2">
              <div className="flex items-center gap-3">
                <span className="text-4xl text-red-600 font-bold">
                  {formatPrice(product.displayPrice?.discountedPrice || product.price || 0)}
                </span>
                {product.displayPrice?.originalPrice && product.displayPrice.originalPrice > (product.displayPrice?.discountedPrice || 0) && (
                  <span className="text-2xl text-gray-500 line-through">
                    {formatPrice(product.displayPrice.originalPrice)}
                  </span>
                )}
              </div>
              {product.displayPrice?.discountAmount && product.displayPrice.discountAmount > 0 && (
                <p className="text-sm text-green-600">
                  Tiết kiệm {formatPrice(product.displayPrice.discountAmount)}
                </p>
              )}
            </div>

            {/* Description */}
            <Card className="bg-amber-50 border-amber-200">
              <CardContent className="p-4">
                <h3 className="text-amber-900 font-semibold mb-2">Mô tả sản phẩm</h3>
                <p className="text-gray-700 leading-relaxed">
                  {product.description || product.shortDescription || 'Không có mô tả'}
                </p>
              </CardContent>
            </Card>

            {/* Stock Status */}
            <div className="flex items-center gap-2">
              <span className="text-gray-700">Tình trạng:</span>
              <span className={(product as any).inStock ? 'text-green-600 font-semibold' : 'text-red-600 font-semibold'}>
                {(product as any).inStock ? `Còn ${product.stockQuantity || 0} sản phẩm` : 'Hết hàng'}
              </span>
            </div>

            {/* Quantity */}
            <div className="space-y-3">
              <h3 className="text-amber-900 font-semibold">Số lượng</h3>
              <div className="flex items-center gap-3">
                <Button
                  variant="outline"
                  size="icon"
                  onClick={() => setQuantity(prev => Math.max(1, prev - 1))}
                  disabled={quantity <= 1 || isAddingToCart}
                  className="border-amber-300 text-amber-700 hover:bg-amber-50"
                >
                  <Minus className="w-4 h-4" />
                </Button>
                <span className="w-12 text-center text-lg font-medium">{quantity}</span>
                <Button
                  variant="outline"
                  size="icon"
                  onClick={() => setQuantity(prev => prev + 1)}
                  disabled={isAddingToCart || !(product as any).inStock}
                  className="border-amber-300 text-amber-700 hover:bg-amber-50"
                >
                  <Plus className="w-4 h-4" />
                </Button>
              </div>
            </div>

            {/* Action Buttons */}
            <div className="space-y-3 pt-4">
              <Button
                className="w-full bg-red-600 hover:bg-red-700 text-white text-lg py-6"
                onClick={handleAddToCart}
                disabled={isAddingToCart || !(product as any).inStock}
              >
                {isAddingToCart ? (
                  <>
                    <Loader2 className="w-5 h-5 mr-2 animate-spin" />
                    Đang thêm...
                  </>
                ) : (
                  <>
                    <ShoppingCart className="w-5 h-5 mr-2" />
                    Thêm vào giỏ hàng
                  </>
                )}
              </Button>

              <Button
                variant="outline"
                className={`w-full transition-all duration-200 ${
                  isProductInWishlist
                    ? 'border-red-500 text-red-600 hover:bg-red-50 bg-red-50/50'
                    : 'border-amber-300 text-amber-700 hover:bg-amber-50'
                }`}
                onClick={handleToggleWishlist}
                disabled={isTogglingWishlist}
              >
                {isTogglingWishlist ? (
                  <>
                    <Loader2 className="w-5 h-5 mr-2 animate-spin" />
                    Đang xử lý...
                  </>
                ) : (
                  <>
                    <Heart
                      className={`w-5 h-5 mr-2 transition-all ${
                        isProductInWishlist ? 'fill-current' : ''
                      }`}
                    />
                    {isProductInWishlist ? 'Đã thêm vào yêu thích' : 'Thêm vào yêu thích'}
                  </>
                )}
              </Button>
            </div>

            {/* Product Info */}
            <Card className="border-amber-200">
              <CardContent className="p-4 space-y-3 text-sm">
                {product.categoryName && (
                  <div className="flex justify-between">
                    <span className="text-gray-600">Danh mục:</span>
                    <span className="text-amber-900 font-semibold">{product.categoryName}</span>
                  </div>
                )}
                {(product as any).code && (
                  <div className="flex justify-between">
                    <span className="text-gray-600">Mã sản phẩm:</span>
                    <span className="text-amber-900 font-semibold">{(product as any).code}</span>
                  </div>
                )}
                {(product as any).slug && (
                  <div className="flex justify-between">
                    <span className="text-gray-600">Slug:</span>
                    <span className="text-amber-900 font-semibold text-xs">{(product as any).slug}</span>
                  </div>
                )}
                {product.brandName && (
                  <div className="flex justify-between">
                    <span className="text-gray-600">Thương hiệu:</span>
                    <span className="text-amber-900 font-semibold">{product.brandName}</span>
                  </div>
                )}
                <div className="flex justify-between">
                  <span className="text-gray-600">Lượt xem:</span>
                  <span className="text-amber-900 font-semibold">{product.viewCount || 0}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Yêu thích:</span>
                  <span className="text-amber-900 font-semibold">{product.favoriteCount || 0}</span>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>

        {/* Related Products */}
        {relatedProducts.length > 0 && (
          <div className="mt-16">
            <h2 className="text-3xl text-amber-900 font-bold mb-6">Sản phẩm liên quan</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {relatedProducts.map(relatedProduct => (
                <Card
                  key={relatedProduct.id}
                  className="border-amber-200 hover:shadow-lg transition-shadow cursor-pointer"
                  onClick={() => navigate(`/product/${relatedProduct.id}`)}
                >
                  <CardContent className="p-4">
                    <div className="relative mb-4 bg-gray-100 rounded overflow-hidden">
                      <ImageWithFallback
                        src={relatedProduct.primaryImage || relatedProduct.images?.[0]}
                        alt={relatedProduct.name}
                        className="w-full h-48 object-cover"
                      />
                    </div>
                    <h3 className="text-amber-900 font-semibold line-clamp-2 mb-2">
                      {relatedProduct.name}
                    </h3>
                    <div className="flex items-center justify-between">
                      <span className="text-red-600 font-bold">
                        {formatPrice(relatedProduct.price || 0)}
                      </span>
                      <div className="flex items-center gap-1">
                        <Star className="w-4 h-4 fill-yellow-400 text-yellow-400" />
                        <span className="text-sm text-gray-600">
                          {(relatedProduct.averageRating || 0).toFixed(1)}
                        </span>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

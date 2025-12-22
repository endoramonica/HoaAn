import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { Card, CardContent } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { ImageWithFallback } from '../components/figma/ImageWithFallback';
import { Star, ShoppingCart, Heart, Minus, Plus, Loader2, ChevronLeft, ChevronRight, ArrowLeft, Package, Shield, Truck, RotateCcw, Eye, Sparkles, Tag, Save, Check } from 'lucide-react';
import { toast } from 'sonner';
import { productService } from '../lib/services/productService';
import { useCart } from '../lib/hooks/useCart';
import { useWishlist } from '../lib/hooks/useWishlist';
import { customizationStateService } from '../lib/services/customizationStateService';
import { cartService } from '../lib/services/cartService';
import { getActionTrackingService } from '../lib/services/actionTrackingService';
import type { ProductDetailDto } from '../../Api/generated-orval/schemas';
import type { AddToCartDto } from '../../Api/generated-orval/schemas';

export function ProductDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const actionTracking = getActionTrackingService();
  
  const [product, setProduct] = useState<ProductDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [quantity, setQuantity] = useState(1);
  const [isAddingToCart, setIsAddingToCart] = useState(false);
  const [isTogglingWishlist, setIsTogglingWishlist] = useState(false);
  const [relatedProducts, setRelatedProducts] = useState<any[]>([]);
  const [selectedTab, setSelectedTab] = useState<'description' | 'details'>('description');
  const [customizationQuantities, setCustomizationQuantities] = useState<Record<string, number>>({});
  const [isSavingCustomization, setIsSavingCustomization] = useState(false);
  const [isCustomizationSaved, setIsCustomizationSaved] = useState(false);

  const { isInWishlist, toggleWishlist, loadWishlist } = useWishlist();
  const { refreshCart } = useCart();

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
        console.log('[ProductDetailPage] Product loaded:', data);
        console.log('[ProductDetailPage] Customizable options:', data.customizableOptions);
        
        // Track product view
        actionTracking.trackAction('ViewProduct', {}, id, data.categoryId);
        
        setProduct(data);
        setCurrentImageIndex(0);
        
        // Initialize customization quantities with baseQuantity or saved state
        if (data.customizableOptions && data.customizableOptions.length > 0) {
          console.log('[ProductDetailPage] Initializing customization quantities');
          
          // Kiểm tra xem có trạng thái đã lưu không
          const savedState = customizationStateService.getCustomizationState(id);
          let quantities: Record<string, number> = {};
          
          if (savedState) {
            console.log('[ProductDetailPage] Loading saved customization state');
            quantities = customizationStateService.stateToQuantitiesMap(savedState);
            setIsCustomizationSaved(true);
          } else {
            // Sử dụng baseQuantity mặc định
            data.customizableOptions.forEach(option => {
              quantities[option.id] = option.baseQuantity || 0;
            });
          }
          
          setCustomizationQuantities(quantities);
          console.log('[ProductDetailPage] Customization quantities:', quantities);
        } else {
          console.log('[ProductDetailPage] No customizable options found');
        }
        
        await productService.recordProductView(id);

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

  const loadRelatedProducts = async (categoryId: string) => {
    try {
      const products = await productService.getProductsByCategory(categoryId);
      setRelatedProducts(products.filter(p => p.id !== id).slice(0, 6));
    } catch (err) {
      console.error('Failed to load related products:', err);
    }
  };

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
      
      // Track add to cart action
      actionTracking.trackAction('AddToCart', { quantity }, product.id, product.categoryId);
      
      // Build customizations array from user selections
      const customizations = product.customizableOptions?.map(option => {
        const qty = customizationQuantities[option.id] || 0;
        return {
          optionId: option.id,
          quantity: qty,
          unitPrice: option.unitPrice || 0,
          totalPrice: qty * (option.unitPrice || 0)
        };
      }) || undefined;

      console.log('[ProductDetailPage] Add to cart:', {
        productId: product.id,
        quantity,
        customizationsCount: customizations?.length || 0,
        customizations: customizations
      });

      const dto: AddToCartDto = {
        productId: product.id,
        quantity,
        customizations
      };

      console.log('[ProductDetailPage] Request DTO:', JSON.stringify(dto, null, 2));

      // Use cart service which automatically routes to correct endpoint
      console.log('[ProductDetailPage] 📤 Calling addItem API...');
      await cartService.addItem({
        productId: product.id,
        quantity,
        customizations
      });
      console.log('[ProductDetailPage] ✅ Item added to cart successfully');

      console.log('[ProductDetailPage] 🔄 Refreshing cart state...');
      await refreshCart();
      console.log('[ProductDetailPage] ✅ Cart state refreshed');
      
      toast.success(`Đã thêm ${quantity} sản phẩm vào giỏ hàng!`);
      setQuantity(1);
    } catch (error: any) {
      console.error('Add to cart error:', error);
      toast.error(error.message || 'Không thể thêm vào giỏ hàng');
    } finally {
      setIsAddingToCart(false);
    }
  };

  const handleUpdateCustomization = (optionId: string, newQuantity: number) => {
    const option = product?.customizableOptions?.find(o => o.id === optionId);
    if (!option) return;

    // Validate quantity within min/max bounds
    const min = option.minQuantity || 0;
    const max = option.maxQuantity || Infinity;
    const validQuantity = Math.max(min, Math.min(max, newQuantity));

    setCustomizationQuantities(prev => ({
      ...prev,
      [optionId]: validQuantity
    }));
    
    // Reset saved state indicator when user modifies
    setIsCustomizationSaved(false);
  };

  const handleSaveCustomization = async () => {
    if (!product?.id || !product.customizableOptions) {
      toast.error('Không thể lưu tùy chọn');
      return;
    }

    try {
      setIsSavingCustomization(true);
      
      customizationStateService.saveCustomizationState(
        product.id || '',
        product.name || '',
        (product as any).type,
        product.customizableOptions || [],
        customizationQuantities
      );

      setIsCustomizationSaved(true);
      toast.success('Đã lưu tùy chọn thêm!');
      
      // Reset indicator after 2 seconds
      setTimeout(() => {
        setIsCustomizationSaved(false);
      }, 2000);
    } catch (error) {
      console.error('Error saving customization:', error);
      toast.error('Không thể lưu tùy chọn');
    } finally {
      setIsSavingCustomization(false);
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

  const getImageUrls = (): string[] => {
    if (!product?.images) return [];
    return product.images.map(img => img.url).filter(Boolean) as string[];
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('vi-VN').format(price) + '₫';
  };

  const isProductInWishlist = product?.id ? isInWishlist(product.id) : false;

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-rose-50 via-white to-amber-50 flex items-center justify-center">
        <div className="text-center space-y-4">
          <Loader2 className="w-12 h-12 animate-spin text-rose-600 mx-auto" />
          <p className="text-gray-600">Đang tải sản phẩm...</p>
        </div>
      </div>
    );
  }

  if (error || !product) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-rose-50 via-white to-amber-50 flex flex-col items-center justify-center gap-6 p-4">
        <div className="text-center space-y-4">
          <div className="w-20 h-20 bg-red-100 rounded-full flex items-center justify-center mx-auto">
            <Package className="w-10 h-10 text-red-600" />
          </div>
          <h2 className="text-2xl font-bold text-gray-900">Oops!</h2>
          <p className="text-gray-600 max-w-md">{error || 'Không tìm thấy sản phẩm'}</p>
        </div>
        <Button 
          onClick={() => navigate(-1)} 
          className="bg-gradient-to-r from-rose-600 to-pink-600 hover:from-rose-700 hover:to-pink-700 text-white shadow-lg"
        >
          <ArrowLeft className="w-4 h-4 mr-2" />
          Quay lại
        </Button>
      </div>
    );
  }

  const images = getImageUrls();
  const currentImage = images[currentImageIndex] || product.primaryImage || '';
  const discount = product.discountPercentage || 0;

  return (
    <div className="min-h-screen bg-gradient-to-br from-rose-50 via-white to-amber-50">
      {/* Modern Header with Blur Effect */}
      <div className="bg-white/80 backdrop-blur-lg border-b border-gray-200 sticky top-0 z-50 shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              <Button
                variant="ghost"
                size="icon"
                onClick={() => navigate(-1)}
                className="text-gray-700 hover:bg-gray-100 rounded-full"
              >
                <ArrowLeft className="w-5 h-5" />
              </Button>
              <div>
                <p className="text-sm text-gray-500">Chi tiết sản phẩm</p>
                <h1 className="text-lg font-semibold text-gray-900 line-clamp-1">{product.name}</h1>
              </div>
            </div>
            <Button
              variant="ghost"
              size="icon"
              onClick={handleToggleWishlist}
              disabled={isTogglingWishlist}
              className={`rounded-full transition-all ${
                isProductInWishlist ? 'text-rose-600 hover:bg-rose-50' : 'text-gray-400 hover:bg-gray-100'
              }`}
            >
              <Heart className={`w-6 h-6 ${isProductInWishlist ? 'fill-current' : ''}`} />
            </Button>
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="grid lg:grid-cols-2 gap-8 lg:gap-12">
          {/* Enhanced Image Gallery */}
          <div className="space-y-4">
            <div className="relative bg-white rounded-3xl overflow-hidden shadow-xl group">
              <div className="aspect-square relative">
                <ImageWithFallback
                  src={currentImage || ''}
                  alt={product.name || ''}
                  className="w-full h-full object-contain transition-transform duration-500 group-hover:scale-105"
                  key={currentImageIndex}
                />
                
                {/* Floating Badges */}
                <div className="absolute top-4 left-4 flex flex-col gap-2">
                  {product.isFeatured && (
                    <Badge className="bg-gradient-to-r from-purple-600 to-pink-600 text-white border-0 shadow-lg px-3 py-1">
                      <Sparkles className="w-3 h-3 mr-1" />
                      Nổi bật
                    </Badge>
                  )}
                  {discount > 0 && (
                    <Badge className="bg-gradient-to-r from-green-600 to-emerald-600 text-white border-0 shadow-lg px-3 py-1 text-base font-bold">
                      -{Math.round(discount)}%
                    </Badge>
                  )}
                </div>

                {/* Image Navigation */}
                {images.length > 1 && (
                  <>
                    <Button
                      size="icon"
                      variant="ghost"
                      className="absolute left-3 top-1/2 -translate-y-1/2 bg-white/90 hover:bg-white shadow-lg rounded-full opacity-0 group-hover:opacity-100 transition-opacity"
                      onClick={handlePrevImage}
                    >
                      <ChevronLeft className="w-5 h-5" />
                    </Button>
                    <Button
                      size="icon"
                      variant="ghost"
                      className="absolute right-3 top-1/2 -translate-y-1/2 bg-white/90 hover:bg-white shadow-lg rounded-full opacity-0 group-hover:opacity-100 transition-opacity"
                      onClick={handleNextImage}
                    >
                      <ChevronRight className="w-5 h-5" />
                    </Button>
                  </>
                )}

                {/* Image Counter */}
                {images.length > 1 && (
                  <div className="absolute bottom-4 left-1/2 -translate-x-1/2 bg-black/60 backdrop-blur-sm text-white px-3 py-1 rounded-full text-sm">
                    {currentImageIndex + 1} / {images.length}
                  </div>
                )}
              </div>
            </div>

            {/* Modern Thumbnails */}
            {images.length > 1 && (
              <div className="flex gap-3 overflow-x-auto pb-2 scrollbar-hide">
                {images.map((image: string, index: number) => (
                  <button
                    key={index}
                    onClick={() => setCurrentImageIndex(index)}
                    className={`flex-shrink-0 w-20 h-20 rounded-xl overflow-hidden transition-all ${
                      currentImageIndex === index
                        ? 'ring-4 ring-rose-600 ring-offset-2 scale-110'
                        : 'ring-2 ring-gray-200 hover:ring-gray-300 opacity-60 hover:opacity-100'
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

          {/* Enhanced Product Info */}
          <div className="space-y-6">
            {/* Category & Brand */}
            <div className="flex flex-wrap items-center gap-2">
              {product.categoryName && (
                <Badge variant="outline" className="border-rose-200 text-rose-700 bg-rose-50">
                  {product.categoryName}
                </Badge>
              )}
              {product.brandName && (
                <Badge variant="outline" className="border-amber-200 text-amber-700 bg-amber-50">
                  {product.brandName}
                </Badge>
              )}
            </div>

            {/* Product Name */}
            <div>
              <h1 className="text-3xl lg:text-4xl font-bold text-gray-900 leading-tight mb-3">
                {product.name}
              </h1>
              
              {/* Rating & Views */}
              <div className="flex flex-wrap items-center gap-4 text-sm">
                <div className="flex items-center gap-2">
                  <div className="flex">
                    {[...Array(5)].map((_, i) => (
                      <Star
                        key={i}
                        className={`w-4 h-4 ${
                          i < Math.floor(product.averageRating || 0)
                            ? 'fill-yellow-400 text-yellow-400'
                            : 'text-gray-300'
                        }`}
                      />
                    ))}
                  </div>
                  <span className="font-semibold text-gray-900">
                    {(product.averageRating || 0).toFixed(1)}
                  </span>
                  <span className="text-gray-500">
                    ({product.reviewCount || 0} đánh giá)
                  </span>
                </div>
                <div className="flex items-center gap-1 text-gray-500">
                  <Eye className="w-4 h-4" />
                  <span>{product.viewCount || 0} lượt xem</span>
                </div>
              </div>
            </div>

            {/* Modern Price Display */}
            <div className="bg-gradient-to-r from-rose-50 to-pink-50 rounded-2xl p-6 border-2 border-rose-100">
              <div className="flex items-end gap-3 mb-2">
                <span className="text-5xl font-bold bg-gradient-to-r from-rose-600 to-pink-600 bg-clip-text text-transparent">
                  {formatPrice(product.displayPrice?.discountedPrice || product.price || 0)}
                </span>
                {product.displayPrice?.originalPrice && product.displayPrice.originalPrice > (product.displayPrice?.discountedPrice || 0) && (
                  <span className="text-2xl text-gray-400 line-through mb-2">
                    {formatPrice(product.displayPrice.originalPrice)}
                  </span>
                )}
              </div>
              {product.displayPrice?.discountAmount && product.displayPrice.discountAmount > 0 && (
                <div className="flex items-center gap-2">
                  <Badge className="bg-green-600 text-white">
                    Tiết kiệm {formatPrice(product.displayPrice.discountAmount)}
                  </Badge>
                  <span className="text-sm text-gray-600">so với giá gốc</span>
                </div>
              )}
            </div>

            {/* Stock Status */}
            <div className="flex items-center justify-between p-4 bg-white rounded-xl border-2 border-gray-100">
              <span className="text-gray-700 font-medium">Tình trạng kho:</span>
              <div className="flex items-center gap-2">
                <div className={`w-2 h-2 rounded-full ${product.stockQuantity && product.stockQuantity > 0 ? 'bg-green-500' : 'bg-red-500'}`} />
                <span className={`font-semibold ${product.stockQuantity && product.stockQuantity > 0 ? 'text-green-600' : 'text-red-600'}`}>
                  {product.stockQuantity && product.stockQuantity > 0 ? `Còn ${product.stockQuantity} sản phẩm` : 'Hết hàng'}
                </span>
              </div>
            </div>

            {/* Quantity Selector */}
            <div className="bg-white rounded-xl border-2 border-gray-100 p-6">
              <h3 className="text-gray-900 font-semibold mb-4">Chọn số lượng</h3>
              <div className="flex items-center gap-4">
                <Button
                  variant="outline"
                  size="icon"
                  onClick={() => setQuantity(prev => Math.max(1, prev - 1))}
                  disabled={quantity <= 1 || isAddingToCart}
                  className="h-12 w-12 rounded-full border-2 border-gray-200 hover:border-rose-300 hover:bg-rose-50"
                >
                  <Minus className="w-5 h-5" />
                </Button>
                <div className="flex-1 text-center">
                  <span className="text-3xl font-bold text-gray-900">{quantity}</span>
                </div>
                <Button
                  variant="outline"
                  size="icon"
                  onClick={() => setQuantity(prev => prev + 1)}
                  disabled={isAddingToCart || !product.stockQuantity || product.stockQuantity <= 0}
                  className="h-12 w-12 rounded-full border-2 border-gray-200 hover:border-rose-300 hover:bg-rose-50"
                >
                  <Plus className="w-5 h-5" />
                </Button>
              </div>
            </div>

            {/* Customization Options */}
            {product.customizableOptions && product.customizableOptions.length > 0 && (
              <div className="bg-white rounded-xl border-2 border-gray-100 p-6">
                <div className="flex items-center justify-between mb-4">
                  <h3 className="text-gray-900 font-semibold">Tùy chọn thêm</h3>
                  {(product as any).type === 'service' && (
                    <Button
                      size="sm"
                      onClick={handleSaveCustomization}
                      disabled={isSavingCustomization}
                      className={`transition-all ${
                        isCustomizationSaved
                          ? 'bg-green-600 hover:bg-green-700 text-white'
                          : 'bg-blue-600 hover:bg-blue-700 text-white'
                      }`}
                    >
                      {isSavingCustomization ? (
                        <>
                          <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                          Đang lưu...
                        </>
                      ) : isCustomizationSaved ? (
                        <>
                          <Check className="w-4 h-4 mr-2" />
                          Đã lưu
                        </>
                      ) : (
                        <>
                          <Save className="w-4 h-4 mr-2" />
                          Lưu tùy chọn
                        </>
                      )}
                    </Button>
                  )}
                </div>
                <div className="space-y-4">
                  {product.customizableOptions.map(option => (
                    <div key={option.id} className="border border-gray-200 rounded-lg p-4">
                      <div className="flex justify-between items-start mb-3">
                        <div>
                          <p className="font-medium text-gray-900">{option.name}</p>
                          <p className="text-sm text-gray-600">
                            {formatPrice(option.unitPrice)} / {option.unit}
                          </p>
                        </div>
                        <div className="text-right">
                          <p className="text-sm text-gray-600">Cộng thêm</p>
                          <p className="font-semibold text-rose-600">
                            {formatPrice((customizationQuantities[option.id] || 0) * option.unitPrice)}
                          </p>
                        </div>
                      </div>
                      <div className="flex items-center gap-2">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleUpdateCustomization(option.id, (customizationQuantities[option.id] || 0) - 1)}
                          disabled={isAddingToCart || (customizationQuantities[option.id] || 0) <= (option.minQuantity || 0)}
                          className="h-8 w-8 p-0"
                        >
                          <Minus className="w-4 h-4" />
                        </Button>
                        <span className="flex-1 text-center font-semibold">
                          {customizationQuantities[option.id] || 0}
                        </span>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleUpdateCustomization(option.id, (customizationQuantities[option.id] || 0) + 1)}
                          disabled={isAddingToCart || (customizationQuantities[option.id] || 0) >= (option.maxQuantity || Infinity)}
                          className="h-8 w-8 p-0"
                        >
                          <Plus className="w-4 h-4" />
                        </Button>
                      </div>
                      <p className="text-xs text-gray-500 mt-2">
                        Tối thiểu: {option.minQuantity}, Tối đa: {option.maxQuantity || 'Không giới hạn'}
                      </p>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Action Buttons */}
            <div className="grid grid-cols-5 gap-3">
              <Button
                className="col-span-4 bg-gradient-to-r from-rose-600 to-pink-600 hover:from-rose-700 hover:to-pink-700 text-white text-lg py-7 rounded-xl shadow-lg hover:shadow-xl transition-all"
                onClick={handleAddToCart}
                disabled={isAddingToCart || !product.stockQuantity || product.stockQuantity <= 0}
              >
                {isAddingToCart ? (
                  <>
                    <Loader2 className="w-5 h-5 mr-2 animate-spin" />
                    Đang thêm...
                  </>
                ) : (
                  <>
                    <ShoppingCart className="w-5 h-5 mr-2" />
                    Thêm vào giỏ
                  </>
                )}
              </Button>
              <Button
                variant="outline"
                size="icon"
                className={`col-span-1 py-7 rounded-xl border-2 transition-all ${
                  isProductInWishlist
                    ? 'border-rose-500 bg-rose-50 text-rose-600 hover:bg-rose-100'
                    : 'border-gray-200 hover:border-rose-300 hover:bg-rose-50'
                }`}
                onClick={handleToggleWishlist}
                disabled={isTogglingWishlist}
              >
                {isTogglingWishlist ? (
                  <Loader2 className="w-6 h-6 animate-spin" />
                ) : (
                  <Heart className={`w-6 h-6 ${isProductInWishlist ? 'fill-current' : ''}`} />
                )}
              </Button>
            </div>

            {/* Benefits Section */}
            <div className="grid grid-cols-2 gap-3">
              <div className="flex items-center gap-3 p-4 bg-white rounded-xl border border-gray-100">
                <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0">
                  <Shield className="w-5 h-5 text-blue-600" />
                </div>
                <div>
                  <p className="font-semibold text-sm text-gray-900">Chính hãng</p>
                  <p className="text-xs text-gray-500">100% bảo đảm</p>
                </div>
              </div>
              <div className="flex items-center gap-3 p-4 bg-white rounded-xl border border-gray-100">
                <div className="w-10 h-10 bg-green-100 rounded-full flex items-center justify-center flex-shrink-0">
                  <Truck className="w-5 h-5 text-green-600" />
                </div>
                <div>
                  <p className="font-semibold text-sm text-gray-900">Giao nhanh</p>
                  <p className="text-xs text-gray-500">Miễn phí ship</p>
                </div>
              </div>
              <div className="flex items-center gap-3 p-4 bg-white rounded-xl border border-gray-100">
                <div className="w-10 h-10 bg-orange-100 rounded-full flex items-center justify-center flex-shrink-0">
                  <RotateCcw className="w-5 h-5 text-orange-600" />
                </div>
                <div>
                  <p className="font-semibold text-sm text-gray-900">Đổi trả</p>
                  <p className="text-xs text-gray-500">Trong 7 ngày</p>
                </div>
              </div>
              <div className="flex items-center gap-3 p-4 bg-white rounded-xl border border-gray-100">
                <div className="w-10 h-10 bg-purple-100 rounded-full flex items-center justify-center flex-shrink-0">
                  <Heart className="w-5 h-5 text-purple-600" />
                </div>
                <div>
                  <p className="font-semibold text-sm text-gray-900">Ưu đãi</p>
                  <p className="text-xs text-gray-500">{product.favoriteCount || 0} yêu thích</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Tabbed Content Section */}
        <div className="mt-12">
          <div className="bg-white rounded-2xl shadow-lg overflow-hidden">
            <div className="flex border-b border-gray-200">
              <button
                onClick={() => setSelectedTab('description')}
                className={`flex-1 px-6 py-4 font-semibold transition-colors ${
                  selectedTab === 'description'
                    ? 'text-rose-600 border-b-2 border-rose-600 bg-rose-50'
                    : 'text-gray-600 hover:text-gray-900 hover:bg-gray-50'
                }`}
              >
                Mô tả sản phẩm
              </button>
              <button
                onClick={() => setSelectedTab('details')}
                className={`flex-1 px-6 py-4 font-semibold transition-colors ${
                  selectedTab === 'details'
                    ? 'text-rose-600 border-b-2 border-rose-600 bg-rose-50'
                    : 'text-gray-600 hover:text-gray-900 hover:bg-gray-50'
                }`}
              >
                Thông tin chi tiết
              </button>
            </div>
            <div className="p-8">
              {selectedTab === 'description' ? (
                <div className="prose max-w-none">
                  <p className="text-gray-700 leading-relaxed text-lg">
                    {product.description || product.shortDescription || 'Không có mô tả sản phẩm.'}
                  </p>
                </div>
              ) : (
                <div className="grid md:grid-cols-2 gap-6">
                  {product.categoryName && (
                    <div className="flex justify-between py-3 border-b border-gray-100">
                      <span className="text-gray-600 font-medium">Danh mục</span>
                      <span className="text-gray-900 font-semibold">{product.categoryName}</span>
                    </div>
                  )}
                  {(product as any).code && (
                    <div className="flex justify-between py-3 border-b border-gray-100">
                      <span className="text-gray-600 font-medium">Mã sản phẩm</span>
                      <span className="text-gray-900 font-semibold">{(product as any).code}</span>
                    </div>
                  )}
                  {product.brandName && (
                    <div className="flex justify-between py-3 border-b border-gray-100">
                      <span className="text-gray-600 font-medium">Thương hiệu</span>
                      <span className="text-gray-900 font-semibold">{product.brandName}</span>
                    </div>
                  )}
                  <div className="flex justify-between py-3 border-b border-gray-100">
                    <span className="text-gray-600 font-medium">Lượt xem</span>
                    <span className="text-gray-900 font-semibold">{product.viewCount || 0}</span>
                  </div>
                  <div className="flex justify-between py-3 border-b border-gray-100">
                    <span className="text-gray-600 font-medium">Yêu thích</span>
                    <span className="text-gray-900 font-semibold">{product.favoriteCount || 0}</span>
                  </div>
                  <div className="flex justify-between py-3 border-b border-gray-100">
                    <span className="text-gray-600 font-medium">Đánh giá</span>
                    <span className="text-gray-900 font-semibold">{product.reviewCount || 0} lượt</span>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
        {/* Category/Brand & Benefits - Two Column Layout */}
            <div className="grid md:grid-cols-2 gap-6">
              {/* Left: Category & Brand Info */}
              <div className="space-y-4">
                <h3 className="text-lg font-semibold text-gray-900 mb-3">Thông tin sản phẩm</h3>
                {product.categoryName && (
                  <div className="flex items-center gap-3 p-4 bg-white rounded-xl border border-gray-100">
                    <div className="w-10 h-10 bg-rose-100 rounded-full flex items-center justify-center flex-shrink-0">
                      <Package className="w-5 h-5 text-rose-600" />
                    </div>
                    <div>
                      <p className="font-semibold text-sm text-gray-900">Danh mục</p>
                      <p className="text-xs text-gray-600">{product.categoryName}</p>
                    </div>
                  </div>
                )}
                {product.brandName && (
                  <div className="flex items-center gap-3 p-4 bg-white rounded-xl border border-gray-100">
                    <div className="w-10 h-10 bg-amber-100 rounded-full flex items-center justify-center flex-shrink-0">
                      <Tag className="w-5 h-5 text-amber-600" />
                    </div>
                    <div>
                      <p className="font-semibold text-sm text-gray-900">Thương hiệu</p>
                      <p className="text-xs text-gray-600">{product.brandName}</p>
                    </div>
                  </div>
                )}
              </div>

              {/* Right: Service Benefits */}
              <div className="space-y-4">
                <h3 className="text-lg font-semibold text-gray-900 mb-3">Dịch vụ của chúng tôi</h3>
                <div className="flex items-center gap-3 p-4 bg-white rounded-xl border border-gray-100">
                  <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0">
                    <Shield className="w-5 h-5 text-blue-600" />
                  </div>
                  <div>
                    <p className="font-semibold text-sm text-gray-900">Chính hãng 100%</p>
                    <p className="text-xs text-gray-500">Cam kết bảo đảm</p>
                  </div>
                </div>
                <div className="flex items-center gap-3 p-4 bg-white rounded-xl border border-gray-100">
                  <div className="w-10 h-10 bg-green-100 rounded-full flex items-center justify-center flex-shrink-0">
                    <Truck className="w-5 h-5 text-green-600" />
                  </div>
                  <div>
                    <p className="font-semibold text-sm text-gray-900">Giao hàng nhanh</p>
                    <p className="text-xs text-gray-500">Miễn phí vận chuyển</p>
                  </div>
                </div>
                <div className="flex items-center gap-3 p-4 bg-white rounded-xl border border-gray-100">
                  <div className="w-10 h-10 bg-orange-100 rounded-full flex items-center justify-center flex-shrink-0">
                    <RotateCcw className="w-5 h-5 text-orange-600" />
                  </div>
                  <div>
                    <p className="font-semibold text-sm text-gray-900">Đổi trả dễ dàng</p>
                    <p className="text-xs text-gray-500">Trong vòng 7 ngày</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Additional Options - Below */}
            <div className="bg-gradient-to-r from-purple-50 to-pink-50 rounded-xl p-6 border border-purple-100">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Tùy chọn bổ sung</h3>
              <div className="grid grid-cols-2 gap-4">
                <div className="flex items-center gap-3 p-3 bg-white rounded-lg">
                  <Heart className="w-5 h-5 text-purple-600" />
                  <div>
                    <p className="font-semibold text-sm text-gray-900">Yêu thích</p>
                    <p className="text-xs text-gray-500">{product.favoriteCount || 0} người</p>
                  </div>
                </div>
                <div className="flex items-center gap-3 p-3 bg-white rounded-lg">
                  <Eye className="w-5 h-5 text-blue-600" />
                  <div>
                    <p className="font-semibold text-sm text-gray-900">Lượt xem</p>
                    <p className="text-xs text-gray-500">{product.viewCount || 0} lượt</p>
                  </div>
                </div>
              </div>
            </div>

        {/* Related Products Section */}
        {relatedProducts.length > 0 && (
          <div className="mt-16">
            <div className="flex items-center justify-between mb-8">
              <h2 className="text-3xl font-bold bg-gradient-to-r from-rose-600 to-pink-600 bg-clip-text text-transparent">
                Sản phẩm liên quan
              </h2>
              <Button variant="ghost" className="text-rose-600 hover:text-rose-700 hover:bg-rose-50">
                Xem tất cả
                <ChevronRight className="w-4 h-4 ml-1" />
              </Button>
            </div>
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
              {relatedProducts.map(relatedProduct => (
                <Card
                  key={relatedProduct.id}
                  className="group cursor-pointer border-2 border-gray-100 hover:border-rose-200 hover:shadow-xl transition-all duration-300 overflow-hidden rounded-2xl"
                  onClick={() => {
                    actionTracking.trackAction('ViewProduct', {}, relatedProduct.id, product?.categoryId);
                    navigate(`/product/${relatedProduct.id}`);
                  }}
                >
                  <CardContent className="p-0">
                    <div className="relative aspect-square bg-gray-50 overflow-hidden">
                      <ImageWithFallback
                        src={relatedProduct.primaryImage || relatedProduct.images?.[0]?.url || ''}
                        alt={relatedProduct.name || ''}
                        className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                      />
                      <div className="absolute inset-0 bg-gradient-to-t from-black/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity" />
                    </div>
                    <div className="p-4 space-y-2">
                      <h3 className="font-semibold text-sm text-gray-900 line-clamp-2 group-hover:text-rose-600 transition-colors">
                        {relatedProduct.name}
                      </h3>
                      <div className="flex items-center justify-between">
                        <span className="text-rose-600 font-bold text-lg">
                          {formatPrice(relatedProduct.price || 0)}
                        </span>
                        <div className="flex items-center gap-1">
                          <Star className="w-3 h-3 fill-yellow-400 text-yellow-400" />
                          <span className="text-xs text-gray-600 font-medium">
                            {(relatedProduct.averageRating || 0).toFixed(1)}
                          </span>
                        </div>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          </div>
        )}
      </div>

      {/* Fixed Bottom Action Bar (Mobile) */}
      <div className="lg:hidden fixed bottom-0 left-0 right-0 bg-white border-t-2 border-gray-200 p-4 shadow-2xl z-40">
        <div className="flex items-center gap-3">
          <Button
            variant="outline"
            size="icon"
            onClick={handleToggleWishlist}
            disabled={isTogglingWishlist}
            className={`flex-shrink-0 h-12 w-12 rounded-xl border-2 ${
              isProductInWishlist
                ? 'border-rose-500 bg-rose-50 text-rose-600'
                : 'border-gray-200'
            }`}
          >
            <Heart className={`w-5 h-5 ${isProductInWishlist ? 'fill-current' : ''}`} />
          </Button>
          <Button
            className="flex-1 bg-gradient-to-r from-rose-600 to-pink-600 hover:from-rose-700 hover:to-pink-700 text-white h-12 rounded-xl shadow-lg text-base font-semibold"
            onClick={handleAddToCart}
            disabled={isAddingToCart || !product.stockQuantity || product.stockQuantity <= 0}
          >
            {isAddingToCart ? (
              <>
                <Loader2 className="w-5 h-5 mr-2 animate-spin" />
                Đang thêm...
              </>
            ) : (
              <>
                <ShoppingCart className="w-5 h-5 mr-2" />
                Thêm vào giỏ • {formatPrice((product.displayPrice?.discountedPrice || product.price || 0) * quantity)}
              </>
            )}
          </Button>
        </div>
      </div>

      {/* Spacer for fixed bottom bar on mobile */}
      <div className="lg:hidden h-20" />
    </div>
  );
}
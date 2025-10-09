import { Dialog, DialogContent, DialogHeader, DialogTitle } from './ui/dialog';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { Star, ShoppingCart, Heart, X, Plus, Minus } from 'lucide-react';
import { useState } from 'react';

interface Product {
  id: number;
  name: string;
  price: number;
  originalPrice: number | null;
  discount: number;
  rating: number;
  reviews: number;
  image: string;
  category: string;
  featured: boolean;
}

interface QuickViewModalProps {
  product: Product | null;
  isOpen: boolean;
  onClose: () => void;
}

export function QuickViewModal({ product, isOpen, onClose }: QuickViewModalProps) {
  const [quantity, setQuantity] = useState(1);

  if (!product) return null;

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('vi-VN').format(price) + '₫';
  };

  const handleQuantityChange = (delta: number) => {
    setQuantity(prev => Math.max(1, prev + delta));
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-amber-900">Chi tiết sản phẩm</DialogTitle>
        </DialogHeader>
        
        <div className="grid md:grid-cols-2 gap-6">
          {/* Product Image */}
          <div className="relative">
            <ImageWithFallback
              src={product.image}
              alt={product.name}
              className="w-full h-96 object-cover rounded-lg"
            />
            {product.featured && (
              <Badge className="absolute top-3 left-3 bg-red-600 text-white">
                Nổi bật
              </Badge>
            )}
            {product.discount > 0 && (
              <Badge className="absolute top-3 right-3 bg-green-600 text-white">
                -{product.discount}%
              </Badge>
            )}
          </div>

          {/* Product Details */}
          <div className="space-y-4">
            <div>
              <h2 className="text-2xl text-amber-900 mb-2">{product.name}</h2>
              
              {/* Rating */}
              <div className="flex items-center gap-2 mb-4">
                <div className="flex items-center">
                  {[...Array(5)].map((_, i) => (
                    <Star key={i} className={`w-5 h-5 ${
                      i < Math.floor(product.rating) 
                        ? 'fill-yellow-400 text-yellow-400' 
                        : 'text-gray-300'
                    }`} />
                  ))}
                  <span className="text-sm text-gray-600 ml-2">
                    {product.rating} ({product.reviews} đánh giá)
                  </span>
                </div>
              </div>

              {/* Price */}
              <div className="flex items-center gap-3 mb-6">
                <span className="text-3xl text-red-600">
                  {formatPrice(product.price)}
                </span>
                {product.originalPrice && (
                  <span className="text-xl text-gray-500 line-through">
                    {formatPrice(product.originalPrice)}
                  </span>
                )}
              </div>
            </div>

            {/* Description */}
            <div className="border-t border-gray-200 pt-4">
              <h3 className="text-lg text-amber-900 mb-2">Mô tả sản phẩm</h3>
              <p className="text-gray-600 leading-relaxed">
                Sản phẩm {product.name.toLowerCase()} chất lượng cao, được tuyển chọn kỹ lưỡng 
                để mang đến trải nghiệm tâm linh tốt nhất. Phù hợp cho các nghi lễ truyền thống 
                và thể hiện lòng thành kính với tổ tiên.
              </p>
            </div>

            {/* Quantity Selection */}
            <div className="border-t border-gray-200 pt-4">
              <h3 className="text-lg text-amber-900 mb-3">Số lượng</h3>
              <div className="flex items-center gap-3 mb-4">
                <Button
                  variant="outline"
                  size="icon"
                  onClick={() => handleQuantityChange(-1)}
                  disabled={quantity <= 1}
                  className="border-amber-300 text-amber-700 hover:bg-amber-50"
                >
                  <Minus className="w-4 h-4" />
                </Button>
                <span className="w-12 text-center text-lg font-medium">{quantity}</span>
                <Button
                  variant="outline"
                  size="icon"
                  onClick={() => handleQuantityChange(1)}
                  className="border-amber-300 text-amber-700 hover:bg-amber-50"
                >
                  <Plus className="w-4 h-4" />
                </Button>
              </div>
            </div>

            {/* Action Buttons */}
            <div className="space-y-3 pt-4">
              <Button className="w-full bg-red-600 hover:bg-red-700 text-white text-lg py-3">
                <ShoppingCart className="w-5 h-5 mr-2" />
                Thêm {quantity} sản phẩm vào giỏ hàng
              </Button>
              <Button 
                variant="outline" 
                className="w-full border-amber-300 text-amber-700 hover:bg-amber-50"
              >
                <Heart className="w-5 h-5 mr-2" />
                Thêm vào yêu thích
              </Button>
            </div>

            {/* Additional Info */}
            <div className="border-t border-gray-200 pt-4 text-sm text-gray-600">
              <div className="space-y-2">
                <div className="flex justify-between">
                  <span>Danh mục:</span>
                  <span className="capitalize">{product.category}</span>
                </div>
                <div className="flex justify-between">
                  <span>Mã sản phẩm:</span>
                  <span>SP{product.id.toString().padStart(6, '0')}</span>
                </div>
                <div className="flex justify-between">
                  <span>Tình trạng:</span>
                  <span className="text-green-600">Còn hàng</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}

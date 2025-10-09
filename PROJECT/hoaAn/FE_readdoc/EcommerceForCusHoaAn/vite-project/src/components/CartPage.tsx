import { useState } from 'react';
import { Button } from './ui/button';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Separator } from './ui/separator';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { 
  Flower2, 
  ShoppingCart, 
  Plus, 
  Minus, 
  Trash2, 
  Tag,
  Truck,
  Shield,
  Clock,
  ArrowRight,
  Gift,
  CreditCard,
  MapPin
} from 'lucide-react';

interface CartItem {
  id: number;
  name: string;
  price: number;
  originalPrice?: number;
  quantity: number;
  image: string;
  category: string;
  inStock: boolean;
  maxQuantity: number;
}

export function CartPage() {
  const [cartItems, setCartItems] = useState<CartItem[]>([
    {
      id: 1,
      name: 'Mâm Cúng Trọn Gói Cao Cấp',
      price: 2890000,
      originalPrice: 3490000,
      quantity: 1,
      image: 'https://images.unsplash.com/photo-1519097000072-e44ffa116485?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwb2ZmZXJpbmdzJTIwYWx0YXIlMjBmcnVpdHN8ZW58MXx8fHwxNzU3Njc0NDI5fDA&ixlib=rb-4.1.0&q=80&w=1080',
      category: 'Mâm cúng',
      inStock: true,
      maxQuantity: 5
    },
    {
      id: 2,
      name: 'Hương Trầm Cao Cấp - Hộp 100 Cây',
      price: 450000,
      originalPrice: 520000,
      quantity: 2,
      image: 'https://images.unsplash.com/photo-1532334722716-c5850cdd878d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwaW5jZW5zZSUyMGNlcmVtb255JTIwdHJhZGl0aW9uYWx8ZW58MXx8fHwxNzU3Njc0NDI4fDA&ixlib=rb-4.1.0&q=80&w=1080',
      category: 'Hương',
      inStock: true,
      maxQuantity: 10
    },
    {
      id: 3,
      name: 'Nến Đỏ Phong Thủy - Bộ 12 Cây',
      price: 280000,
      originalPrice: 350000,
      quantity: 1,
      image: 'https://images.unsplash.com/photo-1732117924212-39bfaec174c9?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx0cmFkaXRpb25hbCUyMGNhbmRsZXMlMjByZWQlMjBnb2xkfGVufDF8fHx8MTc1NzY3NDQyOXww&ixlib=rb-4.1.0&q=80&w=1080',
      category: 'Nến',
      inStock: false,
      maxQuantity: 8
    }
  ]);

  const [promoCode, setPromoCode] = useState('');
  const [appliedPromo, setAppliedPromo] = useState<string | null>(null);

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('vi-VN').format(price) + '₫';
  };

  const updateQuantity = (id: number, newQuantity: number) => {
    if (newQuantity < 1) return;
    setCartItems(items =>
      items.map(item =>
        item.id === id
          ? { ...item, quantity: Math.min(newQuantity, item.maxQuantity) }
          : item
      )
    );
  };

  const removeItem = (id: number) => {
    setCartItems(items => items.filter(item => item.id !== id));
  };

  const applyPromoCode = () => {
    if (promoCode === 'WELCOME10') {
      setAppliedPromo('WELCOME10');
      setPromoCode('');
    } else if (promoCode === 'LUNAR2025') {
      setAppliedPromo('LUNAR2025');
      setPromoCode('');
    }
  };

  const subtotal = cartItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
  const originalTotal = cartItems.reduce((sum, item) => sum + ((item.originalPrice || item.price) * item.quantity), 0);
  const totalSavings = originalTotal - subtotal;
  
  const promoDiscount = appliedPromo === 'WELCOME10' ? subtotal * 0.1 : 
                       appliedPromo === 'LUNAR2025' ? subtotal * 0.15 : 0;
  
  const shippingFee = subtotal > 1000000 ? 0 : 50000;
  const finalTotal = subtotal - promoDiscount + shippingFee;

  if (cartItems.length === 0) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50">
        {/* Header */}
        <section className="bg-gradient-to-r from-amber-900 to-red-800 text-white py-16">
          <div className="max-w-6xl mx-auto px-4 text-center">
            <ShoppingCart className="w-16 h-16 mx-auto mb-4 text-yellow-300" />
            <h1 className="text-4xl md:text-5xl mb-4">Giỏ hàng của bạn</h1>
            <p className="text-xl text-yellow-100 max-w-2xl mx-auto">
              Quản lý các sản phẩm đồ cúng đã chọn
            </p>
          </div>
        </section>

        {/* Empty Cart */}
        <div className="max-w-4xl mx-auto px-4 py-16">
          <Card className="text-center border-2 border-amber-200">
            <CardContent className="p-12">
              <ShoppingCart className="w-24 h-24 mx-auto mb-6 text-gray-400" />
              <h2 className="text-2xl text-amber-900 mb-4">Giỏ hàng trống</h2>
              <p className="text-gray-600 mb-8 max-w-md mx-auto">
                Bạn chưa có sản phẩm nào trong giỏ hàng. Khám phá các sản phẩm đồ cúng chất lượng cao của chúng tôi.
              </p>
              <Button className="bg-red-600 hover:bg-red-700 text-white px-8 py-3">
                <ArrowRight className="w-4 h-4 mr-2" />
                Tiếp tục mua sắm
              </Button>
            </CardContent>
          </Card>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50">
      {/* Header */}
      <section className="bg-gradient-to-r from-amber-900 to-red-800 text-white py-16">
        <div className="max-w-6xl mx-auto px-4 text-center">
          <ShoppingCart className="w-16 h-16 mx-auto mb-4 text-yellow-300" />
          <h1 className="text-4xl md:text-5xl mb-4">Giỏ hàng của bạn</h1>
          <p className="text-xl text-yellow-100 max-w-2xl mx-auto">
            {cartItems.length} sản phẩm đang chờ thanh toán
          </p>
        </div>
      </section>

      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="grid lg:grid-cols-3 gap-8">
          {/* Cart Items */}
          <div className="lg:col-span-2 space-y-4">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-amber-900">
                  <ShoppingCart className="w-5 h-5" />
                  Sản phẩm trong giỏ hàng ({cartItems.length})
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {cartItems.map((item, index) => (
                  <div key={item.id}>
                    <div className="flex flex-col sm:flex-row gap-4">
                      {/* Product Image */}
                      <div className="relative flex-shrink-0">
                        <ImageWithFallback
                          src={item.image}
                          alt={item.name}
                          className="w-full sm:w-24 h-24 object-cover rounded-lg"
                        />
                        {!item.inStock && (
                          <div className="absolute inset-0 bg-black/50 rounded-lg flex items-center justify-center">
                            <span className="text-white text-xs">Hết hàng</span>
                          </div>
                        )}
                      </div>

                      {/* Product Info */}
                      <div className="flex-1 min-w-0">
                        <div className="flex flex-col sm:flex-row sm:justify-between gap-2">
                          <div className="flex-1">
                            <h3 className="text-amber-900 line-clamp-2">{item.name}</h3>
                            <Badge variant="secondary" className="bg-amber-100 text-amber-800 text-xs mt-1">
                              {item.category}
                            </Badge>
                            {!item.inStock && (
                              <Badge variant="destructive" className="text-xs mt-1 ml-2">
                                Hết hàng
                              </Badge>
                            )}
                          </div>

                          {/* Price */}
                          <div className="text-right">
                            <div className="text-lg text-red-600">{formatPrice(item.price)}</div>
                            {item.originalPrice && (
                              <div className="text-sm text-gray-500 line-through">
                                {formatPrice(item.originalPrice)}
                              </div>
                            )}
                          </div>
                        </div>

                        {/* Quantity & Actions */}
                        <div className="flex items-center justify-between mt-4">
                          <div className="flex items-center gap-2">
                            <Button
                              variant="outline"
                              size="icon"
                              className="h-8 w-8 border-amber-300"
                              onClick={() => updateQuantity(item.id, item.quantity - 1)}
                              disabled={item.quantity <= 1 || !item.inStock}
                            >
                              <Minus className="w-3 h-3" />
                            </Button>
                            
                            <Input
                              type="number"
                              value={item.quantity}
                              onChange={(e) => updateQuantity(item.id, parseInt(e.target.value) || 1)}
                              className="w-16 h-8 text-center border-amber-300"
                              min="1"
                              max={item.maxQuantity}
                              disabled={!item.inStock}
                            />
                            
                            <Button
                              variant="outline"
                              size="icon"
                              className="h-8 w-8 border-amber-300"
                              onClick={() => updateQuantity(item.id, item.quantity + 1)}
                              disabled={item.quantity >= item.maxQuantity || !item.inStock}
                            >
                              <Plus className="w-3 h-3" />
                            </Button>
                            
                            <span className="text-xs text-gray-500 ml-2">
                              Tối đa: {item.maxQuantity}
                            </span>
                          </div>

                          <Button
                            variant="outline"
                            size="sm"
                            className="text-red-600 border-red-300 hover:bg-red-50"
                            onClick={() => removeItem(item.id)}
                          >
                            <Trash2 className="w-4 h-4 mr-1" />
                            Xóa
                          </Button>
                        </div>
                      </div>
                    </div>
                    
                    {index < cartItems.length - 1 && <Separator className="mt-4" />}
                  </div>
                ))}
              </CardContent>
            </Card>

            {/* Promo Code */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-amber-900">
                  <Tag className="w-5 h-5" />
                  Mã giảm giá
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex gap-2">
                  <Input
                    placeholder="Nhập mã giảm giá"
                    value={promoCode}
                    onChange={(e) => setPromoCode(e.target.value)}
                    className="border-amber-300"
                  />
                  <Button 
                    onClick={applyPromoCode}
                    className="bg-amber-600 hover:bg-amber-700 text-white"
                  >
                    Áp dụng
                  </Button>
                </div>
                
                {appliedPromo && (
                  <div className="mt-3 p-3 bg-green-50 border border-green-200 rounded-lg">
                    <div className="flex items-center justify-between">
                      <span className="text-green-800">
                        Mã "{appliedPromo}" đã được áp dụng
                      </span>
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => setAppliedPromo(null)}
                        className="text-green-700 hover:text-green-800"
                      >
                        Hủy
                      </Button>
                    </div>
                  </div>
                )}

                <div className="mt-4">
                  <p className="text-sm text-gray-600 mb-2">Mã giảm giá có sẵn:</p>
                  <div className="space-y-2">
                    <Badge variant="outline" className="border-amber-300 text-amber-700">
                      WELCOME10 - Giảm 10% cho khách hàng mới
                    </Badge>
                    <Badge variant="outline" className="border-red-300 text-red-700">
                      LUNAR2025 - Giảm 15% mừng năm mới
                    </Badge>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Order Summary */}
          <div className="space-y-4">
            <Card className="sticky top-24">
              <CardHeader>
                <CardTitle className="text-amber-900">Tóm tắt đơn hàng</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Price Breakdown */}
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <span>Tạm tính:</span>
                    <span>{formatPrice(subtotal)}</span>
                  </div>
                  
                  {totalSavings > 0 && (
                    <div className="flex justify-between text-green-600">
                      <span>Tiết kiệm:</span>
                      <span>-{formatPrice(totalSavings)}</span>
                    </div>
                  )}
                  
                  {promoDiscount > 0 && (
                    <div className="flex justify-between text-green-600">
                      <span>Giảm giá ({appliedPromo}):</span>
                      <span>-{formatPrice(promoDiscount)}</span>
                    </div>
                  )}
                  
                  <div className="flex justify-between">
                    <span>Phí vận chuyển:</span>
                    <span className={shippingFee === 0 ? 'text-green-600' : ''}>
                      {shippingFee === 0 ? 'Miễn phí' : formatPrice(shippingFee)}
                    </span>
                  </div>
                </div>

                <Separator />
                
                <div className="flex justify-between text-lg">
                  <span>Tổng cộng:</span>
                  <span className="text-red-600">{formatPrice(finalTotal)}</span>
                </div>

                {/* Benefits */}
                <div className="space-y-2 text-sm">
                  <div className="flex items-center gap-2 text-green-600">
                    <Truck className="w-4 h-4" />
                    <span>
                      {shippingFee === 0 ? 'Miễn phí vận chuyển' : 'Mua thêm ' + formatPrice(1000000 - subtotal) + ' để được miễn phí ship'}
                    </span>
                  </div>
                  <div className="flex items-center gap-2 text-blue-600">
                    <Shield className="w-4 h-4" />
                    <span>Đảm bảo chất lượng 100%</span>
                  </div>
                  <div className="flex items-center gap-2 text-purple-600">
                    <Clock className="w-4 h-4" />
                    <span>Giao hàng trong 2-4 giờ</span>
                  </div>
                  <div className="flex items-center gap-2 text-orange-600">
                    <Gift className="w-4 h-4" />
                    <span>Tư vấn ngày giờ tốt miễn phí</span>
                  </div>
                </div>

                <Separator />

                {/* Checkout Button */}
                <Button 
                  className="w-full bg-red-600 hover:bg-red-700 text-white py-3"
                  disabled={cartItems.some(item => !item.inStock)}
                >
                  <CreditCard className="w-4 h-4 mr-2" />
                  Tiến hành thanh toán
                </Button>

                <Button variant="outline" className="w-full border-amber-300 text-amber-700 hover:bg-amber-50">
                  <ArrowRight className="w-4 h-4 mr-2" />
                  Tiếp tục mua sắm
                </Button>

                {/* Delivery Info */}
                <Card className="bg-amber-50 border-amber-200">
                  <CardContent className="p-4">
                    <div className="flex items-start gap-2">
                      <MapPin className="w-4 h-4 text-amber-600 mt-1 flex-shrink-0" />
                      <div className="text-sm">
                        <p className="text-amber-900 mb-1">Giao hàng tới:</p>
                        <p className="text-gray-700">123 Đường ABC, Quận 1, TP.HCM</p>
                        <Button variant="link" className="text-amber-700 p-0 h-auto text-sm">
                          Thay đổi địa chỉ
                        </Button>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                {/* Payment Methods */}
                <div className="text-center">
                  <p className="text-sm text-gray-600 mb-2">Phương thức thanh toán</p>
                  <div className="flex justify-center gap-2">
                    <Badge variant="outline" className="text-xs">MoMo</Badge>
                    <Badge variant="outline" className="text-xs">VNPay</Badge>
                    <Badge variant="outline" className="text-xs">Thẻ tín dụng</Badge>
                    <Badge variant="outline" className="text-xs">COD</Badge>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
}

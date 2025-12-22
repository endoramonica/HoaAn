import { useState, useEffect } from "react";
import { Button } from "./ui/button";
import { useNavigate } from "react-router-dom";
import { Card, CardContent, CardHeader, CardTitle } from "./ui/card";
import { Badge } from "./ui/badge";
import { Input } from "./ui/input";
import { Separator } from "./ui/separator";
import { ImageWithFallback } from "./figma/ImageWithFallback";
import {
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
  MapPin,
  Loader2,
  AlertCircle,
  Sparkles,
} from "lucide-react";
import { useCart } from "../lib/hooks/useCart";
import { CartItemCustomizations } from "./CartItemCustomizations";
import { CustomizationEditor } from "./CustomizationEditor";
import { toast } from "sonner";
import { getActionTrackingService } from "../lib/services/actionTrackingService";
import { getRecommendationService } from "../lib/services/recommendationService";
import { getUserPreferenceService } from "../lib/services/userPreferenceService";
import { RitualRecommendation } from "./RitualRecommendation";

export function CartPage() {
  const navigate = useNavigate();
  const [promoCode, setPromoCode] = useState("");
  const [editingCustomizationId, setEditingCustomizationId] = useState<string | null>(null);
  const [recommendations, setRecommendations] = useState<any[]>([]);
  const [isGeneratingRecommendations, setIsGeneratingRecommendations] = useState(false);
  
  // Initialize services
  const actionTracking = getActionTrackingService();
  const recommendationService = getRecommendationService();
  const userPreferenceService = getUserPreferenceService();
  
  // ✅ useCart tự động xử lý guest/user dựa trên useAuth
  // ✅ Backend tự động lấy sessionId từ HTTP-only cookie
  const {
    cartItems,
    summary,
    isLoading,
    error,
    isGuest,
    updateQuantity,
    removeItem,
    applyCoupon,
    removeCoupon,
  } = useCart();

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("vi-VN").format(price) + "₫";
  };

  // Subscribe to action changes and generate recommendations
  useEffect(() => {
    const generateRecommendation = async () => {
      try {
        setIsGeneratingRecommendations(true);
        const sequence = actionTracking.getActionSequenceObject();
        
        // Only generate if we have actions
        if (sequence.actions.length > 0) {
          const recs = await recommendationService.generateRecommendations(sequence);
          setRecommendations(recs || []);
        }
      } catch (err) {
        console.error('Error generating recommendations:', err);
      } finally {
        setIsGeneratingRecommendations(false);
      }
    };

    // Subscribe to action changes
    const unsubscribe = actionTracking.subscribe(() => {
      generateRecommendation();
    });

    // Generate initial recommendations
    generateRecommendation();

    return unsubscribe;
  }, []);

  const handleDismissRecommendation = (ritualId: string) => {
    userPreferenceService.dismissRitual(ritualId);
    setRecommendations(prev => prev.filter(r => r.id !== ritualId));
    toast.info('Đã ẩn gợi ý này');
  };

  const handleDisableRitual = (ritualId: string) => {
    userPreferenceService.disableRitual(ritualId);
    setRecommendations(prev => prev.filter(r => r.id !== ritualId));
    toast.info('Đã tắt gợi ý này');
  };

  const handleAddRecommendationToCart = (ritual: any) => {
    // Track AddToCart action for recommended ritual
    actionTracking.trackAction('AddToCart', { source: 'recommendation' }, ritual.id, 'ritual-items');
    toast.success(`Đã thêm "${ritual.name}" vào giỏ hàng`);
  };

  const handleUpdateQuantity = async (cartItemId: string, newQuantity: number) => {
    if (newQuantity < 1) return;
    try {
      await updateQuantity(cartItemId, newQuantity);
    } catch (err) {
      console.error("Failed to update quantity:", err);
    }
  };

  const handleRemoveItem = async (cartItemId: string) => {
    try {
      await removeItem(cartItemId);
      toast.success('Đã xóa sản phẩm khỏi giỏ hàng');
    } catch (err: any) {
      console.error("Failed to remove item:", err);
      console.error("Error details:", {
        message: err.message,
        status: err.status,
        statusCode: err.statusCode,
        body: err.body,
      });
      
      // Hiển thị lỗi chi tiết
      const errorMessage = err.body?.message || err.message || 'Không thể xóa sản phẩm';
      toast.error(errorMessage);
      
      // Log chi tiết để debug
      console.error('[CartPage] Remove item failed:', {
        cartItemId,
        error: err,
        body: err.body,
      });
    }
  };

  const handleApplyPromoCode = async () => {
    if (!promoCode.trim()) return;
    
    try {
      await applyCoupon(promoCode.trim());
      setPromoCode("");
    } catch (err) {
      console.error("Failed to apply coupon:", err);
    }
  };

  const handleRemoveCoupon = async () => {
    try {
      await removeCoupon();
    } catch (err) {
      console.error("Failed to remove coupon:", err);
    }
  };

  // Loading state
  if (isLoading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50 flex items-center justify-center">
        <div className="text-center">
          <Loader2 className="w-12 h-12 animate-spin text-amber-600 mx-auto mb-4" />
          <p className="text-gray-600">Đang tải giỏ hàng...</p>
        </div>
      </div>
    );
  }

  // Error state - CHỈ hiển thị cho lỗi thật sự, KHÔNG hiển thị cho 404
  if (error && !error.includes('Not Found') && !error.includes('404')) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50">
        <section className="bg-gradient-to-r from-amber-900 to-red-800 text-white py-16">
          <div className="max-w-6xl mx-auto px-4 text-center">
            <ShoppingCart className="w-16 h-16 mx-auto mb-4 text-yellow-300" />
            <h1 className="text-4xl md:text-5xl mb-4">Giỏ hàng của bạn</h1>
          </div>
        </section>

        <div className="max-w-4xl mx-auto px-4 py-16">
          <Card className="border-2 border-red-200">
            <CardContent className="p-12 text-center">
              <AlertCircle className="w-16 h-16 mx-auto mb-4 text-red-500" />
              <h2 className="text-2xl text-red-700 mb-4">Có lỗi xảy ra</h2>
              <p className="text-gray-600 mb-8">{error}</p>
              <Button
                onClick={() => window.location.reload()}
                className="bg-red-600 hover:bg-red-700 text-white"
              >
                Thử lại
              </Button>
            </CardContent>
          </Card>
        </div>
      </div>
    );
  }

  // Empty cart state
  if (!cartItems || cartItems.length === 0) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50">
        <section className="bg-gradient-to-r from-amber-900 to-red-800 text-white py-16">
          <div className="max-w-6xl mx-auto px-4 text-center">
            <ShoppingCart className="w-16 h-16 mx-auto mb-4 text-yellow-300" />
            <h1 className="text-4xl md:text-5xl mb-4">Giỏ hàng của bạn</h1>
            <p className="text-xl text-yellow-100 max-w-2xl mx-auto">
              Quản lý các sản phẩm đồ cúng đã chọn
            </p>
          </div>
        </section>

        <div className="max-w-4xl mx-auto px-4 py-16">
          <Card className="text-center border-2 border-amber-200">
            <CardContent className="p-12">
              <ShoppingCart className="w-24 h-24 mx-auto mb-6 text-gray-400" />
              <h2 className="text-2xl text-amber-900 mb-4">Giỏ hàng trống</h2>
              <p className="text-gray-600 mb-8 max-w-md mx-auto">
                Bạn chưa có sản phẩm nào trong giỏ hàng. Khám phá các sản phẩm
                đồ cúng chất lượng cao của chúng tôi.
              </p>
              <Button
                onClick={() => navigate("/")}
                className="bg-red-600 hover:bg-red-700 text-white px-8 py-3"
              >
                <ArrowRight className="w-4 h-4 mr-2" />
                Tiếp tục mua sắm
              </Button>
            </CardContent>
          </Card>
        </div>
      </div>
    );
  }

  const subtotal = summary?.subtotal || 0;
  const discount = summary?.discount || 0;
  const shippingFee = summary?.shippingFee || 0;
  const finalTotal = summary?.total || 0;

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
                            <h3 className="text-amber-900 line-clamp-2">
                              {item.name}
                            </h3>
                            <Badge
                              variant="secondary"
                              className="bg-amber-100 text-amber-800 text-xs mt-1"
                            >
                              {item.category}
                            </Badge>
                            {!item.inStock && (
                              <Badge
                                variant="destructive"
                                className="text-xs mt-1 ml-2"
                              >
                                Hết hàng
                              </Badge>
                            )}
                          </div>

                          {/* Price */}
                          <div className="text-right">
                            {/* Display finalPrice if customizations exist, otherwise unitPrice */}
                            <div className="text-lg text-red-600">
                              {formatPrice(item.finalPrice || item.price)}
                            </div>
                            {/* Show price breakdown if customizations exist */}
                            {item.customizations && item.customizations.some(c => (c.quantity || 0) > 0) && (
                              <div className="text-xs text-gray-600 mt-1">
                                <div>Base: {formatPrice(item.basePrice || item.price)}</div>
                                <div className="text-amber-600">+Custom: {formatPrice(item.customizationPrice || 0)}</div>
                              </div>
                            )}
                            {item.originalPrice && item.originalPrice > (item.finalPrice || item.price) && (
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
                              onClick={() =>
                                handleUpdateQuantity(item.id, item.quantity - 1)
                              }
                              disabled={item.quantity <= 1 || !item.inStock}
                            >
                              <Minus className="w-3 h-3" />
                            </Button>

                            <Input
                              type="number"
                              value={item.quantity}
                              onChange={(e) =>
                                handleUpdateQuantity(
                                  item.id,
                                  parseInt(e.target.value) || 1
                                )
                              }
                              className="w-16 h-8 text-center border-amber-300"
                              min="1"
                              max={item.maxQuantity}
                              disabled={!item.inStock}
                            />

                            <Button
                              variant="outline"
                              size="icon"
                              className="h-8 w-8 border-amber-300"
                              onClick={() =>
                                handleUpdateQuantity(item.id, item.quantity + 1)
                              }
                              disabled={
                                item.quantity >= item.maxQuantity || !item.inStock
                              }
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
                            onClick={() => handleRemoveItem(item.id)}
                          >
                            <Trash2 className="w-4 h-4 mr-1" />
                            Xóa
                          </Button>
                        </div>

                        {/* Customizations */}
                        {item.customizations && item.customizations.length > 0 && (
                          <>
                            <CartItemCustomizations
                              customizations={item.customizations}
                              productName={item.name}
                            />
                            {/* Edit Customizations Button */}
                            <Button
                              variant="outline"
                              size="sm"
                              className="mt-3 w-full border-amber-300 text-amber-700 hover:bg-amber-50"
                              onClick={() => setEditingCustomizationId(item.id)}
                            >
                              Chỉnh sửa tùy chọn
                            </Button>
                          </>
                        )}
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
                    onKeyPress={(e) => e.key === "Enter" && handleApplyPromoCode()}
                    className="border-amber-300"
                  />
                  <Button
                    onClick={handleApplyPromoCode}
                    disabled={!promoCode.trim()}
                    className="bg-amber-600 hover:bg-amber-700 text-white"
                  >
                    Áp dụng
                  </Button>
                </div>

                {summary?.appliedCoupon && (
                  <div className="mt-3 p-3 bg-green-50 border border-green-200 rounded-lg">
                    <div className="flex items-center justify-between">
                      <span className="text-green-800">
                        Mã "{summary.appliedCoupon}" đã được áp dụng
                      </span>
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={handleRemoveCoupon}
                        className="text-green-700 hover:text-green-800"
                      >
                        Hủy
                      </Button>
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Ritual Recommendations Section */}
            {recommendations.length > 0 && (
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2 text-amber-900">
                    <Sparkles className="w-5 h-5 text-yellow-500" />
                    Gợi ý nghi lễ phù hợp
                  </CardTitle>
                  <p className="text-sm text-gray-600 mt-2">
                    Dựa trên hành động mua sắm của bạn, chúng tôi gợi ý những nghi lễ phù hợp
                  </p>
                </CardHeader>
                <CardContent>
                  <RitualRecommendation
                    recommendations={recommendations}
                    onDismiss={handleDismissRecommendation}
                    onDisable={handleDisableRitual}
                    onAddToCart={handleAddRecommendationToCart}
                    isLoading={isGeneratingRecommendations}
                  />
                </CardContent>
              </Card>
            )}
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

                  {discount > 0 && (
                    <div className="flex justify-between text-green-600">
                      <span>Giảm giá:</span>
                      <span>-{formatPrice(discount)}</span>
                    </div>
                  )}

                  <div className="flex justify-between">
                    <span>Phí vận chuyển:</span>
                    <span className={shippingFee === 0 ? "text-green-600" : ""}>
                      {shippingFee === 0 ? "Miễn phí" : formatPrice(shippingFee)}
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
                      {shippingFee === 0
                        ? "Miễn phí vận chuyển"
                        : `Mua thêm ${formatPrice(1000000 - subtotal)} để được miễn phí ship`}
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
                  disabled={cartItems.some((item) => !item.inStock)}
                  onClick={() => navigate("/checkout")}
                >
                  <CreditCard className="w-4 h-4 mr-2" />
                  Tiến hành thanh toán
                </Button>

                <Button
                  variant="outline"
                  className="w-full border-amber-300 text-amber-700 hover:bg-amber-50"
                  onClick={() => navigate("/")}
                >
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
                        <p className="text-gray-700">
                          123 Đường ABC, Quận 1, TP.HCM
                        </p>
                        <Button
                          variant="link"
                          className="text-amber-700 p-0 h-auto text-sm"
                        >
                          Thay đổi địa chỉ
                        </Button>
                      </div>
                    </div>
                  </CardContent>
                </Card>

                {/* Payment Methods */}
                <div className="text-center">
                  <p className="text-sm text-gray-600 mb-2">
                    Phương thức thanh toán
                  </p>
                  <div className="flex justify-center gap-2 flex-wrap">
                    <Badge variant="outline" className="text-xs">
                      MoMo
                    </Badge>
                    <Badge variant="outline" className="text-xs">
                      VNPay
                    </Badge>
                    <Badge variant="outline" className="text-xs">
                      Thẻ tín dụng
                    </Badge>
                    <Badge variant="outline" className="text-xs">
                      COD
                    </Badge>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>

        {/* Customization Editor Modal */}
        {editingCustomizationId && (
          <CustomizationEditor
            open={!!editingCustomizationId}
            onOpenChange={(open) => {
              if (!open) setEditingCustomizationId(null);
            }}
            customizations={
              cartItems.find(item => item.id === editingCustomizationId)?.customizations || []
            }
            productName={
              cartItems.find(item => item.id === editingCustomizationId)?.name || 'Sản phẩm'
            }
            onSave={async (updatedCustomizations) => {
              // TODO: Call API to update customizations
              // For now, just show a message
              toast.info('Chức năng cập nhật tùy chọn sẽ được hoàn thành');
            }}
          />
        )}
      </div>
    </div>
  );
}
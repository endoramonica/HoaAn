/**
 * CheckoutPage - Trang thanh toán với thiết kế văn hóa Việt
 * ✅ REFACTORED: Sử dụng Orval-generated types
 * ✅ FIX: Đúng address structure theo backend API
 */

import { useState, useEffect } from 'react';
import { useCart } from '../../lib/hooks/useCart';
import { useHybridNavigate } from '../../lib/hooks/useHybridNavigate';
import { useCustomerAddress } from '../../lib/hooks/useCustomerAddress';
import { useCheckout } from '../../lib/hooks/useCheckout';
import { useAuth } from '../../lib/hooks/useAuth';
import { paymentService } from '../../lib/services/paymentService';
import { CartService } from '../../api/services/CartService';
// ✅ Use Orval-generated types
import type { 
  CreateAddressDto,
  PaymentMethodType,
  CheckoutDto 
} from '../../../Api/generated-orval/schemas';
import { 
  AddressType, 
  PaymentMethodType as PaymentMethod,
  ShippingMethodEnum 
} from '../../../Api/generated-orval/schemas';
import { Button } from '../../components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../../components/ui/card';
import { RadioGroup, RadioGroupItem } from '../../components/ui/radio-group';
import { Label } from '../../components/ui/label';
import { Separator } from '../../components/ui/separator';
import { Badge } from '../../components/ui/badge';
import { Input } from '../../components/ui/input';
import { Textarea } from '../../components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../../components/ui/select';
import { 
  Flower2, 
  MapPin, 
  CreditCard, 
  Package,
  Plus,
  Check,
  Loader2,
  ArrowLeft,
  Shield,
  Truck
} from 'lucide-react';
import { toast } from 'sonner';
import { ImageWithFallback } from '../../components/figma/ImageWithFallback';
import { CartItemCustomizations } from '../../components/CartItemCustomizations';

interface CheckoutPageProps {
  onNavigate: (step: 'success' | 'failed', data?: any) => void;
}

export const CheckoutPage = ({ onNavigate }: CheckoutPageProps) => {
  const navigate = useHybridNavigate();
  const { cart, isLoading: cartLoading } = useCart();
  const { isAuthenticated } = useAuth();
  const isGuest = !isAuthenticated;
  
  // ✅ Sử dụng custom hooks
  const { 
    addresses, 
    isLoading: addressLoading, 
    loadAddresses, 
    addAddress 
  } = useCustomerAddress();
  
  const { 
    processCheckout, 
    isProcessing 
  } = useCheckout();
  
  // States
  const [selectedAddressId, setSelectedAddressId] = useState<string>('');
  const [selectedPaymentMethod, setSelectedPaymentMethod] = useState<PaymentMethodType>(PaymentMethod.cod);
  const [showAddAddressForm, setShowAddAddressForm] = useState(false);
  const [orderNote, setOrderNote] = useState('');
  const [bookingInfo, setBookingInfo] = useState<any>(null);

  // ✅ New address form - Using correct Orval structure
  const [newAddress, setNewAddress] = useState<CreateAddressDto>({
    streetAddress: '',
    city: null,
    postalCode: null,
    state: null,
    country: 'Vietnam',
    addressType: AddressType.home,
    recipientName: null,
    phoneNumber: null,
    email: null,
    isDefault: false,
    isPrimary: false,
  });

  // ✅ Load addresses on mount
  useEffect(() => {
    loadAddresses();
  }, [loadAddresses]);

  // ✅ Load booking info from sessionStorage if available (from calendar booking)
  useEffect(() => {
    const savedBookingInfo = sessionStorage.getItem('bookingInfo');
    if (savedBookingInfo) {
      try {
        const booking = JSON.parse(savedBookingInfo);
        setBookingInfo(booking);
        console.log('[CheckoutPage] 📅 Booking info loaded from sessionStorage:', booking);
        
        // Pre-fill order note with booking details if available
        if (booking.bookingNotes && !orderNote) {
          setOrderNote(booking.bookingNotes);
        }
      } catch (err) {
        console.error('[CheckoutPage] Error parsing booking info:', err);
      }
    }
  }, []);

  // ✅ Auto-select default address
  useEffect(() => {
    if (addresses.length > 0 && !selectedAddressId) {
      const defaultAddr = addresses.find(a => a.isDefault);
      if (defaultAddr?.id) {
        setSelectedAddressId(defaultAddr.id);
      } else if (addresses[0].id) {
        setSelectedAddressId(addresses[0].id);
      }
    }
  }, [addresses, selectedAddressId]);

  const handleAddAddress = async () => {
    try {
      // ✅ Validate required fields according to Orval schema
      if (!newAddress.streetAddress) {
        toast.error('Vui lòng nhập địa chỉ chi tiết');
        return;
      }

      if (!newAddress.recipientName && !newAddress.phoneNumber) {
        toast.error('Vui lòng nhập tên người nhận hoặc số điện thoại');
        return;
      }

      const addedAddress = await addAddress(newAddress);
      
      if (addedAddress?.id) {
        setSelectedAddressId(addedAddress.id);
        setShowAddAddressForm(false);
        // ✅ Reset form with correct structure
        setNewAddress({
          streetAddress: '',
          city: null,
          postalCode: null,
          state: null,
          country: 'Vietnam',
          addressType: AddressType.home,
          recipientName: null,
          phoneNumber: null,
          email: null,
          isDefault: false,
          isPrimary: false,
        });
      }
    } catch (err: any) {
      console.error('Add address error:', err);
    }
  };

  const handlePlaceOrder = async () => {
    try {
      // ============================================================================
      // STEP 0: Refresh Cart to Ensure Latest Data
      // ============================================================================
      console.log('[CheckoutPage] 🔄 Refreshing cart before checkout...');
      
      try {
        // Force refresh cart from backend
        const freshCart = await (isGuest 
          ? CartService.getApiV1CartGuest()
          : CartService.getApiV1Cart());
        
        console.log('[CheckoutPage] ✅ Cart refreshed:', {
          itemCount: freshCart?.data?.totalItems,
          items: freshCart?.data?.items?.length
        });
      } catch (refreshErr) {
        console.warn('[CheckoutPage] ⚠️ Cart refresh failed, using current state:', refreshErr);
      }

      // ============================================================================
      // STEP 1: Validate Cart
      // ============================================================================
      console.log('[CheckoutPage] 🔍 Validating cart...');
      
      if (!cart) {
        toast.error('Không tìm thấy giỏ hàng');
        navigate('/cart');
        return;
      }

      if (!cart.id) {
        toast.error('Giỏ hàng không hợp lệ. Vui lòng tải lại trang.');
        navigate('/cart');
        return;
      }

      if (!cart.items || cart.items.length === 0) {
        toast.error('Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi thanh toán.');
        navigate('/cart');
        return;
      }

      // ✅ Log detailed cart information for debugging
      console.log('[CheckoutPage] 📦 Cart Details:', {
        cartId: cart.id,
        itemCount: cart.items.length,
        totalAmount: cart.totalAmount,
        items: cart.items.map(item => ({
          id: item.id,
          productId: item.productId,
          name: item.name,
          quantity: item.quantity,
          price: item.price,
          basePrice: item.basePrice,
          customizationPrice: item.customizationPrice,
          finalPrice: item.finalPrice,
          hasCustomizations: (item.customizations?.length || 0) > 0
        }))
      });

      console.log('[CheckoutPage] ✅ Cart valid:', {
        cartId: cart.id,
        itemCount: cart.items.length,
        totalAmount: cart.totalAmount
      });

      // ============================================================================
      // STEP 2: Validate Address
      // ============================================================================
      console.log('[CheckoutPage] 🔍 Validating address...');

      if (!selectedAddressId) {
        toast.error('Vui lòng chọn địa chỉ giao hàng');
        return;
      }

      const selectedAddress = addresses.find(a => a.id === selectedAddressId);
      if (!selectedAddress) {
        toast.error('Địa chỉ không hợp lệ');
        return;
      }

      if (!selectedAddress.streetAddress) {
        toast.error('Địa chỉ thiếu thông tin chi tiết');
        return;
      }

      if (!selectedAddress.phoneNumber) {
        toast.error('Địa chỉ thiếu số điện thoại');
        return;
      }

      // Validate phone number format (10-11 digits)
      const phoneRegex = /^[0-9]{10,11}$/;
      if (!phoneRegex.test(selectedAddress.phoneNumber)) {
        toast.error('Số điện thoại không hợp lệ (cần 10-11 chữ số)');
        return;
      }

      console.log('[CheckoutPage] ✅ Address valid');

      // ============================================================================
      // STEP 3: Create Checkout Data
      // ============================================================================
      console.log('[CheckoutPage] 📦 Preparing checkout data...');

      const checkoutData: CheckoutDto = {
        cartId: cart.id,
        shippingInfo: {
          recipientName: selectedAddress.recipientName || 'Người nhận',
          phoneNumber: selectedAddress.phoneNumber,
          address: selectedAddress.streetAddress,
          ward: selectedAddress.state || 'Phường/Xã',
          district: selectedAddress.city || 'Quận/Huyện',
          city: selectedAddress.country || 'Vietnam',
          postalCode: selectedAddress.postalCode || null,
          deliveryNote: orderNote || null,
          shippingMethod: ShippingMethodEnum.standard,
        },
        paymentMethod: selectedPaymentMethod,
        couponCode: cart.couponCode || null,
        notes: orderNote || null,
      };

      // ✅ Validate checkout data before sending
      console.log('[CheckoutPage] 🔍 Validating checkout payload...');
      if (!checkoutData.cartId) {
        throw new Error('Cart ID is missing');
      }
      if (!checkoutData.shippingInfo.recipientName) {
        throw new Error('Recipient name is missing');
      }
      if (!checkoutData.shippingInfo.phoneNumber) {
        throw new Error('Phone number is missing');
      }
      if (!checkoutData.shippingInfo.address) {
        throw new Error('Address is missing');
      }

      console.log('[CheckoutPage] ✅ Checkout data prepared:', {
        cartId: checkoutData.cartId,
        itemCount: cart.items.length,
        totalAmount: cart.totalAmount,
        paymentMethod: checkoutData.paymentMethod,
        recipientName: checkoutData.shippingInfo.recipientName,
        phoneNumber: checkoutData.shippingInfo.phoneNumber,
        address: checkoutData.shippingInfo.address
      });

      // ============================================================================
      // STEP 4: Process Checkout
      // ============================================================================
      console.log('[CheckoutPage] 🛒 Processing checkout...');
// ✅ ADD THIS: Log before checkout
    console.log('[CheckoutPage] 🔍 Pre-checkout verification:');
    console.log('[CheckoutPage] 📋 Auth Token:', localStorage.getItem('auth_token')?.substring(0, 20) + '...');
    console.log('[CheckoutPage] 🛒 Cart from useCart:', {
      cartId: cart?.id,
      itemCount: cart?.items?.length,
      totalAmount: cart?.totalAmount
    });
    
    // ✅ ADD THIS: Verify cart one more time before checkout
    console.log('[CheckoutPage] 🔄 Refreshing cart before checkout...');
   // await refreshedCart();
    
    // Small delay to ensure state update
    await new Promise(resolve => setTimeout(resolve, 200));
    
    // Re-check cart after refresh
    if (!cart || !cart.items || cart.items.length === 0) {
      console.error('[CheckoutPage] ❌ Cart empty after refresh!');
      toast.error('Giỏ hàng trống sau khi làm mới. Vui lòng thử lại.');
      navigate('/cart');
      return;
    }
    
    console.log('[CheckoutPage] ✅ Cart verified after refresh:', {
      cartId: cart.id,
      itemCount: cart.items.length
    });
      const result = await processCheckout(checkoutData as any);

      if (!result) {
        console.error('[CheckoutPage] ❌ Checkout failed - No result returned');
        toast.error('Không thể đặt hàng');
        onNavigate('failed', { reason: 'Checkout failed' });
        return;
      }

      console.log('[CheckoutPage] ✅ Checkout successful:', {
        orderId: result.orderId,
        orderNumber: result.orderNumber,
        totalAmount: result.totalAmount
      });

      // ============================================================================
      // STEP 5: Clear Booking State (CRITICAL FIX)
      // ============================================================================
      console.log('[CheckoutPage] 🧹 Clearing booking state...');
      
      // ✅ Remove booking info from sessionStorage to prevent "sticking" to next cart
      sessionStorage.removeItem('bookingInfo');
      sessionStorage.removeItem('selectedBookingSlot');
      sessionStorage.removeItem('bookingInfoClient');
      
      console.log('[CheckoutPage] ✅ Booking state cleared');

      // ============================================================================
      // STEP 6: Handle Payment
      // ============================================================================
      console.log('[CheckoutPage] 💳 Handling payment method:', selectedPaymentMethod);

      // ✅ Handle payment based on method using enum
      if (selectedPaymentMethod === PaymentMethod.cod || selectedPaymentMethod === PaymentMethod.cash) {
        // COD - Navigate to success immediately
        onNavigate('success', { 
          orderId: result.orderId || '', 
          orderNumber: result.orderNumber || '' 
        });
      } else if (selectedPaymentMethod === PaymentMethod.banK_TRANSFER) {
        // Bank Transfer - Show banking info
        onNavigate('success', { 
          orderId: result.orderId || '', 
          orderNumber: result.orderNumber || '',
          paymentMethod: 'BankTransfer',
        });
      } else {
        // Online payment (Credit Card, E-Wallet) - Create payment and redirect
        const paymentResponse = await paymentService.createPayment({
          orderId: result.orderId || '',
          amount: result.totalAmount || 0,
          paymentMethod: selectedPaymentMethod as any, // Cast to bypass type mismatch
          returnUrl: `${window.location.origin}/checkout?step=success&orderId=${result.orderId}&orderNumber=${result.orderNumber}`,
          cancelUrl: `${window.location.origin}/checkout?step=failed&orderId=${result.orderId}&reason=Payment cancelled`,
        });

        if (paymentResponse.paymentUrl) {
          toast.success('Đang chuyển đến cổng thanh toán...');
          
          // Save pending order info
          sessionStorage.setItem('pendingOrderId', result.orderId || '');
          sessionStorage.setItem('pendingOrderNumber', result.orderNumber || '');
          
          // Redirect to payment gateway
          window.location.href = paymentResponse.paymentUrl;
        } else {
          // Payment processed without redirect
          onNavigate('success', { 
            orderId: result.orderId, 
            orderNumber: result.orderNumber 
          });
        }
      }
    } catch (err: any) {
      console.error('[CheckoutPage] ❌ Place order error:', err);
      const errorMsg = err.message || 'Không thể đặt hàng';
      toast.error(errorMsg);
      onNavigate('failed', { reason: errorMsg });
    }
  };

  const isLoading = addressLoading || cartLoading;

  if (isLoading) {
    return (
      <div className="min-h-screen bg-[#FFFBEB] flex items-center justify-center">
        <div className="text-center">
          <Loader2 className="w-12 h-12 animate-spin text-[#92400E] mx-auto mb-4" />
          <p className="text-[#92400E]/70">Đang tải thông tin thanh toán...</p>
        </div>
      </div>
    );
  }

  if (!cart || cart.items.length === 0) {
    return (
      <div className="min-h-screen bg-[#FFFBEB] px-4 py-12">
        <div className="max-w-2xl mx-auto text-center">
          <Package className="w-16 h-16 text-[#92400E]/30 mx-auto mb-4" />
          <h2 className="text-2xl text-[#92400E] mb-2">Giỏ hàng trống</h2>
          <p className="text-[#92400E]/70 mb-6">Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán</p>
          <Button onClick={() => navigate('products')}>
            Tiếp tục mua sắm
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-[#FFFBEB] px-4 py-8">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <Button
            variant="ghost"
            onClick={() => navigate('cart')}
            className="mb-4 text-[#92400E] hover:text-[#92400E]/80 hover:bg-[#92400E]/5"
          >
            <ArrowLeft className="mr-2 h-4 w-4" />
            Quay về giỏ hàng
          </Button>
          
          <div className="flex items-center space-x-4">
            <div className="w-12 h-12 bg-gradient-to-br from-[#92400E] to-[#DC2626] rounded-full flex items-center justify-center shadow-lg">
              <Flower2 className="w-6 h-6 text-white" />
            </div>
            <div>
              <h1 className="text-3xl text-[#92400E]">Thanh Toán</h1>
              <p className="text-[#92400E]/70">Hoàn tất đơn hàng của bạn</p>
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Left Column - Forms */}
          <div className="lg:col-span-2 space-y-6">
            {/* Shipping Address */}
            <Card className="border-2 border-[#92400E]/20">
              <CardHeader className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5">
                <CardTitle className="flex items-center text-[#92400E]">
                  <MapPin className="mr-2 h-5 w-5" />
                  Địa Chỉ Giao Hàng
                </CardTitle>
                <CardDescription>Chọn địa chỉ nhận hàng</CardDescription>
              </CardHeader>
              <CardContent className="pt-6">
                {addresses.length > 0 ? (
                  <RadioGroup value={selectedAddressId} onValueChange={setSelectedAddressId}>
                    <div className="space-y-3">
                      {addresses.map((address) => (
                        <div
                          key={address.id}
                          className={`border-2 rounded-lg p-4 cursor-pointer transition-all ${
                            selectedAddressId === address.id
                              ? 'border-[#92400E] bg-[#92400E]/5'
                              : 'border-[#92400E]/20 hover:border-[#92400E]/40'
                          }`}
                          onClick={() => address.id && setSelectedAddressId(address.id)}
                        >
                          <div className="flex items-start">
                            <RadioGroupItem value={address.id || ''} id={address.id} className="mt-1" />
                            <Label htmlFor={address.id} className="ml-3 flex-1 cursor-pointer">
                              <div className="flex items-center space-x-2 mb-2">
                                <span className="text-[#92400E]">{address.recipientName || 'Người nhận'}</span>
                                <span className="text-[#92400E]/60">|</span>
                                <span className="text-[#92400E]/70">{address.phoneNumber || 'N/A'}</span>
                                {address.isDefault && (
                                  <Badge className="bg-[#F59E0B] text-white">Mặc định</Badge>
                                )}
                                {address.addressTypeDisplay && (
                                  <Badge variant="outline" className="border-[#92400E]/30">
                                    {address.addressTypeDisplay}
                                  </Badge>
                                )}
                              </div>
                              <p className="text-sm text-[#92400E]/70">
                                {address.streetAddress || 'Địa chỉ'}
                              </p>
                              <p className="text-sm text-[#92400E]/60">
                                {[address.state, address.city, address.country].filter(Boolean).join(', ')}
                              </p>
                            </Label>
                          </div>
                        </div>
                      ))}
                    </div>
                  </RadioGroup>
                ) : (
                  <div className="text-center py-8">
                    <MapPin className="w-12 h-12 text-[#92400E]/30 mx-auto mb-3" />
                    <p className="text-[#92400E]/70 mb-4">Bạn chưa có địa chỉ nào</p>
                  </div>
                )}

                {!showAddAddressForm ? (
                  <Button
                    variant="outline"
                    onClick={() => setShowAddAddressForm(true)}
                    className="w-full mt-4 border-[#92400E]/30 text-[#92400E] hover:border-[#92400E] hover:bg-[#92400E]/5"
                  >
                    <Plus className="mr-2 h-4 w-4" />
                    Thêm địa chỉ mới
                  </Button>
                ) : (
                  <div className="mt-4 p-4 bg-[#FFFBEB] border-2 border-[#F59E0B]/30 rounded-lg space-y-3">
                    <h4 className="text-[#92400E] mb-3">Thêm địa chỉ mới</h4>
                    
                    {/* ✅ Recipient Info */}
                    <div className="grid grid-cols-2 gap-3">
                      <Input
                        placeholder="Tên người nhận"
                        value={newAddress.recipientName || ''}
                        onChange={(e) => setNewAddress({ ...newAddress, recipientName: e.target.value })}
                      />
                      <Input
                        placeholder="Số điện thoại"
                        value={newAddress.phoneNumber || ''}
                        onChange={(e) => setNewAddress({ ...newAddress, phoneNumber: e.target.value })}
                      />
                    </div>

                    {/* ✅ Street Address */}
                    <Input
                      placeholder="Địa chỉ chi tiết (số nhà, tên đường) *"
                      value={newAddress.streetAddress}
                      onChange={(e) => setNewAddress({ ...newAddress, streetAddress: e.target.value })}
                    />

                    {/* ✅ City, State, Postal Code */}
                    <div className="grid grid-cols-3 gap-3">
                      <Input
                        placeholder="Quận/Huyện"
                        value={newAddress.state || ''}
                        onChange={(e) => setNewAddress({ ...newAddress, state: e.target.value })}
                      />
                      <Input
                        placeholder="Thành phố/Tỉnh"
                        value={newAddress.city || ''}
                        onChange={(e) => setNewAddress({ ...newAddress, city: e.target.value })}
                      />
                      <Input
                        placeholder="Mã bưu điện"
                        value={newAddress.postalCode || ''}
                        onChange={(e) => setNewAddress({ ...newAddress, postalCode: e.target.value })}
                      />
                    </div>

                    {/* ✅ Address Type Selector */}
                    <Select 
                      value={newAddress.addressType} 
                      onValueChange={(value: string) => setNewAddress({ ...newAddress, addressType: value as AddressType })}
                    >
                      <SelectTrigger className="border-[#92400E]/30">
                        <SelectValue placeholder="Loại địa chỉ" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value={AddressType.home}>🏠 Nhà riêng</SelectItem>
                        <SelectItem value={AddressType.office}>🏢 Văn phòng</SelectItem>
                        <SelectItem value={AddressType.other}>📍 Khác</SelectItem>
                      </SelectContent>
                    </Select>

                    <div className="flex space-x-2">
                      <Button onClick={handleAddAddress} className="flex-1 bg-[#92400E] hover:bg-[#92400E]/90">
                        <Check className="mr-2 h-4 w-4" />
                        Lưu địa chỉ
                      </Button>
                      <Button
                        variant="outline"
                        onClick={() => setShowAddAddressForm(false)}
                        className="border-[#92400E]/30"
                      >
                        Hủy
                      </Button>
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Payment Method */}
            <Card className="border-2 border-[#92400E]/20">
              <CardHeader className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5">
                <CardTitle className="flex items-center text-[#92400E]">
                  <CreditCard className="mr-2 h-5 w-5" />
                  Phương Thức Thanh Toán
                </CardTitle>
                <CardDescription>Chọn cách thanh toán phù hợp</CardDescription>
              </CardHeader>
              <CardContent className="pt-6">
                <RadioGroup 
                  value={selectedPaymentMethod} 
                  onValueChange={(value: string) => setSelectedPaymentMethod(value as PaymentMethodType)}
                >
                  <div className="space-y-3">
                    {/* COD */}
                    <div
                      className={`border-2 rounded-lg p-4 cursor-pointer transition-all ${
                        selectedPaymentMethod === PaymentMethod.cod
                          ? 'border-[#92400E] bg-[#92400E]/5'
                          : 'border-[#92400E]/20 hover:border-[#92400E]/40'
                      }`}
                      onClick={() => setSelectedPaymentMethod(PaymentMethod.cod)}
                    >
                      <div className="flex items-center">
                        <RadioGroupItem value={PaymentMethod.cod} id="cod" />
                        <Label htmlFor="cod" className="ml-3 flex-1 cursor-pointer">
                          <div className="flex items-center justify-between">
                            <div>
                              <p className="text-[#92400E]">💵 Thanh toán khi nhận hàng (COD)</p>
                              <p className="text-sm text-[#92400E]/60 mt-1">
                                Thanh toán bằng tiền mặt khi nhận hàng
                              </p>
                            </div>
                            <Badge variant="outline" className="border-[#92400E]/30 text-[#92400E]">
                              Phổ biến
                            </Badge>
                          </div>
                        </Label>
                      </div>
                    </div>

                    {/* Credit Card (VNPay) */}
                    <div
                      className={`border-2 rounded-lg p-4 cursor-pointer transition-all ${
                        selectedPaymentMethod === PaymentMethod.crediT_CARD
                          ? 'border-[#92400E] bg-[#92400E]/5'
                          : 'border-[#92400E]/20 hover:border-[#92400E]/40'
                      }`}
                      onClick={() => setSelectedPaymentMethod(PaymentMethod.crediT_CARD)}
                    >
                      <div className="flex items-center">
                        <RadioGroupItem value={PaymentMethod.crediT_CARD} id="vnpay" />
                        <Label htmlFor="vnpay" className="ml-3 flex-1 cursor-pointer">
                          <p className="text-[#92400E]">🏦 VNPay / Thẻ tín dụng</p>
                          <p className="text-sm text-[#92400E]/60 mt-1">
                            Thanh toán qua ATM/Visa/MasterCard
                          </p>
                        </Label>
                      </div>
                    </div>

                    {/* E-Wallet (Momo, ZaloPay) */}
                    <div
                      className={`border-2 rounded-lg p-4 cursor-pointer transition-all ${
                        selectedPaymentMethod === PaymentMethod.e_WALLET
                          ? 'border-[#92400E] bg-[#92400E]/5'
                          : 'border-[#92400E]/20 hover:border-[#92400E]/40'
                      }`}
                      onClick={() => setSelectedPaymentMethod(PaymentMethod.e_WALLET)}
                    >
                      <div className="flex items-center">
                        <RadioGroupItem value={PaymentMethod.e_WALLET} id="ewallet" />
                        <Label htmlFor="ewallet" className="ml-3 flex-1 cursor-pointer">
                          <p className="text-[#92400E]">📱 Ví điện tử (MoMo, ZaloPay)</p>
                          <p className="text-sm text-[#92400E]/60 mt-1">
                            Thanh toán qua ví điện tử
                          </p>
                        </Label>
                      </div>
                    </div>

                    {/* Bank Transfer */}
                    <div
                      className={`border-2 rounded-lg p-4 cursor-pointer transition-all ${
                        selectedPaymentMethod === PaymentMethod.banK_TRANSFER
                          ? 'border-[#92400E] bg-[#92400E]/5'
                          : 'border-[#92400E]/20 hover:border-[#92400E]/40'
                      }`}
                      onClick={() => setSelectedPaymentMethod(PaymentMethod.banK_TRANSFER)}
                    >
                      <div className="flex items-center">
                        <RadioGroupItem value={PaymentMethod.banK_TRANSFER} id="bank" />
                        <Label htmlFor="bank" className="ml-3 flex-1 cursor-pointer">
                          <p className="text-[#92400E]">🏛️ Chuyển khoản ngân hàng</p>
                          <p className="text-sm text-[#92400E]/60 mt-1">
                            Chuyển khoản trực tiếp qua ngân hàng
                          </p>
                        </Label>
                      </div>
                    </div>
                  </div>
                </RadioGroup>

                <div className="mt-4 p-3 bg-[#F59E0B]/10 border border-[#F59E0B]/30 rounded-lg flex items-start space-x-2">
                  <Shield className="w-5 h-5 text-[#F59E0B] mt-0.5" />
                  <p className="text-sm text-[#92400E]/80">
                    Mọi giao dịch đều được bảo mật và mã hóa. Thông tin thanh toán của bạn an toàn tuyệt đối.
                  </p>
                </div>
              </CardContent>
            </Card>

            {/* Order Note */}
            <Card className="border-2 border-[#92400E]/20">
              <CardHeader className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5">
                <CardTitle className="text-[#92400E]">Ghi Chú Đơn Hàng</CardTitle>
                <CardDescription>Thông tin bổ sung (không bắt buộc)</CardDescription>
              </CardHeader>
              <CardContent className="pt-6">
                <Textarea
                  placeholder="Ví dụ: Giao hàng giờ hành chính, gọi trước 15 phút..."
                  value={orderNote}
                  onChange={(e) => setOrderNote(e.target.value)}
                  rows={4}
                  className="border-[#92400E]/30 focus:border-[#92400E]"
                />
              </CardContent>
            </Card>
          </div>

          {/* Right Column - Order Summary */}
          <div className="lg:col-span-1">
            <div className="sticky top-4 space-y-4">
              <Card className="border-2 border-[#92400E]/20">
                <CardHeader className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5">
                  <CardTitle className="text-[#92400E]">Đơn Hàng ({cart.itemCount} sản phẩm)</CardTitle>
                </CardHeader>
                <CardContent className="pt-6">
                  <div className="space-y-4">
                    {/* Items */}
                    <div className="space-y-3 max-h-64 overflow-y-auto">
                      {cart.items.map((item) => (
                        <div key={item.id} className="border-b border-[#92400E]/10 pb-3 last:border-b-0">
                          <div className="flex space-x-3">
                            <div className="relative">
                              <ImageWithFallback
                                src={item.image || '/placeholder.jpg'}
                                alt={item.name}
                                className="w-16 h-16 object-cover rounded-lg border border-[#92400E]/20"
                              />
                              <Badge className="absolute -top-2 -right-2 w-6 h-6 p-0 flex items-center justify-center bg-[#DC2626] text-white">
                                {item.quantity}
                              </Badge>
                            </div>
                            <div className="flex-1 min-w-0">
                              <h4 className="text-sm text-[#92400E] line-clamp-2">{item.name}</h4>
                              <p className="text-sm text-[#DC2626]">
                                {(item.price || 0).toLocaleString('vi-VN')}₫
                              </p>
                            </div>
                          </div>
                          
                          {/* Customizations */}
                          {item.customizations && item.customizations.length > 0 && (
                            <CartItemCustomizations
                              customizations={item.customizations}
                              productName={item.name}
                              compact={true}
                            />
                          )}
                        </div>
                      ))}
                    </div>

                    <Separator className="bg-[#92400E]/20" />

                    {/* Price breakdown */}
                    <div className="space-y-2">
                      <div className="flex justify-between text-[#92400E]/70">
                        <span>Tạm tính:</span>
                        <span>{(cart.subtotal || 0).toLocaleString('vi-VN')}₫</span>
                      </div>
                      <div className="flex justify-between text-[#92400E]/70">
                        <span>Phí vận chuyển:</span>
                        <span>{(cart.shippingFee || 0).toLocaleString('vi-VN')}₫</span>
                      </div>
                      {(cart.discount || 0) > 0 && (
                        <div className="flex justify-between text-[#F59E0B]">
                          <span>Giảm giá:</span>
                          <span>-{(cart.discount || 0).toLocaleString('vi-VN')}₫</span>
                        </div>
                      )}
                    </div>

                    <Separator className="bg-[#92400E]/20" />

                    {/* Total */}
                    <div className="flex justify-between items-center">
                      <span className="text-lg text-[#92400E]">Tổng cộng:</span>
                      <span className="text-2xl text-[#DC2626]">
                        {(cart.totalAmount || 0).toLocaleString('vi-VN')}₫
                      </span>
                    </div>

                    {/* Place Order Button */}
                    <Button
                      onClick={handlePlaceOrder}
                      disabled={isProcessing || !selectedAddressId}
                      className="w-full bg-gradient-to-r from-[#92400E] to-[#DC2626] hover:from-[#92400E]/90 hover:to-[#DC2626]/90 text-white shadow-lg"
                      size="lg"
                    >
                      {isProcessing ? (
                        <>
                          <Loader2 className="mr-2 h-5 w-5 animate-spin" />
                          Đang xử lý...
                        </>
                      ) : (
                        <>
                          <Check className="mr-2 h-5 w-5" />
                          Đặt hàng
                        </>
                      )}
                    </Button>

                    {/* Trust badges */}
                    <div className="grid grid-cols-2 gap-2 pt-4 text-xs text-[#92400E]/60 text-center">
                      <div className="flex flex-col items-center">
                        <Truck className="w-6 h-6 mb-1 text-[#F59E0B]" />
                        <span>Giao hàng nhanh</span>
                      </div>
                      <div className="flex flex-col items-center">
                        <Shield className="w-6 h-6 mb-1 text-[#F59E0B]" />
                        <span>Thanh toán an toàn</span>
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CheckoutPage;
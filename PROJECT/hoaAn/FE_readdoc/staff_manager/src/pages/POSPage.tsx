import React, { useState, useRef, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useI18n } from '../contexts/I18nContext';
import { CashPaymentModal } from '../components/modals/CashPaymentModal';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { Separator } from '../components/ui/separator';
import { 
  Camera, 
  Plus, 
  Minus, 
  Trash2, 
  ShoppingCart, 
  CreditCard,
  DollarSign,
  Scan
} from 'lucide-react';
import { mockProducts } from '../services/mockData';
import { Product, OrderItem } from '../types';

interface CartItem extends OrderItem {
  product: Product;
}

export const POSPage: React.FC = () => {
  const { t } = useI18n();
  const navigate = useNavigate();
  const videoRef = useRef<HTMLVideoElement>(null);
  const canvasRef = useRef<HTMLCanvasElement>(null);
  
  const [cart, setCart] = useState<CartItem[]>([]);
  const [barcode, setBarcode] = useState('');
  const [isCameraActive, setIsCameraActive] = useState(false);
  const [customerName, setCustomerName] = useState('');
  const [customerPhone, setCustomerPhone] = useState('');
  const [cashPaymentModalOpen, setCashPaymentModalOpen] = useState(false);

  const subtotal = cart.reduce((sum, item) => sum + item.total, 0);
  const tax = subtotal * 0.1; // 10% tax
  const total = subtotal + tax;

  const startCamera = async () => {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ 
        video: { facingMode: 'environment' } 
      });
      if (videoRef.current) {
        videoRef.current.srcObject = stream;
        setIsCameraActive(true);
      }
    } catch (err) {
      console.error('Error accessing camera:', err);
      alert('Camera access denied or not available');
    }
  };

  const stopCamera = () => {
    if (videoRef.current?.srcObject) {
      const stream = videoRef.current.srcObject as MediaStream;
      stream.getTracks().forEach(track => track.stop());
      videoRef.current.srcObject = null;
      setIsCameraActive(false);
    }
  };

  const handleBarcodeSearch = (barcodeValue: string) => {
    const product = mockProducts.find(p => p.barcode === barcodeValue);
    if (product) {
      addToCart(product);
      setBarcode('');
    } else {
      alert('Product not found with barcode: ' + barcodeValue);
    }
  };

  const addToCart = (product: Product, quantity: number = 1) => {
    setCart(prevCart => {
      const existingItem = prevCart.find(item => item.productId === product.id);
      
      if (existingItem) {
        return prevCart.map(item =>
          item.productId === product.id
            ? {
                ...item,
                quantity: item.quantity + quantity,
                total: (item.quantity + quantity) * item.price - item.discount,
              }
            : item
        );
      } else {
        const newItem: CartItem = {
          id: `cart-${Date.now()}`,
          productId: product.id,
          productName: product.name,
          quantity,
          price: product.price,
          discount: 0,
          total: product.price * quantity,
          product,
        };
        return [...prevCart, newItem];
      }
    });
  };

  const removeFromCart = (productId: string) => {
    setCart(prevCart => prevCart.filter(item => item.productId !== productId));
  };

  const updateQuantity = (productId: string, newQuantity: number) => {
    if (newQuantity <= 0) {
      removeFromCart(productId);
      return;
    }

    setCart(prevCart =>
      prevCart.map(item =>
        item.productId === productId
          ? {
              ...item,
              quantity: newQuantity,
              total: newQuantity * item.price - item.discount,
            }
          : item
      )
    );
  };

  const clearCart = () => {
    setCart([]);
    setCustomerName('');
    setCustomerPhone('');
  };

  const handleCashPayment = () => {
    if (cart.length === 0) {
      alert('Cart is empty');
      return;
    }
    setCashPaymentModalOpen(true);
  };

  const handleCardPayment = () => {
    if (cart.length === 0) {
      alert('Cart is empty');
      return;
    }

    const orderData = {
      orderNumber: `ORD-${Date.now()}`,
      items: cart,
      subtotal,
      tax,
      total,
      customer: {
        name: customerName,
        phone: customerPhone,
      },
    };

    // Navigate to card payment page with order data
    navigate('/card-payment', { state: { orderData } });
  };

  const handlePaymentComplete = () => {
    clearCart();
  };

  useEffect(() => {
    return () => {
      stopCamera();
    };
  }, []);

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 h-full">
      {/* Product Selection & Barcode Scanner */}
      <div className="lg:col-span-2 space-y-6">
        {/* Barcode Scanner */}
        <Card>
          <CardHeader>
            <CardTitle>{t('pos.scanBarcode')}</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex gap-2">
              <Input
                placeholder="Enter barcode manually"
                value={barcode}
                onChange={(e) => setBarcode(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    handleBarcodeSearch(barcode);
                  }
                }}
              />
              <Button onClick={() => handleBarcodeSearch(barcode)}>
                <Scan className="w-4 h-4 mr-2" />
                Search
              </Button>
            </div>

            {/* Camera Scanner */}
            <div className="space-y-2">
              <div className="flex gap-2">
                {!isCameraActive ? (
                  <Button onClick={startCamera} variant="outline">
                    <Camera className="w-4 h-4 mr-2" />
                    Start Camera
                  </Button>
                ) : (
                  <Button onClick={stopCamera} variant="outline">
                    Stop Camera
                  </Button>
                )}
              </div>
              
              {isCameraActive && (
                <div className="relative">
                  <video
                    ref={videoRef}
                    autoPlay
                    playsInline
                    className="w-full max-w-md h-64 bg-gray-200 rounded"
                  />
                  <canvas ref={canvasRef} className="hidden" />
                  <div className="absolute inset-0 border-2 border-primary rounded pointer-events-none">
                    <div className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-48 h-2 border-t-2 border-b-2 border-primary"></div>
                  </div>
                </div>
              )}
            </div>
          </CardContent>
        </Card>

        {/* Quick Product Selection */}
        <Card>
          <CardHeader>
            <CardTitle>Quick Add Products</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-2 md:grid-cols-3 gap-3">
              {mockProducts.map((product) => (
                <div
                  key={product.id}
                  className="p-3 border rounded-lg hover:bg-accent cursor-pointer transition-colors"
                  onClick={() => addToCart(product)}
                >
                  <div className="aspect-square bg-gray-200 rounded mb-2 overflow-hidden">
                    {product.image && (
                      <img
                        src={product.image}
                        alt={product.name}
                        className="w-full h-full object-cover"
                      />
                    )}
                  </div>
                  <h3 className="font-medium text-sm">{product.name}</h3>
                  <p className="text-primary font-semibold">${product.price}</p>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Cart & Checkout */}
      <div className="space-y-6">
        {/* Customer Info */}
        <Card>
          <CardHeader>
            <CardTitle>Customer Information</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            <Input
              placeholder="Customer name (optional)"
              value={customerName}
              onChange={(e) => setCustomerName(e.target.value)}
            />
            <Input
              placeholder="Phone number (optional)"
              value={customerPhone}
              onChange={(e) => setCustomerPhone(e.target.value)}
            />
          </CardContent>
        </Card>

        {/* Cart */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle className="flex items-center">
              <ShoppingCart className="w-5 h-5 mr-2" />
              Cart ({cart.length})
            </CardTitle>
            {cart.length > 0 && (
              <Button
                variant="outline"
                size="sm"
                onClick={clearCart}
              >
                Clear
              </Button>
            )}
          </CardHeader>
          <CardContent>
            {cart.length === 0 ? (
              <p className="text-muted-foreground text-center py-8">
                Cart is empty
              </p>
            ) : (
              <div className="space-y-3">
                {cart.map((item) => (
                  <div key={item.id} className="flex items-center justify-between">
                    <div className="flex-1">
                      <h4 className="font-medium text-sm">{item.productName}</h4>
                      <p className="text-xs text-muted-foreground">
                        ${item.price} each
                      </p>
                    </div>
                    <div className="flex items-center space-x-2">
                      <Button
                        size="sm"
                        variant="outline"
                        onClick={() => updateQuantity(item.productId, item.quantity - 1)}
                      >
                        <Minus className="w-3 h-3" />
                      </Button>
                      <span className="w-8 text-center text-sm">{item.quantity}</span>
                      <Button
                        size="sm"
                        variant="outline"
                        onClick={() => updateQuantity(item.productId, item.quantity + 1)}
                      >
                        <Plus className="w-3 h-3" />
                      </Button>
                      <Button
                        size="sm"
                        variant="outline"
                        onClick={() => removeFromCart(item.productId)}
                      >
                        <Trash2 className="w-3 h-3" />
                      </Button>
                    </div>
                    <div className="w-16 text-right">
                      <p className="font-semibold text-sm">${item.total.toFixed(2)}</p>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>

        {/* Order Summary */}
        {cart.length > 0 && (
          <Card>
            <CardHeader>
              <CardTitle>Order Summary</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="flex justify-between">
                <span>Subtotal:</span>
                <span>${subtotal.toFixed(2)}</span>
              </div>
              <div className="flex justify-between">
                <span>Tax (10%):</span>
                <span>${tax.toFixed(2)}</span>
              </div>
              <Separator />
              <div className="flex justify-between font-semibold">
                <span>Total:</span>
                <span>${total.toFixed(2)}</span>
              </div>
              
              <div className="grid grid-cols-2 gap-2 mt-4">
                <Button variant="outline" className="flex-1" onClick={handleCashPayment}>
                  <DollarSign className="w-4 h-4 mr-2" />
                  {t('pos.cash')}
                </Button>
                <Button className="flex-1" onClick={handleCardPayment}>
                  <CreditCard className="w-4 h-4 mr-2" />
                  {t('pos.card')}
                </Button>
              </div>
            </CardContent>
          </Card>
        )}
      </div>
      
      {/* Cash Payment Modal */}
      <CashPaymentModal
        open={cashPaymentModalOpen}
        onOpenChange={setCashPaymentModalOpen}
        orderTotal={total}
        onPaymentComplete={handlePaymentComplete}
      />
    </div>
  );
};
import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Button } from '../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { Separator } from '../components/ui/separator';
import { Progress } from '../components/ui/progress';
import { 
  CreditCard, 
  Smartphone, 
  QrCode, 
  CheckCircle, 
  ArrowLeft,
  Clock,
  Wifi,
  Shield
} from 'lucide-react';
import { toast } from 'sonner';

const paymentMethods = [
  { id: 'card', name: 'Credit/Debit Card', icon: CreditCard, description: 'Visa, Mastercard, AMEX' },
  { id: 'mobile', name: 'Mobile Payment', icon: Smartphone, description: 'Apple Pay, Google Pay, Samsung Pay' },
  { id: 'qr', name: 'QR Code Payment', icon: QrCode, description: 'Bank transfer via QR code' },
];

export const CardPaymentPage: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [selectedMethod, setSelectedMethod] = useState<string>('');
  const [paymentStatus, setPaymentStatus] = useState<'selecting' | 'processing' | 'success' | 'failed'>('selecting');
  const [countdown, setCountdown] = useState(30);
  const [progress, setProgress] = useState(0);

  // Get order data from location state (passed from POS)
  const orderData = location.state?.orderData || {
    total: 0,
    items: [],
    orderNumber: 'Unknown',
  };

  const generateQRCode = () => {
    // Generate mock QR code data
    const qrData = {
      merchantId: 'POS_STORE_001',
      amount: orderData.total,
      currency: 'USD',
      orderNumber: orderData.orderNumber,
      timestamp: new Date().toISOString(),
    };
    return `QR:${btoa(JSON.stringify(qrData))}`;
  };

  const qrCodeData = generateQRCode();

  useEffect(() => {
    if (selectedMethod === 'qr' && paymentStatus === 'processing') {
      const timer = setInterval(() => {
        setCountdown((prev) => {
          if (prev <= 1) {
            clearInterval(timer);
            setPaymentStatus('failed');
            toast.error('Payment timeout. Please try again.');
            return 0;
          }
          return prev - 1;
        });
        setProgress((prev) => prev + (100 / 30));
      }, 1000);

      return () => clearInterval(timer);
    }
  }, [selectedMethod, paymentStatus]);

  const handleMethodSelect = (methodId: string) => {
    setSelectedMethod(methodId);
    setPaymentStatus('processing');
    setCountdown(30);
    setProgress(0);

    // Simulate payment processing
    if (methodId === 'qr') {
      // QR code payment - wait for scan
      toast.info('QR code generated. Waiting for payment...');
    } else {
      // Card/Mobile payment - simulate faster processing
      setTimeout(() => {
        setPaymentStatus('success');
        toast.success('Payment successful!');
        setTimeout(() => {
          navigate('/pos', { replace: true });
        }, 2000);
      }, 3000);
    }
  };

  const handleQRPaymentComplete = () => {
    setPaymentStatus('success');
    toast.success('QR payment received successfully!');
    setTimeout(() => {
      navigate('/pos', { replace: true });
    }, 2000);
  };

  const handleCancel = () => {
    navigate('/pos', { replace: true });
  };

  const formatCurrency = (amount: number) => `$${amount.toFixed(2)}`;

  return (
    <div className="min-h-screen bg-background p-6">
      <div className="max-w-6xl mx-auto">
        {/* Header */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-4">
            <Button variant="outline" size="sm" onClick={handleCancel}>
              <ArrowLeft className="h-4 w-4 mr-2" />
              Back to POS
            </Button>
            <div>
              <h1 className="text-2xl font-semibold">Card Payment</h1>
              <p className="text-muted-foreground">
                Order #{orderData.orderNumber} - {formatCurrency(orderData.total)}
              </p>
            </div>
          </div>
          <Badge variant="secondary" className="px-3 py-1">
            <Shield className="h-3 w-3 mr-1" />
            Secure Payment
          </Badge>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Payment Methods */}
          <div className="lg:col-span-2 space-y-4">
            <Card>
              <CardHeader>
                <CardTitle>Choose Payment Method</CardTitle>
              </CardHeader>
              <CardContent>
                {paymentStatus === 'selecting' ? (
                  <div className="grid gap-3">
                    {paymentMethods.map((method) => {
                      const IconComponent = method.icon;
                      return (
                        <Button
                          key={method.id}
                          variant="outline"
                          className="h-auto p-4 justify-start"
                          onClick={() => handleMethodSelect(method.id)}
                        >
                          <div className="flex items-center gap-4 w-full">
                            <div className="p-2 bg-primary/10 rounded-lg">
                              <IconComponent className="h-6 w-6 text-primary" />
                            </div>
                            <div className="text-left flex-1">
                              <p className="font-medium">{method.name}</p>
                              <p className="text-sm text-muted-foreground">{method.description}</p>
                            </div>
                          </div>
                        </Button>
                      );
                    })}
                  </div>
                ) : paymentStatus === 'processing' ? (
                  <div className="text-center py-8">
                    <div className="mb-4">
                      {selectedMethod === 'qr' ? (
                        <QrCode className="h-16 w-16 text-primary mx-auto mb-4" />
                      ) : selectedMethod === 'mobile' ? (
                        <Smartphone className="h-16 w-16 text-primary mx-auto mb-4" />
                      ) : (
                        <CreditCard className="h-16 w-16 text-primary mx-auto mb-4" />
                      )}
                    </div>
                    <h3 className="text-lg font-semibold mb-2">Processing Payment...</h3>
                    <p className="text-muted-foreground mb-4">
                      {selectedMethod === 'qr' 
                        ? 'Scan the QR code with your banking app' 
                        : selectedMethod === 'mobile'
                        ? 'Hold your device near the payment terminal'
                        : 'Insert or tap your card on the terminal'
                      }
                    </p>
                    <Progress value={progress} className="w-full max-w-xs mx-auto" />
                    {selectedMethod === 'qr' && (
                      <div className="mt-4">
                        <p className="text-sm text-muted-foreground flex items-center justify-center gap-1">
                          <Clock className="h-3 w-3" />
                          {countdown}s remaining
                        </p>
                        <Button 
                          variant="outline" 
                          size="sm" 
                          className="mt-2"
                          onClick={handleQRPaymentComplete}
                        >
                          Simulate Payment Complete
                        </Button>
                      </div>
                    )}
                  </div>
                ) : paymentStatus === 'success' ? (
                  <div className="text-center py-8">
                    <CheckCircle className="h-16 w-16 text-green-600 mx-auto mb-4" />
                    <h3 className="text-lg font-semibold text-green-700 mb-2">Payment Successful!</h3>
                    <p className="text-muted-foreground">
                      Redirecting back to POS...
                    </p>
                  </div>
                ) : (
                  <div className="text-center py-8">
                    <div className="h-16 w-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
                      <Clock className="h-8 w-8 text-red-600" />
                    </div>
                    <h3 className="text-lg font-semibold text-red-700 mb-2">Payment Failed</h3>
                    <p className="text-muted-foreground mb-4">
                      The payment timed out or was declined.
                    </p>
                    <Button onClick={() => setPaymentStatus('selecting')}>
                      Try Again
                    </Button>
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Connection Status */}
            <Card>
              <CardContent className="pt-6">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <Wifi className="h-4 w-4 text-green-600" />
                    <span className="text-sm">Payment Terminal Connected</span>
                  </div>
                  <Badge variant="default" className="bg-green-600">
                    Online
                  </Badge>
                </div>
              </CardContent>
            </Card>
          </div>

          {/* QR Code & Order Summary */}
          <div className="space-y-4">
            {/* QR Code */}
            {selectedMethod === 'qr' && paymentStatus === 'processing' && (
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <QrCode className="h-5 w-5" />
                    QR Payment Code
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="text-center">
                    {/* Mock QR Code */}
                    <div className="w-48 h-48 bg-white border-2 border-gray-300 mx-auto mb-4 flex items-center justify-center">
                      <div className="grid grid-cols-8 gap-1">
                        {Array.from({ length: 64 }).map((_, i) => (
                          <div
                            key={i}
                            className={`w-2 h-2 ${
                              Math.random() > 0.5 ? 'bg-black' : 'bg-white'
                            }`}
                          />
                        ))}
                      </div>
                    </div>
                    <p className="text-xs text-muted-foreground font-mono break-all">
                      {qrCodeData.substring(0, 32)}...
                    </p>
                    <div className="mt-4 p-3 bg-blue-50 border border-blue-200 rounded-lg">
                      <p className="text-sm text-blue-800">
                        Open your banking app and scan this QR code to complete payment
                      </p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Order Summary */}
            <Card>
              <CardHeader>
                <CardTitle>Order Summary</CardTitle>
              </CardHeader>
              <CardContent className="space-y-3">
                <div className="space-y-2">
                  {orderData.items?.slice(0, 3).map((item: any, index: number) => (
                    <div key={index} className="flex justify-between text-sm">
                      <span>{item.productName} x{item.quantity}</span>
                      <span>${item.total.toFixed(2)}</span>
                    </div>
                  ))}
                  {orderData.items?.length > 3 && (
                    <div className="text-sm text-muted-foreground">
                      +{orderData.items.length - 3} more items
                    </div>
                  )}
                </div>
                <Separator />
                <div className="flex justify-between font-semibold">
                  <span>Total:</span>
                  <span>{formatCurrency(orderData.total)}</span>
                </div>
              </CardContent>
            </Card>

            {/* Security Notice */}
            <Card>
              <CardContent className="pt-6">
                <div className="flex items-start gap-2">
                  <Shield className="h-4 w-4 text-green-600 mt-0.5" />
                  <div>
                    <p className="text-sm font-medium text-green-800">
                      Secure Payment
                    </p>
                    <p className="text-xs text-green-600">
                      All transactions are encrypted and secure
                    </p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
};
/**
 * PaymentPage - Trang xử lý thanh toán
 */

import { useState, useEffect } from 'react';
import { useHybridNavigate } from '../../lib/hooks/useHybridNavigate';
import { paymentService } from '../../lib/services/paymentService';
import { orderService } from '../../lib/services/orderService';
import { PaymentMethod } from '../../lib/api/types';
import { Button } from '../../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../../components/ui/card';
import { Badge } from '../../components/ui/badge';
import { Separator } from '../../components/ui/separator';
import { Alert, AlertDescription } from '../../components/ui/alert';
import { 
  CreditCard, 
  Loader2, 
  ArrowLeft,
  AlertCircle,
  CheckCircle2,
  Flower2,
  Shield,
  Clock
} from 'lucide-react';
import { toast } from 'sonner@2.0.3';

interface PaymentPageProps {
  orderData: {
    orderId: string;
    orderNumber: string;
    amount: number;
    paymentMethod: PaymentMethod;
  };
  onNavigate: (step: 'success' | 'failed', data?: any) => void;
}

export const PaymentPage = ({ orderData, onNavigate }: PaymentPageProps) => {
  const navigate = useHybridNavigate();
  const [isProcessing, setIsProcessing] = useState(false);
  const [paymentStatus, setPaymentStatus] = useState<'pending' | 'processing' | 'success' | 'failed'>('pending');
  const [countdown, setCountdown] = useState(60);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    // Auto process payment
    processPayment();
  }, []);

  useEffect(() => {
    // Countdown timer
    if (paymentStatus === 'processing' && countdown > 0) {
      const timer = setTimeout(() => setCountdown(countdown - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [paymentStatus, countdown]);

  const processPayment = async () => {
    try {
      setIsProcessing(true);
      setPaymentStatus('processing');
      setError(null);

      // Create payment
      const paymentResponse = await paymentService.createPayment({
        orderId: orderData.orderId,
        amount: orderData.amount,
        paymentMethod: orderData.paymentMethod,
        returnUrl: `${window.location.origin}/checkout?payment=success&orderId=${orderData.orderId}`,
        cancelUrl: `${window.location.origin}/checkout?payment=failed&orderId=${orderData.orderId}`,
      });

      if (paymentResponse.paymentUrl) {
        // Redirect to payment gateway
        toast.success('Đang chuyển đến cổng thanh toán...');
        setTimeout(() => {
          window.location.href = paymentResponse.paymentUrl!;
        }, 1000);
      } else {
        // Payment processed successfully without redirect
        setPaymentStatus('success');
        toast.success('Thanh toán thành công!');
        setTimeout(() => {
          onNavigate('success', {
            orderId: orderData.orderId,
            orderNumber: orderData.orderNumber,
          });
        }, 2000);
      }
    } catch (err: any) {
      console.error('Payment processing error:', err);
      const errorMsg = err.message || 'Không thể xử lý thanh toán';
      setError(errorMsg);
      setPaymentStatus('failed');
      toast.error(errorMsg);
    } finally {
      setIsProcessing(false);
    }
  };

  const handleRetry = () => {
    setPaymentStatus('pending');
    setCountdown(60);
    processPayment();
  };

  const handleCancel = () => {
    onNavigate('failed', {
      orderId: orderData.orderId,
      orderNumber: orderData.orderNumber,
      reason: 'Người dùng hủy thanh toán',
    });
  };

  const getPaymentMethodName = (method: PaymentMethod): string => {
    const methods: Record<PaymentMethod, string> = {
      COD: 'Thanh toán khi nhận hàng',
      VNPay: 'VNPay',
      Momo: 'Ví MoMo',
      ZaloPay: 'ZaloPay',
      BankTransfer: 'Chuyển khoản ngân hàng',
    };
    return methods[method] || method;
  };

  return (
    <div className="min-h-screen bg-[#FFFBEB] px-4 py-12">
      <div className="max-w-2xl mx-auto">
        {/* Header */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-gradient-to-br from-[#92400E] to-[#DC2626] rounded-full mb-4 shadow-lg">
            <CreditCard className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-3xl text-[#92400E] mb-2">Thanh Toán</h1>
          <p className="text-[#92400E]/70">
            Đơn hàng: <span className="text-[#DC2626]">{orderData.orderNumber}</span>
          </p>
        </div>

        {/* Payment Status Card */}
        <Card className="border-2 border-[#92400E]/20 mb-6">
          <CardHeader className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5">
            <CardTitle className="flex items-center justify-between text-[#92400E]">
              <span>Thông Tin Thanh Toán</span>
              {paymentStatus === 'processing' && (
                <Badge className="bg-[#F59E0B] text-white">
                  <Clock className="w-3 h-3 mr-1" />
                  {countdown}s
                </Badge>
              )}
            </CardTitle>
          </CardHeader>
          <CardContent className="pt-6">
            <div className="space-y-4">
              {/* Payment Method */}
              <div className="flex items-center justify-between">
                <span className="text-[#92400E]/70">Phương thức thanh toán:</span>
                <span className="text-[#92400E]">{getPaymentMethodName(orderData.paymentMethod)}</span>
              </div>

              <Separator className="bg-[#92400E]/20" />

              {/* Amount */}
              <div className="flex items-center justify-between">
                <span className="text-[#92400E]/70">Số tiền:</span>
                <span className="text-xl text-[#DC2626]">
                  {orderData.amount.toLocaleString('vi-VN')}₫
                </span>
              </div>

              <Separator className="bg-[#92400E]/20" />

              {/* Status */}
              <div className="mt-6">
                {paymentStatus === 'pending' && (
                  <div className="text-center py-6">
                    <Loader2 className="w-12 h-12 animate-spin text-[#92400E] mx-auto mb-4" />
                    <p className="text-[#92400E]">Đang chuẩn bị thanh toán...</p>
                  </div>
                )}

                {paymentStatus === 'processing' && (
                  <div className="text-center py-6">
                    <Loader2 className="w-12 h-12 animate-spin text-[#F59E0B] mx-auto mb-4" />
                    <p className="text-[#92400E] mb-2">Đang xử lý thanh toán...</p>
                    <p className="text-sm text-[#92400E]/60">
                      Vui lòng không đóng trang này
                    </p>
                  </div>
                )}

                {paymentStatus === 'success' && (
                  <Alert className="border-[#92400E] bg-[#92400E]/5">
                    <CheckCircle2 className="h-5 w-5 text-[#92400E]" />
                    <AlertDescription className="text-[#92400E]">
                      Thanh toán thành công! Đang chuyển hướng...
                    </AlertDescription>
                  </Alert>
                )}

                {paymentStatus === 'failed' && error && (
                  <Alert variant="destructive" className="border-[#DC2626] bg-[#DC2626]/10">
                    <AlertCircle className="h-5 w-5" />
                    <AlertDescription>{error}</AlertDescription>
                  </Alert>
                )}
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Security Notice */}
        <Card className="border-2 border-[#F59E0B]/20 mb-6">
          <CardContent className="pt-6">
            <div className="flex items-start space-x-3">
              <Shield className="w-6 h-6 text-[#F59E0B] mt-0.5 flex-shrink-0" />
              <div className="flex-1">
                <h4 className="text-[#92400E] mb-2">Bảo Mật Thanh Toán</h4>
                <p className="text-sm text-[#92400E]/70">
                  Mọi giao dịch đều được mã hóa và bảo mật theo tiêu chuẩn quốc tế PCI DSS. 
                  Thông tin thanh toán của bạn hoàn toàn an toàn và không được lưu trữ trên hệ thống của chúng tôi.
                </p>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Action Buttons */}
        <div className="space-y-3">
          {paymentStatus === 'failed' && (
            <>
              <Button
                onClick={handleRetry}
                className="w-full bg-gradient-to-r from-[#92400E] to-[#DC2626] hover:from-[#92400E]/90 hover:to-[#DC2626]/90 text-white"
              >
                Thử Lại
              </Button>
              <Button
                onClick={handleCancel}
                variant="outline"
                className="w-full border-[#92400E]/30 text-[#92400E] hover:border-[#92400E] hover:bg-[#92400E]/5"
              >
                Hủy và Quay Lại
              </Button>
            </>
          )}

          {(paymentStatus === 'pending' || paymentStatus === 'processing') && (
            <Button
              onClick={handleCancel}
              variant="outline"
              className="w-full border-[#92400E]/30 text-[#92400E] hover:border-[#92400E] hover:bg-[#92400E]/5"
            >
              <ArrowLeft className="mr-2 h-4 w-4" />
              Hủy Thanh Toán
            </Button>
          )}
        </div>

        {/* Footer */}
        <div className="text-center mt-12 pt-8 border-t-2 border-[#92400E]/10">
          <Flower2 className="w-10 h-10 text-[#92400E]/20 mx-auto mb-3" />
          <p className="text-sm text-[#92400E]/60">
            Cần hỗ trợ? Liên hệ hotline: <span className="text-[#DC2626]">1900 1234</span>
          </p>
        </div>
      </div>
    </div>
  );
};

export default PaymentPage;

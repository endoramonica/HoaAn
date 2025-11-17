/**
 * OrderFailedPage - Trang thông báo thanh toán thất bại
 */

import { useState, useEffect } from 'react';
import { Button } from '../../components/ui/button';
import { useHybridNavigate } from '../../lib/hooks/useHybridNavigate';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Alert, AlertDescription } from '../../components/ui/alert';
import {
  XCircle,
  AlertTriangle,
  RefreshCw,
  Home,
  CreditCard,
  Phone,
  Mail,
  Flower2,
} from 'lucide-react';

interface OrderFailedPageProps {
  errorData?: {
    orderId?: string;
    orderNumber?: string;
    reason?: string;
    errorCode?: string;
  };
}

export const OrderFailedPage = ({ errorData }: OrderFailedPageProps) => {
  const navigate = useHybridNavigate();
  const [errorReason, setErrorReason] = useState('');

  useEffect(() => {
    if (errorData?.reason) {
      setErrorReason(errorData.reason);
    } else {
      const params = new URLSearchParams(window.location.search);
      const reason = params.get('reason') || 'Thanh toán không thành công';
      setErrorReason(reason);
    }
  }, [errorData]);

  const handleRetry = () => {
    if (errorData?.orderId) {
      navigate('checkout');
    } else {
      navigate('cart');
    }
  };

  return (
    <div className="min-h-screen bg-[#FFFBEB] px-4 py-12">
      <div className="max-w-2xl mx-auto">
        {/* Error Header */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-20 h-20 bg-gradient-to-br from-[#DC2626] to-[#92400E] rounded-full mb-4 shadow-lg">
            <XCircle className="w-10 h-10 text-white" />
          </div>
          <h1 className="text-3xl text-[#DC2626] mb-2">Thanh Toán Thất Bại</h1>
          <p className="text-[#92400E]/70">
            Đã có lỗi xảy ra trong quá trình thanh toán
          </p>
        </div>

        {/* Error Details */}
        <Alert variant="destructive" className="mb-6 border-[#DC2626] bg-[#DC2626]/10">
          <AlertTriangle className="h-4 w-4" />
          <AlertDescription>
            {errorReason || 'Giao dịch không thành công. Vui lòng thử lại hoặc liên hệ hỗ trợ.'}
          </AlertDescription>
        </Alert>

        {/* Order Number */}
        {errorData?.orderNumber && (
          <Card className="mb-6 border-2 border-[#92400E]/20">
            <CardContent className="pt-6">
              <div className="text-center">
                <p className="text-sm text-[#92400E]/70 mb-2">Mã đơn hàng</p>
                <p className="text-xl text-[#92400E]">{errorData.orderNumber}</p>
                <p className="text-xs text-[#92400E]/60 mt-2">
                  Đơn hàng của bạn vẫn được lưu và chờ thanh toán
                </p>
              </div>
            </CardContent>
          </Card>
        )}

        {/* Possible Reasons */}
        <Card className="mb-6 border-2 border-[#92400E]/20">
          <CardHeader className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5">
            <CardTitle className="text-[#92400E]">Nguyên Nhân Có Thể</CardTitle>
          </CardHeader>
          <CardContent className="pt-6">
            <ul className="space-y-3 text-[#92400E]/80">
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Số dư tài khoản không đủ</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Thông tin thẻ không chính xác</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Thẻ/tài khoản đã hết hạn hoặc bị khóa</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Vượt quá hạn mức giao dịch</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Mất kết nối trong quá trình thanh toán</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Hủy giao dịch từ cổng thanh toán</span>
              </li>
            </ul>
          </CardContent>
        </Card>

        {/* Contact Support */}
        <Card className="mb-6 border-2 border-[#F59E0B]">
          <CardHeader className="bg-[#F59E0B]/10">
            <CardTitle className="text-[#92400E]">Cần Hỗ Trợ?</CardTitle>
          </CardHeader>
          <CardContent className="pt-6">
            <div className="space-y-3">
              <div className="flex items-center space-x-3 p-3 bg-white rounded-lg border border-[#92400E]/20">
                <Phone className="w-5 h-5 text-[#DC2626]" />
                <div>
                  <p className="text-xs text-[#92400E]/70">Hotline</p>
                  <p className="text-[#92400E]">1900 xxxx</p>
                </div>
              </div>

              <div className="flex items-center space-x-3 p-3 bg-white rounded-lg border border-[#92400E]/20">
                <Mail className="w-5 h-5 text-[#DC2626]" />
                <div>
                  <p className="text-xs text-[#92400E]/70">Email</p>
                  <p className="text-[#92400E]">support@docungvietnam.com</p>
                </div>
              </div>
            </div>

            <p className="text-xs text-[#92400E]/60 mt-4 text-center">
              Thời gian hỗ trợ: 8:00 - 22:00 hàng ngày
            </p>
          </CardContent>
        </Card>

        {/* Action Buttons */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Button
            onClick={handleRetry}
            className="bg-gradient-to-r from-[#92400E] to-[#DC2626] hover:from-[#92400E]/90 hover:to-[#DC2626]/90 text-white"
          >
            <RefreshCw className="mr-2 h-4 w-4" />
            Thử lại thanh toán
          </Button>
          <Button
            onClick={() => navigate('home')}
            variant="outline"
            className="border-[#92400E] text-[#92400E] hover:bg-[#92400E]/5"
          >
            <Home className="mr-2 h-4 w-4" />
            Về trang chủ
          </Button>
        </div>

        {/* Alternative Payment Methods */}
        <Card className="mt-6 border-2 border-[#92400E]/20">
          <CardHeader className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5">
            <CardTitle className="text-[#92400E] flex items-center">
              <CreditCard className="mr-2 h-5 w-5" />
              Phương Thức Thanh Toán Khác
            </CardTitle>
          </CardHeader>
          <CardContent className="pt-6">
            <p className="text-[#92400E]/70 mb-4">
              Bạn có thể chọn các phương thức thanh toán sau:
            </p>
            <div className="grid grid-cols-2 gap-3">
              <div className="p-3 bg-white border border-[#92400E]/20 rounded-lg text-center">
                <span className="text-2xl mb-1 block">💵</span>
                <p className="text-sm text-[#92400E]">COD</p>
              </div>
              <div className="p-3 bg-white border border-[#92400E]/20 rounded-lg text-center">
                <span className="text-2xl mb-1 block">🏛️</span>
                <p className="text-sm text-[#92400E]">Chuyển khoản</p>
              </div>
              <div className="p-3 bg-white border border-[#92400E]/20 rounded-lg text-center">
                <span className="text-2xl mb-1 block">📱</span>
                <p className="text-sm text-[#92400E]">Ví MoMo</p>
              </div>
              <div className="p-3 bg-white border border-[#92400E]/20 rounded-lg text-center">
                <span className="text-2xl mb-1 block">💳</span>
                <p className="text-sm text-[#92400E]">ZaloPay</p>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Footer */}
        <div className="text-center mt-12 pt-8 border-t-2 border-[#92400E]/10">
          <Flower2 className="w-12 h-12 text-[#92400E]/20 mx-auto mb-4" />
          <p className="text-[#92400E]/60">
            Chúng tôi luôn sẵn sàng hỗ trợ bạn. Xin lỗi vì sự bất tiện này! 🙏
          </p>
        </div>
      </div>
    </div>
  );
};

export default OrderFailedPage;

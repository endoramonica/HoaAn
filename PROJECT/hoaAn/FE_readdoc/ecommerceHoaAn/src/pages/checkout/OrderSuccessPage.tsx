/**
 * OrderSuccessPage - Trang thông báo đặt hàng thành công
 */

import { useEffect, useState } from 'react';
import { orderService } from '../../lib/services/orderService';
import { useHybridNavigate } from '../../lib/hooks/useHybridNavigate';
import { paymentService } from '../../lib/services/paymentService';
import { OrderDto } from '../../lib/api/types';
import { Button } from '../../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Badge } from '../../components/ui/badge';
import { Separator } from '../../components/ui/separator';
import {
  CheckCircle2,
  Package,
  MapPin,
  CreditCard,
  Truck,
  Copy,
  Home,
  Loader2,
  Flower2,
} from 'lucide-react';
import { toast } from 'sonner@2.0.3';

interface OrderSuccessPageProps {
  orderData?: { 
    orderId: string; 
    orderNumber: string;
    paymentMethod?: string;
  };
}

export const OrderSuccessPage = ({ orderData }: OrderSuccessPageProps) => {
  const navigate = useHybridNavigate();
  const [order, setOrder] = useState<OrderDto | null>(null);
  const [bankingInfo, setBankingInfo] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadOrderDetails();
  }, []);

  const loadOrderDetails = async () => {
    try {
      setIsLoading(true);

      if (!orderData?.orderId) {
        const pendingOrderId = sessionStorage.getItem('pendingOrderId');
        if (pendingOrderId) {
          const orderDetails = await orderService.getOrderById(pendingOrderId);
          setOrder(orderDetails);
          sessionStorage.removeItem('pendingOrderId');
          sessionStorage.removeItem('pendingOrderNumber');
        }
      } else {
        const orderDetails = await orderService.getOrderById(orderData.orderId);
        setOrder(orderDetails);

        if (orderData.paymentMethod === 'BankTransfer') {
          const banking = await paymentService.getBankingInfo();
          setBankingInfo(banking);
        }
      }
    } catch (error) {
      console.error('Load order details error:', error);
      toast.error('Không thể tải thông tin đơn hàng');
    } finally {
      setIsLoading(false);
    }
  };

  const handleCopyOrderNumber = () => {
    if (order) {
      navigator.clipboard.writeText(order.orderNumber);
      toast.success('Đã sao chép mã đơn hàng');
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-[#FFFBEB] flex items-center justify-center">
        <div className="text-center">
          <Loader2 className="w-12 h-12 animate-spin text-[#92400E] mx-auto mb-4" />
          <p className="text-[#92400E]/70">Đang tải thông tin đơn hàng...</p>
        </div>
      </div>
    );
  }

  if (!order) {
    return (
      <div className="min-h-screen bg-[#FFFBEB] flex items-center justify-center px-4">
        <div className="text-center max-w-md">
          <Package className="w-16 h-16 text-[#92400E]/30 mx-auto mb-4" />
          <h2 className="text-2xl text-[#92400E] mb-2">Không tìm thấy đơn hàng</h2>
          <p className="text-[#92400E]/70 mb-6">Vui lòng kiểm tra lại thông tin hoặc liên hệ hỗ trợ</p>
          <Button onClick={() => navigate('home')}>
            Về trang chủ
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-[#FFFBEB] px-4 py-12">
      <div className="max-w-3xl mx-auto">
        {/* Success Header */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-20 h-20 bg-gradient-to-br from-[#92400E] to-[#F59E0B] rounded-full mb-4 shadow-lg">
            <CheckCircle2 className="w-10 h-10 text-white" />
          </div>
          <h1 className="text-3xl text-[#92400E] mb-2">Đặt Hàng Thành Công!</h1>
          <p className="text-[#92400E]/70">
            Cảm ơn bạn đã tin tưởng và mua sắm tại cửa hàng đồ cúng truyền thống
          </p>
        </div>

        {/* Order Number */}
        <Card className="mb-6 border-2 border-[#92400E]/20">
          <CardContent className="pt-6">
            <div className="text-center">
              <p className="text-sm text-[#92400E]/70 mb-2">Mã đơn hàng</p>
              <div className="flex items-center justify-center space-x-2">
                <span className="text-2xl text-[#DC2626]">{order.orderNumber}</span>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={handleCopyOrderNumber}
                  className="text-[#92400E] hover:text-[#92400E]/80 hover:bg-[#92400E]/5"
                >
                  <Copy className="w-4 h-4" />
                </Button>
              </div>
              <p className="text-xs text-[#92400E]/60 mt-2">
                Vui lòng lưu lại mã đơn hàng để tra cứu
              </p>
            </div>
          </CardContent>
        </Card>

        {/* Bank Transfer Info */}
        {bankingInfo && (
          <Card className="mb-6 border-2 border-[#F59E0B]">
            <CardHeader className="bg-[#F59E0B]/10">
              <CardTitle className="text-[#92400E] flex items-center">
                <CreditCard className="mr-2 h-5 w-5" />
                Thông Tin Chuyển Khoản
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-6">
              <div className="space-y-3 bg-white p-4 rounded-lg border border-[#92400E]/20">
                <div>
                  <p className="text-sm text-[#92400E]/70">Ngân hàng</p>
                  <p className="text-[#92400E]">{bankingInfo.bankName}</p>
                </div>
                <div>
                  <p className="text-sm text-[#92400E]/70">Số tài khoản</p>
                  <p className="text-lg text-[#DC2626]">{bankingInfo.accountNumber}</p>
                </div>
                <div>
                  <p className="text-sm text-[#92400E]/70">Chủ tài khoản</p>
                  <p className="text-[#92400E]">{bankingInfo.accountName}</p>
                </div>
                <div>
                  <p className="text-sm text-[#92400E]/70">Số tiền</p>
                  <p className="text-xl text-[#DC2626]">
                    {order.total.toLocaleString('vi-VN')}₫
                  </p>
                </div>
                <div>
                  <p className="text-sm text-[#92400E]/70">Nội dung chuyển khoản</p>
                  <p className="text-[#92400E] font-mono bg-[#FFFBEB] px-3 py-2 rounded border border-[#92400E]/20">
                    {order.orderNumber} {order.shippingAddress.fullName}
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>
        )}

        {/* Order Summary */}
        <Card className="mb-6 border-2 border-[#92400E]/20">
          <CardHeader className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5">
            <CardTitle className="text-[#92400E] flex items-center">
              <Package className="mr-2 h-5 w-5" />
              Thông Tin Đơn Hàng
            </CardTitle>
          </CardHeader>
          <CardContent className="pt-6">
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <span className="text-[#92400E]/70">Trạng thái</span>
                <Badge className="bg-[#F59E0B] text-white">
                  {order.status === 'Pending' ? 'Chờ xác nhận' : order.status}
                </Badge>
              </div>

              <Separator className="bg-[#92400E]/20" />

              <div>
                <div className="flex items-start space-x-2 mb-2">
                  <MapPin className="w-5 h-5 text-[#92400E] mt-0.5" />
                  <div className="flex-1">
                    <p className="text-sm text-[#92400E]/70 mb-1">Địa chỉ giao hàng</p>
                    <p className="text-[#92400E]">{order.shippingAddress.fullName}</p>
                    <p className="text-[#92400E]/80">{order.shippingAddress.phoneNumber}</p>
                    <p className="text-sm text-[#92400E]/70 mt-1">
                      {order.shippingAddress.addressLine1}
                    </p>
                    <p className="text-sm text-[#92400E]/70">
                      {order.shippingAddress.ward}, {order.shippingAddress.district}, {order.shippingAddress.province}
                    </p>
                  </div>
                </div>
              </div>

              <Separator className="bg-[#92400E]/20" />

              <div className="flex items-center justify-between">
                <span className="text-[#92400E]/70">Tổng cộng</span>
                <span className="text-xl text-[#DC2626]">
                  {order.total.toLocaleString('vi-VN')}₫
                </span>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Action Buttons */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <Button
            onClick={() => navigate('profile')}
            variant="outline"
            className="border-[#92400E] text-[#92400E] hover:bg-[#92400E]/5"
          >
            <Package className="mr-2 h-4 w-4" />
            Xem đơn hàng của tôi
          </Button>
          <Button
            onClick={() => navigate('home')}
            className="bg-gradient-to-r from-[#92400E] to-[#DC2626] hover:from-[#92400E]/90 hover:to-[#DC2626]/90 text-white"
          >
            <Home className="mr-2 h-4 w-4" />
            Về trang chủ
          </Button>
        </div>

        {/* Footer */}
        <div className="text-center mt-12 pt-8 border-t-2 border-[#92400E]/10">
          <Flower2 className="w-12 h-12 text-[#92400E]/20 mx-auto mb-4" />
          <p className="text-[#92400E]/60">
            Cảm ơn bạn đã lựa chọn chúng tôi. Chúc gia đình bạn sức khỏe và thịnh vượng! 🙏
          </p>
        </div>
      </div>
    </div>
  );
};

export default OrderSuccessPage;

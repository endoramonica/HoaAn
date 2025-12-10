import { useParams, useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Loader2, CheckCircle, AlertCircle, ArrowLeft } from 'lucide-react';
import { useBookingOrder } from '../../lib/hooks/useBookingOrder';

export function BookingSuccessPage() {
  const { orderId } = useParams<{ orderId: string }>();
  const navigate = useNavigate();
  const { data: order, isLoading, error } = useBookingOrder(orderId);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <Loader2 className="w-12 h-12 animate-spin text-amber-500 mx-auto mb-4" />
          <p className="text-slate-600">Đang tải thông tin đơn hàng...</p>
        </div>
      </div>
    );
  }

  if (error || !orderId) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100 p-4">
        <div className="max-w-2xl mx-auto">
          <button
            onClick={() => navigate(-1)}
            className="flex items-center gap-2 text-slate-600 hover:text-slate-900 mb-6"
          >
            <ArrowLeft className="w-4 h-4" />
            Quay lại
          </button>

          <Card className="border-red-200 bg-red-50">
            <CardContent className="pt-6">
              <div className="flex items-start gap-4">
                <AlertCircle className="w-8 h-8 text-red-600 flex-shrink-0 mt-1" />
                <div>
                  <h2 className="text-lg font-semibold text-red-900 mb-2">Lỗi</h2>
                  <p className="text-red-700">{error?.message || 'Không thể tải thông tin đơn hàng'}</p>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100 p-4">
      <div className="max-w-2xl mx-auto">
        <button
          onClick={() => navigate(-1)}
          className="flex items-center gap-2 text-slate-600 hover:text-slate-900 mb-6"
        >
          <ArrowLeft className="w-4 h-4" />
          Quay lại
        </button>

        {/* Success Card */}
        <Card className="border-green-200 bg-gradient-to-br from-green-50 to-emerald-50 shadow-lg mb-6">
          <CardContent className="pt-6">
            <div className="flex items-start gap-4">
              <CheckCircle className="w-12 h-12 text-green-600 flex-shrink-0" />
              <div>
                <h1 className="text-2xl font-bold text-green-900 mb-2">Đặt lịch thành công!</h1>
                <p className="text-green-700 mb-4">
                  Cảm ơn bạn đã đặt lịch. Nhân viên của chúng tôi sẽ gọi lại để tư vấn chi tiết trong thời gian sớm nhất.
                </p>
                <div className="bg-white/50 rounded-lg p-3 text-sm text-green-800">
                  <p className="font-semibold">Mã đơn hàng: {order?.id || orderId}</p>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Order Details */}
        {order && (
          <Card className="shadow-md">
            <CardHeader className="border-b border-slate-100 bg-slate-50/50">
              <CardTitle className="text-lg">Chi tiết đơn hàng</CardTitle>
            </CardHeader>
            <CardContent className="pt-6 space-y-6">
              {/* Customer Info */}
              <div>
                <h3 className="font-semibold text-slate-900 mb-3">Thông tin khách hàng</h3>
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <p className="text-slate-600">Họ và tên</p>
                    <p className="font-medium text-slate-900">{order.customerName || 'N/A'}</p>
                  </div>
                  <div>
                    <p className="text-slate-600">Số điện thoại</p>
                    <p className="font-medium text-slate-900">{order.customerPhone || 'N/A'}</p>
                  </div>
                  <div className="col-span-2">
                    <p className="text-slate-600">Email</p>
                    <p className="font-medium text-slate-900">{order.customerEmail || 'N/A'}</p>
                  </div>
                </div>
              </div>

              {/* Order Info */}
              <div className="border-t border-slate-200 pt-6">
                <h3 className="font-semibold text-slate-900 mb-3">Thông tin đơn hàng</h3>
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <p className="text-slate-600">Trạng thái</p>
                    <p className="font-medium text-amber-600">{order.status || 'Pending'}</p>
                  </div>
                  <div>
                    <p className="text-slate-600">Ngày tạo</p>
                    <p className="font-medium text-slate-900">
                      {order.createdAt ? new Date(order.createdAt).toLocaleDateString('vi-VN') : 'N/A'}
                    </p>
                  </div>
                  <div className="col-span-2">
                    <p className="text-slate-600">Ghi chú</p>
                    <p className="font-medium text-slate-900 whitespace-pre-wrap">{order.notes || 'Không có'}</p>
                  </div>
                </div>
              </div>

              {/* Items */}
              {order.items && order.items.length > 0 && (
                <div className="border-t border-slate-200 pt-6">
                  <h3 className="font-semibold text-slate-900 mb-3">Dịch vụ</h3>
                  <div className="space-y-2">
                    {order.items.map((item: any, index: number) => (
                      <div key={index} className="flex justify-between items-start p-3 bg-slate-50 rounded-lg">
                        <div>
                          <p className="font-medium text-slate-900">{item.name || item.productName}</p>
                          <p className="text-xs text-slate-600">{item.description}</p>
                        </div>
                        <p className="font-semibold text-slate-900">x{item.quantity}</p>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {/* Next Steps */}
              <div className="border-t border-slate-200 pt-6 bg-blue-50 rounded-lg p-4">
                <h3 className="font-semibold text-blue-900 mb-2">Bước tiếp theo</h3>
                <ul className="text-sm text-blue-800 space-y-2">
                  <li className="flex gap-2">
                    <span className="font-bold">1.</span>
                    <span>Nhân viên sẽ gọi lại số điện thoại của bạn để xác nhận và tư vấn chi tiết</span>
                  </li>
                  <li className="flex gap-2">
                    <span className="font-bold">2.</span>
                    <span>Thảo luận về các dịch vụ phù hợp và lịch tư vấn</span>
                  </li>
                  <li className="flex gap-2">
                    <span className="font-bold">3.</span>
                    <span>Hoàn tất thanh toán và bắt đầu dịch vụ</span>
                  </li>
                </ul>
              </div>
            </CardContent>
          </Card>
        )}

        {/* Action Buttons */}
        <div className="flex gap-3 mt-6">
          <button
            onClick={() => navigate('/calendar')}
            className="flex-1 px-4 py-3 bg-amber-500 hover:bg-amber-600 text-white font-semibold rounded-lg transition-colors"
          >
            Quay lại lịch
          </button>
          <button
            onClick={() => navigate('/bookings')}
            className="flex-1 px-4 py-3 bg-slate-200 hover:bg-slate-300 text-slate-900 font-semibold rounded-lg transition-colors"
          >
            Lịch sử booking
          </button>
        </div>
      </div>
    </div>
  );
}

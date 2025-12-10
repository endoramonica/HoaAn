import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Loader2, ArrowLeft, Calendar, Phone, Mail, Clock } from 'lucide-react';
import { orderService } from '../../lib/services/orderService';
import { toast } from 'sonner';

interface BookingItem {
  id: string;
  customerName?: string;
  customerPhone?: string;
  customerEmail?: string;
  status: string;
  createdAt: string;
  notes?: string;
  items?: any[];
}

export function BookingHistoryPage() {
  const navigate = useNavigate();
  const [bookings, setBookings] = useState<BookingItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchBookings = async () => {
      try {
        setIsLoading(true);
        const result = await orderService.getMyServiceOrders({
          pageSize: 20,
        });
        setBookings(result.items || []);
      } catch (err) {
        console.error('Error fetching bookings:', err);
        setError('Không thể tải lịch sử booking');
        toast.error('Không thể tải lịch sử booking');
      } finally {
        setIsLoading(false);
      }
    };

    fetchBookings();
  }, []);

  const getStatusColor = (status: string) => {
    switch (status?.toLowerCase()) {
      case 'pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'confirmed':
        return 'bg-blue-100 text-blue-800';
      case 'processing':
        return 'bg-purple-100 text-purple-800';
      case 'completed':
        return 'bg-green-100 text-green-800';
      case 'cancelled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-slate-100 text-slate-800';
    }
  };

  const getStatusLabel = (status: string) => {
    const labels: Record<string, string> = {
      pending: 'Chờ xác nhận',
      confirmed: 'Đã xác nhận',
      processing: 'Đang xử lý',
      completed: 'Hoàn thành',
      cancelled: 'Đã hủy',
    };
    return labels[status?.toLowerCase()] || status;
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <Loader2 className="w-12 h-12 animate-spin text-amber-500 mx-auto mb-4" />
          <p className="text-slate-600">Đang tải lịch sử booking...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100 p-4">
      <div className="max-w-4xl mx-auto">
        <button
          onClick={() => navigate(-1)}
          className="flex items-center gap-2 text-slate-600 hover:text-slate-900 mb-6"
        >
          <ArrowLeft className="w-4 h-4" />
          Quay lại
        </button>

        <div className="mb-6">
          <h1 className="text-3xl font-bold text-slate-900 mb-2">Lịch sử booking</h1>
          <p className="text-slate-600">Xem tất cả các booking dịch vụ của bạn</p>
        </div>

        {error && (
          <Card className="border-red-200 bg-red-50 mb-6">
            <CardContent className="pt-6">
              <p className="text-red-700">{error}</p>
            </CardContent>
          </Card>
        )}

        {bookings.length === 0 ? (
          <Card className="shadow-md">
            <CardContent className="pt-12 pb-12 text-center">
              <Calendar className="w-12 h-12 text-slate-300 mx-auto mb-4" />
              <p className="text-slate-600 mb-4">Bạn chưa có booking nào</p>
              <button
                onClick={() => navigate('/calendar')}
                className="px-4 py-2 bg-amber-500 hover:bg-amber-600 text-white font-semibold rounded-lg transition-colors"
              >
                Đặt lịch ngay
              </button>
            </CardContent>
          </Card>
        ) : (
          <div className="space-y-4">
            {bookings.map((booking) => (
              <Card
                key={booking.id}
                className="shadow-md hover:shadow-lg transition-shadow cursor-pointer"
                onClick={() => navigate(`/bookings/${booking.id}`)}
              >
                <CardContent className="pt-6">
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    {/* Left side - Booking info */}
                    <div>
                      <div className="mb-4">
                        <p className="text-xs text-slate-600 uppercase tracking-wide mb-1">Mã booking</p>
                        <p className="font-mono text-sm font-semibold text-slate-900">{booking.id}</p>
                      </div>

                      <div className="mb-4">
                        <p className="text-xs text-slate-600 uppercase tracking-wide mb-1">Trạng thái</p>
                        <span className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${getStatusColor(booking.status)}`}>
                          {getStatusLabel(booking.status)}
                        </span>
                      </div>

                      <div className="mb-4">
                        <p className="text-xs text-slate-600 uppercase tracking-wide mb-1">Ngày tạo</p>
                        <div className="flex items-center gap-2 text-sm text-slate-900">
                          <Clock className="w-4 h-4 text-slate-500" />
                          {new Date(booking.createdAt).toLocaleDateString('vi-VN', {
                            year: 'numeric',
                            month: 'long',
                            day: 'numeric',
                            hour: '2-digit',
                            minute: '2-digit',
                          })}
                        </div>
                      </div>
                    </div>

                    {/* Right side - Contact info */}
                    <div>
                      {booking.customerName && (
                        <div className="mb-4">
                          <p className="text-xs text-slate-600 uppercase tracking-wide mb-1">Tên khách hàng</p>
                          <p className="text-sm font-medium text-slate-900">{booking.customerName}</p>
                        </div>
                      )}

                      {booking.customerPhone && (
                        <div className="mb-4">
                          <p className="text-xs text-slate-600 uppercase tracking-wide mb-1">Số điện thoại</p>
                          <div className="flex items-center gap-2 text-sm text-slate-900">
                            <Phone className="w-4 h-4 text-slate-500" />
                            <a href={`tel:${booking.customerPhone}`} className="hover:text-amber-600">
                              {booking.customerPhone}
                            </a>
                          </div>
                        </div>
                      )}

                      {booking.customerEmail && (
                        <div className="mb-4">
                          <p className="text-xs text-slate-600 uppercase tracking-wide mb-1">Email</p>
                          <div className="flex items-center gap-2 text-sm text-slate-900">
                            <Mail className="w-4 h-4 text-slate-500" />
                            <a href={`mailto:${booking.customerEmail}`} className="hover:text-amber-600 truncate">
                              {booking.customerEmail}
                            </a>
                          </div>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Services */}
                  {booking.items && booking.items.length > 0 && (
                    <div className="mt-6 pt-6 border-t border-slate-200">
                      <p className="text-xs text-slate-600 uppercase tracking-wide mb-3">Dịch vụ</p>
                      <div className="space-y-2">
                        {booking.items.map((item, index) => (
                          <div key={index} className="flex justify-between items-start text-sm">
                            <span className="text-slate-900 font-medium">{item.name || item.productName}</span>
                            <span className="text-slate-600">x{item.quantity}</span>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}

                  {/* Notes */}
                  {booking.notes && (
                    <div className="mt-6 pt-6 border-t border-slate-200">
                      <p className="text-xs text-slate-600 uppercase tracking-wide mb-2">Ghi chú</p>
                      <p className="text-sm text-slate-700 whitespace-pre-wrap">{booking.notes}</p>
                    </div>
                  )}

                  {/* Action */}
                  <div className="mt-6 pt-6 border-t border-slate-200">
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        navigate(`/bookings/${booking.id}`);
                      }}
                      className="w-full px-4 py-2 bg-amber-500 hover:bg-amber-600 text-white font-semibold rounded-lg transition-colors"
                    >
                      Xem chi tiết
                    </button>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

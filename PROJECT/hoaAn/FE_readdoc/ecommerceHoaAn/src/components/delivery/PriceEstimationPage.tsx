import { useState, useEffect } from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { MapPin, Clock, Package, Calculator, CheckCircle } from 'lucide-react';

interface PriceEstimationPageProps {
  onBack: () => void;
  onNavigate: (page: string) => void;
}

export function PriceEstimationPage({ onBack, onNavigate }: PriceEstimationPageProps) {
  const [loading, setLoading] = useState(true);
  const [prices, setPrices] = useState({
    instant: 0,
    economy: 0,
    scheduled: 0
  });

  // Mock price calculation
  useEffect(() => {
    const timer = setTimeout(() => {
      setPrices({
        instant: 25000,
        economy: 18000,
        scheduled: 20000
      });
      setLoading(false);
    }, 2000);

    return () => clearTimeout(timer);
  }, []);

  const handleConfirmBooking = () => {
    // Simulate booking confirmation
    onNavigate('order-tracking');
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(price);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50">
        {/* Header */}
        <div className="bg-white shadow-sm">
          <div className="max-w-md mx-auto px-4 py-4">
            <div className="flex items-center justify-between">
              <button
                onClick={onBack}
                className="text-gray-600 hover:text-gray-800"
              >
                ← Quay lại
              </button>
              <h1 className="text-lg text-gray-800">Ước tính chi phí</h1>
              <div className="w-6"></div>
            </div>
          </div>
        </div>

        <div className="max-w-md mx-auto px-4 py-6">
          <Card className="p-6">
            <div className="text-center">
              <div className="animate-spin w-12 h-12 border-4 border-green-200 border-t-green-600 rounded-full mx-auto mb-4"></div>
              <h2 className="text-gray-800 mb-2">Đang tính toán chi phí</h2>
              <p className="text-gray-600 text-sm">Vui lòng chờ trong giây lát...</p>
            </div>
          </Card>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white shadow-sm">
        <div className="max-w-md mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <button
              onClick={onBack}
              className="text-gray-600 hover:text-gray-800"
            >
              ← Quay lại
            </button>
            <h1 className="text-lg text-gray-800">Ước tính chi phí</h1>
            <div className="w-6"></div>
          </div>
        </div>
      </div>

      <div className="max-w-md mx-auto px-4 py-6 space-y-6">
        {/* Route Summary */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4">Thông tin tuyến đường</h2>
          <div className="space-y-3">
            <div className="flex items-start gap-3">
              <div className="w-3 h-3 bg-green-500 rounded-full mt-2"></div>
              <div className="flex-1">
                <p className="text-sm text-gray-500">Điểm lấy hàng</p>
                <p className="text-gray-800">123 Nguyễn Huệ, Quận 1, TP.HCM</p>
              </div>
            </div>
            <div className="border-l-2 border-dashed border-gray-300 ml-1.5 h-6"></div>
            <div className="flex items-start gap-3">
              <div className="w-3 h-3 bg-red-500 rounded-full mt-2"></div>
              <div className="flex-1">
                <p className="text-sm text-gray-500">Điểm giao hàng</p>
                <p className="text-gray-800">456 Lê Lợi, Quận 3, TP.HCM</p>
              </div>
            </div>
          </div>
          <div className="mt-4 pt-4 border-t border-gray-200">
            <div className="grid grid-cols-2 gap-4 text-sm">
              <div>
                <p className="text-gray-500">Khoảng cách</p>
                <p className="text-gray-800">5.2 km</p>
              </div>
              <div>
                <p className="text-gray-500">Khối lượng</p>
                <p className="text-gray-800">2.5 kg</p>
              </div>
            </div>
          </div>
        </Card>

        {/* Price Options */}
        <div className="space-y-4">
          <h2 className="text-gray-800">Chọn loại giao hàng</h2>

          {/* Instant Delivery */}
          <Card className="p-4 border-green-200 hover:shadow-md transition-shadow cursor-pointer">
            <div className="flex items-center justify-between">
              <div className="flex items-start gap-3">
                <div className="w-10 h-10 bg-green-100 rounded-full flex items-center justify-center">
                  <Clock className="w-5 h-5 text-green-600" />
                </div>
                <div>
                  <h3 className="text-gray-800 mb-1">Giao nhanh</h3>
                  <p className="text-sm text-gray-600 mb-2">15-30 phút</p>
                  <div className="flex items-center gap-2 text-sm">
                    <CheckCircle className="w-4 h-4 text-green-600" />
                    <span className="text-green-600">Ưu tiên cao nhất</span>
                  </div>
                </div>
              </div>
              <div className="text-right">
                <p className="text-lg text-gray-800">{formatPrice(prices.instant)}</p>
                <p className="text-sm text-gray-500">~4,800₫/km</p>
              </div>
            </div>
          </Card>

          {/* Economy Delivery */}
          <Card className="p-4 border-orange-200 hover:shadow-md transition-shadow cursor-pointer border-2">
            <div className="flex items-center justify-between">
              <div className="flex items-start gap-3">
                <div className="w-10 h-10 bg-orange-100 rounded-full flex items-center justify-center">
                  <Package className="w-5 h-5 text-orange-500" />
                </div>
                <div>
                  <h3 className="text-gray-800 mb-1">Tiết kiệm</h3>
                  <p className="text-sm text-gray-600 mb-2">1-2 giờ</p>
                  <div className="flex items-center gap-2 text-sm">
                    <CheckCircle className="w-4 h-4 text-orange-500" />
                    <span className="text-orange-500">Được khuyến nghị</span>
                  </div>
                </div>
              </div>
              <div className="text-right">
                <p className="text-lg text-gray-800">{formatPrice(prices.economy)}</p>
                <p className="text-sm text-gray-500">~3,500₫/km</p>
              </div>
            </div>
          </Card>

          {/* Scheduled Delivery */}
          <Card className="p-4 border-blue-200 hover:shadow-md transition-shadow cursor-pointer">
            <div className="flex items-center justify-between">
              <div className="flex items-start gap-3">
                <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
                  <Clock className="w-5 h-5 text-blue-600" />
                </div>
                <div>
                  <h3 className="text-gray-800 mb-1">Đặt lịch</h3>
                  <p className="text-sm text-gray-600 mb-2">Theo lịch hẹn</p>
                  <div className="flex items-center gap-2 text-sm">
                    <CheckCircle className="w-4 h-4 text-blue-600" />
                    <span className="text-blue-600">Linh hoạt thời gian</span>
                  </div>
                </div>
              </div>
              <div className="text-right">
                <p className="text-lg text-gray-800">{formatPrice(prices.scheduled)}</p>
                <p className="text-sm text-gray-500">~3,800₫/km</p>
              </div>
            </div>
          </Card>
        </div>

        {/* Price Breakdown */}
        <Card className="p-4">
          <h3 className="text-gray-800 mb-3">Chi tiết chi phí (Tiết kiệm)</h3>
          <div className="space-y-2 text-sm">
            <div className="flex justify-between">
              <span className="text-gray-600">Phí cơ bản</span>
              <span className="text-gray-800">15,000₫</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Phí khoảng cách (5.2km)</span>
              <span className="text-gray-800">3,000₫</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Phí khối lượng</span>
              <span className="text-gray-800">0₫</span>
            </div>
            <div className="border-t border-gray-200 pt-2 mt-2">
              <div className="flex justify-between">
                <span className="text-gray-800">Tổng cộng</span>
                <span className="text-gray-800">{formatPrice(prices.economy)}</span>
              </div>
            </div>
          </div>
        </Card>

        {/* Confirm Button */}
        <Button
          onClick={handleConfirmBooking}
          className="w-full bg-orange-500 hover:bg-orange-600 text-white py-3"
        >
          <Calculator className="w-4 h-4 mr-2" />
          Xác nhận đặt xe giao hàng
        </Button>

        {/* Terms */}
        <p className="text-xs text-gray-500 text-center">
          Bằng việc xác nhận, bạn đồng ý với{' '}
          <span className="text-orange-500 underline">Điều khoản sử dụng</span>{' '}
          và{' '}
          <span className="text-orange-500 underline">Chính sách bảo mật</span>
        </p>
      </div>
    </div>
  );
}
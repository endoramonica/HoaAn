import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { MapPin, Phone, MessageCircle, Package, Clock, User, Copy, Star } from 'lucide-react';

interface OrderDetailsPageProps {
  onBack: () => void;
  onNavigate: (page: string) => void;
}

export function OrderDetailsPage({ onBack, onNavigate }: OrderDetailsPageProps) {
  const orderDetails = {
    id: 'DL003',
    status: 'Đang giao hàng',
    createdAt: '14:30, 15/01/2024',
    estimatedDelivery: '15:00, 15/01/2024',
    deliveryType: 'Giao nhanh',
    totalFee: 25000,
    pickup: {
      address: '123 Nguyễn Huệ, Phường Bến Nghé, Quận 1, TP.HCM',
      contactName: 'Nguyễn Văn A',
      contactPhone: '0123 456 789',
      note: 'Gọi trước khi đến'
    },
    dropoff: {
      address: '456 Lê Lợi, Phường Bến Thành, Quận 1, TP.HCM',
      contactName: 'Trần Thị B',
      contactPhone: '0987 654 321',
      note: 'Giao tại quầy lễ tân'
    },
    package: {
      type: 'Giấy tờ, tài liệu',
      weight: '0.5 kg',
      value: '500,000 VNĐ',
      description: 'Hồ sơ công việc quan trọng'
    },
    driver: {
      name: 'Nguyễn Văn An',
      phone: '0123 456 789',
      rating: 4.8,
      vehicleType: 'Xe máy',
      licensePlate: '59-F1 12345'
    }
  };

  const copyToClipboard = (text: string) => {
    navigator.clipboard.writeText(text);
    // In a real app, you'd show a toast notification here
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(price);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white shadow-sm sticky top-0 z-10">
        <div className="max-w-md mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <button
              onClick={onBack}
              className="text-gray-600 hover:text-gray-800"
            >
              ← Quay lại
            </button>
            <h1 className="text-lg text-gray-800">Chi tiết đơn hàng</h1>
            <div className="w-6"></div>
          </div>
        </div>
      </div>

      <div className="max-w-md mx-auto px-4 py-6 space-y-6">
        {/* Order Status */}
        <Card className="p-4">
          <div className="flex items-center justify-between mb-4">
            <div>
              <div className="flex items-center gap-2 mb-2">
                <h2 className="text-gray-800">#{orderDetails.id}</h2>
                <button
                  onClick={() => copyToClipboard(orderDetails.id)}
                  className="text-gray-400 hover:text-gray-600"
                >
                  <Copy className="w-4 h-4" />
                </button>
              </div>
              <p className="text-sm text-gray-600">Đặt lúc: {orderDetails.createdAt}</p>
            </div>
            <div className="text-right">
              <div className="px-3 py-1 bg-green-100 text-green-700 rounded-full text-sm mb-2">
                {orderDetails.status}
              </div>
              <p className="text-sm text-gray-600">{orderDetails.deliveryType}</p>
            </div>
          </div>
          
          <div className="flex items-center gap-3 p-3 bg-blue-50 rounded-lg">
            <Clock className="w-5 h-5 text-blue-600" />
            <div>
              <p className="text-blue-800">Dự kiến giao hàng</p>
              <p className="text-sm text-blue-600">{orderDetails.estimatedDelivery}</p>
            </div>
          </div>
        </Card>

        {/* Pickup & Dropoff */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4">Thông tin giao nhận</h2>
          
          {/* Pickup */}
          <div className="mb-6">
            <div className="flex items-center gap-2 mb-3">
              <div className="w-3 h-3 bg-green-500 rounded-full"></div>
              <h3 className="text-gray-800">Điểm lấy hàng</h3>
            </div>
            <div className="ml-5 space-y-2">
              <div className="flex items-start gap-2">
                <MapPin className="w-4 h-4 text-gray-400 mt-0.5" />
                <p className="text-gray-700 text-sm">{orderDetails.pickup.address}</p>
              </div>
              <div className="flex items-center gap-2">
                <User className="w-4 h-4 text-gray-400" />
                <span className="text-gray-700 text-sm">{orderDetails.pickup.contactName}</span>
                <button
                  className="text-green-600 hover:text-green-700 ml-auto"
                  onClick={() => window.open(`tel:${orderDetails.pickup.contactPhone}`)}
                >
                  <Phone className="w-4 h-4" />
                </button>
              </div>
              {orderDetails.pickup.note && (
                <p className="text-gray-500 text-sm italic">Ghi chú: {orderDetails.pickup.note}</p>
              )}
            </div>
          </div>
          
          {/* Route line */}
          <div className="flex justify-center mb-6">
            <div className="border-l-2 border-dashed border-gray-300 h-8"></div>
          </div>
          
          {/* Dropoff */}
          <div>
            <div className="flex items-center gap-2 mb-3">
              <div className="w-3 h-3 bg-red-500 rounded-full"></div>
              <h3 className="text-gray-800">Điểm giao hàng</h3>
            </div>
            <div className="ml-5 space-y-2">
              <div className="flex items-start gap-2">
                <MapPin className="w-4 h-4 text-gray-400 mt-0.5" />
                <p className="text-gray-700 text-sm">{orderDetails.dropoff.address}</p>
              </div>
              <div className="flex items-center gap-2">
                <User className="w-4 h-4 text-gray-400" />
                <span className="text-gray-700 text-sm">{orderDetails.dropoff.contactName}</span>
                <button
                  className="text-green-600 hover:text-green-700 ml-auto"
                  onClick={() => window.open(`tel:${orderDetails.dropoff.contactPhone}`)}
                >
                  <Phone className="w-4 h-4" />
                </button>
              </div>
              {orderDetails.dropoff.note && (
                <p className="text-gray-500 text-sm italic">Ghi chú: {orderDetails.dropoff.note}</p>
              )}
            </div>
          </div>
        </Card>

        {/* Package Info */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4">Thông tin hàng hóa</h2>
          <div className="space-y-3">
            <div className="flex justify-between">
              <span className="text-gray-600">Loại hàng hóa</span>
              <span className="text-gray-800">{orderDetails.package.type}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Khối lượng</span>
              <span className="text-gray-800">{orderDetails.package.weight}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Giá trị khai báo</span>
              <span className="text-gray-800">{orderDetails.package.value}</span>
            </div>
            {orderDetails.package.description && (
              <div>
                <span className="text-gray-600 block mb-1">Mô tả</span>
                <p className="text-gray-800 text-sm">{orderDetails.package.description}</p>
              </div>
            )}
          </div>
        </Card>

        {/* Driver Info */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4">Thông tin tài xế</h2>
          <div className="flex items-center gap-4 mb-4">
            <div className="w-12 h-12 bg-gray-200 rounded-full flex items-center justify-center">
              <span className="text-gray-600">AN</span>
            </div>
            <div className="flex-1">
              <h3 className="text-gray-800">{orderDetails.driver.name}</h3>
              <div className="flex items-center gap-4 text-sm text-gray-600">
                <span>{orderDetails.driver.vehicleType}</span>
                <span>{orderDetails.driver.licensePlate}</span>
              </div>
              <div className="flex items-center gap-1 text-sm text-gray-600">
                <Star className="w-4 h-4 text-yellow-500 fill-current" />
                <span>{orderDetails.driver.rating} (234 đánh giá)</span>
              </div>
            </div>
          </div>
          
          <div className="flex gap-3">
            <Button 
              variant="outline" 
              className="flex-1"
              onClick={() => window.open(`tel:${orderDetails.driver.phone}`)}
            >
              <Phone className="w-4 h-4 mr-2" />
              Gọi điện
            </Button>
            <Button variant="outline" className="flex-1">
              <MessageCircle className="w-4 h-4 mr-2" />
              Nhắn tin
            </Button>
          </div>
        </Card>

        {/* Payment Info */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4">Thông tin thanh toán</h2>
          <div className="space-y-3">
            <div className="flex justify-between">
              <span className="text-gray-600">Phí giao hàng</span>
              <span className="text-gray-800">{formatPrice(orderDetails.totalFee)}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-600">Phương thức</span>
              <span className="text-gray-800">Tiền mặt</span>
            </div>
            <div className="border-t border-gray-200 pt-3">
              <div className="flex justify-between">
                <span className="text-gray-800">Tổng cộng</span>
                <span className="text-gray-800">{formatPrice(orderDetails.totalFee)}</span>
              </div>
            </div>
          </div>
        </Card>

        {/* Action Buttons */}
        <div className="space-y-3">
          <Button
            onClick={() => onNavigate('order-tracking')}
            className="w-full bg-green-600 hover:bg-green-700 text-white"
          >
            <Package className="w-4 h-4 mr-2" />
            Theo dõi trực tiếp
          </Button>
          
          <div className="grid grid-cols-2 gap-3">
            <Button
              variant="outline"
              onClick={() => onNavigate('driver-rating')}
            >
              <Star className="w-4 h-4 mr-2" />
              Đánh giá
            </Button>
            <Button variant="outline">
              <MessageCircle className="w-4 h-4 mr-2" />
              Hỗ trợ
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
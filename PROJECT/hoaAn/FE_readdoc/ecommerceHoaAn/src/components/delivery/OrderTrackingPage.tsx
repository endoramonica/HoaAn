import { useState, useEffect } from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { MapPin, Phone, MessageCircle, Clock, CheckCircle, Truck, Package } from 'lucide-react';

interface OrderTrackingPageProps {
  onBack: () => void;
  onNavigate: (page: string) => void;
}

export function OrderTrackingPage({ onBack, onNavigate }: OrderTrackingPageProps) {
  const [currentStatus, setCurrentStatus] = useState(2);
  const [driverLocation, setDriverLocation] = useState({ lat: 10.7769, lng: 106.7009 });

  const orderInfo = {
    id: 'DL003',
    driverName: 'Nguyễn Văn An',
    driverPhone: '0123 456 789',
    driverRating: 4.8,
    vehicleType: 'Xe máy',
    licensePlate: '59-F1 12345',
    estimatedTime: '12 phút'
  };

  const statusSteps = [
    {
      id: 1,
      title: 'Đã nhận đơn',
      description: 'Tài xế đã nhận đơn hàng của bạn',
      time: '14:30',
      completed: true
    },
    {
      id: 2,
      title: 'Đang đến điểm lấy',
      description: 'Tài xế đang di chuyển đến địa chỉ lấy hàng',
      time: '14:35',
      completed: true
    },
    {
      id: 3,
      title: 'Đã lấy hàng',
      description: 'Tài xế đã lấy hàng và bắt đầu giao',
      time: '14:42',
      completed: currentStatus >= 3
    },
    {
      id: 4,
      title: 'Đang giao hàng',
      description: 'Tài xế đang trên đường giao hàng',
      time: '',
      completed: currentStatus >= 4
    },
    {
      id: 5,
      title: 'Đã giao thành công',
      description: 'Hàng đã được giao đến người nhận',
      time: '',
      completed: currentStatus >= 5
    }
  ];

  // Simulate real-time updates
  useEffect(() => {
    const timer = setInterval(() => {
      // Simulate driver movement
      setDriverLocation(prev => ({
        lat: prev.lat + (Math.random() - 0.5) * 0.001,
        lng: prev.lng + (Math.random() - 0.5) * 0.001
      }));
    }, 5000);

    return () => clearInterval(timer);
  }, []);

  const MapPlaceholder = () => (
    <div className="relative bg-green-50 rounded-lg h-64 overflow-hidden">
      <div className="absolute inset-0 bg-gradient-to-br from-green-100 to-green-200">
        {/* Mock map with driver location */}
        <div className="absolute top-4 left-4 bg-white rounded-lg p-3 shadow-lg">
          <div className="flex items-center gap-2">
            <div className="w-3 h-3 bg-green-500 rounded-full animate-pulse"></div>
            <span className="text-sm text-gray-700">Tài xế đang di chuyển</span>
          </div>
        </div>
        
        {/* Route visualization */}
        <div className="absolute top-1/2 left-1/4 transform -translate-y-1/2">
          <div className="w-4 h-4 bg-green-500 rounded-full shadow-lg"></div>
          <div className="text-xs text-gray-700 mt-1 whitespace-nowrap">Điểm lấy</div>
        </div>
        
        <div className="absolute top-1/3 left-1/2 transform -translate-y-1/2">
          <div className="w-6 h-6 bg-blue-500 rounded-full shadow-lg flex items-center justify-center animate-bounce">
            <Truck className="w-3 h-3 text-white" />
          </div>
          <div className="text-xs text-gray-700 mt-1 whitespace-nowrap">Tài xế</div>
        </div>
        
        <div className="absolute bottom-1/4 right-1/4">
          <div className="w-4 h-4 bg-red-500 rounded-full shadow-lg"></div>
          <div className="text-xs text-gray-700 mt-1 whitespace-nowrap">Điểm giao</div>
        </div>
        
        {/* Distance indicator */}
        <div className="absolute bottom-4 right-4 bg-white rounded-lg p-2 shadow-lg">
          <div className="text-sm text-gray-700">
            <div className="text-blue-600">2.3 km</div>
            <div className="text-xs text-gray-500">còn lại</div>
          </div>
        </div>
      </div>
    </div>
  );

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
            <h1 className="text-lg text-gray-800">Theo dõi đơn hàng</h1>
            <button
              onClick={() => onNavigate('order-details')}
              className="text-green-600 text-sm hover:underline"
            >
              Chi tiết
            </button>
          </div>
        </div>
      </div>

      <div className="max-w-md mx-auto px-4 py-6 space-y-6">
        {/* Order Status */}
        <Card className="p-4">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h2 className="text-gray-800">Đơn hàng #{orderInfo.id}</h2>
              <p className="text-sm text-gray-600">Dự kiến: {orderInfo.estimatedTime} nữa</p>
            </div>
            <div className="text-right">
              <div className="px-3 py-1 bg-green-100 text-green-700 rounded-full text-sm">
                Đang giao
              </div>
            </div>
          </div>
          
          {/* Current Status */}
          <div className="flex items-center gap-3 p-3 bg-green-50 rounded-lg">
            <div className="w-10 h-10 bg-green-500 rounded-full flex items-center justify-center">
              <Truck className="w-5 h-5 text-white" />
            </div>
            <div>
              <h3 className="text-green-800">Đang đến điểm lấy</h3>
              <p className="text-sm text-green-600">Tài xế sẽ đến trong vài phút</p>
            </div>
          </div>
        </Card>

        {/* Map */}
        <Card className="p-0 overflow-hidden">
          <MapPlaceholder />
        </Card>

        {/* Driver Info */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4">Thông tin tài xế</h2>
          <div className="flex items-center gap-4 mb-4">
            <div className="w-12 h-12 bg-gray-200 rounded-full flex items-center justify-center">
              <span className="text-gray-600">AN</span>
            </div>
            <div className="flex-1">
              <h3 className="text-gray-800">{orderInfo.driverName}</h3>
              <div className="flex items-center gap-4 text-sm text-gray-600">
                <span>{orderInfo.vehicleType}</span>
                <span>{orderInfo.licensePlate}</span>
                <div className="flex items-center gap-1">
                  <span>⭐</span>
                  <span>{orderInfo.driverRating}</span>
                </div>
              </div>
            </div>
          </div>
          
          <div className="flex gap-3">
            <Button variant="outline" className="flex-1">
              <Phone className="w-4 h-4 mr-2" />
              Gọi điện
            </Button>
            <Button variant="outline" className="flex-1">
              <MessageCircle className="w-4 h-4 mr-2" />
              Nhắn tin
            </Button>
          </div>
        </Card>

        {/* Status Timeline */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4">Trạng thái đơn hàng</h2>
          <div className="space-y-4">
            {statusSteps.map((step, index) => (
              <div key={step.id} className="flex gap-4">
                <div className="flex flex-col items-center">
                  <div className={`w-6 h-6 rounded-full flex items-center justify-center ${
                    step.completed 
                      ? 'bg-green-500 text-white' 
                      : currentStatus === step.id
                      ? 'bg-blue-500 text-white animate-pulse'
                      : 'bg-gray-200 text-gray-400'
                  }`}>
                    {step.completed ? (
                      <CheckCircle className="w-4 h-4" />
                    ) : currentStatus === step.id ? (
                      <Clock className="w-4 h-4" />
                    ) : (
                      <div className="w-2 h-2 bg-current rounded-full" />
                    )}
                  </div>
                  {index < statusSteps.length - 1 && (
                    <div className={`w-0.5 h-8 ${
                      step.completed ? 'bg-green-500' : 'bg-gray-200'
                    }`} />
                  )}
                </div>
                <div className="flex-1 pb-8">
                  <div className="flex items-center justify-between mb-1">
                    <h3 className={`${
                      step.completed ? 'text-gray-800' : 
                      currentStatus === step.id ? 'text-blue-600' : 'text-gray-400'
                    }`}>
                      {step.title}
                    </h3>
                    {step.time && (
                      <span className="text-sm text-gray-500">{step.time}</span>
                    )}
                  </div>
                  <p className="text-sm text-gray-600">{step.description}</p>
                </div>
              </div>
            ))}
          </div>
        </Card>

        {/* Quick Actions */}
        <div className="grid grid-cols-2 gap-4">
          <Button
            variant="outline"
            onClick={() => onNavigate('order-details')}
            className="h-12"
          >
            <Package className="w-4 h-4 mr-2" />
            Chi tiết đơn
          </Button>
          <Button
            variant="outline"
            className="h-12"
          >
            <MessageCircle className="w-4 h-4 mr-2" />
            Hỗ trợ
          </Button>
        </div>
      </div>
    </div>
  );
}
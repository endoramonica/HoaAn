import { useState } from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { Package, Clock, MapPin, User, Plus, Truck, Star } from 'lucide-react';

interface DeliveryHomePageProps {
  onNavigate: (page: string) => void;
  onDeliveryNavigate: (page: string) => void;
}

export function DeliveryHomePage({ onNavigate, onDeliveryNavigate }: DeliveryHomePageProps) {
  const [activeTab, setActiveTab] = useState('home');

  const recentOrders = [
    {
      id: 'DL001',
      from: 'Quận 1, TP.HCM',
      to: 'Quận 3, TP.HCM',
      status: 'Đang giao',
      estimatedTime: '15 phút',
      type: 'Giao nhanh'
    },
    {
      id: 'DL002',
      from: 'Quận Bình Thạnh',
      to: 'Quận 7, TP.HCM',
      status: 'Đã giao',
      estimatedTime: 'Hoàn thành',
      type: 'Tiết kiệm'
    }
  ];

  const renderBottomNav = () => (
    <div className="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-200 px-4 py-2 z-50">
      <div className="flex justify-around">
        <button
          onClick={() => setActiveTab('home')}
          className={`flex flex-col items-center py-2 px-3 rounded-lg transition-colors ${
            activeTab === 'home' ? 'text-green-600 bg-green-50' : 'text-gray-500'
          }`}
        >
          <Package className="w-5 h-5 mb-1" />
          <span className="text-xs">Trang chủ</span>
        </button>
        <button
          onClick={() => {
            setActiveTab('orders');
            onDeliveryNavigate('order-tracking');
          }}
          className={`flex flex-col items-center py-2 px-3 rounded-lg transition-colors ${
            activeTab === 'orders' ? 'text-green-600 bg-green-50' : 'text-gray-500'
          }`}
        >
          <Clock className="w-5 h-5 mb-1" />
          <span className="text-xs">Đơn hàng</span>
        </button>
        <button
          onClick={() => {
            setActiveTab('delivery');
            onDeliveryNavigate('delivery-booking');
          }}
          className={`flex flex-col items-center py-2 px-3 rounded-lg transition-colors ${
            activeTab === 'delivery' ? 'text-orange-500 bg-orange-50' : 'text-gray-500'
          }`}
        >
          <Plus className="w-5 h-5 mb-1" />
          <span className="text-xs">Giao hàng</span>
        </button>
        <button
          onClick={() => setActiveTab('account')}
          className={`flex flex-col items-center py-2 px-3 rounded-lg transition-colors ${
            activeTab === 'account' ? 'text-green-600 bg-green-50' : 'text-gray-500'
          }`}
        >
          <User className="w-5 h-5 mb-1" />
          <span className="text-xs">Tài khoản</span>
        </button>
      </div>
    </div>
  );

  return (
    <div className="min-h-screen bg-gray-50 pb-20">
      {/* Header */}
      <div className="bg-white shadow-sm">
        <div className="max-w-md mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <button
              onClick={() => onNavigate('home')}
              className="text-gray-600 hover:text-gray-800"
            >
              ← Quay lại
            </button>
            <h1 className="text-lg text-gray-800">VietDelivery</h1>
            <div className="w-6"></div>
          </div>
        </div>
      </div>

      <div className="max-w-md mx-auto px-4 py-6 space-y-6">
        {/* Quick Actions */}
        <div className="grid grid-cols-2 gap-4">
          <Card
            className="p-4 cursor-pointer hover:shadow-md transition-shadow bg-gradient-to-br from-green-50 to-green-100 border-green-200"
            onClick={() => onDeliveryNavigate('delivery-booking')}
          >
            <div className="flex flex-col items-center text-center">
              <div className="w-12 h-12 bg-green-500 rounded-full flex items-center justify-center mb-3">
                <Truck className="w-6 h-6 text-white" />
              </div>
              <h3 className="text-green-800 mb-1">Giao hàng ngay</h3>
              <p className="text-green-600 text-sm">Đặt xe giao hàng</p>
            </div>
          </Card>

          <Card
            className="p-4 cursor-pointer hover:shadow-md transition-shadow bg-gradient-to-br from-orange-50 to-orange-100 border-orange-200"
            onClick={() => onDeliveryNavigate('price-estimation')}
          >
            <div className="flex flex-col items-center text-center">
              <div className="w-12 h-12 bg-orange-500 rounded-full flex items-center justify-center mb-3">
                <MapPin className="w-6 h-6 text-white" />
              </div>
              <h3 className="text-orange-800 mb-1">Tính phí giao</h3>
              <p className="text-orange-600 text-sm">Ước tính chi phí</p>
            </div>
          </Card>
        </div>

        {/* Recent Orders */}
        <div>
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-gray-800">Đơn hàng gần đây</h2>
            <button
              onClick={() => onDeliveryNavigate('order-tracking')}
              className="text-green-600 text-sm hover:underline"
            >
              Xem tất cả
            </button>
          </div>

          <div className="space-y-3">
            {recentOrders.map((order) => (
              <Card key={order.id} className="p-4">
                <div className="flex items-start justify-between mb-3">
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-2">
                      <span className="text-sm text-gray-800">#{order.id}</span>
                      <span className={`px-2 py-1 rounded-full text-xs ${
                        order.status === 'Đang giao' 
                          ? 'bg-green-100 text-green-700'
                          : 'bg-blue-100 text-blue-700'
                      }`}>
                        {order.status}
                      </span>
                    </div>
                    <div className="space-y-1">
                      <div className="flex items-center gap-2 text-sm text-gray-600">
                        <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                        <span>{order.from}</span>
                      </div>
                      <div className="flex items-center gap-2 text-sm text-gray-600">
                        <div className="w-2 h-2 bg-red-500 rounded-full"></div>
                        <span>{order.to}</span>
                      </div>
                    </div>
                  </div>
                  <div className="text-right">
                    <p className="text-sm text-gray-500 mb-1">{order.type}</p>
                    <p className="text-sm text-gray-700">{order.estimatedTime}</p>
                  </div>
                </div>
                <Button
                  variant="outline"
                  size="sm"
                  className="w-full"
                  onClick={() => onDeliveryNavigate('order-details')}
                >
                  Xem chi tiết
                </Button>
              </Card>
            ))}
          </div>
        </div>

        {/* Services */}
        <div>
          <h2 className="text-gray-800 mb-4">Dịch vụ khác</h2>
          <div className="grid grid-cols-3 gap-3">
            <button
              onClick={() => onDeliveryNavigate('order-tracking')}
              className="flex flex-col items-center p-3 bg-white rounded-lg border hover:shadow-md transition-shadow"
            >
              <Clock className="w-8 h-8 text-green-600 mb-2" />
              <span className="text-xs text-gray-700 text-center">Theo dõi đơn hàng</span>
            </button>
            <button
              onClick={() => onDeliveryNavigate('driver-rating')}
              className="flex flex-col items-center p-3 bg-white rounded-lg border hover:shadow-md transition-shadow"
            >
              <Star className="w-8 h-8 text-orange-500 mb-2" />
              <span className="text-xs text-gray-700 text-center">Đánh giá tài xế</span>
            </button>
            <button className="flex flex-col items-center p-3 bg-white rounded-lg border hover:shadow-md transition-shadow">
              <User className="w-8 h-8 text-gray-600 mb-2" />
              <span className="text-xs text-gray-700 text-center">Hỗ trợ</span>
            </button>
          </div>
        </div>
      </div>

      {renderBottomNav()}
    </div>
  );
}

import { useState } from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Textarea } from '../ui/textarea';
import { MapPin, Package, Clock, ArrowRight } from 'lucide-react';

interface DeliveryBookingPageProps {
  onBack: () => void;
  onNavigate: (page: string) => void;
}

export function DeliveryBookingPage({ onBack, onNavigate }: DeliveryBookingPageProps) {
  const [formData, setFormData] = useState({
    pickupAddress: '',
    pickupPhone: '',
    dropoffAddress: '',
    dropoffPhone: '',
    packageType: '',
    packageWeight: '',
    packageValue: '',
    deliveryType: 'instant',
    notes: '',
    scheduledDate: '',
    scheduledTime: ''
  });

  const handleInputChange = (field: string, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const handleBooking = () => {
    // Simulate booking process
    onNavigate('price-estimation');
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
            <h1 className="text-lg text-gray-800">Đặt xe giao hàng</h1>
            <div className="w-6"></div>
          </div>
        </div>
      </div>

      <div className="max-w-md mx-auto px-4 py-6 space-y-6">
        {/* Delivery Type Selection */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4">Loại giao hàng</h2>
          <div className="space-y-3">
            <label className="flex items-center p-3 border rounded-lg cursor-pointer hover:bg-gray-50">
              <input
                type="radio"
                name="deliveryType"
                value="instant"
                checked={formData.deliveryType === 'instant'}
                onChange={(e) => handleInputChange('deliveryType', e.target.value)}
                className="mr-3 text-green-600"
              />
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-1">
                  <Clock className="w-4 h-4 text-green-600" />
                  <span className="text-gray-800">Giao ngay</span>
                  <span className="text-green-600 text-sm">(15-30 phút)</span>
                </div>
                <p className="text-sm text-gray-600">Giao hàng trong vòng 30 phút</p>
              </div>
            </label>

            <label className="flex items-center p-3 border rounded-lg cursor-pointer hover:bg-gray-50">
              <input
                type="radio"
                name="deliveryType"
                value="economy"
                checked={formData.deliveryType === 'economy'}
                onChange={(e) => handleInputChange('deliveryType', e.target.value)}
                className="mr-3 text-orange-500"
              />
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-1">
                  <Package className="w-4 h-4 text-orange-500" />
                  <span className="text-gray-800">Tiết kiệm</span>
                  <span className="text-orange-500 text-sm">(1-2 giờ)</span>
                </div>
                <p className="text-sm text-gray-600">Giá tốt, thời gian linh hoạt</p>
              </div>
            </label>

            <label className="flex items-center p-3 border rounded-lg cursor-pointer hover:bg-gray-50">
              <input
                type="radio"
                name="deliveryType"
                value="scheduled"
                checked={formData.deliveryType === 'scheduled'}
                onChange={(e) => handleInputChange('deliveryType', e.target.value)}
                className="mr-3 text-blue-600"
              />
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-1">
                  <Clock className="w-4 h-4 text-blue-600" />
                  <span className="text-gray-800">Đặt lịch</span>
                  <span className="text-blue-600 text-sm">(Tùy chọn)</span>
                </div>
                <p className="text-sm text-gray-600">Chọn thời gian giao hàng</p>
              </div>
            </label>
          </div>
        </Card>

        {/* Pickup Location */}
        <Card className="p-4">
          <div className="flex items-center gap-2 mb-4">
            <div className="w-3 h-3 bg-green-500 rounded-full"></div>
            <h2 className="text-gray-800">Địa chỉ lấy hàng</h2>
          </div>
          <div className="space-y-4">
            <div>
              <Label htmlFor="pickupAddress">Địa chỉ</Label>
              <div className="relative mt-1">
                <MapPin className="absolute left-3 top-3 w-4 h-4 text-gray-400" />
                <Input
                  id="pickupAddress"
                  placeholder="Nhập địa chỉ lấy hàng"
                  value={formData.pickupAddress}
                  onChange={(e) => handleInputChange('pickupAddress', e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            <div>
              <Label htmlFor="pickupPhone">Số điện thoại người gửi</Label>
              <Input
                id="pickupPhone"
                placeholder="0123 456 789"
                value={formData.pickupPhone}
                onChange={(e) => handleInputChange('pickupPhone', e.target.value)}
              />
            </div>
          </div>
        </Card>

        {/* Dropoff Location */}
        <Card className="p-4">
          <div className="flex items-center gap-2 mb-4">
            <div className="w-3 h-3 bg-red-500 rounded-full"></div>
            <h2 className="text-gray-800">Địa chỉ giao hàng</h2>
          </div>
          <div className="space-y-4">
            <div>
              <Label htmlFor="dropoffAddress">Địa chỉ</Label>
              <div className="relative mt-1">
                <MapPin className="absolute left-3 top-3 w-4 h-4 text-gray-400" />
                <Input
                  id="dropoffAddress"
                  placeholder="Nhập địa chỉ giao hàng"
                  value={formData.dropoffAddress}
                  onChange={(e) => handleInputChange('dropoffAddress', e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            <div>
              <Label htmlFor="dropoffPhone">Số điện thoại người nhận</Label>
              <Input
                id="dropoffPhone"
                placeholder="0123 456 789"
                value={formData.dropoffPhone}
                onChange={(e) => handleInputChange('dropoffPhone', e.target.value)}
              />
            </div>
          </div>
        </Card>

        {/* Package Details */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4">Thông tin hàng hóa</h2>
          <div className="space-y-4">
            <div>
              <Label htmlFor="packageType">Loại hàng hóa</Label>
              <Select onValueChange={(value) => handleInputChange('packageType', value)}>
                <SelectTrigger>
                  <SelectValue placeholder="Chọn loại hàng hóa" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="documents">Giấy tờ, tài liệu</SelectItem>
                  <SelectItem value="food">Thực phẩm</SelectItem>
                  <SelectItem value="clothes">Quần áo</SelectItem>
                  <SelectItem value="electronics">Điện tử</SelectItem>
                  <SelectItem value="gifts">Quà tặng</SelectItem>
                  <SelectItem value="other">Khác</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="packageWeight">Khối lượng (kg)</Label>
                <Input
                  id="packageWeight"
                  placeholder="1.0"
                  type="number"
                  value={formData.packageWeight}
                  onChange={(e) => handleInputChange('packageWeight', e.target.value)}
                />
              </div>
              <div>
                <Label htmlFor="packageValue">Giá trị (VNĐ)</Label>
                <Input
                  id="packageValue"
                  placeholder="100,000"
                  value={formData.packageValue}
                  onChange={(e) => handleInputChange('packageValue', e.target.value)}
                />
              </div>
            </div>
            <div>
              <Label htmlFor="notes">Ghi chú</Label>
              <Textarea
                id="notes"
                placeholder="Mô tả thêm về hàng hóa hoặc yêu cầu đặc biệt"
                value={formData.notes}
                onChange={(e) => handleInputChange('notes', e.target.value)}
                rows={3}
              />
            </div>
          </div>
        </Card>

        {/* Scheduled Delivery */}
        {formData.deliveryType === 'scheduled' && (
          <Card className="p-4">
            <h2 className="text-gray-800 mb-4">Thời gian giao hàng</h2>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="scheduledDate">Ngày</Label>
                <Input
                  id="scheduledDate"
                  type="date"
                  value={formData.scheduledDate}
                  onChange={(e) => handleInputChange('scheduledDate', e.target.value)}
                />
              </div>
              <div>
                <Label htmlFor="scheduledTime">Giờ</Label>
                <Input
                  id="scheduledTime"
                  type="time"
                  value={formData.scheduledTime}
                  onChange={(e) => handleInputChange('scheduledTime', e.target.value)}
                />
              </div>
            </div>
          </Card>
        )}

        {/* Continue Button */}
        <Button
          onClick={handleBooking}
          className="w-full bg-green-600 hover:bg-green-700 text-white py-3"
          disabled={!formData.pickupAddress || !formData.dropoffAddress}
        >
          <span>Tiếp tục</span>
          <ArrowRight className="w-4 h-4 ml-2" />
        </Button>
      </div>
    </div>
  );
}
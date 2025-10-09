import { useState } from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Textarea } from '../ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Badge } from '../ui/badge';
import { Star, Video, MessageCircle, Calendar, Clock, Users, Phone, Award } from 'lucide-react';

interface FengShuiConsultationPageProps {
  onBack: () => void;
}

export function FengShuiConsultationPage({ onBack }: FengShuiConsultationPageProps) {
  const [selectedMaster, setSelectedMaster] = useState<number | null>(null);
  const [consultationType, setConsultationType] = useState('');
  const [selectedDate, setSelectedDate] = useState('');
  const [selectedTime, setSelectedTime] = useState('');
  const [clientInfo, setClientInfo] = useState({
    name: '',
    birthDate: '',
    birthTime: '',
    gender: '',
    address: '',
    issue: ''
  });

  const masters = [
    {
      id: 1,
      name: "Thầy Nguyễn Văn Minh",
      title: "Thạc sĩ Phong Thủy",
      experience: "25 năm",
      rating: 4.9,
      reviews: 2847,
      specialties: ["Phong thủy nhà ở", "Phong thủy văn phòng", "Chọn ngày tốt"],
      price: {
        chat: 50000,
        call: 100000,
        video: 150000
      },
      avatar: "👨‍🦳",
      status: "online",
      languages: ["Tiếng Việt", "English"],
      description: "Chuyên gia phong thủy với hơn 25 năm kinh nghiệm, từng tư vấn cho nhiều doanh nghiệp lớn."
    },
    {
      id: 2,
      name: "Cô Trần Thị Lan",
      title: "Giáo sư Phong Thủy",
      experience: "30 năm",
      rating: 4.8,
      reviews: 1923,
      specialties: ["Phong thủy gia đình", "Tử vi học", "Khắc phục hạn"],
      price: {
        chat: 75000,
        call: 120000,
        video: 180000
      },
      avatar: "👩‍🦳",
      status: "busy",
      languages: ["Tiếng Việt"],
      description: "Giáo sư Phong Thủy tại Đại học, chuyên về phong thủy gia đình và tử vi học."
    },
    {
      id: 3,
      name: "Thầy Lê Quang Minh",
      title: "Chuyên gia Phong Thủy",
      experience: "20 năm",
      rating: 4.7,
      reviews: 1456,
      specialties: ["Phong thủy kinh doanh", "Đầu tư bất động sản", "Năng lượng không gian"],
      price: {
        chat: 60000,
        call: 110000,
        video: 160000
      },
      avatar: "👨‍💼",
      status: "online",
      languages: ["Tiếng Việt", "中文"],
      description: "Chuyên gia phong thủy kinh doanh, đã tư vấn cho hơn 500 doanh nghiệp thành công."
    }
  ];

  const timeSlots = [
    "08:00", "08:30", "09:00", "09:30", "10:00", "10:30",
    "14:00", "14:30", "15:00", "15:30", "16:00", "16:30",
    "19:00", "19:30", "20:00", "20:30", "21:00", "21:30"
  ];

  const consultationTypes = [
    { id: 'house', name: 'Phong thủy nhà ở', icon: '🏠' },
    { id: 'office', name: 'Phong thủy văn phòng', icon: '🏢' },
    { id: 'business', name: 'Phong thủy kinh doanh', icon: '💼' },
    { id: 'destiny', name: 'Tử vi - vận mệnh', icon: '🔮' },
    { id: 'wedding', name: 'Chọn ngày cưới', icon: '💒' },
    { id: 'general', name: 'Tư vấn tổng quát', icon: '🎯' }
  ];

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('vi-VN', {
      style: 'currency',
      currency: 'VND'
    }).format(price);
  };

  const handleBooking = () => {
    if (!selectedMaster || !consultationType || !selectedDate || !selectedTime) {
      alert('Vui lòng điền đầy đủ thông tin');
      return;
    }
    
    // Simulate booking process
    alert('Đặt lịch thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất.');
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-amber-50 via-yellow-50 to-orange-50">
      {/* Header */}
      <div className="bg-gradient-to-r from-amber-700 to-orange-700 text-white">
        <div className="max-w-md mx-auto px-4 py-6">
          <div className="flex items-center justify-between mb-4">
            <button
              onClick={onBack}
              className="text-amber-100 hover:text-white"
            >
              ← Quay lại
            </button>
            <h1 className="text-lg">Tư Vấn Phong Thủy</h1>
            <div className="w-6"></div>
          </div>
          
          <div className="text-center">
            <p className="text-amber-100 text-sm">Kết nối với chuyên gia phong thủy hàng đầu</p>
          </div>
        </div>
      </div>

      <div className="max-w-md mx-auto px-4 py-6 space-y-6">
        {/* Consultation Type */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4">Loại tư vấn</h2>
          <div className="grid grid-cols-2 gap-3">
            {consultationTypes.map((type) => (
              <button
                key={type.id}
                onClick={() => setConsultationType(type.id)}
                className={`p-3 rounded-lg border-2 text-left transition-all ${
                  consultationType === type.id
                    ? 'border-amber-300 bg-amber-50'
                    : 'border-gray-200 bg-white hover:border-gray-300'
                }`}
              >
                <div className="text-lg mb-1">{type.icon}</div>
                <div className={`text-sm ${
                  consultationType === type.id ? 'text-amber-800' : 'text-gray-700'
                }`}>
                  {type.name}
                </div>
              </button>
            ))}
          </div>
        </Card>

        {/* Masters List */}
        <div>
          <h2 className="text-gray-800 mb-4">Chọn chuyên gia</h2>
          <div className="space-y-4">
            {masters.map((master) => (
              <Card 
                key={master.id}
                className={`p-4 cursor-pointer transition-all ${
                  selectedMaster === master.id
                    ? 'border-amber-300 bg-amber-50 shadow-md'
                    : 'hover:shadow-md'
                }`}
                onClick={() => setSelectedMaster(master.id)}
              >
                <div className="flex items-start gap-4">
                  <div className="relative">
                    <div className="w-16 h-16 bg-gradient-to-br from-amber-200 to-orange-300 rounded-full flex items-center justify-center text-2xl">
                      {master.avatar}
                    </div>
                    <div className={`absolute -bottom-1 -right-1 w-4 h-4 rounded-full border-2 border-white ${
                      master.status === 'online' ? 'bg-green-500' : 'bg-yellow-500'
                    }`} />
                  </div>
                  
                  <div className="flex-1">
                    <div className="flex items-start justify-between mb-2">
                      <div>
                        <h3 className="text-gray-800">{master.name}</h3>
                        <p className="text-sm text-gray-600">{master.title}</p>
                      </div>
                      <div className="text-right">
                        <div className="flex items-center gap-1 mb-1">
                          <Star className="w-4 h-4 text-yellow-500 fill-current" />
                          <span className="text-sm text-gray-700">{master.rating}</span>
                        </div>
                        <p className="text-xs text-gray-500">{master.reviews} đánh giá</p>
                      </div>
                    </div>
                    
                    <div className="flex items-center gap-4 mb-3 text-sm text-gray-600">
                      <div className="flex items-center gap-1">
                        <Award className="w-4 h-4" />
                        <span>{master.experience}</span>
                      </div>
                      <div className="flex items-center gap-1">
                        <Users className="w-4 h-4" />
                        <span>{master.status === 'online' ? 'Trực tuyến' : 'Bận'}</span>
                      </div>
                    </div>
                    
                    <div className="flex flex-wrap gap-1 mb-3">
                      {master.specialties.slice(0, 2).map((specialty) => (
                        <Badge key={specialty} variant="secondary" className="text-xs">
                          {specialty}
                        </Badge>
                      ))}
                      {master.specialties.length > 2 && (
                        <Badge variant="outline" className="text-xs">
                          +{master.specialties.length - 2}
                        </Badge>
                      )}
                    </div>
                    
                    <p className="text-xs text-gray-600 mb-3">{master.description}</p>
                    
                    <div className="flex items-center justify-between">
                      <div className="flex gap-2">
                        <div className="flex items-center gap-1 text-xs text-gray-600">
                          <MessageCircle className="w-3 h-3" />
                          <span>{formatPrice(master.price.chat)}</span>
                        </div>
                        <div className="flex items-center gap-1 text-xs text-gray-600">
                          <Phone className="w-3 h-3" />
                          <span>{formatPrice(master.price.call)}</span>
                        </div>
                        <div className="flex items-center gap-1 text-xs text-gray-600">
                          <Video className="w-3 h-3" />
                          <span>{formatPrice(master.price.video)}</span>
                        </div>
                      </div>
                      <div className="flex gap-1">
                        {master.languages.map((lang) => (
                          <Badge key={lang} variant="outline" className="text-xs">
                            {lang}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  </div>
                </div>
              </Card>
            ))}
          </div>
        </div>

        {/* Consultation Method */}
        {selectedMaster && (
          <Card className="p-4">
            <h3 className="text-gray-800 mb-4">Hình thức tư vấn</h3>
            <div className="grid grid-cols-3 gap-3">
              <button className="p-3 border-2 border-gray-200 rounded-lg hover:border-amber-300 text-center">
                <MessageCircle className="w-6 h-6 text-gray-600 mx-auto mb-2" />
                <div className="text-xs text-gray-700 mb-1">Chat</div>
                <div className="text-xs text-gray-500">{formatPrice(masters.find(m => m.id === selectedMaster)?.price.chat || 0)}</div>
              </button>
              <button className="p-3 border-2 border-gray-200 rounded-lg hover:border-amber-300 text-center">
                <Phone className="w-6 h-6 text-gray-600 mx-auto mb-2" />
                <div className="text-xs text-gray-700 mb-1">Gọi điện</div>
                <div className="text-xs text-gray-500">{formatPrice(masters.find(m => m.id === selectedMaster)?.price.call || 0)}</div>
              </button>
              <button className="p-3 border-2 border-amber-300 bg-amber-50 rounded-lg text-center">
                <Video className="w-6 h-6 text-amber-600 mx-auto mb-2" />
                <div className="text-xs text-amber-700 mb-1">Video call</div>
                <div className="text-xs text-amber-600">{formatPrice(masters.find(m => m.id === selectedMaster)?.price.video || 0)}</div>
              </button>
            </div>
          </Card>
        )}

        {/* Date & Time Selection */}
        {selectedMaster && (
          <Card className="p-4">
            <h3 className="text-gray-800 mb-4">Chọn thời gian</h3>
            <div className="grid grid-cols-2 gap-4 mb-4">
              <div>
                <Label htmlFor="date">Ngày</Label>
                <Input
                  id="date"
                  type="date"
                  value={selectedDate}
                  onChange={(e) => setSelectedDate(e.target.value)}
                  min={new Date().toISOString().split('T')[0]}
                />
              </div>
              <div>
                <Label htmlFor="time">Giờ</Label>
                <Select onValueChange={setSelectedTime}>
                  <SelectTrigger>
                    <SelectValue placeholder="Chọn giờ" />
                  </SelectTrigger>
                  <SelectContent>
                    {timeSlots.map((time) => (
                      <SelectItem key={time} value={time}>{time}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>
          </Card>
        )}

        {/* Client Information */}
        {selectedMaster && selectedDate && selectedTime && (
          <Card className="p-4">
            <h3 className="text-gray-800 mb-4">Thông tin cá nhân</h3>
            <div className="space-y-4">
              <div>
                <Label htmlFor="name">Họ tên *</Label>
                <Input
                  id="name"
                  value={clientInfo.name}
                  onChange={(e) => setClientInfo(prev => ({ ...prev, name: e.target.value }))}
                  placeholder="Nhập họ tên của bạn"
                />
              </div>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="birthDate">Ngày sinh *</Label>
                  <Input
                    id="birthDate"
                    type="date"
                    value={clientInfo.birthDate}
                    onChange={(e) => setClientInfo(prev => ({ ...prev, birthDate: e.target.value }))}
                  />
                </div>
                <div>
                  <Label htmlFor="birthTime">Giờ sinh</Label>
                  <Input
                    id="birthTime"
                    type="time"
                    value={clientInfo.birthTime}
                    onChange={(e) => setClientInfo(prev => ({ ...prev, birthTime: e.target.value }))}
                  />
                </div>
              </div>
              
              <div>
                <Label htmlFor="gender">Giới tính *</Label>
                <Select onValueChange={(value) => setClientInfo(prev => ({ ...prev, gender: value }))}>
                  <SelectTrigger>
                    <SelectValue placeholder="Chọn giới tính" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="male">Nam</SelectItem>
                    <SelectItem value="female">Nữ</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              
              <div>
                <Label htmlFor="address">Địa chỉ hiện tại</Label>
                <Input
                  id="address"
                  value={clientInfo.address}
                  onChange={(e) => setClientInfo(prev => ({ ...prev, address: e.target.value }))}
                  placeholder="Nhập địa chỉ của bạn"
                />
              </div>
              
              <div>
                <Label htmlFor="issue">Vấn đề cần tư vấn</Label>
                <Textarea
                  id="issue"
                  value={clientInfo.issue}
                  onChange={(e) => setClientInfo(prev => ({ ...prev, issue: e.target.value }))}
                  placeholder="Mô tả vấn đề bạn muốn được tư vấn..."
                  rows={3}
                />
              </div>
            </div>
          </Card>
        )}

        {/* Booking Summary & Button */}
        {selectedMaster && selectedDate && selectedTime && (
          <Card className="p-4 bg-gradient-to-r from-amber-50 to-orange-50 border-amber-200">
            <h3 className="text-amber-900 mb-3">Tóm tắt đặt lịch</h3>
            <div className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-gray-600">Chuyên gia:</span>
                <span className="text-gray-800">{masters.find(m => m.id === selectedMaster)?.name}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Hình thức:</span>
                <span className="text-gray-800">Video call</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Thời gian:</span>
                <span className="text-gray-800">{selectedTime}, {selectedDate}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Chi phí:</span>
                <span className="text-amber-800">{formatPrice(masters.find(m => m.id === selectedMaster)?.price.video || 0)}</span>
              </div>
            </div>
            
            <Button
              onClick={handleBooking}
              className="w-full mt-4 bg-gradient-to-r from-amber-600 to-orange-600 hover:from-amber-700 hover:to-orange-700 text-white"
              disabled={!clientInfo.name || !clientInfo.birthDate || !clientInfo.gender}
            >
              <Calendar className="w-4 h-4 mr-2" />
              Đặt lịch tư vấn
            </Button>
          </Card>
        )}

        {/* FAQ */}
        <Card className="p-4">
          <h3 className="text-gray-800 mb-3">Câu hỏi thường gặp</h3>
          <div className="space-y-3 text-sm">
            <div>
              <h4 className="text-gray-700 mb-1">Tôi có thể hủy lịch hẹn không?</h4>
              <p className="text-gray-600">Bạn có thể hủy lịch hẹn trước 24 giờ để được hoàn tiền 100%.</p>
            </div>
            <div>
              <h4 className="text-gray-700 mb-1">Thông tin cá nhân có được bảo mật?</h4>
              <p className="text-gray-600">Chúng tôi cam kết bảo mật tuyệt đối thông tin cá nhân của khách hàng.</p>
            </div>
            <div>
              <h4 className="text-gray-700 mb-1">Có được ghi âm buổi tư vấn không?</h4>
              <p className="text-gray-600">Bạn có thể yêu cầu ghi âm để xem lại sau buổi tư vấn.</p>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}

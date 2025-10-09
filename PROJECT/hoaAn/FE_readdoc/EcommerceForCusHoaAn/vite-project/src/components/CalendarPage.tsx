import { useState } from 'react';
import { Button } from './ui/button';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { 
  ArrowLeft, 
  ChevronLeft, 
  ChevronRight, 
  Calendar as CalendarIcon,
  Filter,
  Flower2,
  Star,
  Heart,
  Gift,
  Home,
  Sparkles,
  Moon,
  Sun
} from 'lucide-react';

interface CalendarEvent {
  id: string;
  title: string;
  lunarDate: string;
  solarDate: string;
  type: 'festival' | 'ceremony' | 'good-day' | 'bad-day';
  category: 'gia-tien' | 'phat-giao' | 'phong-thuy' | 'le-hoi';
  description: string;
  color: string;
  recommendations?: string[];
}

interface CalendarPageProps {
  onBack: () => void;
}

export function CalendarPage({ onBack }: CalendarPageProps) {
  const [currentDate, setCurrentDate] = useState(new Date(2025, 0, 12)); // January 12, 2025
  const [viewMode, setViewMode] = useState<'month' | 'week' | 'day'>('month');
  const [selectedCategory, setSelectedCategory] = useState<string>('all');
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);

  // Sample lunar calendar events
  const calendarEvents: CalendarEvent[] = [
    {
      id: '1',
      title: 'Rằm tháng Giêng',
      lunarDate: '15/01/Ất Tỵ',
      solarDate: '2025-01-14',
      type: 'festival',
      category: 'phat-giao',
      description: 'Lễ cúng Phật đầu năm, cầu bình an và may mắn',
      color: 'bg-yellow-500',
      recommendations: ['Cúng Phật', 'Thắp hương', 'Ăn chay', 'Làm việc thiện']
    },
    {
      id: '2',
      title: 'Ngày tốt khai trương',
      lunarDate: '18/01/Ất Tỵ',
      solarDate: '2025-01-17',
      type: 'good-day',
      category: 'phong-thuy',
      description: 'Ngày hoàng đạo, thích hợp khai trương, khởi công',
      color: 'bg-green-500',
      recommendations: ['Khai trương', 'Khởi công', 'Ký hợp đồng', 'Mua sắm lớn']
    },
    {
      id: '3',
      title: 'Lễ cúng ông táo',
      lunarDate: '23/12/Giáp Thìn',
      solarDate: '2025-01-22',
      type: 'ceremony',
      category: 'gia-tien',
      description: 'Cúng tiễn ông táo về trời báo cáo',
      color: 'bg-red-500',
      recommendations: ['Cúng ông táo', 'Dọn dẹp nhà cửa', 'Chuẩn bị Tết']
    },
    {
      id: '4',
      title: 'Ngày xấu - Tuyệt nhật',
      lunarDate: '25/01/Ất Tỵ',
      solarDate: '2025-01-24',
      type: 'bad-day',
      category: 'phong-thuy',
      description: 'Ngày không nên làm việc quan trọng',
      color: 'bg-gray-500',
      recommendations: ['Tránh khai trương', 'Không cưới hỏi', 'Nghỉ ngơi']
    },
    {
      id: '5',
      title: 'Tết Nguyên Đán',
      lunarDate: '01/01/Ất Tỵ',
      solarDate: '2025-01-29',
      type: 'festival',
      category: 'le-hoi',
      description: 'Tết cổ truyền của dân tộc Việt Nam',
      color: 'bg-red-600',
      recommendations: ['Cúng tổ tiên', 'Đi chúc Tết', 'Mặc đồ đỏ', 'Li xì']
    },
    {
      id: '6',
      title: 'Mùng 2 Tết - Về nhà ngoại',
      lunarDate: '02/01/Ất Tỵ',
      solarDate: '2025-01-30',
      type: 'ceremony',
      category: 'gia-tien',
      description: 'Ngày về thăm họ ngoại theo truyền thống',
      color: 'bg-pink-500',
      recommendations: ['Về nhà ngoại', 'Mang quà biếu', 'Cúng gia tiên']
    },
    {
      id: '7',
      title: 'Khai hội đền - Ngày tốt cầu duyên',
      lunarDate: '05/01/Ất Tỵ',
      solarDate: '2025-02-02',
      type: 'good-day',
      category: 'phat-giao',
      description: 'Ngày tốt để cầu duyên, kết hôn',
      color: 'bg-purple-500',
      recommendations: ['Cầu duyên', 'Đi chùa', 'Cưới hỏi', 'Hẹn hò']
    }
  ];

  // Get events for current month
  const getCurrentMonthEvents = () => {
    return calendarEvents.filter(event => {
      const eventDate = new Date(event.solarDate);
      return eventDate.getMonth() === currentDate.getMonth() && 
             eventDate.getFullYear() === currentDate.getFullYear();
    }).filter(event => {
      if (selectedCategory === 'all') return true;
      return event.category === selectedCategory;
    });
  };

  // Generate calendar days
  const generateCalendarDays = () => {
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth();
    
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const startDate = new Date(firstDay);
    startDate.setDate(startDate.getDate() - firstDay.getDay());
    
    const days = [];
    for (let i = 0; i < 42; i++) {
      const date = new Date(startDate);
      date.setDate(startDate.getDate() + i);
      days.push(date);
    }
    
    return days;
  };

  const getEventForDate = (date: Date) => {
    return getCurrentMonthEvents().find(event => {
      const eventDate = new Date(event.solarDate);
      return eventDate.toDateString() === date.toDateString();
    });
  };

  const navigateMonth = (direction: 'prev' | 'next') => {
    const newDate = new Date(currentDate);
    if (direction === 'prev') {
      newDate.setMonth(newDate.getMonth() - 1);
    } else {
      newDate.setMonth(newDate.getMonth() + 1);
    }
    setCurrentDate(newDate);
  };

  const getEventTypeIcon = (type: CalendarEvent['type']) => {
    switch (type) {
      case 'festival': return <Star className="w-3 h-3" />;
      case 'ceremony': return <Flower2 className="w-3 h-3" />;
      case 'good-day': return <Sun className="w-3 h-3" />;
      case 'bad-day': return <Moon className="w-3 h-3" />;
      default: return <CalendarIcon className="w-3 h-3" />;
    }
  };

  const getCategoryName = (category: string) => {
    switch (category) {
      case 'gia-tien': return 'Gia tiên';
      case 'phat-giao': return 'Phật giáo';
      case 'phong-thuy': return 'Phong thủy';
      case 'le-hoi': return 'Lễ hội';
      default: return 'Tất cả';
    }
  };

  const monthNames = [
    'Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
    'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'
  ];

  const dayNames = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'];

  return (
    <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50">
      {/* Header */}
      <section className="bg-gradient-to-r from-amber-900 to-red-800 text-white py-8">
        <div className="max-w-6xl mx-auto px-4">
          <div className="flex items-center gap-4 mb-4">
            <Button
              variant="outline"
              size="icon"
              onClick={onBack}
              className="border-white/30 text-white hover:bg-white/10"
              aria-label="Quay lại trang trước"
            >
              <ArrowLeft className="w-4 h-4" />
            </Button>
            <div>
              <h1 className="text-3xl md:text-4xl">Lịch âm đầy đủ</h1>
              <p className="text-yellow-100">Xem các ngày tốt, lễ hội và nghi lễ theo truyền thống</p>
            </div>
          </div>
          
          {/* View Controls */}
          <div className="flex flex-wrap items-center gap-4">
            <div className="flex bg-white/10 rounded-lg p-1">
              {(['month', 'week', 'day'] as const).map((mode) => (
                <Button
                  key={mode}
                  variant={viewMode === mode ? 'default' : 'ghost'}
                  size="sm"
                  onClick={() => setViewMode(mode)}
                  className={`text-white ${viewMode === mode ? 'bg-white/20' : 'hover:bg-white/10'}`}
                >
                  {mode === 'month' ? 'Tháng' : mode === 'week' ? 'Tuần' : 'Ngày'}
                </Button>
              ))}
            </div>

            <Select value={selectedCategory} onValueChange={setSelectedCategory}>
              <SelectTrigger className="w-48 border-white/30 text-white bg-white/10">
                <Filter className="w-4 h-4 mr-2" />
                <SelectValue placeholder="Lọc theo danh mục" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả</SelectItem>
                <SelectItem value="gia-tien">Gia tiên</SelectItem>
                <SelectItem value="phat-giao">Phật giáo</SelectItem>
                <SelectItem value="phong-thuy">Phong thủy</SelectItem>
                <SelectItem value="le-hoi">Lễ hội</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </div>
      </section>

      <div className="max-w-6xl mx-auto px-4 py-8">
        <div className="grid lg:grid-cols-4 gap-8">
          {/* Main Calendar */}
          <div className="lg:col-span-3">
            <Card className="border-2 border-amber-200 shadow-lg">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2 text-amber-900">
                    <CalendarIcon className="w-5 h-5" />
                    {monthNames[currentDate.getMonth()]} {currentDate.getFullYear()}
                  </CardTitle>
                  
                  <div className="flex items-center gap-2">
                    <Button
                      variant="outline"
                      size="icon"
                      onClick={() => navigateMonth('prev')}
                      className="border-amber-300 text-amber-700 hover:bg-amber-50"
                      aria-label="Tháng trước"
                    >
                      <ChevronLeft className="w-4 h-4" />
                    </Button>
                    
                    <Button
                      variant="outline"
                      onClick={() => setCurrentDate(new Date())}
                      className="border-amber-300 text-amber-700 hover:bg-amber-50"
                    >
                      Hôm nay
                    </Button>
                    
                    <Button
                      variant="outline"
                      size="icon"
                      onClick={() => navigateMonth('next')}
                      className="border-amber-300 text-amber-700 hover:bg-amber-50"
                      aria-label="Tháng sau"
                    >
                      <ChevronRight className="w-4 h-4" />
                    </Button>
                  </div>
                </div>
              </CardHeader>
              
              <CardContent>
                {/* Calendar Grid */}
                <div className="grid grid-cols-7 gap-1 mb-4">
                  {dayNames.map((day) => (
                    <div key={day} className="p-3 text-center text-amber-900">
                      {day}
                    </div>
                  ))}
                </div>
                
                <div className="grid grid-cols-7 gap-1">
                  {generateCalendarDays().map((date, index) => {
                    const event = getEventForDate(date);
                    const isCurrentMonth = date.getMonth() === currentDate.getMonth();
                    const isToday = date.toDateString() === new Date().toDateString();
                    
                    return (
                      <div
                        key={index}
                        className={`
                          relative p-2 h-24 border border-amber-100 rounded-lg cursor-pointer
                          transition-all duration-200 hover:shadow-md
                          ${isCurrentMonth ? 'bg-white' : 'bg-gray-50 text-gray-400'}
                          ${isToday ? 'ring-2 ring-amber-400 bg-amber-50' : ''}
                          ${event ? 'hover:scale-105' : 'hover:bg-amber-50'}
                        `}
                        onClick={() => event && setSelectedEvent(event)}
                        role="button"
                        tabIndex={0}
                        aria-label={`${date.getDate()} ${monthNames[date.getMonth()]} ${date.getFullYear()}${event ? `, có sự kiện: ${event.title}` : ''}`}
                        onKeyDown={(e) => {
                          if (e.key === 'Enter' || e.key === ' ') {
                            event && setSelectedEvent(event);
                          }
                        }}
                      >
                        <div className={`text-sm ${isToday ? 'text-amber-900' : ''}`}>
                          {date.getDate()}
                        </div>
                        
                        {event && (
                          <div className={`
                            absolute bottom-1 left-1 right-1 p-1 rounded text-xs text-white
                            ${event.color} opacity-90 hover:opacity-100
                            flex items-center gap-1 transition-opacity
                          `}>
                            {getEventTypeIcon(event.type)}
                            <span className="truncate text-xs">{event.title}</span>
                          </div>
                        )}
                      </div>
                    );
                  })}
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Events Sidebar */}
          <div className="space-y-6">
            {/* Legend */}
            <Card className="border-2 border-amber-200">
              <CardHeader>
                <CardTitle className="text-amber-900 text-lg">Chú thích</CardTitle>
              </CardHeader>
              <CardContent className="space-y-3">
                <div className="flex items-center gap-2">
                  <div className="w-4 h-4 bg-yellow-500 rounded"></div>
                  <span className="text-sm">Lễ hội</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="w-4 h-4 bg-red-500 rounded"></div>
                  <span className="text-sm">Nghi lễ</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="w-4 h-4 bg-green-500 rounded"></div>
                  <span className="text-sm">Ngày tốt</span>
                </div>
                <div className="flex items-center gap-2">
                  <div className="w-4 h-4 bg-gray-500 rounded"></div>
                  <span className="text-sm">Ngày xấu</span>
                </div>
              </CardContent>
            </Card>

            {/* Current Month Events */}
            <Card className="border-2 border-amber-200">
              <CardHeader>
                <CardTitle className="text-amber-900 text-lg">
                  Sự kiện tháng này
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-3 max-h-96 overflow-y-auto">
                {getCurrentMonthEvents().map((event) => (
                  <div
                    key={event.id}
                    className={`
                      p-3 rounded-lg border-l-4 cursor-pointer transition-all duration-200
                      hover:shadow-md hover:scale-105 bg-white
                    `}
                    style={{ borderLeftColor: event.color.replace('bg-', '#') }}
                    onClick={() => setSelectedEvent(event)}
                    role="button"
                    tabIndex={0}
                    aria-label={`Xem chi tiết sự kiện ${event.title}`}
                    onKeyDown={(e) => {
                      if (e.key === 'Enter' || e.key === ' ') {
                        setSelectedEvent(event);
                      }
                    }}
                  >
                    <div className="flex items-start justify-between">
                      <div className="flex-1">
                        <h3 className="text-amber-900 text-sm mb-1">{event.title}</h3>
                        <p className="text-xs text-gray-600 mb-1">{event.lunarDate}</p>
                        <Badge variant="outline" className="text-xs">
                          {getCategoryName(event.category)}
                        </Badge>
                      </div>
                      <div className="text-amber-600">
                        {getEventTypeIcon(event.type)}
                      </div>
                    </div>
                  </div>
                ))}
              </CardContent>
            </Card>
          </div>
        </div>

        {/* Event Detail Modal/Card */}
        {selectedEvent && (
          <div 
            className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50"
            onClick={() => setSelectedEvent(null)}
            role="dialog"
            aria-modal="true"
            aria-labelledby="event-title"
          >
            <Card 
              className="max-w-md w-full border-2 border-amber-300 shadow-2xl"
              onClick={(e) => e.stopPropagation()}
            >
              <CardHeader className="bg-gradient-to-r from-amber-600 to-yellow-600 text-white">
                <div className="flex items-center justify-between">
                  <CardTitle id="event-title" className="flex items-center gap-2">
                    {getEventTypeIcon(selectedEvent.type)}
                    {selectedEvent.title}
                  </CardTitle>
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => setSelectedEvent(null)}
                    className="text-white hover:bg-white/20"
                    aria-label="Đóng chi tiết sự kiện"
                  >
                    ×
                  </Button>
                </div>
              </CardHeader>
              
              <CardContent className="p-6 space-y-4">
                <div>
                  <h3 className="text-amber-900 mb-2">Thông tin sự kiện</h3>
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-600">Ngày âm lịch:</span>
                      <span>{selectedEvent.lunarDate}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Ngày dương lịch:</span>
                      <span>{new Date(selectedEvent.solarDate).toLocaleDateString('vi-VN')}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Danh mục:</span>
                      <Badge variant="outline">{getCategoryName(selectedEvent.category)}</Badge>
                    </div>
                  </div>
                </div>
                
                <div>
                  <h3 className="text-amber-900 mb-2">Mô tả</h3>
                  <p className="text-gray-700 text-sm">{selectedEvent.description}</p>
                </div>
                
                {selectedEvent.recommendations && (
                  <div>
                    <h3 className="text-amber-900 mb-2">Nên làm</h3>
                    <div className="flex flex-wrap gap-2">
                      {selectedEvent.recommendations.map((rec, index) => (
                        <Badge key={index} className="bg-green-100 text-green-800 text-xs">
                          {rec}
                        </Badge>
                      ))}
                    </div>
                  </div>
                )}
                
                <Button 
                  className="w-full bg-red-600 hover:bg-red-700 text-white"
                  onClick={() => setSelectedEvent(null)}
                >
                  Đặt sản phẩm cho ngày này
                </Button>
              </CardContent>
            </Card>
          </div>
        )}
      </div>
    </div>
  );
}

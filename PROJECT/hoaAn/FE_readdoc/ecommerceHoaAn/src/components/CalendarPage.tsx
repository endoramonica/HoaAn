// components/calendar/CalendarPage.tsx
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { convertSolarToLunar, type LunarDateResult } from '@/lib/services/lunarCalendarService';
import CalendarHeader from '../pages/calendar/CalendarHeader';
import CalendarGrid from '../pages/calendar/CalendarGrid';
import { EventSidebar } from '../pages/calendar/EventSidebar';

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

export default function CalendarPage() {
  const navigate = useNavigate();
  const [currentDate, setCurrentDate] = useState(new Date());
  const [viewMode, setViewMode] = useState<'month' | 'week' | 'day'>('month');
  const [selectedCategory, setSelectedCategory] = useState<string>('all');
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);
  const [selectedDate, setSelectedDate] = useState<Date | null>(null);
  const [lunarData, setLunarData] = useState<LunarDateResult | null>(null);
  const [isLoadingLunar, setIsLoadingLunar] = useState(false);
  const [lunarCache, setLunarCache] = useState<Record<string, LunarDateResult>>({});

  // Sample events for testing - sẽ được truyền từ API thực tế sau
  const [events] = useState<CalendarEvent[]>([
    {
      id: '1',
      title: 'Tết Nguyên Đán',
      lunarDate: '1/1',
      solarDate: '2025-01-29',
      type: 'festival',
      category: 'le-hoi',
      description: 'Tết Nguyên Đán - Ngày đầu năm mới',
      color: 'red',
      recommendations: ['Nên đặt lịch tư vấn phong thủy']
    },
    {
      id: '2',
      title: 'Rằm tháng Giêng',
      lunarDate: '15/1',
      solarDate: '2025-02-12',
      type: 'ceremony',
      category: 'phat-giao',
      description: 'Rằm tháng Giêng - Ngày lễ Phật giáo',
      color: 'yellow',
      recommendations: ['Nên đi chùa']
    },
    {
      id: '3',
      title: 'Ngày tốt để khởi công',
      lunarDate: '8/1',
      solarDate: '2025-02-05',
      type: 'good-day',
      category: 'phong-thuy',
      description: 'Ngày tốt để khởi công xây dựng',
      color: 'green',
      recommendations: ['Nên khởi công']
    }
  ]);

  // Hàm chuyển đổi âm lịch có cache
  const getLunarForDate = async (date: Date): Promise<LunarDateResult | null> => {
    const key = date.toISOString().split('T')[0];
    if (lunarCache[key]) return lunarCache[key];

    try {
      const result = await convertSolarToLunar(date);
      setLunarCache(prev => ({ ...prev, [key]: result }));
      return result;
    } catch (err) {
      console.error(err);
      return null;
    }
  };

  const handleDateClick = async (date: Date, event?: CalendarEvent) => {
    setSelectedDate(date);
    setSelectedEvent(event || null);
    setIsLoadingLunar(true);
    const result = await getLunarForDate(date);
    setLunarData(result);
    setIsLoadingLunar(false);
  };

  const handleBookingClick = async (date: Date, event: CalendarEvent) => {
    setSelectedDate(date);
    setSelectedEvent(event);
    setIsLoadingLunar(true);
    const result = await getLunarForDate(date);
    setLunarData(result);
    setIsLoadingLunar(false);
  };

  const navigateMonth = (direction: 'prev' | 'next') => {
    const newDate = new Date(currentDate);
    newDate.setMonth(newDate.getMonth() + (direction === 'next' ? 1 : -1));
    setCurrentDate(newDate);
  };

  // Tải lại cache âm lịch khi đổi tháng
  useEffect(() => {
    // Có thể preload cả tháng nếu cần
  }, [currentDate]);

  return (
    <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50">
      <CalendarHeader
        currentDate={currentDate}
        viewMode={viewMode}
        selectedCategory={selectedCategory}
        onViewModeChange={setViewMode}
        onCategoryChange={setSelectedCategory}
        onNavigateBack={() => navigate('/')}
        onNavigateMonth={navigateMonth}
        onToday={() => setCurrentDate(new Date())}
      />

      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
          <div className="lg:col-span-3">
            <CalendarGrid
              currentDate={currentDate}
              events={events}
              lunarCache={lunarCache}
              selectedCategory={selectedCategory}
              onDateClick={handleDateClick}
              onBookingClick={handleBookingClick}
            />
          </div>

          <div className="lg:col-span-1 h-fit sticky top-4">
            <EventSidebar
              events={events.filter(e => {
                const d = new Date(e.solarDate);
                return d.getMonth() === currentDate.getMonth() && d.getFullYear() === currentDate.getFullYear();
              }).filter(e => selectedCategory === 'all' || e.category === selectedCategory)}
              selectedDate={selectedDate}
              lunarData={lunarData}
              isLoadingLunar={isLoadingLunar}
              selectedEvent={selectedEvent}
              onEventSelect={setSelectedEvent}
              onCloseEvent={() => setSelectedEvent(null)}
            />
          </div>
        </div>
      </div>
    </div>
  );
}
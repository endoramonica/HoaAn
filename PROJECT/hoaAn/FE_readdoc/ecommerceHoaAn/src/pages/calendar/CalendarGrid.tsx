import { Calendar as CalendarIcon, Star, Flower2, Sun, Moon, BookOpen } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import type { CalendarEvent } from './types';
import { getLunarDate } from '@/lib/services/lunarCalendarService';
import { useState } from 'react';

interface CalendarGridProps {
  currentDate: Date;
  events: CalendarEvent[];
  lunarCache: Record<string, any>;
  selectedCategory: string;
  viewMode?: 'month' | 'week' | 'day';
  onDateClick: (date: Date, event?: CalendarEvent) => void;
  onBookingClick?: (date: Date, event: CalendarEvent) => void;
}

export default function CalendarGrid({
  currentDate,
  events,
  onDateClick,
  onBookingClick
}: CalendarGridProps) {
  const [hoveredDateKey, setHoveredDateKey] = useState<string | null>(null);

  const getEventTypeIcon = (type: CalendarEvent['type']) => {
    switch (type) {
      case 'festival': return <Star className="w-3 h-3" />;
      case 'ceremony': return <Flower2 className="w-3 h-3" />;
      case 'good-day': return <Sun className="w-3 h-3" />;
      case 'bad-day': return <Moon className="w-3 h-3" />;
      default: return <CalendarIcon className="w-3 h-3" />;
    }
  };

  const generateCalendarDays = () => {
    const year = currentDate.getFullYear();
    const month = currentDate.getMonth();
    const firstDay = new Date(year, month, 1);

    const start = new Date(firstDay);
    start.setDate(start.getDate() - start.getDay());

    const days = [];
    for (let i = 0; i < 42; i++) {
      const date = new Date(start);
      date.setDate(start.getDate() + i);
      days.push(date);
    }
    return days;
  };

  const getEventForDate = (date: Date) => {
    return events.find(event => {
      const eventDate = new Date(event.solarDate);
      return eventDate.toDateString() === date.toDateString();
    });
  };

  const dayNames = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'];
  const monthNames = [
    'Tháng 1','Tháng 2','Tháng 3','Tháng 4','Tháng 5','Tháng 6',
    'Tháng 7','Tháng 8','Tháng 9','Tháng 10','Tháng 11','Tháng 12'
  ];

  return (
    <Card className="border-none shadow-xl bg-white/80 backdrop-blur-sm overflow-hidden">
      {/* Header */}
      <div className="p-4 bg-amber-100/50 border-b border-amber-100">
        <h2 className="text-lg font-semibold text-slate-800">
          {monthNames[currentDate.getMonth()]} {currentDate.getFullYear()}
        </h2>
      </div>

      <CardContent className="p-0">

        {/* Weekday Row */}
        <div className="grid grid-cols-7 bg-amber-100/50 border-b border-amber-100">
          {dayNames.map((day, i) => (
            <div
              key={day}
              className={`p-3 text-center text-sm font-semibold
              ${i === 0 || i === 6 ? 'text-red-600' : 'text-slate-700'}`}
            >
              {day}
            </div>
          ))}
        </div>

        {/* Calendar Grid */}
        <div className="grid grid-cols-7 auto-rows-[140px]">
          {generateCalendarDays().map((date, index) => {
            const event = getEventForDate(date);
            const isToday = date.toDateString() === new Date().toDateString();
            const isCurrentMonth = date.getMonth() === currentDate.getMonth();
            const dateKey = date.toISOString().split('T')[0];
            const isHovered = hoveredDateKey === dateKey;

            const lunar = getLunarDate(date.getDate(), date.getMonth() + 1, date.getFullYear());
            const lunarDisplay = lunar.day === 1 ? `${lunar.day}/${lunar.month}` : lunar.day;

            return (
              <div
                key={index}
                onMouseEnter={() => setHoveredDateKey(dateKey)}
                onMouseLeave={() => setHoveredDateKey(null)}
                onClick={() => onDateClick(date, event)}
                className={`
                  border-r border-b border-slate-100 px-3 py-3 cursor-pointer
                  flex flex-col items-center justify-center gap-2 transition-all duration-150
                  relative group

                  ${isCurrentMonth ? 'bg-white hover:bg-amber-50' : 'bg-slate-50 text-slate-400'}
                  ${isToday ? 'bg-red-50 ring-1 ring-red-300' : ''}
                `}
              >

                {/* Date Number */}
                <span
                  className={`
                    w-8 h-8 flex items-center justify-center rounded-full
                    text-sm font-semibold
                    ${isToday ? 'bg-red-600 text-white shadow' : 'text-slate-700'}
                  `}
                >
                  {date.getDate()}
                </span>

                {/* Event Tag */}
                {event && (
                  <div
                    className={`
                      px-2 py-1 rounded text-[10px] font-medium text-white shadow-sm
                      truncate flex items-center gap-1 max-w-full
                      ${event.color}
                    `}
                  >
                    {getEventTypeIcon(event.type)}
                    <span className="truncate">{event.title}</span>
                  </div>
                )}

                {/* Lunar date */}
                <div
                  className={`
                    text-xs font-medium
                    ${isCurrentMonth
                      ? lunar.day === 1 ? 'text-red-500 font-bold' : 'text-slate-400'
                      : 'text-slate-300'}
                  `}
                >
                  {lunarDisplay}
                </div>

                {/* Booking Button - Hover Animation */}
                {event && isHovered && (
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      onBookingClick?.(date, event);
                    }}
                    className="
                      absolute inset-0 flex items-center justify-center
                      bg-gradient-to-br from-yellow-400 to-yellow-500 rounded
                      opacity-0 group-hover:opacity-100 hover:opacity-100
                      transition-all duration-200 ease-out
                      animate-in fade-in zoom-in-50
                      shadow-lg hover:shadow-xl
                    "
                    title="Đặt lịch"
                  >
                    <BookOpen className="w-6 h-6 text-white drop-shadow-lg" />
                  </button>
                )}

              </div>
            );
          })}
        </div>
      </CardContent>
    </Card>
  );
}

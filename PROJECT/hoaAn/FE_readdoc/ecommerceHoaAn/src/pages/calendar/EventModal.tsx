import { X, Loader2, Sun } from 'lucide-react';
import { Button } from '../../components/ui/button';
import { Badge } from '../../components/ui/badge';
import type { CalendarEvent } from './types';
import { type LunarDateResult } from '../../lib/services/lunarCalendarService';

interface EventModalProps {
  isOpen: boolean;
  date: Date | null;
  event: CalendarEvent | null;
  lunarInfo: LunarDateResult | null;
  isLoading: boolean;
  error: string | null;
  onClose: () => void;
  onRetry: () => void;
}

export function EventModal({
  isOpen,
  date,
  event,
  lunarInfo,
  isLoading,
  error,
  onClose,
  onRetry
}: EventModalProps) {
  if (!isOpen || !date) return null;

  const getCategoryName = (category: string) => {
    switch (category) {
      case 'gia-tien': return 'Gia tiên';
      case 'phat-giao': return 'Phật giáo';
      case 'phong-thuy': return 'Phong thủy';
      case 'le-hoi': return 'Lễ hội';
      default: return 'Tất cả';
    }
  };

  return (
    <div 
      className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm animate-in fade-in duration-200"
      onClick={onClose}
    >
      <div 
        className="w-full max-w-lg bg-white rounded-xl shadow-2xl overflow-hidden animate-in zoom-in-95 duration-200"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Modal Header */}
        <div className="relative bg-[url('https://picsum.photos/800/200?blur=2')] bg-cover bg-center h-32 flex items-center justify-center">
          <div className="absolute inset-0 bg-gradient-to-t from-amber-900/80 to-transparent"></div>
          <Button 
            variant="ghost" 
            size="icon" 
            onClick={onClose}
            className="absolute top-2 right-2 text-white/80 hover:bg-white/20 hover:text-white rounded-full"
          >
            <X className="w-5 h-5" />
          </Button>
          
          <div className="relative text-center z-10">
            <div className="inline-block px-4 py-1 rounded-full bg-red-600/90 text-white text-xs font-bold uppercase tracking-wider mb-2 shadow-sm">
              Dương Lịch
            </div>
            <h2 className="text-4xl font-bold text-white shadow-md font-serif">
              {date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })}
            </h2>
          </div>
        </div>

        {/* Modal Content */}
        <div className="p-6">
          {isLoading ? (
            <div className="flex flex-col items-center justify-center py-12 space-y-4 text-amber-600">
              <Loader2 className="w-10 h-10 animate-spin" />
              <p className="text-sm font-medium">Đang luận giải thiên cơ...</p>
            </div>
          ) : error ? (
            <div className="text-center py-8">
              <div className="bg-red-50 text-red-600 p-4 rounded-lg mb-4 text-sm">
                {error}
              </div>
              <Button onClick={onRetry}>Thử lại</Button>
            </div>
          ) : (
            <div className="space-y-6">
              {/* Lunar Date Display */}
              {lunarInfo && (
                <div className="text-center">
                  <div className="text-xs font-semibold text-slate-400 uppercase tracking-wider mb-1">Âm Lịch</div>
                  <div className="flex items-center justify-center gap-6">
                    <div className="flex flex-col">
                      <span className="text-5xl font-serif font-bold text-amber-600">{lunarInfo.day}</span>
                      <span className="text-xs text-amber-800 font-medium">Ngày</span>
                    </div>
                    <div className="h-12 w-px bg-amber-200"></div>
                    <div className="flex flex-col">
                      <span className="text-5xl font-serif font-bold text-amber-600">{lunarInfo.month}</span>
                      <span className="text-xs text-amber-800 font-medium">Tháng {lunarInfo.leap ? '(Nhuận)' : ''}</span>
                    </div>
                    <div className="h-12 w-px bg-amber-200"></div>
                    <div className="flex flex-col">
                      <span className="text-5xl font-serif font-bold text-amber-600">{lunarInfo.year}</span>
                      <span className="text-xs text-amber-800 font-medium">Năm</span>
                    </div>
                  </div>

                  {/* Can Chi Details */}
                  <div className="mt-6 grid grid-cols-3 gap-2 bg-amber-50 rounded-lg p-3 border border-amber-100">
                    <div className="text-center">
                      <p className="text-xs text-slate-500">Ngày</p>
                      <p className="font-semibold text-slate-800">{lunarInfo.canChiDay}</p>
                    </div>
                    <div className="text-center border-x border-amber-200">
                      <p className="text-xs text-slate-500">Tháng</p>
                      <p className="font-semibold text-slate-800">{lunarInfo.canChiMonth}</p>
                    </div>
                    <div className="text-center">
                      <p className="text-xs text-slate-500">Năm</p>
                      <p className="font-semibold text-slate-800">{lunarInfo.canChiYear}</p>
                    </div>
                  </div>
                  
                   <div className="mt-2 text-center">
                      <span className="inline-flex items-center rounded-full bg-amber-100 px-2.5 py-0.5 text-xs font-medium text-amber-800">
                         Tiết: {lunarInfo.tietKhi}
                      </span>
                   </div>
                </div>
              )}

              {/* Event Details (if any) */}
              {event && (
                <div className="border-t border-slate-100 pt-4 animate-in slide-in-from-bottom-4 duration-500">
                  <div className="flex items-center gap-2 mb-2">
                     <Badge>{getCategoryName(event.category)}</Badge>
                     {event.recommendations && <span className="text-xs text-green-600 font-medium flex items-center gap-1"><Sun className="w-3 h-3"/> Nên làm</span>}
                  </div>
                  <h3 className="text-xl font-bold text-slate-800 mb-2 font-serif">{event.title}</h3>
                  <p className="text-slate-600 text-sm leading-relaxed mb-4">
                    {event.description}
                  </p>
                  
                  {event.recommendations && (
                    <div className="flex flex-wrap gap-2">
                      {event.recommendations.map((rec: string, i: number) => (
                        <span key={i} className="px-2 py-1 bg-green-50 text-green-700 text-xs rounded border border-green-100">
                          {rec}
                        </span>
                      ))}
                    </div>
                  )}
                </div>
              )}
              
              {!event && lunarInfo && (
                 <div className="border-t border-slate-100 pt-4 text-center">
                    <p className="text-slate-500 text-sm italic">Không có sự kiện đặc biệt được ghi nhận cho ngày này.</p>
                 </div>
              )}

              <div className="pt-2">
                <Button className="w-full bg-red-600 hover:bg-red-700 h-12 text-lg shadow-md shadow-red-200" onClick={onClose}>
                  Đóng
                </Button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
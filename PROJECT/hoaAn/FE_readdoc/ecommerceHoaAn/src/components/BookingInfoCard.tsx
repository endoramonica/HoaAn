import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Calendar, Clock, MapPin, FileText } from 'lucide-react';
import type { BookingInfo } from '../lib/hooks/useBookingInfo';
import { parseServiceNotes } from '../lib/hooks/useBookingInfo';

interface BookingInfoCardProps {
  bookingInfo: BookingInfo;
}

export function BookingInfoCard({ bookingInfo }: BookingInfoCardProps) {
  const parsed = parseServiceNotes(bookingInfo.serviceNotes);

  return (
    <Card className="border-amber-200 bg-gradient-to-br from-amber-50 to-yellow-50 shadow-md">
      <CardHeader className="pb-3 border-b border-amber-100/50">
        <CardTitle className="text-sm text-amber-900 font-bold uppercase tracking-wide">
          Thông tin đặt lịch
        </CardTitle>
      </CardHeader>
      <CardContent className="pt-4 space-y-3">
        {/* Sự kiện */}
        {parsed.eventTitle && (
          <div className="flex items-start gap-3">
            <FileText className="w-4 h-4 text-amber-600 mt-0.5 flex-shrink-0" />
            <div className="flex-1 min-w-0">
              <p className="text-xs text-slate-600">Sự kiện</p>
              <p className="text-sm font-semibold text-slate-900">{parsed.eventTitle}</p>
            </div>
          </div>
        )}

        {/* Ngày dự kiến */}
        <div className="flex items-start gap-3">
          <Calendar className="w-4 h-4 text-amber-600 mt-0.5 flex-shrink-0" />
          <div className="flex-1 min-w-0">
            <p className="text-xs text-slate-600">Ngày dự kiến</p>
            <p className="text-sm font-semibold text-slate-900">
              {new Date(bookingInfo.serviceDate).toLocaleDateString('vi-VN', {
                weekday: 'long',
                year: 'numeric',
                month: 'long',
                day: 'numeric',
              })}
            </p>
            {parsed.lunarDate && (
              <p className="text-xs text-slate-500 mt-1">Âm lịch: {parsed.lunarDate}</p>
            )}
          </div>
        </div>

        {/* Thời lượng */}
        <div className="flex items-start gap-3">
          <Clock className="w-4 h-4 text-amber-600 mt-0.5 flex-shrink-0" />
          <div className="flex-1 min-w-0">
            <p className="text-xs text-slate-600">Thời lượng</p>
            <p className="text-sm font-semibold text-slate-900">{bookingInfo.serviceDuration}</p>
          </div>
        </div>

        {/* Địa điểm */}
        <div className="flex items-start gap-3">
          <MapPin className="w-4 h-4 text-amber-600 mt-0.5 flex-shrink-0" />
          <div className="flex-1 min-w-0">
            <p className="text-xs text-slate-600">Địa điểm</p>
            <p className="text-sm font-semibold text-slate-900">{bookingInfo.serviceLocation}</p>
          </div>
        </div>

        {/* Ghi chú thêm */}
        {parsed.additionalNotes && (
          <div className="pt-2 border-t border-amber-200/50">
            <p className="text-xs text-slate-600 mb-1">Ghi chú thêm</p>
            <p className="text-sm text-slate-700 whitespace-pre-wrap">{parsed.additionalNotes}</p>
          </div>
        )}

        {/* Thông tin khách hàng */}
        <div className="pt-2 border-t border-amber-200/50 space-y-1">
          <p className="text-xs text-slate-600">Khách hàng</p>
          <p className="text-sm text-slate-900">
            <span className="font-semibold">{bookingInfo.customerName}</span>
            {' • '}
            <span className="text-slate-600">{bookingInfo.customerPhone}</span>
          </p>
          <p className="text-sm text-slate-600">{bookingInfo.customerEmail}</p>
        </div>
      </CardContent>
    </Card>
  );
}

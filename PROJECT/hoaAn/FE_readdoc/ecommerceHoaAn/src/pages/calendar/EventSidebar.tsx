import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Loader2, Calendar, AlertCircle, CheckCircle } from 'lucide-react';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';
import type { CalendarEvent } from './types';
import { type LunarDateResult } from '../../lib/services/lunarCalendarService';
import { calendarBookingService } from '../../lib/services/calendarBookingService';
import { cartService, transformCustomizationState } from '../../lib/services/cartService';
import { customizationStateService } from '../../lib/services/customizationStateService';
import { BookingDialog, type BookingFormData } from './BookingDialog';
import { useApp } from '../../lib/contexts/AppContext';

interface EventSidebarProps {
  events: CalendarEvent[];
  selectedDate: Date | null;
  lunarData: LunarDateResult | null;
  isLoadingLunar: boolean;
  selectedEvent: CalendarEvent | null;
  onEventSelect: (event: CalendarEvent) => void;
  onCloseEvent: () => void;
}

export function EventSidebar({
  events,
  selectedDate,
  lunarData,
  isLoadingLunar,
  selectedEvent,
  onEventSelect
}: EventSidebarProps) {
  const navigate = useNavigate();
  const { selectedServiceProductId } = useApp();
  const [isBooking, setIsBooking] = useState(false);
  const [bookingResult, setBookingResult] = useState<{ success: boolean; orderId?: string; message: string } | null>(null);
  const [showBookingDialog, setShowBookingDialog] = useState(false);

  const handleBookingClick = () => {
    if (!selectedDate || !lunarData) {
      toast.error('Vui lòng chọn ngày');
      return;
    }
    if (!selectedServiceProductId) {
      toast.error('Vui lòng chọn dịch vụ trước khi đặt lịch');
      return;
    }
    setShowBookingDialog(true);
  };

  const handleBookingConfirm = async (formData: BookingFormData) => {
    if (!selectedDate || !lunarData) {
      toast.error('Vui lòng chọn ngày');
      return;
    }
    if (!selectedServiceProductId) {
      toast.error('Vui lòng chọn dịch vụ trước khi đặt lịch');
      return;
    }

    setIsBooking(true);
    try {
      // Step 1: Retrieve customization state if available
      const customizationState = customizationStateService.getCustomizationState(selectedServiceProductId);
      const customizations = transformCustomizationState(customizationState);

      // Step 2: Add service product to cart with customizations
      console.log('[EventSidebar] Adding product to cart:', selectedServiceProductId);
      console.log('[EventSidebar] Customizations:', customizations);
      await cartService.addItem({
        productId: selectedServiceProductId,
        quantity: 1,
        customizations,
      });
      console.log('[EventSidebar] Product added to cart successfully');

      // Step 3: Get updated cart to get cart ID
      const cartResponse = await cartService.getCart();
      const cartId = cartResponse?.data?.id || cartResponse?.data?.cartId;
      
      if (!cartId) {
        toast.error('Không thể lấy ID giỏ hàng. Vui lòng thử lại.');
        setIsBooking(false);
        return;
      }

      console.log('[EventSidebar] Cart ID:', cartId);

      // Step 4: Create booking
      const event = selectedEvent || {
        id: `event-${selectedDate.toISOString()}`,
        title: 'Đặt lịch tư vấn',
        lunarDate: `${lunarData.day}/${lunarData.month}`,
        solarDate: selectedDate.toISOString().split('T')[0],
        type: 'ceremony' as const,
        category: 'phong-thuy' as const,
        description: 'Đặt lịch tư vấn dịch vụ',
        color: 'bg-amber-500',
      };

      const result = await calendarBookingService.createBookingFromEvent({
        event,
        lunarData,
        selectedDate,
        customerName: formData.customerName,
        customerPhone: formData.customerPhone,
        customerEmail: formData.customerEmail,
        serviceDate: formData.serviceDate,
        serviceDuration: formData.serviceDuration,
        serviceLocation: formData.serviceLocation,
        serviceNotes: formData.serviceNotes,
        serviceProductId: selectedServiceProductId,
        cartId: cartId,
        customizationState: formData.customizationState,
      });

      setBookingResult(result);
      setShowBookingDialog(false);

      if (result.success) {
        toast.success(result.message);
        // Redirect đến checkout page
        setTimeout(() => {
          navigate('/checkout');
        }, 1000);
      } else {
        toast.error(result.message);
      }
    } catch (error) {
      console.error('Booking error:', error);
      toast.error('Có lỗi xảy ra khi đặt lịch');
    } finally {
      setIsBooking(false);
    }
  };

  const getCategoryColor = (category: string) => {
    switch (category) {
      case 'gia-tien': return 'bg-purple-100 text-purple-800';
      case 'phat-giao': return 'bg-yellow-100 text-yellow-800';
      case 'phong-thuy': return 'bg-blue-100 text-blue-800';
      case 'le-hoi': return 'bg-red-100 text-red-800';
      default: return 'bg-slate-100 text-slate-800';
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

  return (
    <>
      <BookingDialog
        open={showBookingDialog}
        onOpenChange={setShowBookingDialog}
        onConfirm={handleBookingConfirm}
        isLoading={isBooking}
        eventTitle={selectedEvent?.title || undefined}
        selectedDate={selectedDate || undefined}
        serviceProductId={selectedServiceProductId || undefined}
      />
      <div className="space-y-4">
      {/* Lunar Info Card */}
      {selectedDate && (
        <Card className="border-amber-200 bg-gradient-to-br from-amber-50 to-yellow-50 shadow-md">
          <CardHeader className="pb-3 border-b border-amber-100/50">
            <CardTitle className="text-sm text-amber-900 font-bold uppercase tracking-wide">
              {selectedDate.toLocaleDateString('vi-VN', { weekday: 'long', day: '2-digit', month: '2-digit', year: 'numeric' })}
            </CardTitle>
          </CardHeader>
          <CardContent className="pt-4">
            {isLoadingLunar ? (
              <div className="flex items-center justify-center py-4 text-amber-600">
                <Loader2 className="w-5 h-5 animate-spin mr-2" />
                <span className="text-sm">Đang tính toán...</span>
              </div>
            ) : lunarData ? (
              <div className="space-y-3 text-sm">
                <div className="flex justify-between items-center">
                  <span className="text-slate-600">Âm lịch:</span>
                  <span className="font-serif font-bold text-2xl text-amber-700">
                    {lunarData.day}/{lunarData.month}
                  </span>
                </div>
                <div className="flex justify-between border-t border-amber-200/50 pt-2">
                  <span className="text-slate-600">Can Chi:</span>
                  <span className="font-semibold text-amber-900">{lunarData.canChiDay}</span>
                </div>
                <div className="flex justify-between border-t border-amber-200/50 pt-2">
                  <span className="text-slate-600">Tháng:</span>
                  <span className="font-semibold text-amber-900">{lunarData.canChiMonth}</span>
                </div>
                {lunarData.tietKhi && (
                  <div className="flex justify-between border-t border-amber-200/50 pt-2">
                    <span className="text-slate-600">Tiết khí:</span>
                    <span className="font-semibold text-amber-900">{lunarData.tietKhi}</span>
                  </div>
                )}
                
                {/* Booking Button */}
                <button
                  onClick={handleBookingClick}
                  disabled={isBooking}
                  className="w-full mt-4 px-3 py-2 bg-amber-500 hover:bg-amber-600 disabled:bg-amber-300 text-white text-xs font-semibold rounded-lg transition-colors flex items-center justify-center gap-2 border-t border-amber-200/50 pt-3"
                >
                  {isBooking ? (
                    <>
                      <Loader2 className="w-3.5 h-3.5 animate-spin" />
                      Đang xử lý...
                    </>
                  ) : (
                    <>
                      <Calendar className="w-3.5 h-3.5" />
                      Đặt lịch
                    </>
                  )}
                </button>
              </div>
            ) : null}
          </CardContent>
        </Card>
      )}

      {/* Events List */}
      <Card className="shadow-sm">
        <CardHeader className="border-b border-slate-100 bg-slate-50/50">
          <CardTitle className="text-sm font-semibold text-slate-800">
            Sự kiện trong tháng ({events.length})
          </CardTitle>
        </CardHeader>
        <CardContent className="pt-4 max-h-[500px] overflow-y-auto">
          {events.length === 0 ? (
            <p className="text-sm text-slate-500 text-center py-8 italic">Không có sự kiện nào trong tháng này</p>
          ) : (
            <div className="space-y-3">
              {events.map((event) => (
                <div
                  key={event.id}
                  onClick={() => onEventSelect(event)}
                  className={`p-3 rounded-lg cursor-pointer transition-all border ${
                    selectedEvent?.id === event.id
                      ? 'bg-amber-50 border-amber-400 shadow-sm ring-1 ring-amber-200'
                      : 'bg-white hover:bg-slate-50 border-slate-200 hover:border-amber-300'
                  }`}
                >
                  <div className="flex items-start justify-between gap-2">
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-semibold text-slate-800 truncate">
                        {event.title}
                      </p>
                      <span className={`inline-block mt-1.5 px-2 py-0.5 rounded text-[10px] font-bold uppercase tracking-wider ${getCategoryColor(event.category)}`}>
                        {getCategoryName(event.category)}
                      </span>
                    </div>
                  </div>
                  {selectedEvent?.id === event.id && (
                    <div className="mt-3 pt-3 border-t border-amber-200 animate-in fade-in">
                      <p className="text-xs text-slate-600 leading-relaxed mb-3">{event.description}</p>
                      
                      {/* Booking Result */}
                      {bookingResult && (
                        <div className={`mb-3 p-2 rounded-lg text-xs flex gap-2 ${
                          bookingResult.success
                            ? 'bg-green-50 text-green-700 border border-green-200'
                            : 'bg-red-50 text-red-700 border border-red-200'
                        }`}>
                          {bookingResult.success ? (
                            <CheckCircle className="w-4 h-4 flex-shrink-0 mt-0.5" />
                          ) : (
                            <AlertCircle className="w-4 h-4 flex-shrink-0 mt-0.5" />
                          )}
                          <div>
                            <p className="font-semibold">{bookingResult.message}</p>
                            {bookingResult.orderId && (
                              <p className="text-xs opacity-75 mt-1">Mã đơn: {bookingResult.orderId}</p>
                            )}
                          </div>
                        </div>
                      )}

                      {/* Booking Button */}
                      {!bookingResult?.success && (
                        <button
                          onClick={handleBookingClick}
                          disabled={isBooking}
                          className="w-full px-3 py-2 bg-amber-500 hover:bg-amber-600 disabled:bg-amber-300 text-white text-xs font-semibold rounded-lg transition-colors flex items-center justify-center gap-2"
                        >
                          {isBooking ? (
                            <>
                              <Loader2 className="w-3.5 h-3.5 animate-spin" />
                              Đang xử lý...
                            </>
                          ) : (
                            <>
                              <Calendar className="w-3.5 h-3.5" />
                              Đặt lịch
                            </>
                          )}
                        </button>
                      )}


                    </div>
                  )}
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
      </div>
    </>
  );
}
/**
 * Calendar Booking Service
 * Xử lý lưu thông tin booking từ sự kiện lịch
 * Flow: Add to Cart (via EventSidebar) → Save Booking Info → User goes to Checkout → Checkout API called
 * 
 * Note: 
 * - Requires selectedServiceProductId from AppContext to be set before booking
 * - Product is already added to cart by EventSidebar before calling this service
 * - This service ONLY saves booking info to sessionStorage, does NOT call Checkout API
 * - Checkout API is called by CheckoutPage when user clicks "Đặt hàng"
 */

import type { CalendarEvent } from '../../pages/calendar/types';
import type { LunarDateResult } from './lunarCalendarService';
import type { SavedCustomizationState } from './customizationStateService';

export interface CalendarBookingRequest {
    event: CalendarEvent;
    lunarData: LunarDateResult;
    selectedDate: Date;
    customerName: string;
    customerPhone: string;
    customerEmail: string;
    serviceDate: string;
    serviceDuration: string;
    serviceLocation: string;
    serviceNotes: string;
    serviceProductId: string; // GUID of the service product from ServicePage/ServiceDetail
    cartId: string; // Cart ID from useCart hook (already has product added)
    customizationState?: SavedCustomizationState; // Saved customization options
}

export interface CalendarBookingResponse {
    success: boolean;
    orderId?: string;
    message: string;
    error?: string;
}

class CalendarBookingService {
    /**
     * Lưu thông tin booking từ sự kiện lịch
     * Flow:
     * 1. Sản phẩm dịch vụ đã được thêm vào giỏ hàng bởi EventSidebar
     * 2. Lưu thông tin booking vào sessionStorage
     * 3. Redirect đến checkout page
     * 4. Checkout page sẽ gọi Checkout API khi user click "Đặt hàng"
     * 
     * Note: Không gọi Checkout API ở đây để tránh lỗi "cart đã dùng"
     */
    async createBookingFromEvent(
        request: CalendarBookingRequest
    ): Promise<CalendarBookingResponse> {
        try {
            const {
                event,
                lunarData,
                customerName,
                customerPhone,
                customerEmail,
                serviceDuration,
                serviceLocation,
                serviceNotes,
                serviceProductId,
                cartId,
                customizationState
            } = request;

            if (!serviceProductId) {
                return {
                    success: false,
                    message: 'Vui lòng chọn dịch vụ trước khi đặt lịch.',
                    error: 'No service product ID provided',
                };
            }

            if (!cartId) {
                return {
                    success: false,
                    message: 'Không thể lấy ID giỏ hàng. Vui lòng thử lại.',
                    error: 'No cart ID provided',
                };
            }

            console.log('Saving booking info for event:', event.id, 'with product:', serviceProductId, 'and cart:', cartId);

            // Build customization details for notes
            let customizationDetails = '';
            if (customizationState && customizationState.customizations.length > 0) {
                customizationDetails = '\n\n--- Tùy chọn thêm ---\n';
                customizationState.customizations.forEach(custom => {
                    customizationDetails += `${custom.optionName}: ${custom.quantity} ${custom.unit}\n`;
                });
            }

            // Save booking info to sessionStorage for CheckoutPage to use
            const bookingInfo = {
                eventId: event.id,
                eventTitle: event.title,
                cartId: cartId,
                serviceProductId: serviceProductId,
                customerName: customerName,
                customerPhone: customerPhone,
                customerEmail: customerEmail,
                serviceDate: request.serviceDate,
                serviceDuration: serviceDuration,
                serviceLocation: serviceLocation,
                serviceNotes: serviceNotes,
                lunarData: lunarData,
                customizationState: customizationState,
                bookingNotes: `${event.title}\nNgày âm lịch: ${lunarData.day}/${lunarData.month}\nCan Chi: ${lunarData.canChiDay}\nThời lượng: ${serviceDuration}\nĐịa điểm: ${serviceLocation}\n${serviceNotes || ''}${customizationDetails}`,
                createdAt: new Date().toISOString(),
            };

            sessionStorage.setItem('bookingInfo', JSON.stringify(bookingInfo));
            console.log('Booking info saved to sessionStorage:', bookingInfo);

            // Generate a temporary booking ID for reference
            const tempBookingId = `BOOKING-${Date.now()}`;

            return {
                success: true,
                orderId: tempBookingId,
                message: 'Đặt lịch thành công! Chuyển hướng đến thanh toán...',
            };
        } catch (error: any) {
            console.error('Calendar booking error:', error);
            const errorMessage = error?.message || 'Unknown error';
            return {
                success: false,
                message: 'Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại sau.',
                error: errorMessage,
            };
        }
    }


}

export const calendarBookingService = new CalendarBookingService();
export default calendarBookingService;

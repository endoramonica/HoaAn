/**
 * Calendar Booking Service
 * Xử lý tạo đơn hàng dịch vụ từ sự kiện lịch
 * Flow: Add to Cart (via useCart hook) → Get Cart ID → Checkout with Shipping Info
 * 
 * Note: 
 * - Requires selectedServiceProductId from AppContext to be set before booking
 * - Cart operations should be done via useCart hook in component (handles auth/guest automatically)
 * - This service only handles checkout API call
 */

import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type { CalendarEvent } from '../../pages/calendar/types';
import type { LunarDateResult } from './lunarCalendarService';
import type { ShippingMethodEnum } from '../../../Api/generated-orval/schemas';
import type { SavedCustomizationState } from './customizationStateService';

const api = getVietCommerceAPI();

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
     * Tạo đơn hàng dịch vụ từ sự kiện lịch
     * Flow:
     * 1. Thêm sản phẩm dịch vụ vào giỏ hàng (guest cart)
     * 2. Lấy cart ID
     * 3. Gọi checkout với shipping info
     * Thành công: Thông báo sẽ có nhân viên gọi lại tư vấn
     * Thất bại: Thông báo lỗi cho người dùng
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

            console.log('Starting booking process for event:', event.id, 'with product:', serviceProductId, 'and cart:', cartId);

            // Build customization details for notes
            let customizationDetails = '';
            if (customizationState && customizationState.customizations.length > 0) {
                customizationDetails = '\n\n--- Tùy chọn thêm ---\n';
                customizationState.customizations.forEach(custom => {
                    customizationDetails += `${custom.optionName}: ${custom.quantity} ${custom.unit}\n`;
                });
            }

            // Prepare checkout request with correct schema
            // Note: Product is already added to cart via useCart hook in component
            const checkoutRequest = {
                cartId: cartId,
                shippingInfo: {
                    recipientName: customerName,
                    phoneNumber: customerPhone,
                    address: `${serviceLocation} - Tư vấn dịch vụ`,
                    ward: 'Phường 1',
                    district: 'Quận 1',
                    city: 'Hà Nội',
                    postalCode: '100000',
                    deliveryNote: `${event.title}\nNgày âm lịch: ${lunarData.day}/${lunarData.month}\nCan Chi: ${lunarData.canChiDay}\nThời lượng: ${serviceDuration}\nĐịa điểm: ${serviceLocation}\n${serviceNotes || ''}${customizationDetails}`,
                    shippingMethod: 'standard' as ShippingMethodEnum,
                },
                notes: `${event.title}\nNgày âm lịch: ${lunarData.day}/${lunarData.month}\nCan Chi: ${lunarData.canChiDay}\nThời lượng: ${serviceDuration}\nĐịa điểm: ${serviceLocation}\n${serviceNotes || ''}${customizationDetails}`,
            };

            console.log('Checkout request:', checkoutRequest);

            // Call checkout API
            const checkoutResponse = await api.postApiV1CheckoutProcess(checkoutRequest as any);

            console.log('Checkout response:', checkoutResponse);

            if (checkoutResponse) {
                const responseData = (checkoutResponse as any)?.data || checkoutResponse;
                const orderId = responseData?.orderId || responseData?.id;

                if (orderId) {
                    return {
                        success: true,
                        orderId: orderId,
                        message: 'Đặt lịch thành công! Chuyển hướng đến thanh toán...',
                    };
                }
            }

            return {
                success: false,
                message: 'Không thể tạo đơn hàng. Vui lòng thử lại.',
                error: 'Invalid response from server',
            };
        } catch (error: any) {
            console.error('Calendar booking error:', error);
            const errorMessage = error?.response?.data?.message || error?.message || 'Unknown error';
            return {
                success: false,
                message: 'Có lỗi xảy ra khi đặt lịch. Vui lòng thử lại sau.',
                error: errorMessage,
            };
        }
    }

    /**
     * Lấy chi tiết đơn hàng đã tạo
     */
    async getBookingOrder(orderId: string): Promise<any> {
        try {
            const response = await api.getApiV1CheckoutOrderId(orderId);
            return response.data;
        } catch (error) {
            console.error('Get booking order error:', error);
            throw error;
        }
    }

    /**
     * Hủy đơn hàng booking
     */
    async cancelBookingOrder(orderId: string, reason?: string): Promise<boolean> {
        try {
            const response = await api.postApiV1CheckoutOrderIdCancel(orderId, {
                reason: reason || 'Hủy đặt lịch',
            });
            return !!response.data;
        } catch (error) {
            console.error('Cancel booking order error:', error);
            return false;
        }
    }
}

export const calendarBookingService = new CalendarBookingService();
export default calendarBookingService;

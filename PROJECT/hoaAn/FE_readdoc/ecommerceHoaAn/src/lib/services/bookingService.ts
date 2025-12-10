// src/lib/services/bookingService.ts
import { apiClient } from '../api/orval-client';

export interface LunarDateInfo {
    day: number;
    month: number;
    year: number;
    heavenlyStems: string;
    earthlyBranches: string;
    sexagenaryCycle: string;
}

export interface BookingCreateRequest {
    customerId: string;
    serviceId: string;
    solarDate: string; // ISO format: YYYY-MM-DD
    lunarDate: LunarDateInfo;
    customerName: string;
    customerPhone: string;
    customerEmail: string;
    notes?: string;
}

export interface BookingResponse {
    id: string;
    customerId: string;
    serviceId: string;
    solarDate: string;
    lunarDate: LunarDateInfo;
    customerName: string;
    customerPhone: string;
    customerEmail: string;
    notes?: string;
    status: string;
    createdAt: string;
}

const FIXED_CUSTOMER_ID = 'D93A557C-A41E-4DD9-A0A7-D6B3897BC4A7';

/**
 * Create a booking with lunar date information
 */
export async function createBooking(
    request: Omit<BookingCreateRequest, 'customerId'>
): Promise<BookingResponse> {
    const payload: BookingCreateRequest = {
        ...request,
        customerId: FIXED_CUSTOMER_ID,
    };

    return apiClient<BookingResponse>({
        url: '/api/v1/Booking/create',
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        data: payload,
    });
}

/**
 * Get booking by ID
 */
export async function getBooking(bookingId: string): Promise<BookingResponse> {
    return apiClient<BookingResponse>({
        url: `/api/v1/Booking/${bookingId}`,
        method: 'GET',
    });
}

/**
 * Get all bookings for customer
 */
export async function getCustomerBookings(): Promise<BookingResponse[]> {
    return apiClient<BookingResponse[]>({
        url: `/api/v1/Booking/customer/${FIXED_CUSTOMER_ID}`,
        method: 'GET',
    });
}

/**
 * Cancel booking
 */
export async function cancelBooking(bookingId: string): Promise<void> {
    return apiClient<void>({
        url: `/api/v1/Booking/${bookingId}/cancel`,
        method: 'POST',
    });
}

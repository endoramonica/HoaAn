import { useState, useCallback } from 'react';
import { toast } from 'sonner';
import { calendarBookingService, type CalendarBookingRequest } from '../services/calendarBookingService';

export interface UseCalendarBookingState {
    isLoading: boolean;
    isSuccess: boolean;
    error: string | null;
    orderId: string | null;
}

export function useCalendarBooking() {
    const [state, setState] = useState<UseCalendarBookingState>({
        isLoading: false,
        isSuccess: false,
        error: null,
        orderId: null,
    });

    const createBooking = useCallback(
        async (request: CalendarBookingRequest) => {
            setState({ isLoading: true, isSuccess: false, error: null, orderId: null });

            try {
                const result = await calendarBookingService.createBookingFromEvent(request);

                if (result.success) {
                    setState({
                        isLoading: false,
                        isSuccess: true,
                        error: null,
                        orderId: result.orderId || null,
                    });
                    toast.success(result.message);
                    return result;
                } else {
                    setState({
                        isLoading: false,
                        isSuccess: false,
                        error: result.message,
                        orderId: null,
                    });
                    toast.error(result.message);
                    return result;
                }
            } catch (error: any) {
                const errorMessage = error?.message || 'Có lỗi xảy ra khi đặt lịch';
                setState({
                    isLoading: false,
                    isSuccess: false,
                    error: errorMessage,
                    orderId: null,
                });
                toast.error(errorMessage);
                throw error;
            }
        },
        []
    );

    const reset = useCallback(() => {
        setState({
            isLoading: false,
            isSuccess: false,
            error: null,
            orderId: null,
        });
    }, []);

    return {
        ...state,
        createBooking,
        reset,
    };
}

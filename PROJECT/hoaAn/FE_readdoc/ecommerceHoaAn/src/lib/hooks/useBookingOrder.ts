import { useQuery } from '@tanstack/react-query';
import { calendarBookingService } from '../services/calendarBookingService';

export function useBookingOrder(orderId: string | undefined) {
    return useQuery({
        queryKey: ['booking', orderId],
        queryFn: async () => {
            if (!orderId) throw new Error('Order ID is required');
            return calendarBookingService.getBookingOrder(orderId);
        },
        enabled: !!orderId,
        staleTime: 5 * 60 * 1000, // 5 minutes
        retry: 1,
    });
}

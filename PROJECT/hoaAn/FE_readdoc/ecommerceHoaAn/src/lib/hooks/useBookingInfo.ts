import { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';

export interface BookingInfo {
    orderId: string;
    serviceDate: string;
    serviceDuration: string;
    serviceLocation: string;
    serviceNotes: string;
    customerName: string;
    customerPhone: string;
    customerEmail: string;
}

export function useBookingInfo(): BookingInfo | null {
    const [searchParams] = useSearchParams();
    const [bookingInfo, setBookingInfo] = useState<BookingInfo | null>(null);

    useEffect(() => {
        // Lấy từ sessionStorage
        const stored = sessionStorage.getItem('bookingInfo');
        if (stored) {
            try {
                const parsed = JSON.parse(stored);
                setBookingInfo(parsed);
            } catch (error) {
                console.error('Error parsing booking info:', error);
            }
        }

        // Hoặc từ URL params
        const orderId = searchParams.get('orderId');
        if (orderId && !stored) {
            setBookingInfo({
                orderId,
                serviceDate: '',
                serviceDuration: '',
                serviceLocation: '',
                serviceNotes: '',
                customerName: '',
                customerPhone: '',
                customerEmail: '',
            });
        }
    }, [searchParams]);

    return bookingInfo;
}

/**
 * Parse serviceNotes để lấy thông tin chi tiết
 */
export function parseServiceNotes(notes: string): {
    eventTitle: string;
    lunarDate: string;
    canChi: string;
    duration: string;
    location: string;
    additionalNotes: string;
} {
    const lines = notes.split('\n').filter(line => line.trim());

    return {
        eventTitle: lines[0] || '',
        lunarDate: lines[1]?.replace('Ngày âm lịch: ', '') || '',
        canChi: lines[2]?.replace('Can Chi: ', '') || '',
        duration: lines[3]?.replace('Thời lượng: ', '') || '',
        location: lines[4]?.replace('Địa điểm: ', '') || '',
        additionalNotes: lines.slice(5).join('\n'),
    };
}

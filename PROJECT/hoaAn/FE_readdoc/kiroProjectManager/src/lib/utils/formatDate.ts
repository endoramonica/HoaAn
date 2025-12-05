/**
 * Format a date string or Date object to Vietnamese locale format
 * @param date - The date to format (string, Date, or timestamp)
 * @param options - Intl.DateTimeFormatOptions for customization
 * @returns Formatted date string
 */
export function formatDate(
    date: string | Date | number,
    options?: Intl.DateTimeFormatOptions
): string {
    if (!date) return '';

    try {
        const dateObj = typeof date === 'string' || typeof date === 'number'
            ? new Date(date)
            : date;

        if (isNaN(dateObj.getTime())) {
            return '';
        }

        const defaultOptions: Intl.DateTimeFormatOptions = {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            ...options,
        };

        return new Intl.DateTimeFormat('vi-VN', defaultOptions).format(dateObj);
    } catch (error) {
        console.error('Error formatting date:', error);
        return '';
    }
}

/**
 * Format a date with time in Vietnamese locale
 * @param date - The date to format
 * @returns Formatted date and time string (e.g., "01/12/2024, 14:30")
 */
export function formatDateTime(date: string | Date | number): string {
    return formatDate(date, {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
    });
}

/**
 * Format a date as a relative time string (e.g., "2 giờ trước")
 * @param date - The date to format
 * @returns Relative time string in Vietnamese
 */
export function formatRelativeTime(date: string | Date | number): string {
    if (!date) return '';

    try {
        const dateObj = typeof date === 'string' || typeof date === 'number'
            ? new Date(date)
            : date;

        if (isNaN(dateObj.getTime())) {
            return '';
        }

        const now = new Date();
        const diffInSeconds = Math.floor((now.getTime() - dateObj.getTime()) / 1000);

        if (diffInSeconds < 60) {
            return 'vừa xong';
        }

        const diffInMinutes = Math.floor(diffInSeconds / 60);
        if (diffInMinutes < 60) {
            return `${diffInMinutes} phút trước`;
        }

        const diffInHours = Math.floor(diffInMinutes / 60);
        if (diffInHours < 24) {
            return `${diffInHours} giờ trước`;
        }

        const diffInDays = Math.floor(diffInHours / 24);
        if (diffInDays < 7) {
            return `${diffInDays} ngày trước`;
        }

        const diffInWeeks = Math.floor(diffInDays / 7);
        if (diffInWeeks < 4) {
            return `${diffInWeeks} tuần trước`;
        }

        const diffInMonths = Math.floor(diffInDays / 30);
        if (diffInMonths < 12) {
            return `${diffInMonths} tháng trước`;
        }

        const diffInYears = Math.floor(diffInDays / 365);
        return `${diffInYears} năm trước`;
    } catch (error) {
        console.error('Error formatting relative time:', error);
        return '';
    }
}

/**
 * Format a date as a short date string (e.g., "01/12/2024")
 * @param date - The date to format
 * @returns Short date string
 */
export function formatShortDate(date: string | Date | number): string {
    return formatDate(date, {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
    });
}

/**
 * Format a date as a long date string (e.g., "Thứ Sáu, 01 tháng 12, 2024")
 * @param date - The date to format
 * @returns Long date string
 */
export function formatLongDate(date: string | Date | number): string {
    return formatDate(date, {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric',
    });
}

/**
 * Format time only (e.g., "14:30")
 * @param date - The date to format
 * @returns Time string
 */
export function formatTime(date: string | Date | number): string {
    if (!date) return '';

    try {
        const dateObj = typeof date === 'string' || typeof date === 'number'
            ? new Date(date)
            : date;

        if (isNaN(dateObj.getTime())) {
            return '';
        }

        return new Intl.DateTimeFormat('vi-VN', {
            hour: '2-digit',
            minute: '2-digit',
        }).format(dateObj);
    } catch (error) {
        console.error('Error formatting time:', error);
        return '';
    }
}

import { AxiosError } from 'axios';

/**
 * Standardized API error interface
 */
export interface ApiError {
    status: number;
    message: string;
    errors?: Record<string, string[]>; // Validation errors
    timestamp: string;
}

/**
 * Vietnamese error messages for common HTTP status codes
 */
export const ErrorMessages = {
    // Network errors
    NETWORK_ERROR: 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.',

    // Client errors (4xx)
    BAD_REQUEST: 'Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin.',
    UNAUTHORIZED: 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.',
    FORBIDDEN: 'Bạn không có quyền truy cập tài nguyên này.',
    NOT_FOUND: 'Không tìm thấy tài nguyên yêu cầu.',

    // Server errors (5xx)
    SERVER_ERROR: 'Lỗi server. Vui lòng thử lại sau.',
    BAD_GATEWAY: 'Lỗi kết nối server. Vui lòng thử lại sau.',
    SERVICE_UNAVAILABLE: 'Dịch vụ tạm thời không khả dụng. Vui lòng thử lại sau.',

    // Default
    UNKNOWN_ERROR: 'Đã xảy ra lỗi không xác định. Vui lòng thử lại.',
} as const;

/**
 * Get error message based on HTTP status code
 * @param status - HTTP status code
 * @returns Vietnamese error message
 */
export function getErrorMessage(status: number): string {
    switch (status) {
        case 0:
            return ErrorMessages.NETWORK_ERROR;
        case 400:
            return ErrorMessages.BAD_REQUEST;
        case 401:
            return ErrorMessages.UNAUTHORIZED;
        case 403:
            return ErrorMessages.FORBIDDEN;
        case 404:
            return ErrorMessages.NOT_FOUND;
        case 502:
            return ErrorMessages.BAD_GATEWAY;
        case 503:
            return ErrorMessages.SERVICE_UNAVAILABLE;
        case 500:
        case 501:
        case 504:
        case 505:
            return ErrorMessages.SERVER_ERROR;
        default:
            if (status >= 500) {
                return ErrorMessages.SERVER_ERROR;
            }
            return ErrorMessages.UNKNOWN_ERROR;
    }
}

/**
 * Transform HTTP errors into standardized ApiError format
 * @param error - Axios error or generic error
 * @returns Standardized ApiError object
 */
export function createApiError(error: unknown): ApiError {
    const timestamp = new Date().toISOString();

    // Handle Axios errors
    if (error && typeof error === 'object' && 'isAxiosError' in error) {
        const axiosError = error as AxiosError<any>;

        // Network error (no response from server)
        if (!axiosError.response) {
            return {
                status: 0,
                message: getErrorMessage(0),
                timestamp,
            };
        }

        const { status, data } = axiosError.response;

        // Extract message from response if available
        let message = getErrorMessage(status);
        if (data && typeof data === 'object') {
            if (data.message && typeof data.message === 'string') {
                message = data.message;
            } else if (data.error && typeof data.error === 'string') {
                message = data.error;
            }
        }

        // Extract validation errors for 400 Bad Request
        let validationErrors: Record<string, string[]> | undefined;
        if (status === 400 && data && typeof data === 'object') {
            if (data.errors && typeof data.errors === 'object') {
                validationErrors = data.errors;
            } else if (data.validationErrors && typeof data.validationErrors === 'object') {
                validationErrors = data.validationErrors;
            }
        }

        return {
            status,
            message,
            errors: validationErrors,
            timestamp,
        };
    }

    // Handle generic errors
    if (error instanceof Error) {
        return {
            status: 0,
            message: error.message || ErrorMessages.UNKNOWN_ERROR,
            timestamp,
        };
    }

    // Handle unknown error types
    return {
        status: 0,
        message: ErrorMessages.UNKNOWN_ERROR,
        timestamp,
    };
}

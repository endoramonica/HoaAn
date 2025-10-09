/**
 * Error handling tập trung theo format ASP.NET Core
 * Hỗ trợ ProblemDetails và ValidationProblemDetails
 */

import type { ProblemDetails, ValidationError } from './types';

export class ApiError extends Error {
  public status: number;
  public errors?: ValidationError[];
  public problemDetails?: ProblemDetails;

  constructor(
    message: string,
    status: number,
    errors?: ValidationError[],
    problemDetails?: ProblemDetails
  ) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.errors = errors;
    this.problemDetails = problemDetails;
  }

  /**
   * Kiểm tra xem lỗi có phải validation error không
   */
  isValidationError(): boolean {
    return this.status === 400 && !!this.errors && this.errors.length > 0;
  }

  /**
   * Kiểm tra xem lỗi có phải unauthorized không
   */
  isUnauthorized(): boolean {
    return this.status === 401;
  }

  /**
   * Kiểm tra xem lỗi có phải forbidden không
   */
  isForbidden(): boolean {
    return this.status === 403;
  }

  /**
   * Kiểm tra xem lỗi có phải not found không
   */
  isNotFound(): boolean {
    return this.status === 404;
  }

  /**
   * Lấy validation errors cho một field cụ thể
   */
  getFieldErrors(fieldName: string): string[] {
    if (!this.errors) return [];
    return this.errors
      .filter(e => e.field.toLowerCase() === fieldName.toLowerCase())
      .map(e => e.message);
  }

  /**
   * Format lỗi để hiển thị cho user
   */
  getDisplayMessage(): string {
    if (this.isValidationError() && this.errors) {
      return this.errors.map(e => e.message).join(', ');
    }
    return this.message;
  }
}

/**
 * Parse ProblemDetails từ ASP.NET Core response
 */
export function parseProblemDetails(data: any): {
  message: string;
  errors?: ValidationError[];
  problemDetails?: ProblemDetails;
} {
  // Nếu là ValidationProblemDetails (status 400 với errors object)
  if (data.errors && typeof data.errors === 'object') {
    const validationErrors: ValidationError[] = [];
    
    for (const [field, messages] of Object.entries(data.errors)) {
      if (Array.isArray(messages)) {
        messages.forEach((msg: string) => {
          validationErrors.push({ field, message: msg });
        });
      }
    }

    return {
      message: data.title || 'Validation failed',
      errors: validationErrors,
      problemDetails: data as ProblemDetails
    };
  }

  // Nếu là ProblemDetails thông thường
  if (data.title || data.detail) {
    return {
      message: data.detail || data.title || 'An error occurred',
      problemDetails: data as ProblemDetails
    };
  }

  // Fallback
  return {
    message: data.message || 'An error occurred'
  };
}

/**
 * Tạo ApiError từ response
 */
export function createApiError(status: number, data: any): ApiError {
  const parsed = parseProblemDetails(data);
  return new ApiError(
    parsed.message,
    status,
    parsed.errors,
    parsed.problemDetails
  );
}

/**
 * Error messages mặc định
 */
export const ErrorMessages = {
  NETWORK_ERROR: 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối internet.',
  TIMEOUT_ERROR: 'Yêu cầu đã hết thời gian chờ. Vui lòng thử lại.',
  UNAUTHORIZED: 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.',
  FORBIDDEN: 'Bạn không có quyền truy cập chức năng này.',
  NOT_FOUND: 'Không tìm thấy tài nguyên yêu cầu.',
  SERVER_ERROR: 'Lỗi server. Vui lòng thử lại sau.',
  VALIDATION_ERROR: 'Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.',
  UNKNOWN_ERROR: 'Đã có lỗi xảy ra. Vui lòng thử lại.',
};

/**
 * Lấy error message dựa trên status code
 */
export function getErrorMessage(status: number, defaultMessage?: string): string {
  switch (status) {
    case 400:
      return ErrorMessages.VALIDATION_ERROR;
    case 401:
      return ErrorMessages.UNAUTHORIZED;
    case 403:
      return ErrorMessages.FORBIDDEN;
    case 404:
      return ErrorMessages.NOT_FOUND;
    case 408:
      return ErrorMessages.TIMEOUT_ERROR;
    case 500:
    case 502:
    case 503:
    case 504:
      return ErrorMessages.SERVER_ERROR;
    default:
      return defaultMessage || ErrorMessages.UNKNOWN_ERROR;
  }
}

/**
 * User Service Layer - Quản lý người dùng
 * Tích hợp với ASP.NET Core Users API endpoints
 */

import { apiRequest } from '../api/client';

// ============================================================================
// User Types (dựa trên OpenAPI spec)
// ============================================================================

export interface UserDto {
  id: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  avatar?: string;
  isActive: boolean;
  isEmailVerified: boolean;
  createdAt: string;
  updatedAt?: string;
  customerId?: string;
  tenantId?: string;
}

export interface UserUpdateRequest {
  fullName?: string;
  email?: string;
  phoneNumber?: string;
  isActive?: boolean;
  customerId?: string;
  tenantId?: string;
}

export interface PaginatedUserResponse {
  items: UserDto[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface GetUsersParams {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
}

// ============================================================================
// User Service
// ============================================================================

export const userService = {
  /**
   * Lấy danh sách người dùng (có phân trang và tìm kiếm)
   */
  getUsers: async (params?: GetUsersParams): Promise<PaginatedUserResponse> => {
    const queryParams = new URLSearchParams();

    if (params) {
      if (params.pageNumber) {
        queryParams.append('pageNumber', String(params.pageNumber));
      }
      if (params.pageSize) {
        queryParams.append('pageSize', String(params.pageSize));
      }
      if (params.search) {
        queryParams.append('search', params.search);
      }
    }

    const queryString = queryParams.toString();
    const url = queryString ? `/Users?${queryString}` : '/Users';

    return apiRequest.get<PaginatedUserResponse>(url);
  },

  /**
   * Lấy thông tin user theo ID
   */
  getUserById: async (userId: string): Promise<UserDto> => {
    return apiRequest.get<UserDto>(`/Users/${userId}`);
  },

  /**
   * Cập nhật thông tin user
   */
  updateUser: async (userId: string, data: UserUpdateRequest): Promise<UserDto> => {
    return apiRequest.put<UserDto>(`/Users/${userId}`, data);
  },

  /**
   * Vô hiệu hóa user
   */
  deactivateUser: async (userId: string): Promise<void> => {
    return apiRequest.post<void>(`/Users/${userId}/deactivate`);
  },

  /**
   * Kích hoạt lại user
   */
  activateUser: async (userId: string): Promise<void> => {
    return apiRequest.post<void>(`/Users/${userId}/activate`);
  },

  /**
   * Lấy danh sách địa chỉ của user
   */
  getAddresses: async (): Promise<AddressDto[]> => {
    // Mock mode
    if (typeof import.meta !== 'undefined' && import.meta.env && import.meta.env.VITE_USE_MOCK_DATA === 'true') {
      await new Promise(resolve => setTimeout(resolve, 500));
      
      return [
        {
          id: 'addr-1',
          fullName: 'Nguyễn Văn A',
          phoneNumber: '0901234567',
          addressLine1: '123 Nguyễn Huệ',
          ward: 'Phường Bến Nghé',
          district: 'Quận 1',
          province: 'TP. Hồ Chí Minh',
          isDefault: true,
        },
        {
          id: 'addr-2',
          fullName: 'Nguyễn Văn A',
          phoneNumber: '0901234567',
          addressLine1: '456 Lê Lợi',
          ward: 'Phường Bến Thành',
          district: 'Quận 1',
          province: 'TP. Hồ Chí Minh',
          isDefault: false,
        },
      ];
    }

    return apiRequest.get<AddressDto[]>('/users/addresses');
  },

  /**
   * Thêm địa chỉ mới
   */
  addAddress: async (address: Omit<AddressDto, 'id'>): Promise<AddressDto> => {
    // Mock mode
    if (typeof import.meta !== 'undefined' && import.meta.env && import.meta.env.VITE_USE_MOCK_DATA === 'true') {
      await new Promise(resolve => setTimeout(resolve, 800));
      
      return {
        id: `addr-${Date.now()}`,
        ...address,
      };
    }

    return apiRequest.post<AddressDto>('/users/addresses', address);
  },

  /**
   * Cập nhật địa chỉ
   */
  updateAddress: async (addressId: string, address: Partial<AddressDto>): Promise<AddressDto> => {
    return apiRequest.put<AddressDto>(`/users/addresses/${addressId}`, address);
  },

  /**
   * Xóa địa chỉ
   */
  deleteAddress: async (addressId: string): Promise<void> => {
    return apiRequest.delete<void>(`/users/addresses/${addressId}`);
  },

  /**
   * Đặt địa chỉ mặc định
   */
  setDefaultAddress: async (addressId: string): Promise<void> => {
    return apiRequest.post<void>(`/users/addresses/${addressId}/set-default`);
  },
};

// Import AddressDto type
import { AddressDto } from '../api/types';

export default userService;

/**
 * useCustomerAddress Hook
 * Custom hook để quản lý địa chỉ khách hàng
 * ✅ Tích hợp với CustomerAddressService
 * ✅ FIX: Không dùng getDisplayMessage() - dùng helper function
 */

import { useState, useCallback } from 'react';
import { CustomerAddressService } from '@/api/services/CustomerAddressService';
import { ApiError } from '@/api/core/ApiError';
import type { 
  AddressResponseDto, 
  CreateAddressDto, 
  UpdateAddressDto 
} from '@/api';
import { toast } from 'sonner';

/**
 * ✅ Helper: Extract error message từ ApiError
 * Vì ApiError từ openapi-typescript-codegen không có method getDisplayMessage()
 */
const getErrorMessage = (err: any): string => {
  if (err instanceof ApiError) {
    // Thử lấy message từ response body
    if (err.body?.message) {
      return err.body.message;
    }
    if (err.body?.error) {
      return err.body.error;
    }
    if (err.body?.title) {
      return err.body.title;
    }
    // Fallback về statusText
    return err.statusText || `Lỗi ${err.status}`;
  }
  
  // Fallback cho errors khác
  return err.message || 'Đã xảy ra lỗi';
};

interface UseCustomerAddressReturn {
  addresses: AddressResponseDto[];
  isLoading: boolean;
  error: string | null;
  loadAddresses: () => Promise<void>;
  addAddress: (address: CreateAddressDto) => Promise<AddressResponseDto | null>;
  updateAddress: (id: string, address: UpdateAddressDto) => Promise<AddressResponseDto | null>;
  deleteAddress: (id: string) => Promise<boolean>;
  setDefaultAddress: (id: string) => Promise<boolean>;
}

/**
 * ✅ Hook quản lý địa chỉ khách hàng
 * 
 * @example
 * ```tsx
 * const { addresses, isLoading, addAddress } = useCustomerAddress();
 * 
 * // Load addresses on mount
 * useEffect(() => {
 *   loadAddresses();
 * }, []);
 * 
 * // Add new address
 * await addAddress({
 *   fullName: 'John Doe',
 *   phoneNumber: '0123456789',
 *   addressLine1: '123 Main St',
 *   ward: 'Ward 1',
 *   district: 'District 1',
 *   province: 'Ho Chi Minh',
 *   isDefault: false,
 * });
 * ```
 */
export const useCustomerAddress = (): UseCustomerAddressReturn => {
  const [addresses, setAddresses] = useState<AddressResponseDto[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  /**
   * ✅ Load danh sách địa chỉ
   */
  const loadAddresses = useCallback(async () => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await CustomerAddressService.getApiV1CustomerAddresses();

      if (!response.success || !response.data) {
        throw new Error('Không thể tải danh sách địa chỉ');
      }

      setAddresses(response.data);
      console.log('[useCustomerAddress] ✅ Loaded addresses:', response.data.length);
    } catch (err: any) {
      console.error('[useCustomerAddress] ❌ Load addresses error:', err);
      
      const errorMessage = getErrorMessage(err);
      setError(errorMessage);
      
      // Chỉ hiển thị toast nếu không phải lỗi 401/403 (auth issues)
      if (err.status !== 401 && err.status !== 403) {
        toast.error(errorMessage);
      }
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * ✅ Thêm địa chỉ mới
   */
  const addAddress = useCallback(async (
    address: CreateAddressDto
  ): Promise<AddressResponseDto | null> => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await CustomerAddressService.postApiV1CustomerAddresses(address);

      if (!response.success || !response.data) {
        throw new Error('Không thể thêm địa chỉ');
      }

      // Cập nhật danh sách addresses
      setAddresses(prev => [...prev, response.data!]);
      
      toast.success('Thêm địa chỉ thành công');
      console.log('[useCustomerAddress] ✅ Address added:', response.data.id);
      
      return response.data;
    } catch (err: any) {
      console.error('[useCustomerAddress] ❌ Add address error:', err);
      
      const errorMessage = getErrorMessage(err);
      setError(errorMessage);
      toast.error(errorMessage);
      
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * ✅ Cập nhật địa chỉ
   */
  const updateAddress = useCallback(async (
    id: string,
    address: UpdateAddressDto
  ): Promise<AddressResponseDto | null> => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await CustomerAddressService.putApiV1CustomerAddresses(id, address);

      if (!response.success || !response.data) {
        throw new Error('Không thể cập nhật địa chỉ');
      }

      // Cập nhật trong danh sách
      setAddresses(prev => 
        prev.map(addr => addr.id === id ? response.data! : addr)
      );
      
      toast.success('Cập nhật địa chỉ thành công');
      console.log('[useCustomerAddress] ✅ Address updated:', id);
      
      return response.data;
    } catch (err: any) {
      console.error('[useCustomerAddress] ❌ Update address error:', err);
      
      const errorMessage = getErrorMessage(err);
      setError(errorMessage);
      toast.error(errorMessage);
      
      return null;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * ✅ Xóa địa chỉ
   */
  const deleteAddress = useCallback(async (id: string): Promise<boolean> => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await CustomerAddressService.deleteApiV1CustomerAddresses(id);

      if (!response.success) {
        throw new Error('Không thể xóa địa chỉ');
      }

      // Xóa khỏi danh sách
      setAddresses(prev => prev.filter(addr => addr.id !== id));
      
      toast.success('Xóa địa chỉ thành công');
      console.log('[useCustomerAddress] ✅ Address deleted:', id);
      
      return true;
    } catch (err: any) {
      console.error('[useCustomerAddress] ❌ Delete address error:', err);
      
      const errorMessage = getErrorMessage(err);
      setError(errorMessage);
      toast.error(errorMessage);
      
      return false;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * ✅ Đặt địa chỉ mặc định
   */
  const setDefaultAddress = useCallback(async (id: string): Promise<boolean> => {
    try {
      setIsLoading(true);
      setError(null);

      const response = await CustomerAddressService.postApiV1CustomerAddressesSetDefault(id);

      if (!response.success) {
        throw new Error('Không thể đặt địa chỉ mặc định');
      }

      // Cập nhật isDefault trong danh sách
      setAddresses(prev =>
        prev.map(addr => ({
          ...addr,
          isDefault: addr.id === id,
        }))
      );
      
      toast.success('Đã đặt làm địa chỉ mặc định');
      console.log('[useCustomerAddress] ✅ Default address set:', id);
      
      return true;
    } catch (err: any) {
      console.error('[useCustomerAddress] ❌ Set default address error:', err);
      
      const errorMessage = getErrorMessage(err);
      setError(errorMessage);
      toast.error(errorMessage);
      
      return false;
    } finally {
      setIsLoading(false);
    }
  }, []);

  return {
    addresses,
    isLoading,
    error,
    loadAddresses,
    addAddress,
    updateAddress,
    deleteAddress,
    setDefaultAddress,
  };
};

export default useCustomerAddress;
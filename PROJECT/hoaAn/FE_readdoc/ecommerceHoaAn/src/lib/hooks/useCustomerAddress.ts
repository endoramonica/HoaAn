/**
 * useCustomerAddress Hook
 * Custom hook để quản lý địa chỉ khách hàng
 * ✅ UPDATED: Sử dụng Orval API client
 */

import { useState, useCallback } from 'react';
import { getVietCommerceAPI } from '../../../Api/generated-orval';
import type {
  AddressResponseDto,
  CreateAddressDto,
  UpdateAddressDto
} from '../../../Api/generated-orval/schemas';
import { toast } from 'sonner';

const api = getVietCommerceAPI();

/**
 * ✅ Helper: Extract error message
 */
const getErrorMessage = (err: any): string => {
  if (err?.body?.message) return err.body.message;
  if (err?.body?.error) return err.body.error;
  if (err?.body?.title) return err.body.title;
  if (err?.statusText) return err.statusText;
  if (err?.message) return err.message;
  return 'Đã xảy ra lỗi';
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

      const response = await api.getApiV1CustomerAddresses();

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

      const response = await api.postApiV1CustomerAddresses(address);

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

      const response = await api.putApiV1CustomerAddressesId(id, address);

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

      const response = await api.deleteApiV1CustomerAddressesId(id);

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

      const response = await api.postApiV1CustomerAddressesIdSetDefault(id);

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

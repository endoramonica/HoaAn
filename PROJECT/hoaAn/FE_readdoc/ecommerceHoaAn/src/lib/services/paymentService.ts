/**
 * Payment Service - Xử lý các phương thức thanh toán
 * Hỗ trợ: COD, VNPay, Momo, ZaloPay, BankTransfer
 */

import { apiRequest } from '../api/client';
import { PaymentMethod } from '../api/types';

// ============================================================================
// Payment Types
// ============================================================================

export interface PaymentRequest {
  orderId: string;
  amount: number;
  paymentMethod: PaymentMethod;
  returnUrl: string;
  cancelUrl?: string;
}

export interface PaymentResponse {
  success: boolean;
  paymentUrl?: string;
  qrCode?: string;
  transactionId: string;
  message?: string;
}

export interface PaymentCallbackData {
  orderId: string;
  transactionId: string;
  amount: number;
  status: 'success' | 'failed' | 'cancelled';
  paymentMethod: PaymentMethod;
  message?: string;
}

export interface PaymentStatusResponse {
  orderId: string;
  transactionId: string;
  status: 'pending' | 'processing' | 'completed' | 'failed' | 'cancelled';
  amount: number;
  paidAmount?: number;
  paymentMethod: PaymentMethod;
  paidAt?: string;
  createdAt: string;
  updatedAt?: string;
}

// ============================================================================
// Mock Mode Configuration
// ============================================================================

const getMockMode = () => {
  if (typeof import.meta !== 'undefined' && import.meta.env) {
    return import.meta.env.VITE_USE_MOCK_DATA === 'true';
  }
  return true;
};

const USE_MOCK = getMockMode();

// ============================================================================
// Payment Service Class
// ============================================================================

class PaymentService {
  /**
   * Tạo payment link cho online payment
   */
  async createPayment(request: PaymentRequest): Promise<PaymentResponse> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1500));

        // COD không cần payment URL
        if (request.paymentMethod === 'COD') {
          return {
            success: true,
            transactionId: `COD-${Date.now()}`,
            message: 'Đơn hàng đã được tạo. Thanh toán khi nhận hàng.',
          };
        }

        // Mock payment URL cho các phương thức online
        const baseUrls: Record<string, string> = {
          VNPay: 'https://sandbox.vnpayment.vn/paymentv2/vpcpay.html',
          Momo: 'https://test-payment.momo.vn/gw_payment/transactionProcessor',
          ZaloPay: 'https://sbgateway.zalopay.vn/order/pay',
          BankTransfer: '#', // Không redirect, hiển thị thông tin banking
        };

        const mockPaymentUrl = baseUrls[request.paymentMethod] || '#';
        const mockParams = new URLSearchParams({
          orderId: request.orderId,
          amount: String(request.amount),
          returnUrl: request.returnUrl,
        });

        return {
          success: true,
          paymentUrl: mockPaymentUrl !== '#' ? `${mockPaymentUrl}?${mockParams.toString()}` : undefined,
          transactionId: `${request.paymentMethod}-${Date.now()}`,
          message: 'Đang chuyển hướng đến cổng thanh toán...',
        };
      }

      // Real API call
      return await apiRequest.post<PaymentResponse>('/payments/create', request);
    } catch (error) {
      console.error('Create payment error:', error);
      throw error;
    }
  }

  /**
   * Xử lý callback từ payment gateway
   */
  async handlePaymentCallback(
    paymentMethod: PaymentMethod,
    callbackData: Record<string, any>
  ): Promise<PaymentCallbackData> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 1000));

        return {
          orderId: callbackData.orderId || 'unknown',
          transactionId: callbackData.transactionId || `TRANS-${Date.now()}`,
          amount: Number(callbackData.amount) || 0,
          status: 'success',
          paymentMethod,
          message: 'Thanh toán thành công',
        };
      }

      return await apiRequest.post<PaymentCallbackData>(
        `/payments/callback/${paymentMethod.toLowerCase()}`,
        callbackData
      );
    } catch (error) {
      console.error('Handle payment callback error:', error);
      throw error;
    }
  }

  /**
   * Kiểm tra trạng thái thanh toán
   */
  async checkPaymentStatus(orderId: string): Promise<PaymentStatusResponse> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 500));

        return {
          orderId,
          transactionId: `TRANS-${orderId}`,
          status: 'completed',
          amount: 1000000,
          paidAmount: 1000000,
          paymentMethod: 'VNPay',
          paidAt: new Date().toISOString(),
          createdAt: new Date(Date.now() - 3600000).toISOString(),
          updatedAt: new Date().toISOString(),
        };
      }

      return await apiRequest.get<PaymentStatusResponse>(`/payments/status/${orderId}`);
    } catch (error) {
      console.error('Check payment status error:', error);
      throw error;
    }
  }

  /**
   * Lấy thông tin banking cho chuyển khoản
   */
  async getBankingInfo(): Promise<{
    bankName: string;
    accountNumber: string;
    accountName: string;
    branch?: string;
    swiftCode?: string;
    note?: string;
  }> {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 300));

        return {
          bankName: 'Ngân hàng TMCP Á Châu (ACB)',
          accountNumber: '123456789',
          accountName: 'CONG TY TNHH DO CUNG VIET NAM',
          branch: 'Chi nhánh Sài Gòn',
          note: 'Nội dung chuyển khoản: [Mã đơn hàng] - [Họ tên]',
        };
      }

      return await apiRequest.get('/payments/banking-info');
    } catch (error) {
      console.error('Get banking info error:', error);
      throw error;
    }
  }

  /**
   * Lấy danh sách phương thức thanh toán khả dụng
   */
  async getAvailablePaymentMethods(): Promise<
    Array<{
      method: PaymentMethod;
      name: string;
      description: string;
      icon: string;
      isAvailable: boolean;
    }>
  > {
    try {
      if (USE_MOCK) {
        await new Promise(resolve => setTimeout(resolve, 300));

        return [
          {
            method: 'COD',
            name: 'Thanh toán khi nhận hàng',
            description: 'Thanh toán bằng tiền mặt khi nhận hàng',
            icon: '💵',
            isAvailable: true,
          },
          {
            method: 'VNPay',
            name: 'VNPay',
            description: 'Thanh toán qua VNPay (ATM/Visa/MasterCard)',
            icon: '🏦',
            isAvailable: true,
          },
          {
            method: 'Momo',
            name: 'Ví MoMo',
            description: 'Thanh toán qua ví điện tử MoMo',
            icon: '📱',
            isAvailable: true,
          },
          {
            method: 'ZaloPay',
            name: 'ZaloPay',
            description: 'Thanh toán qua ví điện tử ZaloPay',
            icon: '💳',
            isAvailable: true,
          },
          {
            method: 'BankTransfer',
            name: 'Chuyển khoản ngân hàng',
            description: 'Chuyển khoản trực tiếp qua ngân hàng',
            icon: '🏛️',
            isAvailable: true,
          },
        ];
      }

      return await apiRequest.get('/payments/methods');
    } catch (error) {
      console.error('Get payment methods error:', error);
      throw error;
    }
  }
}

export const paymentService = new PaymentService();
export default paymentService;

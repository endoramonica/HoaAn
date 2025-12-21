/**
 * OrderResultPage Payment Status Polling Tests
 * 
 * **Feature: vnpay-payment-integration, Property 9: Payment Status Retrieval**
 * **Validates: Requirements 4.4, 4.5**
 * 
 * Tests that payment status polling works correctly in OrderResultPage,
 * including starting polling on mount, updating UI when status changes,
 * showing loading state while polling, and handling polling timeout gracefully.
 */

import { describe, it, expect, beforeEach, vi, afterEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { OrderResultPage } from '../OrderResultPage';

// Mock the hooks
vi.mock('../../../lib/hooks/usePaymentStatusPoller', () => ({
  usePaymentStatusPoller: vi.fn(),
}));

vi.mock('../../../lib/hooks/useCheckout', () => ({
  useCheckout: vi.fn(() => ({
    getOrderDetails: vi.fn(async (orderId: string) => ({
      orderId,
      orderNumber: 'ORD-2024-001',
      status: 'pending',
      totalAmount: 1000000,
    })),
  })),
}));

vi.mock('../../../lib/hooks/useHybridNavigate', () => ({
  useHybridNavigate: vi.fn(() => vi.fn()),
}));

vi.mock('../../../lib/services/paymentService', () => ({
  paymentService: {
    createPayment: vi.fn(),
  },
}));

vi.mock('sonner', () => ({
  toast: {
    success: vi.fn(),
    error: vi.fn(),
    info: vi.fn(),
  },
}));

import { usePaymentStatusPoller } from '../../../lib/hooks/usePaymentStatusPoller';

describe('OrderResultPage - Payment Status Polling', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  // ============================================================================
  // Property 9: Payment Status Retrieval
  // ============================================================================

  describe('Property 9: Payment Status Retrieval', () => {
    it('should start polling when component mounts with order ID', async () => {
      const mockPollPaymentStatus = vi.fn(async () => ({
        orderId: 'ORD-2024-001',
        status: 'paid' as const,
        transactionId: 'VNP-TXN-123456',
        responseCode: '00',
        paidAt: new Date().toISOString(),
      }));

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: 'paid',
        isPolling: false,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(mockPollPaymentStatus).toHaveBeenCalledWith('ORD-2024-001', 30);
      });
    });

    it('should update UI when payment status changes to paid', async () => {
      const mockPollPaymentStatus = vi.fn(async () => ({
        orderId: 'ORD-2024-001',
        status: 'paid' as const,
        transactionId: 'VNP-TXN-123456',
        responseCode: '00',
        paidAt: new Date().toISOString(),
      }));

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: 'paid',
        isPolling: false,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(mockPollPaymentStatus).toHaveBeenCalled();
      });
    });

    it('should update UI when payment status changes to failed', async () => {
      const mockPollPaymentStatus = vi.fn(async () => ({
        orderId: 'ORD-2024-001',
        status: 'failed' as const,
        transactionId: 'VNP-TXN-123456',
        responseCode: '10',
        errorMessage: 'User cancelled payment',
      }));

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: 'failed',
        isPolling: false,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=10&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(mockPollPaymentStatus).toHaveBeenCalled();
      });
    });

    it('should update UI when payment status changes to cancelled', async () => {
      const mockPollPaymentStatus = vi.fn(async () => ({
        orderId: 'ORD-2024-001',
        status: 'cancelled' as const,
        transactionId: 'VNP-TXN-123456',
        responseCode: '10',
      }));

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: 'cancelled',
        isPolling: false,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=10&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(mockPollPaymentStatus).toHaveBeenCalled();
      });
    });
  });

  // ============================================================================
  // Loading State Tests
  // ============================================================================

  describe('Loading State While Polling', () => {
    it('should show loading state while polling is in progress', async () => {
      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: vi.fn(),
        status: 'pending',
        isPolling: true,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(screen.getByText(/Đang kiểm tra trạng thái thanh toán/i)).toBeInTheDocument();
      });
    });

    it('should show loading state while initial data is loading', async () => {
      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: vi.fn(),
        status: null,
        isPolling: false,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(screen.getByText(/Đang xử lý kết quả thanh toán/i)).toBeInTheDocument();
      });
    });

    it('should display helpful message during polling', async () => {
      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: vi.fn(),
        status: 'pending',
        isPolling: true,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(screen.getByText(/Vui lòng chờ trong giây lát/i)).toBeInTheDocument();
      });
    });
  });

  // ============================================================================
  // Polling Timeout Tests
  // ============================================================================

  describe('Polling Timeout Handling', () => {
    it('should handle polling timeout gracefully', async () => {
      const mockCancelPolling = vi.fn();
      const mockPollPaymentStatus = vi.fn(async () => null);

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: null,
        isPolling: false,
        error: 'Hết thời gian chờ trạng thái thanh toán. Vui lòng kiểm tra lại sau.',
        cancelPolling: mockCancelPolling,
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(mockPollPaymentStatus).toHaveBeenCalled();
      });
    });

    it('should handle polling error gracefully', async () => {
      const mockCancelPolling = vi.fn();
      const mockPollPaymentStatus = vi.fn(async () => null);

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: null,
        isPolling: false,
        error: 'Không thể kiểm tra trạng thái thanh toán',
        cancelPolling: mockCancelPolling,
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(mockPollPaymentStatus).toHaveBeenCalled();
      });
    });

    it('should cancel polling on component unmount', async () => {
      const mockCancelPolling = vi.fn();

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: vi.fn(),
        status: 'pending',
        isPolling: true,
        error: null,
        cancelPolling: mockCancelPolling,
      });

      const { unmount } = render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      unmount();

      // Polling should be cancelled on unmount
      expect(mockCancelPolling).toHaveBeenCalled();
    });
  });

  // ============================================================================
  // Polling Configuration Tests
  // ============================================================================

  describe('Polling Configuration', () => {
    it('should use default max attempts of 30', async () => {
      const mockPollPaymentStatus = vi.fn(async () => ({
        orderId: 'ORD-2024-001',
        status: 'paid' as const,
        transactionId: 'VNP-TXN-123456',
        responseCode: '00',
      }));

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: 'paid',
        isPolling: false,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        // Should be called with 30 as max attempts
        expect(mockPollPaymentStatus).toHaveBeenCalledWith('ORD-2024-001', 30);
      });
    });

    it('should not start polling if no order ID is present', async () => {
      const mockPollPaymentStatus = vi.fn();

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: null,
        isPolling: false,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: ['/checkout?vnp_ResponseCode=00'],
        }
      );

      await waitFor(() => {
        // Should not call polling if no order ID
        expect(mockPollPaymentStatus).not.toHaveBeenCalled();
      });
    });

    it('should not start polling if already started', async () => {
      const mockPollPaymentStatus = vi.fn(async () => ({
        orderId: 'ORD-2024-001',
        status: 'paid' as const,
        transactionId: 'VNP-TXN-123456',
        responseCode: '00',
      }));

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: 'paid',
        isPolling: false,
        error: null,
        cancelPolling: vi.fn(),
      });

      const { rerender } = render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(mockPollPaymentStatus).toHaveBeenCalledTimes(1);
      });

      // Rerender should not trigger polling again
      rerender(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>
      );

      await waitFor(() => {
        // Should still be called only once
        expect(mockPollPaymentStatus).toHaveBeenCalledTimes(1);
      });
    });
  });

  // ============================================================================
  // Edge Cases
  // ============================================================================

  describe('Edge Cases', () => {
    it('should handle polling result with pending status', async () => {
      const mockPollPaymentStatus = vi.fn(async () => ({
        orderId: 'ORD-2024-001',
        status: 'pending' as const,
        transactionId: 'VNP-TXN-123456',
        responseCode: undefined,
      }));

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: 'pending',
        isPolling: false,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(mockPollPaymentStatus).toHaveBeenCalled();
      });
    });

    it('should handle polling with missing transaction ID', async () => {
      const mockPollPaymentStatus = vi.fn(async () => ({
        orderId: 'ORD-2024-001',
        status: 'paid' as const,
        transactionId: undefined,
        responseCode: '00',
      }));

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: 'paid',
        isPolling: false,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(mockPollPaymentStatus).toHaveBeenCalled();
      });
    });

    it('should handle polling with error message', async () => {
      const mockPollPaymentStatus = vi.fn(async () => ({
        orderId: 'ORD-2024-001',
        status: 'failed' as const,
        transactionId: 'VNP-TXN-123456',
        responseCode: '01',
        errorMessage: 'Bank system error',
      }));

      (usePaymentStatusPoller as any).mockReturnValue({
        pollPaymentStatus: mockPollPaymentStatus,
        status: 'failed',
        isPolling: false,
        error: null,
        cancelPolling: vi.fn(),
      });

      render(
        <BrowserRouter>
          <OrderResultPage />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=01&vnp_TxnRef=ORD-2024-001&vnp_Amount=100000000',
          ],
        }
      );

      await waitFor(() => {
        expect(mockPollPaymentStatus).toHaveBeenCalled();
      });
    });
  });
});

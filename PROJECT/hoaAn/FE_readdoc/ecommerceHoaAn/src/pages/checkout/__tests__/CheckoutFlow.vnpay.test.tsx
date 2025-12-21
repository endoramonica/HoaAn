/**
 * CheckoutFlow VNPay Response Handling Tests
 * 
 * **Feature: vnpay-payment-integration, Property 10: Response Code Parameter Extraction**
 * **Validates: Requirements 2.1**
 * 
 * Tests that VNPay response parameters are correctly extracted from URL query strings
 * and routed to appropriate success/failed pages based on response code.
 */

import { describe, it, expect, beforeEach, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { CheckoutFlow } from '../CheckoutFlow';

// Mock the child components
vi.mock('../CheckoutPage', () => ({
  CheckoutPage: () => <div data-testid="checkout-page">Checkout Page</div>,
}));

vi.mock('../OrderSuccessPage', () => ({
  OrderSuccessPage: ({ orderData }: any) => (
    <div data-testid="order-success-page">
      Success Page - Order: {orderData?.orderId}
    </div>
  ),
}));

vi.mock('../OrderFailedPage', () => ({
  OrderFailedPage: ({ errorData }: any) => (
    <div data-testid="order-failed-page">
      Failed Page - Error: {errorData?.errorReason}
    </div>
  ),
}));

describe('CheckoutFlow - VNPay Response Handling', () => {
  // ============================================================================
  // Property 10: Response Code Parameter Extraction
  // ============================================================================

  describe('Property 10: Response Code Parameter Extraction', () => {
    it('should extract vnp_ResponseCode=00 and route to success page', async () => {
      const { container } = render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD123&vnp_TransactionNo=TXN456&vnp_Amount=1000000',
          ],
        }
      );

      await waitFor(() => {
        expect(screen.getByTestId('order-success-page')).toBeInTheDocument();
      });
    });

    it('should extract vnp_ResponseCode=01 and route to failed page', async () => {
      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=01&vnp_TxnRef=ORD123&vnp_TransactionNo=TXN456',
          ],
        }
      );

      await waitFor(() => {
        expect(screen.getByTestId('order-failed-page')).toBeInTheDocument();
      });
    });

    it('should extract vnp_ResponseCode=10 (user cancelled) and route to failed page', async () => {
      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=10&vnp_TxnRef=ORD123&vnp_TransactionNo=TXN456',
          ],
        }
      );

      await waitFor(() => {
        expect(screen.getByTestId('order-failed-page')).toBeInTheDocument();
      });
    });

    it('should extract all VNPay response parameters correctly', async () => {
      const testParams = {
        vnp_ResponseCode: '00',
        vnp_TxnRef: 'ORD-2024-001',
        vnp_TransactionNo: 'VNP-TXN-123456',
        vnp_Amount: '5000000',
        vnp_OrderInfo: 'Order for customer',
        vnp_TransactionDate: '20240101120000',
      };

      const queryString = new URLSearchParams(testParams).toString();

      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: [`/checkout?${queryString}`],
        }
      );

      await waitFor(() => {
        expect(screen.getByTestId('order-success-page')).toBeInTheDocument();
      });
    });

    it('should handle missing vnp_ResponseCode and show checkout page', async () => {
      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: ['/checkout'],
        }
      );

      await waitFor(() => {
        expect(screen.getByTestId('checkout-page')).toBeInTheDocument();
      });
    });

    it('should handle unknown response codes as failures', async () => {
      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=99&vnp_TxnRef=ORD123&vnp_TransactionNo=TXN456',
          ],
        }
      );

      await waitFor(() => {
        expect(screen.getByTestId('order-failed-page')).toBeInTheDocument();
      });
    });
  });

  // ============================================================================
  // VNPay Response Code Mapping Tests
  // ============================================================================

  describe('VNPay Response Code Mapping', () => {
    const testCases = [
      { code: '00', isSuccess: true, description: 'Success' },
      { code: '01', isSuccess: false, description: 'Bank system error' },
      { code: '02', isSuccess: false, description: 'Card locked' },
      { code: '03', isSuccess: false, description: 'Card expired' },
      { code: '04', isSuccess: false, description: 'Transaction declined' },
      { code: '05', isSuccess: false, description: 'Insufficient funds' },
      { code: '06', isSuccess: false, description: 'Incorrect OTP' },
      { code: '07', isSuccess: false, description: 'Transaction timeout' },
      { code: '09', isSuccess: false, description: 'Card not registered' },
      { code: '10', isSuccess: false, description: 'User cancelled' },
      { code: '11', isSuccess: false, description: 'Invalid amount' },
      { code: '12', isSuccess: false, description: 'Merchant not found' },
    ];

    testCases.forEach(({ code, isSuccess, description }) => {
      it(`should map response code ${code} (${description}) to ${isSuccess ? 'success' : 'failed'} page`, async () => {
        render(
          <BrowserRouter>
            <CheckoutFlow />
          </BrowserRouter>,
          {
            initialEntries: [
              `/checkout?vnp_ResponseCode=${code}&vnp_TxnRef=ORD123&vnp_TransactionNo=TXN456`,
            ],
          }
        );

        await waitFor(() => {
          const testId = isSuccess ? 'order-success-page' : 'order-failed-page';
          expect(screen.getByTestId(testId)).toBeInTheDocument();
        });
      });
    });
  });

  // ============================================================================
  // Legacy URL Parameter Support
  // ============================================================================

  describe('Legacy URL Parameter Support', () => {
    it('should support legacy step=success parameter', async () => {
      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: ['/checkout?step=success&orderId=ORD123&orderNumber=ORD-2024-001'],
        }
      );

      await waitFor(() => {
        expect(screen.getByTestId('order-success-page')).toBeInTheDocument();
      });
    });

    it('should support legacy step=failed parameter', async () => {
      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: ['/checkout?step=failed&orderId=ORD123&reason=Payment failed'],
        }
      );

      await waitFor(() => {
        expect(screen.getByTestId('order-failed-page')).toBeInTheDocument();
      });
    });

    it('should prioritize VNPay parameters over legacy parameters', async () => {
      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD123&step=failed&reason=Old reason',
          ],
        }
      );

      await waitFor(() => {
        // VNPay parameters should take precedence
        expect(screen.getByTestId('order-success-page')).toBeInTheDocument();
      });
    });
  });

  // ============================================================================
  // Edge Cases
  // ============================================================================

  describe('Edge Cases', () => {
    it('should handle empty response code gracefully', async () => {
      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: ['/checkout?vnp_ResponseCode=&vnp_TxnRef=ORD123'],
        }
      );

      // Should show checkout page when response code is empty
      await waitFor(() => {
        expect(screen.getByTestId('checkout-page')).toBeInTheDocument();
      });
    });

    it('should handle special characters in order info', async () => {
      const orderInfo = encodeURIComponent('Order for customer with special chars: @#$%');

      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: [
            `/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD123&vnp_OrderInfo=${orderInfo}`,
          ],
        }
      );

      await waitFor(() => {
        expect(screen.getByTestId('order-success-page')).toBeInTheDocument();
      });
    });

    it('should handle very large transaction amounts', async () => {
      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: [
            '/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD123&vnp_Amount=999999999999',
          ],
        }
      );

      await waitFor(() => {
        expect(screen.getByTestId('order-success-page')).toBeInTheDocument();
      });
    });

    it('should handle transaction date in various formats', async () => {
      const transactionDate = '20240101235959'; // YYYYMMDDHHmmss format

      render(
        <BrowserRouter>
          <CheckoutFlow />
        </BrowserRouter>,
        {
          initialEntries: [
            `/checkout?vnp_ResponseCode=00&vnp_TxnRef=ORD123&vnp_TransactionDate=${transactionDate}`,
          ],
        }
      );

      await waitFor(() => {
        expect(screen.getByTestId('order-success-page')).toBeInTheDocument();
      });
    });
  });
});

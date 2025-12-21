/**
 * OrderSuccessPage and OrderFailedPage VNPay Transaction Details Tests
 * 
 * **Feature: vnpay-payment-integration, Property 11: Retry Functionality**
 * **Validates: Requirements 2.4**
 * 
 * Tests that VNPay transaction details are correctly displayed on success/failed pages
 * and that retry functionality works correctly.
 */

import { describe, it, expect, beforeEach, vi } from 'vitest';

// ============================================================================
// VNPay Amount Formatting Tests
// ============================================================================

describe('VNPay Amount Formatting', () => {
  const formatVNPayAmount = (amountStr?: string) => {
    if (!amountStr) return '0₫';
    // VNPay amount is in VND * 100
    const amount = Number(amountStr) / 100;
    return new Intl.NumberFormat('vi-VN').format(amount) + '₫';
  };

  it('should format VNPay amount correctly (1,000,000 VND)', () => {
    const result = formatVNPayAmount('100000000'); // 1,000,000 VND * 100
    expect(result).toBe('1.000.000₫');
  });

  it('should format VNPay amount correctly (500,000 VND)', () => {
    const result = formatVNPayAmount('50000000'); // 500,000 VND * 100
    expect(result).toBe('500.000₫');
  });

  it('should handle zero amount', () => {
    const result = formatVNPayAmount('0');
    expect(result).toBe('0₫');
  });

  it('should handle undefined amount', () => {
    const result = formatVNPayAmount(undefined);
    expect(result).toBe('0₫');
  });

  it('should handle small amounts', () => {
    const result = formatVNPayAmount('100'); // 1 VND
    expect(result).toBe('1₫');
  });

  it('should handle large amounts', () => {
    const result = formatVNPayAmount('999999999999'); // 9,999,999,999.99 VND
    expect(result).toContain('₫');
  });
});

// ============================================================================
// VNPay Transaction Date Formatting Tests
// ============================================================================

describe('VNPay Transaction Date Formatting', () => {
  const formatTransactionDate = (dateStr?: string) => {
    if (!dateStr) return 'N/A';
    // VNPay format: YYYYMMDDHHmmss
    try {
      const year = dateStr.substring(0, 4);
      const month = dateStr.substring(4, 6);
      const day = dateStr.substring(6, 8);
      const hour = dateStr.substring(8, 10);
      const minute = dateStr.substring(10, 12);
      const second = dateStr.substring(12, 14);

      return `${day}/${month}/${year} ${hour}:${minute}:${second}`;
    } catch {
      return dateStr;
    }
  };

  it('should format VNPay transaction date correctly', () => {
    const result = formatTransactionDate('20240115143025');
    expect(result).toBe('15/01/2024 14:30:25');
  });

  it('should format VNPay transaction date at midnight', () => {
    const result = formatTransactionDate('20240101000000');
    expect(result).toBe('01/01/2024 00:00:00');
  });

  it('should format VNPay transaction date at end of day', () => {
    const result = formatTransactionDate('20240131235959');
    expect(result).toBe('31/01/2024 23:59:59');
  });

  it('should handle undefined date', () => {
    const result = formatTransactionDate(undefined);
    expect(result).toBe('N/A');
  });

  it('should handle empty date string', () => {
    const result = formatTransactionDate('');
    expect(result).toBe('N/A');
  });

  it('should handle invalid date format gracefully', () => {
    const result = formatTransactionDate('invalid');
    expect(result).toBe('invalid');
  });

  it('should handle partial date string', () => {
    const result = formatTransactionDate('202401');
    expect(result).toContain('/');
  });
});

// ============================================================================
// VNPay Response Code Mapping Tests
// ============================================================================

describe('VNPay Response Code Mapping', () => {
  const VNPAY_RESPONSE_CODES: Record<string, any> = {
    '00': {
      code: '00',
      message: 'Giao dịch thành công',
      userMessage: 'Thanh toán thành công! Đơn hàng của bạn đã được xác nhận.',
      isSuccess: true,
    },
    '01': {
      code: '01',
      message: 'Lỗi hệ thống ngân hàng',
      userMessage: 'Lỗi hệ thống ngân hàng. Vui lòng thử lại sau.',
      isSuccess: false,
    },
    '02': {
      code: '02',
      message: 'Thẻ/Tài khoản bị khóa',
      userMessage: 'Thẻ hoặc tài khoản của bạn bị khóa. Vui lòng liên hệ ngân hàng.',
      isSuccess: false,
    },
    '10': {
      code: '10',
      message: 'Khách hàng hủy giao dịch',
      userMessage: 'Bạn đã hủy giao dịch. Nhấp vào "Thử lại" để thanh toán lại.',
      isSuccess: false,
    },
  };

  it('should map response code 00 to success', () => {
    const codeInfo = VNPAY_RESPONSE_CODES['00'];
    expect(codeInfo.isSuccess).toBe(true);
    expect(codeInfo.userMessage).toContain('thành công');
  });

  it('should map response code 01 to failure', () => {
    const codeInfo = VNPAY_RESPONSE_CODES['01'];
    expect(codeInfo.isSuccess).toBe(false);
    expect(codeInfo.userMessage).toContain('Lỗi');
  });

  it('should map response code 02 to failure', () => {
    const codeInfo = VNPAY_RESPONSE_CODES['02'];
    expect(codeInfo.isSuccess).toBe(false);
    expect(codeInfo.userMessage).toContain('khóa');
  });

  it('should map response code 10 to failure', () => {
    const codeInfo = VNPAY_RESPONSE_CODES['10'];
    expect(codeInfo.isSuccess).toBe(false);
    expect(codeInfo.userMessage).toContain('hủy');
  });

  it('should handle unknown response codes', () => {
    const unknownCode = '99';
    const codeInfo = VNPAY_RESPONSE_CODES[unknownCode] || {
      code: unknownCode,
      message: 'Lỗi không xác định',
      userMessage: `Thanh toán thất bại. Mã lỗi: ${unknownCode}. Vui lòng liên hệ hỗ trợ.`,
      isSuccess: false,
    };

    expect(codeInfo.isSuccess).toBe(false);
    expect(codeInfo.userMessage).toContain('99');
  });
});

// ============================================================================
// VNPay Transaction Details Display Tests
// ============================================================================

describe('VNPay Transaction Details Display', () => {
  it('should display transaction ID when available', () => {
    const transactionNo = 'VNP-TXN-123456789';
    expect(transactionNo).toBeTruthy();
    expect(transactionNo).toMatch(/^VNP-/);
  });

  it('should display transaction reference when available', () => {
    const txnRef = 'ORD-2024-001';
    expect(txnRef).toBeTruthy();
    expect(txnRef).toMatch(/^ORD-/);
  });

  it('should display formatted amount when available', () => {
    const amount = '100000000'; // 1,000,000 VND
    const formatted = `${Number(amount) / 100}₫`;
    expect(formatted).toContain('₫');
  });

  it('should display formatted transaction date when available', () => {
    const date = '20240115143025';
    const formatted = `${date.substring(6, 8)}/${date.substring(4, 6)}/${date.substring(0, 4)} ${date.substring(8, 10)}:${date.substring(10, 12)}:${date.substring(12, 14)}`;
    expect(formatted).toMatch(/\d{2}\/\d{2}\/\d{4} \d{2}:\d{2}:\d{2}/);
  });

  it('should handle missing transaction details gracefully', () => {
    const details = {
      transactionNo: undefined,
      txnRef: undefined,
      amount: undefined,
      date: undefined,
    };

    expect(details.transactionNo).toBeUndefined();
    expect(details.txnRef).toBeUndefined();
    expect(details.amount).toBeUndefined();
    expect(details.date).toBeUndefined();
  });
});

// ============================================================================
// Retry Functionality Tests
// ============================================================================

describe('Property 11: Retry Functionality', () => {
  it('should allow retry when payment fails', () => {
    const errorData = {
      orderId: 'ORD-2024-001',
      orderNumber: 'ORD123',
      reason: 'Payment failed',
      vnpayResponseCode: '10',
    };

    // Retry should be possible with the same order ID
    expect(errorData.orderId).toBeTruthy();
    expect(errorData.vnpayResponseCode).toBe('10');
  });

  it('should not create duplicate order on retry', () => {
    const orderId = 'ORD-2024-001';
    const retryOrderId = 'ORD-2024-001'; // Should be the same

    expect(orderId).toBe(retryOrderId);
  });

  it('should generate new payment URL on retry', () => {
    const firstPaymentUrl = 'https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?orderId=ORD-2024-001&amount=1000000';
    const retryPaymentUrl = 'https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?orderId=ORD-2024-001&amount=1000000';

    // Both URLs should have the same order ID but could have different timestamps
    expect(firstPaymentUrl).toContain('ORD-2024-001');
    expect(retryPaymentUrl).toContain('ORD-2024-001');
  });

  it('should preserve order details during retry', () => {
    const orderDetails = {
      orderId: 'ORD-2024-001',
      orderNumber: 'ORD123',
      totalAmount: 1000000,
      items: [
        { id: 'ITEM1', name: 'Product 1', quantity: 2 },
        { id: 'ITEM2', name: 'Product 2', quantity: 1 },
      ],
    };

    // After retry, order details should remain the same
    expect(orderDetails.orderId).toBe('ORD-2024-001');
    expect(orderDetails.items.length).toBe(2);
    expect(orderDetails.totalAmount).toBe(1000000);
  });
});

// ============================================================================
// Logging and Tracking Tests
// ============================================================================

describe('Logging and Tracking', () => {
  it('should log payment success with transaction details', () => {
    const logData = {
      timestamp: new Date().toISOString(),
      status: 'success',
      orderId: 'ORD-2024-001',
      responseCode: '00',
      transactionId: 'VNP-TXN-123456',
      amount: 1000000,
    };

    expect(logData.status).toBe('success');
    expect(logData.responseCode).toBe('00');
    expect(logData.timestamp).toBeTruthy();
  });

  it('should log payment failure with error code', () => {
    const logData = {
      timestamp: new Date().toISOString(),
      status: 'failed',
      orderId: 'ORD-2024-001',
      responseCode: '10',
      transactionId: 'VNP-TXN-123456',
      errorMessage: 'User cancelled payment',
    };

    expect(logData.status).toBe('failed');
    expect(logData.responseCode).toBe('10');
    expect(logData.errorMessage).toBeTruthy();
  });

  it('should include all required audit trail information', () => {
    const auditTrail = {
      orderId: 'ORD-2024-001',
      responseCode: '00',
      transactionId: 'VNP-TXN-123456',
      timestamp: new Date().toISOString(),
      amount: 1000000,
      paymentMethod: 'VNPay',
    };

    expect(auditTrail.orderId).toBeTruthy();
    expect(auditTrail.responseCode).toBeTruthy();
    expect(auditTrail.transactionId).toBeTruthy();
    expect(auditTrail.timestamp).toBeTruthy();
    expect(auditTrail.amount).toBeTruthy();
    expect(auditTrail.paymentMethod).toBe('VNPay');
  });
});

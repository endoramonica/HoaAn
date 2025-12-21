/**
 * CheckoutFlow - Quản lý luồng checkout với useState
 * Steps: cart -> checkout -> payment -> success/failed
 * 
 * Hỗ trợ cả navigation thông qua URL params:
 * - /checkout?step=success&orderId=xxx
 * - /checkout?step=failed&reason=xxx
 * - /checkout?vnp_ResponseCode=00&vnp_TxnRef=xxx (VNPay callback)
 */

import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { CheckoutPage } from './CheckoutPage';
import { OrderSuccessPage } from './OrderSuccessPage';
import { OrderFailedPage } from './OrderFailedPage';

type CheckoutStep = 'checkout' | 'success' | 'failed';

interface CheckoutFlowData {
  orderId?: string;
  orderNumber?: string;
  paymentMethod?: string;
  errorReason?: string;
  vnpayResponseCode?: string;
  vnpayTransactionRef?: string;
  vnpayTransactionNo?: string;
  vnpayAmount?: string;
  vnpayOrderInfo?: string;
  vnpayTransactionDate?: string;
}

// ============================================================================
// VNPay Response Code Mapping
// ============================================================================

interface VNPayResponseCodeInfo {
  code: string;
  message: string;
  userMessage: string;
  isSuccess: boolean;
}

const VNPAY_RESPONSE_CODES: Record<string, VNPayResponseCodeInfo> = {
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
  '03': {
    code: '03',
    message: 'Thẻ/Tài khoản hết hạn',
    userMessage: 'Thẻ hoặc tài khoản của bạn đã hết hạn. Vui lòng sử dụng thẻ khác.',
    isSuccess: false,
  },
  '04': {
    code: '04',
    message: 'Giao dịch bị từ chối',
    userMessage: 'Giao dịch bị từ chối. Vui lòng thử lại hoặc sử dụng thẻ khác.',
    isSuccess: false,
  },
  '05': {
    code: '05',
    message: 'Số dư không đủ',
    userMessage: 'Số dư tài khoản không đủ. Vui lòng kiểm tra lại số dư.',
    isSuccess: false,
  },
  '06': {
    code: '06',
    message: 'Nhập sai OTP',
    userMessage: 'Bạn nhập sai mã OTP. Vui lòng thử lại.',
    isSuccess: false,
  },
  '07': {
    code: '07',
    message: 'Hết thời gian chờ',
    userMessage: 'Giao dịch hết thời gian chờ. Vui lòng thử lại.',
    isSuccess: false,
  },
  '09': {
    code: '09',
    message: 'Thẻ chưa đăng ký dịch vụ',
    userMessage: 'Thẻ của bạn chưa được đăng ký dịch vụ thanh toán online. Vui lòng liên hệ ngân hàng.',
    isSuccess: false,
  },
  '10': {
    code: '10',
    message: 'Khách hàng hủy giao dịch',
    userMessage: 'Bạn đã hủy giao dịch. Nhấp vào "Thử lại" để thanh toán lại.',
    isSuccess: false,
  },
  '11': {
    code: '11',
    message: 'Số tiền không hợp lệ',
    userMessage: 'Số tiền thanh toán không hợp lệ. Vui lòng liên hệ hỗ trợ.',
    isSuccess: false,
  },
  '12': {
    code: '12',
    message: 'Merchant không tồn tại',
    userMessage: 'Lỗi cấu hình cổng thanh toán. Vui lòng liên hệ hỗ trợ.',
    isSuccess: false,
  },
};

export const CheckoutFlow = () => {
  const [searchParams] = useSearchParams();
  const [currentStep, setCurrentStep] = useState<CheckoutStep>('checkout');
  const [flowData, setFlowData] = useState<CheckoutFlowData>({});

  // ============================================================================
  // Extract and Process VNPay Response Parameters
  // ============================================================================

  const extractVNPayResponseParams = () => {
    const vnpResponseCode = searchParams.get('vnp_ResponseCode');
    const vnpTxnRef = searchParams.get('vnp_TxnRef');
    const vnpTransactionNo = searchParams.get('vnp_TransactionNo');
    const vnpAmount = searchParams.get('vnp_Amount');
    const vnpOrderInfo = searchParams.get('vnp_OrderInfo');
    const vnpTransactionDate = searchParams.get('vnp_TransactionDate');

    // Only process if we have VNPay response code
    if (!vnpResponseCode) {
      return null;
    }

    console.log('[CheckoutFlow] 📋 VNPay response parameters extracted:', {
      responseCode: vnpResponseCode,
      txnRef: vnpTxnRef,
      transactionNo: vnpTransactionNo,
      amount: vnpAmount,
      orderInfo: vnpOrderInfo,
      transactionDate: vnpTransactionDate,
    });

    // Log transaction details for audit trail
    console.log('[CheckoutFlow] 📊 VNPay Transaction Details:', {
      timestamp: new Date().toISOString(),
      responseCode: vnpResponseCode,
      orderId: vnpTxnRef,
      transactionId: vnpTransactionNo,
      amount: vnpAmount ? `${Number(vnpAmount) / 100}₫` : 'N/A',
    });

    return {
      vnpResponseCode,
      vnpTxnRef,
      vnpTransactionNo,
      vnpAmount,
      vnpOrderInfo,
      vnpTransactionDate,
    };
  };

  // Check URL params on mount
  useEffect(() => {
    // ============================================================================
    // STEP 1: Check for VNPay Response Parameters
    // ============================================================================
    const vnpayParams = extractVNPayResponseParams();

    if (vnpayParams) {
      console.log('[CheckoutFlow] 🔄 Processing VNPay response...');

      const responseCodeInfo = VNPAY_RESPONSE_CODES[vnpayParams.vnpResponseCode] || {
        code: vnpayParams.vnpResponseCode,
        message: 'Lỗi không xác định',
        userMessage: `Thanh toán thất bại. Mã lỗi: ${vnpayParams.vnpResponseCode}. Vui lòng liên hệ hỗ trợ.`,
        isSuccess: false,
      };

      console.log('[CheckoutFlow] 📊 Response code info:', responseCodeInfo);

      // Route based on response code
      if (responseCodeInfo.isSuccess) {
        console.log('[CheckoutFlow] ✅ VNPay payment successful - routing to success page');
        setCurrentStep('success');
        setFlowData({
          orderId: vnpayParams.vnpTxnRef || undefined,
          orderNumber: vnpayParams.vnpOrderInfo || undefined,
          paymentMethod: 'VNPay',
          vnpayResponseCode: vnpayParams.vnpResponseCode,
          vnpayTransactionRef: vnpayParams.vnpTxnRef,
          vnpayTransactionNo: vnpayParams.vnpTransactionNo,
          vnpayAmount: vnpayParams.vnpAmount,
          vnpayOrderInfo: vnpayParams.vnpOrderInfo,
          vnpayTransactionDate: vnpayParams.vnpTransactionDate,
        });
      } else {
        console.log('[CheckoutFlow] ❌ VNPay payment failed - routing to failed page');
        setCurrentStep('failed');
        setFlowData({
          orderId: vnpayParams.vnpTxnRef || undefined,
          orderNumber: vnpayParams.vnpOrderInfo || undefined,
          errorReason: responseCodeInfo.userMessage,
          vnpayResponseCode: vnpayParams.vnpResponseCode,
          vnpayTransactionRef: vnpayParams.vnpTxnRef,
          vnpayTransactionNo: vnpayParams.vnpTransactionNo,
          vnpayAmount: vnpayParams.vnpAmount,
          vnpayOrderInfo: vnpayParams.vnpOrderInfo,
          vnpayTransactionDate: vnpayParams.vnpTransactionDate,
        });
      }

      return;
    }

    // ============================================================================
    // STEP 2: Check for Legacy URL Parameters
    // ============================================================================
    const stepParam = searchParams.get('step') as CheckoutStep | null;
    const paymentParam = searchParams.get('payment');
    const orderId = searchParams.get('orderId');
    const orderNumber = searchParams.get('orderNumber');
    const reason = searchParams.get('reason');

    if (stepParam && (stepParam === 'success' || stepParam === 'failed')) {
      console.log('[CheckoutFlow] 🔄 Processing legacy URL parameters:', { step: stepParam });
      setCurrentStep(stepParam);
      setFlowData({
        orderId: orderId || undefined,
        orderNumber: orderNumber || undefined,
        errorReason: reason || undefined,
      });
    } else if (paymentParam) {
      // Legacy support for payment callback URLs
      console.log('[CheckoutFlow] 🔄 Processing legacy payment parameter:', { payment: paymentParam });
      if (paymentParam === 'success') {
        setCurrentStep('success');
        setFlowData({ orderId: orderId || undefined });
      } else if (paymentParam === 'failed') {
        setCurrentStep('failed');
        setFlowData({ orderId: orderId || undefined, errorReason: reason });
      }
    }
  }, [searchParams]);

  const handleNavigate = (step: CheckoutStep, data?: CheckoutFlowData) => {
    setCurrentStep(step);
    if (data) {
      setFlowData(prev => ({ ...prev, ...data }));
    }
  };

  switch (currentStep) {
    case 'success':
      return (
        <OrderSuccessPage
          orderData={{
            orderId: flowData.orderId || '',
            orderNumber: flowData.orderNumber || '',
            paymentMethod: flowData.paymentMethod,
          }}
        />
      );

    case 'failed':
      return (
        <OrderFailedPage
          errorData={{
            orderId: flowData.orderId,
            orderNumber: flowData.orderNumber,
            reason: flowData.errorReason,
          }}
        />
      );

    case 'checkout':
    default:
      return <CheckoutPage onNavigate={handleNavigate} />;
  }
};

export default CheckoutFlow;

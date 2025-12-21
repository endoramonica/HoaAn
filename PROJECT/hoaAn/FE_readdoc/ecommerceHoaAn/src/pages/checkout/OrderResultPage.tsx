/**
 * OrderResultPage - Trang kết quả thanh toán VNPay
 * Xử lý response từ VNPay gateway và hiển thị kết quả thanh toán
 * 
 * URL Parameters:
 * - vnp_ResponseCode: Mã kết quả thanh toán (00 = thành công)
 * - vnp_TxnRef: Mã tham chiếu giao dịch (Order ID)
 * - vnp_TransactionNo: Mã giao dịch VNPay
 * - vnp_Amount: Số tiền thanh toán (tính bằng VND * 100)
 * - vnp_OrderInfo: Thông tin đơn hàng
 * - vnp_TransactionDate: Ngày giờ giao dịch
 * - vnp_SecureHash: Chữ ký bảo mật
 */

import { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import { useHybridNavigate } from '../../lib/hooks/useHybridNavigate';
import { useCheckout } from '../../lib/hooks/useCheckout';
import { usePaymentStatusPoller } from '../../lib/hooks/usePaymentStatusPoller';
import { paymentService } from '../../lib/services/paymentService';
import { Button } from '../../components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Alert, AlertDescription } from '../../components/ui/alert';
import {
  CheckCircle2,
  XCircle,
  AlertTriangle,
  RefreshCw,
  Home,
  Loader2,
  Copy,
  Flower2,
} from 'lucide-react';
import { toast } from 'sonner';

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

// ============================================================================
// VNPay Response Parameters Interface
// ============================================================================

interface VNPayResponseParams {
  vnp_ResponseCode?: string;
  vnp_TxnRef?: string;
  vnp_TransactionNo?: string;
  vnp_Amount?: string;
  vnp_OrderInfo?: string;
  vnp_TransactionDate?: string;
  vnp_SecureHash?: string;
  vnp_BankCode?: string;
  vnp_BankTranNo?: string;
  vnp_CardType?: string;
}

// ============================================================================
// OrderResultPage Component
// ============================================================================

export const OrderResultPage = () => {
  const navigate = useHybridNavigate();
  const [searchParams] = useSearchParams();
  const { getOrderDetails } = useCheckout();
  const { pollPaymentStatus, status: pollingStatus, isPolling, error: pollingError, cancelPolling } = usePaymentStatusPoller();

  // State
  const [responseParams, setResponseParams] = useState<VNPayResponseParams>({});
  const [responseCodeInfo, setResponseCodeInfo] = useState<VNPayResponseCodeInfo | null>(null);
  const [orderDetails, setOrderDetails] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isRetrying, setIsRetrying] = useState(false);
  const [pollingStarted, setPollingStarted] = useState(false);

  // ============================================================================
  // Extract VNPay Response Parameters from URL
  // ============================================================================

  useEffect(() => {
    console.log('[OrderResultPage] 📋 Extracting VNPay response parameters...');

    const params: VNPayResponseParams = {
      vnp_ResponseCode: searchParams.get('vnp_ResponseCode') || undefined,
      vnp_TxnRef: searchParams.get('vnp_TxnRef') || undefined,
      vnp_TransactionNo: searchParams.get('vnp_TransactionNo') || undefined,
      vnp_Amount: searchParams.get('vnp_Amount') || undefined,
      vnp_OrderInfo: searchParams.get('vnp_OrderInfo') || undefined,
      vnp_TransactionDate: searchParams.get('vnp_TransactionDate') || undefined,
      vnp_SecureHash: searchParams.get('vnp_SecureHash') || undefined,
      vnp_BankCode: searchParams.get('vnp_BankCode') || undefined,
      vnp_BankTranNo: searchParams.get('vnp_BankTranNo') || undefined,
      vnp_CardType: searchParams.get('vnp_CardType') || undefined,
    };

    console.log('[OrderResultPage] ✅ Response parameters extracted:', {
      responseCode: params.vnp_ResponseCode,
      txnRef: params.vnp_TxnRef,
      transactionNo: params.vnp_TransactionNo,
      amount: params.vnp_Amount,
    });

    setResponseParams(params);

    // Get response code info
    const responseCode = params.vnp_ResponseCode || 'unknown';
    const codeInfo = VNPAY_RESPONSE_CODES[responseCode] || {
      code: responseCode,
      message: 'Lỗi không xác định',
      userMessage: `Thanh toán thất bại. Mã lỗi: ${responseCode}. Vui lòng liên hệ hỗ trợ.`,
      isSuccess: false,
    };

    console.log('[OrderResultPage] 📊 Response code info:', codeInfo);
    setResponseCodeInfo(codeInfo);

    // Load order details if we have txnRef (Order ID)
    if (params.vnp_TxnRef) {
      loadOrderDetails(params.vnp_TxnRef);
    } else {
      setIsLoading(false);
    }
  }, [searchParams]);

  // ============================================================================
  // Load Order Details
  // ============================================================================

  const loadOrderDetails = async (orderId: string) => {
    try {
      console.log('[OrderResultPage] 🔍 Loading order details for:', orderId);

      const details = await getOrderDetails(orderId);

      if (details) {
        console.log('[OrderResultPage] ✅ Order details loaded:', {
          orderId: details.orderId,
          orderNumber: details.orderNumber,
          status: details.status,
          totalAmount: details.totalAmount,
        });

        setOrderDetails(details);
      } else {
        console.warn('[OrderResultPage] ⚠️ Failed to load order details');
      }
    } catch (error) {
      console.error('[OrderResultPage] ❌ Load order details error:', error);
    } finally {
      setIsLoading(false);
    }
  };

  // ============================================================================
  // Start Payment Status Polling
  // ============================================================================

  useEffect(() => {
    // Only start polling if we have an order ID and haven't started yet
    if (responseParams.vnp_TxnRef && !pollingStarted && !isLoading) {
      console.log('[OrderResultPage] 🔄 Starting payment status polling for order:', responseParams.vnp_TxnRef);
      setPollingStarted(true);

      const startPolling = async () => {
        try {
          const result = await pollPaymentStatus(responseParams.vnp_TxnRef, 30);

          if (result) {
            console.log('[OrderResultPage] ✅ Payment status polling completed:', {
              status: result.status,
              transactionId: result.transactionId,
              responseCode: result.responseCode,
            });

            // Update UI based on polling result
            if (result.status === 'paid') {
              console.log('[OrderResultPage] 💚 Payment confirmed as paid');
              toast.success('Thanh toán đã được xác nhận!');
            } else if (result.status === 'failed') {
              console.log('[OrderResultPage] ❌ Payment confirmed as failed');
              toast.error('Thanh toán thất bại');
            } else if (result.status === 'cancelled') {
              console.log('[OrderResultPage] ⚠️ Payment was cancelled');
              toast.info('Thanh toán đã bị hủy');
            }
          } else {
            console.warn('[OrderResultPage] ⚠️ Payment status polling returned no result');
            if (pollingError) {
              toast.error(pollingError);
            }
          }
        } catch (error) {
          console.error('[OrderResultPage] ❌ Payment status polling error:', error);
          toast.error('Không thể kiểm tra trạng thái thanh toán');
        }
      };

      startPolling();
    }

    // Cleanup on unmount
    return () => {
      if (pollingStarted) {
        console.log('[OrderResultPage] 🛑 Cancelling payment status polling on unmount');
        cancelPolling();
      }
    };
  }, [responseParams.vnp_TxnRef, pollingStarted, isLoading, pollPaymentStatus, cancelPolling, pollingError]);

  // ============================================================================
  // Handle Retry Payment
  // ============================================================================

  const handleRetry = async () => {
    try {
      setIsRetrying(true);
      console.log('[OrderResultPage] 🔄 Retrying payment for order:', responseParams.vnp_TxnRef);

      if (!responseParams.vnp_TxnRef) {
        toast.error('Không tìm thấy mã đơn hàng');
        navigate('checkout');
        return;
      }

      // Create new payment for the same order
      const paymentResponse = await paymentService.createPayment({
        orderId: responseParams.vnp_TxnRef,
        amount: orderDetails?.totalAmount || Number(responseParams.vnp_Amount || 0) / 100,
        paymentMethod: 'VNPay' as any,
        returnUrl: `${window.location.origin}/checkout?step=result`,
        cancelUrl: `${window.location.origin}/checkout?step=result`,
      });

      console.log('[OrderResultPage] ✅ Payment retry response:', {
        hasPaymentUrl: !!paymentResponse.paymentUrl,
        transactionId: paymentResponse.transactionId,
      });

      if (paymentResponse.paymentUrl) {
        console.log('[OrderResultPage] 🔄 Redirecting to payment gateway for retry');
        toast.success('Đang chuyển đến cổng thanh toán...');
        window.location.href = paymentResponse.paymentUrl;
      } else {
        toast.error('Không thể tạo link thanh toán');
      }
    } catch (error) {
      console.error('[OrderResultPage] ❌ Retry payment error:', error);
      toast.error('Không thể thực hiện thanh toán lại');
    } finally {
      setIsRetrying(false);
    }
  };

  // ============================================================================
  // Handle Copy Order Number
  // ============================================================================

  const handleCopyOrderNumber = () => {
    if (orderDetails?.orderNumber) {
      navigator.clipboard.writeText(orderDetails.orderNumber);
      toast.success('Đã sao chép mã đơn hàng');
    }
  };

  // ============================================================================
  // Format Price
  // ============================================================================

  const formatPrice = (price?: number | string) => {
    if (!price) return '0₫';
    const numPrice = typeof price === 'string' ? Number(price) : price;
    return new Intl.NumberFormat('vi-VN').format(numPrice) + '₫';
  };

  // ============================================================================
  // Format Transaction Date
  // ============================================================================

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

  // ============================================================================
  // Loading State
  // ============================================================================

  if (isLoading || isPolling) {
    return (
      <div className="min-h-screen bg-[#FFFBEB] flex items-center justify-center">
        <div className="text-center">
          <Loader2 className="w-12 h-12 animate-spin text-[#92400E] mx-auto mb-4" />
          <p className="text-[#92400E]/70">
            {isLoading ? 'Đang xử lý kết quả thanh toán...' : 'Đang kiểm tra trạng thái thanh toán...'}
          </p>
          {isPolling && (
            <p className="text-sm text-[#92400E]/50 mt-2">Vui lòng chờ trong giây lát</p>
          )}
        </div>
      </div>
    );
  }

  // ============================================================================
  // Success Page
  // ============================================================================

  if (responseCodeInfo?.isSuccess) {
    return (
      <div className="min-h-screen bg-[#FFFBEB] px-4 py-12">
        <div className="max-w-3xl mx-auto">
          {/* Success Header */}
          <div className="text-center mb-8">
            <div className="inline-flex items-center justify-center w-20 h-20 bg-gradient-to-br from-[#92400E] to-[#F59E0B] rounded-full mb-4 shadow-lg">
              <CheckCircle2 className="w-10 h-10 text-white" />
            </div>
            <h1 className="text-3xl text-[#92400E] mb-2">Thanh Toán Thành Công!</h1>
            <p className="text-[#92400E]/70">
              Cảm ơn bạn đã tin tưởng và mua sắm tại cửa hàng đồ cúng truyền thống
            </p>
          </div>

          {/* Order Number */}
          {orderDetails?.orderNumber && (
            <Card className="mb-6 border-2 border-[#92400E]/20">
              <CardContent className="pt-6">
                <div className="text-center">
                  <p className="text-sm text-[#92400E]/70 mb-2">Mã đơn hàng</p>
                  <div className="flex items-center justify-center space-x-2">
                    <span className="text-2xl text-[#DC2626]">{orderDetails.orderNumber}</span>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={handleCopyOrderNumber}
                      className="text-[#92400E] hover:text-[#92400E]/80 hover:bg-[#92400E]/5"
                    >
                      <Copy className="w-4 h-4" />
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          )}

          {/* VNPay Transaction Details */}
          {responseParams.vnp_TransactionNo && (
            <Card className="mb-6 border-2 border-[#92400E]/20">
              <CardHeader className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5">
                <CardTitle className="text-[#92400E]">Chi Tiết Giao Dịch VNPay</CardTitle>
              </CardHeader>
              <CardContent className="pt-6">
                <div className="space-y-3 bg-white p-4 rounded-lg border border-[#92400E]/20">
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <p className="text-xs text-[#92400E]/70">Mã giao dịch VNPay</p>
                      <p className="text-sm text-[#92400E] font-mono break-all">{responseParams.vnp_TransactionNo}</p>
                    </div>
                    <div>
                      <p className="text-xs text-[#92400E]/70">Mã tham chiếu</p>
                      <p className="text-sm text-[#92400E] font-mono break-all">{responseParams.vnp_TxnRef}</p>
                    </div>
                  </div>
                  {responseParams.vnp_Amount && (
                    <div>
                      <p className="text-xs text-[#92400E]/70">Số tiền thanh toán</p>
                      <p className="text-lg text-[#92400E]">{formatPrice(Number(responseParams.vnp_Amount) / 100)}</p>
                    </div>
                  )}
                  {responseParams.vnp_TransactionDate && (
                    <div>
                      <p className="text-xs text-[#92400E]/70">Thời gian giao dịch</p>
                      <p className="text-sm text-[#92400E]">{formatTransactionDate(responseParams.vnp_TransactionDate)}</p>
                    </div>
                  )}
                </div>
              </CardContent>
            </Card>
          )}

          {/* Action Buttons */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
            <Button
              onClick={() => navigate('orders')}
              className="bg-gradient-to-r from-[#92400E] to-[#F59E0B] hover:from-[#92400E]/90 hover:to-[#F59E0B]/90 text-white"
            >
              <Home className="mr-2 h-4 w-4" />
              Xem Đơn Hàng
            </Button>
            <Button
              onClick={() => navigate('home')}
              variant="outline"
              className="border-[#92400E] text-[#92400E] hover:bg-[#92400E]/5"
            >
              <Home className="mr-2 h-4 w-4" />
              Về Trang Chủ
            </Button>
          </div>

          {/* Footer */}
          <div className="text-center pt-8 border-t-2 border-[#92400E]/10">
            <Flower2 className="w-12 h-12 text-[#92400E]/20 mx-auto mb-4" />
            <p className="text-[#92400E]/60">
              Cảm ơn bạn đã mua sắm tại cửa hàng đồ cúng truyền thống! 🙏
            </p>
          </div>
        </div>
      </div>
    );
  }

  // ============================================================================
  // Failed Page
  // ============================================================================

  return (
    <div className="min-h-screen bg-[#FFFBEB] px-4 py-12">
      <div className="max-w-3xl mx-auto">
        {/* Error Header */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-20 h-20 bg-gradient-to-br from-[#DC2626] to-[#92400E] rounded-full mb-4 shadow-lg">
            <XCircle className="w-10 h-10 text-white" />
          </div>
          <h1 className="text-3xl text-[#DC2626] mb-2">Thanh Toán Thất Bại</h1>
          <p className="text-[#92400E]/70">
            {responseCodeInfo?.userMessage || 'Đã có lỗi xảy ra trong quá trình thanh toán'}
          </p>
        </div>

        {/* Error Details */}
        <Alert variant="destructive" className="mb-6 border-[#DC2626] bg-[#DC2626]/10">
          <AlertTriangle className="h-4 w-4" />
          <AlertDescription>
            {responseCodeInfo?.message || 'Giao dịch không thành công'}
          </AlertDescription>
        </Alert>

        {/* Order Number */}
        {orderDetails?.orderNumber && (
          <Card className="mb-6 border-2 border-[#92400E]/20">
            <CardContent className="pt-6">
              <div className="text-center">
                <p className="text-sm text-[#92400E]/70 mb-2">Mã đơn hàng</p>
                <p className="text-xl text-[#92400E]">{orderDetails.orderNumber}</p>
                <p className="text-xs text-[#92400E]/60 mt-2">
                  Đơn hàng của bạn vẫn được lưu và chờ thanh toán
                </p>
              </div>
            </CardContent>
          </Card>
        )}

        {/* VNPay Transaction Details */}
        {responseParams.vnp_TransactionNo && (
          <Card className="mb-6 border-2 border-[#92400E]/20">
            <CardHeader className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5">
              <CardTitle className="text-[#92400E] flex items-center">
                <AlertTriangle className="mr-2 h-5 w-5" />
                Chi Tiết Giao Dịch VNPay
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-6">
              <div className="space-y-3 bg-white p-4 rounded-lg border border-[#92400E]/20">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-xs text-[#92400E]/70">Mã giao dịch VNPay</p>
                    <p className="text-sm text-[#92400E] font-mono break-all">{responseParams.vnp_TransactionNo}</p>
                  </div>
                  <div>
                    <p className="text-xs text-[#92400E]/70">Mã tham chiếu</p>
                    <p className="text-sm text-[#92400E] font-mono break-all">{responseParams.vnp_TxnRef}</p>
                  </div>
                </div>
                {responseParams.vnp_ResponseCode && (
                  <div>
                    <p className="text-xs text-[#92400E]/70">Mã lỗi VNPay</p>
                    <p className="text-sm text-[#DC2626] font-mono">{responseParams.vnp_ResponseCode}</p>
                  </div>
                )}
                {responseParams.vnp_Amount && (
                  <div>
                    <p className="text-xs text-[#92400E]/70">Số tiền thanh toán</p>
                    <p className="text-lg text-[#DC2626]">{formatPrice(Number(responseParams.vnp_Amount) / 100)}</p>
                  </div>
                )}
                {responseParams.vnp_TransactionDate && (
                  <div>
                    <p className="text-xs text-[#92400E]/70">Thời gian giao dịch</p>
                    <p className="text-sm text-[#92400E]">{formatTransactionDate(responseParams.vnp_TransactionDate)}</p>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        )}

        {/* Possible Reasons */}
        <Card className="mb-6 border-2 border-[#92400E]/20">
          <CardHeader className="bg-gradient-to-br from-[#92400E]/5 to-[#F59E0B]/5">
            <CardTitle className="text-[#92400E]">Nguyên Nhân Có Thể</CardTitle>
          </CardHeader>
          <CardContent className="pt-6">
            <ul className="space-y-3 text-[#92400E]/80">
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Số dư tài khoản không đủ</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Thông tin thẻ không chính xác</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Thẻ/tài khoản đã hết hạn hoặc bị khóa</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Vượt quá hạn mức giao dịch</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Mất kết nối trong quá trình thanh toán</span>
              </li>
              <li className="flex items-start space-x-2">
                <span className="text-[#DC2626] mt-1">•</span>
                <span>Hủy giao dịch từ cổng thanh toán</span>
              </li>
            </ul>
          </CardContent>
        </Card>

        {/* Action Buttons */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
          <Button
            onClick={handleRetry}
            disabled={isRetrying}
            className="bg-gradient-to-r from-[#92400E] to-[#DC2626] hover:from-[#92400E]/90 hover:to-[#DC2626]/90 text-white"
          >
            {isRetrying ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Đang xử lý...
              </>
            ) : (
              <>
                <RefreshCw className="mr-2 h-4 w-4" />
                Thử Lại Thanh Toán
              </>
            )}
          </Button>
          <Button
            onClick={() => navigate('home')}
            variant="outline"
            className="border-[#92400E] text-[#92400E] hover:bg-[#92400E]/5"
          >
            <Home className="mr-2 h-4 w-4" />
            Về Trang Chủ
          </Button>
        </div>

        {/* Footer */}
        <div className="text-center pt-8 border-t-2 border-[#92400E]/10">
          <Flower2 className="w-12 h-12 text-[#92400E]/20 mx-auto mb-4" />
          <p className="text-[#92400E]/60">
            Chúng tôi luôn sẵn sàng hỗ trợ bạn. Xin lỗi vì sự bất tiện này! 🙏
          </p>
        </div>
      </div>
    </div>
  );
};

export default OrderResultPage;
        
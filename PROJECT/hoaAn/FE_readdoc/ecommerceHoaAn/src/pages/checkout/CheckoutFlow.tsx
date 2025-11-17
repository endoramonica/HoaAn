/**
 * CheckoutFlow - Quản lý luồng checkout với useState
 * Steps: cart -> checkout -> payment -> success/failed
 * 
 * Hỗ trợ cả navigation thông qua URL params:
 * - /checkout?step=success&orderId=xxx
 * - /checkout?step=failed&reason=xxx
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
}

export const CheckoutFlow = () => {
  const [searchParams] = useSearchParams();
  const [currentStep, setCurrentStep] = useState<CheckoutStep>('checkout');
  const [flowData, setFlowData] = useState<CheckoutFlowData>({});

  // Check URL params on mount
  useEffect(() => {
    const stepParam = searchParams.get('step') as CheckoutStep | null;
    const paymentParam = searchParams.get('payment');
    const orderId = searchParams.get('orderId');
    const orderNumber = searchParams.get('orderNumber');
    const reason = searchParams.get('reason');

    if (stepParam && (stepParam === 'success' || stepParam === 'failed')) {
      setCurrentStep(stepParam);
      setFlowData({
        orderId: orderId || undefined,
        orderNumber: orderNumber || undefined,
        errorReason: reason || undefined,
      });
    } else if (paymentParam) {
      // Legacy support for payment callback URLs
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

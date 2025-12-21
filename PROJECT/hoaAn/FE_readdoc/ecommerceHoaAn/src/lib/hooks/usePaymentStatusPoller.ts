/**
 * usePaymentStatusPoller Hook
 * Polls payment status from backend until payment is completed or failed
 * 
 * Requirements: 4.5 - When the frontend polls for payment status, the system SHALL return the current payment status
 */

import { useState, useCallback, useRef, useEffect } from 'react';
import axios from 'axios';
import { tokenStorage } from '../api/client';

// ============================================================================
// Types
// ============================================================================

export type PaymentStatusType = 'pending' | 'paid' | 'failed' | 'cancelled';

export interface PaymentStatusDto {
    orderId: string;
    status: PaymentStatusType;
    transactionId?: string;
    responseCode?: string;
    paidAt?: string;
    errorMessage?: string;
}

export interface UsePaymentStatusPollerReturn {
    status: PaymentStatusType | null;
    isPolling: boolean;
    error: string | null;
    pollPaymentStatus: (orderId: string, maxAttempts?: number) => Promise<PaymentStatusDto | null>;
    cancelPolling: () => void;
}

// ============================================================================
// Helper Functions
// ============================================================================

/**
 * Extract user-friendly error message from API error
 */
const extractErrorMessage = (err: any): string => {
    if (err?.response?.data?.message) return err.response.data.message;
    if (err?.response?.data?.error) return err.response.data.error;
    if (err?.response?.data?.title) return err.response.data.title;
    if (err?.message) return err.message;
    return 'Không thể kiểm tra trạng thái thanh toán';
};

/**
 * Log error details for debugging
 */
const logError = (context: string, err: any) => {
    console.error(`[usePaymentStatusPoller] ❌ ${context}:`, {
        message: err?.message,
        status: err?.response?.status,
        statusText: err?.response?.statusText,
        data: err?.response?.data,
        error: err
    });
};

// ============================================================================
// Hook Implementation
// ============================================================================

/**
 * Hook to poll payment status from backend
 * 
 * @example
 * ```tsx
 * const { pollPaymentStatus, status, isPolling, cancelPolling } = usePaymentStatusPoller();
 * 
 * useEffect(() => {
 *   const startPolling = async () => {
 *     const result = await pollPaymentStatus(orderId, 30);
 *     if (result?.status === 'paid') {
 *       navigate('/order-success');
 *     }
 *   };
 *   startPolling();
 * }, [orderId]);
 * 
 * return (
 *   <div>
 *     {isPolling && <p>Đang kiểm tra trạng thái thanh toán...</p>}
 *     {status === 'paid' && <p>Thanh toán thành công!</p>}
 *     {status === 'failed' && <p>Thanh toán thất bại</p>}
 *     <button onClick={cancelPolling}>Hủy</button>
 *   </div>
 * );
 * ```
 */
export const usePaymentStatusPoller = (): UsePaymentStatusPollerReturn => {
    const [status, setStatus] = useState<PaymentStatusType | null>(null);
    const [isPolling, setIsPolling] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Use ref to track if polling should continue
    const shouldContinuePolling = useRef(true);
    const pollingTimeoutRef = useRef<NodeJS.Timeout | null>(null);

    /**
     * Create axios instance with auth token
     */
    const createAxiosInstance = useCallback(() => {
        const instance = axios.create({
            baseURL: import.meta.env.VITE_API_URL || 'https://localhost:7131',
            timeout: 30000,
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json',
            },
            withCredentials: true,
        });

        // Add auth token to requests
        instance.interceptors.request.use((config) => {
            const token = tokenStorage.getAccessToken();
            if (token && token.trim() && token !== 'null' && token !== 'undefined') {
                config.headers.Authorization = `Bearer ${token}`;
            }
            return config;
        });

        return instance;
    }, []);

    /**
     * Poll payment status repeatedly until completion or max attempts reached
     */
    const pollPaymentStatus = useCallback(async (
        orderId: string,
        maxAttempts: number = 30
    ): Promise<PaymentStatusDto | null> => {
        try {
            setIsPolling(true);
            setError(null);
            shouldContinuePolling.current = true;

            console.log('[usePaymentStatusPoller] 🔄 Starting payment status polling:', {
                orderId,
                maxAttempts
            });

            let attempt = 0;
            let lastStatus: PaymentStatusDto | null = null;
            const axiosInstance = createAxiosInstance();

            while (attempt < maxAttempts && shouldContinuePolling.current) {
                try {
                    attempt++;
                    console.log(`[usePaymentStatusPoller] 📊 Polling attempt ${attempt}/${maxAttempts}`);

                    // Call the payment status endpoint
                    const response = await axiosInstance.get<PaymentStatusDto>(
                        `/api/v1/payment/status/${orderId}`
                    );

                    if (!response.data) {
                        throw new Error('No payment status data returned');
                    }

                    lastStatus = response.data;
                    setStatus(lastStatus.status);

                    console.log('[usePaymentStatusPoller] 📋 Payment status:', {
                        orderId,
                        status: lastStatus.status,
                        transactionId: lastStatus.transactionId,
                        responseCode: lastStatus.responseCode
                    });

                    // Stop polling if payment is completed or failed
                    if (lastStatus.status === 'paid' || lastStatus.status === 'failed' || lastStatus.status === 'cancelled') {
                        console.log('[usePaymentStatusPoller] ✅ Payment status finalized:', lastStatus.status);
                        setIsPolling(false);
                        return lastStatus;
                    }

                    // Wait 2 seconds before next attempt
                    if (attempt < maxAttempts && shouldContinuePolling.current) {
                        await new Promise(resolve => {
                            pollingTimeoutRef.current = setTimeout(resolve, 2000);
                        });
                    }

                } catch (err: any) {
                    logError(`Polling attempt ${attempt} failed`, err);

                    // If it's a 404 (order not found), stop polling
                    if (err?.response?.status === 404) {
                        const errorMsg = 'Không tìm thấy đơn hàng';
                        setError(errorMsg);
                        console.error('[usePaymentStatusPoller] 🔴 Order not found:', orderId);
                        setIsPolling(false);
                        return null;
                    }

                    // For other errors, continue polling (might be temporary network issue)
                    if (attempt < maxAttempts && shouldContinuePolling.current) {
                        console.warn(`[usePaymentStatusPoller] ⚠️ Attempt ${attempt} failed, retrying...`);
                        await new Promise(resolve => {
                            pollingTimeoutRef.current = setTimeout(resolve, 2000);
                        });
                    }
                }
            }

            // Max attempts reached
            if (attempt >= maxAttempts) {
                const timeoutMsg = 'Hết thời gian chờ trạng thái thanh toán. Vui lòng kiểm tra lại sau.';
                setError(timeoutMsg);
                console.warn('[usePaymentStatusPoller] ⏱️ Max polling attempts reached');
            }

            setIsPolling(false);
            return lastStatus;

        } catch (err: any) {
            logError('Payment status polling failed', err);

            const errorMessage = extractErrorMessage(err);
            setError(errorMessage);
            setIsPolling(false);

            return null;
        }
    }, [createAxiosInstance]);

    /**
     * Cancel ongoing polling
     */
    const cancelPolling = useCallback(() => {
        console.log('[usePaymentStatusPoller] 🛑 Cancelling payment status polling');
        shouldContinuePolling.current = false;

        // Clear any pending timeout
        if (pollingTimeoutRef.current) {
            clearTimeout(pollingTimeoutRef.current);
            pollingTimeoutRef.current = null;
        }

        setIsPolling(false);
    }, []);

    /**
     * Cleanup on unmount
     */
    useEffect(() => {
        return () => {
            cancelPolling();
        };
    }, [cancelPolling]);

    return {
        status,
        isPolling,
        error,
        pollPaymentStatus,
        cancelPolling,
    };
};

export default usePaymentStatusPoller;

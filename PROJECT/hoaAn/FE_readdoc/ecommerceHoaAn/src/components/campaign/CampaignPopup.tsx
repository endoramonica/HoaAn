/**
 * Campaign Popup Component
 * Displays campaign information with voucher application
 */

import { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import type { Campaign, Promotion } from '@/lib/api/types';
import {
  applyVoucher,
  removeVoucher,
  getDiscountBadgeText,
  formatPrice,
} from '@/lib/services/campaignService';

interface CampaignPopupProps {
  campaign: Campaign;
  promotions?: Promotion[];
  onClose: () => void;
  onVoucherApplied?: (result: any) => void;
}

export function CampaignPopup({
  campaign,
  promotions = [],
  onClose,
  onVoucherApplied,
}: CampaignPopupProps) {
  const [voucherCode, setVoucherCode] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleApplyVoucher = async () => {
    if (!voucherCode.trim()) {
      setError('Please enter a voucher code');
      return;
    }

    setIsLoading(true);
    setError(null);
    setSuccess(false);

    try {
      const result = await applyVoucher(voucherCode);
      setSuccess(true);
      setVoucherCode('');
      onVoucherApplied?.(result);

      // Auto-close success message after 3 seconds
      setTimeout(() => {
        setSuccess(false);
      }, 3000);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to apply voucher');
    } finally {
      setIsLoading(false);
    }
  };

  const handleRemoveVoucher = async () => {
    setIsLoading(true);
    setError(null);

    try {
      await removeVoucher();
      setSuccess(true);
      setVoucherCode('');
      onVoucherApplied?.(null);

      setTimeout(() => {
        setSuccess(false);
      }, 3000);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to remove voucher');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full animate-in fade-in zoom-in">
        {/* Header */}
        <div className="bg-gradient-to-r from-blue-600 to-blue-700 text-white p-6 rounded-t-lg flex justify-between items-start">
          <div>
            <h2 className="text-2xl font-bold">{campaign.campaignName}</h2>
            <p className="text-blue-100 text-sm mt-1">{campaign.description}</p>
          </div>
          <button
            onClick={onClose}
            className="text-white hover:bg-blue-800 p-1 rounded transition"
          >
            <X size={24} />
          </button>
        </div>

        {/* Content */}
        <div className="p-6 space-y-4">
          {/* Promotions */}
          {promotions.length > 0 && (
            <div className="space-y-3">
              <h3 className="font-semibold text-gray-800">Available Promotions:</h3>
              {promotions.map((promo) => (
                <div
                  key={promo.promotionId}
                  className="bg-gradient-to-r from-orange-50 to-red-50 border border-orange-200 rounded-lg p-4"
                >
                  <div className="flex justify-between items-start">
                    <div>
                      <p className="text-lg font-bold text-orange-600">
                        {getDiscountBadgeText(promo)}
                      </p>
                      <p className="text-sm text-gray-600 mt-1">
                        {promo.description}
                      </p>
                      {promo.minOrderValue && (
                        <p className="text-xs text-gray-500 mt-2">
                          Min order: {formatPrice(promo.minOrderValue)}
                        </p>
                      )}
                    </div>
                    {promo.maxUsage && (
                      <div className="text-right">
                        <p className="text-xs text-gray-500">
                          {promo.currentUsage}/{promo.maxUsage} used
                        </p>
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}

          {/* Voucher Input */}
          <div className="space-y-2">
            <label className="block text-sm font-medium text-gray-700">
              Voucher Code
            </label>
            <div className="flex gap-2">
              <input
                type="text"
                value={voucherCode}
                onChange={(e) => setVoucherCode(e.target.value.toUpperCase())}
                placeholder="Enter voucher code"
                disabled={isLoading}
                className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:bg-gray-100"
              />
              <button
                onClick={handleApplyVoucher}
                disabled={isLoading || !voucherCode.trim()}
                className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-400 transition font-medium"
              >
                {isLoading ? 'Applying...' : 'Apply'}
              </button>
            </div>
          </div>

          {/* Error Message */}
          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
              {error}
            </div>
          )}

          {/* Success Message */}
          {success && (
            <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-lg text-sm">
              ✓ Voucher applied successfully!
            </div>
          )}
        </div>

        {/* Footer */}
        <div className="bg-gray-50 px-6 py-4 rounded-b-lg flex gap-3">
          <button
            onClick={onClose}
            className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-100 transition font-medium"
          >
            Close
          </button>
          {voucherCode && (
            <button
              onClick={handleRemoveVoucher}
              disabled={isLoading}
              className="flex-1 px-4 py-2 bg-red-100 text-red-700 rounded-lg hover:bg-red-200 disabled:bg-gray-100 transition font-medium"
            >
              Remove
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

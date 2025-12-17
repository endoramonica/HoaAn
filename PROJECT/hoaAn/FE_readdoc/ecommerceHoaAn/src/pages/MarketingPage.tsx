/**
 * Marketing Page
 * Showcases campaigns and marketing posts
 */

import { useState, useEffect } from 'react';
import { CampaignPopup } from '@/components/campaign/CampaignPopup';
import { PostsFeed } from '@/components/marketing/PostsFeed';
import { useCampaigns } from '@/lib/hooks/useCampaigns';\nimport type { Campaign } from '@/lib/api/types';

export function MarketingPage() {
  const [selectedCampaign, setSelectedCampaign] = useState<Campaign | null>(null);
  const [showCampaignPopup, setShowCampaignPopup] = useState(false);

  const { campaigns, isLoading, error, getApplicableCampaigns, trackImpression } =
    useCampaigns({
      currentPage: 'marketing',
      autoTrack: true,
    });

  // Show campaign popup on mount
  useEffect(() => {
    if (!isLoading && campaigns.length > 0) {
      const applicable = getApplicableCampaigns();
      if (applicable.length > 0) {
        const campaign = applicable[0];
        setSelectedCampaign(campaign);
        setShowCampaignPopup(true);
        trackImpression(campaign.campaignId);
      }
    }
  }, [isLoading, campaigns, getApplicableCampaigns, trackImpression]);

  const handleProductClick = (productId: string) => {
    // Navigate to product detail page
    window.location.href = `/products/${productId}`;
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-gradient-to-r from-blue-600 to-indigo-600 text-white py-12">
        <div className="max-w-7xl mx-auto px-4">
          <h1 className="text-4xl font-bold mb-4">Marketing & Promotions</h1>
          <p className="text-blue-100 text-lg">
            Discover our latest campaigns and featured products
          </p>
        </div>
      </div>

      {/* Main Content */}
      <div className="max-w-7xl mx-auto px-4 py-12">
        {/* Campaign Error */}
        {error && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-4 mb-8">
            <p className="text-red-700">{error}</p>
          </div>
        )}

        {/* Active Campaigns Section */}
        {campaigns.length > 0 && (
          <div className="mb-12">
            <h2 className="text-2xl font-bold text-gray-800 mb-6">
              Active Campaigns ({campaigns.length})
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {campaigns.map((campaign) => (
                <div
                  key={campaign.campaignId}
                  className="bg-white rounded-lg shadow-md p-6 hover:shadow-lg transition cursor-pointer"
                  onClick={() => {
                    setSelectedCampaign(campaign);
                    setShowCampaignPopup(true);
                    trackImpression(campaign.campaignId);
                  }}
                >
                  <h3 className="font-bold text-lg text-gray-800 mb-2">
                    {campaign.campaignName}
                  </h3>
                  <p className="text-gray-600 text-sm mb-4">
                    {campaign.description}
                  </p>
                  <div className="flex justify-between items-center text-xs text-gray-500">
                    <span>Budget: {campaign.budget.toLocaleString()} VND</span>
                    <span className="bg-blue-100 text-blue-700 px-2 py-1 rounded">
                      {campaign.status}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Marketing Posts Section */}
        <div>
          <h2 className="text-2xl font-bold text-gray-800 mb-6">
            Featured Posts
          </h2>
          <PostsFeed
            query={{
              status: 'Published',
              isFeatured: true,
              pageSize: 12,
            }}
            onProductClick={handleProductClick}
          />
        </div>
      </div>

      {/* Campaign Popup */}
      {showCampaignPopup && selectedCampaign && (
        <CampaignPopup
          campaign={selectedCampaign}
          onClose={() => setShowCampaignPopup(false)}
          onVoucherApplied={(result) => {
            if (result) {
              alert(`Voucher applied! Discount: ${result.discountAmount} VND`);
            }
          }}
        />
      )}
    </div>
  );
}

/**
 * Hook for managing campaigns
 */

import { useState, useEffect, useCallback } from 'react';
import {
    getActiveCampaigns,
    shouldShowCampaign,
    getSessionId,
    trackCampaignImpression,
    trackCampaignClick,
} from '@/lib/services/campaignService';
import type { Campaign, TrackingEvent } from '@/lib/api/types';

interface UseCampaignsOptions {
    currentPage?: string;
    autoTrack?: boolean;
}

export function useCampaigns(options: UseCampaignsOptions = {}) {
    const { currentPage = 'home', autoTrack = true } = options;

    const [campaigns, setCampaigns] = useState<Campaign[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [shownCampaigns, setShownCampaigns] = useState<Set<string>>(new Set());

    // Load campaigns
    useEffect(() => {
        loadCampaigns();
    }, []);

    const loadCampaigns = async () => {
        setIsLoading(true);
        setError(null);

        try {
            const data = await getActiveCampaigns();
            setCampaigns(data);
        } catch (err: any) {
            setError(err.message || 'Failed to load campaigns');
            console.error('Error loading campaigns:', err);
        } finally {
            setIsLoading(false);
        }
    };

    // Get applicable campaigns for current page
    const getApplicableCampaigns = useCallback(() => {
        return campaigns.filter((campaign) => {
            const sessionKey = `campaign_${campaign.campaignId}_shown`;
            return shouldShowCampaign(campaign, currentPage, sessionKey);
        });
    }, [campaigns, currentPage]);

    // Track impression
    const trackImpression = useCallback(
        async (campaignId: string) => {
            if (!autoTrack) return;

            try {
                const event: TrackingEvent = {
                    sessionId: getSessionId(),
                    page: currentPage,
                    timestamp: new Date().toISOString(),
                };

                await trackCampaignImpression(campaignId, event);

                // Mark as shown
                const sessionKey = `campaign_${campaignId}_shown`;
                sessionStorage.setItem(sessionKey, 'true');
                setShownCampaigns((prev) => new Set([...prev, campaignId]));
            } catch (err) {
                console.error('Error tracking impression:', err);
            }
        },
        [autoTrack, currentPage]
    );

    // Track click
    const trackClick = useCallback(
        async (campaignId: string) => {
            if (!autoTrack) return;

            try {
                const event: TrackingEvent = {
                    sessionId: getSessionId(),
                    page: currentPage,
                    timestamp: new Date().toISOString(),
                };

                await trackCampaignClick(campaignId, event);
            } catch (err) {
                console.error('Error tracking click:', err);
            }
        },
        [autoTrack, currentPage]
    );

    return {
        campaigns,
        isLoading,
        error,
        shownCampaigns,
        getApplicableCampaigns,
        trackImpression,
        trackClick,
        reload: loadCampaigns,
    };
}

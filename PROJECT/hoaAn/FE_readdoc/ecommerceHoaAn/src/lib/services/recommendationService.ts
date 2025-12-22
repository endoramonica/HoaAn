/**
 * Recommendation Service - Gọi BE-AI và FE-AI để lấy recommendations
 * Handles caching, retries, and error handling
 */

import { Action } from './actionTrackingService';

// ============================================================================
// Types
// ============================================================================

export interface Product {
  id: string;
  name: string;
  price: number;
  image?: string;
  categoryId?: string;
  description?: string;
}

export interface MatchingMetadata {
  matchedSequenceLength: number;
  totalSequenceLength: number;
  matchedActionIndices: number[];
}

export interface SystemReport {
  matchedPattern: any[];
  matchingSteps: string[];
  reasonsForMissingItems: Record<string, string>;
}

export interface RecommendationPayload {
  matched: boolean;
  ritualId?: string;
  ritualName?: string;
  confidenceScore?: number;
  missingItems?: Product[];
  matchingMetadata?: MatchingMetadata;
  systemReport?: SystemReport;
}

export interface ItemExplanation {
  productId: string;
  productName: string;
  whyNeeded: string;
  traditionalUsage: string;
}

export interface ExplanationPayload {
  ritualName: string;
  culturalContext: string;
  itemExplanations: ItemExplanation[];
  sources: string[];
  generatedBy: 'gemini' | 'fallback';
}

export interface RecommendationCache {
  payload: RecommendationPayload;
  explanation: ExplanationPayload | null;
  timestamp: number;
}

// ============================================================================
// Configuration
// ============================================================================

const CONFIG = {
  API_BASE_URL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
  CACHE_TTL: 15 * 60 * 1000, // 15 minutes
  API_TIMEOUT: 5000, // 5 seconds
  MAX_RETRIES: 3,
  RETRY_DELAY: 1000, // 1 second
};

// ============================================================================
// Recommendation Service
// ============================================================================

class RecommendationService {
  private cache: RecommendationCache | null = null;
  private isLoading: boolean = false;
  private listeners: Set<() => void> = new Set();

  /**
   * Call BE-AI to analyze action sequence
   */
  async callBeAiAnalysis(
    actionSequence: Action[],
    userId?: string,
    sessionId?: string
  ): Promise<RecommendationPayload> {
    if (!actionSequence || actionSequence.length === 0) {
      return { matched: false };
    }

    try {
      this.isLoading = true;
      this.notifyListeners();

      const response = await this.fetchWithRetry(
        `${CONFIG.API_BASE_URL}/api/recommendations/analyze`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            userId,
            sessionId,
            actionSequence,
          }),
        }
      );

      const payload: RecommendationPayload = await response.json();
      return payload;
    } catch (error) {
      console.error('Error calling BE-AI analysis:', error);
      return { matched: false };
    } finally {
      this.isLoading = false;
      this.notifyListeners();
    }
  }

  /**
   * Call FE-AI to generate explanation
   */
  async callFeAiExplanation(
    payload: RecommendationPayload
  ): Promise<ExplanationPayload | null> {
    if (!payload.matched || !payload.ritualId) {
      return null;
    }

    try {
      const response = await this.fetchWithRetry(
        `${CONFIG.API_BASE_URL}/api/explanations/generate`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(payload),
        }
      );

      const explanation: ExplanationPayload = await response.json();
      return explanation;
    } catch (error) {
      console.error('Error calling FE-AI explanation:', error);
      // Return fallback explanation
      return this.getFallbackExplanation(payload);
    }
  }

  /**
   * Get fallback explanation when Gemini API fails
   */
  private getFallbackExplanation(payload: RecommendationPayload): ExplanationPayload {
    return {
      ritualName: payload.ritualName || 'Nghi thức Việt Nam',
      culturalContext: `Đây là một nghi thức truyền thống Việt Nam quan trọng. Các sản phẩm được đề xuất là những vật phẩm cần thiết theo phong tục truyền thống.`,
      itemExplanations: (payload.missingItems || []).map((item) => ({
        productId: item.id,
        productName: item.name,
        whyNeeded: payload.systemReport?.reasonsForMissingItems?.[item.id] || 'Vật phẩm truyền thống cần thiết',
        traditionalUsage: 'Được sử dụng trong nghi thức truyền thống Việt Nam',
      })),
      sources: ['Phong tục Việt cổ truyền'],
      generatedBy: 'fallback',
    };
  }

  /**
   * Generate recommendation (BE-AI + FE-AI)
   */
  async generateRecommendation(
    actionSequence: Action[],
    userId?: string,
    sessionId?: string
  ): Promise<{ payload: RecommendationPayload; explanation: ExplanationPayload | null }> {
    // Check cache first
    if (this.cache && Date.now() - this.cache.timestamp < CONFIG.CACHE_TTL) {
      return {
        payload: this.cache.payload,
        explanation: this.cache.explanation,
      };
    }

    // Call BE-AI
    const payload = await this.callBeAiAnalysis(actionSequence, userId, sessionId);

    // Call FE-AI if matched
    let explanation: ExplanationPayload | null = null;
    if (payload.matched) {
      explanation = await this.callFeAiExplanation(payload);
    }

    // Cache result
    this.cache = {
      payload,
      explanation,
      timestamp: Date.now(),
    };

    this.notifyListeners();

    return { payload, explanation };
  }

  /**
   * Get cached recommendation
   */
  getCachedRecommendation(): RecommendationCache | null {
    if (this.cache && Date.now() - this.cache.timestamp < CONFIG.CACHE_TTL) {
      return this.cache;
    }
    return null;
  }

  /**
   * Clear cached recommendation
   */
  clearCachedRecommendation(): void {
    this.cache = null;
    this.notifyListeners();
  }

  /**
   * Check if currently loading
   */
  isLoadingRecommendation(): boolean {
    return this.isLoading;
  }

  /**
   * Fetch with retry logic
   */
  private async fetchWithRetry(
    url: string,
    options: RequestInit,
    attempt: number = 1
  ): Promise<Response> {
    try {
      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), CONFIG.API_TIMEOUT);

      const response = await fetch(url, {
        ...options,
        signal: controller.signal,
      });

      clearTimeout(timeoutId);

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }

      return response;
    } catch (error) {
      if (attempt < CONFIG.MAX_RETRIES) {
        // Exponential backoff
        const delay = CONFIG.RETRY_DELAY * Math.pow(2, attempt - 1);
        await new Promise((resolve) => setTimeout(resolve, delay));
        return this.fetchWithRetry(url, options, attempt + 1);
      }
      throw error;
    }
  }

  /**
   * Subscribe to recommendation changes
   */
  subscribe(callback: () => void): () => void {
    this.listeners.add(callback);
    return () => {
      this.listeners.delete(callback);
    };
  }

  /**
   * Notify all listeners
   */
  private notifyListeners(): void {
    this.listeners.forEach((callback) => {
      try {
        callback();
      } catch (error) {
        console.error('Error in recommendation listener:', error);
      }
    });
  }

  /**
   * Get debug info
   */
  getDebugInfo(): Record<string, any> {
    return {
      isLoading: this.isLoading,
      cache: this.cache,
      cacheAge: this.cache ? Date.now() - this.cache.timestamp : null,
      cacheTTL: CONFIG.CACHE_TTL,
    };
  }
}

// ============================================================================
// Singleton Instance
// ============================================================================

let instance: RecommendationService | null = null;

export function getRecommendationService(): RecommendationService {
  if (!instance) {
    instance = new RecommendationService();
  }
  return instance;
}

// Export for testing
export { RecommendationService };

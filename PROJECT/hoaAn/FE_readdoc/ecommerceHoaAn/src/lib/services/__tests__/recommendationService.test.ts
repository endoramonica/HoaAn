import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import {
  RecommendationService,
  getRecommendationService,
  RecommendationPayload,
  ExplanationPayload,
} from '../recommendationService';
import { Action } from '../actionTrackingService';

describe('RecommendationService', () => {
  let service: RecommendationService;

  beforeEach(() => {
    service = new RecommendationService();
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  describe('BE-AI Analysis', () => {
    it('should call BE-AI with action sequence', async () => {
      const mockResponse: RecommendationPayload = {
        matched: true,
        ritualId: 'ritual-1',
        ritualName: 'Tết Nguyên Đán',
        confidenceScore: 0.95,
        missingItems: [
          { id: 'product-1', name: 'Hoa', price: 50000 },
          { id: 'product-2', name: 'Nến', price: 30000 },
        ],
      };

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => mockResponse,
      });

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
          productId: 'product-1',
          categoryId: 'category-1',
        },
      ];

      const result = await service.callBeAiAnalysis(actions, 'user-1', 'session-1');

      expect(result.matched).toBe(true);
      expect(result.ritualId).toBe('ritual-1');
      expect(result.missingItems?.length).toBe(2);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining('/api/recommendations/analyze'),
        expect.objectContaining({
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
        })
      );
    });

    it('should return unmatched when no pattern found', async () => {
      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => ({ matched: false }),
      });

      const actions: Action[] = [];
      const result = await service.callBeAiAnalysis(actions);

      expect(result.matched).toBe(false);
    });

    it('should handle empty action sequence', async () => {
      const result = await service.callBeAiAnalysis([]);
      expect(result.matched).toBe(false);
    });

    it('should handle API errors gracefully', async () => {
      global.fetch = vi.fn().mockRejectedValueOnce(new Error('Network error'));

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
        },
      ];

      const result = await service.callBeAiAnalysis(actions);
      expect(result.matched).toBe(false);
    });

    it('should retry on failure', async () => {
      global.fetch = vi
        .fn()
        .mockRejectedValueOnce(new Error('Network error'))
        .mockResolvedValueOnce({
          ok: true,
          json: async () => ({ matched: true, ritualId: 'ritual-1' }),
        });

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
        },
      ];

      const result = await service.callBeAiAnalysis(actions);
      expect(result.matched).toBe(true);
      expect(global.fetch).toHaveBeenCalledTimes(2);
    });
  });

  describe('FE-AI Explanation', () => {
    it('should call FE-AI with recommendation payload', async () => {
      const mockExplanation: ExplanationPayload = {
        ritualName: 'Tết Nguyên Đán',
        culturalContext: 'Lễ hội truyền thống Việt Nam',
        itemExplanations: [
          {
            productId: 'product-1',
            productName: 'Hoa',
            whyNeeded: 'Trang trí bàn thờ',
            traditionalUsage: 'Hoa tươi để cúng',
          },
        ],
        sources: ['Phong tục Việt cổ truyền'],
        generatedBy: 'gemini',
      };

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => mockExplanation,
      });

      const payload: RecommendationPayload = {
        matched: true,
        ritualId: 'ritual-1',
        ritualName: 'Tết Nguyên Đán',
        missingItems: [{ id: 'product-1', name: 'Hoa', price: 50000 }],
      };

      const result = await service.callFeAiExplanation(payload);

      expect(result?.ritualName).toBe('Tết Nguyên Đán');
      expect(result?.itemExplanations.length).toBe(1);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining('/api/explanations/generate'),
        expect.any(Object)
      );
    });

    it('should return null for unmatched payload', async () => {
      const payload: RecommendationPayload = { matched: false };
      const result = await service.callFeAiExplanation(payload);

      expect(result).toBeNull();
    });

    it('should return fallback explanation on API error', async () => {
      global.fetch = vi.fn().mockRejectedValueOnce(new Error('API error'));

      const payload: RecommendationPayload = {
        matched: true,
        ritualId: 'ritual-1',
        ritualName: 'Tết Nguyên Đán',
        missingItems: [{ id: 'product-1', name: 'Hoa', price: 50000 }],
      };

      const result = await service.callFeAiExplanation(payload);

      expect(result).toBeDefined();
      expect(result?.generatedBy).toBe('fallback');
      expect(result?.ritualName).toBe('Tết Nguyên Đán');
    });
  });

  describe('Full Recommendation Generation', () => {
    it('should generate complete recommendation', async () => {
      const mockPayload: RecommendationPayload = {
        matched: true,
        ritualId: 'ritual-1',
        ritualName: 'Tết Nguyên Đán',
        missingItems: [{ id: 'product-1', name: 'Hoa', price: 50000 }],
      };

      const mockExplanation: ExplanationPayload = {
        ritualName: 'Tết Nguyên Đán',
        culturalContext: 'Lễ hội truyền thống',
        itemExplanations: [],
        sources: [],
        generatedBy: 'gemini',
      };

      global.fetch = vi
        .fn()
        .mockResolvedValueOnce({
          ok: true,
          json: async () => mockPayload,
        })
        .mockResolvedValueOnce({
          ok: true,
          json: async () => mockExplanation,
        });

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
        },
      ];

      const result = await service.generateRecommendation(actions, 'user-1', 'session-1');

      expect(result.payload.matched).toBe(true);
      expect(result.explanation?.ritualName).toBe('Tết Nguyên Đán');
    });

    it('should cache recommendations', async () => {
      const mockPayload: RecommendationPayload = {
        matched: true,
        ritualId: 'ritual-1',
        ritualName: 'Tết Nguyên Đán',
      };

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => mockPayload,
      });

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
        },
      ];

      // First call
      const result1 = await service.generateRecommendation(actions);

      // Second call should use cache
      const result2 = await service.generateRecommendation(actions);

      expect(result1.payload).toEqual(result2.payload);
      expect(global.fetch).toHaveBeenCalledTimes(1); // Only called once due to cache
    });

    it('should invalidate cache after TTL', async () => {
      const mockPayload: RecommendationPayload = {
        matched: true,
        ritualId: 'ritual-1',
      };

      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        json: async () => mockPayload,
      });

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
        },
      ];

      // First call
      await service.generateRecommendation(actions);

      // Clear cache manually (simulating TTL expiration)
      service.clearCachedRecommendation();

      // Second call should not use cache
      await service.generateRecommendation(actions);

      expect(global.fetch).toHaveBeenCalledTimes(2);
    });
  });

  describe('Caching', () => {
    it('should get cached recommendation', async () => {
      const mockPayload: RecommendationPayload = {
        matched: true,
        ritualId: 'ritual-1',
      };

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => mockPayload,
      });

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
        },
      ];

      await service.generateRecommendation(actions);

      const cached = service.getCachedRecommendation();
      expect(cached).toBeDefined();
      expect(cached?.payload.matched).toBe(true);
    });

    it('should clear cached recommendation', async () => {
      const mockPayload: RecommendationPayload = {
        matched: true,
        ritualId: 'ritual-1',
      };

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => mockPayload,
      });

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
        },
      ];

      await service.generateRecommendation(actions);
      service.clearCachedRecommendation();

      const cached = service.getCachedRecommendation();
      expect(cached).toBeNull();
    });
  });

  describe('Loading State', () => {
    it('should track loading state', async () => {
      global.fetch = vi.fn().mockImplementationOnce(
        () =>
          new Promise((resolve) =>
            setTimeout(
              () =>
                resolve({
                  ok: true,
                  json: async () => ({ matched: true }),
                }),
              100
            )
          )
      );

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
        },
      ];

      const promise = service.generateRecommendation(actions);
      expect(service.isLoadingRecommendation()).toBe(true);

      await promise;
      expect(service.isLoadingRecommendation()).toBe(false);
    });
  });

  describe('Subscriptions', () => {
    it('should notify listeners on recommendation change', async () => {
      const callback = vi.fn();
      service.subscribe(callback);

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => ({ matched: true }),
      });

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
        },
      ];

      await service.generateRecommendation(actions);

      expect(callback).toHaveBeenCalled();
    });

    it('should allow unsubscribing', async () => {
      const callback = vi.fn();
      const unsubscribe = service.subscribe(callback);

      unsubscribe();

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => ({ matched: true }),
      });

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
        },
      ];

      await service.generateRecommendation(actions);

      expect(callback).not.toHaveBeenCalled();
    });
  });

  describe('Singleton Pattern', () => {
    it('should return same instance from getter', () => {
      const instance1 = getRecommendationService();
      const instance2 = getRecommendationService();

      expect(instance1).toBe(instance2);
    });
  });

  describe('Debug Info', () => {
    it('should provide debug information', async () => {
      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => ({ matched: true }),
      });

      const actions: Action[] = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct',
          timestamp: Date.now(),
        },
      ];

      await service.generateRecommendation(actions);

      const debug = service.getDebugInfo();
      expect(debug.isLoading).toBe(false);
      expect(debug.cache).toBeDefined();
      expect(debug.cacheAge).toBeGreaterThanOrEqual(0);
    });
  });
});

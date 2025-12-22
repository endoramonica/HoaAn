import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import { getActionTrackingService } from '../lib/services/actionTrackingService';
import { getRecommendationService } from '../lib/services/recommendationService';
import { getUserPreferenceService } from '../lib/services/userPreferenceService';

describe('Integration Tests - Sequential Ritual Recommendation System', () => {
  let actionTracking: ReturnType<typeof getActionTrackingService>;
  let recommendationService: ReturnType<typeof getRecommendationService>;
  let userPreferenceService: ReturnType<typeof getUserPreferenceService>;

  beforeEach(() => {
    sessionStorage.clear();
    vi.clearAllMocks();

    actionTracking = getActionTrackingService();
    recommendationService = getRecommendationService();
    userPreferenceService = getUserPreferenceService();
  });

  afterEach(() => {
    sessionStorage.clear();
    vi.clearAllMocks();
  });

  describe('Action Tracking Flow', () => {
    it('should track complete user journey', (done) => {
      const callback = vi.fn();
      actionTracking.subscribe(callback);

      // Simulate user journey
      actionTracking.trackAction('BrowseCategory', {}, undefined, 'category-1');
      actionTracking.trackAction('ViewProduct', {}, 'product-1', 'category-1');
      actionTracking.trackAction('ViewProduct', {}, 'product-2', 'category-1');
      actionTracking.trackAction('AddToCart', {}, 'product-1', 'category-1');

      setTimeout(() => {
        const sequence = actionTracking.getActionSequenceObject();

        expect(sequence.actions.length).toBe(4);
        expect(sequence.sessionId).toBeDefined();
        expect(sequence.startTime).toBeDefined();
        expect(sequence.lastActionTime).toBeDefined();
        expect(callback).toHaveBeenCalled();
        done();
      }, 600);
    });

    it('should persist session across service instances', (done) => {
      actionTracking.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      setTimeout(() => {
        const sessionId1 = actionTracking.getSessionId();

        // Create new instance
        const newActionTracking = getActionTrackingService();
        const sessionId2 = newActionTracking.getSessionId();

        expect(sessionId1).toBe(sessionId2);
        done();
      }, 600);
    });

    it('should track user ID across actions', (done) => {
      actionTracking.setUserId('user-123');
      actionTracking.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      setTimeout(() => {
        const sequence = actionTracking.getActionSequenceObject();
        expect(sequence.userId).toBe('user-123');
        expect(sequence.actions[0].userId).toBe('user-123');
        done();
      }, 600);
    });
  });

  describe('Recommendation Generation Flow', () => {
    it('should generate recommendation from action sequence', async () => {
      const mockPayload = {
        matched: true,
        ritualId: 'ritual-1',
        ritualName: 'Tết Nguyên Đán',
        missingItems: [
          { id: 'product-1', name: 'Hoa', price: 50000 },
        ],
      };

      const mockExplanation = {
        ritualName: 'Tết Nguyên Đán',
        culturalContext: 'Lễ hội truyền thống',
        itemExplanations: [],
        sources: [],
        generatedBy: 'gemini' as const,
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

      const actions = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct' as const,
          timestamp: Date.now(),
          productId: 'product-1',
          categoryId: 'category-1',
        },
      ];

      const result = await recommendationService.generateRecommendation(actions);

      expect(result.payload.matched).toBe(true);
      expect(result.payload.ritualName).toBe('Tết Nguyên Đán');
      expect(result.explanation?.ritualName).toBe('Tết Nguyên Đán');
    });

    it('should cache recommendations for same action sequence', async () => {
      const mockPayload = {
        matched: true,
        ritualId: 'ritual-1',
        ritualName: 'Tết Nguyên Đán',
      };

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => mockPayload,
      });

      const actions = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct' as const,
          timestamp: Date.now(),
        },
      ];

      // First call
      const result1 = await recommendationService.generateRecommendation(actions);

      // Second call should use cache
      const result2 = await recommendationService.generateRecommendation(actions);

      expect(result1.payload).toEqual(result2.payload);
      expect(global.fetch).toHaveBeenCalledTimes(1);
    });
  });

  describe('User Preference Flow', () => {
    it('should record dismissal and prevent re-recommendation', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      await userPreferenceService.recordDismissal('user-1', 'ritual-1');

      expect(userPreferenceService.isRitualDismissed('ritual-1')).toBe(true);
      expect(userPreferenceService.getDismissalCount('ritual-1')).toBe(1);
    });

    it('should disable ritual for session', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      await userPreferenceService.disableRitual('user-1', 'session-1', 'ritual-1');

      expect(userPreferenceService.isRitualDisabled('ritual-1')).toBe(true);
    });

    it('should track multiple dismissals', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      await userPreferenceService.recordDismissal('user-1', 'ritual-1');
      await userPreferenceService.recordDismissal('user-1', 'ritual-1');
      await userPreferenceService.recordDismissal('user-1', 'ritual-1');

      expect(userPreferenceService.getDismissalCount('ritual-1')).toBe(3);
    });
  });

  describe('End-to-End Flow', () => {
    it('should complete full user journey: track → recommend → dismiss', async () => {
      // Setup mocks
      const mockPayload = {
        matched: true,
        ritualId: 'ritual-1',
        ritualName: 'Tết Nguyên Đán',
        missingItems: [
          { id: 'product-1', name: 'Hoa', price: 50000 },
        ],
      };

      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
        json: async () => mockPayload,
      });

      // Step 1: User browses and adds to cart
      actionTracking.setUserId('user-123');
      actionTracking.trackAction('BrowseCategory', {}, undefined, 'category-1');
      actionTracking.trackAction('ViewProduct', {}, 'product-1', 'category-1');
      actionTracking.trackAction('AddToCart', {}, 'product-1', 'category-1');

      // Wait for debounce
      await new Promise((resolve) => setTimeout(resolve, 600));

      // Step 2: Get action sequence
      const sequence = actionTracking.getActionSequenceObject();
      expect(sequence.actions.length).toBe(3);
      expect(sequence.userId).toBe('user-123');

      // Step 3: Generate recommendation
      const recommendation = await recommendationService.generateRecommendation(
        sequence.actions,
        sequence.userId,
        sequence.sessionId
      );

      expect(recommendation.payload.matched).toBe(true);
      expect(recommendation.payload.ritualName).toBe('Tết Nguyên Đán');

      // Step 4: User dismisses recommendation
      await userPreferenceService.recordDismissal('user-123', 'ritual-1');

      expect(userPreferenceService.isRitualDismissed('ritual-1')).toBe(true);
      expect(userPreferenceService.getDismissalCount('ritual-1')).toBe(1);
    });

    it('should handle complete user session lifecycle', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      // Session start
      const sessionId = actionTracking.getSessionId();
      expect(sessionId).toBeDefined();

      // User login
      actionTracking.setUserId('user-123');
      expect(actionTracking.getUserId()).toBe('user-123');

      // User actions
      actionTracking.trackAction('ViewProduct', {}, 'product-1', 'category-1');
      actionTracking.trackAction('AddToCart', {}, 'product-1', 'category-1');

      await new Promise((resolve) => setTimeout(resolve, 600));

      // User preferences
      await userPreferenceService.recordDismissal('user-123', 'ritual-1');
      await userPreferenceService.disableRitual('user-123', sessionId, 'ritual-2');

      expect(userPreferenceService.getDismissedRituals().length).toBe(1);
      expect(userPreferenceService.getDisabledRituals().length).toBe(1);

      // Session end (logout)
      actionTracking.clearSession();
      userPreferenceService.clearPreferences();

      expect(actionTracking.getActionCount()).toBe(0);
      expect(actionTracking.getUserId()).toBeUndefined();
      expect(userPreferenceService.getDismissedRituals().length).toBe(0);
      expect(userPreferenceService.getDisabledRituals().length).toBe(0);
    });

    it('should handle multiple users in same session', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      const sessionId = actionTracking.getSessionId();

      // User 1
      actionTracking.setUserId('user-1');
      actionTracking.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      await new Promise((resolve) => setTimeout(resolve, 600));

      expect(actionTracking.getUserId()).toBe('user-1');

      // User 2 (logout and login)
      actionTracking.clearSession();
      actionTracking.setUserId('user-2');
      actionTracking.trackAction('ViewProduct', {}, 'product-2', 'category-2');

      await new Promise((resolve) => setTimeout(resolve, 600));

      expect(actionTracking.getUserId()).toBe('user-2');
      expect(actionTracking.getSessionId()).not.toBe(sessionId);
    });
  });

  describe('Error Handling and Recovery', () => {
    it('should recover from API failures', async () => {
      global.fetch = vi
        .fn()
        .mockRejectedValueOnce(new Error('Network error'))
        .mockResolvedValueOnce({
          ok: true,
          json: async () => ({ matched: true, ritualId: 'ritual-1' }),
        });

      const actions = [
        {
          id: 'action-1',
          sessionId: 'session-1',
          type: 'ViewProduct' as const,
          timestamp: Date.now(),
        },
      ];

      const result = await recommendationService.generateRecommendation(actions);

      expect(result.payload.matched).toBe(true);
      expect(global.fetch).toHaveBeenCalledTimes(2);
    });

    it('should handle corrupted storage gracefully', () => {
      sessionStorage.setItem('ritual_actions', 'invalid json');
      sessionStorage.setItem('ritual_dismissed_rituals', 'invalid json');

      const newActionTracking = getActionTrackingService();
      const newUserPreferenceService = getUserPreferenceService();

      expect(newActionTracking.getActionCount()).toBe(0);
      expect(newUserPreferenceService.getDismissedRituals().length).toBe(0);
    });

    it('should continue functioning after preference clear', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      // Add some preferences
      await userPreferenceService.recordDismissal('user-1', 'ritual-1');
      await userPreferenceService.disableRitual('user-1', 'session-1', 'ritual-2');

      expect(userPreferenceService.getDismissedRituals().length).toBe(1);

      // Clear preferences
      userPreferenceService.clearPreferences();

      expect(userPreferenceService.getDismissedRituals().length).toBe(0);

      // Should still work
      await userPreferenceService.recordDismissal('user-1', 'ritual-3');
      expect(userPreferenceService.getDismissedRituals().length).toBe(1);
    });
  });

  describe('Performance and Limits', () => {
    it('should handle large action sequences', (done) => {
      // Track many actions
      for (let i = 0; i < 100; i++) {
        actionTracking.trackAction('ViewProduct', {}, `product-${i}`, 'category-1');
      }

      setTimeout(() => {
        const actions = actionTracking.getActionSequence();
        // Should be limited to max actions per session
        expect(actions.length).toBeLessThanOrEqual(50);
        done();
      }, 600);
    });

    it('should handle rapid preference updates', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      // Rapid updates
      const promises = [];
      for (let i = 0; i < 10; i++) {
        promises.push(
          userPreferenceService.recordDismissal('user-1', `ritual-${i}`)
        );
      }

      await Promise.all(promises);

      expect(userPreferenceService.getDismissedRituals().length).toBe(10);
    });
  });

  describe('Subscription and Notification', () => {
    it('should notify all services on changes', async () => {
      const actionCallback = vi.fn();
      const prefCallback = vi.fn();

      actionTracking.subscribe(actionCallback);
      userPreferenceService.subscribe(prefCallback);

      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      // Trigger action
      actionTracking.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      await new Promise((resolve) => setTimeout(resolve, 600));

      expect(actionCallback).toHaveBeenCalled();

      // Trigger preference
      await userPreferenceService.recordDismissal('user-1', 'ritual-1');

      expect(prefCallback).toHaveBeenCalled();
    });
  });
});

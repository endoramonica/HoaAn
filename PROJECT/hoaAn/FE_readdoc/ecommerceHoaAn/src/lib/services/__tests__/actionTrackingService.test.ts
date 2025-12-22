import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import {
  ActionTrackingService,
  getActionTrackingService,
  Action,
} from '../actionTrackingService';

describe('ActionTrackingService', () => {
  let service: ActionTrackingService;

  beforeEach(() => {
    // Create fresh instance for each test
    service = new ActionTrackingService();
    sessionStorage.clear();
  });

  afterEach(() => {
    sessionStorage.clear();
  });

  describe('Session Management', () => {
    it('should create a session ID on initialization', () => {
      const sessionId = service.getSessionId();
      expect(sessionId).toBeDefined();
      expect(sessionId.length).toBeGreaterThan(0);
    });

    it('should persist session ID in session storage', () => {
      const sessionId = service.getSessionId();
      const stored = sessionStorage.getItem('ritual_session_id');
      expect(stored).toBe(sessionId);
    });

    it('should reuse existing session ID from storage', () => {
      const firstService = new ActionTrackingService();
      const firstSessionId = firstService.getSessionId();

      const secondService = new ActionTrackingService();
      const secondSessionId = secondService.getSessionId();

      expect(firstSessionId).toBe(secondSessionId);
    });

    it('should set and get user ID', () => {
      const userId = 'user-123';
      service.setUserId(userId);

      expect(service.getUserId()).toBe(userId);
      expect(sessionStorage.getItem('ritual_user_id')).toBe(userId);
    });

    it('should clear session on logout', () => {
      service.setUserId('user-123');
      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      const oldSessionId = service.getSessionId();
      service.clearSession();

      expect(service.getSessionId()).not.toBe(oldSessionId);
      expect(service.getUserId()).toBeUndefined();
      expect(service.getActionCount()).toBe(0);
    });
  });

  describe('Action Tracking', () => {
    it('should track a single action', (done) => {
      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      // Wait for debounce
      setTimeout(() => {
        const actions = service.getActionSequence();
        expect(actions.length).toBe(1);
        expect(actions[0].type).toBe('ViewProduct');
        expect(actions[0].productId).toBe('product-1');
        expect(actions[0].categoryId).toBe('category-1');
        done();
      }, 600);
    });

    it('should track multiple actions', (done) => {
      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');
      service.trackAction('AddToCart', {}, 'product-1', 'category-1');
      service.trackAction('ViewProduct', {}, 'product-2', 'category-1');

      setTimeout(() => {
        const actions = service.getActionSequence();
        expect(actions.length).toBe(3);
        expect(actions[0].type).toBe('ViewProduct');
        expect(actions[1].type).toBe('AddToCart');
        expect(actions[2].type).toBe('ViewProduct');
        done();
      }, 600);
    });

    it('should include metadata in action', (done) => {
      const metadata = { source: 'recommendation', price: 100 };
      service.trackAction('AddToCart', metadata, 'product-1', 'category-1');

      setTimeout(() => {
        const actions = service.getActionSequence();
        expect(actions[0].metadata).toEqual(metadata);
        done();
      }, 600);
    });

    it('should debounce rapid action calls', (done) => {
      // Call trackAction multiple times rapidly
      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');
      service.trackAction('ViewProduct', {}, 'product-2', 'category-1');
      service.trackAction('ViewProduct', {}, 'product-3', 'category-1');

      setTimeout(() => {
        // Should only have 1 action due to debouncing
        const actions = service.getActionSequence();
        expect(actions.length).toBe(1);
        done();
      }, 600);
    });

    it('should limit actions to max per session', (done) => {
      // Track more than max actions
      for (let i = 0; i < 60; i++) {
        service.trackAction('ViewProduct', {}, `product-${i}`, 'category-1');
      }

      setTimeout(() => {
        const actions = service.getActionSequence();
        expect(actions.length).toBeLessThanOrEqual(50);
        done();
      }, 600);
    });

    it('should persist actions to session storage', (done) => {
      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      setTimeout(() => {
        const stored = sessionStorage.getItem('ritual_actions');
        expect(stored).toBeDefined();
        const parsed = JSON.parse(stored!);
        expect(parsed.length).toBe(1);
        done();
      }, 600);
    });

    it('should load actions from session storage', () => {
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
      sessionStorage.setItem('ritual_actions', JSON.stringify(actions));

      const newService = new ActionTrackingService();
      const loaded = newService.getActionSequence();

      expect(loaded.length).toBe(1);
      expect(loaded[0].type).toBe('ViewProduct');
    });
  });

  describe('Action Queries', () => {
    beforeEach((done) => {
      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');
      service.trackAction('AddToCart', {}, 'product-1', 'category-1');
      service.trackAction('ViewProduct', {}, 'product-2', 'category-1');
      service.trackAction('BrowseCategory', {}, undefined, 'category-2');

      setTimeout(() => done(), 600);
    });

    it('should get action count', () => {
      expect(service.getActionCount()).toBe(4);
    });

    it('should get last action', () => {
      const lastAction = service.getLastAction();
      expect(lastAction?.type).toBe('BrowseCategory');
    });

    it('should get actions by type', () => {
      const viewActions = service.getActionsByType('ViewProduct');
      expect(viewActions.length).toBe(2);
      expect(viewActions.every((a) => a.type === 'ViewProduct')).toBe(true);
    });

    it('should get action sequence object', () => {
      const sequence = service.getActionSequenceObject();
      expect(sequence.sessionId).toBeDefined();
      expect(sequence.actions.length).toBe(4);
      expect(sequence.startTime).toBeDefined();
      expect(sequence.lastActionTime).toBeDefined();
    });

    it('should calculate session duration', () => {
      const duration = service.getSessionDuration();
      expect(duration).toBeGreaterThanOrEqual(0);
    });
  });

  describe('Action Clearing', () => {
    it('should clear action sequence', (done) => {
      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      setTimeout(() => {
        expect(service.getActionCount()).toBe(1);

        service.clearActionSequence();
        expect(service.getActionCount()).toBe(0);
        expect(sessionStorage.getItem('ritual_actions')).toBeNull();
        done();
      }, 600);
    });

    it('should reset start time when clearing', (done) => {
      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      setTimeout(() => {
        const oldStartTime = service.getActionSequenceObject().startTime;

        // Wait a bit then clear
        setTimeout(() => {
          service.clearActionSequence();
          const newStartTime = service.getActionSequenceObject().startTime;

          expect(newStartTime).toBeGreaterThan(oldStartTime);
          done();
        }, 100);
      }, 600);
    });
  });

  describe('Subscriptions', () => {
    it('should notify listeners on action', (done) => {
      const callback = vi.fn();
      service.subscribe(callback);

      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      setTimeout(() => {
        expect(callback).toHaveBeenCalled();
        done();
      }, 600);
    });

    it('should allow unsubscribing', (done) => {
      const callback = vi.fn();
      const unsubscribe = service.subscribe(callback);

      unsubscribe();

      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      setTimeout(() => {
        expect(callback).not.toHaveBeenCalled();
        done();
      }, 600);
    });

    it('should support multiple subscribers', (done) => {
      const callback1 = vi.fn();
      const callback2 = vi.fn();

      service.subscribe(callback1);
      service.subscribe(callback2);

      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      setTimeout(() => {
        expect(callback1).toHaveBeenCalled();
        expect(callback2).toHaveBeenCalled();
        done();
      }, 600);
    });
  });

  describe('Singleton Pattern', () => {
    it('should return same instance from getter', () => {
      const instance1 = getActionTrackingService();
      const instance2 = getActionTrackingService();

      expect(instance1).toBe(instance2);
    });
  });

  describe('Debug Info', () => {
    it('should provide debug information', (done) => {
      service.setUserId('user-123');
      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      setTimeout(() => {
        const debug = service.getDebugInfo();

        expect(debug.sessionId).toBeDefined();
        expect(debug.userId).toBe('user-123');
        expect(debug.actionCount).toBe(1);
        expect(debug.sessionDuration).toBeGreaterThanOrEqual(0);
        expect(debug.lastAction).toBeDefined();
        expect(debug.actions).toBeInstanceOf(Array);
        done();
      }, 600);
    });
  });

  describe('Edge Cases', () => {
    it('should handle missing optional parameters', (done) => {
      service.trackAction('Navigation');

      setTimeout(() => {
        const actions = service.getActionSequence();
        expect(actions.length).toBe(1);
        expect(actions[0].productId).toBeUndefined();
        expect(actions[0].categoryId).toBeUndefined();
        done();
      }, 600);
    });

    it('should handle empty metadata', (done) => {
      service.trackAction('ViewProduct', {}, 'product-1', 'category-1');

      setTimeout(() => {
        const actions = service.getActionSequence();
        expect(actions[0].metadata).toEqual({});
        done();
      }, 600);
    });

    it('should handle corrupted storage data gracefully', () => {
      sessionStorage.setItem('ritual_actions', 'invalid json');

      const newService = new ActionTrackingService();
      expect(newService.getActionCount()).toBe(0);
    });
  });
});

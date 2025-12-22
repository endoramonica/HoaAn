import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';
import {
  UserPreferenceService,
  getUserPreferenceService,
} from '../userPreferenceService';

describe('UserPreferenceService', () => {
  let service: UserPreferenceService;

  beforeEach(() => {
    service = new UserPreferenceService();
    sessionStorage.clear();
    vi.clearAllMocks();
  });

  afterEach(() => {
    sessionStorage.clear();
    vi.clearAllMocks();
  });

  describe('Dismissal Recording', () => {
    it('should record a dismissal', async () => {
      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
      });

      await service.recordDismissal('user-1', 'ritual-1');

      expect(service.getDismissalCount('ritual-1')).toBe(1);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining('/api/preferences/dismiss-ritual'),
        expect.objectContaining({
          method: 'POST',
        })
      );
    });

    it('should increment dismissal count on multiple dismissals', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      await service.recordDismissal('user-1', 'ritual-1');
      await service.recordDismissal('user-1', 'ritual-1');
      await service.recordDismissal('user-1', 'ritual-1');

      expect(service.getDismissalCount('ritual-1')).toBe(3);
    });

    it('should handle API errors gracefully', async () => {
      global.fetch = vi.fn().mockRejectedValueOnce(new Error('Network error'));

      await service.recordDismissal('user-1', 'ritual-1');

      // Should still update local state
      expect(service.getDismissalCount('ritual-1')).toBe(1);
    });

    it('should persist dismissals to storage', async () => {
      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
      });

      await service.recordDismissal('user-1', 'ritual-1');

      const stored = sessionStorage.getItem('ritual_dismissed_rituals');
      expect(stored).toBeDefined();
      const parsed = JSON.parse(stored!);
      expect(parsed['ritual-1']).toBeDefined();
      expect(parsed['ritual-1'].count).toBe(1);
    });

    it('should load dismissals from storage', async () => {
      const dismissals = {
        'ritual-1': {
          ritualId: 'ritual-1',
          dismissedAt: Date.now(),
          count: 2,
        },
      };
      sessionStorage.setItem('ritual_dismissed_rituals', JSON.stringify(dismissals));

      const newService = new UserPreferenceService();

      expect(newService.getDismissalCount('ritual-1')).toBe(2);
    });
  });

  describe('Ritual Disabling', () => {
    it('should disable a ritual', async () => {
      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
      });

      await service.disableRitual('user-1', 'session-1', 'ritual-1');

      expect(service.isRitualDisabled('ritual-1')).toBe(true);
      expect(global.fetch).toHaveBeenCalledWith(
        expect.stringContaining('/api/preferences/disable-ritual'),
        expect.objectContaining({
          method: 'POST',
        })
      );
    });

    it('should handle multiple disabled rituals', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      await service.disableRitual('user-1', 'session-1', 'ritual-1');
      await service.disableRitual('user-1', 'session-1', 'ritual-2');
      await service.disableRitual('user-1', 'session-1', 'ritual-3');

      const disabled = service.getDisabledRituals();
      expect(disabled.length).toBe(3);
      expect(disabled).toContain('ritual-1');
      expect(disabled).toContain('ritual-2');
      expect(disabled).toContain('ritual-3');
    });

    it('should handle API errors gracefully', async () => {
      global.fetch = vi.fn().mockRejectedValueOnce(new Error('Network error'));

      await service.disableRitual('user-1', 'session-1', 'ritual-1');

      // Should still update local state
      expect(service.isRitualDisabled('ritual-1')).toBe(true);
    });

    it('should persist disabled rituals to storage', async () => {
      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
      });

      await service.disableRitual('user-1', 'session-1', 'ritual-1');

      const stored = sessionStorage.getItem('ritual_disabled_rituals');
      expect(stored).toBeDefined();
      const parsed = JSON.parse(stored!);
      expect(parsed).toContain('ritual-1');
    });

    it('should load disabled rituals from storage', async () => {
      const disabled = ['ritual-1', 'ritual-2'];
      sessionStorage.setItem('ritual_disabled_rituals', JSON.stringify(disabled));

      const newService = new UserPreferenceService();

      expect(newService.isRitualDisabled('ritual-1')).toBe(true);
      expect(newService.isRitualDisabled('ritual-2')).toBe(true);
    });
  });

  describe('Preference Queries', () => {
    beforeEach(async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      await service.recordDismissal('user-1', 'ritual-1');
      await service.recordDismissal('user-1', 'ritual-1');
      await service.recordDismissal('user-1', 'ritual-2');
      await service.disableRitual('user-1', 'session-1', 'ritual-3');
    });

    it('should get dismissal count', () => {
      expect(service.getDismissalCount('ritual-1')).toBe(2);
      expect(service.getDismissalCount('ritual-2')).toBe(1);
      expect(service.getDismissalCount('ritual-3')).toBe(0);
    });

    it('should get dismissal history', () => {
      const history = service.getDismissalHistory();
      expect(history.length).toBe(2);
      expect(history[0].ritualId).toBe('ritual-1');
    });

    it('should get dismissed rituals', () => {
      const dismissed = service.getDismissedRituals();
      expect(dismissed.length).toBe(2);
      expect(dismissed).toContain('ritual-1');
      expect(dismissed).toContain('ritual-2');
    });

    it('should check if ritual is dismissed', () => {
      expect(service.isRitualDismissed('ritual-1')).toBe(true);
      expect(service.isRitualDismissed('ritual-2')).toBe(true);
      expect(service.isRitualDismissed('ritual-3')).toBe(false);
    });

    it('should get disabled rituals', () => {
      const disabled = service.getDisabledRituals();
      expect(disabled.length).toBe(1);
      expect(disabled).toContain('ritual-3');
    });

    it('should check if ritual is disabled', () => {
      expect(service.isRitualDisabled('ritual-1')).toBe(false);
      expect(service.isRitualDisabled('ritual-3')).toBe(true);
    });
  });

  describe('Preference Clearing', () => {
    it('should clear all preferences', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      await service.recordDismissal('user-1', 'ritual-1');
      await service.disableRitual('user-1', 'session-1', 'ritual-2');

      service.clearPreferences();

      expect(service.getDismissedRituals().length).toBe(0);
      expect(service.getDisabledRituals().length).toBe(0);
      expect(sessionStorage.getItem('ritual_dismissed_rituals')).toBeNull();
      expect(sessionStorage.getItem('ritual_disabled_rituals')).toBeNull();
    });
  });

  describe('Subscriptions', () => {
    it('should notify listeners on dismissal', async () => {
      const callback = vi.fn();
      service.subscribe(callback);

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
      });

      await service.recordDismissal('user-1', 'ritual-1');

      expect(callback).toHaveBeenCalled();
    });

    it('should notify listeners on disable', async () => {
      const callback = vi.fn();
      service.subscribe(callback);

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
      });

      await service.disableRitual('user-1', 'session-1', 'ritual-1');

      expect(callback).toHaveBeenCalled();
    });

    it('should notify listeners on clear', async () => {
      const callback = vi.fn();
      service.subscribe(callback);

      service.clearPreferences();

      expect(callback).toHaveBeenCalled();
    });

    it('should allow unsubscribing', async () => {
      const callback = vi.fn();
      const unsubscribe = service.subscribe(callback);

      unsubscribe();

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
      });

      await service.recordDismissal('user-1', 'ritual-1');

      expect(callback).not.toHaveBeenCalled();
    });

    it('should support multiple subscribers', async () => {
      const callback1 = vi.fn();
      const callback2 = vi.fn();

      service.subscribe(callback1);
      service.subscribe(callback2);

      global.fetch = vi.fn().mockResolvedValueOnce({
        ok: true,
      });

      await service.recordDismissal('user-1', 'ritual-1');

      expect(callback1).toHaveBeenCalled();
      expect(callback2).toHaveBeenCalled();
    });
  });

  describe('Singleton Pattern', () => {
    it('should return same instance from getter', () => {
      const instance1 = getUserPreferenceService();
      const instance2 = getUserPreferenceService();

      expect(instance1).toBe(instance2);
    });
  });

  describe('Debug Info', () => {
    it('should provide debug information', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      await service.recordDismissal('user-1', 'ritual-1');
      await service.disableRitual('user-1', 'session-1', 'ritual-2');

      const debug = service.getDebugInfo();

      expect(debug.dismissedRituals).toBeDefined();
      expect(debug.disabledRituals).toBeDefined();
      expect(debug.dismissalHistory).toBeDefined();
    });
  });

  describe('Edge Cases', () => {
    it('should handle corrupted storage data gracefully', () => {
      sessionStorage.setItem('ritual_dismissed_rituals', 'invalid json');
      sessionStorage.setItem('ritual_disabled_rituals', 'invalid json');

      const newService = new UserPreferenceService();

      expect(newService.getDismissedRituals().length).toBe(0);
      expect(newService.getDisabledRituals().length).toBe(0);
    });

    it('should handle duplicate disables', async () => {
      global.fetch = vi.fn().mockResolvedValue({
        ok: true,
      });

      await service.disableRitual('user-1', 'session-1', 'ritual-1');
      await service.disableRitual('user-1', 'session-1', 'ritual-1');

      const disabled = service.getDisabledRituals();
      expect(disabled.length).toBe(1);
    });
  });
});

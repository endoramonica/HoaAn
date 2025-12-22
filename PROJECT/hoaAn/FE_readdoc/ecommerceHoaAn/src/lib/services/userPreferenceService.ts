/**
 * User Preference Service - Quản lý dismissals và disabled rituals
 * Stores preferences in session storage (per session only)
 */

// ============================================================================
// Types
// ============================================================================

export interface DismissalRecord {
  ritualId: string;
  dismissedAt: number;
  count: number;
}

export interface UserPreferences {
  dismissedRituals: Record<string, DismissalRecord>;
  disabledRituals: Set<string>;
}

// ============================================================================
// Configuration
// ============================================================================

const CONFIG = {
  API_BASE_URL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
  STORAGE_KEY_DISMISSED: 'ritual_dismissed_rituals',
  STORAGE_KEY_DISABLED: 'ritual_disabled_rituals',
};

// ============================================================================
// User Preference Service
// ============================================================================

class UserPreferenceService {
  private dismissedRituals: Map<string, DismissalRecord> = new Map();
  private disabledRituals: Set<string> = new Set();
  private listeners: Set<() => void> = new Set();

  constructor() {
    this.loadFromStorage();
  }

  /**
   * Record a dismissal
   */
  async recordDismissal(userId: string, ritualId: string): Promise<void> {
    try {
      // Call backend API
      const response = await fetch(
        `${CONFIG.API_BASE_URL}/api/preferences/dismiss-ritual`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            userId,
            ritualId,
          }),
        }
      );

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}`);
      }

      // Update local state
      const existing = this.dismissedRituals.get(ritualId);
      if (existing) {
        existing.count += 1;
        existing.dismissedAt = Date.now();
      } else {
        this.dismissedRituals.set(ritualId, {
          ritualId,
          dismissedAt: Date.now(),
          count: 1,
        });
      }

      this.saveToStorage();
      this.notifyListeners();
    } catch (error) {
      console.error('Error recording dismissal:', error);
      // Still update local state even if API fails
      const existing = this.dismissedRituals.get(ritualId);
      if (existing) {
        existing.count += 1;
        existing.dismissedAt = Date.now();
      } else {
        this.dismissedRituals.set(ritualId, {
          ritualId,
          dismissedAt: Date.now(),
          count: 1,
        });
      }
      this.saveToStorage();
      this.notifyListeners();
    }
  }

  /**
   * Disable a ritual for current session
   */
  async disableRitual(userId: string, sessionId: string, ritualId: string): Promise<void> {
    try {
      // Call backend API
      const response = await fetch(
        `${CONFIG.API_BASE_URL}/api/preferences/disable-ritual`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            userId,
            sessionId,
            ritualId,
          }),
        }
      );

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}`);
      }

      // Update local state
      this.disabledRituals.add(ritualId);
      this.saveToStorage();
      this.notifyListeners();
    } catch (error) {
      console.error('Error disabling ritual:', error);
      // Still update local state even if API fails
      this.disabledRituals.add(ritualId);
      this.saveToStorage();
      this.notifyListeners();
    }
  }

  /**
   * Check if ritual is disabled
   */
  isRitualDisabled(ritualId: string): boolean {
    return this.disabledRituals.has(ritualId);
  }

  /**
   * Get all disabled rituals
   */
  getDisabledRituals(): string[] {
    return Array.from(this.disabledRituals);
  }

  /**
   * Get dismissal count for a ritual
   */
  getDismissalCount(ritualId: string): number {
    return this.dismissedRituals.get(ritualId)?.count || 0;
  }

  /**
   * Get dismissal history
   */
  getDismissalHistory(): DismissalRecord[] {
    return Array.from(this.dismissedRituals.values()).sort(
      (a, b) => b.dismissedAt - a.dismissedAt
    );
  }

  /**
   * Get all dismissed rituals
   */
  getDismissedRituals(): string[] {
    return Array.from(this.dismissedRituals.keys());
  }

  /**
   * Check if ritual is dismissed
   */
  isRitualDismissed(ritualId: string): boolean {
    return this.dismissedRituals.has(ritualId);
  }

  /**
   * Clear all preferences (on logout)
   */
  clearPreferences(): void {
    this.dismissedRituals.clear();
    this.disabledRituals.clear();
    sessionStorage.removeItem(CONFIG.STORAGE_KEY_DISMISSED);
    sessionStorage.removeItem(CONFIG.STORAGE_KEY_DISABLED);
    this.notifyListeners();
  }

  /**
   * Save preferences to session storage
   */
  private saveToStorage(): void {
    try {
      const dismissed = Object.fromEntries(this.dismissedRituals);
      sessionStorage.setItem(CONFIG.STORAGE_KEY_DISMISSED, JSON.stringify(dismissed));

      const disabled = Array.from(this.disabledRituals);
      sessionStorage.setItem(CONFIG.STORAGE_KEY_DISABLED, JSON.stringify(disabled));
    } catch (error) {
      console.warn('Failed to save preferences to storage:', error);
    }
  }

  /**
   * Load preferences from session storage
   */
  private loadFromStorage(): void {
    try {
      const dismissedStr = sessionStorage.getItem(CONFIG.STORAGE_KEY_DISMISSED);
      if (dismissedStr) {
        const dismissed = JSON.parse(dismissedStr);
        this.dismissedRituals = new Map(Object.entries(dismissed));
      }

      const disabledStr = sessionStorage.getItem(CONFIG.STORAGE_KEY_DISABLED);
      if (disabledStr) {
        const disabled = JSON.parse(disabledStr);
        this.disabledRituals = new Set(disabled);
      }
    } catch (error) {
      console.warn('Failed to load preferences from storage:', error);
      this.dismissedRituals.clear();
      this.disabledRituals.clear();
    }
  }

  /**
   * Subscribe to preference changes
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
        console.error('Error in preference listener:', error);
      }
    });
  }

  /**
   * Get debug info
   */
  getDebugInfo(): Record<string, any> {
    return {
      dismissedRituals: Array.from(this.dismissedRituals.entries()),
      disabledRituals: Array.from(this.disabledRituals),
      dismissalHistory: this.getDismissalHistory(),
    };
  }
}

// ============================================================================
// Singleton Instance
// ============================================================================

let instance: UserPreferenceService | null = null;

export function getUserPreferenceService(): UserPreferenceService {
  if (!instance) {
    instance = new UserPreferenceService();
  }
  return instance;
}

// Export for testing
export { UserPreferenceService };

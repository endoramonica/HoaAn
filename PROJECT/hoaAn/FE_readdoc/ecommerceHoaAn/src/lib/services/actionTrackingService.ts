/**
 * Action Tracking Service - Theo dõi hành động người dùng cho Sequential Ritual Recommendation
 * Captures user interactions (view product, add to cart, browse category)
 * Sends action sequence to BE-AI for pattern matching
 */

// Generate UUID without external dependency
const uuidv4 = (): string => {
  if (typeof crypto !== 'undefined' && crypto.randomUUID) {
    return crypto.randomUUID();
  }
  // Fallback for older browsers
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0;
    const v = c === 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
  });
};

// ============================================================================
// Types
// ============================================================================

export interface Action {
  id: string;
  userId?: string;
  sessionId: string;
  type: 'ViewProduct' | 'AddToCart' | 'BrowseCategory' | 'ViewCheckout' | 'Navigation';
  timestamp: number;
  productId?: string;
  categoryId?: string;
  metadata?: Record<string, any>;
}

export interface ActionSequence {
  sessionId: string;
  userId?: string;
  actions: Action[];
  startTime: number;
  lastActionTime: number;
}

// ============================================================================
// Action Tracking Service
// ============================================================================

class ActionTrackingService {
  private sessionId: string;
  private userId?: string;
  private actions: Action[] = [];
  private startTime: number = Date.now();
  private lastActionTime: number = Date.now();
  private maxActionsPerSession: number = 50; // Limit to prevent memory issues
  private debounceTimer: ReturnType<typeof setTimeout> | null = null;
  private debounceDelay: number = 500; // ms

  constructor() {
    this.sessionId = this.getOrCreateSessionId();
    this.loadActionsFromStorage();
  }

  /**
   * Get or create session ID from session storage
   */
  private getOrCreateSessionId(): string {
    const stored = sessionStorage.getItem('ritual_session_id');
    if (stored) {
      return stored;
    }
    const newSessionId = uuidv4();
    sessionStorage.setItem('ritual_session_id', newSessionId);
    return newSessionId;
  }

  /**
   * Set user ID (called after login)
   */
  setUserId(userId: string): void {
    this.userId = userId;
    sessionStorage.setItem('ritual_user_id', userId);
  }

  /**
   * Get current user ID
   */
  getUserId(): string | undefined {
    return this.userId || sessionStorage.getItem('ritual_user_id') || undefined;
  }

  /**
   * Get current session ID
   */
  getSessionId(): string {
    return this.sessionId;
  }

  /**
   * Track a user action
   * Debounced to prevent too many rapid calls
   */
  trackAction(
    type: Action['type'],
    metadata?: Record<string, any>,
    productId?: string,
    categoryId?: string
  ): void {
    // Clear existing debounce timer
    if (this.debounceTimer) {
      clearTimeout(this.debounceTimer);
    }

    // Debounce the action tracking
    this.debounceTimer = setTimeout(() => {
      const action: Action = {
        id: uuidv4(),
        userId: this.userId,
        sessionId: this.sessionId,
        type,
        timestamp: Date.now(),
        productId,
        categoryId,
        metadata,
      };

      // Add action to sequence
      this.actions.push(action);
      this.lastActionTime = Date.now();

      // Keep only last N actions
      if (this.actions.length > this.maxActionsPerSession) {
        this.actions = this.actions.slice(-this.maxActionsPerSession);
      }

      // Save to session storage
      this.saveActionsToStorage();

      // Trigger callback for listeners
      this.notifyListeners();
    }, this.debounceDelay);
  }

  /**
   * Get current action sequence
   */
  getActionSequence(): Action[] {
    return [...this.actions];
  }

  /**
   * Get action sequence as ActionSequence object
   */
  getActionSequenceObject(): ActionSequence {
    return {
      sessionId: this.sessionId,
      userId: this.userId,
      actions: this.getActionSequence(),
      startTime: this.startTime,
      lastActionTime: this.lastActionTime,
    };
  }

  /**
   * Clear action sequence (on navigation away)
   */
  clearActionSequence(): void {
    this.actions = [];
    this.startTime = Date.now();
    this.lastActionTime = Date.now();
    sessionStorage.removeItem('ritual_actions');
    this.notifyListeners();
  }

  /**
   * Clear session (on logout)
   */
  clearSession(): void {
    this.clearActionSequence();
    this.userId = undefined;
    this.sessionId = uuidv4();
    sessionStorage.removeItem('ritual_session_id');
    sessionStorage.removeItem('ritual_user_id');
    sessionStorage.removeItem('ritual_actions');
  }

  /**
   * Get action count
   */
  getActionCount(): number {
    return this.actions.length;
  }

  /**
   * Get last action
   */
  getLastAction(): Action | undefined {
    return this.actions[this.actions.length - 1];
  }

  /**
   * Get actions of specific type
   */
  getActionsByType(type: Action['type']): Action[] {
    return this.actions.filter((a) => a.type === type);
  }

  /**
   * Get session duration in milliseconds
   */
  getSessionDuration(): number {
    return this.lastActionTime - this.startTime;
  }

  /**
   * Save actions to session storage
   */
  private saveActionsToStorage(): void {
    try {
      sessionStorage.setItem('ritual_actions', JSON.stringify(this.actions));
    } catch (error) {
      console.warn('Failed to save actions to session storage:', error);
    }
  }

  /**
   * Load actions from session storage
   */
  private loadActionsFromStorage(): void {
    try {
      const stored = sessionStorage.getItem('ritual_actions');
      if (stored) {
        this.actions = JSON.parse(stored);
        if (this.actions.length > 0) {
          this.lastActionTime = this.actions[this.actions.length - 1].timestamp;
        }
      }
    } catch (error) {
      console.warn('Failed to load actions from session storage:', error);
      this.actions = [];
    }
  }

  /**
   * Listeners for action changes
   */
  private listeners: Set<() => void> = new Set();

  /**
   * Subscribe to action changes
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
        console.error('Error in action tracking listener:', error);
      }
    });
  }

  /**
   * Get debug info
   */
  getDebugInfo(): Record<string, any> {
    return {
      sessionId: this.sessionId,
      userId: this.userId,
      actionCount: this.actions.length,
      sessionDuration: this.getSessionDuration(),
      lastAction: this.getLastAction(),
      actions: this.actions,
    };
  }
}

// ============================================================================
// Singleton Instance
// ============================================================================

let instance: ActionTrackingService | null = null;

export function getActionTrackingService(): ActionTrackingService {
  if (!instance) {
    instance = new ActionTrackingService();
  }
  return instance;
}

// Export for testing
export { ActionTrackingService };

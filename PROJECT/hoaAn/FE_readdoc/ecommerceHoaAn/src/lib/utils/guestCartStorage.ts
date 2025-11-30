/**
 * Guest Cart Storage Utilities
 * Manages guest cart sessionId in localStorage
 */

const GUEST_CART_SESSION_KEY = 'guest_cart_session_id';

export const guestCartStorage = {
    /**
     * Get guest cart sessionId
     */
    getSessionId(): string | null {
        try {
            return localStorage.getItem(GUEST_CART_SESSION_KEY);
        } catch (error) {
            console.error('Failed to get guest cart sessionId:', error);
            return null;
        }
    },

    /**
     * Set guest cart sessionId
     */
    setSessionId(sessionId: string): void {
        try {
            localStorage.setItem(GUEST_CART_SESSION_KEY, sessionId);
            console.log('[GuestCart] 💾 Saved sessionId:', sessionId);
        } catch (error) {
            console.error('Failed to save guest cart sessionId:', error);
        }
    },

    /**
     * Clear guest cart sessionId (after merge or logout)
     */
    clearSessionId(): void {
        try {
            localStorage.removeItem(GUEST_CART_SESSION_KEY);
            console.log('[GuestCart] 🗑️ Cleared sessionId');
        } catch (error) {
            console.error('Failed to clear guest cart sessionId:', error);
        }
    },

    /**
     * Generate new sessionId for guest cart
     */
    generateSessionId(): string {
        const sessionId = `guest_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
        this.setSessionId(sessionId);
        return sessionId;
    },

    /**
     * Get or create sessionId
     */
    getOrCreateSessionId(): string {
        let sessionId = this.getSessionId();
        if (!sessionId) {
            sessionId = this.generateSessionId();
        }
        return sessionId;
    },
};

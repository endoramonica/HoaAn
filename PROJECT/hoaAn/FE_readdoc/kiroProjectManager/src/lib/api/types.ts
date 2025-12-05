/**
 * Shared type definitions for API layer
 */

/**
 * Storage keys for token management
 */
export const STORAGE_KEYS = {
    ACCESS_TOKEN: 'access_token',
    REFRESH_TOKEN: 'refresh_token',
    REMEMBER_ME: 'remember_me',
} as const;

/**
 * Token storage interface
 */
export interface TokenStorage {
    // Storage selection
    isRememberMe(): boolean;
    setRememberMe(remember: boolean): void;
    getStorage(): Storage;

    // Token operations
    getAccessToken(): string | null;
    setAccessToken(token: string): void;
    getRefreshToken(): string | null;
    setRefreshToken(token: string): void;

    // Batch operations
    setTokens(accessToken: string, refreshToken: string, rememberMe?: boolean): void;
    clearTokens(): void;
}

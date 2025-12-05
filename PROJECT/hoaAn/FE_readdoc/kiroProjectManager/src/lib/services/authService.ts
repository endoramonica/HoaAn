import {
    postApiAuthLogin,
    postApiAuthLogout,
    postApiAuthRefreshToken
} from '../../../api/generated-orval/auth/auth';
import { getApiCurrentUser } from '../../../api/generated-orval/current-user/current-user';
import type { LoginDTO, RefreshTokenRequestDTO } from '../../../api/generated-orval/schemas';
import { tokenStorage } from '../api/client';
import { createApiError } from '../api/errors';

/**
 * Authentication response interface
 * Represents the response from login and refresh token endpoints
 */
export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    expiresIn?: number;
}

/**
 * User information interface
 * Represents the current authenticated user
 */
export interface User {
    id: string;
    email: string;
    fullName?: string;
    role?: string;
    [key: string]: any; // Allow additional properties
}

/**
 * Authentication Service
 * Handles user authentication, token management, and user information retrieval
 */
class AuthService {

    /**
     * Login user with email and password
     * Stores tokens in appropriate storage based on rememberMe preference
     * @param email - User email
     * @param password - User password
     * @param rememberMe - Whether to persist session (localStorage vs sessionStorage)
     * @returns Promise with authentication response
     */
    async login(email: string, password: string, rememberMe: boolean = false): Promise<AuthResponse> {
        try {
            const loginDto: LoginDTO = {
                email,
                password,
            };

            const response = await postApiAuthLogin(loginDto);

            // Extract tokens from response
            // The API returns tokens wrapped in a data object
            const responseData = response.data as any;
            const data = responseData?.data || responseData;

            if (!data || (!data.token && !data.accessToken) || !data.refreshToken) {
                throw new Error('Invalid response from login endpoint');
            }

            const authResponse: AuthResponse = {
                accessToken: data.token || data.accessToken,
                refreshToken: data.refreshToken,
                expiresIn: data.expiresIn,
            };

            // Store tokens with remember me preference
            tokenStorage.setTokens(
                authResponse.accessToken,
                authResponse.refreshToken,
                rememberMe
            );

            return authResponse;
        } catch (error) {
            console.error('[AuthService] Login failed:', error);
            throw createApiError(error);
        }
    }

    /**
     * Logout user
     * Clears all stored tokens and calls logout endpoint
     * @returns Promise that resolves when logout is complete
     */
    async logout(): Promise<void> {
        try {
            // Call logout endpoint to invalidate tokens on server
            await postApiAuthLogout();
        } catch (error) {
            // Log error but don't throw - we still want to clear local tokens
            console.error('[AuthService] Logout API call failed:', error);
        } finally {
            // Always clear tokens from storage
            tokenStorage.clearTokens();
        }
    }

    /**
     * Refresh access token using refresh token
     * Updates stored tokens with new values
     * @returns Promise with new authentication response
     */
    async refreshToken(): Promise<AuthResponse> {
        try {
            const refreshToken = tokenStorage.getRefreshToken();

            if (!refreshToken) {
                throw new Error('No refresh token available');
            }

            const refreshDto: RefreshTokenRequestDTO = {
                refreshToken,
            };

            const response = await postApiAuthRefreshToken(refreshDto);

            // Extract tokens from response
            // The API returns tokens wrapped in a data object
            const responseData = response.data as any;
            const data = responseData?.data || responseData;

            if (!data || (!data.token && !data.accessToken) || !data.refreshToken) {
                throw new Error('Invalid response from refresh token endpoint');
            }

            const authResponse: AuthResponse = {
                accessToken: data.token || data.accessToken,
                refreshToken: data.refreshToken,
                expiresIn: data.expiresIn,
            };

            // Update stored tokens (preserve remember me preference)
            tokenStorage.setTokens(
                authResponse.accessToken,
                authResponse.refreshToken
            );

            return authResponse;
        } catch (error) {
            console.error('[AuthService] Token refresh failed:', error);
            // Clear tokens on refresh failure
            tokenStorage.clearTokens();
            throw createApiError(error);
        }
    }

    /**
     * Get current authenticated user information
     * @returns Promise with user information
     */
    async getCurrentUser(): Promise<User> {
        try {
            const response = await getApiCurrentUser();

            // Extract user data from response
            const data = response.data as any;

            if (!data) {
                throw new Error('Invalid response from current user endpoint');
            }

            // Return user data
            return data as User;
        } catch (error) {
            console.error('[AuthService] Get current user failed:', error);
            throw createApiError(error);
        }
    }

    /**
     * Check if user is authenticated
     * @returns true if access token exists
     */
    isAuthenticated(): boolean {
        return tokenStorage.getAccessToken() !== null;
    }
}

// Export singleton instance
export const authService = new AuthService();

// Export class for testing purposes
export { AuthService };

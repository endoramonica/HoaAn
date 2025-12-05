/**
 * Example usage of AuthService
 * This file demonstrates how to use the authentication service
 */

import { authService } from './authService';

/**
 * Example: Login user
 */
async function exampleLogin() {
    try {
        const response = await authService.login(
            'user@example.com',
            'password123',
            true // remember me
        );

        console.log('Login successful:', response);
        console.log('Access token:', response.accessToken);
        console.log('Refresh token:', response.refreshToken);
    } catch (error) {
        console.error('Login failed:', error);
    }
}

/**
 * Example: Get current user
 */
async function exampleGetCurrentUser() {
    try {
        const user = await authService.getCurrentUser();
        console.log('Current user:', user);
        console.log('User email:', user.email);
        console.log('User role:', user.role);
    } catch (error) {
        console.error('Failed to get current user:', error);
    }
}

/**
 * Example: Refresh token
 */
async function exampleRefreshToken() {
    try {
        const response = await authService.refreshToken();
        console.log('Token refreshed:', response);
        console.log('New access token:', response.accessToken);
    } catch (error) {
        console.error('Token refresh failed:', error);
    }
}

/**
 * Example: Check authentication status
 */
function exampleCheckAuth() {
    const isAuthenticated = authService.isAuthenticated();
    console.log('Is authenticated:', isAuthenticated);
}

/**
 * Example: Logout user
 */
async function exampleLogout() {
    try {
        await authService.logout();
        console.log('Logout successful');
        console.log('Is authenticated:', authService.isAuthenticated()); // Should be false
    } catch (error) {
        console.error('Logout failed:', error);
    }
}

/**
 * Example: Complete authentication flow
 */
async function exampleCompleteFlow() {
    try {
        // 1. Login
        console.log('Step 1: Login');
        await authService.login('user@example.com', 'password123', true);
        console.log('Authenticated:', authService.isAuthenticated());

        // 2. Get current user
        console.log('\nStep 2: Get current user');
        const user = await authService.getCurrentUser();
        console.log('User:', user);

        // 3. Refresh token (simulating token expiration)
        console.log('\nStep 3: Refresh token');
        await authService.refreshToken();
        console.log('Token refreshed');

        // 4. Logout
        console.log('\nStep 4: Logout');
        await authService.logout();
        console.log('Authenticated:', authService.isAuthenticated());
    } catch (error) {
        console.error('Flow failed:', error);
    }
}

// Export examples for documentation purposes
export {
    exampleLogin,
    exampleGetCurrentUser,
    exampleRefreshToken,
    exampleCheckAuth,
    exampleLogout,
    exampleCompleteFlow,
};

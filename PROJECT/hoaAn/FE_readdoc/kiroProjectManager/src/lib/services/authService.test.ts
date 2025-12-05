import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { AuthService, authService } from './authService';
import { tokenStorage } from '../api/client';

// Mock the generated API modules
vi.mock('../../../api/generated-orval/auth/auth', () => ({
    getAuth: () => ({
        postApiAuthLogin: vi.fn(),
        postApiAuthLogout: vi.fn(),
        postApiAuthRefreshToken: vi.fn(),
    }),
}));

vi.mock('../../../api/generated-orval/current-user/current-user', () => ({
    getCurrentUser: () => ({
        getApiCurrentUser: vi.fn(),
    }),
}));

describe('AuthService', () => {
    beforeEach(() => {
        // Clear storage before each test
        tokenStorage.clearTokens();
        vi.clearAllMocks();
    });

    afterEach(() => {
        // Clean up after each test
        tokenStorage.clearTokens();
    });

    describe('login', () => {
        it('should store tokens after successful login', async () => {
            const mockResponse = {
                data: {
                    accessToken: 'test-access-token',
                    refreshToken: 'test-refresh-token',
                    expiresIn: 3600,
                },
            };

            const service = new AuthService();
            const authApi = (service as any).authApi;
            authApi.postApiAuthLogin = vi.fn().mockResolvedValue(mockResponse);

            await service.login('test@example.com', 'password', true);

            expect(tokenStorage.getAccessToken()).toBe('test-access-token');
            expect(tokenStorage.getRefreshToken()).toBe('test-refresh-token');
            expect(tokenStorage.isRememberMe()).toBe(true);
        });

        it('should use sessionStorage when rememberMe is false', async () => {
            const mockResponse = {
                data: {
                    accessToken: 'test-access-token',
                    refreshToken: 'test-refresh-token',
                },
            };

            const service = new AuthService();
            const authApi = (service as any).authApi;
            authApi.postApiAuthLogin = vi.fn().mockResolvedValue(mockResponse);

            await service.login('test@example.com', 'password', false);

            expect(tokenStorage.getAccessToken()).toBe('test-access-token');
            expect(tokenStorage.isRememberMe()).toBe(false);
        });

        it('should throw error and log when login fails', async () => {
            const consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => { });
            const service = new AuthService();
            const authApi = (service as any).authApi;
            authApi.postApiAuthLogin = vi.fn().mockRejectedValue(new Error('Login failed'));

            await expect(service.login('test@example.com', 'wrong-password')).rejects.toThrow();
            expect(consoleErrorSpy).toHaveBeenCalledWith(
                expect.stringContaining('[AuthService] Login failed:'),
                expect.any(Error)
            );

            consoleErrorSpy.mockRestore();
        });
    });

    describe('logout', () => {
        it('should clear tokens after logout', async () => {
            // Set up tokens first
            tokenStorage.setTokens('access-token', 'refresh-token', true);

            const service = new AuthService();
            const authApi = (service as any).authApi;
            authApi.postApiAuthLogout = vi.fn().mockResolvedValue({});

            await service.logout();

            expect(tokenStorage.getAccessToken()).toBeNull();
            expect(tokenStorage.getRefreshToken()).toBeNull();
        });

        it('should clear tokens even if API call fails', async () => {
            const consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => { });
            tokenStorage.setTokens('access-token', 'refresh-token', true);

            const service = new AuthService();
            const authApi = (service as any).authApi;
            authApi.postApiAuthLogout = vi.fn().mockRejectedValue(new Error('Logout failed'));

            await service.logout();

            expect(tokenStorage.getAccessToken()).toBeNull();
            expect(tokenStorage.getRefreshToken()).toBeNull();
            expect(consoleErrorSpy).toHaveBeenCalled();

            consoleErrorSpy.mockRestore();
        });
    });

    describe('refreshToken', () => {
        it('should update tokens after successful refresh', async () => {
            tokenStorage.setTokens('old-access-token', 'old-refresh-token', true);

            const mockResponse = {
                data: {
                    accessToken: 'new-access-token',
                    refreshToken: 'new-refresh-token',
                    expiresIn: 3600,
                },
            };

            const service = new AuthService();
            const authApi = (service as any).authApi;
            authApi.postApiAuthRefreshToken = vi.fn().mockResolvedValue(mockResponse);

            const result = await service.refreshToken();

            expect(result.accessToken).toBe('new-access-token');
            expect(result.refreshToken).toBe('new-refresh-token');
            expect(tokenStorage.getAccessToken()).toBe('new-access-token');
            expect(tokenStorage.getRefreshToken()).toBe('new-refresh-token');
        });

        it('should clear tokens and throw error when refresh fails', async () => {
            const consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => { });
            tokenStorage.setTokens('access-token', 'refresh-token', true);

            const service = new AuthService();
            const authApi = (service as any).authApi;
            authApi.postApiAuthRefreshToken = vi.fn().mockRejectedValue(new Error('Refresh failed'));

            await expect(service.refreshToken()).rejects.toThrow();
            expect(tokenStorage.getAccessToken()).toBeNull();
            expect(tokenStorage.getRefreshToken()).toBeNull();
            expect(consoleErrorSpy).toHaveBeenCalled();

            consoleErrorSpy.mockRestore();
        });
    });

    describe('getCurrentUser', () => {
        it('should return user data from API', async () => {
            const mockUser = {
                id: '123',
                email: 'test@example.com',
                fullName: 'Test User',
                role: 'admin',
            };

            const mockResponse = {
                data: mockUser,
            };

            const service = new AuthService();
            const userApi = (service as any).userApi;
            userApi.getApiCurrentUser = vi.fn().mockResolvedValue(mockResponse);

            const result = await service.getCurrentUser();

            expect(result).toEqual(mockUser);
        });

        it('should throw error and log when getCurrentUser fails', async () => {
            const consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => { });
            const service = new AuthService();
            const userApi = (service as any).userApi;
            userApi.getApiCurrentUser = vi.fn().mockRejectedValue(new Error('Failed to get user'));

            await expect(service.getCurrentUser()).rejects.toThrow();
            expect(consoleErrorSpy).toHaveBeenCalledWith(
                expect.stringContaining('[AuthService] Get current user failed:'),
                expect.any(Error)
            );

            consoleErrorSpy.mockRestore();
        });
    });

    describe('isAuthenticated', () => {
        it('should return true when access token exists', () => {
            tokenStorage.setTokens('access-token', 'refresh-token');
            expect(authService.isAuthenticated()).toBe(true);
        });

        it('should return false when no access token exists', () => {
            tokenStorage.clearTokens();
            expect(authService.isAuthenticated()).toBe(false);
        });
    });
});

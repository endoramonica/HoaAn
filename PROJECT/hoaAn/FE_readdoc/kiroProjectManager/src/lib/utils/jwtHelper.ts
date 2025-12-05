/**
 * JWT Helper utilities
 * Decode and extract information from JWT tokens
 */

interface JWTPayload {
    nameid?: string;
    email?: string;
    role?: string | string[];
    permission?: string[];
    exp?: number;
    iat?: number;
    [key: string]: any;
}

/**
 * Decode JWT token without verification
 * @param token - JWT token string
 * @returns Decoded payload or null if invalid
 */
export function decodeJWT(token: string): JWTPayload | null {
    try {
        // JWT format: header.payload.signature
        const parts = token.split('.');
        if (parts.length !== 3) {
            return null;
        }

        // Decode base64url payload
        const payload = parts[1];
        const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        );

        return JSON.parse(jsonPayload);
    } catch (error) {
        console.error('[JWT] Failed to decode token:', error);
        return null;
    }
}

/**
 * Extract user ID from token
 * @param token - JWT token string
 * @returns User ID or null
 */
export function getUserIdFromToken(token: string): string | null {
    const payload = decodeJWT(token);
    return payload?.nameid || payload?.sub || null;
}

/**
 * Extract email from token
 * @param token - JWT token string
 * @returns Email or null
 */
export function getEmailFromToken(token: string): string | null {
    const payload = decodeJWT(token);
    return payload?.email || null;
}

/**
 * Extract roles from token
 * Handles both single role (string) and multiple roles (array)
 * @param token - JWT token string
 * @returns Array of roles
 */
export function getRolesFromToken(token: string): string[] {
    const payload = decodeJWT(token);

    if (!payload?.role) {
        return [];
    }

    // Handle array of roles
    if (Array.isArray(payload.role)) {
        return payload.role;
    }

    // Handle single role as string
    return [payload.role];
}

/**
 * Check if token has specific role(s)
 * @param token - JWT token string
 * @param requiredRoles - Role or array of roles to check
 * @returns true if token has any of the required roles
 */
export function hasRole(token: string, requiredRoles: string | string[]): boolean {
    const userRoles = getRolesFromToken(token);
    const rolesToCheck = Array.isArray(requiredRoles) ? requiredRoles : [requiredRoles];

    return rolesToCheck.some(role =>
        userRoles.some(userRole =>
            userRole.toLowerCase() === role.toLowerCase()
        )
    );
}

/**
 * Check if token has Administrator role
 * @param token - JWT token string
 * @returns true if token has Administrator role
 */
export function isAdministrator(token: string): boolean {
    return hasRole(token, 'Administrator');
}

/**
 * Extract permissions from token
 * @param token - JWT token string
 * @returns Array of permissions
 */
export function getPermissionsFromToken(token: string): string[] {
    const payload = decodeJWT(token);
    return payload?.permission || [];
}

/**
 * Check if token has specific permission(s)
 * @param token - JWT token string
 * @param requiredPermissions - Permission or array of permissions to check
 * @returns true if token has any of the required permissions
 */
export function hasPermission(token: string, requiredPermissions: string | string[]): boolean {
    const userPermissions = getPermissionsFromToken(token);
    const permissionsToCheck = Array.isArray(requiredPermissions) ? requiredPermissions : [requiredPermissions];

    return permissionsToCheck.some(permission =>
        userPermissions.includes(permission)
    );
}

/**
 * Check if token is expired
 * @param token - JWT token string
 * @returns true if token is expired
 */
export function isTokenExpired(token: string): boolean {
    const payload = decodeJWT(token);

    if (!payload?.exp) {
        return true;
    }

    // exp is in seconds, Date.now() is in milliseconds
    return payload.exp * 1000 < Date.now();
}

/**
 * Get token expiration time
 * @param token - JWT token string
 * @returns Expiration date or null
 */
export function getTokenExpiration(token: string): Date | null {
    const payload = decodeJWT(token);

    if (!payload?.exp) {
        return null;
    }

    return new Date(payload.exp * 1000);
}

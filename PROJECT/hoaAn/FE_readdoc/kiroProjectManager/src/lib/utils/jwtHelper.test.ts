/**
 * JWT Helper Tests
 * Test cases for JWT token parsing and validation
 */

import {
    decodeJWT,
    getRolesFromToken,
    getPermissionsFromToken,
    hasRole,
    hasPermission,
    isAdministrator,
    isTokenExpired
} from './jwtHelper';

// Sample token payload (base64 encoded)
const samplePayload = {
    nameid: "2aaa2386-e5a8-469e-959b-147adddf15a1",
    email: "user111@example.com",
    jti: "b897b6b6-d0da-47c2-8f91-64ce6e2dbb63",
    iat: 1764749434,
    role: ["Staff", "Administrator"],
    permission: [
        "order.view_all",
        "inventory.view",
        "product.create",
        "order.cancel",
        "pos.access",
        "admin.manage_users"
    ],
    nbf: 1764749434,
    exp: 1764756634,
    iss: "VietCommerce.Api",
    aud: "VietCommerce.Client"
};

// Create a mock JWT token
function createMockToken(payload: any): string {
    const header = btoa(JSON.stringify({ alg: "HS256", typ: "JWT" }));
    const payloadStr = btoa(JSON.stringify(payload));
    const signature = "mock-signature";
    return `${header}.${payloadStr}.${signature}`;
}

describe('JWT Helper', () => {
    const mockToken = createMockToken(samplePayload);

    describe('decodeJWT', () => {
        it('should decode valid token', () => {
            const decoded = decodeJWT(mockToken);
            expect(decoded).toBeTruthy();
            expect(decoded?.email).toBe('user111@example.com');
        });

        it('should return null for invalid token', () => {
            const decoded = decodeJWT('invalid-token');
            expect(decoded).toBeNull();
        });
    });

    describe('getRolesFromToken', () => {
        it('should extract roles array', () => {
            const roles = getRolesFromToken(mockToken);
            expect(roles).toEqual(['Staff', 'Administrator']);
        });

        it('should handle single role as string', () => {
            const singleRolePayload = { ...samplePayload, role: 'Admin' };
            const token = createMockToken(singleRolePayload);
            const roles = getRolesFromToken(token);
            expect(roles).toEqual(['Admin']);
        });

        it('should return empty array for no roles', () => {
            const noRolePayload = { ...samplePayload, role: undefined };
            const token = createMockToken(noRolePayload);
            const roles = getRolesFromToken(token);
            expect(roles).toEqual([]);
        });
    });

    describe('hasRole', () => {
        it('should return true for existing role', () => {
            expect(hasRole(mockToken, 'Administrator')).toBe(true);
            expect(hasRole(mockToken, 'Staff')).toBe(true);
        });

        it('should be case-insensitive', () => {
            expect(hasRole(mockToken, 'administrator')).toBe(true);
            expect(hasRole(mockToken, 'STAFF')).toBe(true);
        });

        it('should return false for non-existing role', () => {
            expect(hasRole(mockToken, 'Manager')).toBe(false);
        });

        it('should handle array of roles', () => {
            expect(hasRole(mockToken, ['Manager', 'Administrator'])).toBe(true);
            expect(hasRole(mockToken, ['Manager', 'Supervisor'])).toBe(false);
        });
    });

    describe('isAdministrator', () => {
        it('should return true for Administrator role', () => {
            expect(isAdministrator(mockToken)).toBe(true);
        });

        it('should return false for non-Administrator', () => {
            const staffOnlyPayload = { ...samplePayload, role: ['Staff'] };
            const token = createMockToken(staffOnlyPayload);
            expect(isAdministrator(token)).toBe(false);
        });
    });

    describe('getPermissionsFromToken', () => {
        it('should extract permissions array', () => {
            const permissions = getPermissionsFromToken(mockToken);
            expect(permissions).toContain('product.create');
            expect(permissions).toContain('order.view_all');
            expect(permissions.length).toBe(6);
        });

        it('should return empty array for no permissions', () => {
            const noPermPayload = { ...samplePayload, permission: undefined };
            const token = createMockToken(noPermPayload);
            const permissions = getPermissionsFromToken(token);
            expect(permissions).toEqual([]);
        });
    });

    describe('hasPermission', () => {
        it('should return true for existing permission', () => {
            expect(hasPermission(mockToken, 'product.create')).toBe(true);
            expect(hasPermission(mockToken, 'order.view_all')).toBe(true);
        });

        it('should return false for non-existing permission', () => {
            expect(hasPermission(mockToken, 'product.delete')).toBe(false);
        });

        it('should handle array of permissions', () => {
            expect(hasPermission(mockToken, ['product.delete', 'product.create'])).toBe(true);
            expect(hasPermission(mockToken, ['product.delete', 'product.update'])).toBe(false);
        });
    });

    describe('isTokenExpired', () => {
        it('should return true for expired token', () => {
            const expiredPayload = { ...samplePayload, exp: Math.floor(Date.now() / 1000) - 3600 };
            const token = createMockToken(expiredPayload);
            expect(isTokenExpired(token)).toBe(true);
        });

        it('should return false for valid token', () => {
            const validPayload = { ...samplePayload, exp: Math.floor(Date.now() / 1000) + 3600 };
            const token = createMockToken(validPayload);
            expect(isTokenExpired(token)).toBe(false);
        });

        it('should return true for token without exp', () => {
            const noExpPayload = { ...samplePayload, exp: undefined };
            const token = createMockToken(noExpPayload);
            expect(isTokenExpired(token)).toBe(true);
        });
    });
});

// Manual test function (for browser console)
export function testJWTHelper() {
    console.log('🧪 Testing JWT Helper...\n');

    const mockToken = createMockToken(samplePayload);

    console.log('1. Decode Token:');
    console.log(decodeJWT(mockToken));

    console.log('\n2. Extract Roles:');
    console.log(getRolesFromToken(mockToken));

    console.log('\n3. Check Administrator:');
    console.log('Is Administrator?', isAdministrator(mockToken));

    console.log('\n4. Extract Permissions:');
    console.log(getPermissionsFromToken(mockToken));

    console.log('\n5. Check Permission:');
    console.log('Has product.create?', hasPermission(mockToken, 'product.create'));

    console.log('\n✅ All tests completed!');
}

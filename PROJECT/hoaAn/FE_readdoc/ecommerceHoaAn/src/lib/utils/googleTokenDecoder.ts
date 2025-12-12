/**
 * Google Token Decoder
 * Decode Google ID token để lấy thông tin user
 */

export interface GoogleTokenPayload {
    iss: string;
    azp: string;
    aud: string;
    sub: string;
    email: string;
    email_verified: boolean;
    at_hash: string;
    name: string;
    picture: string;
    given_name: string;
    family_name: string;
    locale: string;
    iat: number;
    exp: number;
}

/**
 * Decode JWT token (không verify signature, chỉ decode payload)
 * Lưu ý: Trong production, backend sẽ verify signature
 */
export function decodeGoogleToken(token: string): GoogleTokenPayload | null {
    try {
        const parts = token.split('.');
        if (parts.length !== 3) {
            console.error('[GoogleTokenDecoder] Invalid token format');
            return null;
        }

        const payload = parts[1];
        const decoded = JSON.parse(atob(payload));

        console.log('[GoogleTokenDecoder] ✅ Token decoded:', {
            email: decoded.email,
            name: decoded.name,
            picture: decoded.picture,
        });

        return decoded as GoogleTokenPayload;
    } catch (error) {
        console.error('[GoogleTokenDecoder] ❌ Failed to decode token:', error);
        return null;
    }
}

/**
 * Extract user info from Google token
 */
export function extractGoogleUserInfo(token: string) {
    const payload = decodeGoogleToken(token);

    if (!payload) {
        return null;
    }

    return {
        email: payload.email,
        name: payload.name,
        picture: payload.picture,
        givenName: payload.given_name,
        familyName: payload.family_name,
    };
}

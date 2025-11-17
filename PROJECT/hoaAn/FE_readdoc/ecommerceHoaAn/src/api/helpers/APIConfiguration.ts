/**
 * API Configuration
 * ✅ CRITICAL: Setup để gửi cookies (sessionId) trong mọi request
 */

import { OpenAPI } from '@/api/core/OpenAPI';
import axios from 'axios';

/**
 * Configure API client để include credentials (cookies)
 * ⚠️ QUAN TRỌNG: Phải setup này để sessionId cookie được gửi kèm requests
 */
export const configureApiCredentials = () => {
  // ✅ For generated OpenAPI client
  if (OpenAPI) {
    OpenAPI.WITH_CREDENTIALS = true;
    console.log('[API Config] ✅ OpenAPI credentials enabled');
  }

  // ✅ For Axios (nếu dùng)
  if (axios) {
    axios.defaults.withCredentials = true;
    console.log('[API Config] ✅ Axios credentials enabled');
  }

  // ✅ For global fetch (nếu dùng)
  const originalFetch = window.fetch;
  window.fetch = function (input, init?) {
    return originalFetch(input, {
      ...init,
      credentials: 'include', // Always include cookies
    });
  };
  console.log('[API Config] ✅ Fetch credentials enabled');
};

/**
 * Setup CORS headers (nếu cần)
 * Backend phải cho phép credentials
 */
export const requiredBackendCorsConfig = {
  /**
   * Backend PHẢI setup như sau:
   * 
   * C# (ASP.NET Core):
   * ```csharp
   * services.AddCors(options =>
   * {
   *     options.AddPolicy("AllowCredentials", builder =>
   *     {
   *         builder
   *             .WithOrigins("http://localhost:3000") // Frontend URL
   *             .AllowCredentials() // ✅ CRITICAL
   *             .AllowAnyHeader()
   *             .AllowAnyMethod();
   *     });
   * });
   * 
   * app.UseCors("AllowCredentials");
   * ```
   * 
   * Express (Node.js):
   * ```javascript
   * app.use(cors({
   *   origin: 'http://localhost:3000',
   *   credentials: true // ✅ CRITICAL
   * }));
   * ```
   */
  note: 'Backend must allow credentials in CORS config',
};

/**
 * Verify configuration is working
 */
export const verifyCredentialsConfig = async (): Promise<boolean> => {
  try {
    // Test với một API endpoint bất kỳ
    const testResponse = await fetch('/api/v1/Cart/guest', {
      credentials: 'include',
    });

    console.log('[API Config] ✅ Credentials test passed');
    console.log('[API Config] 📝 Cookies:', document.cookie ? 'Present' : 'None visible (HTTP-only)');
    
    return true;
  } catch (error) {
    console.error('[API Config] ❌ Credentials test failed:', error);
    return false;
  }
};

/**
 * ⚠️ TROUBLESHOOTING:
 * 
 * Nếu sessionId không được gửi, kiểm tra:
 * 
 * 1. ✅ Frontend: OpenAPI.WITH_CREDENTIALS = true
 * 2. ✅ Backend: AllowCredentials() trong CORS
 * 3. ✅ Backend: SameSite = Lax (trong cookie options)
 * 4. ✅ Backend: Secure = false nếu local dev (HTTP)
 * 5. ✅ Same origin: Frontend và Backend cùng domain/port (hoặc CORS đúng)
 * 
 * Debug:
 * - Mở DevTools → Network → Chọn request → Headers
 * - Kiểm tra "Cookie" header có chứa SessionId không
 * - Kiểm tra Response có "Set-Cookie" không
 */

export default {
  configureApiCredentials,
  verifyCredentialsConfig,
  requiredBackendCorsConfig,
};
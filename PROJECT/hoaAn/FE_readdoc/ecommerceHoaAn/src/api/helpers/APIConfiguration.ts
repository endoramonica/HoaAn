/**
 * API Configuration
 * ✅ FIXED: Setup để gửi cookies (sessionId) và JWT token trong mọi request
 */

import { OpenAPI } from '@/api/core/OpenAPI';
import axios from 'axios';

/**
 * ✅ Helper: Get token từ sessionStorage hoặc localStorage
 * ✅ FIXED: Validate token trước khi return
 */
const getAuthToken = (): string | null => {
  // Priority 1: sessionStorage (current session)
  const sessionToken = sessionStorage.getItem('authToken');
  if (sessionToken && sessionToken.trim() && sessionToken !== 'null' && sessionToken !== 'undefined') {
    return sessionToken;
  }

  // Priority 2: localStorage (remember me)
  const localToken = localStorage.getItem('authToken');
  if (localToken && localToken.trim() && localToken !== 'null' && localToken !== 'undefined') {
    return localToken;
  }

  return null;
};

/**
 * Configure API client để include credentials (cookies) và JWT token
 */
export const configureApiCredentials = () => {
  // ✅ 1. Configure OpenAPI client
  if (OpenAPI) {
    OpenAPI.WITH_CREDENTIALS = true;

    // ✅ Dynamic token resolver
    OpenAPI.TOKEN = async () => {
      const token = getAuthToken();
      if (token) {
        console.log('[API Config] 🔑 Token resolved for OpenAPI request');
      }
      return token || 'undefined';
    };

    console.log('[API Config] ✅ OpenAPI credentials + token configured');

  }

  // ✅ 2. Configure Axios (if used)
  if (axios) {
    axios.defaults.withCredentials = true;

    // Axios interceptor for token
    axios.interceptors.request.use(
      (config) => {
        const token = getAuthToken();
        // ✅ FIXED: Chỉ gửi Authorization nếu token hợp lệ
        if (token && !config.headers.Authorization) {
          config.headers.Authorization = `Bearer ${token}`;
          console.log('[API Config] 🔑 Token attached to Axios request');
        } else if (!token) {
          // ❌ Xóa Authorization header nếu không có token
          delete config.headers.Authorization;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    console.log('[API Config] ✅ Axios credentials + token configured');
  }
};

/**
 * ✅ FIXED: Override fetch để tự động gửi token
 */
const originalFetch = window.fetch;
(window as any).__originalFetch__ = originalFetch;

window.fetch = new Proxy(originalFetch, {
  apply: (target, thisArg, args) => {
    const [url, config = {}] = args;

    // Skip public APIs (không cần credentials)
    const publicApis = ['open.oapi.vn'];
    const isPublicApi = typeof url === 'string' && publicApis.some(api => url.includes(api));

    if (isPublicApi) {
      // Gọi fetch gốc mà không thêm credentials
      return target.call(thisArg, url, config);
    }

    // ✅ FIXED: Lấy token từ sessionStorage hoặc localStorage
    const token = getAuthToken();

    // Merge headers
    const headers = new Headers(config.headers || {});

    // Thêm Authorization nếu có token hợp lệ
    if (token && !headers.has('Authorization')) {
      headers.set('Authorization', `Bearer ${token}`);

      // Only log for API calls
      if (typeof url === 'string' && url.includes('/api/')) {
        console.log('[API Config] 🔑 Token added to fetch request:',
          url.substring(url.indexOf('/api/'))
        );
      }
    } else if (!token && headers.has('Authorization')) {
      // ❌ Xóa Authorization header nếu không có token
      headers.delete('Authorization');
    }

    return target.call(thisArg, url, {
      ...config,
      headers,
      credentials: 'include',
    });
  }
});

console.log('[API Config] ✅ Fetch interceptor configured');

/**
 * Setup CORS headers (nếu cần)
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
   */
  note: 'Backend must allow credentials in CORS config',
};

/**
 * Verify configuration is working
 */
export const verifyCredentialsConfig = async (): Promise<boolean> => {
  try {
    const token = getAuthToken();
    console.log('[API Config] 🔍 Verifying config...');
    console.log('[API Config] Token available:', !!token);
    console.log('[API Config] Cookies:', document.cookie ? 'Present' : 'None visible (HTTP-only)');

    // Test với một API endpoint
    const testResponse = await fetch('https://localhost:7131/api/v1/Cart', {
      credentials: 'include',
      headers: token ? { 'Authorization': `Bearer ${token}` } : {},
    });

    console.log('[API Config] ✅ Test response status:', testResponse.status);

    return testResponse.ok;
  } catch (error) {
    console.error('[API Config] ❌ Credentials test failed:', error);
    return false;
  }
};

/**
 * ⚠️ TROUBLESHOOTING:
 * 
 * Nếu vẫn 401 Unauthorized:
 * 
 * 1. ✅ Check token có trong storage không: sessionStorage.getItem('authToken')
 * 2. ✅ Check OpenAPI.TOKEN có return token không
 * 3. ✅ Check Network tab → Request Headers → Authorization: Bearer ...
 * 4. ✅ Backend CORS phải allow credentials
 * 5. ✅ Backend JWT middleware phải validate token
 */

export default {
  configureApiCredentials,
  verifyCredentialsConfig,
  requiredBackendCorsConfig,
};
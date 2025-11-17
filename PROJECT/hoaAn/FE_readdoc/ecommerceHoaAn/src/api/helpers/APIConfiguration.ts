/**
 * API Configuration
 * ✅ FIXED: Setup để gửi cookies (sessionId) và JWT token trong mọi request
 */

import { OpenAPI } from '@/api/core/OpenAPI';
import axios from 'axios';

/**
 * ✅ Helper: Get token từ sessionStorage hoặc localStorage
 */
const getAuthToken = (): string | null => {
  // Priority 1: sessionStorage (current session)
  const sessionToken = sessionStorage.getItem('authToken');
  if (sessionToken) return sessionToken;
  
  // Priority 2: localStorage (remember me)
  const localToken = localStorage.getItem('authToken');
  if (localToken) return localToken;
  
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
        if (token && !config.headers.Authorization) {
          config.headers.Authorization = `Bearer ${token}`;
          console.log('[API Config] 🔑 Token attached to Axios request');
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

window.fetch = new Proxy(originalFetch, {
  apply: (target, thisArg, args) => {
    const [url, config = {}] = args;
    
    // ✅ FIXED: Lấy token từ sessionStorage hoặc localStorage
    const token = getAuthToken();
    
    // Merge headers
    const headers = new Headers(config.headers || {});
    
    // Thêm Authorization nếu có token
    if (token && !headers.has('Authorization')) {
      headers.set('Authorization', `Bearer ${token}`);
      
      // Only log for API calls
      if (typeof url === 'string' && url.includes('/api/')) {
        console.log('[API Config] 🔑 Token added to fetch request:', 
          url.substring(url.indexOf('/api/'))
        );
      }
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
    const testResponse = await fetch('https://hbh1z72d-7131.asse.devtunnels.ms/api/v1/Cart', {
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
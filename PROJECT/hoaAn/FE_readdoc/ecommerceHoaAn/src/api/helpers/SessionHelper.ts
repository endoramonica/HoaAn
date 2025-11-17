/**
 * Session Helper
 * ✅ Helper functions để làm việc với session
 * ⚠️ CHÚ Ý: Backend tự động quản lý sessionId qua HTTP-only cookie
 * Frontend KHÔNG CẦN tự quản lý sessionId
 * 
 * File này chỉ để debug hoặc kiểm tra session state
 */

/**
 * Kiểm tra xem có sessionId cookie không
 * ⚠️ Không thể đọc được HTTP-only cookie từ JavaScript
 * Chỉ có thể kiểm tra thông qua API response headers
 */
export const hasSessionCookie = (): boolean => {
  // HTTP-only cookies KHÔNG thể đọc từ JavaScript
  // Chỉ có thể kiểm tra bằng cách gọi API
  console.warn('[Session] Cannot read HTTP-only cookie from JavaScript');
  return false;
};

/**
 * Get session info từ backend
 * Backend có thể expose endpoint để get session info
 */
export const getSessionInfo = async (): Promise<{
  hasSession: boolean;
  sessionId?: string;
}> => {
  try {
    // Nếu backend có endpoint để get session info
    // const response = await fetch('/api/v1/session/info', {
    //   credentials: 'include' // Important: Include cookies
    // });
    // return await response.json();

    console.log('[Session] Session managed by backend HTTP-only cookie');
    return { hasSession: true };
  } catch (error) {
    console.error('[Session] Error getting session info:', error);
    return { hasSession: false };
  }
};

/**
 * Debug: Log session status
 * Hữu ích cho debugging
 */
export const debugSession = () => {
  console.group('🔍 Session Debug Info');
  console.log('📌 Session managed by backend via HTTP-only cookie');
  console.log('🔒 Frontend CANNOT read sessionId (security feature)');
  console.log('✅ Backend automatically includes sessionId in requests');
  console.log('📝 All cart operations work transparently');
  console.groupEnd();
};

/**
 * Verify session is working
 * Test bằng cách gọi guest cart API
 */
export const verifySession = async (): Promise<boolean> => {
  try {
    // Import CartService để test
    const { CartService } = await import('@/api/services/CartService');
    
    // Try to get guest cart - backend sẽ tự tạo session nếu chưa có
    await CartService.getApiV1CartGuest();
    
    console.log('[Session] ✅ Session verified - backend is handling cookies');
    return true;
  } catch (error: any) {
    if (error?.status === 404) {
      // Cart trống nhưng session hoạt động
      console.log('[Session] ✅ Session verified - cart is empty');
      return true;
    }
    console.error('[Session] ❌ Session verification failed:', error);
    return false;
  }
};

/**
 * ⚠️ IMPORTANT NOTES:
 * 
 * 1. SessionId được lưu trong HTTP-only cookie
 *    → JavaScript KHÔNG THỂ đọc được (bảo mật)
 *    → Browser tự động gửi cookie trong mỗi request
 * 
 * 2. Backend tự động xử lý:
 *    - Tạo sessionId nếu chưa có
 *    - Đọc sessionId từ cookie
 *    - Liên kết cart với session
 * 
 * 3. Frontend CHỈ CẦN:
 *    - Gọi API bình thường
 *    - Đảm bảo credentials: 'include' trong fetch config
 *    - Không cần truyền sessionId thủ công
 * 
 * 4. Axios/Fetch config:
 *    ```typescript
 *    axios.defaults.withCredentials = true;
 *    // hoặc
 *    fetch(url, { credentials: 'include' });
 *    ```
 */

export default {
  hasSessionCookie,
  getSessionInfo,
  debugSession,
  verifySession,
};
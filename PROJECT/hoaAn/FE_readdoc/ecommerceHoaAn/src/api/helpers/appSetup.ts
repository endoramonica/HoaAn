/**
 * App Setup
 * ✅ Initialize tất cả cấu hình cần thiết cho app
 * 
 * Gọi trong main.tsx hoặc App.tsx:
 * ```typescript
 * import { initializeApp } from './config/appSetup';
 * initializeApp();
 * ```
 */

import { configureApiCredentials, verifyCredentialsConfig } from '@/api/helpers/APIConfiguration';
import { debugSession } from '@/api/helpers/SessionHelper';

/**
 * Initialize application
 * Setup tất cả configs cần thiết
 */
export const initializeApp = async () => {
  console.group('🚀 Application Initialization');

  try {
    // ✅ 1. Configure API credentials
    console.log('📡 Configuring API credentials...');
    configureApiCredentials();

    // ✅ 2. Verify credentials config is working
    console.log('🔍 Verifying credentials config...');
    const isConfigured = await verifyCredentialsConfig();
    
    if (!isConfigured) {
      console.warn('⚠️ Credentials config verification failed');
      console.warn('💡 Guest cart may not work properly');
    }

    // ✅ 3. Debug session info (development only)
    if (process.env.NODE_ENV === 'development') {
      debugSession();
    }

    console.log('✅ Application initialized successfully');
  } catch (error) {
    console.error('❌ Application initialization failed:', error);
  } finally {
    console.groupEnd();
  }
};

/**
 * Setup instructions cho developer
 */
export const setupInstructions = {
  frontend: {
    step1: 'Import và gọi initializeApp() trong main.tsx',
    step2: 'Đảm bảo OpenAPI.WITH_CREDENTIALS = true',
    step3: 'Sử dụng useCart() hook trong components',
    
    example: `
      // main.tsx
      import { initializeApp } from './config/appSetup';
      
      initializeApp();
      
      ReactDOM.createRoot(document.getElementById('root')!).render(
        <App />
      );
    `,
  },
  
  backend: {
    step1: 'Đảm bảo CORS cho phép credentials',
    step2: 'Cookie phải có SameSite = Lax',
    step3: 'Local dev: Secure = false, Production: Secure = true',
    
    csharpExample: `
      // Program.cs hoặc Startup.cs
      services.AddCors(options =>
      {
          options.AddPolicy("AllowCredentials", builder =>
          {
              builder
                  .WithOrigins("http://localhost:3000", "https://yourdomain.com")
                  .AllowCredentials() // ✅ CRITICAL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
          });
      });
      
      // CartService.cs - GetOrCreateSessionId()
      response.Cookies.Append("SessionId", sessionId, new CookieOptions
      {
          Expires = DateTime.UtcNow.AddDays(7),
          HttpOnly = true,
          Secure = false, // ✅ false for local dev, true for production
          SameSite = SameSiteMode.Lax // ✅ CRITICAL
      });
    `,
  },
  
  testing: {
    step1: 'Thêm sản phẩm vào cart khi chưa login (guest)',
    step2: 'Kiểm tra DevTools → Application → Cookies → SessionId',
    step3: 'Login và kiểm tra cart đã merge',
    step4: 'Kiểm tra Network tab → Request headers có Cookie',
    
    troubleshooting: {
      noCookie: 'Backend không set cookie hoặc CORS config sai',
      cookieNotSent: 'Frontend chưa config WITH_CREDENTIALS',
      cartNotMerge: 'Kiểm tra API merge có được gọi sau login',
      sessionExpired: 'Cookie đã hết hạn (7 days), tạo session mới',
    },
  },
};

/**
 * Quick test function
 * Chạy trong console để test setup
 */
export const quickTest = async () => {
  console.group('🧪 Quick Test');
  
  try {
    // Test 1: API credentials
    console.log('Test 1: API Credentials...');
    const isConfigured = await verifyCredentialsConfig();
    console.log(isConfigured ? '✅ Pass' : '❌ Fail');
    
    // Test 2: Guest cart
    console.log('Test 2: Guest Cart Access...');
    const { CartService } = await import('@/api/services/CartService');
    
    try {
      await CartService.getApiV1CartGuest();
      console.log('✅ Pass - Guest cart accessible');
    } catch (error: any) {
      if (error?.status === 404) {
        console.log('✅ Pass - Guest cart empty but accessible');
      } else {
        console.log('❌ Fail -', error.message);
      }
    }
    
    console.log('✅ Quick test completed');
  } catch (error) {
    console.error('❌ Quick test failed:', error);
  } finally {
    console.groupEnd();
  }
};

// Export for console testing
if (typeof window !== 'undefined') {
  (window as any).quickTest = quickTest;
  (window as any).setupInstructions = setupInstructions;
}

export default {
  initializeApp,
  setupInstructions,
  quickTest,
};
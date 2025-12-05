/**
 * Application-wide constants
 */

// API Configuration
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
export const API_TIMEOUT = 30000; // 30 seconds

// Authentication
export const TOKEN_STORAGE_KEY = 'auth_token';
export const REFRESH_TOKEN_STORAGE_KEY = 'refresh_token';
export const USER_STORAGE_KEY = 'user_data';

// LocalStorage Keys
export const MARKETING_POSTS_KEY = 'marketing_posts';
export const NOTIFICATIONS_KEY = 'notifications';
export const THEME_KEY = 'theme_preference';
export const LANGUAGE_KEY = 'language_preference';

// React Query Configuration
export const QUERY_STALE_TIME = 5 * 60 * 1000; // 5 minutes
export const QUERY_CACHE_TIME = 10 * 60 * 1000; // 10 minutes

// Pagination
export const DEFAULT_PAGE_SIZE = 20;
export const PAGE_SIZE_OPTIONS = [10, 20, 50, 100];

// Order Status
export const ORDER_STATUS = {
    PENDING: 'PENDING',
    PROCESSING: 'PROCESSING',
    COMPLETED: 'COMPLETED',
    CANCELLED: 'CANCELLED',
} as const;

export const ORDER_STATUS_LABELS: Record<string, string> = {
    PENDING: 'Chờ xử lý',
    PROCESSING: 'Đang xử lý',
    COMPLETED: 'Hoàn thành',
    CANCELLED: 'Đã hủy',
};

export const ORDER_STATUS_COLORS: Record<string, string> = {
    PENDING: 'yellow',
    PROCESSING: 'blue',
    COMPLETED: 'green',
    CANCELLED: 'red',
};

// Employee Roles
export const EMPLOYEE_ROLES = {
    ADMIN: 'ADMIN',
    MANAGER: 'MANAGER',
    CASHIER: 'CASHIER',
    STAFF: 'STAFF',
} as const;

export const EMPLOYEE_ROLE_LABELS: Record<string, string> = {
    ADMIN: 'Quản trị viên',
    MANAGER: 'Quản lý',
    CASHIER: 'Thu ngân',
    STAFF: 'Nhân viên',
};

// Payment Methods
export const PAYMENT_METHODS = {
    CASH: 'CASH',
    CARD: 'CARD',
    BANK_TRANSFER: 'BANK_TRANSFER',
    E_WALLET: 'E_WALLET',
} as const;

export const PAYMENT_METHOD_LABELS: Record<string, string> = {
    CASH: 'Tiền mặt',
    CARD: 'Thẻ',
    BANK_TRANSFER: 'Chuyển khoản',
    E_WALLET: 'Ví điện tử',
};

// Inventory Thresholds
export const LOW_STOCK_THRESHOLD = 10;
export const OUT_OF_STOCK_THRESHOLD = 0;

// AI Models
export const AI_MODELS = {
    GEMINI: 'gemini',
    OPENAI: 'openai',
} as const;

export const AI_MODEL_LABELS: Record<string, string> = {
    gemini: 'Google Gemini',
    openai: 'OpenAI GPT',
};

// Date Formats
export const DATE_FORMAT = 'dd/MM/yyyy';
export const DATETIME_FORMAT = 'dd/MM/yyyy HH:mm';
export const TIME_FORMAT = 'HH:mm';

// Validation Rules
export const VALIDATION = {
    MIN_PASSWORD_LENGTH: 8,
    MAX_PASSWORD_LENGTH: 128,
    MIN_PRODUCT_NAME_LENGTH: 3,
    MAX_PRODUCT_NAME_LENGTH: 200,
    MIN_PRICE: 0,
    MAX_PRICE: 999999999,
    PHONE_REGEX: /^(0|\+84)[0-9]{9}$/,
    EMAIL_REGEX: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
} as const;

// UI Constants
export const SIDEBAR_WIDTH = 256; // 16rem
export const TOPBAR_HEIGHT = 64; // 4rem
export const MOBILE_BREAKPOINT = 640; // sm breakpoint
export const TABLET_BREAKPOINT = 1024; // lg breakpoint

// Toast/Notification Duration
export const TOAST_DURATION = 3000; // 3 seconds
export const ERROR_TOAST_DURATION = 5000; // 5 seconds

// File Upload
export const MAX_FILE_SIZE = 5 * 1024 * 1024; // 5MB
export const ALLOWED_IMAGE_TYPES = ['image/jpeg', 'image/png', 'image/webp', 'image/gif'];

// Languages
export const LANGUAGES = {
    VI: 'vi',
    EN: 'en',
} as const;

export const LANGUAGE_LABELS: Record<string, string> = {
    vi: 'Tiếng Việt',
    en: 'English',
};

// Routes
export const ROUTES = {
    LOGIN: '/login',
    DASHBOARD: '/',
    POS: '/pos',
    PRODUCTS: '/products',
    CUSTOMERS: '/customers',
    ORDERS: '/orders',
    EMPLOYEES: '/employees',
    INVENTORY: '/inventory',
    MARKETING: '/marketing',
    MARKETING_CREATE: '/marketing/create',
    NOTIFICATIONS: '/notifications',
    SETTINGS: '/settings',
} as const;

// Navigation Items
export const NAVIGATION_ITEMS = [
    { label: 'Tổng quan', path: ROUTES.DASHBOARD, icon: 'LayoutDashboard' },
    { label: 'Bán hàng', path: ROUTES.POS, icon: 'ShoppingCart' },
    { label: 'Sản phẩm', path: ROUTES.PRODUCTS, icon: 'Package' },
    { label: 'Khách hàng', path: ROUTES.CUSTOMERS, icon: 'Users' },
    { label: 'Đơn hàng', path: ROUTES.ORDERS, icon: 'ShoppingBag' },
    { label: 'Nhân viên', path: ROUTES.EMPLOYEES, icon: 'UserCog' },
    { label: 'Kho hàng', path: ROUTES.INVENTORY, icon: 'Warehouse' },
    { label: 'Marketing', path: ROUTES.MARKETING, icon: 'Megaphone' },
    { label: 'Thông báo', path: ROUTES.NOTIFICATIONS, icon: 'Bell' },
    { label: 'Cài đặt', path: ROUTES.SETTINGS, icon: 'Settings' },
] as const;

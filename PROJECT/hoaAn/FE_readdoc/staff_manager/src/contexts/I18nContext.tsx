import React, { createContext, useContext, useState, useEffect } from 'react';
import { LANGUAGES } from '../utils/constants';

interface I18nContextType {
  language: string;
  setLanguage: (language: string) => void;
  t: (key: string, params?: Record<string, string>) => string;
}

const I18nContext = createContext<I18nContextType | undefined>(undefined);

export const useI18n = () => {
  const context = useContext(I18nContext);
  if (context === undefined) {
    throw new Error('useI18n must be used within an I18nProvider');
  }
  return context;
};

// Simple translation object - in a real app, this would be loaded from files
const translations = {
  [LANGUAGES.EN]: {
    // Navigation
    'nav.dashboard': 'Dashboard',
    'nav.pos': 'POS',
    'nav.orders': 'Orders',
    'nav.inventory': 'Inventory',
    'nav.staff': 'Staff',
    'nav.analytics': 'Analytics',
    'nav.tasks': 'Tasks',
    'nav.crm': 'Customers',
    'nav.lrm': 'Logistics',
    'nav.hrm': 'HR',
    'nav.settings': 'Settings',
    'nav.logout': 'Logout',
    'nav.profile': 'Profile',
    'nav.notifications': 'Notifications',
    
    // Admin Navigation
    'nav.admin': 'Admin',
    'nav.admin.users': 'Users',
    'nav.admin.settings': 'System Settings',
    'nav.admin.audit': 'Audit Logs',
    'nav.admin.monitor': 'Monitor',
    'nav.admin.backup': 'Backup',
    
    // Common
    'common.loading': 'Loading...',
    'common.save': 'Save',
    'common.cancel': 'Cancel',
    'common.delete': 'Delete',
    'common.edit': 'Edit',
    'common.view': 'View',
    'common.add': 'Add',
    'common.search': 'Search',
    'common.filter': 'Filter',
    'common.export': 'Export',
    'common.total': 'Total',
    'common.status': 'Status',
    'common.actions': 'Actions',
    'common.all': 'All',
    'common.pending': 'Pending',
    'common.completed': 'Completed',
    'common.never': 'Never',
    
    // Dashboard
    'dashboard.welcome': 'Welcome back, {{name}}!',
    'dashboard.todaysSales': 'Today\'s Sales',
    'dashboard.totalOrders': 'Total Orders',
    'dashboard.lowStock': 'Low Stock Items',
    'dashboard.activeStaff': 'Active Staff',
    
    // POS
    'pos.scanBarcode': 'Scan Barcode',
    'pos.addItem': 'Add Item',
    'pos.checkout': 'Checkout',
    'pos.payment': 'Payment',
    'pos.cash': 'Cash',
    'pos.card': 'Card',
    'pos.openShift': 'Open Shift',
    'pos.closeShift': 'Close Shift',
    
    // Orders
    'orders.myOrders': 'My Orders',
    'orders.allOrders': 'All Orders',
    'orders.orderNumber': 'Order #{{number}}',
    'orders.customer': 'Customer',
    'orders.total': 'Total',
    'orders.status': 'Status',
    
    // Auth
    'auth.login': 'Login',
    'auth.email': 'Email',
    'auth.password': 'Password',
    'auth.invalidCredentials': 'Invalid credentials',
    'auth.logoutSuccess': 'Logged out successfully',
    
    // CRM
    'crm.title': 'Customer Relationship Management',
    'crm.subtitle': 'Manage customers, leads, and interactions',
    'crm.customers': 'Customers',
    'crm.customerDetails': 'Customer Details',
    'crm.analytics': 'Analytics',
    'crm.addCustomer': 'Add Customer',
    'crm.searchCustomers': 'Search customers...',
    'crm.customer': 'Customer',
    'crm.contact': 'Contact',
    'crm.totalOrders': 'Total Orders',
    'crm.totalSpent': 'Total Spent',
    'crm.lastOrder': 'Last Order',
    'crm.lead': 'Lead',
    'crm.inactive': 'Inactive',
    'crm.interactions': 'Interactions',
    'crm.addInteraction': 'Add Interaction',
    'crm.noInteractions': 'No interactions found',
    'crm.totalCustomers': 'Total Customers',
    'crm.activeLeads': 'Active Leads',
    'crm.conversionRate': 'Conversion Rate',
    'crm.fromLastMonth': 'from last month',
    'crm.interactionAdded': 'Interaction added successfully',
    'crm.exportStarted': 'Export started',
    
    // LRM
    'lrm.title': 'Logistics & Resource Management',
    'lrm.subtitle': 'Manage shipping, transfers, and suppliers',
    'lrm.shipping': 'Shipping',
    'lrm.stockTransfers': 'Stock Transfers',
    'lrm.suppliers': 'Suppliers',
    'lrm.requestTransfer': 'Request Transfer',
    'lrm.searchShipping': 'Search shipping...',
    'lrm.searchTransfers': 'Search transfers...',
    'lrm.searchSuppliers': 'Search suppliers...',
    'lrm.orderId': 'Order ID',
    'lrm.trackingNumber': 'Tracking Number',
    'lrm.carrier': 'Carrier',
    'lrm.estimatedDelivery': 'Estimated Delivery',
    'lrm.actualDelivery': 'Actual Delivery',
    'lrm.pending': 'Pending',
    'lrm.picked': 'Picked',
    'lrm.shipped': 'Shipped',
    'lrm.delivered': 'Delivered',
    'lrm.cancelled': 'Cancelled',
    'lrm.transferId': 'Transfer ID',
    'lrm.from': 'From',
    'lrm.to': 'To',
    'lrm.items': 'Items',
    'lrm.requestedBy': 'Requested By',
    'lrm.deliveryDate': 'Delivery Date',
    'lrm.inTransit': 'In Transit',
    'lrm.supplier': 'Supplier',
    'lrm.products': 'Products',
    'lrm.paymentTerms': 'Payment Terms',
    'lrm.deliverySchedule': 'Delivery Schedule',
    'lrm.addSupplier': 'Add Supplier',
    'lrm.active': 'Active',
    'lrm.deliveryConfirmed': 'Delivery confirmed successfully',
    'lrm.transferRequested': 'Transfer requested successfully',
    'lrm.exportStarted': 'Export started',
    
    // HRM
    'hrm.title': 'Human Resource Management',
    'hrm.subtitle': 'Manage employees, schedules, and performance',
    'hrm.myProfile': 'My Profile',
    'hrm.employees': 'Employees',
    'hrm.leaveRequests': 'Leave Requests',
    'hrm.schedules': 'Schedules',
    'hrm.requestLeave': 'Request Leave',
    'hrm.searchEmployees': 'Search employees...',
    'hrm.employee': 'Employee',
    'hrm.position': 'Position',
    'hrm.department': 'Department',
    'hrm.hireDate': 'Hire Date',
    'hrm.performance': 'Performance',
    'hrm.active': 'Active',
    'hrm.inactive': 'Inactive',
    'hrm.onLeave': 'On Leave',
    'hrm.ordersProcessed': 'Orders Processed',
    'hrm.tasksCompleted': 'Tasks Completed',
    'hrm.sales': 'Sales',
    'hrm.myLeaveRequests': 'My Leave Requests',
    'hrm.type': 'Type',
    'hrm.dates': 'Dates',
    'hrm.days': 'Days',
    'hrm.submittedAt': 'Submitted At',
    'hrm.pendingLeaveRequests': 'Pending Leave Requests',
    'hrm.reason': 'Reason',
    'hrm.vacation': 'Vacation',
    'hrm.sick': 'Sick',
    'hrm.personal': 'Personal',
    'hrm.emergency': 'Emergency',
    'hrm.approved': 'Approved',
    'hrm.rejected': 'Rejected',
    'hrm.workSchedules': 'Work Schedules',
    'hrm.assignShift': 'Assign Shift',
    'hrm.scheduleManagementComingSoon': 'Schedule management coming soon',
    'hrm.leaveRequestSubmitted': 'Leave request submitted successfully',
    'hrm.leaveRequestApproved': 'Leave request approved',
    'hrm.leaveRequestRejected': 'Leave request rejected',
    'hrm.exportStarted': 'Export started',
    
    // Profile
    'profile.title': 'Profile',
    'profile.description': 'Manage your personal information and preferences',
    'profile.information': 'Personal Information',
    'profile.informationDesc': 'Update your personal details',
    'profile.name': 'Full Name',
    'profile.email': 'Email Address',
    'profile.role': 'Role',
    'profile.store': 'Store',
    'profile.security': 'Security',
    'profile.securityDesc': 'Manage your password and security settings',
    'profile.password': 'Change Password',
    'profile.passwordDesc': 'Last changed 30 days ago',
    'profile.changePassword': 'Change Password',
    'profile.preferences': 'Preferences',
    'profile.preferencesDesc': 'Customize your experience',
    'profile.theme': 'Theme',
    'profile.light': 'Light',
    'profile.dark': 'Dark',
    'profile.language': 'Language',
    'profile.accountInfo': 'Account Information',
    'profile.accountCreated': 'Account Created',
    'profile.lastLogin': 'Last Login',
    'profile.status': 'Status',
    'profile.active': 'Active',
    'profile.inactive': 'Inactive',
    
    // Notifications
    'notifications.title': 'Notifications',
    'notifications.description': 'Stay updated with your latest notifications',
    'notifications.list': 'Notification List',
    'notifications.all': 'All',
    'notifications.unread': 'Unread',
    'notifications.read': 'Read',
    'notifications.allTypes': 'All Types',
    'notifications.orders': 'Orders',
    'notifications.inventory': 'Inventory',
    'notifications.staff': 'Staff',
    'notifications.system': 'System',
    'notifications.markAllRead': 'Mark All Read',
    'notifications.deleteRead': 'Delete Read',
    'notifications.new': 'New',
    'notifications.noNotifications': 'No notifications found',
  },
  [LANGUAGES.VI]: {
    // Navigation
    'nav.dashboard': 'Bảng điều khiển',
    'nav.pos': 'Bán hàng',
    'nav.orders': 'Đơn hàng',
    'nav.inventory': 'Kho hàng',
    'nav.staff': 'Nhân viên',
    'nav.analytics': 'Phân tích',
    'nav.tasks': 'Nhiệm vụ',
    'nav.crm': 'Khách hàng',
    'nav.lrm': 'Vận chuyển',
    'nav.hrm': 'HR',
    'nav.settings': 'Cài đặt',
    'nav.logout': 'Đăng xuất',
    'nav.profile': 'Hồ sơ',
    'nav.notifications': 'Thông báo',
    
    // Admin Navigation
    'nav.admin': 'Admin',
    'nav.admin.users': 'Users',
    'nav.admin.settings': 'System Settings',
    'nav.admin.audit': 'Audit Logs',
    'nav.admin.monitor': 'Monitor',
    'nav.admin.backup': 'Backup',
    
    // Common
    'common.loading': 'Đang tải...',
    'common.save': 'Lưu',
    'common.cancel': 'Hủy',
    'common.delete': 'Xóa',
    'common.edit': 'Sửa',
    'common.view': 'Xem',
    'common.add': 'Thêm',
    'common.search': 'Tìm kiếm',
    'common.filter': 'Lọc',
    'common.export': 'Xuất',
    'common.total': 'Tổng',
    'common.status': 'Trạng thái',
    'common.actions': 'Hành động',
    'common.all': 'Tất cả',
    'common.pending': 'Chờ xử lý',
    'common.completed': 'Hoàn thành',
    'common.never': 'Chưa bao giờ',
    
    // Dashboard
    'dashboard.welcome': 'Chào mừng trở lại, {{name}}!',
    'dashboard.todaysSales': 'Doanh thu hôm nay',
    'dashboard.totalOrders': 'Tổng đơn hàng',
    'dashboard.lowStock': 'Hàng sắp hết',
    'dashboard.activeStaff': 'Nhân viên hoạt động',
    
    // POS
    'pos.scanBarcode': 'Quét mã vạch',
    'pos.addItem': 'Thêm sản phẩm',
    'pos.checkout': 'Thanh toán',
    'pos.payment': 'Thanh toán',
    'pos.cash': 'Tiền mặt',
    'pos.card': 'Thẻ',
    'pos.openShift': 'Mở ca',
    'pos.closeShift': 'Đóng ca',
    
    // Orders
    'orders.myOrders': 'Đơn hàng của tôi',
    'orders.allOrders': 'Tất cả đơn hàng',
    'orders.orderNumber': 'Đơn hàng #{{number}}',
    'orders.customer': 'Khách hàng',
    'orders.total': 'Tổng tiền',
    'orders.status': 'Trạng thái',
    
    // Auth
    'auth.login': 'Đăng nhập',
    'auth.email': 'Email',
    'auth.password': 'Mật khẩu',
    'auth.invalidCredentials': 'Thông tin đăng nhập không hợp lệ',
    'auth.logoutSuccess': 'Đăng xuất thành công',
    
    // CRM (Vietnamese)
    'crm.title': 'Quản lý quan hệ khách hàng',
    'crm.subtitle': 'Quản lý khách hàng, khách hàng tiềm năng và tương tác',
    'crm.customers': 'Khách hàng',
    'crm.customerDetails': 'Chi tiết khách hàng',
    'crm.analytics': 'Phân tích',
    'crm.addCustomer': 'Thêm khách hàng',
    'crm.searchCustomers': 'Tìm kiếm khách hàng...',
    'crm.customer': 'Khách hàng',
    'crm.contact': 'Liên hệ',
    'crm.totalOrders': 'Tổng đơn hàng',
    'crm.totalSpent': 'Tổng chi tiêu',
    'crm.lastOrder': 'Đơn hàng cuối',
    'crm.lead': 'Khách hàng tiềm năng',
    'crm.inactive': 'Không hoạt động',
    'crm.interactions': 'Tương tác',
    'crm.addInteraction': 'Thêm tương tác',
    'crm.noInteractions': 'Không tìm thấy tương tác',
    'crm.totalCustomers': 'Tổng khách hàng',
    'crm.activeLeads': 'Khách hàng tiềm năng',
    'crm.conversionRate': 'Tỷ lệ chuyển đổi',
    'crm.fromLastMonth': 'so với tháng trước',
    'crm.interactionAdded': 'Đã thêm tương tác thành công',
    'crm.exportStarted': 'Bắt đầu xuất dữ liệu',
    
    // LRM (Vietnamese)
    'lrm.title': 'Quản lý vận chuyển & tài nguyên',
    'lrm.subtitle': 'Quản lý vận chuyển, chuyển kho và nhà cung cấp',
    'lrm.shipping': 'Vận chuyển',
    'lrm.stockTransfers': 'Chuyển kho',
    'lrm.suppliers': 'Nhà cung cấp',
    'lrm.requestTransfer': 'Yêu cầu chuyển kho',
    'lrm.searchShipping': 'Tìm kiếm vận chuyển...',
    'lrm.searchTransfers': 'Tìm kiếm chuyển kho...',
    'lrm.searchSuppliers': 'Tìm kiếm nhà cung cấp...',
    'lrm.orderId': 'Mã đơn hàng',
    'lrm.trackingNumber': 'Mã vận đơn',
    'lrm.carrier': 'Đơn vị vận chuyển',
    'lrm.estimatedDelivery': 'Dự kiến giao hàng',
    'lrm.actualDelivery': 'Thực tế giao hàng',
    'lrm.pending': 'Chờ xử lý',
    'lrm.picked': 'Đã lấy hàng',
    'lrm.shipped': 'Đã giao vận',
    'lrm.delivered': 'Đã giao hàng',
    'lrm.cancelled': 'Đã hủy',
    'lrm.transferId': 'Mã chuyển kho',
    'lrm.from': 'Từ',
    'lrm.to': 'Đến',
    'lrm.items': 'Sản phẩm',
    'lrm.requestedBy': 'Người yêu cầu',
    'lrm.deliveryDate': 'Ngày giao hàng',
    'lrm.inTransit': 'Đang vận chuyển',
    'lrm.supplier': 'Nhà cung cấp',
    'lrm.products': 'Sản phẩm',
    'lrm.paymentTerms': 'Điều khoản thanh toán',
    'lrm.deliverySchedule': 'Lịch giao hàng',
    'lrm.addSupplier': 'Thêm nhà cung cấp',
    'lrm.active': 'Hoạt động',
    'lrm.deliveryConfirmed': 'Xác nhận giao hàng thành công',
    'lrm.transferRequested': 'Yêu cầu chuyển kho thành công',
    'lrm.exportStarted': 'Bắt đầu xuất dữ liệu',
    
    // HRM (Vietnamese)
    'hrm.title': 'Quản lý nhân sự',
    'hrm.subtitle': 'Quản lý nhân viên, lịch làm việc và hiệu suất',
    'hrm.myProfile': 'Hồ sơ của tôi',
    'hrm.employees': 'Nhân viên',
    'hrm.leaveRequests': 'Yêu cầu nghỉ phép',
    'hrm.schedules': 'Lịch làm việc',
    'hrm.requestLeave': 'Yêu cầu nghỉ phép',
    'hrm.searchEmployees': 'Tìm kiếm nhân viên...',
    'hrm.employee': 'Nhân viên',
    'hrm.position': 'Vị trí',
    'hrm.department': 'Phòng ban',
    'hrm.hireDate': 'Ngày tuyển dụng',
    'hrm.performance': 'Hiệu suất',
    'hrm.active': 'Hoạt động',
    'hrm.inactive': 'Không hoạt động',
    'hrm.onLeave': 'Đang nghỉ phép',
    'hrm.ordersProcessed': 'Đơn hàng xử lý',
    'hrm.tasksCompleted': 'Nhiệm vụ hoàn thành',
    'hrm.sales': 'Doanh số',
    'hrm.myLeaveRequests': 'Yêu cầu nghỉ phép của tôi',
    'hrm.type': 'Loại',
    'hrm.dates': 'Ngày',
    'hrm.days': 'Số ngày',
    'hrm.submittedAt': 'Ngày gửi',
    'hrm.pendingLeaveRequests': 'Yêu cầu nghỉ phép chờ duyệt',
    'hrm.reason': 'Lý do',
    'hrm.vacation': 'Nghỉ phép',
    'hrm.sick': 'Nghỉ ốm',
    'hrm.personal': 'Cá nhân',
    'hrm.emergency': 'Khẩn cấp',
    'hrm.approved': 'Đã duyệt',
    'hrm.rejected': 'Đã từ chối',
    'hrm.workSchedules': 'Lịch làm việc',
    'hrm.assignShift': 'Phân ca',
    'hrm.scheduleManagementComingSoon': 'Quản lý lịch làm việc sẽ sớm có',
    'hrm.leaveRequestSubmitted': 'Gửi yêu cầu nghỉ phép thành công',
    'hrm.leaveRequestApproved': 'Đã duyệt yêu cầu nghỉ phép',
    'hrm.leaveRequestRejected': 'Đã từ chối yêu cầu nghỉ phép',
    'hrm.exportStarted': 'Bắt đầu xuất dữ liệu',
    
    // Profile (Vietnamese)
    'profile.title': 'Hồ sơ',
    'profile.description': 'Quản lý thông tin cá nhân và tùy chỉnh',
    'profile.information': 'Thông tin cá nhân',
    'profile.informationDesc': 'Cập nhật thông tin chi tiết',
    'profile.name': 'Họ và tên',
    'profile.email': 'Địa chỉ Email',
    'profile.role': 'Vai trò',
    'profile.store': 'Cửa hàng',
    'profile.security': 'Bảo mật',
    'profile.securityDesc': 'Quản lý mật khẩu và bảo mật',
    'profile.password': 'Đổi mật khẩu',
    'profile.passwordDesc': 'Thay đổi lần cuối 30 ngày trước',
    'profile.changePassword': 'Đổi mật khẩu',
    'profile.preferences': 'Tùy chỉnh',
    'profile.preferencesDesc': 'Cá nhân hóa trải nghiệm',
    'profile.theme': 'Giao diện',
    'profile.light': 'Sáng',
    'profile.dark': 'Tối',
    'profile.language': 'Ngôn ngữ',
    'profile.accountInfo': 'Thông tin tài khoản',
    'profile.accountCreated': 'Tài khoản tạo',
    'profile.lastLogin': 'Đăng nhập cuối',
    'profile.status': 'Trạng thái',
    'profile.active': 'Hoạt động',
    'profile.inactive': 'Không hoạt động',
    
    // Notifications (Vietnamese)
    'notifications.title': 'Thông báo',
    'notifications.description': 'Cập nhật các thông báo mới nhất',
    'notifications.list': 'Danh sách thông báo',
    'notifications.all': 'Tất cả',
    'notifications.unread': 'Chưa đọc',
    'notifications.read': 'Đã đọc',
    'notifications.allTypes': 'Tất cả loại',
    'notifications.orders': 'Đơn hàng',
    'notifications.inventory': 'Kho hàng',
    'notifications.staff': 'Nhân viên',
    'notifications.system': 'Hệ thống',
    'notifications.markAllRead': 'Đánh dấu đã đọc tất cả',
    'notifications.deleteRead': 'Xóa đã đọc',
    'notifications.new': 'Mới',
    'notifications.noNotifications': 'Không có thông báo',
  },
};

export const I18nProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [language, setLanguageState] = useState<string>(LANGUAGES.EN);

  useEffect(() => {
    const savedLanguage = localStorage.getItem('language') || LANGUAGES.EN;
    setLanguageState(savedLanguage);
  }, []);

  const setLanguage = (newLanguage: string) => {
    setLanguageState(newLanguage);
    localStorage.setItem('language', newLanguage);
  };

  const t = (key: string, params?: Record<string, string>): string => {
    const languageTranslations = translations[language as keyof typeof translations] || translations[LANGUAGES.EN];
    let translation = languageTranslations[key as keyof typeof languageTranslations] || key;
    
    if (params) {
      Object.keys(params).forEach(param => {
        translation = translation.replace(`{{${param}}}`, params[param]);
      });
    }
    
    return translation;
  };

  return (
    <I18nContext.Provider value={{
      language,
      setLanguage,
      t
    }}>
      {children}
    </I18nContext.Provider>
  );
};
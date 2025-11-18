export interface User {
  id: string;
  email: string;
  name: string;
  role: string;
  storeId: string;
  managerId?: string; // For staff members
  avatar?: string;
  permissions: string[];
  isActive: boolean;
  createdAt: string;
  lastLogin?: string;
}

export interface Store {
  id: string;
  name: string;
  address: string;
  phone: string;
  email: string;
  currency: string;
  timezone: string;
  isActive: boolean;
}

export interface Product {
  id: string;
  name: string;
  description: string;
  barcode: string;
  price: number;
  cost: number;
  category: string;
  image?: string;
  isActive: boolean;
  stock: InventoryItem[];
}

export interface InventoryItem {
  id: string;
  productId: string;
  storeId: string;
  quantity: number;
  minStock: number;
  maxStock: number;
  lastUpdated: string;
}

export interface Order {
  id: string;
  orderNumber: string;
  customerId?: string;
  customerName?: string;
  customerPhone?: string;
  storeId: string;
  staffId: string;
  status: string;
  items: OrderItem[];
  subtotal: number;
  tax: number;
  discount: number;
  tip: number;
  total: number;
  paymentMethod: string;
  paymentStatus: string;
  notes?: string;
  createdAt: string;
  updatedAt: string;
}

export interface OrderItem {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  price: number;
  discount: number;
  total: number;
}

export interface Shift {
  id: string;
  staffId: string;
  storeId: string;
  startTime: string;
  endTime?: string;
  openingCash: number;
  closingCash?: number;
  totalSales: number;
  totalTransactions: number;
  status: 'open' | 'closed';
  notes?: string;
}

export interface Task {
  id: string;
  title: string;
  description: string;
  assignedTo: string;
  assignedBy: string;
  priority: 'low' | 'medium' | 'high';
  status: 'pending' | 'in_progress' | 'completed';
  dueDate?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Notification {
  id: string;
  userId: string;
  type: string;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
  data?: any;
}

export interface Analytics {
  revenue: {
    total: number;
    today: number;
    week: number;
    month: number;
    growth: number;
  };
  orders: {
    total: number;
    today: number;
    pending: number;
    completed: number;
  };
  products: {
    total: number;
    lowStock: number;
    topSelling: Array<{
      id: string;
      name: string;
      sales: number;
    }>;
  };
  staff: {
    total: number;
    active: number;
    performance: Array<{
      id: string;
      name: string;
      sales: number;
    }>;
  };
}

export interface RouteConfig {
  path: string;
  element: React.ComponentType;
  allowedRoles: string[];
  allowedStores?: string[];
  featureFlag?: string;
  title: string;
}

export interface PermissionWrapperProps {
  children: React.ReactNode;
  permission: string;
  fallback?: React.ReactNode;
}

// CRM Types
export interface Customer {
  id: string;
  name: string;
  email: string;
  phone: string;
  address?: string;
  company?: string;
  status: 'lead' | 'customer' | 'inactive';
  totalOrders: number;
  totalSpent: number;
  lastOrderDate?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CRMInteraction {
  id: string;
  customerId: string;
  type: 'call' | 'email' | 'meeting' | 'note';
  title: string;
  description: string;
  createdBy: string;
  createdAt: string;
  followUpDate?: string;
  status: 'pending' | 'completed';
}

// LRM Types
export interface Supplier {
  id: string;
  name: string;
  email: string;
  phone: string;
  address: string;
  products: string[];
  status: 'active' | 'inactive';
  paymentTerms: string;
  deliverySchedule: string;
  createdAt: string;
}

export interface StockTransfer {
  id: string;
  fromWarehouse: string;
  toWarehouse: string;
  items: TransferItem[];
  status: 'pending' | 'in_transit' | 'delivered' | 'cancelled';
  requestedBy: string;
  approvedBy?: string;
  createdAt: string;
  deliveryDate?: string;
}

export interface TransferItem {
  productId: string;
  productName: string;
  quantity: number;
  received?: number;
}

export interface ShippingStatus {
  orderId: string;
  status: 'pending' | 'picked' | 'shipped' | 'delivered' | 'cancelled';
  trackingNumber?: string;
  carrier?: string;
  estimatedDelivery?: string;
  actualDelivery?: string;
  updatedAt: string;
}

// HRM Types
export interface Employee {
  id: string;
  name: string;
  email: string;
  phone: string;
  position: string;
  department: string;
  hireDate: string;
  salary: number;
  status: 'active' | 'inactive' | 'on_leave';
  managerId?: string;
  avatar?: string;
  skills: string[];
  performance: PerformanceMetric[];
}

export interface PerformanceMetric {
  period: string;
  ordersProcessed: number;
  tasksCompleted: number;
  customerRating: number;
  sales: number;
}

export interface LeaveRequest {
  id: string;
  employeeId: string;
  type: 'vacation' | 'sick' | 'personal' | 'emergency';
  startDate: string;
  endDate: string;
  days: number;
  reason: string;
  status: 'pending' | 'approved' | 'rejected';
  approvedBy?: string;
  submittedAt: string;
  reviewedAt?: string;
  comments?: string;
}

export interface WorkSchedule {
  id: string;
  employeeId: string;
  date: string;
  startTime: string;
  endTime: string;
  type: 'regular' | 'overtime' | 'holiday';
  status: 'scheduled' | 'confirmed' | 'completed' | 'missed';
}

// Pagination Types
export interface PaginationParams {
  page: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  filters?: Record<string, any>;
}

export interface PaginationMeta {
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  meta: PaginationMeta;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

// Audit Log Types
export interface AuditLog {
  id: string;
  userId: string;
  userName: string;
  action: string;
  module: string;
  details: string;
  ipAddress?: string;
  timestamp: string;
}
/**
 * API Service Layer with Server-Side Pagination
 * 
 * This file provides mock API functions that simulate server-side pagination.
 * In production, these will call real C# backend endpoints.
 * 
 * All endpoints follow the pattern:
 * GET /api/{module}?page={page}&pageSize={pageSize}&sortBy={field}&sortOrder={asc|desc}&filter[key]={value}
 */

import { 
  PaginatedResponse, 
  PaginationParams,
  Order,
  Product,
  InventoryItem,
  Task,
  User,
  Customer,
  CRMInteraction,
  Supplier,
  StockTransfer,
  Employee,
  LeaveRequest,
  WorkSchedule,
  AuditLog,
  Notification,
  Shift
} from '../types';

import {
  mockOrders,
  mockProducts,
  mockInventory,
  mockTasks,
  mockUsers,
  mockCustomers,
  mockCRMInteractions,
  mockSuppliers,
  mockStockTransfers,
  mockEmployees,
  mockLeaveRequests,
  mockWorkSchedules,
  mockNotifications,
  mockShifts,
  mockInventoryEntries
} from './mockData';

// Simulate network delay
const delay = (ms: number = 300) => new Promise(resolve => setTimeout(resolve, ms));

// Generic pagination function
function paginateData<T>(
  data: T[],
  params: PaginationParams
): PaginatedResponse<T> {
  const { page, pageSize, sortBy, sortOrder = 'desc', filters = {} } = params;
  
  // Apply filters
  let filteredData = [...data];
  
  Object.keys(filters).forEach(key => {
    const filterValue = filters[key];
    if (filterValue !== undefined && filterValue !== null && filterValue !== '' && filterValue !== 'all') {
      filteredData = filteredData.filter(item => {
        const itemValue = (item as any)[key];
        
        // Handle different filter types
        if (typeof filterValue === 'string') {
          if (typeof itemValue === 'string') {
            return itemValue.toLowerCase().includes(filterValue.toLowerCase());
          }
          return String(itemValue).toLowerCase().includes(filterValue.toLowerCase());
        }
        
        return itemValue === filterValue;
      });
    }
  });
  
  // Apply sorting
  if (sortBy) {
    filteredData.sort((a, b) => {
      const aValue = (a as any)[sortBy];
      const bValue = (b as any)[sortBy];
      
      if (aValue === bValue) return 0;
      
      const comparison = aValue > bValue ? 1 : -1;
      return sortOrder === 'asc' ? comparison : -comparison;
    });
  }
  
  // Calculate pagination
  const totalItems = filteredData.length;
  const totalPages = Math.ceil(totalItems / pageSize);
  const startIndex = (page - 1) * pageSize;
  const endIndex = startIndex + pageSize;
  const paginatedData = filteredData.slice(startIndex, endIndex);
  
  return {
    data: paginatedData,
    meta: {
      currentPage: page,
      pageSize,
      totalItems,
      totalPages,
      hasNextPage: page < totalPages,
      hasPreviousPage: page > 1,
    },
  };
}

// =============================================================================
// ORDERS API
// =============================================================================

export async function fetchOrders(params: PaginationParams): Promise<PaginatedResponse<Order>> {
  await delay();
  return paginateData(mockOrders, params);
}

export async function fetchOrderById(id: string): Promise<Order | null> {
  await delay();
  return mockOrders.find(o => o.id === id) || null;
}

// =============================================================================
// PRODUCTS API
// =============================================================================

export async function fetchProducts(params: PaginationParams): Promise<PaginatedResponse<Product>> {
  await delay();
  return paginateData(mockProducts, params);
}

export async function fetchProductById(id: string): Promise<Product | null> {
  await delay();
  return mockProducts.find(p => p.id === id) || null;
}

// =============================================================================
// INVENTORY API
// =============================================================================

export async function fetchInventory(params: PaginationParams): Promise<PaginatedResponse<InventoryItem>> {
  await delay();
  return paginateData(mockInventory, params);
}

export async function fetchInventoryEntries(params: PaginationParams): Promise<PaginatedResponse<any>> {
  await delay();
  return paginateData(mockInventoryEntries, params);
}

// =============================================================================
// TASKS API
// =============================================================================

export async function fetchTasks(params: PaginationParams): Promise<PaginatedResponse<Task>> {
  await delay();
  return paginateData(mockTasks, params);
}

export async function fetchTaskById(id: string): Promise<Task | null> {
  await delay();
  return mockTasks.find(t => t.id === id) || null;
}

// =============================================================================
// STAFF/USERS API
// =============================================================================

export async function fetchUsers(params: PaginationParams): Promise<PaginatedResponse<User>> {
  await delay();
  return paginateData(mockUsers, params);
}

export async function fetchUserById(id: string): Promise<User | null> {
  await delay();
  return mockUsers.find(u => u.id === id) || null;
}

// =============================================================================
// CRM API
// =============================================================================

export async function fetchCustomers(params: PaginationParams): Promise<PaginatedResponse<Customer>> {
  await delay();
  return paginateData(mockCustomers, params);
}

export async function fetchCustomerById(id: string): Promise<Customer | null> {
  await delay();
  return mockCustomers.find(c => c.id === id) || null;
}

export async function fetchCRMInteractions(params: PaginationParams): Promise<PaginatedResponse<CRMInteraction>> {
  await delay();
  return paginateData(mockCRMInteractions, params);
}

// =============================================================================
// LRM API
// =============================================================================

export async function fetchSuppliers(params: PaginationParams): Promise<PaginatedResponse<Supplier>> {
  await delay();
  return paginateData(mockSuppliers, params);
}

export async function fetchStockTransfers(params: PaginationParams): Promise<PaginatedResponse<StockTransfer>> {
  await delay();
  return paginateData(mockStockTransfers, params);
}

// =============================================================================
// HRM API
// =============================================================================

export async function fetchEmployees(params: PaginationParams): Promise<PaginatedResponse<Employee>> {
  await delay();
  return paginateData(mockEmployees, params);
}

export async function fetchLeaveRequests(params: PaginationParams): Promise<PaginatedResponse<LeaveRequest>> {
  await delay();
  return paginateData(mockLeaveRequests, params);
}

export async function fetchWorkSchedules(params: PaginationParams): Promise<PaginatedResponse<WorkSchedule>> {
  await delay();
  return paginateData(mockWorkSchedules, params);
}

// =============================================================================
// NOTIFICATIONS API
// =============================================================================

// Generate more notifications for pagination testing
const generateNotifications = (): Notification[] => {
  const notifications: Notification[] = [];
  const types = ['order', 'inventory', 'staff', 'system'];
  const titles = {
    order: ['New Order', 'Order Completed', 'Payment Received', 'Order Cancelled'],
    inventory: ['Low Stock Alert', 'Stock Replenished', 'Inventory Adjustment', 'New Product Added'],
    staff: ['New Staff Member', 'Shift Change Request', 'Performance Review', 'Leave Request'],
    system: ['System Update', 'Maintenance Scheduled', 'Backup Completed', 'Security Alert'],
  };
  
  for (let i = 1; i <= 50; i++) {
    const type = types[Math.floor(Math.random() * types.length)] as keyof typeof titles;
    const titleOptions = titles[type];
    const title = titleOptions[Math.floor(Math.random() * titleOptions.length)];
    
    notifications.push({
      id: `notif-${i}`,
      userId: 'user-1',
      type,
      title,
      message: `This is notification message ${i}. Lorem ipsum dolor sit amet.`,
      isRead: Math.random() > 0.5,
      createdAt: new Date(Date.now() - Math.random() * 30 * 24 * 60 * 60 * 1000).toISOString(),
      data: {},
    });
  }
  
  return notifications.sort((a, b) => 
    new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
  );
};

const allNotifications = generateNotifications();

export async function fetchNotifications(params: PaginationParams): Promise<PaginatedResponse<Notification>> {
  await delay();
  return paginateData(allNotifications, params);
}

// =============================================================================
// SHIFTS API
// =============================================================================

export async function fetchShifts(params: PaginationParams): Promise<PaginatedResponse<Shift>> {
  await delay();
  return paginateData(mockShifts, params);
}

// =============================================================================
// AUDIT LOGS API
// =============================================================================

// Generate mock audit logs
const generateAuditLogs = (): AuditLog[] => {
  const logs: AuditLog[] = [];
  const actions = ['CREATE', 'UPDATE', 'DELETE', 'VIEW', 'LOGIN', 'LOGOUT'];
  const modules = ['Orders', 'Products', 'Inventory', 'Users', 'CRM', 'LRM', 'HRM', 'System'];
  const users = mockUsers;
  
  for (let i = 1; i <= 100; i++) {
    const user = users[Math.floor(Math.random() * users.length)];
    const action = actions[Math.floor(Math.random() * actions.length)];
    const module = modules[Math.floor(Math.random() * modules.length)];
    
    logs.push({
      id: `audit-${i}`,
      userId: user.id,
      userName: user.name,
      action,
      module,
      details: `${action} operation performed on ${module}`,
      ipAddress: `192.168.1.${Math.floor(Math.random() * 255)}`,
      timestamp: new Date(Date.now() - Math.random() * 30 * 24 * 60 * 60 * 1000).toISOString(),
    });
  }
  
  return logs.sort((a, b) => 
    new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
  );
};

const allAuditLogs = generateAuditLogs();

export async function fetchAuditLogs(params: PaginationParams): Promise<PaginatedResponse<AuditLog>> {
  await delay();
  return paginateData(allAuditLogs, params);
}

// =============================================================================
// ANALYTICS API
// =============================================================================

export async function fetchAnalyticsData(params: {
  startDate?: string;
  endDate?: string;
  groupBy?: 'day' | 'week' | 'month';
}): Promise<any> {
  await delay();
  
  // Mock analytics data
  return {
    revenue: {
      total: 15250.75,
      trend: [
        { date: '2024-01-01', amount: 1250 },
        { date: '2024-01-02', amount: 1450 },
        { date: '2024-01-03', amount: 1350 },
      ],
    },
    orders: {
      total: 1250,
      trend: [
        { date: '2024-01-01', count: 15 },
        { date: '2024-01-02', count: 18 },
        { date: '2024-01-03', count: 16 },
      ],
    },
  };
}

// =============================================================================
// EXPORT UTILITIES
// =============================================================================

export const api = {
  // Orders
  fetchOrders,
  fetchOrderById,
  
  // Products
  fetchProducts,
  fetchProductById,
  
  // Inventory
  fetchInventory,
  fetchInventoryEntries,
  
  // Tasks
  fetchTasks,
  fetchTaskById,
  
  // Users/Staff
  fetchUsers,
  fetchUserById,
  
  // CRM
  fetchCustomers,
  fetchCustomerById,
  fetchCRMInteractions,
  
  // LRM
  fetchSuppliers,
  fetchStockTransfers,
  
  // HRM
  fetchEmployees,
  fetchLeaveRequests,
  fetchWorkSchedules,
  
  // Notifications
  fetchNotifications,
  
  // Shifts
  fetchShifts,
  
  // Audit Logs
  fetchAuditLogs,
  
  // Analytics
  fetchAnalyticsData,
};

export default api;

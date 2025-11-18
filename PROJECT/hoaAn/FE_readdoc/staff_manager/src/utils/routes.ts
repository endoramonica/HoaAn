import { RouteConfig } from '../types';
import { ROLES, PERMISSIONS } from './constants';
import { DashboardPage } from '../pages/DashboardPage';
import { POSPage } from '../pages/POSPage';
import { OrdersPage } from '../pages/OrdersPage';
import { InventoryPage } from '../pages/InventoryPage';
import { StaffPage } from '../pages/StaffPage';
import { AnalyticsPage } from '../pages/AnalyticsPage';
import { TasksPage } from '../pages/TasksPage';
import { CRMPage } from '../pages/CRMPage';
import { LRMPage } from '../pages/LRMPage';
import { HRMPage } from '../pages/HRMPage';

export const ROUTES = {
  LOGIN: '/login',
  DASHBOARD: '/dashboard',
  POS: '/pos',
  ORDERS: '/orders',
  INVENTORY: '/inventory',
  STAFF: '/staff',
  ANALYTICS: '/analytics',
  TASKS: '/tasks',
  CRM: '/crm',
  LRM: '/lrm',
  HRM: '/hrm',
  UNAUTHORIZED: '/unauthorized',
} as const;

export const routesConfig: RouteConfig[] = [
  {
    path: '/dashboard',
    element: DashboardPage,
    allowedRoles: [ROLES.STAFF, ROLES.MANAGER],
    title: 'Dashboard',
  },
  {
    path: '/pos',
    element: POSPage,
    allowedRoles: [ROLES.STAFF, ROLES.MANAGER],
    title: 'Point of Sale',
  },
  {
    path: '/orders',
    element: OrdersPage,
    allowedRoles: [ROLES.STAFF, ROLES.MANAGER],
    title: 'Orders',
  },
  {
    path: '/inventory',
    element: InventoryPage,
    allowedRoles: [ROLES.STAFF, ROLES.MANAGER],
    title: 'Inventory',
  },
  {
    path: '/staff',
    element: StaffPage,
    allowedRoles: [ROLES.MANAGER],
    title: 'Staff Management',
  },
  {
    path: '/analytics',
    element: AnalyticsPage,
    allowedRoles: [ROLES.MANAGER],
    title: 'Analytics',
  },
  {
    path: '/tasks',
    element: TasksPage,
    allowedRoles: [ROLES.STAFF, ROLES.MANAGER],
    title: 'Tasks',
  },
  {
    path: '/crm',
    element: CRMPage,
    allowedRoles: [ROLES.STAFF, ROLES.MANAGER],
    title: 'Customer Management',
  },
  {
    path: '/lrm',
    element: LRMPage,
    allowedRoles: [ROLES.STAFF, ROLES.MANAGER],
    title: 'Logistics & Resources',
  },
  {
    path: '/hrm',
    element: HRMPage,
    allowedRoles: [ROLES.STAFF, ROLES.MANAGER],
    title: 'Human Resources',
  },
];
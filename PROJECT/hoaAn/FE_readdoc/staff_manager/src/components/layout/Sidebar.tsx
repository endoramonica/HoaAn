import React from 'react';
import { NavLink } from 'react-router-dom';
import {
  LayoutDashboard,
  ShoppingCart,
  Package,
  Users,
  BarChart3,
  Settings,
  CreditCard,
  ClipboardList,
  UserCheck,
  Truck,
  Building2,
  Shield,
  FileText,
  Activity,
  Database,
} from 'lucide-react';
import { useAuth } from '../../contexts/AuthContext';
import { useI18n } from '../../contexts/I18nContext';
import { PermissionWrapper } from '../PermissionWrapper';
import { PERMISSIONS, ROLES } from '../../utils/constants';
import { cn } from '../../components/ui/utils';
import { Separator } from '../ui/separator';

const navigationItems = [
  {
    path: '/dashboard',
    icon: LayoutDashboard,
    labelKey: 'nav.dashboard',
    permission: null,
  },
  {
    path: '/pos',
    icon: CreditCard,
    labelKey: 'nav.pos',
    permission: PERMISSIONS.ACCESS_POS,
  },
  {
    path: '/orders',
    icon: ShoppingCart,
    labelKey: 'nav.orders',
    permission: PERMISSIONS.VIEW_MY_ORDERS,
  },
  {
    path: '/inventory',
    icon: Package,
    labelKey: 'nav.inventory',
    permission: PERMISSIONS.VIEW_INVENTORY,
  },
  {
    path: '/staff',
    icon: Users,
    labelKey: 'nav.staff',
    permission: PERMISSIONS.VIEW_STAFF,
    roles: [ROLES.MANAGER],
  },
  {
    path: '/analytics',
    icon: BarChart3,
    labelKey: 'nav.analytics',
    permission: PERMISSIONS.VIEW_ANALYTICS,
    roles: [ROLES.MANAGER],
  },
  {
    path: '/tasks',
    icon: ClipboardList,
    labelKey: 'nav.tasks',
    permission: null,
  },
  {
    path: '/crm',
    icon: UserCheck,
    labelKey: 'nav.crm',
    permission: PERMISSIONS.VIEW_CRM,
  },
  {
    path: '/lrm',
    icon: Truck,
    labelKey: 'nav.lrm',
    permission: PERMISSIONS.VIEW_LRM,
  },
  {
    path: '/hrm',
    icon: Building2,
    labelKey: 'nav.hrm',
    permission: PERMISSIONS.VIEW_OWN_HRM,
  },
];

const adminNavigationItems = [
  {
    path: '/admin',
    icon: Shield,
    labelKey: 'nav.admin',
    permission: PERMISSIONS.SUPER_ADMIN,
    roles: [ROLES.ADMIN],
  },
  {
    path: '/admin/users',
    icon: Users,
    labelKey: 'nav.admin.users',
    permission: PERMISSIONS.MANAGE_USERS,
    roles: [ROLES.ADMIN],
  },
  {
    path: '/admin/settings',
    icon: Settings,
    labelKey: 'nav.admin.settings',
    permission: PERMISSIONS.SYSTEM_CONFIGURATION,
    roles: [ROLES.ADMIN],
  },
  {
    path: '/admin/audit-logs',
    icon: FileText,
    labelKey: 'nav.admin.audit',
    permission: PERMISSIONS.SUPER_ADMIN,
    roles: [ROLES.ADMIN],
  },
  {
    path: '/admin/system-monitor',
    icon: Activity,
    labelKey: 'nav.admin.monitor',
    permission: PERMISSIONS.SYSTEM_MONITOR,
    roles: [ROLES.ADMIN],
  },
  {
    path: '/admin/backup',
    icon: Database,
    labelKey: 'nav.admin.backup',
    permission: PERMISSIONS.DATABASE_BACKUP,
    roles: [ROLES.ADMIN],
  },
];

export const Sidebar: React.FC = () => {
  const { user, hasPermission } = useAuth();
  const { t } = useI18n();

  const isItemVisible = (item: any) => {
    if (item.permission && !hasPermission(item.permission)) {
      return false;
    }
    if (item.roles && user && !item.roles.includes(user.role)) {
      return false;
    }
    return true;
  };

  const hasAdminAccess = user?.role === ROLES.ADMIN;

  return (
    <div className="flex flex-col h-full bg-sidebar border-r border-sidebar-border">
      <div className="p-6">
        <h2 className="text-xl font-semibold text-sidebar-foreground">Store POS</h2>
      </div>
      
      <nav className="flex-1 px-4 space-y-1 overflow-y-auto">
        {navigationItems.map((item) => {
          if (!isItemVisible(item)) return null;

          return (
            <NavLink
              key={item.path}
              to={item.path}
              className={({ isActive }) =>
                cn(
                  'flex items-center px-3 py-2 text-sm rounded-md transition-colors',
                  isActive
                    ? 'bg-sidebar-accent text-sidebar-accent-foreground'
                    : 'text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground'
                )
              }
            >
              <item.icon className="w-5 h-5 mr-3" />
              {t(item.labelKey)}
            </NavLink>
          );
        })}

        {/* Admin Section */}
        {hasAdminAccess && (
          <>
            <Separator className="my-4" />
            <div className="px-3 py-2">
              <p className="text-xs uppercase text-muted-foreground">Administration</p>
            </div>
            {adminNavigationItems.map((item) => {
              if (!isItemVisible(item)) return null;

              return (
                <NavLink
                  key={item.path}
                  to={item.path}
                  className={({ isActive }) =>
                    cn(
                      'flex items-center px-3 py-2 text-sm rounded-md transition-colors',
                      isActive
                        ? 'bg-sidebar-accent text-sidebar-accent-foreground'
                        : 'text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground'
                    )
                  }
                >
                  <item.icon className="w-5 h-5 mr-3" />
                  {t(item.labelKey)}
                </NavLink>
              );
            })}
          </>
        )}
      </nav>
    </div>
  );
};
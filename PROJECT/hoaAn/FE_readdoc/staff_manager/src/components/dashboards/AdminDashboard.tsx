import React from 'react';
import { Link } from 'react-router-dom';
import {
  Users,
  Shield,
  Settings,
  FileText,
  Activity,
  Database,
  Building2,
  BarChart3,
  Lock,
  Bell,
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { useI18n } from '../../contexts/I18nContext';

export const AdminDashboard: React.FC = () => {
  const { t } = useI18n();

  const adminModules = [
    {
      title: 'User Management',
      description: 'Manage all system users, roles, and permissions',
      icon: Users,
      path: '/admin/users',
      color: 'text-blue-500',
      bgColor: 'bg-blue-50 dark:bg-blue-950',
      stats: '48 users',
    },
    {
      title: 'Role & Permissions',
      description: 'Configure roles and granular permission controls',
      icon: Shield,
      path: '/admin/roles',
      color: 'text-purple-500',
      bgColor: 'bg-purple-50 dark:bg-purple-950',
      stats: '3 roles',
    },
    {
      title: 'System Settings',
      description: 'Configure global system settings and preferences',
      icon: Settings,
      path: '/admin/settings',
      color: 'text-green-500',
      bgColor: 'bg-green-50 dark:bg-green-950',
      stats: '12 settings',
    },
    {
      title: 'Audit Logs',
      description: 'View detailed system activity and user actions',
      icon: FileText,
      path: '/admin/audit-logs',
      color: 'text-orange-500',
      bgColor: 'bg-orange-50 dark:bg-orange-950',
      stats: '1,234 logs',
    },
    {
      title: 'System Monitor',
      description: 'Monitor system health, performance, and metrics',
      icon: Activity,
      path: '/admin/system-monitor',
      color: 'text-red-500',
      bgColor: 'bg-red-50 dark:bg-red-950',
      stats: 'Healthy',
    },
    {
      title: 'Database Backup',
      description: 'Manage database backups and restoration',
      icon: Database,
      path: '/admin/backup',
      color: 'text-cyan-500',
      bgColor: 'bg-cyan-50 dark:bg-cyan-950',
      stats: 'Last: Today',
    },
    {
      title: 'Store Management',
      description: 'Manage all stores, locations, and configurations',
      icon: Building2,
      path: '/admin/stores',
      color: 'text-indigo-500',
      bgColor: 'bg-indigo-50 dark:bg-indigo-950',
      stats: '2 stores',
    },
    {
      title: 'Global Analytics',
      description: 'View comprehensive analytics across all stores',
      icon: BarChart3,
      path: '/admin/analytics',
      color: 'text-pink-500',
      bgColor: 'bg-pink-50 dark:bg-pink-950',
      stats: '$125K revenue',
    },
    {
      title: 'Security Center',
      description: 'Manage security policies and access controls',
      icon: Lock,
      path: '/admin/security',
      color: 'text-yellow-500',
      bgColor: 'bg-yellow-50 dark:bg-yellow-950',
      stats: '0 threats',
    },
    {
      title: 'Notifications',
      description: 'Send system-wide notifications and alerts',
      icon: Bell,
      path: '/admin/notifications',
      color: 'text-teal-500',
      bgColor: 'bg-teal-50 dark:bg-teal-950',
      stats: '5 pending',
    },
  ];

  const systemHealth = [
    { label: 'System Status', value: 'Operational', status: 'success' },
    { label: 'Database', value: 'Connected', status: 'success' },
    { label: 'API Response Time', value: '45ms', status: 'success' },
    { label: 'Active Sessions', value: '128', status: 'info' },
    { label: 'CPU Usage', value: '32%', status: 'success' },
    { label: 'Memory Usage', value: '68%', status: 'warning' },
  ];

  const recentActivity = [
    { action: 'New user created', user: 'admin@store.com', time: '5 minutes ago' },
    { action: 'System backup completed', user: 'System', time: '1 hour ago' },
    { action: 'Permission updated', user: 'admin@store.com', time: '2 hours ago' },
    { action: 'New store added', user: 'admin@store.com', time: '3 hours ago' },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1>System Administration</h1>
        <p className="text-muted-foreground">
          Manage and configure the entire POS system
        </p>
      </div>

      {/* System Health Overview */}
      <Card>
        <CardHeader>
          <CardTitle>System Health</CardTitle>
          <CardDescription>Real-time system status and performance metrics</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
            {systemHealth.map((item) => (
              <div key={item.label} className="space-y-2">
                <p className="text-sm text-muted-foreground">{item.label}</p>
                <div className="flex items-center gap-2">
                  <p className="font-medium">{item.value}</p>
                  <Badge
                    variant={
                      item.status === 'success'
                        ? 'default'
                        : item.status === 'warning'
                        ? 'secondary'
                        : 'outline'
                    }
                    className="h-5 px-1.5"
                  >
                    {item.status === 'success' ? '✓' : item.status === 'warning' ? '!' : 'i'}
                  </Badge>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Admin Modules Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {adminModules.map((module) => {
          const Icon = module.icon;
          return (
            <Card key={module.path} className="hover:shadow-lg transition-shadow">
              <CardHeader>
                <div className="flex items-start justify-between">
                  <div className={`p-3 rounded-lg ${module.bgColor}`}>
                    <Icon className={`h-6 w-6 ${module.color}`} />
                  </div>
                  <Badge variant="secondary">{module.stats}</Badge>
                </div>
                <CardTitle className="mt-4">{module.title}</CardTitle>
                <CardDescription>{module.description}</CardDescription>
              </CardHeader>
              <CardContent>
                <Link to={module.path}>
                  <Button className="w-full" variant="outline">
                    Access Module
                  </Button>
                </Link>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Recent Activity */}
      <Card>
        <CardHeader>
          <CardTitle>Recent Activity</CardTitle>
          <CardDescription>Latest administrative actions and system events</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {recentActivity.map((activity, index) => (
              <div
                key={index}
                className="flex items-center justify-between py-3 border-b last:border-0"
              >
                <div>
                  <p className="font-medium">{activity.action}</p>
                  <p className="text-sm text-muted-foreground">{activity.user}</p>
                </div>
                <p className="text-sm text-muted-foreground">{activity.time}</p>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
};
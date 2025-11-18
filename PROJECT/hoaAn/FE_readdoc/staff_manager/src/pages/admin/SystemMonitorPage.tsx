import React, { useState } from 'react';
import {
  Activity,
  Cpu,
  HardDrive,
  Server,
  Zap,
  Users,
  Database,
  Globe,
  TrendingUp,
  TrendingDown,
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../../components/ui/card';
import { Badge } from '../../components/ui/badge';
import { Progress } from '../../components/ui/progress';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../../components/ui/breadcrumb';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../../components/ui/tabs';

interface SystemMetric {
  label: string;
  value: string;
  percentage: number;
  status: 'success' | 'warning' | 'danger';
  icon: any;
  trend?: 'up' | 'down';
  trendValue?: string;
}

export const SystemMonitorPage: React.FC = () => {
  const systemMetrics: SystemMetric[] = [
    {
      label: 'CPU Usage',
      value: '32%',
      percentage: 32,
      status: 'success',
      icon: Cpu,
      trend: 'down',
      trendValue: '5%',
    },
    {
      label: 'Memory Usage',
      value: '68%',
      percentage: 68,
      status: 'warning',
      icon: HardDrive,
      trend: 'up',
      trendValue: '8%',
    },
    {
      label: 'Disk Usage',
      value: '45%',
      percentage: 45,
      status: 'success',
      icon: Database,
      trend: 'up',
      trendValue: '2%',
    },
    {
      label: 'Network',
      value: '125 Mbps',
      percentage: 62,
      status: 'success',
      icon: Globe,
    },
  ];

  const activeServices = [
    { name: 'Web Server', status: 'running', uptime: '45d 12h' },
    { name: 'Database', status: 'running', uptime: '45d 12h' },
    { name: 'Cache Server', status: 'running', uptime: '45d 12h' },
    { name: 'API Gateway', status: 'running', uptime: '45d 12h' },
    { name: 'Message Queue', status: 'running', uptime: '45d 12h' },
    { name: 'Background Jobs', status: 'running', uptime: '45d 12h' },
  ];

  const performanceData = [
    { time: '00:00', requests: 1200, response: 45 },
    { time: '04:00', requests: 800, response: 38 },
    { time: '08:00', requests: 2400, response: 52 },
    { time: '12:00', requests: 3200, response: 58 },
    { time: '16:00', requests: 2800, response: 48 },
    { time: '20:00', requests: 1800, response: 42 },
  ];

  const recentErrors = [
    {
      id: 1,
      timestamp: '2024-01-20 10:30:00',
      level: 'error',
      message: 'Database connection timeout',
      service: 'API',
    },
    {
      id: 2,
      timestamp: '2024-01-20 10:25:00',
      level: 'warning',
      message: 'High memory usage detected',
      service: 'Web Server',
    },
    {
      id: 3,
      timestamp: '2024-01-20 10:15:00',
      level: 'warning',
      message: 'Slow query detected (>5s)',
      service: 'Database',
    },
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'success':
        return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300';
      case 'warning':
        return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300';
      case 'danger':
        return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300';
      default:
        return 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300';
    }
  };

  const getLevelColor = (level: string) => {
    switch (level) {
      case 'error':
        return 'destructive';
      case 'warning':
        return 'secondary';
      default:
        return 'outline';
    }
  };

  return (
    <div className="space-y-6">
      {/* Breadcrumb */}
      <Breadcrumb>
        <BreadcrumbList>
          <BreadcrumbItem>
            <BreadcrumbLink href="/dashboard">Dashboard</BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbSeparator />
          <BreadcrumbItem>
            <BreadcrumbLink href="/admin">Admin</BreadcrumbLink>
          </BreadcrumbItem>
          <BreadcrumbSeparator />
          <BreadcrumbItem>
            <BreadcrumbPage>System Monitor</BreadcrumbPage>
          </BreadcrumbItem>
        </BreadcrumbList>
      </Breadcrumb>

      {/* Header */}
      <div>
        <h1>System Monitor</h1>
        <p className="text-muted-foreground">
          Real-time system health and performance monitoring
        </p>
      </div>

      {/* System Health */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {systemMetrics.map((metric) => {
          const Icon = metric.icon;
          const TrendIcon = metric.trend === 'up' ? TrendingUp : TrendingDown;
          return (
            <Card key={metric.label}>
              <CardHeader className="pb-2">
                <div className="flex items-center justify-between">
                  <div className={`p-2 rounded-lg ${getStatusColor(metric.status)}`}>
                    <Icon className="h-4 w-4" />
                  </div>
                  {metric.trend && (
                    <div className="flex items-center gap-1 text-xs text-muted-foreground">
                      <TrendIcon className="h-3 w-3" />
                      {metric.trendValue}
                    </div>
                  )}
                </div>
              </CardHeader>
              <CardContent className="space-y-2">
                <div className="flex items-center justify-between">
                  <p className="text-sm text-muted-foreground">{metric.label}</p>
                  <p className="text-xl">{metric.value}</p>
                </div>
                <Progress value={metric.percentage} className="h-2" />
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Tabs */}
      <Tabs defaultValue="services" className="space-y-4">
        <TabsList>
          <TabsTrigger value="services">Services</TabsTrigger>
          <TabsTrigger value="performance">Performance</TabsTrigger>
          <TabsTrigger value="errors">Errors & Logs</TabsTrigger>
        </TabsList>

        {/* Services Tab */}
        <TabsContent value="services" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle>Active Services</CardTitle>
              <CardDescription>All system services and their status</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {activeServices.map((service) => (
                  <div
                    key={service.name}
                    className="flex items-center justify-between py-3 border-b last:border-0"
                  >
                    <div className="flex items-center gap-4">
                      <div className="h-2 w-2 rounded-full bg-green-500 animate-pulse" />
                      <div>
                        <p className="font-medium">{service.name}</p>
                        <p className="text-sm text-muted-foreground">
                          Uptime: {service.uptime}
                        </p>
                      </div>
                    </div>
                    <Badge variant="default">Running</Badge>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Performance Tab */}
        <TabsContent value="performance" className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm">Avg Response Time</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-2xl">45ms</p>
                <p className="text-xs text-muted-foreground">Last 24 hours</p>
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm">Total Requests</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-2xl">32.5K</p>
                <p className="text-xs text-muted-foreground">Last 24 hours</p>
              </CardContent>
            </Card>
            <Card>
              <CardHeader className="pb-2">
                <CardTitle className="text-sm">Error Rate</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-2xl">0.12%</p>
                <p className="text-xs text-muted-foreground">Last 24 hours</p>
              </CardContent>
            </Card>
          </div>

          <Card>
            <CardHeader>
              <CardTitle>Performance Metrics</CardTitle>
              <CardDescription>Request volume and response times over time</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {performanceData.map((data) => (
                  <div key={data.time} className="space-y-2">
                    <div className="flex items-center justify-between text-sm">
                      <span className="text-muted-foreground">{data.time}</span>
                      <div className="flex items-center gap-4">
                        <span>{data.requests} requests</span>
                        <span className="text-muted-foreground">{data.response}ms</span>
                      </div>
                    </div>
                    <Progress value={(data.requests / 3200) * 100} className="h-2" />
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Errors Tab */}
        <TabsContent value="errors" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle>Recent Errors & Warnings</CardTitle>
              <CardDescription>System logs and error messages</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {recentErrors.map((error) => (
                  <div
                    key={error.id}
                    className="flex items-start gap-4 py-3 border-b last:border-0"
                  >
                    <Badge variant={getLevelColor(error.level) as any}>
                      {error.level}
                    </Badge>
                    <div className="flex-1 space-y-1">
                      <p className="font-medium">{error.message}</p>
                      <div className="flex items-center gap-4 text-sm text-muted-foreground">
                        <span>{error.service}</span>
                        <span>•</span>
                        <span>{error.timestamp}</span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
};

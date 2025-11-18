import React, { useState } from 'react';
import {
  FileText,
  Search,
  Filter,
  Download,
  Calendar,
  User,
  Activity,
  AlertCircle,
  CheckCircle,
  XCircle,
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { Input } from '../../components/ui/input';
import { Badge } from '../../components/ui/badge';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../../components/ui/select';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../../components/ui/table';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from '../../components/ui/breadcrumb';
import { toast } from 'sonner';
import { PaginationCustom } from '../../components/ui/pagination-custom';
import { usePagination } from '../../hooks/usePagination';

interface AuditLog {
  id: string;
  timestamp: string;
  user: string;
  userId: string;
  action: string;
  resource: string;
  details: string;
  ipAddress: string;
  status: 'success' | 'failed' | 'warning';
}

const mockAuditLogs: AuditLog[] = [
  {
    id: 'log-1',
    timestamp: '2024-01-20T10:30:00Z',
    user: 'System Admin',
    userId: 'user-admin',
    action: 'USER_CREATED',
    resource: 'User Management',
    details: 'Created new user: John Doe (john@example.com)',
    ipAddress: '192.168.1.100',
    status: 'success',
  },
  {
    id: 'log-2',
    timestamp: '2024-01-20T10:25:00Z',
    user: 'System Admin',
    userId: 'user-admin',
    action: 'SETTINGS_UPDATED',
    resource: 'System Settings',
    details: 'Updated email configuration',
    ipAddress: '192.168.1.100',
    status: 'success',
  },
  {
    id: 'log-3',
    timestamp: '2024-01-20T10:20:00Z',
    user: 'Jane Manager',
    userId: 'user-2',
    action: 'LOGIN_FAILED',
    resource: 'Authentication',
    details: 'Failed login attempt - invalid password',
    ipAddress: '192.168.1.105',
    status: 'failed',
  },
  {
    id: 'log-4',
    timestamp: '2024-01-20T10:15:00Z',
    user: 'System Admin',
    userId: 'user-admin',
    action: 'ROLE_UPDATED',
    resource: 'Role Management',
    details: 'Updated permissions for Manager role',
    ipAddress: '192.168.1.100',
    status: 'success',
  },
  {
    id: 'log-5',
    timestamp: '2024-01-20T10:10:00Z',
    user: 'System Admin',
    userId: 'user-admin',
    action: 'DATABASE_BACKUP',
    resource: 'System',
    details: 'Manual database backup initiated',
    ipAddress: '192.168.1.100',
    status: 'success',
  },
  {
    id: 'log-6',
    timestamp: '2024-01-20T10:05:00Z',
    user: 'System Admin',
    userId: 'user-admin',
    action: 'USER_DELETED',
    resource: 'User Management',
    details: 'Deleted user: test@example.com',
    ipAddress: '192.168.1.100',
    status: 'warning',
  },
  {
    id: 'log-7',
    timestamp: '2024-01-20T10:00:00Z',
    user: 'John Staff',
    userId: 'user-1',
    action: 'PASSWORD_CHANGED',
    resource: 'User Account',
    details: 'Password changed successfully',
    ipAddress: '192.168.1.110',
    status: 'success',
  },
  {
    id: 'log-8',
    timestamp: '2024-01-20T09:55:00Z',
    user: 'System Admin',
    userId: 'user-admin',
    action: 'PERMISSION_GRANTED',
    resource: 'Access Control',
    details: 'Granted admin.manage_users permission to user-2',
    ipAddress: '192.168.1.100',
    status: 'success',
  },
];

export const AuditLogsPage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [actionFilter, setActionFilter] = useState<string>('all');
  const [statusFilter, setStatusFilter] = useState<string>('all');

  const filteredLogs = mockAuditLogs.filter((log) => {
    const matchesSearch =
      log.user.toLowerCase().includes(searchTerm.toLowerCase()) ||
      log.action.toLowerCase().includes(searchTerm.toLowerCase()) ||
      log.details.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesAction = actionFilter === 'all' || log.action.includes(actionFilter);
    const matchesStatus = statusFilter === 'all' || log.status === statusFilter;
    return matchesSearch && matchesAction && matchesStatus;
  });

  const { 
    currentPage, 
    totalPages, 
    itemsPerPage, 
    totalItems, 
    paginatedData, 
    goToPage, 
    setItemsPerPage 
  } = usePagination({ data: filteredLogs, initialItemsPerPage: 10 });

  const handleExport = () => {
    toast.success('Audit logs exported successfully');
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'success':
        return <CheckCircle className="h-4 w-4 text-green-500" />;
      case 'failed':
        return <XCircle className="h-4 w-4 text-red-500" />;
      case 'warning':
        return <AlertCircle className="h-4 w-4 text-yellow-500" />;
      default:
        return null;
    }
  };

  const getStatusBadgeVariant = (status: string) => {
    switch (status) {
      case 'success':
        return 'default';
      case 'failed':
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
            <BreadcrumbPage>Audit Logs</BreadcrumbPage>
          </BreadcrumbItem>
        </BreadcrumbList>
      </Breadcrumb>

      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1>Audit Logs</h1>
          <p className="text-muted-foreground">
            View detailed system activity and user actions
          </p>
        </div>
        <Button onClick={handleExport}>
          <Download className="h-4 w-4 mr-2" />
          Export Logs
        </Button>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm">Total Logs</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-2xl">{mockAuditLogs.length}</p>
            <p className="text-xs text-muted-foreground">Last 24 hours</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm">Successful</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-2xl text-green-500">
              {mockAuditLogs.filter((l) => l.status === 'success').length}
            </p>
            <p className="text-xs text-muted-foreground">Actions completed</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm">Failed</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-2xl text-red-500">
              {mockAuditLogs.filter((l) => l.status === 'failed').length}
            </p>
            <p className="text-xs text-muted-foreground">Action failures</p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm">Warnings</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="text-2xl text-yellow-500">
              {mockAuditLogs.filter((l) => l.status === 'warning').length}
            </p>
            <p className="text-xs text-muted-foreground">Requires attention</p>
          </CardContent>
        </Card>
      </div>

      {/* Filters */}
      <Card>
        <CardContent className="pt-6">
          <div className="flex flex-col md:flex-row gap-4">
            <div className="flex-1 relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                placeholder="Search logs by user, action, or details..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10"
              />
            </div>
            <Select value={actionFilter} onValueChange={setActionFilter}>
              <SelectTrigger className="w-full md:w-[200px]">
                <Activity className="h-4 w-4 mr-2" />
                <SelectValue placeholder="Action" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Actions</SelectItem>
                <SelectItem value="USER">User Actions</SelectItem>
                <SelectItem value="SETTINGS">Settings</SelectItem>
                <SelectItem value="LOGIN">Login</SelectItem>
                <SelectItem value="DATABASE">Database</SelectItem>
                <SelectItem value="ROLE">Roles</SelectItem>
              </SelectContent>
            </Select>
            <Select value={statusFilter} onValueChange={setStatusFilter}>
              <SelectTrigger className="w-full md:w-[180px]">
                <Filter className="h-4 w-4 mr-2" />
                <SelectValue placeholder="Status" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Status</SelectItem>
                <SelectItem value="success">Success</SelectItem>
                <SelectItem value="failed">Failed</SelectItem>
                <SelectItem value="warning">Warning</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </CardContent>
      </Card>

      {/* Logs Table */}
      <Card>
        <CardHeader>
          <CardTitle>Activity Log ({filteredLogs.length})</CardTitle>
          <CardDescription>Detailed audit trail of system activities</CardDescription>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Status</TableHead>
                <TableHead>Timestamp</TableHead>
                <TableHead>User</TableHead>
                <TableHead>Action</TableHead>
                <TableHead>Resource</TableHead>
                <TableHead>Details</TableHead>
                <TableHead>IP Address</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {paginatedData.map((log) => (
                <TableRow key={log.id}>
                  <TableCell>
                    <div className="flex items-center gap-2">
                      {getStatusIcon(log.status)}
                      <Badge variant={getStatusBadgeVariant(log.status)}>
                        {log.status}
                      </Badge>
                    </div>
                  </TableCell>
                  <TableCell className="text-sm">
                    {new Date(log.timestamp).toLocaleString()}
                  </TableCell>
                  <TableCell>
                    <div>
                      <p className="font-medium">{log.user}</p>
                      <p className="text-xs text-muted-foreground">{log.userId}</p>
                    </div>
                  </TableCell>
                  <TableCell>
                    <Badge variant="outline">{log.action}</Badge>
                  </TableCell>
                  <TableCell className="text-sm">{log.resource}</TableCell>
                  <TableCell className="text-sm max-w-xs truncate">{log.details}</TableCell>
                  <TableCell className="text-sm text-muted-foreground">
                    {log.ipAddress}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="mt-4">
              <PaginationCustom
                currentPage={currentPage}
                totalPages={totalPages}
                itemsPerPage={itemsPerPage}
                totalItems={totalItems}
                onPageChange={goToPage}
                onItemsPerPageChange={setItemsPerPage}
              />
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
};
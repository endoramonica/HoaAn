import React, { useState, useMemo } from 'react';
import { Search, Plus, User, Calendar, Clock, TrendingUp, Filter, Download, Eye, Check, X } from 'lucide-react';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { Avatar, AvatarFallback } from '../components/ui/avatar';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../components/ui/tabs';
import { Progress } from '../components/ui/progress';
import { PermissionWrapper } from '../components/PermissionWrapper';
import { useAuth } from '../contexts/AuthContext';
import { useI18n } from '../contexts/I18nContext';
import { mockData } from '../services/mockData';
import { Employee, LeaveRequest, WorkSchedule } from '../types';
import { PERMISSIONS } from '../utils/constants';
import { format } from 'date-fns';
import { toast } from 'sonner';

export function HRMPage() {
  const { user } = useAuth();
  const { t } = useI18n();
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [activeTab, setActiveTab] = useState(user?.role === 'staff' ? 'profile' : 'employees');
  const [selectedEmployee, setSelectedEmployee] = useState<Employee | null>(null);

  // Mock data - in real app this would come from API
  const employees: Employee[] = mockData.employees || [];
  const leaveRequests: LeaveRequest[] = mockData.leaveRequests || [];
  const workSchedules: WorkSchedule[] = mockData.workSchedules || [];

  const currentEmployee = useMemo(() => {
    return employees.find(emp => emp.email === user?.email);
  }, [employees, user]);

  const myLeaveRequests = useMemo(() => {
    if (!currentEmployee) return [];
    return leaveRequests.filter(req => req.employeeId === currentEmployee.id);
  }, [leaveRequests, currentEmployee]);

  const mySchedule = useMemo(() => {
    if (!currentEmployee) return [];
    return workSchedules.filter(schedule => schedule.employeeId === currentEmployee.id);
  }, [workSchedules, currentEmployee]);

  const filteredEmployees = useMemo(() => {
    return employees.filter(employee => {
      const matchesSearch = employee.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                           employee.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
                           employee.position.toLowerCase().includes(searchTerm.toLowerCase());
      const matchesStatus = statusFilter === 'all' || employee.status === statusFilter;
      return matchesSearch && matchesStatus;
    });
  }, [employees, searchTerm, statusFilter]);

  const pendingLeaveRequests = useMemo(() => {
    return leaveRequests.filter(req => req.status === 'pending');
  }, [leaveRequests]);

  const handleSubmitLeaveRequest = () => {
    toast.success(t('hrm.leaveRequestSubmitted'));
  };

  const handleApproveLeave = (requestId: string) => {
    toast.success(t('hrm.leaveRequestApproved'));
  };

  const handleRejectLeave = (requestId: string) => {
    toast.success(t('hrm.leaveRequestRejected'));
  };

  const handleExportData = () => {
    toast.success(t('hrm.exportStarted'));
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'active': return 'bg-green-100 text-green-800 border-green-200';
      case 'inactive': return 'bg-gray-100 text-gray-800 border-gray-200';
      case 'on_leave': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'pending': return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'approved': return 'bg-green-100 text-green-800 border-green-200';
      case 'rejected': return 'bg-red-100 text-red-800 border-red-200';
      default: return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getLeaveTypeColor = (type: string) => {
    switch (type) {
      case 'vacation': return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'sick': return 'bg-red-100 text-red-800 border-red-200';
      case 'personal': return 'bg-purple-100 text-purple-800 border-purple-200';
      case 'emergency': return 'bg-orange-100 text-orange-800 border-orange-200';
      default: return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  return (
    <div className="p-6 space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1>{t('hrm.title')}</h1>
          <p className="text-muted-foreground">{t('hrm.subtitle')}</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={handleExportData}>
            <Download className="h-4 w-4 mr-2" />
            {t('common.export')}
          </Button>
          {user?.role === 'staff' && (
            <Button onClick={handleSubmitLeaveRequest}>
              <Plus className="h-4 w-4 mr-2" />
              {t('hrm.requestLeave')}
            </Button>
          )}
        </div>
      </div>

      <Tabs value={activeTab} onValueChange={setActiveTab}>
        <TabsList>
          {user?.role === 'staff' && (
            <TabsTrigger value="profile">{t('hrm.myProfile')}</TabsTrigger>
          )}
          <PermissionWrapper permission={PERMISSIONS.VIEW_ALL_HRM}>
            <TabsTrigger value="employees">{t('hrm.employees')}</TabsTrigger>
          </PermissionWrapper>
          <PermissionWrapper permission={PERMISSIONS.APPROVE_LEAVE_REQUESTS}>
            <TabsTrigger value="leave-requests">
              {t('hrm.leaveRequests')}
              {pendingLeaveRequests.length > 0 && (
                <Badge className="ml-2 bg-red-500 text-white">
                  {pendingLeaveRequests.length}
                </Badge>
              )}
            </TabsTrigger>
          </PermissionWrapper>
          <PermissionWrapper permission={PERMISSIONS.ASSIGN_SHIFTS}>
            <TabsTrigger value="schedules">{t('hrm.schedules')}</TabsTrigger>
          </PermissionWrapper>
        </TabsList>

        {user?.role === 'staff' && (
          <TabsContent value="profile" className="space-y-4">
            {currentEmployee && (
              <>
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-3">
                      <Avatar className="h-12 w-12">
                        <AvatarFallback>
                          {currentEmployee.name.split(' ').map(n => n[0]).join('')}
                        </AvatarFallback>
                      </Avatar>
                      <div>
                        <div>{currentEmployee.name}</div>
                        <div className="text-sm text-muted-foreground">{currentEmployee.position}</div>
                      </div>
                    </CardTitle>
                  </CardHeader>
                  <CardContent className="space-y-6">
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                      <Card>
                        <CardContent className="p-4">
                          <div className="text-sm text-muted-foreground">{t('hrm.department')}</div>
                          <div className="text-lg">{currentEmployee.department}</div>
                        </CardContent>
                      </Card>
                      <Card>
                        <CardContent className="p-4">
                          <div className="text-sm text-muted-foreground">{t('hrm.hireDate')}</div>
                          <div className="text-lg">{format(new Date(currentEmployee.hireDate), 'MMM dd, yyyy')}</div>
                        </CardContent>
                      </Card>
                      <Card>
                        <CardContent className="p-4">
                          <div className="text-sm text-muted-foreground">{t('hrm.status')}</div>
                          <Badge className={getStatusColor(currentEmployee.status)}>
                            {t(`hrm.${currentEmployee.status}`)}
                          </Badge>
                        </CardContent>
                      </Card>
                    </div>

                    <Card>
                      <CardHeader>
                        <CardTitle>{t('hrm.performance')}</CardTitle>
                      </CardHeader>
                      <CardContent>
                        <div className="space-y-4">
                          {currentEmployee.performance.map((perf, index) => (
                            <div key={index} className="space-y-2">
                              <div className="flex justify-between text-sm">
                                <span>{perf.period}</span>
                                <span>{perf.customerRating}/5.0</span>
                              </div>
                              <div className="grid grid-cols-3 gap-4 text-sm">
                                <div>
                                  <div className="text-muted-foreground">{t('hrm.ordersProcessed')}</div>
                                  <div>{perf.ordersProcessed}</div>
                                </div>
                                <div>
                                  <div className="text-muted-foreground">{t('hrm.tasksCompleted')}</div>
                                  <div>{perf.tasksCompleted}</div>
                                </div>
                                <div>
                                  <div className="text-muted-foreground">{t('hrm.sales')}</div>
                                  <div>${perf.sales.toFixed(2)}</div>
                                </div>
                              </div>
                              <Progress value={perf.customerRating * 20} className="h-2" />
                            </div>
                          ))}
                        </div>
                      </CardContent>
                    </Card>
                  </CardContent>
                </Card>

                <Card>
                  <CardHeader>
                    <CardTitle>{t('hrm.myLeaveRequests')}</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>{t('hrm.type')}</TableHead>
                          <TableHead>{t('hrm.dates')}</TableHead>
                          <TableHead>{t('hrm.days')}</TableHead>
                          <TableHead>{t('hrm.status')}</TableHead>
                          <TableHead>{t('hrm.submittedAt')}</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {myLeaveRequests.map((request) => (
                          <TableRow key={request.id}>
                            <TableCell>
                              <Badge className={getLeaveTypeColor(request.type)}>
                                {t(`hrm.${request.type}`)}
                              </Badge>
                            </TableCell>
                            <TableCell>
                              {format(new Date(request.startDate), 'MMM dd')} - {format(new Date(request.endDate), 'MMM dd')}
                            </TableCell>
                            <TableCell>{request.days}</TableCell>
                            <TableCell>
                              <Badge className={getStatusColor(request.status)}>
                                {t(`hrm.${request.status}`)}
                              </Badge>
                            </TableCell>
                            <TableCell>{format(new Date(request.submittedAt), 'MMM dd, yyyy')}</TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </CardContent>
                </Card>
              </>
            )}
          </TabsContent>
        )}

        <TabsContent value="employees" className="space-y-4">
          <PermissionWrapper permission={PERMISSIONS.VIEW_ALL_HRM}>
            <Card>
              <CardContent className="p-6">
                <div className="flex gap-4 mb-6">
                  <div className="relative flex-1">
                    <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-4 w-4" />
                    <Input
                      placeholder={t('hrm.searchEmployees')}
                      value={searchTerm}
                      onChange={(e) => setSearchTerm(e.target.value)}
                      className="pl-10"
                    />
                  </div>
                  <Select value={statusFilter} onValueChange={setStatusFilter}>
                    <SelectTrigger className="w-40">
                      <Filter className="h-4 w-4 mr-2" />
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="all">{t('common.all')}</SelectItem>
                      <SelectItem value="active">{t('hrm.active')}</SelectItem>
                      <SelectItem value="inactive">{t('hrm.inactive')}</SelectItem>
                      <SelectItem value="on_leave">{t('hrm.onLeave')}</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>{t('hrm.employee')}</TableHead>
                      <TableHead>{t('hrm.position')}</TableHead>
                      <TableHead>{t('hrm.department')}</TableHead>
                      <TableHead>{t('hrm.status')}</TableHead>
                      <TableHead>{t('hrm.hireDate')}</TableHead>
                      <TableHead>{t('hrm.performance')}</TableHead>
                      <TableHead>{t('common.actions')}</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {filteredEmployees.map((employee) => (
                      <TableRow key={employee.id}>
                        <TableCell>
                          <div className="flex items-center gap-3">
                            <Avatar className="h-8 w-8">
                              <AvatarFallback>
                                {employee.name.split(' ').map(n => n[0]).join('')}
                              </AvatarFallback>
                            </Avatar>
                            <div>
                              <div>{employee.name}</div>
                              <div className="text-xs text-muted-foreground">{employee.email}</div>
                            </div>
                          </div>
                        </TableCell>
                        <TableCell>{employee.position}</TableCell>
                        <TableCell>{employee.department}</TableCell>
                        <TableCell>
                          <Badge className={getStatusColor(employee.status)}>
                            {t(`hrm.${employee.status}`)}
                          </Badge>
                        </TableCell>
                        <TableCell>{format(new Date(employee.hireDate), 'MMM dd, yyyy')}</TableCell>
                        <TableCell>
                          {employee.performance.length > 0 && (
                            <div className="flex items-center gap-2">
                              <TrendingUp className="h-4 w-4 text-green-600" />
                              <span>{employee.performance[0].customerRating}/5.0</span>
                            </div>
                          )}
                        </TableCell>
                        <TableCell>
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => setSelectedEmployee(employee)}
                          >
                            <Eye className="h-4 w-4" />
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          </PermissionWrapper>
        </TabsContent>

        <TabsContent value="leave-requests" className="space-y-4">
          <PermissionWrapper permission={PERMISSIONS.APPROVE_LEAVE_REQUESTS}>
            <Card>
              <CardHeader>
                <CardTitle>{t('hrm.pendingLeaveRequests')}</CardTitle>
              </CardHeader>
              <CardContent>
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>{t('hrm.employee')}</TableHead>
                      <TableHead>{t('hrm.type')}</TableHead>
                      <TableHead>{t('hrm.dates')}</TableHead>
                      <TableHead>{t('hrm.days')}</TableHead>
                      <TableHead>{t('hrm.reason')}</TableHead>
                      <TableHead>{t('hrm.submittedAt')}</TableHead>
                      <TableHead>{t('common.actions')}</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {pendingLeaveRequests.map((request) => {
                      const employee = employees.find(emp => emp.id === request.employeeId);
                      return (
                        <TableRow key={request.id}>
                          <TableCell>
                            <div className="flex items-center gap-3">
                              <Avatar className="h-8 w-8">
                                <AvatarFallback>
                                  {employee?.name.split(' ').map(n => n[0]).join('') || 'NA'}
                                </AvatarFallback>
                              </Avatar>
                              <div>
                                <div>{employee?.name || 'Unknown'}</div>
                                <div className="text-xs text-muted-foreground">{employee?.position}</div>
                              </div>
                            </div>
                          </TableCell>
                          <TableCell>
                            <Badge className={getLeaveTypeColor(request.type)}>
                              {t(`hrm.${request.type}`)}
                            </Badge>
                          </TableCell>
                          <TableCell>
                            {format(new Date(request.startDate), 'MMM dd')} - {format(new Date(request.endDate), 'MMM dd')}
                          </TableCell>
                          <TableCell>{request.days}</TableCell>
                          <TableCell className="max-w-xs truncate">{request.reason}</TableCell>
                          <TableCell>{format(new Date(request.submittedAt), 'MMM dd, yyyy')}</TableCell>
                          <TableCell>
                            <div className="flex gap-2">
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => handleApproveLeave(request.id)}
                                className="text-green-600 hover:text-green-700"
                              >
                                <Check className="h-4 w-4" />
                              </Button>
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => handleRejectLeave(request.id)}
                                className="text-red-600 hover:text-red-700"
                              >
                                <X className="h-4 w-4" />
                              </Button>
                            </div>
                          </TableCell>
                        </TableRow>
                      );
                    })}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          </PermissionWrapper>
        </TabsContent>

        <TabsContent value="schedules" className="space-y-4">
          <PermissionWrapper permission={PERMISSIONS.ASSIGN_SHIFTS}>
            <Card>
              <CardHeader>
                <div className="flex justify-between items-center">
                  <CardTitle>{t('hrm.workSchedules')}</CardTitle>
                  <Button>
                    <Plus className="h-4 w-4 mr-2" />
                    {t('hrm.assignShift')}
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <div className="text-center py-8 text-muted-foreground">
                  {t('hrm.scheduleManagementComingSoon')}
                </div>
              </CardContent>
            </Card>
          </PermissionWrapper>
        </TabsContent>
      </Tabs>
    </div>
  );
}
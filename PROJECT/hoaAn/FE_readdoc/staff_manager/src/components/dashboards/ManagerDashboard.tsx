import React, { useState } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Switch } from '../ui/switch';
import { Textarea } from '../ui/textarea';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../ui/dialog';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Progress } from '../ui/progress';
import { toast } from 'sonner';
import { 
  Users, 
  TrendingUp, 
  DollarSign, 
  Package, 
  Clock, 
  UserPlus, 
  Edit, 
  Eye,
  BarChart3,
  Calendar,
  AlertTriangle,
  CheckCircle,
  UserCheck
} from 'lucide-react';
import { mockUsers, mockShifts, mockInventoryEntries, mockAnalytics } from '../../services/mockData';
import { ROLES } from '../../utils/constants';

export const ManagerDashboard: React.FC = () => {
  const { user } = useAuth();
  const [newStaffData, setNewStaffData] = useState({
    name: '',
    email: '',
    phone: '',
  });

  // Get staff managed by current manager
  const managedStaff = mockUsers.filter(u => 
    u.role === ROLES.STAFF && u.managerId === user?.id
  );

  // Get all shifts for managed staff
  const staffShifts = mockShifts.filter(shift => 
    managedStaff.some(staff => staff.id === shift.staffId)
  );

  // Get today's active shifts
  const today = new Date().toDateString();
  const todayShifts = staffShifts.filter(shift => 
    new Date(shift.startTime).toDateString() === today
  );

  // Calculate staff performance
  const staffPerformance = managedStaff.map(staff => {
    const staffShiftsData = staffShifts.filter(s => s.staffId === staff.id);
    const totalSales = staffShiftsData.reduce((sum, shift) => sum + (shift.totalSales || 0), 0);
    const totalShifts = staffShiftsData.length;
    const currentShift = staffShiftsData.find(s => s.status === 'open');
    
    return {
      ...staff,
      totalSales,
      totalShifts,
      averageSales: totalShifts > 0 ? totalSales / totalShifts : 0,
      currentShift,
      isCurrentlyWorking: !!currentShift,
    };
  });

  const handleAddStaff = () => {
    if (!newStaffData.name || !newStaffData.email) {
      toast.error('Please fill in all required fields');
      return;
    }

    // Mock API call to add staff
    toast.success(`${newStaffData.name} has been added to your team!`);
    setNewStaffData({ name: '', email: '', phone: '' });
  };

  const handleToggleStaffStatus = (staffId: string, currentStatus: boolean) => {
    const staff = managedStaff.find(s => s.id === staffId);
    if (staff) {
      toast.success(
        `${staff.name} has been ${currentStatus ? 'deactivated' : 'activated'}`
      );
    }
  };

  const sendNotificationToStaff = (staffId: string) => {
    const staff = managedStaff.find(s => s.id === staffId);
    if (staff) {
      toast.success(`Notification sent to ${staff.name}`);
    }
  };

  return (
    <div className="space-y-6">
      {/* Welcome Message */}
      <div>
        <h1>Manager Dashboard</h1>
        <p className="text-muted-foreground mt-2">
          Manage your team, monitor performance, and oversee operations.
        </p>
      </div>

      {/* Overview Stats */}
      <div className="grid gap-4 md:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              Total Staff
            </CardTitle>
            <Users className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{managedStaff.length}</div>
            <p className="text-xs text-muted-foreground">
              {managedStaff.filter(s => s.isActive).length} active
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              Today's Shifts
            </CardTitle>
            <Clock className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{todayShifts.length}</div>
            <p className="text-xs text-muted-foreground">
              {todayShifts.filter(s => s.status === 'open').length} currently active
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              Team Sales Today
            </CardTitle>
            <DollarSign className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              ${todayShifts.reduce((sum, shift) => sum + (shift.totalSales || 0), 0).toFixed(2)}
            </div>
            <p className="text-xs text-muted-foreground">
              {todayShifts.reduce((sum, shift) => sum + (shift.totalTransactions || 0), 0)} transactions
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              Inventory Entries
            </CardTitle>
            <Package className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {mockInventoryEntries.filter(entry => 
                managedStaff.some(staff => staff.id === entry.staffId)
              ).length}
            </div>
            <p className="text-xs text-muted-foreground">
              This week
            </p>
          </CardContent>
        </Card>
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        {/* Staff Management */}
        <Card>
          <CardHeader>
            <div className="flex items-center justify-between">
              <div>
                <CardTitle className="flex items-center gap-2">
                  <Users className="h-5 w-5" />
                  Staff Management
                </CardTitle>
                <CardDescription>
                  Manage your team members
                </CardDescription>
              </div>
              <Dialog>
                <DialogTrigger asChild>
                  <Button size="sm">
                    <UserPlus className="h-4 w-4 mr-2" />
                    Add Staff
                  </Button>
                </DialogTrigger>
                <DialogContent>
                  <DialogHeader>
                    <DialogTitle>Add New Staff Member</DialogTitle>
                    <DialogDescription>
                      Add a new staff member to your team
                    </DialogDescription>
                  </DialogHeader>
                  
                  <div className="space-y-4">
                    <div>
                      <Label htmlFor="staffName">Full Name *</Label>
                      <Input
                        id="staffName"
                        placeholder="Enter full name"
                        value={newStaffData.name}
                        onChange={(e) => setNewStaffData({...newStaffData, name: e.target.value})}
                      />
                    </div>
                    <div>
                      <Label htmlFor="staffEmail">Email Address *</Label>
                      <Input
                        id="staffEmail"
                        type="email"
                        placeholder="Enter email address"
                        value={newStaffData.email}
                        onChange={(e) => setNewStaffData({...newStaffData, email: e.target.value})}
                      />
                    </div>
                    <div>
                      <Label htmlFor="staffPhone">Phone Number</Label>
                      <Input
                        id="staffPhone"
                        placeholder="Enter phone number"
                        value={newStaffData.phone}
                        onChange={(e) => setNewStaffData({...newStaffData, phone: e.target.value})}
                      />
                    </div>
                    <Button onClick={handleAddStaff} className="w-full">
                      Add Staff Member
                    </Button>
                  </div>
                </DialogContent>
              </Dialog>
            </div>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {staffPerformance.map((staff) => (
                <div key={staff.id} className="flex items-center justify-between p-3 border rounded-lg">
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center">
                      {staff.name.charAt(0)}
                    </div>
                    <div>
                      <p className="font-medium">{staff.name}</p>
                      <p className="text-sm text-muted-foreground">{staff.email}</p>
                    </div>
                  </div>
                  
                  <div className="flex items-center gap-3">
                    {staff.isCurrentlyWorking && (
                      <Badge variant="default" className="bg-green-500">
                        <UserCheck className="h-3 w-3 mr-1" />
                        Working
                      </Badge>
                    )}
                    
                    <Badge variant={staff.isActive ? 'default' : 'secondary'}>
                      {staff.isActive ? 'Active' : 'Inactive'}
                    </Badge>
                    
                    <div className="flex items-center gap-1">
                      <Switch
                        checked={staff.isActive}
                        onCheckedChange={() => handleToggleStaffStatus(staff.id, staff.isActive)}
                      />
                    </div>
                  </div>
                </div>
              ))}
              
              {managedStaff.length === 0 && (
                <p className="text-center text-muted-foreground py-4">
                  No staff members yet. Add your first team member!
                </p>
              )}
            </div>
          </CardContent>
        </Card>

        {/* Staff Performance */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <BarChart3 className="h-5 w-5" />
              Staff Performance
            </CardTitle>
            <CardDescription>
              Individual performance metrics
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {staffPerformance
                .sort((a, b) => b.totalSales - a.totalSales)
                .map((staff, index) => (
                  <div key={staff.id} className="space-y-2">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <span className="w-6 h-6 rounded-full bg-primary/10 flex items-center justify-center text-sm">
                          {index + 1}
                        </span>
                        <span className="font-medium">{staff.name}</span>
                      </div>
                      <span className="font-medium">${staff.totalSales.toFixed(2)}</span>
                    </div>
                    
                    <Progress 
                      value={staffPerformance.length > 0 ? (staff.totalSales / Math.max(...staffPerformance.map(s => s.totalSales))) * 100 : 0} 
                    />
                    
                    <div className="flex items-center justify-between text-sm text-muted-foreground">
                      <span>{staff.totalShifts} shifts</span>
                      <span>Avg: ${staff.averageSales.toFixed(2)}/shift</span>
                    </div>
                  </div>
                ))}
              
              {staffPerformance.length === 0 && (
                <p className="text-center text-muted-foreground py-4">
                  No performance data available
                </p>
              )}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Current Shifts */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Clock className="h-5 w-5" />
            Current Shifts
          </CardTitle>
          <CardDescription>
            Monitor active staff shifts and cash handling
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Staff Member</TableHead>
                <TableHead>Shift Start</TableHead>
                <TableHead>Opening Cash</TableHead>
                <TableHead>Current Sales</TableHead>
                <TableHead>Transactions</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {todayShifts.map((shift) => {
                const staff = managedStaff.find(s => s.id === shift.staffId);
                return (
                  <TableRow key={shift.id}>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center text-sm">
                          {staff?.name.charAt(0)}
                        </div>
                        {staff?.name}
                      </div>
                    </TableCell>
                    <TableCell>
                      {new Date(shift.startTime).toLocaleTimeString()}
                    </TableCell>
                    <TableCell>${shift.openingCash?.toFixed(2)}</TableCell>
                    <TableCell>${(shift.totalSales || 0).toFixed(2)}</TableCell>
                    <TableCell>{shift.totalTransactions || 0}</TableCell>
                    <TableCell>
                      <Badge variant={shift.status === 'open' ? 'default' : 'secondary'}>
                        {shift.status}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => sendNotificationToStaff(shift.staffId)}
                      >
                        Send Message
                      </Button>
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
          
          {todayShifts.length === 0 && (
            <p className="text-center text-muted-foreground py-8">
              No active shifts today
            </p>
          )}
        </CardContent>
      </Card>

      {/* Inventory Report */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5" />
            Inventory Report
          </CardTitle>
          <CardDescription>
            Recent inventory entries by staff
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {mockInventoryEntries
              .filter(entry => managedStaff.some(staff => staff.id === entry.staffId))
              .slice(0, 8)
              .map((entry) => (
                <div key={entry.id} className="flex items-center justify-between p-3 border rounded-lg">
                  <div className="flex items-center gap-3">
                    <Package className="h-5 w-5 text-muted-foreground" />
                    <div>
                      <p className="font-medium">{entry.productName}</p>
                      <p className="text-sm text-muted-foreground">
                        By {entry.staffName} • {entry.deliveryNote}
                      </p>
                    </div>
                  </div>
                  
                  <div className="text-right">
                    <p className="font-medium">+{entry.quantity}</p>
                    <p className="text-sm text-muted-foreground">
                      {new Date(entry.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                </div>
              ))}
            
            {mockInventoryEntries.filter(entry => 
              managedStaff.some(staff => staff.id === entry.staffId)
            ).length === 0 && (
              <p className="text-center text-muted-foreground py-4">
                No inventory entries yet
              </p>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  );
};
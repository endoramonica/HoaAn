import React, { useState } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Textarea } from '../ui/textarea';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from '../ui/dialog';
import { toast } from 'sonner';
import { 
  Clock, 
  DollarSign, 
  Package, 
  Bell, 
  History, 
  PlayCircle, 
  StopCircle,
  Truck,
  CheckCircle,
  AlertCircle
} from 'lucide-react';
import { mockShifts, mockInventoryEntries, mockNotifications } from '../../services/mockData';

export const StaffDashboard: React.FC = () => {
  const { user } = useAuth();
  const [isShiftActive, setIsShiftActive] = useState(false);
  const [currentShift, setCurrentShift] = useState<any>(null);
  const [openingCash, setOpeningCash] = useState('');
  const [closingCash, setClosingCash] = useState('');
  const [deliveryItems, setDeliveryItems] = useState([{ productName: '', quantity: '', deliveryNote: '' }]);

  // Get current user's data
  const userShifts = mockShifts.filter(shift => shift.staffId === user?.id);
  const userNotifications = mockNotifications.filter(notif => notif.staffId === user?.id);
  const unreadNotifications = userNotifications.filter(notif => !notif.isRead);

  const handleStartShift = () => {
    if (!openingCash) {
      toast.error('Please enter opening cash amount');
      return;
    }

    const newShift = {
      id: `shift-new-${Date.now()}`,
      staffId: user?.id,
      storeId: user?.storeId,
      startTime: new Date().toISOString(),
      openingCash: parseFloat(openingCash),
      status: 'open',
      totalSales: 0,
      totalTransactions: 0,
    };

    setCurrentShift(newShift);
    setIsShiftActive(true);
    setOpeningCash('');
    toast.success('Shift started successfully!');
  };

  const handleEndShift = () => {
    if (!closingCash) {
      toast.error('Please enter closing cash amount');
      return;
    }

    setIsShiftActive(false);
    setCurrentShift(null);
    setClosingCash('');
    toast.success('Shift ended successfully!');
  };

  const handleDeliverySubmit = () => {
    const validItems = deliveryItems.filter(item => 
      item.productName && item.quantity && item.deliveryNote
    );

    if (validItems.length === 0) {
      toast.error('Please fill in at least one complete delivery item');
      return;
    }

    // Mock API call to save delivery items
    toast.success(`${validItems.length} delivery items recorded successfully!`);
    setDeliveryItems([{ productName: '', quantity: '', deliveryNote: '' }]);
  };

  const addDeliveryItem = () => {
    setDeliveryItems([...deliveryItems, { productName: '', quantity: '', deliveryNote: '' }]);
  };

  const updateDeliveryItem = (index: number, field: string, value: string) => {
    const updated = [...deliveryItems];
    updated[index] = { ...updated[index], [field]: value };
    setDeliveryItems(updated);
  };

  const removeDeliveryItem = (index: number) => {
    setDeliveryItems(deliveryItems.filter((_, i) => i !== index));
  };

  return (
    <div className="space-y-6">
      {/* Welcome Message */}
      <div>
        <h1>Welcome, {user?.name}!</h1>
        <p className="text-muted-foreground mt-2">
          Manage your shifts, handle deliveries, and stay updated with notifications.
        </p>
      </div>

      {/* Current Shift Status */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Clock className="h-5 w-5" />
            Current Shift Status
          </CardTitle>
        </CardHeader>
        <CardContent>
          {isShiftActive ? (
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <Badge variant="default" className="bg-green-500">
                  <PlayCircle className="h-3 w-3 mr-1" />
                  Shift Active
                </Badge>
                <span className="text-sm text-muted-foreground">
                  Started: {new Date(currentShift?.startTime).toLocaleTimeString()}
                </span>
              </div>
              
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label>Opening Cash</Label>
                  <p className="text-2xl font-bold">${currentShift?.openingCash?.toFixed(2)}</p>
                </div>
                <div>
                  <Label>Current Sales</Label>
                  <p className="text-2xl font-bold">${currentShift?.totalSales?.toFixed(2) || '0.00'}</p>
                </div>
              </div>

              <div className="flex gap-4">
                <div className="flex-1">
                  <Label htmlFor="closingCash">Closing Cash Amount</Label>
                  <Input
                    id="closingCash"
                    type="number"
                    step="0.01"
                    placeholder="Enter closing cash amount"
                    value={closingCash}
                    onChange={(e) => setClosingCash(e.target.value)}
                  />
                </div>
                <div className="flex items-end">
                  <Button onClick={handleEndShift} variant="destructive">
                    <StopCircle className="h-4 w-4 mr-2" />
                    End Shift
                  </Button>
                </div>
              </div>
            </div>
          ) : (
            <div className="space-y-4">
              <Badge variant="secondary">
                <StopCircle className="h-3 w-3 mr-1" />
                No Active Shift
              </Badge>
              
              <div className="flex gap-4">
                <div className="flex-1">
                  <Label htmlFor="openingCash">Opening Cash Amount</Label>
                  <Input
                    id="openingCash"
                    type="number"
                    step="0.01"
                    placeholder="Enter opening cash amount"
                    value={openingCash}
                    onChange={(e) => setOpeningCash(e.target.value)}
                  />
                </div>
                <div className="flex items-end">
                  <Button onClick={handleStartShift}>
                    <PlayCircle className="h-4 w-4 mr-2" />
                    Start Shift
                  </Button>
                </div>
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      <div className="grid gap-6 md:grid-cols-2">
        {/* Delivery Management */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Truck className="h-5 w-5" />
              Receive Delivery
            </CardTitle>
            <CardDescription>
              Record incoming inventory deliveries
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Dialog>
              <DialogTrigger asChild>
                <Button className="w-full">
                  <Package className="h-4 w-4 mr-2" />
                  Process New Delivery
                </Button>
              </DialogTrigger>
              <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
                <DialogHeader>
                  <DialogTitle>Process Delivery</DialogTitle>
                  <DialogDescription>
                    Enter details for each item in the delivery
                  </DialogDescription>
                </DialogHeader>
                
                <div className="space-y-4">
                  {deliveryItems.map((item, index) => (
                    <div key={index} className="p-4 border rounded-lg space-y-3">
                      <div className="flex items-center justify-between">
                        <h4>Item {index + 1}</h4>
                        {deliveryItems.length > 1 && (
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => removeDeliveryItem(index)}
                          >
                            Remove
                          </Button>
                        )}
                      </div>
                      
                      <div className="grid grid-cols-3 gap-3">
                        <div>
                          <Label>Product Name</Label>
                          <Input
                            placeholder="Enter product name"
                            value={item.productName}
                            onChange={(e) => updateDeliveryItem(index, 'productName', e.target.value)}
                          />
                        </div>
                        <div>
                          <Label>Quantity</Label>
                          <Input
                            type="number"
                            placeholder="Enter quantity"
                            value={item.quantity}
                            onChange={(e) => updateDeliveryItem(index, 'quantity', e.target.value)}
                          />
                        </div>
                        <div>
                          <Label>Delivery Note</Label>
                          <Input
                            placeholder="DEL-2024-xxx"
                            value={item.deliveryNote}
                            onChange={(e) => updateDeliveryItem(index, 'deliveryNote', e.target.value)}
                          />
                        </div>
                      </div>
                    </div>
                  ))}
                  
                  <div className="flex gap-3">
                    <Button variant="outline" onClick={addDeliveryItem}>
                      Add Another Item
                    </Button>
                    <Button onClick={handleDeliverySubmit} className="flex-1">
                      <CheckCircle className="h-4 w-4 mr-2" />
                      Submit Delivery
                    </Button>
                  </div>
                </div>
              </DialogContent>
            </Dialog>

            <div className="mt-4">
              <h4 className="font-medium mb-2">Recent Deliveries</h4>
              <div className="space-y-2">
                {mockInventoryEntries
                  .filter(entry => entry.staffId === user?.id)
                  .slice(0, 3)
                  .map((entry) => (
                    <div key={entry.id} className="flex items-center justify-between p-2 bg-muted rounded">
                      <div>
                        <p className="font-medium">{entry.productName}</p>
                        <p className="text-sm text-muted-foreground">
                          Qty: {entry.quantity} • {entry.deliveryNote}
                        </p>
                      </div>
                      <Badge variant="outline" className="text-xs">
                        {new Date(entry.createdAt).toLocaleDateString()}
                      </Badge>
                    </div>
                  ))}
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Notifications */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Bell className="h-5 w-5" />
              Notifications
              {unreadNotifications.length > 0 && (
                <Badge variant="destructive" className="ml-2">
                  {unreadNotifications.length}
                </Badge>
              )}
            </CardTitle>
            <CardDescription>
              Messages from your manager
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {userNotifications.slice(0, 4).map((notification) => (
                <div
                  key={notification.id}
                  className={`p-3 rounded-lg border ${
                    !notification.isRead ? 'bg-blue-50 border-blue-200' : ''
                  }`}
                >
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center gap-2">
                        <h4 className="font-medium">{notification.title}</h4>
                        {!notification.isRead && (
                          <AlertCircle className="h-4 w-4 text-blue-500" />
                        )}
                      </div>
                      <p className="text-sm text-muted-foreground mt-1">
                        {notification.message}
                      </p>
                      <p className="text-xs text-muted-foreground mt-2">
                        From: {notification.fromManagerName} • {new Date(notification.createdAt).toLocaleString()}
                      </p>
                    </div>
                  </div>
                </div>
              ))}
              
              {userNotifications.length === 0 && (
                <p className="text-center text-muted-foreground py-4">
                  No notifications yet
                </p>
              )}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Shift History */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <History className="h-5 w-5" />
            Shift History
          </CardTitle>
          <CardDescription>
            Your recent work shifts and performance
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {userShifts.slice(0, 5).map((shift) => (
              <div key={shift.id} className="flex items-center justify-between p-4 border rounded-lg">
                <div className="flex-1">
                  <div className="flex items-center gap-3">
                    <Badge variant={shift.status === 'closed' ? 'default' : 'secondary'}>
                      {shift.status}
                    </Badge>
                    <span className="font-medium">
                      {new Date(shift.startTime).toLocaleDateString()}
                    </span>
                  </div>
                  <div className="mt-2 grid grid-cols-3 gap-4 text-sm">
                    <div>
                      <span className="text-muted-foreground">Duration:</span>
                      <p className="font-medium">
                        {shift.endTime ? 
                          `${Math.round((new Date(shift.endTime).getTime() - new Date(shift.startTime).getTime()) / (1000 * 60 * 60))}h` 
                          : 'Active'
                        }
                      </p>
                    </div>
                    <div>
                      <span className="text-muted-foreground">Opening Cash:</span>
                      <p className="font-medium">${shift.openingCash?.toFixed(2)}</p>
                    </div>
                    <div>
                      <span className="text-muted-foreground">Closing Cash:</span>
                      <p className="font-medium">
                        {shift.closingCash ? `$${shift.closingCash.toFixed(2)}` : '-'}
                      </p>
                    </div>
                  </div>
                  <div className="mt-2 grid grid-cols-2 gap-4 text-sm">
                    <div>
                      <span className="text-muted-foreground">Total Sales:</span>
                      <p className="font-medium">${shift.totalSales?.toFixed(2) || '0.00'}</p>
                    </div>
                    <div>
                      <span className="text-muted-foreground">Transactions:</span>
                      <p className="font-medium">{shift.totalTransactions || 0}</p>
                    </div>
                  </div>
                  {shift.notes && (
                    <p className="text-sm text-muted-foreground mt-2 italic">
                      "{shift.notes}"
                    </p>
                  )}
                </div>
              </div>
            ))}
            
            {userShifts.length === 0 && (
              <p className="text-center text-muted-foreground py-8">
                No shift history yet
              </p>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  );
};
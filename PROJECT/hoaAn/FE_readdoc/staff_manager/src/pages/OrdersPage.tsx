import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useI18n } from '../contexts/I18nContext';
import { PermissionWrapper } from '../components/PermissionWrapper';
import { OrderDetailsModal } from '../components/modals/OrderDetailsModal';
import { NewOrderModal } from '../components/modals/NewOrderModal';
import { PaginationCustom } from '../components/ui/pagination-custom';
import { usePaginatedApi } from '../hooks/usePaginatedApi';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { 
  Search, 
  Filter, 
  Download, 
  Eye, 
  Edit, 
  MoreHorizontal,
  Plus,
  Loader2
} from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '../components/ui/dropdown-menu';
import { api } from '../services/api';
import { PERMISSIONS, ORDER_STATUS } from '../utils/constants';
import { Order } from '../types';
import { toast } from 'sonner';

export const OrdersPage: React.FC = () => {
  const { user, hasPermission } = useAuth();
  const { t } = useI18n();
  
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const [orderDetailsModalOpen, setOrderDetailsModalOpen] = useState(false);
  const [newOrderModalOpen, setNewOrderModalOpen] = useState(false);

  const canViewAllOrders = hasPermission(PERMISSIONS.VIEW_ALL_ORDERS);
  
  // Server-side pagination with filters
  const {
    data: orders,
    meta,
    isLoading,
    error,
    page,
    pageSize,
    setPage,
    setPageSize,
    setFilters,
  } = usePaginatedApi<Order>({
    fetchFn: api.fetchOrders,
    initialPageSize: 10,
    initialFilters: {
      status: statusFilter,
      search: searchTerm,
      staffId: canViewAllOrders ? undefined : user?.id,
    },
  });

  // Update filters when search or status changes
  useEffect(() => {
    setFilters({
      status: statusFilter,
      search: searchTerm,
      staffId: canViewAllOrders ? undefined : user?.id,
    });
  }, [searchTerm, statusFilter, canViewAllOrders, user?.id, setFilters]);

  const getStatusColor = (status: string) => {
    switch (status) {
      case ORDER_STATUS.COMPLETED:
        return 'default';
      case ORDER_STATUS.PENDING:
        return 'secondary';
      case ORDER_STATUS.PROCESSING:
        return 'outline';
      case ORDER_STATUS.CANCELLED:
        return 'destructive';
      default:
        return 'secondary';
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString();
  };

  const handleViewOrder = (order: Order) => {
    setSelectedOrder(order);
    setOrderDetailsModalOpen(true);
  };

  const handleEditOrder = (order: Order) => {
    console.log('Editing order:', order.id);
    // Mock edit functionality
    toast.success(`Edit functionality for order ${order.orderNumber} - feature coming soon!`);
  };

  const handlePrintReceipt = (order: Order) => {
    console.log('Printing receipt for order:', order.id);
    // Mock print functionality
    toast.success(`Receipt for ${order.orderNumber} sent to printer`);
  };

  const handleExportOrders = () => {
    console.log('Exporting orders:', orders);
    // Mock export functionality
    toast.success(`Exporting ${meta?.totalItems || 0} orders to CSV`);
  };

  const handleMoreFilters = () => {
    // Mock advanced filters
    toast.info('Advanced filters - feature coming soon!');
  };

  const handleNewOrder = () => {
    setNewOrderModalOpen(true);
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-semibold">
            {canViewAllOrders ? t('orders.allOrders') : t('orders.myOrders')}
          </h1>
          <p className="text-muted-foreground">
            {canViewAllOrders 
              ? 'Manage all store orders' 
              : 'View and manage your assigned orders'
            }
          </p>
        </div>
        
        <PermissionWrapper permission={PERMISSIONS.CREATE_ORDER}>
          <Button onClick={handleNewOrder}>
            <Plus className="w-4 h-4 mr-2" />
            New Order
          </Button>
        </PermissionWrapper>
      </div>

      {/* Filters */}
      <Card>
        <CardHeader>
          <CardTitle>Filters</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col md:flex-row gap-4">
            <div className="flex-1">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground w-4 h-4" />
                <Input
                  placeholder="Search orders..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            
            <Select value={statusFilter} onValueChange={setStatusFilter}>
              <SelectTrigger className="w-48">
                <SelectValue placeholder="Filter by status" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Status</SelectItem>
                <SelectItem value={ORDER_STATUS.PENDING}>Pending</SelectItem>
                <SelectItem value={ORDER_STATUS.PROCESSING}>Processing</SelectItem>
                <SelectItem value={ORDER_STATUS.COMPLETED}>Completed</SelectItem>
                <SelectItem value={ORDER_STATUS.CANCELLED}>Cancelled</SelectItem>
              </SelectContent>
            </Select>

            <Button variant="outline" onClick={handleMoreFilters}>
              <Filter className="w-4 h-4 mr-2" />
              More Filters
            </Button>

            <Button variant="outline" onClick={handleExportOrders}>
              <Download className="w-4 h-4 mr-2" />
              Export
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Orders Table */}
      <Card>
        <CardHeader>
          <CardTitle>
            Orders {meta && `(${meta.totalItems})`}
            {isLoading && <Loader2 className="inline-block w-4 h-4 ml-2 animate-spin" />}
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          {error && (
            <div className="text-center py-8 text-destructive">
              <p>Error loading orders: {error}</p>
            </div>
          )}
          
          {!error && !isLoading && orders.length === 0 ? (
            <div className="text-center py-8">
              <p className="text-muted-foreground">No orders found</p>
            </div>
          ) : (
            <>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Order #</TableHead>
                    <TableHead>Customer</TableHead>
                    <TableHead>Items</TableHead>
                    <TableHead>Total</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Payment</TableHead>
                    <TableHead>Date</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {isLoading ? (
                    <TableRow>
                      <TableCell colSpan={8} className="text-center py-8">
                        <Loader2 className="w-6 h-6 animate-spin mx-auto" />
                      </TableCell>
                    </TableRow>
                  ) : (
                    orders.map((order) => (
                    <TableRow key={order.id}>
                      <TableCell className="font-medium">
                        {order.orderNumber}
                      </TableCell>
                      <TableCell>
                        <div>
                          <p className="font-medium">
                            {order.customerName || 'Walk-in'}
                          </p>
                          {order.customerPhone && (
                            <p className="text-sm text-muted-foreground">
                              {order.customerPhone}
                            </p>
                          )}
                        </div>
                      </TableCell>
                      <TableCell>
                        <div>
                          <p>{order.items.length} item(s)</p>
                          <p className="text-sm text-muted-foreground">
                            {order.items[0]?.productName}
                            {order.items.length > 1 && ` +${order.items.length - 1} more`}
                          </p>
                        </div>
                      </TableCell>
                      <TableCell className="font-medium">
                        ${order.total.toFixed(2)}
                      </TableCell>
                      <TableCell>
                        <Badge variant={getStatusColor(order.status)}>
                          {order.status}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div>
                          <p className="capitalize">{order.paymentMethod}</p>
                          <Badge 
                            variant={order.paymentStatus === 'paid' ? 'default' : 'secondary'}
                            className="text-xs"
                          >
                            {order.paymentStatus}
                          </Badge>
                        </div>
                      </TableCell>
                      <TableCell>
                        {formatDate(order.createdAt)}
                      </TableCell>
                      <TableCell>
                        <DropdownMenu>
                          <DropdownMenuTrigger asChild>
                            <Button variant="ghost" size="sm">
                              <MoreHorizontal className="w-4 h-4" />
                            </Button>
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="end">
                            <DropdownMenuItem onClick={() => handleViewOrder(order)}>
                              <Eye className="w-4 h-4 mr-2" />
                              View Details
                            </DropdownMenuItem>
                            <PermissionWrapper permission={PERMISSIONS.UPDATE_ORDER}>
                              <DropdownMenuItem onClick={() => handleEditOrder(order)}>
                                <Edit className="w-4 h-4 mr-2" />
                                Edit Order
                              </DropdownMenuItem>
                            </PermissionWrapper>
                            <DropdownMenuItem onClick={() => handlePrintReceipt(order)}>
                              <Download className="w-4 h-4 mr-2" />
                              Print Receipt
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </TableCell>
                    </TableRow>
                  ))
                  )}
                </TableBody>
              </Table>

              {/* Pagination */}
              {meta && (
                <PaginationCustom
                  currentPage={meta.currentPage}
                  totalPages={meta.totalPages}
                  itemsPerPage={meta.pageSize}
                  totalItems={meta.totalItems}
                  onPageChange={setPage}
                  onItemsPerPageChange={setPageSize}
                />
              )}
            </>
          )}
        </CardContent>
      </Card>
      
      {/* Modals */}
      <OrderDetailsModal
        open={orderDetailsModalOpen}
        onOpenChange={setOrderDetailsModalOpen}
        order={selectedOrder}
      />
      <NewOrderModal
        open={newOrderModalOpen}
        onOpenChange={setNewOrderModalOpen}
      />
    </div>
  );
};
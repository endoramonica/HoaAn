import React, { useState, useMemo } from 'react';
import { Search, Plus, Truck, Package, MapPin, Calendar, Filter, Download, Eye, CheckCircle } from 'lucide-react';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../components/ui/tabs';
import { PaginationCustom } from '../components/ui/pagination-custom';
import { usePagination } from '../hooks/usePagination';
import { PermissionWrapper } from '../components/PermissionWrapper';
import { useAuth } from '../contexts/AuthContext';
import { useI18n } from '../contexts/I18nContext';
import { mockData } from '../services/mockData';
import { Supplier, StockTransfer, ShippingStatus } from '../types';
import { PERMISSIONS } from '../utils/constants';
import { format } from 'date-fns';
import { toast } from 'sonner';

export function LRMPage() {
  const { user } = useAuth();
  const { t } = useI18n();
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [activeTab, setActiveTab] = useState('shipping');
  const [selectedTransfer, setSelectedTransfer] = useState<StockTransfer | null>(null);

  // Mock data - in real app this would come from API
  const suppliers: Supplier[] = mockData.suppliers || [];
  const stockTransfers: StockTransfer[] = mockData.stockTransfers || [];
  const shippingStatus: ShippingStatus[] = mockData.shippingStatus || [];

  const filteredShipping = useMemo(() => {
    return shippingStatus.filter(shipping => {
      const matchesSearch = shipping.orderId.toLowerCase().includes(searchTerm.toLowerCase()) ||
                           (shipping.trackingNumber && shipping.trackingNumber.toLowerCase().includes(searchTerm.toLowerCase()));
      const matchesStatus = statusFilter === 'all' || shipping.status === statusFilter;
      return matchesSearch && matchesStatus;
    });
  }, [shippingStatus, searchTerm, statusFilter]);

  const filteredTransfers = useMemo(() => {
    return stockTransfers.filter(transfer => {
      const matchesSearch = transfer.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
                           transfer.fromWarehouse.toLowerCase().includes(searchTerm.toLowerCase()) ||
                           transfer.toWarehouse.toLowerCase().includes(searchTerm.toLowerCase());
      const matchesStatus = statusFilter === 'all' || transfer.status === statusFilter;
      return matchesSearch && matchesStatus;
    });
  }, [stockTransfers, searchTerm, statusFilter]);

  // Pagination for shipping
  const {
    currentPage: shippingCurrentPage,
    totalPages: shippingTotalPages,
    itemsPerPage: shippingItemsPerPage,
    totalItems: shippingTotalItems,
    paginatedData: paginatedShipping,
    goToPage: shippingGoToPage,
    setItemsPerPage: setShippingItemsPerPage,
  } = usePagination({
    data: filteredShipping,
    initialItemsPerPage: 10,
  });

  // Pagination for transfers
  const {
    currentPage: transfersCurrentPage,
    totalPages: transfersTotalPages,
    itemsPerPage: transfersItemsPerPage,
    totalItems: transfersTotalItems,
    paginatedData: paginatedTransfers,
    goToPage: transfersGoToPage,
    setItemsPerPage: setTransfersItemsPerPage,
  } = usePagination({
    data: filteredTransfers,
    initialItemsPerPage: 10,
  });

  // Pagination for suppliers
  const filteredSuppliers = useMemo(() => {
    return suppliers.filter(supplier => 
      supplier.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      supplier.email.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [suppliers, searchTerm]);

  const {
    currentPage: suppliersCurrentPage,
    totalPages: suppliersTotalPages,
    itemsPerPage: suppliersItemsPerPage,
    totalItems: suppliersTotalItems,
    paginatedData: paginatedSuppliers,
    goToPage: suppliersGoToPage,
    setItemsPerPage: setSuppliersItemsPerPage,
  } = usePagination({
    data: filteredSuppliers,
    initialItemsPerPage: 10,
  });

  const handleConfirmDelivery = (orderId: string) => {
    toast.success(t('lrm.deliveryConfirmed'));
  };

  const handleRequestTransfer = () => {
    toast.success(t('lrm.transferRequested'));
  };

  const handleExportData = () => {
    toast.success(t('lrm.exportStarted'));
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'pending': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'picked': case 'in_transit': return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'shipped': return 'bg-purple-100 text-purple-800 border-purple-200';
      case 'delivered': return 'bg-green-100 text-green-800 border-green-200';
      case 'cancelled': return 'bg-red-100 text-red-800 border-red-200';
      default: return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'pending': return <Calendar className="h-4 w-4" />;
      case 'picked': case 'in_transit': return <Package className="h-4 w-4" />;
      case 'shipped': return <Truck className="h-4 w-4" />;
      case 'delivered': return <CheckCircle className="h-4 w-4" />;
      default: return <Package className="h-4 w-4" />;
    }
  };

  return (
    <div className="p-6 space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1>{t('lrm.title')}</h1>
          <p className="text-muted-foreground">{t('lrm.subtitle')}</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={handleExportData}>
            <Download className="h-4 w-4 mr-2" />
            {t('common.export')}
          </Button>
          <PermissionWrapper permission={PERMISSIONS.REQUEST_TRANSFERS}>
            <Button onClick={handleRequestTransfer}>
              <Plus className="h-4 w-4 mr-2" />
              {t('lrm.requestTransfer')}
            </Button>
          </PermissionWrapper>
        </div>
      </div>

      <Tabs value={activeTab} onValueChange={setActiveTab}>
        <TabsList>
          <TabsTrigger value="shipping">{t('lrm.shipping')}</TabsTrigger>
          <TabsTrigger value="transfers">{t('lrm.stockTransfers')}</TabsTrigger>
          <PermissionWrapper permission={PERMISSIONS.MANAGE_SUPPLIERS}>
            <TabsTrigger value="suppliers">{t('lrm.suppliers')}</TabsTrigger>
          </PermissionWrapper>
        </TabsList>

        <TabsContent value="shipping" className="space-y-4">
          <Card>
            <CardContent className="p-6">
              <div className="flex gap-4 mb-6">
                <div className="relative flex-1">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-4 w-4" />
                  <Input
                    placeholder={t('lrm.searchShipping')}
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
                    <SelectItem value="pending">{t('lrm.pending')}</SelectItem>
                    <SelectItem value="picked">{t('lrm.picked')}</SelectItem>
                    <SelectItem value="shipped">{t('lrm.shipped')}</SelectItem>
                    <SelectItem value="delivered">{t('lrm.delivered')}</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              {shippingTotalItems === 0 ? (
                <div className="text-center py-8">
                  <p className="text-muted-foreground">No shipping records found</p>
                </div>
              ) : (
                <>
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>{t('lrm.orderId')}</TableHead>
                        <TableHead>{t('lrm.trackingNumber')}</TableHead>
                        <TableHead>{t('lrm.carrier')}</TableHead>
                        <TableHead>{t('lrm.status')}</TableHead>
                        <TableHead>{t('lrm.estimatedDelivery')}</TableHead>
                        <TableHead>{t('lrm.actualDelivery')}</TableHead>
                        <TableHead>{t('common.actions')}</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {paginatedShipping.map((shipping) => (
                        <TableRow key={shipping.orderId}>
                          <TableCell>
                            <div className="flex items-center gap-2">
                              {getStatusIcon(shipping.status)}
                              <span>{shipping.orderId}</span>
                            </div>
                          </TableCell>
                          <TableCell>{shipping.trackingNumber || '-'}</TableCell>
                          <TableCell>{shipping.carrier || '-'}</TableCell>
                          <TableCell>
                            <Badge className={getStatusColor(shipping.status)}>
                              {t(`lrm.${shipping.status}`)}
                            </Badge>
                          </TableCell>
                          <TableCell>
                            {shipping.estimatedDelivery ? 
                              format(new Date(shipping.estimatedDelivery), 'MMM dd, yyyy') : 
                              '-'
                            }
                          </TableCell>
                          <TableCell>
                            {shipping.actualDelivery ? 
                              format(new Date(shipping.actualDelivery), 'MMM dd, yyyy') : 
                              '-'
                            }
                          </TableCell>
                          <TableCell>
                            <div className="flex gap-2">
                              <Button variant="ghost" size="sm">
                                <Eye className="h-4 w-4" />
                              </Button>
                              <PermissionWrapper permission={PERMISSIONS.CONFIRM_DELIVERIES}>
                                {shipping.status === 'shipped' && (
                                  <Button 
                                    variant="ghost" 
                                    size="sm"
                                    onClick={() => handleConfirmDelivery(shipping.orderId)}
                                  >
                                    <CheckCircle className="h-4 w-4" />
                                  </Button>
                                )}
                              </PermissionWrapper>
                            </div>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>

                  {/* Pagination */}
                  <PaginationCustom
                    currentPage={shippingCurrentPage}
                    totalPages={shippingTotalPages}
                    itemsPerPage={shippingItemsPerPage}
                    totalItems={shippingTotalItems}
                    onPageChange={shippingGoToPage}
                    onItemsPerPageChange={setShippingItemsPerPage}
                  />
                </>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="transfers" className="space-y-4">
          <Card>
            <CardContent className="p-6">
              <div className="flex gap-4 mb-6">
                <div className="relative flex-1">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-4 w-4" />
                  <Input
                    placeholder={t('lrm.searchTransfers')}
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
                    <SelectItem value="pending">{t('lrm.pending')}</SelectItem>
                    <SelectItem value="in_transit">{t('lrm.inTransit')}</SelectItem>
                    <SelectItem value="delivered">{t('lrm.delivered')}</SelectItem>
                    <SelectItem value="cancelled">{t('lrm.cancelled')}</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              {transfersTotalItems === 0 ? (
                <div className="text-center py-8">
                  <p className="text-muted-foreground">No transfers found</p>
                </div>
              ) : (
                <>
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>{t('lrm.transferId')}</TableHead>
                        <TableHead>{t('lrm.from')}</TableHead>
                        <TableHead>{t('lrm.to')}</TableHead>
                        <TableHead>{t('lrm.items')}</TableHead>
                        <TableHead>{t('lrm.status')}</TableHead>
                        <TableHead>{t('lrm.requestedBy')}</TableHead>
                        <TableHead>{t('lrm.deliveryDate')}</TableHead>
                        <TableHead>{t('common.actions')}</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {paginatedTransfers.map((transfer) => (
                        <TableRow key={transfer.id}>
                          <TableCell>{transfer.id}</TableCell>
                          <TableCell>
                            <div className="flex items-center gap-2">
                              <MapPin className="h-4 w-4 text-muted-foreground" />
                              {transfer.fromWarehouse}
                            </div>
                          </TableCell>
                          <TableCell>
                            <div className="flex items-center gap-2">
                              <MapPin className="h-4 w-4 text-muted-foreground" />
                              {transfer.toWarehouse}
                            </div>
                          </TableCell>
                          <TableCell>{transfer.items.length} items</TableCell>
                          <TableCell>
                            <Badge className={getStatusColor(transfer.status)}>
                              {t(`lrm.${transfer.status}`)}
                            </Badge>
                          </TableCell>
                          <TableCell>{transfer.requestedBy}</TableCell>
                          <TableCell>
                            {transfer.deliveryDate ? 
                              format(new Date(transfer.deliveryDate), 'MMM dd, yyyy') : 
                              '-'
                            }
                          </TableCell>
                          <TableCell>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => setSelectedTransfer(transfer)}
                            >
                              <Eye className="h-4 w-4" />
                            </Button>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>

                  {/* Pagination */}
                  <PaginationCustom
                    currentPage={transfersCurrentPage}
                    totalPages={transfersTotalPages}
                    itemsPerPage={transfersItemsPerPage}
                    totalItems={transfersTotalItems}
                    onPageChange={transfersGoToPage}
                    onItemsPerPageChange={setTransfersItemsPerPage}
                  />
                </>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="suppliers" className="space-y-4">
          <PermissionWrapper permission={PERMISSIONS.MANAGE_SUPPLIERS}>
            <Card>
              <CardContent className="p-6">
                <div className="flex gap-4 mb-6">
                  <div className="relative flex-1">
                    <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-4 w-4" />
                    <Input
                      placeholder={t('lrm.searchSuppliers')}
                      value={searchTerm}
                      onChange={(e) => setSearchTerm(e.target.value)}
                      className="pl-10"
                    />
                  </div>
                  <Button>
                    <Plus className="h-4 w-4 mr-2" />
                    {t('lrm.addSupplier')}
                  </Button>
                </div>

                {suppliersTotalItems === 0 ? (
                  <div className="text-center py-8">
                    <p className="text-muted-foreground">No suppliers found</p>
                  </div>
                ) : (
                  <>
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>{t('lrm.supplier')}</TableHead>
                          <TableHead>{t('lrm.contact')}</TableHead>
                          <TableHead>{t('lrm.products')}</TableHead>
                          <TableHead>{t('lrm.paymentTerms')}</TableHead>
                          <TableHead>{t('lrm.deliverySchedule')}</TableHead>
                          <TableHead>{t('lrm.status')}</TableHead>
                          <TableHead>{t('common.actions')}</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {paginatedSuppliers.map((supplier) => (
                          <TableRow key={supplier.id}>
                            <TableCell>
                              <div>
                                <div>{supplier.name}</div>
                                <div className="text-xs text-muted-foreground">{supplier.address}</div>
                              </div>
                            </TableCell>
                            <TableCell>
                              <div className="space-y-1">
                                <div className="text-sm">{supplier.email}</div>
                                <div className="text-xs text-muted-foreground">{supplier.phone}</div>
                              </div>
                            </TableCell>
                            <TableCell>{supplier.products.length} products</TableCell>
                            <TableCell>{supplier.paymentTerms}</TableCell>
                            <TableCell>{supplier.deliverySchedule}</TableCell>
                            <TableCell>
                              <Badge className={getStatusColor(supplier.status)}>
                                {t(`lrm.${supplier.status}`)}
                              </Badge>
                            </TableCell>
                            <TableCell>
                              <Button variant="ghost" size="sm">
                                <Eye className="h-4 w-4" />
                              </Button>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>

                    {/* Pagination */}
                    <PaginationCustom
                      currentPage={suppliersCurrentPage}
                      totalPages={suppliersTotalPages}
                      itemsPerPage={suppliersItemsPerPage}
                      totalItems={suppliersTotalItems}
                      onPageChange={suppliersGoToPage}
                      onItemsPerPageChange={setSuppliersItemsPerPage}
                    />
                  </>
                )}
              </CardContent>
            </Card>
          </PermissionWrapper>
        </TabsContent>
      </Tabs>
    </div>
  );
}
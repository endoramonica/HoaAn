import React, { useState, useMemo } from 'react';
import { Search, Plus, Phone, Mail, Building2, User, Filter, Download, Eye, Edit } from 'lucide-react';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Badge } from '../components/ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../components/ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../components/ui/table';
import { Avatar, AvatarFallback } from '../components/ui/avatar';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../components/ui/tabs';
import { PaginationCustom } from '../components/ui/pagination-custom';
import { usePagination } from '../hooks/usePagination';
import { PermissionWrapper } from '../components/PermissionWrapper';
import { useAuth } from '../contexts/AuthContext';
import { useI18n } from '../contexts/I18nContext';
import { mockData } from '../services/mockData';
import { Customer, CRMInteraction } from '../types';
import { PERMISSIONS } from '../utils/constants';
import { format } from 'date-fns';
import { toast } from 'sonner';

export function CRMPage() {
  const { user } = useAuth();
  const { t } = useI18n();
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);
  const [activeTab, setActiveTab] = useState('customers');

  // Mock data - in real app this would come from API
  const customers: Customer[] = mockData.customers || [];
  const interactions: CRMInteraction[] = mockData.crmInteractions || [];

  const filteredCustomers = useMemo(() => {
    return customers.filter(customer => {
      const matchesSearch = customer.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                           customer.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
                           customer.phone.includes(searchTerm);
      const matchesStatus = statusFilter === 'all' || customer.status === statusFilter;
      return matchesSearch && matchesStatus;
    });
  }, [customers, searchTerm, statusFilter]);

  const {
    currentPage,
    totalPages,
    itemsPerPage,
    totalItems,
    paginatedData: paginatedCustomers,
    goToPage,
    setItemsPerPage,
  } = usePagination({
    data: filteredCustomers,
    initialItemsPerPage: 10,
  });

  const customerInteractions = useMemo(() => {
    if (!selectedCustomer) return [];
    return interactions.filter(interaction => interaction.customerId === selectedCustomer.id);
  }, [interactions, selectedCustomer]);

  const handleViewCustomer = (customer: Customer) => {
    setSelectedCustomer(customer);
    setActiveTab('details');
  };

  const handleAddInteraction = () => {
    if (!selectedCustomer) return;
    
    // Mock interaction creation
    toast.success(t('crm.interactionAdded'));
  };

  const handleExportCustomers = () => {
    toast.success(t('crm.exportStarted'));
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'lead': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'customer': return 'bg-green-100 text-green-800 border-green-200';
      case 'inactive': return 'bg-gray-100 text-gray-800 border-gray-200';
      default: return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getInteractionTypeIcon = (type: string) => {
    switch (type) {
      case 'call': return <Phone className="h-4 w-4" />;
      case 'email': return <Mail className="h-4 w-4" />;
      case 'meeting': return <User className="h-4 w-4" />;
      default: return <Edit className="h-4 w-4" />;
    }
  };

  return (
    <div className="p-6 space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1>{t('crm.title')}</h1>
          <p className="text-muted-foreground">{t('crm.subtitle')}</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={handleExportCustomers}>
            <Download className="h-4 w-4 mr-2" />
            {t('common.export')}
          </Button>
          <PermissionWrapper permission={PERMISSIONS.EDIT_CRM}>
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              {t('crm.addCustomer')}
            </Button>
          </PermissionWrapper>
        </div>
      </div>

      <Tabs value={activeTab} onValueChange={setActiveTab}>
        <TabsList>
          <TabsTrigger value="customers">{t('crm.customers')}</TabsTrigger>
          <TabsTrigger value="details" disabled={!selectedCustomer}>
            {t('crm.customerDetails')}
          </TabsTrigger>
          <PermissionWrapper permission={PERMISSIONS.VIEW_CRM_ANALYTICS}>
            <TabsTrigger value="analytics">{t('crm.analytics')}</TabsTrigger>
          </PermissionWrapper>
        </TabsList>

        <TabsContent value="customers" className="space-y-4">
          <Card>
            <CardContent className="p-6">
              <div className="flex gap-4 mb-6">
                <div className="relative flex-1">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-4 w-4" />
                  <Input
                    placeholder={t('crm.searchCustomers')}
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
                    <SelectItem value="lead">{t('crm.lead')}</SelectItem>
                    <SelectItem value="customer">{t('crm.customer')}</SelectItem>
                    <SelectItem value="inactive">{t('crm.inactive')}</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              {totalItems === 0 ? (
                <div className="text-center py-8">
                  <p className="text-muted-foreground">No customers found</p>
                </div>
              ) : (
                <>
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>{t('crm.customer')}</TableHead>
                        <TableHead>{t('crm.contact')}</TableHead>
                        <TableHead>{t('crm.status')}</TableHead>
                        <TableHead>{t('crm.totalOrders')}</TableHead>
                        <TableHead>{t('crm.totalSpent')}</TableHead>
                        <TableHead>{t('crm.lastOrder')}</TableHead>
                        <TableHead>{t('common.actions')}</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {paginatedCustomers.map((customer) => (
                        <TableRow key={customer.id}>
                          <TableCell>
                            <div className="flex items-center gap-3">
                              <Avatar className="h-8 w-8">
                                <AvatarFallback>
                                  {customer.name.split(' ').map(n => n[0]).join('')}
                                </AvatarFallback>
                              </Avatar>
                              <div>
                                <div>{customer.name}</div>
                                {customer.company && (
                                  <div className="text-xs text-muted-foreground flex items-center gap-1">
                                    <Building2 className="h-3 w-3" />
                                    {customer.company}
                                  </div>
                                )}
                              </div>
                            </div>
                          </TableCell>
                          <TableCell>
                            <div className="space-y-1">
                              <div className="text-sm">{customer.email}</div>
                              <div className="text-xs text-muted-foreground">{customer.phone}</div>
                            </div>
                          </TableCell>
                          <TableCell>
                            <Badge className={getStatusColor(customer.status)}>
                              {t(`crm.${customer.status}`)}
                            </Badge>
                          </TableCell>
                          <TableCell>{customer.totalOrders}</TableCell>
                          <TableCell>${customer.totalSpent.toFixed(2)}</TableCell>
                          <TableCell>
                            {customer.lastOrderDate ? 
                              format(new Date(customer.lastOrderDate), 'MMM dd, yyyy') : 
                              t('common.never')
                            }
                          </TableCell>
                          <TableCell>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => handleViewCustomer(customer)}
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
                    currentPage={currentPage}
                    totalPages={totalPages}
                    itemsPerPage={itemsPerPage}
                    totalItems={totalItems}
                    onPageChange={goToPage}
                    onItemsPerPageChange={setItemsPerPage}
                  />
                </>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="details" className="space-y-4">
          {selectedCustomer && (
            <>
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-3">
                    <Avatar className="h-10 w-10">
                      <AvatarFallback>
                        {selectedCustomer.name.split(' ').map(n => n[0]).join('')}
                      </AvatarFallback>
                    </Avatar>
                    <div>
                      <div>{selectedCustomer.name}</div>
                      <div className="text-sm text-muted-foreground">{selectedCustomer.email}</div>
                    </div>
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <Card>
                      <CardContent className="p-4">
                        <div className="text-sm text-muted-foreground">{t('crm.totalOrders')}</div>
                        <div className="text-2xl">{selectedCustomer.totalOrders}</div>
                      </CardContent>
                    </Card>
                    <Card>
                      <CardContent className="p-4">
                        <div className="text-sm text-muted-foreground">{t('crm.totalSpent')}</div>
                        <div className="text-2xl">${selectedCustomer.totalSpent.toFixed(2)}</div>
                      </CardContent>
                    </Card>
                    <Card>
                      <CardContent className="p-4">
                        <div className="text-sm text-muted-foreground">{t('crm.status')}</div>
                        <Badge className={getStatusColor(selectedCustomer.status)}>
                          {t(`crm.${selectedCustomer.status}`)}
                        </Badge>
                      </CardContent>
                    </Card>
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <div className="flex justify-between items-center">
                    <CardTitle>{t('crm.interactions')}</CardTitle>
                    <PermissionWrapper permission={PERMISSIONS.CREATE_CRM_INTERACTIONS}>
                      <Button onClick={handleAddInteraction}>
                        <Plus className="h-4 w-4 mr-2" />
                        {t('crm.addInteraction')}
                      </Button>
                    </PermissionWrapper>
                  </div>
                </CardHeader>
                <CardContent>
                  <div className="space-y-4">
                    {customerInteractions.map((interaction) => (
                      <div key={interaction.id} className="flex gap-4 p-4 border rounded-lg">
                        <div className="flex-shrink-0">
                          {getInteractionTypeIcon(interaction.type)}
                        </div>
                        <div className="flex-1">
                          <div className="flex justify-between items-start">
                            <div>
                              <h4>{interaction.title}</h4>
                              <p className="text-sm text-muted-foreground">{interaction.description}</p>
                            </div>
                            <Badge variant={interaction.status === 'completed' ? 'default' : 'secondary'}>
                              {t(`common.${interaction.status}`)}
                            </Badge>
                          </div>
                          <div className="text-xs text-muted-foreground mt-2">
                            {format(new Date(interaction.createdAt), 'MMM dd, yyyy HH:mm')}
                          </div>
                        </div>
                      </div>
                    ))}
                    {customerInteractions.length === 0 && (
                      <div className="text-center py-8 text-muted-foreground">
                        {t('crm.noInteractions')}
                      </div>
                    )}
                  </div>
                </CardContent>
              </Card>
            </>
          )}
        </TabsContent>

        <TabsContent value="analytics" className="space-y-4">
          <PermissionWrapper permission={PERMISSIONS.VIEW_CRM_ANALYTICS}>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <Card>
                <CardContent className="p-6">
                  <div className="text-sm text-muted-foreground">{t('crm.totalCustomers')}</div>
                  <div className="text-3xl">{customers.length}</div>
                  <div className="text-sm text-green-600">↑ 12% {t('crm.fromLastMonth')}</div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-6">
                  <div className="text-sm text-muted-foreground">{t('crm.activeLeads')}</div>
                  <div className="text-3xl">{customers.filter(c => c.status === 'lead').length}</div>
                  <div className="text-sm text-blue-600">↑ 8% {t('crm.fromLastMonth')}</div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-6">
                  <div className="text-sm text-muted-foreground">{t('crm.conversionRate')}</div>
                  <div className="text-3xl">68%</div>
                  <div className="text-sm text-green-600">↑ 5% {t('crm.fromLastMonth')}</div>
                </CardContent>
              </Card>
            </div>
          </PermissionWrapper>
        </TabsContent>
      </Tabs>
    </div>
  );
}
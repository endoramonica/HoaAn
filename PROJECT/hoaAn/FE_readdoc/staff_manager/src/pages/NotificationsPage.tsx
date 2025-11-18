import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useI18n } from '../contexts/I18nContext';
import { Card, CardContent, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';
import { PaginationCustom } from '../components/ui/pagination-custom';
import { usePaginatedApi } from '../hooks/usePaginatedApi';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../components/ui/tabs';
import { 
  Bell, Check, CheckCheck, Trash2, ShoppingCart, 
  Package, Users, AlertCircle, Settings, Filter, Loader2 
} from 'lucide-react';
import { toast } from 'sonner';
import { api } from '../services/api';
import { Notification as NotificationType } from '../types';

export const NotificationsPage: React.FC = () => {
  const { user } = useAuth();
  const { t } = useI18n();
  const [filter, setFilter] = useState<'all' | 'unread' | 'read'>('all');
  const [typeFilter, setTypeFilter] = useState<string>('all');
  
  // Server-side pagination
  const {
    data: notifications,
    meta,
    isLoading,
    page,
    pageSize,
    setPage,
    setPageSize,
    setFilters,
    refetch,
  } = usePaginatedApi<NotificationType>({
    fetchFn: api.fetchNotifications,
    initialPageSize: 10,
    initialFilters: {
      userId: user?.id,
      isRead: filter === 'all' ? undefined : filter === 'read',
      type: typeFilter === 'all' ? undefined : typeFilter,
    },
  });

  // Update filters when filter changes
  useEffect(() => {
    setFilters({
      userId: user?.id,
      isRead: filter === 'all' ? undefined : filter === 'read',
      type: typeFilter === 'all' ? undefined : typeFilter,
    });
  }, [filter, typeFilter, user?.id, setFilters]);

  const unreadCount = notifications.filter(n => !n.isRead).length;

  const getNotificationIcon = (type: string) => {
    switch (type) {
      case 'order': return <ShoppingCart className="h-5 w-5" />;
      case 'inventory': return <Package className="h-5 w-5" />;
      case 'staff': return <Users className="h-5 w-5" />;
      case 'system': return <Settings className="h-5 w-5" />;
      default: return <Bell className="h-5 w-5" />;
    }
  };

  const markAsRead = (id: string) => {
    // In real app, call API to mark as read
    toast.success('Marked as read');
    refetch();
  };

  const markAllAsRead = () => {
    // In real app, call API to mark all as read
    toast.success('All notifications marked as read');
    refetch();
  };

  const deleteNotification = (id: string) => {
    // In real app, call API to delete notification
    toast.success('Notification deleted');
    refetch();
  };

  const deleteAllRead = () => {
    // In real app, call API to delete all read notifications
    toast.success('All read notifications deleted');
    refetch();
  };

  return (
    <div className="space-y-6">
      <Breadcrumb items={breadcrumbItems} />
      
      <div className="flex items-center justify-between">
        <div>
          <div className="flex items-center gap-3">
            <h1 className="mb-2">{t('notifications.title')}</h1>
            {unreadCount > 0 && (
              <Badge variant="destructive" className="rounded-full">
                {unreadCount}
              </Badge>
            )}
          </div>
          <p className="text-muted-foreground">{t('notifications.description')}</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={deleteAllRead}>
            <Trash2 className="mr-2 h-4 w-4" />
            {t('notifications.deleteRead')}
          </Button>
          <Button onClick={markAllAsRead}>
            <CheckCheck className="mr-2 h-4 w-4" />
            {t('notifications.markAllRead')}
          </Button>
        </div>
      </div>

      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <Bell className="h-5 w-5" />
              {t('notifications.list')}
            </CardTitle>
            <div className="flex items-center gap-2">
              <Filter className="h-4 w-4 text-muted-foreground" />
              <Tabs value={filter} onValueChange={(v) => setFilter(v as any)} className="w-auto">
                <TabsList>
                  <TabsTrigger value="all">{t('notifications.all')}</TabsTrigger>
                  <TabsTrigger value="unread">{t('notifications.unread')}</TabsTrigger>
                  <TabsTrigger value="read">{t('notifications.read')}</TabsTrigger>
                </TabsList>
              </Tabs>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          <Tabs value={typeFilter} onValueChange={(v) => setTypeFilter(v as any)} className="mb-4">
            <TabsList className="grid w-full grid-cols-5">
              <TabsTrigger value="all">
                {t('notifications.allTypes')}
              </TabsTrigger>
              <TabsTrigger value="order">
                <ShoppingCart className="mr-2 h-4 w-4" />
                {t('notifications.orders')}
              </TabsTrigger>
              <TabsTrigger value="inventory">
                <Package className="mr-2 h-4 w-4" />
                {t('notifications.inventory')}
              </TabsTrigger>
              <TabsTrigger value="staff">
                <Users className="mr-2 h-4 w-4" />
                {t('notifications.staff')}
              </TabsTrigger>
              <TabsTrigger value="system">
                <Settings className="mr-2 h-4 w-4" />
                {t('notifications.system')}
              </TabsTrigger>
            </TabsList>
          </Tabs>

          {paginatedData.length === 0 ? (
            <div className="text-center py-12">
              <Bell className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
              <p className="text-muted-foreground">{t('notifications.noNotifications')}</p>
            </div>
          ) : (
            <div className="space-y-2">
              {paginatedData.map((notification) => (
                <div
                  key={notification.id}
                  className={`flex items-start gap-4 p-4 rounded-lg border transition-colors ${
                    !notification.isRead 
                      ? 'bg-primary/5 border-primary/20' 
                      : 'hover:bg-muted/50'
                  }`}
                >
                  <div className={`p-2 rounded-lg ${getPriorityColor(notification.priority)}`}>
                    {getNotificationIcon(notification.type)}
                  </div>
                  
                  <div className="flex-1 space-y-1">
                    <div className="flex items-start justify-between gap-4">
                      <div className="flex-1">
                        <p className={`font-medium ${!notification.isRead && 'text-primary'}`}>
                          {notification.title}
                        </p>
                        <p className="text-sm text-muted-foreground">
                          {notification.message}
                        </p>
                      </div>
                      {!notification.isRead && (
                        <Badge variant="secondary" className="shrink-0">
                          {t('notifications.new')}
                        </Badge>
                      )}
                    </div>
                    
                    <div className="flex items-center gap-4 text-xs text-muted-foreground">
                      <span>{new Date(notification.timestamp).toLocaleString()}</span>
                      <Badge variant="outline" className="text-xs">
                        {notification.type}
                      </Badge>
                      <Badge variant="outline" className="text-xs">
                        {notification.priority}
                      </Badge>
                    </div>
                  </div>

                  <div className="flex gap-2">
                    {!notification.isRead && (
                      <Button
                        variant="ghost"
                        size="icon"
                        onClick={() => markAsRead(notification.id)}
                      >
                        <Check className="h-4 w-4" />
                      </Button>
                    )}
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => deleteNotification(notification.id)}
                    >
                      <Trash2 className="h-4 w-4 text-destructive" />
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          )}

          {paginatedData.length > 0 && (
            <div className="mt-6">
              <Pagination
                currentPage={currentPage}
                totalPages={totalPages}
                onPageChange={goToPage}
                onNext={nextPage}
                onPrevious={prevPage}
              />
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
};

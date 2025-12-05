import { useState } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { Modal } from '../components/ui/Modal';
import {
    useNotifications,
    useMarkNotificationRead,
    useMarkAllNotificationsRead,
    useClearAllNotifications,
} from '../lib/hooks/useNotifications';
import { toast } from 'sonner';
import { Bell, CheckCheck, Trash2 } from 'lucide-react';

export const NotificationsPage = () => {
    const [filter, setFilter] = useState<'all' | 'unread' | 'read'>('all');
    const [clearModalOpen, setClearModalOpen] = useState(false);

    const { data: notifications = [] } = useNotifications();
    const markRead = useMarkNotificationRead();
    const markAllRead = useMarkAllNotificationsRead();
    const clearAll = useClearAllNotifications();

    const filteredNotifications = notifications.filter((n) => {
        if (filter === 'unread') return !n.read;
        if (filter === 'read') return n.read;
        return true;
    });

    const handleMarkRead = (id: string) => {
        markRead.mutate(id);
    };

    const handleMarkAllRead = () => {
        markAllRead.mutate(undefined, {
            onSuccess: () => {
                toast.success('Đã đánh dấu tất cả là đã đọc');
            },
        });
    };

    const handleClearAll = () => {
        clearAll.mutate(undefined, {
            onSuccess: () => {
                toast.success('Đã xóa tất cả thông báo');
                setClearModalOpen(false);
            },
        });
    };

    const getTypeVariant = (type: string) => {
        switch (type) {
            case 'success': return 'success';
            case 'warning': return 'warning';
            case 'error': return 'danger';
            default: return 'info';
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">
                    Thông Báo
                </h1>
                <div className="flex items-center gap-2">
                    <Button
                        variant="ghost"
                        size="sm"
                        icon={<CheckCheck className="w-5 h-5" />}
                        onClick={handleMarkAllRead}
                    >
                        Đánh dấu tất cả
                    </Button>
                    <Button
                        variant="danger"
                        size="sm"
                        icon={<Trash2 className="w-5 h-5" />}
                        onClick={() => setClearModalOpen(true)}
                    >
                        Xóa tất cả
                    </Button>
                </div>
            </div>

            <Card>
                <div className="flex gap-2 mb-4">
                    <Button
                        variant={filter === 'all' ? 'primary' : 'ghost'}
                        size="sm"
                        onClick={() => setFilter('all')}
                    >
                        Tất cả
                    </Button>
                    <Button
                        variant={filter === 'unread' ? 'primary' : 'ghost'}
                        size="sm"
                        onClick={() => setFilter('unread')}
                    >
                        Chưa đọc
                    </Button>
                    <Button
                        variant={filter === 'read' ? 'primary' : 'ghost'}
                        size="sm"
                        onClick={() => setFilter('read')}
                    >
                        Đã đọc
                    </Button>
                </div>

                <div className="space-y-3">
                    {filteredNotifications.length === 0 ? (
                        <div className="text-center py-12">
                            <Bell className="w-12 h-12 mx-auto text-gray-400 mb-3" />
                            <p className="text-gray-500">
                                Không có thông báo
                            </p>
                        </div>
                    ) : (
                        filteredNotifications.map((notification) => (
                            <div
                                key={notification.id}
                                onClick={() => !notification.read && handleMarkRead(notification.id)}
                                className={`
                                    p-4 rounded-lg border cursor-pointer transition-colors
                                    ${
                                        notification.read
                                            ? 'bg-white border-gray-200'
                                            : 'bg-blue-50 border-blue-200'
                                    }
                                    hover:border-blue-400:border-blue-500
                                `}
                            >
                                <div className="flex items-start justify-between">
                                    <div className="flex-1">
                                        <div className="flex items-center gap-2 mb-1">
                                            <h3 className="font-semibold text-gray-900">
                                                {notification.title}
                                            </h3>
                                            <Badge variant={getTypeVariant(notification.type)} size="sm">
                                                {notification.type}
                                            </Badge>
                                        </div>
                                        <p className="text-gray-600 text-sm mb-2">
                                            {notification.message}
                                        </p>
                                        <p className="text-xs text-gray-500">
                                            {new Date(notification.createdAt).toLocaleString('vi-VN')}
                                        </p>
                                    </div>
                                    {!notification.read && (
                                        <div className="w-2 h-2 bg-blue-600 rounded-full mt-2" />
                                    )}
                                </div>
                            </div>
                        ))
                    )}
                </div>
            </Card>

            <Modal
                isOpen={clearModalOpen}
                onClose={() => setClearModalOpen(false)}
                title="Xác Nhận Xóa Tất Cả"
                footer={
                    <>
                        <Button variant="ghost" onClick={() => setClearModalOpen(false)}>
                            Hủy
                        </Button>
                        <Button
                            variant="danger"
                            onClick={handleClearAll}
                            isLoading={clearAll.isPending}
                        >
                            Xóa Tất Cả
                        </Button>
                    </>
                }
            >
                <p className="text-gray-700">
                    Bạn có chắc chắn muốn xóa tất cả thông báo? Hành động này không thể hoàn tác.
                </p>
            </Modal>
        </div>
    );
};

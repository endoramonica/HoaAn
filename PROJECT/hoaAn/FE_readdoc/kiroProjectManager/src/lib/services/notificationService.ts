/**
 * Notification Service
 * LocalStorage-based service for notifications
 */

export interface Notification {
    id: string;
    title: string;
    message: string;
    type: 'info' | 'success' | 'warning' | 'error';
    read: boolean;
    createdAt: string;
}

const STORAGE_KEY = 'notifications';

export const notificationService = {
    async getNotifications(): Promise<Notification[]> {
        const data = localStorage.getItem(STORAGE_KEY);
        return data ? JSON.parse(data) : [];
    },

    async createNotification(notification: Omit<Notification, 'id' | 'read' | 'createdAt'>): Promise<Notification> {
        const notifications = await this.getNotifications();
        const newNotification: Notification = {
            ...notification,
            id: crypto.randomUUID(),
            read: false,
            createdAt: new Date().toISOString(),
        };
        notifications.unshift(newNotification); // Add to beginning
        localStorage.setItem(STORAGE_KEY, JSON.stringify(notifications));
        return newNotification;
    },

    async markAsRead(id: string): Promise<void> {
        const notifications = await this.getNotifications();
        const notification = notifications.find(n => n.id === id);
        if (notification) {
            notification.read = true;
            localStorage.setItem(STORAGE_KEY, JSON.stringify(notifications));
        }
    },

    async markAllAsRead(): Promise<void> {
        const notifications = await this.getNotifications();
        notifications.forEach(n => n.read = true);
        localStorage.setItem(STORAGE_KEY, JSON.stringify(notifications));
    },

    async deleteNotification(id: string): Promise<void> {
        const notifications = await this.getNotifications();
        const filtered = notifications.filter(n => n.id !== id);
        localStorage.setItem(STORAGE_KEY, JSON.stringify(filtered));
    },

    async clearAll(): Promise<void> {
        localStorage.setItem(STORAGE_KEY, JSON.stringify([]));
    },

    async getUnreadCount(): Promise<number> {
        const notifications = await this.getNotifications();
        return notifications.filter(n => !n.read).length;
    },
};

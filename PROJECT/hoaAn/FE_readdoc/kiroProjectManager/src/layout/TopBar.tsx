import { Menu, Bell, LogOut, User } from 'lucide-react';
import { useUnreadCount } from '../lib/hooks/useNotifications';
import { useAuth } from '../lib/contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import { toast } from 'sonner';

interface TopBarProps {
    onMenuClick: () => void;
}

export const TopBar: React.FC<TopBarProps> = ({ onMenuClick }) => {
    const { data: unreadCount = 0 } = useUnreadCount();
    const { logout, user, roles } = useAuth();
    const navigate = useNavigate();

    const handleLogout = async () => {
        try {
            await logout();
            toast.success('Đăng xuất thành công');
        } catch (error) {
            toast.error('Đăng xuất thất bại');
        }
    };

    return (
        <header className="bg-white border-b border-gray-200 sticky top-0 z-20">
            <div className="flex items-center justify-between px-4 py-3">
                {/* Left side */}
                <div className="flex items-center gap-4">
                    <button
                        onClick={onMenuClick}
                        className="lg:hidden text-gray-500 hover:text-gray-700"
                    >
                        <Menu className="w-6 h-6" />
                    </button>
                    <h2 className="text-lg font-semibold text-gray-900">
                        Admin Dashboard
                    </h2>
                </div>

                {/* Right side */}
                <div className="flex items-center gap-3">
                    {/* User info */}
                    {user && (
                        <div className="hidden md:flex items-center gap-2 px-3 py-1.5 bg-gray-100 rounded-lg">
                            <User className="w-4 h-4 text-gray-600" />
                            <span className="text-sm text-gray-700">
                                {user.fullName || user.email}
                            </span>
                            {roles.length > 0 && (
                                <div className="flex gap-1">
                                    {roles.map((role, index) => (
                                        <span 
                                            key={index}
                                            className="text-xs px-2 py-0.5 bg-green-100 text-green-700 rounded"
                                        >
                                            {role}
                                        </span>
                                    ))}
                                </div>
                            )}
                        </div>
                    )}

                    {/* Notifications */}
                    <button
                        onClick={() => navigate('/notifications')}
                        className="relative p-2 text-gray-500 hover:text-gray-700 rounded-lg hover:bg-gray-100"
                        aria-label="Notifications"
                    >
                        <Bell className="w-5 h-5" />
                        {unreadCount > 0 && (
                            <span className="absolute top-1 right-1 w-4 h-4 bg-red-500 text-white text-xs rounded-full flex items-center justify-center">
                                {unreadCount > 9 ? '9+' : unreadCount}
                            </span>
                        )}
                    </button>

                    {/* Logout */}
                    <button
                        onClick={handleLogout}
                        className="p-2 text-gray-500 hover:text-red-600 rounded-lg hover:bg-gray-100"
                        aria-label="Logout"
                    >
                        <LogOut className="w-5 h-5" />
                    </button>
                </div>
            </div>
        </header>
    );
};

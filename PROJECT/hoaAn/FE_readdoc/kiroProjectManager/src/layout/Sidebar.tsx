import { Link, useLocation } from 'react-router-dom';
import {
    LayoutDashboard,
    ShoppingCart,
    Package,
    Users,
    ShoppingBag,
    UserCog,
    UsersRound,
    Warehouse,
    Megaphone,
    Bell,
    Settings,
    X,
    Clock,
    CheckSquare,
    Truck,
    FileText,
    ArrowRightLeft,
    CalendarDays,
} from 'lucide-react';

interface SidebarProps {
    isOpen: boolean;
    onClose: () => void;
}

const navigation = [
    { name: 'Tổng Quan', href: '/', icon: LayoutDashboard },
    { name: 'Bán Hàng', href: '/pos', icon: ShoppingCart },
    { name: 'Sản Phẩm', href: '/products', icon: Package },
    { name: 'Khách Hàng', href: '/customers', icon: Users },
    { name: 'Đơn Hàng', href: '/orders', icon: ShoppingBag },
    { name: 'Nhân Viên', href: '/employees', icon: UserCog },
    { name: 'Người Dùng', href: '/users', icon: UsersRound },
    { name: 'Ca Làm Việc', href: '/shifts', icon: Clock },
    { name: 'Lịch Làm Việc', href: '/work-schedules', icon: CalendarDays },
    { name: 'Đơn Xin Nghỉ', href: '/leave-requests', icon: FileText },
    { name: 'Công Việc', href: '/tasks', icon: CheckSquare },
    { name: 'Kho Hàng', href: '/inventory', icon: Warehouse },
    { name: 'Chuyển Kho', href: '/stock-transfers', icon: ArrowRightLeft },
    { name: 'Nhà Cung Cấp', href: '/suppliers', icon: Truck },
    { name: 'Marketing', href: '/marketing', icon: Megaphone },
    { name: 'Thông Báo', href: '/notifications', icon: Bell },
    { name: 'Cài Đặt', href: '/settings', icon: Settings },
];

export const Sidebar: React.FC<SidebarProps> = ({ isOpen, onClose }) => {
    const location = useLocation();

    return (
        <>
            {/* Mobile backdrop */}
            {isOpen && (
                <div
                    className="fixed inset-0 bg-black bg-opacity-50 z-30 lg:hidden"
                    onClick={onClose}
                />
            )}

            {/* Sidebar */}
            <aside
                className={`
                    fixed top-0 left-0 z-40 h-full w-64 bg-white 
                    border-r border-gray-200 
                    transform transition-transform duration-300 ease-in-out
                    lg:translate-x-0
                    ${isOpen ? 'translate-x-0' : '-translate-x-full'}
                `}
            >
                <div className="flex flex-col h-full">
                    {/* Header */}
                    <div className="flex items-center justify-between p-4 border-b border-gray-200">
                        <h1 className="text-xl font-bold text-gray-900">
                            Đồ Cúng Store
                        </h1>
                        <button
                            onClick={onClose}
                            className="lg:hidden text-gray-500 hover:text-gray-700:text-gray-200"
                        >
                            <X className="w-6 h-6" />
                        </button>
                    </div>

                    {/* Navigation */}
                    <nav className="flex-1 overflow-y-auto p-4">
                        <ul className="space-y-2">
                            {navigation.map((item) => {
                                const isActive = location.pathname === item.href;
                                const Icon = item.icon;

                                return (
                                    <li key={item.name}>
                                        <Link
                                            to={item.href}
                                            onClick={() => onClose()}
                                            className={`
                                                flex items-center gap-3 px-4 py-3 rounded-lg
                                                transition-colors duration-200
                                                ${
                                                    isActive
                                                        ? 'bg-blue-50 text-blue-600'
                                                        : 'text-gray-700 hover:bg-gray-100:bg-gray-700'
                                                }
                                            `}
                                        >
                                            <Icon className="w-5 h-5" />
                                            <span className="font-medium">{item.name}</span>
                                        </Link>
                                    </li>
                                );
                            })}
                        </ul>
                    </nav>
                </div>
            </aside>
        </>
    );
};

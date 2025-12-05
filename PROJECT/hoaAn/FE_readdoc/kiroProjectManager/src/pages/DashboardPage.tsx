import { Card } from '../components/ui/Card';
import { DollarSign, Users, ShoppingBag, Package } from 'lucide-react';
import { LineChart, Line, BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';

const revenueData = [
    { month: 'T1', revenue: 45000000 },
    { month: 'T2', revenue: 52000000 },
    { month: 'T3', revenue: 48000000 },
    { month: 'T4', revenue: 61000000 },
    { month: 'T5', revenue: 55000000 },
    { month: 'T6', revenue: 67000000 },
];

const salesData = [
    { month: 'T1', sales: 120 },
    { month: 'T2', sales: 145 },
    { month: 'T3', sales: 132 },
    { month: 'T4', sales: 168 },
    { month: 'T5', sales: 155 },
    { month: 'T6', sales: 189 },
];

export const DashboardPage = () => {
    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND',
        }).format(value);
    };

    const stats = [
        {
            title: 'Tổng Doanh Thu',
            value: formatCurrency(328000000),
            icon: DollarSign,
            color: 'text-green-600',
            bgColor: 'bg-green-100',
        },
        {
            title: 'Khách Hàng',
            value: '1,234',
            icon: Users,
            color: 'text-blue-600',
            bgColor: 'bg-blue-100',
        },
        {
            title: 'Đơn Hàng',
            value: '909',
            icon: ShoppingBag,
            color: 'text-purple-600',
            bgColor: 'bg-purple-100',
        },
        {
            title: 'Sản Phẩm',
            value: '456',
            icon: Package,
            color: 'text-orange-600',
            bgColor: 'bg-orange-100',
        },
    ];

    return (
        <div className="space-y-6">
            <h1 className="text-2xl font-bold text-gray-900">
                Tổng Quan
            </h1>

            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                {stats.map((stat) => {
                    const Icon = stat.icon;
                    return (
                        <Card key={stat.title}>
                            <div className="flex items-center justify-between">
                                <div>
                                    <p className="text-sm text-gray-600">
                                        {stat.title}
                                    </p>
                                    <p className="text-2xl font-bold text-gray-900 mt-1">
                                        {stat.value}
                                    </p>
                                </div>
                                <div className={`p-3 rounded-lg ${stat.bgColor}`}>
                                    <Icon className={`w-6 h-6 ${stat.color}`} />
                                </div>
                            </div>
                        </Card>
                    );
                })}
            </div>

            {/* Charts */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                <Card title="Doanh Thu Theo Tháng">
                    <ResponsiveContainer width="100%" height={300}>
                        <LineChart data={revenueData}>
                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis dataKey="month" />
                            <YAxis />
                            <Tooltip formatter={(value: number) => formatCurrency(value)} />
                            <Line type="monotone" dataKey="revenue" stroke="#3B82F6" strokeWidth={2} />
                        </LineChart>
                    </ResponsiveContainer>
                </Card>

                <Card title="Đơn Hàng Theo Tháng">
                    <ResponsiveContainer width="100%" height={300}>
                        <BarChart data={salesData}>
                            <CartesianGrid strokeDasharray="3 3" />
                            <XAxis dataKey="month" />
                            <YAxis />
                            <Tooltip />
                            <Bar dataKey="sales" fill="#10B981" />
                        </BarChart>
                    </ResponsiveContainer>
                </Card>
            </div>
        </div>
    );
};

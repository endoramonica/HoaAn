import { useState } from 'react';
import { Card } from '../components/ui/Card';
import { Select } from '../components/ui/Select';
import { Badge } from '../components/ui/Badge';
import { useOrders } from '../lib/hooks/useOrders';

export const OrdersPage = () => {
    const [status, setStatus] = useState('');
    const { data: ordersData } = useOrders({ Status: status });
    const orders = ordersData?.data || [];

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND',
        }).format(value);
    };

    const statusOptions = [
        { value: '', label: 'Tất cả' },
        { value: 'PENDING', label: 'Chờ xử lý' },
        { value: 'PROCESSING', label: 'Đang xử lý' },
        { value: 'COMPLETED', label: 'Hoàn thành' },
        { value: 'CANCELLED', label: 'Đã hủy' },
    ];

    const getStatusVariant = (status: string) => {
        switch (status) {
            case 'COMPLETED': return 'success';
            case 'PROCESSING': return 'info';
            case 'CANCELLED': return 'danger';
            default: return 'warning';
        }
    };

    return (
        <div className="space-y-6">
            <h1 className="text-2xl font-bold text-gray-900">
                Quản Lý Đơn Hàng
            </h1>

            <Card>
                <div className="mb-4 w-64">
                    <Select
                        label="Trạng thái"
                        options={statusOptions}
                        value={status}
                        onChange={setStatus}
                    />
                </div>

                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Mã ĐH</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Khách Hàng</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Tổng Tiền</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Trạng Thái</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Ngày Tạo</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {orders.map((order: any) => (
                                <tr key={order.id} className="hover:bg-gray-50:bg-gray-700">
                                    <td className="px-4 py-3">{order.orderNumber}</td>
                                    <td className="px-4 py-3">{order.customerName || '-'}</td>
                                    <td className="px-4 py-3">{formatCurrency(order.totalAmount)}</td>
                                    <td className="px-4 py-3">
                                        <Badge variant={getStatusVariant(order.status)}>
                                            {statusOptions.find(s => s.value === order.status)?.label}
                                        </Badge>
                                    </td>
                                    <td className="px-4 py-3">
                                        {new Date(order.createdAt).toLocaleDateString('vi-VN')}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </Card>
        </div>
    );
};

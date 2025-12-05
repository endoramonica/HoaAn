import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { useCustomers } from '../lib/hooks/useCustomers';
import { Search, Plus, Edit, Trash2, Eye } from 'lucide-react';

export const CustomersPage = () => {
    const navigate = useNavigate();
    const [search, setSearch] = useState('');
    const { data: customersData } = useCustomers({ SearchTerm: search });
    
    // Extract customers from paginated response
    // Response structure: { data: { data: { items: [...], pageNumber, ... } } }
    const customers = customersData?.data?.data?.items || [];

    const formatCurrency = (value: number) => {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND',
        }).format(value);
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">
                    Quản Lý Khách Hàng
                </h1>
                <Button variant="primary" icon={<Plus className="w-5 h-5" />}>
                    Thêm Khách Hàng
                </Button>
            </div>

            <Card>
                <Input
                    placeholder="Tìm kiếm khách hàng..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                    icon={<Search className="w-5 h-5" />}
                    className="mb-4"
                />

                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Tên</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Số ĐT</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Email</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Tổng Chi Tiêu</th>
                                <th className="px-4 py-3 text-right text-sm font-semibold">Thao Tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {customers.map((customer: any) => (
                                <tr 
                                    key={customer.id} 
                                    className="hover:bg-gray-50 cursor-pointer transition-colors"
                                    onClick={() => navigate(`/customers/${customer.id}`)}
                                >
                                    <td className="px-4 py-3">{customer.name || customer.fullName}</td>
                                    <td className="px-4 py-3">{customer.phone || customer.phoneNumber}</td>
                                    <td className="px-4 py-3">{customer.email || '-'}</td>
                                    <td className="px-4 py-3">{formatCurrency(customer.totalSpent || 0)}</td>
                                    <td className="px-4 py-3 text-right">
                                        <div className="flex items-center justify-end gap-2" onClick={(e) => e.stopPropagation()}>
                                            <Button 
                                                variant="ghost" 
                                                size="sm" 
                                                icon={<Eye className="w-4 h-4" />}
                                                onClick={() => navigate(`/customers/${customer.id}`)}
                                            >
                                                Xem
                                            </Button>
                                            <Button variant="ghost" size="sm" icon={<Edit className="w-4 h-4" />}>
                                                Sửa
                                            </Button>
                                            <Button variant="danger" size="sm" icon={<Trash2 className="w-4 h-4" />}>
                                                Xóa
                                            </Button>
                                        </div>
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

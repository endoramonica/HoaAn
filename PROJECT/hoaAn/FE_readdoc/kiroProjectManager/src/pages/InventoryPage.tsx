import { Card } from '../components/ui/Card';
import { Select } from '../components/ui/Select';
import { Badge } from '../components/ui/Badge';
import { useInventory } from '../lib/hooks/useInventory';
import { AlertTriangle } from 'lucide-react';
import { useState } from 'react';

export const InventoryPage = () => {
    const [filter, setFilter] = useState('');
    const { data: inventoryData } = useInventory({ LowStock: filter === 'low' });
    
    // Debug logging
    console.log('🔍 InventoryPage - Full response:', inventoryData);
    console.log('🔍 InventoryPage - inventoryData?.data:', inventoryData?.data);
    console.log('🔍 InventoryPage - Is array?:', Array.isArray(inventoryData?.data));
    
    // Extract inventory from response
    // Response structure: { data: [...], totalCount, pageNumber, pageSize }
    const inventory = Array.isArray(inventoryData?.data) ? inventoryData.data : [];
    
    console.log('✅ InventoryPage - Final inventory:', inventory);
    console.log('✅ InventoryPage - Inventory count:', inventory.length);

    const filterOptions = [
        { value: '', label: 'Tất cả' },
        { value: 'low', label: 'Sắp hết hàng' },
        { value: 'out', label: 'Hết hàng' },
    ];

    return (
        <div className="space-y-6">
            <h1 className="text-2xl font-bold text-gray-900">
                Quản Lý Kho Hàng
            </h1>

            <Card>
                <div className="mb-4 w-64">
                    <Select
                        label="Lọc"
                        options={filterOptions}
                        value={filter}
                        onChange={setFilter}
                    />
                </div>

                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Sản Phẩm</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Số Lượng</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Tối Thiểu</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Vị Trí</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Trạng Thái</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {inventory.map((item: any) => {
                                const isLow = item.quantity <= item.minQuantity;
                                const isOut = item.quantity === 0;

                                return (
                                    <tr key={item.id} className="hover:bg-gray-50:bg-gray-700">
                                        <td className="px-4 py-3">{item.productName}</td>
                                        <td className="px-4 py-3">
                                            <span className={isOut ? 'text-red-600 font-semibold' : ''}>
                                                {item.quantity}
                                            </span>
                                        </td>
                                        <td className="px-4 py-3">{item.minQuantity}</td>
                                        <td className="px-4 py-3">{item.location || '-'}</td>
                                        <td className="px-4 py-3">
                                            {isOut ? (
                                                <Badge variant="danger">
                                                    <AlertTriangle className="w-3 h-3 mr-1" />
                                                    Hết hàng
                                                </Badge>
                                            ) : isLow ? (
                                                <Badge variant="warning">
                                                    <AlertTriangle className="w-3 h-3 mr-1" />
                                                    Sắp hết
                                                </Badge>
                                            ) : (
                                                <Badge variant="success">Đủ hàng</Badge>
                                            )}
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            </Card>
        </div>
    );
};

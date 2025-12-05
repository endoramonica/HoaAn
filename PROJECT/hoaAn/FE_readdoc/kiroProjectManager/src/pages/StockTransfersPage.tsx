import { useState } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { Plus, Package, ArrowRight, XCircle } from 'lucide-react';
import { useGetApiStockTransfers, usePostApiStockTransfers, usePatchApiStockTransfersIdCancel } from '../../api/generated-orval/stock-transfers/stock-transfers';

export const StockTransfersPage = () => {
    const [showModal, setShowModal] = useState(false);
    const [statusFilter, setStatusFilter] = useState('');

    const { data: transfersData, refetch } = useGetApiStockTransfers({ Status: statusFilter as any });
    const createMutation = usePostApiStockTransfers();
    const cancelMutation = usePatchApiStockTransfersIdCancel();

    const transfers = transfersData?.data?.data?.items || [];

    const handleCreate = async (data: any) => {
        try {
            await createMutation.mutateAsync({ data });
            setShowModal(false);
            refetch();
        } catch (error) {
            console.error('Error:', error);
        }
    };

    const handleCancel = async (id: string) => {
        const reason = prompt('Lý do hủy chuyển kho:');
        if (reason) {
            try {
                await cancelMutation.mutateAsync({ id, data: { reason } });
                refetch();
            } catch (error) {
                console.error('Error:', error);
            }
        }
    };

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'pending':
                return <Badge variant="warning">Chờ Xử Lý</Badge>;
            case 'inTransit':
                return <Badge variant="info">Đang Vận Chuyển</Badge>;
            case 'delivered':
                return <Badge variant="success">Đã Giao</Badge>;
            case 'cancelled':
                return <Badge variant="danger">Đã Hủy</Badge>;
            default:
                return <Badge variant="default">{status}</Badge>;
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">Quản Lý Chuyển Kho</h1>
                <Button 
                    variant="primary" 
                    icon={<Plus className="w-5 h-5" />}
                    onClick={() => setShowModal(true)}
                >
                    Tạo Phiếu Chuyển
                </Button>
            </div>

            <Card>
                <div className="mb-4">
                    <label className="block text-sm font-medium mb-1">Lọc Theo Trạng Thái</label>
                    <select 
                        className="px-3 py-2 border rounded-lg"
                        value={statusFilter}
                        onChange={(e) => setStatusFilter(e.target.value)}
                    >
                        <option value="">Tất Cả</option>
                        <option value="pending">Chờ Xử Lý</option>
                        <option value="inTransit">Đang Vận Chuyển</option>
                        <option value="delivered">Đã Giao</option>
                        <option value="cancelled">Đã Hủy</option>
                    </select>
                </div>
            </Card>

            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Mã Phiếu</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Chuyển Kho</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Sản Phẩm</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Số Lượng</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Ngày Tạo</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Trạng Thái</th>
                                <th className="px-4 py-3 text-right text-sm font-semibold">Thao Tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {transfers.map((transfer: any) => (
                                <tr key={transfer.id} className="hover:bg-gray-50">
                                    <td className="px-4 py-3 font-mono text-sm">{transfer.transferCode}</td>
                                    <td className="px-4 py-3">
                                        <div className="flex items-center gap-2">
                                            <div className="text-sm">
                                                <p className="font-medium">{transfer.fromStoreName}</p>
                                                <p className="text-gray-500 text-xs">Kho nguồn</p>
                                            </div>
                                            <ArrowRight className="w-4 h-4 text-gray-400" />
                                            <div className="text-sm">
                                                <p className="font-medium">{transfer.toStoreName}</p>
                                                <p className="text-gray-500 text-xs">Kho đích</p>
                                            </div>
                                        </div>
                                    </td>
                                    <td className="px-4 py-3">
                                        <div className="flex items-center gap-2">
                                            <Package className="w-4 h-4 text-gray-400" />
                                            <span className="text-sm">{transfer.productName}</span>
                                        </div>
                                    </td>
                                    <td className="px-4 py-3 text-center font-medium">{transfer.quantity}</td>
                                    <td className="px-4 py-3 text-sm">
                                        {new Date(transfer.createdAt).toLocaleDateString('vi-VN')}
                                    </td>
                                    <td className="px-4 py-3">{getStatusBadge(transfer.status)}</td>
                                    <td className="px-4 py-3 text-right">
                                        {(transfer.status === 'pending' || transfer.status === 'inTransit') && (
                                            <Button 
                                                variant="danger" 
                                                size="sm"
                                                icon={<XCircle className="w-4 h-4" />}
                                                onClick={() => handleCancel(transfer.id)}
                                            >
                                                Hủy
                                            </Button>
                                        )}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </Card>

            {showModal && (
                <StockTransferFormModal 
                    onClose={() => setShowModal(false)}
                    onSubmit={handleCreate}
                />
            )}
        </div>
    );
};

const StockTransferFormModal = ({ onClose, onSubmit }: any) => {
    const [formData, setFormData] = useState({
        fromStoreId: '',
        toStoreId: '',
        productId: '',
        quantity: 1,
        notes: ''
    });

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <Card className="w-full max-w-md">
                <h2 className="text-xl font-bold mb-4">Tạo Phiếu Chuyển Kho</h2>
                <div className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium mb-1">Kho Nguồn (ID) *</label>
                        <input 
                            type="text"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.fromStoreId}
                            onChange={(e) => setFormData({...formData, fromStoreId: e.target.value})}
                            required
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Kho Đích (ID) *</label>
                        <input 
                            type="text"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.toStoreId}
                            onChange={(e) => setFormData({...formData, toStoreId: e.target.value})}
                            required
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Sản Phẩm (ID) *</label>
                        <input 
                            type="text"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.productId}
                            onChange={(e) => setFormData({...formData, productId: e.target.value})}
                            required
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Số Lượng *</label>
                        <input 
                            type="number"
                            min="1"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.quantity}
                            onChange={(e) => setFormData({...formData, quantity: Number(e.target.value)})}
                            required
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Ghi Chú</label>
                        <textarea 
                            className="w-full px-3 py-2 border rounded-lg"
                            rows={3}
                            value={formData.notes}
                            onChange={(e) => setFormData({...formData, notes: e.target.value})}
                        />
                    </div>
                    <div className="flex gap-2 justify-end pt-4">
                        <Button variant="ghost" onClick={onClose}>Hủy</Button>
                        <Button variant="primary" onClick={() => onSubmit(formData)}>
                            Tạo Phiếu
                        </Button>
                    </div>
                </div>
            </Card>
        </div>
    );
};

import { useState } from 'react';
import { useGetApiShifts, usePostApiShiftsOpen, usePostApiShiftsCloseId } from '../../api/generated-orval/shifts/shifts';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { Plus, Clock, DollarSign } from 'lucide-react';
import { useAuth } from '../lib/contexts/AuthContext';

export const ShiftsPage = () => {
    const { user } = useAuth();
    const [showOpenModal, setShowOpenModal] = useState(false);
    const [showCloseModal, setShowCloseModal] = useState(false);
    const [selectedShift, setSelectedShift] = useState<any>(null);

    const { data: shiftsData, refetch } = useGetApiShifts({});
    const openShiftMutation = usePostApiShiftsOpen();
    const closeShiftMutation = usePostApiShiftsCloseId();

    const shifts = shiftsData?.data?.data?.items || [];

    const handleOpenShift = async (data: { storeId: string; openingCash: number; notes?: string }) => {
        try {
            await openShiftMutation.mutateAsync({ data });
            setShowOpenModal(false);
            refetch();
        } catch (error) {
            console.error('Error opening shift:', error);
        }
    };

    const handleCloseShift = async (shiftId: string, data: { closingCash: number; notes?: string }) => {
        try {
            await closeShiftMutation.mutateAsync({ id: shiftId, data });
            setShowCloseModal(false);
            setSelectedShift(null);
            refetch();
        } catch (error) {
            console.error('Error closing shift:', error);
        }
    };

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'open':
                return <Badge variant="success">Đang Mở</Badge>;
            case 'closed':
                return <Badge variant="default">Đã Đóng</Badge>;
            default:
                return <Badge variant="default">{status}</Badge>;
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">Quản Lý Ca Làm</h1>
                <Button 
                    variant="primary" 
                    icon={<Plus className="w-5 h-5" />}
                    onClick={() => setShowOpenModal(true)}
                >
                    Mở Ca Mới
                </Button>
            </div>

            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Nhân Viên</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Cửa Hàng</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Thời Gian Bắt Đầu</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Thời Gian Kết Thúc</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Tiền Mở Ca</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Tiền Đóng Ca</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Trạng Thái</th>
                                <th className="px-4 py-3 text-right text-sm font-semibold">Thao Tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {shifts.map((shift: any) => (
                                <tr key={shift.id} className="hover:bg-gray-50">
                                    <td className="px-4 py-3">{shift.userName || 'N/A'}</td>
                                    <td className="px-4 py-3">{shift.storeName || 'N/A'}</td>
                                    <td className="px-4 py-3">
                                        <div className="flex items-center gap-2">
                                            <Clock className="w-4 h-4 text-gray-400" />
                                            {shift.startTime ? new Date(shift.startTime).toLocaleString('vi-VN') : 'N/A'}
                                        </div>
                                    </td>
                                    <td className="px-4 py-3">
                                        {shift.endTime ? new Date(shift.endTime).toLocaleString('vi-VN') : '-'}
                                    </td>
                                    <td className="px-4 py-3">
                                        <div className="flex items-center gap-1">
                                            <DollarSign className="w-4 h-4 text-green-600" />
                                            {shift.openingCash?.toLocaleString('vi-VN')} đ
                                        </div>
                                    </td>
                                    <td className="px-4 py-3">
                                        {shift.closingCash ? (
                                            <div className="flex items-center gap-1">
                                                <DollarSign className="w-4 h-4 text-blue-600" />
                                                {shift.closingCash.toLocaleString('vi-VN')} đ
                                            </div>
                                        ) : '-'}
                                    </td>
                                    <td className="px-4 py-3">{getStatusBadge(shift.status)}</td>
                                    <td className="px-4 py-3 text-right">
                                        {shift.status === 'open' && (
                                            <Button 
                                                variant="danger" 
                                                size="sm"
                                                onClick={() => {
                                                    setSelectedShift(shift);
                                                    setShowCloseModal(true);
                                                }}
                                            >
                                                Đóng Ca
                                            </Button>
                                        )}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </Card>

            {/* Open Shift Modal */}
            {showOpenModal && (
                <OpenShiftModal 
                    onClose={() => setShowOpenModal(false)}
                    onSubmit={handleOpenShift}
                />
            )}

            {/* Close Shift Modal */}
            {showCloseModal && selectedShift && (
                <CloseShiftModal 
                    shift={selectedShift}
                    onClose={() => {
                        setShowCloseModal(false);
                        setSelectedShift(null);
                    }}
                    onSubmit={(data) => handleCloseShift(selectedShift.id, data)}
                />
            )}
        </div>
    );
};

// Open Shift Modal Component
const OpenShiftModal = ({ onClose, onSubmit }: any) => {
    const [formData, setFormData] = useState({
        storeId: '',
        openingCash: 0,
        notes: ''
    });

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <Card className="w-full max-w-md">
                <h2 className="text-xl font-bold mb-4">Mở Ca Làm Mới</h2>
                <div className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium mb-1">Cửa Hàng ID</label>
                        <input 
                            type="text"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.storeId}
                            onChange={(e) => setFormData({...formData, storeId: e.target.value})}
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Tiền Mở Ca (đ)</label>
                        <input 
                            type="number"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.openingCash}
                            onChange={(e) => setFormData({...formData, openingCash: Number(e.target.value)})}
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
                    <div className="flex gap-2 justify-end">
                        <Button variant="ghost" onClick={onClose}>Hủy</Button>
                        <Button variant="primary" onClick={() => onSubmit(formData)}>Mở Ca</Button>
                    </div>
                </div>
            </Card>
        </div>
    );
};

// Close Shift Modal Component
const CloseShiftModal = ({ shift, onClose, onSubmit }: any) => {
    const [formData, setFormData] = useState({
        closingCash: 0,
        notes: ''
    });

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <Card className="w-full max-w-md">
                <h2 className="text-xl font-bold mb-4">Đóng Ca Làm</h2>
                <div className="space-y-4">
                    <div className="bg-gray-50 p-3 rounded-lg">
                        <p className="text-sm text-gray-600">Tiền Mở Ca: <span className="font-semibold">{shift.openingCash?.toLocaleString('vi-VN')} đ</span></p>
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Tiền Đóng Ca (đ)</label>
                        <input 
                            type="number"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.closingCash}
                            onChange={(e) => setFormData({...formData, closingCash: Number(e.target.value)})}
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
                    <div className="flex gap-2 justify-end">
                        <Button variant="ghost" onClick={onClose}>Hủy</Button>
                        <Button variant="danger" onClick={() => onSubmit(formData)}>Đóng Ca</Button>
                    </div>
                </div>
            </Card>
        </div>
    );
};

import { useState } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { Plus, CheckCircle, XCircle, Calendar } from 'lucide-react';
import { useGetApiLeaveRequest, usePostApiLeaveRequest, usePatchApiLeaveRequestIdStatus } from '../../api/generated-orval/leave-request/leave-request';

export const LeaveRequestsPage = () => {
    const [showModal, setShowModal] = useState(false);
    const [statusFilter, setStatusFilter] = useState('');

    const { data: requestsData, refetch } = useGetApiLeaveRequest({ Status: statusFilter as any });
    const createMutation = usePostApiLeaveRequest();
    const updateStatusMutation = usePatchApiLeaveRequestIdStatus();

    const requests = requestsData?.data?.data?.items || [];

    const handleCreate = async (data: any) => {
        try {
            await createMutation.mutateAsync({ data });
            setShowModal(false);
            refetch();
        } catch (error) {
            console.error('Error:', error);
        }
    };

    const handleUpdateStatus = async (id: string, status: string, comments?: string) => {
        try {
            await updateStatusMutation.mutateAsync({ 
                id, 
                data: { status: status as any, comments } 
            });
            refetch();
        } catch (error) {
            console.error('Error:', error);
        }
    };

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'pending':
                return <Badge variant="warning">Chờ Duyệt</Badge>;
            case 'approved':
                return <Badge variant="success">Đã Duyệt</Badge>;
            case 'rejected':
                return <Badge variant="danger">Từ Chối</Badge>;
            default:
                return <Badge variant="default">{status}</Badge>;
        }
    };

    const getLeaveTypeBadge = (type: string) => {
        const types: any = {
            annual: { label: 'Nghỉ Phép', variant: 'info' },
            sick: { label: 'Nghỉ Ốm', variant: 'warning' },
            unpaid: { label: 'Không Lương', variant: 'default' },
            maternity: { label: 'Thai Sản', variant: 'success' }
        };
        const config = types[type] || { label: type, variant: 'default' };
        return <Badge variant={config.variant as any}>{config.label}</Badge>;
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">Quản Lý Đơn Xin Nghỉ</h1>
                <Button 
                    variant="primary" 
                    icon={<Plus className="w-5 h-5" />}
                    onClick={() => setShowModal(true)}
                >
                    Tạo Đơn Mới
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
                        <option value="pending">Chờ Duyệt</option>
                        <option value="approved">Đã Duyệt</option>
                        <option value="rejected">Từ Chối</option>
                    </select>
                </div>
            </Card>

            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Nhân Viên</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Loại Nghỉ</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Thời Gian</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Số Ngày</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Lý Do</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Trạng Thái</th>
                                <th className="px-4 py-3 text-right text-sm font-semibold">Thao Tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {requests.map((request: any) => (
                                <tr key={request.id} className="hover:bg-gray-50">
                                    <td className="px-4 py-3">
                                        <div>
                                            <p className="font-medium">{request.employeeName}</p>
                                            <p className="text-xs text-gray-500">
                                                Gửi: {new Date(request.submittedAt).toLocaleDateString('vi-VN')}
                                            </p>
                                        </div>
                                    </td>
                                    <td className="px-4 py-3">{getLeaveTypeBadge(request.leaveType)}</td>
                                    <td className="px-4 py-3">
                                        <div className="flex items-center gap-1 text-sm">
                                            <Calendar className="w-4 h-4 text-gray-400" />
                                            <div>
                                                <p>{new Date(request.startDate).toLocaleDateString('vi-VN')}</p>
                                                <p className="text-gray-500">đến</p>
                                                <p>{new Date(request.endDate).toLocaleDateString('vi-VN')}</p>
                                            </div>
                                        </div>
                                    </td>
                                    <td className="px-4 py-3 text-center font-medium">{request.days} ngày</td>
                                    <td className="px-4 py-3 max-w-xs truncate text-sm">{request.reason}</td>
                                    <td className="px-4 py-3">{getStatusBadge(request.status)}</td>
                                    <td className="px-4 py-3 text-right">
                                        {request.status === 'pending' && (
                                            <div className="flex items-center justify-end gap-2">
                                                <Button 
                                                    variant="success" 
                                                    size="sm"
                                                    icon={<CheckCircle className="w-4 h-4" />}
                                                    onClick={() => handleUpdateStatus(request.id, 'approved', 'Đã duyệt')}
                                                >
                                                    Duyệt
                                                </Button>
                                                <Button 
                                                    variant="danger" 
                                                    size="sm"
                                                    icon={<XCircle className="w-4 h-4" />}
                                                    onClick={() => {
                                                        const reason = prompt('Lý do từ chối:');
                                                        if (reason) handleUpdateStatus(request.id, 'rejected', reason);
                                                    }}
                                                >
                                                    Từ Chối
                                                </Button>
                                            </div>
                                        )}
                                        {request.status !== 'pending' && request.approvedByName && (
                                            <p className="text-xs text-gray-500">
                                                Bởi: {request.approvedByName}
                                            </p>
                                        )}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </Card>

            {showModal && (
                <LeaveRequestFormModal 
                    onClose={() => setShowModal(false)}
                    onSubmit={handleCreate}
                />
            )}
        </div>
    );
};

const LeaveRequestFormModal = ({ onClose, onSubmit }: any) => {
    const [formData, setFormData] = useState({
        leaveType: 'annual',
        startDate: '',
        endDate: '',
        reason: ''
    });

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <Card className="w-full max-w-md">
                <h2 className="text-xl font-bold mb-4">Tạo Đơn Xin Nghỉ</h2>
                <div className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium mb-1">Loại Nghỉ *</label>
                        <select 
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.leaveType}
                            onChange={(e) => setFormData({...formData, leaveType: e.target.value})}
                        >
                            <option value="annual">Nghỉ Phép</option>
                            <option value="sick">Nghỉ Ốm</option>
                            <option value="unpaid">Không Lương</option>
                            <option value="maternity">Thai Sản</option>
                        </select>
                    </div>
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium mb-1">Từ Ngày *</label>
                            <input 
                                type="date"
                                className="w-full px-3 py-2 border rounded-lg"
                                value={formData.startDate}
                                onChange={(e) => setFormData({...formData, startDate: e.target.value})}
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium mb-1">Đến Ngày *</label>
                            <input 
                                type="date"
                                className="w-full px-3 py-2 border rounded-lg"
                                value={formData.endDate}
                                onChange={(e) => setFormData({...formData, endDate: e.target.value})}
                                required
                            />
                        </div>
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Lý Do *</label>
                        <textarea 
                            className="w-full px-3 py-2 border rounded-lg"
                            rows={4}
                            value={formData.reason}
                            onChange={(e) => setFormData({...formData, reason: e.target.value})}
                            required
                        />
                    </div>
                    <div className="flex gap-2 justify-end pt-4">
                        <Button variant="ghost" onClick={onClose}>Hủy</Button>
                        <Button variant="primary" onClick={() => onSubmit({
                            ...formData,
                            startDate: new Date(formData.startDate).toISOString(),
                            endDate: new Date(formData.endDate).toISOString()
                        })}>
                            Gửi Đơn
                        </Button>
                    </div>
                </div>
            </Card>
        </div>
    );
};

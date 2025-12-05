import { useState } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { Plus, Calendar, Clock, Edit, Trash2 } from 'lucide-react';
import { useGetApiWorkSchedules, usePostApiWorkSchedules, usePutApiWorkSchedulesId, useDeleteApiWorkSchedulesId } from '../../api/generated-orval/work-schedules/work-schedules';

export const WorkSchedulesPage = () => {
    const [showModal, setShowModal] = useState(false);
    const [selectedSchedule, setSelectedSchedule] = useState<any>(null);

    const { data: schedulesData, refetch } = useGetApiWorkSchedules({});
    const createMutation = usePostApiWorkSchedules();
    const updateMutation = usePutApiWorkSchedulesId();
    const deleteMutation = useDeleteApiWorkSchedulesId();

    const schedules = schedulesData?.data?.data?.items || [];

    const handleSubmit = async (data: any) => {
        try {
            if (selectedSchedule) {
                await updateMutation.mutateAsync({ id: selectedSchedule.id, data });
            } else {
                await createMutation.mutateAsync({ data });
            }
            setShowModal(false);
            setSelectedSchedule(null);
            refetch();
        } catch (error) {
            console.error('Error:', error);
        }
    };

    const handleDelete = async (id: string) => {
        if (confirm('Bạn có chắc muốn xóa lịch làm việc này?')) {
            try {
                await deleteMutation.mutateAsync({ id });
                refetch();
            } catch (error) {
                console.error('Error:', error);
            }
        }
    };

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'scheduled':
                return <Badge variant="warning">Đã Lên Lịch</Badge>;
            case 'confirmed':
                return <Badge variant="info">Đã Xác Nhận</Badge>;
            case 'completed':
                return <Badge variant="success">Hoàn Thành</Badge>;
            case 'missed':
                return <Badge variant="danger">Vắng Mặt</Badge>;
            default:
                return <Badge variant="default">{status}</Badge>;
        }
    };

    const getTypeBadge = (type: string) => {
        switch (type) {
            case 'regular':
                return <Badge variant="default">Thường</Badge>;
            case 'overtime':
                return <Badge variant="warning">Tăng Ca</Badge>;
            case 'holiday':
                return <Badge variant="info">Lễ</Badge>;
            default:
                return <Badge variant="default">{type}</Badge>;
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">Quản Lý Lịch Làm Việc</h1>
                <Button 
                    variant="primary" 
                    icon={<Plus className="w-5 h-5" />}
                    onClick={() => setShowModal(true)}
                >
                    Thêm Lịch Mới
                </Button>
            </div>

            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Nhân Viên</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Ngày</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Giờ Làm</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Loại Ca</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Trạng Thái</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Ghi Chú</th>
                                <th className="px-4 py-3 text-right text-sm font-semibold">Thao Tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {schedules.map((schedule: any) => (
                                <tr key={schedule.id} className="hover:bg-gray-50">
                                    <td className="px-4 py-3">
                                        <div>
                                            <p className="font-medium">{schedule.employeeName}</p>
                                            <p className="text-xs text-gray-500">{schedule.employeeCode}</p>
                                        </div>
                                    </td>
                                    <td className="px-4 py-3">
                                        <div className="flex items-center gap-2">
                                            <Calendar className="w-4 h-4 text-gray-400" />
                                            <span className="text-sm">
                                                {new Date(schedule.date).toLocaleDateString('vi-VN')}
                                            </span>
                                        </div>
                                    </td>
                                    <td className="px-4 py-3">
                                        <div className="flex items-center gap-2">
                                            <Clock className="w-4 h-4 text-gray-400" />
                                            <div className="text-sm">
                                                <p>{schedule.startTime} - {schedule.endTime}</p>
                                            </div>
                                        </div>
                                    </td>
                                    <td className="px-4 py-3">{getTypeBadge(schedule.type)}</td>
                                    <td className="px-4 py-3">{getStatusBadge(schedule.status)}</td>
                                    <td className="px-4 py-3 max-w-xs truncate text-sm text-gray-600">
                                        {schedule.notes || '-'}
                                    </td>
                                    <td className="px-4 py-3 text-right">
                                        <div className="flex items-center justify-end gap-2">
                                            <Button 
                                                variant="ghost" 
                                                size="sm" 
                                                icon={<Edit className="w-4 h-4" />}
                                                onClick={() => {
                                                    setSelectedSchedule(schedule);
                                                    setShowModal(true);
                                                }}
                                            >
                                                Sửa
                                            </Button>
                                            <Button 
                                                variant="danger" 
                                                size="sm" 
                                                icon={<Trash2 className="w-4 h-4" />}
                                                onClick={() => handleDelete(schedule.id)}
                                            >
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

            {showModal && (
                <WorkScheduleFormModal 
                    schedule={selectedSchedule}
                    onClose={() => {
                        setShowModal(false);
                        setSelectedSchedule(null);
                    }}
                    onSubmit={handleSubmit}
                />
            )}
        </div>
    );
};

const WorkScheduleFormModal = ({ schedule, onClose, onSubmit }: any) => {
    const [formData, setFormData] = useState({
        employeeId: schedule?.employeeId || '',
        date: schedule?.date ? new Date(schedule.date).toISOString().split('T')[0] : '',
        startTime: schedule?.startTime || '08:00:00',
        endTime: schedule?.endTime || '17:00:00',
        type: schedule?.type || 'regular',
        status: schedule?.status || 'scheduled',
        notes: schedule?.notes || ''
    });

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <Card className="w-full max-w-2xl max-h-[90vh] overflow-y-auto">
                <h2 className="text-xl font-bold mb-4">
                    {schedule ? 'Chỉnh Sửa Lịch Làm Việc' : 'Thêm Lịch Làm Việc Mới'}
                </h2>
                <div className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium mb-1">Nhân Viên (ID) *</label>
                        <input 
                            type="text"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.employeeId}
                            onChange={(e) => setFormData({...formData, employeeId: e.target.value})}
                            required
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Ngày *</label>
                        <input 
                            type="date"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.date}
                            onChange={(e) => setFormData({...formData, date: e.target.value})}
                            required
                        />
                    </div>
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium mb-1">Giờ Bắt Đầu *</label>
                            <input 
                                type="time"
                                className="w-full px-3 py-2 border rounded-lg"
                                value={formData.startTime}
                                onChange={(e) => setFormData({...formData, startTime: e.target.value})}
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium mb-1">Giờ Kết Thúc *</label>
                            <input 
                                type="time"
                                className="w-full px-3 py-2 border rounded-lg"
                                value={formData.endTime}
                                onChange={(e) => setFormData({...formData, endTime: e.target.value})}
                                required
                            />
                        </div>
                    </div>
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium mb-1">Loại Ca *</label>
                            <select 
                                className="w-full px-3 py-2 border rounded-lg"
                                value={formData.type}
                                onChange={(e) => setFormData({...formData, type: e.target.value})}
                            >
                                <option value="regular">Thường</option>
                                <option value="overtime">Tăng Ca</option>
                                <option value="holiday">Lễ</option>
                            </select>
                        </div>
                        {schedule && (
                            <div>
                                <label className="block text-sm font-medium mb-1">Trạng Thái</label>
                                <select 
                                    className="w-full px-3 py-2 border rounded-lg"
                                    value={formData.status}
                                    onChange={(e) => setFormData({...formData, status: e.target.value})}
                                >
                                    <option value="scheduled">Đã Lên Lịch</option>
                                    <option value="confirmed">Đã Xác Nhận</option>
                                    <option value="completed">Hoàn Thành</option>
                                    <option value="missed">Vắng Mặt</option>
                                </select>
                            </div>
                        )}
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
                        <Button variant="primary" onClick={() => onSubmit({
                            ...formData,
                            date: new Date(formData.date).toISOString()
                        })}>
                            {schedule ? 'Cập Nhật' : 'Thêm Mới'}
                        </Button>
                    </div>
                </div>
            </Card>
        </div>
    );
};

import { useState } from 'react';
import { useGetApiTask, usePostApiTask, usePutApiTaskId, useDeleteApiTaskId, usePatchApiTaskIdStatus } from '../../api/generated-orval/task/task';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { Plus, Edit, Trash2, CheckCircle, Clock, AlertCircle } from 'lucide-react';
import { useAuth } from '../lib/contexts/AuthContext';
import type { TasksCreateTaskRequest, TasksUpdateTaskRequest, EntitiesTasksTaskStatus, EntitiesTasksTaskPriority } from '../../api/generated-orval/schemas';

export const TasksPage = () => {
    const { user } = useAuth();
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [selectedTask, setSelectedTask] = useState<any>(null);
    const [statusFilter, setStatusFilter] = useState<string>('');
    const [priorityFilter, setPriorityFilter] = useState<string>('');

    const { data: tasksData, refetch } = useGetApiTask({
        Status: statusFilter as any,
        Priority: priorityFilter as any
    });
    
    const createTaskMutation = usePostApiTask();
    const updateTaskMutation = usePutApiTaskId();
    const deleteTaskMutation = useDeleteApiTaskId();
    const updateStatusMutation = usePatchApiTaskIdStatus();

    const tasks = tasksData?.data?.data?.items || [];

    const handleCreateTask = async (data: TasksCreateTaskRequest) => {
        try {
            await createTaskMutation.mutateAsync({ data });
            setShowCreateModal(false);
            refetch();
        } catch (error) {
            console.error('Error creating task:', error);
        }
    };

    const handleUpdateTask = async (taskId: string, data: TasksUpdateTaskRequest) => {
        try {
            await updateTaskMutation.mutateAsync({ id: taskId, data });
            setShowEditModal(false);
            setSelectedTask(null);
            refetch();
        } catch (error) {
            console.error('Error updating task:', error);
        }
    };

    const handleDeleteTask = async (taskId: string) => {
        if (confirm('Bạn có chắc muốn xóa task này?')) {
            try {
                await deleteTaskMutation.mutateAsync({ id: taskId });
                refetch();
            } catch (error) {
                console.error('Error deleting task:', error);
            }
        }
    };

    const handleUpdateStatus = async (taskId: string, status: EntitiesTasksTaskStatus) => {
        try {
            await updateStatusMutation.mutateAsync({ id: taskId, data: status });
            refetch();
        } catch (error) {
            console.error('Error updating status:', error);
        }
    };

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'pending':
                return <Badge variant="warning">Chờ Xử Lý</Badge>;
            case 'inProgress':
                return <Badge variant="info">Đang Làm</Badge>;
            case 'completed':
                return <Badge variant="success">Hoàn Thành</Badge>;
            default:
                return <Badge variant="default">{status}</Badge>;
        }
    };

    const getPriorityBadge = (priority: string) => {
        switch (priority) {
            case 'high':
                return <Badge variant="danger">Cao</Badge>;
            case 'medium':
                return <Badge variant="warning">Trung Bình</Badge>;
            case 'low':
                return <Badge variant="default">Thấp</Badge>;
            default:
                return <Badge variant="default">{priority}</Badge>;
        }
    };

    const getPriorityIcon = (priority: string) => {
        switch (priority) {
            case 'high':
                return <AlertCircle className="w-4 h-4 text-red-600" />;
            case 'medium':
                return <Clock className="w-4 h-4 text-yellow-600" />;
            case 'low':
                return <CheckCircle className="w-4 h-4 text-gray-600" />;
            default:
                return null;
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">Quản Lý Công Việc</h1>
                <Button 
                    variant="primary" 
                    icon={<Plus className="w-5 h-5" />}
                    onClick={() => setShowCreateModal(true)}
                >
                    Tạo Task Mới
                </Button>
            </div>

            {/* Filters */}
            <Card>
                <div className="flex gap-4">
                    <div>
                        <label className="block text-sm font-medium mb-1">Trạng Thái</label>
                        <select 
                            className="px-3 py-2 border rounded-lg"
                            value={statusFilter}
                            onChange={(e) => setStatusFilter(e.target.value)}
                        >
                            <option value="">Tất Cả</option>
                            <option value="pending">Chờ Xử Lý</option>
                            <option value="inProgress">Đang Làm</option>
                            <option value="completed">Hoàn Thành</option>
                        </select>
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Độ Ưu Tiên</label>
                        <select 
                            className="px-3 py-2 border rounded-lg"
                            value={priorityFilter}
                            onChange={(e) => setPriorityFilter(e.target.value)}
                        >
                            <option value="">Tất Cả</option>
                            <option value="low">Thấp</option>
                            <option value="medium">Trung Bình</option>
                            <option value="high">Cao</option>
                        </select>
                    </div>
                </div>
            </Card>

            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Tiêu Đề</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Mô Tả</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Người Được Giao</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Độ Ưu Tiên</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Trạng Thái</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Hạn Chót</th>
                                <th className="px-4 py-3 text-right text-sm font-semibold">Thao Tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {tasks.map((task: any) => (
                                <tr key={task.id} className="hover:bg-gray-50">
                                    <td className="px-4 py-3 font-medium">{task.title}</td>
                                    <td className="px-4 py-3 text-sm text-gray-600 max-w-xs truncate">
                                        {task.description}
                                    </td>
                                    <td className="px-4 py-3">
                                        <div>
                                            <p className="font-medium">{task.assignedToName}</p>
                                            <p className="text-xs text-gray-500">{task.assignedToEmail}</p>
                                        </div>
                                    </td>
                                    <td className="px-4 py-3">
                                        <div className="flex items-center gap-2">
                                            {getPriorityIcon(task.priority)}
                                            {getPriorityBadge(task.priority)}
                                        </div>
                                    </td>
                                    <td className="px-4 py-3">{getStatusBadge(task.status)}</td>
                                    <td className="px-4 py-3">
                                        {task.dueDate ? (
                                            <div className={task.isOverdue ? 'text-red-600 font-medium' : ''}>
                                                {new Date(task.dueDate).toLocaleDateString('vi-VN')}
                                                {task.isOverdue && <span className="ml-1">(Quá hạn)</span>}
                                            </div>
                                        ) : '-'}
                                    </td>
                                    <td className="px-4 py-3 text-right">
                                        <div className="flex items-center justify-end gap-2">
                                            {task.status !== 'completed' && (
                                                <Button 
                                                    variant="success" 
                                                    size="sm"
                                                    icon={<CheckCircle className="w-4 h-4" />}
                                                    onClick={() => handleUpdateStatus(task.id, 'completed' as EntitiesTasksTaskStatus)}
                                                >
                                                    Hoàn Thành
                                                </Button>
                                            )}
                                            <Button 
                                                variant="ghost" 
                                                size="sm" 
                                                icon={<Edit className="w-4 h-4" />}
                                                onClick={() => {
                                                    setSelectedTask(task);
                                                    setShowEditModal(true);
                                                }}
                                            >
                                                Sửa
                                            </Button>
                                            <Button 
                                                variant="danger" 
                                                size="sm" 
                                                icon={<Trash2 className="w-4 h-4" />}
                                                onClick={() => handleDeleteTask(task.id)}
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

            {/* Create Task Modal */}
            {showCreateModal && (
                <TaskFormModal 
                    onClose={() => setShowCreateModal(false)}
                    onSubmit={handleCreateTask}
                    title="Tạo Task Mới"
                />
            )}

            {/* Edit Task Modal */}
            {showEditModal && selectedTask && (
                <TaskFormModal 
                    task={selectedTask}
                    onClose={() => {
                        setShowEditModal(false);
                        setSelectedTask(null);
                    }}
                    onSubmit={(data) => handleUpdateTask(selectedTask.id, data)}
                    title="Chỉnh Sửa Task"
                />
            )}
        </div>
    );
};

// Task Form Modal Component
const TaskFormModal = ({ task, onClose, onSubmit, title }: any) => {
    const [formData, setFormData] = useState({
        title: task?.title || '',
        description: task?.description || '',
        assignedTo: task?.assignedTo || '',
        priority: task?.priority || 'medium',
        status: task?.status || 'pending',
        dueDate: task?.dueDate ? new Date(task.dueDate).toISOString().split('T')[0] : ''
    });

    const handleSubmit = () => {
        const submitData: any = {
            title: formData.title,
            description: formData.description,
            assignedTo: formData.assignedTo,
            priority: formData.priority as EntitiesTasksTaskPriority,
        };

        if (task) {
            submitData.status = formData.status as EntitiesTasksTaskStatus;
        }

        if (formData.dueDate) {
            submitData.dueDate = new Date(formData.dueDate).toISOString();
        }

        onSubmit(submitData);
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <Card className="w-full max-w-2xl max-h-[90vh] overflow-y-auto">
                <h2 className="text-xl font-bold mb-4">{title}</h2>
                <div className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium mb-1">Tiêu Đề *</label>
                        <input 
                            type="text"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.title}
                            onChange={(e) => setFormData({...formData, title: e.target.value})}
                            required
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Mô Tả *</label>
                        <textarea 
                            className="w-full px-3 py-2 border rounded-lg"
                            rows={4}
                            value={formData.description}
                            onChange={(e) => setFormData({...formData, description: e.target.value})}
                            required
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Người Được Giao (User ID) *</label>
                        <input 
                            type="text"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.assignedTo}
                            onChange={(e) => setFormData({...formData, assignedTo: e.target.value})}
                            required
                        />
                    </div>
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium mb-1">Độ Ưu Tiên *</label>
                            <select 
                                className="w-full px-3 py-2 border rounded-lg"
                                value={formData.priority}
                                onChange={(e) => setFormData({...formData, priority: e.target.value})}
                            >
                                <option value="low">Thấp</option>
                                <option value="medium">Trung Bình</option>
                                <option value="high">Cao</option>
                            </select>
                        </div>
                        {task && (
                            <div>
                                <label className="block text-sm font-medium mb-1">Trạng Thái</label>
                                <select 
                                    className="w-full px-3 py-2 border rounded-lg"
                                    value={formData.status}
                                    onChange={(e) => setFormData({...formData, status: e.target.value})}
                                >
                                    <option value="pending">Chờ Xử Lý</option>
                                    <option value="inProgress">Đang Làm</option>
                                    <option value="completed">Hoàn Thành</option>
                                </select>
                            </div>
                        )}
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Hạn Chót</label>
                        <input 
                            type="date"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.dueDate}
                            onChange={(e) => setFormData({...formData, dueDate: e.target.value})}
                        />
                    </div>
                    <div className="flex gap-2 justify-end pt-4">
                        <Button variant="ghost" onClick={onClose}>Hủy</Button>
                        <Button variant="primary" onClick={handleSubmit}>
                            {task ? 'Cập Nhật' : 'Tạo Task'}
                        </Button>
                    </div>
                </div>
            </Card>
        </div>
    );
};

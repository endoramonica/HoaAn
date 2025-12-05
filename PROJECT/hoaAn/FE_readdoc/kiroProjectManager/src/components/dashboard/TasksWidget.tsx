import { useNavigate } from 'react-router-dom';
import { Card } from '../ui/Card';
import { Badge } from '../ui/Badge';
import { CheckSquare, AlertCircle, Clock } from 'lucide-react';
import { useTasks } from '../../lib/hooks/useTasks';

export const TasksWidget = () => {
    const navigate = useNavigate();
    const { data: tasksData } = useTasks({ PageSize: 5 });
    
    const tasks = tasksData?.data?.data?.items || [];
    const totalTasks = tasksData?.data?.data?.totalCount || 0;

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'pending':
                return <Badge variant="warning">Chờ</Badge>;
            case 'inProgress':
                return <Badge variant="info">Đang Làm</Badge>;
            case 'completed':
                return <Badge variant="success">Xong</Badge>;
            default:
                return <Badge variant="default">{status}</Badge>;
        }
    };

    const getPriorityIcon = (priority: string) => {
        switch (priority) {
            case 'high':
                return <AlertCircle className="w-4 h-4 text-red-600" />;
            case 'medium':
                return <Clock className="w-4 h-4 text-yellow-600" />;
            default:
                return <CheckSquare className="w-4 h-4 text-gray-600" />;
        }
    };

    return (
        <Card>
            <div className="flex items-center justify-between mb-4">
                <div className="flex items-center gap-2">
                    <CheckSquare className="w-5 h-5 text-blue-600" />
                    <h3 className="text-lg font-semibold">Công Việc Gần Đây</h3>
                </div>
                <button 
                    onClick={() => navigate('/tasks')}
                    className="text-sm text-blue-600 hover:text-blue-700"
                >
                    Xem tất cả ({totalTasks})
                </button>
            </div>

            <div className="space-y-3">
                {tasks.length === 0 ? (
                    <p className="text-sm text-gray-500 text-center py-4">
                        Chưa có công việc nào
                    </p>
                ) : (
                    tasks.map((task: any) => (
                        <div 
                            key={task.id}
                            className="flex items-start gap-3 p-3 bg-gray-50 rounded-lg hover:bg-gray-100 cursor-pointer transition-colors"
                            onClick={() => navigate('/tasks')}
                        >
                            {getPriorityIcon(task.priority)}
                            <div className="flex-1 min-w-0">
                                <p className="font-medium text-sm truncate">{task.title}</p>
                                <p className="text-xs text-gray-600 truncate">{task.description}</p>
                                <div className="flex items-center gap-2 mt-1">
                                    {getStatusBadge(task.status)}
                                    {task.isOverdue && (
                                        <span className="text-xs text-red-600 font-medium">Quá hạn</span>
                                    )}
                                </div>
                            </div>
                        </div>
                    ))
                )}
            </div>
        </Card>
    );
};

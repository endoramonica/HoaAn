import { useNavigate } from 'react-router-dom';
import { Card } from '../ui/Card';
import { Badge } from '../ui/Badge';
import { Clock, DollarSign } from 'lucide-react';
import { useShifts } from '../../lib/hooks/useShifts';

export const ShiftsWidget = () => {
    const navigate = useNavigate();
    const { data: shiftsData } = useShifts({ PageSize: 5 });
    
    const shifts = shiftsData?.data?.data?.items || [];
    const totalShifts = shiftsData?.data?.data?.totalCount || 0;

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
        <Card>
            <div className="flex items-center justify-between mb-4">
                <div className="flex items-center gap-2">
                    <Clock className="w-5 h-5 text-green-600" />
                    <h3 className="text-lg font-semibold">Ca Làm Việc</h3>
                </div>
                <button 
                    onClick={() => navigate('/shifts')}
                    className="text-sm text-blue-600 hover:text-blue-700"
                >
                    Xem tất cả ({totalShifts})
                </button>
            </div>

            <div className="space-y-3">
                {shifts.length === 0 ? (
                    <p className="text-sm text-gray-500 text-center py-4">
                        Chưa có ca làm việc nào
                    </p>
                ) : (
                    shifts.map((shift: any) => (
                        <div 
                            key={shift.id}
                            className="flex items-start gap-3 p-3 bg-gray-50 rounded-lg hover:bg-gray-100 cursor-pointer transition-colors"
                            onClick={() => navigate('/shifts')}
                        >
                            <Clock className="w-5 h-5 text-gray-400 mt-0.5" />
                            <div className="flex-1 min-w-0">
                                <div className="flex items-center justify-between">
                                    <p className="font-medium text-sm">{shift.userName || 'N/A'}</p>
                                    {getStatusBadge(shift.status)}
                                </div>
                                <p className="text-xs text-gray-600">{shift.storeName || 'N/A'}</p>
                                <div className="flex items-center gap-3 mt-1">
                                    <div className="flex items-center gap-1 text-xs text-gray-600">
                                        <DollarSign className="w-3 h-3" />
                                        {shift.openingCash?.toLocaleString('vi-VN')} đ
                                    </div>
                                    {shift.startTime && (
                                        <span className="text-xs text-gray-500">
                                            {new Date(shift.startTime).toLocaleString('vi-VN')}
                                        </span>
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

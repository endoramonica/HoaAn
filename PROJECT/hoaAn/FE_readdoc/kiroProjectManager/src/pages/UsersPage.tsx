import { useNavigate } from 'react-router-dom';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { RoleGuard } from '../components/auth/RoleGuard';
import { useUsers } from '../lib/hooks/useUsers';
import { Plus, Edit, UserCheck, UserX, User, Eye } from 'lucide-react';

export const UsersPage = () => {
    const navigate = useNavigate();
    const { data: usersData } = useUsers();
    
    // Extract users from response
    const users = (usersData as any)?.data?.data?.items || [];

    const getRoleBadge = (role: string) => {
        switch (role) {
            case 'ADMIN': return <Badge variant="danger">Quản Trị</Badge>;
            case 'MANAGER': return <Badge variant="info">Quản Lý</Badge>;
            case 'CASHIER': return <Badge variant="success">Thu Ngân</Badge>;
            case 'USER': return <Badge variant="default">Người Dùng</Badge>;
            default: return <Badge variant="default">{role}</Badge>;
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">
                    Quản Lý Người Dùng
                </h1>
                
                <RoleGuard requiredRoles={['admin', 'administrator']}>
                    <Button variant="primary" icon={<Plus className="w-5 h-5" />}>
                        Thêm Người Dùng
                    </Button>
                </RoleGuard>
            </div>

            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Ảnh</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Tên</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Email</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Số Điện Thoại</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Vai Trò</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Trạng Thái</th>
                                <th className="px-4 py-3 text-right text-sm font-semibold">Thao Tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {users.map((user: any) => (
                                <tr 
                                    key={user.id} 
                                    className="hover:bg-gray-50 cursor-pointer transition-colors"
                                    onClick={() => navigate(`/users/${user.id}`)}
                                >
                                    <td className="px-4 py-3">
                                        <div className="flex items-center justify-center">
                                            {user.profilePictureUrl ? (
                                                <img 
                                                    src={user.profilePictureUrl} 
                                                    alt={user.fullName || 'User'} 
                                                    className="w-10 h-10 rounded-full object-cover border-2 border-gray-200"
                                                />
                                            ) : (
                                                <div className="w-10 h-10 rounded-full bg-gray-200 flex items-center justify-center">
                                                    <User className="w-5 h-5 text-gray-500" />
                                                </div>
                                            )}
                                        </div>
                                    </td>
                                    <td className="px-4 py-3">{user.fullName || '-'}</td>
                                    <td className="px-4 py-3">{user.email}</td>
                                    <td className="px-4 py-3">{user.phoneNumber || '-'}</td>
                                    <td className="px-4 py-3">{getRoleBadge(user.role)}</td>
                                    <td className="px-4 py-3">
                                        <Badge variant={user.isActive ? 'success' : 'danger'}>
                                            {user.isActive ? 'Hoạt động' : 'Ngừng'}
                                        </Badge>
                                    </td>
                                    <td className="px-4 py-3 text-right">
                                        <div className="flex items-center justify-end gap-2" onClick={(e) => e.stopPropagation()}>
                                            <Button 
                                                variant="ghost" 
                                                size="sm" 
                                                icon={<Eye className="w-4 h-4" />}
                                                onClick={() => navigate(`/users/${user.id}`)}
                                            >
                                                Xem
                                            </Button>
                                            <Button variant="ghost" size="sm" icon={<Edit className="w-4 h-4" />}>
                                                Sửa
                                            </Button>
                                            
                                            <RoleGuard requiredRoles={['admin', 'administrator']}>
                                                {user.isActive ? (
                                                    <Button variant="danger" size="sm" icon={<UserX className="w-4 h-4" />}>
                                                        Vô hiệu hóa
                                                    </Button>
                                                ) : (
                                                    <Button variant="primary" size="sm" icon={<UserCheck className="w-4 h-4" />}>
                                                        Kích hoạt
                                                    </Button>
                                                )}
                                            </RoleGuard>
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

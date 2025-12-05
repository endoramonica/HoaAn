import { useNavigate } from 'react-router-dom';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { RoleGuard } from '../components/auth/RoleGuard';
import { useAuth } from '../lib/contexts/AuthContext';
import { useEmployees } from '../lib/hooks/useEmployees';
import { Plus, Edit, Trash2, Shield, Eye } from 'lucide-react';

export const EmployeesPage = () => {
    const navigate = useNavigate();
    const { user, roles, permissions } = useAuth();
    const { data: employeesData } = useEmployees();
    
    // Debug logging
    console.log('🔍 EmployeesPage - Full response:', employeesData);
    console.log('🔍 EmployeesPage - employeesData?.data:', employeesData?.data);
    console.log('🔍 EmployeesPage - employeesData?.data?.data:', employeesData?.data?.data);
    console.log('🔍 EmployeesPage - employeesData?.data?.data?.items:', employeesData?.data?.data?.items);
    
    // Extract employees from response
    // Response structure: { data: { data: { items: [...], pageNumber, ... } } }
    const employees = employeesData?.data?.data?.items || [];
    
    console.log('✅ EmployeesPage - Final employees:', employees);
    console.log('✅ EmployeesPage - Employees count:', employees.length);

    const getRoleBadge = (role: string) => {
        switch (role) {
            case 'ADMIN': return <Badge variant="danger">Quản Trị</Badge>;
            case 'MANAGER': return <Badge variant="info">Quản Lý</Badge>;
            case 'CASHIER': return <Badge variant="success">Thu Ngân</Badge>;
            default: return <Badge variant="default">Nhân Viên</Badge>;
        }
    };

    return (
        <div className="space-y-6">
            {/* User info banner */}
            <div className="bg-green-50 border border-green-200 rounded-lg p-4">
                <div className="flex items-start gap-3">
                    <Shield className="w-5 h-5 text-green-600 mt-0.5" />
                    <div className="flex-1">
                        <p className="text-sm font-medium text-green-900 mb-1">
                            {user?.fullName || user?.email}
                        </p>
                        <div className="flex flex-wrap gap-1 mb-2">
                            {roles.map((role, index) => (
                                <span 
                                    key={index}
                                    className="text-xs px-2 py-0.5 bg-green-200 text-green-900 rounded font-medium"
                                >
                                    {role}
                                </span>
                            ))}
                        </div>
                        <p className="text-xs text-green-700">
                            {permissions.length} quyền hạn được cấp
                        </p>
                    </div>
                </div>
            </div>

            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">
                    Quản Lý Nhân Viên
                </h1>
                
                {/* Chỉ admin mới thấy nút thêm nhân viên */}
                <RoleGuard requiredRoles={['admin', 'administrator']}>
                    <Button variant="primary" icon={<Plus className="w-5 h-5" />}>
                        Thêm Nhân Viên
                    </Button>
                </RoleGuard>
            </div>

            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Tên</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Email</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Vai Trò</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Trạng Thái</th>
                                <th className="px-4 py-3 text-right text-sm font-semibold">Thao Tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {employees.map((employee: any) => (
                                <tr 
                                    key={employee.userId || employee.id} 
                                    className="hover:bg-gray-50 cursor-pointer transition-colors"
                                    onClick={() => navigate(`/users/${employee.userId || employee.id}`)}
                                >
                                    <td className="px-4 py-3">{employee.name || employee.fullName}</td>
                                    <td className="px-4 py-3">{employee.email}</td>
                                    <td className="px-4 py-3">{getRoleBadge(employee.role)}</td>
                                    <td className="px-4 py-3">
                                        <Badge variant={employee.isActive ? 'success' : 'danger'}>
                                            {employee.isActive ? 'Hoạt động' : 'Ngừng'}
                                        </Badge>
                                    </td>
                                    <td className="px-4 py-3 text-right">
                                        <div className="flex items-center justify-end gap-2" onClick={(e) => e.stopPropagation()}>
                                            <Button 
                                                variant="ghost" 
                                                size="sm" 
                                                icon={<Eye className="w-4 h-4" />}
                                                onClick={() => navigate(`/users/${employee.userId || employee.id}`)}
                                            >
                                                Xem
                                            </Button>
                                            <Button variant="ghost" size="sm" icon={<Edit className="w-4 h-4" />}>
                                                Sửa
                                            </Button>
                                            
                                            {/* Chỉ admin mới có quyền xóa */}
                                            <RoleGuard requiredRoles={['admin', 'administrator']}>
                                                <Button variant="danger" size="sm" icon={<Trash2 className="w-4 h-4" />}>
                                                    Xóa
                                                </Button>
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

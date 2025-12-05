import { useParams, useNavigate } from 'react-router-dom';
import { useGetApiUsersId } from '../../api/generated-orval/users/users';
import { useGetApiEmployeesUserId } from '../../api/generated-orval/employees/employees';
import { useGetApiAdminCustomerId } from '../../api/generated-orval/admin-customer/admin-customer';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { 
    ArrowLeft, Mail, Phone, Calendar, User, 
    Briefcase, DollarSign, Building, Users, 
    ShoppingBag, Star, MapPin 
} from 'lucide-react';

export const UserDetailPage = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    
    // Fetch User data
    const { data: userData, isLoading: userLoading } = useGetApiUsersId(id!);
    const user = (userData as any)?.data;
    
    // Try to fetch Employee data (might fail if user is not an employee)
    const { data: employeeData } = useGetApiEmployeesUserId(id!, {
        query: { enabled: !!id }
    });
    const employee = employeeData?.data;
    
    // Try to fetch Customer data if customerId exists
    const customerId = user?.customerId;
    const { data: customerData } = useGetApiAdminCustomerId(customerId!, {
        query: { enabled: !!customerId }
    });
    const customer = customerData?.data;

    const formatCurrency = (value?: number) => {
        if (!value) return '0 ₫';
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND',
        }).format(value);
    };

    const formatDate = (dateString?: string) => {
        if (!dateString) return '-';
        return new Date(dateString).toLocaleDateString('vi-VN');
    };

    const getRoleBadge = (role?: string) => {
        switch (role?.toUpperCase()) {
            case 'ADMIN': return <Badge variant="danger">Quản Trị</Badge>;
            case 'MANAGER': return <Badge variant="info">Quản Lý</Badge>;
            case 'CASHIER': return <Badge variant="success">Thu Ngân</Badge>;
            case 'USER': return <Badge variant="default">Người Dùng</Badge>;
            default: return <Badge variant="default">{role || 'N/A'}</Badge>;
        }
    };

    const getEmployeeStatusBadge = (status?: string) => {
        switch (status?.toUpperCase()) {
            case 'ACTIVE': return <Badge variant="success">Đang làm việc</Badge>;
            case 'INACTIVE': return <Badge variant="danger">Nghỉ việc</Badge>;
            case 'ON_LEAVE': return <Badge variant="warning">Nghỉ phép</Badge>;
            default: return <Badge variant="default">{status || 'N/A'}</Badge>;
        }
    };

    if (userLoading) {
        return (
            <div className="flex items-center justify-center h-64">
                <div className="text-gray-500">Đang tải...</div>
            </div>
        );
    }

    if (!user) {
        return (
            <div className="flex items-center justify-center h-64">
                <div className="text-gray-500">Không tìm thấy người dùng</div>
            </div>
        );
    }

    const isEmployee = !!employee;
    const isCustomer = !!customer;

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div className="flex items-center gap-4">
                    <Button 
                        variant="ghost" 
                        icon={<ArrowLeft className="w-5 h-5" />}
                        onClick={() => navigate('/users')}
                    >
                        Quay lại
                    </Button>
                    <h1 className="text-2xl font-bold text-gray-900">
                        Chi Tiết Người Dùng
                    </h1>
                </div>
                <div className="flex items-center gap-2">
                    {isEmployee && <Badge variant="info">Nhân viên</Badge>}
                    {isCustomer && <Badge variant="primary">Khách hàng</Badge>}
                    <Badge variant={user.isActive ? 'success' : 'danger'}>
                        {user.isActive ? 'Hoạt động' : 'Ngừng hoạt động'}
                    </Badge>
                </div>
            </div>

            {/* User Basic Info Card - ALWAYS SHOWN */}
            <Card>
                <div className="p-6">
                    <div className="flex items-start gap-6">
                        <div className="flex-shrink-0">
                            {user.profilePictureUrl || employee?.avatar ? (
                                <img 
                                    src={user.profilePictureUrl || employee?.avatar} 
                                    alt={user.fullName || 'User'} 
                                    className="w-24 h-24 rounded-full object-cover border-2 border-gray-200"
                                />
                            ) : (
                                <div className="w-24 h-24 rounded-full bg-gray-200 flex items-center justify-center">
                                    <User className="w-12 h-12 text-gray-500" />
                                </div>
                            )}
                        </div>
                        <div className="flex-1">
                            <h2 className="text-2xl font-bold text-gray-900 mb-2">
                                {user.fullName || employee?.name || customer?.name || 'Chưa có tên'}
                            </h2>
                            <div className="flex items-center gap-4 mb-4">
                                {getRoleBadge(user.role)}
                            </div>
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                {(user.email || employee?.email || customer?.email) && (
                                    <div className="flex items-center gap-2 text-gray-600">
                                        <Mail className="w-4 h-4" />
                                        <span>{user.email || employee?.email || customer?.email}</span>
                                    </div>
                                )}
                                {(user.phoneNumber || employee?.phone || customer?.phone) && (
                                    <div className="flex items-center gap-2 text-gray-600">
                                        <Phone className="w-4 h-4" />
                                        <span>{user.phoneNumber || employee?.phone || customer?.phone}</span>
                                    </div>
                                )}
                                {(user.createdAt || employee?.createdAt || customer?.createdAt) && (
                                    <div className="flex items-center gap-2 text-gray-600">
                                        <Calendar className="w-4 h-4" />
                                        <span>Tham gia: {formatDate(user.createdAt || employee?.createdAt || customer?.createdAt)}</span>
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
            </Card>

            {/* Employee Profile Section - ONLY IF EMPLOYEE */}
            {isEmployee && (
                <>
                    <Card>
                        <div className="p-6">
                            <h3 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
                                <Briefcase className="w-5 h-5" />
                                Thông Tin Nhân Viên
                            </h3>
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                                <div>
                                    <p className="text-sm text-gray-600">Mã nhân viên</p>
                                    <p className="text-base font-medium text-gray-900">{employee.code || '-'}</p>
                                </div>
                                <div>
                                    <p className="text-sm text-gray-600">Chức vụ</p>
                                    <p className="text-base font-medium text-gray-900">{employee.position || '-'}</p>
                                </div>
                                <div>
                                    <p className="text-sm text-gray-600">Phòng ban</p>
                                    <p className="text-base font-medium text-gray-900">{employee.department || '-'}</p>
                                </div>
                                <div>
                                    <p className="text-sm text-gray-600">Trạng thái</p>
                                    <div className="mt-1">{getEmployeeStatusBadge(employee.status)}</div>
                                </div>
                                <div>
                                    <p className="text-sm text-gray-600">Ngày vào làm</p>
                                    <p className="text-base font-medium text-gray-900">{formatDate(employee.hireDate)}</p>
                                </div>
                                <div>
                                    <p className="text-sm text-gray-600">Lương</p>
                                    <p className="text-base font-medium text-gray-900">{formatCurrency(employee.salary)}</p>
                                </div>
                                {employee.managerName && (
                                    <div>
                                        <p className="text-sm text-gray-600">Quản lý trực tiếp</p>
                                        <p className="text-base font-medium text-gray-900">{employee.managerName}</p>
                                    </div>
                                )}
                                {employee.storeName && (
                                    <div>
                                        <p className="text-sm text-gray-600">Cửa hàng</p>
                                        <p className="text-base font-medium text-gray-900 flex items-center gap-2">
                                            <Building className="w-4 h-4" />
                                            {employee.storeName}
                                        </p>
                                    </div>
                                )}
                                {employee.skills && (
                                    <div className="md:col-span-2">
                                        <p className="text-sm text-gray-600">Kỹ năng</p>
                                        <p className="text-base font-medium text-gray-900">{employee.skills}</p>
                                    </div>
                                )}
                            </div>
                        </div>
                    </Card>
                </>
            )}

            {/* Customer Profile Section - ONLY IF CUSTOMER */}
            {isCustomer && (
                <>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                        <Card>
                            <div className="p-6">
                                <div className="flex items-center gap-4">
                                    <div className="p-3 bg-blue-100 rounded-lg">
                                        <ShoppingBag className="w-6 h-6 text-blue-600" />
                                    </div>
                                    <div>
                                        <p className="text-sm text-gray-600">Tổng Đơn Hàng</p>
                                        <p className="text-2xl font-bold text-gray-900">
                                            {customer.totalOrders || 0}
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </Card>

                        <Card>
                            <div className="p-6">
                                <div className="flex items-center gap-4">
                                    <div className="p-3 bg-green-100 rounded-lg">
                                        <DollarSign className="w-6 h-6 text-green-600" />
                                    </div>
                                    <div>
                                        <p className="text-sm text-gray-600">Tổng Chi Tiêu</p>
                                        <p className="text-2xl font-bold text-gray-900">
                                            {formatCurrency(customer.totalSpent)}
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </Card>

                        <Card>
                            <div className="p-6">
                                <div className="flex items-center gap-4">
                                    <div className="p-3 bg-purple-100 rounded-lg">
                                        <Star className="w-6 h-6 text-purple-600" />
                                    </div>
                                    <div>
                                        <p className="text-sm text-gray-600">Điểm Tích Lũy</p>
                                        <p className="text-2xl font-bold text-gray-900">
                                            {customer.loyaltyPoints || 0}
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </Card>
                    </div>

                    {customer.addresses && customer.addresses.length > 0 && (
                        <Card>
                            <div className="p-6">
                                <h3 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
                                    <MapPin className="w-5 h-5" />
                                    Địa Chỉ Khách Hàng
                                </h3>
                                <div className="space-y-4">
                                    {customer.addresses.map((address: any, index: number) => (
                                        <div key={address.id || index} className="p-4 bg-gray-50 rounded-lg">
                                            <div className="flex items-start justify-between">
                                                <div>
                                                    <div className="flex items-center gap-2 mb-2">
                                                        <p className="font-medium text-gray-900">
                                                            {address.recipientName || customer.name}
                                                        </p>
                                                        {address.addressTypeText && (
                                                            <Badge variant="default">{address.addressTypeText}</Badge>
                                                        )}
                                                    </div>
                                                    {address.streetAddress && (
                                                        <p className="text-gray-600 mt-1">{address.streetAddress}</p>
                                                    )}
                                                    <p className="text-gray-600">
                                                        {[address.city, address.state, address.country].filter(Boolean).join(', ')}
                                                    </p>
                                                    {address.phoneNumber && (
                                                        <p className="text-gray-600 mt-1">SĐT: {address.phoneNumber}</p>
                                                    )}
                                                </div>
                                                <div className="flex flex-col gap-2">
                                                    {address.isDefault && <Badge variant="primary">Mặc định</Badge>}
                                                    {address.isPrimary && <Badge variant="info">Chính</Badge>}
                                                </div>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </Card>
                    )}
                </>
            )}

            {/* Show message if neither Employee nor Customer */}
            {!isEmployee && !isCustomer && (
                <Card>
                    <div className="p-6 text-center text-gray-500">
                        <Users className="w-12 h-12 mx-auto mb-2 text-gray-400" />
                        <p>Người dùng này chưa có hồ sơ nhân viên hoặc khách hàng</p>
                    </div>
                </Card>
            )}
        </div>
    );
};

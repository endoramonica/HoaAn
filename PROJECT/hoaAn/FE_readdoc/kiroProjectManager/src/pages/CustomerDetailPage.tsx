import { useParams, useNavigate } from 'react-router-dom';
import { useGetApiAdminCustomerId, useGetApiAdminCustomerIdStatistics, useGetApiAdminCustomerIdOrdersSummary } from '../../api/generated-orval/admin-customer/admin-customer';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { ArrowLeft, Mail, Phone, MapPin, Calendar, ShoppingBag, DollarSign, Star, User } from 'lucide-react';

export const CustomerDetailPage = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    
    const { data: customerData, isLoading } = useGetApiAdminCustomerId(id!);
    const { data: statisticsData } = useGetApiAdminCustomerIdStatistics(id!);
    const { data: orderSummaryData } = useGetApiAdminCustomerIdOrdersSummary(id!);
    
    const customer = customerData?.data;
    const statistics = statisticsData?.data;
    const orderSummary = orderSummaryData?.data;

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

    const getTierBadge = (tier?: string | null) => {
        switch (tier?.toUpperCase()) {
            case 'GOLD': return <Badge variant="warning">Vàng</Badge>;
            case 'SILVER': return <Badge variant="info">Bạc</Badge>;
            case 'BRONZE': return <Badge variant="default">Đồng</Badge>;
            case 'PLATINUM': return <Badge variant="danger">Bạch Kim</Badge>;
            default: return <Badge variant="default">Thường</Badge>;
        }
    };

    if (isLoading) {
        return (
            <div className="flex items-center justify-center h-64">
                <div className="text-gray-500">Đang tải...</div>
            </div>
        );
    }

    if (!customer) {
        return (
            <div className="flex items-center justify-center h-64">
                <div className="text-gray-500">Không tìm thấy khách hàng</div>
            </div>
        );
    }

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div className="flex items-center gap-4">
                    <Button 
                        variant="ghost" 
                        icon={<ArrowLeft className="w-5 h-5" />}
                        onClick={() => navigate('/customers')}
                    >
                        Quay lại
                    </Button>
                    <h1 className="text-2xl font-bold text-gray-900">
                        Chi Tiết Khách Hàng
                    </h1>
                </div>
                <Badge variant={customer.isActive ? 'success' : 'danger'}>
                    {customer.isActive ? 'Hoạt động' : 'Ngừng hoạt động'}
                </Badge>
            </div>

            {/* Customer Info Card */}
            <Card>
                <div className="p-6">
                    <div className="flex items-start gap-6">
                        <div className="flex-shrink-0">
                            <div className="w-24 h-24 rounded-full bg-gray-200 flex items-center justify-center">
                                <User className="w-12 h-12 text-gray-500" />
                            </div>
                        </div>
                        <div className="flex-1">
                            <h2 className="text-2xl font-bold text-gray-900 mb-2">
                                {customer.name || 'Chưa có tên'}
                            </h2>
                            <div className="flex items-center gap-4 mb-4">
                                {getTierBadge(customer.tier)}
                                <div className="flex items-center gap-2 text-gray-600">
                                    <Star className="w-4 h-4" />
                                    <span>{customer.loyaltyPoints || 0} điểm</span>
                                </div>
                            </div>
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                {customer.email && (
                                    <div className="flex items-center gap-2 text-gray-600">
                                        <Mail className="w-4 h-4" />
                                        <span>{customer.email}</span>
                                    </div>
                                )}
                                {customer.phone && (
                                    <div className="flex items-center gap-2 text-gray-600">
                                        <Phone className="w-4 h-4" />
                                        <span>{customer.phone}</span>
                                    </div>
                                )}
                                <div className="flex items-center gap-2 text-gray-600">
                                    <Calendar className="w-4 h-4" />
                                    <span>Tham gia: {formatDate(customer.createdAt)}</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </Card>

            {/* Statistics Cards */}
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

            {/* Addresses */}
            {customer.addresses && customer.addresses.length > 0 && (
                <Card>
                    <div className="p-6">
                        <h3 className="text-lg font-semibold text-gray-900 mb-4 flex items-center gap-2">
                            <MapPin className="w-5 h-5" />
                            Địa Chỉ
                        </h3>
                        <div className="space-y-4">
                            {customer.addresses.map((address, index) => (
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
                                                <p className="text-gray-600 mt-1">
                                                    {address.streetAddress}
                                                </p>
                                            )}
                                            <p className="text-gray-600">
                                                {[address.city, address.state, address.country].filter(Boolean).join(', ')}
                                            </p>
                                            {address.postalCode && (
                                                <p className="text-gray-600">
                                                    Mã bưu điện: {address.postalCode}
                                                </p>
                                            )}
                                            {address.phoneNumber && (
                                                <p className="text-gray-600 mt-1">
                                                    SĐT: {address.phoneNumber}
                                                </p>
                                            )}
                                            {address.email && (
                                                <p className="text-gray-600">
                                                    Email: {address.email}
                                                </p>
                                            )}
                                        </div>
                                        <div className="flex flex-col gap-2">
                                            {address.isDefault && (
                                                <Badge variant="primary">Mặc định</Badge>
                                            )}
                                            {address.isPrimary && (
                                                <Badge variant="info">Chính</Badge>
                                            )}
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </Card>
            )}

            {/* Store Info */}
            {customer.storeName && (
                <Card>
                    <div className="p-6">
                        <h3 className="text-lg font-semibold text-gray-900 mb-4">
                            Thông Tin Cửa Hàng
                        </h3>
                        <p className="text-gray-600">
                            <span className="font-medium">Cửa hàng:</span> {customer.storeName}
                        </p>
                    </div>
                </Card>
            )}
        </div>
    );
};

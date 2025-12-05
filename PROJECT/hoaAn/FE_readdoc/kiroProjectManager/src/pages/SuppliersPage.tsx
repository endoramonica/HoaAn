import { useState } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { Plus, Edit, Trash2, Phone, Mail, MapPin } from 'lucide-react';
import { useGetApiSuppliers, usePostApiSuppliers, usePutApiSuppliersId, useDeleteApiSuppliersId } from '../../api/generated-orval/suppliers/suppliers';

export const SuppliersPage = () => {
    const [showModal, setShowModal] = useState(false);
    const [selectedSupplier, setSelectedSupplier] = useState<any>(null);

    const { data: suppliersData, refetch } = useGetApiSuppliers({});
    const createMutation = usePostApiSuppliers();
    const updateMutation = usePutApiSuppliersId();
    const deleteMutation = useDeleteApiSuppliersId();

    const suppliers = suppliersData?.data?.data?.items || [];

    const handleSubmit = async (data: any) => {
        try {
            if (selectedSupplier) {
                await updateMutation.mutateAsync({ id: selectedSupplier.id, data });
            } else {
                await createMutation.mutateAsync({ data });
            }
            setShowModal(false);
            setSelectedSupplier(null);
            refetch();
        } catch (error) {
            console.error('Error:', error);
        }
    };

    const handleDelete = async (id: string) => {
        if (confirm('Bạn có chắc muốn xóa nhà cung cấp này?')) {
            try {
                await deleteMutation.mutateAsync({ id });
                refetch();
            } catch (error) {
                console.error('Error:', error);
            }
        }
    };

    const getStatusBadge = (status: string) => {
        return status === 'active' 
            ? <Badge variant="success">Hoạt Động</Badge>
            : <Badge variant="default">Ngừng</Badge>;
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">Quản Lý Nhà Cung Cấp</h1>
                <Button 
                    variant="primary" 
                    icon={<Plus className="w-5 h-5" />}
                    onClick={() => setShowModal(true)}
                >
                    Thêm Nhà Cung Cấp
                </Button>
            </div>

            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Tên</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Liên Hệ</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Địa Chỉ</th>
                                <th className="px-4 py-3 text-left text-sm font-semibold">Trạng Thái</th>
                                <th className="px-4 py-3 text-right text-sm font-semibold">Thao Tác</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {suppliers.map((supplier: any) => (
                                <tr key={supplier.id} className="hover:bg-gray-50">
                                    <td className="px-4 py-3 font-medium">{supplier.name}</td>
                                    <td className="px-4 py-3">
                                        <div className="space-y-1">
                                            {supplier.contactPerson && (
                                                <p className="text-sm">{supplier.contactPerson}</p>
                                            )}
                                            {supplier.phone && (
                                                <div className="flex items-center gap-1 text-sm text-gray-600">
                                                    <Phone className="w-3 h-3" />
                                                    {supplier.phone}
                                                </div>
                                            )}
                                            {supplier.email && (
                                                <div className="flex items-center gap-1 text-sm text-gray-600">
                                                    <Mail className="w-3 h-3" />
                                                    {supplier.email}
                                                </div>
                                            )}
                                        </div>
                                    </td>
                                    <td className="px-4 py-3">
                                        {supplier.address && (
                                            <div className="flex items-start gap-1 text-sm text-gray-600">
                                                <MapPin className="w-3 h-3 mt-0.5" />
                                                <span className="line-clamp-2">{supplier.address}</span>
                                            </div>
                                        )}
                                    </td>
                                    <td className="px-4 py-3">{getStatusBadge(supplier.status)}</td>
                                    <td className="px-4 py-3 text-right">
                                        <div className="flex items-center justify-end gap-2">
                                            <Button 
                                                variant="ghost" 
                                                size="sm" 
                                                icon={<Edit className="w-4 h-4" />}
                                                onClick={() => {
                                                    setSelectedSupplier(supplier);
                                                    setShowModal(true);
                                                }}
                                            >
                                                Sửa
                                            </Button>
                                            <Button 
                                                variant="danger" 
                                                size="sm" 
                                                icon={<Trash2 className="w-4 h-4" />}
                                                onClick={() => handleDelete(supplier.id)}
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
                <SupplierFormModal 
                    supplier={selectedSupplier}
                    onClose={() => {
                        setShowModal(false);
                        setSelectedSupplier(null);
                    }}
                    onSubmit={handleSubmit}
                />
            )}
        </div>
    );
};

const SupplierFormModal = ({ supplier, onClose, onSubmit }: any) => {
    const [formData, setFormData] = useState({
        name: supplier?.name || '',
        contactPerson: supplier?.contactPerson || '',
        phone: supplier?.phone || '',
        email: supplier?.email || '',
        address: supplier?.address || '',
        status: supplier?.status || 'active',
        notes: supplier?.notes || ''
    });

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <Card className="w-full max-w-2xl max-h-[90vh] overflow-y-auto">
                <h2 className="text-xl font-bold mb-4">
                    {supplier ? 'Chỉnh Sửa Nhà Cung Cấp' : 'Thêm Nhà Cung Cấp Mới'}
                </h2>
                <div className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium mb-1">Tên Nhà Cung Cấp *</label>
                        <input 
                            type="text"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.name}
                            onChange={(e) => setFormData({...formData, name: e.target.value})}
                            required
                        />
                    </div>
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium mb-1">Người Liên Hệ</label>
                            <input 
                                type="text"
                                className="w-full px-3 py-2 border rounded-lg"
                                value={formData.contactPerson}
                                onChange={(e) => setFormData({...formData, contactPerson: e.target.value})}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium mb-1">Số Điện Thoại</label>
                            <input 
                                type="tel"
                                className="w-full px-3 py-2 border rounded-lg"
                                value={formData.phone}
                                onChange={(e) => setFormData({...formData, phone: e.target.value})}
                            />
                        </div>
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Email</label>
                        <input 
                            type="email"
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.email}
                            onChange={(e) => setFormData({...formData, email: e.target.value})}
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Địa Chỉ</label>
                        <textarea 
                            className="w-full px-3 py-2 border rounded-lg"
                            rows={3}
                            value={formData.address}
                            onChange={(e) => setFormData({...formData, address: e.target.value})}
                        />
                    </div>
                    <div>
                        <label className="block text-sm font-medium mb-1">Trạng Thái</label>
                        <select 
                            className="w-full px-3 py-2 border rounded-lg"
                            value={formData.status}
                            onChange={(e) => setFormData({...formData, status: e.target.value})}
                        >
                            <option value="active">Hoạt Động</option>
                            <option value="inactive">Ngừng Hoạt Động</option>
                        </select>
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
                        <Button variant="primary" onClick={() => onSubmit(formData)}>
                            {supplier ? 'Cập Nhật' : 'Thêm Mới'}
                        </Button>
                    </div>
                </div>
            </Card>
        </div>
    );
};

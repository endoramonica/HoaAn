import { useState } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { Select } from '../components/ui/Select';
import { toast } from 'sonner';
import { Save } from 'lucide-react';

export const SettingsPage = () => {
    const [storeName, setStoreName] = useState('Cửa Hàng Đồ Cúng');
    const [phone, setPhone] = useState('0123456789');
    const [address, setAddress] = useState('123 Đường ABC, Quận 1, TP.HCM');
    const [language, setLanguage] = useState('vi');
    const [vatRate, setVatRate] = useState('10');

    const handleSave = () => {
        // Save to localStorage
        localStorage.setItem('settings', JSON.stringify({
            storeName,
            phone,
            address,
            language,
            vatRate,
        }));
        toast.success('Lưu cài đặt thành công');
    };

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-gray-900">
                    Cài Đặt
                </h1>
                <Button
                    variant="primary"
                    icon={<Save className="w-5 h-5" />}
                    onClick={handleSave}
                >
                    Lưu Thay Đổi
                </Button>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                <Card title="Thông Tin Cửa Hàng">
                    <div className="space-y-4">
                        <Input
                            label="Tên cửa hàng"
                            value={storeName}
                            onChange={(e) => setStoreName(e.target.value)}
                        />
                        <Input
                            label="Số điện thoại"
                            value={phone}
                            onChange={(e) => setPhone(e.target.value)}
                        />
                        <Input
                            label="Địa chỉ"
                            value={address}
                            onChange={(e) => setAddress(e.target.value)}
                        />
                    </div>
                </Card>

                <Card title="Giao Diện">
                    <div className="space-y-4">
                        <Select
                            label="Ngôn ngữ"
                            options={[
                                { value: 'vi', label: 'Tiếng Việt' },
                                { value: 'en', label: 'English' },
                            ]}
                            value={language}
                            onChange={setLanguage}
                        />
                    </div>
                </Card>

                <Card title="Hệ Thống">
                    <div className="space-y-4">
                        <Input
                            label="Thuế VAT (%)"
                            type="number"
                            value={vatRate}
                            onChange={(e) => setVatRate(e.target.value)}
                        />
                    </div>
                </Card>

                <Card title="Tài Khoản">
                    <div className="space-y-4">
                        <Input
                            label="Email"
                            type="email"
                            value="admin@example.com"
                            disabled
                        />
                        <Button variant="secondary" className="w-full">
                            Đổi Mật Khẩu
                        </Button>
                    </div>
                </Card>
            </div>
        </div>
    );
};

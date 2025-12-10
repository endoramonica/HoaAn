import { useState } from 'react';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogHeader,
  AlertDialogTitle,
} from '../../components/ui/alert-dialog';
import { Input } from '../../components/ui/input';
import { Label } from '../../components/ui/label';
import { Loader2 } from 'lucide-react';

interface BookingDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onConfirm: (data: BookingFormData) => Promise<void>;
  isLoading?: boolean;
  eventTitle?: string;
  selectedDate?: Date;
}

export interface BookingFormData {
  // Thông tin khách hàng
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  
  // Thông tin lịch
  serviceDate: string;
  serviceDuration: string;
  serviceLocation: string;
  serviceNotes: string;
}

export function BookingDialog({
  open,
  onOpenChange,
  onConfirm,
  isLoading = false,
  eventTitle = 'Sự kiện',
  selectedDate,
}: BookingDialogProps) {
  const [formData, setFormData] = useState<BookingFormData>({
    customerName: localStorage.getItem('customerName') || '',
    customerPhone: localStorage.getItem('customerPhone') || '',
    customerEmail: localStorage.getItem('customerEmail') || '',
    serviceDate: selectedDate?.toISOString().split('T')[0] || '',
    serviceDuration: '1 giờ',
    serviceLocation: 'Online',
    serviceNotes: '',
  });

  const [errors, setErrors] = useState<Partial<BookingFormData>>({});

  const validateForm = (): boolean => {
    const newErrors: Partial<BookingFormData> = {};

    if (!formData.customerName.trim()) {
      newErrors.customerName = 'Vui lòng nhập tên';
    }

    if (!formData.customerPhone.trim()) {
      newErrors.customerPhone = 'Vui lòng nhập số điện thoại';
    } else if (!/^[0-9]{10,11}$/.test(formData.customerPhone.replace(/\D/g, ''))) {
      newErrors.customerPhone = 'Số điện thoại không hợp lệ';
    }

    if (!formData.customerEmail.trim()) {
      newErrors.customerEmail = 'Vui lòng nhập email';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.customerEmail)) {
      newErrors.customerEmail = 'Email không hợp lệ';
    }

    if (!formData.serviceDate.trim()) {
      newErrors.serviceDate = 'Vui lòng chọn ngày';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleConfirm = async () => {
    if (!validateForm()) return;

    // Lưu thông tin vào localStorage
    localStorage.setItem('customerName', formData.customerName);
    localStorage.setItem('customerPhone', formData.customerPhone);
    localStorage.setItem('customerEmail', formData.customerEmail);

    await onConfirm(formData);
  };

  return (
    <AlertDialog open={open} onOpenChange={onOpenChange}>
      <AlertDialogContent className="max-w-md">
        <AlertDialogHeader>
          <AlertDialogTitle>Đặt lịch - {eventTitle}</AlertDialogTitle>
          <AlertDialogDescription>
            Vui lòng nhập thông tin liên hệ để nhân viên có thể gọi lại tư vấn
          </AlertDialogDescription>
        </AlertDialogHeader>

        <div className="py-4 max-h-[70vh] overflow-y-auto">
          {/* Thông tin khách hàng */}
          <div className="mb-6 pb-4 border-b">
            <h3 className="text-sm font-semibold text-slate-900 mb-4">Thông tin khách hàng</h3>
            
            <div className="space-y-3">
              <div className="space-y-1.5">
                <Label htmlFor="name" className="text-xs font-medium text-slate-700">
                  Họ và tên <span className="text-red-500">*</span>
                </Label>
                <Input
                  id="name"
                  placeholder="Nhập họ và tên"
                  value={formData.customerName}
                  onChange={(e) => {
                    setFormData({ ...formData, customerName: e.target.value });
                    if (errors.customerName) {
                      setErrors({ ...errors, customerName: undefined });
                    }
                  }}
                  disabled={isLoading}
                  className={`text-sm ${errors.customerName ? 'border-red-500' : ''}`}
                />
                {errors.customerName && (
                  <p className="text-xs text-red-500">{errors.customerName}</p>
                )}
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div className="space-y-1.5">
                  <Label htmlFor="phone" className="text-xs font-medium text-slate-700">
                    Số điện thoại <span className="text-red-500">*</span>
                  </Label>
                  <Input
                    id="phone"
                    placeholder="0123456789"
                    value={formData.customerPhone}
                    onChange={(e) => {
                      setFormData({ ...formData, customerPhone: e.target.value });
                      if (errors.customerPhone) {
                        setErrors({ ...errors, customerPhone: undefined });
                      }
                    }}
                    disabled={isLoading}
                    className={`text-sm ${errors.customerPhone ? 'border-red-500' : ''}`}
                  />
                  {errors.customerPhone && (
                    <p className="text-xs text-red-500">{errors.customerPhone}</p>
                  )}
                </div>

                <div className="space-y-1.5">
                  <Label htmlFor="email" className="text-xs font-medium text-slate-700">
                    Email <span className="text-red-500">*</span>
                  </Label>
                  <Input
                    id="email"
                    type="email"
                    placeholder="email@example.com"
                    value={formData.customerEmail}
                    onChange={(e) => {
                      setFormData({ ...formData, customerEmail: e.target.value });
                      if (errors.customerEmail) {
                        setErrors({ ...errors, customerEmail: undefined });
                      }
                    }}
                    disabled={isLoading}
                    className={`text-sm ${errors.customerEmail ? 'border-red-500' : ''}`}
                  />
                  {errors.customerEmail && (
                    <p className="text-xs text-red-500">{errors.customerEmail}</p>
                  )}
                </div>
              </div>
            </div>
          </div>

          {/* Thông tin lịch */}
          <div>
            <h3 className="text-sm font-semibold text-slate-900 mb-4">Thông tin lịch</h3>
            
            <div className="space-y-3">
              <div className="grid grid-cols-2 gap-3">
                <div className="space-y-1.5">
                  <Label htmlFor="serviceDate" className="text-xs font-medium text-slate-700">
                    Ngày dự kiến <span className="text-red-500">*</span>
                  </Label>
                  <Input
                    id="serviceDate"
                    type="date"
                    value={formData.serviceDate}
                    onChange={(e) => {
                      setFormData({ ...formData, serviceDate: e.target.value });
                      if (errors.serviceDate) {
                        setErrors({ ...errors, serviceDate: undefined });
                      }
                    }}
                    disabled={isLoading}
                    className={`text-sm ${errors.serviceDate ? 'border-red-500' : ''}`}
                  />
                  {errors.serviceDate && (
                    <p className="text-xs text-red-500">{errors.serviceDate}</p>
                  )}
                </div>

                <div className="space-y-1.5">
                  <Label htmlFor="serviceDuration" className="text-xs font-medium text-slate-700">
                    Thời lượng
                  </Label>
                  <select
                    id="serviceDuration"
                    value={formData.serviceDuration}
                    onChange={(e) => setFormData({ ...formData, serviceDuration: e.target.value })}
                    disabled={isLoading}
                    className="w-full px-3 py-2 border border-slate-300 rounded-md text-sm bg-white"
                  >
                    <option value="30 phút">30 phút</option>
                    <option value="1 giờ">1 giờ</option>
                    <option value="2 giờ">2 giờ</option>
                    <option value="3 giờ">3 giờ</option>
                    <option value="Cả ngày">Cả ngày</option>
                  </select>
                </div>
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="serviceLocation" className="text-xs font-medium text-slate-700">
                  Địa điểm
                </Label>
                <select
                  id="serviceLocation"
                  value={formData.serviceLocation}
                  onChange={(e) => setFormData({ ...formData, serviceLocation: e.target.value })}
                  disabled={isLoading}
                  className="w-full px-3 py-2 border border-slate-300 rounded-md text-sm bg-white"
                >
                  <option value="Online">Online</option>
                  <option value="Tại nhà">Tại nhà</option>
                  <option value="Tại cửa hàng">Tại cửa hàng</option>
                </select>
              </div>

              <div className="space-y-1.5">
                <Label htmlFor="serviceNotes" className="text-xs font-medium text-slate-700">
                  Ghi chú thêm
                </Label>
                <textarea
                  id="serviceNotes"
                  placeholder="Nhập ghi chú thêm (tùy chọn)"
                  value={formData.serviceNotes}
                  onChange={(e) => setFormData({ ...formData, serviceNotes: e.target.value })}
                  disabled={isLoading}
                  className="w-full px-3 py-2 border border-slate-300 rounded-md text-sm resize-none"
                  rows={2}
                />
              </div>
            </div>
          </div>
        </div>

        <div className="flex gap-3 justify-end">
          <AlertDialogCancel disabled={isLoading}>Hủy</AlertDialogCancel>
          <AlertDialogAction
            onClick={handleConfirm}
            disabled={isLoading}
            className="bg-amber-500 hover:bg-amber-600 flex items-center gap-2"
          >
            {isLoading && <Loader2 className="w-4 h-4 animate-spin" />}
            {isLoading ? 'Đang xử lý...' : 'Đặt lịch'}
          </AlertDialogAction>
        </div>
      </AlertDialogContent>
    </AlertDialog>
  );
}

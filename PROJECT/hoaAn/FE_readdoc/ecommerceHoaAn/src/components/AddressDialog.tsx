import { useState } from "react";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Label } from "./ui/label";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "./ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";
import { Loader2 } from "lucide-react";
import { toast } from "sonner";
import { addressService } from "../lib/services/addressService";
import type { CreateAddressDto, UpdateAddressDto } from "../../../Api/generated-orval/schemas";
import type { AddressResponseDto } from "../../../Api/generated-orval/schemas/addressResponseDto";

interface AddressDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  address?: AddressResponseDto;
  onSuccess: (address: AddressResponseDto) => void;
}

export function AddressDialog({
  open,
  onOpenChange,
  address,
  onSuccess,
}: AddressDialogProps) {
  const isEditing = !!address;
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    recipientName: address?.recipientName || "",
    phoneNumber: address?.phoneNumber || "",
    streetAddress: address?.streetAddress || "",
    city: address?.city || "",
    state: address?.state || "",
    postalCode: address?.postalCode || "",
    country: address?.country || "",
    addressType: (address?.addressType as string) || "home",
    email: address?.email || "",
  });

  const handleChange = (field: string, value: string) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const handleSubmit = async () => {
    // Validation
    if (!formData.recipientName.trim()) {
      toast.error("Vui lòng nhập tên người nhận");
      return;
    }
    if (!formData.phoneNumber.trim()) {
      toast.error("Vui lòng nhập số điện thoại");
      return;
    }
    if (!formData.streetAddress.trim()) {
      toast.error("Vui lòng nhập địa chỉ");
      return;
    }
    if (!formData.city.trim()) {
      toast.error("Vui lòng nhập thành phố");
      return;
    }

    try {
      setLoading(true);

      let result: AddressResponseDto;

      if (isEditing && address?.id) {
        // Update address
        const updateData: UpdateAddressDto = {
          recipientName: formData.recipientName,
          phoneNumber: formData.phoneNumber,
          streetAddress: formData.streetAddress,
          city: formData.city,
          state: formData.state || undefined,
          postalCode: formData.postalCode || undefined,
          country: formData.country || undefined,
          addressType: formData.addressType as any,
          email: formData.email || undefined,
        };

        result = await addressService.updateAddress(address.id, updateData);
        toast.success("Cập nhật địa chỉ thành công!");
      } else {
        // Create new address
        const createData: CreateAddressDto = {
          recipientName: formData.recipientName,
          phoneNumber: formData.phoneNumber,
          streetAddress: formData.streetAddress,
          city: formData.city,
          state: formData.state || undefined,
          postalCode: formData.postalCode || undefined,
          country: formData.country || undefined,
          addressType: formData.addressType as any,
          email: formData.email || undefined,
        };

        result = await addressService.createAddress(createData);
        toast.success("Thêm địa chỉ thành công!");
      }

      onSuccess(result);
      onOpenChange(false);
    } catch (error) {
      console.error("[AddressDialog] Error:", error);
      toast.error(isEditing ? "Cập nhật địa chỉ thất bại" : "Thêm địa chỉ thất bại");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle className="text-amber-900">
            {isEditing ? "Chỉnh sửa địa chỉ" : "Thêm địa chỉ mới"}
          </DialogTitle>
          <DialogDescription>
            {isEditing
              ? "Cập nhật thông tin địa chỉ của bạn"
              : "Thêm một địa chỉ giao hàng mới"}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          {/* Recipient Name */}
          <div>
            <Label htmlFor="recipientName" className="text-gray-700">
              Tên người nhận *
            </Label>
            <Input
              id="recipientName"
              value={formData.recipientName}
              onChange={(e) => handleChange("recipientName", e.target.value)}
              placeholder="Nhập tên người nhận"
              className="border-amber-200"
            />
          </div>

          {/* Phone Number */}
          <div>
            <Label htmlFor="phoneNumber" className="text-gray-700">
              Số điện thoại *
            </Label>
            <Input
              id="phoneNumber"
              value={formData.phoneNumber}
              onChange={(e) => handleChange("phoneNumber", e.target.value)}
              placeholder="Nhập số điện thoại"
              className="border-amber-200"
            />
          </div>

          {/* Street Address */}
          <div>
            <Label htmlFor="streetAddress" className="text-gray-700">
              Địa chỉ *
            </Label>
            <Input
              id="streetAddress"
              value={formData.streetAddress}
              onChange={(e) => handleChange("streetAddress", e.target.value)}
              placeholder="Nhập địa chỉ đầy đủ"
              className="border-amber-200"
            />
          </div>

          {/* City */}
          <div>
            <Label htmlFor="city" className="text-gray-700">
              Thành phố *
            </Label>
            <Input
              id="city"
              value={formData.city}
              onChange={(e) => handleChange("city", e.target.value)}
              placeholder="Nhập thành phố"
              className="border-amber-200"
            />
          </div>

          {/* State */}
          <div>
            <Label htmlFor="state" className="text-gray-700">
              Tỉnh/Bang
            </Label>
            <Input
              id="state"
              value={formData.state}
              onChange={(e) => handleChange("state", e.target.value)}
              placeholder="Nhập tỉnh/bang"
              className="border-amber-200"
            />
          </div>

          {/* Postal Code */}
          <div>
            <Label htmlFor="postalCode" className="text-gray-700">
              Mã bưu điện
            </Label>
            <Input
              id="postalCode"
              value={formData.postalCode}
              onChange={(e) => handleChange("postalCode", e.target.value)}
              placeholder="Nhập mã bưu điện"
              className="border-amber-200"
            />
          </div>

          {/* Country */}
          <div>
            <Label htmlFor="country" className="text-gray-700">
              Quốc gia
            </Label>
            <Input
              id="country"
              value={formData.country}
              onChange={(e) => handleChange("country", e.target.value)}
              placeholder="Nhập quốc gia"
              className="border-amber-200"
            />
          </div>

          {/* Address Type */}
          <div>
            <Label htmlFor="addressType" className="text-gray-700">
              Loại địa chỉ *
            </Label>
            <Select value={formData.addressType} onValueChange={(value) => handleChange("addressType", value)}>
              <SelectTrigger className="border-amber-200">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="home">Nhà riêng</SelectItem>
                <SelectItem value="office">Văn phòng</SelectItem>
                <SelectItem value="other">Khác</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Email */}
          <div>
            <Label htmlFor="email" className="text-gray-700">
              Email
            </Label>
            <Input
              id="email"
              type="email"
              value={formData.email}
              onChange={(e) => handleChange("email", e.target.value)}
              placeholder="Nhập email"
              className="border-amber-200"
            />
          </div>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={loading}
          >
            Hủy
          </Button>
          <Button
            className="bg-red-600 hover:bg-red-700 text-white"
            onClick={handleSubmit}
            disabled={loading}
          >
            {loading ? (
              <>
                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                Đang lưu...
              </>
            ) : (
              isEditing ? "Cập nhật" : "Thêm"
            )}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}

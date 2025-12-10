# Tab Địa chỉ - API Integration (Updated)

## Tóm tắt
Đã cập nhật tab Địa chỉ trong ProfilePage để sử dụng API `/api/v1/customer/addresses` thay vì dữ liệu tĩnh. Tất cả các event (thêm, sửa, xóa, đặt mặc định) đều gọi đến các phương thức API được generate bởi Orval.

## Các file được tạo/cập nhật

### 1. **src/lib/services/addressService.ts** (NEW)
Service mới để quản lý các API call liên quan đến địa chỉ.

**Các phương thức:**
- `getAddresses()` - GET /api/v1/customer/addresses
- `getAddressById(id)` - GET /api/v1/customer/addresses/{id}
- `createAddress(data)` - POST /api/v1/customer/addresses
- `updateAddress(id, data)` - PUT /api/v1/customer/addresses/{id}
- `deleteAddress(id)` - DELETE /api/v1/customer/addresses/{id}
- `setDefaultAddress(id)` - POST /api/v1/customer/addresses/{id}/set-default

### 2. **src/components/AddressDialog.tsx** (NEW)
Dialog component để thêm/sửa địa chỉ với form validation.

**Features:**
- Form fields: recipientName, phoneNumber, streetAddress, city, state, postalCode, country, addressType, email
- Validation cho các field bắt buộc
- Support cho cả thêm mới và chỉnh sửa
- Loading state khi submit
- Toast notifications cho success/error

### 3. **src/components/ProfilePage.tsx** (UPDATED)

#### Import thêm:
```typescript
import { AddressDialog } from "./AddressDialog";
```

#### State mới:
```typescript
const [addresses, setAddresses] = useState<CustomerAddressDto[]>([]);
const [addressesLoading, setAddressesLoading] = useState(true);
const [addressesError, setAddressesError] = useState<string | null>(null);
const [addressDialogOpen, setAddressDialogOpen] = useState(false);
const [selectedAddress, setSelectedAddress] = useState<CustomerAddressDto | undefined>();
```

#### useEffect mới:
- Load địa chỉ từ API khi component mount (không cần customerId)

#### Event handlers:
- `handleDeleteAddress(addressId)` - Xóa địa chỉ
- `handleSetDefaultAddress(addressId)` - Đặt địa chỉ mặc định
- `handleEditAddress(addressId)` - Mở dialog để chỉnh sửa
- `handleAddAddress()` - Mở dialog để thêm mới
- `handleAddressDialogSuccess(address)` - Callback khi dialog thành công

#### Tab Địa chỉ cập nhật:
- Hiển thị loading state khi đang tải
- Hiển thị error state nếu có lỗi
- Hiển thị empty state nếu không có địa chỉ
- Nút "Thêm địa chỉ mới" gọi `handleAddAddress()`
- Nút "Chỉnh sửa" gọi `handleEditAddress()`
- Nút "Đặt mặc định" gọi `handleSetDefaultAddress()`
- Nút "Xóa" gọi `handleDeleteAddress()`

## API Endpoints sử dụng

Tất cả các endpoint đã được generate bởi Orval từ swagger.json:

| Method | Endpoint | Hàm API |
|--------|----------|---------|
| GET | /api/v1/customer/addresses | getApiV1CustomerAddresses |
| GET | /api/v1/customer/addresses/{id} | getApiV1CustomerAddressesId |
| POST | /api/v1/customer/addresses | postApiV1CustomerAddresses |
| PUT | /api/v1/customer/addresses/{id} | putApiV1CustomerAddressesId |
| DELETE | /api/v1/customer/addresses/{id} | deleteApiV1CustomerAddressesId |
| POST | /api/v1/customer/addresses/{id}/set-default | postApiV1CustomerAddressesIdSetDefault |

## Các tính năng đã implement

✅ Load danh sách địa chỉ từ API
✅ Hiển thị loading/error/empty states
✅ Xóa địa chỉ
✅ Đặt địa chỉ mặc định
✅ Thêm địa chỉ mới (với form validation)
✅ Chỉnh sửa địa chỉ (với form validation)
✅ Toast notifications cho các action
✅ Real-time UI update sau mỗi action

## Cách sử dụng

### Thêm địa chỉ mới:
```typescript
const newAddress = await addressService.createAddress({
  streetAddress: "123 Main St",
  city: "Hanoi",
  country: "Vietnam",
  phoneNumber: "0912345678",
  recipientName: "John Doe",
  addressType: "home"
});
```

### Cập nhật địa chỉ:
```typescript
const updated = await addressService.updateAddress(addressId, {
  streetAddress: "456 New St",
  city: "Ho Chi Minh",
  // ... other fields
});
```

### Xóa địa chỉ:
```typescript
await addressService.deleteAddress(addressId);
```

### Đặt mặc định:
```typescript
await addressService.setDefaultAddress(addressId);
```

## Form Fields

- **recipientName** (required): Tên người nhận
- **phoneNumber** (required): Số điện thoại
- **streetAddress** (required): Địa chỉ đầy đủ
- **city** (required): Thành phố
- **state** (optional): Tỉnh/Bang
- **postalCode** (optional): Mã bưu điện
- **country** (optional): Quốc gia
- **addressType** (required): Loại địa chỉ (home, office, other)
- **email** (optional): Email

## Notes

- Tất cả các API call đều có error handling và logging
- Toast notifications được hiển thị cho user feedback
- Loading states được hiển thị trong UI
- Dữ liệu được cập nhật real-time sau mỗi action
- Form validation trước khi submit
- Support cho cả thêm mới và chỉnh sửa trong cùng một dialog

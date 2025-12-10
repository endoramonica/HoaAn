# Profile Page API Integration - Fix

## Vấn đề
Lỗi: `apiClient.get is not a function` khi gọi API CustomerAdmin

## Nguyên nhân
- `apiClient` từ `orval-client.ts` là một hàm nhận config, không phải axios instance
- Cần sử dụng các hàm được generate từ Orval thay vì gọi trực tiếp

## Giải pháp

### Cập nhật customerAdminService.ts
Thay vì sử dụng `apiClient.get()` và `apiClient.put()`, sử dụng các hàm được generate từ Orval:

```typescript
import { getVietCommerceAPI } from '../../Api/generated-orval';

const api = getVietCommerceAPI();

export const customerAdminService = {
  async getCustomerById(id: string): Promise<CustomerDetailDto> {
    const response = await api.getApiV1CustomerAdminId(id);
    return response.data;
  },

  async updateCustomer(
    id: string,
    data: UpdateCustomerRequest
  ): Promise<CustomerDetailDto> {
    const response = await api.putApiV1CustomerAdminId(id, data);
    return response.data;
  },
};
```

### Các hàm Orval được sử dụng
- `api.getApiV1CustomerAdminId(id)` - GET /api/v1/CustomerAdmin/{id}
- `api.putApiV1CustomerAdminId(id, data)` - PUT /api/v1/CustomerAdmin/{id}

### Response Format
Cả hai hàm trả về `CustomerDetailDtoApiResponse`:
```typescript
{
  success: boolean;
  data: CustomerDetailDto;
  message?: string;
  errors?: string[];
}
```

Vì vậy cần lấy `response.data` để có được `CustomerDetailDto`

## Kiểm tra
✅ Service không có lỗi type
✅ ProfilePage component không có lỗi
✅ Sử dụng đúng API từ Orval generated

## Lưu ý
- Tất cả các hàm API được generate từ Orval đều có sẵn authentication (Bearer token)
- Response luôn có format `{ success, data, message, errors }`
- Cần extract `response.data` để lấy actual data

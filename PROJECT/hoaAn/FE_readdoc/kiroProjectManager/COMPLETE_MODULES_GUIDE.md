# Hướng Dẫn Đầy Đủ - Hệ Thống Quản Lý VietCommerce

## Tổng Quan

Hệ thống quản lý toàn diện cho VietCommerce bao gồm 9 module chính được tích hợp với API backend thông qua Orval code generation.

## Danh Sách Modules

### 1. 🔄 Quản Lý Ca Làm Việc (Shifts)
**Route:** `/shifts`  
**Icon:** Clock

#### Tính năng:
- ✅ Xem danh sách ca làm việc
- ✅ Mở ca mới (storeId, openingCash, notes)
- ✅ Đóng ca (closingCash, notes)
- ✅ Cập nhật thông tin ca
- ✅ Hiển thị trạng thái: Open / Closed

#### API Endpoints:
- `GET /api/Shifts` - Danh sách ca
- `POST /api/Shifts/open` - Mở ca mới
- `POST /api/Shifts/close/{id}` - Đóng ca
- `PUT /api/Shifts/{id}` - Cập nhật ca
- `GET /api/Shifts/current/{userId}` - Ca hiện tại

---

### 2. ✅ Quản Lý Công Việc (Tasks)
**Route:** `/tasks`  
**Icon:** CheckSquare

#### Tính năng:
- ✅ Xem danh sách tasks với phân trang
- ✅ Tạo task mới
- ✅ Chỉnh sửa task
- ✅ Xóa task
- ✅ Cập nhật trạng thái (Pending, In Progress, Completed)
- ✅ Lọc theo trạng thái và độ ưu tiên
- ✅ Cảnh báo tasks quá hạn

#### API Endpoints:
- `GET /api/Task` - Danh sách tasks
- `POST /api/Task` - Tạo task
- `PUT /api/Task/{id}` - Cập nhật task
- `DELETE /api/Task/{id}` - Xóa task
- `PATCH /api/Task/{id}/status` - Cập nhật trạng thái

#### Enums:
- **Status:** pending, inProgress, completed
- **Priority:** low, medium, high

---

### 3. 🚚 Quản Lý Nhà Cung Cấp (Suppliers)
**Route:** `/suppliers`  
**Icon:** Truck

#### Tính năng:
- ✅ Xem danh sách nhà cung cấp
- ✅ Thêm nhà cung cấp mới
- ✅ Chỉnh sửa thông tin
- ✅ Xóa nhà cung cấp
- ✅ Quản lý trạng thái (Active/Inactive)
- ✅ Thông tin liên hệ đầy đủ

#### API Endpoints:
- `GET /api/Suppliers` - Danh sách nhà cung cấp
- `POST /api/Suppliers` - Thêm mới
- `PUT /api/Suppliers/{id}` - Cập nhật
- `DELETE /api/Suppliers/{id}` - Xóa
- `GET /api/Suppliers/{id}/stats` - Thống kê

#### Schema:
```typescript
{
  name: string (required),
  contactPerson?: string,
  phone?: string,
  email?: string,
  address?: string,
  status: 'active' | 'inactive',
  notes?: string
}
```

---

### 4. 📝 Quản Lý Đơn Xin Nghỉ (Leave Requests)
**Route:** `/leave-requests`  
**Icon:** FileText

#### Tính năng:
- ✅ Xem danh sách đơn xin nghỉ
- ✅ Tạo đơn xin nghỉ mới
- ✅ Duyệt/Từ chối đơn
- ✅ Lọc theo trạng thái
- ✅ Hiển thị số ngày nghỉ
- ✅ Theo dõi người duyệt

#### API Endpoints:
- `GET /api/LeaveRequest` - Danh sách đơn
- `POST /api/LeaveRequest` - Tạo đơn mới
- `PATCH /api/LeaveRequest/{id}/status` - Cập nhật trạng thái

#### Enums:
- **Status:** pending, approved, rejected
- **Type:** annual, sick, unpaid, maternity

#### Schema:
```typescript
{
  leaveType: 'annual' | 'sick' | 'unpaid' | 'maternity',
  startDate: string (date-time),
  endDate: string (date-time),
  reason: string
}
```

---

### 5. 📦 Quản Lý Chuyển Kho (Stock Transfers)
**Route:** `/stock-transfers`  
**Icon:** ArrowRightLeft

#### Tính năng:
- ✅ Xem danh sách phiếu chuyển kho
- ✅ Tạo phiếu chuyển kho mới
- ✅ Hủy phiếu chuyển
- ✅ Lọc theo trạng thái
- ✅ Hiển thị kho nguồn/đích
- ✅ Theo dõi số lượng sản phẩm

#### API Endpoints:
- `GET /api/StockTransfers` - Danh sách phiếu
- `POST /api/StockTransfers` - Tạo phiếu mới
- `PATCH /api/StockTransfers/{id}/cancel` - Hủy phiếu
- `PUT /api/StockTransfers/{id}/status` - Cập nhật trạng thái
- `GET /api/StockTransfers/summary` - Thống kê

#### Enums:
- **Status:** pending, inTransit, delivered, cancelled

#### Schema:
```typescript
{
  fromStoreId: string (uuid),
  toStoreId: string (uuid),
  productId: string (uuid),
  quantity: number,
  notes?: string
}
```

---

### 6. 📅 Quản Lý Lịch Làm Việc (Work Schedules)
**Route:** `/work-schedules`  
**Icon:** CalendarDays

#### Tính năng:
- ✅ Xem lịch làm việc của nhân viên
- ✅ Thêm lịch làm việc mới
- ✅ Chỉnh sửa lịch
- ✅ Xóa lịch
- ✅ Phân loại ca (Thường, Tăng ca, Lễ)
- ✅ Theo dõi trạng thái (Scheduled, Confirmed, Completed, Missed)

#### API Endpoints:
- `GET /api/WorkSchedules` - Danh sách lịch
- `POST /api/WorkSchedules` - Thêm lịch mới
- `PUT /api/WorkSchedules/{id}` - Cập nhật lịch
- `DELETE /api/WorkSchedules/{id}` - Xóa lịch

#### Enums:
- **Type:** regular, overtime, holiday
- **Status:** scheduled, confirmed, completed, missed

#### Schema:
```typescript
{
  employeeId: string (uuid),
  date: string (date-time),
  startTime: string (time),
  endTime: string (time),
  type: 'regular' | 'overtime' | 'holiday',
  notes?: string
}
```

---

## Cấu Trúc Dự Án

```
src/
├── pages/
│   ├── ShiftsPage.tsx              # Quản lý ca làm việc
│   ├── TasksPage.tsx               # Quản lý công việc
│   ├── SuppliersPage.tsx           # Quản lý nhà cung cấp
│   ├── LeaveRequestsPage.tsx       # Quản lý đơn xin nghỉ
│   ├── StockTransfersPage.tsx      # Quản lý chuyển kho
│   └── WorkSchedulesPage.tsx       # Quản lý lịch làm việc
│
├── components/
│   ├── dashboard/
│   │   ├── TasksWidget.tsx         # Widget tasks
│   │   └── ShiftsWidget.tsx        # Widget shifts
│   └── ui/
│       ├── Card.tsx
│       ├── Button.tsx
│       └── Badge.tsx
│
├── lib/
│   └── hooks/
│       ├── useTasks.ts             # Custom hooks cho Tasks
│       └── useShifts.ts            # Custom hooks cho Shifts
│
├── layout/
│   ├── Layout.tsx
│   └── Sidebar.tsx                 # Navigation menu
│
└── router/
    └── index.tsx                   # Route definitions

api/generated-orval/
├── shifts/
├── task/
├── suppliers/
├── leave-request/
├── stock-transfers/
├── work-schedules/
└── schemas/
```

## Navigation Menu

Menu sidebar đã được cập nhật với các mục sau:

1. 🏠 Tổng Quan
2. 🛒 Bán Hàng
3. 📦 Sản Phẩm
4. 👥 Khách Hàng
5. 🛍️ Đơn Hàng
6. 👔 Nhân Viên
7. 👤 Người Dùng
8. 🕐 **Ca Làm Việc** (Mới)
9. 📅 **Lịch Làm Việc** (Mới)
10. 📝 **Đơn Xin Nghỉ** (Mới)
11. ✅ **Công Việc** (Mới)
12. 🏪 Kho Hàng
13. 🔄 **Chuyển Kho** (Mới)
14. 🚚 **Nhà Cung Cấp** (Mới)
15. 📢 Marketing
16. 🔔 Thông Báo
17. ⚙️ Cài Đặt

## Tính Năng Chung

### Pagination
Tất cả danh sách đều hỗ trợ phân trang:
- `Page`: Số trang (min: 1)
- `PageSize`: Số items/trang (min: 1, max: 100)
- `SortBy`: Sắp xếp theo field
- `SortDescending`: Sắp xếp giảm dần

### Filtering
Mỗi module có bộ lọc riêng:
- **Shifts:** Status, StoreId, UserId, StartTime
- **Tasks:** Status, Priority, AssignedTo, DueDate, IsOverdue
- **Suppliers:** Status, SearchTerm
- **Leave Requests:** Status, Type, EmployeeId
- **Stock Transfers:** Status, FromStore, ToStore, Product
- **Work Schedules:** EmployeeId, Date, Type, Status

### Response Structure
```typescript
{
  data: {
    items: T[],
    totalCount: number,
    pageNumber: number,
    pageSize: number,
    totalPages: number
  }
}
```

## Sử Dụng Custom Hooks

### Tasks
```typescript
import { useTasks, useCreateTask, useUpdateTask } from '@/lib/hooks/useTasks';

const { data } = useTasks({ Status: 'pending' });
const createMutation = useCreateTask();
```

### Shifts
```typescript
import { useShifts, useOpenShift, useCloseShift } from '@/lib/hooks/useShifts';

const { data } = useShifts({ Status: 'open' });
const openMutation = useOpenShift();
```

### Direct API Usage
```typescript
import { useGetApiSuppliers, usePostApiSuppliers } from '@/api/generated-orval/suppliers/suppliers';

const { data } = useGetApiSuppliers({});
const createMutation = usePostApiSuppliers();
```

## Authentication

Tất cả endpoints yêu cầu JWT Bearer token:
```typescript
Authorization: Bearer <token>
```

Token được quản lý tự động bởi `apiClient` trong `src/lib/api/orval-client.ts`

## Validation Rules

### Common
- UUID format cho tất cả IDs
- Date-time format: ISO 8601
- Max length cho text fields

### Specific
- **Task Title:** max 200 chars
- **Task Description:** max 1000 chars
- **Supplier Name:** required
- **Leave Request Reason:** required
- **Stock Transfer Quantity:** min 1

## Error Handling

```typescript
try {
  await mutation.mutateAsync({ data });
  refetch();
} catch (error) {
  console.error('Error:', error);
  // Handle error (show toast, alert, etc.)
}
```

## Best Practices

1. **Always refetch after mutations**
   ```typescript
   await createMutation.mutateAsync({ data });
   refetch();
   ```

2. **Use loading states**
   ```typescript
   const { data, isLoading } = useGetApiTasks({});
   if (isLoading) return <Spinner />;
   ```

3. **Handle empty states**
   ```typescript
   {items.length === 0 && <EmptyState />}
   ```

4. **Confirm destructive actions**
   ```typescript
   if (confirm('Bạn có chắc?')) {
     await deleteMutation.mutateAsync({ id });
   }
   ```

5. **Use TypeScript types from schemas**
   ```typescript
   import type { TasksCreateTaskRequest } from '@/api/generated-orval/schemas';
   ```

## Testing

### Manual Testing Checklist

#### Shifts
- [ ] Mở ca mới
- [ ] Đóng ca
- [ ] Xem danh sách ca
- [ ] Lọc theo trạng thái

#### Tasks
- [ ] Tạo task mới
- [ ] Cập nhật task
- [ ] Xóa task
- [ ] Cập nhật trạng thái
- [ ] Lọc theo status/priority

#### Suppliers
- [ ] Thêm nhà cung cấp
- [ ] Sửa thông tin
- [ ] Xóa nhà cung cấp
- [ ] Thay đổi trạng thái

#### Leave Requests
- [ ] Tạo đơn xin nghỉ
- [ ] Duyệt đơn
- [ ] Từ chối đơn
- [ ] Lọc theo trạng thái

#### Stock Transfers
- [ ] Tạo phiếu chuyển
- [ ] Hủy phiếu
- [ ] Xem chi tiết
- [ ] Lọc theo trạng thái

#### Work Schedules
- [ ] Thêm lịch làm việc
- [ ] Sửa lịch
- [ ] Xóa lịch
- [ ] Lọc theo nhân viên/ngày

## Troubleshooting

### Common Issues

1. **401 Unauthorized**
   - Kiểm tra JWT token
   - Đảm bảo user đã đăng nhập

2. **400 Bad Request**
   - Kiểm tra validation rules
   - Đảm bảo required fields có giá trị

3. **404 Not Found**
   - Kiểm tra ID có tồn tại
   - Đảm bảo UUID format đúng

4. **500 Internal Server Error**
   - Kiểm tra backend logs
   - Liên hệ team backend

### Debug Tips

```typescript
// Log response data
console.log('Data:', data);
console.log('Items:', data?.data?.data?.items);

// Log mutation errors
mutation.mutate(data, {
  onError: (error) => console.error('Mutation error:', error)
});

// Check network tab
// Xem request/response trong DevTools
```

## Performance Optimization

1. **Use pagination** - Không load tất cả data cùng lúc
2. **Implement caching** - React Query tự động cache
3. **Debounce search** - Giảm số lượng API calls
4. **Lazy load modals** - Chỉ render khi cần
5. **Optimize re-renders** - Use React.memo khi cần

## Security

1. **Never expose sensitive data** - Không log tokens
2. **Validate user input** - Client-side validation
3. **Use HTTPS** - Luôn dùng secure connection
4. **Implement RBAC** - Role-based access control
5. **Sanitize data** - Tránh XSS attacks

## Roadmap

### Phase 1 (Completed) ✅
- [x] Shifts Management
- [x] Tasks Management
- [x] Suppliers Management
- [x] Leave Requests Management
- [x] Stock Transfers Management
- [x] Work Schedules Management

### Phase 2 (Planned)
- [ ] Dashboard widgets cho tất cả modules
- [ ] Advanced filtering & search
- [ ] Export to Excel/PDF
- [ ] Bulk operations
- [ ] Real-time notifications
- [ ] Mobile responsive improvements

### Phase 3 (Future)
- [ ] Calendar view cho schedules
- [ ] Kanban board cho tasks
- [ ] Analytics & reporting
- [ ] Integration với third-party services
- [ ] Mobile app

## Support & Contact

Nếu có vấn đề hoặc câu hỏi:
1. Kiểm tra documentation này
2. Xem SHIFTS_TASKS_GUIDE.md cho chi tiết
3. Tạo issue trên repository
4. Liên hệ team phát triển

## License

Copyright © 2024 VietCommerce. All rights reserved.

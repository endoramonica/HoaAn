# Hướng Dẫn Sử Dụng Quản Lý Shift và Task

## Tổng Quan

Hệ thống quản lý Shift (Ca làm việc) và Task (Công việc) đã được tích hợp vào ứng dụng, sử dụng các API endpoints và schema được generate bởi Orval từ OpenAPI specification.

## Cấu Trúc Dự Án

### 1. API Generated (Orval)

```
api/generated-orval/
├── shifts/
│   └── shifts.ts          # API hooks cho Shifts
├── task/
│   └── task.ts            # API hooks cho Tasks
└── schemas/               # TypeScript types & schemas
```

### 2. Pages

```
src/pages/
├── ShiftsPage.tsx         # Trang quản lý ca làm việc
└── TasksPage.tsx          # Trang quản lý công việc
```

### 3. Custom Hooks

```
src/lib/hooks/
├── useShifts.ts           # Custom hooks cho Shifts
└── useTasks.ts            # Custom hooks cho Tasks
```

### 4. Dashboard Widgets

```
src/components/dashboard/
├── ShiftsWidget.tsx       # Widget hiển thị shifts trên dashboard
└── TasksWidget.tsx        # Widget hiển thị tasks trên dashboard
```

## Tính Năng

### Quản Lý Ca Làm Việc (Shifts)

#### Endpoints:
- `GET /api/Shifts` - Lấy danh sách ca làm việc
- `GET /api/Shifts/{id}` - Lấy chi tiết ca làm việc
- `POST /api/Shifts/open` - Mở ca làm việc mới
- `POST /api/Shifts/close/{id}` - Đóng ca làm việc
- `PUT /api/Shifts/{id}` - Cập nhật ca làm việc
- `GET /api/Shifts/current/{userId}` - Lấy ca làm việc hiện tại của user

#### Chức năng:
- ✅ Xem danh sách tất cả ca làm việc
- ✅ Mở ca làm việc mới (với thông tin: storeId, openingCash, notes)
- ✅ Đóng ca làm việc (với thông tin: closingCash, notes)
- ✅ Cập nhật thông tin ca làm việc
- ✅ Hiển thị trạng thái: Open (Đang Mở) / Closed (Đã Đóng)
- ✅ Hiển thị thông tin: Nhân viên, Cửa hàng, Thời gian, Tiền mở/đóng ca

#### Schema:
```typescript
// Open Shift Request
{
  storeId: string (uuid),
  openingCash: number,
  notes?: string
}

// Close Shift Request
{
  closingCash: number,
  notes?: string
}

// Update Shift Request
{
  notes?: string,
  status: 'open' | 'closed'
}
```

### Quản Lý Công Việc (Tasks)

#### Endpoints:
- `GET /api/Task` - Lấy danh sách công việc (có phân trang & filter)
- `GET /api/Task/{id}` - Lấy chi tiết công việc
- `POST /api/Task` - Tạo công việc mới
- `PUT /api/Task/{id}` - Cập nhật công việc
- `DELETE /api/Task/{id}` - Xóa công việc
- `PATCH /api/Task/{id}/status` - Cập nhật trạng thái công việc
- `POST /api/Task/{taskId}/assign/{assigneeId}` - Gán công việc cho người khác
- `POST /api/Task/{taskId}/complete` - Đánh dấu hoàn thành
- `GET /api/Task/assignee/{assigneeId}` - Lấy công việc của một người

#### Chức năng:
- ✅ Xem danh sách công việc với phân trang
- ✅ Tạo công việc mới
- ✅ Chỉnh sửa công việc
- ✅ Xóa công việc
- ✅ Cập nhật trạng thái nhanh
- ✅ Lọc theo trạng thái (Pending, In Progress, Completed)
- ✅ Lọc theo độ ưu tiên (Low, Medium, High)
- ✅ Hiển thị thông tin: Tiêu đề, Mô tả, Người được giao, Độ ưu tiên, Trạng thái, Hạn chót
- ✅ Cảnh báo công việc quá hạn

#### Schema:
```typescript
// Create Task Request
{
  title: string (required, max 200 chars),
  description: string (required, max 1000 chars),
  assignedTo: string (uuid, required),
  priority: 'low' | 'medium' | 'high' (required),
  dueDate?: string (date-time)
}

// Update Task Request
{
  title?: string (max 200 chars),
  description?: string (max 1000 chars),
  assignedTo?: string (uuid),
  priority?: 'low' | 'medium' | 'high',
  status?: 'pending' | 'inProgress' | 'completed',
  dueDate?: string (date-time)
}

// Task DTO (Response)
{
  id: string (uuid),
  title: string,
  description: string,
  assignedTo: string (uuid),
  assignedToName: string,
  assignedToEmail: string,
  assignedBy: string (uuid),
  assignedByName: string,
  priority: 'low' | 'medium' | 'high',
  priorityName: string,
  status: 'pending' | 'inProgress' | 'completed',
  statusName: string,
  dueDate?: string (date-time),
  completedAt?: string (date-time),
  createdAt: string (date-time),
  updatedAt: string (date-time),
  isOverdue: boolean,
  daysUntilDue?: number
}
```

## Cách Sử Dụng

### 1. Sử dụng Custom Hooks

#### Tasks:
```typescript
import { useTasks, useCreateTask, useUpdateTask, useDeleteTask } from '@/lib/hooks/useTasks';

// Lấy danh sách tasks
const { data: tasksData } = useTasks({ 
  Status: 'pending',
  Priority: 'high',
  PageSize: 10 
});

// Tạo task mới
const createMutation = useCreateTask();
await createMutation.mutateAsync({
  data: {
    title: 'Task mới',
    description: 'Mô tả task',
    assignedTo: 'user-uuid',
    priority: 'high'
  }
});

// Cập nhật task
const updateMutation = useUpdateTask();
await updateMutation.mutateAsync({
  id: 'task-id',
  data: { status: 'completed' }
});
```

#### Shifts:
```typescript
import { useShifts, useOpenShift, useCloseShift } from '@/lib/hooks/useShifts';

// Lấy danh sách shifts
const { data: shiftsData } = useShifts({ Status: 'open' });

// Mở ca mới
const openMutation = useOpenShift();
await openMutation.mutateAsync({
  data: {
    storeId: 'store-uuid',
    openingCash: 1000000,
    notes: 'Ca sáng'
  }
});

// Đóng ca
const closeMutation = useCloseShift();
await closeMutation.mutateAsync({
  id: 'shift-id',
  data: {
    closingCash: 2000000,
    notes: 'Kết thúc ca'
  }
});
```

### 2. Truy Cập Trang

- **Ca Làm Việc**: Truy cập `/shifts` hoặc click menu "Ca Làm Việc"
- **Công Việc**: Truy cập `/tasks` hoặc click menu "Công Việc"

### 3. Thêm Widgets vào Dashboard

```typescript
import { TasksWidget } from '@/components/dashboard/TasksWidget';
import { ShiftsWidget } from '@/components/dashboard/ShiftsWidget';

// Trong DashboardPage
<div className="grid grid-cols-1 md:grid-cols-2 gap-6">
  <TasksWidget />
  <ShiftsWidget />
</div>
```

## Filter & Pagination

### Tasks Filters:
- `Page`: Số trang (min: 1, max: 2147483647)
- `PageSize`: Số items mỗi trang (min: 1, max: 100)
- `SortBy`: Sắp xếp theo field (max 50 chars)
- `SortDescending`: Sắp xếp giảm dần (boolean)
- `UserId`: Lọc theo user ID
- `AssignedTo`: Lọc theo người được giao
- `AssignedBy`: Lọc theo người giao việc
- `Status`: Lọc theo trạng thái
- `Priority`: Lọc theo độ ưu tiên
- `DueDateFrom`: Lọc từ ngày
- `DueDateTo`: Lọc đến ngày
- `SearchTerm`: Tìm kiếm
- `IsOverdue`: Lọc công việc quá hạn
- `StoreId`: Lọc theo cửa hàng

### Shifts Filters:
- `Page`, `PageSize`, `SortBy`, `SortDescending`: Giống Tasks
- `UserId`: Lọc theo user ID
- `StoreId`: Lọc theo cửa hàng
- `Status`: Lọc theo trạng thái (open/closed)
- `StartTimeFrom`: Lọc từ thời gian bắt đầu
- `StartTimeTo`: Lọc đến thời gian bắt đầu

## Enums

### Task Status:
- `pending` - Chờ Xử Lý
- `inProgress` - Đang Làm
- `completed` - Hoàn Thành

### Task Priority:
- `low` - Thấp
- `medium` - Trung Bình
- `high` - Cao

### Shift Status:
- `open` - Đang Mở
- `closed` - Đã Đóng

## Lưu Ý

1. **Authentication**: Tất cả endpoints yêu cầu JWT Bearer token
2. **Validation**: 
   - Title: max 200 ký tự
   - Description: max 1000 ký tự
   - SortBy: max 50 ký tự
3. **UUID Format**: Tất cả ID phải là UUID hợp lệ
4. **Date Format**: Sử dụng ISO 8601 format (date-time)
5. **Pagination**: Response có cấu trúc PaginatedResponse với items, totalCount, pageNumber, pageSize

## Troubleshooting

### Lỗi thường gặp:

1. **401 Unauthorized**: Kiểm tra JWT token
2. **400 Bad Request**: Kiểm tra validation (required fields, max length)
3. **404 Not Found**: Kiểm tra ID có tồn tại không
4. **UUID Invalid**: Đảm bảo format UUID đúng

### Debug:

```typescript
// Enable logging
console.log('Tasks data:', tasksData);
console.log('Shifts data:', shiftsData);

// Check response structure
console.log('Items:', tasksData?.data?.data?.items);
console.log('Total:', tasksData?.data?.data?.totalCount);
```

## Tích Hợp Với Các Module Khác

### Với Employees:
- Gán task cho nhân viên
- Xem ca làm việc của nhân viên

### Với Stores:
- Quản lý ca làm việc theo cửa hàng
- Lọc task theo cửa hàng

### Với Dashboard:
- Hiển thị thống kê tasks & shifts
- Cảnh báo tasks quá hạn
- Hiển thị ca làm việc đang mở

## Roadmap

- [ ] Thêm notifications cho tasks mới
- [ ] Thêm calendar view cho shifts
- [ ] Thêm kanban board cho tasks
- [ ] Thêm báo cáo thống kê
- [ ] Thêm export/import tasks
- [ ] Thêm recurring tasks
- [ ] Thêm task templates
- [ ] Thêm shift templates

## Liên Hệ & Hỗ Trợ

Nếu có vấn đề hoặc câu hỏi, vui lòng tạo issue hoặc liên hệ team phát triển.

# POS System Documentation

## Tổng Quan / Overview

Hệ thống POS hoàn chỉnh với frontend React/TypeScript và backend C# .NET, hỗ trợ 3 roles (Staff, Manager, Admin) với phân quyền chi tiết, quản lý đa module (POS, CRM, LRM, HRM), và server-side pagination.

---

## 📚 Mục Lục / Table of Contents

### Backend Documentation

#### Database & Architecture
1. **[Database Schema](./01_database_schema.md)**
   - Cấu trúc database đầy đủ
   - Relationships & constraints
   - Indexes & performance

2. **[Backend Directory Structure](./BACKEND_DIRECTORY_STRUCTURE_COMPLETE.md)**
   - Cấu trúc thư mục dự án C#
   - Naming conventions
   - Best practices

#### API & Integration
3. **[API Endpoints](./02_api_endpoints.md)**
   - Tất cả REST API endpoints
   - Request/Response formats
   - Authentication & authorization

4. **[Server-Side Pagination](./07_server_side_pagination_implementation.md)** ⭐ NEW
   - Hướng dẫn implement pagination cho C# team
   - Endpoint patterns & examples
   - Performance optimization

5. **[Pagination Quick Start (Tiếng Việt)](./PAGINATION_QUICKSTART_VI.md)** ⭐ NEW
   - Hướng dẫn nhanh bằng Tiếng Việt
   - Code mẫu C#
   - Checklist implementation

6. **[Integration Notes](./06_integration_notes.md)**
   - Frontend-Backend integration
   - CORS & security
   - Error handling

#### Business Logic & Services
7. **[Workflow & Business Logic](./03_workflow_business_logic.md)**
   - Business rules cho từng module
   - State transitions
   - Validation rules

8. **[Backend Services Methods](./BACKEND_SERVICES_METHODS.md)**
   - Service layer methods
   - Repository patterns
   - Unit of Work

9. **[Notifications & Reports](./04_notifications_and_reports.md)**
   - Real-time notifications
   - Report generation
   - Email/SMS integration

#### Security & Permissions
10. **[Permissions & Roles](./05_permissions_roles.md)**
    - Role definitions (Staff, Manager, Admin)
    - Permission matrix
    - Access control logic

11. **[Backend Requirements Specification](./BACKEND_REQUIREMENTS_SPECIFICATION.md)**
    - Yêu cầu hệ thống đầy đủ
    - Non-functional requirements
    - Security requirements

#### Implementation Guides
12. **[Comprehensive Backend Guide](./COMPREHENSIVE_BACKEND_IMPLEMENTATION_GUIDE.txt)**
    - Hướng dẫn implementation tổng thể
    - Best practices
    - Common patterns

13. **[Detailed C# Implementation Guide](./DETAILED_BACKEND_IMPLEMENTATION_GUIDE_FOR_CSHARP_TEAM.txt)**
    - Chi tiết cho C# team
    - Code examples
    - Step-by-step instructions

---

### Frontend Documentation

#### User Guides
14. **[Frontend Pagination Guide](./FRONTEND_PAGINATION_GUIDE.md)** ⭐ NEW
    - Cách sử dụng `usePaginatedApi` hook
    - Examples & patterns
    - Best practices

---

## 🚀 Quick Start

### Cho Backend Team (C#)

1. **Setup Database**
   - Đọc [Database Schema](./01_database_schema.md)
   - Chạy migration scripts
   - Tạo indexes

2. **Implement Server-Side Pagination** ⭐ PRIORITY
   - Đọc [Pagination Quick Start VI](./PAGINATION_QUICKSTART_VI.md)
   - Implement core modules (Orders, Products, Inventory) trước
   - Test với Postman

3. **Implement API Endpoints**
   - Tham khảo [API Endpoints](./02_api_endpoints.md)
   - Follow pagination pattern
   - Implement authentication

4. **Business Logic**
   - Đọc [Workflow & Business Logic](./03_workflow_business_logic.md)
   - Implement validation rules
   - Handle state transitions

### Cho Frontend Team

1. **Sử dụng Pagination**
   - Đọc [Frontend Pagination Guide](./FRONTEND_PAGINATION_GUIDE.md)
   - Sử dụng `usePaginatedApi` hook
   - Update các pages còn lại

2. **Testing**
   - Test với mock API hiện tại
   - Khi backend ready, switch to real API
   - Verify role-based access

---

## 📋 Implementation Checklist

### Backend Pagination (Priority Order)

#### Week 1: Core Modules
- [ ] Orders (`GET /api/orders`)
- [ ] Products (`GET /api/products`)
- [ ] Inventory (`GET /api/inventory`)

#### Week 2: Management Modules
- [ ] Tasks (`GET /api/tasks`)
- [ ] Users/Staff (`GET /api/users`)
- [ ] Notifications (`GET /api/notifications`)
- [ ] Shifts (`GET /api/shifts`)

#### Week 3: Extended Modules
- [ ] Customers (`GET /api/crm/customers`)
- [ ] CRM Interactions (`GET /api/crm/interactions`)
- [ ] Suppliers (`GET /api/lrm/suppliers`)
- [ ] Stock Transfers (`GET /api/lrm/stock-transfers`)
- [ ] Employees (`GET /api/hrm/employees`)
- [ ] Leave Requests (`GET /api/hrm/leave-requests`)
- [ ] Work Schedules (`GET /api/hrm/work-schedules`)

#### Week 4: Admin Modules
- [ ] Audit Logs (`GET /api/admin/audit-logs`)
- [ ] User Management (Admin view)
- [ ] System Monitoring

### Frontend Updates
- [x] Create `usePaginatedApi` hook
- [x] Create API service layer
- [x] Update OrdersPage
- [x] Update NotificationsPage
- [ ] Update remaining pages (In Progress)

---

## 🔐 Roles & Permissions Summary

### Admin
- **Tất cả permissions tự động** (hard-coded trong `AuthContext`)
- Access toàn bộ modules
- View tất cả data across stores

### Manager
- Manage store của mình
- View tất cả orders/data trong store
- Assign tasks, approve leaves
- Access CRM, LRM, HRM modules

### Staff
- View own orders/tasks only
- Process payments, scan barcodes
- Submit leave requests
- Limited CRM/LRM access (view only)

Chi tiết đầy đủ: [Permissions & Roles](./05_permissions_roles.md)

---

## 🔧 Technical Stack

### Backend
- C# .NET 8.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- SignalR (for real-time notifications)

### Frontend
- React 18 + TypeScript
- React Router v6
- Context API (Auth, Theme, I18n)
- Tailwind CSS + shadcn/ui
- Recharts (analytics)

---

## 📞 API Response Format Standard

### Success Response (Paginated)
```json
{
  "data": [...],
  "meta": {
    "currentPage": 1,
    "pageSize": 10,
    "totalItems": 150,
    "totalPages": 15,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

### Success Response (Single Item)
```json
{
  "success": true,
  "data": { ... }
}
```

### Error Response
```json
{
  "success": false,
  "message": "Error message",
  "errors": ["Detail 1", "Detail 2"]
}
```

---

## 🎯 Performance Guidelines

### Database
- Index all filtered/sorted fields
- Use composite indexes for common filter combinations
- Regular maintenance & statistics updates

### API
- Use `AsNoTracking()` for read-only queries
- Only `Include()` necessary relations
- Implement caching for frequently accessed data
- Validate & sanitize all inputs
- Max PageSize = 100 (prevent abuse)

### Frontend
- Debounce search inputs (500ms)
- Use React.memo for list items
- Implement virtual scrolling for large lists
- Cache API responses when appropriate

---

## 🧪 Testing

### Backend
```csharp
// Unit test example
[Fact]
public async Task GetOrders_WithPagination_ReturnsCorrectPage()
{
    var result = await _controller.GetOrders(
        new PaginationParameters { Page = 2, PageSize = 5 });
    
    Assert.Equal(2, result.Meta.CurrentPage);
    Assert.True(result.Data.Count <= 5);
}
```

### Frontend
```typescript
// Hook test example
test('fetches paginated data', async () => {
  const { result } = renderHook(() =>
    usePaginatedApi({ fetchFn: mockFn })
  );
  
  await waitFor(() => expect(result.current.isLoading).toBe(false));
  expect(result.current.data).toHaveLength(10);
});
```

---

## 📖 Code Examples Repository

### Backend Examples
- See: `/docs/PAGINATION_QUICKSTART_VI.md` → Section 3
- Full controller examples with filters, sorting, role-based access

### Frontend Examples
- See: `/docs/FRONTEND_PAGINATION_GUIDE.md` → Complete Example section
- Hook usage patterns
- Advanced filtering & sorting

### Mock Implementation
- See: `/services/api.ts` → Reference implementation
- Demonstrates expected behavior
- Use for testing before backend is ready

---

## 🔄 Version History

| Version | Date | Description |
|---------|------|-------------|
| 1.0 | Nov 12, 2025 | Initial complete documentation |
| 1.1 | Nov 12, 2025 | Added server-side pagination guides |
| 1.2 | Nov 12, 2025 | Added frontend pagination guide |

---

## 📝 Contributing

### Documentation Updates
1. Update relevant `.md` file
2. Update version in document footer
3. Update this README if adding new docs

### Code Examples
1. Test examples before adding
2. Include comments
3. Follow existing patterns

---

## 🆘 Support & Contact

### Backend Team Lead
- [Name]
- [Email]
- [Slack/Teams]

### Frontend Team Lead
- [Name]
- [Email]
- [Slack/Teams]

---

## 📌 Important Notes

1. **Admin có TẤT CẢ permissions** - Không cần check từng permission riêng
2. **Pagination là PRIORITY** - Implement ngay để improve performance
3. **Role-based filtering là BẮT BUỘC** - Staff chỉ thấy data của mình
4. **Validate inputs** - Luôn validate PageSize, Page, filters
5. **Error handling** - Return standard error format
6. **Date format** - Luôn dùng ISO 8601
7. **UTF-8 encoding** - Support Vietnamese characters

---

## 🔗 Related Resources

- [React Documentation](https://react.dev/)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/)
- [Tailwind CSS](https://tailwindcss.com/)
- [shadcn/ui](https://ui.shadcn.com/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)

---

**Last Updated**: November 12, 2025  
**Maintained By**: POS Development Team

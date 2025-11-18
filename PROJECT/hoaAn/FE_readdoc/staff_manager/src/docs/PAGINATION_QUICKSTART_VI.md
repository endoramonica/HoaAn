# Hướng Dẫn Nhanh: Server-Side Pagination

## Tổng Quan

Frontend đã sẵn sàng sử dụng server-side pagination. Backend team cần implement các API endpoints theo format chuẩn dưới đây.

---

## 1. Format Request & Response Chuẩn

### Request
```
GET /api/{module}?page=1&pageSize=10&sortBy=createdAt&sortOrder=desc&filter[status]=pending
```

### Response
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

---

## 2. Danh Sách Endpoints Cần Implement

| Module | Endpoint | Filters Quan Trọng |
|--------|----------|-------------------|
| **Orders** | `GET /api/orders` | status, search, staffId, startDate, endDate |
| **Products** | `GET /api/products` | search, category, isActive, minPrice, maxPrice |
| **Inventory** | `GET /api/inventory` | search, storeId, lowStock, outOfStock |
| **Tasks** | `GET /api/tasks` | status, priority, assignedTo, dueDate |
| **Users** | `GET /api/users` | search, role, isActive, storeId |
| **Customers** | `GET /api/crm/customers` | search, status, minSpent, maxSpent |
| **Interactions** | `GET /api/crm/interactions` | customerId, type, status, createdBy |
| **Suppliers** | `GET /api/lrm/suppliers` | search, status, productId |
| **Stock Transfers** | `GET /api/lrm/stock-transfers` | status, fromWarehouse, toWarehouse |
| **Employees** | `GET /api/hrm/employees` | search, department, position, status |
| **Leave Requests** | `GET /api/hrm/leave-requests` | employeeId, type, status, startDate |
| **Work Schedules** | `GET /api/hrm/work-schedules` | employeeId, date, type, status |
| **Notifications** | `GET /api/notifications` | userId, type, isRead |
| **Shifts** | `GET /api/shifts` | staffId, storeId, status, startDate |
| **Audit Logs** | `GET /api/admin/audit-logs` | userId, action, module, startDate |

---

## 3. Code Mẫu C#

### 3.1 DTO Classes

```csharp
public class PaginationParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string SortOrder { get; set; } = "desc";
}

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; }
    public PaginationMeta Meta { get; set; }
}

public class PaginationMeta
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
```

### 3.2 Extension Method

```csharp
public static async Task<PaginatedResponse<T>> ToPaginatedResponseAsync<T>(
    this IQueryable<T> query,
    PaginationParameters parameters)
{
    var totalItems = await query.CountAsync();
    var totalPages = (int)Math.Ceiling(totalItems / (double)parameters.PageSize);
    
    var items = await query
        .Skip((parameters.Page - 1) * parameters.PageSize)
        .Take(parameters.PageSize)
        .ToListAsync();
    
    return new PaginatedResponse<T>
    {
        Data = items,
        Meta = new PaginationMeta
        {
            CurrentPage = parameters.Page,
            PageSize = parameters.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            HasNextPage = parameters.Page < totalPages,
            HasPreviousPage = parameters.Page > 1
        }
    };
}
```

### 3.3 Controller Example - Orders

```csharp
[HttpGet]
public async Task<ActionResult<PaginatedResponse<OrderDto>>> GetOrders(
    [FromQuery] PaginationParameters pagination,
    [FromQuery] string? status = null,
    [FromQuery] string? search = null,
    [FromQuery] string? staffId = null,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)
{
    try
    {
        // 1. Bắt đầu với base query
        var query = _context.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .AsQueryable();
        
        // 2. Apply Role-Based Filtering
        query = ApplyRoleBasedFiltering(query, User);
        
        // 3. Apply Filters
        if (!string.IsNullOrEmpty(status) && status != "all")
        {
            query = query.Where(o => o.Status == status);
        }
        
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(o => 
                o.OrderNumber.Contains(search) ||
                o.CustomerName.Contains(search) ||
                o.CustomerPhone.Contains(search));
        }
        
        if (!string.IsNullOrEmpty(staffId))
        {
            query = query.Where(o => o.StaffId == staffId);
        }
        
        if (startDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= startDate.Value);
        }
        
        if (endDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= endDate.Value);
        }
        
        // 4. Apply Sorting
        query = ApplySorting(query, pagination.SortBy, pagination.SortOrder);
        
        // 5. Execute Pagination
        var result = await query.ToPaginatedResponseAsync(pagination);
        
        // 6. Map to DTOs
        var mappedData = _mapper.Map<List<OrderDto>>(result.Data);
        
        return Ok(new PaginatedResponse<OrderDto>
        {
            Data = mappedData,
            Meta = result.Meta
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching orders");
        return StatusCode(500, "Error occurred");
    }
}

private IQueryable<Order> ApplySorting(
    IQueryable<Order> query, 
    string sortBy, 
    string sortOrder)
{
    if (string.IsNullOrEmpty(sortBy))
    {
        return query.OrderByDescending(o => o.CreatedAt);
    }
    
    return sortBy.ToLower() switch
    {
        "ordernumber" => sortOrder == "asc" 
            ? query.OrderBy(o => o.OrderNumber)
            : query.OrderByDescending(o => o.OrderNumber),
        "total" => sortOrder == "asc"
            ? query.OrderBy(o => o.Total)
            : query.OrderByDescending(o => o.Total),
        "createdat" => sortOrder == "asc"
            ? query.OrderBy(o => o.CreatedAt)
            : query.OrderByDescending(o => o.CreatedAt),
        _ => query.OrderByDescending(o => o.CreatedAt)
    };
}

private IQueryable<Order> ApplyRoleBasedFiltering(
    IQueryable<Order> query, 
    ClaimsPrincipal user)
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
    
    // Admin thấy tất cả
    if (userRole == "admin")
        return query;
    
    // Manager thấy tất cả trong store của mình
    if (userRole == "manager")
    {
        var storeId = user.FindFirst("StoreId")?.Value;
        return query.Where(o => o.StoreId == storeId);
    }
    
    // Staff chỉ thấy order của mình
    return query.Where(o => o.StaffId == userId);
}
```

---

## 4. Lưu Ý Quan Trọng

### Performance
1. **Tạo Index** cho các field hay filter/sort:
```sql
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt DESC);
CREATE INDEX IX_Orders_StaffId_Status ON Orders(StaffId, Status);
```

2. **Sử dụng AsNoTracking()** cho read-only queries
3. **Chỉ Include** các related entities thực sự cần
4. **Validate** PageSize (max 100 để tránh abuse)

### Security
1. **Luôn apply Role-Based Filtering** trước khi return data
2. **Validate** tất cả input parameters
3. **Sanitize** search terms để tránh SQL injection

### Error Handling
```csharp
// Nếu page number invalid, return page 1
if (pagination.Page < 1) pagination.Page = 1;

// Nếu pageSize invalid, use default
if (pagination.PageSize < 1 || pagination.PageSize > 100)
    pagination.PageSize = 10;
```

---

## 5. Testing Checklist

- [ ] Pagination hoạt động đúng (page 1, 2, 3...)
- [ ] Search filter hoạt động
- [ ] Status filter hoạt động  
- [ ] Sorting ASC/DESC hoạt động
- [ ] Role-based filtering đúng (Staff chỉ thấy data của mình)
- [ ] Admin thấy tất cả data
- [ ] Total count đúng sau khi filter
- [ ] HasNextPage/HasPreviousPage chính xác
- [ ] Performance OK với large dataset
- [ ] Error handling khi invalid parameters

---

## 6. Ví Dụ Test Request

### Postman/Curl Example
```bash
# Get page 2, 20 items, pending orders only
curl -X GET "http://localhost:5000/api/orders?page=2&pageSize=20&filter[status]=pending&sortBy=createdAt&sortOrder=desc" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### Expected Response
```json
{
  "data": [
    {
      "id": "order-123",
      "orderNumber": "ORD-2024-001",
      "customerName": "John Doe",
      "status": "pending",
      "total": 125.50,
      "createdAt": "2024-11-12T10:30:00Z"
    }
  ],
  "meta": {
    "currentPage": 2,
    "pageSize": 20,
    "totalItems": 85,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": true
  }
}
```

---

## 7. Frontend Integration

Frontend đã có sẵn:
- Hook `usePaginatedApi` tự động gọi API với pagination
- Component `PaginationCustom` hiển thị pagination UI
- Loading states và error handling

Backend chỉ cần:
- Return đúng format response
- Ensure CORS headers
- UTF-8 encoding
- ISO 8601 date formats

---

## 8. Rollout Plan

### Week 1: Core Modules
- Orders ✅
- Products
- Inventory

### Week 2: Management  
- Tasks
- Users
- Notifications

### Week 3: CRM/LRM/HRM
- Customers
- Suppliers
- Employees
- Leave Requests

### Week 4: Admin
- Audit Logs
- System Monitoring

---

## 9. Support

Xem chi tiết đầy đủ trong:
- `/docs/07_server_side_pagination_implementation.md` (English, full details)
- `/services/api.ts` (Frontend mock implementation)

Contact: Frontend Team

---

**Version**: 1.0  
**Updated**: November 12, 2025

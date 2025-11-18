# Server-Side Pagination Implementation Guide

## Overview

This document provides comprehensive guidance for implementing server-side pagination across all modules in the POS system. The frontend is already configured to consume paginated API endpoints.

---

## 1. API Endpoint Pattern

All list/collection endpoints should follow this standardized pattern:

### Request Format
```
GET /api/{module}?page={page}&pageSize={pageSize}&sortBy={field}&sortOrder={asc|desc}&filter[key]={value}
```

### Query Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `page` | integer | No | 1 | Current page number (1-indexed) |
| `pageSize` | integer | No | 10 | Number of items per page (max: 100) |
| `sortBy` | string | No | - | Field name to sort by |
| `sortOrder` | string | No | 'desc' | Sort direction: 'asc' or 'desc' |
| `filter[{key}]` | mixed | No | - | Dynamic filters (see Filter section) |

### Response Format

All paginated endpoints must return this structure:

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

#### Response Fields

**`data`**: Array of items for current page

**`meta`**: Pagination metadata object containing:
- `currentPage`: Current page number (1-indexed)
- `pageSize`: Items per page
- `totalItems`: Total count of items matching filters
- `totalPages`: Total number of pages (`Math.Ceiling(totalItems / pageSize)`)
- `hasNextPage`: Boolean indicating if next page exists
- `hasPreviousPage`: Boolean indicating if previous page exists

---

## 2. Module-Specific Endpoints

### 2.1 Orders Module

#### Endpoint
```
GET /api/orders
```

#### Supported Filters
- `filter[status]` - Order status (pending, processing, completed, cancelled, refunded)
- `filter[search]` - Search in orderNumber, customerName, customerPhone
- `filter[staffId]` - Filter by staff member (for role-based access)
- `filter[customerId]` - Filter by customer
- `filter[paymentMethod]` - Payment method (cash, card, digital)
- `filter[paymentStatus]` - Payment status (paid, pending, refunded)
- `filter[startDate]` - Orders created after this date (ISO 8601)
- `filter[endDate]` - Orders created before this date (ISO 8601)

#### Supported Sort Fields
- `createdAt` (default)
- `updatedAt`
- `orderNumber`
- `total`
- `status`

#### Example Request
```
GET /api/orders?page=2&pageSize=20&sortBy=createdAt&sortOrder=desc&filter[status]=pending&filter[search]=john
```

#### Example Response
```json
{
  "data": [
    {
      "id": "order-123",
      "orderNumber": "ORD-2024-001",
      "customerId": "customer-1",
      "customerName": "John Doe",
      "customerPhone": "+1234567890",
      "storeId": "store-1",
      "staffId": "user-1",
      "status": "pending",
      "items": [...],
      "subtotal": 25.98,
      "tax": 2.60,
      "discount": 0,
      "tip": 3.00,
      "total": 31.58,
      "paymentMethod": "card",
      "paymentStatus": "pending",
      "notes": "Customer requested gift wrapping",
      "createdAt": "2024-01-20T09:30:00Z",
      "updatedAt": "2024-01-20T09:45:00Z"
    }
  ],
  "meta": {
    "currentPage": 2,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8,
    "hasNextPage": true,
    "hasPreviousPage": true
  }
}
```

---

### 2.2 Products Module

#### Endpoint
```
GET /api/products
```

#### Supported Filters
- `filter[search]` - Search in name, description, barcode
- `filter[category]` - Product category
- `filter[isActive]` - Active status (true/false)
- `filter[minPrice]` - Minimum price
- `filter[maxPrice]` - Maximum price

#### Supported Sort Fields
- `name`
- `price`
- `category`
- `createdAt`

---

### 2.3 Inventory Module

#### Endpoint
```
GET /api/inventory
```

#### Supported Filters
- `filter[search]` - Search product name/ID
- `filter[storeId]` - Filter by store
- `filter[lowStock]` - Only show items below minStock (true/false)
- `filter[outOfStock]` - Only show items with quantity = 0 (true/false)

#### Supported Sort Fields
- `quantity`
- `lastUpdated`
- `productName`

---

### 2.4 Tasks Module

#### Endpoint
```
GET /api/tasks
```

#### Supported Filters
- `filter[status]` - Task status (pending, in_progress, completed)
- `filter[priority]` - Priority (low, medium, high)
- `filter[assignedTo]` - Assigned staff ID
- `filter[assignedBy]` - Creator ID
- `filter[dueDate]` - Filter by due date

#### Supported Sort Fields
- `createdAt`
- `dueDate`
- `priority`
- `status`

---

### 2.5 Users/Staff Module

#### Endpoint
```
GET /api/users
```

#### Supported Filters
- `filter[search]` - Search in name, email
- `filter[role]` - User role (staff, manager, admin)
- `filter[isActive]` - Active status (true/false)
- `filter[storeId]` - Filter by store
- `filter[managerId]` - Filter by manager

#### Supported Sort Fields
- `name`
- `email`
- `createdAt`
- `lastLogin`

---

### 2.6 CRM - Customers Module

#### Endpoint
```
GET /api/crm/customers
```

#### Supported Filters
- `filter[search]` - Search in name, email, phone, company
- `filter[status]` - Customer status (lead, customer, inactive)
- `filter[minSpent]` - Minimum total spent
- `filter[maxSpent]` - Maximum total spent

#### Supported Sort Fields
- `name`
- `totalSpent`
- `totalOrders`
- `lastOrderDate`
- `createdAt`

---

### 2.7 CRM - Interactions Module

#### Endpoint
```
GET /api/crm/interactions
```

#### Supported Filters
- `filter[customerId]` - Filter by customer
- `filter[type]` - Interaction type (call, email, meeting, note)
- `filter[status]` - Status (pending, completed)
- `filter[createdBy]` - Creator user ID

#### Supported Sort Fields
- `createdAt`
- `followUpDate`
- `type`

---

### 2.8 LRM - Suppliers Module

#### Endpoint
```
GET /api/lrm/suppliers
```

#### Supported Filters
- `filter[search]` - Search in name, email, phone
- `filter[status]` - Supplier status (active, inactive)
- `filter[productId]` - Suppliers for specific product

#### Supported Sort Fields
- `name`
- `createdAt`
- `status`

---

### 2.9 LRM - Stock Transfers Module

#### Endpoint
```
GET /api/lrm/stock-transfers
```

#### Supported Filters
- `filter[status]` - Transfer status (pending, in_transit, delivered, cancelled)
- `filter[fromWarehouse]` - Source warehouse
- `filter[toWarehouse]` - Destination warehouse
- `filter[requestedBy]` - Requester user ID

#### Supported Sort Fields
- `createdAt`
- `deliveryDate`
- `status`

---

### 2.10 HRM - Employees Module

#### Endpoint
```
GET /api/hrm/employees
```

#### Supported Filters
- `filter[search]` - Search in name, email, phone
- `filter[department]` - Department
- `filter[position]` - Position/title
- `filter[status]` - Employment status (active, inactive, on_leave)
- `filter[managerId]` - Filter by manager

#### Supported Sort Fields
- `name`
- `hireDate`
- `department`
- `salary`

---

### 2.11 HRM - Leave Requests Module

#### Endpoint
```
GET /api/hrm/leave-requests
```

#### Supported Filters
- `filter[employeeId]` - Filter by employee
- `filter[type]` - Leave type (vacation, sick, personal, emergency)
- `filter[status]` - Status (pending, approved, rejected)
- `filter[startDate]` - Start date range
- `filter[endDate]` - End date range

#### Supported Sort Fields
- `submittedAt`
- `startDate`
- `status`

---

### 2.12 HRM - Work Schedules Module

#### Endpoint
```
GET /api/hrm/work-schedules
```

#### Supported Filters
- `filter[employeeId]` - Filter by employee
- `filter[date]` - Specific date
- `filter[startDate]` - Date range start
- `filter[endDate]` - Date range end
- `filter[type]` - Schedule type (regular, overtime, holiday)
- `filter[status]` - Status (scheduled, confirmed, completed, missed)

#### Supported Sort Fields
- `date`
- `startTime`
- `type`

---

### 2.13 Notifications Module

#### Endpoint
```
GET /api/notifications
```

#### Supported Filters
- `filter[userId]` - User ID (required for non-admin users)
- `filter[type]` - Notification type (order, inventory, staff, system)
- `filter[isRead]` - Read status (true/false)

#### Supported Sort Fields
- `createdAt` (default)

---

### 2.14 Shifts Module

#### Endpoint
```
GET /api/shifts
```

#### Supported Filters
- `filter[staffId]` - Filter by staff member
- `filter[storeId]` - Filter by store
- `filter[status]` - Shift status (open, closed)
- `filter[startDate]` - Shifts started after this date
- `filter[endDate]` - Shifts started before this date

#### Supported Sort Fields
- `startTime`
- `endTime`
- `totalSales`

---

### 2.15 Admin - Audit Logs Module

#### Endpoint
```
GET /api/admin/audit-logs
```

#### Supported Filters
- `filter[userId]` - Filter by user
- `filter[action]` - Action type (CREATE, UPDATE, DELETE, VIEW, LOGIN, LOGOUT)
- `filter[module]` - Module name (Orders, Products, Inventory, etc.)
- `filter[startDate]` - Logs after this date
- `filter[endDate]` - Logs before this date

#### Supported Sort Fields
- `timestamp` (default)
- `action`
- `module`

---

### 2.16 Admin - User Management Module

Same as Users/Staff Module (Section 2.5) but accessible only to admins.

---

## 3. Implementation Guidelines (C# .NET)

### 3.1 Create PaginationParameters DTO

```csharp
public class PaginationParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string SortOrder { get; set; } = "desc";
    
    // Max page size to prevent abuse
    private const int MaxPageSize = 100;
    private int _pageSize = 10;
    
    public int GetPageSize()
    {
        return (_pageSize > MaxPageSize) ? MaxPageSize : _pageSize;
    }
}
```

### 3.2 Create PaginatedResponse<T> Generic Class

```csharp
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

### 3.3 Create Pagination Extension Method

```csharp
public static class QueryableExtensions
{
    public static async Task<PaginatedResponse<T>> ToPaginatedResponseAsync<T>(
        this IQueryable<T> query,
        PaginationParameters parameters)
    {
        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)parameters.GetPageSize());
        
        var items = await query
            .Skip((parameters.Page - 1) * parameters.GetPageSize())
            .Take(parameters.GetPageSize())
            .ToListAsync();
        
        return new PaginatedResponse<T>
        {
            Data = items,
            Meta = new PaginationMeta
            {
                CurrentPage = parameters.Page,
                PageSize = parameters.GetPageSize(),
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasNextPage = parameters.Page < totalPages,
                HasPreviousPage = parameters.Page > 1
            }
        };
    }
}
```

### 3.4 Example Controller Implementation

```csharp
[HttpGet]
public async Task<ActionResult<PaginatedResponse<OrderDto>>> GetOrders(
    [FromQuery] PaginationParameters pagination,
    [FromQuery] string? status = null,
    [FromQuery] string? search = null,
    [FromQuery] string? staffId = null)
{
    try
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .AsQueryable();
        
        // Apply filters
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
        
        // Apply sorting
        if (!string.IsNullOrEmpty(pagination.SortBy))
        {
            query = ApplySorting(query, pagination.SortBy, pagination.SortOrder);
        }
        else
        {
            // Default sorting
            query = query.OrderByDescending(o => o.CreatedAt);
        }
        
        // Execute pagination
        var result = await query.ToPaginatedResponseAsync(pagination);
        
        // Map to DTOs if needed
        var mappedData = _mapper.Map<List<OrderDto>>(result.Data);
        
        return Ok(new PaginatedResponse<OrderDto>
        {
            Data = mappedData,
            Meta = result.Meta
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching paginated orders");
        return StatusCode(500, "An error occurred while fetching orders");
    }
}

private IQueryable<Order> ApplySorting(IQueryable<Order> query, string sortBy, string sortOrder)
{
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
```

### 3.5 Advanced Filtering with Dynamic Filters

```csharp
public class DynamicFilterExtensions
{
    public static IQueryable<T> ApplyFilters<T>(
        this IQueryable<T> query,
        Dictionary<string, string> filters)
    {
        foreach (var filter in filters)
        {
            var propertyName = filter.Key;
            var propertyValue = filter.Value;
            
            if (string.IsNullOrEmpty(propertyValue) || propertyValue == "all")
                continue;
            
            var property = typeof(T).GetProperty(propertyName, 
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            
            if (property != null)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.Property(parameter, property);
                var constant = Expression.Constant(propertyValue);
                
                Expression comparison;
                
                if (property.PropertyType == typeof(string))
                {
                    // String contains (case-insensitive)
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var lowerProperty = Expression.Call(propertyAccess, toLowerMethod);
                    var lowerConstant = Expression.Constant(propertyValue.ToLower());
                    comparison = Expression.Call(lowerProperty, containsMethod, lowerConstant);
                }
                else
                {
                    // Exact match
                    comparison = Expression.Equal(propertyAccess, constant);
                }
                
                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                query = query.Where(lambda);
            }
        }
        
        return query;
    }
}
```

---

## 4. Performance Considerations

### 4.1 Database Indexing

Ensure proper indexes on commonly filtered/sorted fields:

```sql
-- Orders
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt DESC);
CREATE INDEX IX_Orders_StaffId ON Orders(StaffId);
CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);

-- Products
CREATE INDEX IX_Products_Category ON Products(Category);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);

-- Users
CREATE INDEX IX_Users_Role ON Users(Role);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);

-- Composite indexes for common filter combinations
CREATE INDEX IX_Orders_Status_CreatedAt ON Orders(Status, CreatedAt DESC);
```

### 4.2 Query Optimization

- Use `AsNoTracking()` for read-only queries
- Only `Include()` necessary related entities
- Use projection (Select) instead of loading full entities when possible

```csharp
var query = _context.Orders
    .AsNoTracking()
    .Include(o => o.Items)
    .Where(/* filters */)
    .Select(o => new OrderDto
    {
        // Only select needed fields
        Id = o.Id,
        OrderNumber = o.OrderNumber,
        // ... other fields
    });
```

### 4.3 Caching Strategy

Consider caching for frequently accessed, slowly changing data:

```csharp
public async Task<PaginatedResponse<ProductDto>> GetProducts(PaginationParameters pagination)
{
    var cacheKey = $"products_page_{pagination.Page}_size_{pagination.PageSize}";
    
    if (!_cache.TryGetValue(cacheKey, out PaginatedResponse<ProductDto> result))
    {
        result = await FetchProductsFromDatabase(pagination);
        
        _cache.Set(cacheKey, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });
    }
    
    return result;
}
```

---

## 5. Security & Authorization

### 5.1 Role-Based Filtering

Ensure users can only access data they're authorized to see:

```csharp
private IQueryable<Order> ApplyRoleBasedFiltering(IQueryable<Order> query, ClaimsPrincipal user)
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
    
    // Admin can see all
    if (userRole == "admin")
        return query;
    
    // Manager can see all in their store
    if (userRole == "manager")
    {
        var userStoreId = user.FindFirst("StoreId")?.Value;
        return query.Where(o => o.StoreId == userStoreId);
    }
    
    // Staff can only see their own orders
    return query.Where(o => o.StaffId == userId);
}
```

### 5.2 Input Validation

Always validate and sanitize user inputs:

```csharp
public class PaginationParametersValidator : AbstractValidator<PaginationParameters>
{
    public PaginationParametersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
        
        RuleFor(x => x.SortOrder)
            .Must(x => x == "asc" || x == "desc")
            .When(x => !string.IsNullOrEmpty(x.SortOrder))
            .WithMessage("Sort order must be 'asc' or 'desc'");
    }
}
```

---

## 6. Error Handling

### 6.1 Standard Error Response

```csharp
public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public string Message { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}
```

### 6.2 Common Error Scenarios

- **Invalid Page Number**: Return first page
- **Invalid PageSize**: Use default (10) or max (100)
- **Invalid Sort Field**: Use default sorting
- **Database Error**: Return 500 with generic error message

```csharp
try
{
    // ... pagination logic
}
catch (ArgumentException ex)
{
    return BadRequest(new ErrorResponse
    {
        Message = "Invalid pagination parameters",
        Errors = new List<string> { ex.Message }
    });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error in pagination");
    return StatusCode(500, new ErrorResponse
    {
        Message = "An error occurred while fetching data"
    });
}
```

---

## 7. Testing

### 7.1 Unit Tests Example

```csharp
[Fact]
public async Task GetOrders_WithPagination_ReturnsCorrectPage()
{
    // Arrange
    var pagination = new PaginationParameters { Page = 2, PageSize = 5 };
    
    // Act
    var result = await _controller.GetOrders(pagination);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var response = Assert.IsType<PaginatedResponse<OrderDto>>(okResult.Value);
    
    Assert.Equal(2, response.Meta.CurrentPage);
    Assert.Equal(5, response.Meta.PageSize);
    Assert.True(response.Data.Count <= 5);
}
```

---

## 8. Frontend Integration Notes

The frontend uses the `usePaginatedApi` hook which automatically:
- Manages pagination state (page, pageSize)
- Handles loading states
- Manages filters and sorting
- Provides refetch functionality

Backend must ensure:
- Consistent response format across all endpoints
- Proper CORS headers
- UTF-8 encoding for international characters
- ISO 8601 date formats in responses

---

## 9. Migration Path

### Phase 1: Core Modules (Week 1)
- Orders
- Products
- Inventory

### Phase 2: Management Modules (Week 2)
- Tasks
- Users/Staff
- Notifications

### Phase 3: Extended Modules (Week 3)
- CRM (Customers, Interactions)
- LRM (Suppliers, Stock Transfers)
- HRM (Employees, Leave Requests, Schedules)

### Phase 4: Admin Modules (Week 4)
- Audit Logs
- User Management
- System Monitoring

---

## 10. Changelog & Versioning

All pagination endpoints should include API version in header:

```
X-API-Version: 1.0
```

Future breaking changes should increment version and maintain backward compatibility for at least 6 months.

---

## Appendix A: Complete Example - Orders Module

See `/services/api.ts` in frontend codebase for reference implementation of mock API that demonstrates the expected behavior.

## Appendix B: Postman Collection

A Postman collection with example requests for all endpoints will be provided separately.

---

**Document Version**: 1.0  
**Last Updated**: November 12, 2025  
**Maintained By**: Frontend Development Team  
**Contact**: [Your contact information]

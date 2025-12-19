# Backend Specification: Services-Product Unification

## 📋 Tổng Quan

Backend cần mở rộng **2 models** để hỗ trợ Services:

1. **Product Model** - Thêm type discriminator để phân biệt products và services
2. **Order Model** - Thêm type discriminator và service-specific fields

---

## Part 1: Product Model

### 1.1 Database Schema Changes

#### Products Table

```sql
ALTER TABLE Products ADD COLUMN Type NVARCHAR(50) DEFAULT 'product';
ALTER TABLE Products ADD COLUMN ServiceCategory NVARCHAR(100) NULL;
ALTER TABLE Products ADD COLUMN ServiceDuration NVARCHAR(100) NULL;
ALTER TABLE Products ADD COLUMN Rating DECIMAL(3,2) NULL;

-- Set Type='product' for all existing products
UPDATE Products SET Type = 'product' WHERE Type IS NULL;

-- Create index for filtering
CREATE INDEX IX_Products_Type ON Products(Type);
CREATE INDEX IX_Products_ServiceCategory ON Products(ServiceCategory);
```

### 1.2 C# Models

#### ProductListDto

```csharp
public class ProductListDto
{
    // Existing fields
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? PrimaryImage { get; set; }
    public string? CategoryName { get; set; }
    public int StockQuantity { get; set; }
    public bool InStock { get; set; }
    
    // NEW: Type discriminator
    public string Type { get; set; } = "product"; // "product" | "service"
    
    // NEW: Service-specific fields
    public string? ServiceCategory { get; set; }
    public string? ServiceDuration { get; set; }
    public double? Rating { get; set; }
}
```

#### ProductFilterDto

```csharp
public class ProductFilterDto
{
    // Existing fields
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; } = false;
    
    // NEW: Type filter
    public string? Type { get; set; } // "product" | "service" | null (all)
    
    // NEW: Service category filter
    public string? ServiceCategory { get; set; }
}
```

### 1.3 API Endpoints

#### GET /api/v1/Product

**Enhanced with type filtering**:

```csharp
[HttpGet]
public async Task<ActionResult<PaginatedResult<ProductListDto>>> GetProducts(
    [FromQuery] ProductFilterDto filter)
{
    var query = _context.Products.AsQueryable();
    
    // NEW: Filter by type
    if (!string.IsNullOrEmpty(filter.Type))
    {
        query = query.Where(p => p.Type == filter.Type);
    }
    
    // NEW: Filter by service category
    if (!string.IsNullOrEmpty(filter.ServiceCategory))
    {
        query = query.Where(p => p.ServiceCategory == filter.ServiceCategory);
    }
    
    // Existing filters (search, category, etc.)
    if (!string.IsNullOrEmpty(filter.SearchTerm))
    {
        query = query.Where(p => p.Name.Contains(filter.SearchTerm) 
                              || p.Description.Contains(filter.SearchTerm));
    }
    
    // Pagination
    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToListAsync();
    
    return Ok(new PaginatedResult<ProductListDto>
    {
        Data = items,
        TotalCount = totalCount,
        PageNumber = filter.PageNumber,
        PageSize = filter.PageSize
    });
}
```

### 1.4 Validation

```csharp
public class ProductFilterDtoValidator : AbstractValidator<ProductFilterDto>
{
    public ProductFilterDtoValidator()
    {
        RuleFor(x => x.Type)
            .Must(type => type == null || type == "product" || type == "service")
            .WithMessage("Type must be 'product' or 'service'");
            
        RuleFor(x => x.ServiceCategory)
            .Must(BeValidServiceCategory)
            .When(x => !string.IsNullOrEmpty(x.ServiceCategory))
            .WithMessage("Invalid service category");
    }
    
    private bool BeValidServiceCategory(string category)
    {
        var validCategories = new[] {
            "ancestor-worship",
            "opening-ceremony",
            "wedding",
            "buddha-worship",
            "new-house",
            "feng-shui-consultation"
        };
        return validCategories.Contains(category);
    }
}
```

---

## Part 2: Order Model

### 2.1 Database Schema Changes

#### Orders Table

```sql
ALTER TABLE Orders ADD COLUMN Type NVARCHAR(50) DEFAULT 'product';
ALTER TABLE Orders ADD COLUMN ServiceCategory NVARCHAR(100) NULL;
ALTER TABLE Orders ADD COLUMN ServiceDuration NVARCHAR(100) NULL;
ALTER TABLE Orders ADD COLUMN ServiceLocation NVARCHAR(500) NULL;
ALTER TABLE Orders ADD COLUMN ServiceDate DATETIME NULL;
ALTER TABLE Orders ADD COLUMN ServiceTime NVARCHAR(50) NULL;
ALTER TABLE Orders ADD COLUMN ServiceNotes NVARCHAR(1000) NULL;

-- Set Type='product' for all existing orders
UPDATE Orders SET Type = 'product' WHERE Type IS NULL;

-- Create index for filtering
CREATE INDEX IX_Orders_Type ON Orders(Type);
CREATE INDEX IX_Orders_ServiceCategory ON Orders(ServiceCategory);
```

#### OrderItems Table

```sql
ALTER TABLE OrderItems ADD COLUMN Type NVARCHAR(50) DEFAULT 'product';
ALTER TABLE OrderItems ADD COLUMN ServiceCategory NVARCHAR(100) NULL;
ALTER TABLE OrderItems ADD COLUMN ServiceDuration NVARCHAR(100) NULL;

-- Set Type='product' for all existing order items
UPDATE OrderItems SET Type = 'product' WHERE Type IS NULL;

-- Create index
CREATE INDEX IX_OrderItems_Type ON OrderItems(Type);
```

### 2.2 C# Models

#### OrderDetailDto

```csharp
public class OrderDetailDto
{
    // Existing fields
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemDTO> Items { get; set; }
    public string? ShippingAddress { get; set; }
    public string? ShippingMethod { get; set; }
    
    // NEW: Type discriminator
    public string Type { get; set; } = "product"; // "product" | "service"
    
    // NEW: Service-specific fields
    public string? ServiceCategory { get; set; }
    public string? ServiceDuration { get; set; }
    public string? ServiceLocation { get; set; }
    public DateTime? ServiceDate { get; set; }
    public string? ServiceTime { get; set; }
    public string? ServiceNotes { get; set; }
}
```

#### OrderItemDTO

```csharp
public class OrderItemDTO
{
    // Existing fields
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? Image { get; set; }
    
    // NEW: Type field
    public string Type { get; set; } = "product";
    
    // NEW: Service-specific fields
    public string? ServiceCategory { get; set; }
    public string? ServiceDuration { get; set; }
}
```

#### OrderFilterDto

```csharp
public class OrderFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    // NEW: Type filter
    public string? Type { get; set; } // "product" | "service" | null (all)
    
    // NEW: Service category filter
    public string? ServiceCategory { get; set; }
    
    // Status filter
    public string? Status { get; set; }
    
    // Date range
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Search
    public string? SearchTerm { get; set; }
    
    // Sorting
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; } = true;
}
```

### 2.3 API Endpoints

#### GET /api/v1/Order/my-orders

**Enhanced with type filtering**:

```csharp
[HttpGet("my-orders")]
[Authorize]
public async Task<ActionResult<PaginatedResult<OrderDetailDto>>> GetMyOrders(
    [FromQuery] OrderFilterDto filter)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    var query = _context.Orders
        .Where(o => o.UserId == userId)
        .AsQueryable();
    
    // NEW: Filter by type
    if (!string.IsNullOrEmpty(filter.Type))
    {
        query = query.Where(o => o.Type == filter.Type);
    }
    
    // NEW: Filter by service category
    if (!string.IsNullOrEmpty(filter.ServiceCategory))
    {
        query = query.Where(o => o.ServiceCategory == filter.ServiceCategory);
    }
    
    // Filter by status
    if (!string.IsNullOrEmpty(filter.Status))
    {
        query = query.Where(o => o.Status == filter.Status);
    }
    
    // Filter by date range
    if (filter.StartDate.HasValue)
    {
        query = query.Where(o => o.OrderDate >= filter.StartDate.Value);
    }
    if (filter.EndDate.HasValue)
    {
        query = query.Where(o => o.OrderDate <= filter.EndDate.Value);
    }
    
    // Search
    if (!string.IsNullOrEmpty(filter.SearchTerm))
    {
        query = query.Where(o => o.Id.ToString().Contains(filter.SearchTerm));
    }
    
    // Sorting
    query = filter.SortBy?.ToLower() switch
    {
        "date" => filter.IsDescending 
            ? query.OrderByDescending(o => o.OrderDate)
            : query.OrderBy(o => o.OrderDate),
        "total" => filter.IsDescending
            ? query.OrderByDescending(o => o.Total)
            : query.OrderBy(o => o.Total),
        _ => query.OrderByDescending(o => o.OrderDate)
    };
    
    // Pagination
    var totalCount = await query.CountAsync();
    var items = await query
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .Include(o => o.Items)
        .ToListAsync();
    
    return Ok(new PaginatedResult<OrderDetailDto>
    {
        Data = _mapper.Map<List<OrderDetailDto>>(items),
        TotalCount = totalCount,
        PageNumber = filter.PageNumber,
        PageSize = filter.PageSize,
        TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
    });
}
```

### 2.4 Validation

```csharp
public class OrderFilterDtoValidator : AbstractValidator<OrderFilterDto>
{
    public OrderFilterDtoValidator()
    {
        RuleFor(x => x.Type)
            .Must(type => type == null || type == "product" || type == "service")
            .WithMessage("Type must be 'product' or 'service'");
            
        RuleFor(x => x.ServiceCategory)
            .Must(BeValidServiceCategory)
            .When(x => !string.IsNullOrEmpty(x.ServiceCategory))
            .WithMessage("Invalid service category");
            
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");
            
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100");
    }
    
    private bool BeValidServiceCategory(string category)
    {
        var validCategories = new[] {
            "ancestor-worship",
            "opening-ceremony",
            "wedding",
            "buddha-worship",
            "new-house",
            "feng-shui-consultation"
        };
        return validCategories.Contains(category);
    }
}
```

---

## Testing

### Unit Tests

```csharp
[Fact]
public async Task GetProducts_WithTypeService_ReturnsOnlyServices()
{
    // Arrange
    var filter = new ProductFilterDto { Type = "service" };
    
    // Act
    var result = await _controller.GetProducts(filter);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var products = Assert.IsType<PaginatedResult<ProductListDto>>(okResult.Value);
    Assert.All(products.Data, p => Assert.Equal("service", p.Type));
}

[Fact]
public async Task GetMyOrders_WithTypeService_ReturnsOnlyServiceOrders()
{
    // Arrange
    var filter = new OrderFilterDto { Type = "service" };
    
    // Act
    var result = await _controller.GetMyOrders(filter);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var orders = Assert.IsType<PaginatedResult<OrderDetailDto>>(okResult.Value);
    Assert.All(orders.Data, o => Assert.Equal("service", o.Type));
}
```

---

## Swagger Documentation

Update Swagger to document new query parameters:

```csharp
/// <summary>
/// Get products with optional filtering
/// </summary>
/// <param name="filter">Filter parameters</param>
/// <returns>Paginated list of products</returns>
/// <response code="200">Returns the paginated list of products</response>
/// <response code="400">If the filter parameters are invalid</response>
[HttpGet]
[ProducesResponseType(typeof(PaginatedResult<ProductListDto>), 200)]
[ProducesResponseType(400)]
public async Task<ActionResult<PaginatedResult<ProductListDto>>> GetProducts(
    [FromQuery] ProductFilterDto filter)
{
    // Implementation
}
```

---

## Checklist

### Product Model
- [ ] Add Type column to Products table
- [ ] Add ServiceCategory column to Products table
- [ ] Add ServiceDuration column to Products table
- [ ] Add Rating column to Products table
- [ ] Create indexes
- [ ] Update ProductListDto
- [ ] Update ProductFilterDto
- [ ] Modify GET /api/v1/Product endpoint
- [ ] Add validation
- [ ] Write unit tests
- [ ] Update Swagger docs

### Order Model
- [ ] Add Type column to Orders table
- [ ] Add service-specific columns to Orders table
- [ ] Add Type column to OrderItems table
- [ ] Add service-specific columns to OrderItems table
- [ ] Create indexes
- [ ] Update OrderDetailDto
- [ ] Update OrderItemDTO
- [ ] Create OrderFilterDto
- [ ] Modify GET /api/v1/Order/my-orders endpoint
- [ ] Add validation
- [ ] Write unit tests
- [ ] Update Swagger docs


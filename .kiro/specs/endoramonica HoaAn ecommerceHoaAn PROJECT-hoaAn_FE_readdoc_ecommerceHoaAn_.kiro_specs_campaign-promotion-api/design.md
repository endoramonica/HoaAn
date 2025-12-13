# Campaign & Promotion Management System - Design Document

## Overview

This design document specifies the technical architecture and implementation approach for the Campaign & Promotion Management System API. The system enables administrators to create, manage, and track marketing campaigns with associated promotions and vouchers. The frontend already has UI components (AdPopup with modal/banner types) ready to consume these APIs.

### Key Design Decisions

1. **Reuse Existing Entities**: Leverage existing Campaign, Promotion, and PromotionProduct entities from the codebase
2. **Minimal New Tables**: Create only Voucher, CampaignImpression, CampaignClick, and VoucherRedemption tables
3. **Soft Delete Pattern**: Use ISoftDelete interface for logical deletion of promotions and vouchers
4. **Audit Trail**: Leverage AuditableEntity for tracking creation/modification
5. **Status-Based Lifecycle**: Use CampaignStatus enum for campaign state management
6. **JSON Storage**: Store targeting rules as JSON in Campaign entity

---

## Architecture

### Layered Architecture

```
┌─────────────────────────────────────────┐
│         API Controllers Layer            │
│  (CampaignController, PromotionController)
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│      Application Services Layer          │
│  (CampaignService, PromotionService)    │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│      Data Access Layer (Repositories)    │
│  (CampaignRepository, VoucherRepository) │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│      Database Layer (EF Core)            │
│  (AppDbContext, Migrations)              │
└─────────────────────────────────────────┘
```

### Component Interaction Flow

```
Frontend (AdPopup)
    │
    ├─→ GET /api/v1/campaigns (fetch active campaigns)
    │
    ├─→ POST /api/v1/campaigns/{id}/track-impression (when shown)
    │
    ├─→ POST /api/v1/campaigns/{id}/track-click (when clicked)
    │
    ├─→ POST /api/v1/cart/apply-voucher (apply discount)
    │
    └─→ GET /api/v1/campaigns/{id}/stats (view analytics)

Backend Services
    │
    ├─→ CampaignService (CRUD, status transitions)
    │
    ├─→ PromotionService (CRUD, discount calculations)
    │
    ├─→ VoucherService (generation, validation, redemption)
    │
    ├─→ AnalyticsService (tracking, statistics)
    │
    └─→ CartService (voucher application)
```

---

## Components and Interfaces

### 1. Campaign Management Component

**Responsibilities:**
- Create, read, update, delete campaigns
- Manage campaign status lifecycle
- Validate campaign data and business rules
- Handle campaign targeting rules

**Key Interfaces:**
```csharp
public interface ICampaignService
{
    Task<CampaignDto> CreateCampaignAsync(CreateCampaignDto dto);
    Task<PaginatedResult<CampaignDto>> GetCampaignsAsync(GetCampaignsQueryDto query);
    Task<CampaignDto> UpdateCampaignAsync(Guid id, UpdateCampaignDto dto);
    Task DeleteCampaignAsync(Guid id);
    Task<CampaignDto> ChangeCampaignStatusAsync(Guid id, CampaignStatus newStatus);
}
```

### 2. Promotion Management Component

**Responsibilities:**
- Create, read, update, delete promotions
- Link products to promotions
- Validate promotion data
- Track promotion usage

**Key Interfaces:**
```csharp
public interface IPromotionService
{
    Task<PromotionDto> CreatePromotionAsync(Guid campaignId, CreatePromotionDto dto);
    Task<List<PromotionDto>> GetPromotionsByCampaignAsync(Guid campaignId);
    Task<PromotionDto> UpdatePromotionAsync(Guid id, UpdatePromotionDto dto);
    Task DeletePromotionAsync(Guid id);
    Task LinkProductsAsync(Guid promotionId, LinkProductsDto dto);
    Task<List<ProductDto>> GetLinkedProductsAsync(Guid promotionId);
}
```

### 3. Voucher Management Component

**Responsibilities:**
- Generate unique voucher codes
- Validate voucher codes
- Apply vouchers to carts
- Track voucher usage and redemptions

**Key Interfaces:**
```csharp
public interface IVoucherService
{
    Task<GenerateVouchersResultDto> GenerateVouchersAsync(Guid promotionId, GenerateVouchersDto dto);
    Task<VoucherApplicationResultDto> ApplyVoucherAsync(string code, Guid cartId);
    Task RemoveVoucherAsync(Guid cartId);
    Task<VoucherDto> ValidateVoucherAsync(string code);
}
```

### 4. Analytics & Tracking Component

**Responsibilities:**
- Track campaign impressions
- Track campaign clicks
- Track voucher redemptions
- Generate campaign statistics

**Key Interfaces:**
```csharp
public interface IAnalyticsService
{
    Task TrackImpressionAsync(Guid campaignId, TrackImpressionDto dto);
    Task TrackClickAsync(Guid campaignId, TrackClickDto dto);
    Task<CampaignStatsDto> GetCampaignStatsAsync(Guid campaignId, GetStatsQueryDto query);
}
```

---

## Data Models

### Campaign Entity (Existing - Enhanced)

```csharp
[Table("Campaigns")]
public class Campaign : AuditableEntity
{
    public Guid StoreId { get; set; }
    [Required, MaxLength(255)]
    public string CampaignName { get; set; }
    [MaxLength(1000)]
    public string Description { get; set; }
    [Required]
    public CampaignType CampaignType { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Column(TypeName = "decimal(15,2)")]
    public decimal Budget { get; set; }
    [Column(TypeName = "decimal(15,2)")]
    public decimal ActualCost { get; set; }
    [Required]
    public CampaignStatus Status { get; set; }
    
    // NEW: Targeting rules (stored as JSON)
    [MaxLength(2000)]
    public string TargetingRules { get; set; } // JSON: { pages: [], frequency: "", delayMs: 0, autoDismissMs: 0 }
    
    // Navigation
    public virtual Store Store { get; set; }
    public virtual ICollection<Promotion> Promotions { get; set; }
    public virtual ICollection<CampaignImpression> Impressions { get; set; }
    public virtual ICollection<CampaignClick> Clicks { get; set; }
}
```

### Promotion Entity (Existing - Enhanced)

```csharp
[Table("Promotions")]
public class Promotion : AuditableEntity, ISoftDelete
{
    [Required]
    public string PromotionName { get; set; }
    [Required]
    public PromotionType PromotionType { get; set; }
    [Column(TypeName = "decimal(15,2)")]
    public decimal DiscountValue { get; set; }
    [Column(TypeName = "decimal(15,2)")]
    public decimal? MinOrderAmount { get; set; }
    [Column(TypeName = "decimal(15,2)")]
    public decimal? MaxDiscount { get; set; }
    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public PromotionStatus Status { get; set; }
    
    // Foreign Key
    public Guid CampaignId { get; set; }
    
    // Navigation
    public virtual Campaign Campaign { get; set; }
    public virtual ICollection<PromotionProduct> PromotionProducts { get; set; }
    public virtual ICollection<Voucher> Vouchers { get; set; }
    
    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
```

### PromotionProduct Entity (Existing)

```csharp
[Table("PromotionProducts")]
public class PromotionProduct : BaseEntity, ISoftDelete
{
    public Guid PromotionId { get; set; }
    public Guid ProductId { get; set; }
    [Column(TypeName = "decimal(15,2)")]
    public decimal? DiscountOverride { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation
    public virtual Promotion Promotion { get; set; }
    public virtual Product Product { get; set; }
    
    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
```

### Voucher Entity (NEW)

```csharp
[Table("Vouchers")]
public class Voucher : BaseEntity, ISoftDelete
{
    [Required, MaxLength(100)]
    public string Code { get; set; }
    
    public Guid PromotionId { get; set; }
    
    [Required]
    public DateTime ExpiryDate { get; set; }
    
    public int UsageCount { get; set; } = 0
    
    public DateTime? LastUsedAt { get; set; }
    
    public Guid? LastUsedBy { get; set; }
    
    // Navigation
    public virtual Promotion Promotion { get; set; }
    public virtual ICollection<VoucherRedemption> Redemptions { get; set; }
    
    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
}
```

### CampaignImpression Entity (NEW)

```csharp
[Table("CampaignImpressions")]
public class CampaignImpression : BaseEntity
{
    public Guid CampaignId { get; set; }
    
    [MaxLength(100)]
    public string SessionId { get; set; }
    
    [MaxLength(100)]
    public string Page { get; set; }
    
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public virtual Campaign Campaign { get; set; }
}
```

### CampaignClick Entity (NEW)

```csharp
[Table("CampaignClicks")]
public class CampaignClick : BaseEntity
{
    public Guid CampaignId { get; set; }
    
    [MaxLength(100)]
    public string SessionId { get; set; }
    
    [MaxLength(100)]
    public string Page { get; set; }
    
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public virtual Campaign Campaign { get; set; }
}
```

### VoucherRedemption Entity (NEW)

```csharp
[Table("VoucherRedemptions")]
public class VoucherRedemption : BaseEntity
{
    public Guid VoucherId { get; set; }
    
    public Guid? OrderId { get; set; }
    
    [Column(TypeName = "decimal(15,2)")]
    public decimal DiscountAmount { get; set; }
    
    public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;
    
    public Guid? RedeemedBy { get; set; }
    
    // Navigation
    public virtual Voucher Voucher { get; set; }
}
```

---

## Correctness Properties

A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.

### Property 1: Campaign Creation Persists All Fields
*For any* valid campaign data, creating a campaign should result in all fields being persisted and retrievable with the same values.
**Validates: Requirements 1.1**

### Property 2: Invalid Date Range Rejected
*For any* campaign where EndDate <= StartDate, the system should reject creation with HTTP 400 error.
**Validates: Requirements 1.2**

### Property 3: Negative Budget Rejected
*For any* campaign where Budget < 0, the system should reject creation with HTTP 400 error.
**Validates: Requirements 1.3**

### Property 4: Campaign Updates Persist Changes
*For any* campaign, updating a field should result in the new value being persisted and retrievable.
**Validates: Requirements 1.4**

### Property 5: Campaign Retrieval Supports Pagination and Filtering
*For any* set of campaigns with different statuses and dates, retrieving with filters should return only matching campaigns in the correct page.
**Validates: Requirements 1.5**

### Property 6: Promotions Accepted for DRAFT Campaigns
*For any* DRAFT campaign, creating a promotion should succeed and the promotion should be linked to the campaign.
**Validates: Requirements 2.1**

### Property 7: Promotions Rejected for Non-DRAFT Campaigns
*For any* non-DRAFT campaign, attempting to create a promotion should fail with HTTP 409 error.
**Validates: Requirements 2.2**

### Property 8: Promotion Required Fields Validation
*For any* promotion missing DiscountValue, DiscountType, or CampaignId, creation should fail with HTTP 400 error.
**Validates: Requirements 2.3**

### Property 9: Promotion Invalid Date Range Rejected
*For any* promotion where EndDate <= StartDate, creation should fail with HTTP 400 error.
**Validates: Requirements 2.4**

### Property 10: Promotions Retrieved for Campaign
*For any* campaign with multiple promotions, retrieving promotions should return all linked promotions.
**Validates: Requirements 2.5**

### Property 11: Voucher Codes Generated Uniquely
*For any* promotion, generating N voucher codes should result in N unique codes all linked to the promotion.
**Validates: Requirements 3.1**

### Property 12: Voucher Code Validation
*For any* valid voucher code, validation should succeed; for any invalid or expired code, validation should fail with HTTP 400 error.
**Validates: Requirements 3.2, 3.4**

### Property 13: Discount Calculation Accuracy
*For any* promotion with DiscountType='percentage' and DiscountValue=X, applying to price P should result in discount = P × (X / 100).
**Validates: Requirements 5.1**

### Property 14: Fixed Discount Calculation
*For any* promotion with DiscountType='fixed' and DiscountValue=X, applying to price P should result in discount = X.
**Validates: Requirements 5.2**

### Property 15: Highest Discount Applied (No Stacking)
*For any* order with multiple applicable promotions, only the promotion with the highest discount should be applied.
**Validates: Requirements 5.3**

### Property 16: Minimum Order Value Enforced
*For any* promotion with MinOrderAmount=M, applying to cart with total < M should fail with HTTP 400 error.
**Validates: Requirements 5.4**

### Property 17: Usage Limit Enforced
*For any* promotion with UsageLimit=L, after L applications, further applications should fail with HTTP 409 error.
**Validates: Requirements 5.5**

### Property 18: Campaign Initial Status is DRAFT
*For any* newly created campaign, the initial status should be DRAFT.
**Validates: Requirements 6.1**

### Property 19: DRAFT Campaign Allows Promotion Management
*For any* DRAFT campaign, adding and removing promotions should succeed.
**Validates: Requirements 6.2**

### Property 20: Status Transition Validation
*For any* campaign transitioning from DRAFT to ACTIVE, all required fields must be set or transition should fail with HTTP 400 error.
**Validates: Requirements 6.3**

### Property 21: ACTIVE Campaign Prevents New Promotions
*For any* ACTIVE campaign, attempting to add a promotion should fail with HTTP 409 error.
**Validates: Requirements 6.4**

### Property 22: Automatic Status Completion
*For any* campaign with EndDate in the past, the status should be automatically transitioned to COMPLETED.
**Validates: Requirements 6.5**

### Property 23: Impression Tracking Records Events
*For any* active campaign, tracking an impression should result in a CampaignImpression record being created.
**Validates: Requirements 7.1**

### Property 24: Click Tracking Records Events
*For any* active campaign, tracking a click should result in a CampaignClick record being created.
**Validates: Requirements 7.2**

### Property 25: Redemption Tracking Records Events
*For any* applied voucher, a VoucherRedemption record should be created with timestamp and user info.
**Validates: Requirements 7.3**

### Property 26: Campaign Stats Calculation
*For any* campaign with tracked impressions, clicks, and redemptions, requesting stats should return accurate counts and calculated rates (CTR, conversion rate).
**Validates: Requirements 7.4**

### Property 27: Stats Filtering by Date Range
*For any* campaign stats request with date range filters, only events within the range should be included in calculations.
**Validates: Requirements 7.5**

### Property 28: Success Response Format Consistency
*For any* successful API request, the response should contain success=true, data object, and message field.
**Validates: Requirements 8.1**

### Property 29: Validation Error Response Format
*For any* validation failure, the response should return HTTP 400 with field-level error details in errors object.
**Validates: Requirements 8.2**

### Property 30: Not Found Error Response
*For any* request for non-existent resource, the response should return HTTP 404 with descriptive message.
**Validates: Requirements 8.3**

### Property 31: Business Rule Violation Response
*For any* business rule violation, the response should return HTTP 409 with specific conflict reason and errorCode.
**Validates: Requirements 8.4**

### Property 32: Authorization Error Response
*For any* unauthorized request, the response should return HTTP 401 or 403 with appropriate message.
**Validates: Requirements 8.5**

---

## Error Handling

### Error Response Format

```json
{
  "success": false,
  "message": "Human-readable error message",
  "errorCode": "MACHINE_READABLE_CODE",
  "errors": {
    "fieldName": ["Field-specific error message"]
  }
}
```

### HTTP Status Codes

| Status | Scenario | Example |
|--------|----------|---------|
| 200 | Successful request | Campaign created successfully |
| 400 | Validation error | EndDate <= StartDate |
| 401 | Unauthorized | Missing authentication token |
| 403 | Forbidden | User lacks permission |
| 404 | Not found | Campaign ID doesn't exist |
| 409 | Business rule violation | Adding promotion to non-DRAFT campaign |
| 500 | Server error | Unexpected exception |

### Error Codes

- `INVALID_DATE_RANGE` - EndDate <= StartDate
- `NEGATIVE_BUDGET` - Budget < 0
- `CAMPAIGN_NOT_DRAFT` - Cannot add promotion to non-DRAFT campaign
- `INVALID_VOUCHER` - Voucher code invalid or expired
- `VOUCHER_LIMIT_EXCEEDED` - Voucher usage limit reached
- `MIN_ORDER_VALUE_NOT_MET` - Cart total below minimum
- `PROMOTION_NOT_FOUND` - Promotion doesn't exist
- `CAMPAIGN_NOT_FOUND` - Campaign doesn't exist
- `INVALID_STATUS_TRANSITION` - Cannot transition to requested status

---

## Testing Strategy

### Unit Testing Approach

Unit tests verify specific examples, edge cases, and error conditions:

- Test campaign creation with valid data
- Test campaign creation with invalid dates
- Test campaign creation with negative budget
- Test promotion creation for DRAFT vs non-DRAFT campaigns
- Test discount calculations (percentage and fixed)
- Test voucher code generation and uniqueness
- Test status transitions and validations
- Test error response formats

### Property-Based Testing Approach

Property-based tests verify universal properties that should hold across all inputs using **xUnit with Bogus** for C#:

- For each correctness property, create a property-based test
- Use Bogus to generate random valid/invalid campaign, promotion, and voucher data
- Run minimum 100 iterations per property test
- Tag each test with the property number and requirement reference

**Example Property Test Structure:**
```csharp
[Fact]
public void Property_1_CampaignCreationPersistsAllFields()
{
    // **Feature: campaign-promotion-api, Property 1: Campaign Creation Persists All Fields**
    // **Validates: Requirements 1.1**
    
    // Generate random valid campaign data
    var faker = new Faker<CreateCampaignDto>();
    
    // Run 100+ iterations with random data
    for (int i = 0; i < 100; i++)
    {
        var dto = faker.Generate();
        
        // Create campaign
        var result = _campaignService.CreateCampaignAsync(dto).Result;
        
        // Verify all fields persisted
        Assert.Equal(dto.CampaignName, result.CampaignName);
        Assert.Equal(dto.Budget, result.Budget);
        // ... verify all fields
    }
}
```

### Test Coverage Goals

- **Unit Tests**: 80%+ coverage of business logic
- **Property Tests**: 100% coverage of correctness properties
- **Integration Tests**: Key workflows (create campaign → add promotion → generate vouchers → apply voucher)

---

## Implementation Notes

### Database Optimization

1. **Indexes**: Add indexes on frequently queried fields
   - Campaign: (StoreId, Status, StartDate, EndDate)
   - Promotion: (CampaignId, Status)
   - Voucher: (Code, ExpiryDate)
   - CampaignImpression: (CampaignId, RecordedAt)
   - CampaignClick: (CampaignId, RecordedAt)

2. **Partitioning**: Consider partitioning analytics tables by date for large datasets

3. **Archival**: Archive old campaigns and analytics data to separate tables

### Caching Strategy

1. **Campaign Caching**: Cache active campaigns for 5 minutes
2. **Promotion Caching**: Cache promotions for 5 minutes
3. **Voucher Validation**: Cache valid voucher codes for 1 minute
4. **Stats Caching**: Cache campaign stats for 15 minutes

### Validation Rules

1. **Campaign**: Name required, budget >= 0, EndDate > StartDate
2. **Promotion**: DiscountValue > 0, EndDate > StartDate, Campaign must be DRAFT
3. **Voucher**: Code unique, ExpiryDate in future, Usage <= Limit
4. **Targeting**: Pages array not empty, frequency in [once-per-session, once-per-page, always]

### Business Rules

1. Only DRAFT campaigns can have promotions added
2. No discount stacking (apply highest discount only)
3. Minimum order value must be met to apply voucher
4. Voucher usage limit must be enforced
5. Campaign status transitions must be validated
6. Automatic status completion when end date reached

---

## Frontend Integration Points

### Campaign Fetching
```
GET /api/v1/campaigns?status=ACTIVE&pages=home
Response: List of active campaigns targeting the home page
```

### Impression Tracking
```
POST /api/v1/campaigns/{id}/track-impression
Body: { sessionId, page, timestamp }
```

### Click Tracking
```
POST /api/v1/campaigns/{id}/track-click
Body: { sessionId, page, timestamp }
```

### Voucher Application
```
POST /api/v1/cart/apply-voucher
Body: { couponCode }
Response: Updated cart with discount applied
```

### Analytics
```
GET /api/v1/campaigns/{id}/stats?fromDate=2025-01-01&toDate=2025-02-15
Response: Impressions, clicks, redemptions, revenue, CTR, conversion rate
```

---

## Summary

This design leverages existing entities (Campaign, Promotion, PromotionProduct) and adds minimal new tables (Voucher, CampaignImpression, CampaignClick, VoucherRedemption) to implement a complete Campaign & Promotion Management System. The architecture follows layered design principles with clear separation of concerns. Correctness properties ensure the system behaves correctly across all scenarios, validated through comprehensive unit and property-based testing.

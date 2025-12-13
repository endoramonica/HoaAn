# Discount Calculation Service - Implementation Guide

## Quick Reference

### Service Interface
```csharp
public interface IDiscountCalculationService
{
    Task<decimal> CalculateDiscountAsync(PromotionDto promotion, decimal originalPrice);
    Task<ApiResponse<DiscountValidationResultDto>> ValidatePromotionAsync(PromotionDto promotion, decimal cartTotal);
    Task<decimal> CalculateFinalDiscountAsync(PromotionDto promotion, decimal cartTotal);
}
```

### Usage Example

#### 1. Calculate Percentage Discount
```csharp
var promotion = new PromotionDto
{
    PromotionType = PromotionType.PERCENTAGE,
    DiscountValue = 20 // 20%
};

var discount = await _discountCalculationService.CalculateDiscountAsync(promotion, 100m);
// Result: 20m (100 * 20 / 100)
```

#### 2. Calculate Fixed Amount Discount
```csharp
var promotion = new PromotionDto
{
    PromotionType = PromotionType.FIXED_AMOUNT,
    DiscountValue = 50000m // 50,000 VND
};

var discount = await _discountCalculationService.CalculateDiscountAsync(promotion, 199000m);
// Result: 50000m
```

#### 3. Validate Promotion with Minimum Order Value
```csharp
var promotion = new PromotionDto
{
    MinOrderAmount = 100000m,
    UsageLimit = 100,
    UsedCount = 50,
    Status = PromotionStatus.ACTIVE,
    StartDate = DateTime.UtcNow.AddDays(-1),
    EndDate = DateTime.UtcNow.AddDays(1)
};

var result = await _discountCalculationService.ValidatePromotionAsync(promotion, 150000m);
// Result: IsValid = true
```

#### 4. Calculate Final Discount with All Validations
```csharp
var promotion = new PromotionDto
{
    PromotionType = PromotionType.PERCENTAGE,
    DiscountValue = 50,
    MaxDiscount = 50000m,
    MinOrderAmount = 100000m,
    UsageLimit = 100,
    UsedCount = 50,
    Status = PromotionStatus.ACTIVE,
    StartDate = DateTime.UtcNow.AddDays(-1),
    EndDate = DateTime.UtcNow.AddDays(1)
};

var finalDiscount = await _discountCalculationService.CalculateFinalDiscountAsync(promotion, 200000m);
// Result: 50000m (capped at MaxDiscount)
```

---

## Calculation Formulas

### Percentage Discount (Requirement 5.1)
```
discount = originalPrice × (discountValue / 100)
```

**Example**: 20% off 100,000 VND
```
discount = 100,000 × (20 / 100) = 20,000 VND
```

### Fixed Amount Discount (Requirement 5.2)
```
discount = discountValue
```

**Example**: 50,000 VND off
```
discount = 50,000 VND
```

---

## Validation Rules

### Minimum Order Value (Requirement 5.4)
```
IF cartTotal < promotion.MinOrderAmount
  THEN reject with error code: MIN_ORDER_VALUE_NOT_MET
```

**Example**: Promotion requires minimum 100,000 VND
```
cartTotal = 50,000 VND → REJECTED
cartTotal = 100,000 VND → ACCEPTED
cartTotal = 150,000 VND → ACCEPTED
```

### Usage Limit (Requirement 5.5)
```
IF promotion.UsedCount >= promotion.UsageLimit
  THEN reject with error code: USAGE_LIMIT_EXCEEDED
```

**Example**: Promotion limited to 100 uses
```
UsedCount = 50, UsageLimit = 100 → ACCEPTED
UsedCount = 100, UsageLimit = 100 → REJECTED
UsedCount = 101, UsageLimit = 100 → REJECTED
```

### Additional Validations
- Promotion status must be ACTIVE
- Current time must be within StartDate and EndDate
- Discount cannot exceed original price
- Discount cannot exceed cart total
- Maximum discount cap applied if configured

---

## Error Codes

| Error Code | Meaning | HTTP Status |
|-----------|---------|------------|
| `MIN_ORDER_VALUE_NOT_MET` | Cart total below minimum | 400 |
| `USAGE_LIMIT_EXCEEDED` | Promotion usage limit reached | 409 |
| `PROMOTION_NOT_ACTIVE` | Promotion status is not ACTIVE | 400 |
| `PROMOTION_EXPIRED` | Current time outside promotion period | 400 |

---

## Integration with Other Services

### CartService Integration
```csharp
// In CartService.ApplyVoucherAsync()
var promotion = await _promotionService.GetPromotionByIdAsync(promotionId);
var finalDiscount = await _discountCalculationService.CalculateFinalDiscountAsync(
    promotion, 
    cartTotal
);
cart.DiscountAmount = finalDiscount;
```

### CheckoutService Integration
```csharp
// In CheckoutService.CalculateOrderTotal()
var discount = await _discountCalculationService.CalculateDiscountAsync(
    promotion,
    subtotal
);
var total = subtotal - discount;
```

### VoucherService Integration
```csharp
// In VoucherService.ApplyVoucherAsync()
var validationResult = await _discountCalculationService.ValidatePromotionAsync(
    promotion,
    cartTotal
);
if (!validationResult.Data.IsValid)
{
    throw new InvalidOperationException(validationResult.Data.ErrorMessage);
}
```

---

## Testing

### Unit Tests Location
`VietCommerce.Tests/Services/DiscountCalculationServiceTests.cs`

### Test Coverage
- ✅ Percentage discount calculation
- ✅ Fixed amount discount calculation
- ✅ Minimum order value validation
- ✅ Usage limit validation
- ✅ Maximum discount cap
- ✅ Edge cases (zero price, null promotion, etc.)
- ✅ Error scenarios

### Running Tests
```bash
dotnet test VietCommerce.Tests/VietCommerce.Tests.csproj --filter "DiscountCalculationServiceTests"
```

---

## Performance Considerations

### Caching
- Service inherits from BaseService which provides caching
- Promotion data should be cached to avoid repeated database queries
- Cache invalidation on promotion updates

### Optimization Tips
1. Cache promotion data for 5-15 minutes
2. Pre-calculate discounts during cart operations
3. Use database indexes on promotion lookup fields
4. Consider batch discount calculations for bulk operations

---

## Future Enhancements

### Potential Extensions
1. **Tiered Discounts**: Different discount values based on quantity
2. **Combo Discounts**: Discounts for product combinations
3. **Time-based Discounts**: Different discounts at different times
4. **User Segment Discounts**: Different discounts for user groups
5. **Discount Stacking**: Allow multiple promotions (with rules)

### Related Tasks
- Task 12: Implement No-Stacking Rule
- Task 16: Implement Voucher Application to Cart
- Task 11.1: Write property tests for discount calculations

---

## Troubleshooting

### Issue: Discount exceeds original price
**Solution**: Service automatically caps discount at original price

### Issue: Promotion validation fails unexpectedly
**Solution**: Check promotion status, dates, and usage limits

### Issue: Minimum order value not enforced
**Solution**: Ensure MinOrderAmount is set on promotion and validation is called

### Issue: Usage limit not working
**Solution**: Ensure UsageLimit is set and UsedCount is tracked correctly

---

## References

- **Requirements**: 5.1, 5.2, 5.4, 5.5
- **Design Document**: See Correctness Properties section
- **Related Services**: CartService, CheckoutService, VoucherService, PromotionService

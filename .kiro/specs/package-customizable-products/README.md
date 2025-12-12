# Package & Customizable Products Feature Spec

## 📋 Overview

This spec defines the implementation of **Package Products with Customizable Options** for VietCommerce - enabling merchants to create pre-configured ritual packages that customers can customize.

**Status:** ✅ Approved (Requirements, Design, Tasks)

## 📁 Files

- **requirements.md** - 9 requirements with acceptance criteria covering all aspects of the feature
- **design.md** - Architecture, data models, DTOs, service interfaces, and testing strategy
- **tasks.md** - 55 implementation tasks organized in 9 phases

## 🎯 Key Features

### For Merchants
- Create package products with fixed items and customizable options
- Set min/max quantities and prices for each option
- Update options anytime (changes apply only to new orders)
- View analytics on popular customization options

### For Customers
- View package details with included items and customizable options
- Add packages to cart with custom quantities
- Update customizations before checkout
- See price breakdown (base + customization surcharges)

### For System
- Preserve customization details in orders (snapshots)
- Maintain historical accuracy (prices at order time)
- Validate all JSON data before storage
- Support analytics on customization patterns

## 📊 Implementation Phases

| Phase | Tasks | Duration | Focus |
|-------|-------|----------|-------|
| 1 | Database & Entities (1-6) | 1 day | Schema changes |
| 2 | DTOs & Validators (7-17) | 1 day | Data structures |
| 3 | Product Service (18-26) | 1.5 days | Product management |
| 4 | Cart Service (27-32) | 1.5 days | Cart operations |
| 5 | Order Service (33-36) | 1 day | Order processing |
| 6 | API Controllers (37-43) | 1 day | Endpoints |
| 7 | Analytics (44-46) | 0.5 days | Reporting |
| 8 | Property Tests (47-51) | 1 day | Correctness properties |
| 9 | Integration (52-55) | 1 day | E2E testing |

**Total:** ~9 days (1.5 sprints)

## 🔧 Core Concepts

### JSON Storage
- **Product.DetailsJson**: Array of included items (strings)
- **Product.CustomizableOptionsJson**: Array of customizable options (objects)
- **CartItem.CustomizationsJson**: Array of customer's customization choices
- **OrderItem.CustomizationsJson**: Snapshot of customizations at order time

### Price Calculation
```
FinalPrice = BasePrice + sum(customization quantities × unit prices)
```

### Snapshot Pattern
- CartItem stores current customizations
- OrderItem snapshots CartItem data at checkout
- Product updates don't affect existing orders
- Enables accurate fulfillment and analytics

## ✅ Testing Strategy

### Unit Tests (Optional)
- DTO validators
- Service methods
- Price calculations
- JSON serialization

### Property-Based Tests (Optional)
- JSON round-trip (serialize → deserialize = original)
- Price calculation accuracy
- Quantity validation
- Snapshot preservation
- Product update isolation

### Integration Tests (Optional)
- API endpoints
- Complete workflows
- Error handling

## 🚀 Next Steps

1. **Start Phase 1:** Create database migrations
2. **Execute tasks sequentially** - each task builds on previous
3. **Run tests after each phase** - catch issues early
4. **Deploy incrementally** - can release after Phase 6 (core features)

## 📝 Notes

- Optional tasks (marked with *) can be skipped for MVP
- All core functionality (Phases 1-6) is required
- Property-based tests provide strong correctness guarantees
- Analytics (Phase 7) can be added later if needed


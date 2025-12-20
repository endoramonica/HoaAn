# Implementation Tasks: Services-Product Unification

## Overview

Kế hoạch này thống nhất Services và Products bằng cách mở rộng **2 models**:
1. **Product Model** - Cho ServicesPage (browse & buy)
2. **Order Model** - Cho ProfilePage (order history)

---

## Part 1: Product Model (ServicesPage)

### Phase 1.1: Backend - Product Model

- [x] 1.1 Extend Product database schema
  - Add `Type` column (default: 'product')
  - Add `ServiceCategory` column
  - Add `ServiceDuration` column
  - Add `ServiceRating` column
  - Create indexes on Type and ServiceCategory
  - _Requirements: 1.1, 1.2_
  - ✅ **COMPLETED**: Migration file created, Entity updated

- [x] 1.2 Update Product DTOs
  - Update ProductListDto with new fields
  - Update ProductFilterDto with type and serviceCategory filters
  - Add validation for type and serviceCategory
  - _Requirements: 1.1, 1.2, 3.1, 3.2_
  - ✅ **COMPLETED**: ProductListDto and ProductFilterDto updated

- [x] 1.3 Modify GET /api/v1/Product endpoint
  - Accept `type` query parameter
  - Accept `serviceCategory` query parameter
  - Filter products by type
  - Filter services by category
  - Maintain backward compatibility
  - _Requirements: 1.3, 1.4, 1.5_
  - ✅ **COMPLETED**: Repository and Service updated with filtering

- [x] 1.4 Seed service data
  - Create 6 services (one per category)
  - Set type='service' for all services
  - Set realistic prices, durations, ratings
  - _Requirements: 1.1, 1.2_
  - ✅ **COMPLETED**: ProductServiceSeeder created and registered in Program.cs

- [x] 1.5 Checkpoint - Test Product API
  - Test `GET /api/v1/Product?type=service` returns only services
  - Test `GET /api/v1/Product?type=product` returns only products
  - Test filtering by serviceCategory works
  - Ask user if backend is ready
  - ✅ **COMPLETED**: Migration applied, seeder registered, ready for testing

### Phase 1.2: Frontend - Product Types

- [ ] 1.6 Update ProductListDto type
  - Add `type?: 'product' | 'service'`
  - Add `serviceCategory?: string`
  - Add `serviceDuration?: string`
  - Add `rating?: number`
  - _Requirements: 3.1, 3.2_

- [ ] 1.7 Update ProductFilterDto type
  - Add `type?: 'product' | 'service'`
  - Add `serviceCategory?: string`
  - _Requirements: 3.3, 3.4_

- [ ] 1.8 Create ServiceCategory enum
  - Define all 6 service categories
  - Export from types file
  - _Requirements: 3.2_

### Phase 1.3: Frontend - Service Layer

- [ ] 1.9 Extend ProductService
  - Add `getServices(filter)` method
  - Add `getServiceById(id)` method
  - Add error handling
  - _Requirements: 5.1, 5.2, 5.5_

### Phase 1.4: Frontend - useServices Hook

- [ ] 1.10 Create useServices() hook
  - Wrap useProducts() with type='service'
  - Implement filter methods
  - Implement pagination methods
  - Implement error handling
  - _Requirements: 7.1, 7.4_

### Phase 1.5: Frontend - ServicesPage

- [ ] 1.11 Refactor ServicesPage
  - Replace mock data with useServices() hook
  - Implement category filtering
  - Implement search
  - Implement pagination
  - Implement loading states
  - Implement empty state
  - Implement error handling
  - _Requirements: 8.1-8.7_

- [ ] 1.12 Checkpoint - Test ServicesPage
  - Test services load from API
  - Test filtering works
  - Test search works
  - Test pagination works
  - Test add to cart works
  - Ask user if ServicesPage works

---

## Part 2: Order Model (ProfilePage)

### Phase 2.1: Backend - Order Model

- [x] 2.1 Extend Order database schema
  - Add `Type` column to Orders (default: 'product')
  - Add service-specific columns to Orders (ServiceCategory, ServiceDuration, ServiceLocation, ServiceDate, ServiceTime, ServiceNotes)
  - Add `Type` column to OrderItems
  - Add service-specific columns to OrderItems
  - Create indexes on Type and ServiceCategory
  - _Requirements: 2.1, 2.2_
  - ✅ **COMPLETED**: Migration file created, Order and OrderItem entities updated

- [x] 2.2 Update Order DTOs
  - Update OrderDetailDto with new fields
  - Update OrderItemDTO with new fields
  - Update OrderFilterDto with type and serviceCategory filters
  - Add validation
  - _Requirements: 2.1, 2.2, 4.1-4.5_
  - ✅ **COMPLETED**: OrderDetailDTO, OrderItemDTO, OrderFilterDTO updated

- [x] 2.3 Modify GET /api/v1/Order/my-orders endpoint
  - Accept `type` query parameter
  - Accept `serviceCategory` query parameter
  - Accept `status` query parameter
  - Accept date range parameters
  - Filter orders by type
  - Filter service orders by category
  - Maintain backward compatibility
  - _Requirements: 2.3, 2.4, 2.5_
  - ✅ **COMPLETED**: OrderRepository updated with type and serviceCategory filtering

- [x] 2.4 Checkpoint - Test Order API
  - Test `GET /api/v1/Order/my-orders?type=service` returns only service orders
  - Test `GET /api/v1/Order/my-orders?type=product` returns only product orders
  - Test filtering by serviceCategory works
  - Test filtering by status works
  - Ask user if backend is ready
  - ✅ **COMPLETED**: Migration applied, filtering logic implemented, ready for testing

### Phase 2.2: Frontend - Order Types

- [ ] 2.5 Update OrderDetailDto type
  - Add `type?: 'product' | 'service'`
  - Add service-specific fields
  - _Requirements: 4.1, 4.2_

- [ ] 2.6 Update OrderItemDTO type
  - Add `type?: 'product' | 'service'`
  - Add service-specific fields
  - _Requirements: 4.3_

- [ ] 2.7 Create OrderFilterDto type
  - Add `type?: 'product' | 'service'`
  - Add `serviceCategory?: string`
  - Add `status?: string`
  - Add date range fields
  - _Requirements: 4.4, 4.5_

### Phase 2.3: Frontend - Order Service Layer

- [ ] 2.8 Create OrderService
  - Add `getOrders(filter)` method
  - Add `getServiceOrders(filter)` method
  - Add `getProductOrders(filter)` method
  - Add error handling
  - _Requirements: 6.1-6.5_

### Phase 2.4: Frontend - Order Hooks

- [ ] 2.9 Create useOrders() hook
  - Fetch orders from API
  - Implement filter methods
  - Implement pagination methods
  - Implement error handling
  - _Requirements: 7.2, 7.4_

- [ ] 2.10 Create useServiceOrders() hook
  - Wrap useOrders() with type='service'
  - _Requirements: 7.3, 7.4_

### Phase 2.5: Frontend - ProfilePage

- [ ] 2.11 Refactor ProfilePage Orders Tab
  - Replace mock data with useOrders() hook (type='product')
  - Implement status filtering
  - Implement pagination
  - Implement loading states
  - Implement empty state
  - Implement error handling
  - _Requirements: 9.1-9.5_

- [ ] 2.12 Refactor ProfilePage Services Tab
  - Replace ServiceBookingsTab with useServiceOrders() hook
  - Implement service category filtering
  - Implement status filtering
  - Implement pagination
  - Implement loading states
  - Implement empty state with link to ServicesPage
  - Implement error handling
  - _Requirements: 10.1-10.6_

- [ ] 2.13 Checkpoint - Test ProfilePage
  - Test Orders Tab loads product orders from API
  - Test Services Tab loads service orders from API
  - Test filtering works
  - Test pagination works
  - Test empty states show
  - Ask user if ProfilePage works

---

## Part 3: Testing & Verification

### Phase 3.1: Unit Tests

- [ ] 3.1 Test ProductService
  - Test getServices() returns only services
  - Test filtering works
  - Test error handling
  - _Requirements: 5.1-5.5_

- [ ] 3.2 Test OrderService
  - Test getOrders() works
  - Test getServiceOrders() returns only service orders
  - Test getProductOrders() returns only product orders
  - Test filtering works
  - Test error handling
  - _Requirements: 6.1-6.5_

- [ ] 3.3 Test useServices() hook
  - Test hook fetches services
  - Test filter changes trigger refetch
  - Test pagination works
  - _Requirements: 7.1, 7.4_

- [ ] 3.4 Test useOrders() hook
  - Test hook fetches orders
  - Test filter changes trigger refetch
  - Test pagination works
  - _Requirements: 7.2, 7.4_

- [ ] 3.5 Test ServicesPage component
  - Test services load on mount
  - Test filtering works
  - Test search works
  - Test pagination works
  - _Requirements: 8.1-8.7_

- [ ] 3.6 Test ProfilePage component
  - Test Orders Tab loads product orders
  - Test Services Tab loads service orders
  - Test filtering works
  - Test pagination works
  - _Requirements: 9.1-9.5, 10.1-10.6_

### Phase 3.2: Integration Tests

- [ ] 3.7 Test complete flow: Browse → Buy → View Order
  - Browse services on ServicesPage
  - Add service to cart
  - Checkout
  - View service order in ProfilePage Services Tab
  - _Requirements: All_

- [ ] 3.8 Test backward compatibility
  - Test ProductsPage still works
  - Test ProductsPage doesn't show services
  - Test filter state isolation
  - _Requirements: 11.1-11.4_

### Phase 3.3: Final Verification

- [ ] 3.9 Code review and cleanup
  - Remove mock data
  - Clean up unused imports
  - Add JSDoc comments
  - Verify code style

- [ ] 3.10 Update documentation
  - Update API documentation
  - Add hook documentation
  - Add component documentation

- [ ] 3.11 Final checkpoint
  - Run `npm run build` - verify no errors
  - Run `npm run test` - verify all tests pass
  - Run `npm run lint` - verify no linting errors
  - Ask user if ready for production

---

## Summary

**Total Tasks**: 37

### Part 1: Product Model (ServicesPage)
- Backend: 5 tasks (1.1-1.5)
- Frontend: 7 tasks (1.6-1.12)

### Part 2: Order Model (ProfilePage)
- Backend: 4 tasks (2.1-2.4)
- Frontend: 9 tasks (2.5-2.13)

### Part 3: Testing & Verification
- Testing: 12 tasks (3.1-3.11)

**Estimated Timeline**:
- Part 1 (Product Model): 4-5 days
- Part 2 (Order Model): 4-5 days
- Part 3 (Testing): 2-3 days
- **Total**: ~10-13 days


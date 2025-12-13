# Campaign & Promotion Management System - Implementation Plan

## Overview

This implementation plan breaks down the Campaign & Promotion Management System into discrete, manageable coding tasks. Each task builds incrementally on previous tasks, with testing integrated throughout to catch errors early.

---

## Phase 1: Database Schema & Entity Setup

- [X] 1. Create new database entities and migrations

  - Create Voucher entity with Code, PromotionId, ExpiryDate, UsageCount, LastUsedAt, LastUsedBy
  - Create CampaignImpression entity with CampaignId, SessionId, Page, RecordedAt
  - Create CampaignClick entity with CampaignId, SessionId, Page, RecordedAt
  - Create VoucherRedemption entity with VoucherId, OrderId, DiscountAmount, RedeemedAt, RedeemedBy
  - Add navigation properties to Campaign, Promotion entities
  - Create EF Core migration for new tables
  - _Requirements: 1.1, 2.1, 3.1, 7.1, 7.2, 7.3_

- [ ]* 1.1 Write property test for entity creation
  - **Property 1: Campaign Creation Persists All Fields**
  - **Validates: Requirements 1.1**

- [x] 2. Update Campaign entity with targeting rules





  - Add TargetingRules property (JSON string) to Campaign entity
  - Create TargetingRulesDto class with Pages, Frequency, DelayMs, AutoDismissMs
  - Add JSON serialization/deserialization helpers
  - Create migration for new column
  - _Requirements: 4.1_

- [x]* 2.1 Write property test for targeting rules


  - **Property 4: Campaign Updates Persist Changes**
  - **Validates: Requirements 1.4**

---

## Phase 2: Campaign Management Service & API

- [x] 3. Implement Campaign CRUD operations




  - Create CampaignService with CreateCampaignAsync, GetCampaignsAsync, UpdateCampaignAsync, DeleteCampaignAsync
  - Implement campaign creation with validation (EndDate > StartDate, Budget >= 0)
  - Implement campaign retrieval with pagination and filtering by status/date range
  - Implement campaign update with change tracking
  - Implement campaign deletion (soft delete via IsDeleted flag if needed)
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_

- [ ]* 3.1 Write property tests for campaign CRUD
  - **Property 2: Invalid Date Range Rejected**
  - **Property 3: Negative Budget Rejected**
  - **Property 5: Campaign Retrieval Supports Pagination and Filtering**
  - **Validates: Requirements 1.2, 1.3, 1.5**

- [x] 4. Implement Campaign API endpoints





  - Create CampaignController with POST, GET, PUT, DELETE endpoints
  - Implement request/response DTOs (CreateCampaignDto, UpdateCampaignDto, CampaignDto)
  - Add validation attributes and error handling
  - Return proper HTTP status codes (200, 400, 404, 409)
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 8.1, 8.2, 8.3, 8.4_

- [ ]* 4.1 Write unit tests for Campaign endpoints
  - Test successful campaign creation
  - Test campaign creation with invalid dates
  - Test campaign creation with negative budget
  - Test campaign retrieval with pagination
  - Test campaign update
  - Test campaign deletion
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_

- [x] 5. Implement Campaign Status Lifecycle




  - Create ChangeCampaignStatusAsync method in CampaignService
  - Implement status transition validation (DRAFT → ACTIVE requires all fields)
  - Implement automatic status completion (DRAFT → COMPLETED when end date reached)
  - Add status transition endpoint to CampaignController
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5_

- [x]* 5.1 Write property tests for status lifecycle

  - **Property 18: Campaign Initial Status is DRAFT**
  - **Property 20: Status Transition Validation**
  - **Property 22: Automatic Status Completion**
  - **Validates: Requirements 6.1, 6.3, 6.5**

- [x] 6. Checkpoint - Ensure all tests pass





  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 3: Promotion Management Service & API

- [x] 7. Implement Promotion CRUD operations





  - Create PromotionService with CreatePromotionAsync, GetPromotionsByCampaignAsync, UpdatePromotionAsync, DeletePromotionAsync
  - Implement promotion creation with validation (Campaign must be DRAFT, EndDate > StartDate, DiscountValue > 0)
  - Implement promotion retrieval for a campaign
  - Implement promotion update
  - Implement promotion deletion (soft delete)
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [ ]* 7.1 Write property tests for promotion CRUD
  - **Property 6: Promotions Accepted for DRAFT Campaigns**
  - **Property 7: Promotions Rejected for Non-DRAFT Campaigns**
  - **Property 8: Promotion Required Fields Validation**
  - **Property 9: Promotion Invalid Date Range Rejected**
  - **Property 10: Promotions Retrieved for Campaign**
  - **Validates: Requirements 2.1, 2.2, 2.3, 2.4, 2.5**

- [x] 8. Implement Promotion API endpoints





  - Create PromotionController with POST, GET, PUT, DELETE endpoints
  - Implement request/response DTOs (CreatePromotionDto, UpdatePromotionDto, PromotionDto)
  - Add validation and error handling
  - Return proper HTTP status codes
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 8.1, 8.2, 8.3, 8.4_

- [ ]* 8.1 Write unit tests for Promotion endpoints
  - Test successful promotion creation for DRAFT campaign
  - Test promotion creation rejection for non-DRAFT campaign
  - Test promotion creation with invalid dates
  - Test promotion retrieval
  - Test promotion update
  - Test promotion deletion
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [x] 9. Implement Product Linking to Promotions





  - Create LinkProductsAsync method in PromotionService
  - Create GetLinkedProductsAsync method
  - Implement endpoints for linking/unlinking products
  - Add validation to ensure products exist
  - _Requirements: 2.1, 2.3_

- [ ]* 9.1 Write unit tests for product linking
  - Test linking products to promotion
  - Test retrieving linked products
  - Test unlinking products
  - _Requirements: 2.1, 2.3_

- [ ] 10. Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 4: Discount Calculation & Validation

- [x] 11. Implement Discount Calculation Logic




  - Create DiscountCalculationService with CalculateDiscountAsync method
  - Implement percentage discount calculation: discount = price × (value / 100)
  - Implement fixed amount discount calculation: discount = value
  - Implement minimum order value validation
  - Implement maximum usage limit validation
  - _Requirements: 5.1, 5.2, 5.4, 5.5_

- [ ]* 11.1 Write property tests for discount calculations
  - **Property 13: Discount Calculation Accuracy (Percentage)**
  - **Property 14: Fixed Discount Calculation**
  - **Property 16: Minimum Order Value Enforced**
  - **Property 17: Usage Limit Enforced**
  - **Validates: Requirements 5.1, 5.2, 5.4, 5.5**

- [x] 12. Implement No-Stacking Rule





  - Create GetBestPromotionAsync method to select highest discount
  - Implement logic to prevent discount stacking
  - Add validation to ensure only one promotion per order
  - _Requirements: 5.3_

- [ ]* 12.1 Write property test for no-stacking rule
  - **Property 15: Highest Discount Applied (No Stacking)**
  - **Validates: Requirements 5.3**

- [x] 13. Checkpoint - Ensure all tests pass





  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 5: Voucher Management Service & API

- [x] 14. Implement Voucher Generation




  - Create VoucherService with GenerateVouchersAsync method
  - Implement unique code generation (prefix + random suffix)
  - Implement bulk generation with specified quantity
  - Link generated vouchers to promotion
  - _Requirements: 3.1_

- [ ]* 14.1 Write property test for voucher generation
  - **Property 11: Voucher Codes Generated Uniquely**
  - **Validates: Requirements 3.1**

- [x] 15. Implement Voucher Validation




  - Create ValidateVoucherAsync method
  - Check if code exists
  - Check if code is expired (ExpiryDate > now)
  - Check if code has reached usage limit
  - Check if promotion is active
  - _Requirements: 3.2, 3.4, 3.5_

- [ ]* 15.1 Write property test for voucher validation
  - **Property 12: Voucher Code Validation**
  - **Validates: Requirements 3.2, 3.4**

- [x] 16. Implement Voucher Application to Cart





  - Create ApplyVoucherAsync method in VoucherService
  - Integrate with CartService to apply discount
  - Update cart total with discount
  - Track voucher usage
  - _Requirements: 3.2, 3.3_

- [ ]* 16.1 Write unit tests for voucher application
  - Test applying valid voucher
  - Test applying invalid voucher
  - Test applying expired voucher
  - Test applying voucher with usage limit exceeded
  - Test applying voucher with minimum order value not met
  - _Requirements: 3.2, 3.3, 3.4, 3.5_

- [x] 17. Implement Voucher Removal from Cart





  - Create RemoveVoucherAsync method
  - Remove discount from cart
  - Reset cart total
  - _Requirements: 3.2_

- [x] 18. Implement Voucher API endpoints





  - Create VoucherController with POST endpoints for generation
  - Create CartController endpoints for apply/remove voucher
  - Implement request/response DTOs
  - Add validation and error handling
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 8.1, 8.2, 8.3, 8.4_

- [ ]* 18.1 Write unit tests for Voucher endpoints
  - Test voucher generation endpoint
  - Test apply voucher endpoint
  - Test remove voucher endpoint

  - _Requirements: 3.1, 3.2, 3.3_


- [x] 19. Checkpoint - Ensure all tests pass




  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 6: Analytics & Tracking


- [x] 20. Implement Impression Tracking


  - Create AnalyticsService with TrackImpressionAsync method
  - Create CampaignImpression record with CampaignId, SessionId, Page, RecordedAt
  - Implement impression tracking endpoint
  - _Requirements: 7.1_

- [x]* 20.1 Write property test for impression tracking

  - **Property 23: Impression Tracking Records Events**
  - **Validates: Requirements 7.1**

- [x] 21. Implement Click Tracking



  - Create TrackClickAsync method in AnalyticsService
  - Create CampaignClick record with CampaignId, SessionId, Page, RecordedAt
  - Implement click tracking endpoint
  - _Requirements: 7.2_

- [x]* 21.1 Write property test for click tracking


  - **Property 24: Click Tracking Records Events**
  - **Validates: Requirements 7.2**

- [x] 22. Implement Redemption Tracking




  - Create TrackRedemptionAsync method in AnalyticsService
  - Create VoucherRedemption record when voucher is applied
  - Track DiscountAmount, RedeemedAt, RedeemedBy
  - _Requirements: 7.3_

- [ ]* 22.1 Write property test for redemption tracking
  - **Property 25: Redemption Tracking Records Events**
  - **Validates: Requirements 7.3**

- [x] 23. Implement Campaign Statistics Calculation





  - Create GetCampaignStatsAsync method in AnalyticsService
  - Calculate impressions count
  - Calculate clicks count
  - Calculate click-through rate (CTR = clicks / impressions × 100)
  - Calculate redemptions count
  - Calculate redemption rate (redemptions / clicks × 100)
  - Calculate total revenue
  - Calculate total discount
  - Calculate conversion rate
  - _Requirements: 7.4_

- [ ]* 23.1 Write property test for stats calculation
  - **Property 26: Campaign Stats Calculation**
  - **Validates: Requirements 7.4**


- [x] 24. Implement Stats Filtering by Date Range







  - Add date range filtering to GetCampaignStatsAsync
  - Filter impressions, clicks, redemptions by date range
  - Recalculate stats based on filtered data
  - _Requirements: 7.5_


- [x]* 24.1 Write property test for stats filtering

  - **Property 27: Stats Filtering by Date Range**
  - **Validates: Requirements 7.5**


- [x] 25. Implement Analytics API endpoints




  - Create AnalyticsController with tracking endpoints
  - Create stats endpoint with date range filtering
  - Implement request/response DTOs
  - Add validation and error handling
  - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5, 8.1, 8.2, 8.3, 8.4_

- [ ]* 25.1 Write unit tests for Analytics endpoints
  - Test impression tracking endpoint
  - Test click tracking endpoint
  - Test stats retrieval endpoint
  - Test stats filtering by date range
  - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5_

- [x] 26. Checkpoint - Ensure all tests pass





  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 7: Error Handling & Response Format

- [x] 27. Implement Consistent Response Format




  - Create ApiResponse<T> wrapper class with success, data, message fields
  - Create error response format with errorCode and errors fields
  - Implement response formatting middleware
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [ ]* 27.1 Write property tests for response format
  - **Property 28: Success Response Format Consistency**
  - **Property 29: Validation Error Response Format**
  - **Property 30: Not Found Error Response**
  - **Property 31: Business Rule Violation Response**
  - **Property 32: Authorization Error Response**
  - **Validates: Requirements 8.1, 8.2, 8.3, 8.4, 8.5**

- [x] 28. Implement Error Handling Middleware





  - Create global exception handler middleware
  - Map exceptions to appropriate HTTP status codes
  - Format error responses consistently
  - Log errors appropriately
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [ ]* 28.1 Write unit tests for error handling
  - Test 400 error response format
  - Test 404 error response format
  - Test 409 error response format
  - Test 401/403 error response format
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_


- [x] 29. Checkpoint - Ensure all tests pass




  - Ensure all tests pass, ask the user if questions arise.

---

## Phase 8: Integration & Validation

- [x] 30. Implement Campaign Targeting Rules





  - Create TargetingRulesService to parse and validate targeting rules
  - Implement page targeting logic
  - Implement frequency logic (once-per-session, once-per-page, always)
  - Implement delay and auto-dismiss logic
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_

- [ ]* 30.1 Write property tests for targeting rules
  - **Property 19: DRAFT Campaign Allows Promotion Management**
  - **Property 21: ACTIVE Campaign Prevents New Promotions**
  - **Validates: Requirements 6.2, 6.4**

- [x] 31. Implement Comprehensive Validation





  - Add FluentValidation validators for all DTOs
  - Implement campaign validation (name, budget, dates)
  - Implement promotion validation (discount value, dates, campaign status)
  - Implement voucher validation (code, expiry date)
  - _Requirements: 1.1, 1.2, 1.3, 2.1, 2.2, 2.3, 2.4, 3.1, 3.2, 3.4, 3.5_

- [ ]* 31.1 Write unit tests for validation
  - Test campaign validation
  - Test promotion validation
  - Test voucher validation
  - _Requirements: 1.1, 1.2, 1.3, 2.1, 2.2, 2.3, 2.4, 3.1, 3.2, 3.4, 3.5_





- [X] 32. Implement Integration Tests

  - Test complete workflow: create campaign → add promotion → generate vouchers → apply voucher



  - Test campaign status transitions

  - Test analytics tracking and stats calculation
  - Test error scenarios
  - _Requirements: All_





- [X] 33. Checkpoint - Ensure all tests pass

  - Ensure all tests pass, ask the user if questions arise.




---

## Phase 9: Documentation & Deployment

- [x] 34. check have Added Swagger/OpenAPI Documentation






 iff not have then :


  - Add Swagger annotations to all controllers
  - Document all endpoints with descriptions, parameters, responses
  - Add example requests/responses
  - Generate OpenAPI specification



  - _Requirements: All_

- [X] 35. Create Database Indexes





  - Add index on Campaign (StoreId, Status, StartDate, EndDate)
  - Add index on Promotion (CampaignId, Status)
  - Add index on Voucher (Code, ExpiryDate)
  - Add index on CampaignImpression (CampaignId, RecordedAt)
  - Add index on CampaignClick (CampaignId, RecordedAt)
  - Create migration for indexes
  - _Requirements: All_

- [X] 36. Final Integration Test

  - Test all endpoints with real data
  - Test with FE AdPopup component
  - Verify all correctness properties
  - _Requirements: All_

- [X] 37. Final Checkpoint - Ensure all tests pass

  - Ensure all tests pass, ask the user if questions arise.

---

## Summary

This implementation plan consists of 37 tasks organized into 9 phases:

1. **Phase 1**: Database schema and entity setup (2 tasks)
2. **Phase 2**: Campaign management service and API (4 tasks)
3. **Phase 3**: Promotion management service and API (3 tasks)
4. **Phase 4**: Discount calculation and validation (3 tasks)
5. **Phase 5**: Voucher management service and API (5 tasks)
6. **Phase 6**: Analytics and tracking (7 tasks)
7. **Phase 7**: Error handling and response format (3 tasks)
8. **Phase 8**: Integration and validation (4 tasks)
9. **Phase 9**: Documentation and deployment (3 tasks)

**Total Core Tasks**: 24
**Total Optional Testing Tasks**: 13
**Total Checkpoints**: 7

Each task includes specific requirements references and builds incrementally on previous tasks. Testing is integrated throughout with property-based tests validating correctness properties and unit tests validating specific scenarios.

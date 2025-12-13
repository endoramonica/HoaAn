# Campaign & Promotion Management System - Requirements

## Introduction

This document specifies the requirements for a Campaign & Promotion Management System that enables administrators to create, manage, and track marketing campaigns with associated promotions and vouchers. The system supports multiple campaign types (modal popups, banners, vouchers), discount strategies, and targeting rules. The FE already has UI components for displaying campaigns (AdPopup with modal/banner types), and this spec defines the backend API contract needed to power these features.

## Glossary

- **Campaign**: A marketing initiative with a name, budget, status, and validity period. Contains one or more promotions.
- **Promotion**: A discount offer linked to a campaign. Contains discount value, applicable products, and validity rules.
- **PromotionProduct**: A junction entity linking promotions to specific products for targeted discounts.
- **Voucher**: A redeemable code (coupon) that applies a promotion's discount to orders.
- **Status**: Campaign lifecycle state (DRAFT, ACTIVE, PAUSED, COMPLETED, ARCHIVED).
- **Discount Value**: The amount or percentage reduction applied (e.g., 40% off, 100,000 VND off).
- **Budget**: Total allocated spend for a campaign (must be >= 0).
- **Validity Period**: Date range during which a campaign/promotion is active (EndDate > StartDate).
- **Targeting**: Rules determining which users/pages see a campaign (pages, frequency, delay).

## Requirements

### Requirement 1: Campaign Management

**User Story:** As an admin, I want to create and manage marketing campaigns, so that I can organize promotions and track their performance.

#### Acceptance Criteria

1. WHEN an admin creates a new campaign THEN the system SHALL accept CampaignName, Budget, StartDate, EndDate, and Status fields
2. WHEN a campaign is created with EndDate <= StartDate THEN the system SHALL reject the request with a 400 error
3. WHEN a campaign is created with Budget < 0 THEN the system SHALL reject the request with a 400 error
4. WHEN an admin updates a campaign THEN the system SHALL persist changes and return the updated campaign
5. WHEN an admin retrieves campaigns THEN the system SHALL return a paginated list with filtering by status and date range

### Requirement 2: Promotion Management

**User Story:** As an admin, I want to create promotions linked to campaigns, so that I can define specific discount offers.

#### Acceptance Criteria

1. WHEN a promotion is created for a DRAFT campaign THEN the system SHALL accept the promotion
2. WHEN a promotion is created for a non-DRAFT campaign THEN the system SHALL reject with a 409 error
3. WHEN a promotion is created THEN the system SHALL require DiscountValue, DiscountType (percentage/fixed), and CampaignId
4. WHEN a promotion's EndDate <= StartDate THEN the system SHALL reject the request with a 400 error
5. WHEN an admin retrieves promotions THEN the system SHALL return all promotions for a given campaign

### Requirement 3: Voucher Code Management

**User Story:** As an admin, I want to generate and manage voucher codes, so that customers can redeem discounts.

#### Acceptance Criteria

1. WHEN an admin generates voucher codes THEN the system SHALL create unique, reusable codes linked to a promotion
2. WHEN a voucher code is applied to a cart THEN the system SHALL validate the code exists and is active
3. WHEN a voucher code is applied THEN the system SHALL calculate and apply the discount to the cart total
4. WHEN a voucher code is invalid or expired THEN the system SHALL return a 400 error with a descriptive message
5. WHEN a voucher code reaches its usage limit THEN the system SHALL reject further applications with a 409 error

### Requirement 4: Campaign Targeting & Display Rules

**User Story:** As an admin, I want to configure targeting rules for campaigns, so that promotions reach the right audience.

#### Acceptance Criteria

1. WHEN a campaign is configured THEN the system SHALL accept targeting rules (pages, frequency, delayMs, autoDismissMs)
2. WHEN a campaign targets specific pages THEN the system SHALL only display it on those pages
3. WHEN a campaign has frequency='once-per-session' THEN the system SHALL show it only once per user session
4. WHEN a campaign has frequency='once-per-page' THEN the system SHALL show it only once per page per session
5. WHEN a campaign has frequency='always' THEN the system SHALL show it every time the targeting conditions are met

### Requirement 5: Discount Calculation & Validation

**User Story:** As a system, I want to accurately calculate discounts and validate promotion rules, so that pricing is correct.

#### Acceptance Criteria

1. WHEN a promotion with DiscountType='percentage' is applied THEN the system SHALL calculate discount as (originalPrice * discountPercentage / 100)
2. WHEN a promotion with DiscountType='fixed' is applied THEN the system SHALL deduct the fixed amount from the total
3. WHEN multiple promotions apply to an order THEN the system SHALL apply the highest discount (no stacking)
4. WHEN a promotion has a minimum order value THEN the system SHALL reject application if cart total < minimum
5. WHEN a promotion has maximum usage THEN the system SHALL track usage and reject when limit is reached

### Requirement 6: Campaign Status Lifecycle

**User Story:** As an admin, I want to manage campaign status transitions, so that campaigns follow a proper lifecycle.

#### Acceptance Criteria

1. WHEN a campaign is created THEN the system SHALL set its initial status to DRAFT
2. WHEN a campaign status is DRAFT THEN the system SHALL allow adding/removing promotions
3. WHEN a campaign status changes from DRAFT to ACTIVE THEN the system SHALL validate all required fields are set
4. WHEN a campaign is ACTIVE THEN the system SHALL prevent adding new promotions (only DRAFT allows this)
5. WHEN a campaign end date is reached THEN the system SHALL automatically transition status to COMPLETED

### Requirement 7: Campaign Analytics & Reporting

**User Story:** As an admin, I want to track campaign performance, so that I can measure ROI and optimize future campaigns.

#### Acceptance Criteria

1. WHEN a campaign is active THEN the system SHALL track impressions (times shown to users)
2. WHEN a user clicks a campaign CTA THEN the system SHALL record a click event
3. WHEN a voucher code is applied THEN the system SHALL record the redemption with timestamp and user info
4. WHEN an admin requests campaign stats THEN the system SHALL return impressions, clicks, redemptions, and revenue
5. WHEN an admin requests campaign stats THEN the system SHALL support filtering by date range and campaign status

### Requirement 8: API Response Format & Error Handling

**User Story:** As a frontend developer, I want consistent API responses and clear error messages, so that I can handle all scenarios properly.

#### Acceptance Criteria

1. WHEN an API request succeeds THEN the system SHALL return HTTP 200 with data in a consistent format
2. WHEN validation fails THEN the system SHALL return HTTP 400 with field-level error details
3. WHEN a resource is not found THEN the system SHALL return HTTP 404 with a descriptive message
4. WHEN a business rule is violated THEN the system SHALL return HTTP 409 with the specific conflict reason
5. WHEN an unauthorized request is made THEN the system SHALL return HTTP 401 or 403 as appropriate


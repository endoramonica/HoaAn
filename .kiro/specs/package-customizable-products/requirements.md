# Requirements Document: Package & Customizable Products

## Introduction

VietCommerce is an e-commerce platform specializing in Vietnamese ritual items and services (mâm cúng, lễ vật). This feature enables merchants to create and manage **package products** (fixed bundles) and **customizable products** (packages with optional add-ons that customers can modify). This is critical for the business model where customers often purchase pre-configured ritual packages but want flexibility to adjust quantities of specific items.

**Business Context:**
- Merchants create "Mâm Cúng Ông Công Ông Táo" (fixed package) - no customization
- Merchants create "Mâm Cúng Khai Trương" (package with options) - customers can increase/decrease xôi, chè, rượu quantities
- Customers add packages to cart with or without customizations
- System calculates final price: base price + customization surcharges
- Orders preserve customization details for fulfillment and analytics

## Glossary

- **Package Product**: A product that bundles multiple items together (e.g., ritual set with incense, candles, offerings)
- **Customizable Option**: An item within a package that customers can modify (quantity, add/remove)
- **Base Price**: The fixed price of the package before customizations
- **Customization Surcharge**: Additional cost from modifying options (e.g., adding extra xôi)
- **Snapshot**: Storing product/price data at order time to preserve historical accuracy
- **Customizable Products System**: The complete feature enabling package creation, customization, cart management, and order processing
- **Option**: A modifiable component of a package (e.g., "Xôi gấc đậu xanh" with min/max quantities)
- **Unit Price**: Price per unit of an option (e.g., 45,000đ per dĩa of xôi)

## Requirements

### Requirement 1: Create Package Products

**User Story:** As a merchant, I want to create package products with fixed items and optional customizable items, so that I can offer pre-configured ritual packages with flexibility for customer preferences.

#### Acceptance Criteria

1. WHEN a merchant submits a product creation form with package details THEN the system SHALL validate all required fields (name, base price, details list) and create the product with status active
   - **Validates:** Product creation with package structure
   - **Edge case:** Empty details list should be rejected

2. WHEN a merchant includes customizable options in the package THEN the system SHALL validate each option has id, name, baseQuantity, unitPrice, minQuantity, and unit, and store them as JSON
   - **Validates:** Option structure validation
   - **Edge case:** Missing required fields should be rejected

3. WHEN a merchant sets min/max quantities for an option THEN the system SHALL enforce that minQuantity ≤ maxQuantity (if maxQuantity exists) and minQuantity > 0
   - **Validates:** Quantity constraints
   - **Edge case:** Invalid ranges should be rejected

4. WHEN a merchant creates a package product THEN the system SHALL store details as JSON array and customizable options as JSON array in the Product entity
   - **Validates:** JSON serialization and storage
   - **Edge case:** Malformed JSON should be rejected

5. IF a merchant lacks product.create permission THEN the system SHALL deny the request and return 403 Forbidden
   - **Validates:** Permission-based access control

### Requirement 2: Display Package Products

**User Story:** As a customer, I want to view package product details including all included items and customizable options, so that I can understand what I'm purchasing and what I can modify.

#### Acceptance Criteria

1. WHEN a customer requests a product by ID THEN the system SHALL return product details including details array and customizable options array (if present)
   - **Validates:** Product retrieval with package data
   - **Edge case:** Product with null details/options should return empty arrays

2. WHEN a product has customizable options THEN the system SHALL display each option with name, baseQuantity, unitPrice, minQuantity, maxQuantity, and unit
   - **Validates:** Complete option information display
   - **Edge case:** Options with null maxQuantity should display as unlimited

3. WHEN displaying a package product THEN the system SHALL show base price and indicate which items are included vs. customizable
   - **Validates:** Clear distinction between fixed and customizable items
   - **Edge case:** Products with only fixed items should display clearly

4. WHEN a customer views product details THEN the system SHALL deserialize JSON fields without errors and return structured data
   - **Validates:** JSON deserialization reliability
   - **Edge case:** Corrupted JSON should be handled gracefully

### Requirement 3: Add Customizable Products to Cart

**User Story:** As a customer, I want to add a package product to my cart with custom quantities for optional items, so that I can purchase exactly what I need.

#### Acceptance Criteria

1. WHEN a customer adds a package product to cart with customizations THEN the system SHALL validate each customization against the product's options (id, quantity within min/max bounds)
   - **Validates:** Customization validation
   - **Edge case:** Invalid option IDs should be rejected

2. WHEN a customer specifies quantities for customizable options THEN the system SHALL validate quantity ≥ minQuantity and quantity ≤ maxQuantity (if set)
   - **Validates:** Quantity constraint enforcement
   - **Edge case:** Quantities outside bounds should be rejected

3. WHEN a customer adds a package to cart THEN the system SHALL calculate final price as: basePrice + sum(customization quantities × unit prices)
   - **Validates:** Price calculation accuracy
   - **Edge case:** Zero customizations should equal base price

4. WHEN a customer adds a package to cart THEN the system SHALL store customizations as JSON in CartItem and preserve basePrice, customizationPrice, and finalPrice
   - **Validates:** Cart item data persistence
   - **Edge case:** Null customizations should be handled

5. WHEN a customer lacks authentication THEN the system SHALL return 401 Unauthorized
   - **Validates:** Authentication requirement

### Requirement 4: Update Cart Items with Customizations

**User Story:** As a customer, I want to modify customization quantities for items already in my cart, so that I can adjust my order before checkout.

#### Acceptance Criteria

1. WHEN a customer updates customization quantities for a cart item THEN the system SHALL validate new quantities against option constraints and recalculate final price
   - **Validates:** Update validation and recalculation
   - **Edge case:** Invalid quantities should be rejected

2. WHEN a customer updates a cart item THEN the system SHALL update customizationsJson, customizationPrice, and finalPrice in the CartItem
   - **Validates:** Cart item update persistence
   - **Edge case:** Null updates should be handled

3. WHEN a customer updates customizations THEN the system SHALL return updated CartItemDetailDto with new prices and customization details
   - **Validates:** Response accuracy
   - **Edge case:** Concurrent updates should use optimistic concurrency

### Requirement 5: Process Orders with Customizations

**User Story:** As the system, I want to preserve customization details when converting cart items to order items, so that fulfillment teams have complete information and analytics can track customization patterns.

#### Acceptance Criteria

1. WHEN an order is created from cart items THEN the system SHALL snapshot customizations from CartItem into OrderItem as customizationsJson
   - **Validates:** Customization snapshot preservation
   - **Edge case:** Null customizations should be handled

2. WHEN an order is created THEN the system SHALL snapshot basePrice and customizationPrice from CartItem into OrderItem for historical accuracy
   - **Validates:** Price snapshot accuracy
   - **Edge case:** Price changes after cart addition should not affect order

3. WHEN an order item is created THEN the system SHALL store complete customization data (optionId, quantity, unitPrice) for fulfillment and analytics
   - **Validates:** Complete data preservation
   - **Edge case:** Missing customization data should be logged

### Requirement 6: Manage Package Product Options

**User Story:** As a merchant, I want to update package product options (add, remove, modify prices), so that I can adjust offerings based on inventory and market conditions.

#### Acceptance Criteria

1. WHEN a merchant updates a product's customizable options THEN the system SHALL validate the new options structure and update customizableOptionsJson
   - **Validates:** Option update validation
   - **Edge case:** Invalid structure should be rejected

2. WHEN a merchant changes an option's unit price THEN the system SHALL update only the Product entity, not affect existing orders (which have snapshots)
   - **Validates:** Price change isolation
   - **Edge case:** Concurrent updates should use optimistic concurrency

3. WHEN a merchant adds a new option to an existing package THEN the system SHALL validate the new option and append it to customizableOptionsJson
   - **Validates:** Option addition
   - **Edge case:** Duplicate option IDs should be rejected

4. WHEN a merchant removes an option THEN the system SHALL remove it from customizableOptionsJson and validate remaining options
   - **Validates:** Option removal
   - **Edge case:** Removing all options should be allowed

5. IF a merchant lacks product.update permission THEN the system SHALL deny the request and return 403 Forbidden
   - **Validates:** Permission-based access control

### Requirement 7: Validate Package Product Data Integrity

**User Story:** As the system, I want to ensure all package product data is valid and consistent, so that customers have reliable shopping experience and merchants can trust their data.

#### Acceptance Criteria

1. WHEN a product's customizableOptionsJson is deserialized THEN the system SHALL validate JSON structure matches CustomizableOptionDto schema
   - **Validates:** JSON schema compliance
   - **Edge case:** Malformed JSON should be rejected

2. WHEN validating customizable options THEN the system SHALL ensure all required fields are present (id, name, unitPrice, minQuantity) and have valid values
   - **Validates:** Field presence and validity
   - **Edge case:** Null or empty fields should be rejected

3. WHEN a customer's customization request is validated THEN the system SHALL verify option exists in product and quantity is within bounds
   - **Validates:** Customization-to-product mapping
   - **Edge case:** Stale option references should be rejected

4. WHEN storing JSON data THEN the system SHALL handle serialization errors gracefully and log them for debugging
   - **Validates:** Error handling
   - **Edge case:** Serialization failures should not crash the system

### Requirement 8: Display Cart Items with Customizations

**User Story:** As a customer, I want to see my cart items with all customization details and calculated prices, so that I can verify my order before checkout.

#### Acceptance Criteria

1. WHEN a customer views their cart THEN the system SHALL return CartItemDetailDto with customizations array showing each option, quantity, and price
   - **Validates:** Customization display
   - **Edge case:** Null customizations should display as empty array

2. WHEN displaying a cart item THEN the system SHALL show basePrice, customizationPrice, and finalPrice separately so customer understands the breakdown
   - **Validates:** Price transparency
   - **Edge case:** Zero customization price should display as 0

3. WHEN a cart item has customizations THEN the system SHALL deserialize customizationsJson and return structured CustomizationDto objects
   - **Validates:** JSON deserialization for display
   - **Edge case:** Corrupted JSON should be handled gracefully

### Requirement 9: Analytics - Track Customization Patterns

**User Story:** As a merchant, I want to analyze which customizable options are most popular, so that I can optimize package offerings and inventory planning.

#### Acceptance Criteria

1. WHEN analyzing order data THEN the system SHALL query OrderItem.customizationsJson and aggregate option usage across all orders
   - **Validates:** Customization data aggregation
   - **Edge case:** Null customizations should be skipped

2. WHEN generating analytics THEN the system SHALL calculate total quantity ordered for each option across all orders
   - **Validates:** Aggregation accuracy
   - **Edge case:** No orders should return empty results

3. WHEN displaying analytics THEN the system SHALL show option name, total quantity ordered, and percentage of orders containing that option
   - **Validates:** Analytics calculation
   - **Edge case:** Division by zero should be handled


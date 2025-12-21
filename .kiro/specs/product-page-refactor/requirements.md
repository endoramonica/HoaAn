# Requirements Document - Product Page Refactor

## Introduction

This feature refactors the Product page to properly handle product pricing through the existing ProductPrice entity with correct mapping and DTOs, and enhances the frontend UI to support direct image file uploads using the existing file upload service instead of URL-based image management.

## Glossary

- **Product Entity**: Core product information (name, code, slug, SKU, stock, etc.)
- **ProductPrice Entity**: Separate entity containing pricing information (price, priceType, effectiveFrom, effectiveTo, isActive)
- **ProductImage Entity**: Separate entity for product images (filePath, url, thumbnailUrl, displayOrder, isMain)
- **ProductFavorite Entity**: Intermediate table linking User and Product for favorites (composite key: UserId + ProductId)
- **ProductView Entity**: Log table for product views (tracks ProductId, UserId/SessionId, viewedAt)
- **ProductReview Entity**: Product reviews linked to OrderItem (ensures purchase verification)
- **File Upload Service**: Existing backend service that handles image file uploads and returns file paths/URLs
- **Product Page**: Frontend UI component displaying product details and allowing product management
- **Image Upload**: Frontend functionality allowing users to select and upload product image files directly

## Requirements

### Requirement 1

**User Story:** As a backend developer, I want the Product entity to have proper DTO mappings for prices and related entities, so that the API returns consistent and well-structured data.

#### Acceptance Criteria

1. WHEN a Product is queried via API THEN the system SHALL return ProductPrice entities with PriceType, Price, EffectiveFrom, EffectiveTo, and IsActive fields
2. WHEN a Product is queried THEN the system SHALL include ProductImage entities with FilePath, Url, ThumbnailUrl, DisplayOrder, and IsMain fields
3. WHEN a Product is queried THEN the system SHALL include ProductFavorite count and ProductReview count in the response
4. WHEN a ProductPrice is created THEN the system SHALL validate that EffectiveFrom is before EffectiveTo (if provided)
5. WHEN a ProductPrice is created THEN the system SHALL validate that Price is greater than zero

### Requirement 2

**User Story:** As a frontend developer, I want the Product page to support direct image file uploads, so that users can easily upload product images without manually entering URLs.

#### Acceptance Criteria

1. WHEN a user clicks the image upload button THEN the system SHALL display a file picker dialog accepting image files (jpg, png, gif, webp)
2. WHEN a user selects an image file THEN the system SHALL validate the file type and size (max 5MB)
3. WHEN a valid image is selected THEN the system SHALL call the existing file upload service
4. WHEN the file upload completes successfully THEN the system SHALL receive the file URL from the service and display the uploaded image
5. WHEN an image upload fails THEN the system SHALL display an error message to the user with the reason
6. WHEN images are displayed THEN the system SHALL show them in a draggable list to allow reordering
7. WHEN a user marks an image as main THEN the system SHALL set IsMain flag and update the display

### Requirement 3

**User Story:** As a product manager, I want the Product page to display and manage pricing information correctly, so that I can maintain accurate product pricing with multiple price types and effective dates.

#### Acceptance Criteria

1. WHEN the Product page loads THEN the system SHALL display all active prices associated with the product
2. WHEN a user adds a new price THEN the system SHALL validate the price data (type, amount, effective dates) and save it to the database
3. WHEN a user updates an existing price THEN the system SHALL persist the changes to the database
4. WHEN a user deletes a price THEN the system SHALL remove it from the product and database
5. WHEN prices are displayed THEN the system SHALL show them in a table format with PriceType, Price, EffectiveFrom, EffectiveTo, and IsActive columns
6. WHEN a price's EffectiveTo date passes THEN the system SHALL automatically mark it as inactive

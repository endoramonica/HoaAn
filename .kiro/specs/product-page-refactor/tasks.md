# Implementation Plan - Product Page Refactor

## Overview
This implementation plan breaks down the Product Page Refactor into discrete, manageable coding tasks. Each task builds incrementally on previous steps, starting with backend DTO enhancements, then frontend components, and finally property-based tests.

---

## Backend Implementation

- [x] 1. Enhance ProductDetailDto with Prices and Images





  - Update ProductDetailDto to include ProductPriceDto[] array
  - Update ProductDetailDto to include ProductImageDto[] array
  - Add FavoriteCount and ReviewCount properties
  - Update ProductDetailDto mapping in AutoMapper configuration
  - _Requirements: 1.1, 1.2, 1.3_

- [ ]* 1.1 Write property test for ProductDetailDto completeness
  - **Property 7: Product Detail DTO Completeness**
  - **Validates: Requirements 1.1, 1.2, 1.3**

- [x] 2. Create ProductPrice DTOs and Validation





  - Create ProductPriceCreateDto with validation (Price > 0, EffectiveFrom < EffectiveTo)
  - Create ProductPriceUpdateDto with same validation
  - Add validation attributes and custom validators
  - Update ProductService to handle price creation/update/delete
  - _Requirements: 1.4, 1.5_

- [ ]* 2.1 Write property test for price validation
  - **Property 1: Price Validation**
  - **Validates: Requirements 1.4, 1.5**


- [x] 3. Implement ProductPrice API Endpoints




  - Add POST /api/v1/products/{productId}/prices endpoint
  - Add PUT /api/v1/products/{productId}/prices/{priceId} endpoint
  - Add DELETE /api/v1/products/{productId}/prices/{priceId} endpoint
  - Add GET /api/v1/products/{productId}/prices endpoint
  - _Requirements: 1.1, 1.2_

- [ ]* 3.1 Write property test for price ordering
  - **Property 5: Price Effective Date Ordering**
  - **Validates: Requirements 3.1, 3.5**

- [x] 4. Implement ProductImage File Upload Endpoint





  - Create POST /api/v1/products/{productId}/images/upload endpoint
  - Accept multipart/form-data with file
  - Validate file type (jpg, png, gif, webp) and size (max 5MB)
  - Call existing file upload service to save file
  - Create ProductImage record with returned URL
  - Return ProductImageDto with uploaded image info
  - _Requirements: 2.3, 2.4_

- [ ]* 4.1 Write property test for file upload validation
  - **Property 8: Image Upload Validation**
  - **Validates: Requirements 2.2, 2.5**

- [x] 5. Implement ProductImage Management Endpoints




  - Add PUT /api/v1/products/{productId}/images/{imageId}/set-main endpoint
  - Add PUT /api/v1/products/{productId}/images/reorder endpoint (for drag-and-drop)
  - Add DELETE /api/v1/products/{productId}/images/{imageId} endpoint
  - Ensure only one image can be main per product
  - _Requirements: 2.7_

- [ ]* 5.1 Write property test for main image uniqueness
  - **Property 2: Image Main Flag Uniqueness**
  - **Validates: Requirements 2.7**

- [ ]* 5.2 Write property test for image display order
  - **Property 3: Image Display Order Consistency**
  - **Validates: Requirements 2.6**

- [ ] 6. Implement Automatic Price Inactivity
  - Add background job or scheduled task to mark expired prices as inactive
  - Query ProductPrices where EffectiveTo < DateTime.UtcNow and IsActive = true
  - Update IsActive to false for expired prices
  - _Requirements: 3.6_

- [ ]* 6.1 Write property test for automatic price inactivity
  - **Property 6: Active Price Consistency**
  - **Validates: Requirements 3.6**

- [ ] 7. Checkpoint - Ensure all backend tests pass
  - Ensure all tests pass, ask the user if questions arise.

---

## Frontend Implementation

- [ ] 8. Create useImageUpload Hook
  - Create hook for handling image file uploads
  - Accept file and productId as parameters
  - Validate file type and size before upload
  - Call POST /api/v1/products/{productId}/images/upload endpoint
  - Return uploaded image URL and handle errors
  - _Requirements: 2.2, 2.3, 2.4, 2.5_

- [ ]* 8.1 Write unit test for useImageUpload hook
  - Test file validation (type and size)
  - Test successful upload
  - Test error handling

- [ ] 9. Create ImageUploadComponent
  - Create component with file input and drag-and-drop support
  - Display file picker dialog for image selection
  - Show upload progress indicator
  - Display uploaded image preview
  - Handle upload errors with user-friendly messages
  - _Requirements: 2.1, 2.2, 2.5_

- [ ]* 9.1 Write unit test for ImageUploadComponent
  - Test file picker interaction
  - Test drag-and-drop functionality
  - Test error display

- [ ] 10. Enhance ProductImageModal
  - Replace URL input with ImageUploadComponent
  - Add image reordering with drag-and-drop
  - Add "Set as Main" button for each image
  - Update image list display with new features
  - Call reorder endpoint when images are reordered
  - Call set-main endpoint when main image is changed
  - _Requirements: 2.1, 2.6, 2.7_

- [ ]* 10.1 Write unit test for ProductImageModal
  - Test image upload integration
  - Test image reordering
  - Test set-main functionality

- [ ] 11. Create PriceManagementSection Component
  - Create component to display and manage product prices
  - Display table of existing prices (PriceType, Price, EffectiveFrom, EffectiveTo, IsActive)
  - Add form to create new price
  - Add edit/delete buttons for each price
  - Validate price data before submission
  - Call price API endpoints
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [ ]* 11.1 Write unit test for PriceManagementSection
  - Test price display
  - Test price creation
  - Test price update
  - Test price deletion

- [ ] 12. Enhance ProductFormModal
  - Integrate PriceManagementSection into form
  - Update form layout to accommodate price management
  - Ensure prices are saved when product is created/updated
  - _Requirements: 3.1, 3.2, 3.3, 3.4_

- [ ]* 12.1 Write unit test for ProductFormModal
  - Test price section integration
  - Test form submission with prices

- [ ] 13. Update ProductsPage
  - Update product table to display main image from ProductImage
  - Update product table to display price from ProductPrice (latest active price)
  - Ensure image and price modals work with new components
  - _Requirements: 2.1, 3.1_

- [ ]* 13.1 Write unit test for ProductsPage
  - Test product table rendering
  - Test modal interactions

- [ ] 14. Checkpoint - Ensure all frontend tests pass
  - Ensure all tests pass, ask the user if questions arise.

---

## Integration and Validation

- [ ] 15. End-to-End Testing
  - Test creating a product with prices and images
  - Test updating product prices
  - Test uploading and managing product images
  - Test price effective date transitions
  - Test image reordering and main image selection
  - _Requirements: All_

- [ ]* 15.1 Write property test for file upload round trip
  - **Property 4: File Upload Round Trip**
  - **Validates: Requirements 2.3, 2.4**

- [ ] 16. Final Checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

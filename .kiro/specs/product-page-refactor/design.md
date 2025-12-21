# Design Document - Product Page Refactor

## Overview

This design document outlines the refactoring of the Product page to properly utilize the existing ProductPrice entity with correct DTO mappings and enhance the frontend UI to support direct image file uploads. The system will leverage existing backend infrastructure while improving the user experience for product management.

## Architecture

### Backend Architecture

```
ProductController (API)
    ↓
IProductService (Application Layer)
    ↓
ProductRepository (Data Access)
    ↓
Product, ProductPrice, ProductImage Entities (Domain)
```

### Frontend Architecture

```
ProductsPage (Main Page)
    ├── ProductFormModal (Create/Edit)
    │   └── PriceManagementSection
    ├── ProductImageModal (Image Management)
    │   └── ImageUploadComponent
    └── ProductTable (Display)
```

## Components and Interfaces

### Backend Components

#### 1. ProductPrice Management
- **Entity**: ProductPrice (already exists)
  - ProductId (FK)
  - PriceType (enum: REGULAR, PROMOTIONAL, WHOLESALE, etc.)
  - Price (decimal)
  - EffectiveFrom (DateTime)
  - EffectiveTo (DateTime?)
  - IsActive (bool)

- **DTOs**:
  - ProductPriceDto (read)
  - ProductPriceCreateDto (create)
  - ProductPriceUpdateDto (update)

#### 2. ProductImage Management
- **Entity**: ProductImage (already exists)
  - ProductId (FK)
  - FilePath (string)
  - Url (string)
  - ThumbnailUrl (string?)
  - DisplayOrder (int)
  - IsMain (bool)
  - MediaType (enum)

- **DTOs**:
  - ProductImageDto (read)
  - ProductImageCreateDto (create)
  - UploadProductImageDto (for file upload)

#### 3. Product Detail DTO Enhancement
- Include ProductPriceDto[] in ProductDetailDto
- Include ProductImageDto[] in ProductDetailDto
- Include ProductFavoriteCount
- Include ProductReviewCount

### Frontend Components

#### 1. ProductFormModal Enhancement
- Add PriceManagementSection component
  - Display table of existing prices
  - Add new price form (PriceType, Price, EffectiveFrom, EffectiveTo)
  - Edit/Delete price functionality
  - Validation for price data

#### 2. ProductImageModal Enhancement
- Replace URL input with file upload
- Add file picker with validation (type, size)
- Add drag-and-drop support
- Add image reordering (drag to reorder)
- Add "Set as Main" button for each image
- Display upload progress

#### 3. Image Upload Service
- Create useImageUpload hook
- Call existing file upload endpoint
- Handle file validation
- Return uploaded image URL

## Data Models

### Backend Data Flow

```
ProductDetailDto {
  id: Guid
  name: string
  code: string
  slug: string
  price: decimal (main price)
  compareAtPrice: decimal?
  cost: decimal?
  
  // NEW: Prices array
  prices: ProductPriceDto[] {
    id: Guid
    priceType: PriceType
    price: decimal
    effectiveFrom: DateTime
    effectiveTo: DateTime?
    isActive: bool
  }
  
  // Images
  images: ProductImageDto[] {
    id: Guid
    url: string
    thumbnailUrl: string?
    displayOrder: int
    isMain: bool
  }
  
  // Stats
  favoriteCount: int
  reviewCount: int
  averageRating: decimal
  viewCount: int
  
  // Status
  isActive: bool
  isFeatured: bool
  
  // Audit
  createdAt: DateTime
  updatedAt: DateTime?
}
```

### Frontend State Management

```
ProductFormState {
  // Basic info
  name: string
  code: string
  sku: string
  
  // Prices
  prices: {
    id?: Guid
    priceType: string
    price: number
    effectiveFrom: Date
    effectiveTo?: Date
    isActive: boolean
  }[]
  
  // Images
  images: {
    id?: Guid
    url: string
    displayOrder: number
    isMain: boolean
    file?: File (for new uploads)
  }[]
  
  // Status
  isActive: boolean
  isFeatured: boolean
}
```

## Error Handling

### Backend Error Handling
- Validate ProductPrice data (price > 0, EffectiveFrom < EffectiveTo)
- Validate ProductImage file paths exist
- Return appropriate HTTP status codes (400, 404, 500)
- Log errors for debugging

### Frontend Error Handling
- File validation errors (type, size)
- Upload failures with retry option
- Network errors with user-friendly messages
- Form validation errors with field-level feedback
- Display toast notifications for all operations

## Testing Strategy

### Unit Testing
- Test ProductPrice validation logic
- Test ProductImage URL generation
- Test price calculation with multiple price types
- Test image ordering logic

### Property-Based Testing
- Test that prices are always sorted by EffectiveFrom date
- Test that only one price can be marked as main per product
- Test that file uploads preserve image metadata
- Test that price updates maintain data integrity

### Integration Testing
- Test product creation with prices and images
- Test product update with price changes
- Test image upload and retrieval
- Test price effective date transitions

## Correctness Properties

A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.

### Property 1: Price Validation
*For any* ProductPrice, the Price value SHALL be greater than zero and EffectiveFrom SHALL be before or equal to EffectiveTo (if provided).
**Validates: Requirements 1.4, 1.5**

### Property 2: Image Main Flag Uniqueness
*For any* Product, only one ProductImage SHALL have IsMain set to true at any given time.
**Validates: Requirements 2.7**

### Property 3: Image Display Order Consistency
*For any* Product, the DisplayOrder of ProductImages SHALL be unique and sequential starting from 0.
**Validates: Requirements 2.6**

### Property 4: File Upload Round Trip
*For any* image file uploaded through the file upload service, the returned URL SHALL be accessible and return the same file content.
**Validates: Requirements 2.3, 2.4**

### Property 5: Price Effective Date Ordering
*For any* Product, when querying ProductPrices, they SHALL be ordered by EffectiveFrom date in ascending order.
**Validates: Requirements 3.1, 3.5**

### Property 6: Active Price Consistency
*For any* ProductPrice with EffectiveTo date in the past, IsActive SHALL be automatically set to false.
**Validates: Requirements 3.6**

### Property 7: Product Detail DTO Completeness
*For any* Product queried via API, the returned ProductDetailDto SHALL include all associated ProductPrices, ProductImages, and statistics (favoriteCount, reviewCount).
**Validates: Requirements 1.1, 1.2, 1.3**

### Property 8: Image Upload Validation
*For any* image file upload request, if the file type is not in the allowed list (jpg, png, gif, webp) or size exceeds 5MB, the upload SHALL be rejected with an appropriate error message.
**Validates: Requirements 2.2, 2.5**

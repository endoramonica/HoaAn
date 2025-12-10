# Product Images API Response - Updated Structure

## Overview
Backend hiện tại đã được cập nhật để trả về **cả `primaryImage` + mảng `images` đầy đủ** cho mỗi sản phẩm/dịch vụ.

---

## 1. GET /api/v1/Product (List Products)

### Response Structure
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "00000000-0000-0000-0000-000000001009",
        "name": "Đèn nến thủy tinh hình sen",
        "code": "DN01",
        "slug": "den-nen-thuy-tinh-hinh-sen",
        "price": 650000,
        "compareAtPrice": 650000,
        "displayPrice": {
          "originalPrice": 650000,
          "discountedPrice": 650000,
          "discountAmount": 0
        },
        "stockQuantity": 120,
        "inStock": true,
        "categoryName": "Đèn dầu, nến & phụ kiện thờ cúng",
        
        // ✅ NEW: Primary image (ảnh chính)
        "primaryImage": "https://example.com/images/den-nen-thuy-tinh.jpg",
        
        // ✅ NEW: Full images array (mảng ảnh đầy đủ)
        "images": [
          {
            "id": "12D9C30B-5FC4-4E5A-8A44-01F5CB5F2B90",
            "productId": "00000000-0000-0000-0000-000000001009",
            "url": "https://example.com/images/den-nen-thuy-tinh.jpg",
            "thumbnailUrl": null,
            "displayOrder": 0,
            "mediaType": 0,
            "isMain": true,
            "createdAt": "2025-11-07T17:05:07.2333333",
            "updatedAt": null
          }
        ],
        
        "viewCount": 230,
        "favoriteCount": 38,
        "averageRating": 4.6,
        "isActive": true,
        "isFeatured": false,
        "createdAt": "0001-01-01T00:00:00",
        "type": "product"
      }
    ],
    "pageNumber": 1,
    "pageSize": 20,
    "totalItems": 22,
    "totalPages": 2
  },
  "message": "Products retrieved successfully"
}
```

---

## 2. GET /api/v1/Product/{id} (Product Detail)

### Response Structure
```json
{
  "success": true,
  "data": {
    "id": "11d63acd-24f3-4e85-b061-6494b62902bb",
    "name": "Lễ Tân Gia Trọn Gói Đầy Đủ",
    "code": "SVC-NEWHOUSE-001",
    "slug": "le-tan-gia-tron-goi-day-du",
    "price": 6500000,
    "compareAtPrice": 6500000,
    "displayPrice": {
      "originalPrice": 6500000,
      "discountedPrice": 6500000,
      "discountAmount": 0
    },
    "stockQuantity": 100,
    "sku": "SKU-SVC-NEWHOUSE-001",
    "categoryId": "00000000-0000-0000-0000-000000000015",
    
    // ✅ NEW: Primary image (ảnh chính)
    "primaryImage": "https://images.unsplash.com/photo-1560518883-ce09059eeffa?w=800",
    
    // ✅ NEW: Full images array (mảng ảnh đầy đủ, sắp xếp theo displayOrder)
    "images": [
      {
        "id": "43F33037-4793-47E9-A7A5-D3B3BFF0AF4D",
        "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
        "url": "https://images.unsplash.com/photo-1560518883-ce09059eeffa?w=800",
        "thumbnailUrl": "https://images.unsplash.com/photo-1560518883-ce09059eeffa?w=200",
        "displayOrder": 0,
        "mediaType": 0,
        "isMain": true,
        "createdAt": "2025-12-07T16:58:59.4300000",
        "updatedAt": null
      },
      {
        "id": "DC9B3A73-96EA-49B1-A3F6-8D9F2A3CC1D8",
        "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
        "url": "https://images.unsplash.com/photo-1582268611958-ebfd161ef9cf?w=800",
        "thumbnailUrl": "https://images.unsplash.com/photo-1582268611958-ebfd161ef9cf?w=200",
        "displayOrder": 1,
        "mediaType": 0,
        "isMain": false,
        "createdAt": "2025-12-07T16:58:59.4300000",
        "updatedAt": null
      },
      {
        "id": "693C8E30-5664-4B50-9F94-A5341074BCDD",
        "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
        "url": "https://images.unsplash.com/photo-1600210492486-724fe5c67fb0?w=800",
        "thumbnailUrl": "https://images.unsplash.com/photo-1600210492486-724fe5c67fb0?w=200",
        "displayOrder": 2,
        "mediaType": 0,
        "isMain": false,
        "createdAt": "2025-12-07T16:58:59.4300000",
        "updatedAt": null
      },
      {
        "id": "B3B37CE7-189D-4FBA-AE96-5ED81873C838",
        "productId": "11d63acd-24f3-4e85-b061-6494b62902bb",
        "url": "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?w=800",
        "thumbnailUrl": "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?w=200",
        "displayOrder": 3,
        "mediaType": 0,
        "isMain": false,
        "createdAt": "2025-12-07T16:58:59.4300000",
        "updatedAt": null
      }
    ],
    
    "tags": [],
    "isActive": true,
    "isFeatured": false,
    "viewCount": 98,
    "favoriteCount": 0,
    "averageRating": 4.5,
    "reviewCount": 0,
    "storeId": "47aa5519-c503-4cfa-8101-2edb36fd9d8c",
    "createdAt": "2025-12-07T04:14:58.3944708",
    "updatedAt": "2025-12-07T16:40:39.8233333",
    "createdBy": "00000000-0000-0000-0000-000000000000",
    "type": "service",
    "serviceCategory": "new-house",
    "serviceDuration": "3-4 giờ",
    "serviceRating": 4.7
  },
  "message": "Product retrieved successfully"
}
```

---

## 3. Image Object Structure

### ProductImageDto
```typescript
{
  id: string;                    // UUID của image record
  productId: string;             // UUID của product
  url: string;                   // URL ảnh full size
  thumbnailUrl?: string;         // URL ảnh thumbnail (nếu có)
  displayOrder: number;          // Thứ tự hiển thị (0, 1, 2, ...)
  mediaType: number;             // 0 = Image, 1 = Video, etc.
  isMain: boolean;               // true = ảnh chính, false = ảnh phụ
  createdAt: string;             // ISO datetime
  updatedAt?: string;            // ISO datetime (null nếu chưa update)
}
```

---

## 4. Frontend Implementation Guide

### 4.1 Hiển thị Gallery (Thumbnail + Main Image)

```typescript
// Lấy ảnh chính (IsMain = true hoặc ảnh đầu tiên)
const mainImage = product.images.find(img => img.isMain) || product.images[0];

// Lấy danh sách thumbnail
const thumbnails = product.images;

// Render
<div className="gallery">
  <div className="main-image">
    <img src={mainImage.url} alt={product.name} />
  </div>
  <div className="thumbnails">
    {thumbnails.map(img => (
      <img 
        key={img.id}
        src={img.thumbnailUrl || img.url}
        alt={`${product.name} - ${img.displayOrder}`}
        onClick={() => setMainImage(img)}
      />
    ))}
  </div>
</div>
```

### 4.2 Hiển thị Carousel Slideshow

```typescript
const [currentImageIndex, setCurrentImageIndex] = useState(0);

const handleNext = () => {
  setCurrentImageIndex((prev) => (prev + 1) % product.images.length);
};

const handlePrev = () => {
  setCurrentImageIndex((prev) => 
    prev === 0 ? product.images.length - 1 : prev - 1
  );
};

// Render
<div className="carousel">
  <img src={product.images[currentImageIndex].url} alt={product.name} />
  <button onClick={handlePrev}>← Prev</button>
  <button onClick={handleNext}>Next →</button>
  <div className="indicators">
    {product.images.map((_, idx) => (
      <span 
        key={idx}
        className={idx === currentImageIndex ? 'active' : ''}
        onClick={() => setCurrentImageIndex(idx)}
      />
    ))}
  </div>
</div>
```

### 4.3 Hiển thị Grid Ảnh

```typescript
<div className="image-grid">
  {product.images.map(img => (
    <div key={img.id} className="grid-item">
      <img src={img.thumbnailUrl || img.url} alt={product.name} />
      {img.isMain && <span className="badge">Main</span>}
    </div>
  ))}
</div>
```

---

## 5. Key Changes

| Aspect | Before | After |
|--------|--------|-------|
| **List API** | Chỉ `primaryImage` (string) | `primaryImage` + `images[]` (array) |
| **Detail API** | `images: []` (empty) | `images[]` (populated with full data) |
| **Image Info** | Chỉ URL | URL + Thumbnail + DisplayOrder + IsMain + Metadata |
| **Sorting** | N/A | Images sorted by `displayOrder` |
| **Primary Image** | Lấy từ `primaryImage` field | Lấy từ `images` array (IsMain=true) |

---

## 6. Database Schema (Reference)

```sql
-- ProductImages table
CREATE TABLE ProductImages (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    Url NVARCHAR(500) NOT NULL,
    ThumbnailUrl NVARCHAR(500),
    DisplayOrder INT DEFAULT 0,
    MediaType INT DEFAULT 0,
    IsMain BIT DEFAULT 0,
    CreatedAt DATETIME2,
    UpdatedAt DATETIME2,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- Index for performance
CREATE INDEX IX_ProductImages_ProductId_DisplayOrder 
ON ProductImages(ProductId, DisplayOrder);
```

---

## 7. Notes

- ✅ Images được sắp xếp theo `displayOrder` (tăng dần)
- ✅ Ảnh chính được đánh dấu `isMain = true`
- ✅ Nếu không có ảnh nào có `isMain = true`, ảnh đầu tiên (displayOrder=0) được coi là ảnh chính
- ✅ `thumbnailUrl` có thể null - frontend nên fallback sang `url`
- ✅ Hỗ trợ cả products và services (type = "product" hoặc "service")
- ✅ Cache được invalidate khi product/images thay đổi

---

## 8. Testing

### Test List API
```bash
curl -X GET "https://localhost:7131/api/v1/Product?pageNumber=1&pageSize=10"
```

### Test Detail API
```bash
curl -X GET "https://localhost:7131/api/v1/Product/11d63acd-24f3-4e85-b061-6494b62902bb"
```

### Expected: 
- `primaryImage` không null
- `images` array có ít nhất 1 phần tử
- Mỗi image có đầy đủ fields (id, url, thumbnailUrl, displayOrder, isMain, etc.)

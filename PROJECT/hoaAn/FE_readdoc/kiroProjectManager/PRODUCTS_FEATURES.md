# Tính Năng Quản Lý Sản Phẩm

## Các Endpoint Đã Tích Hợp

### 1. **GET /api/Product** - Lấy danh sách sản phẩm (có phân trang)
- ✅ Đã tích hợp
- Hỗ trợ các tham số:
  - `SearchTerm`: Tìm kiếm theo tên
  - `CategoryId`: Lọc theo danh mục
  - `StoreId`: Lọc theo cửa hàng
  - `IsActive`: Lọc theo trạng thái hoạt động
  - `IsFeatured`: Lọc theo sản phẩm nổi bật
  - `MinPrice`, `MaxPrice`: Lọc theo khoảng giá
  - `SortBy`, `IsDescending`: Sắp xếp
  - `Page`, `PageSize`: Phân trang

### 2. **POST /api/Product** - Tạo sản phẩm mới
- ✅ Đã tích hợp
- Modal form với các trường:
  - Tên sản phẩm (bắt buộc)
  - Mã sản phẩm (bắt buộc)
  - Giá bán (bắt buộc)
  - Giá so sánh, Giá vốn
  - SKU, Barcode
  - Số lượng kho
  - Mô tả ngắn, Mô tả chi tiết
  - Trạng thái: Hoạt động, Nổi bật

### 3. **PUT /api/Product/{id}** - Cập nhật sản phẩm
- ✅ Đã tích hợp
- Sử dụng cùng modal form với chế độ edit
- Tự động invalidate cache sau khi cập nhật

### 4. **DELETE /api/Product/{id}** - Xóa sản phẩm
- ✅ Đã tích hợp
- Modal xác nhận trước khi xóa
- Tự động refresh danh sách sau khi xóa

### 5. **GET /api/Product/{id}** - Lấy chi tiết sản phẩm
- ✅ Hook đã có sẵn: `useProduct(id)`
- Có thể sử dụng cho trang chi tiết sản phẩm

### 6. **PATCH /api/Product/{id}/stock** - Cập nhật số lượng kho
- ✅ Hook đã có sẵn: `useUpdateProductStock()`
- Có thể thêm UI để cập nhật nhanh số lượng kho

### 7. **PATCH /api/Product/{id}/active** - Bật/tắt trạng thái hoạt động
- ✅ Đã tích hợp
- Click vào badge trạng thái để toggle
- Tự động refresh sau khi thay đổi

### 8. **PATCH /api/Product/{id}/featured** - Đánh dấu/bỏ đánh dấu nổi bật
- ✅ Đã tích hợp
- Nút Star để toggle trạng thái nổi bật
- Hiển thị icon Star vàng cho sản phẩm nổi bật

### 9. **GET /api/Product/slug/{slug}** - Lấy sản phẩm theo slug
- ✅ Hook đã có sẵn: `useGetApiProductSlugSlug(slug)`
- Hữu ích cho trang public/customer

### 10. **GET /api/Product/store/{storeId}** - Lấy sản phẩm theo cửa hàng
- ✅ Hook đã có sẵn: `useProductsByStore(storeId)`
- Có thể sử dụng khi filter theo store

### 11. **GET /api/Product/category/{categoryId}** - Lấy sản phẩm theo danh mục
- ✅ Hook đã có sẵn: `useProductsByCategory(categoryId)`
- Có thể sử dụng khi filter theo category

### 12. **Cập nhật ảnh sản phẩm**
- ✅ Đã tích hợp
- Modal riêng để quản lý danh sách ảnh
- Thêm/xóa ảnh bằng URL
- Ảnh đầu tiên là ảnh chính (primaryImage)

## Tính Năng UI

### Danh Sách Sản Phẩm
- ✅ Hiển thị bảng với các cột: Ảnh, Tên, Giá, Kho, Trạng thái
- ✅ Tìm kiếm theo tên sản phẩm
- ✅ Bộ lọc nâng cao:
  - Trạng thái (Hoạt động/Ngừng)
  - Nổi bật (Có/Không)
  - Khoảng giá (Min/Max)
- ✅ Format giá theo VND
- ✅ Hiển thị icon Star cho sản phẩm nổi bật

### Thao Tác
- ✅ Nút "Thêm Sản Phẩm" - Mở modal tạo mới
- ✅ Nút "Sửa" - Mở modal chỉnh sửa
- ✅ Nút "Ảnh" - Mở modal quản lý ảnh
- ✅ Nút "Star" - Toggle trạng thái nổi bật
- ✅ Nút "Xóa" - Xóa sản phẩm (có xác nhận)
- ✅ Click vào badge trạng thái - Toggle hoạt động/ngừng

### Modal Forms
- ✅ **ProductFormModal**: Thêm/sửa thông tin sản phẩm
  - Validation: Tên, mã, giá bắt buộc
  - Grid layout cho các trường
  - Checkbox cho trạng thái
  - Textarea cho mô tả chi tiết
  
- ✅ **ProductImageModal**: Quản lý ảnh sản phẩm
  - Thêm ảnh bằng URL
  - Preview ảnh
  - Xóa ảnh
  - Hiển thị ảnh chính

## Hooks Đã Tạo

```typescript
// Queries
useProducts(params)              // Lấy danh sách có phân trang + filter
useProduct(id)                   // Lấy chi tiết 1 sản phẩm
useProductsByStore(storeId)      // Lấy theo store
useProductsByCategory(categoryId) // Lấy theo category

// Mutations
useCreateProduct()               // Tạo mới
useUpdateProduct()               // Cập nhật
useDeleteProduct()               // Xóa
useUpdateProductStock()          // Cập nhật số lượng kho
useToggleProductActive()         // Bật/tắt hoạt động
useToggleProductFeatured()       // Đánh dấu nổi bật
```

## Tính Năng Có Thể Mở Rộng

### Chưa Tích Hợp UI (nhưng đã có hook)
1. **Cập nhật nhanh số lượng kho** - Có thể thêm input inline trong bảng
2. **Lọc theo Category** - Có thể thêm dropdown category trong filter
3. **Lọc theo Store** - Có thể thêm dropdown store trong filter
4. **Sắp xếp** - Có thể thêm sort cho các cột
5. **Phân trang** - Có thể thêm pagination controls

### Các Endpoint Khác (nếu có trong API)
- POST /api/Product/{id}/view - Tăng lượt xem
- POST /api/Product/{id}/favorite - Thêm vào yêu thích
- GET /api/Product/favorites - Lấy danh sách yêu thích

## Cache Management

Tất cả các mutations đều tự động invalidate cache:
- Sau khi tạo/sửa/xóa → Refresh danh sách
- Sau khi cập nhật → Refresh chi tiết + danh sách
- Đảm bảo dữ liệu luôn đồng bộ

## Validation

### Client-side
- Tên sản phẩm: Bắt buộc
- Mã sản phẩm: Bắt buộc (chỉ khi tạo mới)
- Giá: Phải > 0

### Server-side (theo schema)
- Tên: 3-200 ký tự
- Mã: Tối đa 50 ký tự
- Mô tả ngắn: Tối đa 500 ký tự
- Mô tả: Tối đa 5000 ký tự
- Giá: >= 0
- Số lượng kho: 0-2147483647

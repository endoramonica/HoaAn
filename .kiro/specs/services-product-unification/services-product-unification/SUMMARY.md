# Tóm Tắt: Services-Product Unification

## 🎯 Mục Tiêu

Thống nhất Services và Products bằng cách mở rộng **2 models**:

### 1. Product Model (ServicesPage)
**Mục đích**: User browse và mua services

**Cần làm**:
- Backend: Thêm `type='service'` vào Product model
- Backend: API `GET /api/v1/Product?type=service`
- Frontend: ServicesPage fetch từ Product API
- Frontend: Add to cart, wishlist

### 2. Order Model (ProfilePage)
**Mục đích**: User xem lịch sử orders

**Cần làm**:
- Backend: Thêm `type='product'|'service'` vào Order model
- Backend: API `GET /api/v1/Order/my-orders?type=service`
- Frontend: ProfilePage Orders Tab fetch product orders
- Frontend: ProfilePage Services Tab fetch service orders

---

## 📊 Hiện Tại vs Mục Tiêu

### Hiện Tại
```
ServicesPage: Mock data ❌
ProfilePage Orders Tab: Mock data ❌
ProfilePage Services Tab: localStorage ❌
```

### Mục Tiêu
```
ServicesPage: GET /api/v1/Product?type=service ✅
ProfilePage Orders Tab: GET /api/v1/Order/my-orders?type=product ✅
ProfilePage Services Tab: GET /api/v1/Order/my-orders?type=service ✅
```

---

## 🔧 Backend Cần Làm

### Product Model
- [ ] Thêm `Type` field (product | service)
- [ ] Thêm `ServiceCategory` field
- [ ] Thêm `ServiceDuration` field
- [ ] Thêm `Rating` field
- [ ] Modify `GET /api/v1/Product` để support type filter

### Order Model
- [ ] Thêm `Type` field (product | service)
- [ ] Thêm `ServiceCategory`, `ServiceDuration`, `ServiceLocation`, `ServiceDate`, `ServiceTime`, `ServiceNotes` fields
- [ ] Modify `GET /api/v1/Order/my-orders` để support type filter

---

## 💻 Frontend Cần Làm

### ServicesPage (Product Model)
- [ ] Create `useServices()` hook
- [ ] Fetch từ `GET /api/v1/Product?type=service`
- [ ] Add to cart, wishlist functionality

### ProfilePage (Order Model)
- [ ] Create `useOrders()` hook
- [ ] Create `useServiceOrders()` hook
- [ ] Orders Tab: Fetch từ `GET /api/v1/Order/my-orders?type=product`
- [ ] Services Tab: Fetch từ `GET /api/v1/Order/my-orders?type=service`
- [ ] Replace mock data và localStorage

---

## ⏱️ Timeline

- **Backend**: 3-4 days
- **Frontend**: 5-7 days
- **Total**: ~10 days

---

## 📁 Files Quan Trọng

1. **requirements.md** - Requirements chi tiết
2. **design.md** - Architecture và design
3. **tasks.md** - Task list
4. **API_REQUIREMENTS.md** - Hướng dẫn cho Backend
5. **BACKEND_QUESTIONS.md** - Câu hỏi cho Backend


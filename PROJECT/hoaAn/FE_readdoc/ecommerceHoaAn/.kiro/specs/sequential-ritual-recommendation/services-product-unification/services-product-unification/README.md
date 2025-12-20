# Services-Product Unification Spec

## 📋 Tổng Quan

Thống nhất Services và Products thành một hệ thống duy nhất bằng cách:

1. **Product Model**: Thêm `type='service'` để ServicesPage có thể browse và mua services
2. **Order Model**: Thêm `type='service'` để ProfilePage có thể xem lịch sử service orders

---

## 🎯 Status

**Backend**: ✅ **COMPLETE** (December 6, 2025)  
**Frontend**: 🔄 **READY FOR INTEGRATION**

---

## 📁 Documentation Files

### For Backend Team
| File | Purpose |
|------|---------|
| ✅ **BACKEND_IMPLEMENTATION_COMPLETE.md** | Full implementation details, API examples, testing guide |
| **API_REQUIREMENTS.md** | Original API specifications |
| **BACKEND_SPEC.md** | Technical specifications |
| **BACKEND_QUESTIONS.md** | Q&A for backend team |

### For Frontend Team
| File | Purpose |
|------|---------|
| 🚀 **FRONTEND_INTEGRATION_GUIDE.md** | Step-by-step integration guide with code examples |
| **requirements.md** | Requirements (EARS format) |
| **design.md** | Architecture and design |
| **tasks.md** | Implementation task list |

### Summary
| File | Purpose |
|------|---------|
| 📊 **IMPLEMENTATION_SUMMARY.md** | Quick overview of what was done |
| **README.md** | This file |

---

## 🚀 Quick Start

### Backend Team (DONE ✅)

1. ✅ Run migration:
```bash
dotnet ef database update --startup-project ../BE
```

2. ✅ Verify API endpoints:
```bash
GET /api/Product?type=service
GET /api/Order/my-orders?type=service
```

### Frontend Team (TODO 🔄)

1. **Read**: `FRONTEND_INTEGRATION_GUIDE.md`
2. **Update Types**: Add new fields to TypeScript interfaces
3. **Create Hooks**: `useServices()`, `useOrders()`, `useServiceOrders()`
4. **Update Pages**: ServicesPage, ProfilePage
5. **Test**: Verify integration works
6. **Cleanup**: Remove mock data and localStorage

---

## 🔑 Key Changes

### Product Model
- ✅ Added `type` field ("product" | "service")
- ✅ Added `serviceCategory` field
- ✅ Added `serviceDuration` field
- ✅ Added `rating` field
- ✅ API: `GET /api/Product?type=service&serviceCategory=ancestor-worship`

### Order Model
- ✅ Added `type` field ("product" | "service")
- ✅ Added 6 service-specific fields (category, duration, location, date, time, notes)
- ✅ API: `GET /api/Order/my-orders?type=service&serviceCategory=ancestor-worship`

### Backward Compatibility
- ✅ All existing products have `Type='product'`
- ✅ All existing orders have `Type='product'`
- ✅ API works without type filter (returns all)
- ✅ No breaking changes

---

## 📞 Next Steps

### For Frontend Team

**Start Here**: Read `FRONTEND_INTEGRATION_GUIDE.md`

**Quick Checklist**:
- [ ] Update TypeScript interfaces (ProductListDto, OrderDetailDto, etc.)
- [ ] Update ProductService to pass `type` and `serviceCategory`
- [ ] Update OrderService to pass `type` and `serviceCategory`
- [ ] Create `useServices()` hook
- [ ] Create `useOrders()` hook
- [ ] Create `useServiceOrders()` hook
- [ ] Update ServicesPage to use `useServices()`
- [ ] Update ProfilePage Orders Tab to use `useOrders({ type: 'product' })`
- [ ] Update ProfilePage Services Tab to use `useServiceOrders()`
- [ ] Test all scenarios
- [ ] Remove mock data
- [ ] Remove localStorage usage

**Estimated Time**: 5-7 days

---

## 🎉 Summary

Backend đã hoàn thành việc mở rộng Product Model và Order Model để hỗ trợ Services. 

**API Endpoints Ready**:
- ✅ `GET /api/Product?type=service` - Get services
- ✅ `GET /api/Order/my-orders?type=service` - Get service orders

**Frontend can now**:
- Fetch services from Product API
- Display services on ServicesPage
- Fetch service orders from Order API
- Display service orders on ProfilePage

**All changes are backward compatible** - existing functionality continues to work without any modifications.

---

## 📚 Service Categories

| Value | Label |
|-------|-------|
| `ancestor-worship` | Cúng Gia Tiên |
| `opening-ceremony` | Lễ Khai Trương |
| `wedding` | Lễ Cưới Hỏi |
| `buddha-worship` | Cúng Phật |
| `new-house` | Lễ Tân Gia |
| `feng-shui-consultation` | Tư Vấn Phong Thủy |

---

## 📞 Contact

**Questions?**
- Backend: Check `BACKEND_IMPLEMENTATION_COMPLETE.md`
- Frontend: Check `FRONTEND_INTEGRATION_GUIDE.md`
- Overview: Check `IMPLEMENTATION_SUMMARY.md`

Ready to integrate! 🚀


# Booking Feature - Integration Checklist

## ✅ Frontend Integration

### 1. Setup & Dependencies
- [x] Create `src/lib/services/lunarDateService.ts`
- [x] Create `src/lib/services/bookingService.ts`
- [x] Create `src/pages/booking/CalendarBookingPage.tsx`
- [ ] Verify axios is installed in `package.json`
- [ ] Verify react-router-dom is installed

### 2. Add Route
```typescript
// src/App.tsx
import { CalendarBookingPage } from './pages/booking/CalendarBookingPage';

// In your router configuration:
<Route path="/booking/calendar" element={<CalendarBookingPage />} />
```

### 3. Add Navigation Link
```typescript
// Add link to CalendarBookingPage from relevant pages
<Link to="/booking/calendar">Đặt dịch vụ theo lịch âm</Link>
```

### 4. Test Frontend
- [ ] Navigate to `/booking/calendar`
- [ ] Select a date on calendar
- [ ] Verify lunar date conversion works
- [ ] Fill in customer info form
- [ ] Verify form validation
- [ ] Test error handling

### 5. Environment Variables
```env
# .env
VITE_API_URL=https://localhost:7131
```

---

## ✅ Backend Integration

### 1. Create DTOs
- [ ] Create `Models/LunarDateInfo.cs`
- [ ] Create `Models/Requests/BookingCreateRequest.cs`
- [ ] Create `Models/Responses/BookingResponse.cs`

### 2. Create Entity
- [ ] Create `Models/Entities/Booking.cs`
- [ ] Add DbSet to `ApplicationDbContext.cs`:
```csharp
public DbSet<Booking> Bookings { get; set; }
```

### 3. Create Repository
- [ ] Create `Repositories/Interfaces/IBookingRepository.cs`
- [ ] Create `Repositories/Implementations/BookingRepository.cs`

### 4. Create Service
- [ ] Create `Services/Interfaces/IBookingService.cs`
- [ ] Create `Services/Implementations/BookingService.cs`

### 5. Create Controller
- [ ] Create `Controllers/BookingController.cs`
- [ ] Add endpoints:
  - `POST /api/v1/Booking/create`
  - `GET /api/v1/Booking/{bookingId}`
  - `GET /api/v1/Booking/customer/{customerId}`
  - `GET /api/v1/Booking/date-range`
  - `POST /api/v1/Booking/{bookingId}/cancel`

### 6. Dependency Injection
```csharp
// Program.cs or Startup.cs
services.AddScoped<IBookingRepository, BookingRepository>();
services.AddScoped<IBookingService, BookingService>();
```

### 7. Database Migration
```bash
# Create migration
dotnet ef migrations add AddBookingTable

# Apply migration
dotnet ef database update
```

### 8. Test Backend
- [ ] Test POST /api/v1/Booking/create with valid data
- [ ] Test validation errors
- [ ] Test authentication
- [ ] Test database persistence
- [ ] Test GET endpoints

---

## ✅ API Integration

### 1. Update Swagger/OpenAPI
- [ ] Add BookingController to swagger.json
- [ ] Document all endpoints
- [ ] Include request/response examples

### 2. Regenerate API Client
```bash
npm run api:generate
```

### 3. Verify Generated Client
- [ ] Check `Api/generated-orval/index.ts` for booking functions
- [ ] Verify DTOs are generated correctly

---

## ✅ Testing

### Frontend Tests
- [ ] Test calendar date selection
- [ ] Test lunar date conversion API call
- [ ] Test form validation
- [ ] Test booking submission
- [ ] Test error handling
- [ ] Test success message
- [ ] Test responsive design on mobile

### Backend Tests
- [ ] Unit tests for BookingService
- [ ] Integration tests for BookingController
- [ ] Test validation logic
- [ ] Test database operations
- [ ] Test error responses

### E2E Tests
- [ ] Complete booking flow from start to finish
- [ ] Test with different dates
- [ ] Test with different services
- [ ] Test error scenarios

---

## ✅ Security Checklist

### Frontend Security
- [ ] Input validation on all fields
- [ ] XSS prevention (sanitize user input)
- [ ] CSRF protection (if applicable)
- [ ] Secure token storage
- [ ] HTTPS only in production

### Backend Security
- [ ] Authentication required on all endpoints
- [ ] Authorization checks
- [ ] CustomerId validation (fixed ID)
- [ ] Input validation on all fields
- [ ] SQL injection prevention
- [ ] Rate limiting
- [ ] Audit logging

### Data Protection
- [ ] HTTPS/TLS encryption
- [ ] Database encryption at rest
- [ ] PII protection
- [ ] Secure error messages (no sensitive info)

---

## ✅ Deployment

### Pre-Deployment
- [ ] All tests passing
- [ ] Code review completed
- [ ] No console errors/warnings
- [ ] Performance optimized
- [ ] Security audit passed

### Frontend Deployment
- [ ] Build: `npm run build`
- [ ] Test build: `npm run preview`
- [ ] Deploy to hosting (Vercel, Netlify, etc.)
- [ ] Verify routes work
- [ ] Test API calls in production

### Backend Deployment
- [ ] Build: `dotnet build`
- [ ] Run tests: `dotnet test`
- [ ] Publish: `dotnet publish -c Release`
- [ ] Deploy to server
- [ ] Run migrations: `dotnet ef database update`
- [ ] Verify endpoints work
- [ ] Check logs for errors

### Post-Deployment
- [ ] Monitor error logs
- [ ] Test all endpoints
- [ ] Verify database connectivity
- [ ] Check performance metrics
- [ ] Monitor user feedback

---

## 📋 File Checklist

### Frontend Files
- [x] `src/lib/services/lunarDateService.ts`
- [x] `src/lib/services/bookingService.ts`
- [x] `src/pages/booking/CalendarBookingPage.tsx`
- [ ] Update `src/App.tsx` with route
- [ ] Update navigation components

### Backend Files
- [ ] `Models/LunarDateInfo.cs`
- [ ] `Models/Requests/BookingCreateRequest.cs`
- [ ] `Models/Responses/BookingResponse.cs`
- [ ] `Models/Entities/Booking.cs`
- [ ] `Repositories/Interfaces/IBookingRepository.cs`
- [ ] `Repositories/Implementations/BookingRepository.cs`
- [ ] `Services/Interfaces/IBookingService.cs`
- [ ] `Services/Implementations/BookingService.cs`
- [ ] `Controllers/BookingController.cs`
- [ ] Database migration file

### Documentation Files
- [x] `BOOKING_FEATURE_GUIDE.md`
- [x] `BOOKING_BACKEND_IMPLEMENTATION.md`
- [x] `BOOKING_FLOW_DIAGRAM.md`
- [x] `BOOKING_INTEGRATION_CHECKLIST.md`

---

## 🔗 Related Files

- `swagger.json` - Update with BookingController endpoints
- `orval.config.js` - Regenerate API client
- `package.json` - Verify dependencies
- `tsconfig.json` - Verify TypeScript config
- `vite.config.ts` - Verify Vite config
- `ApplicationDbContext.cs` - Add Bookings DbSet

---

## 📞 Troubleshooting

### Frontend Issues

**Issue: "Cannot find module 'lunarDateService'"**
- Solution: Verify file path is correct
- Check: `src/lib/services/lunarDateService.ts` exists

**Issue: "Lunar date conversion fails"**
- Solution: Check network connectivity
- Verify: API endpoint `https://open.oapi.vn/date/convert-to-lunar` is accessible
- Check: Request format is correct

**Issue: "Booking submission fails"**
- Solution: Check authentication token
- Verify: Backend endpoint is accessible
- Check: Request payload format matches DTO

### Backend Issues

**Issue: "DbSet 'Bookings' not found"**
- Solution: Add to ApplicationDbContext:
```csharp
public DbSet<Booking> Bookings { get; set; }
```

**Issue: "Migration fails"**
- Solution: Check database connection string
- Verify: SQL Server is running
- Run: `dotnet ef migrations remove` and try again

**Issue: "Endpoint returns 401 Unauthorized"**
- Solution: Verify authentication is configured
- Check: Bearer token is valid
- Verify: User has required permissions

---

## 📊 Performance Optimization

### Frontend
- [ ] Lazy load CalendarBookingPage component
- [ ] Memoize calendar grid rendering
- [ ] Debounce form input validation
- [ ] Cache lunar date conversions

### Backend
- [ ] Add database indexes on frequently queried columns
- [ ] Implement caching for lunar date conversions
- [ ] Use async/await for I/O operations
- [ ] Implement pagination for list endpoints

---

## 📈 Monitoring & Analytics

### Frontend Monitoring
- [ ] Track page views
- [ ] Track booking submissions
- [ ] Track error rates
- [ ] Monitor API response times

### Backend Monitoring
- [ ] Log all API requests
- [ ] Track error rates
- [ ] Monitor database performance
- [ ] Alert on failures

---

## 🎯 Success Criteria

- [x] Frontend page created and styled
- [x] Lunar date conversion integrated
- [x] Booking form with validation
- [x] Backend DTOs and entities created
- [x] API endpoints implemented
- [x] Database schema designed
- [ ] All tests passing
- [ ] Security audit passed
- [ ] Performance optimized
- [ ] Documentation complete
- [ ] Deployed to production
- [ ] User feedback positive

---

## 📝 Notes

- CustomerId is fixed: `D93A557C-A41E-4DD9-A0A7-D6B3897BC4A7`
- Backend does NOT calculate lunar dates, only stores them
- Lunar date conversion is done by external API: `https://open.oapi.vn/date/convert-to-lunar`
- All dates are stored in UTC timezone
- Phone validation: 10-11 digits
- Email validation: standard email format

---

## 🚀 Next Steps

1. **Immediate**: Implement backend DTOs and entities
2. **Short-term**: Create repository and service layers
3. **Medium-term**: Implement controller and endpoints
4. **Long-term**: Add advanced features (notifications, reminders, etc.)

---

## 📞 Support

For questions or issues, contact the development team.

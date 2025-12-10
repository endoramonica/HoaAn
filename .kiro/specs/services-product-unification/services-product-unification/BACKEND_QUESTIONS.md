# Questions for Backend Team

## 🔍 Database & Model Questions

### 1. Order Model
- [ ] Can we add `Type` field to Order model? (product | service)
- [ ] Can we add `ServiceCategory` field to Order model?
- [ ] Can we add `ServiceDuration`, `ServiceLocation`, `ServiceDate`, `ServiceTime`, `ServiceNotes` fields?
- [ ] Should these fields be nullable or required?
- [ ] What's the current Order model structure?

### 2. OrderItem Model
- [ ] Can we add `Type` field to OrderItem model?
- [ ] Can we add `ServiceCategory` and `ServiceDuration` fields?
- [ ] Should these fields be nullable?

### 3. Database Migration
- [ ] Can you create a migration to add these fields?
- [ ] Should we set `Type='product'` for all existing orders?
- [ ] Do we need to create indexes on `Type` and `ServiceCategory`?

---

## 🔌 API Endpoint Questions

### 1. GET /api/v1/Order/my-orders
- [ ] Can we add `type` query parameter to filter by product/service?
- [ ] Can we add `serviceCategory` query parameter?
- [ ] Can we add `status` query parameter?
- [ ] Can we add `startDate` and `endDate` query parameters?
- [ ] Can we add `searchTerm` query parameter?
- [ ] Can we add `sortBy` and `isDescending` query parameters?
- [ ] What's the current response structure?

### 2. New Endpoints (Optional)
- [ ] Should we create `GET /api/v1/Order/my-orders/services` endpoint?
- [ ] Should we create `GET /api/v1/Order/my-orders/products` endpoint?
- [ ] Or should we just use query parameters on existing endpoint?

### 3. POST /api/v1/Order (Checkout)
- [ ] Does the checkout endpoint support service items?
- [ ] Can we pass `serviceCategory`, `serviceDate`, `serviceTime`, `serviceLocation` in the request?
- [ ] How should we structure the request for service items?

---

## 📊 DTO Questions

### 1. OrderDetailDto
- [ ] Can we add these fields to OrderDetailDto?
  - `Type` (string: product | service)
  - `ServiceCategory` (string)
  - `ServiceDuration` (string)
  - `ServiceLocation` (string)
  - `ServiceDate` (DateTime)
  - `ServiceTime` (string)
  - `ServiceNotes` (string)

### 2. OrderItemDTO
- [ ] Can we add these fields to OrderItemDTO?
  - `Type` (string: product | service)
  - `ServiceCategory` (string)
  - `ServiceDuration` (string)

### 3. New DTOs
- [ ] Can we create `OrderFilterDto` with these fields?
  - `PageNumber` (int)
  - `PageSize` (int)
  - `Type` (string)
  - `ServiceCategory` (string)
  - `Status` (string)
  - `StartDate` (DateTime)
  - `EndDate` (DateTime)
  - `SearchTerm` (string)
  - `SortBy` (string)
  - `IsDescending` (bool)

---

## 🔄 Data Consistency Questions

### 1. Existing Data
- [ ] How many existing orders do we have?
- [ ] Should we migrate all existing orders to have `Type='product'`?
- [ ] Are there any service bookings in the database that should be converted to orders?

### 2. Validation
- [ ] Should `ServiceCategory` be required when `Type='service'`?
- [ ] Should `ServiceDate` and `ServiceTime` be required when `Type='service'`?
- [ ] Should `ShippingAddress` be required when `Type='product'`?
- [ ] What are the valid values for `ServiceCategory`?

### 3. Backward Compatibility
- [ ] Will existing API clients break if we add new fields?
- [ ] Should we make new fields optional in the response?
- [ ] Should we version the API?

---

## 📈 Performance Questions

### 1. Indexing
- [ ] Should we create an index on `Type` column?
- [ ] Should we create an index on `ServiceCategory` column?
- [ ] Should we create a composite index on `Type` + `ServiceCategory`?

### 2. Pagination
- [ ] What's the recommended page size?
- [ ] Should we have a maximum page size limit?
- [ ] How should we handle very large result sets?

### 3. Caching
- [ ] Should we cache order lists?
- [ ] How long should the cache be valid?
- [ ] Should we invalidate cache when new orders are created?

---

## 🧪 Testing Questions

### 1. Test Data
- [ ] Can you create test data with both product and service orders?
- [ ] Can you create test data with different service categories?
- [ ] Can you create test data with different order statuses?

### 2. API Testing
- [ ] Can you test the new endpoints with Postman/Insomnia?
- [ ] Can you provide example requests and responses?
- [ ] Can you test error scenarios (invalid filters, etc.)?

### 3. Documentation
- [ ] Can you update the Swagger/OpenAPI documentation?
- [ ] Can you provide API documentation for the new endpoints?
- [ ] Can you provide example requests and responses?

---

## 🚀 Timeline Questions

### 1. Implementation
- [ ] When can you start working on this?
- [ ] How long will it take to implement?
- [ ] Do you need any clarification on the requirements?

### 2. Testing
- [ ] When will the endpoints be ready for testing?
- [ ] Can we do integration testing before full release?
- [ ] What's the testing timeline?

### 3. Deployment
- [ ] When can we deploy to production?
- [ ] Do we need to do a database migration?
- [ ] Do we need to do a staged rollout?

---

## 📋 Checklist for Backend Team

- [ ] Review requirements document
- [ ] Review API requirements document
- [ ] Clarify any questions
- [ ] Design database schema changes
- [ ] Create database migration
- [ ] Update DTOs
- [ ] Update API endpoints
- [ ] Add validation
- [ ] Update Swagger documentation
- [ ] Write unit tests
- [ ] Write integration tests
- [ ] Test with Postman/Insomnia
- [ ] Provide test data
- [ ] Provide example requests/responses
- [ ] Ready for frontend integration

---

## 📞 Communication

### Preferred Format
- [ ] Email
- [ ] Slack
- [ ] Meeting
- [ ] GitHub Issues

### Frequency
- [ ] Daily standup
- [ ] Weekly sync
- [ ] As needed

### Point of Contact
- Name: _______________
- Email: _______________
- Slack: _______________

---

## 📎 Attachments

- Requirements document: `.kiro/specs/services-product-unification/requirements.md`
- Design document: `.kiro/specs/services-product-unification/design.md`
- API requirements: `.kiro/specs/services-product-unification/API_REQUIREMENTS.md`
- Implementation checklist: `.kiro/specs/services-product-unification/IMPLEMENTATION_CHECKLIST.md`


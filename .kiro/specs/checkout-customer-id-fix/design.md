Refactor toàn bộ CheckoutService theo các yêu cầu sau, sử dụng ICurrentUser hiện có:

========================================================
1. Unified CustomerId logic
========================================================
- Tạo helper private async Task<Guid> GetCustomerIdAsync()
- Logic helper:
  1. Lấy CustomerId từ _currentUser.CustomerId (JWT claim) nếu có và khác Guid.Empty
  2. Nếu không có, lấy UserId từ _currentUser.UserId và dùng:
        var customer = await _customerRepository.GetByUserIdAsync(userId);
     - Nếu customer == null → throw NotFoundException hoặc UnauthorizedAccessException
  3. Trả về CustomerId hợp lệ
- Tất cả các phương thức CheckoutService (GetOrderByIdAsync, GetOrdersAsync, CancelOrderAsync, CheckoutAsync) **phải sử dụng helper này**, không dùng UserId để authorize order.

========================================================
2. Sửa toàn bộ authorization
========================================================
- Thay mọi đoạn:
      if (order.CustomerId != userId)
  Bằng:
      var customerId = await GetCustomerIdAsync();
      if (order.CustomerId != customerId) → Unauthorized
- Không được so sánh order.CustomerId với UserId.

========================================================
3. Cập nhật các phương thức
========================================================
3.1 GetOrderByIdAsync
- Lấy customerId từ helper
- Nếu order.CustomerId != customerId → Unauthorized
- Nếu order không tồn tại → NotFound

3.2 GetOrdersAsync
- Query bắt buộc filter theo customerId
- Áp dụng các filter trạng thái, pagination
- Không được query theo UserId

3.3 CancelOrderAsync
- Authorize bằng customerId
- Kiểm tra trạng thái cho phép cancel
- Lưu lý do và timestamp

3.4 CheckoutAsync
- Dùng helper lấy customerId
- Nếu Customer chưa tồn tại → tạo mới qua _customerRepository.CreateAsync
- Validate cart, items, shipping, payment
- Tạo order với đúng CustomerId
- Clear cart

========================================================
4. Repository interface validation
========================================================
### ICustomerRepository
- Task<Customer?> GetByUserIdAsync(Guid userId);
- Task<Customer?> GetByIdAsync(Guid customerId);
- Task CreateAsync(Customer entity);

### IOrderRepository
- Task<Order?> GetByIdAsync(Guid orderId);
- Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId, OrderFilter filter);
- Task UpdateAsync(Order order);

### ICartRepository / ICartItemRepository
- Task<Cart?> GetCartByCustomerIdAsync(Guid customerId);

### IProductRepository
- Phải có method kiểm tra availability, price
- Không dùng UserId để truy xuất data order/cart

========================================================
5. Coding standards & compilation
========================================================
- Code compile được, không syntax error, không orphan code
- CheckoutService implement đầy đủ ICheckoutService
- Các method signatures đầy đủ
- Không placeholder, không incomplete statement
- Sử dụng _currentUser từ ICurrentUser, không parse JWT thủ công

========================================================
6. Output
========================================================
- Code CheckoutService hoàn chỉnh
- Repository interfaces đầy đủ
- Helper GetCustomerIdAsync chuẩn
- Authorization chuẩn theo CustomerId
- Tuân thủ tất cả Requirement Document được cung cấp

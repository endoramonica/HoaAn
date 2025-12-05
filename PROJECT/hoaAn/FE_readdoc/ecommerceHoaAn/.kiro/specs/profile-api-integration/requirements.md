# Requirements Document

## Introduction

Tài liệu này mô tả các yêu cầu cho việc tích hợp API vào ProfilePage. ProfilePage hiện tại sử dụng dữ liệu mock/hardcoded và cần được kết nối với backend API thực. 

Hệ thống có 2 thực thể chính:
- **User**: Thực thể đăng nhập, chứa thông tin xác thực (email, password)
- **Customer**: Thực thể "buyer profile", chứa thông tin khách hàng mua sắm (name, phone, addresses, loyalty points, tier)

User và Customer có quan hệ 1-1. Khi lấy profile, chúng ta lấy thông tin Customer được ánh xạ từ User đã đăng nhập.

## Glossary

- **ProfilePage**: Trang hiển thị và quản lý thông tin cá nhân của người dùng
- **Customer**: Thực thể buyer profile, chứa thông tin khách hàng (name, phone, addresses, loyalty, tier)
- **User**: Thực thể đăng nhập, chứa thông tin xác thực
- **CustomerDetailDto**: DTO chứa thông tin chi tiết customer từ backend
- **AddressResponseDto**: DTO chứa thông tin địa chỉ từ backend
- **OrderDetailDto**: DTO chứa thông tin đơn hàng từ backend
- **Orval API Client**: Client được generate tự động từ OpenAPI spec

## Requirements

### Requirement 1: Hiển thị thông tin Customer Profile

**User Story:** As a logged-in user, I want to view my customer profile information, so that I can see my personal details and membership status.

#### Acceptance Criteria

1. WHEN the ProfilePage loads THEN the System SHALL fetch customer profile data from the backend API using the authenticated user's token
2. WHEN customer data is successfully retrieved THEN the System SHALL display name, email, phone, loyalty points, tier, and membership date
3. WHEN customer data is loading THEN the System SHALL display a loading skeleton/indicator
4. IF the API request fails THEN the System SHALL display an error message and provide a retry option
5. WHEN displaying loyalty tier THEN the System SHALL show the tier name (e.g., "VIP", "Gold", "Silver") with appropriate styling

### Requirement 2: Cập nhật thông tin Customer Profile

**User Story:** As a logged-in user, I want to edit my customer profile information, so that I can keep my personal details up to date.

#### Acceptance Criteria

1. WHEN a user clicks the edit button THEN the System SHALL enable editing mode for profile fields (name, phone)
2. WHEN a user saves profile changes THEN the System SHALL send an update request to the backend API
3. WHEN the update is successful THEN the System SHALL display a success notification and refresh the displayed data
4. IF the update fails THEN the System SHALL display an error message with the reason from the backend
5. WHILE the update is in progress THEN the System SHALL disable the save button and show a loading indicator

### Requirement 3: Quản lý địa chỉ khách hàng

**User Story:** As a logged-in user, I want to manage my delivery addresses, so that I can have multiple shipping options for my orders.

#### Acceptance Criteria

1. WHEN the addresses tab is selected THEN the System SHALL fetch and display all customer addresses from the backend
2. WHEN a user adds a new address THEN the System SHALL send a create request with streetAddress, city, state, postalCode, country, recipientName, phoneNumber, and addressType
3. WHEN a user edits an existing address THEN the System SHALL send an update request with the modified fields
4. WHEN a user deletes an address THEN the System SHALL send a delete request and remove the address from the list upon success
5. WHEN a user sets an address as default THEN the System SHALL call the set-default endpoint and update the UI to reflect the change
6. IF any address operation fails THEN the System SHALL display an error message and maintain the previous state

### Requirement 4: Hiển thị lịch sử đơn hàng

**User Story:** As a logged-in user, I want to view my order history, so that I can track my purchases and their status.

#### Acceptance Criteria

1. WHEN the orders tab is selected THEN the System SHALL fetch the user's orders from the backend with pagination support
2. WHEN orders are displayed THEN the System SHALL show order number, date, status, total amount, and item summary for each order
3. WHEN a user clicks "View details" on an order THEN the System SHALL fetch and display the full order details including all items and shipping information
4. WHEN displaying order status THEN the System SHALL use appropriate color coding (pending=yellow, processing=blue, shipped=purple, delivered=green, cancelled=red)
5. IF the orders request fails THEN the System SHALL display an error message and provide a retry option

### Requirement 5: Đổi mật khẩu

**User Story:** As a logged-in user, I want to change my password, so that I can maintain the security of my account.

#### Acceptance Criteria

1. WHEN a user initiates password change THEN the System SHALL display a form requiring current password, new password, and confirm new password
2. WHEN the user submits the password change form THEN the System SHALL validate that new password matches confirm password before sending to backend
3. WHEN the password change is successful THEN the System SHALL display a success notification and close the form
4. IF the current password is incorrect THEN the System SHALL display an error message from the backend
5. IF the new password does not meet requirements THEN the System SHALL display validation errors

### Requirement 6: Đăng xuất

**User Story:** As a logged-in user, I want to log out of my account, so that I can secure my session when I'm done.

#### Acceptance Criteria

1. WHEN a user clicks the logout button THEN the System SHALL display a confirmation dialog
2. WHEN the user confirms logout THEN the System SHALL call the logout API endpoint
3. WHEN logout is successful THEN the System SHALL clear all stored tokens and user data, then redirect to the home page
4. IF logout fails THEN the System SHALL still clear local data and redirect to home page (graceful degradation)

### Requirement 7: Cài đặt thông báo

**User Story:** As a logged-in user, I want to manage my notification preferences, so that I can control what communications I receive.

#### Acceptance Criteria

1. WHEN the notifications tab is selected THEN the System SHALL display current notification settings
2. WHEN a user toggles a notification setting THEN the System SHALL persist the change (note: may require backend endpoint or local storage)
3. WHEN notification settings are changed THEN the System SHALL provide visual feedback of the change

### Requirement 8: Hiển thị thống kê khách hàng

**User Story:** As a logged-in user, I want to see my purchase statistics, so that I can understand my shopping history at a glance.

#### Acceptance Criteria

1. WHEN the profile header loads THEN the System SHALL display total completed orders count
2. WHEN the profile header loads THEN the System SHALL display total amount spent
3. WHEN the profile header loads THEN the System SHALL display current loyalty tier with appropriate badge styling

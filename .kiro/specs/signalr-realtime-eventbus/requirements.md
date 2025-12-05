# Requirements Document

## Introduction

Tính năng SignalR Real-time và EventBus cho phép hệ thống VietCommerce gửi thông báo real-time đến clients khi có các sự kiện xảy ra như tạo/cập nhật sản phẩm, thay đổi trạng thái đơn hàng, thông báo mới, v.v. Hệ thống sử dụng EventBus pattern để decouple các components và SignalR để broadcast messages đến connected clients.

**Thông tin kỹ thuật dự án:**
- ASP.NET Core Version: .NET 9.0
- SignalR Version: Chưa có (cần thêm Microsoft.AspNetCore.SignalR)
- Database: SQL Server
- Existing Infrastructure: Redis (đã có), JWT Authentication (đã có)

**Cấu trúc đề xuất dựa trên dự án hiện tại:**
```
VietCommerce.Application/
  /SignalR
    /Hubs
      - NotificationHub.cs
      - ProductHub.cs
      - OrderHub.cs
    /Interfaces
      - INotificationHubClient.cs
      - IProductHubClient.cs
      - IOrderHubClient.cs
  /EventBus
    /Events
      - Event.cs (base class)
      - ProductCreatedEvent.cs
      - ProductUpdatedEvent.cs
      - ProductDeletedEvent.cs
      - OrderStatusChangedEvent.cs
      - NotificationEvent.cs
      - StockChangedEvent.cs
    /Handlers
      - IEventHandler.cs
      - ProductEventHandler.cs
      - OrderEventHandler.cs
      - NotificationEventHandler.cs
    - IEventBus.cs
    - InMemoryEventBus.cs
  /Services
    - SignalRService.cs
```

## Glossary

- **SignalR**: Thư viện ASP.NET Core cho phép giao tiếp real-time giữa server và client thông qua WebSocket, Server-Sent Events, hoặc Long Polling.
- **Hub**: Điểm kết nối trung tâm trong SignalR, nơi clients kết nối và nhận messages.
- **EventBus**: Pattern cho phép publish/subscribe events giữa các components mà không cần coupling trực tiếp.
- **Event**: Đối tượng chứa thông tin về một sự kiện xảy ra trong hệ thống.
- **EventHandler**: Component xử lý một loại event cụ thể.
- **HubContext**: Interface cho phép gửi messages đến SignalR clients từ bên ngoài Hub.
- **InMemoryEventBus**: Implementation của EventBus sử dụng bộ nhớ trong process (phù hợp cho single-instance deployment).

## Requirements

### Requirement 1: SignalR Infrastructure Setup

**User Story:** As a developer, I want to set up SignalR infrastructure in the project, so that the system can support real-time communication with clients.

#### Acceptance Criteria

1. WHEN the application starts THEN the System SHALL register SignalR services with proper configuration including connection timeout of 30 seconds and keep-alive interval of 15 seconds.
2. WHEN SignalR is configured THEN the System SHALL enable CORS for origins "http://localhost:3000" and "http://localhost:3001" with credentials support.
3. WHEN a client connects to a Hub THEN the System SHALL validate the JWT token from query string or Authorization header.
4. WHEN Hub endpoints are mapped THEN the System SHALL expose endpoints at "/hubs/notification", "/hubs/product", and "/hubs/order".

### Requirement 2: NotificationHub Implementation

**User Story:** As a user, I want to receive real-time notifications, so that I can stay updated on important events without refreshing the page.

#### Acceptance Criteria

1. WHEN a client connects to NotificationHub THEN the System SHALL add the client to a group based on their UserId.
2. WHEN SendNotification method is called with userId and message THEN the System SHALL deliver the notification to all connections of that user.
3. WHEN MarkAsRead method is called with notificationId THEN the System SHALL update the notification status and broadcast the change to the user's connections.
4. WHEN GetUnreadCount method is called THEN the System SHALL return the count of unread notifications for the authenticated user.
5. WHEN a client disconnects THEN the System SHALL remove the client from their user group.

### Requirement 3: ProductHub Implementation

**User Story:** As a user browsing products, I want to see real-time updates when products change, so that I always see the latest information.

#### Acceptance Criteria

1. WHEN a product is created THEN the System SHALL broadcast ProductCreated event to all connected clients with product details including ProductId, ProductName, and CategoryId.
2. WHEN a product is updated THEN the System SHALL broadcast ProductUpdated event to all connected clients with updated product information.
3. WHEN a product is deleted THEN the System SHALL broadcast ProductDeleted event to all connected clients with the ProductId.
4. WHEN stock quantity changes THEN the System SHALL broadcast StockChanged event to all connected clients with ProductId, OldQuantity, and NewQuantity.
5. WHEN a client subscribes to a specific category THEN the System SHALL add the client to a category-specific group to receive only relevant product updates.

### Requirement 4: OrderHub Implementation

**User Story:** As a customer, I want to track my order status in real-time, so that I know immediately when my order status changes.

#### Acceptance Criteria

1. WHEN an order status changes THEN the System SHALL broadcast OrderStatusChanged event to the order owner with OrderId, OldStatus, NewStatus, and UpdatedAt.
2. WHEN a client connects to OrderHub THEN the System SHALL add the client to a group based on their CustomerId.
3. WHEN TrackOrder method is called with orderId THEN the System SHALL add the client to an order-specific group to receive updates for that order.
4. WHEN StopTrackingOrder method is called with orderId THEN the System SHALL remove the client from the order-specific group.

### Requirement 5: EventBus Infrastructure

**User Story:** As a developer, I want an EventBus system to decouple event publishers from subscribers, so that the codebase remains maintainable and extensible.

#### Acceptance Criteria

1. WHEN IEventBus.PublishAsync is called with an event THEN the System SHALL invoke all registered handlers for that event type.
2. WHEN IEventBus.Subscribe is called with event type and handler THEN the System SHALL register the handler to receive events of that type.
3. WHEN multiple handlers are registered for the same event type THEN the System SHALL invoke all handlers in registration order.
4. WHEN a handler throws an exception THEN the System SHALL log the error and continue processing remaining handlers.
5. WHEN the application starts THEN the System SHALL automatically register all EventHandlers from the assembly.

### Requirement 6: Event Definitions

**User Story:** As a developer, I want well-defined event classes, so that event data is consistent and type-safe across the system.

#### Acceptance Criteria

1. WHEN an Event is created THEN the System SHALL assign a unique EventId (GUID) and CreatedAt timestamp automatically.
2. WHEN ProductCreatedEvent is published THEN the System SHALL include ProductId, ProductName, CategoryId, Price, and CreatedBy properties.
3. WHEN ProductUpdatedEvent is published THEN the System SHALL include ProductId, ProductName, CategoryId, Price, UpdatedBy, and ChangedFields properties.
4. WHEN ProductDeletedEvent is published THEN the System SHALL include ProductId and DeletedBy properties.
5. WHEN OrderStatusChangedEvent is published THEN the System SHALL include OrderId, CustomerId, OldStatus, NewStatus, and ChangedBy properties.
6. WHEN NotificationEvent is published THEN the System SHALL include UserId, Title, Message, NotificationType, and RelatedEntityId properties.
7. WHEN StockChangedEvent is published THEN the System SHALL include ProductId, ProductName, OldQuantity, NewQuantity, and Reason properties.

### Requirement 7: EventHandler to SignalR Integration

**User Story:** As a developer, I want EventHandlers to automatically broadcast events via SignalR, so that clients receive real-time updates when events occur.

#### Acceptance Criteria

1. WHEN ProductEventHandler receives ProductCreatedEvent THEN the System SHALL broadcast to ProductHub clients via IHubContext.
2. WHEN ProductEventHandler receives ProductUpdatedEvent THEN the System SHALL broadcast to ProductHub clients and category-specific groups.
3. WHEN ProductEventHandler receives StockChangedEvent THEN the System SHALL broadcast to ProductHub clients.
4. WHEN OrderEventHandler receives OrderStatusChangedEvent THEN the System SHALL broadcast to the specific customer's group in OrderHub.
5. WHEN NotificationEventHandler receives NotificationEvent THEN the System SHALL broadcast to the specific user's group in NotificationHub.

### Requirement 8: Controller Integration

**User Story:** As a developer, I want to easily publish events from Controllers and Services, so that real-time updates are triggered when data changes.

#### Acceptance Criteria

1. WHEN IEventBus is injected into a Controller THEN the System SHALL provide a valid instance via dependency injection.
2. WHEN a product is created via ProductController THEN the System SHALL publish ProductCreatedEvent after successful creation.
3. WHEN a product is updated via ProductController THEN the System SHALL publish ProductUpdatedEvent after successful update.
4. WHEN a product is deleted via ProductController THEN the System SHALL publish ProductDeletedEvent after successful deletion.
5. WHEN an order status is changed via OrderController THEN the System SHALL publish OrderStatusChangedEvent after successful update.

### Requirement 9: Authentication and Authorization

**User Story:** As a system administrator, I want SignalR connections to be authenticated, so that only authorized users can receive real-time updates.

#### Acceptance Criteria

1. WHEN a client connects without a valid JWT token THEN the System SHALL reject the connection with 401 Unauthorized.
2. WHEN a client connects with a valid JWT token THEN the System SHALL extract UserId and CustomerId claims for group assignment.
3. WHEN a client attempts to access another user's notifications THEN the System SHALL deny the request.
4. WHEN admin-only events are broadcast THEN the System SHALL send only to clients with admin role.

### Requirement 10: Error Handling and Logging

**User Story:** As a developer, I want comprehensive error handling and logging for SignalR and EventBus, so that issues can be diagnosed and resolved quickly.

#### Acceptance Criteria

1. WHEN a SignalR connection fails THEN the System SHALL log the error with connection details and exception information.
2. WHEN an EventHandler fails THEN the System SHALL log the event type, handler name, and exception details.
3. WHEN a client reconnects after disconnection THEN the System SHALL log the reconnection and restore group memberships.
4. WHEN broadcasting fails for a specific client THEN the System SHALL log the failure and continue broadcasting to other clients.

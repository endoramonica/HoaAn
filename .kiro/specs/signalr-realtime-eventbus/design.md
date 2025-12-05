# Design Document: SignalR Real-time và EventBus

## Overview

Tài liệu này mô tả thiết kế chi tiết cho hệ thống SignalR Real-time và EventBus trong VietCommerce. Hệ thống cho phép giao tiếp real-time giữa server và clients thông qua SignalR Hubs, sử dụng EventBus pattern để decouple event publishers từ subscribers.

**Tech Stack:**
- ASP.NET Core 9.0
- Microsoft.AspNetCore.SignalR (cần thêm)
- SQL Server (existing)
- Redis (existing - có thể dùng cho SignalR backplane trong tương lai)
- JWT Authentication (existing)

## Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              VietCommerce.Api                                │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐              │
│  │ ProductController│  │ OrderController │  │ NotificationCtrl│              │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘              │
│           │                    │                    │                        │
│           └────────────────────┼────────────────────┘                        │
│                                │                                             │
│                                ▼                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │                         IEventBus                                    │    │
│  │  PublishAsync<T>(event) / Subscribe<T, THandler>()                  │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
│                                │                                             │
│                                ▼                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │                     InMemoryEventBus                                 │    │
│  │  Dictionary<Type, List<IEventHandler>>                              │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
│                                │                                             │
└────────────────────────────────┼─────────────────────────────────────────────┘
                                 │
                                 ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                        VietCommerce.Application                              │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │                        EventHandlers                                 │    │
│  │  ┌──────────────────┐ ┌──────────────────┐ ┌──────────────────┐     │    │
│  │  │ProductEventHandler│ │OrderEventHandler │ │NotificationHandler│    │    │
│  │  └────────┬─────────┘ └────────┬─────────┘ └────────┬─────────┘     │    │
│  └───────────┼────────────────────┼────────────────────┼───────────────┘    │
│              │                    │                    │                     │
│              └────────────────────┼────────────────────┘                     │
│                                   ▼                                          │
│  ┌─────────────────────────────────────────────────────────────────────┐    │
│  │                        SignalR Hubs                                  │    │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐               │    │
│  │  │ProductHub    │  │OrderHub      │  │NotificationHub│              │    │
│  │  │/hubs/product │  │/hubs/order   │  │/hubs/notification│           │    │
│  │  └──────────────┘  └──────────────┘  └──────────────┘               │    │
│  └─────────────────────────────────────────────────────────────────────┘    │
│                                   │                                          │
└───────────────────────────────────┼──────────────────────────────────────────┘
                                    │
                                    ▼
                    ┌───────────────────────────────┐
                    │         WebSocket             │
                    │    Connected Clients          │
                    │  (Web, Mobile, Admin Panel)   │
                    └───────────────────────────────┘
```

## Components and Interfaces

### 1. EventBus Interfaces

```csharp
// VietCommerce.Application/EventBus/IEventBus.cs
public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) 
        where T : Event;
    
    void Subscribe<T, THandler>() 
        where T : Event 
        where THandler : IEventHandler<T>;
    
    void Unsubscribe<T, THandler>() 
        where T : Event 
        where THandler : IEventHandler<T>;
}

// VietCommerce.Application/EventBus/Handlers/IEventHandler.cs
public interface IEventHandler<in T> where T : Event
{
    Task HandleAsync(T @event, CancellationToken cancellationToken = default);
}
```

### 2. SignalR Hub Interfaces (Strongly-typed clients)

```csharp
// VietCommerce.Application/SignalR/Interfaces/INotificationHubClient.cs
public interface INotificationHubClient
{
    Task ReceiveNotification(NotificationDto notification);
    Task NotificationRead(Guid notificationId);
    Task UnreadCountChanged(int count);
}

// VietCommerce.Application/SignalR/Interfaces/IProductHubClient.cs
public interface IProductHubClient
{
    Task ProductCreated(ProductCreatedEventDto product);
    Task ProductUpdated(ProductUpdatedEventDto product);
    Task ProductDeleted(Guid productId);
    Task StockChanged(StockChangedEventDto stockInfo);
}

// VietCommerce.Application/SignalR/Interfaces/IOrderHubClient.cs
public interface IOrderHubClient
{
    Task OrderStatusChanged(OrderStatusChangedEventDto orderStatus);
    Task OrderUpdated(OrderUpdatedEventDto order);
}
```

### 3. SignalR Hubs

```csharp
// VietCommerce.Application/SignalR/Hubs/NotificationHub.cs
[Authorize]
public class NotificationHub : Hub<INotificationHubClient>
{
    public async Task JoinUserGroup();
    public async Task LeaveUserGroup();
    public async Task MarkAsRead(Guid notificationId);
    public async Task<int> GetUnreadCount();
}

// VietCommerce.Application/SignalR/Hubs/ProductHub.cs
public class ProductHub : Hub<IProductHubClient>
{
    public async Task SubscribeToCategory(int categoryId);
    public async Task UnsubscribeFromCategory(int categoryId);
    public async Task SubscribeToProduct(Guid productId);
    public async Task UnsubscribeFromProduct(Guid productId);
}

// VietCommerce.Application/SignalR/Hubs/OrderHub.cs
[Authorize]
public class OrderHub : Hub<IOrderHubClient>
{
    public async Task JoinCustomerGroup();
    public async Task LeaveCustomerGroup();
    public async Task TrackOrder(Guid orderId);
    public async Task StopTrackingOrder(Guid orderId);
}
```

### 4. EventBus Implementation

```csharp
// VietCommerce.Application/EventBus/InMemoryEventBus.cs
public class InMemoryEventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InMemoryEventBus> _logger;
    private readonly ConcurrentDictionary<Type, List<Type>> _handlers;
    
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) 
        where T : Event;
    
    public void Subscribe<T, THandler>() 
        where T : Event 
        where THandler : IEventHandler<T>;
}
```

## Data Models

### Event Base Class and DTOs

```csharp
// VietCommerce.Application/EventBus/Events/Event.cs
public abstract class Event
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
}

// Product Events
public class ProductCreatedEvent : Event
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public Guid CreatedBy { get; set; }
}

public class ProductUpdatedEvent : Event
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public Guid UpdatedBy { get; set; }
    public List<string> ChangedFields { get; set; } = new();
}

public class ProductDeletedEvent : Event
{
    public Guid ProductId { get; set; }
    public Guid DeletedBy { get; set; }
}

public class StockChangedEvent : Event
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int OldQuantity { get; set; }
    public int NewQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
}

// Order Events
public class OrderStatusChangedEvent : Event
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public Guid ChangedBy { get; set; }
}

// Notification Events
public class NotificationEvent : Event
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public Guid? RelatedEntityId { get; set; }
}
```

### SignalR Event DTOs

```csharp
// VietCommerce.Core/DTOs/SignalR/ProductCreatedEventDto.cs
public class ProductCreatedEventDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}

// VietCommerce.Core/DTOs/SignalR/OrderStatusChangedEventDto.cs
public class OrderStatusChangedEventDto
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

// VietCommerce.Core/DTOs/SignalR/StockChangedEventDto.cs
public class StockChangedEventDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int OldQuantity { get; set; }
    public int NewQuantity { get; set; }
    public int QuantityChange { get; set; }
    public DateTime ChangedAt { get; set; }
}
```



## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

Based on the prework analysis, the following correctness properties have been identified:

### Property 1: EventBus Handler Invocation
*For any* event published via IEventBus.PublishAsync, all registered handlers for that event type should be invoked exactly once.
**Validates: Requirements 5.1, 5.2**

### Property 2: EventBus Handler Order Preservation
*For any* event with multiple registered handlers, all handlers should be invoked in their registration order.
**Validates: Requirements 5.3**

### Property 3: EventBus Fault Tolerance
*For any* event where one handler throws an exception, all remaining handlers should still be invoked and the exception should be logged.
**Validates: Requirements 5.4, 10.2**

### Property 4: Event Identity Uniqueness
*For any* two events created, their EventId values should be unique (different GUIDs), and CreatedAt should be set to a valid timestamp.
**Validates: Requirements 6.1**

### Property 5: Event Structure Completeness
*For any* domain event (ProductCreatedEvent, ProductUpdatedEvent, ProductDeletedEvent, OrderStatusChangedEvent, NotificationEvent, StockChangedEvent), all required properties as defined in the event class should be non-null when the event is valid.
**Validates: Requirements 6.2, 6.3, 6.4, 6.5, 6.6, 6.7**

### Property 6: Hub Group Assignment on Connect
*For any* authenticated client connecting to NotificationHub or OrderHub, the client should be added to a group matching their UserId or CustomerId claim.
**Validates: Requirements 2.1, 4.2**

### Property 7: Hub Group Removal on Disconnect
*For any* client disconnecting from a Hub, the client should be removed from all groups they were a member of.
**Validates: Requirements 2.5**

### Property 8: Notification Delivery to User Connections
*For any* notification sent to a user, all active connections belonging to that user should receive the notification.
**Validates: Requirements 2.2**

### Property 9: Category Subscription Isolation
*For any* client subscribed to a specific category, they should only receive product events for products in that category (when category filtering is active).
**Validates: Requirements 3.5**

### Property 10: Order Tracking Group Management
*For any* TrackOrder call followed by StopTrackingOrder for the same orderId, the client should be removed from the order-specific group.
**Validates: Requirements 4.3, 4.4**

### Property 11: Authentication Rejection for Invalid Tokens
*For any* connection attempt to an [Authorize] Hub without a valid JWT token, the connection should be rejected with 401 status.
**Validates: Requirements 1.3, 9.1**

### Property 12: Cross-User Access Prevention
*For any* attempt by a client to access notifications or orders belonging to a different user, the request should be denied.
**Validates: Requirements 9.3**

### Property 13: Broadcast Resilience
*For any* broadcast operation where one client connection fails, other clients should still receive the message.
**Validates: Requirements 10.4**

### Property 14: Controller Event Publishing
*For any* successful create/update/delete operation on Product or Order via their respective controllers, the corresponding event should be published to the EventBus.
**Validates: Requirements 8.2, 8.3, 8.4, 8.5**

### Property 15: EventHandler to SignalR Integration
*For any* event received by an EventHandler, the handler should broadcast to the appropriate SignalR Hub clients via IHubContext.
**Validates: Requirements 7.1, 7.2, 7.3, 7.4, 7.5**

## Error Handling

### SignalR Connection Errors
- **Connection Timeout**: Client receives timeout error after 30 seconds of no response
- **Authentication Failure**: 401 Unauthorized returned, connection rejected
- **Hub Not Found**: 404 Not Found returned
- **Server Error**: 500 Internal Server Error with error details logged

### EventBus Errors
- **Handler Exception**: Exception logged with event type and handler name, other handlers continue
- **No Handlers Registered**: Event published successfully but no action taken (logged as warning)
- **Invalid Event**: ArgumentNullException thrown if event is null

### Broadcast Errors
- **Client Disconnected**: Message skipped for disconnected client, logged as debug
- **Serialization Error**: Exception logged, broadcast continues to other clients
- **Group Not Found**: No error, empty broadcast

## Testing Strategy

### Property-Based Testing Framework
- **Framework**: FsCheck for .NET (NuGet: FsCheck.Xunit)
- **Minimum Iterations**: 100 per property test

### Unit Tests
Unit tests will cover:
- SignalR Hub method invocations
- EventBus subscription and publication
- Event creation and validation
- Authentication/Authorization flows

### Property-Based Tests
Property tests will validate:
- EventBus handler invocation guarantees (Property 1, 2, 3)
- Event identity uniqueness (Property 4)
- Event structure completeness (Property 5)
- Group management correctness (Property 6, 7, 10)
- Broadcast resilience (Property 13)

### Integration Tests
Integration tests will cover:
- End-to-end flow: Controller → EventBus → EventHandler → SignalR → Client
- Authentication flow with JWT tokens
- Multiple client scenarios

### Test Annotations
Each property-based test MUST be annotated with:
```csharp
// **Feature: signalr-realtime-eventbus, Property {number}: {property_text}**
// **Validates: Requirements X.Y**
```

### Test Structure
```
VietCommerce.Tests/
  /SignalR
    /Hubs
      - NotificationHubTests.cs
      - ProductHubTests.cs
      - OrderHubTests.cs
    /Properties
      - EventBusPropertyTests.cs
      - HubGroupPropertyTests.cs
      - BroadcastPropertyTests.cs
  /EventBus
    - InMemoryEventBusTests.cs
    - EventHandlerTests.cs
```

## File Structure

Based on the existing project structure, the following files will be created:

```
VietCommerce.Application/
  /SignalR/
    /Hubs/
      - NotificationHub.cs
      - ProductHub.cs
      - OrderHub.cs
    /Interfaces/
      - INotificationHubClient.cs
      - IProductHubClient.cs
      - IOrderHubClient.cs
  /EventBus/
    /Events/
      - Event.cs
      - ProductCreatedEvent.cs
      - ProductUpdatedEvent.cs
      - ProductDeletedEvent.cs
      - OrderStatusChangedEvent.cs
      - NotificationEvent.cs
      - StockChangedEvent.cs
    /Handlers/
      - IEventHandler.cs
      - ProductEventHandler.cs
      - OrderEventHandler.cs
      - NotificationEventHandler.cs
    - IEventBus.cs
    - InMemoryEventBus.cs
  /Extensions/
    - SignalRExtensions.cs (service registration)
    - EventBusExtensions.cs (service registration)

VietCommerce.Core/
  /DTOs/
    /SignalR/
      - ProductCreatedEventDto.cs
      - ProductUpdatedEventDto.cs
      - OrderStatusChangedEventDto.cs
      - StockChangedEventDto.cs
      - NotificationEventDto.cs

VietCommerce.Api/
  - Program.cs (update with SignalR configuration)
```

## Configuration

### appsettings.json additions
```json
{
  "SignalR": {
    "ConnectionTimeout": 30,
    "KeepAliveInterval": 15,
    "EnableDetailedErrors": true
  }
}
```

### Program.cs SignalR Configuration
```csharp
// Add SignalR services
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// Map Hub endpoints
app.MapHub<NotificationHub>("/hubs/notification");
app.MapHub<ProductHub>("/hubs/product");
app.MapHub<OrderHub>("/hubs/order");
```

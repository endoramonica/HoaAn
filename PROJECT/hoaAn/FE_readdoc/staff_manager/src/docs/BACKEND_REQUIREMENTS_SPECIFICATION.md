# Backend Requirements Specification - POS System
## Frontend to Backend Development Handoff Document

### Project Overview
Chúng tôi đã hoàn thành frontend development cho một hệ thống POS (Point of Sale) multi-module với role-based access control. Frontend được xây dựng bằng React/TypeScript với các tính năng hoàn chỉnh bao gồm pagination, cross-module integration, real-time notifications, và responsive design.

**Technology Stack Frontend:**
- React 18 + TypeScript
- React Router (SPA routing)
- Context API (state management) 
- Tailwind CSS + shadcn/ui components
- Real-time WebSocket connections
- Multi-language support (Vi/En)
- Dark/Light theme

**Modules đã hoàn thành:**
- ✅ POS (Point of Sale) System
- ✅ Order Management with pagination
- ✅ Inventory Management with real-time updates
- ✅ Staff Management & Task Assignment
- ✅ Analytics & Reporting Dashboard
- ✅ CRM (Customer Relationship Management)
- ✅ LRM (Loyalty/Location Resource Management)
- ✅ HRM (Human Resource Management)
- ✅ Role-based Authentication & Authorization
- ✅ Shift Management với cash handling
- ✅ Multi-payment support (Cash/Card/Digital)

---

## 1. Database Schema Requirements

### Core Tables (PRIORITY: HIGH)

#### 1.1 Authentication & User Management
```sql
-- Stores table (Multi-tenant support)
CREATE TABLE stores (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    address TEXT NOT NULL,
    phone VARCHAR(50),
    email VARCHAR(255),
    currency VARCHAR(3) DEFAULT 'USD',
    timezone VARCHAR(50) DEFAULT 'UTC',
    tax_rate DECIMAL(5,4) DEFAULT 0.1000,
    is_active BOOLEAN DEFAULT true,
    settings JSONB DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Users table với role-based permissions
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    name VARCHAR(255) NOT NULL,
    phone VARCHAR(50),
    avatar_url VARCHAR(500),
    role VARCHAR(50) NOT NULL CHECK (role IN ('staff', 'manager', 'admin')),
    store_id UUID NOT NULL REFERENCES stores(id) ON DELETE CASCADE,
    permissions TEXT[] DEFAULT '{}',
    is_active BOOLEAN DEFAULT true,
    last_login TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);
```

#### 1.2 Product & Inventory Management
```sql
-- Categories table
CREATE TABLE categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    parent_id UUID REFERENCES categories(id),
    store_id UUID NOT NULL REFERENCES stores(id) ON DELETE CASCADE,
    sort_order INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Products table
CREATE TABLE products (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    description TEXT,
    sku VARCHAR(100),
    barcode VARCHAR(100),
    category_id UUID REFERENCES categories(id),
    price DECIMAL(10,2) NOT NULL,
    cost DECIMAL(10,2),
    image_url VARCHAR(500),
    images JSONB DEFAULT '[]',
    is_active BOOLEAN DEFAULT true,
    track_inventory BOOLEAN DEFAULT true,
    allow_negative_stock BOOLEAN DEFAULT false,
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Inventory table (Store-specific stock levels)
CREATE TABLE inventory (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    store_id UUID NOT NULL REFERENCES stores(id) ON DELETE CASCADE,
    quantity INTEGER NOT NULL DEFAULT 0,
    reserved_quantity INTEGER DEFAULT 0,
    min_stock INTEGER DEFAULT 0,
    max_stock INTEGER DEFAULT 1000,
    reorder_point INTEGER DEFAULT 10,
    last_counted TIMESTAMP WITH TIME ZONE,
    cost_per_unit DECIMAL(10,2),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(product_id, store_id)
);
```

#### 1.3 Order Management
```sql
-- Customers table
CREATE TABLE customers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255),
    phone VARCHAR(50),
    name VARCHAR(255),
    date_of_birth DATE,
    address JSONB,
    store_id UUID NOT NULL REFERENCES stores(id) ON DELETE CASCADE,
    total_spent DECIMAL(10,2) DEFAULT 0,
    visit_count INTEGER DEFAULT 0,
    last_visit TIMESTAMP WITH TIME ZONE,
    notes TEXT,
    tags TEXT[] DEFAULT '{}',
    loyalty_points INTEGER DEFAULT 0,
    loyalty_tier VARCHAR(50) DEFAULT 'bronze',
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Orders table
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_number VARCHAR(50) UNIQUE NOT NULL,
    customer_id UUID REFERENCES customers(id),
    customer_name VARCHAR(255),
    customer_phone VARCHAR(50),
    customer_email VARCHAR(255),
    store_id UUID NOT NULL REFERENCES stores(id) ON DELETE CASCADE,
    staff_id UUID NOT NULL REFERENCES users(id),
    status VARCHAR(50) NOT NULL CHECK (status IN ('pending', 'processing', 'completed', 'cancelled', 'refunded')),
    payment_status VARCHAR(50) DEFAULT 'pending' CHECK (payment_status IN ('pending', 'paid', 'partially_paid', 'refunded')),
    payment_method VARCHAR(50) CHECK (payment_method IN ('cash', 'card', 'digital', 'mixed')),
    subtotal DECIMAL(10,2) NOT NULL DEFAULT 0,
    tax_amount DECIMAL(10,2) DEFAULT 0,
    discount_amount DECIMAL(10,2) DEFAULT 0,
    tip_amount DECIMAL(10,2) DEFAULT 0,
    total_amount DECIMAL(10,2) NOT NULL,
    notes TEXT,
    internal_notes TEXT,
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Order Items table
CREATE TABLE order_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    product_id UUID NOT NULL REFERENCES products(id),
    product_name VARCHAR(255) NOT NULL,
    product_sku VARCHAR(100),
    quantity INTEGER NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    discount_amount DECIMAL(10,2) DEFAULT 0,
    total_amount DECIMAL(10,2) NOT NULL,
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);
```

#### 1.4 HRM & Shift Management
```sql
-- Shifts table
CREATE TABLE shifts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    staff_id UUID NOT NULL REFERENCES users(id),
    store_id UUID NOT NULL REFERENCES stores(id) ON DELETE CASCADE,
    start_time TIMESTAMP WITH TIME ZONE NOT NULL,
    end_time TIMESTAMP WITH TIME ZONE,
    opening_cash DECIMAL(10,2) DEFAULT 0,
    closing_cash DECIMAL(10,2),
    expected_cash DECIMAL(10,2),
    cash_difference DECIMAL(10,2),
    total_sales DECIMAL(10,2) DEFAULT 0,
    total_transactions INTEGER DEFAULT 0,
    status VARCHAR(50) DEFAULT 'open' CHECK (status IN ('open', 'closed')),
    notes TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Tasks table
CREATE TABLE tasks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    assigned_to UUID NOT NULL REFERENCES users(id),
    assigned_by UUID NOT NULL REFERENCES users(id),
    store_id UUID NOT NULL REFERENCES stores(id) ON DELETE CASCADE,
    priority VARCHAR(20) DEFAULT 'medium' CHECK (priority IN ('low', 'medium', 'high')),
    status VARCHAR(50) DEFAULT 'pending' CHECK (status IN ('pending', 'in_progress', 'completed', 'cancelled')),
    due_date TIMESTAMP WITH TIME ZONE,
    completed_at TIMESTAMP WITH TIME ZONE,
    estimated_hours DECIMAL(4,2),
    actual_hours DECIMAL(4,2),
    tags TEXT[] DEFAULT '{}',
    attachments JSONB DEFAULT '[]',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);
```

#### 1.5 CRM Extended Tables
```sql
-- Customer Interactions (CRM)
CREATE TABLE customer_interactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    staff_id UUID NOT NULL REFERENCES users(id),
    store_id UUID NOT NULL REFERENCES stores(id) ON DELETE CASCADE,
    interaction_type VARCHAR(50) NOT NULL CHECK (interaction_type IN ('call', 'email', 'visit', 'complaint', 'feedback', 'support')),
    subject VARCHAR(255),
    notes TEXT,
    sentiment VARCHAR(20) DEFAULT 'neutral' CHECK (sentiment IN ('positive', 'neutral', 'negative')),
    follow_up_required BOOLEAN DEFAULT false,
    follow_up_date DATE,
    status VARCHAR(50) DEFAULT 'completed' CHECK (status IN ('pending', 'in_progress', 'completed', 'cancelled')),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Loyalty Programs (LRM)
CREATE TABLE loyalty_programs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    store_id UUID NOT NULL REFERENCES stores(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    program_type VARCHAR(50) CHECK (program_type IN ('points', 'discount', 'cashback')),
    rules JSONB DEFAULT '{}',
    is_active BOOLEAN DEFAULT true,
    start_date DATE,
    end_date DATE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Customer Loyalty Transactions
CREATE TABLE loyalty_transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    order_id UUID REFERENCES orders(id),
    program_id UUID NOT NULL REFERENCES loyalty_programs(id) ON DELETE CASCADE,
    transaction_type VARCHAR(50) CHECK (transaction_type IN ('earn', 'redeem', 'expire', 'adjust')),
    points_amount INTEGER DEFAULT 0,
    monetary_value DECIMAL(10,2) DEFAULT 0,
    description TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);
```

#### 1.6 System Tables
```sql
-- Notifications table
CREATE TABLE notifications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    type VARCHAR(50) NOT NULL,
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    data JSONB DEFAULT '{}',
    is_read BOOLEAN DEFAULT false,
    read_at TIMESTAMP WITH TIME ZONE,
    expires_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Audit Logs table
CREATE TABLE audit_logs (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    table_name VARCHAR(100) NOT NULL,
    record_id UUID NOT NULL,
    action VARCHAR(20) NOT NULL CHECK (action IN ('CREATE', 'UPDATE', 'DELETE')),
    user_id UUID REFERENCES users(id),
    store_id UUID REFERENCES stores(id),
    old_values JSONB,
    new_values JSONB,
    changed_fields TEXT[],
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Required Indexes
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_store ON users(store_id);
CREATE INDEX idx_orders_store_date ON orders(store_id, created_at);
CREATE INDEX idx_orders_staff ON orders(staff_id);
CREATE INDEX idx_inventory_product_store ON inventory(product_id, store_id);
CREATE INDEX idx_inventory_low_stock ON inventory(store_id, quantity) WHERE quantity <= min_stock;
CREATE INDEX idx_notifications_user_unread ON notifications(user_id, is_read) WHERE is_read = false;
CREATE INDEX idx_audit_logs_table_record ON audit_logs(table_name, record_id);
```

---

## 2. API Endpoints Requirements

### 2.1 Authentication (PRIORITY: HIGH)
```
POST /api/v1/auth/login
POST /api/v1/auth/refresh
POST /api/v1/auth/logout
POST /api/v1/auth/change-password
```

### 2.2 User Management
```
GET    /api/v1/users                     # List users with pagination
GET    /api/v1/users/{id}                # Get user details
POST   /api/v1/users                     # Create new user (Manager only)
PUT    /api/v1/users/{id}                # Update user
DELETE /api/v1/users/{id}                # Deactivate user
GET    /api/v1/users/profile             # Get current user profile
PUT    /api/v1/users/profile             # Update current user profile
```

### 2.3 Product & Inventory Management
```
GET    /api/v1/products                  # List products with pagination & search
GET    /api/v1/products/{id}             # Get product details
POST   /api/v1/products                  # Create product
PUT    /api/v1/products/{id}             # Update product
DELETE /api/v1/products/{id}             # Deactivate product

GET    /api/v1/inventory                 # List inventory with low-stock alerts
PUT    /api/v1/inventory/{product_id}    # Update inventory levels
POST   /api/v1/inventory/adjustments     # Record inventory adjustments
GET    /api/v1/inventory/alerts          # Get low-stock alerts

GET    /api/v1/categories                # List categories
POST   /api/v1/categories                # Create category
PUT    /api/v1/categories/{id}           # Update category
DELETE /api/v1/categories/{id}           # Delete category
```

### 2.4 Order Management & POS
```
GET    /api/v1/orders                    # List orders with pagination & filters
GET    /api/v1/orders/{id}               # Get order details
POST   /api/v1/orders                    # Create new order
PUT    /api/v1/orders/{id}               # Update order
DELETE /api/v1/orders/{id}               # Cancel/refund order

POST   /api/v1/pos/calculate             # Calculate order totals
POST   /api/v1/pos/process-payment       # Process payment
POST   /api/v1/pos/void-transaction      # Void transaction
```

### 2.5 Customer Management (CRM)
```
GET    /api/v1/customers                 # List customers with pagination
GET    /api/v1/customers/{id}            # Get customer details
POST   /api/v1/customers                 # Create customer
PUT    /api/v1/customers/{id}            # Update customer
DELETE /api/v1/customers/{id}            # Delete customer

GET    /api/v1/customers/{id}/orders     # Get customer order history
GET    /api/v1/customers/{id}/interactions # Get customer interactions
POST   /api/v1/customers/{id}/interactions # Create customer interaction
```

### 2.6 Loyalty & Rewards (LRM)
```
GET    /api/v1/loyalty/programs          # List loyalty programs
POST   /api/v1/loyalty/programs          # Create loyalty program
PUT    /api/v1/loyalty/programs/{id}     # Update loyalty program

GET    /api/v1/loyalty/customers/{id}/points    # Get customer points
POST   /api/v1/loyalty/customers/{id}/redeem    # Redeem points
POST   /api/v1/loyalty/customers/{id}/earn      # Earn points
GET    /api/v1/loyalty/transactions             # List loyalty transactions
```

### 2.7 Human Resources (HRM)
```
GET    /api/v1/shifts                    # List shifts
GET    /api/v1/shifts/current            # Get current open shift
POST   /api/v1/shifts/open               # Open new shift
PUT    /api/v1/shifts/{id}/close         # Close shift

GET    /api/v1/tasks                     # List tasks with filters
POST   /api/v1/tasks                     # Create task
PUT    /api/v1/tasks/{id}                # Update task
PUT    /api/v1/tasks/{id}/status         # Update task status
DELETE /api/v1/tasks/{id}                # Delete task
```

### 2.8 Analytics & Reporting
```
GET    /api/v1/analytics/dashboard       # Manager dashboard data
GET    /api/v1/analytics/sales           # Sales analytics
GET    /api/v1/analytics/products        # Product performance
GET    /api/v1/analytics/staff           # Staff performance
GET    /api/v1/analytics/inventory       # Inventory reports
```

### 2.9 Notifications & Alerts
```
GET    /api/v1/notifications             # List user notifications
PUT    /api/v1/notifications/{id}/read   # Mark notification as read
PUT    /api/v1/notifications/read-all    # Mark all as read
DELETE /api/v1/notifications/{id}        # Delete notification
```

### 2.10 Export Functionality
```
GET    /api/v1/export/orders             # Export orders (CSV/Excel/PDF)
GET    /api/v1/export/inventory          # Export inventory
GET    /api/v1/export/sales-report       # Export sales report
GET    /api/v1/export/customer-data      # Export customer data
```

---

## 3. Real-time Features Requirements

### 3.1 WebSocket Events (PRIORITY: HIGH)
Frontend đã implement WebSocket client, cần backend hỗ trợ các events:

```javascript
// Inventory Updates
{
  type: 'inventory_updated',
  data: {
    product_id: 'uuid',
    new_quantity: 25,
    store_id: 'uuid'
  }
}

// Order Status Changes
{
  type: 'order_status_changed',
  data: {
    order_id: 'uuid',
    old_status: 'pending',
    new_status: 'completed',
    staff_id: 'uuid'
  }
}

// Low Stock Alerts
{
  type: 'low_stock_alert',
  data: {
    product_id: 'uuid',
    product_name: 'Coffee Mug',
    current_quantity: 5,
    min_stock: 10,
    store_id: 'uuid'
  }
}

// New Task Assignments
{
  type: 'task_assigned',
  data: {
    task_id: 'uuid',
    assigned_to: 'user_id',
    assigned_by: 'manager_id',
    priority: 'high'
  }
}

// System Notifications
{
  type: 'notification',
  data: {
    id: 'uuid',
    title: 'New Order',
    message: 'Order #ORD-001 has been created',
    type: 'order_created',
    user_id: 'uuid'
  }
}
```

### 3.2 WebSocket Rooms/Channels
```
store_{store_id}              # Store-specific updates
user_{user_id}               # User-specific notifications
role_manager_{store_id}      # Manager-only alerts
pos_{store_id}               # POS real-time updates
```

---

## 4. Permission System Requirements

### 4.1 Role-based Permissions (IMPLEMENTED in Frontend)
Frontend đã implement permission checking system, backend cần validate:

**Staff Role:**
- `pos.access`, `orders.view_own`, `orders.create`
- `inventory.view`, `tasks.view_own`, `tasks.mark_complete`
- `customers.view`, `customers.create`, `customers.edit`

**Manager Role:**
- All Staff permissions +
- `orders.view_all`, `orders.edit_all`, `orders.delete`
- `inventory.adjust`, `inventory.approve_adjustment`
- `analytics.view_store_dashboard`, `users.view_all`, `users.create`
- `tasks.create`, `tasks.assign`, `tasks.view_all`

**Admin Role:**
- All Manager permissions +
- `system.manage_integrations`, `users.manage_permissions`
- Multi-store access

### 4.2 Permission Middleware Required
```python
@permission_required('orders.view_all')
def get_all_orders(request):
    # Implementation
    pass

@permission_required('inventory.adjust')
def adjust_inventory(request):
    # Implementation  
    pass
```

---

## 5. Cross-Module Integration Requirements

### 5.1 POS ↔ CRM Integration
- Automatic customer creation from POS transactions
- Customer purchase history sync
- Loyalty points calculation and redemption

### 5.2 POS ↔ Inventory Integration
- Real-time stock deduction on order completion
- Low stock alerts triggered from POS sales
- Reserved quantity management for pending orders

### 5.3 Orders ↔ HRM Integration
- Order assignment to staff members
- Sales performance tracking per staff
- Commission calculation based on sales

### 5.4 CRM ↔ LRM Integration
- Customer tier calculation based on total spent
- Automatic loyalty point earning on purchases
- Targeted promotions based on customer segments

---

## 6. Payment Gateway Integration

### 6.1 Required Payment Methods
Frontend hỗ trợ các payment methods:
- **Cash payments** - với cash drawer management
- **Card payments** - cần integrate Stripe/Square
- **Digital payments** - Apple Pay, Google Pay, PayPal
- **Split payments** - combination of above

### 6.2 Payment Processing Flow
```
1. Frontend sends payment intent to backend
2. Backend creates payment with provider (Stripe/PayPal)
3. Payment processed and webhook received
4. Backend updates order status
5. WebSocket notification sent to frontend
6. Receipt generated and sent
```

---

## 7. Data Export & Reporting Requirements

### 7.1 Export Formats
Frontend có export functionality, cần backend support:
- **CSV** - cho Excel compatibility
- **PDF** - cho professional reports
- **JSON** - cho API integrations

### 7.2 Report Types
- Daily/Weekly/Monthly sales reports
- Inventory valuation reports
- Staff performance reports
- Customer analysis reports
- Tax reports với proper calculations

---

## 8. Security Requirements

### 8.1 Authentication & Authorization
- **JWT tokens** với refresh token mechanism
- **Password hashing** với bcrypt hoặc Argon2
- **Role-based access control** với granular permissions
- **Multi-factor authentication** (optional)

### 8.2 Data Security
- **Audit logging** cho tất cả data changes
- **Data encryption** cho sensitive information
- **HTTPS only** cho production
- **Input validation** và SQL injection prevention
- **Rate limiting** cho API endpoints

---

## 9. Performance Requirements

### 9.1 Response Time Targets
- **Authentication**: < 200ms
- **Product search**: < 300ms
- **Order creation**: < 500ms
- **Report generation**: < 2 seconds
- **Real-time updates**: < 100ms

### 9.2 Pagination Requirements
Frontend đã implement pagination, backend cần support:
```json
{
  "data": [...],
  "meta": {
    "pagination": {
      "current_page": 1,
      "per_page": 25,
      "total_pages": 10,
      "total_items": 250,
      "has_next": true,
      "has_prev": false
    }
  }
}
```

---

## 10. Development Environment Setup

### 10.1 Required Technology Stack
- **Database**: PostgreSQL 14+ (với UUID support)
- **Backend Framework**: Django REST Framework hoặc FastAPI
- **WebSocket**: Django Channels hoặc FastAPI WebSockets
- **Cache**: Redis cho session và real-time data
- **Message Queue**: Celery với Redis/RabbitMQ
- **File Storage**: AWS S3 hoặc local filesystem

### 10.2 Environment Variables
```env
# Database
DATABASE_URL=postgresql://user:pass@localhost:5432/pos_db

# Redis
REDIS_URL=redis://localhost:6379/0

# JWT
JWT_SECRET_KEY=your-secret-key
JWT_EXPIRE_MINUTES=60

# Payment Gateways
STRIPE_SECRET_KEY=sk_test_...
STRIPE_WEBHOOK_SECRET=whsec_...
PAYPAL_CLIENT_ID=...
PAYPAL_CLIENT_SECRET=...

# Email/SMS
SENDGRID_API_KEY=...
TWILIO_ACCOUNT_SID=...
TWILIO_AUTH_TOKEN=...

# Storage
AWS_ACCESS_KEY_ID=...
AWS_SECRET_ACCESS_KEY=...
AWS_STORAGE_BUCKET_NAME=...
```

---

## 11. Testing Requirements

### 11.1 Unit Tests Required
- Authentication và authorization logic
- Payment processing logic
- Inventory calculation logic
- Permission checking functions

### 11.2 Integration Tests Required
- API endpoint functionality
- WebSocket connections
- Payment gateway integrations
- Database transactions

### 11.3 Performance Tests Required
- API response times under load
- WebSocket connection limits
- Database query performance
- Concurrent user handling

---

## 12. Deployment & Infrastructure

### 12.1 Production Requirements
- **Docker containerization** cho easy deployment
- **Health checks** cho monitoring
- **Logging system** với log levels
- **Backup strategy** cho database
- **SSL certificates** cho HTTPS

### 12.2 Monitoring & Alerts
- API response time monitoring
- Database performance monitoring
- Payment gateway status monitoring
- Low stock alerts via email/SMS
- System error notifications

---

## 13. API Response Formats

### 13.1 Standard Success Response
```json
{
  "success": true,
  "data": { /* response data */ },
  "meta": {
    "timestamp": "2024-01-20T10:30:00Z",
    "request_id": "req_123456789"
  }
}
```

### 13.2 Standard Error Response
```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input data",
    "details": {
      "field": "email is required"
    }
  },
  "meta": {
    "timestamp": "2024-01-20T10:30:00Z",
    "request_id": "req_123456789"
  }
}
```

### 13.3 Pagination Response Format
```json
{
  "success": true,
  "data": [/* array of items */],
  "meta": {
    "pagination": {
      "current_page": 1,
      "per_page": 25,
      "total_pages": 10,
      "total_items": 250,
      "has_next": true,
      "has_prev": false
    },
    "timestamp": "2024-01-20T10:30:00Z"
  }
}
```

---

## 14. Migration & Data Seeding

### 14.1 Initial Data Requirements
```sql
-- Sample Store
INSERT INTO stores (name, address, phone, email, currency, tax_rate) VALUES
('Downtown Coffee Shop', '123 Main St, Downtown, NY 10001', '+1-555-0123', 'info@store.com', 'USD', 0.0875);

-- Sample Admin User
INSERT INTO users (email, password_hash, name, role, store_id, permissions) VALUES
('admin@store.com', 'hashed_password', 'System Admin', 'admin', store_id, ARRAY['all']);

-- Sample Categories
INSERT INTO categories (name, description, store_id) VALUES
('Beverages', 'Hot and cold drinks', store_id),
('Food', 'Snacks and meals', store_id),
('Merchandise', 'Store merchandise and gifts', store_id);

-- Sample Products với inventory
INSERT INTO products (name, sku, barcode, price, cost, category_id) VALUES
('Coffee Mug', 'MUG-001', '1234567890123', 12.99, 6.50, category_id);
```

---

## 15. Next Steps & Priorities

### Phase 1 (High Priority - Week 1-2)
1. ✅ Database schema implementation
2. ✅ Authentication system
3. ✅ Basic CRUD APIs for products, orders, customers
4. ✅ Permission middleware
5. ✅ WebSocket setup for real-time updates

### Phase 2 (Medium Priority - Week 3-4)  
1. Payment gateway integration (Stripe primary)
2. Advanced reporting APIs
3. Export functionality
4. Email/SMS notifications
5. Audit logging system

### Phase 3 (Low Priority - Week 5-6)
1. Advanced analytics APIs
2. Multi-store support enhancements
3. Third-party integrations (QuickBooks, etc.)
4. Performance optimizations
5. Advanced security features

---

## 16. Communication & Support

### 16.1 Frontend Team Contacts
- **Lead Developer**: [Tên và contact]
- **UI/UX Developer**: [Tên và contact]
- **QA Tester**: [Tên và contact]

### 16.2 API Documentation
- Frontend team sẽ provide detailed API testing với Postman collection
- Swagger/OpenAPI documentation required cho production
- Real-time testing environment setup needed

### 16.3 Deployment Coordination
- Frontend production build ready
- Staging environment cần setup cho integration testing
- CI/CD pipeline configuration needed

---

**Note cho Backend Team:**
Frontend đã hoàn toàn ready với comprehensive error handling, loading states, real-time updates, và responsive design. Tất cả components đã được tested với mock data. Chúng tôi ready để integrate ngay khi backend APIs available.

Vui lòng liên hệ để clarify bất kỳ technical details nào hoặc nếu cần additional information về frontend implementation.
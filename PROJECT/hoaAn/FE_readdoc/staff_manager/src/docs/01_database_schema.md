# Database Schema Documentation

## Overview
This document outlines the complete database schema for the multi-page role-based POS and store management system. The system supports multi-store operations, role-based permissions, real-time inventory management, and comprehensive analytics.

## Database Design Principles
- **Multi-tenancy**: Store-based data isolation
- **RBAC**: Role-based access control with granular permissions
- **Audit Trail**: Track all data changes with timestamps and user info
- **Scalability**: Indexed for performance with large datasets
- **Data Integrity**: Foreign key constraints and data validation

## Core Tables

### 1. stores
Manages multiple store locations within the system.

```sql
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

CREATE INDEX idx_stores_active ON stores(is_active);
```

### 2. users
System users with role-based permissions.

```sql
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

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_store ON users(store_id);
CREATE INDEX idx_users_role ON users(role);
```

### 3. categories
Product categories for organization.

```sql
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

CREATE INDEX idx_categories_store ON categories(store_id);
CREATE INDEX idx_categories_active ON categories(is_active);
```

### 4. products
Master product catalog.

```sql
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

CREATE INDEX idx_products_sku ON products(sku);
CREATE INDEX idx_products_barcode ON products(barcode);
CREATE INDEX idx_products_category ON products(category_id);
CREATE INDEX idx_products_active ON products(is_active);
```

### 5. inventory
Store-specific inventory levels.

```sql
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

CREATE INDEX idx_inventory_product ON inventory(product_id);
CREATE INDEX idx_inventory_store ON inventory(store_id);
CREATE INDEX idx_inventory_low_stock ON inventory(store_id, quantity) WHERE quantity <= min_stock;
```

### 6. customers
Customer information for tracking and marketing.

```sql
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
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX idx_customers_email ON customers(email);
CREATE INDEX idx_customers_phone ON customers(phone);
CREATE INDEX idx_customers_store ON customers(store_id);
```

### 7. orders
Customer orders and transactions.

```sql
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

CREATE INDEX idx_orders_number ON orders(order_number);
CREATE INDEX idx_orders_customer ON orders(customer_id);
CREATE INDEX idx_orders_store ON orders(store_id);
CREATE INDEX idx_orders_staff ON orders(staff_id);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_date ON orders(created_at);
```

### 8. order_items
Line items for each order.

```sql
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

CREATE INDEX idx_order_items_order ON order_items(order_id);
CREATE INDEX idx_order_items_product ON order_items(product_id);
```

### 9. shifts
Staff shift management and cash handling.

```sql
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

CREATE INDEX idx_shifts_staff ON shifts(staff_id);
CREATE INDEX idx_shifts_store ON shifts(store_id);
CREATE INDEX idx_shifts_date ON shifts(start_time);
CREATE INDEX idx_shifts_status ON shifts(status);
```

### 10. tasks
Task management and assignment.

```sql
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

CREATE INDEX idx_tasks_assigned_to ON tasks(assigned_to);
CREATE INDEX idx_tasks_assigned_by ON tasks(assigned_by);
CREATE INDEX idx_tasks_store ON tasks(store_id);
CREATE INDEX idx_tasks_status ON tasks(status);
CREATE INDEX idx_tasks_due_date ON tasks(due_date);
```

### 11. notifications
System notifications and alerts.

```sql
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

CREATE INDEX idx_notifications_user ON notifications(user_id);
CREATE INDEX idx_notifications_read ON notifications(user_id, is_read);
CREATE INDEX idx_notifications_type ON notifications(type);
```

### 12. audit_logs
Comprehensive audit trail for all system changes.

```sql
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

CREATE INDEX idx_audit_logs_table ON audit_logs(table_name);
CREATE INDEX idx_audit_logs_record ON audit_logs(record_id);
CREATE INDEX idx_audit_logs_user ON audit_logs(user_id);
CREATE INDEX idx_audit_logs_date ON audit_logs(created_at);
```

## Relationships Summary

### One-to-Many Relationships
- stores → users (1:N)
- stores → categories (1:N)
- stores → inventory (1:N)
- stores → customers (1:N)
- stores → orders (1:N)
- stores → shifts (1:N)
- stores → tasks (1:N)
- categories → products (1:N)
- products → inventory (1:N)
- products → order_items (1:N)
- users → orders (1:N) [staff assignments]
- users → shifts (1:N)
- users → tasks (1:N) [assigned_to and assigned_by]
- users → notifications (1:N)
- orders → order_items (1:N)

### Many-to-Many Relationships
- products ↔ stores (via inventory table)
- users ↔ stores (via store_id, but users can potentially access multiple stores through permissions)

## Sample Data

### Sample Store
```json
{
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Downtown Coffee Shop",
    "address": "123 Main St, Downtown, NY 10001",
    "phone": "+1-555-0123",
    "email": "info@downtowncoffee.com",
    "currency": "USD",
    "timezone": "America/New_York",
    "tax_rate": 0.0875
}
```

### Sample User
```json
{
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "email": "john.doe@store.com",
    "name": "John Doe",
    "role": "staff",
    "store_id": "550e8400-e29b-41d4-a716-446655440000",
    "permissions": ["view_my_orders", "create_order", "access_pos"],
    "is_active": true
}
```

### Sample Product
```json
{
    "id": "550e8400-e29b-41d4-a716-446655440002",
    "name": "Premium Coffee Mug",
    "sku": "MUG-001",
    "barcode": "1234567890123",
    "price": 12.99,
    "cost": 6.50,
    "category_id": "550e8400-e29b-41d4-a716-446655440003"
}
```

## Migration Notes

### Initial Migration Order
1. Create `stores` table first (referenced by many tables)
2. Create `users` table (depends on stores)
3. Create `categories` table (depends on stores)
4. Create `products` table (depends on categories)
5. Create `inventory` table (depends on products and stores)
6. Create `customers` table (depends on stores)
7. Create `orders` table (depends on customers, stores, users)
8. Create `order_items` table (depends on orders and products)
9. Create `shifts` table (depends on users and stores)
10. Create `tasks` table (depends on users and stores)
11. Create `notifications` table (depends on users)
12. Create `audit_logs` table (can be created last)

### Indexes and Constraints
- All foreign key constraints should be added after table creation
- Indexes should be created for frequently queried columns
- Partial indexes for conditional queries (e.g., active records only)
- Consider partitioning large tables like `audit_logs` by date

### Data Validation Rules
- Email format validation on users and customers
- Phone number format validation
- Currency amount precision (2 decimal places)
- Enum constraints for status fields
- Check constraints for positive quantities and amounts
# API Endpoints Documentation

## Overview
This document outlines all REST API endpoints for the multi-page role-based POS system. All endpoints follow RESTful conventions and return JSON responses.

## Base Configuration
- **Base URL**: `https://api.storepos.com/v1`
- **Authentication**: JWT Bearer Token
- **Content-Type**: `application/json`
- **Rate Limiting**: 1000 requests per minute per user

## Standard Response Format

### Success Response
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

### Error Response
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

### Pagination Response
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
        }
    }
}
```

## Authentication Endpoints

### POST /auth/login
Authenticate user and receive JWT token.

**Request Body:**
```json
{
    "email": "user@store.com",
    "password": "password123"
}
```

**Response:**
```json
{
    "success": true,
    "data": {
        "user": {
            "id": "uuid",
            "email": "user@store.com",
            "name": "John Doe",
            "role": "staff",
            "store_id": "uuid",
            "permissions": ["view_orders", "create_order"]
        },
        "token": "jwt_token_here",
        "expires_at": "2024-01-21T10:30:00Z"
    }
}
```

### POST /auth/refresh
Refresh JWT token.

**Headers:** `Authorization: Bearer {token}`

**Response:**
```json
{
    "success": true,
    "data": {
        "token": "new_jwt_token_here",
        "expires_at": "2024-01-21T10:30:00Z"
    }
}
```

### POST /auth/logout
Invalidate current token.

**Headers:** `Authorization: Bearer {token}`

## User Management Endpoints

### GET /users
List all users (Manager only).

**Headers:** `Authorization: Bearer {token}`
**Query Parameters:**
- `page`: Page number (default: 1)
- `per_page`: Items per page (default: 25, max: 100)
- `store_id`: Filter by store
- `role`: Filter by role
- `is_active`: Filter by active status

**Response:**
```json
{
    "success": true,
    "data": [
        {
            "id": "uuid",
            "email": "user@store.com",
            "name": "John Doe",
            "role": "staff",
            "store_id": "uuid",
            "is_active": true,
            "last_login": "2024-01-20T09:30:00Z",
            "created_at": "2024-01-15T08:00:00Z"
        }
    ],
    "meta": { /* pagination info */ }
}
```

### GET /users/{id}
Get specific user details.

### POST /users
Create new user (Manager only).

**Request Body:**
```json
{
    "email": "newuser@store.com",
    "password": "password123",
    "name": "Jane Smith",
    "phone": "+1-555-0123",
    "role": "staff",
    "store_id": "uuid",
    "permissions": ["view_my_orders", "create_order"]
}
```

### PUT /users/{id}
Update user information.

### DELETE /users/{id}
Deactivate user (Manager only).

## Store Management Endpoints

### GET /stores
List all stores accessible to user.

### GET /stores/{id}
Get specific store details.

### POST /stores
Create new store (Admin only).

### PUT /stores/{id}
Update store information (Manager only).

## Product Management Endpoints

### GET /products
List all products.

**Query Parameters:**
- `category_id`: Filter by category
- `search`: Search in name, description, SKU, barcode
- `is_active`: Filter by active status
- `low_stock`: Show only low stock items (if inventory tracking enabled)

**Response:**
```json
{
    "success": true,
    "data": [
        {
            "id": "uuid",
            "name": "Coffee Mug",
            "description": "Premium ceramic mug",
            "sku": "MUG-001",
            "barcode": "1234567890123",
            "price": 12.99,
            "cost": 6.50,
            "category": {
                "id": "uuid",
                "name": "Drinkware"
            },
            "image_url": "https://...",
            "is_active": true,
            "inventory": {
                "quantity": 25,
                "min_stock": 10,
                "max_stock": 100
            }
        }
    ]
}
```

### GET /products/{id}
Get specific product details.

### POST /products
Create new product (Manager only).

**Request Body:**
```json
{
    "name": "Coffee Mug",
    "description": "Premium ceramic mug",
    "sku": "MUG-001",
    "barcode": "1234567890123",
    "category_id": "uuid",
    "price": 12.99,
    "cost": 6.50,
    "image_url": "https://...",
    "track_inventory": true
}
```

### PUT /products/{id}
Update product information.

### DELETE /products/{id}
Deactivate product (Manager only).

## Inventory Management Endpoints

### GET /inventory
List inventory for current store.

**Query Parameters:**
- `product_id`: Filter by product
- `low_stock`: Show only low stock items
- `out_of_stock`: Show only out of stock items

### PUT /inventory/{product_id}
Update inventory levels (Manager/authorized staff only).

**Request Body:**
```json
{
    "quantity": 50,
    "min_stock": 10,
    "max_stock": 100,
    "adjustment_reason": "Restock delivery",
    "cost_per_unit": 6.50
}
```

### POST /inventory/adjustments
Record inventory adjustment.

**Request Body:**
```json
{
    "product_id": "uuid",
    "adjustment_type": "increase", // or "decrease"
    "quantity": 10,
    "reason": "Damage adjustment",
    "notes": "5 items damaged during transport"
}
```

## Order Management Endpoints

### GET /orders
List orders.

**Query Parameters:**
- `status`: Filter by status
- `staff_id`: Filter by staff (Manager can see all)
- `customer_id`: Filter by customer
- `date_from`: Start date filter
- `date_to`: End date filter
- `payment_status`: Filter by payment status

**Response:**
```json
{
    "success": true,
    "data": [
        {
            "id": "uuid",
            "order_number": "ORD-2024-001",
            "customer": {
                "id": "uuid",
                "name": "John Customer",
                "phone": "+1-555-0123"
            },
            "staff": {
                "id": "uuid",
                "name": "Jane Staff"
            },
            "status": "completed",
            "payment_status": "paid",
            "payment_method": "card",
            "subtotal": 25.98,
            "tax_amount": 2.60,
            "discount_amount": 0,
            "tip_amount": 3.00,
            "total_amount": 31.58,
            "items": [
                {
                    "product_id": "uuid",
                    "product_name": "Coffee Mug",
                    "quantity": 2,
                    "unit_price": 12.99,
                    "total_amount": 25.98
                }
            ],
            "created_at": "2024-01-20T09:30:00Z"
        }
    ]
}
```

### GET /orders/{id}
Get specific order details.

### POST /orders
Create new order.

**Request Body:**
```json
{
    "customer_id": "uuid", // optional
    "customer_name": "Walk-in Customer", // if no customer_id
    "customer_phone": "+1-555-0123", // optional
    "items": [
        {
            "product_id": "uuid",
            "quantity": 2,
            "unit_price": 12.99,
            "discount_amount": 0
        }
    ],
    "payment_method": "card",
    "tip_amount": 3.00,
    "notes": "Customer requested gift wrap"
}
```

### PUT /orders/{id}
Update order (limited fields after creation).

### DELETE /orders/{id}
Cancel/refund order (Manager only).

## POS Endpoints

### POST /pos/calculate
Calculate order totals before finalizing.

**Request Body:**
```json
{
    "items": [
        {
            "product_id": "uuid",
            "quantity": 2
        }
    ],
    "discount_amount": 0,
    "tip_amount": 3.00
}
```

**Response:**
```json
{
    "success": true,
    "data": {
        "items": [
            {
                "product_id": "uuid",
                "product_name": "Coffee Mug",
                "quantity": 2,
                "unit_price": 12.99,
                "total_amount": 25.98
            }
        ],
        "subtotal": 25.98,
        "tax_amount": 2.60,
        "discount_amount": 0,
        "tip_amount": 3.00,
        "total_amount": 31.58
    }
}
```

### POST /pos/process-payment
Process payment for order.

**Request Body:**
```json
{
    "order_id": "uuid",
    "payment_method": "card",
    "amount": 31.58,
    "payment_details": {
        "card_last_four": "1234",
        "transaction_id": "txn_123456"
    }
}
```

## Shift Management Endpoints

### GET /shifts
List shifts for current user or store.

### GET /shifts/current
Get current open shift for user.

### POST /shifts/open
Open new shift.

**Request Body:**
```json
{
    "opening_cash": 200.00,
    "notes": "Starting new shift"
}
```

### PUT /shifts/{id}/close
Close shift.

**Request Body:**
```json
{
    "closing_cash": 425.50,
    "notes": "Smooth shift, no issues"
}
```

## Task Management Endpoints

### GET /tasks
List tasks (filtered by user permissions).

**Query Parameters:**
- `status`: Filter by status
- `assigned_to`: Filter by assignee
- `priority`: Filter by priority
- `due_date`: Filter by due date

### POST /tasks
Create new task (Manager only).

**Request Body:**
```json
{
    "title": "Restock Coffee Mugs",
    "description": "Check inventory and restock display",
    "assigned_to": "uuid",
    "priority": "high",
    "due_date": "2024-01-21T16:00:00Z"
}
```

### PUT /tasks/{id}
Update task.

### PUT /tasks/{id}/status
Update task status.

**Request Body:**
```json
{
    "status": "completed",
    "notes": "Task completed successfully"
}
```

## Analytics Endpoints

### GET /analytics/dashboard
Get dashboard analytics (Manager only).

**Query Parameters:**
- `date_from`: Start date (default: 30 days ago)
- `date_to`: End date (default: today)
- `store_id`: Filter by store

**Response:**
```json
{
    "success": true,
    "data": {
        "revenue": {
            "total": 15250.75,
            "today": 425.50,
            "week": 2840.25,
            "month": 8950.00,
            "growth": 12.5
        },
        "orders": {
            "total": 1250,
            "today": 18,
            "pending": 5,
            "completed": 1245
        },
        "products": {
            "total": 125,
            "low_stock": 8,
            "top_selling": [
                {
                    "product_id": "uuid",
                    "name": "Coffee Mug",
                    "sales": 245
                }
            ]
        },
        "staff": {
            "total": 12,
            "active": 8,
            "performance": [
                {
                    "user_id": "uuid",
                    "name": "John Staff",
                    "sales": 2450.75
                }
            ]
        }
    }
}
```

### GET /analytics/sales
Get detailed sales analytics.

### GET /analytics/products
Get product performance analytics.

### GET /analytics/staff
Get staff performance analytics.

## Notification Endpoints

### GET /notifications
List user notifications.

### PUT /notifications/{id}/read
Mark notification as read.

### PUT /notifications/read-all
Mark all notifications as read.

## Export Endpoints

### GET /export/orders
Export orders data.

**Query Parameters:**
- `format`: csv|xlsx|pdf
- `date_from`: Start date
- `date_to`: End date
- `status`: Filter by status

### GET /export/inventory
Export inventory data.

### GET /export/sales-report
Export sales report.

## Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `VALIDATION_ERROR` | 400 | Request validation failed |
| `UNAUTHORIZED` | 401 | Authentication required |
| `FORBIDDEN` | 403 | Insufficient permissions |
| `NOT_FOUND` | 404 | Resource not found |
| `CONFLICT` | 409 | Resource conflict (e.g., duplicate SKU) |
| `RATE_LIMITED` | 429 | Too many requests |
| `INTERNAL_ERROR` | 500 | Server error |

## Permission Requirements

| Endpoint Pattern | Required Permission |
|-----------------|-------------------|
| `GET /orders` (own) | `view_my_orders` |
| `GET /orders` (all) | `view_all_orders` |
| `POST /orders` | `create_order` |
| `PUT /orders` | `update_order` |
| `DELETE /orders` | `delete_order` |
| `GET /inventory` | `view_inventory` |
| `PUT /inventory` | `manage_inventory` |
| `POST /users` | `manage_staff` |
| `GET /analytics/*` | `view_analytics` |
| `POST /pos/*` | `access_pos` |
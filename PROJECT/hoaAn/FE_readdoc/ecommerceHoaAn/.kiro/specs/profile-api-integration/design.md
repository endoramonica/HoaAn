# Design Document: Profile API Integration

## Overview

Tài liệu này mô tả thiết kế kỹ thuật cho việc tích hợp API vào ProfilePage. Mục tiêu là thay thế dữ liệu mock hiện tại bằng dữ liệu thực từ backend API, sử dụng Orval-generated API client.

### Key Design Decisions

1. **Sử dụng Orval API Client**: Tận dụng API client đã được generate từ OpenAPI spec
2. **Custom Hooks Pattern**: Tạo các custom hooks để quản lý state và API calls
3. **Tái sử dụng hooks hiện có**: Sử dụng `useAuth`, `useCustomerAddress`, `useOrders` đã có
4. **Tạo hook mới**: `useCustomerProfile` để quản lý thông tin customer

## Architecture

```mermaid
graph TB
    subgraph "ProfilePage Components"
        PP[ProfilePage]
        PT[Profile Tab]
        OT[Orders Tab]
        AT[Addresses Tab]
        NT[Notifications Tab]
        ST[Security Tab]
    end
    
    subgraph "Custom Hooks"
        UCP[useCustomerProfile]
        UCA[useCustomerAddress]
        UO[useOrders]
        UA[useAuth]
    end
    
    subgraph "Orval API Client"
        API[getVietCommerceAPI]
    end
    
    subgraph "Backend Endpoints"
        CE[/api/v1/CustomerAdmin/:id]
        AE[/api/v1/customer/addresses]
        OE[/api/v1/Order/my-orders]
        AUE[/api/v1/Auth/*]
    end
    
    PP --> PT
    PP --> OT
    PP --> AT
    PP --> NT
    PP --> ST
    
    PT --> UCP
    PT --> UA
    OT --> UO
    AT --> UCA
    ST --> UA
    
    UCP --> API
    UCA --> API
    UO --> API
    UA --> API
    
    API --> CE
    API --> AE
    API --> OE
    API --> AUE
```

## Components and Interfaces

### 1. useCustomerProfile Hook (New)

```typescript
interface UseCustomerProfileReturn {
  customer: CustomerDetailDto | null;
  isLoading: boolean;
  error: string | null;
  loadProfile: () => Promise<void>;
  updateProfile: (data: UpdateCustomerRequest) => Promise<boolean>;
  statistics: CustomerStatistics | null;
}

interface CustomerStatistics {
  totalOrders: number;
  totalSpent: number;
  tier: string;
  loyaltyPoints: number;
}
```

### 2. ProfilePage Component Updates

```typescript
// Current: Uses hardcoded useState
const [userInfo, setUserInfo] = useState({...});

// New: Uses useCustomerProfile hook
const { customer, isLoading, updateProfile } = useCustomerProfile();
```

### 3. API Endpoints Mapping

| Feature | Endpoint | Method | Hook |
|---------|----------|--------|------|
| Get Profile | `/api/v1/CustomerAdmin/{id}` | GET | useCustomerProfile |
| Update Profile | `/api/v1/CustomerAdmin/{id}` | PUT | useCustomerProfile |
| Get Addresses | `/api/v1/customer/addresses` | GET | useCustomerAddress |
| Create Address | `/api/v1/customer/addresses` | POST | useCustomerAddress |
| Update Address | `/api/v1/customer/addresses/{id}` | PUT | useCustomerAddress |
| Delete Address | `/api/v1/customer/addresses/{id}` | DELETE | useCustomerAddress |
| Set Default Address | `/api/v1/customer/addresses/{id}/set-default` | POST | useCustomerAddress |
| Get My Orders | `/api/v1/Order/my-orders` | GET | useOrders |
| Get Order Detail | `/api/v1/Order/{id}` | GET | useOrders |
| Change Password | `/api/v1/Auth/change-password` | POST | useAuth |
| Logout | `/api/v1/Auth/logout` | POST | useAuth |

## Data Models

### CustomerDetailDto (from backend)

```typescript
interface CustomerDetailDto {
  id?: string;
  name?: string | null;
  email?: string | null;
  phone?: string | null;
  loyaltyPoints?: number;
  tier?: string | null;
  isActive?: boolean;
  userId?: string | null;
  storeId?: string | null;
  storeName?: string | null;
  tenantId?: string;
  createdAt?: string;
  updatedAt?: string;
  totalOrders?: number;
  totalSpent?: number;
  totalInteractions?: number;
  addresses?: CustomerAddressDto[] | null;
}
```

### AddressResponseDto (from backend)

```typescript
interface AddressResponseDto {
  id?: string;
  customerId?: string;
  streetAddress?: string | null;
  city?: string | null;
  postalCode?: string | null;
  state?: string | null;
  country?: string | null;
  addressType?: AddressType;
  recipientName?: string | null;
  phoneNumber?: string | null;
  email?: string | null;
  isDefault?: boolean;
  isPrimary?: boolean;
  isActive?: boolean;
  fullAddress?: string | null;
}
```

### OrderDetailDto (from backend)

```typescript
interface OrderDetailDto {
  orderId?: string;
  orderNumber?: string | null;
  customerId?: string;
  customerName?: string | null;
  status?: OrderStatus;
  statusText?: string | null;
  subTotal?: number;
  shippingFee?: number;
  totalAmount?: number;
  createdAt?: string;
  items?: OrderItemDTO[] | null;
  shipping?: OrderShippingDto;
}
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Profile data display completeness
*For any* valid CustomerDetailDto returned from the API, the ProfilePage SHALL display all non-null fields including name, email, phone, loyaltyPoints, and tier in the appropriate UI locations.
**Validates: Requirements 1.1, 1.2**

### Property 2: Profile update round-trip consistency
*For any* valid UpdateCustomerRequest, after a successful update API call, fetching the profile again SHALL return data that matches the update request for all modified fields.
**Validates: Requirements 2.2, 2.3**

### Property 3: Address CRUD operations maintain list integrity
*For any* sequence of address create, update, delete operations, the displayed address list SHALL always reflect the current backend state, with exactly one address marked as default if any addresses exist.
**Validates: Requirements 3.1, 3.2, 3.3, 3.4, 3.5**

### Property 4: Address operation failure preserves state
*For any* failed address operation (create, update, delete, set-default), the UI state SHALL remain unchanged from before the operation was attempted.
**Validates: Requirements 3.6**

### Property 5: Orders display with correct status styling
*For any* OrderDetailDto with a valid status, the displayed order SHALL show the correct color coding: pending=yellow, processing=blue, shipped=purple, delivered=green, cancelled=red.
**Validates: Requirements 4.2, 4.4**

### Property 6: Password validation before submission
*For any* password change form submission where newPassword !== confirmNewPassword, the System SHALL prevent the API call and display a validation error.
**Validates: Requirements 5.2, 5.5**

### Property 7: Logout clears all authentication state
*For any* logout operation (successful or failed), all stored tokens and user data SHALL be cleared from localStorage/sessionStorage.
**Validates: Requirements 6.3, 6.4**

### Property 8: Customer statistics display accuracy
*For any* CustomerDetailDto, the profile header SHALL display totalOrders, totalSpent, and tier values that exactly match the API response data.
**Validates: Requirements 8.1, 8.2, 8.3**

### Property 9: Notification settings persistence
*For any* notification toggle action, the setting value SHALL persist across page reloads.
**Validates: Requirements 7.2**

## Error Handling

### Error Types and Handling Strategy

| Error Type | HTTP Status | User Message | Action |
|------------|-------------|--------------|--------|
| Unauthorized | 401 | "Phiên đăng nhập hết hạn" | Redirect to login |
| Forbidden | 403 | "Bạn không có quyền truy cập" | Show error, stay on page |
| Not Found | 404 | "Không tìm thấy dữ liệu" | Show error with retry |
| Validation Error | 400 | Backend message | Show inline errors |
| Server Error | 500 | "Lỗi hệ thống, vui lòng thử lại" | Show error with retry |
| Network Error | - | "Không thể kết nối server" | Show error with retry |

### Error Extraction Helper

```typescript
const getErrorMessage = (err: any): string => {
  if (err?.body?.message) return err.body.message;
  if (err?.body?.error) return err.body.error;
  if (err?.body?.title) return err.body.title;
  if (err?.statusText) return err.statusText;
  if (err?.message) return err.message;
  return 'Đã xảy ra lỗi';
};
```

## Testing Strategy

### Dual Testing Approach

This implementation uses both unit tests and property-based tests:

1. **Unit Tests**: Verify specific examples, edge cases, and error conditions
2. **Property-Based Tests**: Verify universal properties that should hold across all inputs

### Property-Based Testing Library

- **Library**: fast-check (for TypeScript/JavaScript)
- **Minimum iterations**: 100 per property test

### Test Categories

#### Unit Tests
- Component rendering with mock data
- Hook state management
- Error handling scenarios
- Loading states

#### Property-Based Tests
- Data transformation correctness
- API request/response mapping
- State consistency after operations
- Validation logic

### Test File Structure

```
src/
├── lib/
│   └── hooks/
│       ├── useCustomerProfile.ts
│       ├── useCustomerProfile.test.ts      # Unit tests
│       └── useCustomerProfile.property.test.ts  # Property tests
├── components/
│   ├── ProfilePage.tsx
│   └── ProfilePage.test.tsx
```

### Property Test Annotations

Each property-based test MUST include a comment referencing the correctness property:
```typescript
// **Feature: profile-api-integration, Property 1: Profile data display completeness**
```

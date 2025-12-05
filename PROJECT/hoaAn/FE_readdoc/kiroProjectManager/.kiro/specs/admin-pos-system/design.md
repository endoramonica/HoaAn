# Design Document

## Overview

This design document outlines the architecture for building a complete Admin Dashboard & POS System for a Vietnamese Worship Supplies Store. The system builds upon existing infrastructure including Orval-generated API clients, JWT authentication, and token management. The design focuses on creating a comprehensive user interface with the following key layers:

1. **Routing Layer**: HashRouter with protected routes and authentication guards
2. **Layout Layer**: Responsive sidebar navigation and top bar
3. **Page Layer**: Feature-specific pages for dashboard, POS, products, customers, orders, employees, inventory, marketing, notifications, and settings
4. **Component Layer**: Reusable UI components including forms, tables, modals, and cards
5. **Hook Layer**: React Query hooks wrapping service methods for data fetching and mutations
6. **Service Layer**: Business logic wrappers around Orval-generated API clients
7. **Storage Layer**: LocalStorage for features without backend APIs (marketing posts, notifications)
8. **AI Integration Layer**: Gemini/OpenAI integration for marketing content generation

The design emphasizes responsive design, dark mode support, Vietnamese localization, and excellent user experience.

## Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     React Pages                              │
│  (Dashboard, POS, Products, Customers, Orders, etc.)        │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                  Layout Components                           │
│              (Sidebar, TopBar, ProtectedRoute)              │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                  React Query Hooks                           │
│    (useProducts, useCustomers, useOrders, useAuth, etc.)    │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                   Service Layer                              │
│  (ProductService, CustomerService, OrderService, etc.)      │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│         Orval-Generated API Client (Existing)                │
│           (getApiProduct, getApiAdminCustomer, etc.)        │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│         Axios Instance with Interceptors (Existing)          │
│              (Token management, error handling)              │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                    Backend API                               │
└─────────────────────────────────────────────────────────────┘

     ┌──────────────────┐              ┌──────────────────┐
     │  LocalStorage    │              │   AI Services    │
     │  (Marketing,     │              │  (Gemini/OpenAI) │
     │  Notifications)  │              │                  │
     └──────────────────┘              └──────────────────┘
```


### Directory Structure

```
src/
├── components/
│   ├── ui/
│   │   ├── Button.tsx
│   │   ├── Input.tsx
│   │   ├── Card.tsx
│   │   ├── Table.tsx
│   │   ├── Modal.tsx
│   │   ├── Select.tsx
│   │   ├── Badge.tsx
│   │   └── LoadingSkeleton.tsx
│   ├── forms/
│   │   ├── ProductForm.tsx
│   │   ├── CustomerForm.tsx
│   │   ├── OrderForm.tsx
│   │   ├── EmployeeForm.tsx
│   │   └── MarketingForm.tsx
│   ├── tables/
│   │   ├── ProductTable.tsx
│   │   ├── CustomerTable.tsx
│   │   ├── OrderTable.tsx
│   │   └── EmployeeTable.tsx
│   └── charts/
│       ├── RevenueChart.tsx
│       ├── SalesChart.tsx
│       └── InventoryChart.tsx
│
├── layout/
│   ├── Layout.tsx
│   ├── Sidebar.tsx
│   ├── TopBar.tsx
│   └── ProtectedRoute.tsx
│
├── pages/
│   ├── LoginPage.tsx
│   ├── DashboardPage.tsx
│   ├── POSPage.tsx
│   ├── ProductsPage.tsx
│   ├── CustomersPage.tsx
│   ├── OrdersPage.tsx
│   ├── EmployeesPage.tsx
│   ├── InventoryPage.tsx
│   ├── MarketingPage.tsx
│   ├── MarketingCreatePage.tsx
│   ├── NotificationsPage.tsx
│   └── SettingsPage.tsx
│
├── router/
│   └── index.tsx
│
├── lib/
│   ├── api/ (existing)
│   │   ├── client.ts
│   │   ├── orval-client.ts
│   │   ├── errors.ts
│   │   └── types.ts
│   ├── services/
│   │   ├── authService.ts (existing)
│   │   ├── productService.ts
│   │   ├── customerService.ts
│   │   ├── orderService.ts
│   │   ├── employeeService.ts
│   │   ├── inventoryService.ts
│   │   ├── marketingService.ts (localStorage)
│   │   ├── notificationService.ts (localStorage)
│   │   └── aiService.ts
│   ├── hooks/
│   │   ├── useAuth.ts
│   │   ├── useProducts.ts
│   │   ├── useCustomers.ts
│   │   ├── useOrders.ts
│   │   ├── useEmployees.ts
│   │   ├── useInventory.ts
│   │   ├── useMarketing.ts
│   │   ├── useNotifications.ts
│   │   └── useTheme.ts
│   └── utils/
│       ├── formatCurrency.ts
│       ├── formatDate.ts
│       └── constants.ts
│
└── api/generated-orval/ (existing)
    ├── product/
    ├── admin-customer/
    ├── auth/
    ├── employees/
    ├── inventory/
    ├── payment/
    └── schemas/
```

## Components and Interfaces

### 1. Routing System

**Router Configuration** (`src/router/index.tsx`):
- Uses HashRouter for client-side routing
- Defines all application routes
- Wraps protected routes with ProtectedRoute component
- Applies Layout to authenticated routes

**ProtectedRoute Component**:
```typescript
interface ProtectedRouteProps {
  children: React.ReactNode;
}

// Checks authentication status
// Redirects to /login if not authenticated
// Renders children if authenticated
```

**Routes**:
- `/login` - Public route for authentication
- `/` - Dashboard (protected)
- `/pos` - Point of Sale interface (protected)
- `/products` - Product management (protected)
- `/customers` - Customer management (protected)
- `/orders` - Order management (protected)
- `/employees` - Employee management (protected)
- `/inventory` - Inventory management (protected)
- `/marketing` - Marketing posts list (protected)
- `/marketing/create` - Create marketing content (protected)
- `/notifications` - Notifications (protected)
- `/settings` - Settings (protected)

### 2. Layout System

**Layout Component** (`src/layout/Layout.tsx`):
- Wraps all authenticated pages
- Contains Sidebar and TopBar
- Manages responsive behavior
- Applies dark mode classes

**Sidebar Component** (`src/layout/Sidebar.tsx`):
```typescript
interface SidebarProps {
  isOpen: boolean;
  onClose: () => void;
}

// Navigation items with icons and labels
// Active route highlighting
// Collapsible on mobile
// Dark mode support
```

**TopBar Component** (`src/layout/TopBar.tsx`):
```typescript
interface TopBarProps {
  onMenuClick: () => void;
}

// User profile display
// Logout button
// Dark mode toggle
// Mobile menu button
```

### 3. Service Layer

All services follow this pattern:

**ProductService** (`src/lib/services/productService.ts`):
```typescript
import { getApiProduct, postApiProduct, putApiProduct, deleteApiProduct } from '@/api/generated-orval/product/product';

class ProductService {
  async getProducts(params?: QueryParams): Promise<Product[]> {
    try {
      const response = await getApiProduct(params);
      return response.data;
    } catch (error) {
      console.error('Failed to fetch products:', error);
      throw error;
    }
  }

  async getProduct(id: string): Promise<Product> {
    try {
      const response = await getApiProduct({ id });
      return response.data;
    } catch (error) {
      console.error(`Failed to fetch product ${id}:`, error);
      throw error;
    }
  }

  async createProduct(data: CreateProductDto): Promise<Product> {
    try {
      const response = await postApiProduct(data);
      return response.data;
    } catch (error) {
      console.error('Failed to create product:', error);
      throw error;
    }
  }

  async updateProduct(id: string, data: UpdateProductDto): Promise<Product> {
    try {
      const response = await putApiProduct(id, data);
      return response.data;
    } catch (error) {
      console.error(`Failed to update product ${id}:`, error);
      throw error;
    }
  }

  async deleteProduct(id: string): Promise<void> {
    try {
      await deleteApiProduct(id);
    } catch (error) {
      console.error(`Failed to delete product ${id}:`, error);
      throw error;
    }
  }
}

export const productService = new ProductService();
```

**Similar services**:
- CustomerService (uses admin-customer API)
- OrderService (uses order API)
- EmployeeService (uses employees API)
- InventoryService (uses inventory API)

**LocalStorage Services**:

**MarketingService** (`src/lib/services/marketingService.ts`):
```typescript
interface MarketingPost {
  id: string;
  title: string;
  content: string;
  image?: string; // base64
  createdAt: string;
  updatedAt: string;
}

class MarketingService {
  private STORAGE_KEY = 'marketing_posts';

  getPosts(): MarketingPost[] {
    const data = localStorage.getItem(this.STORAGE_KEY);
    return data ? JSON.parse(data) : [];
  }

  getPost(id: string): MarketingPost | null {
    const posts = this.getPosts();
    return posts.find(p => p.id === id) || null;
  }

  createPost(post: Omit<MarketingPost, 'id' | 'createdAt' | 'updatedAt'>): MarketingPost {
    const posts = this.getPosts();
    const newPost: MarketingPost = {
      ...post,
      id: crypto.randomUUID(),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };
    posts.push(newPost);
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(posts));
    return newPost;
  }

  updatePost(id: string, data: Partial<MarketingPost>): MarketingPost {
    const posts = this.getPosts();
    const index = posts.findIndex(p => p.id === id);
    if (index === -1) throw new Error('Post not found');
    
    posts[index] = {
      ...posts[index],
      ...data,
      updatedAt: new Date().toISOString(),
    };
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(posts));
    return posts[index];
  }

  deletePost(id: string): void {
    const posts = this.getPosts();
    const filtered = posts.filter(p => p.id !== id);
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(filtered));
  }
}

export const marketingService = new MarketingService();
```

**NotificationService** - Similar localStorage CRUD pattern

### 4. AI Integration

**AIService** (`src/lib/services/aiService.ts`):
```typescript
interface AIGenerateTextRequest {
  prompt: string;
  model: 'gemini' | 'openai';
}

interface AIGenerateImageRequest {
  prompt: string;
}

class AIService {
  async generateText(request: AIGenerateTextRequest): Promise<string> {
    if (request.model === 'gemini') {
      return this.generateTextWithGemini(request.prompt);
    } else {
      return this.generateTextWithOpenAI(request.prompt);
    }
  }

  private async generateTextWithGemini(prompt: string): Promise<string> {
    // Call Gemini API (gemini-2.5-flash)
    const apiKey = import.meta.env.VITE_GEMINI_API_KEY;
    // Implementation details
  }

  private async generateTextWithOpenAI(prompt: string): Promise<string> {
    // Call OpenAI API (gpt-4o-mini)
    const apiKey = import.meta.env.VITE_OPENAI_API_KEY;
    // Implementation details
  }

  async generateImage(request: AIGenerateImageRequest): Promise<string> {
    // Call Gemini API for image generation
    // Return base64 encoded image
    const apiKey = import.meta.env.VITE_GEMINI_API_KEY;
    // Implementation details
  }
}

export const aiService = new AIService();
```

### 5. React Query Hooks

**useProducts Hook** (`src/lib/hooks/useProducts.ts`):
```typescript
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { productService } from '@/lib/services/productService';

export function useProducts(params?: QueryParams) {
  return useQuery({
    queryKey: ['products', params],
    queryFn: () => productService.getProducts(params),
  });
}

export function useProduct(id: string) {
  return useQuery({
    queryKey: ['products', id],
    queryFn: () => productService.getProduct(id),
    enabled: !!id,
  });
}

export function useCreateProduct() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateProductDto) => productService.createProduct(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
    },
  });
}

export function useUpdateProduct() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateProductDto }) => 
      productService.updateProduct(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
    },
  });
}

export function useDeleteProduct() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => productService.deleteProduct(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
    },
  });
}
```

**Similar hooks**:
- useCustomers, useCustomer, useCreateCustomer, useUpdateCustomer, useDeleteCustomer
- useOrders, useOrder, useCreateOrder, useUpdateOrder, useUpdateOrderStatus
- useEmployees, useEmployee, useCreateEmployee, useUpdateEmployee, useDeleteEmployee
- useInventory, useInventoryItem, useUpdateInventory

**LocalStorage hooks**:
- useMarketing, useMarketingPost, useCreateMarketingPost, useUpdateMarketingPost, useDeleteMarketingPost
- useNotifications, useCreateNotification, useMarkNotificationRead, useDeleteNotification

### 6. UI Components

**Button Component** (`src/components/ui/Button.tsx`):
```typescript
interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'danger' | 'ghost';
  size?: 'sm' | 'md' | 'lg';
  isLoading?: boolean;
  icon?: React.ReactNode;
}

// Tailwind-styled button with variants
// Loading state with spinner
// Icon support
// Dark mode support
```

**Table Component** (`src/components/ui/Table.tsx`):
```typescript
interface Column<T> {
  key: string;
  header: string;
  render: (item: T) => React.ReactNode;
}

interface TableProps<T> {
  data: T[];
  columns: Column<T>[];
  onRowClick?: (item: T) => void;
  isLoading?: boolean;
}

// Responsive table with loading states
// Sortable columns
// Row click handlers
// Mobile card layout fallback
```

**Modal Component** (`src/components/ui/Modal.tsx`):
```typescript
interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  children: React.ReactNode;
  footer?: React.ReactNode;
}

// Overlay with backdrop
// Close on escape or backdrop click
// Accessible with focus trap
// Dark mode support
```

**Card Component** (`src/components/ui/Card.tsx`):
```typescript
interface CardProps {
  title?: string;
  children: React.ReactNode;
  actions?: React.ReactNode;
  className?: string;
}

// Container with shadow and border
// Optional header with title and actions
// Dark mode support
```

## Data Models

All data models are generated by Orval from OpenAPI schemas. Key models include:

**Product**:
```typescript
interface Product {
  id: string;
  name: string;
  description?: string;
  price: number;
  categoryId: string;
  category?: Category;
  imageUrl?: string;
  stock: number;
  sku: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}
```

**Customer**:
```typescript
interface Customer {
  id: string;
  fullName: string;
  email?: string;
  phoneNumber: string;
  address?: string;
  totalOrders: number;
  totalSpent: number;
  createdAt: string;
  updatedAt: string;
}
```

**Order**:
```typescript
interface Order {
  id: string;
  customerId: string;
  customer?: Customer;
  items: OrderItem[];
  totalAmount: number;
  status: OrderStatus;
  paymentMethod: string;
  paymentStatus: PaymentStatus;
  createdAt: string;
  updatedAt: string;
}

interface OrderItem {
  id: string;
  productId: string;
  product?: Product;
  quantity: number;
  price: number;
  subtotal: number;
}

enum OrderStatus {
  PENDING = 'PENDING',
  PROCESSING = 'PROCESSING',
  COMPLETED = 'COMPLETED',
  CANCELLED = 'CANCELLED',
}
```

**Employee**:
```typescript
interface Employee {
  id: string;
  fullName: string;
  email: string;
  phoneNumber?: string;
  role: EmployeeRole;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

enum EmployeeRole {
  ADMIN = 'ADMIN',
  MANAGER = 'MANAGER',
  CASHIER = 'CASHIER',
  STAFF = 'STAFF',
}
```

**Inventory**:
```typescript
interface InventoryItem {
  id: string;
  productId: string;
  product?: Product;
  quantity: number;
  minQuantity: number;
  maxQuantity: number;
  location?: string;
  lastRestocked?: string;
}
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Authentication redirect
*For any* unauthenticated user attempting to access a protected route, the system should redirect them to the login page.
**Validates: Requirements 1.1**

### Property 2: Token persistence
*For any* successful login, the authentication tokens should be stored and retrievable for subsequent requests.
**Validates: Requirements 1.2**

### Property 3: Sidebar navigation highlighting
*For any* active route, the corresponding navigation item in the sidebar should be highlighted.
**Validates: Requirements 2.3**

### Property 4: Responsive layout adaptation
*For any* screen width below tablet size, the sidebar should collapse into a mobile menu.
**Validates: Requirements 2.2**

### Property 5: Dashboard metrics display
*For any* dashboard load, all key metrics (revenue, orders, customers, products) should be displayed.
**Validates: Requirements 3.1**

### Property 6: POS cart calculation
*For any* cart with items, the total amount should equal the sum of all item subtotals.
**Validates: Requirements 4.3**

### Property 7: Product CRUD operations
*For any* product operation (create, update, delete), the corresponding API endpoint should be called with correct parameters.
**Validates: Requirements 5.2, 5.3, 5.4**

### Property 8: Customer search filtering
*For any* search query, only customers matching the query in name, phone, or email should be displayed.
**Validates: Requirements 6.5**

### Property 9: Order status filtering
*For any* selected status filter, only orders with that status should be displayed.
**Validates: Requirements 7.4**

### Property 10: Employee role display
*For any* employee with a role, the role badge should be displayed in the employee list.
**Validates: Requirements 8.5**

### Property 11: Inventory low stock warning
*For any* inventory item with quantity below minimum, a warning indicator should be displayed.
**Validates: Requirements 9.2**

### Property 12: Marketing post persistence
*For any* saved marketing post, it should be stored in localStorage and retrievable on subsequent loads.
**Validates: Requirements 10.5**

### Property 13: Notification read status
*For any* clicked notification, its read status should be updated to true.
**Validates: Requirements 11.3**

### Property 14: Dark mode persistence
*For any* dark mode toggle, the preference should be saved to localStorage and applied on next load.
**Validates: Requirements 12.2, 19.2**

### Property 15: Query cache invalidation
*For any* successful mutation, related query caches should be invalidated to trigger refetch.
**Validates: Requirements 13.4**

### Property 16: VND currency formatting
*For any* monetary value displayed, it should be formatted using Vietnamese Dong currency format.
**Validates: Requirements 14.1, 14.2**

### Property 17: Mobile responsive tables
*For any* table viewed on mobile, it should either be horizontally scrollable or use card layout.
**Validates: Requirements 15.4**

### Property 18: Loading state display
*For any* data fetching operation, a loading skeleton or spinner should be displayed.
**Validates: Requirements 16.1**

### Property 19: Error boundary catching
*For any* component error, it should be caught by an error boundary and display an error message.
**Validates: Requirements 16.4**

### Property 20: Confirmation modal for deletions
*For any* delete action, a confirmation modal should be displayed before execution.
**Validates: Requirements 17.1, 17.2**

## Error Handling

### Error Display Strategy

1. **API Errors**:
   - Use existing error handling system from `src/lib/api/errors.ts`
   - Display Vietnamese error messages
   - Show retry option for network errors
   - Log errors to console in development

2. **Form Validation Errors**:
   - Display inline validation messages
   - Highlight invalid fields
   - Prevent submission until valid

3. **Component Errors**:
   - Wrap pages in error boundaries
   - Display fallback UI with error message
   - Provide option to reload page

4. **Loading States**:
   - Show skeletons for content loading
   - Disable buttons during mutations
   - Display progress indicators

### User Feedback

1. **Success Notifications** (using sonner):
   - "Sản phẩm đã được tạo thành công"
   - "Đơn hàng đã được cập nhật"
   - "Xóa thành công"

2. **Error Notifications**:
   - "Không thể kết nối đến server"
   - "Dữ liệu không hợp lệ"
   - "Bạn không có quyền thực hiện thao tác này"

3. **Confirmation Dialogs**:
   - "Bạn có chắc chắn muốn xóa sản phẩm này?"
   - "Hành động này không thể hoàn tác"

## Testing Strategy

### Unit Testing

**Component Tests**:
- Test button variants and states
- Test form validation
- Test table rendering and sorting
- Test modal open/close behavior

**Hook Tests**:
- Test query hooks fetch data correctly
- Test mutation hooks call service methods
- Test cache invalidation after mutations
- Mock service layer

**Service Tests**:
- Test service methods call Orval API functions
- Test error logging
- Test data transformation
- Mock Orval-generated functions

**LocalStorage Tests**:
- Test marketing service CRUD operations
- Test notification service operations
- Test data persistence

### Integration Testing

**Page Tests**:
- Test login flow with authentication
- Test product CRUD flow
- Test order creation flow
- Test POS checkout flow

**Routing Tests**:
- Test protected route redirects
- Test navigation between pages
- Test route parameter handling

### Property-Based Testing

We will use **fast-check** for property-based testing. Each test should run a minimum of 100 iterations.

**Property Tests**:

1. **Currency Formatting Property**:
   - Generate random numbers
   - Format each as VND
   - Verify format matches Vietnamese currency pattern

2. **Cart Total Calculation Property**:
   - Generate random cart items
   - Calculate total
   - Verify total equals sum of subtotals

3. **Search Filtering Property**:
   - Generate random customer data
   - Apply random search queries
   - Verify all results match query

4. **LocalStorage Persistence Property**:
   - Generate random marketing posts
   - Save to localStorage
   - Verify retrieval returns same data

Each property-based test MUST be tagged with:
```typescript
// **Feature: admin-pos-system, Property X: [property description]**
```

### Testing Tools

- **Vitest**: Unit test runner
- **fast-check**: Property-based testing
- **React Testing Library**: Component testing
- **MSW**: API mocking
- **@tanstack/react-query**: Query testing utilities

## Performance Considerations

### Code Splitting

- Lazy load pages with React.lazy()
- Split routes for faster initial load
- Dynamic imports for heavy components

### React Query Optimization

- Set appropriate stale times (5 minutes for products, 1 minute for orders)
- Use cache time to balance freshness and performance
- Implement pagination for large lists
- Use optimistic updates for better UX

### Image Optimization

- Lazy load images
- Use appropriate image sizes
- Compress images before upload
- Use placeholder images during load

### Bundle Size

- Tree-shake unused Orval functions
- Use dynamic imports for AI services
- Minimize third-party dependencies
- Use production builds

## Security Considerations

### Authentication

- Use existing JWT token management
- Implement route guards
- Clear tokens on logout
- Handle token expiration

### Data Validation

- Validate all form inputs
- Sanitize user input
- Use TypeScript for type safety
- Validate API responses

### Sensitive Data

- Don't log sensitive information
- Don't expose API keys in client code
- Use environment variables for configuration
- Implement proper error messages without exposing internals

## Deployment Considerations

### Environment Configuration

- Development: Local API, verbose logging
- Staging: Staging API, moderate logging
- Production: Production API, minimal logging

### Build Optimization

- Enable production mode
- Minify code
- Optimize assets
- Generate source maps for debugging

### Monitoring

- Track page load times
- Monitor API response times
- Log errors to external service
- Track user interactions

## UI/UX Guidelines

### Color Scheme

**Light Mode**:
- Primary: Blue (#3B82F6)
- Secondary: Gray (#6B7280)
- Success: Green (#10B981)
- Warning: Yellow (#F59E0B)
- Danger: Red (#EF4444)
- Background: White (#FFFFFF)
- Surface: Gray (#F9FAFB)

**Dark Mode**:
- Primary: Blue (#60A5FA)
- Secondary: Gray (#9CA3AF)
- Success: Green (#34D399)
- Warning: Yellow (#FBBF24)
- Danger: Red (#F87171)
- Background: Dark Gray (#111827)
- Surface: Dark Gray (#1F2937)

### Typography

- Font Family: Inter, system-ui, sans-serif
- Headings: Bold, larger sizes
- Body: Regular, readable size (16px)
- Small text: 14px
- Tiny text: 12px

### Spacing

- Use Tailwind spacing scale (4px increments)
- Consistent padding and margins
- Adequate white space
- Responsive spacing

### Icons

- Use lucide-react consistently
- 20px for inline icons
- 24px for standalone icons
- 16px for small icons

### Responsive Breakpoints

- Mobile: < 640px
- Tablet: 640px - 1024px
- Desktop: > 1024px

### Accessibility

- Proper heading hierarchy
- Alt text for images
- Keyboard navigation
- Focus indicators
- ARIA labels where needed

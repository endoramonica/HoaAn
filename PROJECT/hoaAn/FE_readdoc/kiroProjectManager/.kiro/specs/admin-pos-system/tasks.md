# Implementation Plan
# Implementation Plan

- [x] 1. Set up React Query provider and project utilities





  - Install @tanstack/react-query-devtools for development
  - Create QueryClient instance with default options in src/main.tsx
  - Wrap application with QueryClientProvider
  - Create src/lib/utils/formatCurrency.ts for VND formatting
  - Create src/lib/utils/formatDate.ts for date formatting
  - Create src/lib/utils/constants.ts for app-wide constants
  - _Requirements: 13.1, 13.3, 14.1_

- [x] 2. Create base UI components







  - Create src/components/ui/Button.tsx with variants (primary, secondary, danger, ghost)
  - Create src/components/ui/Input.tsx with validation states
  - Create src/components/ui/Card.tsx with title and actions
  - Create src/components/ui/Badge.tsx for status indicators
  - Create src/components/ui/LoadingSkeleton.tsx for loading states
  - Create src/components/ui/Modal.tsx with backdrop and close handlers
  - Create src/components/ui/Select.tsx for dropdowns
  - All components must support dark mode via Tailwind classes
  - _Requirements: 16.1, 19.1, 20.2_


- [x] 3. Create Table component with responsive behavior






  - Create src/components/ui/Table.tsx with generic type support
  - Implement column configuration with render functions
  - Add loading state with skeletons
  - Add row click handlers
  - Implement mobile card layout fallback for small screens
  - Add dark mode support
  - _Requirements: 15.4, 16.1, 19.1_

- [ ] 4. Create service layer for products


  - Create src/lib/services/productService.ts
  - Implement getProducts method using Orval-generated getApiProduct
  - Implement getProduct(id) method
  - Implement createProduct method using postApiProduct
  - Implement updateProduct method using putApiProduct
  - Implement deleteProduct method using deleteApiProduct
  - Add error logging to all methods
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 18.1, 18.2_


- [ ] 5. Create React Query hooks for products

  - Create src/lib/hooks/useProducts.ts
  - Implement useProducts query hook with optional params
  - Implement useProduct(id) query hook with enabled condition
  - Implement useCreateProduct mutation hook with cache invalidation
  - Implement useUpdateProduct mutation hook with cache invalidation
  - Implement useDeleteProduct mutation hook with cache invalidation
  - _Requirements: 13.1, 13.2, 13.4, 13.5_

- [ ] 6. Create service layer for customers
  - Create src/lib/services/customerService.ts
  - Implement getCustomers method using Orval-generated getApiAdminCustomer
  - Implement getCustomer(id) method
  - Implement createCustomer method
  - Implement updateCustomer method
  - Implement deleteCustomer method
  - Add error logging to all methods
  - _Requirements: 6.1, 6.2, 6.3, 18.1, 18.2_

- [ ] 7. Create React Query hooks for customers
  - Create src/lib/hooks/useCustomers.ts
  - Implement useCustomers query hook with search params
  - Implement useCustomer(id) query hook
  - Implement useCreateCustomer mutation hook
  - Implement useUpdateCustomer mutation hook
  - Implement useDeleteCustomer mutation hook
  - _Requirements: 13.1, 13.2, 13.4_

- [ ] 8. Create service layer for orders
  - Create src/lib/services/orderService.ts
  - Implement getOrders method using Orval-generated API
  - Implement getOrder(id) method
  - Implement createOrder method
  - Implement updateOrderStatus method
  - Add error logging to all methods
  - _Requirements: 7.1, 7.2, 7.3, 18.1_

- [ ] 9. Create React Query hooks for orders
  - Create src/lib/hooks/useOrders.ts
  - Implement useOrders query hook with status filter params
  - Implement useOrder(id) query hook
  - Implement useCreateOrder mutation hook
  - Implement useUpdateOrderStatus mutation hook
  - _Requirements: 13.1, 13.2, 13.4_

- [ ] 10. Create service layer for employees
  - Create src/lib/services/employeeService.ts
  - Implement getEmployees method using Orval-generated API
  - Implement getEmployee(id) method
  - Implement createEmployee method
  - Implement updateEmployee method
  - Implement deleteEmployee method
  - Add error logging to all methods
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 18.1_

- [ ] 11. Create React Query hooks for employees
  - Create src/lib/hooks/useEmployees.ts
  - Implement useEmployees query hook
  - Implement useEmployee(id) query hook
  - Implement useCreateEmployee mutation hook
  - Implement useUpdateEmployee mutation hook
  - Implement useDeleteEmployee mutation hook
  - _Requirements: 13.1, 13.2, 13.4_

- [ ] 12. Create service layer for inventory
  - Create src/lib/services/inventoryService.ts
  - Implement getInventory method using Orval-generated API
  - Implement getInventoryItem(id) method
  - Implement updateInventory method
  - Add error logging to all methods
  - _Requirements: 9.1, 9.3, 18.1_

- [ ] 13. Create React Query hooks for inventory
  - Create src/lib/hooks/useInventory.ts
  - Implement useInventory query hook with filter params
  - Implement useInventoryItem(id) query hook
  - Implement useUpdateInventory mutation hook
  - _Requirements: 13.1, 13.2, 13.4_

- [ ] 14. Create localStorage service for marketing
  - Create src/lib/services/marketingService.ts
  - Implement MarketingPost interface with id, title, content, image, timestamps
  - Implement getPosts method reading from localStorage
  - Implement getPost(id) method
  - Implement createPost method with UUID generation
  - Implement updatePost method
  - Implement deletePost method
  - _Requirements: 10.1, 10.5_

- [ ] 15. Create React Query hooks for marketing (localStorage)
  - Create src/lib/hooks/useMarketing.ts
  - Implement useMarketingPosts query hook reading from localStorage
  - Implement useMarketingPost(id) query hook
  - Implement useCreateMarketingPost mutation hook
  - Implement useUpdateMarketingPost mutation hook
  - Implement useDeleteMarketingPost mutation hook
  - Use queryClient.setQueryData for optimistic updates
  - _Requirements: 10.1, 10.5_

- [ ] 16. Create AI service for content generation
  - Create src/lib/services/aiService.ts
  - Implement generateText method supporting both Gemini and OpenAI
  - Implement generateTextWithGemini using gemini-2.5-flash model
  - Implement generateTextWithOpenAI using gpt-4o-mini model
  - Implement generateImage method using Gemini API
  - Return base64 encoded image from generateImage
  - Read API keys from environment variables (VITE_GEMINI_API_KEY, VITE_OPENAI_API_KEY)
  - _Requirements: 10.3, 10.4_

- [ ] 17. Create localStorage service for notifications
  - Create src/lib/services/notificationService.ts
  - Implement Notification interface with id, title, message, read status, timestamp
  - Implement getNotifications method
  - Implement createNotification method
  - Implement markAsRead method
  - Implement deleteNotification method
  - _Requirements: 11.1, 11.2, 11.3, 11.5_

- [ ] 18. Create React Query hooks for notifications (localStorage)
  - Create src/lib/hooks/useNotifications.ts
  - Implement useNotifications query hook with filter params
  - Implement useCreateNotification mutation hook
  - Implement useMarkNotificationRead mutation hook
  - Implement useDeleteNotification mutation hook
  - _Requirements: 11.1, 11.2, 11.3, 11.5_

- [ ] 19. Create theme management hook
  - Create src/lib/hooks/useTheme.ts
  - Implement useTheme hook with dark mode state
  - Read initial theme from localStorage
  - Toggle theme and persist to localStorage
  - Apply 'dark' class to document root element
  - _Requirements: 12.2, 19.2, 19.3_

- [ ] 20. Create authentication hooks
  - Create src/lib/hooks/useAuth.ts
  - Implement useLogin mutation hook calling authService.login
  - Implement useLogout mutation hook calling authService.logout
  - Implement useCurrentUser query hook
  - Invalidate user query on login/logout
  - _Requirements: 1.2, 1.3, 13.1, 13.2_

- [ ] 21. Create ProtectedRoute component
  - Create src/layout/ProtectedRoute.tsx
  - Check if user is authenticated using tokenStorage
  - Redirect to /login if not authenticated
  - Render children if authenticated
  - _Requirements: 1.1_

- [ ] 22. Create Sidebar component
  - Create src/layout/Sidebar.tsx
  - Define navigation items with icons from lucide-react and routes
  - Implement active route highlighting using useLocation
  - Add collapse/expand functionality for mobile
  - Add dark mode support with Tailwind dark: classes
  - Include icons for: Dashboard, POS, Products, Customers, Orders, Employees, Inventory, Marketing, Notifications, Settings
  - _Requirements: 2.1, 2.2, 2.3, 19.1, 20.1_

- [ ] 23. Create TopBar component
  - Create src/layout/TopBar.tsx
  - Display user information from useCurrentUser hook
  - Add logout button calling useLogout hook
  - Add dark mode toggle button using useTheme hook
  - Add mobile menu button for sidebar toggle
  - Add dark mode support
  - _Requirements: 2.4, 12.2, 19.1_

- [ ] 24. Create Layout component
  - Create src/layout/Layout.tsx
  - Compose Sidebar and TopBar components
  - Manage sidebar open/close state for mobile
  - Render children in main content area
  - Apply responsive padding and spacing
  - Add dark mode support to container
  - _Requirements: 2.1, 2.2, 15.1, 15.2, 15.3, 19.1_

- [ ] 25. Create router configuration
  - Create src/router/index.tsx
  - Configure HashRouter from react-router-dom
  - Define all routes: /login, /, /pos, /products, /customers, /orders, /employees, /inventory, /marketing, /marketing/create, /notifications, /settings
  - Wrap protected routes with ProtectedRoute component
  - Apply Layout to all authenticated routes
  - _Requirements: 1.1, 1.4_

- [ ] 26. Create Login page
  - Create src/pages/LoginPage.tsx
  - Create login form with email and password fields
  - Add "Remember me" checkbox
  - Use useLogin hook for form submission
  - Display loading state during login
  - Display error messages from API
  - Redirect to dashboard on successful login
  - Add dark mode support
  - _Requirements: 1.2, 16.3, 19.1_

- [ ] 27. Create Dashboard page with analytics
  - Create src/pages/DashboardPage.tsx
  - Display key metrics cards: total revenue, orders, customers, products
  - Create src/components/charts/RevenueChart.tsx using recharts
  - Create src/components/charts/SalesChart.tsx using recharts
  - Display recent orders list with status badges
  - Display top-selling products list
  - Use loading skeletons while data is fetching
  - Format currency values in VND
  - Add dark mode support to charts
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 14.1, 14.4, 16.1, 19.1, 19.4_

- [ ] 28. Create POS page
  - Create src/pages/POSPage.tsx
  - Create product search and selection interface
  - Create cart component with item list
  - Add quantity controls for cart items
  - Calculate and display total in VND format
  - Create checkout button calling useCreateOrder hook
  - Clear cart on successful order creation
  - Display success notification using sonner
  - Add dark mode support
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 14.1, 16.5, 19.1_

- [ ] 29. Create Products page with CRUD
  - Create src/pages/ProductsPage.tsx
  - Use useProducts hook to fetch and display products
  - Create src/components/tables/ProductTable.tsx
  - Create src/components/forms/ProductForm.tsx with all fields
  - Add create button opening modal with ProductForm
  - Add edit button for each row opening modal with ProductForm
  - Add delete button with confirmation modal
  - Display product images as thumbnails
  - Format prices in VND
  - Add loading and error states
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 14.1, 16.1, 16.2, 17.1, 17.2, 19.1_

- [ ] 30. Create Customers page with CRUD
  - Create src/pages/CustomersPage.tsx
  - Use useCustomers hook to fetch and display customers
  - Create src/components/tables/CustomerTable.tsx
  - Create src/components/forms/CustomerForm.tsx
  - Add search input filtering by name, phone, email
  - Add create button opening modal with CustomerForm
  - Add edit button for each row
  - Add delete button with confirmation modal
  - Display customer details and order history on row click
  - Add loading and error states
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 16.1, 16.2, 17.1, 19.1_

- [ ] 31. Create Orders page with status management
  - Create src/pages/OrdersPage.tsx
  - Use useOrders hook to fetch and display orders
  - Create src/components/tables/OrderTable.tsx
  - Add status filter dropdown (All, Pending, Processing, Completed, Cancelled)
  - Display order details modal on row click
  - Show order items, customer info, and payment details
  - Add status change dropdown using useUpdateOrderStatus hook
  - Format amounts in VND
  - Display payment information
  - Add loading and error states
  - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5, 14.1, 16.1, 19.1_

- [ ] 32. Create Employees page with CRUD
  - Create src/pages/EmployeesPage.tsx
  - Use useEmployees hook to fetch and display employees
  - Create src/components/tables/EmployeeTable.tsx
  - Create src/components/forms/EmployeeForm.tsx with role selection
  - Add create button opening modal with EmployeeForm
  - Add edit button for each row
  - Add deactivate button updating employee status
  - Display role badges (Admin, Manager, Cashier, Staff)
  - Add loading and error states
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 16.1, 19.1, 20.3_

- [ ] 33. Create Inventory page with stock management
  - Create src/pages/InventoryPage.tsx
  - Use useInventory hook to fetch and display inventory
  - Create inventory table with product info and stock levels
  - Highlight low stock items with warning indicators
  - Display out-of-stock items with clear indicators
  - Add filter dropdown for stock status (All, Low Stock, Out of Stock)
  - Add stock adjustment modal using useUpdateInventory hook
  - Add loading and error states
  - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5, 16.1, 19.1, 20.3_

- [ ] 34. Create Marketing page with AI content list
  - Create src/pages/MarketingPage.tsx
  - Use useMarketingPosts hook to display saved posts
  - Display posts in card grid layout
  - Show post title, content preview, and image thumbnail
  - Add create button navigating to /marketing/create
  - Add edit button for each post
  - Add delete button with confirmation modal
  - Display timestamps for created/updated dates
  - Add loading and error states
  - _Requirements: 10.1, 10.5, 17.1, 19.1_

- [ ] 35. Create Marketing Create page with AI generation
  - Create src/pages/MarketingCreatePage.tsx
  - Create src/components/forms/MarketingForm.tsx
  - Add title and content text inputs
  - Add AI model selector (Gemini / OpenAI)
  - Add "Generate Text" button calling aiService.generateText
  - Add "Generate Image" button calling aiService.generateImage
  - Display generated text in content field
  - Display generated base64 image preview
  - Add save button using useCreateMarketingPost hook
  - Display loading states during AI generation
  - Add error handling for AI API failures
  - _Requirements: 10.2, 10.3, 10.4, 10.5, 16.1, 16.3, 19.1_

- [ ] 36. Create Notifications page
  - Create src/pages/NotificationsPage.tsx
  - Use useNotifications hook to display notifications
  - Display notifications in list with title, message, and timestamp
  - Add filter tabs (All, Unread, Read)
  - Mark notification as read on click using useMarkNotificationRead hook
  - Add delete button for each notification
  - Display unread indicator badge
  - Add loading and error states
  - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5, 19.1_

- [ ] 37. Create Settings page
  - Create src/pages/SettingsPage.tsx
  - Display current theme setting with dark mode toggle
  - Display language selector (Vietnamese / English)
  - Create user profile section with edit form
  - Save settings to localStorage
  - Update user profile using backend API
  - Display success notification on save
  - Add loading and error states
  - _Requirements: 12.1, 12.2, 12.3, 12.4, 12.5, 16.5, 19.1_

- [ ] 38. Add error boundaries to pages
  - Create src/components/ErrorBoundary.tsx
  - Wrap each page component with ErrorBoundary
  - Display fallback UI with error message
  - Add "Reload Page" button in fallback UI
  - Log errors to console in development
  - _Requirements: 16.4_

- [ ] 39. Implement confirmation modal component
  - Create src/components/ConfirmationModal.tsx
  - Accept title, message, and action callbacks as props
  - Display clear action description and consequences
  - Add confirm and cancel buttons
  - Prevent background interaction when open
  - Add dark mode support
  - _Requirements: 17.1, 17.2, 17.3, 17.4, 17.5_

- [ ] 40. Add success notifications throughout app
  - Install and configure sonner toast library
  - Add success toasts for all create operations
  - Add success toasts for all update operations
  - Add success toasts for all delete operations
  - Use Vietnamese messages
  - _Requirements: 16.5_

- [ ] 41. Implement responsive mobile layouts
  - Test all pages on mobile viewport (< 640px)
  - Ensure sidebar collapses to mobile menu
  - Ensure tables use card layout or horizontal scroll on mobile
  - Ensure forms stack vertically on mobile
  - Test all modals on mobile
  - Adjust spacing and font sizes for mobile
  - _Requirements: 15.1, 15.2, 15.3, 15.4, 15.5_

- [ ] 42. Add dark mode to all components
  - Verify all UI components support dark mode
  - Test dark mode on all pages
  - Ensure charts are visible in dark mode
  - Ensure images are visible in dark mode
  - Test dark mode toggle persistence
  - _Requirements: 19.1, 19.2, 19.3, 19.4, 19.5_

- [ ] 43. Add icons to all UI elements
  - Add lucide-react icons to navigation items
  - Add icons to all buttons
  - Add icons to status indicators
  - Add icons to form inputs where appropriate
  - Add icon buttons for compact actions
  - Ensure consistent icon sizes (16px, 20px, 24px)
  - _Requirements: 20.1, 20.2, 20.3, 20.4, 20.5_

- [ ] 44. Final testing and polish
  - Test all CRUD operations for each entity
  - Test authentication flow (login, logout, token refresh)
  - Test POS checkout flow end-to-end
  - Test AI content generation
  - Test localStorage persistence
  - Test responsive layouts on all breakpoints
  - Test dark mode on all pages
  - Verify VND formatting throughout
  - Test error handling and loading states
  - Fix any bugs or UI issues

- [ ]* 45. Write property tests for currency formatting
  - **Property 16: VND currency formatting**
  - **Validates: Requirements 14.1**

- [ ]* 46. Write property tests for cart calculations
  - **Property 6: POS cart calculation**
  - **Validates: Requirements 4.3**

- [ ]* 47. Write property tests for search filtering
  - **Property 8: Customer search filtering**
  - **Validates: Requirements 6.5**

- [ ]* 48. Write property tests for localStorage persistence
  - **Property 12: Marketing post persistence**
  - **Validates: Requirements 10.5**

- [ ]* 49. Write unit tests for services
  - Test productService methods call Orval functions
  - Test customerService methods
  - Test orderService methods
  - Test employeeService methods
  - Test inventoryService methods
  - Test marketingService localStorage operations
  - Test notificationService localStorage operations
  - Mock Orval-generated API functions
  - _Requirements: 18.1, 18.2, 18.3_

- [ ]* 50. Write unit tests for hooks
  - Test useProducts hook fetches data
  - Test useCreateProduct invalidates cache
  - Test useCustomers hook with search params
  - Test useOrders hook with status filter
  - Test useMarketing hook with localStorage
  - Mock service layer
  - _Requirements: 13.1, 13.2, 13.4_

- [ ]* 51. Write component tests
  - Test Button component variants
  - Test Table component rendering
  - Test Modal component open/close
  - Test Form validation
  - Test ProtectedRoute redirects
  - Use React Testing Library
  - _Requirements: 1.1, 16.1, 17.1_

- [ ]* 52. Write integration tests
  - Test login flow with token storage
  - Test product CRUD flow
  - Test order creation flow
  - Test POS checkout flow
  - Use MSW for API mocking
  - _Requirements: 1.2, 4.4, 5.3_

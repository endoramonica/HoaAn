# Requirements Document

## Introduction

This document specifies the requirements for building a complete Admin Dashboard & POS (Point of Sale) System for a Vietnamese Worship Supplies Store ("Cửa Hàng Đồ Cúng"). The system will provide comprehensive management capabilities for products, customers, orders, employees, inventory, and sales operations, along with AI-powered marketing content generation. The application builds upon an existing Vite + React + TypeScript foundation with Orval-generated API clients and JWT authentication already implemented.

## Glossary

- **POS (Point of Sale)**: The system interface where sales transactions are processed
- **Admin Dashboard**: The main administrative interface showing business analytics and metrics
- **Orval API Client**: Pre-generated TypeScript API functions from OpenAPI specification
- **React Query**: Library for server state management with caching and synchronization
- **Protected Route**: Route that requires authentication to access
- **Layout Component**: Reusable wrapper component containing Sidebar and TopBar
- **Service Layer**: Business logic wrappers around generated API functions
- **Hook Layer**: React Query hooks for data fetching and mutations
- **LocalStorage CRUD**: Create, Read, Update, Delete operations using browser localStorage
- **AI Content Generator**: Module using Gemini or OpenAI to generate marketing content
- **VND**: Vietnamese Dong currency
- **Dark Mode**: UI theme with dark color scheme
- **Responsive Design**: UI that adapts to different screen sizes

## Requirements

### Requirement 1

**User Story:** As a store administrator, I want a protected routing system with authentication, so that only authorized users can access the admin dashboard and POS system.

#### Acceptance Criteria

1. WHEN an unauthenticated user attempts to access a protected route THEN the System SHALL redirect them to the login page
2. WHEN a user successfully logs in THEN the System SHALL store authentication tokens and redirect to the dashboard
3. WHEN a user logs out THEN the System SHALL clear authentication tokens and redirect to the login page
4. WHEN an authenticated user navigates between routes THEN the System SHALL maintain their session without re-authentication
5. WHERE a user's token expires THEN the System SHALL automatically refresh the token or redirect to login

### Requirement 2

**User Story:** As a store administrator, I want a responsive layout with sidebar navigation and top bar, so that I can easily navigate between different sections of the system on any device.

#### Acceptance Criteria

1. WHEN the application loads THEN the System SHALL display a sidebar with navigation links to all main sections
2. WHEN the screen width is below tablet size THEN the System SHALL collapse the sidebar into a mobile menu
3. WHEN a user clicks a navigation link THEN the System SHALL highlight the active route in the sidebar
4. WHEN the application loads THEN the System SHALL display a top bar with user information and logout button
5. WHERE dark mode is enabled THEN the System SHALL apply dark theme colors to the layout

### Requirement 3

**User Story:** As a store administrator, I want a dashboard with business analytics and charts, so that I can monitor store performance at a glance.

#### Acceptance Criteria

1. WHEN the dashboard loads THEN the System SHALL display key metrics including total revenue, orders, customers, and products
2. WHEN the dashboard loads THEN the System SHALL display revenue charts using data from the backend API
3. WHEN the dashboard loads THEN the System SHALL display recent orders list with status indicators
4. WHEN the dashboard loads THEN the System SHALL display top-selling products with sales data
5. WHERE data is loading THEN the System SHALL display loading skeletons for all dashboard components

### Requirement 4

**User Story:** As a cashier, I want a POS interface to process sales transactions, so that I can quickly sell products to customers.

#### Acceptance Criteria

1. WHEN the POS page loads THEN the System SHALL display a product search and selection interface
2. WHEN a product is selected THEN the System SHALL add it to the cart with quantity controls
3. WHEN cart items are modified THEN the System SHALL automatically calculate the total amount in VND
4. WHEN the checkout button is clicked THEN the System SHALL create an order via the backend API
5. WHEN an order is successfully created THEN the System SHALL clear the cart and display a success message

### Requirement 5

**User Story:** As a store administrator, I want to manage products with full CRUD operations, so that I can maintain an accurate product catalog.

#### Acceptance Criteria

1. WHEN the products page loads THEN the System SHALL fetch and display all products using the Orval-generated API client
2. WHEN the create button is clicked THEN the System SHALL display a form to add a new product
3. WHEN a product form is submitted THEN the System SHALL call the create or update API endpoint
4. WHEN a delete button is clicked THEN the System SHALL display a confirmation modal before deleting
5. WHERE a product has an image THEN the System SHALL display the image thumbnail in the product list

### Requirement 6

**User Story:** As a store administrator, I want to manage customers with full CRUD operations, so that I can maintain customer relationships and track purchase history.

#### Acceptance Criteria

1. WHEN the customers page loads THEN the System SHALL fetch and display all customers using the Orval-generated API client
2. WHEN the create button is clicked THEN the System SHALL display a form to add a new customer
3. WHEN a customer form is submitted THEN the System SHALL call the create or update API endpoint
4. WHEN a customer row is clicked THEN the System SHALL display customer details and order history
5. WHERE search is used THEN the System SHALL filter customers by name, phone, or email

### Requirement 7

**User Story:** As a store administrator, I want to manage orders and track their status, so that I can fulfill customer orders efficiently.

#### Acceptance Criteria

1. WHEN the orders page loads THEN the System SHALL fetch and display all orders using the Orval-generated API client
2. WHEN an order row is clicked THEN the System SHALL display order details including items and customer information
3. WHEN an order status is changed THEN the System SHALL update the order via the backend API
4. WHEN orders are filtered by status THEN the System SHALL display only orders matching the selected status
5. WHERE an order is paid THEN the System SHALL display payment information and method

### Requirement 8

**User Story:** As a store administrator, I want to manage employees with role-based access, so that I can control who can access different parts of the system.

#### Acceptance Criteria

1. WHEN the employees page loads THEN the System SHALL fetch and display all employees using the Orval-generated API client
2. WHEN the create button is clicked THEN the System SHALL display a form to add a new employee with role selection
3. WHEN an employee form is submitted THEN the System SHALL call the create or update API endpoint
4. WHEN an employee is deactivated THEN the System SHALL update their status via the backend API
5. WHERE an employee has a role THEN the System SHALL display their role badge in the employee list

### Requirement 9

**User Story:** As a store administrator, I want to manage inventory levels and track stock movements, so that I can prevent stockouts and overstocking.

#### Acceptance Criteria

1. WHEN the inventory page loads THEN the System SHALL fetch and display all inventory items using the Orval-generated API client
2. WHEN stock levels are low THEN the System SHALL highlight products with warning indicators
3. WHEN a stock adjustment is made THEN the System SHALL update inventory via the backend API
4. WHEN inventory is filtered THEN the System SHALL display only items matching the filter criteria
5. WHERE a product is out of stock THEN the System SHALL display a clear out-of-stock indicator

### Requirement 10

**User Story:** As a marketing manager, I want an AI-powered content generator for marketing posts, so that I can create engaging content for social media and promotions.

#### Acceptance Criteria

1. WHEN the marketing page loads THEN the System SHALL display a list of saved marketing posts from localStorage
2. WHEN the create button is clicked THEN the System SHALL display a form with AI generation options
3. WHEN AI text generation is requested THEN the System SHALL call Gemini or OpenAI API to generate marketing copy
4. WHEN AI image generation is requested THEN the System SHALL call Gemini API to generate a base64 image
5. WHERE a marketing post is saved THEN the System SHALL store it in localStorage with timestamp and content

### Requirement 11

**User Story:** As a store administrator, I want to receive and manage notifications, so that I can stay informed about important events and actions.

#### Acceptance Criteria

1. WHEN the notifications page loads THEN the System SHALL display all notifications from localStorage
2. WHEN a new notification is created THEN the System SHALL store it in localStorage with timestamp
3. WHEN a notification is clicked THEN the System SHALL mark it as read
4. WHEN notifications are filtered THEN the System SHALL display only unread or read notifications
5. WHERE a notification is deleted THEN the System SHALL remove it from localStorage

### Requirement 12

**User Story:** As a store administrator, I want to configure system settings, so that I can customize the application behavior and preferences.

#### Acceptance Criteria

1. WHEN the settings page loads THEN the System SHALL display current settings including theme and language
2. WHEN dark mode is toggled THEN the System SHALL apply the dark theme to all components
3. WHEN settings are saved THEN the System SHALL persist them to localStorage
4. WHEN the language is changed THEN the System SHALL update all UI text to the selected language
5. WHERE user profile is edited THEN the System SHALL update user information via the backend API

### Requirement 13

**User Story:** As a developer, I want React Query hooks for all API operations, so that I can manage server state with caching and automatic refetching.

#### Acceptance Criteria

1. WHEN a query hook is used THEN the System SHALL fetch data using the Orval-generated API client
2. WHEN a mutation hook is used THEN the System SHALL execute the mutation and invalidate related queries on success
3. WHEN data is fetched THEN the System SHALL cache the result for subsequent requests
4. WHEN a mutation succeeds THEN the System SHALL automatically refetch affected queries
5. WHERE an API call fails THEN the System SHALL display an error message using the error handling system

### Requirement 14

**User Story:** As a user, I want all monetary values displayed in Vietnamese Dong format, so that I can easily understand prices and amounts.

#### Acceptance Criteria

1. WHEN a price is displayed THEN the System SHALL format it using Vietnamese Dong currency format
2. WHEN a total is calculated THEN the System SHALL display it with proper VND formatting
3. WHEN currency values are in tables THEN the System SHALL align them consistently
4. WHEN currency values are in charts THEN the System SHALL use VND formatting on axes
5. WHERE a currency value is zero THEN the System SHALL display "0 ₫" or equivalent

### Requirement 15

**User Story:** As a user, I want a fully responsive interface that works on desktop, tablet, and mobile devices, so that I can access the system from any device.

#### Acceptance Criteria

1. WHEN the application is viewed on desktop THEN the System SHALL display the full sidebar and all columns
2. WHEN the application is viewed on tablet THEN the System SHALL adjust layout for medium screens
3. WHEN the application is viewed on mobile THEN the System SHALL collapse sidebar and stack content vertically
4. WHEN tables are viewed on mobile THEN the System SHALL make them horizontally scrollable or use card layout
5. WHERE forms are displayed THEN the System SHALL stack form fields vertically on small screens

### Requirement 16

**User Story:** As a user, I want loading states and error boundaries throughout the application, so that I have clear feedback during operations and graceful error handling.

#### Acceptance Criteria

1. WHEN data is being fetched THEN the System SHALL display loading skeletons or spinners
2. WHEN an API call fails THEN the System SHALL display an error message with retry option
3. WHEN a form is being submitted THEN the System SHALL disable the submit button and show loading state
4. WHEN a component throws an error THEN the System SHALL catch it with an error boundary
5. WHERE an operation succeeds THEN the System SHALL display a success notification using sonner

### Requirement 17

**User Story:** As a user, I want confirmation modals for destructive actions, so that I can prevent accidental data loss.

#### Acceptance Criteria

1. WHEN a delete button is clicked THEN the System SHALL display a confirmation modal
2. WHEN a confirmation modal is displayed THEN the System SHALL clearly state the action and consequences
3. WHEN the confirm button is clicked THEN the System SHALL execute the destructive action
4. WHEN the cancel button is clicked THEN the System SHALL close the modal without taking action
5. WHERE a modal is open THEN the System SHALL prevent interaction with background content

### Requirement 18

**User Story:** As a developer, I want service classes wrapping Orval-generated API functions, so that I can add business logic and consistent error handling.

#### Acceptance Criteria

1. WHEN a service method is called THEN the System SHALL invoke the corresponding Orval-generated API function
2. WHEN a service method encounters an error THEN the System SHALL log the error and re-throw it
3. WHEN a service method returns data THEN the System SHALL return it in a consistent format
4. WHERE data transformation is needed THEN the System SHALL transform the API response before returning
5. WHERE business logic is needed THEN the System SHALL execute it before or after the API call

### Requirement 19

**User Story:** As a user, I want dark mode support throughout the application, so that I can use the system comfortably in low-light environments.

#### Acceptance Criteria

1. WHEN dark mode is enabled THEN the System SHALL apply dark theme colors to all components
2. WHEN dark mode is toggled THEN the System SHALL persist the preference to localStorage
3. WHEN the application loads THEN the System SHALL apply the saved theme preference
4. WHEN charts are displayed in dark mode THEN the System SHALL use appropriate colors for visibility
5. WHERE images are displayed THEN the System SHALL ensure they are visible in both light and dark modes

### Requirement 20

**User Story:** As a user, I want icons from lucide-react throughout the interface, so that I have consistent and recognizable visual indicators.

#### Acceptance Criteria

1. WHEN navigation items are displayed THEN the System SHALL show appropriate icons for each section
2. WHEN buttons are displayed THEN the System SHALL include icons to indicate their action
3. WHEN status indicators are shown THEN the System SHALL use icons to represent different states
4. WHEN forms are displayed THEN the System SHALL use icons for input fields where appropriate
5. WHERE actions are available THEN the System SHALL use icon buttons for compact interfaces

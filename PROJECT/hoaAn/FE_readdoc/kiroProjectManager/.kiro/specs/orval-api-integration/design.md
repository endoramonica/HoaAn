# Design Document

## Overview

This design document outlines the architecture for integrating API communication into a React + TypeScript application using Orval for automatic code generation. The system consists of several layers:

1. **Code Generation Layer**: Orval generates TypeScript API clients from OpenAPI specifications
2. **HTTP Client Layer**: Axios instances with interceptors for authentication and error handling
3. **Service Layer**: Business logic wrappers around generated API functions
4. **Hook Layer**: React Query hooks for state management and caching
5. **Storage Layer**: Token persistence using browser storage APIs

The design emphasizes type safety, developer experience, maintainability, and separation of concerns.

## Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     React Components                         │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                  React Query Hooks                           │
│              (useProducts, useAuth, etc.)                    │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                   Service Layer                              │
│         (ProductService, AuthService, etc.)                  │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│              Generated API Client (Orval)                    │
│           (getApiV1Products, postApiV1Auth, etc.)           │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                 Custom Mutator (Axios)                       │
│              (Token attachment, logging)                     │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│              Main Axios Instance (apiClient)                 │
│         (Interceptors: auth, refresh, errors)                │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                    Backend API                               │
└─────────────────────────────────────────────────────────────┘

                     ┌──────────────────┐
                     │  Token Storage   │
                     │  (localStorage/  │
                     │  sessionStorage) │
                     └──────────────────┘
```

### Directory Structure

```
project-root/
├── swagger.json                    # OpenAPI specification
├── orval.config.js                 # Orval configuration
├── .env                            # Environment variables
│
├── api/
│   ├── scripts/
│   │   └── generate.ts             # Code generation script
│   └── generated-orval/
│       ├── index.ts                # Generated API functions
│       └── schemas/                # Generated TypeScript types
│
└── src/
    └── lib/
        ├── api/
        │   ├── client.ts           # Main Axios instance + interceptors
        │   ├── orval-client.ts     # Custom mutator for Orval
        │   ├── types.ts            # Shared API types
        │   └── errors.ts           # Error handling utilities
        ├── services/
        │   ├── authService.ts      # Authentication service
        │   └── productService.ts   # Product service (example)
        └── hooks/
            ├── useAuth.ts          # Auth hooks
            └── useProducts.ts      # Product hooks (example)
```

## Components and Interfaces

### 1. Code Generation System

**Orval Configuration** (`orval.config.js`):
- Reads OpenAPI specification from `swagger.json`
- Generates TypeScript functions for each API endpoint
- Generates TypeScript types for all schemas
- Configures custom mutator for HTTP requests
- Enables response type generation

**Generation Script** (`api/scripts/generate.ts`):
- Executes Orval code generation
- Can be run manually or in watch mode
- Validates that swagger.json exists
- Outputs to `api/generated-orval/`

### 2. Token Storage System

**Interface**:
```typescript
interface TokenStorage {
  // Storage selection
  isRememberMe(): boolean;
  setRememberMe(remember: boolean): void;
  getStorage(): Storage;
  
  // Token operations
  getAccessToken(): string | null;
  setAccessToken(token: string): void;
  getRefreshToken(): string | null;
  setRefreshToken(token: string): void;
  
  // Batch operations
  setTokens(accessToken: string, refreshToken: string, rememberMe?: boolean): void;
  clearTokens(): void;
}
```

**Implementation Details**:
- Uses localStorage when "remember me" is enabled
- Uses sessionStorage when "remember me" is disabled
- Checks both storages when retrieving tokens (for migration)
- Clears tokens from both storages for security

### 3. HTTP Client Layer

**Main Axios Instance** (`apiClient`):
- Base URL from environment variables
- 30-second timeout
- JSON content type headers
- Request interceptor for token attachment
- Response interceptor for error handling and token refresh

**Custom Mutator** (`orval-client.ts`):
- Separate Axios instance for Orval-generated functions
- Automatically attaches JWT tokens
- Used by all generated API functions

### 4. Token Refresh Mechanism

**Flow**:
1. Request fails with 401 Unauthorized
2. Check if refresh is already in progress
3. If yes, queue the request
4. If no, start refresh process
5. Call refresh token endpoint
6. Update stored tokens
7. Retry original request with new token
8. Process queued requests
9. If refresh fails, clear tokens and redirect to login

**State Management**:
```typescript
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (value?: any) => void;
  reject: (reason?: any) => void;
}> = [];
```

### 5. Error Handling System

**ApiError Interface**:
```typescript
interface ApiError {
  status: number;
  message: string;
  errors?: Record<string, string[]>;  // Validation errors
  timestamp: string;
}
```

**Error Transformation**:
- Network errors → status 0, connection message
- 400 → Validation error with field details
- 401 → Unauthorized, session expired
- 403 → Forbidden, insufficient permissions
- 404 → Not found
- 500+ → Server error

### 6. Service Layer

**Purpose**:
- Wrap generated API functions
- Add business logic
- Transform data
- Handle errors consistently
- Provide clean interface to hooks/components

**Example Structure**:
```typescript
class ProductService {
  private api = getGeneratedAPI();
  
  async getProducts(): Promise<Product[]> {
    try {
      const response = await this.api.getApiV1Products();
      return response.data;
    } catch (error) {
      console.error('Failed to fetch products:', error);
      throw error;
    }
  }
}
```

### 7. React Query Integration

**Query Hooks**:
- Fetch data with caching
- Automatic refetching
- Loading and error states
- Query key management

**Mutation Hooks**:
- Execute create/update/delete operations
- Invalidate related queries on success
- Optimistic updates (optional)
- Error handling

**Example**:
```typescript
export function useProducts() {
  return useQuery({
    queryKey: ['products'],
    queryFn: () => productService.getProducts(),
  });
}

export function useCreateProduct() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (dto: CreateProductDto) => productService.createProduct(dto),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
    },
  });
}
```

## Data Models

### Generated Types

All data models are generated by Orval from OpenAPI schemas. Examples include:

**User DTO**:
```typescript
interface UserDto {
  id: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  avatar?: string;
  role: string;
  isEmailConfirmed: boolean;
  createdAt: string;
  updatedAt: string;
}
```

**Authentication Types**:
```typescript
interface LoginRequest {
  email: string;
  password: string;
  rememberMe?: boolean;
}

interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: UserDto;
}

interface RefreshTokenRequest {
  refreshToken: string;
}

interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}
```

**Pagination**:
```typescript
interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
```

### Custom Types

**API Response Wrapper**:
```typescript
interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}
```

**Error Types**:
```typescript
interface ApiError {
  status: number;
  message: string;
  errors?: Record<string, string[]>;
  timestamp: string;
}
```

## Corr
ectness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Endpoint generation completeness
*For any* endpoint definition in the OpenAPI specification, there should exist a corresponding TypeScript function in the generated code with matching path and HTTP method.
**Validates: Requirements 1.2**

### Property 2: Schema generation completeness
*For any* schema definition in the OpenAPI specification, there should exist a corresponding TypeScript interface or type in the generated schemas directory.
**Validates: Requirements 1.3**

### Property 3: Token attachment for authenticated requests
*For any* API request made when an access token is stored, the request should include an Authorization header with the Bearer token format.
**Validates: Requirements 2.1**

### Property 4: Token storage round-trip
*For any* valid token string, storing it and then retrieving it should return the same token value.
**Validates: Requirements 2.3**

### Property 5: Token retrieval checks both storages
*For any* token retrieval operation, the system should check both localStorage and sessionStorage to find the token.
**Validates: Requirements 5.3**

### Property 6: Error transformation consistency
*For any* failed API request, the error should be transformed into an ApiError object with status, message, and timestamp fields.
**Validates: Requirements 4.1**

### Property 7: Error message extraction
*For any* error response containing a message field, that message should appear in the resulting ApiError.
**Validates: Requirements 4.4**

### Property 8: Default error messages
*For any* error response without a message field, the ApiError should contain a default message based on the HTTP status code.
**Validates: Requirements 4.5**

### Property 9: Service method delegation
*For any* service method call, it should invoke the corresponding generated API function.
**Validates: Requirements 6.1**

### Property 10: Service error logging
*For any* error encountered in a service method, the error should be logged before being re-thrown.
**Validates: Requirements 6.2**

### Property 11: Service return format consistency
*For any* successful service method call, the returned data should be in a consistent format (unwrapped from response envelope if needed).
**Validates: Requirements 6.3**

### Property 12: Query hook service delegation
*For any* React Query hook, it should fetch data using the corresponding service method.
**Validates: Requirements 7.1**

### Property 13: Mutation invalidation
*For any* successful mutation, the related query keys should be invalidated to trigger refetch.
**Validates: Requirements 7.2**

### Property 14: Query caching
*For any* successful query, subsequent requests with the same query key should use cached data until invalidated.
**Validates: Requirements 7.3**

### Property 15: Mutation triggers refetch
*For any* successful mutation that invalidates a query, the affected queries should automatically refetch.
**Validates: Requirements 7.4**

### Property 16: Query key usage
*For any* query with a specified query key, that key should be used for both caching and invalidation operations.
**Validates: Requirements 7.5**

### Property 17: Development logging for requests
*For any* API request made in development mode, the request method and URL should be logged to the console.
**Validates: Requirements 10.1**

### Property 18: Development logging for responses
*For any* API response received in development mode, the response data should be logged to the console.
**Validates: Requirements 10.2**

### Property 19: Development logging for errors
*For any* API error in development mode, detailed error information should be logged to the console.
**Validates: Requirements 10.3**

## Error Handling

### Error Types

1. **Network Errors** (status 0):
   - No response from server
   - Connection timeout
   - CORS issues
   - Message: "Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng."

2. **Client Errors** (4xx):
   - 400 Bad Request: Validation errors with field details
   - 401 Unauthorized: Token expired or invalid
   - 403 Forbidden: Insufficient permissions
   - 404 Not Found: Resource doesn't exist

3. **Server Errors** (5xx):
   - 500 Internal Server Error
   - 502 Bad Gateway
   - 503 Service Unavailable
   - Message: "Lỗi server. Vui lòng thử lại sau."

### Error Handling Flow

```
API Request
    │
    ▼
Request Interceptor (attach token)
    │
    ▼
HTTP Request
    │
    ├─── Success ──────────────────────────────────────┐
    │                                                   │
    └─── Error ────────────────────────────────────┐   │
                                                   │   │
                                                   ▼   ▼
                                        Response Interceptor
                                                   │
                                    ┌──────────────┼──────────────┐
                                    │              │              │
                                Network Error   401 Error    Other Error
                                    │              │              │
                                    ▼              ▼              ▼
                            Create ApiError   Refresh Token   Create ApiError
                                    │              │              │
                                    │         ┌────┴────┐         │
                                    │         │         │         │
                                    │      Success   Failure      │
                                    │         │         │         │
                                    │    Retry Req  Clear Tokens  │
                                    │         │         │         │
                                    │         │    Redirect       │
                                    │         │      Login        │
                                    └─────────┴─────────┴─────────┘
                                                   │
                                                   ▼
                                            Reject Promise
                                                   │
                                                   ▼
                                            Service Layer
                                                   │
                                                   ▼
                                            Log & Re-throw
                                                   │
                                                   ▼
                                            React Query
                                                   │
                                                   ▼
                                            Component
```

### Error Recovery Strategies

1. **Automatic Token Refresh**:
   - Intercept 401 errors
   - Attempt token refresh
   - Retry original request
   - Queue concurrent requests

2. **Request Retry**:
   - Network errors: Can be retried
   - 5xx errors: Can be retried with exponential backoff
   - 4xx errors: Should not be retried (except 401)

3. **User Notification**:
   - Display user-friendly error messages
   - Provide actionable feedback
   - Log technical details for debugging

4. **Graceful Degradation**:
   - Show cached data when available
   - Provide offline mode indicators
   - Allow retry actions

## Testing Strategy

### Unit Testing

**Token Storage Tests**:
- Test token storage and retrieval
- Test remember me functionality
- Test token clearing
- Test storage migration

**Error Handling Tests**:
- Test error transformation
- Test error message extraction
- Test default error messages
- Test network error handling

**Service Layer Tests**:
- Test service method calls
- Test error logging
- Test data transformation
- Mock generated API functions

**Axios Interceptor Tests**:
- Test token attachment
- Test token refresh flow
- Test request queuing
- Test error interception

### Property-Based Testing

We will use **fast-check** as the property-based testing library for JavaScript/TypeScript. Each property-based test should run a minimum of 100 iterations.

**Property Tests to Implement**:

1. **Token Round-Trip Property**:
   - Generate random token strings
   - Store and retrieve each token
   - Verify retrieved token matches original

2. **Error Transformation Property**:
   - Generate random error responses
   - Transform each to ApiError
   - Verify all have required fields (status, message, timestamp)

3. **Service Delegation Property**:
   - Generate random service method calls
   - Verify each calls corresponding API function
   - Verify parameters are passed correctly

4. **Query Key Consistency Property**:
   - Generate random query keys
   - Verify caching uses the same key
   - Verify invalidation targets the same key

Each property-based test MUST be tagged with a comment in this format:
```typescript
// **Feature: orval-api-integration, Property X: [property description]**
```

### Integration Testing

**API Client Generation**:
- Test with sample OpenAPI spec
- Verify generated files exist
- Verify generated code compiles
- Verify types are correct

**End-to-End Flow**:
- Test login flow with token storage
- Test authenticated API calls
- Test token refresh on 401
- Test logout and token clearing

**React Query Integration**:
- Test query hooks fetch data
- Test mutation hooks update data
- Test cache invalidation
- Test optimistic updates

### Testing Tools

- **Vitest**: Unit test runner
- **fast-check**: Property-based testing
- **MSW (Mock Service Worker)**: API mocking
- **React Testing Library**: Component testing
- **@tanstack/react-query**: Query testing utilities

## Implementation Phases

### Phase 1: Foundation
1. Install dependencies
2. Create directory structure
3. Configure environment variables
4. Set up Orval configuration

### Phase 2: Core Infrastructure
1. Implement token storage utilities
2. Create main Axios instance with interceptors
3. Implement error handling system
4. Create custom mutator for Orval

### Phase 3: Code Generation
1. Create generation script
2. Obtain OpenAPI specification
3. Generate API client
4. Verify generated types

### Phase 4: Service Layer
1. Create base service class (optional)
2. Implement authentication service
3. Implement example domain services
4. Add error handling and logging

### Phase 5: React Query Integration
1. Set up React Query provider
2. Create query hooks
3. Create mutation hooks
4. Implement cache invalidation

### Phase 6: Testing
1. Write unit tests for utilities
2. Write property-based tests
3. Write integration tests
4. Test with real API

### Phase 7: Documentation & Polish
1. Document API usage
2. Create example components
3. Add development logging
4. Optimize performance

## Performance Considerations

### Code Generation
- Run generation as a build step, not at runtime
- Cache generated files
- Use watch mode during development

### HTTP Requests
- Set appropriate timeouts (30s default)
- Implement request cancellation
- Use AbortController for cleanup

### Token Refresh
- Prevent duplicate refresh requests
- Queue concurrent requests during refresh
- Implement exponential backoff for failures

### React Query Caching
- Set appropriate stale times
- Use cache time to balance freshness and performance
- Implement background refetching for critical data
- Use optimistic updates for better UX

### Bundle Size
- Tree-shake unused generated functions
- Lazy load services when possible
- Use dynamic imports for large dependencies

## Security Considerations

### Token Storage
- Never store tokens in cookies without httpOnly flag
- Clear tokens on logout
- Implement token expiration checking
- Use secure storage when available

### API Communication
- Always use HTTPS in production
- Validate SSL certificates
- Implement CSRF protection if needed
- Sanitize user input before sending

### Error Messages
- Don't expose sensitive information in errors
- Log detailed errors server-side only
- Show generic messages to users
- Sanitize error logs

### Development vs Production
- Disable verbose logging in production
- Remove debug endpoints in production
- Use environment-specific configurations
- Implement rate limiting

## Deployment Considerations

### Environment Configuration
- Use different API URLs per environment
- Configure timeouts per environment
- Set up error tracking (Sentry, etc.)
- Enable/disable features per environment

### CI/CD Integration
- Auto-generate API client on backend changes
- Run tests before deployment
- Validate OpenAPI spec
- Check for breaking changes

### Monitoring
- Track API response times
- Monitor error rates
- Log failed requests
- Alert on high error rates

### Versioning
- Version the OpenAPI spec
- Support multiple API versions if needed
- Handle breaking changes gracefully
- Provide migration guides

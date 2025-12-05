# Implementation Plan

- [x] 1. Install dependencies and configure project structure





  - Install axios, @tanstack/react-query, orval, and development dependencies
  - Create directory structure: api/scripts, api/generated-orval, src/lib/api, src/lib/services, src/lib/hooks
  - Set up .env file with VITE_API_URL and other environment variables
  - Add npm scripts for api:generate and api:watch
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 9.1, 9.2_

- [x] 2. Implement error handling system





  - Create src/lib/api/errors.ts with ApiError interface and error transformation functions
  - Implement createApiError function to transform HTTP errors into ApiError format
  - Implement getErrorMessage function with status code mapping
  - Define ErrorMessages constants for Vietnamese error messages
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_

- [ ]* 2.1 Write property test for error transformation
  - **Property 6: Error transformation consistency**
  - **Validates: Requirements 4.1**

- [ ]* 2.2 Write property test for error message extraction
  - **Property 7: Error message extraction**
  - **Validates: Requirements 4.4**

- [ ]* 2.3 Write property test for default error messages
  - **Property 8: Default error messages**
  - **Validates: Requirements 4.5**

- [x] 3. Implement token storage utilities





  - Create src/lib/api/types.ts with shared type definitions
  - Implement tokenStorage object in src/lib/api/client.ts with all storage methods
  - Implement isRememberMe, setRememberMe, and getStorage methods
  - Implement getAccessToken, setAccessToken, getRefreshToken, setRefreshToken methods
  - Implement setTokens and clearTokens batch operations
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5_

- [ ]* 3.1 Write property test for token storage round-trip
  - **Property 4: Token storage round-trip**
  - **Validates: Requirements 2.3**

- [ ]* 3.2 Write property test for token retrieval
  - **Property 5: Token retrieval checks both storages**
  - **Validates: Requirements 5.3**

- [ ]* 3.3 Write unit tests for token storage
  - Test remember me functionality with localStorage
  - Test session storage when remember me is disabled
  - Test token clearing from both storages
  - Test storage migration when preference changes
  - _Requirements: 5.1, 5.2, 5.4, 5.5_

- [x] 4. Create main Axios instance with interceptors



  - Create createAxiosInstance function with base configuration
  - Implement request interceptor to attach JWT tokens from storage
  - Implement development logging in request interceptor
  - Export apiClient instance and apiRequest wrapper functions
  - _Requirements: 2.1, 2.2, 2.3, 9.1, 9.2, 10.1_

- [ ]* 4.1 Write property test for token attachment
  - **Property 3: Token attachment for authenticated requests**
  - **Validates: Requirements 2.1**

- [ ]* 4.2 Write property test for development logging
  - **Property 17: Development logging for requests**
  - **Validates: Requirements 10.1**

- [x] 5. Implement token refresh mechanism





  - Implement response interceptor with 401 error detection
  - Implement isRefreshing flag and failedQueue for request queuing
  - Implement processQueue function to retry queued requests
  - Implement token refresh API call with error handling
  - Implement redirect to login on refresh failure
  - Add development logging for responses and errors
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 10.2, 10.3_

- [ ]* 5.1 Write unit tests for token refresh flow
  - Test 401 triggers refresh attempt
  - Test successful refresh retries original request
  - Test multiple concurrent 401s queue properly
  - Test refresh failure clears tokens and redirects
  - Test duplicate refresh prevention
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [ ]* 5.2 Write property test for response logging
  - **Property 18: Development logging for responses**
  - **Validates: Requirements 10.2**

- [ ]* 5.3 Write property test for error logging
  - **Property 19: Development logging for errors**
  - **Validates: Requirements 10.3**

- [x] 6. Configure Orval and create generation script




  - Create orval.config.js with input/output configuration
  - Configure custom mutator path and override settings
  - Create api/scripts/generate.ts to execute Orval generation
  - Add error handling and validation to generation script
  - Test generation script with a sample swagger.json
  - _Requirements: 1.1, 1.2, 1.3, 1.5_

- [ ]* 6.1 Write integration test for code generation
  - Test generation script reads OpenAPI spec
  - Test generated files exist in correct locations
  - Test generated code compiles without errors
  - Test generated types provide autocomplete
  - _Requirements: 1.1, 1.4_

- [x] 7. Create custom mutator for Orval





  - Create src/lib/api/orval-client.ts with separate Axios instance
  - Configure base URL, timeout, and headers
  - Implement request interceptor to attach tokens
  - Export apiClient function for Orval to use
  - _Requirements: 2.1, 2.2, 9.1_
- [x] 8. Obtain OpenAPI specification and generate API client








- [ ] 8. Obtain OpenAPI specification and generate API client

  - Obtain swagger.json from backend (download or copy)
  - Place swagger.json in project root
  - Run npm run api:generate to generate API client
  - Verify generated files in api/generated-orval/
  - Verify TypeScript types in api/generated-orval/schemas/
  - _Requirements: 1.1, 1.2, 1.3, 8.2_

- [ ]* 8.1 Write property test for endpoint generation
  - **Property 1: Endpoint generation completeness**
  - **Validates: Requirements 1.2**

- [ ]* 8.2 Write property test for schema generation
  - **Property 2: Schema generation completeness**
  - **Validates: Requirements 1.3**


- [x] 9. Implement authentication service









  - Create src/lib/services/authService.ts class
  - Implement login method that calls generated API and stores tokens
  - Implement logout method that clears tokens
  - Implement refreshToken method
  - Implement getCurrentUser method
  - Add error logging to all service methods
  - _Requirements: 6.1, 6.2, 6.3_

- [ ]* 9.1 Write property test for service delegation
  - **Property 9: Service method delegation**
  - **Validates: Requirements 6.1**

- [ ]* 9.2 Write property test for service error logging
  - **Property 10: Service error logging**
  - **Validates: Requirements 6.2**

- [ ]* 9.3 Write property test for service return format
  - **Property 11: Service return format consistency**
  - **Validates: Requirements 6.3**

- [ ]* 9.4 Write unit tests for authentication service
  - Test login stores tokens correctly
  - Test logout clears tokens
  - Test error handling and logging
  - Mock generated API functions
  - _Requirements: 6.1, 6.2_

- [ ] 10. Create example domain service (ProductService)
  - Create src/lib/services/productService.ts class
  - Implement getProducts method using generated API
  - Implement getProduct(id) method
  - Implement createProduct method
  - Implement updateProduct method
  - Implement deleteProduct method
  - Add error logging to all methods
  - _Requirements: 6.1, 6.2, 6.3_

- [ ] 11. Set up React Query provider
  - Install @tanstack/react-query if not already installed
  - Create QueryClient instance with default options
  - Wrap application with QueryClientProvider in main.tsx or App.tsx
  - Configure stale time and cache time
  - Add React Query DevTools for development
  - _Requirements: 7.1, 7.3_

- [ ] 12. Create authentication hooks
  - Create src/lib/hooks/useAuth.ts
  - Implement useLogin mutation hook with token storage
  - Implement useLogout mutation hook with token clearing
  - Implement useCurrentUser query hook
  - Implement query invalidation on login/logout
  - _Requirements: 7.1, 7.2, 7.4, 7.5_

- [ ]* 12.1 Write property test for query hook delegation
  - **Property 12: Query hook service delegation**
  - **Validates: Requirements 7.1**

- [ ]* 12.2 Write property test for mutation invalidation
  - **Property 13: Mutation invalidation**
  - **Validates: Requirements 7.2**

- [ ]* 12.3 Write unit tests for auth hooks
  - Test useLogin calls authService.login
  - Test useLogout clears tokens
  - Test query invalidation after mutations
  - Mock service layer
  - _Requirements: 7.1, 7.2_

- [ ] 13. Create example domain hooks (useProducts)
  - Create src/lib/hooks/useProducts.ts
  - Implement useProducts query hook
  - Implement useProduct(id) query hook with enabled condition
  - Implement useCreateProduct mutation hook
  - Implement useUpdateProduct mutation hook
  - Implement useDeleteProduct mutation hook
  - Configure query invalidation for all mutations
  - _Requirements: 7.1, 7.2, 7.4, 7.5_

- [ ]* 13.1 Write property test for query caching
  - **Property 14: Query caching**
  - **Validates: Requirements 7.3**

- [ ]* 13.2 Write property test for mutation refetch
  - **Property 15: Mutation triggers refetch**
  - **Validates: Requirements 7.4**

- [ ]* 13.3 Write property test for query key usage
  - **Property 16: Query key usage**
  - **Validates: Requirements 7.5**

- [ ]* 13.4 Write integration tests for React Query hooks
  - Test query hooks fetch and cache data
  - Test mutation hooks update and invalidate
  - Test optimistic updates if implemented
  - Use React Testing Library and MSW
  - _Requirements: 7.1, 7.2, 7.3, 7.4_

- [ ] 14. Create example component using hooks
  - Create a sample component (e.g., ProductList.tsx) that uses useProducts
  - Demonstrate loading, error, and success states
  - Demonstrate mutation usage with useCreateProduct
  - Add error handling and user feedback
  - _Requirements: 7.1, 7.2_

- [ ] 15. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

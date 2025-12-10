# ProfilePage Orders Tab - API Integration

## Changes Made

Updated `src/components/ProfilePage.tsx` to use real API for the Orders tab instead of mock data.

### Key Updates:

1. **Imports Added**
   - `useEffect` from React for data fetching
   - `orderService` from `../lib/services/orderService`
   - `OrderDetailDto` type from generated Orval API
   - `Loader2` icon for loading state

2. **State Management**
   - Replaced static mock orders with dynamic state: `orders`, `ordersLoading`, `ordersError`
   - Added `useEffect` hook to fetch orders on component mount
   - Calls `orderService.getMyOrders()` with pagination (page 1, size 10)

3. **Order Status Mapping**
   - Updated status values to match API: `Pending`, `Confirmed`, `Processing`, `Shipping`, `Delivered`, `Cancelled`, `Refunded`
   - Updated `getStatusColor()` and `getStatusText()` functions accordingly

4. **Data Binding**
   - Changed from mock structure to `OrderDetailDto` structure:
     - `order.id` → `order.orderId`
     - `order.date` → `order.createdAt`
     - `order.total` → `order.totalAmount`
     - `order.items[].name` → `order.items[].productName`
     - `order.items[].price` → `order.items[].unitPrice`
     - `order.items[].image` → `order.items[].productImageUrl`

5. **UI Enhancements**
   - Added loading spinner while fetching orders
   - Added error message display if fetch fails
   - Added empty state when no orders exist
   - Graceful fallbacks for missing data

### API Endpoint Used
- **GET** `/api/v1/Order/my-orders`
- Returns paginated list of user's orders with full details including items, shipping, and status history

### Error Handling
- Try-catch block in useEffect
- User-friendly error messages in Vietnamese
- Console logging for debugging

## Testing

To test the integration:
1. Ensure backend API is running
2. User must be authenticated (useAuth hook)
3. Navigate to Profile → Orders tab
4. Orders should load from API automatically
5. Check browser console for any errors

## Notes

- The component now requires authentication to fetch orders
- Orders are fetched once on component mount
- To add pagination/filtering, extend the `loadOrders` function
- Status values are now API-compliant (capitalized)

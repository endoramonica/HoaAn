
  # Cultural E-commerce Landing Page

  This is a code bundle for Cultural E-commerce Landing Page. The original project is available at https://www.figma.com/design/d9BRMD1j7KbGBKjwhUwrc0/Cultural-E-commerce-Landing-Page.

  ## Running the code

  Run `npm i` to install the dependencies.

  Run `npm run dev` to start the development server.
  


  Tôi sẽ tạo hệ thống quản lý API chuyên nghiệp với kiến trúc 3 lớp cho frontend React + TypeScript, tương thích hoàn toàn với backend ASP.NET Core (.NET 8). Hệ thống sẽ bao gồm API client layer, service layer, custom hooks layer, với type safety đầy đủ và error handling chuẩn ASP.NET Core.

Tôi đã hoàn thành việc thiết lập hệ thống quản lý API chuyên nghiệp với kiến trúc 3 lớp tích hợp hoàn toàn với backend ASP.NET Core (.NET 8). Hệ thống bao gồm API client layer với Axios interceptors và JWT authentication, service layer với RESTful API calls và mock data fallback, cùng custom hooks layer để quản lý state, error handling và caching.



Các file đã tạo:

`/lib/api/types.ts` - TypeScript types tương ứng C# DTOs (User, Product, Order, Delivery, Spiritual, Community, etc.)
`/lib/api/errors.ts` - Error handling theo format ASP.NET Core ProblemDetails
`/lib/api/client.ts` - Axios instance với auto JWT attach, refresh token, interceptors
`/lib/services/authService.ts` - Authentication service (login, register, profile, password)
`/lib/services/productService.ts` - Product CRUD service với filter, search, pagination
`/lib/services/orderService.ts` - Order service với tracking, rating, refund
`/lib/hooks/useAuth.ts` - Auth hook với state management và auto toast
`/lib/hooks/useProducts.ts` - Products hook với pagination và refresh
`/lib/hooks/useOrders.ts` - Orders hook với tracking và status updates
`/.env.example` - Environment variables template
`/API_INTEGRATION_GUIDE.md` - Tài liệu đầy đủ 50+ trang với examples


Tính năng chính: JWT auto-refresh khi 401, ProblemDetails error parsing, mock data mode cho development, file upload với progress, type-safe với 60+ TypeScript interfaces, pagination navigation helpers, auto toast notifications, và retry logic.


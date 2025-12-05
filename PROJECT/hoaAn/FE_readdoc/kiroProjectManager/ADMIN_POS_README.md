# Admin Dashboard & POS System - Cửa Hàng Đồ Cúng

Hệ thống quản lý và bán hàng hoàn chỉnh cho cửa hàng đồ cúng, được xây dựng với React + TypeScript + Tailwind CSS.

## ✨ Tính Năng

### 🔐 Authentication
- Đăng nhập với email/password
- Protected routes
- Token management

### 📊 Dashboard
- Tổng quan doanh thu, đơn hàng, khách hàng
- Biểu đồ doanh thu và bán hàng (Recharts)
- Thống kê real-time

### 🛒 POS (Point of Sale)
- Giao diện bán hàng nhanh
- Tìm kiếm sản phẩm
- Giỏ hàng với tính năng tăng/giảm số lượng
- Thanh toán với quick cash buttons
- Tính tiền thừa tự động

### 📦 Quản Lý Sản Phẩm
- CRUD sản phẩm
- Tìm kiếm và lọc
- Quản lý kho
- Trạng thái hoạt động

### 👥 Quản Lý Khách Hàng
- CRUD khách hàng
- Tìm kiếm theo tên, SĐT, email
- Theo dõi tổng chi tiêu
- Lịch sử đơn hàng

### 📋 Quản Lý Đơn Hàng
- Xem danh sách đơn hàng
- Lọc theo trạng thái
- Chi tiết đơn hàng
- Cập nhật trạng thái

### 👨‍💼 Quản Lý Nhân Viên
- CRUD nhân viên
- Phân quyền (Admin, Manager, Cashier, Staff)
- Quản lý trạng thái

### 📦 Quản Lý Kho
- Theo dõi tồn kho
- Cảnh báo sắp hết hàng
- Cảnh báo hết hàng
- Lọc theo trạng thái kho

### 🎨 Marketing AI
- Tạo nội dung bằng AI (Gemini/OpenAI)
- Tạo ảnh bằng Gemini
- Lên lịch đăng bài
- Quản lý bài viết (localStorage)

### 🔔 Thông Báo
- Hệ thống thông báo
- Đánh dấu đã đọc
- Lọc theo trạng thái
- LocalStorage persistence

### ⚙️ Cài Đặt
- Thông tin cửa hàng
- Dark/Light mode
- Ngôn ngữ
- Thuế VAT

## 🚀 Cài Đặt

### 1. Clone và cài đặt dependencies

\`\`\`bash
npm install
\`\`\`

### 2. Cấu hình API Keys

Copy file \`.env.example\` thành \`.env\` và điền API keys:

\`\`\`bash
cp .env.example .env
\`\`\`

Chỉnh sửa \`.env\`:

\`\`\`env
VITE_GEMINI_API_KEY=your_gemini_api_key_here
VITE_OPENAI_API_KEY=your_openai_api_key_here
\`\`\`

### 3. Chạy development server

\`\`\`bash
npm run dev
\`\`\`

### 4. Build cho production

\`\`\`bash
npm run build
\`\`\`

## 📁 Cấu Trúc Thư Mục

\`\`\`
src/
├── components/
│   └── ui/              # UI components (Button, Input, Card, etc.)
├── layout/              # Layout components (Sidebar, TopBar, ProtectedRoute)
├── pages/               # Page components
│   ├── LoginPage.tsx
│   ├── DashboardPage.tsx
│   ├── POSPage.tsx
│   ├── ProductsPage.tsx
│   ├── CustomersPage.tsx
│   ├── OrdersPage.tsx
│   ├── EmployeesPage.tsx
│   ├── InventoryPage.tsx
│   ├── MarketingPage.tsx
│   ├── MarketingEditPage.tsx
│   ├── NotificationsPage.tsx
│   └── SettingsPage.tsx
├── router/              # Router configuration
├── lib/
│   ├── api/            # API client (Orval generated)
│   ├── services/       # Service layer
│   │   ├── authService.ts
│   │   ├── productService.ts
│   │   ├── customerService.ts
│   │   ├── orderService.ts
│   │   ├── employeeService.ts
│   │   ├── inventoryService.ts
│   │   ├── marketingService.ts (localStorage)
│   │   ├── notificationService.ts (localStorage)
│   │   └── aiService.ts
│   ├── hooks/          # React Query hooks
│   │   ├── useAuth.ts
│   │   ├── useProducts.ts
│   │   ├── useCustomers.ts
│   │   ├── useOrders.ts
│   │   ├── useEmployees.ts
│   │   ├── useInventory.ts
│   │   ├── useMarketing.ts
│   │   ├── useNotifications.ts
│   │   └── useTheme.ts
│   └── utils/          # Utilities
│       ├── formatCurrency.ts
│       ├── formatDate.ts
│       └── constants.ts
└── api/generated-orval/ # Orval generated API clients
\`\`\`

## 🎨 UI Components

Tất cả UI components đều hỗ trợ dark mode:

- **Button**: 4 variants (primary, secondary, danger, ghost), loading state, icon support
- **Input**: Label, validation, error messages, icon support
- **Card**: Title, actions, content area
- **Badge**: 5 variants cho status indicators
- **Modal**: Backdrop, close handlers, footer support
- **Select**: Dropdown với label, error states
- **LoadingSkeleton**: 3 variants (text, circular, rectangular)

## 🔌 API Integration

### Orval Generated APIs
Sử dụng Orval-generated API clients cho:
- Products
- Customers
- Categories
- Auth

### LocalStorage APIs
Sử dụng localStorage cho:
- Marketing posts
- Notifications

### AI APIs
- **Gemini**: Text generation (gemini-2.0-flash-exp) và Image generation
- **OpenAI**: Text generation (gpt-4o-mini)

## 🌙 Dark Mode

Dark mode được quản lý bởi \`useTheme\` hook và persist vào localStorage.

Toggle dark mode:
- Từ TopBar
- Từ Settings page

## 💾 LocalStorage Data

Hệ thống sử dụng localStorage cho:
- **Authentication tokens**: \`access_token\`, \`refresh_token\`
- **Theme preference**: \`theme\`
- **Marketing posts**: \`marketing_posts\`
- **Notifications**: \`notifications\`
- **Settings**: \`settings\`

## 🔐 Authentication

Mock authentication với credentials:
- Email: \`admin@example.com\`
- Password: \`password\`

Hoặc cấu hình backend API trong \`src/lib/api/client.ts\`

## 📱 Responsive Design

Hệ thống hoàn toàn responsive:
- **Mobile** (< 640px): Sidebar collapse, card layout
- **Tablet** (640px - 1024px): Adjusted layout
- **Desktop** (> 1024px): Full layout

## 🎯 Tech Stack

- **React 19** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool
- **Tailwind CSS** - Styling
- **React Router** - Routing (HashRouter)
- **React Query** - Server state management
- **Axios** - HTTP client
- **Orval** - API client generator
- **Recharts** - Charts
- **Lucide React** - Icons
- **Sonner** - Toast notifications

## 📝 Scripts

\`\`\`bash
npm run dev          # Start development server
npm run build        # Build for production
npm run preview      # Preview production build
npm run lint         # Run ESLint
npm run api:generate # Generate API clients from Swagger
npm run api:watch    # Watch Swagger and regenerate
\`\`\`

## 🌐 Deployment

Build production:

\`\`\`bash
npm run build
\`\`\`

Output sẽ ở thư mục \`dist/\`. Deploy lên:
- Vercel
- Netlify
- GitHub Pages
- Hoặc bất kỳ static hosting nào

## 🤝 Contributing

1. Fork the project
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📄 License

MIT License

## 👨‍💻 Author

Developed with ❤️ for Vietnamese Worship Supplies Stores
\`\`\`

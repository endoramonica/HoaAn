# Hướng Dẫn Cung Cấp Ngữ Cảnh Cho Cải Thiện Chức Năng Riêng Lẻ

## 🎯 Mục Đích
Khi bạn muốn cải thiện một chức năng cụ thể trong dự án Vietnamese E-commerce này ở một Figma project mới, bạn cần cung cấp đầy đủ thông tin để AI hiểu được ngữ cảnh và chỉ tập trung vào phần cần thay đổi.

## 📋 Template Cung Cấp Ngữ Cảnh

### 1. Thông Tin Dự Án Gốc
```
## Dự Án Gốc: Vietnamese Traditional E-commerce Website

### Mô tả tổng quan:
- Website thương mại điện tử đồ cúng và nghi lễ truyền thống Việt Nam
- Có 2 module chính: VietDelivery (giao hàng) và Tâm Linh Việt (dịch vụ tâm linh)
- Thiết kế văn hóa với màu sắc: #92400E (nâu), #F59E0B (vàng), #DC2626 (đỏ), #FFFBEB (kem)
- VietDelivery: màu #10B981 (xanh) + #F97316 (cam)
- Spiritual: màu #8B5CF6 (tím) + #3B82F6 (xanh)

### Architecture hiện tại:
- React + TypeScript + Tailwind CSS v4
- State management: useState hooks
- Routing: string-based navigation
- UI Components: shadcn/ui
```

### 2. Xác Định Chức Năng Cần Cải Thiện
```
## Chức Năng Cần Cải Thiện: [TÊN CHỨC NĂNG]

### Vị trí trong dự án:
- Module: [VietDelivery/Spiritual/Core E-commerce/Community/etc.]
- Trang: [Tên trang cụ thể]
- Component: [Tên component nếu biết]
- File path: [đường dẫn file nếu có]

### Mô tả chức năng hiện tại:
[Mô tả chi tiết chức năng đang có, cách hoạt động, UI/UX hiện tại]
```

### 3. Yêu Cầu Thay Đổi Cụ Thể
```
## Yêu Cầu Cải Thiện:

### Mục tiêu:
[Mô tả rõ ràng bạn muốn đạt được gì]

### Tính năng mới cần thêm:
- [Tính năng 1]
- [Tính năng 2]
- [...]

### Thay đổi UI/UX:
- [Thay đổi giao diện]
- [Cải thiện trải nghiệm người dùng]
- [...]

### Yêu cầu kỹ thuật:
- [Integration với API]
- [Performance improvements]
- [Accessibility improvements]
- [...]
```

### 4. Code Hiện Tại (Nếu Có)
```typescript
## Code Component Hiện Tại:
[Paste code của component cần thay đổi]
```

### 5. Ràng Buộc & Yêu Cầu
```
## Ràng Buộc:
- Giữ nguyên design system và màu sắc văn hóa
- Tương thích với architecture hiện tại
- Responsive mobile-first
- Không ảnh hưởng đến các chức năng khác

## Scope Thay Đổi:
- CHỈ thay đổi: [liệt kê cụ thể]
- KHÔNG thay đổi: [liệt kê cụ thể]
- Tích hợp với: [các component/service khác nếu cần]
```

## 🔍 Ví Dụ Cụ Thể

### Ví Dụ 1: Cải Thiện Delivery Tracking
```
## Dự Án Gốc: Vietnamese Traditional E-commerce Website
[Thông tin như trên]

## Chức Năng Cần Cải Thiện: Real-time Order Tracking

### Vị trí trong dự án:
- Module: VietDelivery
- Trang: OrderTrackingPage
- Component: OrderTrackingPage.tsx
- File path: /components/delivery/OrderTrackingPage.tsx

### Mô tả chức năng hiện tại:
Trang tracking hiện tại chỉ hiển thị status tĩnh với 4 bước cơ bản:
- Đã nhận đơn
- Đang lấy hàng  
- Đang giao hàng
- Đã giao thành công

### Yêu Cầu Cải Thiện:

#### Mục tiêu:
Tạo real-time tracking với bản đồ interactive và cập nhật vị trí tự động

#### Tính năng mới cần thêm:
- Bản đồ hiển thị vị trí shipper real-time
- Timeline chi tiết với timestamp
- Push notifications khi có cập nhật
- Ước tính thời gian giao hàng động
- Chat trực tiếp với shipper
- Photo proof khi giao hàng

#### Thay đổi UI/UX:
- Thêm bản đồ full-width ở top
- Timeline vertical với icons động
- Floating chat button
- Progress bar với percentage
- Shipper info card với avatar và rating

#### Yêu cầu kỹ thuật:
- Mock GPS tracking data
- WebSocket simulation cho real-time
- Image upload cho proof of delivery
- Local storage cho chat history

## Code Component Hiện Tại:
[Paste code của OrderTrackingPage.tsx]

## Ràng Buộc:
- Giữ nguyên color scheme VietDelivery (#10B981, #F97316)
- Tương thích với navigation flow hiện tại
- Responsive mobile-first
- Không ảnh hưởng đến other delivery pages

## Scope Thay Đổi:
- CHỈ thay đổi: OrderTrackingPage component và related UI
- KHÔNG thay đổi: Routing, state management, other delivery pages
- Tích hợp với: Existing toast system, support chat modal
```

### Ví Dụ 2: Thêm Tính Năng Spiritual Consultation Booking
```
## Dự Án Gốc: Vietnamese Traditional E-commerce Website
[Thông tin như trên]

## Chức Năng Cần Cải Thiện: Feng Shui Consultation Booking

### Vị trí trong dự án:
- Module: Tâm Linh Việt (Spiritual)
- Trang: FengShuiConsultationPage
- Component: FengShuiConsultationPage.tsx
- File path: /components/spiritual/FengShuiConsultationPage.tsx

### Mô tả chức năng hiện tại:
Trang hiện tại chỉ có thông tin cơ bản về dịch vụ feng shui và form liên hệ đơn giản

### Yêu Cầu Cải Thiện:

#### Mục tiêu:
Tạo hệ thống booking hoàn chỉnh với calendar, payment, và consultation flow

#### Tính năng mới cần thêm:
- Calendar booking với slot time
- Master profile selection
- Service package selection (basic/premium/vip)
- Online payment integration
- Virtual consultation setup
- Document upload (house plans, photos)
- Consultation history tracking

#### Thay đổi UI/UX:
- Multi-step booking wizard
- Interactive calendar component
- Master cards với rating và specialization
- Package comparison table
- File upload drag & drop
- Confirmation and receipt pages

#### Yêu cầu kỹ thuật:
- React Hook Form cho multi-step form
- Calendar component với available slots
- File upload with preview
- Mock payment gateway
- Email/SMS confirmation simulation

## Ràng Buộc:
- Giữ nguyên spiritual color scheme (#8B5CF6, #3B82F6, traditional gold)
- Tích hợp với existing prayer system
- Mobile-first responsive
- Cultural sensitivity cho spiritual content

## Scope Thay Đổi:
- CHỈ thay đổi: FengShuiConsultationPage và thêm các sub-components cần thiết
- KHÔNG thay đổi: Other spiritual pages, navigation, global state
- Tích hợp với: Toast notifications, spiritual AI chat, prayer feed
```

## 💡 Tips Quan Trọng

### 1. Cách Cung Cấp Code Hiện Tại
- Copy toàn bộ component hiện tại
- Bao gồm cả imports và dependencies
- Highlight phần code cần thay đổi
- Đề cập đến styling classes quan trọng

### 2. Mô Tả Requirement Chi Tiết
- Sử dụng user stories: "Là một khách hàng, tôi muốn..."
- Cung cấp mockup hoặc reference design nếu có
- Đề cập đến edge cases và error handling
- Clarify business logic và validation rules

### 3. Technical Context
- Đề cập đến state management hiện tại
- API endpoints nào cần mock
- Dependencies mới nào cần install
- Performance considerations

### 4. Testing & Validation
- Acceptance criteria rõ ràng
- Test scenarios cần cover
- Browser compatibility requirements
- Accessibility requirements

## 📝 Template Nhanh

Sao chép template này và điền thông tin:

```
## CONTEXT: Vietnamese E-commerce Website - [MODULE NAME]

### Target Component: [Component/Page Name]
### Location: [File path]
### Current Status: [Brief description]

### Improvement Goal:
[1-2 sentences về mục tiêu chính]

### New Features:
1. [Feature 1]
2. [Feature 2]
3. [...]

### UI/UX Changes:
- [Change 1]
- [Change 2]

### Technical Requirements:
- [Requirement 1]
- [Requirement 2]

### Constraints:
- Keep existing design system
- Maintain mobile-first responsive
- Integrate with existing [specify systems]

### Code Context:
[Paste relevant current code]

### Out of Scope:
- [What NOT to change]
```

## 🚀 Kết Luận

Với template này, bạn có thể cung cấp đầy đủ ngữ cảnh cho AI để:
1. Hiểu rõ dự án gốc và architecture
2. Tập trung vào chức năng cần cải thiện
3. Đảm bảo consistency với design system
4. Tránh ảnh hưởng đến các phần khác
5. Tạo ra solution phù hợp và chất lượng

Hãy sử dụng template này mỗi khi bạn muốn cải thiện một chức năng riêng lẻ!
# Vietnamese Traditional E-commerce Website - Complete Project Specifications

## 🎯 Project Overview
Xây dựng một website thương mại điện tử Việt Nam chuyên về đồ cúng và nghi lễ truyền thống với hai module dịch vụ chính: VietDelivery (giao hàng) và Tâm Linh Việt (dịch vụ tâm linh). Website có thiết kế văn hóa đậm đà, responsive mobile-first, và tích hợp đầy đủ các tính năng hiện đại.

## 🎨 Design System & Branding

### Color Palette
- **Primary Brown (Nâu ấm)**: #92400E - Màu chủ đạo văn hóa truyền thống
- **Golden Yellow (Vàng)**: #F59E0B - Màu phụ cho highlights và accents
- **Ritual Red (Đỏ nghi lễ)**: #DC2626 - Màu cho các yếu tố quan trọng
- **Cream Background (Nền kem)**: #FFFBEB - Màu nền chính
- **Delivery Green (Xanh giao hàng)**: #10B981 - Cho module delivery
- **Delivery Orange (Cam giao hàng)**: #F97316 - Màu phụ cho delivery
- **Spiritual Purple (Tím tâm linh)**: #8B5CF6 - Cho module spiritual
- **Spiritual Blue (Xanh tâm linh)**: #3B82F6 - Màu phụ cho spiritual

### Typography & Cultural Elements
- Typography tiếng Việt phù hợp với văn hóa truyền thống
- Họa tiết hoa sen, hình ảnh hương trầm
- Icons và symbols phù hợp với nghi lễ Việt Nam
- Responsive mobile-first design
- Tích hợp lịch âm và các yếu tố văn hóa

## 🏗️ Architecture & Navigation

### Main Application Structure
```
App.tsx (Main Router)
├── Header (Global Navigation)
├── Footer (Global Footer)
├── Main Content Area
├── Global Overlays:
│   ├── AdPopup (Promotional popups)
│   ├── ToastContainer (Notifications)
│   ├── PrayerFeed (Global prayer marquee)
│   ├── SupportChatModal (Customer support)
│   └── SpiritualAIChatBox (AI chat for spiritual pages)
```

### Navigation Structure
```
Main Navigation:
├── Trang Chủ (home)
├── Sản Phẩm (products)
├── Dịch Vụ (services)
├── Về Chúng Tôi (about)
├── Liên Hệ (contact)
├── Giỏ Hàng (cart)
├── Tài Khoản (profile)
├── Lịch Âm (calendar)
├── VietDelivery (delivery service)
├── Tâm Linh Việt (spiritual service)
├── Cộng Đồng (community)
└── Hỏi Đáp (qa)
```

## 📱 Core Pages & Features

### 1. HomePage (Trang Chủ)
**Components Required:**
- **Hero Section**: Banner chính với CTA buttons
- **Featured Products**: Sản phẩm nổi bật với carousel
- **Categories Grid**: Lưới danh mục sản phẩm
- **Ceremony Types Carousel**: Carousel các loại lễ (cưới, tang, khai trương, v.v.)
- **Customer Testimonials**: Đánh giá khách hàng
- **Lunar Calendar Widget**: Widget lịch âm với ngày tốt xấu
- **Service Promotion**: Quảng cáo hai module dịch vụ chính
- **Latest News**: Tin tức và bài viết mới nhất

### 2. ProductsPage (Trang Sản Phẩm)
**Features:**
- Product grid with filters (price, category, brand)
- Search functionality
- Product cards with images, prices, ratings
- Add to cart functionality
- Product comparison
- Related products suggestions

### 3. ServicesPage (Trang Dịch Vụ)
**Features:**
- Service categories overview
- Two main service modules promotion:
  - VietDelivery (Giao hàng)
  - Tâm Linh Việt (Dịch vụ tâm linh)
- Service booking interface
- Pricing information
- Service testimonials

### 4. AboutPage (Về Chúng Tôi)
**Content:**
- Company history and mission
- Cultural values and tradition preservation
- Team introduction
- Certifications and awards
- Community involvement

### 5. ContactPage (Liên Hệ)
**Features:**
- Contact form with validation
- Multiple contact methods
- Store locations with maps
- Operating hours
- FAQ section

### 6. CartPage (Giỏ Hàng)
**Features:**
- Cart items management
- Quantity adjustments
- Price calculations
- Delivery options
- Checkout process
- Applied coupons/discounts

### 7. ProfilePage (Tài Khoản)
**Features:**
- User profile management
- Order history
- Wishlist
- Address book
- Notification settings
- Loyalty points

### 8. CalendarPage (Lịch Âm)
**Features:**
- Vietnamese lunar calendar
- Good/bad days indicator
- Traditional festival dates
- Ceremony planning suggestions
- Export calendar events

## 🚚 VietDelivery Module (Complete Flow)

### Design Theme
- **Colors**: Green (#10B981) + Orange (#F97316)
- **Style**: Modern delivery app similar to ShopeeExpress/GrabExpress
- **Icons**: Truck, package, location, time-related icons

### Pages & Flow:

#### 1. DeliveryHomePage
**Features:**
- Service overview and benefits
- Quick booking CTA
- Price calculator preview
- Service areas map
- Driver availability status
- Recent orders tracking
- Customer reviews
- Navigation to all delivery subpages

#### 2. DeliveryBookingPage
**Features:**
- **Pickup & Delivery Forms**:
  - Pickup address with map integration
  - Delivery address with map integration
  - Contact information for both
  - Preferred time slots
- **Package Details**:
  - Package type selection
  - Weight and dimensions
  - Special handling requirements
  - Value declaration
- **Service Options**:
  - Standard delivery (4-6 hours)
  - Express delivery (2-3 hours)
  - Same-day delivery
  - Scheduled delivery
- Form validation and error handling
- Progress indicator

#### 3. PriceEstimationPage
**Features:**
- **Distance Calculation**: Automatic route calculation
- **Price Breakdown**:
  - Base fare by distance
  - Package weight surcharge
  - Service type premium
  - Special handling fees
  - Total cost calculation
- **Payment Options**:
  - Cash on delivery
  - Online payment methods
  - Corporate accounts
- **Booking Confirmation**: Final booking with all details
- **Receipt Generation**: Order reference number

#### 4. OrderTrackingPage
**Features:**
- **Real-time Tracking**:
  - Live map with driver location
  - Estimated time of arrival
  - Status updates (picked up, in transit, delivered)
- **Order Information**:
  - Order number and details
  - Driver information and contact
  - Vehicle information
- **Timeline View**: Step-by-step delivery progress
- **Notifications**: SMS/push notifications
- **Emergency Contact**: Direct call to driver/support

#### 5. OrderDetailsPage
**Features:**
- **Complete Order Summary**:
  - Pickup and delivery addresses
  - Package details and photos
  - Delivery time and date
  - Cost breakdown
- **Driver Details**:
  - Driver photo and name
  - Rating and reviews
  - Contact information
  - Vehicle details
- **Proof of Delivery**:
  - Delivery photos
  - Recipient signature
  - Delivery timestamp
- **Invoice/Receipt**: Downloadable PDF

#### 6. DriverRatingPage
**Features:**
- **Rating System**:
  - 5-star rating for driver
  - Service quality rating
  - Delivery time rating
- **Review Writing**:
  - Text feedback
  - Photo upload for issues
- **Driver Performance Metrics**:
  - Politeness
  - Vehicle condition
  - Packaging care
- **Tip Option**: Optional driver tip
- **Report Issues**: Damage or problem reporting

## 🕯️ Spiritual Services Module (Complete Flow)

### Design Theme
- **Colors**: Purple (#8B5CF6) + Blue (#3B82F6) + Traditional gold
- **Style**: Serene, cultural, temple-inspired design
- **Icons**: Lotus, incense, temple, yin-yang, Buddhist/Hindu symbols

### Pages & Flow:

#### 1. SpiritualHomePage
**Features:**
- **Service Overview**:
  - Virtual temple visits
  - Online ceremony booking
  - Feng shui consultation
  - Audio chanting and meditation
- **Daily Spiritual Content**:
  - Daily horoscope (Vietnamese style)
  - Spiritual advice
  - Lucky numbers/colors
- **Prayer Submission**: Global prayer form
- **Live Ceremonies**: Ongoing virtual ceremonies
- **Spiritual Calendar**: Important spiritual dates
- **Testimonials**: Customer spiritual experiences

#### 2. VirtualIncensePage
**Features:**
- **Virtual Incense Offering**:
  - 3D incense lighting animation
  - Multiple incense types (trầm hương, trúc, v.v.)
  - Prayer text input
  - Dedication options (family, ancestors, v.v.)
- **Temple Selection**:
  - Famous Vietnamese temples
  - Different deity specializations
  - Temple virtual tours
- **Offering Packages**:
  - Basic incense offering
  - Premium ceremony packages
  - Monthly subscription options
- **Prayer History**: Previous offerings and prayers
- **Sharing Options**: Share blessings on social media

#### 3. AudioChantingPage
**Features:**
- **Chanting Library**:
  - Buddhist sutras (Kinh Phật)
  - Traditional Vietnamese chants
  - Meditation music
  - Temple bell sounds
- **Playlist Creation**: Custom spiritual playlists
- **Meditation Timer**: Guided meditation sessions
- **Audio Quality Options**: Different bitrates
- **Download for Offline**: Offline listening
- **Background Play**: Continue while using other apps
- **Sleep Timer**: Auto-stop functionality

#### 4. FengShuiConsultationPage
**Features:**
- **Consultation Booking**:
  - Expert feng shui masters
  - Virtual or in-person options
  - Scheduling system
  - Price packages
- **Home Analysis Tools**:
  - Upload house floor plans
  - Photo analysis
  - Room-by-room evaluation
  - Bagua map overlay
- **Personal Consultation**:
  - Birth date analysis (ngày sinh theo âm lịch)
  - Element compatibility
  - Lucky directions and colors
  - Career and health guidance
- **Feng Shui Store**: Recommended items and placements
- **Report Generation**: Detailed PDF reports

## 🌟 Additional Features

### 1. CommunityPage (Social Media Feed)
**Features:**
- **Social Feed Interface**:
  - Instagram-style photo sharing
  - Community posts and discussions
  - Spiritual journey sharing
  - Traditional recipe sharing
- **Upload Functionality**:
  - Multi-photo upload
  - Photo filters and editing
  - Caption and hashtag support
  - Location tagging
- **Interaction Features**:
  - Like, comment, share
  - Follow users
  - Private messaging
  - Community groups
- **Content Categories**:
  - Spiritual experiences
  - Traditional ceremonies
  - Cultural knowledge
  - Recipe sharing

### 2. QAPage (AI Chat for Spiritual Guidance)
**Features:**
- **AI Spiritual Assistant**:
  - Vietnamese language support
  - Traditional spiritual knowledge
  - Feng shui guidance
  - Cultural ceremony advice
- **Chat Interface**:
  - Real-time messaging
  - Voice message support
  - Image analysis for feng shui
  - Conversation history
- **Knowledge Base**:
  - FAQ about rituals
  - Traditional ceremony guides
  - Spiritual calendar explanations
  - Cultural significance articles
- **Expert Escalation**: Connect to human experts

### 3. Prayer System (Global Feature)
**Components:**
- **PrayerForm**: Prayer submission interface
- **PrayerFeed**: Marquee-style prayer display
- **Features**:
  - Anonymous prayer submission
  - Prayer categories (health, family, success, v.v.)
  - Public prayer sharing option
  - Prayer statistics
  - Marquee runs for 5 seconds across screen
  - Auto-refresh with new prayers

## 🛠️ Technical Implementation

### State Management
```typescript
// Main App State
- currentPage: string (routing)
- deliveryPage: string (delivery module routing)
- spiritualPage: string (spiritual module routing)
- showSpiritualChat: boolean
- showSupportChat: boolean
- prayers: Prayer[] (global prayer array)
- toasts: Toast[] (notification system)
```

### Component Structure
```
/components
├── /delivery (All delivery module components)
├── /spiritual (All spiritual module components)
├── /ui (Reusable UI components from shadcn)
├── Core pages (HomePage, ProductsPage, etc.)
├── Global components (Header, Footer, etc.)
└── Modal/Overlay components
```

### Key Libraries & Integrations
- **UI Framework**: React + TypeScript
- **Styling**: Tailwind CSS v4
- **UI Components**: shadcn/ui
- **Icons**: Lucide React
- **Images**: Unsplash API integration
- **Animations**: Framer Motion for smooth transitions
- **Charts**: Recharts for analytics
- **Notifications**: Custom toast system
- **Forms**: React Hook Form with validation

### Responsive Design
- Mobile-first approach
- Breakpoints: sm, md, lg, xl
- Touch-friendly interactions
- Optimized performance for mobile
- Progressive Web App features

### Cultural Considerations
- Vietnamese language support
- Traditional color schemes
- Lunar calendar integration
- Cultural symbols and iconography
- Respectful spiritual content presentation
- Traditional ceremony accuracy

## 🚀 Implementation Priority

### Phase 1: Core E-commerce (Completed)
- [x] Basic navigation and routing
- [x] Main pages (Home, Products, Services, etc.)
- [x] Shopping cart functionality
- [x] User profile system

### Phase 2: Service Modules (Completed)
- [x] Complete VietDelivery flow (6 pages)
- [x] Complete Spiritual Services flow (4 pages)
- [x] Inter-service navigation
- [x] Service-specific UI themes

### Phase 3: Community & Engagement (Completed)
- [x] Community social feed with photo upload
- [x] AI-powered QA chat system
- [x] Global prayer submission and marquee feed
- [x] Support chat system

### Phase 4: Enhancement & Polish (Completed)
- [x] Toast notification system
- [x] Ad popup system
- [x] Responsive design optimization
- [x] Cultural design refinements

## 📋 Usage Instructions

To recreate this project:

1. **Setup**: Create new React + TypeScript project with Tailwind CSS v4
2. **UI Components**: Install and configure shadcn/ui library
3. **File Structure**: Create the component directory structure as shown
4. **Color System**: Implement the cultural color palette
5. **Core Pages**: Build main e-commerce pages first
6. **Service Modules**: Implement delivery and spiritual flows
7. **Community Features**: Add social and prayer features
8. **Global Systems**: Implement notifications, chat, and overlays
9. **Testing**: Ensure responsive design and cultural accuracy
10. **Refinement**: Polish UI/UX and add Vietnamese content

## 🎯 Success Criteria

- Fully functional e-commerce website with cultural Vietnamese design
- Complete delivery service flow with real-time tracking simulation
- Comprehensive spiritual services with authentic cultural elements
- Social community features with photo sharing
- AI chat system for spiritual guidance
- Global prayer system with live marquee feed
- Responsive mobile-first design
- Professional UI/UX with smooth animations
- Ready for backend API integration
- Cultural authenticity and respect for Vietnamese traditions

---

**Note**: This is a comprehensive specification for a complete Vietnamese e-commerce platform with modern service modules. The project demonstrates advanced React development, complex state management, cultural design sensitivity, and full-stack readiness.
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { Toaster } from "sonner";
import { GoogleOAuthProvider } from "@react-oauth/google";
import { OpenAPI } from "@/api/generated-client";
import { request } from "@/api/generated-client/core/request";

// Context Provider
import { AppProvider } from "./lib/contexts/AppContext";

// Layouts
import { MainLayout } from "./layouts/MainLayout";
import { AuthLayout } from "./layouts/AuthLayout";
import { DeliveryLayout } from "./layouts/DeliveryLayout";
import { SpiritualLayout } from "./layouts/SpiritualLayout";

// Main Pages
import { HomePage } from "./components/HomePage";
import { ProductsPage } from "./components/ProductsPage";
import { ProductDetailPage } from "./pages/ProductDetailPage";
import { ServicesPage } from "./components/ServicesPage";
import { ServiceDetailPage } from "./components/ServiceDetailPage";
import { AboutPage } from "./components/AboutPage";
import { ContactPage } from "./components/ContactPage";
import { CartPage } from "./components/CartPage";
import { ProfilePage } from "./components/ProfilePage";
import CalendarPage from "./components/CalendarPage";
import WishlistPage from "./components/WishlistPage";
import { CommunityPage } from "./components/community";
import { QAPage } from "./components/QAPage";

// Auth Pages
import { LoginPage } from "./pages/auth/LoginPage";
import { RegisterPage } from "./pages/auth/RegisterPage";
import { ForgotPasswordPage } from "./pages/auth/ForgotPasswordPage";

// Delivery Service Pages
import { DeliveryHomePage } from "./components/delivery/DeliveryHomePage";
import { DeliveryBookingPage } from "./components/delivery/DeliveryBookingPage";
import { PriceEstimationPage } from "./components/delivery/PriceEstimationPage";
import { OrderTrackingPage } from "./components/delivery/OrderTrackingPage";
import { OrderDetailsPage } from "./components/delivery/OrderDetailsPage";
import { DriverRatingPage } from "./components/delivery/DriverRatingPage";

// Spiritual Service Pages
import { SpiritualHomePage } from "./components/spiritual/SpiritualHomePage";
import { VirtualIncensePage } from "./components/spiritual/VirtualIncensePage";
import { AudioChantingPage } from "./components/spiritual/AudioChantingPage";
import { FengShuiConsultationPage } from "./components/spiritual/FengShuiConsultationPage";

// Checkout Flow
import { CheckoutFlow } from "./pages/checkout/CheckoutFlow";

// Calendar Booking
import { BookingSuccessPage } from "./pages/calendar/BookingSuccessPage";
import { BookingHistoryPage } from "./pages/calendar/BookingHistoryPage";

// Order Detail
import { OrderDetailPage } from "./pages/OrderDetailPage";

// Create QueryClient instance
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
      staleTime: 30000, // 30 seconds
    },
    mutations: {
      retry: 0,
    },
  },
});

export default function App() {
  const googleClientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;

  if (!googleClientId) {
    console.warn('[App] ⚠️ Google Client ID not configured in .env');
  }

  return (
    <GoogleOAuthProvider 
      clientId={googleClientId || ''}
      onScriptLoad={() => console.log('[App] Google OAuth script loaded')}
    >
      <BrowserRouter>
        <QueryClientProvider client={queryClient}>
          <AppProvider>
          <Routes>
            {/* Main Routes with Header/Footer */}
            <Route element={<MainLayout />}>
              <Route path="/" element={<HomePage />} />
              <Route path="/products" element={<ProductsPage />} />
              <Route path="/product/:id" element={<ProductDetailPage />} />
              <Route path="/services" element={<ServicesPage />} />
              <Route path="/services/:id" element={<ServiceDetailPage />} />
              <Route path="/about" element={<AboutPage />} />
              <Route path="/contact" element={<ContactPage />} />
              <Route path="/cart" element={<CartPage />} />
              <Route path="/wishlist" element={<WishlistPage />} />
              <Route path="/profile" element={<ProfilePage />} />
              <Route path="/calendar" element={<CalendarPage />} />
              <Route
                path="/community"
                element={<CommunityPage onBack={() => window.history.back()} />}
              />
              <Route path="/qa" element={<QAPage />} />
            </Route>

            {/* Auth Routes (no header/footer) */}
            <Route path="/auth" element={<AuthLayout />}>
              <Route path="login" element={<LoginPage />} />
              <Route path="register" element={<RegisterPage />} />
              <Route path="forgot-password" element={<ForgotPasswordPage />} />
            </Route>

            {/* Delivery Service Routes (nested) */}
            <Route path="/delivery" element={<DeliveryLayout />}>
              <Route index element={<DeliveryHomePage />} />
              <Route path="booking" element={<DeliveryBookingPage />} />
              <Route
                path="price-estimation"
                element={<PriceEstimationPage />}
              />
              <Route path="tracking" element={<OrderTrackingPage />} />
              <Route path="order/:orderId" element={<OrderDetailsPage />} />
              <Route path="rating/:orderId" element={<DriverRatingPage />} />
            </Route>

            {/* Spiritual Service Routes (nested) */}
            <Route path="/spiritual" element={<SpiritualLayout />}>
              <Route index element={<SpiritualHomePage />} />
              <Route path="virtual-incense" element={<VirtualIncensePage />} />
              <Route path="audio-chanting" element={<AudioChantingPage />} />
              <Route
                path="fengshui-consultation"
                element={<FengShuiConsultationPage />}
              />
            </Route>

            {/* Checkout Flow (standalone with internal state navigation) */}
            <Route path="/checkout" element={<CheckoutFlow />} />

            {/* Calendar Booking Pages */}
            <Route path="/bookings" element={<BookingHistoryPage />} />
            <Route path="/bookings/:orderId" element={<BookingSuccessPage />} />

            {/* Order Detail Page */}
            <Route path="/orders/:orderId" element={<OrderDetailPage />} />

            {/* Catch all - redirect to home */}
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>

            {/* Global Toast Notifications */}
            <Toaster position="top-right" richColors />

            {/* React Query DevTools */}
            <ReactQueryDevtools initialIsOpen={false} />
          </AppProvider>
        </QueryClientProvider>
      </BrowserRouter>
    </GoogleOAuthProvider>
  );
}

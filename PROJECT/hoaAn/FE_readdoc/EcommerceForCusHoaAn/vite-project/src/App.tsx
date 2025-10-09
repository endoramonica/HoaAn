import { useState } from 'react';
import { Header } from './components/Header';
import { Footer } from './components/Footer';
import { HomePage } from './components/HomePage';
import { ProductsPage } from './components/ProductsPage';
import { ServicesPage } from './components/ServicesPage';
import { AboutPage } from './components/AboutPage';
import { ContactPage } from './components/ContactPage';
import { CartPage } from './components/CartPage';
import { ProfilePage } from './components/ProfilePage';
import { CalendarPage } from './components/CalendarPage';
import { AdPopup } from './components/AdPopup';

// Delivery Service Components
import { DeliveryHomePage } from './components/delivery/DeliveryHomePage';
import { DeliveryBookingPage } from './components/delivery/DeliveryBookingPage';
import { PriceEstimationPage } from './components/delivery/PriceEstimationPage';
import { OrderTrackingPage } from './components/delivery/OrderTrackingPage';
import { OrderDetailsPage } from './components/delivery/OrderDetailsPage';
import { DriverRatingPage } from './components/delivery/DriverRatingPage';

// Spiritual Service Components
import { SpiritualHomePage } from './components/spiritual/SpiritualHomePage';
import { VirtualIncensePage } from './components/spiritual/VirtualIncensePage';
import { AudioChantingPage } from './components/spiritual/AudioChantingPage';
import { FengShuiConsultationPage } from './components/spiritual/FengShuiConsultationPage';
import { SpiritualAIChatBox } from './components/spiritual/SpiritualAIChatBox';
import { ToastContainer, useToast } from './components/ui/toast';
import { SupportChatModal } from './components/SupportChatModal';
import { CommunityPage } from './components/CommunityPage';
import { QAPage } from './components/QAPage';
import { PrayerForm, Prayer } from './components/spiritual/PrayerForm';
import { PrayerFeed } from './components/spiritual/PrayerFeed';

export default function App() {
  const [currentPage, setCurrentPage] = useState('home');
  const [deliveryPage, setDeliveryPage] = useState('home');
  const [spiritualPage, setSpiritualPage] = useState('home');
  const [showSpiritualChat, setShowSpiritualChat] = useState(false);
  const [showSupportChat, setShowSupportChat] = useState(false);
  const [prayers, setPrayers] = useState<Prayer[]>([]);
  const { toasts, addToast, removeToast } = useToast();

  const renderDeliveryPage = () => {
    switch (deliveryPage) {
      case 'home':
        return <DeliveryHomePage onNavigate={setCurrentPage} onDeliveryNavigate={setDeliveryPage} />;
      case 'delivery-booking':
        return <DeliveryBookingPage onBack={() => setDeliveryPage('home')} onNavigate={setDeliveryPage} />;
      case 'price-estimation':
        return <PriceEstimationPage onBack={() => setDeliveryPage('delivery-booking')} onNavigate={setDeliveryPage} />;
      case 'order-tracking':
        return <OrderTrackingPage onBack={() => setDeliveryPage('home')} onNavigate={setDeliveryPage} />;
      case 'order-details':
        return <OrderDetailsPage onBack={() => setDeliveryPage('order-tracking')} onNavigate={setDeliveryPage} />;
      case 'driver-rating':
        return <DriverRatingPage onBack={() => setDeliveryPage('order-details')} onComplete={() => setDeliveryPage('home')} />;
      default:
        return <DeliveryHomePage onNavigate={setCurrentPage} onDeliveryNavigate={setDeliveryPage} />;
    }
  };

  const renderSpiritualPage = () => {
    switch (spiritualPage) {
      case 'home':
        return (
          <SpiritualHomePage 
            onNavigate={setCurrentPage} 
            onSpiritualNavigate={setSpiritualPage}
            onSubmitPrayer={handleSubmitPrayer}
            onAddToast={addToast}
          />
        );
      case 'virtual-incense':
        return <VirtualIncensePage onBack={() => setSpiritualPage('home')} />;
      case 'audio-chanting':
        return <AudioChantingPage onBack={() => setSpiritualPage('home')} />;
      case 'fengshui-consultation':
        return <FengShuiConsultationPage onBack={() => setSpiritualPage('home')} />;
      default:
        return <SpiritualHomePage onNavigate={setCurrentPage} onSpiritualNavigate={setSpiritualPage} />;
    }
  };

  const renderPage = () => {
    switch (currentPage) {
      case 'home':
        return <HomePage onNavigate={handleNavigate} />;
      case 'products':
        return <ProductsPage />;
      case 'services':
        return <ServicesPage />;
      case 'about':
        return <AboutPage />;
      case 'contact':
        return <ContactPage />;
      case 'cart':
        return <CartPage />;
      case 'profile':
        return <ProfilePage />;
      case 'calendar':
        return <CalendarPage onBack={() => setCurrentPage('home')} />;
      case 'delivery':
        return renderDeliveryPage();
      case 'spiritual':
        return renderSpiritualPage();
      case 'community':
        return <CommunityPage onBack={() => setCurrentPage('home')} />;
      case 'qa':
        return <QAPage onBack={() => setCurrentPage('home')} />;
      default:
        return <HomePage onNavigate={setCurrentPage} />;
    }
  };

  // Show welcome toast when navigating to services
  const handleNavigate = (page: string) => {
    setCurrentPage(page);
    if (page === 'delivery') {
      addToast({
        type: 'info',
        title: 'Chào mừng đến VietDelivery!',
        description: 'Giao hàng nhanh chóng, an toàn trong 2 giờ'
      });
    } else if (page === 'spiritual') {
      addToast({
        type: 'info', 
        title: 'Chào mừng đến Tâm Linh Việt!',
        description: 'Kết nối với năng lượng thiêng liêng, tĩnh tâm an lạc'
      });
    } else if (page === 'community') {
      addToast({
        type: 'info',
        title: 'Chào mừng đến Cộng đồng!',
        description: 'Chia sẻ kiến thức và kết nối với cộng đồng tâm linh'
      });
    }
  };

  const handleSubmitPrayer = (prayer: Prayer) => {
    setPrayers(prev => [...prev, prayer]);
  };

  return (
    <div className="min-h-screen">
      <Header 
        currentPage={currentPage} 
        onNavigate={handleNavigate} 
        onToggleSupport={() => setShowSupportChat(!showSupportChat)}
      />
      {renderPage()}
      <Footer />
      <AdPopup currentPage={currentPage} onNavigate={handleNavigate} />
      <ToastContainer toasts={toasts} onRemove={removeToast} />
      
      {/* Prayer Feed - Global overlay */}
      <PrayerFeed prayers={prayers} />
      
      {/* Support Chat - Global */}
      <SupportChatModal 
        isVisible={showSupportChat} 
        onToggle={() => setShowSupportChat(!showSupportChat)} 
      />
      
      {/* Spiritual AI Chat - available on spiritual pages */}
      {currentPage === 'spiritual' && (
        <SpiritualAIChatBox 
          isVisible={showSpiritualChat} 
          onToggle={() => setShowSpiritualChat(!showSpiritualChat)} 
        />
      )}
    </div>
  );
}

import { Outlet, useLocation } from 'react-router-dom';
import { Header } from '../components/Header';
import { Footer } from '../components/Footer';
import { AdPopup } from '../components/AdPopup';
import { PrayerFeed } from '../components/spiritual/PrayerFeed';
import { SupportChatModal } from '../components/SupportChatModal';
import { useApp } from '../lib/contexts/AppContext';

export function MainLayout() {
  const location = useLocation();
  const { showSupportChat, setShowSupportChat, prayers } = useApp();

  // Extract page name from pathname for AdPopup
  const currentPage = location.pathname.slice(1) || 'home';

  return (
    <div className="min-h-screen flex flex-col">
      <Header />
      
      <main className="flex-1">
        <Outlet />
      </main>
      
      <Footer />
      
      {/* Global Components */}
      <AdPopup currentPage={currentPage} />
      <PrayerFeed prayers={prayers} />
      <SupportChatModal 
        isVisible={showSupportChat} 
        onToggle={() => setShowSupportChat(!showSupportChat)} 
      />
    </div>
  );
}

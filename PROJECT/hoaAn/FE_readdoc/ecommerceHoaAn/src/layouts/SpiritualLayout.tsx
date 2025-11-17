import { Outlet, useLocation } from 'react-router-dom';
import { useEffect } from 'react';
import { toast } from 'sonner@2.0.3';
import { SpiritualAIChatBox } from '../components/spiritual/SpiritualAIChatBox';
import { useApp } from '../lib/contexts/AppContext';

/**
 * Layout for Spiritual Service pages
 * Includes AI chatbox and welcome toast
 */
export function SpiritualLayout() {
  const location = useLocation();
  const { showSpiritualChat, setShowSpiritualChat } = useApp();

  // Show welcome toast on first visit to spiritual service
  useEffect(() => {
    if (location.pathname === '/spiritual') {
      toast.info('Chào mừng đến Tâm Linh Việt!', {
        description: 'Kết nối với năng lượng thiêng liêng, tĩnh tâm an lạc'
      });
    }
  }, [location.pathname]);

  return (
    <div className="min-h-screen">
      <Outlet />
      
      {/* Spiritual AI Chat available on all spiritual pages */}
      <SpiritualAIChatBox 
        isVisible={showSpiritualChat} 
        onToggle={() => setShowSpiritualChat(!showSpiritualChat)} 
      />
    </div>
  );
}

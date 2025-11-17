import { Outlet, useLocation } from 'react-router-dom';
import { useEffect } from 'react';
import { toast } from 'sonner@2.0.3';

/**
 * Layout for VietDelivery service pages
 * Wraps delivery routes with consistent layout and welcome toast
 */
export function DeliveryLayout() {
  const location = useLocation();

  // Show welcome toast on first visit to delivery service
  useEffect(() => {
    if (location.pathname === '/delivery') {
      toast.info('Chào mừng đến VietDelivery!', {
        description: 'Giao hàng nhanh chóng, an toàn trong 2 giờ'
      });
    }
  }, [location.pathname]);

  return (
    <div className="min-h-screen">
      <Outlet />
    </div>
  );
}

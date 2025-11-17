import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from './ui/button';
import { Card, CardContent } from './ui/card';
import { Badge } from './ui/badge';
import { Dialog, DialogContent } from './ui/dialog';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { 
  X, 
  Gift, 
  Star, 
  Clock, 
  Sparkles,
  Flower2,
  ChevronRight,
  Calendar,
  ShoppingBag,
  Heart,
  Zap
} from 'lucide-react';

interface Advertisement {
  id: string;
  type: 'modal' | 'banner';
  title: string;
  subtitle?: string;
  description: string;
  image: string;
  ctaText: string;
  ctaLink: string;
  discount?: {
    percentage: number;
    code: string;
  };
  priority: number;
  targeting: {
    pages: string[];
    frequency: 'once-per-session' | 'once-per-page' | 'always';
    delayMs: number;
    autoDismissMs?: number;
  };
  validity: {
    startDate: string;
    endDate: string;
  };
  isActive: boolean;
}

interface AdPopupProps {
  currentPage: string;
}

export function AdPopup({ currentPage }: AdPopupProps) {
  const navigate = useNavigate();
  const [currentAd, setCurrentAd] = useState<Advertisement | null>(null);
  const [isVisible, setIsVisible] = useState(false);
  const [countdown, setCountdown] = useState<number | null>(null);

  // Mock advertisement data
  const advertisements: Advertisement[] = [
    {
      id: 'tet-2025-sale',
      type: 'modal',
      title: 'Đại hạ giá Tết Nguyên Đán 2025',
      subtitle: 'Chuẩn bị đồ cúng Tết trọn gói',
      description: 'Mâm cúng gia tiên, hương nến cao cấp, giấy tiền vàng bạc. Giao hàng tận nhà trong ngày, tư vấn ngày giờ tốt miễn phí.',
      image: 'https://images.unsplash.com/photo-1519097000072-e44ffa116485?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwb2ZmZXJpbmdzJTIwYWx0YXIlMjBmcnVpdHN8ZW58MXx8fHwxNzU3Njc0NDI5fDA&ixlib=rb-4.1.0&q=80&w=1080',
      ctaText: 'Mua ngay - Giảm 40%',
      ctaLink: 'products',
      discount: {
        percentage: 40,
        code: 'TET2025'
      },
      priority: 1,
      targeting: {
        pages: ['home', 'products'],
        frequency: 'once-per-session',
        delayMs: 3000,
        autoDismissMs: 15000
      },
      validity: {
        startDate: '2025-01-01',
        endDate: '2025-02-15'
      },
      isActive: true
    },
    {
      id: 'vip-membership',
      type: 'modal',
      title: 'Trở thành thành viên VIP',
      subtitle: 'Ưu đãi đặc biệt cho khách hàng thân thiết',
      description: 'Giảm giá 15% mọi đơn hàng, miễn phí vận chuyển, tư vấn phong thủy 24/7 và nhiều đặc quyền khác.',
      image: 'https://images.unsplash.com/photo-1573460630303-81cbaf895c54?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsb3R1cyUyMGZsb3dlciUyMGNlcmVtb25pYWwlMjBnb2xkfGVufDF8fHx8MTc1NzY3NDQyOXww&ixlib=rb-4.1.0&q=80&w=1080',
      ctaText: 'Đăng ký VIP ngay',
      ctaLink: 'profile',
      priority: 2,
      targeting: {
        pages: ['cart', 'profile'],
        frequency: 'once-per-session',
        delayMs: 5000,
        autoDismissMs: 20000
      },
      validity: {
        startDate: '2025-01-01',
        endDate: '2025-12-31'
      },
      isActive: true
    },
    {
      id: 'lunar-calendar-promo',
      type: 'banner',
      title: 'Xem lịch âm - Chọn ngày tốt',
      description: 'Tư vấn miễn phí ngày giờ tốt cho mọi nghi lễ. Cập nhật lịch âm 2025 đầy đủ.',
      image: 'https://images.unsplash.com/photo-1532334722716-c5850cdd878d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwaW5jZW5zZSUyMGNlcmVtb255JTIwdHJhZGl0aW9uYWx8ZW58MXx8fHwxNzU3Njc0NDI4fDA&ixlib=rb-4.1.0&q=80&w=1080',
      ctaText: 'Xem lịch ngay',
      ctaLink: 'calendar',
      priority: 3,
      targeting: {
        pages: ['services', 'about'],
        frequency: 'once-per-page',
        delayMs: 2000,
        autoDismissMs: 10000
      },
      validity: {
        startDate: '2025-01-01',
        endDate: '2025-12-31'
      },
      isActive: true
    }
  ];

  // Get interactions from localStorage
  const getStoredInteractions = () => {
    try {
      const stored = localStorage.getItem('ad-interactions');
      return stored ? JSON.parse(stored) : {};
    } catch {
      return {};
    }
  };

  // Store interaction
  const storeInteraction = (adId: string, action: 'shown' | 'clicked' | 'dismissed') => {
    try {
      const interactions = getStoredInteractions();
      const sessionId = sessionStorage.getItem('session-id') || 
                       Math.random().toString(36).substring(7);
      
      if (!sessionStorage.getItem('session-id')) {
        sessionStorage.setItem('session-id', sessionId);
      }

      const key = `${adId}-${action}`;
      const now = new Date().toISOString();
      
      if (!interactions[key]) {
        interactions[key] = [];
      }
      
      interactions[key].push({
        timestamp: now,
        sessionId,
        page: currentPage
      });

      localStorage.setItem('ad-interactions', JSON.stringify(interactions));
    } catch (error) {
      console.error('Failed to store ad interaction:', error);
    }
  };

  // Check if ad should be shown
  const shouldShowAd = (ad: Advertisement): boolean => {
    if (!ad.isActive) return false;
    
    // Check validity dates
    const now = new Date();
    const startDate = new Date(ad.validity.startDate);
    const endDate = new Date(ad.validity.endDate);
    if (now < startDate || now > endDate) return false;

    // Check page targeting
    if (!ad.targeting.pages.includes(currentPage)) return false;

    const interactions = getStoredInteractions();
    const sessionId = sessionStorage.getItem('session-id');

    switch (ad.targeting.frequency) {
      case 'once-per-session':
        const sessionShown = interactions[`${ad.id}-shown`]?.some(
          (interaction: any) => interaction.sessionId === sessionId
        );
        return !sessionShown;
      
      case 'once-per-page':
        const pageShown = interactions[`${ad.id}-shown`]?.some(
          (interaction: any) => 
            interaction.sessionId === sessionId && 
            interaction.page === currentPage
        );
        return !pageShown;
      
      case 'always':
        return true;
      
      default:
        return false;
    }
  };

  // Find the best ad to show
  const findAdToShow = (): Advertisement | null => {
    const eligibleAds = advertisements
      .filter(shouldShowAd)
      .sort((a, b) => a.priority - b.priority);
    
    return eligibleAds[0] || null;
  };

  // Show ad with delay
  useEffect(() => {
    const adToShow = findAdToShow();
    if (!adToShow) return;

    const timer = setTimeout(() => {
      setCurrentAd(adToShow);
      setIsVisible(true);
      storeInteraction(adToShow.id, 'shown');

      // Set up auto-dismiss if configured
      if (adToShow.targeting.autoDismissMs) {
        setCountdown(Math.ceil(adToShow.targeting.autoDismissMs / 1000));
      }
    }, adToShow.targeting.delayMs);

    return () => clearTimeout(timer);
  }, [currentPage]);

  // Handle countdown
  useEffect(() => {
    if (countdown === null || countdown <= 0) return;

    const timer = setTimeout(() => {
      if (countdown === 1) {
        handleClose();
      } else {
        setCountdown(countdown - 1);
      }
    }, 1000);

    return () => clearTimeout(timer);
  }, [countdown]);

  const handleClose = () => {
    if (currentAd) {
      storeInteraction(currentAd.id, 'dismissed');
    }
    setIsVisible(false);
    setCurrentAd(null);
    setCountdown(null);
  };

  const handleClick = () => {
    if (currentAd) {
      storeInteraction(currentAd.id, 'clicked');
      navigate(currentAd.ctaLink);
      handleClose();
    }
  };

  if (!currentAd || !isVisible) return null;

  // Modal Advertisement
  if (currentAd.type === 'modal') {
    return (
      <Dialog open={isVisible} onOpenChange={handleClose}>
        <DialogContent 
          className="max-w-md w-full border-2 border-amber-300 shadow-2xl p-0 gap-0"
          onInteractOutside={(e) => e.preventDefault()}
          aria-labelledby="ad-title"
          aria-describedby="ad-description"
        >
          <div className="relative overflow-hidden">
            {/* Close Button */}
            <Button
              variant="ghost"
              size="icon"
              onClick={handleClose}
              className="absolute top-2 right-2 z-10 bg-black/20 text-white hover:bg-black/40 rounded-full"
              aria-label="Đóng quảng cáo"
            >
              <X className="w-4 h-4" />
            </Button>

            {/* Countdown Badge */}
            {countdown && countdown > 0 && (
              <Badge className="absolute top-2 left-2 z-10 bg-red-600 text-white">
                <Clock className="w-3 h-3 mr-1" />
                {countdown}s
              </Badge>
            )}

            {/* Header Image */}
            <div className="relative h-48 bg-gradient-to-br from-amber-600 to-red-600">
              <ImageWithFallback
                src={currentAd.image}
                alt={currentAd.title}
                className="w-full h-full object-cover opacity-80"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/50 to-transparent" />
              
              {/* Discount Badge */}
              {currentAd.discount && (
                <div className="absolute bottom-4 left-4 flex items-center gap-2">
                  <Badge className="bg-yellow-500 text-black px-3 py-1 text-lg">
                    <Gift className="w-4 h-4 mr-1" />
                    -{currentAd.discount.percentage}%
                  </Badge>
                  <Badge variant="outline" className="bg-white/90 text-red-600 border-red-300">
                    {currentAd.discount.code}
                  </Badge>
                </div>
              )}

              {/* Decorative Elements */}
              <Flower2 className="absolute top-4 left-4 w-8 h-8 text-yellow-300 opacity-70" />
              <Sparkles className="absolute top-8 right-8 w-6 h-6 text-yellow-300 opacity-70" />
            </div>

            {/* Content */}
            <div className="p-6 bg-gradient-to-br from-yellow-50 to-red-50">
              <div className="text-center mb-4">
                <h2 id="ad-title" className="text-2xl text-amber-900 mb-2">
                  {currentAd.title}
                </h2>
                {currentAd.subtitle && (
                  <p className="text-lg text-red-700 mb-3">{currentAd.subtitle}</p>
                )}
                <p id="ad-description" className="text-gray-700 text-sm leading-relaxed">
                  {currentAd.description}
                </p>
              </div>

              {/* Features */}
              <div className="grid grid-cols-2 gap-3 mb-6">
                <div className="flex items-center gap-2 text-sm text-green-600">
                  <Zap className="w-4 h-4" />
                  <span>Giao nhanh 2h</span>
                </div>
                <div className="flex items-center gap-2 text-sm text-blue-600">
                  <Star className="w-4 h-4" />
                  <span>Chất lượng A+</span>
                </div>
                <div className="flex items-center gap-2 text-sm text-purple-600">
                  <Calendar className="w-4 h-4" />
                  <span>Tư vấn miễn phí</span>
                </div>
                <div className="flex items-center gap-2 text-sm text-orange-600">
                  <Heart className="w-4 h-4" />
                  <span>Đảm bảo hài lòng</span>
                </div>
              </div>

              {/* CTA Buttons */}
              <div className="space-y-3">
                <Button 
                  onClick={handleClick}
                  className="w-full bg-gradient-to-r from-red-600 to-red-700 hover:from-red-700 hover:to-red-800 text-white py-3 text-lg shadow-lg transform transition-all duration-200 hover:scale-105"
                  role="button"
                  aria-label={`${currentAd.ctaText} - Chuyển đến trang sản phẩm`}
                >
                  <ShoppingBag className="w-5 h-5 mr-2" />
                  {currentAd.ctaText}
                  <ChevronRight className="w-5 h-5 ml-2" />
                </Button>

                <Button 
                  variant="outline" 
                  onClick={handleClose}
                  className="w-full border-amber-300 text-amber-700 hover:bg-amber-50"
                >
                  Để sau
                </Button>
              </div>

              {/* Trust Indicators */}
              <div className="flex justify-center items-center gap-4 mt-4 pt-4 border-t border-amber-200">
                <div className="flex items-center gap-1">
                  {[...Array(5)].map((_, i) => (
                    <Star key={i} className="w-3 h-3 fill-yellow-400 text-yellow-400" />
                  ))}
                  <span className="text-xs text-gray-600 ml-1">4.9/5</span>
                </div>
                <div className="text-xs text-gray-600">1000+ khách hàng hài lòng</div>
              </div>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    );
  }

  // Banner Advertisement
  if (currentAd.type === 'banner') {
    return (
      <div 
        className="fixed bottom-4 left-4 right-4 z-50 animate-in slide-in-from-bottom-5 duration-500"
        role="banner"
        aria-labelledby="banner-title"
      >
        <Card className="border-2 border-amber-300 shadow-xl overflow-hidden max-w-md mx-auto">
          <div className="relative bg-gradient-to-r from-amber-600 to-yellow-600 p-4">
            {/* Close Button */}
            <Button
              variant="ghost"
              size="icon"
              onClick={handleClose}
              className="absolute top-2 right-2 text-white hover:bg-white/20 rounded-full w-6 h-6"
              aria-label="Đóng quảng cáo"
            >
              <X className="w-3 h-3" />
            </Button>

            {/* Countdown */}
            {countdown && countdown > 0 && (
              <Badge className="absolute top-2 left-2 bg-white/20 text-white text-xs">
                {countdown}s
              </Badge>
            )}

            <div className="flex items-center gap-3 text-white pr-8">
              <div className="flex-shrink-0 w-12 h-12 bg-white/20 rounded-full flex items-center justify-center">
                <Calendar className="w-6 h-6" />
              </div>
              
              <div className="flex-1 min-w-0">
                <h3 id="banner-title" className="text-sm mb-1 truncate">
                  {currentAd.title}
                </h3>
                <p className="text-xs opacity-90 line-clamp-2">
                  {currentAd.description}
                </p>
              </div>
            </div>

            <Button 
              onClick={handleClick}
              variant="secondary"
              size="sm"
              className="w-full mt-3 bg-white text-amber-900 hover:bg-gray-100 text-sm"
              role="button"
              aria-label={`${currentAd.ctaText} - Chuyển đến trang lịch âm`}
            >
              {currentAd.ctaText}
              <ChevronRight className="w-3 h-3 ml-1" />
            </Button>
          </div>
        </Card>
      </div>
    );
  }

  return null;
}

export default AdPopup;
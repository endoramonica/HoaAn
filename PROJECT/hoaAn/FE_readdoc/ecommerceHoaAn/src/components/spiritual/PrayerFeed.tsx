import { useState, useEffect, useRef } from 'react';
import { Button } from '../ui/button';
import { X } from 'lucide-react';
import { Prayer } from './PrayerForm';

interface PrayerFeedProps {
  prayers: Prayer[];
}

interface ActivePrayer extends Prayer {
  displayId: string;
  isVisible: boolean;
}

export function PrayerFeed({ prayers }: PrayerFeedProps) {
  const [activePrayers, setActivePrayers] = useState<ActivePrayer[]>([]);
  const [isVisible, setIsVisible] = useState(false);
  const timeoutRefs = useRef<{ [key: string]: NodeJS.Timeout }>({});

  // Category colors and icons
  const categoryConfig = {
    health: { bg: 'bg-emerald-500', text: 'text-white', icon: '💚' },
    family: { bg: 'bg-blue-500', text: 'text-white', icon: '👨‍👩‍👧‍👦' },
    career: { bg: 'bg-purple-500', text: 'text-white', icon: '💼' },
    love: { bg: 'bg-pink-500', text: 'text-white', icon: '❤️' },
    study: { bg: 'bg-indigo-500', text: 'text-white', icon: '📚' },
    peace: { bg: 'bg-amber-500', text: 'text-white', icon: '🕯️' }
  };

  // Add new prayers when they come in
  useEffect(() => {
    if (prayers.length === 0) return;

    const latestPrayer = prayers[prayers.length - 1];
    const displayId = `${latestPrayer.id}-${Date.now()}`;
    
    const activePrayer: ActivePrayer = {
      ...latestPrayer,
      displayId,
      isVisible: true
    };

    setActivePrayers(prev => [...prev, activePrayer]);
    setIsVisible(true);

    // Auto-remove after 5 seconds
    const timeout = setTimeout(() => {
      removePrayer(displayId);
    }, 5000);

    timeoutRefs.current[displayId] = timeout;

    // Cleanup timeout if component unmounts
    return () => {
      if (timeoutRefs.current[displayId]) {
        clearTimeout(timeoutRefs.current[displayId]);
        delete timeoutRefs.current[displayId];
      }
    };
  }, [prayers]);

  const removePrayer = (displayId: string) => {
    setActivePrayers(prev => 
      prev.map(prayer => 
        prayer.displayId === displayId 
          ? { ...prayer, isVisible: false }
          : prayer
      )
    );

    // Clean up from state after fade out animation
    setTimeout(() => {
      setActivePrayers(prev => prev.filter(prayer => prayer.displayId !== displayId));
      
      // Clear timeout reference
      if (timeoutRefs.current[displayId]) {
        clearTimeout(timeoutRefs.current[displayId]);
        delete timeoutRefs.current[displayId];
      }
    }, 500);
  };

  const endAllPrayers = () => {
    // Clear all timeouts
    Object.values(timeoutRefs.current).forEach(timeout => clearTimeout(timeout));
    timeoutRefs.current = {};

    // Fade out all prayers
    setActivePrayers(prev => prev.map(prayer => ({ ...prayer, isVisible: false })));

    // Clean up after animation
    setTimeout(() => {
      setActivePrayers([]);
      setIsVisible(false);
    }, 500);
  };

  if (!isVisible || activePrayers.length === 0) {
    return null;
  }

  return (
    <>
      {/* Background overlay */}
      <div className="fixed inset-0 bg-black/10 backdrop-blur-[1px] pointer-events-none z-40" />
      
      {/* Prayer marquee overlay */}
      <div className="fixed inset-0 pointer-events-none z-50 overflow-hidden">
        {activePrayers.map((prayer, index) => {
          const config = categoryConfig[prayer.category as keyof typeof categoryConfig] || categoryConfig.peace;
          
          return (
            <div
              key={prayer.displayId}
              className={`absolute transition-all duration-500 ease-in-out ${
                prayer.isVisible ? 'opacity-100 translate-x-0' : 'opacity-0 translate-x-full'
              }`}
              style={{
                top: `${20 + index * 100}px`, // Stack prayers vertically
                left: '0',
                animationDuration: '15s',
                animationTimingFunction: 'linear',
                animationIterationCount: 'infinite'
              }}
            >
              {/* Prayer bubble */}
              <div 
                className={`
                  ${config.bg} ${config.text} 
                  px-6 py-4 rounded-full shadow-lg
                  max-w-md min-w-[300px]
                  border-2 border-white/30
                  backdrop-blur-sm
                  animate-marquee
                `}
                style={{
                  animation: 'marquee 15s linear infinite'
                }}
              >
                <div className="flex items-center gap-3">
                  <span className="text-2xl">{config.icon}</span>
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-1">
                      <span className="text-sm opacity-90">{prayer.author}</span>
                      <span className="text-xs opacity-75">•</span>
                      <span className="text-xs opacity-75">
                        {prayer.timestamp.toLocaleTimeString('vi-VN', { 
                          hour: '2-digit', 
                          minute: '2-digit' 
                        })}
                      </span>
                    </div>
                    <div className="text-sm leading-relaxed">
                      {prayer.text.length > 100 
                        ? `${prayer.text.substring(0, 100)}...` 
                        : prayer.text}
                    </div>
                  </div>
                </div>
              </div>

              {/* Sparkle effects */}
              <div 
                className="absolute -top-1 -left-1 w-3 h-3 bg-white/80 rounded-full animate-ping"
                style={{ animationDelay: '0s', animationDuration: '2s' }}
              />
              <div 
                className="absolute -bottom-1 -right-1 w-2 h-2 bg-yellow-300/80 rounded-full animate-pulse"
                style={{ animationDelay: '1s' }}
              />
            </div>
          );
        })}
      </div>

      {/* Control panel */}
      <div className="fixed top-4 right-4 z-50 pointer-events-auto">
        <div className="bg-white/90 backdrop-blur-sm rounded-lg shadow-lg border border-white/20 p-3">
          <div className="flex items-center gap-3 mb-2">
            <div className="flex items-center gap-2 text-sm text-gray-700">
              <div className="w-2 h-2 bg-red-500 rounded-full animate-pulse" />
              <span>{activePrayers.length} lời cầu nguyện</span>
            </div>
          </div>
          
          <Button
            onClick={endAllPrayers}
            size="sm"
            variant="outline"
            className="w-full text-red-600 border-red-300 hover:bg-red-50"
          >
            <X className="w-4 h-4 mr-1" />
            Kết thúc
          </Button>
        </div>
      </div>

      {/* CSS Animation */}
      <style jsx>{`
        @keyframes marquee {
          0% { 
            transform: translateX(-100vw); 
          }
          100% { 
            transform: translateX(100vw); 
          }
        }
        
        .animate-marquee {
          animation: marquee 15s linear infinite;
        }
      `}</style>
    </>
  );
}
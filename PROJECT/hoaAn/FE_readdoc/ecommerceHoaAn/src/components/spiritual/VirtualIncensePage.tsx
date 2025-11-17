import { useState, useEffect } from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { Textarea } from '../ui/textarea';
import { Flame, Heart, Star, Users, Clock } from 'lucide-react';

interface VirtualIncensePageProps {
  onBack: () => void;
}

export function VirtualIncensePage({ onBack }: VirtualIncensePageProps) {
  const [incenseSticks, setIncenseSticks] = useState<Array<{id: number, burning: boolean, startTime: number}>>([]);
  const [prayer, setPrayer] = useState('');
  const [prayerCategory, setPrayerCategory] = useState('health');
  const [isLighting, setIsLighting] = useState(false);
  const [totalIncense, setTotalIncense] = useState(3247);

  const categories = [
    { id: 'health', name: 'Sức khỏe', icon: '💚', color: 'emerald' },
    { id: 'family', name: 'Gia đình', icon: '👨‍👩‍👧‍👦', color: 'blue' },
    { id: 'career', name: 'Sự nghiệp', icon: '💼', color: 'purple' },
    { id: 'love', name: 'Tình yêu', icon: '❤️', color: 'pink' },
    { id: 'study', name: 'Học tập', icon: '📚', color: 'indigo' },
    { id: 'peace', name: 'Bình an', icon: '🕯️', color: 'amber' }
  ];

  const defaultPrayers = {
    health: "Con xin Phật, Bồ Tát ban cho gia đình con sức khỏe dồi dào, không bệnh tật.",
    family: "Con cầu nguyện cho gia đình con luôn sum vầy, hạnh phúc, yêu thương nhau.",
    career: "Con mong công việc thuận lợi, gặp nhiều may mắn trong sự nghiệp.",
    love: "Con cầu nguyện cho tình yêu của con được viên mãn, bền vững.",
    study: "Con xin Phật phù hộ cho con học hành tấn tới, đỗ đạt như ý.",
    peace: "Con cầu nguyện cho thế giới hòa bình, không chiến tranh, khổ đau."
  };

  const lightIncense = () => {
    if (isLighting) return;
    
    setIsLighting(true);
    const newStick = {
      id: Date.now(),
      burning: true,
      startTime: Date.now()
    };
    
    setIncenseSticks(prev => [...prev, newStick]);
    setTotalIncense(prev => prev + 1);
    
    // Auto-select prayer if none entered
    if (!prayer.trim()) {
      setPrayer(defaultPrayers[prayerCategory as keyof typeof defaultPrayers]);
    }
    
    setTimeout(() => setIsLighting(false), 2000);
    
    // Remove stick after 30 seconds (simulating burn time)
    setTimeout(() => {
      setIncenseSticks(prev => prev.filter(stick => stick.id !== newStick.id));
    }, 30000);
  };

  const IncenseStick = ({ stick }: { stick: { id: number, burning: boolean, startTime: number } }) => {
    const [height, setHeight] = useState(100);
    
    useEffect(() => {
      if (!stick.burning) return;
      
      const interval = setInterval(() => {
        const elapsed = Date.now() - stick.startTime;
        const progress = Math.min(elapsed / 30000, 1); // 30 seconds burn time
        setHeight(100 - (progress * 80)); // Burn down to 20% height
      }, 100);
      
      return () => clearInterval(interval);
    }, [stick.burning, stick.startTime]);
    
    return (
      <div className="relative flex flex-col items-center">
        {/* Smoke animation */}
        {stick.burning && (
          <div className="absolute -top-6 w-1">
            {[1, 2, 3].map((i) => (
              <div
                key={i}
                className="absolute w-2 h-2 bg-gray-300 rounded-full opacity-50 animate-pulse"
                style={{
                  left: `${Math.sin(Date.now() * 0.001 + i) * 10}px`,
                  top: `${-i * 8}px`,
                  animationDelay: `${i * 0.5}s`,
                  animationDuration: '2s'
                }}
              />
            ))}
          </div>
        )}
        
        {/* Glowing tip */}
        {stick.burning && (
          <div className="absolute top-0 w-3 h-3 bg-orange-500 rounded-full animate-pulse shadow-lg shadow-orange-500/50" />
        )}
        
        {/* Incense stick */}
        <div 
          className="w-1 bg-gradient-to-b from-amber-800 to-amber-900 rounded-full transition-all duration-100"
          style={{ height: `${height}px` }}
        />
        
        {/* Base */}
        <div className="w-6 h-2 bg-amber-900 rounded-full mt-1" />
      </div>
    );
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-red-50 via-orange-50 to-amber-50">
      {/* Header */}
      <div className="bg-gradient-to-r from-red-700 to-orange-700 text-white">
        <div className="max-w-md mx-auto px-4 py-6">
          <div className="flex items-center justify-between mb-4">
            <button
              onClick={onBack}
              className="text-red-100 hover:text-white"
            >
              ← Quay lại
            </button>
            <div className="flex items-center gap-2">
              <Flame className="w-6 h-6 text-orange-200" />
              <h1 className="text-lg">Thắp Hương Trực Tuyến</h1>
            </div>
            <div className="w-6"></div>
          </div>
          
          <div className="text-center">
            <p className="text-red-100 text-sm mb-1">Dâng hương cầu nguyện</p>
            <div className="flex items-center justify-center gap-2">
              <Users className="w-4 h-4 text-red-200" />
              <span className="text-sm">{totalIncense.toLocaleString()} người đã thắp hương hôm nay</span>
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-md mx-auto px-4 py-6 space-y-6">
        {/* Virtual Altar */}
        <Card className="p-6 bg-gradient-to-b from-amber-100 to-amber-50 border-amber-300">
          <div className="text-center mb-6">
            <div className="text-6xl mb-2">🙏</div>
            <h2 className="text-amber-900 mb-1">Bàn thờ trực tuyến</h2>
            <p className="text-amber-700 text-sm">Nơi dâng hương cầu nguyện</p>
          </div>
          
          {/* Incense holder */}
          <div className="flex justify-center items-end gap-4 mb-6 h-32">
            <div className="w-24 h-6 bg-gradient-to-r from-amber-600 to-amber-700 rounded-lg shadow-lg">
              <div className="w-full h-full bg-gradient-to-r from-amber-500 to-amber-600 rounded-lg transform -translate-y-1" />
            </div>
          </div>
          
          {/* Active incense sticks */}
          <div className="flex justify-center items-end gap-3 mb-6 min-h-[40px]">
            {incenseSticks.map((stick) => (
              <IncenseStick key={stick.id} stick={stick} />
            ))}
          </div>
          
          {/* Light incense button */}
          <Button
            onClick={lightIncense}
            disabled={isLighting}
            className={`w-full py-3 ${
              isLighting 
                ? 'bg-orange-300 cursor-not-allowed' 
                : 'bg-gradient-to-r from-red-600 to-orange-600 hover:from-red-700 hover:to-orange-700'
            } text-white shadow-lg`}
          >
            {isLighting ? (
              <div className="flex items-center gap-2">
                <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                <span>Đang thắp hương...</span>
              </div>
            ) : (
              <div className="flex items-center gap-2">
                <Flame className="w-5 h-5" />
                <span>Thắp hương cầu nguyện</span>
              </div>
            )}
          </Button>
        </Card>

        {/* Prayer Category */}
        <Card className="p-4">
          <h3 className="text-gray-800 mb-4">Chọn loại cầu nguyện</h3>
          <div className="grid grid-cols-3 gap-3">
            {categories.map((category) => (
              <button
                key={category.id}
                onClick={() => setPrayerCategory(category.id)}
                className={`p-3 rounded-lg border-2 transition-all ${
                  prayerCategory === category.id
                    ? `border-${category.color}-300 bg-${category.color}-50`
                    : 'border-gray-200 bg-white hover:border-gray-300'
                }`}
              >
                <div className="text-2xl mb-1">{category.icon}</div>
                <div className={`text-xs ${
                  prayerCategory === category.id 
                    ? `text-${category.color}-700` 
                    : 'text-gray-600'
                }`}>
                  {category.name}
                </div>
              </button>
            ))}
          </div>
        </Card>

        {/* Prayer Text */}
        <Card className="p-4">
          <h3 className="text-gray-800 mb-4">Lời cầu nguyện</h3>
          <Textarea
            placeholder="Nhập lời cầu nguyện của bạn..."
            value={prayer}
            onChange={(e) => setPrayer(e.target.value)}
            rows={4}
            className="resize-none"
          />
          <div className="flex justify-between items-center mt-2">
            <button
              onClick={() => setPrayer(defaultPrayers[prayerCategory as keyof typeof defaultPrayers])}
              className="text-sm text-orange-600 hover:text-orange-700 underline"
            >
              Sử dụng lời cầu nguyện mẫu
            </button>
            <span className="text-xs text-gray-500">{prayer.length}/500</span>
          </div>
        </Card>

        {/* Instructions */}
        <Card className="p-4 bg-gradient-to-r from-blue-50 to-indigo-50 border-blue-200">
          <h3 className="text-blue-900 mb-3 flex items-center gap-2">
            <Star className="w-5 h-5" />
            Hướng dẫn thắp hương
          </h3>
          <div className="space-y-2 text-sm text-blue-800">
            <div className="flex items-start gap-2">
              <div className="w-1.5 h-1.5 bg-blue-600 rounded-full mt-2" />
              <p>Hãy tĩnh tâm và niệm Phật trước khi thắp hương</p>
            </div>
            <div className="flex items-start gap-2">
              <div className="w-1.5 h-1.5 bg-blue-600 rounded-full mt-2" />
              <p>Viết lời cầu nguyện chân thành từ trái tim</p>
            </div>
            <div className="flex items-start gap-2">
              <div className="w-1.5 h-1.5 bg-blue-600 rounded-full mt-2" />
              <p>Nhấn "Thắp hương" và theo dõi ngọn hương cháy</p>
            </div>
            <div className="flex items-start gap-2">
              <div className="w-1.5 h-1.5 bg-blue-600 rounded-full mt-2" />
              <p>Lời cầu nguyện sẽ được gửi đến chư Phật, Bồ Tát</p>
            </div>
          </div>
        </Card>

        {/* Statistics */}
        <Card className="p-4">
          <h3 className="text-gray-800 mb-4">Thống kê hôm nay</h3>
          <div className="grid grid-cols-2 gap-4">
            <div className="text-center p-3 bg-red-50 rounded-lg">
              <div className="text-2xl text-red-600 mb-1">{totalIncense.toLocaleString()}</div>
              <div className="text-sm text-red-700">Nén hương đã thắp</div>
            </div>
            <div className="text-center p-3 bg-orange-50 rounded-lg">
              <div className="text-2xl text-orange-600 mb-1">8,429</div>
              <div className="text-sm text-orange-700">Lời cầu nguyện</div>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}
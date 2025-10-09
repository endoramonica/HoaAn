import { useState } from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { Heart, Music, Users, MessageCircle, Flower, Flame, Calendar, Star } from 'lucide-react';
import type { PrayerForm, Prayer } from './PrayerForm';

interface SpiritualHomePageProps {
  onNavigate: (page: string) => void;
  onSpiritualNavigate: (page: string) => void;
  onSubmitPrayer?: (prayer: Prayer) => void;
  onAddToast?: (toast: { type: 'success' | 'info'; title: string; description?: string }) => void;
}

export function SpiritualHomePage({ onNavigate, onSpiritualNavigate, onSubmitPrayer, onAddToast }: SpiritualHomePageProps) {
  const [greeting] = useState(() => {
    const hour = new Date().getHours();
    if (hour < 12) return 'Chào buổi sáng';
    if (hour < 18) return 'Chào buổi chiều';
    return 'Chào buổi tối';
  });

  const dailyWisdom = {
    text: "Một giọt nước nhỏ thả vào hồ sen cũng tạo nên những gợn sóng bất tận.",
    author: "Đức Phật"
  };

  const todayActivities = [
    {
      time: '6:00',
      activity: 'Tụng kinh Pháp Hoa',
      status: 'completed'
    },
    {
      time: '12:00',
      activity: 'Thiền định giữa trưa',
      status: 'current'
    },
    {
      time: '18:00',
      activity: 'Cầu nguyện tối',
      status: 'upcoming'
    }
  ];

  return (
    <div className="min-h-screen bg-gradient-to-br from-amber-50 to-orange-50">
      {/* Header */}
      <div className="bg-gradient-to-r from-amber-800 to-orange-800 text-white">
        <div className="max-w-md mx-auto px-4 py-6">
          <div className="flex items-center justify-between mb-4">
            <button
              onClick={() => onNavigate('home')}
              className="text-amber-100 hover:text-white"
            >
              ← Quay lại
            </button>
            <div className="flex items-center gap-2">
              <Flower className="w-6 h-6 text-amber-200" />
              <h1 className="text-lg">Tâm Linh Việt</h1>
            </div>
            <div className="w-6"></div>
          </div>
          
          <div className="text-center">
            <p className="text-amber-100 text-sm mb-1">{greeting}</p>
            <p className="text-white">Chúc bạn ngày an lành</p>
          </div>
        </div>
      </div>

      <div className="max-w-md mx-auto px-4 py-6 space-y-6">
        {/* Daily Wisdom */}
        <Card className="p-4 bg-gradient-to-br from-amber-100 to-orange-100 border-amber-200">
          <div className="text-center">
            <div className="w-8 h-8 bg-amber-600 rounded-full flex items-center justify-center mx-auto mb-3">
              <Star className="w-4 h-4 text-white" />
            </div>
            <h2 className="text-amber-900 mb-3">Lời dạy hôm nay</h2>
            <blockquote className="text-amber-800 italic text-sm mb-2">
              "{dailyWisdom.text}"
            </blockquote>
            <p className="text-amber-700 text-xs">- {dailyWisdom.author}</p>
          </div>
        </Card>

        {/* Main Services */}
        <div className="grid grid-cols-2 gap-4">
          <Card 
            className="p-4 cursor-pointer hover:shadow-lg transition-all bg-gradient-to-br from-red-50 to-red-100 border-red-200 hover:scale-105"
            onClick={() => onSpiritualNavigate('virtual-incense')}
          >
            <div className="text-center">
              <div className="w-12 h-12 bg-red-600 rounded-full flex items-center justify-center mx-auto mb-3">
                <Flame className="w-6 h-6 text-white" />
              </div>
              <h3 className="text-red-900 mb-1">Thắp hương</h3>
              <p className="text-red-700 text-sm">Dâng hương trực tuyến</p>
            </div>
          </Card>

          <Card 
            className="p-4 cursor-pointer hover:shadow-lg transition-all bg-gradient-to-br from-orange-50 to-orange-100 border-orange-200 hover:scale-105"
            onClick={() => onSpiritualNavigate('audio-chanting')}
          >
            <div className="text-center">
              <div className="w-12 h-12 bg-orange-600 rounded-full flex items-center justify-center mx-auto mb-3">
                <Music className="w-6 h-6 text-white" />
              </div>
              <h3 className="text-orange-900 mb-1">Nghe kinh</h3>
              <p className="text-orange-700 text-sm">Kinh Phật, thần chú</p>
            </div>
          </Card>

          <Card 
            className="p-4 cursor-pointer hover:shadow-lg transition-all bg-gradient-to-br from-amber-50 to-amber-100 border-amber-200 hover:scale-105"
            onClick={() => onSpiritualNavigate('fengshui-consultation')}
          >
            <div className="text-center">
              <div className="w-12 h-12 bg-amber-600 rounded-full flex items-center justify-center mx-auto mb-3">
                <Users className="w-6 h-6 text-white" />
              </div>
              <h3 className="text-amber-900 mb-1">Phong thủy</h3>
              <p className="text-amber-700 text-sm">Tư vấn chuyên gia</p>
            </div>
          </Card>

{onSubmitPrayer && onAddToast ? (
            <PrayerForm onSubmitPrayer={onSubmitPrayer} onAddToast={onAddToast} />
          ) : (
            <Card className="p-4 cursor-pointer hover:shadow-lg transition-all bg-gradient-to-br from-emerald-50 to-emerald-100 border-emerald-200 hover:scale-105">
              <div className="text-center">
                <div className="w-12 h-12 bg-emerald-600 rounded-full flex items-center justify-center mx-auto mb-3">
                  <Heart className="w-6 h-6 text-white" />
                </div>
                <h3 className="text-emerald-900 mb-1">Cầu nguyện</h3>
                <p className="text-emerald-700 text-sm">Gửi lời cầu nguyện</p>
              </div>
            </Card>
          )}
        </div>

        {/* Today's Schedule */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4 flex items-center gap-2">
            <Calendar className="w-5 h-5 text-amber-600" />
            Lịch tu tập hôm nay
          </h2>
          <div className="space-y-3">
            {todayActivities.map((activity, index) => (
              <div key={index} className="flex items-center gap-3">
                <div className={`w-3 h-3 rounded-full ${
                  activity.status === 'completed' ? 'bg-green-500' :
                  activity.status === 'current' ? 'bg-orange-500 animate-pulse' :
                  'bg-gray-300'
                }`}></div>
                <div className="flex-1">
                  <div className="flex items-center gap-2">
                    <span className="text-sm text-gray-600">{activity.time}</span>
                    <span className={`text-sm ${
                      activity.status === 'current' ? 'text-orange-700' : 'text-gray-700'
                    }`}>
                      {activity.activity}
                    </span>
                  </div>
                </div>
                {activity.status === 'completed' && (
                  <div className="w-4 h-4 bg-green-100 rounded-full flex items-center justify-center">
                    <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                  </div>
                )}
              </div>
            ))}
          </div>
        </Card>

        {/* Quick Actions */}
        <Card className="p-4">
          <h2 className="text-gray-800 mb-4">Hành động nhanh</h2>
          <div className="grid grid-cols-3 gap-3">
            <button 
              onClick={() => onSpiritualNavigate('virtual-incense')}
              className="flex flex-col items-center p-3 bg-red-50 rounded-lg border border-red-200 hover:bg-red-100 transition-colors"
            >
              <Flame className="w-6 h-6 text-red-600 mb-1" />
              <span className="text-xs text-red-700">Thắp hương</span>
            </button>
            <button 
              onClick={() => onSpiritualNavigate('audio-chanting')}
              className="flex flex-col items-center p-3 bg-orange-50 rounded-lg border border-orange-200 hover:bg-orange-100 transition-colors"
            >
              <Music className="w-6 h-6 text-orange-600 mb-1" />
              <span className="text-xs text-orange-700">Nghe kinh</span>
            </button>
            <button 
              onClick={() => onNavigate('qa')}
              className="flex flex-col items-center p-3 bg-emerald-50 rounded-lg border border-emerald-200 hover:bg-emerald-100 transition-colors"
            >
              <MessageCircle className="w-6 h-6 text-emerald-600 mb-1" />
              <span className="text-xs text-emerald-700">Hỏi đáp</span>
            </button>
          </div>
        </Card>

        {/* Community */}
        <Card className="p-4 bg-gradient-to-r from-purple-50 to-pink-50 border-purple-200">
          <h2 className="text-purple-900 mb-3">Cộng đồng tu tập</h2>
          <div className="flex items-center justify-between mb-3">
            <div>
              <p className="text-purple-800 text-sm">Người đang tu tập trực tuyến</p>
              <p className="text-purple-900">1,247 người</p>
            </div>
            <div className="flex -space-x-2">
              {[1, 2, 3, 4].map((i) => (
                <div key={i} className="w-8 h-8 bg-purple-200 rounded-full border-2 border-white flex items-center justify-center">
                  <span className="text-xs text-purple-700">🙏</span>
                </div>
              ))}
            </div>
          </div>
          <Button 
            variant="outline" 
            size="sm" 
            onClick={() => onNavigate('community')}
            className="w-full border-purple-300 text-purple-700 hover:bg-purple-100"
          >
            Tham gia cộng đồng
          </Button>
        </Card>
      </div>
    </div>
  );
}

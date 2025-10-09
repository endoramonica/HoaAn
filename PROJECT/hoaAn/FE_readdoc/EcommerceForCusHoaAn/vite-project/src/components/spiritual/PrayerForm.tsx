import { useState } from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { Textarea } from '../ui/textarea';
import { Label } from '../ui/label';
import { RadioGroup, RadioGroupItem } from '../ui/radio-group';
import { Checkbox } from '../ui/checkbox';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '../ui/dialog';
import { Heart, Sparkles, Send, Flame, Star } from 'lucide-react';

interface PrayerFormProps {
  onSubmitPrayer: (prayer: Prayer) => void;
  onAddToast: (toast: { type: 'success' | 'info'; title: string; description?: string }) => void;
}

export interface Prayer {
  id: number;
  text: string;
  category: string;
  isAnonymous: boolean;
  timestamp: Date;
  author: string;
}

export function PrayerForm({ onSubmitPrayer, onAddToast }: PrayerFormProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [prayerText, setPrayerText] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('health');
  const [isAnonymous, setIsAnonymous] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const categories = [
    { 
      id: 'health', 
      name: 'Sức khỏe', 
      icon: '💚', 
      color: 'emerald',
      description: 'Cầu cho sức khỏe dồi dào, không bệnh tật',
      sample: 'Con xin Phật, Bồ Tát ban cho gia đình con sức khỏe dồi dào, xa lánh bệnh tật.'
    },
    { 
      id: 'family', 
      name: 'Gia đình', 
      icon: '👨‍👩‍👧‍👦', 
      color: 'blue',
      description: 'Cầu cho gia đình sum vầy, hạnh phúc',
      sample: 'Con cầu nguyện cho gia đình con luôn sum vầy, hạnh phúc, yêu thương nhau.'
    },
    { 
      id: 'career', 
      name: 'Sự nghiệp', 
      icon: '💼', 
      color: 'purple',
      description: 'Cầu cho công việc thuận lợi, thành công',
      sample: 'Con mong công việc thuận lợi, gặp nhiều may mắn trong sự nghiệp.'
    },
    { 
      id: 'love', 
      name: 'Tình duyên', 
      icon: '❤️', 
      color: 'pink',
      description: 'Cầu cho tình yêu viên mãn, bền vững',
      sample: 'Con cầu nguyện cho tình yêu của con được viên mãn, bền vững mãi mãi.'
    },
    { 
      id: 'study', 
      name: 'Học tập', 
      icon: '📚', 
      color: 'indigo',
      description: 'Cầu cho học hành tấn tới, đỗ đạt',
      sample: 'Con xin Phật phù hộ cho con học hành tấn tới, đỗ đạt như ý.'
    },
    { 
      id: 'peace', 
      name: 'Bình an', 
      icon: '🕯️', 
      color: 'amber',
      description: 'Cầu cho thế giới hòa bình, an lạc',
      sample: 'Con cầu nguyện cho thế giới hòa bình, không chiến tranh, khổ đau.'
    }
  ];

  const selectedCat = categories.find(cat => cat.id === selectedCategory);

  const handleSubmit = async () => {
    if (!prayerText.trim()) return;

    setIsSubmitting(true);

    // Simulate submission delay
    await new Promise(resolve => setTimeout(resolve, 1500));

    const prayer: Prayer = {
      id: Date.now(),
      text: prayerText,
      category: selectedCategory,
      isAnonymous,
      timestamp: new Date(),
      author: isAnonymous ? 'Phật tử ẩn danh' : 'Bạn'
    };

    onSubmitPrayer(prayer);
    
    onAddToast({
      type: 'success',
      title: '🙏 Lời cầu nguyện đã được gửi',
      description: 'Lời cầu nguyện sẽ hiển thị trên màn hình trong 5 giây'
    });

    // Reset form
    setPrayerText('');
    setSelectedCategory('health');
    setIsAnonymous(false);
    setIsSubmitting(false);
    setIsOpen(false);
  };

  const useSamplePrayer = () => {
    if (selectedCat) {
      setPrayerText(selectedCat.sample);
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={setIsOpen}>
      <DialogTrigger asChild>
        <Card className="p-4 cursor-pointer hover:shadow-lg transition-all bg-gradient-to-br from-emerald-50 to-emerald-100 border-emerald-200 hover:scale-105 group">
          <div className="text-center">
            <div className="w-12 h-12 bg-emerald-600 rounded-full flex items-center justify-center mx-auto mb-3 group-hover:scale-110 transition-transform">
              <Heart className="w-6 h-6 text-white" />
            </div>
            <h3 className="text-emerald-900 mb-1">Cầu nguyện</h3>
            <p className="text-emerald-700 text-sm">Gửi lời cầu nguyện</p>
          </div>
        </Card>
      </DialogTrigger>
      
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2 text-amber-900">
            <Sparkles className="w-5 h-5" />
            Gửi lời cầu nguyện
          </DialogTitle>
        </DialogHeader>

        <div className="space-y-6">
          {/* Category Selection */}
          <div>
            <Label className="text-base">Chọn loại cầu nguyện</Label>
            <p className="text-sm text-gray-600 mb-4">Mỗi loại cầu nguyện sẽ có màu sắc khác nhau khi hiển thị</p>
            <RadioGroup value={selectedCategory} onValueChange={setSelectedCategory}>
              <div className="grid grid-cols-2 gap-3">
                {categories.map((category) => (
                  <div key={category.id} className="relative">
                    <RadioGroupItem 
                      value={category.id} 
                      id={category.id} 
                      className="peer sr-only" 
                    />
                    <Label
                      htmlFor={category.id}
                      className={`flex items-center p-4 border-2 rounded-lg cursor-pointer transition-all hover:shadow-md peer-checked:border-${category.color}-300 peer-checked:bg-${category.color}-50 peer-checked:shadow-lg`}
                    >
                      <div className="flex items-center gap-3 w-full">
                        <span className="text-2xl">{category.icon}</span>
                        <div className="flex-1">
                          <div className="font-medium text-gray-800">{category.name}</div>
                          <div className="text-xs text-gray-600">{category.description}</div>
                        </div>
                      </div>
                    </Label>
                  </div>
                ))}
              </div>
            </RadioGroup>
          </div>

          {/* Sample Prayer Preview */}
          {selectedCat && (
            <Card className={`p-4 bg-gradient-to-r from-${selectedCat.color}-50 to-${selectedCat.color}-100 border-${selectedCat.color}-200`}>
              <div className="flex items-start gap-3">
                <div className={`w-10 h-10 bg-${selectedCat.color}-600 rounded-full flex items-center justify-center text-white flex-shrink-0`}>
                  <Star className="w-5 h-5" />
                </div>
                <div className="flex-1">
                  <h4 className={`text-${selectedCat.color}-900 mb-2`}>Lời cầu nguyện mẫu cho {selectedCat.name}</h4>
                  <p className={`text-${selectedCat.color}-800 text-sm italic mb-3`}>"{selectedCat.sample}"</p>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={useSamplePrayer}
                    className={`border-${selectedCat.color}-300 text-${selectedCat.color}-700 hover:bg-${selectedCat.color}-100`}
                  >
                    Sử dụng lời cầu nguyện này
                  </Button>
                </div>
              </div>
            </Card>
          )}

          {/* Prayer Text */}
          <div>
            <Label htmlFor="prayer-text" className="text-base">Lời cầu nguyện của bạn</Label>
            <p className="text-sm text-gray-600 mb-3">Hãy viết lời cầu nguyện chân thành từ trái tim</p>
            <Textarea
              id="prayer-text"
              placeholder="Nhập lời cầu nguyện của bạn..."
              value={prayerText}
              onChange={(e) => setPrayerText(e.target.value)}
              rows={6}
              className="resize-none"
            />
            <div className="flex justify-between items-center mt-2">
              <span className="text-xs text-gray-500">{prayerText.length}/500 ký tự</span>
              <span className="text-xs text-gray-500">💡 Lời cầu nguyện chân thành sẽ được lắng nghe</span>
            </div>
          </div>

          {/* Privacy Settings */}
          <div className="space-y-3">
            <Label className="text-base">Cài đặt hiển thị</Label>
            <div className="flex items-center space-x-2">
              <Checkbox 
                id="anonymous" 
                checked={isAnonymous}
                onCheckedChange={(checked) => setIsAnonymous(checked as boolean)}
              />
              <Label htmlFor="anonymous" className="text-sm">
                Gửi ẩn danh (tên sẽ hiển thị là "Phật tử ẩn danh")
              </Label>
            </div>
            <p className="text-xs text-gray-600 pl-6">
              Lời cầu nguyện của bạn sẽ được hiển thị công khai trên màn hình cho mọi người cùng xem
            </p>
          </div>

          {/* Guidelines */}
          <Card className="p-4 bg-gradient-to-r from-blue-50 to-indigo-50 border-blue-200">
            <h4 className="text-blue-900 mb-3 flex items-center gap-2">
              <Flame className="w-5 h-5" />
              Hướng dẫn cầu nguyện
            </h4>
            <div className="space-y-2 text-sm text-blue-800">
              <div className="flex items-start gap-2">
                <div className="w-1.5 h-1.5 bg-blue-600 rounded-full mt-2 flex-shrink-0" />
                <p>Tĩnh tâm và niệm Phật trước khi viết lời cầu nguyện</p>
              </div>
              <div className="flex items-start gap-2">
                <div className="w-1.5 h-1.5 bg-blue-600 rounded-full mt-2 flex-shrink-0" />
                <p>Viết lời cầu nguyện chân thành, không cầu xin để làm hại người khác</p>
              </div>
              <div className="flex items-start gap-2">
                <div className="w-1.5 h-1.5 bg-blue-600 rounded-full mt-2 flex-shrink-0" />
                <p>Lời cầu nguyện sẽ hiển thị trực tiếp trên màn hình để mọi người cùng thấy</p>
              </div>
              <div className="flex items-start gap-2">
                <div className="w-1.5 h-1.5 bg-blue-600 rounded-full mt-2 flex-shrink-0" />
                <p>Hãy cầu nguyện với tấm lòng từ bi và lòng biết ơn</p>
              </div>
            </div>
          </Card>

          {/* Submit Button */}
          <div className="flex gap-3 pt-4">
            <Button
              variant="outline"
              onClick={() => setIsOpen(false)}
              className="flex-1"
              disabled={isSubmitting}
            >
              Hủy
            </Button>
            <Button
              onClick={handleSubmit}
              disabled={!prayerText.trim() || isSubmitting}
              className="flex-1 bg-gradient-to-r from-amber-600 to-orange-600 hover:from-amber-700 hover:to-orange-700 text-white"
            >
              {isSubmitting ? (
                <div className="flex items-center gap-2">
                  <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                  <span>Đang gửi...</span>
                </div>
              ) : (
                <div className="flex items-center gap-2">
                  <Send className="w-4 h-4" />
                  <span>Gửi cầu nguyện</span>
                </div>
              )}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}

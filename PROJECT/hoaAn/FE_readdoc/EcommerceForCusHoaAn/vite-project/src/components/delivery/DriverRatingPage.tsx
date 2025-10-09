import { useState } from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { Textarea } from '../ui/textarea';
import { Star, ThumbsUp, Clock, MessageCircle, Package } from 'lucide-react';

interface DriverRatingPageProps {
  onBack: () => void;
  onComplete: () => void;
}

export function DriverRatingPage({ onBack, onComplete }: DriverRatingPageProps) {
  const [rating, setRating] = useState(0);
  const [selectedTags, setSelectedTags] = useState<string[]>([]);
  const [feedback, setFeedback] = useState('');
  const [hoveredRating, setHoveredRating] = useState(0);

  const driverInfo = {
    name: 'Nguyễn Văn An',
    orderId: 'DL003',
    vehicleType: 'Xe máy',
    licensePlate: '59-F1 12345'
  };

  const positiveTags = [
    'Giao hàng nhanh',
    'Thái độ tốt',
    'Đúng giờ',
    'Cẩn thận',
    'Liên lạc tốt',
    'Chuyên nghiệp'
  ];

  const negativeTags = [
    'Giao hàng chậm',
    'Khó liên lạc',
    'Không đúng giờ',
    'Thái độ không tốt',
    'Không cẩn thận'
  ];

  const handleTagToggle = (tag: string) => {
    setSelectedTags(prev => 
      prev.includes(tag) 
        ? prev.filter(t => t !== tag)
        : [...prev, tag]
    );
  };

  const handleSubmit = () => {
    if (rating === 0) return;
    
    // Simulate rating submission
    setTimeout(() => {
      onComplete();
    }, 1000);
  };

  const renderStars = () => {
    return Array.from({ length: 5 }, (_, index) => {
      const starValue = index + 1;
      const isFilled = starValue <= (hoveredRating || rating);
      
      return (
        <button
          key={index}
          onClick={() => setRating(starValue)}
          onMouseEnter={() => setHoveredRating(starValue)}
          onMouseLeave={() => setHoveredRating(0)}
          className={`w-10 h-10 transition-all duration-150 ${
            isFilled ? 'text-yellow-400' : 'text-gray-300'
          } hover:scale-110`}
        >
          <Star className="w-full h-full fill-current" />
        </button>
      );
    });
  };

  const getRatingText = () => {
    switch (rating) {
      case 1: return 'Rất không hài lòng';
      case 2: return 'Không hài lòng';
      case 3: return 'Bình thường';
      case 4: return 'Hài lòng';
      case 5: return 'Rất hài lòng';
      default: return 'Chọn số sao để đánh giá';
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white shadow-sm">
        <div className="max-w-md mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <button
              onClick={onBack}
              className="text-gray-600 hover:text-gray-800"
            >
              ← Quay lại
            </button>
            <h1 className="text-lg text-gray-800">Đánh giá tài xế</h1>
            <div className="w-6"></div>
          </div>
        </div>
      </div>

      <div className="max-w-md mx-auto px-4 py-6 space-y-6">
        {/* Order Info */}
        <Card className="p-4">
          <div className="flex items-center gap-3 mb-4">
            <div className="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
              <Package className="w-6 h-6 text-green-600" />
            </div>
            <div>
              <h3 className="text-gray-800">Đơn hàng #{driverInfo.orderId}</h3>
              <p className="text-sm text-gray-600">Đã hoàn thành</p>
            </div>
            <div className="ml-auto text-green-600">
              <ThumbsUp className="w-6 h-6" />
            </div>
          </div>
          
          <div className="flex items-center gap-3 p-3 bg-gray-50 rounded-lg">
            <div className="w-10 h-10 bg-gray-200 rounded-full flex items-center justify-center">
              <span className="text-gray-600 text-sm">AN</span>
            </div>
            <div>
              <h4 className="text-gray-800">{driverInfo.name}</h4>
              <p className="text-sm text-gray-600">
                {driverInfo.vehicleType} • {driverInfo.licensePlate}
              </p>
            </div>
          </div>
        </Card>

        {/* Rating */}
        <Card className="p-6 text-center">
          <h2 className="text-gray-800 mb-2">Bạn cảm thấy thế nào về chuyến giao hàng?</h2>
          <p className="text-gray-600 text-sm mb-6">Đánh giá của bạn giúp chúng tôi cải thiện dịch vụ</p>
          
          <div className="flex justify-center gap-1 mb-4">
            {renderStars()}
          </div>
          
          <p className={`text-sm ${rating > 0 ? 'text-gray-800' : 'text-gray-500'}`}>
            {getRatingText()}
          </p>
        </Card>

        {/* Tags */}
        {rating > 0 && (
          <Card className="p-4">
            <h3 className="text-gray-800 mb-4">
              {rating >= 4 ? 'Điểm tốt của tài xế:' : 'Vấn đề gặp phải:'}
            </h3>
            <div className="flex flex-wrap gap-2">
              {(rating >= 4 ? positiveTags : negativeTags).map((tag) => (
                <button
                  key={tag}
                  onClick={() => handleTagToggle(tag)}
                  className={`px-3 py-2 rounded-full text-sm border transition-colors ${
                    selectedTags.includes(tag)
                      ? rating >= 4 
                        ? 'bg-green-100 border-green-300 text-green-700'
                        : 'bg-red-100 border-red-300 text-red-700'
                      : 'bg-white border-gray-300 text-gray-600 hover:border-gray-400'
                  }`}
                >
                  {tag}
                </button>
              ))}
            </div>
          </Card>
        )}

        {/* Feedback */}
        {rating > 0 && (
          <Card className="p-4">
            <h3 className="text-gray-800 mb-4">Góp ý thêm (tùy chọn)</h3>
            <Textarea
              placeholder="Chia sẻ thêm về trải nghiệm của bạn..."
              value={feedback}
              onChange={(e) => setFeedback(e.target.value)}
              rows={4}
              className="resize-none"
            />
          </Card>
        )}

        {/* Tips Section */}
        {rating >= 4 && (
          <Card className="p-4 bg-green-50 border-green-200">
            <div className="flex items-start gap-3">
              <div className="w-8 h-8 bg-green-100 rounded-full flex items-center justify-center">
                <ThumbsUp className="w-4 h-4 text-green-600" />
              </div>
              <div>
                <h3 className="text-green-800 mb-1">Tip cho tài xế</h3>
                <p className="text-green-700 text-sm mb-3">
                  Thể hiện sự cảm ơn với dịch vụ tốt của tài xế
                </p>
                <div className="flex gap-2">
                  {[5000, 10000, 20000].map((amount) => (
                    <button
                      key={amount}
                      className="px-3 py-1 bg-white border border-green-300 rounded-lg text-sm text-green-700 hover:bg-green-100"
                    >
                      {new Intl.NumberFormat('vi-VN').format(amount)}₫
                    </button>
                  ))}
                </div>
              </div>
            </div>
          </Card>
        )}

        {/* Submit Button */}
        <div className="space-y-3">
          <Button
            onClick={handleSubmit}
            disabled={rating === 0}
            className={`w-full py-3 ${
              rating === 0 
                ? 'bg-gray-300 text-gray-500' 
                : rating >= 4
                  ? 'bg-green-600 hover:bg-green-700 text-white'
                  : 'bg-orange-500 hover:bg-orange-600 text-white'
            }`}
          >
            {rating === 0 ? 'Vui lòng chọn số sao' : 'Gửi đánh giá'}
          </Button>
          
          <Button 
            variant="outline" 
            onClick={onBack}
            className="w-full"
          >
            Bỏ qua
          </Button>
        </div>

        {/* Info */}
        <p className="text-xs text-gray-500 text-center">
          Đánh giá của bạn sẽ giúp các khách hàng khác và cải thiện chất lượng dịch vụ
        </p>
      </div>
    </div>
  );
}

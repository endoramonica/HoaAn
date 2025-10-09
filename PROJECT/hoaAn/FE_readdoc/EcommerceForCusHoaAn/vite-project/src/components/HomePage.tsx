import { useState, useEffect } from 'react';
import { Button } from './ui/button';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { 
  Flower2, 
  Star, 
  ChevronLeft, 
  ChevronRight, 
  Calendar,
  Flame,
  Sparkles,
  Gift,
  Clock,
  Shield,
  Award,
  Truck,
  MessageCircle,
  Package,
  MapPin,
  Phone,
  Heart,
  Zap,
  Brain,
  Moon,
  ArrowRight
} from 'lucide-react';

interface HomePageProps {
  onNavigate?: (page: string) => void;
}

export function HomePage({ onNavigate }: HomePageProps = {}) {
  const [currentTestimonial, setCurrentTestimonial] = useState(0);
  const [currentCeremony, setCurrentCeremony] = useState(0);

  // Auto-rotate testimonials every 5 seconds
  useEffect(() => {
    const interval = setInterval(() => {
      setCurrentTestimonial((prev) => (prev + 1) % testimonials.length);
    }, 5000);
    return () => clearInterval(interval);
  }, []);

  const categories = [
    { name: 'Hương', icon: Flame, count: '50+ sản phẩm' },
    { name: 'Nến', icon: Sparkles, count: '30+ loại' },
    { name: 'Hoa quả', icon: Gift, count: '20+ combo' },
    { name: 'Mâm cúng', icon: Award, count: '15+ bộ' },
    { name: 'Giấy tiền', icon: Calendar, count: '25+ mẫu' },
    { name: 'Dịch vụ', icon: Shield, count: 'Trọn gói' }
  ];

  const ceremonyTypes = [
    { name: 'Lễ cưới hỏi', image: 'https://images.unsplash.com/photo-1730130856640-3db880ab33b7?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxhc2lhbiUyMGNlcmVtb25pYWwlMjB3ZWRkaW5nJTIwdHJhZGl0aW9uYWx8ZW58MXx8fHwxNzU3Njc0NDMwfDA&ixlib=rb-4.1.0&q=80&w=1080' },
    { name: 'Lễ khai trương', image: 'https://images.unsplash.com/photo-1588358581442-c0a340052b75?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwdGVtcGxlJTIwcHJheWVyJTIwY2VyZW1vbnl8ZW58MXx8fHwxNzU3Njc0NDMwfDA&ixlib=rb-4.1.0&q=80&w=1080' },
    { name: 'Cúng gia tiên', image: 'https://images.unsplash.com/photo-1519097000072-e44ffa116485?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwb2ZmZXJpbmdzJTIwYWx0YXIlMjBmcnVpdHN8ZW58MXx8fHwxNzU3Njc0NDI5fDA&ixlib=rb-4.1.0&q=80&w=1080' },
    { name: 'Cúng Phật', image: 'https://images.unsplash.com/photo-1573460630303-81cbaf895c54?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsb3R1cyUyMGZsb3dlciUyMGNlcmVtb25pYWwlMjBnb2xkfGVufDF8fHx8MTc1NzY3NDQyOXww&ixlib=rb-4.1.0&q=80&w=1080' },
    { name: 'Lễ sinh nhật', image: 'https://images.unsplash.com/photo-1732117924212-39bfaec174c9?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx0cmFkaXRpb25hbCUyMGNhbmRsZXMlMjByZWQlMjBnb2xkfGVufDF8fHx8MTc1NzY3NDQyOXww&ixlib=rb-4.1.0&q=80&w=1080' },
    { name: 'Tân gia', image: 'https://images.unsplash.com/photo-1532334722716-c5850cdd878d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwaW5jZW5zZSUyMGNlcmVtb255JTIwdHJhZGl0aW9uYWx8ZW58MXx8fHwxNzU3Njc0NDI4fDA&ixlib=rb-4.1.0&q=80&w=1080' }
  ];

  const testimonials = [
    {
      name: 'Chị Nguyễn Thị Hồng',
      location: 'Hà Nội',
      rating: 5,
      comment: 'Mâm cúng rất chuẩn phong thủy, giao hàng đúng giờ. Gia đình rất hài lòng với chất lượng dịch vụ.',
      avatar: 'https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=100&h=100&fit=crop&crop=face'
    },
    {
      name: 'Anh Trần Văn Minh',
      location: 'Hồ Chí Minh',
      rating: 5,
      comment: 'Lễ khai trương được chuẩn bị chu đáo, mọi thứ đều đúng theo phong tục truyền thống.',
      avatar: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=100&h=100&fit=crop&crop=face'
    },
    {
      name: 'Bà Lê Thị Mai',
      location: 'Đà Nẵng',
      rating: 5,
      comment: 'Đội ngũ tư vấn rất chuyên nghiệp, giúp chọn được mâm cúng phù hợp với nghi lễ gia đình.',
      avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=100&h=100&fit=crop&crop=face'
    }
  ];

  const scrollCeremony = (direction: 'left' | 'right') => {
    if (direction === 'left') {
      setCurrentCeremony((prev) => (prev - 1 + ceremonyTypes.length) % ceremonyTypes.length);
    } else {
      setCurrentCeremony((prev) => (prev + 1) % ceremonyTypes.length);
    }
  };

  return (
    <div>
      {/* Hero Section */}
      <section className="relative min-h-screen flex items-center justify-center overflow-hidden">
        <div 
          className="absolute inset-0 bg-gradient-to-br from-amber-900 via-yellow-600 to-red-700"
          style={{
            backgroundImage: `linear-gradient(rgba(146, 64, 14, 0.7), rgba(245, 158, 11, 0.6)), url('https://images.unsplash.com/photo-1532334722716-c5850cdd878d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwaW5jZW5zZSUyMGNlcmVtb255JTIwdHJhZGl0aW9uYWx8ZW58MXx8fHwxNzU3Njc0NDI4fDA&ixlib=rb-4.1.0&q=80&w=1080')`
          }}
        />
        
        <div className="relative z-10 text-center text-white px-4 max-w-4xl mx-auto">
          <div className="mb-8">
            <Flower2 className="w-16 h-16 mx-auto mb-4 text-yellow-300" />
          </div>
          
          <h1 className="text-4xl md:text-6xl mb-6 drop-shadow-lg">
            Dịch vụ đồ cúng & nghi lễ trực tuyến
          </h1>
          
          <p className="text-xl md:text-2xl mb-8 text-yellow-100">
            Nhanh chóng – Chuẩn lễ – Đúng ngày giờ tốt
          </p>
          
          <div className="flex flex-col sm:flex-row gap-4 justify-center mb-12">
            <Button size="lg" className="bg-red-600 hover:bg-red-700 text-white px-8 py-3 text-lg">
              Đặt hàng ngay
            </Button>
            <Button size="lg" variant="outline" className="border-white text-white hover:bg-white hover:text-amber-900 px-8 py-3 text-lg">
              Xem mâm cúng
            </Button>
          </div>
          
          {/* Technology badges */}
          <div className="flex flex-wrap justify-center gap-2 opacity-80">
            <Badge variant="secondary" className="bg-white/20 text-white border-white/30">
              Firebase Auth
            </Badge>
            <Badge variant="secondary" className="bg-white/20 text-white border-white/30">
              MoMo/VNPay
            </Badge>
            <Badge variant="secondary" className="bg-white/20 text-white border-white/30">
              Lunar API
            </Badge>
            <Badge variant="secondary" className="bg-white/20 text-white border-white/30">
              WebSocket
            </Badge>
          </div>
        </div>
      </section>

      {/* Quick Services Section */}
      <section className="py-16 bg-gradient-to-br from-cream-50 to-yellow-50">
        <div className="max-w-6xl mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl md:text-4xl text-amber-900 mb-4">
              Dịch vụ nhanh chóng
            </h2>
            <p className="text-gray-600 max-w-2xl mx-auto">
              Khám phá hai dịch vụ mới của chúng tôi - giao hàng siêu tốc và tâm linh trực tuyến
            </p>
          </div>

          <div className="grid md:grid-cols-2 gap-8">
            {/* Delivery Service Card */}
            <Card className="group cursor-pointer hover:shadow-2xl transition-all duration-300 hover:-translate-y-2 border-2 hover:border-green-400 overflow-hidden">
              <div className="bg-gradient-to-br from-green-500 to-green-600 p-6 text-white relative overflow-hidden">
                <div className="absolute top-0 right-0 w-32 h-32 bg-orange-400 rounded-full -translate-y-16 translate-x-16 opacity-20"></div>
                <div className="relative z-10">
                  <div className="flex items-center gap-3 mb-4">
                    <div className="w-12 h-12 bg-white/20 rounded-full flex items-center justify-center">
                      <Truck className="w-6 h-6" />
                    </div>
                    <h3 className="text-2xl">Giao hàng Express</h3>
                  </div>
                  <p className="text-green-100 mb-6">
                    Giao đồ cúng tận nơi trong vòng 2 giờ với đội xe chuyên nghiệp
                  </p>
                </div>
              </div>
              
              <CardContent className="p-6 bg-gradient-to-br from-green-50 to-emerald-50">
                <div className="grid grid-cols-2 gap-4 mb-6">
                  <div className="flex items-center gap-2">
                    <Package className="w-5 h-5 text-green-600" />
                    <span className="text-sm text-gray-700">Đóng gói cẩn thận</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Clock className="w-5 h-5 text-orange-600" />
                    <span className="text-sm text-gray-700">Giao trong 2h</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <MapPin className="w-5 h-5 text-blue-600" />
                    <span className="text-sm text-gray-700">Theo dõi realtime</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Phone className="w-5 h-5 text-purple-600" />
                    <span className="text-sm text-gray-700">Hỗ trợ 24/7</span>
                  </div>
                </div>
                
                <Button 
                  className="w-full bg-gradient-to-r from-green-600 to-green-700 hover:from-green-700 hover:to-green-800 text-white py-3 text-lg group-hover:shadow-lg transition-all duration-300"
                  onClick={() => onNavigate?.('delivery')}
                >
                  Đặt giao hàng ngay
                  <ArrowRight className="w-4 h-4 ml-2 group-hover:translate-x-1 transition-transform" />
                </Button>
              </CardContent>
            </Card>

            {/* Spiritual Service Card */}
            <Card className="group cursor-pointer hover:shadow-2xl transition-all duration-300 hover:-translate-y-2 border-2 hover:border-purple-400 overflow-hidden">
              <div className="bg-gradient-to-br from-purple-600 to-indigo-700 p-6 text-white relative overflow-hidden">
                <div className="absolute top-0 right-0 w-32 h-32 bg-yellow-400 rounded-full -translate-y-16 translate-x-16 opacity-20"></div>
                <div className="relative z-10">
                  <div className="flex items-center gap-3 mb-4">
                    <div className="w-12 h-12 bg-white/20 rounded-full flex items-center justify-center">
                      <Heart className="w-6 h-6" />
                    </div>
                    <h3 className="text-2xl">Dịch vụ Tâm linh</h3>
                  </div>
                  <p className="text-purple-100 mb-6">
                    Cúng online, tư vấn phong thủy và âm nhạc thiền định trực tuyến
                  </p>
                </div>
              </div>
              
              <CardContent className="p-6 bg-gradient-to-br from-purple-50 to-indigo-50">
                <div className="grid grid-cols-2 gap-4 mb-6">
                  <div className="flex items-center gap-2">
                    <Flame className="w-5 h-5 text-red-600" />
                    <span className="text-sm text-gray-700">Cúng virtual</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Brain className="w-5 h-5 text-blue-600" />
                    <span className="text-sm text-gray-700">Tư vấn AI</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Moon className="w-5 h-5 text-indigo-600" />
                    <span className="text-sm text-gray-700">Âm nhạc thiền</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Zap className="w-5 h-5 text-yellow-600" />
                    <span className="text-sm text-gray-700">Phong thủy online</span>
                  </div>
                </div>
                
                <Button 
                  className="w-full bg-gradient-to-r from-purple-600 to-indigo-700 hover:from-purple-700 hover:to-indigo-800 text-white py-3 text-lg group-hover:shadow-lg transition-all duration-300"
                  onClick={() => onNavigate?.('spiritual')}
                >
                  Khám phá tâm linh
                  <ArrowRight className="w-4 h-4 ml-2 group-hover:translate-x-1 transition-transform" />
                </Button>
              </CardContent>
            </Card>
          </div>

          {/* Service Highlights */}
          <div className="mt-12 text-center">
            <div className="inline-flex items-center gap-6 bg-white/80 backdrop-blur rounded-full px-8 py-4 shadow-lg">
              <div className="flex items-center gap-2">
                <div className="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
                <span className="text-sm text-gray-700">Giao hàng 24/7</span>
              </div>
              <div className="w-px h-6 bg-gray-300"></div>
              <div className="flex items-center gap-2">
                <div className="w-2 h-2 bg-purple-500 rounded-full animate-pulse"></div>
                <span className="text-sm text-gray-700">Tâm linh online</span>
              </div>
              <div className="w-px h-6 bg-gray-300"></div>
              <div className="flex items-center gap-2">
                <div className="w-2 h-2 bg-blue-500 rounded-full animate-pulse"></div>
                <span className="text-sm text-gray-700">AI hỗ trợ</span>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Promoted Product Section */}
      <section className="py-16 bg-gradient-to-r from-yellow-50 to-red-50">
        <div className="max-w-6xl mx-auto px-4">
          <Card className="overflow-hidden border-2 border-yellow-400 shadow-2xl">
            <div className="grid md:grid-cols-2 gap-0">
              <div className="relative">
                <ImageWithFallback
                  src="https://images.unsplash.com/photo-1519097000072-e44ffa116485?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwb2ZmZXJpbmdzJTIwYWx0YXIlMjBmcnVpdHN8ZW58MXx8fHwxNzU3Njc0NDI5fDA&ixlib=rb-4.1.0&q=80&w=1080"
                  alt="Mâm Cúng Trọn Gói Cao Cấp"
                  className="w-full h-64 md:h-full object-cover"
                />
                <Badge className="absolute top-4 left-4 bg-red-600 text-white">
                  Sản phẩm nổi bật
                </Badge>
              </div>
              
              <CardContent className="p-8 bg-gradient-to-br from-amber-50 to-yellow-100">
                <CardHeader className="p-0 mb-6">
                  <CardTitle className="text-3xl text-amber-900 mb-4">
                    Mâm Cúng Trọn Gói Cao Cấp
                  </CardTitle>
                  <div className="flex items-center gap-4 mb-4">
                    <div className="flex items-center">
                      {[...Array(5)].map((_, i) => (
                        <Star key={i} className="w-5 h-5 fill-yellow-400 text-yellow-400" />
                      ))}
                      <span className="ml-2 text-gray-600">(127 đánh giá)</span>
                    </div>
                  </div>
                  
                  <div className="flex items-center gap-4 mb-6">
                    <span className="text-3xl text-red-600">2.890.000₫</span>
                    <span className="text-lg text-gray-500 line-through">3.490.000₫</span>
                    <Badge className="bg-red-100 text-red-700">Giảm 17%</Badge>
                  </div>
                </CardHeader>
                
                <p className="text-gray-700 mb-6">
                  Bộ mâm cúng hoàn chỉnh với đầy đủ ngũ quả, hương nến, giấy tiền theo chuẩn phong thủy. 
                  Phù hợp cho các lễ cúng quan trọng trong năm.
                </p>
                
                <div className="grid grid-cols-2 gap-4 mb-6">
                  <div className="flex items-center gap-2">
                    <Clock className="w-5 h-5 text-green-600" />
                    <span className="text-sm text-gray-700">Giao trong 2h</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Shield className="w-5 h-5 text-blue-600" />
                    <span className="text-sm text-gray-700">Đảm bảo chất lượng</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Award className="w-5 h-5 text-yellow-600" />
                    <span className="text-sm text-gray-700">Chuẩn nghi lễ</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <Calendar className="w-5 h-5 text-purple-600" />
                    <span className="text-sm text-gray-700">Tư vấn ngày tốt</span>
                  </div>
                </div>
                
                <Button className="w-full bg-red-600 hover:bg-red-700 text-white py-3 text-lg">
                  Mua ngay - Giao tận nơi
                </Button>
              </CardContent>
            </div>
          </Card>
        </div>
      </section>

      {/* Categories Section */}
      <section className="py-16 bg-white">
        <div className="max-w-6xl mx-auto px-4">
          <div className="text-center mb-12">
            <Flower2 className="w-12 h-12 mx-auto mb-4 text-yellow-600" />
            <h2 className="text-3xl md:text-4xl text-amber-900 mb-4">
              Danh mục sản phẩm
            </h2>
            <p className="text-gray-600 max-w-2xl mx-auto">
              Khám phá đầy đủ các loại đồ cúng và dịch vụ nghi lễ chất lượng cao
            </p>
          </div>
          
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-6">
            {categories.map((category, index) => (
              <Card key={index} className="group cursor-pointer hover:shadow-lg transition-all duration-300 hover:-translate-y-2 border-2 hover:border-yellow-400">
                <CardContent className="p-6 text-center">
                  <div className="w-16 h-16 mx-auto mb-4 bg-gradient-to-br from-yellow-400 to-red-500 rounded-full flex items-center justify-center group-hover:scale-110 transition-transform">
                    <category.icon className="w-8 h-8 text-white" />
                  </div>
                  <h3 className="text-lg text-amber-900 mb-2">{category.name}</h3>
                  <p className="text-sm text-gray-600">{category.count}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* Ceremony Types Carousel */}
      <section className="py-16 bg-gradient-to-br from-red-50 to-yellow-50">
        <div className="max-w-6xl mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl md:text-4xl text-amber-900 mb-4">
              Các loại nghi lễ
            </h2>
            <p className="text-gray-600 max-w-2xl mx-auto">
              Chúng tôi phục vụ đầy đủ các loại lễ cúng theo truyền thống Việt Nam
            </p>
          </div>
          
          <div className="relative">
            <div className="flex items-center justify-between mb-6">
              <Button
                variant="outline"
                size="icon"
                onClick={() => scrollCeremony('left')}
                className="border-amber-300 text-amber-700 hover:bg-amber-100"
              >
                <ChevronLeft className="w-4 h-4" />
              </Button>
              
              <Button
                variant="outline"
                size="icon"
                onClick={() => scrollCeremony('right')}
                className="border-amber-300 text-amber-700 hover:bg-amber-100"
              >
                <ChevronRight className="w-4 h-4" />
              </Button>
            </div>
            
            <div className="overflow-hidden">
              <div 
                className="flex transition-transform duration-500 ease-in-out"
                style={{ transform: `translateX(-${currentCeremony * (100 / 3)}%)` }}
              >
                {ceremonyTypes.map((ceremony, index) => (
                  <div key={index} className="w-1/3 flex-shrink-0 px-2">
                    <Card className="overflow-hidden group cursor-pointer hover:shadow-xl transition-all duration-300">
                      <div className="relative">
                        <ImageWithFallback
                          src={ceremony.image}
                          alt={ceremony.name}
                          className="w-full h-48 object-cover group-hover:scale-105 transition-transform duration-300"
                        />
                        <div className="absolute inset-0 bg-gradient-to-t from-black/50 to-transparent" />
                        <h3 className="absolute bottom-4 left-4 text-white text-xl">
                          {ceremony.name}
                        </h3>
                      </div>
                    </Card>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Customer Testimonials */}
      <section className="py-16 bg-amber-900 text-white">
        <div className="max-w-4xl mx-auto px-4">
          <div className="text-center mb-12">
            <Flower2 className="w-12 h-12 mx-auto mb-4 text-yellow-300" />
            <h2 className="text-3xl md:text-4xl mb-4">
              Khách hàng nói gì về chúng tôi
            </h2>
            <p className="text-yellow-100 max-w-2xl mx-auto">
              Hàng nghìn gia đình đã tin tương và sử dụng dịch vụ của chúng tôi
            </p>
          </div>
          
          <Card className="bg-white/10 backdrop-blur border-white/20">
            <CardContent className="p-8 text-center">
              <div className="flex justify-center mb-4">
                {[...Array(testimonials[currentTestimonial].rating)].map((_, i) => (
                  <Star key={i} className="w-6 h-6 fill-yellow-400 text-yellow-400" />
                ))}
              </div>
              
              <blockquote className="text-xl mb-6 italic">
                "{testimonials[currentTestimonial].comment}"
              </blockquote>
              
              <div className="flex items-center justify-center gap-4">
                <ImageWithFallback
                  src={testimonials[currentTestimonial].avatar}
                  alt={testimonials[currentTestimonial].name}
                  className="w-12 h-12 rounded-full object-cover"
                />
                <div className="text-left">
                  <p className="text-yellow-300">{testimonials[currentTestimonial].name}</p>
                  <p className="text-yellow-100 text-sm">{testimonials[currentTestimonial].location}</p>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <div className="flex justify-center mt-6 gap-2">
            {testimonials.map((_, index) => (
              <button
                key={index}
                onClick={() => setCurrentTestimonial(index)}
                className={`w-3 h-3 rounded-full transition-colors ${
                  index === currentTestimonial ? 'bg-yellow-400' : 'bg-white/30'
                }`}
              />
            ))}
          </div>
        </div>
      </section>

      {/* Lunar Calendar Widget */}
      <section className="py-16 bg-gradient-to-r from-yellow-50 to-red-50">
        <div className="max-w-4xl mx-auto px-4">
          <div className="grid md:grid-cols-2 gap-8">
            <Card className="border-2 border-yellow-400 shadow-lg">
              <CardHeader className="bg-gradient-to-r from-amber-600 to-yellow-600 text-white">
                <CardTitle className="flex items-center gap-2">
                  <Calendar className="w-6 h-6" />
                  Lịch âm hôm nay
                </CardTitle>
              </CardHeader>
              <CardContent className="p-6">
                <div className="text-center mb-4">
                  <div className="text-3xl text-amber-900 mb-2">22/07</div>
                  <div className="text-lg text-gray-600">Năm Ất Tỵ</div>
                  <div className="text-sm text-gray-500">Thứ Sáu, 12/09/2025</div>
                </div>
                
                <div className="space-y-3">
                  <div className="flex items-center gap-2">
                    <div className="w-3 h-3 bg-green-500 rounded-full"></div>
                    <span className="text-sm">Ngày tốt: Cúng gia tiên</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <div className="w-3 h-3 bg-red-500 rounded-full"></div>
                    <span className="text-sm">Kiêng: Khởi công xây dựng</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <div className="w-3 h-3 bg-blue-500 rounded-full"></div>
                    <span className="text-sm">Giờ tốt: 7-9h, 13-15h</span>
                  </div>
                </div>
              </CardContent>
            </Card>
            
            <Card className="border-2 border-red-400 shadow-lg">
              <CardHeader className="bg-gradient-to-r from-red-600 to-pink-600 text-white">
                <CardTitle className="flex items-center gap-2">
                  <Sparkles className="w-6 h-6" />
                  Gợi ý nghi lễ
                </CardTitle>
              </CardHeader>
              <CardContent className="p-6">
                <div className="space-y-4">
                  <div className="p-3 bg-yellow-50 rounded-lg border-l-4 border-yellow-400">
                    <h4 className="text-amber-900 mb-1">Rằm tháng 7</h4>
                    <p className="text-sm text-gray-600">Lễ Vu Lan báo hiếu - 30/08/2025</p>
                  </div>
                  <div className="p-3 bg-red-50 rounded-lg border-l-4 border-red-400">
                    <h4 className="text-red-900 mb-1">Tết Trung Thu</h4>
                    <p className="text-sm text-gray-600">Lễ cúng trăng - 17/09/2025</p>
                  </div>
                  <div className="p-3 bg-green-50 rounded-lg border-l-4 border-green-400">
                    <h4 className="text-green-900 mb-1">Ngày tốt khai trương</h4>
                    <p className="text-sm text-gray-600">25/09/2025 (7-9h sáng)</p>
                  </div>
                </div>
                
                <Button 
                  className="w-full mt-4 bg-gradient-to-r from-amber-600 to-yellow-600 hover:from-amber-700 hover:to-yellow-700 text-white rounded-lg shadow-lg transition-all duration-300 hover:shadow-xl transform hover:scale-105"
                  onClick={() => onNavigate?.('calendar')}
                  aria-label="Xem lịch âm đầy đủ với các ngày tốt và nghi lễ"
                >
                  <Calendar className="w-4 h-4 mr-2" />
                  Xem lịch đầy đủ
                </Button>
              </CardContent>
            </Card>
          </div>
        </div>
      </section>

      {/* Floating Quick Access */}
      <div className="fixed bottom-6 right-6 z-50 flex flex-col gap-3">
        <Button
          size="lg"
          className="bg-green-600 hover:bg-green-700 text-white rounded-full shadow-2xl hover:shadow-green-500/25 transition-all duration-300 px-6 py-3"
          onClick={() => onNavigate?.('delivery')}
        >
          <Truck className="w-5 h-5 mr-2" />
          Giao hàng
        </Button>
        
        <Button
          size="lg"
          className="bg-purple-600 hover:bg-purple-700 text-white rounded-full shadow-2xl hover:shadow-purple-500/25 transition-all duration-300 px-6 py-3"
          onClick={() => onNavigate?.('spiritual')}
        >
          <Heart className="w-5 h-5 mr-2" />
          Tâm linh
        </Button>
      </div>
    </div>
  );
}

import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { 
  Flower2, 
  Clock, 
  Shield, 
  Award,
  CheckCircle,
  Phone,
  Calendar,
  Users,
  Sparkles,
  Heart,
  Star
} from 'lucide-react';

export function ServicesPage() {
  const services = [
    {
      id: 1,
      title: 'Dịch vụ Cúng Gia Tiên',
      description: 'Chuẩn bị đầy đủ mâm cúng theo truyền thống với ngũ quả, hương nến, giấy tiền. Tư vấn ngày giờ tốt và cách bày trí chuẩn phong thủy.',
      price: 'Từ 1.250.000₫',
      duration: '2-3 giờ',
      image: 'https://images.unsplash.com/photo-1519097000072-e44ffa116485?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwb2ZmZXJpbmdzJTIwYWx0YXIlMjBmcnVpdHN8ZW58MXx8fHwxNzU3Njc0NDI5fDA&ixlib=rb-4.1.0&q=80&w=1080',
      features: ['Tư vấn ngày giờ tốt', 'Chuẩn bị mâm cúng hoàn chỉnh', 'Hướng dẫn nghi lễ', 'Giao hàng tận nơi'],
      popular: true
    },
    {
      id: 2,
      title: 'Dịch vụ Lễ Khai Trương',
      description: 'Trọn gói lễ khai trương với mâm cúng, hoa tươi, băng khai trương. Tư vấn phong thủy và lựa chọn ngày tốt.',
      price: 'Từ 2.890.000₫',
      duration: '3-4 giờ',
      image: 'https://images.unsplash.com/photo-1588358581442-c0a340052b75?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHh2aWV0bmFtZXNlJTIwdGVtcGxlJTIwcHJheWVyJTIwY2VyZW1vbnl8ZW58MXx8fHwxNzU3Njc0NDMwfDA&ixlib=rb-4.1.0&q=80&w=1080',
      features: ['Tư vấn phong thủy', 'Mâm cúng khai trương', 'Hoa tươi trang trí', 'Phục vụ tại chỗ']
    },
    {
      id: 3,
      title: 'Dịch vụ Lễ Cưới Hỏi',
      description: 'Chuẩn bị lễ vật cưới hỏi theo truyền thống, từ mâm quả đến các món lễ cần thiết. Tư vấn nghi thức đầy đủ.',
      price: 'Từ 3.500.000₫',
      duration: '4-5 giờ',
      image: 'https://images.unsplash.com/photo-1730130856640-3db880ab33b7?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxhc2lhbiUyMGNlcmVtb25pYWwlMjB3ZWRkaW5nJTIwdHJhZGl0aW9uYWx8ZW58MXx8fHwxNzU3Njc0NDMwfDA&ixlib=rb-4.1.0&q=80&w=1080',
      features: ['Mâm quả cưới hỏi', 'Tư vấn nghi thức', 'Lễ vật truyền thống', 'Hỗ trợ tổ chức']
    },
    {
      id: 4,
      title: 'Dịch vụ Cúng Phật',
      description: 'Chuẩn bị lễ vật cúng Phật với hoa sen, trái cây, hương nến cao cấp. Tư vấn về các ngày lễ Phật giáo.',
      price: 'Từ 980.000₫',
      duration: '1-2 giờ',
      image: 'https://images.unsplash.com/photo-1573460630303-81cbaf895c54?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsb3R1cyUyMGZsb3dlciUyMGNlcmVtb25pYWwlMjBnb2xkfGVufDF8fHx8MTc1NzY3NDQyOXww&ixlib=rb-4.1.0&q=80&w=1080',
      features: ['Hoa sen tươi', 'Trái cây cúng Phật', 'Hương nến cao cấp', 'Tư vấn ngày lễ Phật']
    },
    {
      id: 5,
      title: 'Dịch vụ Lễ Tân Gia',
      description: 'Trọn gói lễ tân gia với mâm cúng thổ địa, ông táo, tư vấn phong thủy ngôi nhà mới.',
      price: 'Từ 1.800.000₫',
      duration: '2-3 giờ',
      image: 'https://images.unsplash.com/photo-1532334722716-c5850cdd878d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwaW5jZW5zZSUyMGNlcmVtb255JTIwdHJhZGl0aW9uYWx8ZW58MXx8fHwxNzU3Njc0NDI4fDA&ixlib=rb-4.1.0&q=80&w=1080',
      features: ['Mâm cúng thổ địa', 'Tư vấn phong thủy', 'Lễ cúng ông táo', 'Setup tại nhà mới']
    },
    {
      id: 6,
      title: 'Dịch vụ Tư Vấn Phong Thủy',
      description: 'Tư vấn chuyên sâu về phong thủy, lựa chọn ngày tốt, bố trí không gian theo phong thủy.',
      price: 'Từ 500.000₫',
      duration: '1-2 giờ',
      image: 'https://images.unsplash.com/photo-1732117924212-39bfaec174c9?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx0cmFkaXRpb25hbCUyMGNhbmRsZXMlMjByZWQlMjBnb2xkfGVufDF8fHx8MTc1NzY3NDQyOXww&ixlib=rb-4.1.0&q=80&w=1080',
      features: ['Tư vấn phong thủy chuyên sâu', 'Chọn ngày tốt', 'Bố trí không gian', 'Báo cáo chi tiết']
    }
  ];

  const processSteps = [
    {
      step: '01',
      title: 'Tư vấn & Đặt lịch',
      description: 'Liên hệ hotline hoặc website để được tư vấn miễn phí và đặt lịch phù hợp',
      icon: Phone
    },
    {
      step: '02',
      title: 'Lựa chọn ngày tốt',
      description: 'Chuyên gia phong thủy sẽ tư vấn và lựa chọn ngày giờ tốt nhất cho nghi lễ',
      icon: Calendar
    },
    {
      step: '03',
      title: 'Chuẩn bị lễ vật',
      description: 'Đội ngũ chuyên nghiệp chuẩn bị đầy đủ lễ vật theo yêu cầu và truyền thống',
      icon: Award
    },
    {
      step: '04',
      title: 'Thực hiện dịch vụ',
      description: 'Giao hàng và hỗ trợ thực hiện nghi lễ tại địa điểm của khách hàng',
      icon: CheckCircle
    }
  ];

  const whyChooseUs = [
    {
      icon: Users,
      title: 'Đội ngũ chuyên nghiệp',
      description: 'Chuyên gia phong thủy và nhân viên nhiều năm kinh nghiệm'
    },
    {
      icon: Clock,
      title: 'Giao hàng đúng giờ',
      description: 'Cam kết giao hàng và phục vụ đúng thời gian đã hẹn'
    },
    {
      icon: Shield,
      title: 'Chất lượng đảm bảo',
      description: 'Sản phẩm chất lượng cao, nguồn gốc rõ ràng'
    },
    {
      icon: Heart,
      title: 'Tư vấn tận tình',
      description: 'Hỗ trợ 24/7, tư vấn miễn phí về nghi lễ và phong thủy'
    }
  ];

  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <section className="relative py-20 bg-gradient-to-r from-amber-900 to-red-800 text-white overflow-hidden">
        <div 
          className="absolute inset-0 opacity-20"
          style={{
            backgroundImage: `url('https://images.unsplash.com/photo-1588358581442-c0a340052b75?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwdGVtcGxlJTIwcHJheWVyJTIwY2VyZW1vbnl8ZW58MXx8fHwxNzU3Njc0NDMwfDA&ixlib=rb-4.1.0&q=80&w=1080')`,
            backgroundSize: 'cover',
            backgroundPosition: 'center'
          }}
        />
        <div className="relative max-w-6xl mx-auto px-4 text-center">
          <Flower2 className="w-16 h-16 mx-auto mb-6 text-yellow-300" />
          <h1 className="text-4xl md:text-5xl mb-6">Dịch vụ của chúng tôi</h1>
          <p className="text-xl text-yellow-100 max-w-3xl mx-auto leading-relaxed">
            Trọn gói các dịch vụ nghi lễ truyền thống với chất lượng cao và đội ngũ chuyên nghiệp
          </p>
        </div>
      </section>

      {/* Services Grid */}
      <section className="py-16 bg-gradient-to-br from-yellow-50 to-red-50">
        <div className="max-w-6xl mx-auto px-4">
          <div className="text-center mb-12">
            <Flower2 className="w-12 h-12 mx-auto mb-4 text-amber-600" />
            <h2 className="text-3xl md:text-4xl text-amber-900 mb-4">
              Các dịch vụ chuyên nghiệp
            </h2>
            <p className="text-gray-600 max-w-2xl mx-auto">
              Đa dạng các dịch vụ nghi lễ từ cơ bản đến cao cấp, phù hợp với mọi nhu cầu
            </p>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
            {services.map((service) => (
              <Card key={service.id} className="hover:shadow-xl transition-all duration-300 border-2 hover:border-amber-300 relative">
                {service.popular && (
                  <Badge className="absolute -top-3 left-1/2 transform -translate-x-1/2 bg-red-600 text-white z-10">
                    Phổ biến nhất
                  </Badge>
                )}
                
                <div className="relative overflow-hidden">
                  <ImageWithFallback
                    src={service.image}
                    alt={service.title}
                    className="w-full h-48 object-cover group-hover:scale-105 transition-transform duration-300"
                  />
                  <div className="absolute top-4 right-4">
                    <Badge className="bg-white/90 text-amber-900">
                      {service.duration}
                    </Badge>
                  </div>
                </div>

                <CardContent className="p-6">
                  <CardHeader className="p-0 mb-4">
                    <CardTitle className="text-xl text-amber-900 mb-2">
                      {service.title}
                    </CardTitle>
                    <p className="text-gray-600 text-sm leading-relaxed">
                      {service.description}
                    </p>
                  </CardHeader>

                  <div className="space-y-3 mb-6">
                    {service.features.map((feature, idx) => (
                      <div key={idx} className="flex items-center gap-2">
                        <CheckCircle className="w-4 h-4 text-green-600 flex-shrink-0" />
                        <span className="text-sm text-gray-700">{feature}</span>
                      </div>
                    ))}
                  </div>

                  <div className="border-t pt-4">
                    <div className="flex items-center justify-between mb-4">
                      <span className="text-2xl text-red-600">{service.price}</span>
                      <div className="flex items-center">
                        {[...Array(5)].map((_, i) => (
                          <Star key={i} className="w-4 h-4 fill-yellow-400 text-yellow-400" />
                        ))}
                        <span className="text-sm text-gray-600 ml-1">(4.9)</span>
                      </div>
                    </div>
                    
                    <div className="flex gap-2">
                      <Button className="flex-1 bg-red-600 hover:bg-red-700 text-white">
                        Đặt dịch vụ
                      </Button>
                      <Button variant="outline" className="border-amber-300 text-amber-700 hover:bg-amber-50">
                        Chi tiết
                      </Button>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* Process Steps */}
      <section className="py-16 bg-white">
        <div className="max-w-6xl mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl md:text-4xl text-amber-900 mb-4">
              Quy trình phục vụ
            </h2>
            <p className="text-gray-600 max-w-2xl mx-auto">
              4 bước đơn giản để nhận được dịch vụ chuyên nghiệp
            </p>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-8">
            {processSteps.map((step, index) => (
              <Card key={index} className="text-center relative border-2 border-amber-200 hover:border-amber-400 transition-colors">
                <CardContent className="p-6">
                  <div className="w-16 h-16 mx-auto mb-4 bg-gradient-to-br from-amber-400 to-red-500 rounded-full flex items-center justify-center relative">
                    <step.icon className="w-8 h-8 text-white" />
                    <Badge className="absolute -top-2 -right-2 bg-amber-900 text-white text-xs px-2">
                      {step.step}
                    </Badge>
                  </div>
                  <h3 className="text-lg text-amber-900 mb-3">{step.title}</h3>
                  <p className="text-gray-600 text-sm leading-relaxed">{step.description}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* Why Choose Us */}
      <section className="py-16 bg-amber-900 text-white">
        <div className="max-w-6xl mx-auto px-4">
          <div className="text-center mb-12">
            <Flower2 className="w-12 h-12 mx-auto mb-4 text-yellow-300" />
            <h2 className="text-3xl md:text-4xl mb-4">
              Tại sao chọn chúng tôi?
            </h2>
            <p className="text-yellow-100 max-w-2xl mx-auto">
              Những giá trị cốt lõi tạo nên sự khác biệt trong dịch vụ của chúng tôi
            </p>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-8">
            {whyChooseUs.map((item, index) => (
              <Card key={index} className="bg-white/10 backdrop-blur border-white/20 text-white text-center">
                <CardContent className="p-6">
                  <div className="w-16 h-16 mx-auto mb-4 bg-white/20 rounded-full flex items-center justify-center">
                    <item.icon className="w-8 h-8 text-yellow-300" />
                  </div>
                  <h3 className="text-lg mb-3 text-yellow-300">{item.title}</h3>
                  <p className="text-yellow-100 text-sm leading-relaxed">{item.description}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-16 bg-gradient-to-r from-yellow-50 to-red-50">
        <div className="max-w-4xl mx-auto px-4 text-center">
          <Card className="border-2 border-amber-400 shadow-xl">
            <CardContent className="p-8">
              <Sparkles className="w-16 h-16 mx-auto mb-6 text-amber-600" />
              <h2 className="text-3xl text-amber-900 mb-4">
                Sẵn sàng đặt dịch vụ?
              </h2>
              <p className="text-gray-600 mb-8 max-w-2xl mx-auto">
                Liên hệ ngay với chúng tôi để được tư vấn miễn phí và nhận ưu đãi đặc biệt 
                cho khách hàng đặt dịch vụ lần đầu.
              </p>
              
              <div className="flex flex-col sm:flex-row gap-4 justify-center">
                <Button size="lg" className="bg-red-600 hover:bg-red-700 text-white px-8 py-3">
                  <Phone className="w-5 h-5 mr-2" />
                  Gọi ngay: 1900 1234
                </Button>
                <Button size="lg" variant="outline" className="border-amber-300 text-amber-700 hover:bg-amber-50 px-8 py-3">
                  <Calendar className="w-5 h-5 mr-2" />
                  Đặt lịch tư vấn
                </Button>
              </div>
              
              <div className="mt-6 flex items-center justify-center gap-4 text-sm text-gray-600">
                <div className="flex items-center gap-1">
                  <CheckCircle className="w-4 h-4 text-green-600" />
                  <span>Tư vấn miễn phí</span>
                </div>
                <div className="flex items-center gap-1">
                  <CheckCircle className="w-4 h-4 text-green-600" />
                  <span>Ưu đãi khách hàng mới</span>
                </div>
                <div className="flex items-center gap-1">
                  <CheckCircle className="w-4 h-4 text-green-600" />
                  <span>Hỗ trợ 24/7</span>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </section>
    </div>
  );
}

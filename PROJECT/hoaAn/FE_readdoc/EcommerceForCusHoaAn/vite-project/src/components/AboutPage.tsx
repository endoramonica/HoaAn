import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { 
  Flower2, 
  Users, 
  Award, 
  Clock, 
  Heart,
  Shield,
  Target,
  Sparkles
} from 'lucide-react';

export function AboutPage() {
  const stats = [
    { number: '50,000+', label: 'Khách hàng tin tưởng', icon: Users },
    { number: '15+', label: 'Năm kinh nghiệm', icon: Clock },
    { number: '99.8%', label: 'Độ hài lòng khách hàng', icon: Heart },
    { number: '24/7', label: 'Hỗ trợ khách hàng', icon: Shield }
  ];

  const values = [
    {
      icon: Target,
      title: 'Sứ mệnh',
      description: 'Mang đến cho mọi gia đình Việt Nam những sản phẩm đồ cúng chất lượng cao, giúp các nghi lễ truyền thống được thực hiện một cách chuẩn xác và ý nghĩa.'
    },
    {
      icon: Award,
      title: 'Tầm nhìn',
      description: 'Trở thành thương hiệu đồ cúng trực tuyến hàng đầu Việt Nam, góp phần bảo tồn và phát huy những giá trị văn hóa truyền thống của dân tộc.'
    },
    {
      icon: Sparkles,
      title: 'Giá trị cốt lõi',
      description: 'Chất lượng - Tôn trọng - Truyền thống - Đổi mới. Chúng tôi luôn đặt chất lượng sản phẩm và sự hài lòng của khách hàng lên hàng đầu.'
    }
  ];

  const team = [
    {
      name: 'Ông Nguyễn Văn Minh',
      position: 'Nhà sáng lập & CEO',
      description: 'Với hơn 20 năm kinh nghiệm trong lĩnh vực văn hóa tâm linh, ông Minh đã dẫn dắt công ty phát triển vững mạnh.',
      image: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=300&h=300&fit=crop&crop=face'
    },
    {
      name: 'Bà Trần Thị Hoa',
      position: 'Giám đốc Sản xuất',
      description: 'Chuyên gia hàng đầu về các sản phẩm đồ cúng truyền thống, đảm bảo chất lượng theo tiêu chuẩn cao nhất.',
      image: 'https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=300&h=300&fit=crop&crop=face'
    },
    {
      name: 'Thầy Lê Minh Đức',
      position: 'Cố vấn Phong thủy',
      description: 'Thầy phong thủy nổi tiếng với kinh nghiệm tư vấn nghi lễ cho hàng nghìn gia đình trên toàn quốc.',
      image: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300&h=300&fit=crop&crop=face'
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
          <h1 className="text-4xl md:text-5xl mb-6">Về chúng tôi</h1>
          <p className="text-xl text-yellow-100 max-w-3xl mx-auto leading-relaxed">
            Đồ Cúng Online được thành lập với sứ mệnh mang đến cho mọi gia đình Việt Nam 
            những sản phẩm đồ cúng chất lượng cao, giúp bảo tồn và phát huy các giá trị văn hóa truyền thống.
          </p>
        </div>
      </section>

      {/* Stats Section */}
      <section className="py-16 bg-white">
        <div className="max-w-6xl mx-auto px-4">
          <div className="grid grid-cols-2 md:grid-cols-4 gap-8">
            {stats.map((stat, index) => (
              <Card key={index} className="text-center border-2 border-amber-200 hover:border-amber-400 transition-colors">
                <CardContent className="p-6">
                  <div className="w-16 h-16 mx-auto mb-4 bg-gradient-to-br from-amber-400 to-red-500 rounded-full flex items-center justify-center">
                    <stat.icon className="w-8 h-8 text-white" />
                  </div>
                  <div className="text-3xl text-amber-900 mb-2">{stat.number}</div>
                  <p className="text-gray-600">{stat.label}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* Story Section */}
      <section className="py-16 bg-gradient-to-br from-yellow-50 to-red-50">
        <div className="max-w-6xl mx-auto px-4">
          <div className="grid md:grid-cols-2 gap-12 items-center">
            <div>
              <h2 className="text-3xl md:text-4xl text-amber-900 mb-6">
                Câu chuyện của chúng tôi
              </h2>
              <div className="space-y-4 text-gray-700 leading-relaxed">
                <p>
                  Đồ Cúng Online ra đời từ mong muốn giúp các gia đình Việt Nam duy trì 
                  những truyền thống văn hóa tâm linh quý báu trong cuộc sống hiện đại.
                </p>
                <p>
                  Khởi đầu từ một cửa hàng nhỏ tại Hà Nội năm 2008, chúng tôi đã không ngừng 
                  phát triển và hoàn thiện để trở thành một trong những thương hiệu đồ cúng 
                  trực tuyến uy tín nhất Việt Nam.
                </p>
                <p>
                  Với đội ngũ chuyên gia am hiểu sâu sắc về văn hóa tâm linh và công nghệ hiện đại, 
                  chúng tôi mang đến những sản phẩm và dịch vụ chất lượng cao, phù hợp với 
                  nhu cầu của từng gia đình.
                </p>
              </div>
            </div>
            <div className="relative">
              <ImageWithFallback
                src="https://images.unsplash.com/photo-1532334722716-c5850cdd878d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwaW5jZW5zZSUyMGNlcmVtb255JTIwdHJhZGl0aW9uYWx8ZW58MXx8fHwxNzU3Njc0NDI4fDA&ixlib=rb-4.1.0&q=80&w=1080"
                alt="Truyền thống Việt Nam"
                className="w-full h-96 object-cover rounded-lg shadow-xl"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/30 to-transparent rounded-lg"></div>
            </div>
          </div>
        </div>
      </section>

      {/* Values Section */}
      <section className="py-16 bg-white">
        <div className="max-w-6xl mx-auto px-4">
          <div className="text-center mb-12">
            <Flower2 className="w-12 h-12 mx-auto mb-4 text-amber-600" />
            <h2 className="text-3xl md:text-4xl text-amber-900 mb-4">
              Sứ mệnh & Giá trị
            </h2>
            <p className="text-gray-600 max-w-2xl mx-auto">
              Những giá trị cốt lõi định hướng mọi hoạt động của chúng tôi
            </p>
          </div>
          
          <div className="grid md:grid-cols-3 gap-8">
            {values.map((value, index) => (
              <Card key={index} className="text-center border-2 border-amber-200 hover:border-amber-400 hover:shadow-lg transition-all">
                <CardHeader>
                  <div className="w-16 h-16 mx-auto mb-4 bg-gradient-to-br from-amber-400 to-red-500 rounded-full flex items-center justify-center">
                    <value.icon className="w-8 h-8 text-white" />
                  </div>
                  <CardTitle className="text-xl text-amber-900">{value.title}</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-gray-600 leading-relaxed">{value.description}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* Team Section */}
      <section className="py-16 bg-gradient-to-br from-red-50 to-yellow-50">
        <div className="max-w-6xl mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl md:text-4xl text-amber-900 mb-4">
              Đội ngũ chuyên gia
            </h2>
            <p className="text-gray-600 max-w-2xl mx-auto">
              Những con người tâm huyết, giàu kinh nghiệm đứng sau thành công của chúng tôi
            </p>
          </div>
          
          <div className="grid md:grid-cols-3 gap-8">
            {team.map((member, index) => (
              <Card key={index} className="text-center hover:shadow-lg transition-shadow border-2 border-amber-200">
                <CardContent className="p-6">
                  <div className="relative mb-6">
                    <ImageWithFallback
                      src={member.image}
                      alt={member.name}
                      className="w-32 h-32 mx-auto rounded-full object-cover border-4 border-amber-200"
                    />
                  </div>
                  <h3 className="text-xl text-amber-900 mb-2">{member.name}</h3>
                  <Badge className="bg-amber-100 text-amber-800 mb-4">
                    {member.position}
                  </Badge>
                  <p className="text-gray-600 leading-relaxed">{member.description}</p>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </section>

      {/* Awards & Certifications */}
      <section className="py-16 bg-amber-900 text-white">
        <div className="max-w-6xl mx-auto px-4 text-center">
          <Flower2 className="w-12 h-12 mx-auto mb-6 text-yellow-300" />
          <h2 className="text-3xl md:text-4xl mb-6">Chứng nhận & Giải thưởng</h2>
          
          <div className="grid md:grid-cols-4 gap-6 mt-12">
            <Card className="bg-white/10 backdrop-blur border-white/20 text-white">
              <CardContent className="p-6 text-center">
                <Award className="w-12 h-12 mx-auto mb-4 text-yellow-300" />
                <h3 className="text-lg mb-2">Thương hiệu uy tín</h3>
                <p className="text-sm text-yellow-100">Năm 2023</p>
              </CardContent>
            </Card>
            
            <Card className="bg-white/10 backdrop-blur border-white/20 text-white">
              <CardContent className="p-6 text-center">
                <Shield className="w-12 h-12 mx-auto mb-4 text-yellow-300" />
                <h3 className="text-lg mb-2">Chứng nhận ISO</h3>
                <p className="text-sm text-yellow-100">Quản lý chất lượng</p>
              </CardContent>
            </Card>
            
            <Card className="bg-white/10 backdrop-blur border-white/20 text-white">
              <CardContent className="p-6 text-center">
                <Users className="w-12 h-12 mx-auto mb-4 text-yellow-300" />
                <h3 className="text-lg mb-2">Top 10 thương hiệu</h3>
                <p className="text-sm text-yellow-100">Uy tín Việt Nam</p>
              </CardContent>
            </Card>
            
            <Card className="bg-white/10 backdrop-blur border-white/20 text-white">
              <CardContent className="p-6 text-center">
                <Sparkles className="w-12 h-12 mx-auto mb-4 text-yellow-300" />
                <h3 className="text-lg mb-2">Sản phẩm xuất sắc</h3>
                <p className="text-sm text-yellow-100">Năm 2024</p>
              </CardContent>
            </Card>
          </div>
        </div>
      </section>
    </div>
  );
}

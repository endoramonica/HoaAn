import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { 
  Flower2,
  Phone,
  Mail,
  MapPin,
  Facebook,
  Instagram,
  MessageCircle
} from 'lucide-react';

export function Footer() {
  return (
    <footer className="bg-amber-900 text-white py-12">
      <div className="max-w-6xl mx-auto px-4">
        <div className="grid md:grid-cols-4 gap-8">
          {/* Company Info */}
          <div>
            <div className="flex items-center gap-2 mb-4">
              <Flower2 className="w-8 h-8 text-yellow-400" />
              <h3 className="text-xl">Đồ Cúng Online</h3>
            </div>
            <p className="text-yellow-100 mb-4">
              Dịch vụ đồ cúng và nghi lễ trực tuyến hàng đầu Việt Nam. 
              Chuyên cung cấp các sản phẩm cúng chất lượng cao.
            </p>
            <div className="flex gap-3">
              <Button size="icon" variant="outline" className="border-white/30 text-white hover:bg-white hover:text-amber-900">
                <Facebook className="w-4 h-4" />
              </Button>
              <Button size="icon" variant="outline" className="border-white/30 text-white hover:bg-white hover:text-amber-900">
                <Instagram className="w-4 h-4" />
              </Button>
              <Button size="icon" variant="outline" className="border-white/30 text-white hover:bg-white hover:text-amber-900">
                <MessageCircle className="w-4 h-4" />
              </Button>
            </div>
          </div>
          
          {/* Quick Links */}
          <div>
            <h4 className="text-lg mb-4">Liên kết nhanh</h4>
            <ul className="space-y-2 text-yellow-100">
              <li><a href="#" className="hover:text-white transition-colors">Về chúng tôi</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Sản phẩm</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Dịch vụ</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Tin tức</a></li>
              <li><a href="#" className="hover:text-white transition-colors">FAQ</a></li>
            </ul>
          </div>
          
          {/* Policies */}
          <div>
            <h4 className="text-lg mb-4">Chính sách</h4>
            <ul className="space-y-2 text-yellow-100">
              <li><a href="#" className="hover:text-white transition-colors">Chính sách giao hàng</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Chính sách đổi trả</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Chính sách bảo mật</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Điều khoản sử dụng</a></li>
              <li><a href="#" className="hover:text-white transition-colors">Hướng dẫn đặt hàng</a></li>
            </ul>
          </div>
          
          {/* Contact */}
          <div>
            <h4 className="text-lg mb-4">Liên hệ</h4>
            <div className="space-y-3 text-yellow-100">
              <div className="flex items-center gap-2">
                <Phone className="w-4 h-4" />
                <span>1900 1234 (8h-22h)</span>
              </div>
              <div className="flex items-center gap-2">
                <MessageCircle className="w-4 h-4" />
                <span>Zalo OA: @docungonline</span>
              </div>
              <div className="flex items-center gap-2">
                <Mail className="w-4 h-4" />
                <span>support@docungonline.vn</span>
              </div>
              <div className="flex items-start gap-2">
                <MapPin className="w-4 h-4 mt-1 flex-shrink-0" />
                <span>123 Đường Láng, Đống Đa, Hà Nội</span>
              </div>
            </div>
            
            <div className="mt-4">
              <h5 className="text-sm mb-2 text-yellow-300">Công nghệ sử dụng:</h5>
              <div className="flex flex-wrap gap-1">
                <Badge variant="secondary" className="bg-white/20 text-white text-xs">Firebase</Badge>
                <Badge variant="secondary" className="bg-white/20 text-white text-xs">MoMo</Badge>
                <Badge variant="secondary" className="bg-white/20 text-white text-xs">VNPay</Badge>
                <Badge variant="secondary" className="bg-white/20 text-white text-xs">Lunar API</Badge>
              </div>
            </div>
          </div>
        </div>
        
        <div className="border-t border-white/20 mt-8 pt-8 text-center text-yellow-100">
          <p>&copy; 2025 Đồ Cúng Online. Tất cả quyền được bảo lưu.</p>
          <p className="text-sm mt-2">Giấy phép kinh doanh số: 0123456789 do Sở KH&ĐT Hà Nội cấp</p>
        </div>
      </div>
    </footer>
  );
}
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import { Button } from "./ui/button";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "./ui/card";
import { Badge } from "./ui/badge";
import { ImageWithFallback } from "./figma/ImageWithFallback";
import { Skeleton } from "./ui/skeleton";
import { Alert, AlertDescription } from "./ui/alert";
import {
  Truck,
  Package,
  Clock,
  Shield,
  Heart,
  Flame,
  Brain,
  Zap,
  CheckCircle2,
  Star,
  Calendar,
  AlertCircle,
} from "lucide-react";
import { vietCommerceProductService } from "../lib/services/vietCommerceProductService";
import { useApp } from "../lib/contexts/AppContext";
import { getActionTrackingService } from "../lib/services/actionTrackingService";
import type { ProductListDto, ProductFilterDto } from "../lib/services/vietCommerceProductService";

export function ServicesPage() {
  const navigate = useNavigate();
  const { setSelectedServiceProductId } = useApp();
  const actionTracking = getActionTrackingService();
  const [services, setServices] = useState<ProductListDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Fetch services from API
  useEffect(() => {
    const fetchServices = async () => {
      try {
        setIsLoading(true);
        setError(null);

        const filter: ProductFilterDto = {
          pageNumber: 1,
          pageSize: 100,
          type: 'service', // Only fetch services
          isActive: true,
        };

        const result = await vietCommerceProductService.getProducts(filter);
        setServices(result.items || []);
      } catch (err: any) {
        console.error('Error fetching services:', err);
        setError(err.message || 'Không thể tải danh sách dịch vụ. Vui lòng thử lại sau.');
      } finally {
        setIsLoading(false);
      }
    };

    fetchServices();
  }, []);

  const processSteps = [
    {
      step: "01",
      title: "Tư vấn & Đặt lịch",
      description:
        "Liên hệ hotline hoặc website để được tư vấn miễn phí và đặt lịch phù hợp",
      icon: Truck,
    },
    {
      step: "02",
      title: "Lựa chọn ngày tốt",
      description:
        "Chuyên gia phong thủy sẽ tư vấn và lựa chọn ngày giờ tốt nhất cho nghi lễ",
      icon: Calendar,
    },
    {
      step: "03",
      title: "Chuẩn bị lễ vật",
      description:
        "Đội ngũ chuyên nghiệp chuẩn bị đầy đủ lễ vật theo yêu cầu và truyền thống",
      icon: Package,
    },
    {
      step: "04",
      title: "Thực hiện dịch vụ",
      description:
        "Giao hàng và hỗ trợ thực hiện nghi lễ tại địa điểm của khách hàng",
      icon: CheckCircle2,
    },
  ];

  const whyChooseUs = [
    {
      icon: Brain,
      title: "Đội ngũ chuyên nghiệp",
      description:
        "Chuyên gia phong thủy và nhân viên nhiều năm kinh nghiệm",
    },
    {
      icon: Clock,
      title: "Giao hàng đúng giờ",
      description:
        "Cam kết giao hàng và phục vụ đúng thời gian đã hẹn",
    },
    {
      icon: Shield,
      title: "Chất lượng đảm bảo",
      description: "Sản phẩm chất lượng cao, nguồn gốc rõ ràng",
    },
    {
      icon: Heart,
      title: "Tư vấn tận tình",
      description:
        "Hỗ trợ 24/7, tư vấn miễn phí về nghi lễ và phong thủy",
    },
  ];

  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <section className="relative py-20 bg-gradient-to-r from-amber-900 to-red-800 text-white overflow-hidden">
        <div
          className="absolute inset-0 opacity-20"
          style={{
            backgroundImage: `url('https://images.unsplash.com/photo-1588358581442-c0a340052b75?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwdGVtcGxlJTIwcHJheWVyJTIwY2VyZW1vbnl8ZW58MXx8fHwxNzU3Njc0NDMwfDA&ixlib=rb-4.1.0&q=80&w=1080')`,
            backgroundSize: "cover",
            backgroundPosition: "center",
          }}
        />
        <div className="relative max-w-6xl mx-auto px-4 text-center">
          <Flame className="w-16 h-16 mx-auto mb-6 text-yellow-300" />
          <h1 className="text-4xl md:text-5xl mb-6">
            Dịch vụ của chúng tôi
          </h1>
          <p className="text-xl text-yellow-100 max-w-3xl mx-auto leading-relaxed">
            Trọn gói các dịch vụ nghi lễ truyền thống với chất
            lượng cao và đội ngũ chuyên nghiệp
          </p>
        </div>
      </section>

      {/* Services Grid */}
      <section className="py-16 bg-gradient-to-br from-yellow-50 to-red-50">
        <div className="max-w-6xl mx-auto px-4">
          <div className="text-center mb-12">
            <Flame className="w-12 h-12 mx-auto mb-4 text-amber-600" />
            <h2 className="text-3xl md:text-4xl text-amber-900 mb-4">
              Các dịch vụ chuyên nghiệp
            </h2>
            <p className="text-gray-600 max-w-2xl mx-auto">
              Đa dạng các dịch vụ nghi lễ từ cơ bản đến cao cấp,
              phù hợp với mọi nhu cầu
            </p>
          </div>

          {/* Error Alert */}
          {error && (
            <Alert variant="destructive" className="mb-6">
              <AlertCircle className="h-4 w-4" />
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}

          {/* Loading Skeleton */}
          {isLoading && (
            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
              {Array.from({ length: 6 }, (_, i) => (
                <Card key={i} className="border-2 border-amber-200">
                  <Skeleton className="w-full h-48" />
                  <CardContent className="p-6">
                    <Skeleton className="h-6 w-full mb-2" />
                    <Skeleton className="h-6 w-3/4 mb-4" />
                    <Skeleton className="h-8 w-24 mb-4" />
                    <Skeleton className="h-10 w-full" />
                  </CardContent>
                </Card>
              ))}
            </div>
          )}

          {/* Services Grid */}
          {!isLoading && services.length > 0 && (
            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
              {services.map((service) => (
                <Card
                  key={service.id}
                  className="hover:shadow-xl transition-all duration-300 border-2 hover:border-amber-300 relative"
                >
                  {service.isFeatured && (
                    <Badge className="absolute -top-3 left-1/2 transform -translate-x-1/2 bg-red-600 text-white z-10">
                      Phổ biến nhất
                    </Badge>
                  )}
                  <div className="relative overflow-hidden">
                    <ImageWithFallback
                      src={service.primaryImage || 'https://images.unsplash.com/photo-1588358581442-c0a340052b75?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwdGVtcGxlJTIwcHJheWVyJTIwY2VyZW1vbnl8ZW58MXx8fHwxNzU3Njc0NDMwfDA&ixlib=rb-4.1.0&q=80&w=1080'}
                      alt={service.name || 'Dịch vụ'}
                      className="w-full h-48 object-cover group-hover:scale-105 transition-transform duration-300"
                    />
                    <div className="absolute top-4 right-4">
                      <Badge className="bg-white/90 text-amber-900">
                        {service.serviceDuration || '2-3 giờ'}
                      </Badge>
                    </div>
                  </div>
                  <CardContent className="p-6">
                    <CardHeader className="p-0 mb-4">
                      <CardTitle className="text-xl text-amber-900 mb-2">
                        {service.name}
                      </CardTitle>
                      <p className="text-gray-600 text-sm leading-relaxed">
                        {service.shortDescription}
                      </p>
                    </CardHeader>
                    <div className="space-y-3 mb-6">
                      <div className="flex items-center gap-2">
                        <CheckCircle2 className="w-4 h-4 text-green-600 flex-shrink-0" />
                        <span className="text-sm text-gray-700">
                          Danh mục: {service.serviceCategory || 'Dịch vụ'}
                        </span>
                      </div>
                      <div className="flex items-center gap-2">
                        <CheckCircle2 className="w-4 h-4 text-green-600 flex-shrink-0" />
                        <span className="text-sm text-gray-700">
                          Thời gian: {service.serviceDuration || '2-3 giờ'}
                        </span>
                      </div>
                      <div className="flex items-center gap-2">
                        <CheckCircle2 className="w-4 h-4 text-green-600 flex-shrink-0" />
                        <span className="text-sm text-gray-700">
                          Còn lại: {service.stockQuantity} dịch vụ
                        </span>
                      </div>
                    </div>
                    <div className="border-t pt-4">
                      <div className="flex items-center justify-between mb-4">
                        <span className="text-2xl text-red-600">
                          {new Intl.NumberFormat('vi-VN').format(service.price || 0)}₫
                        </span>
                        <div className="flex items-center">
                          {[...Array(5)].map((_, i) => (
                            <Star
                              key={i}
                              className={`w-4 h-4 ${
                                i < Math.floor(service.serviceRating || 0)
                                  ? 'fill-yellow-400 text-yellow-400'
                                  : 'text-gray-300'
                              }`}
                            />
                          ))}
                          <span className="text-sm text-gray-600 ml-1">
                            ({(service.serviceRating || 0).toFixed(1)})
                          </span>
                        </div>
                      </div>
                      <div className="flex gap-2">
                        <Button
                          onClick={() => {
                            // Track ViewProduct action for service
                            actionTracking.trackAction('ViewProduct', { serviceType: service.serviceCategory }, service.id, service.categoryId);
                            // Set the service product ID in context
                            if (service.id) {
                              setSelectedServiceProductId(service.id);
                              // Navigate to calendar
                              navigate("/calendar");
                            }
                          }}
                          className="flex-1 bg-red-600 hover:bg-red-700 text-white"
                        >
                          Đặt dịch vụ
                        </Button>
                        <Button
                          onClick={() => {
                            // Track ViewProduct action for service detail
                            actionTracking.trackAction('ViewProduct', { serviceType: service.serviceCategory }, service.id, service.categoryId);
                            navigate(`/services/${service.id}`);
                          }}
                          variant="outline"
                          className="border-amber-300 text-amber-700 hover:bg-amber-50"
                        >
                          Chi tiết
                        </Button>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          )}

          {/* No Services Found */}
          {!isLoading && services.length === 0 && !error && (
            <div className="text-center py-16">
              <div className="w-24 h-24 mx-auto mb-4 bg-amber-100 rounded-full flex items-center justify-center">
                <Flame className="w-12 h-12 text-amber-600" />
              </div>
              <h3 className="text-xl text-amber-900 mb-2">Chưa có dịch vụ nào</h3>
              <p className="text-gray-600">
                Vui lòng quay lại sau để xem các dịch vụ mới
              </p>
            </div>
          )}
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
              <Card
                key={index}
                className="text-center relative border-2 border-amber-200 hover:border-amber-400 transition-colors"
              >
                <CardContent className="p-6">
                  <div className="w-16 h-16 mx-auto mb-4 bg-gradient-to-br from-amber-400 to-red-500 rounded-full flex items-center justify-center relative">
                    <step.icon className="w-8 h-8 text-white" />
                    <Badge className="absolute -top-2 -right-2 bg-amber-900 text-white text-xs px-2">
                      {step.step}
                    </Badge>
                  </div>
                  <h3 className="text-lg text-amber-900 mb-3">
                    {step.title}
                  </h3>
                  <p className="text-gray-600 text-sm leading-relaxed">
                    {step.description}
                  </p>
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
            <Flame className="w-12 h-12 mx-auto mb-4 text-yellow-300" />
            <h2 className="text-3xl md:text-4xl mb-4">
              Tại sao chọn chúng tôi?
            </h2>
            <p className="text-yellow-100 max-w-2xl mx-auto">
              Những giá trị cốt lõi tạo nên sự khác biệt trong
              dịch vụ của chúng tôi
            </p>
          </div>
          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-8">
            {whyChooseUs.map((item, index) => (
              <Card
                key={index}
                className="bg-white/10 backdrop-blur border-white/20 text-white text-center"
              >
                <CardContent className="p-6">
                  <div className="w-16 h-16 mx-auto mb-4 bg-white/20 rounded-full flex items-center justify-center">
                    <item.icon className="w-8 h-8 text-yellow-300" />
                  </div>
                  <h3 className="text-lg mb-3 text-yellow-300">
                    {item.title}
                  </h3>
                  <p className="text-yellow-100 text-sm leading-relaxed">
                    {item.description}
                  </p>
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
              <Zap className="w-16 h-16 mx-auto mb-6 text-amber-600" />
              <h2 className="text-3xl text-amber-900 mb-4">
                Sẵn sàng đặt dịch vụ?
              </h2>
              <p className="text-gray-600 mb-8 max-w-2xl mx-auto">
                Liên hệ ngay với chúng tôi để được tư vấn miễn
                phí và nhận ưu đãi đặc biệt cho khách hàng đặt
                dịch vụ lần đầu.
              </p>
              <div className="flex flex-col sm:flex-row gap-4 justify-center">
                <Button
                  size="lg"
                  onClick={() =>
                    (window.location.href = "tel:19001234")
                  }
                  className="bg-red-600 hover:bg-red-700 text-white px-8 py-3"
                >
                  <Truck className="w-5 h-5 mr-2" />
                  Gọi ngay: 1900 1234
                </Button>
                <Button
                  size="lg"
                  variant="outline"
                  onClick={() => {
                    // Track BrowseCategory action for service consultation
                    actionTracking.trackAction('BrowseCategory', { action: 'schedule-consultation' }, undefined, 'services');
                    navigate("/contact");
                  }}
                  className="border-amber-300 text-amber-700 hover:bg-amber-50 px-8 py-3"
                >
                  <Calendar className="w-5 h-5 mr-2" />
                  Đặt lịch tư vấn
                </Button>
              </div>
              <div className="mt-6 flex items-center justify-center gap-4 text-sm text-gray-600">
                <div className="flex items-center gap-1">
                  <CheckCircle2 className="w-4 h-4 text-green-600" />
                  <span>Tư vấn miễn phí</span>
                </div>
                <div className="flex items-center gap-1">
                  <CheckCircle2 className="w-4 h-4 text-green-600" />
                  <span>Ưu đãi khách hàng mới</span>
                </div>
                <div className="flex items-center gap-1">
                  <CheckCircle2 className="w-4 h-4 text-green-600" />
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

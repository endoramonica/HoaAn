import { useParams, useNavigate } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { Button } from './ui/button';
import { Card, CardContent } from './ui/card';
import { Badge } from './ui/badge';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { Skeleton } from './ui/skeleton';
import { Alert, AlertDescription } from './ui/alert';
import {
  ArrowLeft,
  Star,
  CheckCircle2,
  AlertCircle,
  ChevronLeft,
  ChevronRight,
  Eye,
  Heart,
} from 'lucide-react';
import { vietCommerceProductService } from '../lib/services/vietCommerceProductService';
import { useApp } from '../lib/contexts/AppContext';
import type { ProductListDto, ProductDetailDto } from '../lib/services/vietCommerceProductService';

export function ServiceDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { setSelectedServiceProductId } = useApp();
  const [service, setService] = useState<ProductDetailDto | null>(null);
  const [relatedServices, setRelatedServices] = useState<ProductListDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [carouselIndex, setCarouselIndex] = useState(0);
  const [selectedImageIndex, setSelectedImageIndex] = useState(0);

  useEffect(() => {
    const fetchService = async () => {
      if (!id) {
        setError('Service ID not found');
        setIsLoading(false);
        return;
      }

      try {
        setIsLoading(true);
        setError(null);
        
        // Fetch service detail by ID
        const serviceDetail = await vietCommerceProductService.getProductById(id);
        
        if (!serviceDetail) {
          setError('Dịch vụ không tìm thấy');
        } else {
          setService(serviceDetail);
          
          // Fetch related services
          const result = await vietCommerceProductService.getProducts({
            pageNumber: 1,
            pageSize: 100,
            type: 'service',
          });
          
          const related = result.items.filter(item => item.id !== id);
          setRelatedServices(related);
        }
      } catch (err: any) {
        console.error('Error fetching service:', err);
        setError(err.message || 'Không thể tải chi tiết dịch vụ. Vui lòng thử lại sau.');
      } finally {
        setIsLoading(false);
      }
    };

    fetchService();
  }, [id]);

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50 py-8">
        <div className="max-w-4xl mx-auto px-4">
          <Skeleton className="h-12 w-32 mb-8" />
          <div className="grid md:grid-cols-2 gap-8">
            <Skeleton className="h-96" />
            <div className="space-y-4">
              <Skeleton className="h-8 w-full" />
              <Skeleton className="h-6 w-3/4" />
              <Skeleton className="h-32 w-full" />
              <Skeleton className="h-10 w-full" />
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (error || !service) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50 py-8">
        <div className="max-w-4xl mx-auto px-4">
          <Button
            variant="outline"
            onClick={() => navigate('/services')}
            className="mb-6 border-amber-300 text-amber-700 hover:bg-amber-50"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Quay lại
          </Button>
          <Alert variant="destructive">
            <AlertCircle className="h-4 w-4" />
            <AlertDescription>{error || 'Dịch vụ không tìm thấy'}</AlertDescription>
          </Alert>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50 py-8">
      <div className="max-w-4xl mx-auto px-4">
        {/* Back Button */}
        <Button
          variant="outline"
          onClick={() => navigate('/services')}
          className="mb-6 border-amber-300 text-amber-700 hover:bg-amber-50"
        >
          <ArrowLeft className="w-4 h-4 mr-2" />
          Quay lại danh sách dịch vụ
        </Button>

        {/* Main Content */}
        <div className="grid md:grid-cols-2 gap-8">
          {/* Image Gallery */}
          <div className="space-y-4">
            {/* Main Image */}
            <Card className="border-2 border-amber-200 overflow-hidden">
              <ImageWithFallback
                src={
                  service.images && service.images.length > 0
                    ? service.images[selectedImageIndex]?.url || service.primaryImage
                    : service.primaryImage || 'https://images.unsplash.com/photo-1588358581442-c0a340052b75?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwdGVtcGxlJTIwcHJheWVyJTIwY2VyZW1vbnl8ZW58MXx8fHwxNzU3Njc0NDMwfDA&ixlib=rb-4.1.0&q=80&w=1080'
                }
                alt={service.name || 'Dịch vụ'}
                className="w-full h-96 object-cover"
              />
            </Card>

            {/* Thumbnail Gallery */}
            {service.images && service.images.length > 1 && (
              <div className="flex gap-2 overflow-x-auto pb-2">
                {service.images.map((image, index) => (
                  <button
                    key={image.id || index}
                    onClick={() => setSelectedImageIndex(index)}
                    className={`flex-shrink-0 w-20 h-20 rounded-lg border-2 overflow-hidden transition-all ${
                      selectedImageIndex === index
                        ? 'border-amber-600 ring-2 ring-amber-400'
                        : 'border-amber-200 hover:border-amber-400'
                    }`}
                  >
                    <ImageWithFallback
                      src={image.thumbnailUrl || image.url || ''}
                      alt={`${service.name} - ${index + 1}`}
                      className="w-full h-full object-cover"
                    />
                  </button>
                ))}
              </div>
            )}
          </div>

          {/* Details */}
          <div className="space-y-6">
            {/* Title & Category */}
            <div>
              <h1 className="text-3xl md:text-4xl text-amber-900 mb-3">
                {service.name}
              </h1>
              {service.categoryName && (
                <Badge className="bg-amber-100 text-amber-900 mb-4">
                  {service.categoryName}
                </Badge>
              )}
            </div>

            {/* Rating & Stats */}
            <div className="space-y-3">
              <div className="flex items-center gap-2">
                <div className="flex">
                  {[...Array(5)].map((_, i) => (
                    <Star
                      key={i}
                      className={`w-5 h-5 ${
                        i < Math.floor(service.averageRating || 0)
                          ? 'fill-yellow-400 text-yellow-400'
                          : 'text-gray-300'
                      }`}
                    />
                  ))}
                </div>
                <span className="text-gray-600">
                  ({(service.averageRating || 0).toFixed(1)}) · {service.reviewCount || 0} đánh giá
                </span>
              </div>
              
              {/* Stats Row */}
              <div className="flex gap-4 text-sm">
                {service.viewCount !== undefined && (
                  <div className="flex items-center gap-1 text-gray-600">
                    <Eye className="w-4 h-4" />
                    <span>{service.viewCount} lượt xem</span>
                  </div>
                )}
                {service.favoriteCount !== undefined && (
                  <div className="flex items-center gap-1 text-gray-600">
                    <Heart className="w-4 h-4" />
                    <span>{service.favoriteCount} yêu thích</span>
                  </div>
                )}
              </div>
            </div>

            {/* Tags */}
            {service.tags && service.tags.length > 0 && (
              <div>
                <p className="text-sm text-gray-600 mb-2">Thẻ:</p>
                <div className="flex flex-wrap gap-2">
                  {service.tags.map((tag, index) => (
                    <Badge key={index} variant="outline" className="text-amber-700 border-amber-300">
                      {tag}
                    </Badge>
                  ))}
                </div>
              </div>
            )}

            {/* Description */}
            <div>
              <h3 className="text-lg text-amber-900 mb-2">Mô tả dịch vụ</h3>
              <p className="text-gray-600 leading-relaxed">
                {service.description || 'Không có mô tả chi tiết'}
              </p>
            </div>

            {/* Service Details */}
            <Card className="border-2 border-amber-200 bg-amber-50">
              <CardContent className="p-6 space-y-4">
                {service.categoryName && (
                  <div className="flex items-start gap-3">
                    <CheckCircle2 className="w-5 h-5 text-amber-600 mt-1 flex-shrink-0" />
                    <div>
                      <p className="text-sm text-gray-600">Danh mục</p>
                      <p className="text-amber-900 font-semibold">
                        {service.categoryName}
                      </p>
                    </div>
                  </div>
                )}
                {service.storeName && (
                  <div className="flex items-start gap-3">
                    <CheckCircle2 className="w-5 h-5 text-amber-600 mt-1 flex-shrink-0" />
                    <div>
                      <p className="text-sm text-gray-600">Cửa hàng</p>
                      <p className="text-amber-900 font-semibold">
                        {service.storeName}
                      </p>
                    </div>
                  </div>
                )}
                {service.sku && (
                  <div className="flex items-start gap-3">
                    <CheckCircle2 className="w-5 h-5 text-amber-600 mt-1 flex-shrink-0" />
                    <div>
                      <p className="text-sm text-gray-600">Mã SKU</p>
                      <p className="text-amber-900 font-semibold">
                        {service.sku}
                      </p>
                    </div>
                  </div>
                )}
                {service.stockQuantity && (
                  <div className="flex items-start gap-3">
                    <CheckCircle2 className="w-5 h-5 text-green-600 mt-1 flex-shrink-0" />
                    <div>
                      <p className="text-sm text-gray-600">Còn lại</p>
                      <p className="text-amber-900 font-semibold">
                        {service.stockQuantity} dịch vụ
                      </p>
                    </div>
                  </div>
                )}
                {service.reviewCount !== undefined && (
                  <div className="flex items-start gap-3">
                    <Star className="w-5 h-5 text-yellow-500 mt-1 flex-shrink-0 fill-yellow-500" />
                    <div>
                      <p className="text-sm text-gray-600">Số đánh giá</p>
                      <p className="text-amber-900 font-semibold">
                        {service.reviewCount} đánh giá
                      </p>
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Price & CTA */}
            <div className="space-y-4 pt-4 border-t-2 border-amber-200">
              <div>
                <p className="text-sm text-gray-600 mb-1">Giá dịch vụ</p>
                <p className="text-3xl text-red-600 font-bold">
                  {service.price && service.price > 0
                    ? new Intl.NumberFormat('vi-VN').format(service.price) + '₫'
                    : 'Liên hệ để biết giá'}
                </p>
              </div>
              <Button
                size="lg"
                className="w-full bg-red-600 hover:bg-red-700 text-white py-6 text-lg"
                onClick={() => {
                  // Set the service product ID in context
                  if (service.id) {
                    setSelectedServiceProductId(service.id);
                    // Navigate to calendar
                    navigate('/calendar');
                  }
                }}
              >
                Đặt dịch vụ ngay
              </Button>
              <Button
                size="lg"
                variant="outline"
                className="w-full border-amber-300 text-amber-700 hover:bg-amber-50 py-6"
                onClick={() => navigate('/contact')}
              >
                Liên hệ tư vấn
              </Button>
            </div>
          </div>
        </div>

        {/* Related Services Carousel */}
        {relatedServices.length > 0 && (
          <div className="mt-16 pt-8 border-t-2 border-amber-200">
            <h2 className="text-2xl text-amber-900 mb-6">Các dịch vụ khác</h2>
            
            <div className="relative">
              {/* Carousel Container */}
              <div className="overflow-hidden">
                <div
                  className="flex transition-transform duration-500 ease-out"
                  style={{
                    transform: `translateX(-${carouselIndex * 100}%)`,
                  }}
                >
                  {relatedServices.map((relService) => (
                    <div
                      key={relService.id}
                      className="min-w-full px-2"
                    >
                      <Card
                        className="border-2 border-amber-200 hover:shadow-lg transition-shadow cursor-pointer h-full"
                        onClick={() => navigate(`/services/${relService.id}`)}
                      >
                        <div className="relative overflow-hidden">
                          <ImageWithFallback
                            src={relService.primaryImage || 'https://images.unsplash.com/photo-1588358581442-c0a340052b75?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwdGVtcGxlJTIwcHJheWVyJTIwY2VyZW1vbnl8ZW58MXx8fHwxNzU3Njc0NDMwfDA&ixlib=rb-4.1.0&q=80&w=1080'}
                            alt={relService.name || 'Dịch vụ'}
                            className="w-full h-48 object-cover"
                          />
                          {relService.isFeatured && (
                            <Badge className="absolute top-2 right-2 bg-red-600 text-white">
                              Nổi bật
                            </Badge>
                          )}
                        </div>
                        <CardContent className="p-4">
                          <h3 className="text-lg text-amber-900 font-semibold mb-2 line-clamp-2">
                            {relService.name}
                          </h3>
                          <div className="flex items-center gap-1 mb-3">
                            {[...Array(5)].map((_, i) => (
                              <Star
                                key={i}
                                className={`w-4 h-4 ${
                                  i < Math.floor(relService.serviceRating || 0)
                                    ? 'fill-yellow-400 text-yellow-400'
                                    : 'text-gray-300'
                                }`}
                              />
                            ))}
                            <span className="text-xs text-gray-600 ml-1">
                              ({(relService.serviceRating || 0).toFixed(1)})
                            </span>
                          </div>
                          <p className="text-sm text-gray-600 mb-3 line-clamp-2">
                            {relService.shortDescription}
                          </p>
                          <div className="flex items-center justify-between">
                            <span className="text-lg text-red-600 font-bold">
                              {relService.price && relService.price > 0
                                ? new Intl.NumberFormat('vi-VN').format(relService.price) + '₫'
                                : 'Liên hệ'}
                            </span>
                            <Badge variant="outline" className="text-amber-700 border-amber-300">
                              {relService.serviceDuration}
                            </Badge>
                          </div>
                        </CardContent>
                      </Card>
                    </div>
                  ))}
                </div>
              </div>

              {/* Navigation Buttons */}
              {relatedServices.length > 1 && (
                <>
                  <button
                    onClick={() =>
                      setCarouselIndex((prev) =>
                        prev === 0 ? relatedServices.length - 1 : prev - 1
                      )
                    }
                    className="absolute left-0 top-1/2 -translate-y-1/2 -translate-x-4 md:-translate-x-6 bg-amber-600 hover:bg-amber-700 text-white rounded-full p-2 transition-colors z-10"
                  >
                    <ChevronLeft className="w-6 h-6" />
                  </button>
                  <button
                    onClick={() =>
                      setCarouselIndex((prev) =>
                        prev === relatedServices.length - 1 ? 0 : prev + 1
                      )
                    }
                    className="absolute right-0 top-1/2 -translate-y-1/2 translate-x-4 md:translate-x-6 bg-amber-600 hover:bg-amber-700 text-white rounded-full p-2 transition-colors z-10"
                  >
                    <ChevronRight className="w-6 h-6" />
                  </button>
                </>
              )}

              {/* Dots Indicator */}
              {relatedServices.length > 1 && (
                <div className="flex justify-center gap-2 mt-4">
                  {relatedServices.map((_, index) => (
                    <button
                      key={index}
                      onClick={() => setCarouselIndex(index)}
                      className={`w-2 h-2 rounded-full transition-colors ${
                        index === carouselIndex
                          ? 'bg-amber-600'
                          : 'bg-amber-200'
                      }`}
                    />
                  ))}
                </div>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

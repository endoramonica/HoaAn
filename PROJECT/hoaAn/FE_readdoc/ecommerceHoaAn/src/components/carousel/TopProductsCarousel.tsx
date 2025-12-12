import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTopProductsByCategory, type MappedProduct } from '../../lib/hooks/useProductsByCategory';
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselPrevious,
  CarouselNext,
} from '../ui/carousel';
import { Button } from '../ui/button';
import { Skeleton } from '../ui/skeleton';
import { ImageWithFallback } from '../figma/ImageWithFallback';
import { Star, ShoppingCart, ArrowRight } from 'lucide-react';

interface TopProductsCarouselProps {
  categoryId: string;
  categoryName: string;
  limit?: number;
}

export function TopProductsCarousel({
  categoryId,
  categoryName,
  limit = 5,
}: TopProductsCarouselProps) {
  const navigate = useNavigate();
  const { data: products, isLoading } = useTopProductsByCategory(categoryId, limit);
  const [api, setApi] = useState<any>(null);
  const [current, setCurrent] = useState(0);
  const [count, setCount] = useState(0);

  useEffect(() => {
    if (!api) return;

    setCount(api.scrollSnapList().length);
    setCurrent(api.selectedScrollSnap());

    api.on('select', () => {
      setCurrent(api.selectedScrollSnap());
    });
  }, [api]);

  if (isLoading) {
    return (
      <div className="w-full py-8">
        <div className="max-w-6xl mx-auto px-4">
          <Skeleton className="h-8 w-64 mb-6" />
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
            {Array.from({ length: 5 }).map((_, i) => (
              <Skeleton key={i} className="h-64 rounded-lg" />
            ))}
          </div>
        </div>
      </div>
    );
  }

  if (!products || products.length === 0) {
    return null;
  }

  return (
    <div className="w-full bg-gradient-to-br from-white to-amber-50 py-12">
      <div className="max-w-6xl mx-auto px-4">
        {/* Header */}
        <div className="flex items-center justify-between mb-8">
          <div>
            <h3 className="text-3xl font-bold text-amber-900 mb-2">
              Top sản phẩm {categoryName}
            </h3>
            <p className="text-gray-600">
              Những sản phẩm bán chạy nhất trong danh mục này
            </p>
          </div>
          <Button
            onClick={() => navigate(`/products?category=${categoryId}`)}
            className="bg-red-600 hover:bg-red-700 text-white flex items-center gap-2"
          >
            Xem tất cả
            <ArrowRight className="w-4 h-4" />
          </Button>
        </div>

        {/* Carousel */}
        <Carousel
          setApi={setApi}
          opts={{
            align: 'start',
            loop: false,
          }}
          className="w-full"
        >
          <CarouselContent className="-ml-2 md:-ml-4">
            {products.map((product: MappedProduct) => (
              <CarouselItem
                key={product.id}
                className="pl-2 md:pl-4 basis-full sm:basis-1/2 md:basis-1/3 lg:basis-1/4 xl:basis-1/5"
              >
                {/* Product Card */}
                <div
                  className="group cursor-pointer h-full min-h-[320px] bg-white rounded-lg border-2 border-transparent hover:border-red-400 hover:shadow-xl hover:-translate-y-2 transition-all duration-300 ease-in-out overflow-hidden flex flex-col"
                  onClick={() => navigate(`/product/${product.id}`)}
                >
                  {/* Image Container */}
                  <div className="relative h-48 overflow-hidden bg-gray-100 rounded-t-lg flex-shrink-0">
                    <ImageWithFallback
                      src={product.imageUrl}
                      alt={product.name}
                      className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-300 ease-out"
                    />

                    {/* Discount Badge */}
                    {product.discount > 0 && (
                      <div className="absolute top-3 right-3 bg-red-600 text-white text-xs font-medium rounded-md px-2 py-1 shadow-md">
                        -{product.discount}%
                      </div>
                    )}

                    {/* Stock Status Overlay */}
                    {!product.inStock && (
                      <div className="absolute inset-0 bg-black/50 flex items-center justify-center rounded-t-lg">
                        <span className="text-white text-sm font-semibold">Hết hàng</span>
                      </div>
                    )}
                  </div>

                  {/* Content Section */}
                  <div className="p-4 flex flex-col flex-grow justify-between">
                    {/* Product Name */}
                    <h4 className="text-sm font-semibold text-amber-900 leading-tight line-clamp-2 mb-2 group-hover:text-red-600 transition-colors duration-200">
                      {product.name}
                    </h4>

                    {/* Rating Section */}
                    {product.rating > 0 && (
                      <div className="flex items-center gap-1 mb-3">
                        <div className="flex items-center gap-0.5">
                          {[...Array(5)].map((_, i) => (
                            <Star
                              key={i}
                              className={`w-3 h-3 ${
                                i < Math.floor(product.rating)
                                  ? 'fill-yellow-400 text-yellow-400'
                                  : 'text-gray-300'
                              }`}
                            />
                          ))}
                        </div>
                        <span className="text-xs text-gray-500 ml-1">
                          ({product.reviewCount})
                        </span>
                      </div>
                    )}

                    {/* Price Section */}
                    <div className="mb-4">
                      <div className="flex items-center gap-2">
                        <span className="text-lg font-bold text-red-600">
                          {Math.round(product.price).toLocaleString('vi-VN')}₫
                        </span>
                        {product.originalPrice > product.price && (
                          <span className="text-xs text-gray-500 line-through ml-1">
                            {Math.round(product.originalPrice).toLocaleString('vi-VN')}₫
                          </span>
                        )}
                      </div>
                    </div>

                    {/* Add to Cart Button */}
                    <Button
                      className={`w-full bg-red-600 hover:bg-red-700 text-white py-2 text-sm font-medium flex items-center justify-center gap-2 rounded-md shadow-sm hover:shadow-lg transition-all duration-300 ${
                        !product.inStock ? 'opacity-50 cursor-not-allowed' : ''
                      }`}
                      disabled={!product.inStock}
                      onClick={(e) => {
                        e.stopPropagation();
                        // TODO: Add to cart logic
                      }}
                    >
                      <ShoppingCart className="w-4 h-4" />
                      Thêm vào giỏ
                    </Button>
                  </div>
                </div>
              </CarouselItem>
            ))}
          </CarouselContent>

          {/* Navigation Buttons */}
          <CarouselPrevious className="bg-white/80 backdrop-blur border-amber-300 text-amber-700 hover:bg-amber-100" />
          <CarouselNext className="bg-white/80 backdrop-blur border-amber-300 text-amber-700 hover:bg-amber-100" />
        </Carousel>

        {/* Slide Indicators */}
        {count > 0 && (
          <div className="flex justify-center gap-2 mt-6">
            {Array.from({ length: count }).map((_, index) => (
              <button
                key={index}
                onClick={() => api?.scrollTo(index)}
                className={`w-2 h-2 rounded-full transition-all duration-300 ${
                  index === current
                    ? 'bg-amber-600 w-8'
                    : 'bg-gray-300 hover:bg-gray-400'
                }`}
                aria-label={`Go to slide ${index + 1}`}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

export default TopProductsCarousel;

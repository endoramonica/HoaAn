import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { useActiveCategories } from '../../lib/hooks/useCategories';
import { Flower2 } from 'lucide-react';
import { Skeleton } from '../ui/skeleton';
import { Card, CardContent } from '../ui/card';
import './styles/category-carousel.css';

interface CategoryCarouselProps {
  onCategorySelect?: (categoryId: string, categoryName: string) => void;
}

export function CategoryCarousel({ onCategorySelect }: CategoryCarouselProps) {
  const navigate = useNavigate();
  const [activeCategory, setActiveCategory] = useState<string | null>(null);
  const [isPaused, setIsPaused] = useState(false);
  const carouselRef = useRef<HTMLDivElement>(null);
  const { data: categories, isLoading } = useActiveCategories();

  // Duplicate categories 4x for infinite loop effect
  const duplicatedCategories = categories ? [...categories, ...categories, ...categories, ...categories] : [];

  // Auto-scroll animation
  useEffect(() => {
    if (!carouselRef.current || isPaused || isLoading) return;

    const carousel = carouselRef.current;
    const scrollSpeed = 1; // pixels per frame
    let animationId: number;

    const animate = () => {
      carousel.scrollLeft += scrollSpeed;

      // Reset to beginning when reaching end
      if (carousel.scrollLeft >= carousel.scrollWidth / 2) {
        carousel.scrollLeft = 0;
      }

      animationId = requestAnimationFrame(animate);
    };

    animationId = requestAnimationFrame(animate);

    return () => cancelAnimationFrame(animationId);
  }, [isPaused, isLoading]);

  const handleCategoryClick = (categoryId: string, categoryName: string) => {
    setActiveCategory(categoryId);
    onCategorySelect?.(categoryId, categoryName);
  };

  if (isLoading) {
    return (
      <div className="w-full bg-gradient-to-r from-amber-50 to-yellow-50 py-8">
        <div className="max-w-6xl mx-auto px-4">
          <div className="flex gap-4 overflow-x-auto pb-4">
            {Array.from({ length: 6 }).map((_, i) => (
              <Skeleton key={i} className="w-32 h-32 rounded-lg flex-shrink-0" />
            ))}
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="w-full bg-gradient-to-r from-amber-50 to-yellow-50 py-8">
      <div className="max-w-6xl mx-auto px-4">
        <div className="mb-6">
          <h3 className="text-2xl font-bold text-amber-900 mb-2">Danh mục sản phẩm</h3>
          <p className="text-gray-600">Chọn danh mục để xem sản phẩm</p>
        </div>

        {/* Carousel Container */}
        <div
          ref={carouselRef}
          className="category-carousel-container"
          onMouseEnter={() => setIsPaused(true)}
          onMouseLeave={() => setIsPaused(false)}
          onTouchStart={() => setIsPaused(true)}
          onTouchEnd={() => setIsPaused(false)}
        >
          {duplicatedCategories.map((category, index) => (
            <div
              key={`${category.id}-${index}`}
              className="category-carousel-item"
              onClick={() => handleCategoryClick(category.id, category.name)}
            >
              <Card
  className={`flex-shrink-0 mx-2 px-6 py-2 rounded-full text-sm font-medium transition-all duration-200 whitespace-nowrap ${
    activeCategory === category.id
      ? "border-yellow-600  shadow-lg scale-105"
      : "border-yellow-300 bg-white text-amber-900 hover:border-yellow-500 hover:bg-amber-100 hover:shadow-md"
  }`}
>
  
    {/* icon */}
   
          <h4
            className={` text-sm font-semibold line-clamp-2 rounded-lg transition-all duration-200 ${
              activeCategory === category.id
                ? " bg-transparent text-amber-900 font-bold shadow-sm"
                : " bg-transparent text-gray-700 hover:text-amber-800"
            }`}
          >
            {category.name}
          </h4>
        
 
   
</Card>

            </div>
          ))}
        </div>

        {/* Pause Indicator */}
        {isPaused}
      </div>
    </div>
  );
}

export default CategoryCarousel;

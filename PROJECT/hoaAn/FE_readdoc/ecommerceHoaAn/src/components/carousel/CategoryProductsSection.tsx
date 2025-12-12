import { TopProductsCarousel } from './TopProductsCarousel';

interface CategoryProductsSectionProps {
  categoryId: string;
  categoryName: string;
}

export function CategoryProductsSection({ categoryId, categoryName }: CategoryProductsSectionProps) {
  return (
    <TopProductsCarousel
      categoryId={categoryId}
      categoryName={categoryName}
      limit={5}
    />
  );
}

export default CategoryProductsSection;

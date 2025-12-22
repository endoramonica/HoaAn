/**
 * Ritual Recommendation Component
 * Displays ritual recommendations with cultural explanations and missing items
 */

import React, { useState, useCallback } from 'react';
import { X, AlertCircle, Loader2, Star } from 'lucide-react';
import { RecommendationPayload, ExplanationPayload } from '@/lib/services/recommendationService';
import { Product } from '@/lib/services/recommendationService';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from '@/components/ui/carousel';

// ============================================================================
// Types
// ============================================================================

export interface RitualRecommendationProps {
  recommendation: RecommendationPayload | null;
  explanation: ExplanationPayload | null;
  isLoading: boolean;
  error: string | null;
  onDismiss: () => void;
  onDisableRitual: () => void;
  onAddToCart: (productId: string) => void;
}

// ============================================================================
// Component
// ============================================================================

export const RitualRecommendation: React.FC<RitualRecommendationProps> = ({
  recommendation,
  explanation,
  isLoading,
  error,
  onDismiss,
  onDisableRitual,
  onAddToCart,
}) => {
  const [dismissed, setDismissed] = useState(false);

  // Handle dismiss
  const handleDismiss = useCallback(() => {
    setDismissed(true);
    onDismiss();
  }, [onDismiss]);

  // Handle disable ritual
  const handleDisableRitual = useCallback(() => {
    setDismissed(true);
    onDisableRitual();
  }, [onDisableRitual]);

  // Handle add to cart
  const handleAddToCart = useCallback(
    (productId: string) => {
      onAddToCart(productId);
    },
    [onAddToCart]
  );

  // Don't show if dismissed
  if (dismissed) {
    return null;
  }

  // Show loading state
  if (isLoading) {
    return (
      <Card className="w-full border-amber-200 bg-amber-50">
        <CardContent className="flex items-center justify-center py-8">
          <Loader2 className="h-6 w-6 animate-spin text-amber-600" />
          <span className="ml-2 text-amber-700">Đang phân tích nghi thức...</span>
        </CardContent>
      </Card>
    );
  }

  // Show error state (silent fail - don't show error to user)
  if (error) {
    return null;
  }

  // Show empty state if no recommendation
  if (!recommendation || !recommendation.matched) {
    return null;
  }

  // Show recommendation
  return (
    <Card className="w-full border-amber-200 bg-gradient-to-br from-amber-50 to-orange-50">
      <CardHeader className="pb-3">
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <div className="flex items-center gap-2">
              <CardTitle className="text-lg text-amber-900">
                {recommendation.ritualName || 'Nghi thức Việt Nam'}
              </CardTitle>
              {recommendation.confidenceScore && (
                <div className="flex items-center gap-1 rounded-full bg-amber-200 px-2 py-1">
                  <Star className="h-4 w-4 fill-amber-600 text-amber-600" />
                  <span className="text-xs font-semibold text-amber-900">
                    {(recommendation.confidenceScore * 100).toFixed(0)}%
                  </span>
                </div>
              )}
            </div>
            {explanation && (
              <CardDescription className="mt-2 text-sm text-amber-800">
                {explanation.culturalContext}
              </CardDescription>
            )}
          </div>
          <button
            onClick={handleDismiss}
            className="ml-2 rounded-lg p-1 hover:bg-amber-200 transition-colors"
            aria-label="Đóng"
          >
            <X className="h-5 w-5 text-amber-700" />
          </button>
        </div>
      </CardHeader>

      <CardContent className="space-y-4">
        {/* Missing Items Carousel */}
        {recommendation.missingItems && recommendation.missingItems.length > 0 && (
          <div className="space-y-2">
            <h3 className="text-sm font-semibold text-amber-900">
              Các sản phẩm cần thiết cho nghi thức:
            </h3>
            <Carousel className="w-full">
              <CarouselContent>
                {recommendation.missingItems.map((item) => (
                  <CarouselItem key={item.id} className="basis-1/2 md:basis-1/3 lg:basis-1/4">
                    <RitualItemCard
                      item={item}
                      explanation={
                        explanation?.itemExplanations.find((e) => e.productId === item.id)
                      }
                      onAddToCart={handleAddToCart}
                    />
                  </CarouselItem>
                ))}
              </CarouselContent>
              {recommendation.missingItems.length > 1 && (
                <>
                  <CarouselPrevious className="left-0" />
                  <CarouselNext className="right-0" />
                </>
              )}
            </Carousel>
          </div>
        )}

        {/* Sources */}
        {explanation?.sources && explanation.sources.length > 0 && (
          <div className="border-t border-amber-200 pt-3">
            <p className="text-xs text-amber-700">
              <span className="font-semibold">Nguồn:</span> {explanation.sources.join(', ')}
            </p>
          </div>
        )}

        {/* Action Buttons */}
        <div className="flex gap-2 border-t border-amber-200 pt-3">
          <Button
            variant="outline"
            size="sm"
            onClick={handleDismiss}
            className="flex-1 text-amber-700 hover:bg-amber-100"
          >
            Bỏ qua
          </Button>
          <Button
            variant="outline"
            size="sm"
            onClick={handleDisableRitual}
            className="flex-1 text-amber-700 hover:bg-amber-100"
          >
            Không hiển thị lại
          </Button>
        </div>
      </CardContent>
    </Card>
  );
};

// ============================================================================
// Item Card Component
// ============================================================================

interface RitualItemCardProps {
  item: Product;
  explanation?: any;
  onAddToCart: (productId: string) => void;
}

const RitualItemCard: React.FC<RitualItemCardProps> = ({
  item,
  explanation,
  onAddToCart,
}) => {
  return (
    <Card className="h-full border-amber-100 bg-white hover:shadow-md transition-shadow">
      <CardContent className="p-3 space-y-2">
        {/* Image */}
        {item.image && (
          <div className="aspect-square overflow-hidden rounded-lg bg-amber-100">
            <img
              src={item.image}
              alt={item.name}
              className="h-full w-full object-cover"
            />
          </div>
        )}

        {/* Name */}
        <h4 className="text-sm font-semibold text-amber-900 line-clamp-2">
          {item.name}
        </h4>

        {/* Price */}
        {item.price && (
          <p className="text-sm font-bold text-amber-700">
            {new Intl.NumberFormat('vi-VN', {
              style: 'currency',
              currency: 'VND',
            }).format(item.price)}
          </p>
        )}

        {/* Explanation */}
        {explanation && (
          <div className="space-y-1 border-t border-amber-100 pt-2">
            <p className="text-xs text-amber-800">
              <span className="font-semibold">Tại sao cần:</span> {explanation.whyNeeded}
            </p>
            <p className="text-xs text-amber-700">
              <span className="font-semibold">Sử dụng:</span> {explanation.traditionalUsage}
            </p>
          </div>
        )}

        {/* Add to Cart Button */}
        <Button
          size="sm"
          onClick={() => onAddToCart(item.id)}
          className="w-full bg-amber-600 hover:bg-amber-700 text-white"
        >
          Thêm vào giỏ
        </Button>
      </CardContent>
    </Card>
  );
};

export default RitualRecommendation;

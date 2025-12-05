import React from 'react';

interface LoadingSkeletonProps {
  variant?: 'text' | 'circular' | 'rectangular';
  width?: string | number;
  height?: string | number;
  className?: string;
  count?: number;
}

export const LoadingSkeleton: React.FC<LoadingSkeletonProps> = ({
  variant = 'text',
  width,
  height,
  className = '',
  count = 1,
}) => {
  const baseStyles = 'animate-pulse bg-gray-200';
  
  const variantStyles = {
    text: 'rounded h-4',
    circular: 'rounded-full',
    rectangular: 'rounded-lg',
  };
  
  const style: React.CSSProperties = {
    width: width || (variant === 'text' ? '100%' : undefined),
    height: height || (variant === 'circular' ? width : undefined),
  };
  
  const skeletons = Array.from({ length: count }, (_, index) => (
    <div
      key={index}
      className={`${baseStyles} ${variantStyles[variant]} ${className}`}
      style={style}
    />
  ));
  
  if (count === 1) {
    return skeletons[0];
  }
  
  return (
    <div className="space-y-3">
      {skeletons}
    </div>
  );
};

import { useState, useEffect, useMemo } from 'react';
import { Button } from './ui/button';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Skeleton } from './ui/skeleton';
import { PaginationNavigation } from './ui/pagination-navigation';
import { QuickViewModal } from './QuickViewModal';
import { BackToTop } from './ui/back-to-top';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { 
  Flower2, 
  Star, 
  Filter,
  Grid3x3,
  List,
  Search,
  ShoppingCart,
  Heart,
  Eye,
  ChevronLeft,
  ChevronRight,
  Loader2
} from 'lucide-react';

type SortOption = 'newest' | 'price-low' | 'price-high' | 'rating' | 'popular';

export function ProductsPage() {
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [currentPage, setCurrentPage] = useState(1);
  const [searchQuery, setSearchQuery] = useState('');
  const [sortBy, setSortBy] = useState<SortOption>('newest');
  const [isLoading, setIsLoading] = useState(false);
  const [itemsPerPage, setItemsPerPage] = useState(12);
  const [selectedProduct, setSelectedProduct] = useState<any>(null);
  const [isQuickViewOpen, setIsQuickViewOpen] = useState(false);

  const categories = [
    { id: 'all', name: 'Tất cả', count: 156 },
    { id: 'incense', name: 'Hương', count: 50 },
    { id: 'candles', name: 'Nến', count: 30 },
    { id: 'fruits', name: 'Hoa quả', count: 20 },
    { id: 'sets', name: 'Mâm cúng', count: 15 },
    { id: 'paper', name: 'Giấy tiền', count: 25 },
    { id: 'services', name: 'Dịch vụ', count: 16 }
  ];

  // Extended product data for pagination demo
  const allProducts = useMemo(() => {
    const baseProducts = [
      {
        id: 1,
        name: 'Mâm Cúng Trọn Gói Cao Cấp',
        price: 2890000,
        originalPrice: 3490000,
        discount: 17,
        rating: 4.8,
        reviews: 127,
        image: 'https://images.unsplash.com/photo-1519097000072-e44ffa116485?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwb2ZmZXJpbmdzJTIwYWx0YXIlMjBmcnVpdHN8ZW58MXx8fHwxNzU3Njc0NDI5fDA&ixlib=rb-4.1.0&q=80&w=1080',
        category: 'sets',
        featured: true,
        createdAt: '2024-01-15'
      },
      {
        id: 2,
        name: 'Hương Trầm Cao Cấp - Hộp 100 Cây',
        price: 450000,
        originalPrice: 520000,
        discount: 13,
        rating: 4.9,
        reviews: 89,
        image: 'https://images.unsplash.com/photo-1532334722716-c5850cdd878d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwaW5jZW5zZSUyMGNlcmVtb255JTIwdHJhZGl0aW9uYWx8ZW58MXx8fHwxNzU3Njc0NDI4fDA&ixlib=rb-4.1.0&q=80&w=1080',
        category: 'incense',
        featured: false,
        createdAt: '2024-01-14'
      },
      {
        id: 3,
        name: 'Nến Đỏ Phong Thủy - Bộ 12 Cây',
        price: 280000,
        originalPrice: 350000,
        discount: 20,
        rating: 4.7,
        reviews: 156,
        image: 'https://images.unsplash.com/photo-1732117924212-39bfaec174c9?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx0cmFkaXRpb25hbCUyMGNhbmRsZXMlMjByZWQlMjBnb2xkfGVufDF8fHx8MTc1NzY3NDQyOXww&ixlib=rb-4.1.0&q=80&w=1080',
        category: 'candles',
        featured: false,
        createdAt: '2024-01-13'
      },
      {
        id: 4,
        name: 'Ngũ Quả Tươi Cao Cấp',
        price: 680000,
        originalPrice: null,
        discount: 0,
        rating: 4.6,
        reviews: 203,
        image: 'https://images.unsplash.com/photo-1519097000072-e44ffa116485?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwb2ZmZXJpbmdzJTIwYWx0YXIlMjBmcnVpdHN8ZW58MXx8fHwxNzU3Njc0NDI5fDA&ixlib=rb-4.1.0&q=80&w=1080',
        category: 'fruits',
        featured: false,
        createdAt: '2024-01-12'
      },
      {
        id: 5,
        name: 'Giấy Tiền Vàng Cao Cấp - Combo 10 Tờ',
        price: 320000,
        originalPrice: 380000,
        discount: 16,
        rating: 4.8,
        reviews: 134,
        image: 'https://images.unsplash.com/photo-1588358581442-c0a340052b75?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwdGVtcGxlJTIwcHJheWVyJTIwY2VyZW1vbnl8ZW58MXx8fHwxNzU3Njc0NDMwfDA&ixlib=rb-4.1.0&q=80&w=1080',
        category: 'paper',
        featured: false,
        createdAt: '2024-01-11'
      },
      {
        id: 6,
        name: 'Dịch Vụ Cúng Gia Tiên Trọn Gói',
        price: 1250000,
        originalPrice: null,
        discount: 0,
        rating: 4.9,
        reviews: 67,
        image: 'https://images.unsplash.com/photo-1573460630303-81cbaf895c54?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsb3R1cyUyMGZsb3dlciUyMGNlcmVtb25pYWwlMjBnb2xkfGVufDF8fHx8MTc1NzY3NDQyOXww&ixlib=rb-4.1.0&q=80&w=1080',
        category: 'services',
        featured: true,
        createdAt: '2024-01-10'
      }
    ];

    // Generate more products for pagination demo
    const additionalProducts = [];
    const names = [
      'Hương Nụ Cao Cấp', 'Nến Thơm Thiên Nhiên', 'Bánh Tét Cúng', 'Hoa Sen Tươi',
      'Giấy Tiền Bạc', 'Cơm Chay Truyền Thống', 'Trái Cây Thập Cẩm', 'Hương Loại 1',
      'Nến Phụng', 'Bánh In Cúng', 'Hoa Ly Trắng', 'Mứt Tết Cúng',
      'Xôi Gấc', 'Bánh Chưng', 'Hương Trầm Ấn Độ', 'Nến Đỏ Cẩm Thạch'
    ];
    const categories = ['incense', 'candles', 'fruits', 'sets', 'paper', 'services'];
    
    for (let i = 7; i <= 48; i++) {
      additionalProducts.push({
        id: i,
        name: `${names[(i - 7) % names.length]} - Phiên bản ${i}`,
        price: Math.floor(Math.random() * 2000000) + 100000,
        originalPrice: Math.random() > 0.6 ? Math.floor(Math.random() * 500000) + 100000 : null,
        discount: Math.random() > 0.6 ? Math.floor(Math.random() * 25) + 5 : 0,
        rating: Math.round((Math.random() * 1.5 + 3.5) * 10) / 10,
        reviews: Math.floor(Math.random() * 200) + 10,
        image: baseProducts[i % 6].image,
        category: categories[i % categories.length],
        featured: Math.random() > 0.8,
        createdAt: `2024-01-${String(Math.floor(Math.random() * 28) + 1).padStart(2, '0')}`
      });
    }

    return [...baseProducts, ...additionalProducts];
  }, []);

  // Filter and sort products
  const filteredAndSortedProducts = useMemo(() => {
    let filtered = allProducts;

    // Filter by category
    if (selectedCategory !== 'all') {
      filtered = filtered.filter(product => product.category === selectedCategory);
    }

    // Filter by search query
    if (searchQuery.trim()) {
      filtered = filtered.filter(product =>
        product.name.toLowerCase().includes(searchQuery.toLowerCase())
      );
    }

    // Sort products
    filtered = [...filtered].sort((a, b) => {
      switch (sortBy) {
        case 'price-low':
          return a.price - b.price;
        case 'price-high':
          return b.price - a.price;
        case 'rating':
          return b.rating - a.rating;
        case 'popular':
          return b.reviews - a.reviews;
        case 'newest':
        default:
          return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
      }
    });

    return filtered;
  }, [allProducts, selectedCategory, searchQuery, sortBy]);

  // Calculate pagination
  const totalProducts = filteredAndSortedProducts.length;
  const totalPages = Math.ceil(totalProducts / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const currentProducts = filteredAndSortedProducts.slice(startIndex, endIndex);

  // Update categories count based on filtered data
  const categoriesWithCount = useMemo(() => {
    return categories.map(category => ({
      ...category,
      count: category.id === 'all' 
        ? allProducts.length 
        : allProducts.filter(p => p.category === category.id).length
    }));
  }, [allProducts]);

  // Simulate loading when filters change
  useEffect(() => {
    setIsLoading(true);
    const timer = setTimeout(() => {
      setIsLoading(false);
    }, 300);
    return () => clearTimeout(timer);
  }, [selectedCategory, searchQuery, sortBy, currentPage]);

  // Reset to first page when filters change
  useEffect(() => {
    setCurrentPage(1);
  }, [selectedCategory, searchQuery, sortBy]);

  // Scroll to top when page changes
  useEffect(() => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }, [currentPage]);

  const handleQuickView = (product: any) => {
    setSelectedProduct(product);
    setIsQuickViewOpen(true);
  };

  const closeQuickView = () => {
    setIsQuickViewOpen(false);
    setSelectedProduct(null);
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('vi-VN').format(price) + '₫';
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50">
      {/* Header */}
      <section className="bg-gradient-to-r from-amber-900 to-red-800 text-white py-16">
        <div className="max-w-6xl mx-auto px-4 text-center">
          <Flower2 className="w-16 h-16 mx-auto mb-4 text-yellow-300" />
          <h1 className="text-4xl md:text-5xl mb-4">Sản phẩm của chúng tôi</h1>
          <p className="text-xl text-yellow-100 max-w-2xl mx-auto">
            Khám phá bộ sưu tập đầy đủ các sản phẩm đồ cúng chất lượng cao
          </p>
        </div>
      </section>

      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="flex flex-col lg:flex-row gap-8">
          {/* Sidebar Filters */}
          <div className="lg:w-64 flex-shrink-0">
            <Card className="sticky top-24">
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-amber-900">
                  <Filter className="w-5 h-5" />
                  Danh mục sản phẩm
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-2">
                {categoriesWithCount.map((category) => (
                  <button
                    key={category.id}
                    onClick={() => setSelectedCategory(category.id)}
                    className={`w-full flex items-center justify-between p-3 rounded-lg transition-colors ${
                      selectedCategory === category.id
                        ? 'bg-amber-100 text-amber-900 border-2 border-amber-300'
                        : 'hover:bg-amber-50 text-gray-700'
                    }`}
                  >
                    <span>{category.name}</span>
                    <Badge variant="secondary" className="bg-amber-200 text-amber-800">
                      {category.count}
                    </Badge>
                  </button>
                ))}
              </CardContent>
            </Card>
          </div>

          {/* Main Content */}
          <div className="flex-1">
            {/* Search and Sort Controls */}
            <div className="bg-white rounded-lg border border-amber-200 p-4 mb-6">
              <div className="flex flex-col md:flex-row gap-4">
                <div className="flex-1 relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
                  <Input
                    placeholder="Tìm kiếm sản phẩm..."
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="pl-10 border-amber-200 focus:border-amber-400"
                  />
                </div>
                <div className="flex gap-2">
                  <Select value={sortBy} onValueChange={(value: SortOption) => setSortBy(value)}>
                    <SelectTrigger className="w-48 border-amber-200">
                      <SelectValue placeholder="Sắp xếp theo" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="newest">Mới nhất</SelectItem>
                      <SelectItem value="price-low">Giá thấp → cao</SelectItem>
                      <SelectItem value="price-high">Giá cao → thấp</SelectItem>
                      <SelectItem value="rating">Đánh giá cao</SelectItem>
                      <SelectItem value="popular">Phổ biến</SelectItem>
                    </SelectContent>
                  </Select>
                  <Select value={itemsPerPage.toString()} onValueChange={(value) => setItemsPerPage(parseInt(value))}>
                    <SelectTrigger className="w-24 border-amber-200">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="12">12</SelectItem>
                      <SelectItem value="24">24</SelectItem>
                      <SelectItem value="36">36</SelectItem>
                      <SelectItem value="48">48</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>
            </div>

            {/* Results Header */}
            <div className="flex flex-col sm:flex-row gap-4 justify-between items-start sm:items-center mb-6">
              <div className="flex items-center gap-4">
                <h2 className="text-2xl text-amber-900">
                  {categoriesWithCount.find(c => c.id === selectedCategory)?.name} 
                  <span className="text-gray-500 text-lg ml-2">
                    ({totalProducts} sản phẩm)
                  </span>
                </h2>
                {searchQuery && (
                  <Badge variant="outline" className="text-amber-700 border-amber-300">
                    Tìm kiếm: "{searchQuery}"
                  </Badge>
                )}
              </div>
              
              <div className="flex items-center gap-2">
                <span className="text-sm text-gray-600">
                  Hiển thị {startIndex + 1}-{Math.min(endIndex, totalProducts)} của {totalProducts}
                </span>
                <div className="flex bg-white rounded-lg border border-amber-200 p-1">
                  <Button
                    variant={viewMode === 'grid' ? 'default' : 'ghost'}
                    size="sm"
                    onClick={() => setViewMode('grid')}
                    className={viewMode === 'grid' ? 'bg-amber-600 text-white' : 'text-amber-700'}
                  >
                    <Grid3x3 className="w-4 h-4" />
                  </Button>
                  <Button
                    variant={viewMode === 'list' ? 'default' : 'ghost'}
                    size="sm"
                    onClick={() => setViewMode('list')}
                    className={viewMode === 'list' ? 'bg-amber-600 text-white' : 'text-amber-700'}
                  >
                    <List className="w-4 h-4" />
                  </Button>
                </div>
              </div>
            </div>

            {/* Loading Skeleton */}
            {isLoading && (
              <div className={`grid gap-6 ${
                viewMode === 'grid' 
                  ? 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3' 
                  : 'grid-cols-1'
              }`}>
                {Array.from({ length: itemsPerPage }, (_, i) => (
                  <Card key={i} className="border-2 border-amber-200">
                    <div className={`${viewMode === 'list' ? 'flex' : 'block'}`}>
                      <div className={`${viewMode === 'list' ? 'w-48 flex-shrink-0' : ''}`}>
                        <Skeleton className={`w-full ${viewMode === 'list' ? 'h-48' : 'h-64'}`} />
                      </div>
                      <CardContent className={`p-4 ${viewMode === 'list' ? 'flex-1' : ''}`}>
                        <div className="flex items-center gap-2 mb-2">
                          <div className="flex gap-1">
                            {[...Array(5)].map((_, i) => (
                              <Skeleton key={i} className="w-4 h-4" />
                            ))}
                          </div>
                          <Skeleton className="h-4 w-16" />
                        </div>
                        <Skeleton className="h-6 w-full mb-2" />
                        <Skeleton className="h-6 w-3/4 mb-4" />
                        <div className="flex items-center gap-2 mb-4">
                          <Skeleton className="h-8 w-24" />
                          <Skeleton className="h-6 w-20" />
                        </div>
                        <Skeleton className="h-10 w-full" />
                      </CardContent>
                    </div>
                  </Card>
                ))}
              </div>
            )}

            {/* Products Grid */}
            {!isLoading && (
              <>
                {currentProducts.length > 0 ? (
                  <div className={`grid gap-6 ${
                    viewMode === 'grid' 
                      ? 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3' 
                      : 'grid-cols-1'
                  }`}>
                    {currentProducts.map((product) => (
                      <Card key={product.id} className="group hover:shadow-xl transition-all duration-300 border-2 hover:border-amber-300">
                        <div className={`${viewMode === 'list' ? 'flex' : 'block'}`}>
                          <div className={`relative overflow-hidden ${viewMode === 'list' ? 'w-48 flex-shrink-0' : ''}`}>
                            <ImageWithFallback
                              src={product.image}
                              alt={product.name}
                              className={`w-full object-cover group-hover:scale-105 transition-transform duration-300 ${
                                viewMode === 'list' ? 'h-48' : 'h-64'
                              }`}
                            />
                            {product.featured && (
                              <Badge className="absolute top-3 left-3 bg-red-600 text-white">
                                Nổi bật
                              </Badge>
                            )}
                            {product.discount > 0 && (
                              <Badge className="absolute top-3 right-3 bg-green-600 text-white">
                                -{product.discount}%
                              </Badge>
                            )}
                            
                            {/* Hover Actions */}
                            <div className="absolute inset-0 bg-black/20 opacity-0 group-hover:opacity-100 transition-opacity duration-300 flex items-center justify-center gap-2">
                              <Button 
                                size="icon" 
                                variant="secondary" 
                                className="bg-white/90 hover:bg-white"
                                onClick={() => handleQuickView(product)}
                              >
                                <Eye className="w-4 h-4" />
                              </Button>
                              <Button size="icon" variant="secondary" className="bg-white/90 hover:bg-white">
                                <Heart className="w-4 h-4" />
                              </Button>
                            </div>
                          </div>
                          
                          <CardContent className={`p-4 ${viewMode === 'list' ? 'flex-1' : ''}`}>
                            <div className="flex items-center gap-2 mb-2">
                              <div className="flex items-center">
                                {[...Array(5)].map((_, i) => (
                                  <Star key={i} className={`w-4 h-4 ${
                                    i < Math.floor(product.rating) 
                                      ? 'fill-yellow-400 text-yellow-400' 
                                      : 'text-gray-300'
                                  }`} />
                                ))}
                                <span className="text-sm text-gray-600 ml-1">
                                  {product.rating} ({product.reviews})
                                </span>
                              </div>
                            </div>
                            
                            <h3 className="text-lg text-amber-900 mb-3 line-clamp-2">
                              {product.name}
                            </h3>
                            
                            <div className="flex items-center gap-2 mb-4">
                              <span className="text-2xl text-red-600">
                                {formatPrice(product.price)}
                              </span>
                              {product.originalPrice && (
                                <span className="text-lg text-gray-500 line-through">
                                  {formatPrice(product.originalPrice)}
                                </span>
                              )}
                            </div>
                            
                            <Button className="w-full bg-red-600 hover:bg-red-700 text-white">
                              <ShoppingCart className="w-4 h-4 mr-2" />
                              Thêm vào giỏ hàng
                            </Button>
                          </CardContent>
                        </div>
                      </Card>
                    ))}
                  </div>
                ) : (
                  <div className="text-center py-16">
                    <div className="w-24 h-24 mx-auto mb-4 bg-amber-100 rounded-full flex items-center justify-center">
                      <Search className="w-12 h-12 text-amber-600" />
                    </div>
                    <h3 className="text-xl text-amber-900 mb-2">Không tìm thấy sản phẩm</h3>
                    <p className="text-gray-600 mb-4">
                      Thử thay đổi từ khóa tìm kiếm hoặc danh mục sản phẩm
                    </p>
                    <Button 
                      variant="outline" 
                      onClick={() => {
                        setSearchQuery('');
                        setSelectedCategory('all');
                      }}
                      className="border-amber-300 text-amber-700 hover:bg-amber-50"
                    >
                      Xem tất cả sản phẩm
                    </Button>
                  </div>
                )}
              </>
            )}

            {/* Pagination */}
            {!isLoading && (
              <PaginationNavigation
                currentPage={currentPage}
                totalPages={totalPages}
                totalItems={totalProducts}
                itemsPerPage={itemsPerPage}
                onPageChange={setCurrentPage}
                className="mt-8"
              />
            )}
          </div>
        </div>
      </div>

      {/* Quick View Modal */}
      <QuickViewModal
        product={selectedProduct}
        isOpen={isQuickViewOpen}
        onClose={closeQuickView}
      />

      {/* Back to Top Button */}
      <BackToTop />
    </div>
  );
}

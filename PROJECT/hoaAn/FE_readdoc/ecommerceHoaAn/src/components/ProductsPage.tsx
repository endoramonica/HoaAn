import { useState, useEffect } from 'react';
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
import { useWishlist } from '../lib/hooks/useWishlist';
import { useCart } from '../lib/hooks/useCart';
import { useAuth } from '../lib/hooks/useAuth';
import { useActiveCategories } from '../lib/hooks/useCategories';
import { getVietCommerceAPI } from '../../Api/generated-orval';
import type { AddToCartDto } from '../../Api/generated-orval/schemas';
import { vietCommerceProductService } from '../lib/services/vietCommerceProductService';
import { type ProductListDto, type ProductFilterDto } from '@/api';
import { 
  Flower2, 
  Filter,
  Grid3x3,
  List,
  Search,
  ShoppingCart,
  Heart,
  Eye,
  Loader2,
  AlertCircle
} from 'lucide-react';
import { Alert, AlertDescription } from './ui/alert';
import { toast } from 'sonner';

const api = getVietCommerceAPI();

type SortOption = 'newest' | 'price-low' | 'price-high' | 'name';
type ViewMode = 'grid' | 'list';

interface ProductsPageProps {
  onNavigate?: (page: string) => void;
}

export function ProductsPage({}: ProductsPageProps) {
  
  // UI State
  const [viewMode, setViewMode] = useState<ViewMode>('grid');
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [currentPage, setCurrentPage] = useState(1);
  const [searchQuery, setSearchQuery] = useState('');
  const [sortBy, setSortBy] = useState<SortOption>('newest');
  const [itemsPerPage, setItemsPerPage] = useState(12);
  const [selectedProduct, setSelectedProduct] = useState<any>(null);
  const [isQuickViewOpen, setIsQuickViewOpen] = useState(false);
  
  // API State
  const [products, setProducts] = useState<ProductListDto[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  
  // Wishlist state - track loading cho từng product
  const [wishlistLoading, setWishlistLoading] = useState<Set<string>>(new Set());
  const [cartLoading, setCartLoading] = useState<Set<string>>(new Set());
  
  // Hooks
  const { toggleWishlist, isInWishlist, loading: wishlistHookLoading } = useWishlist();
  const { refreshCart } = useCart();
  const { isAuthenticated } = useAuth();
  
  // Fetch categories from API
  const { data: apiCategories, isLoading: categoriesLoading } = useActiveCategories();

  // Build categories list with "All" option
  // Note: API may return productsCount: 0, so we use it if available, otherwise don't show count
  const categories = [
    { id: 'all', name: 'Tất cả', count: totalCount },
    ...(apiCategories || []).map(cat => ({
      id: cat.id,
      name: cat.name,
      // Use productsCount from API if > 0, otherwise undefined (won't show badge)
      count: cat.productCount && cat.productCount > 0 ? cat.productCount : undefined,
    })),
  ];

  /**
   * Fetch products từ API với filter và pagination
   */
  const fetchProducts = async () => {
    try {
      setIsLoading(true);
      setError(null);

      // Build filter params
      const filter: ProductFilterDto = {
        pageNumber: currentPage,
        pageSize: itemsPerPage,
        searchTerm: searchQuery || undefined,
        categoryId: selectedCategory !== 'all' ? selectedCategory : undefined,
        isActive: true,
      };

      // Apply sorting
      switch (sortBy) {
        case 'price-low':
          filter.sortBy = 'price';
          filter.isDescending = false;
          break;
        case 'price-high':
          filter.sortBy = 'price';
          filter.isDescending = true;
          break;
        case 'name':
          filter.sortBy = 'name';
          filter.isDescending = false;
          break;
        case 'newest':
        default:
          filter.sortBy = 'createdAt';
          filter.isDescending = true;
          break;
      }

      const result = await vietCommerceProductService.getProducts(filter);

      // Debug log
      console.log('[ProductsPage] Fetched result:', {
        itemsCount: result.items?.length,
        totalItems: result.totalItems,
        totalPages: result.totalPages,
        firstItem: result.items?.[0],
      });

      setProducts(result.items);
      setTotalCount(result.totalItems);
      setTotalPages(result.totalPages);
    } catch (err: any) {
      console.error('Error fetching products:', err);
      setError(err.message || 'Không thể tải danh sách sản phẩm. Vui lòng thử lại sau.');
    } finally {
      setIsLoading(false);
    }
  };

  // Fetch products khi filter thay đổi
  useEffect(() => {
    fetchProducts();
  }, [currentPage, itemsPerPage, searchQuery, selectedCategory, sortBy]);

  // Reset về trang 1 khi filter thay đổi
  useEffect(() => {
    if (currentPage !== 1) {
      setCurrentPage(1);
    }
  }, [searchQuery, selectedCategory, sortBy, itemsPerPage]);

  // Scroll to top khi đổi trang
  useEffect(() => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }, [currentPage]);

  const handleQuickView = (product: ProductListDto) => {
    setSelectedProduct(product);
    setIsQuickViewOpen(true);
  };

  const closeQuickView = () => {
    setIsQuickViewOpen(false);
    setSelectedProduct(null);
  };

  const handleToggleWishlist = async (productId: string, productName: string, e: React.MouseEvent) => {
    e.stopPropagation();
    e.preventDefault();
    
    // Prevent multiple clicks
    if (wishlistLoading.has(productId)) {
      return;
    }
    
    try {
      // Set loading state cho product này
      setWishlistLoading(prev => new Set(prev).add(productId));
      
      // Gọi API toggle wishlist
      const success = await toggleWishlist(productId);
      
      if (!success) {
        // Nếu toggle thất bại, toast đã được hiển thị trong hook
        // Nhưng có thể thêm xử lý cụ thể ở đây nếu cần
        console.warn(`[ProductsPage] Toggle wishlist failed for product: ${productId}`);
      }
    } catch (err: any) {
      // Xử lý các loại lỗi cụ thể
      console.error('[ProductsPage] Wishlist toggle error:', err);
      
      let errorMessage = 'Không thể thay đổi trạng thái yêu thích';
      
      if (err?.response?.status === 401) {
        errorMessage = 'Vui lòng đăng nhập để thêm vào danh sách yêu thích';
      } else if (err?.response?.status === 403) {
        errorMessage = 'Bạn không có quyền thực hiện thao tác này';
      } else if (err?.response?.status >= 500) {
        errorMessage = 'Lỗi máy chủ. Vui lòng thử lại sau';
      } else if (err?.message) {
        errorMessage = err.message;
      }
      
      // Toast đã được hiển thị trong hook, nhưng có thể thêm log chi tiết
      console.error(`[ProductsPage] Error details:`, {
        productId,
        productName,
        error: err,
        status: err?.response?.status,
        message: errorMessage
      });
    } finally {
      // Remove loading state
      setWishlistLoading(prev => {
        const newSet = new Set(prev);
        newSet.delete(productId);
        return newSet;
      });
    }
  };

  const handleAddToCart = async (productId: string, productName: string, e: React.MouseEvent) => {
    e.stopPropagation();
    e.preventDefault();
    
    if (cartLoading.has(productId)) {
      return;
    }
    
    try {
      setCartLoading(prev => new Set(prev).add(productId));
      
      const dto: AddToCartDto = { 
        productId, 
        quantity: 1
      };
      
      if (isAuthenticated) {
        await api.postApiV1CartAdd(dto);
      } else {
        await api.postApiV1CartGuestAdd(dto);
      }
      
      await refreshCart();
      
      toast.success('Đã thêm vào giỏ hàng!', {
        description: productName,
      });
    } catch (error: any) {
      console.error('Add to cart error:', error);
      
      if (error.message?.includes('Insufficient stock')) {
        toast.error('Sản phẩm không đủ số lượng trong kho');
      } else if (error.status === 401) {
        toast.error('Vui lòng đăng nhập để thêm vào giỏ hàng');
      } else {
        toast.error(error.message || 'Không thể thêm vào giỏ hàng');
      }
    } finally {
      setCartLoading(prev => {
        const newSet = new Set(prev);
        newSet.delete(productId);
        return newSet;
      });
    }
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('vi-VN').format(price) + '₫';
  };

  const startIndex = (currentPage - 1) * itemsPerPage;
  

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
        {/* Error Alert */}
        {error && (
          <Alert variant="destructive" className="mb-6">
            <AlertCircle className="h-4 w-4" />
            <AlertDescription>{error}</AlertDescription>
          </Alert>
        )}

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
                {categoriesLoading ? (
                  // Loading skeleton for categories
                  <>
                    {Array.from({ length: 5 }, (_, i) => (
                      <Skeleton key={i} className="h-12 w-full rounded-lg" />
                    ))}
                  </>
                ) : (
                  categories.map((category) => (
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
                      {/* Only show badge for "All" or if category has count > 0 */}
                      {(category.id === 'all' || (category.count !== undefined && category.count > 0)) && (
                        <Badge variant="secondary" className="bg-amber-200 text-amber-800">
                          {category.id === 'all' ? totalCount : category.count}
                        </Badge>
                      )}
                    </button>
                  ))
                )}
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
                      <SelectItem value="name">Tên A-Z</SelectItem>
                    </SelectContent>
                  </Select>
                  <Select 
                      value={itemsPerPage.toString()} 
                      onValueChange={(value: string) => setItemsPerPage(parseInt(value))}
                    >
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
                  {categories.find(c => c.id === selectedCategory)?.name} 
                  <span className="text-gray-500 text-lg ml-2">
                    ({totalCount} sản phẩm)
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
                  Hiển thị {startIndex + 1}-{Math.min( totalCount)} của {totalCount}
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
                        <Skeleton className="h-6 w-full mb-2" />
                        <Skeleton className="h-6 w-3/4 mb-4" />
                        <Skeleton className="h-8 w-24 mb-4" />
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
                {products.length > 0 ? (
                  <div className={`grid gap-6 ${
                    viewMode === 'grid' 
                      ? 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3' 
                      : 'grid-cols-1'
                  }`}>
                    {products.map((product) => {
                      // Safety checks
                      if (!product.id) return null;
                      const productId = product.id;
                      const productName = product.name || 'Sản phẩm';
                      const productPrice = product.price ?? 0;
                      
                      return (
                        <Card key={productId} className="group hover:shadow-xl transition-all duration-300 border-2 hover:border-amber-300">
                          <div className={`${viewMode === 'list' ? 'flex' : 'block'}`}>
                            <div className={`relative overflow-hidden ${viewMode === 'list' ? 'w-48 flex-shrink-0' : ''}`}>
                              <ImageWithFallback
                                src={product.primaryImage || 'https://images.unsplash.com/photo-1602874801006-84c78b4e9dcc?w=400'}
                                alt={productName}
                                className={`w-full object-cover group-hover:scale-105 transition-transform duration-300 ${
                                  viewMode === 'list' ? 'h-48' : 'h-64'
                                }`}
                              />
                              
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
                                <Button 
                                  size="icon" 
                                  variant="secondary" 
                                  className={`bg-white/90 hover:bg-white transition-all ${
                                    isInWishlist(productId) ? 'text-pink-600' : 'text-gray-700'
                                  }`}
                                  onClick={(e) => handleToggleWishlist(productId, productName, e)}
                                  disabled={wishlistLoading.has(productId) || wishlistHookLoading}
                                  title={
                                    wishlistLoading.has(productId) 
                                      ? 'Đang xử lý...' 
                                      : isInWishlist(productId) 
                                      ? 'Xóa khỏi danh sách yêu thích' 
                                      : 'Thêm vào danh sách yêu thích'
                                  }
                                >
                                  {wishlistLoading.has(productId) ? (
                                    <Loader2 className="w-4 h-4 animate-spin" />
                                  ) : (
                                    <Heart className={`w-4 h-4 transition-all ${
                                      isInWishlist(productId) ? 'fill-current' : ''
                                    }`} />
                                  )}
                                </Button>
                              </div>
                            </div>
                            
                            <CardContent className={`p-4 ${viewMode === 'list' ? 'flex-1' : ''}`}>
                              <h3 className="text-lg text-amber-900 mb-2 line-clamp-2">
                                {productName}
                              </h3>
                              
                              {product.categoryName && (
                                <Badge variant="outline" className="mb-2 text-amber-700 border-amber-300">
                                  {product.categoryName}
                                </Badge>
                              )}
                              
                              <div className="flex items-center gap-2 mb-4">
                                <span className="text-2xl text-red-600">
                                  {formatPrice(productPrice)}
                                </span>
                              </div>

                            <div className="text-sm text-gray-600 mb-4">
                              Còn lại: {product.stockQuantity} sản phẩm
                            </div>
                            
                              <Button 
                                className="w-full bg-red-600 hover:bg-red-700 text-white"
                                disabled={!product.inStock || cartLoading.has(productId)}
                                onClick={(e) => handleAddToCart(productId, productName, e)}
                              >
                                {cartLoading.has(productId) ? (
                                  <>
                                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                                    Đang thêm...
                                  </>
                                ) : (
                                  <>
                                    <ShoppingCart className="w-4 h-4 mr-2" />
                                    {product.inStock ? 'Thêm vào giỏ hàng' : 'Hết hàng'}
                                  </>
                                )}
                              </Button>
                            </CardContent>
                          </div>
                        </Card>
                      );
                    })}
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
            {!isLoading && totalPages > 1 && (
              <PaginationNavigation
                currentPage={currentPage}
                totalPages={totalPages}
                totalItems={totalCount}
                itemsPerPage={itemsPerPage}
                onPageChange={setCurrentPage}
                className="mt-8"
              />
            )}
          </div>
        </div>
      </div>

      {/* Quick View Modal */}
      {selectedProduct && (
        <QuickViewModal
          product={selectedProduct}
          isOpen={isQuickViewOpen}
          onClose={closeQuickView}
        />
      )}

      <BackToTop />
    </div>
  );
}
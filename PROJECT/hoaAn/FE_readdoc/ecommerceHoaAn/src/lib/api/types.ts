/**
 * TypeScript types tương ứng với C# models/DTOs từ backend ASP.NET Core
 * Các types này phải được đồng bộ với backend models
 */

// ============================================================================
// Authentication Types
// ============================================================================

export interface LoginRequest {
  email: string;
  password: string;
  rememberMe?: boolean;
}

export interface GoogleLoginRequest {
  idToken: string;
  accessToken?: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  fullName: string;
  phoneNumber?: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: UserDto;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

// ============================================================================
// User Types
// ============================================================================

export interface UserDto {
  id: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  avatar?: string;
  role: UserRole;
  isEmailConfirmed: boolean;
  createdAt: string;
  updatedAt: string;
  customerId?: string; // Customer ID from backend (different from userId)
}

export enum UserRole {
  Customer = "Customer",
  Driver = "Driver",
  Admin = "Admin",
  SpiritualAdvisor = "SpiritualAdvisor"
}

export interface UpdateProfileRequest {
  fullName?: string;
  phoneNumber?: string;
  avatar?: string;
}

// ============================================================================
// Product Types
// ============================================================================

export interface ProductDto {
  id: string;
  name: string;
  slug: string;
  description: string;
  shortDescription?: string;
  price: number;
  originalPrice?: number;
  discount?: number;
  categoryId: string;
  category: CategoryDto;
  images: ProductImageDto[];
  thumbnailUrl?: string;
  stock: number;
  isActive: boolean;
  isFeatured: boolean;
  rating: number;
  reviewCount: number;
  tags: string[];
  specifications?: Record<string, string>;
  createdAt: string;
  updatedAt: string;
}

// ============================================================================
// VietCommerce Product API DTOs (Tương thích với .NET Backend)
// ============================================================================

export interface DisplayPrice {
  originalPrice: number;
  discountedPrice: number;
  discountAmount: number;
}

export interface ProductListDto {
  id: string;
  name: string;
  code: string;
  slug: string;
  price: number;
  compareAtPrice: number;
  displayPrice: DisplayPrice;
  stockQuantity: number;
  inStock: boolean;
  primaryImage?: string;
  isActive: boolean;
  isFeatured: boolean;
  categoryName?: string;
  storeName?: string;
  viewCount: number;
  favoriteCount: number;
  averageRating: number;
  createdAt: string;
}

export interface ProductImageDetailDto {
  id: string;
  productId: string;
  url: string;
  thumbnailUrl: string;
  displayOrder: number;
  mediaType: number;
  isMain: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface CustomizableOptionDto {
  id: string;
  name: string;
  baseQuantity: number;
  unitPrice: number;
  minQuantity: number;
  maxQuantity: number;
  unit: string;
}

export interface ProductDetailDto {
  id: string;
  name: string;
  slug: string;
  code: string;
  categoryId: string;
  categoryName?: string;
  storeId: string;
  storeName?: string;
  sku: string;
  price: number;
  compareAtPrice?: number;
  displayPrice?: DisplayPrice;
  stock?: number;
  stockQuantity?: number;
  isActive: boolean;
  isFeatured?: boolean;
  description?: string;
  primaryImage?: string;
  thumbnailUrl?: string;
  images?: ProductImageDetailDto[];
  tags?: string[];
  viewCount?: number;
  favoriteCount?: number;
  averageRating?: number;
  reviewCount?: number;
  details?: string[];
  customizableOptions?: CustomizableOptionDto[];
  createdAt: string;
  updatedAt: string;
}

export interface ProductCreateDto {
  name: string;
  code: string;
  categoryId?: string;
  sku?: string;
  price?: number;
  stockQuantity?: number;
  description?: string;
  isActive?: boolean;
}

export interface ProductUpdateDto {
  name?: string;
  categoryId?: string;
  sku?: string;
  stockQuantity?: number;
  isActive?: boolean;
  description?: string;
}

export interface ProductFilterDto {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  categoryId?: string;
  storeId?: string;
  isActive?: boolean;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: string;
  isDescending?: boolean;
}

export interface PaginatedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

// ============================================================================
// Product Types (Legacy - Keep for backward compatibility)
// ============================================================================

export interface ProductImageDto {
  id: string;
  url: string;
  altText?: string;
  isPrimary: boolean;
  sortOrder: number;
}

export interface CategoryDto {
  id: string;
  name: string;
  slug: string;
  description?: string;
  parentId?: string;
  icon?: string;
  imageUrl?: string;
  sortOrder: number;
  isActive: boolean;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  shortDescription?: string;
  price: number;
  originalPrice?: number;
  categoryId: string;
  stock: number;
  tags?: string[];
  specifications?: Record<string, string>;
}

export interface UpdateProductRequest {
  name?: string;
  description?: string;
  shortDescription?: string;
  price?: number;
  originalPrice?: number;
  categoryId?: string;
  stock?: number;
  isActive?: boolean;
  isFeatured?: boolean;
  tags?: string[];
  specifications?: Record<string, string>;
}

export interface ProductFilterParams {
  categoryId?: string;
  search?: string;
  minPrice?: number;
  maxPrice?: number;
  tags?: string[];
  isFeatured?: boolean;
  sortBy?: 'name' | 'price' | 'createdAt' | 'rating';
  sortOrder?: 'asc' | 'desc';
  pageNumber?: number;
  pageSize?: number;
}

// ============================================================================
// Order Types
// ============================================================================

export interface OrderDto {
  id: string;
  orderNumber: string;
  userId: string;
  user: UserDto;
  status: OrderStatus;
  paymentStatus: PaymentStatus;
  paymentMethod: PaymentMethod;
  items: OrderItemDto[];
  subtotal: number;
  shippingFee: number;
  discount: number;
  total: number;
  shippingAddress: AddressDto;
  billingAddress?: AddressDto;
  note?: string;
  trackingNumber?: string;
  createdAt: string;
  updatedAt: string;
  completedAt?: string;
}

export interface OrderItemDto {
  id: string;
  productId: string;
  product: ProductDto;
  quantity: number;
  price: number;
  total: number;
}

export enum OrderStatus {
  Pending = "Pending",
  Confirmed = "Confirmed",
  Processing = "Processing",
  Shipping = "Shipping",
  Delivered = "Delivered",
  Cancelled = "Cancelled",
  Refunded = "Refunded"
}

export enum PaymentStatus {
  Pending = "Pending",
  Paid = "Paid",
  Failed = "Failed",
  Refunded = "Refunded"
}

export enum PaymentMethod {
  COD = "COD",
  BankTransfer = "BankTransfer",
  Momo = "Momo",
  ZaloPay = "ZaloPay",
  VNPay = "VNPay"
}

export interface AddressDto {
  id?: string;
  fullName: string;
  phoneNumber: string;
  addressLine1: string;
  addressLine2?: string;
  ward: string;
  district: string;
  province: string;
  postalCode?: string;
  isDefault?: boolean;
}

export interface CreateOrderRequest {
  items: CreateOrderItemRequest[];
  shippingAddress: AddressDto;
  billingAddress?: AddressDto;
  paymentMethod: PaymentMethod;
  note?: string;
  couponCode?: string;
}

export interface CreateOrderItemRequest {
  productId: string;
  quantity: number;
}

export interface OrderFilterParams {
  status?: OrderStatus;
  paymentStatus?: PaymentStatus;
  fromDate?: string;
  toDate?: string;
  search?: string;
  pageNumber?: number;
  pageSize?: number;
}

// ============================================================================
// Delivery Types (VietDelivery Service)
// ============================================================================

export interface DeliveryOrderDto {
  id: string;
  trackingNumber: string;
  senderId: string;
  sender: UserDto;
  recipientName: string;
  recipientPhone: string;
  pickupAddress: AddressDto;
  deliveryAddress: AddressDto;
  packageType: PackageType;
  weight: number;
  dimensions?: PackageDimensions;
  price: number;
  status: DeliveryStatus;
  driverId?: string;
  driver?: DriverDto;
  estimatedPickupTime?: string;
  actualPickupTime?: string;
  estimatedDeliveryTime?: string;
  actualDeliveryTime?: string;
  note?: string;
  createdAt: string;
  updatedAt: string;
}

export enum PackageType {
  Document = "Document",
  Food = "Food",
  Fragile = "Fragile",
  Standard = "Standard"
}

export interface PackageDimensions {
  length: number;
  width: number;
  height: number;
}

export enum DeliveryStatus {
  PendingPickup = "PendingPickup",
  DriverAssigned = "DriverAssigned",
  PickedUp = "PickedUp",
  InTransit = "InTransit",
  Delivered = "Delivered",
  Cancelled = "Cancelled"
}

export interface DriverDto {
  id: string;
  fullName: string;
  phoneNumber: string;
  vehicleType: VehicleType;
  licensePlate: string;
  rating: number;
  totalDeliveries: number;
  avatar?: string;
  isOnline: boolean;
}

export enum VehicleType {
  Motorbike = "Motorbike",
  Car = "Car",
  Truck = "Truck"
}

export interface CreateDeliveryOrderRequest {
  recipientName: string;
  recipientPhone: string;
  pickupAddress: AddressDto;
  deliveryAddress: AddressDto;
  packageType: PackageType;
  weight: number;
  dimensions?: PackageDimensions;
  note?: string;
}

export interface PriceEstimationRequest {
  pickupAddress: AddressDto;
  deliveryAddress: AddressDto;
  packageType: PackageType;
  weight: number;
}

export interface PriceEstimationResponse {
  estimatedPrice: number;
  distance: number;
  estimatedDuration: number;
  breakdown: {
    basePrice: number;
    distanceFee: number;
    weightFee: number;
    packageTypeFee: number;
  };
}

// ============================================================================
// Spiritual Service Types
// ============================================================================

export interface PrayerDto {
  id: string;
  userId: string;
  user: UserDto;
  templeName: string;
  prayerType: PrayerType;
  content: string;
  isAnonymous: boolean;
  isPublic: boolean;
  status: PrayerStatus;
  createdAt: string;
  completedAt?: string;
}

export enum PrayerType {
  Health = "Health",
  Career = "Career",
  Family = "Family",
  Love = "Love",
  Wealth = "Wealth",
  General = "General"
}

export enum PrayerStatus {
  Pending = "Pending",
  Completed = "Completed"
}

export interface CreatePrayerRequest {
  templeName: string;
  prayerType: PrayerType;
  content: string;
  isAnonymous?: boolean;
  isPublic?: boolean;
}

export interface FengShuiConsultationDto {
  id: string;
  userId: string;
  user: UserDto;
  consultationType: ConsultationType;
  birthDate: string;
  gender: string;
  address?: AddressDto;
  questions: string;
  status: ConsultationStatus;
  advisorId?: string;
  advisor?: UserDto;
  response?: string;
  scheduledAt?: string;
  completedAt?: string;
  createdAt: string;
}

export enum ConsultationType {
  HomeAssessment = "HomeAssessment",
  PersonalReading = "PersonalReading",
  BusinessConsultation = "BusinessConsultation",
  DateSelection = "DateSelection"
}

export enum ConsultationStatus {
  Pending = "Pending",
  Scheduled = "Scheduled",
  InProgress = "InProgress",
  Completed = "Completed",
  Cancelled = "Cancelled"
}

export interface CreateConsultationRequest {
  consultationType: ConsultationType;
  birthDate: string;
  gender: string;
  address?: AddressDto;
  questions: string;
  scheduledAt?: string;
}

// ============================================================================
// Wishlist Types
// ============================================================================

export interface WishlistItemDto {
  id: string;
  userId: string;
  productId: string;
  product: ProductDto;
  createdAt: string;
}

export interface AddToWishlistRequest {
  productId: string;
}

export interface WishlistSummaryDto {
  totalItems: number;
  items: WishlistItemDto[];
}

// ============================================================================
// Community Types
// ============================================================================

export interface CommunityPostDto {
  id: string;
  userId: string;
  user: UserDto;
  title: string;
  content: string;
  images?: string[];
  category: PostCategory;
  likes: number;
  commentCount: number;
  isLiked: boolean;
  createdAt: string;
  updatedAt: string;
}

export enum PostCategory {
  Experience = "Experience",
  Question = "Question",
  Discussion = "Discussion",
  News = "News"
}

export interface CommentDto {
  id: string;
  postId: string;
  userId: string;
  user: UserDto;
  content: string;
  likes: number;
  isLiked: boolean;
  createdAt: string;
}

export interface CreatePostRequest {
  title: string;
  content: string;
  images?: string[];
  category: PostCategory;
}

export interface CreateCommentRequest {
  postId: string;
  content: string;
}

// ============================================================================
// Pagination & Common Types
// ============================================================================

export interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

export interface ValidationError {
  field: string;
  message: string;
}

// ASP.NET Core ProblemDetails format
export interface ProblemDetails {
  type: string;
  title: string;
  status: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}

// File upload types
export interface FileUploadRequest {
  file: File;
  folder?: string;
}

export interface FileUploadResponse {
  url: string;
  fileName: string;
  fileSize: number;
  contentType: string;
}

// ============================================================================
// Cart Types
// ============================================================================

export interface CartItemCustomization {
  optionId?: string;
  quantity?: number;
  unitPrice?: number;
  totalPrice?: number;
}

export interface CartItemDto {
  cartItemId: string;
  productId: string;
  productName: string;
  productSlug: string;
  sku: string;
  productImage: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  availableStock: number;
  isProductActive: boolean;
  purchaseCount: number;
  avgRating: number;
  reviewCount: number;
  basePrice: number;
  customizationPrice: number;
  finalPrice: number;
  customizations?: CartItemCustomization[];
  createdAt: string;
  updatedAt: string;
}

export interface CartDto {
  cartId: string;
  userId: string;
  items: CartItemDto[];
  totalItems: number;
  subTotal: number;
  taxAmount: number;
  shippingFee: number;
  totalAmount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CartSummaryDto {
  totalItems: number;
  subTotal: number;
  taxAmount: number;
  shippingFee: number;
  totalAmount: number;
  appliedCoupon?: string;
}

export interface CartResponseDto {
  success: boolean;
  data: CartDto;
  message: string;
}

export interface CartSummaryResponseDto {
  success: boolean;
  data: CartSummaryDto;
  message: string;
}

// ============================================================================
// AI Chat Types
// ============================================================================

export interface ChatMessageDto {
  id: string;
  conversationId: string;
  userId: string;
  role: 'user' | 'assistant';
  content: string;
  createdAt: string;
}

export interface SendMessageRequest {
  conversationId?: string;
  message: string;
}

export interface SendMessageResponse {
  conversationId: string;
  message: ChatMessageDto;
}

// ============================================================================
// Marketing Post Types
// ============================================================================

export interface TaggedProductDto {
  id: string;
  name: string;
  price: number;
  currency: string;
  formattedPrice: string;
  thumbnailUrl: string;
  hasDiscount: boolean;
  discountPercentage: number;
}

export interface MarketingPost {
  id: string;
  title: string;
  shortDescription: string;
  content: string;
  image: string;
  imageUrls?: string[];
  taggedProduct?: TaggedProductDto;
  hashtags?: string[];
  platform?: string;
  status: 'Draft' | 'Published' | 'Scheduled';
  isFeatured: boolean;
  views: number;
  clicks: number;
  shares: number;
  createdAt: string;
  publishedDate?: string;
  productId?: string;
}

export interface GetMarketingPostsQuery {
  status?: 'Draft' | 'Published' | 'Scheduled';
  isFeatured?: boolean;
  pageNumber?: number;
  pageSize?: number;
  search?: string;
}

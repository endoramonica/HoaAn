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
  fullName: string;
  phoneNumber: string;
  addressLine1: string;
  addressLine2?: string;
  ward: string;
  district: string;
  province: string;
  postalCode?: string;
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
  errors?: ValidationError[];
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

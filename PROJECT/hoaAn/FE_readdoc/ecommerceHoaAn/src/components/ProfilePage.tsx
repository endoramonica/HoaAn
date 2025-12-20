import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "./ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "./ui/card";
import { Badge } from "./ui/badge";
import { Input } from "./ui/input";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "./ui/tabs";
import { Avatar, AvatarFallback, AvatarImage } from "./ui/avatar";
import { Switch } from "./ui/switch";
import { ImageWithFallback } from "./figma/ImageWithFallback";
import { useAuth } from "../lib/hooks/useAuth";
import { orderService } from "../lib/services/orderService";
import { customerAdminService } from "../lib/services/customerAdminService";
import { addressService } from "../lib/services/addressService";
import { AddressDialog } from "./AddressDialog";
// Types from Orval generated API
type OrderDetailDto = any;
type CustomerDetailDto = any;
type UpdateCustomerRequest = any;
type CustomerAddressDto = any;
import { Loader2 } from "lucide-react";
import { toast } from "sonner";

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "./ui/alert-dialog";
import {
  User,
  Package,
  MapPin,
  Bell,
  Shield,
  Edit3,
  Camera,
  Star,
  Eye,
  Phone,
  Calendar,
  Award,
  CheckCircle,
  LogOut,
} from "lucide-react";

// Order interface now uses OrderDetailDto from API
type Order = OrderDetailDto;

export function ProfilePage() {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState("profile");
  const [isEditing, setIsEditing] = useState(false);
  const { logout, isLoading, user } = useAuth();
  const [showLogoutDialog, setShowLogoutDialog] = useState(false);
  const [orders, setOrders] = useState<Order[]>([]);
  const [ordersLoading, setOrdersLoading] = useState(true);
  const [ordersError, setOrdersError] = useState<string | null>(null);

  // Customer data from API
  const [customerData, setCustomerData] = useState<CustomerDetailDto | null>(null);
  const [customerLoading, setCustomerLoading] = useState(true);
  const [customerError, setCustomerError] = useState<string | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  const [userInfo, setUserInfo] = useState({
    name: "",
    email: "",
    phone: "",
    birthDate: "",
    gender: "male",
    avatar:
      "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&h=150&fit=crop&crop=face",
  });

  // Address state
  const [addresses, setAddresses] = useState<CustomerAddressDto[]>([]);
  const [addressesLoading, setAddressesLoading] = useState(true);
  const [addressesError, setAddressesError] = useState<string | null>(null);
  const [addressDialogOpen, setAddressDialogOpen] = useState(false);
  const [selectedAddress, setSelectedAddress] = useState<CustomerAddressDto | undefined>();

  // Load customer data from API
  useEffect(() => {
    const loadCustomerData = async () => {
      if (!user?.customerId) {
        console.warn("[ProfilePage] No customer ID available");
        setCustomerLoading(false);
        return;
      }

      try {
        setCustomerLoading(true);
        setCustomerError(null);
        const data = await customerAdminService.getCustomerById(user.customerId);
        setCustomerData(data);

        // Response contains both customer and user data
        // Customer data: loyaltyPoints, isActive, addresses, etc.
        // User data: name, email, phone, etc.
        const userData = (data as any).user || {};

        // Populate userInfo with API data
        setUserInfo((prev) => ({
          ...prev,
          name: userData.name || user.fullName || "",
          email: userData.email || user.email || "",
          phone: userData.phoneNumber || userData.phone || user.phoneNumber || "",
          birthDate: "", // API doesn't provide birthDate
        }));

        console.log("[ProfilePage] Customer data loaded:", data);
      } catch (error) {
        console.error("[ProfilePage] Failed to load customer data:", error);
        setCustomerError("Không thể tải thông tin cá nhân");
        toast.error("Không thể tải thông tin cá nhân");
      } finally {
        setCustomerLoading(false);
      }
    };

    loadCustomerData();
  }, [user?.customerId]);

  // Load orders from API
  // ✅ FIXED: Chỉ load khi user authenticated
  useEffect(() => {
    const loadOrders = async () => {
      if (!user) {
        console.log("[ProfilePage] User not authenticated, skipping orders load");
        setOrdersLoading(false);
        return;
      }

      try {
        setOrdersLoading(true);
        setOrdersError(null);
        const response = await orderService.getMyOrders({
          pageNumber: 1,
          pageSize: 10,
        });
        setOrders(response.items || []);
      } catch (error) {
        console.error("Failed to load orders:", error);
        setOrdersError("Không thể tải danh sách đơn hàng");
      } finally {
        setOrdersLoading(false);
      }
    };

    loadOrders();
  }, [user]);

  // Load addresses from API
  // ✅ FIXED: Chỉ load khi user authenticated
  useEffect(() => {
    const loadAddresses = async () => {
      if (!user) {
        console.log("[ProfilePage] User not authenticated, skipping addresses load");
        setAddressesLoading(false);
        return;
      }

      try {
        setAddressesLoading(true);
        setAddressesError(null);
        const data = await addressService.getAddresses();
        setAddresses(data || []);
        console.log("[ProfilePage] Addresses loaded:", data);
      } catch (error) {
        console.error("[ProfilePage] Failed to load addresses:", error);
        setAddressesError("Không thể tải danh sách địa chỉ");
        toast.error("Không thể tải danh sách địa chỉ");
      } finally {
        setAddressesLoading(false);
      }
    };

    loadAddresses();
  }, [user]);

  const [notifications, setNotifications] = useState({
    orderUpdates: true,
    promotions: true,
    newsletter: false,
    smsNotifications: true,
  });

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("vi-VN").format(price) + "₫";
  };

  const getStatusColor = (status: string | undefined) => {
    const normalizedStatus = status?.toLowerCase();
    switch (normalizedStatus) {
      case "pending":
        return "bg-yellow-100 text-yellow-800";
      case "confirmed":
        return "bg-blue-100 text-blue-800";
      case "processing":
        return "bg-blue-100 text-blue-800";
      case "shipping":
        return "bg-purple-100 text-purple-800";
      case "delivered":
        return "bg-green-100 text-green-800";
      case "cancelled":
        return "bg-red-100 text-red-800";
      case "refunded":
        return "bg-orange-100 text-orange-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  const getStatusText = (status: string | undefined) => {
    const normalizedStatus = status?.toLowerCase();
    switch (normalizedStatus) {
      case "pending":
        return "Chờ xác nhận";
      case "confirmed":
        return "Đã xác nhận";
      case "processing":
        return "Đang xử lý";
      case "shipping":
        return "Đang giao";
      case "delivered":
        return "Đã giao";
      case "cancelled":
        return "Đã hủy";
      case "refunded":
        return "Đã hoàn tiền";
      default:
        return "Không xác định";
    }
  };

  const handleSaveProfile = async () => {
    if (!user?.customerId) {
      toast.error("Không thể xác định khách hàng");
      return;
    }

    try {
      setIsSaving(true);

      const updateData: UpdateCustomerRequest = {
        name: userInfo.name,
        email: userInfo.email,
        phone: userInfo.phone,
        
      };

      const updatedData = await customerAdminService.updateCustomer(
        user.customerId,
        updateData
      );

      setCustomerData(updatedData);
      setIsEditing(false);
      toast.success("Cập nhật thông tin thành công!");
      console.log("[ProfilePage] Customer updated:", updatedData);
    } catch (error) {
      console.error("[ProfilePage] Failed to update customer:", error);
      toast.error("Cập nhật thông tin thất bại");
    } finally {
      setIsSaving(false);
    }
  };

  // Address handlers
  const handleDeleteAddress = async (addressId: string) => {
    try {
      await addressService.deleteAddress(addressId);
      setAddresses(addresses.filter(addr => addr.id !== addressId));
      toast.success("Xóa địa chỉ thành công!");
      console.log("[ProfilePage] Address deleted:", addressId);
    } catch (error) {
      console.error("[ProfilePage] Failed to delete address:", error);
      toast.error("Xóa địa chỉ thất bại");
    }
  };

  const handleSetDefaultAddress = async (addressId: string) => {
    try {
      await addressService.setDefaultAddress(addressId);
      // Update addresses list
      setAddresses(addresses.map(addr => ({
        ...addr,
        isDefault: addr.id === addressId
      })));
      toast.success("Đặt địa chỉ mặc định thành công!");
      console.log("[ProfilePage] Default address set:", addressId);
    } catch (error) {
      console.error("[ProfilePage] Failed to set default address:", error);
      toast.error("Đặt địa chỉ mặc định thất bại");
    }
  };

  const handleEditAddress = (addressId: string) => {
    const address = addresses.find(addr => addr.id === addressId);
    if (address) {
      setSelectedAddress(address);
      setAddressDialogOpen(true);
    }
  };

  const handleAddAddress = () => {
    setSelectedAddress(undefined);
    setAddressDialogOpen(true);
  };

  const handleAddressDialogSuccess = (address: CustomerAddressDto) => {
    if (selectedAddress) {
      // Update existing address
      setAddresses(addresses.map(addr => addr.id === address.id ? address : addr));
    } else {
      // Add new address
      setAddresses([...addresses, address]);
    }
  };

  const totalSpent = orders.reduce((sum, order) => sum + (order.totalAmount || 0), 0);
  const completedOrders = orders.filter(
    (order) => order.status?.toLowerCase() === "delivered"
  ).length;

  return (
    <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50">
      {/* Header */}
      <section className="bg-gradient-to-r from-amber-900 to-red-800 text-white py-16">
        <div className="max-w-6xl mx-auto px-4">
          <div className="flex flex-col md:flex-row items-center gap-6">
            <div className="relative">
              <Avatar className="w-24 h-24 border-4 border-white">
                <AvatarImage src={userInfo.avatar} alt={userInfo.name} />
                <AvatarFallback className="text-2xl bg-amber-600">
                  {userInfo.name
                    .split(" ")
                    .map((n) => n[0])
                    .join("")}
                </AvatarFallback>
              </Avatar>
              <Button
                size="icon"
                className="absolute -bottom-2 -right-2 w-8 h-8 bg-white text-amber-900 hover:bg-amber-50"
              >
                <Camera className="w-4 h-4" />
              </Button>
            </div>

            <div className="text-center md:text-left">
              <h1 className="text-3xl md:text-4xl mb-2">
                Xin chào, {userInfo.name}
              </h1>
              <p className="text-yellow-100 mb-4">Thành viên từ tháng 3/2023</p>

              {/* Stats */}
              <div className="flex flex-wrap gap-6 justify-center md:justify-start">
                <div className="text-center">
                  <div className="text-2xl">{completedOrders}</div>
                  <div className="text-yellow-100 text-sm">Đơn hoàn thành</div>
                </div>
                <div className="text-center">
                  <div className="text-2xl">{formatPrice(totalSpent)}</div>
                  <div className="text-yellow-100 text-sm">Tổng chi tiêu</div>
                </div>
                <div className="text-center">
                  <div className="text-2xl flex items-center gap-1">
                    <Star className="w-5 h-5 fill-yellow-400 text-yellow-400" />
                    VIP
                  </div>
                  <div className="text-yellow-100 text-sm">Hạng thành viên</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <div className="max-w-6xl mx-auto px-4 py-8">
        <Tabs
          value={activeTab}
          onValueChange={setActiveTab}
          className="space-y-6"
        >
          <TabsList className="grid w-full grid-cols-2 md:grid-cols-5 bg-white border-2 border-amber-200">
            <TabsTrigger
              value="profile"
              className="data-[state=active]:bg-amber-100 data-[state=active]:text-amber-900"
            >
              <User className="w-4 h-4 mr-2" />
              <span className="hidden sm:inline">Hồ sơ</span>
            </TabsTrigger>
            <TabsTrigger
              value="orders"
              className="data-[state=active]:bg-amber-100 data-[state=active]:text-amber-900"
            >
              <Package className="w-4 h-4 mr-2" />
              <span className="hidden sm:inline">Đơn hàng</span>
            </TabsTrigger>
            <TabsTrigger
              value="addresses"
              className="data-[state=active]:bg-amber-100 data-[state=active]:text-amber-900"
            >
              <MapPin className="w-4 h-4 mr-2" />
              <span className="hidden sm:inline">Địa chỉ</span>
            </TabsTrigger>
            <TabsTrigger
              value="notifications"
              className="data-[state=active]:bg-amber-100 data-[state=active]:text-amber-900"
            >
              <Bell className="w-4 h-4 mr-2" />
              <span className="hidden sm:inline">Thông báo</span>
            </TabsTrigger>
            <TabsTrigger
              value="security"
              className="data-[state=active]:bg-amber-100 data-[state=active]:text-amber-900"
            >
              <Shield className="w-4 h-4 mr-2" />
              <span className="hidden sm:inline">Bảo mật</span>
            </TabsTrigger>
          </TabsList>

          {/* Profile Tab */}
          <TabsContent value="profile" className="space-y-6">
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2 text-amber-900">
                    <User className="w-5 h-5" />
                    Thông tin cá nhân
                  </CardTitle>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() =>
                      isEditing ? handleSaveProfile() : setIsEditing(true)
                    }
                    disabled={customerLoading || isSaving}
                    className="border-amber-300 text-amber-700 hover:bg-amber-50"
                  >
                    {isSaving ? (
                      <>
                        <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                        Đang lưu...
                      </>
                    ) : (
                      <>
                        <Edit3 className="w-4 h-4 mr-2" />
                        {isEditing ? "Lưu" : "Chỉnh sửa"}
                      </>
                    )}
                  </Button>
                </div>
              </CardHeader>
              <CardContent className="space-y-6">
                {customerLoading ? (
                  <div className="flex items-center justify-center py-8">
                    <Loader2 className="w-6 h-6 animate-spin text-amber-600" />
                    <span className="ml-2 text-gray-600">Đang tải thông tin...</span>
                  </div>
                ) : customerError ? (
                  <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-red-700">
                    {customerError}
                  </div>
                ) : (
                  <div className="space-y-6">
                    <div className="grid md:grid-cols-2 gap-6">
                      <div>
                        <label className="text-sm text-gray-700 mb-2 block">
                          Họ và tên
                        </label>
                        <Input
                          value={userInfo.name}
                          onChange={(e) =>
                            setUserInfo({ ...userInfo, name: e.target.value })
                          }
                          disabled={!isEditing}
                          className="border-amber-200"
                        />
                      </div>

                      <div>
                        <label className="text-sm text-gray-700 mb-2 block">
                          Email
                        </label>
                        <Input
                          value={userInfo.email}
                          onChange={(e) =>
                            setUserInfo({ ...userInfo, email: e.target.value })
                          }
                          disabled={!isEditing}
                          className="border-amber-200"
                        />
                      </div>

                      <div>
                        <label className="text-sm text-gray-700 mb-2 block">
                          Số điện thoại
                        </label>
                        <Input
                          value={userInfo.phone}
                          onChange={(e) =>
                            setUserInfo({ ...userInfo, phone: e.target.value })
                          }
                          disabled={!isEditing}
                          className="border-amber-200"
                        />
                      </div>

                      <div>
                        <label className="text-sm text-gray-700 mb-2 block">
                          Ngày sinh
                        </label>
                        <Input
                          type="date"
                          value={userInfo.birthDate}
                          onChange={(e) =>
                            setUserInfo({ ...userInfo, birthDate: e.target.value })
                          }
                          disabled={!isEditing}
                          className="border-amber-200"
                        />
                      </div>
                    </div>

                    {/* Member Benefits */}
                    <Card className="bg-gradient-to-r from-amber-50 to-yellow-100 border-amber-200">
                      <CardContent className="p-6">
                        <div className="flex items-center gap-3 mb-4">
                          <Award className="w-6 h-6 text-amber-600" />
                          <h3 className="text-lg text-amber-900">
                            Quyền lợi thành viên {(customerData as any)?.customer?.tier || "VIP"}
                          </h3>
                        </div>
                        <div className="grid md:grid-cols-2 gap-4">
                          <div className="flex items-center gap-2">
                            <CheckCircle className="w-4 h-4 text-green-600" />
                            <span className="text-sm">
                              Giảm giá 15% tất cả đơn hàng
                            </span>
                          </div>
                          <div className="flex items-center gap-2">
                            <CheckCircle className="w-4 h-4 text-green-600" />
                            <span className="text-sm">Miễn phí vận chuyển</span>
                          </div>
                          <div className="flex items-center gap-2">
                            <CheckCircle className="w-4 h-4 text-green-600" />
                            <span className="text-sm">
                              Ưu tiên hỗ trợ khách hàng
                            </span>
                          </div>
                          <div className="flex items-center gap-2">
                            <CheckCircle className="w-4 h-4 text-green-600" />
                            <span className="text-sm">
                              Tư vấn phong thủy miễn phí
                            </span>
                          </div>
                        </div>
                      </CardContent>
                    </Card>

                    {/* Loyalty Points */}
                    <Card className="bg-gradient-to-r from-blue-50 to-indigo-100 border-blue-200">
                      <CardContent className="p-6">
                        <div className="flex items-center justify-between">
                          <div>
                            <h3 className="text-lg text-blue-900 mb-1">
                              Điểm thành viên
                            </h3>
                            <p className="text-sm text-blue-700">
                              Tích lũy từ các đơn hàng
                            </p>
                          </div>
                          <div className="text-right">
                            <div className="text-3xl font-bold text-blue-600">
                              {(customerData as any)?.customer?.loyaltyPoints || 0}
                            </div>
                            <p className="text-xs text-blue-600">điểm</p>
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  </div>
                )}
              </CardContent>
            </Card>
          </TabsContent>

          {/* Orders Tab */}
          <TabsContent value="orders" className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-amber-900">
                  <Package className="w-5 h-5" />
                  Lịch sử đơn hàng ({orders.length})
                </CardTitle>
              </CardHeader>
              <CardContent>
                {ordersLoading ? (
                  <div className="flex items-center justify-center py-8">
                    <Loader2 className="w-6 h-6 animate-spin text-amber-600" />
                    <span className="ml-2 text-gray-600">Đang tải đơn hàng...</span>
                  </div>
                ) : ordersError ? (
                  <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-red-700">
                    {ordersError}
                  </div>
                ) : orders.length === 0 ? (
                  <div className="p-8 text-center text-gray-600">
                    <Package className="w-12 h-12 mx-auto mb-3 text-gray-400" />
                    <p>Bạn chưa có đơn hàng nào</p>
                  </div>
                ) : (
                  <div className="space-y-4">
                    {orders.map((order) => (
                      <Card key={order.orderId} className="border-2 border-amber-100">
                        <CardContent className="p-4">
                          <div className="flex flex-col lg:flex-row gap-4">
                            {/* Order Info */}
                            <div className="flex-1">
                              <div className="flex flex-wrap items-center gap-4 mb-3">
                                <h3 className="text-amber-900">
                                  Đơn hàng #{order.orderNumber || order.orderId}
                                </h3>
                                <Badge className={getStatusColor(order.status)}>
                                  {getStatusText(order.status)}
                                </Badge>
                                <div className="flex items-center gap-1 text-gray-600 text-sm">
                                  <Calendar className="w-4 h-4" />
                                  {order.createdAt
                                    ? new Date(order.createdAt).toLocaleDateString(
                                        "vi-VN"
                                      )
                                    : "N/A"}
                                </div>
                              </div>

                              {/* Order Items */}
                              <div className="space-y-2">
                                {order.items && order.items.length > 0 ? (
                                  order.items.map((item: any, index: number) => (
                                    <div
                                      key={index}
                                      className="flex items-center gap-3"
                                    >
                                      {item.productImageUrl && (
                                        <ImageWithFallback
                                          src={item.productImageUrl}
                                          alt={item.productName || "Product"}
                                          className="w-12 h-12 object-cover rounded"
                                        />
                                      )}
                                      <div className="flex-1 min-w-0">
                                        <p className="text-sm line-clamp-1">
                                          {item.productName || "Sản phẩm"}
                                        </p>
                                        <p className="text-xs text-gray-600">
                                          Số lượng: {item.quantity} ×{" "}
                                          {formatPrice(item.unitPrice || 0)}
                                        </p>
                                      </div>
                                    </div>
                                  ))
                                ) : (
                                  <p className="text-sm text-gray-600">
                                    Không có sản phẩm
                                  </p>
                                )}
                              </div>
                            </div>

                            {/* Order Actions */}
                            <div className="flex flex-col items-end gap-2">
                              <div className="text-lg text-red-600">
                                {formatPrice(order.totalAmount || 0)}
                              </div>
                              <div className="flex gap-2">
                                <Button
                                  variant="outline"
                                  size="sm"
                                  className="border-amber-300 text-amber-700"
                                  onClick={() => navigate(`/orders/${order.orderId}`)}
                                >
                                  <Eye className="w-4 h-4 mr-1" />
                                  Xem chi tiết
                                </Button>
                                {order.status?.toLowerCase() === "delivered" && (
                                  <Button
                                    variant="outline"
                                    size="sm"
                                    className="border-blue-300 text-blue-700"
                                  >
                                    Mua lại
                                  </Button>
                                )}
                              </div>
                            </div>
                          </div>
                        </CardContent>
                      </Card>
                    ))}
                  </div>
                )}
              </CardContent>
            </Card>
          </TabsContent>

          {/* Addresses Tab */}
          <TabsContent value="addresses" className="space-y-6">
            <Card>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2 text-amber-900">
                    <MapPin className="w-5 h-5" />
                    Sổ địa chỉ ({addresses.length})
                  </CardTitle>
                  <Button 
                    className="bg-red-600 hover:bg-red-700 text-white"
                    onClick={handleAddAddress}
                  >
                    Thêm địa chỉ mới
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                {addressesLoading ? (
                  <div className="flex items-center justify-center py-8">
                    <Loader2 className="w-6 h-6 animate-spin text-amber-600" />
                    <span className="ml-2 text-gray-600">Đang tải địa chỉ...</span>
                  </div>
                ) : addressesError ? (
                  <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-red-700">
                    {addressesError}
                  </div>
                ) : addresses.length === 0 ? (
                  <div className="p-8 text-center text-gray-600">
                    <MapPin className="w-12 h-12 mx-auto mb-3 text-gray-400" />
                    <p>Bạn chưa có địa chỉ nào</p>
                  </div>
                ) : (
                  <div className="space-y-4">
                    {addresses.map((address) => (
                      <Card
                        key={address.id}
                        className={`border-2 ${
                          address.isDefault
                            ? "border-amber-300 bg-amber-50"
                            : "border-gray-200"
                        }`}
                      >
                        <CardContent className="p-4">
                          <div className="flex justify-between items-start">
                            <div className="flex-1">
                              <div className="flex items-center gap-2 mb-2">
                                <h3 className="text-amber-900">{address.recipientName || "Địa chỉ"}</h3>
                                {address.isDefault && (
                                  <Badge className="bg-amber-600 text-white text-xs">
                                    Mặc định
                                  </Badge>
                                )}
                              </div>
                              <p className="text-gray-700 mb-1">
                                {address.streetAddress}
                              </p>
                              {address.city && (
                                <p className="text-gray-600 text-sm mb-1">
                                  {address.city}
                                  {address.country && `, ${address.country}`}
                                </p>
                              )}
                              <div className="flex items-center gap-1 text-gray-600">
                                <Phone className="w-4 h-4" />
                                <span>{address.phoneNumber || "N/A"}</span>
                              </div>
                            </div>

                            <div className="flex gap-2">
                              <Button
                                variant="outline"
                                size="sm"
                                className="border-amber-300 text-amber-700"
                                onClick={() => handleEditAddress(address.id!)}
                              >
                                <Edit3 className="w-4 h-4" />
                              </Button>
                              {!address.isDefault && (
                                <>
                                  <Button
                                    variant="outline"
                                    size="sm"
                                    className="border-green-300 text-green-700"
                                    onClick={() => handleSetDefaultAddress(address.id!)}
                                  >
                                    Đặt mặc định
                                  </Button>
                                  <Button
                                    variant="outline"
                                    size="sm"
                                    className="border-red-300 text-red-700"
                                    onClick={() => handleDeleteAddress(address.id!)}
                                  >
                                    Xóa
                                  </Button>
                                </>
                              )}
                            </div>
                          </div>
                        </CardContent>
                      </Card>
                    ))}
                  </div>
                )}
              </CardContent>
            </Card>
          </TabsContent>

          {/* Notifications Tab */}
          <TabsContent value="notifications" className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-amber-900">
                  <Bell className="w-5 h-5" />
                  Cài đặt thông báo
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-6">
                <div className="space-y-4">
                  <div className="flex items-center justify-between p-4 border border-amber-200 rounded-lg">
                    <div>
                      <h3 className="text-amber-900 mb-1">Cập nhật đơn hàng</h3>
                      <p className="text-sm text-gray-600">
                        Nhận thông báo về trạng thái đơn hàng
                      </p>
                    </div>
                    <Switch
                      checked={notifications.orderUpdates}
                      onCheckedChange={(checked: boolean) =>
                        setNotifications({
                          ...notifications,
                          orderUpdates: checked,
                        })
                      }
                    />
                  </div>

                  <div className="flex items-center justify-between p-4 border border-amber-200 rounded-lg">
                    <div>
                      <h3 className="text-amber-900 mb-1">
                        Khuyến mãi & Ưu đãi
                      </h3>
                      <p className="text-sm text-gray-600">
                        Nhận thông báo về các chương trình khuyến mãi
                      </p>
                    </div>
                    <Switch
                      checked={notifications.promotions}
                      onCheckedChange={(checked: boolean) =>
                        setNotifications({
                          ...notifications,
                          promotions: checked,
                        })
                      }
                    />
                  </div>

                  <div className="flex items-center justify-between p-4 border border-amber-200 rounded-lg">
                    <div>
                      <h3 className="text-amber-900 mb-1">Tin tức & Bản tin</h3>
                      <p className="text-sm text-gray-600">
                        Nhận bản tin về phong thủy và nghi lễ
                      </p>
                    </div>
                    <Switch
                      checked={notifications.newsletter}
                      onCheckedChange={(checked: boolean) =>
                        setNotifications({
                          ...notifications,
                          newsletter: checked,
                        })
                      }
                    />
                  </div>

                  <div className="flex items-center justify-between p-4 border border-amber-200 rounded-lg">
                    <div>
                      <h3 className="text-amber-900 mb-1">Thông báo SMS</h3>
                      <p className="text-sm text-gray-600">
                        Nhận thông báo qua tin nhắn SMS
                      </p>
                    </div>
                    <Switch
                      checked={notifications.smsNotifications}
                      onCheckedChange={(checked: boolean) =>
                        setNotifications({
                          ...notifications,
                          smsNotifications: checked,
                        })
                      }
                    />
                  </div>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          {/* Security Tab */}
          <TabsContent value="security" className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-amber-900">
                  <Shield className="w-5 h-5" />
                  Bảo mật tài khoản
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-6">
                <div className="space-y-4">
                  <div className="p-4 border border-amber-200 rounded-lg">
                    <h3 className="text-amber-900 mb-2">Đổi mật khẩu</h3>
                    <p className="text-sm text-gray-600 mb-4">
                      Cập nhật mật khẩu để bảo mật tài khoản
                    </p>
                    <Button
                      variant="outline"
                      className="border-amber-300 text-amber-700 hover:bg-amber-50"
                    >
                      Đổi mật khẩu
                    </Button>
                  </div>

                  <div className="p-4 border border-amber-200 rounded-lg">
                    <h3 className="text-amber-900 mb-2">Xác thực hai bước</h3>
                    <p className="text-sm text-gray-600 mb-4">
                      Tăng bảo mật với xác thực qua SMS
                    </p>
                    <div className="flex items-center justify-between">
                      <Badge className="bg-green-100 text-green-800">
                        Đã bật
                      </Badge>
                      <Button
                        variant="outline"
                        size="sm"
                        className="border-amber-300 text-amber-700"
                      >
                        Cài đặt
                      </Button>
                    </div>
                  </div>

                  <div className="p-4 border border-amber-200 rounded-lg">
                    <h3 className="text-amber-900 mb-2">Phiên đăng nhập</h3>
                    <p className="text-sm text-gray-600 mb-4">
                      Quản lý các thiết bị đã đăng nhập
                    </p>
                    <Button
                      variant="outline"
                      className="border-amber-300 text-amber-700 hover:bg-amber-50"
                    >
                      Xem chi tiết
                    </Button>
                  </div>

                  <div className="p-4 border border-blue-200 rounded-lg bg-blue-50">
                    <h3 className="text-blue-900 mb-2">Đăng xuất tài khoản</h3>
                    <p className="text-sm text-blue-700 mb-4">
                      Đăng xuất khỏi tài khoản trên thiết bị này
                    </p>
                    <Button
                      variant="outline"
                      className="border-blue-600 text-blue-700 hover:bg-blue-100"
                      onClick={() => setShowLogoutDialog(true)}
                      disabled={isLoading}
                    >
                      <LogOut className="w-4 h-4 mr-2" />
                      Đăng xuất
                    </Button>
                  </div>

                  <div className="p-4 border border-red-200 rounded-lg bg-red-50">
                    <h3 className="text-red-900 mb-2">Xóa tài khoản</h3>
                    <p className="text-sm text-red-700 mb-4">
                      Xóa vĩnh viễn tài khoản và dữ liệu
                    </p>
                    <Button variant="destructive" size="sm">
                      Xóa tài khoản
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </div>

      {/* Address Dialog */}
      <AddressDialog
        open={addressDialogOpen}
        onOpenChange={setAddressDialogOpen}
        address={selectedAddress}
        onSuccess={handleAddressDialogSuccess}
      />

      {/* Logout Dialog */}
      <AlertDialog open={showLogoutDialog} onOpenChange={setShowLogoutDialog}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Đăng xuất tài khoản</AlertDialogTitle>
            <AlertDialogDescription>
              Bạn có chắc chắn muốn đăng xuất khỏi tài khoản này không?
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel className="border-amber-300 text-amber-700 hover:bg-amber-50">
              Hủy bỏ
            </AlertDialogCancel>
            <AlertDialogAction
              className="bg-red-600 hover:bg-red-700 text-white"
              onClick={() => {
                logout();
                setShowLogoutDialog(false);
              }}
            >
              Đăng xuất
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}

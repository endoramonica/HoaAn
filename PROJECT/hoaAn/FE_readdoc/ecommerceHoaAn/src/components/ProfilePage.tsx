import { useState } from "react";
import { Button } from "./ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "./ui/card";
import { Badge } from "./ui/badge";
import { Input } from "./ui/input";
import { Textarea } from "./ui/textarea";
import { Separator } from "./ui/separator";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "./ui/tabs";
import { Avatar, AvatarFallback, AvatarImage } from "./ui/avatar";
import { Switch } from "./ui/switch";
import { ImageWithFallback } from "./figma/ImageWithFallback";
import { useAuth } from "../lib/hooks/useAuth";
import type { UserDetailDTO } from "/models/UserDetailDTO";

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
  Settings,
  Package,
  MapPin,
  Bell,
  Shield,
  Edit3,
  Camera,
  Star,
  Clock,
  Truck,
  Eye,
  Heart,
  CreditCard,
  Phone,
  Mail,
  Flower2,
  Calendar,
  Gift,
  Award,
  CheckCircle,
  LogOut,
} from "lucide-react";

interface Order {
  id: string;
  date: string;
  status: "pending" | "processing" | "shipped" | "delivered" | "cancelled";
  total: number;
  items: {
    name: string;
    quantity: number;
    price: number;
    image: string;
  }[];
}

interface Address {
  id: string;
  name: string;
  phone: string;
  address: string;
  isDefault: boolean;
}

export function ProfilePage() {
  const [activeTab, setActiveTab] = useState("profile");
  const [isEditing, setIsEditing] = useState(false);
  const { logout, isLoading } = useAuth();
  const [showLogoutDialog, setShowLogoutDialog] = useState(false);

  const [userInfo, setUserInfo] = useState({
    name: "Nguyễn Văn An",
    email: "nguyenvanan@email.com",
    phone: "0912 345 678",
    birthDate: "1985-03-15",
    gender: "male",
    avatar:
      "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150&h=150&fit=crop&crop=face",
  });

  const [addresses, setAddresses] = useState<Address[]>([
    {
      id: "1",
      name: "Nguyễn Văn An",
      phone: "0912 345 678",
      address: "123 Đường Láng, Phường Láng Thượng, Quận Đống Đa, Hà Nội",
      isDefault: true,
    },
    {
      id: "2",
      name: "Nguyễn Văn An",
      phone: "0912 345 678",
      address:
        "456 Nguyễn Trãi, Phường Thanh Xuân Trung, Quận Thanh Xuân, Hà Nội",
      isDefault: false,
    },
  ]);

  const [orders] = useState<Order[]>([
    {
      id: "DH001",
      date: "2025-01-10",
      status: "delivered",
      total: 2890000,
      items: [
        {
          name: "Mâm Cúng Trọn Gói Cao Cấp",
          quantity: 1,
          price: 2890000,
          image:
            "https://images.unsplash.com/photo-1519097000072-e44ffa116485?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwb2ZmZXJpbmdzJTIwYWx0YXIlMjBmcnVpdHN8ZW58MXx8fHwxNzU3Njc0NDI5fDA&ixlib=rb-4.1.0&q=80&w=1080",
        },
      ],
    },
    {
      id: "DH002",
      date: "2025-01-08",
      status: "processing",
      total: 730000,
      items: [
        {
          name: "Hương Trầm Cao Cấp - Hộp 100 Cây",
          quantity: 2,
          price: 450000,
          image:
            "https://images.unsplash.com/photo-1532334722716-c5850cdd878d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx2aWV0bmFtZXNlJTIwaW5jZW5zZSUyMGNlcmVtb255JTIwdHJhZGl0aW9uYWx8ZW58MXx8fHwxNzU3Njc0NDI4fDA&ixlib=rb-4.1.0&q=80&w=1080",
        },
        {
          name: "Nến Đỏ Phong Thủy - Bộ 12 Cây",
          quantity: 1,
          price: 280000,
          image:
            "https://images.unsplash.com/photo-1732117924212-39bfaec174c9?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHx0cmFkaXRpb25hbCUyMGNhbmRsZXMlMjByZWQlMjBnb2xkfGVufDF8fHx8MTc1NzY3NDQyOXww&ixlib=rb-4.1.0&q=80&w=1080",
        },
      ],
    },
    {
      id: "DH003",
      date: "2025-01-05",
      status: "shipped",
      total: 1250000,
      items: [
        {
          name: "Dịch Vụ Cúng Gia Tiên Trọn Gói",
          quantity: 1,
          price: 1250000,
          image:
            "https://images.unsplash.com/photo-1573460630303-81cbaf895c54?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsb3R1cyUyMGZsb3dlciUyMGNlcmVtb25pYWwlMjBnb2xkfGVufDF8fHx8MTc1NzY3NDQyOXww&ixlib=rb-4.1.0&q=80&w=1080",
        },
      ],
    },
  ]);

  const [notifications, setNotifications] = useState({
    orderUpdates: true,
    promotions: true,
    newsletter: false,
    smsNotifications: true,
  });

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("vi-VN").format(price) + "₫";
  };

  const getStatusColor = (status: Order["status"]) => {
    switch (status) {
      case "pending":
        return "bg-yellow-100 text-yellow-800";
      case "processing":
        return "bg-blue-100 text-blue-800";
      case "shipped":
        return "bg-purple-100 text-purple-800";
      case "delivered":
        return "bg-green-100 text-green-800";
      case "cancelled":
        return "bg-red-100 text-red-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  const getStatusText = (status: Order["status"]) => {
    switch (status) {
      case "pending":
        return "Chờ xác nhận";
      case "processing":
        return "Đang xử lý";
      case "shipped":
        return "Đang giao";
      case "delivered":
        return "Đã giao";
      case "cancelled":
        return "Đã hủy";
      default:
        return "Không xác định";
    }
  };

  const handleSaveProfile = () => {
    setIsEditing(false);
    // Handle save logic here
  };

  const totalSpent = orders.reduce((sum, order) => sum + order.total, 0);
  const completedOrders = orders.filter(
    (order) => order.status === "delivered"
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
                    className="border-amber-300 text-amber-700 hover:bg-amber-50"
                  >
                    <Edit3 className="w-4 h-4 mr-2" />
                    {isEditing ? "Lưu" : "Chỉnh sửa"}
                  </Button>
                </div>
              </CardHeader>
              <CardContent className="space-y-6">
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
                        Quyền lợi thành viên VIP
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
                <div className="space-y-4">
                  {orders.map((order) => (
                    <Card key={order.id} className="border-2 border-amber-100">
                      <CardContent className="p-4">
                        <div className="flex flex-col lg:flex-row gap-4">
                          {/* Order Info */}
                          <div className="flex-1">
                            <div className="flex flex-wrap items-center gap-4 mb-3">
                              <h3 className="text-amber-900">
                                Đơn hàng #{order.id}
                              </h3>
                              <Badge className={getStatusColor(order.status)}>
                                {getStatusText(order.status)}
                              </Badge>
                              <div className="flex items-center gap-1 text-gray-600 text-sm">
                                <Calendar className="w-4 h-4" />
                                {new Date(order.date).toLocaleDateString(
                                  "vi-VN"
                                )}
                              </div>
                            </div>

                            {/* Order Items */}
                            <div className="space-y-2">
                              {order.items.map((item, index) => (
                                <div
                                  key={index}
                                  className="flex items-center gap-3"
                                >
                                  <ImageWithFallback
                                    src={item.image}
                                    alt={item.name}
                                    className="w-12 h-12 object-cover rounded"
                                  />
                                  <div className="flex-1 min-w-0">
                                    <p className="text-sm line-clamp-1">
                                      {item.name}
                                    </p>
                                    <p className="text-xs text-gray-600">
                                      Số lượng: {item.quantity} ×{" "}
                                      {formatPrice(item.price)}
                                    </p>
                                  </div>
                                </div>
                              ))}
                            </div>
                          </div>

                          {/* Order Actions */}
                          <div className="flex flex-col items-end gap-2">
                            <div className="text-lg text-red-600">
                              {formatPrice(order.total)}
                            </div>
                            <div className="flex gap-2">
                              <Button
                                variant="outline"
                                size="sm"
                                className="border-amber-300 text-amber-700"
                              >
                                <Eye className="w-4 h-4 mr-1" />
                                Xem chi tiết
                              </Button>
                              {order.status === "delivered" && (
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
                  <Button className="bg-red-600 hover:bg-red-700 text-white">
                    Thêm địa chỉ mới
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
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
                              <h3 className="text-amber-900">{address.name}</h3>
                              {address.isDefault && (
                                <Badge className="bg-amber-600 text-white text-xs">
                                  Mặc định
                                </Badge>
                              )}
                            </div>
                            <p className="text-gray-700 mb-1">
                              {address.address}
                            </p>
                            <div className="flex items-center gap-1 text-gray-600">
                              <Phone className="w-4 h-4" />
                              <span>{address.phone}</span>
                            </div>
                          </div>

                          <div className="flex gap-2">
                            <Button
                              variant="outline"
                              size="sm"
                              className="border-amber-300 text-amber-700"
                            >
                              <Edit3 className="w-4 h-4" />
                            </Button>
                            {!address.isDefault && (
                              <Button
                                variant="outline"
                                size="sm"
                                className="border-red-300 text-red-700"
                              >
                                Xóa
                              </Button>
                            )}
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  ))}
                </div>
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
                      onCheckedChange={(checked) =>
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
                      onCheckedChange={(checked) =>
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
                      onCheckedChange={(checked) =>
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
                      onCheckedChange={(checked) =>
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

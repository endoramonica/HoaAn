import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { Button } from "../components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "../components/ui/card";
import { Badge } from "../components/ui/badge";
import { Separator } from "../components/ui/separator";
import { ImageWithFallback } from "../components/figma/ImageWithFallback";
import { orderService } from "../lib/services/orderService";
import type { OrderDetailDto } from "../../Api/generated-orval/schemas";
import {
  ArrowLeft,
  Package,
  Truck,
  MapPin,
  Phone,
  Calendar,
  DollarSign,
  Loader2,
} from "lucide-react";

export function OrderDetailPage() {
  const { orderId } = useParams<{ orderId: string }>();
  const navigate = useNavigate();
  const [order, setOrder] = useState<OrderDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadOrder = async () => {
      if (!orderId) {
        setError("Không tìm thấy ID đơn hàng");
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        setError(null);
        const orderData = await orderService.getOrderById(orderId);
        setOrder(orderData);
      } catch (err) {
        console.error("Failed to load order:", err);
        setError("Không thể tải chi tiết đơn hàng");
      } finally {
        setLoading(false);
      }
    };

    loadOrder();
  }, [orderId]);

  const formatPrice = (price: number | undefined) => {
    if (!price) return "0₫";
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

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50 flex items-center justify-center">
        <div className="flex flex-col items-center gap-3">
          <Loader2 className="w-8 h-8 animate-spin text-amber-600" />
          <p className="text-gray-600">Đang tải chi tiết đơn hàng...</p>
        </div>
      </div>
    );
  }

  if (error || !order) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50 p-4">
        <div className="max-w-4xl mx-auto">
          <Button
            variant="outline"
            onClick={() => navigate(-1)}
            className="mb-6 border-amber-300 text-amber-700"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Quay lại
          </Button>
          <Card className="border-red-200 bg-red-50">
            <CardContent className="p-6 text-center">
              <p className="text-red-700">{error || "Không tìm thấy đơn hàng"}</p>
            </CardContent>
          </Card>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-yellow-50 to-red-50 p-4">
      <div className="max-w-4xl mx-auto">
        {/* Header */}
        <div className="mb-6">
          <Button
            variant="outline"
            onClick={() => navigate(-1)}
            className="mb-4 border-amber-300 text-amber-700"
          >
            <ArrowLeft className="w-4 h-4 mr-2" />
            Quay lại
          </Button>

          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div>
              <h1 className="text-3xl font-bold text-amber-900">
                Đơn hàng #{order.orderNumber || order.orderId}
              </h1>
              <p className="text-gray-600 mt-1">
                {order.createdAt
                  ? new Date(order.createdAt).toLocaleDateString("vi-VN", {
                      year: "numeric",
                      month: "long",
                      day: "numeric",
                      hour: "2-digit",
                      minute: "2-digit",
                    })
                  : "N/A"}
              </p>
            </div>
            <Badge className={`${getStatusColor(order.status)} text-lg px-4 py-2`}>
              {getStatusText(order.status)}
            </Badge>
          </div>
        </div>

        {/* Order Items */}
        <Card className="mb-6 border-2 border-amber-200">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-amber-900">
              <Package className="w-5 h-5" />
              Sản phẩm trong đơn hàng
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {order.items && order.items.length > 0 ? (
                order.items.map((item, index) => (
                  <div key={index}>
                    <div className="flex gap-4">
                      {item.productImageUrl && (
                        <ImageWithFallback
                          src={item.productImageUrl}
                          alt={item.productName || "Product"}
                          className="w-24 h-24 object-cover rounded-lg"
                        />
                      )}
                      <div className="flex-1">
                        <h3 className="font-semibold text-amber-900 mb-2">
                          {item.productName || "Sản phẩm"}
                        </h3>
                        <div className="grid grid-cols-2 gap-2 text-sm text-gray-600">
                          <div>
                            <span className="text-gray-500">SKU:</span>{" "}
                            {item.productSKU || "N/A"}
                          </div>
                          <div>
                            <span className="text-gray-500">Số lượng:</span>{" "}
                            {item.quantity}
                          </div>
                          <div>
                            <span className="text-gray-500">Đơn giá:</span>{" "}
                            {formatPrice(item.unitPrice)}
                          </div>
                          <div>
                            <span className="text-gray-500">Thành tiền:</span>{" "}
                            <span className="font-semibold text-red-600">
                              {formatPrice(item.totalPrice)}
                            </span>
                          </div>
                        </div>
                      </div>
                    </div>
                    {index < (order.items?.length || 0) - 1 && (
                      <Separator className="mt-4" />
                    )}
                  </div>
                ))
              ) : (
                <p className="text-gray-600 text-center py-4">
                  Không có sản phẩm trong đơn hàng
                </p>
              )}
            </div>
          </CardContent>
        </Card>

        {/* Order Summary */}
        <Card className="mb-6 border-2 border-amber-200">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-amber-900">
              <DollarSign className="w-5 h-5" />
              Tóm tắt đơn hàng
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <div className="flex justify-between">
                <span className="text-gray-600">Tạm tính:</span>
                <span className="font-semibold">{formatPrice(order.subTotal)}</span>
              </div>
              {order.discountAmount ? (
                <div className="flex justify-between text-green-600">
                  <span>Giảm giá:</span>
                  <span className="font-semibold">
                    -{formatPrice(order.discountAmount)}
                  </span>
                </div>
              ) : null}
              {order.shippingFee ? (
                <div className="flex justify-between">
                  <span className="text-gray-600">Phí vận chuyển:</span>
                  <span className="font-semibold">
                    {formatPrice(order.shippingFee)}
                  </span>
                </div>
              ) : null}
              {order.taxAmount ? (
                <div className="flex justify-between">
                  <span className="text-gray-600">Thuế:</span>
                  <span className="font-semibold">{formatPrice(order.taxAmount)}</span>
                </div>
              ) : null}
              <Separator />
              <div className="flex justify-between text-lg">
                <span className="font-bold text-amber-900">Tổng cộng:</span>
                <span className="font-bold text-red-600">
                  {formatPrice(order.totalAmount)}
                </span>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Shipping Information */}
        {order.shipping && (
          <Card className="mb-6 border-2 border-amber-200">
            <CardHeader>
              <CardTitle className="flex items-center gap-2 text-amber-900">
                <Truck className="w-5 h-5" />
                Thông tin giao hàng
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                <div>
                  <p className="text-gray-500 text-sm">Người nhận</p>
                  <p className="font-semibold text-amber-900">
                    {order.shipping.recipientName || "N/A"}
                  </p>
                </div>
                <div className="flex-1">
                  <p className="text-gray-500 text-sm flex items-center gap-1">
                    <Phone className="w-4 h-4" />
                    Số điện thoại
                  </p>
                  <p className="font-semibold">
                    {order.shipping.phoneNumber || "N/A"}
                  </p>
                </div>
                <div>
                  <p className="text-gray-500 text-sm flex items-center gap-1">
                    <MapPin className="w-4 h-4" />
                    Địa chỉ giao hàng
                  </p>
                  <p className="font-semibold">
                    {order.shipping.address || "N/A"}
                  </p>
                </div>
                {order.shipping.ward && (
                  <div className="text-sm text-gray-600">
                    {order.shipping.ward}, {order.shipping.district},{" "}
                    {order.shipping.city}
                  </div>
                )}
                {order.shipping.trackingNumber && (
                  <div>
                    <p className="text-gray-500 text-sm">Mã vận đơn</p>
                    <p className="font-semibold">{order.shipping.trackingNumber}</p>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        )}

        {/* Payment Information */}
        <Card className="border-2 border-amber-200">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-amber-900">
              <DollarSign className="w-5 h-5" />
              Thông tin thanh toán
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <div className="flex justify-between">
                <span className="text-gray-600">Phương thức thanh toán:</span>
                <span className="font-semibold">
                  {order.paymentMethod || "N/A"}
                </span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Trạng thái thanh toán:</span>
                <Badge className={order.isPaid ? "bg-green-100 text-green-800" : "bg-yellow-100 text-yellow-800"}>
                  {order.isPaid ? "Đã thanh toán" : "Chưa thanh toán"}
                </Badge>
              </div>
              {order.paidAmount && (
                <div className="flex justify-between">
                  <span className="text-gray-600">Số tiền đã thanh toán:</span>
                  <span className="font-semibold">
                    {formatPrice(order.paidAmount)}
                  </span>
                </div>
              )}
              {order.paidAt && (
                <div className="flex justify-between">
                  <span className="text-gray-600 flex items-center gap-1">
                    <Calendar className="w-4 h-4" />
                    Ngày thanh toán:
                  </span>
                  <span className="font-semibold">
                    {new Date(order.paidAt).toLocaleDateString("vi-VN")}
                  </span>
                </div>
              )}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

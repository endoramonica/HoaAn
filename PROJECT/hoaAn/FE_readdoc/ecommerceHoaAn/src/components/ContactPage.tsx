import { useState } from "react";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "./ui/card";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Textarea } from "./ui/textarea";
import { Label } from "./ui/label";
import { useNavigate } from "react-router-dom";
import {
  MapPin,
  Phone,
  Mail,
  Clock,
  MessageCircle,
  Flower2,
  Send,
} from "lucide-react";

// Định nghĩa props (nếu cần)
interface ContactPageProps {
  onNavigate?: () => void;
}

export function ContactPage({
  onNavigate,
}: ContactPageProps = {}) {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    phone: "",
    subject: "",
    message: "",
  });

  const handleSubmit = (
    e: React.FormEvent<HTMLFormElement>,
  ) => {
    e.preventDefault();
    console.log("Form submitted:", formData);

    // Reset form
    setFormData({
      name: "",
      email: "",
      phone: "",
      subject: "",
      message: "",
    });

    // Có thể thêm thông báo thành công
    alert(
      "Tin nhắn đã được gửi thành công! Chúng tôi sẽ phản hồi trong 24h.",
    );
  };

  const handleInputChange = (
    e: React.ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement
    >,
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const contactInfo = [
    {
      icon: Phone,
      title: "Hotline",
      details: ["1900 1234 (24/7)", "024 3838 1234"],
      color: "text-green-600",
    },
    {
      icon: Mail,
      title: "Email",
      details: [
        "support@docungonline.vn",
        "contact@docungonline.vn",
      ],
      color: "text-blue-600",
    },
    {
      icon: MapPin,
      title: "Địa chỉ",
      details: [
        "123 Đường Láng, Đống Đa, Hà Nội",
        "Tầng 5, Tòa nhà ABC",
      ],
      color: "text-red-600",
    },
    {
      icon: Clock,
      title: "Giờ làm việc",
      details: [
        "Thứ 2 - Chủ nhật: 8:00 - 22:00",
        "Tư vấn 24/7 qua hotline",
      ],
      color: "text-purple-600",
    },
  ];

  const socialLinks = [
    {
      icon: MessageCircle,
      name: "Zalo",
      url: "#",
      color: "bg-blue-500",
    },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Hero Section */}
      <section className="relative py-20 bg-gradient-to-r from-amber-900 to-red-800 text-white">
        <div className="max-w-6xl mx-auto px-4 text-center">
          <Flower2 className="w-16 h-16 mx-auto mb-6 text-yellow-300" />
          <h1 className="text-4xl md:text-5xl font-bold mb-6">
            Liên hệ với chúng tôi
          </h1>
          <p className="text-xl text-yellow-100 max-w-2xl mx-auto">
            Chúng tôi luôn sẵn sàng hỗ trợ và tư vấn cho bạn về
            các sản phẩm và dịch vụ
          </p>
        </div>
      </section>

      {/* Contact Info Cards */}
      <section className="py-16 bg-gradient-to-br from-yellow-50 to-red-50">
        <div className="max-w-6xl mx-auto px-4">
          <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
            {contactInfo.map((info, index) => {
              const Icon = info.icon;
              return (
                <Card
                  key={index}
                  className="text-center hover:shadow-xl transition-all duration-300 border-2 border-amber-200 bg-white"
                >
                  <CardContent className="p-6">
                    <div className="w-16 h-16 mx-auto mb-4 bg-gradient-to-br from-amber-400 to-red-500 rounded-full flex items-center justify-center shadow-md">
                      <Icon className="w-8 h-8 text-white" />
                    </div>
                    <h3 className="text-lg font-semibold text-amber-900 mb-3">
                      {info.title}
                    </h3>
                    {info.details.map((detail, idx) => (
                      <p
                        key={idx}
                        className="text-gray-600 text-sm mb-1"
                      >
                        {detail}
                      </p>
                    ))}
                  </CardContent>
                </Card>
              );
            })}
          </div>
        </div>
      </section>

      {/* Contact Form & Map */}
      <section className="py-16 bg-white">
        <div className="max-w-6xl mx-auto px-4">
          <div className="grid lg:grid-cols-2 gap-12">
            {/* Contact Form */}
            <div>
              <Card className="border-2 border-amber-200 shadow-lg">
                <CardHeader>
                  <CardTitle className="text-2xl text-amber-900 flex items-center gap-2">
                    <Send className="w-6 h-6" />
                    Gửi tin nhắn cho chúng tôi
                  </CardTitle>
                  <p className="text-gray-600">
                    Điền thông tin vào form bên dưới, chúng tôi
                    sẽ phản hồi trong vòng 24h
                  </p>
                </CardHeader>
                <CardContent>
                  <form
                    onSubmit={handleSubmit}
                    className="space-y-4"
                  >
                    <div className="grid md:grid-cols-2 gap-4">
                      <div>
                        <Label
                          htmlFor="name"
                          className="text-sm text-gray-700 mb-2 block"
                        >
                          Họ và tên{" "}
                          <span className="text-red-500">
                            *
                          </span>
                        </Label>
                        <Input
                          id="name"
                          name="name"
                          value={formData.name}
                          onChange={handleInputChange}
                          placeholder="Nhập họ và tên"
                          required
                          className="border-amber-200 focus:border-amber-400 focus:ring-amber-400"
                        />
                      </div>
                      <div>
                        <Label
                          htmlFor="phone"
                          className="text-sm text-gray-700 mb-2 block"
                        >
                          Số điện thoại{" "}
                          <span className="text-red-500">
                            *
                          </span>
                        </Label>
                        <Input
                          id="phone"
                          name="phone"
                          type="tel"
                          value={formData.phone}
                          onChange={handleInputChange}
                          placeholder="Nhập số điện thoại"
                          required
                          className="border-amber-200 focus:border-amber-400 focus:ring-amber-400"
                        />
                      </div>
                    </div>

                    <div>
                      <Label
                        htmlFor="email"
                        className="text-sm text-gray-700 mb-2 block"
                      >
                        Email
                      </Label>
                      <Input
                        id="email"
                        name="email"
                        type="email"
                        value={formData.email}
                        onChange={handleInputChange}
                        placeholder="Nhập địa chỉ email"
                        className="border-amber-200 focus:border-amber-400 focus:ring-amber-400"
                      />
                    </div>

                    <div>
                      <Label
                        htmlFor="subject"
                        className="text-sm text-gray-700 mb-2 block"
                      >
                        Chủ đề
                      </Label>
                      <Input
                        id="subject"
                        name="subject"
                        value={formData.subject}
                        onChange={handleInputChange}
                        placeholder="Chọn chủ đề cần tư vấn"
                        className="border-amber-200 focus:border-amber-400 focus:ring-amber-400"
                      />
                    </div>

                    <div>
                      <Label
                        htmlFor="message"
                        className="text-sm text-gray-700 mb-2 block"
                      >
                        Nội dung tin nhắn{" "}
                        <span className="text-red-500">*</span>
                      </Label>
                      <Textarea
                        id="message"
                        name="message"
                        value={formData.message}
                        onChange={handleInputChange}
                        placeholder="Nhập nội dung cần tư vấn..."
                        rows={6}
                        required
                        className="border-amber-200 focus:border-amber-400 focus:ring-amber-400 resize-none"
                      />
                    </div>

                    <Button
                      type="submit"
                      className="w-full bg-gradient-to-r from-red-600 to-red-700 hover:from-red-700 hover:to-red-800 text-white py-6 text-lg font-medium transition-all"
                    >
                      <Send className="w-5 h-5 mr-2" />
                      Gửi tin nhắn ngay
                    </Button>
                  </form>
                </CardContent>
              </Card>
            </div>

            {/* Map & Additional Info */}
            <div className="space-y-6">
              {/* Map */}
              <Card className="border-2 border-amber-200 shadow-lg overflow-hidden">
                <CardHeader>
                  <CardTitle className="text-xl text-amber-900">
                    Vị trí cửa hàng
                  </CardTitle>
                </CardHeader>
                <CardContent className="p-0">
                  <div className="w-full h-64 bg-gray-200 relative">
                    <iframe
                      src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3724.4967724908127!2d105.81411931533315!3d21.01624419358892!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3135ab86ec85469f%3A0x5d6b3d0e3c6b7c8a!2zTMO0dCBzw6F0IE5nw7RpIHTDrA!5e0!3m2!1svi!2s!4v1234567890123"
                      width="100%"
                      height="100%"
                      style={{ border: 0 }}
                      allowFullScreen
                      loading="lazy"
                      referrerPolicy="no-referrer-when-downgrade"
                      title="Bản đồ vị trí cửa hàng Đồ Cúng Online"
                      className="absolute inset-0"
                    />
                  </div>
                </CardContent>
              </Card>

              {/* Quick Actions */}
              <Card className="border-2 border-amber-200 shadow-lg">
                <CardHeader>
                  <CardTitle className="text-xl text-amber-900">
                    Kết nối nhanh
                  </CardTitle>
                  <p className="text-gray-600">
                    Liên hệ trực tiếp qua các kênh sau
                  </p>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="grid gap-3">
                    <Button className="w-full bg-green-600 hover:bg-green-700 text-white justify-start text-lg py-6">
                      <Phone className="w-5 h-5 mr-3" />
                      Gọi ngay: 1900 1234
                    </Button>
                    <Button
                      variant="outline"
                      className="w-full border-blue-500 text-blue-600 hover:bg-blue-50 justify-start text-lg py-6"
                      onClick={() =>
                        window.open(
                          "https://zalo.me/docungonline",
                          "_blank",
                        )
                      }
                    >
                      <MessageCircle className="w-5 h-5 mr-3" />
                      Chat Zalo: @docungonline
                    </Button>
                    <Button
                      variant="outline"
                      className="w-full border-red-500 text-red-600 hover:bg-red-50 justify-start text-lg py-6"
                      onClick={() =>
                        (window.location.href =
                          "mailto:support@docungonline.vn")
                      }
                    >
                      <Mail className="w-5 h-5 mr-3" />
                      Email: support@docungonline.vn
                    </Button>
                  </div>

                  <div className="border-t pt-4">
                    <p className="text-sm text-gray-600 mb-3">
                      Theo dõi chúng tôi:
                    </p>
                    <div className="flex gap-3">
                      {socialLinks.map((social, index) => {
                        const Icon = social.icon;
                        return (
                          <Button
                            key={index}
                            size="icon"
                            className={`${social.color} hover:opacity-90 text-white shadow-md`}
                            onClick={() =>
                              window.open(social.url, "_blank")
                            }
                          >
                            <Icon className="w-5 h-5" />
                          </Button>
                        );
                      })}
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* FAQ Quick Access */}
              <Card className="border-2 border-amber-200 bg-gradient-to-br from-amber-50 to-yellow-100 shadow-lg">
                <CardContent className="p-6">
                  <h3 className="text-lg font-semibold text-amber-900 mb-3">
                    Câu hỏi thường gặp
                  </h3>
                  <ul className="space-y-2 text-sm text-gray-700">
                    <li>• Thời gian giao hàng là bao lâu?</li>
                    <li>• Có giao hàng ngoài giờ không?</li>
                    <li>• Làm sao để chọn mâm cúng phù hợp?</li>
                    <li>• Chính sách đổi trả như thế nào?</li>
                  </ul>
                  <Button
                    variant="outline"
                    className="w-full mt-4 border-amber-400 text-amber-700 hover:bg-amber-50 font-medium"
                    onClick={() => navigate("/faq")}
                  >
                    Xem tất cả FAQ
                  </Button>
                </CardContent>
              </Card>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
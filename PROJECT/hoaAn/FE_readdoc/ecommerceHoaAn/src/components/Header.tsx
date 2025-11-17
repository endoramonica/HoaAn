import { useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { Button } from "./ui/button";
import { Badge } from "./ui/badge";
import { Sheet, SheetContent, SheetTrigger } from "./ui/sheet";
import {
  Flower2,
  Menu,
  ShoppingCart,
  User,
  Phone,
  Truck,
  Flower,
  MessageCircle,
  Users,
  HelpCircle,
  Heart,
  LogIn,
} from "lucide-react";
import { useAuth } from "../lib/hooks/useAuth";
import { useCart } from "../lib/hooks/useCart";
import { useApp } from "../lib/contexts/AppContext";
import { useWishlist } from "../lib/hooks/useWishlist";

export function Header() {
  const [isOpen, setIsOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const { user, isAuthenticated } = useAuth();
  const { setShowSupportChat, showSupportChat } = useApp();
  const { wishlistCount } = useWishlist();
  const { cartItemCount } = useCart();

  // Determine current page from URL
  const currentPage = location.pathname.split("/")[1] || "home";

  const navItems = [
    { id: "", label: "Trang chủ", path: "/" },
    { id: "products", label: "Sản phẩm", path: "/products" },
    { id: "services", label: "Dịch vụ", path: "/services" },
    { id: "delivery", label: "Giao hàng", icon: Truck, path: "/delivery" },
    { id: "spiritual", label: "Tâm linh", icon: Flower, path: "/spiritual" },
    { id: "community", label: "Cộng đồng", icon: Users, path: "/community" },
    { id: "qa", label: "Hỏi đáp", icon: HelpCircle, path: "/qa" },
    { id: "about", label: "Về chúng tôi", path: "/about" },
    { id: "contact", label: "Liên hệ", path: "/contact" },
  ];

  const handleNavigation = (path: string) => {
    navigate(path);
    setIsOpen(false);
  };

  const handleToggleSupport = () => {
    setShowSupportChat(!showSupportChat);
  };

  return (
    <header className="sticky top-0 z-50 bg-white/95 backdrop-blur-sm border-b border-amber-200 shadow-sm">
      <div className="max-w-6xl mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <div
            className="flex items-center gap-2 cursor-pointer"
            onClick={() => handleNavigation("/")}
          >
            <Flower2 className="w-8 h-8 text-amber-600" />
            <span className="text-xl text-amber-900">Đồ Cúng Online</span>
          </div>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center gap-6">
            {navItems.map((item) => (
              <button
                key={item.id}
                onClick={() => handleNavigation(item.path)}
                className={`flex items-center gap-2 px-3 py-2 rounded-md transition-colors ${
                  currentPage === item.id ||
                  (item.id === "" && currentPage === "home")
                    ? "text-amber-700 bg-amber-50 border-b-2 border-amber-600"
                    : "text-gray-700 hover:text-amber-700 hover:bg-amber-50"
                }`}
              >
                {item.icon && <item.icon className="w-4 h-4" />}
                {item.label}
              </button>
            ))}
          </nav>

          {/* Desktop Actions */}
          <div className="hidden md:flex items-center gap-4">
            <Button
              variant="outline"
              size="sm"
              className="border-amber-300 text-amber-700 hover:bg-amber-50"
            >
              <Phone className="w-4 h-4 mr-2" />
              1900 1234
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={handleToggleSupport}
              className="border-blue-300 text-blue-700 hover:bg-blue-50"
            >
              <MessageCircle className="w-4 h-4 mr-2" />
              Hỗ trợ
            </Button>

            {/* Login/Profile Button */}
            {isAuthenticated ? (
              <Button
                variant="outline"
                size="icon"
                className="border-amber-300 text-amber-700 hover:bg-amber-50"
                onClick={() => handleNavigation("/profile")}
                title={user?.fullName || "Tài khoản"}
              >
                <User className="w-4 h-4" />
              </Button>
            ) : (
              <Button
                variant="outline"
                size="sm"
                className="border-amber-300 text-amber-700 hover:bg-amber-50"
                onClick={() => handleNavigation("/auth/login")}
              >
                <LogIn className="w-4 h-4 mr-2" />
                Đăng nhập
              </Button>
            )}

            <Button
              variant="outline"
              size="icon"
              className="flex-1 border-pink-300 text-pink-700 relative"
              onClick={() => handleNavigation("/wishlist")}
            >
              <Heart className="w-4 h-4" />
              {wishlistCount > 0 && (
                <Badge className="absolute -top-1 -right-1 bg-pink-500 text-white text-xs w-4 h-4 flex items-center justify-center p-0">
                  {wishlistCount}
                </Badge>
              )}
            </Button>
            <Button
              variant="outline"
              size="icon"
              className="flex-1 border-amber-300 text-amber-700 relative"
              onClick={() => handleNavigation("/cart")}
            >
              <ShoppingCart className="w-4 h-4" />
              {/* ✅ Show badge only if count > 0 */}
              {cartItemCount > 0 && (
                <Badge className="absolute -top-1 -right-1 bg-red-500 text-white text-xs min-w-[1rem] h-4 flex items-center justify-center p-1">
                  {cartItemCount > 99 ? "99+" : cartItemCount}
                </Badge>
              )}
            </Button>
          </div>

          {/* Mobile Menu */}
          <Sheet open={isOpen} onOpenChange={setIsOpen}>
            <SheetTrigger asChild>
              <Button
                variant="outline"
                size="icon"
                className="md:hidden border-amber-300 text-amber-700"
              >
                <Menu className="w-4 h-4" />
              </Button>
            </SheetTrigger>
            <SheetContent side="right" className="w-64 bg-white">
              <div className="flex items-center gap-2 mb-8">
                <Flower2 className="w-8 h-8 text-amber-600" />
                <span className="text-xl text-amber-900">Đồ Cúng Online</span>
              </div>

              <div className="space-y-4">
                {navItems.map((item) => (
                  <button
                    key={item.id}
                    onClick={() => handleNavigation(item.path)}
                    className={`w-full text-left px-4 py-3 rounded-md transition-colors flex items-center gap-3 ${
                      currentPage === item.id ||
                      (item.id === "" && currentPage === "home")
                        ? "text-amber-700 bg-amber-50 border-l-4 border-amber-600"
                        : "text-gray-700 hover:text-amber-700 hover:bg-amber-50"
                    }`}
                  >
                    {item.icon && <item.icon className="w-5 h-5" />}
                    {item.label}
                  </button>
                ))}

                <div className="border-t border-gray-200 pt-4 mt-6">
                  <Button className="w-full mb-3 bg-amber-600 hover:bg-amber-700 text-white">
                    <Phone className="w-4 h-4 mr-2" />
                    Gọi 1900 1234
                  </Button>
                  <Button
                    variant="outline"
                    className="w-full mb-3 border-blue-300 text-blue-700 hover:bg-blue-50"
                    onClick={() => {
                      setShowSupportChat(!showSupportChat);
                      setIsOpen(false);
                    }}
                  >
                    <MessageCircle className="w-4 h-4 mr-2" />
                    Chat hỗ trợ
                  </Button>
                  <div className="flex gap-2">
                    <Button
                      variant="outline"
                      size="icon"
                      className="flex-1 border-amber-300 text-amber-700"
                      onClick={() =>
                        handleNavigation(
                          isAuthenticated ? "/profile" : "/auth/login"
                        )
                      }
                    >
                      <User className="w-4 h-4" />
                    </Button>
                    <Button
                      variant="outline"
                      size="icon"
                      className="flex-1 border-pink-300 text-pink-700 relative"
                      onClick={() => handleNavigation("/wishlist")}
                    >
                      <Heart className="w-4 h-4" />
                      {wishlistCount > 0 && (
                        <Badge className="absolute -top-1 -right-1 bg-pink-500 text-white text-xs w-4 h-4 flex items-center justify-center p-0">
                          {wishlistCount}
                        </Badge>
                      )}
                    </Button>
                    <Button
                      variant="outline"
                      size="icon"
                      className="flex-1 border-amber-300 text-amber-700 relative"
                      onClick={() => handleNavigation("/cart")}
                    >
                      <ShoppingCart className="w-4 h-4" />
                      {/* ✅ Show badge only if count > 0 */}
                      {cartItemCount > 0 && (
                        <Badge className="absolute -top-1 -right-1 bg-red-500 text-white text-xs min-w-[1rem] h-4 flex items-center justify-center p-1">
                          {cartItemCount > 99 ? "99+" : cartItemCount}
                        </Badge>
                      )}
                    </Button>
                  </div>
                </div>
              </div>
            </SheetContent>
          </Sheet>
        </div>
      </div>
    </header>
  );
}

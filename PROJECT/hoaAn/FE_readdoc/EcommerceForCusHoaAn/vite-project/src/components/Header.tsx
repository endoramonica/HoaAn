import { useState } from 'react';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Sheet, SheetContent, SheetTrigger } from './ui/sheet';
import { Flower2, Menu, ShoppingCart, User, Phone, Truck, Flower, MessageCircle, Users, HelpCircle } from 'lucide-react';

interface HeaderProps {
  currentPage: string;
  onNavigate: (page: string) => void;
  onToggleSupport?: () => void;
}

export function Header({ currentPage, onNavigate, onToggleSupport }: HeaderProps) {
  const [isOpen, setIsOpen] = useState(false);

  const navItems = [
    { id: 'home', label: 'Trang chủ' },
    { id: 'products', label: 'Sản phẩm' },
    { id: 'services', label: 'Dịch vụ' },
    { id: 'delivery', label: 'Giao hàng', icon: Truck },
    { id: 'spiritual', label: 'Tâm linh', icon: Flower },
    { id: 'community', label: 'Cộng đồng', icon: Users },
    { id: 'qa', label: 'Hỏi đáp', icon: HelpCircle },
    { id: 'about', label: 'Về chúng tôi' },
    { id: 'contact', label: 'Liên hệ' },
  ];

  const handleNavigation = (page: string) => {
    onNavigate(page);
    setIsOpen(false);
  };

  return (
    <header className="sticky top-0 z-50 bg-white/95 backdrop-blur-sm border-b border-amber-200 shadow-sm">
      <div className="max-w-6xl mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <div 
            className="flex items-center gap-2 cursor-pointer"
            onClick={() => handleNavigation('home')}
          >
            <Flower2 className="w-8 h-8 text-amber-600" />
            <span className="text-xl text-amber-900">Đồ Cúng Online</span>
          </div>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center gap-6">
            {navItems.map((item) => (
              <button
                key={item.id}
                onClick={() => handleNavigation(item.id)}
                className={`flex items-center gap-2 px-3 py-2 rounded-md transition-colors ${
                  currentPage === item.id
                    ? 'text-amber-700 bg-amber-50 border-b-2 border-amber-600'
                    : 'text-gray-700 hover:text-amber-700 hover:bg-amber-50'
                }`}
              >
                {item.icon && <item.icon className="w-4 h-4" />}
                {item.label}
              </button>
            ))}
          </nav>

          {/* Desktop Actions */}
          <div className="hidden md:flex items-center gap-4">
            <Button variant="outline" size="sm" className="border-amber-300 text-amber-700 hover:bg-amber-50">
              <Phone className="w-4 h-4 mr-2" />
              1900 1234
            </Button>
            {onToggleSupport && (
              <Button 
                variant="outline" 
                size="sm" 
                onClick={onToggleSupport}
                className="border-blue-300 text-blue-700 hover:bg-blue-50"
              >
                <MessageCircle className="w-4 h-4 mr-2" />
                Hỗ trợ
              </Button>
            )}
            <Button 
              variant="outline" 
              size="icon" 
              className="border-amber-300 text-amber-700 hover:bg-amber-50"
              onClick={() => handleNavigation('profile')}
            >
              <User className="w-4 h-4" />
            </Button>
            <Button 
              variant="outline" 
              size="icon" 
              className="border-amber-300 text-amber-700 hover:bg-amber-50 relative"
              onClick={() => handleNavigation('cart')}
            >
              <ShoppingCart className="w-4 h-4" />
              <Badge className="absolute -top-2 -right-2 bg-red-500 text-white text-xs w-5 h-5 flex items-center justify-center p-0">
                3
              </Badge>
            </Button>
          </div>

          {/* Mobile Menu */}
          <Sheet open={isOpen} onOpenChange={setIsOpen}>
            <SheetTrigger asChild>
              <Button variant="outline" size="icon" className="md:hidden border-amber-300 text-amber-700">
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
                    onClick={() => handleNavigation(item.id)}
                    className={`w-full text-left px-4 py-3 rounded-md transition-colors flex items-center gap-3 ${
                      currentPage === item.id
                        ? 'text-amber-700 bg-amber-50 border-l-4 border-amber-600'
                        : 'text-gray-700 hover:text-amber-700 hover:bg-amber-50'
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
                  {onToggleSupport && (
                    <Button 
                      variant="outline" 
                      className="w-full mb-3 border-blue-300 text-blue-700 hover:bg-blue-50"
                      onClick={() => {
                        onToggleSupport();
                        setIsOpen(false);
                      }}
                    >
                      <MessageCircle className="w-4 h-4 mr-2" />
                      Chat hỗ trợ
                    </Button>
                  )}
                  <div className="flex gap-2">
                    <Button 
                      variant="outline" 
                      size="icon" 
                      className="flex-1 border-amber-300 text-amber-700"
                      onClick={() => handleNavigation('profile')}
                    >
                      <User className="w-4 h-4" />
                    </Button>
                    <Button 
                      variant="outline" 
                      size="icon" 
                      className="flex-1 border-amber-300 text-amber-700 relative"
                      onClick={() => handleNavigation('cart')}
                    >
                      <ShoppingCart className="w-4 h-4" />
                      <Badge className="absolute -top-1 -right-1 bg-red-500 text-white text-xs w-4 h-4 flex items-center justify-center p-0">
                        3
                      </Badge>
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

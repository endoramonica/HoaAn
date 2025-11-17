import { useState, useRef, useEffect } from 'react';
import { Card } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { MessageCircle, Send, X, Minimize2, Maximize2, Bot, User, Phone, Mail, Clock } from 'lucide-react';

interface SupportChatModalProps {
  isVisible: boolean;
  onToggle: () => void;
}

interface Message {
  id: number;
  text: string;
  sender: 'user' | 'support' | 'ai';
  timestamp: Date;
  type?: 'text' | 'system';
}

export function SupportChatModal({ isVisible, onToggle }: SupportChatModalProps) {
  const [messages, setMessages] = useState<Message[]>([
    {
      id: 1,
      text: "Xin chào! Tôi là trợ lý hỗ trợ của Đồ Cúng Việt Nam. Tôi có thể giúp bạn về:",
      sender: 'support',
      timestamp: new Date(),
      type: 'system'
    },
    {
      id: 2,
      text: "• Hướng dẫn đặt hàng và thanh toán\n• Thông tin sản phẩm và dịch vụ\n• Theo dõi đơn hàng\n• Chính sách đổi trả\n• Hỗ trợ kỹ thuật\n\nBạn cần hỗ trợ gì ạ?",
      sender: 'support',
      timestamp: new Date()
    }
  ]);
  const [inputText, setInputText] = useState('');
  const [isMinimized, setIsMinimized] = useState(false);
  const [isTyping, setIsTyping] = useState(false);
  const [supportStatus, setSupportStatus] = useState<'online' | 'away' | 'offline'>('online');
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const quickActions = [
    "Hướng dẫn đặt hàng",
    "Theo dõi đơn hàng", 
    "Chính sách đổi trả",
    "Liên hệ tư vấn",
    "Báo lỗi kỹ thuật"
  ];

  const supportResponses = {
    "đặt hàng": "📝 **Hướng dẫn đặt hàng:**\n\n1. Chọn sản phẩm từ danh mục\n2. Thêm vào giỏ hàng\n3. Điền thông tin giao hàng\n4. Chọn phương thức thanh toán\n5. Xác nhận đơn hàng\n\n💡 **Mẹo**: Đăng ký tài khoản để theo dõi đơn hàng dễ dàng hơn!",
    
    "theo dõi": "📦 **Theo dõi đơn hàng:**\n\nBạn có thể theo dõi đơn hàng qua:\n• Mã đơn hàng được gửi qua SMS/Email\n• Đăng nhập tài khoản → Đơn hàng của tôi\n• Gọi hotline: 1900-xxx-xxx\n\nVui lòng cung cấp mã đơn hàng để tôi hỗ trợ kiểm tra chi tiết.",
    
    "đổi trả": "🔄 **Chính sách đổi trả:**\n\n✅ **Đổi trả trong 7 ngày**\n• Sản phẩm còn nguyên vẹn, chưa sử dụng\n• Có hóa đơn mua hàng\n• Không áp dụng cho đồ cúng đã khai quang\n\n📞 **Liên hệ đổi trả**: 1900-xxx-xxx\n📧 **Email**: doitra@docungvietnam.com",
    
    "thanh toán": "💳 **Phương thức thanh toán:**\n\n• COD (Thanh toán khi nhận hàng)\n• Chuyển khoản ngân hàng\n• Ví điện tử (MoMo, ZaloPay)\n• Thẻ tín dụng/ghi nợ\n• QR Code Banking\n\n🔒 **Bảo mật**: Mọi giao dịch được mã hóa SSL",
    
    "liên hệ": "📞 **Thông tin liên hệ:**\n\n🏢 **Địa chỉ**: 123 Nguyễn Trãi, Q.1, TP.HCM\n📱 **Hotline**: 1900-xxx-xxx\n📧 **Email**: support@docungvietnam.com\n🕐 **Giờ làm việc**: 8:00 - 20:00 (T2-CN)\n\n💬 **Live Chat**: Trực tuyến 24/7"
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const getAIResponse = (userMessage: string): string => {
    const message = userMessage.toLowerCase();
    
    // Check for keywords in support responses
    for (const [keyword, response] of Object.entries(supportResponses)) {
      if (message.includes(keyword)) {
        return response;
      }
    }
    
    // Default responses
    if (message.includes('xin chào') || message.includes('hello')) {
      return "Xin chào! Tôi là trợ lý hỗ trợ khách hàng. Tôi có thể giúp bạn về đơn hàng, sản phẩm, thanh toán hay bất kỳ vấn đề nào khác. Bạn cần hỗ trợ gì ạ? 😊";
    }
    
    if (message.includes('cảm ơn')) {
      return "Rất vui được hỗ trợ bạn! Nếu có thêm câu hỏi nào khác, đừng ngần ngại liên hệ với chúng tôi nhé. Chúc bạn mua sắm vui vẻ! 🙏";
    }
    
    if (message.includes('hotline') || message.includes('số điện thoại')) {
      return "📞 **Hotline hỗ trợ**: 1900-xxx-xxx\n\n🕐 **Giờ làm việc**: 8:00 - 20:00 (Thứ 2 - Chủ nhật)\n\n💬 **Chat trực tuyến**: 24/7 (như hiện tại)\n\nBạn có thể gọi hoặc chat với chúng tôi bất cứ lúc nào!";
    }

    // Generic helpful response
    return `Tôi đã nhận được câu hỏi: "${userMessage}"\n\nĐể hỗ trợ bạn tốt nhất, tôi có thể giúp về:\n• Hướng dẫn đặt hàng\n• Theo dõi đơn hàng\n• Chính sách đổi trả\n• Phương thức thanh toán\n• Thông tin liên hệ\n\nHoặc bạn có thể gọi hotline: 1900-xxx-xxx để được hỗ trợ trực tiếp! 📞`;
  };

  const sendMessage = async () => {
    if (!inputText.trim()) return;

    const userMessage: Message = {
      id: Date.now(),
      text: inputText,
      sender: 'user',
      timestamp: new Date()
    };

    setMessages(prev => [...prev, userMessage]);
    setInputText('');
    setIsTyping(true);

    // Simulate support response time
    setTimeout(() => {
      const supportResponse: Message = {
        id: Date.now() + 1,
        text: getAIResponse(inputText),
        sender: 'support',
        timestamp: new Date()
      };
      
      setMessages(prev => [...prev, supportResponse]);
      setIsTyping(false);
    }, 1000 + Math.random() * 2000);
  };

  const handleQuickAction = (action: string) => {
    setInputText(action);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'online': return 'bg-green-500';
      case 'away': return 'bg-yellow-500';
      case 'offline': return 'bg-gray-500';
      default: return 'bg-green-500';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'online': return 'Trực tuyến';
      case 'away': return 'Bận';
      case 'offline': return 'Ngoại tuyến';
      default: return 'Trực tuyến';
    }
  };

  if (!isVisible) {
    return (
      <div className="fixed bottom-4 left-4 z-50">
        <Button
          onClick={onToggle}
          className="w-14 h-14 bg-gradient-to-br from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white rounded-full shadow-lg hover:shadow-xl transition-all duration-300"
        >
          <MessageCircle className="w-6 h-6" />
        </Button>
        <div className="absolute -top-2 -right-2 w-4 h-4 bg-green-500 rounded-full border-2 border-white animate-pulse"></div>
      </div>
    );
  }

  return (
    <div className="fixed bottom-4 left-4 z-50">
      <Card className={`w-80 bg-white shadow-2xl border-blue-200 transition-all duration-300 ${ 
        isMinimized ? 'h-16' : 'h-96'
      }`}>
        {/* Header */}
        <div className="flex items-center justify-between p-4 bg-gradient-to-r from-blue-600 to-indigo-600 text-white rounded-t-lg">
          <div className="flex items-center gap-3">
            <div className="relative">
              <div className="w-8 h-8 bg-white/20 rounded-full flex items-center justify-center">
                <MessageCircle className="w-4 h-4" />
              </div>
              <div className={`absolute -bottom-1 -right-1 w-3 h-3 ${getStatusColor(supportStatus)} rounded-full border border-white`}></div>
            </div>
            <div>
              <h3 className="text-sm">Hỗ trợ khách hàng</h3>
              <p className="text-xs text-blue-100">{getStatusText(supportStatus)} • Phản hồi trong 1 phút</p>
            </div>
          </div>
          <div className="flex gap-1">
            <button
              onClick={() => setIsMinimized(!isMinimized)}
              className="p-1 hover:bg-white/20 rounded"
            >
              {isMinimized ? <Maximize2 className="w-4 h-4" /> : <Minimize2 className="w-4 h-4" />}
            </button>
            <button
              onClick={onToggle}
              className="p-1 hover:bg-white/20 rounded"
            >
              <X className="w-4 h-4" />
            </button>
          </div>
        </div>

        {!isMinimized && (
          <>
            {/* Messages */}
            <div className="h-52 overflow-y-auto p-4 space-y-3">
              {messages.map((message) => (
                <div key={message.id} className={`flex gap-2 ${message.sender === 'user' ? 'justify-end' : 'justify-start'}`}>
                  {message.sender !== 'user' && (
                    <div className="w-8 h-8 bg-gradient-to-br from-blue-100 to-indigo-200 rounded-full flex items-center justify-center flex-shrink-0">
                      {message.sender === 'ai' ? (
                        <Bot className="w-4 h-4 text-blue-700" />
                      ) : (
                        <MessageCircle className="w-4 h-4 text-blue-700" />
                      )}
                    </div>
                  )}
                  <div className={`max-w-[70%] p-3 rounded-lg text-sm ${
                    message.sender === 'user'
                      ? 'bg-blue-600 text-white'
                      : message.type === 'system'
                      ? 'bg-blue-50 text-blue-800 border border-blue-200'
                      : 'bg-gray-100 text-gray-800'
                  }`}>
                    <div className="whitespace-pre-line">{message.text}</div>
                    <div className={`text-xs mt-1 opacity-70 ${
                      message.sender === 'user' ? 'text-blue-100' : 'text-gray-500'
                    }`}>
                      {message.timestamp.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })}
                    </div>
                  </div>
                  {message.sender === 'user' && (
                    <div className="w-8 h-8 bg-gradient-to-br from-gray-100 to-gray-200 rounded-full flex items-center justify-center flex-shrink-0">
                      <User className="w-4 h-4 text-gray-700" />
                    </div>
                  )}
                </div>
              ))}
              
              {isTyping && (
                <div className="flex gap-2 justify-start">
                  <div className="w-8 h-8 bg-gradient-to-br from-blue-100 to-indigo-200 rounded-full flex items-center justify-center">
                    <MessageCircle className="w-4 h-4 text-blue-700" />
                  </div>
                  <div className="bg-gray-100 p-3 rounded-lg">
                    <div className="flex gap-1">
                      {[1, 2, 3].map((i) => (
                        <div
                          key={i}
                          className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"
                          style={{ animationDelay: `${i * 0.2}s` }}
                        />
                      ))}
                    </div>
                  </div>
                </div>
              )}
              <div ref={messagesEndRef} />
            </div>

            {/* Quick Actions */}
            <div className="px-4 pb-2">
              <div className="flex gap-1 overflow-x-auto pb-2">
                {quickActions.map((action) => (
                  <Badge
                    key={action}
                    variant="outline"
                    className="cursor-pointer hover:bg-blue-50 whitespace-nowrap text-xs"
                    onClick={() => handleQuickAction(action)}
                  >
                    {action}
                  </Badge>
                ))}
              </div>
            </div>

            {/* Input */}
            <div className="p-4 border-t border-gray-200">
              <div className="flex gap-2">
                <Input
                  value={inputText}
                  onChange={(e) => setInputText(e.target.value)}
                  onKeyPress={(e) => e.key === 'Enter' && sendMessage()}
                  placeholder="Nhập tin nhắn..."
                  className="flex-1 text-sm"
                  disabled={isTyping}
                />
                <Button
                  onClick={sendMessage}
                  disabled={!inputText.trim() || isTyping}
                  size="sm"
                  className="bg-blue-600 hover:bg-blue-700 text-white"
                >
                  <Send className="w-4 h-4" />
                </Button>
              </div>
              
              {/* Contact info */}
              <div className="flex items-center justify-between mt-2 text-xs text-gray-500">
                <div className="flex items-center gap-3">
                  <span className="flex items-center gap-1">
                    <Phone className="w-3 h-3" />
                    1900-xxx-xxx
                  </span>
                  <span className="flex items-center gap-1">
                    <Clock className="w-3 h-3" />
                    8:00-20:00
                  </span>
                </div>
              </div>
            </div>
          </>
        )}
      </Card>
    </div>
  );
}
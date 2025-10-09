import { useState, useRef, useEffect } from 'react';
import { Card } from './ui/card';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Badge } from './ui/badge';
import { Bot, User, Send, Sparkles, MessageCircle, Clock, Heart } from 'lucide-react';

interface QAPageProps {
  onBack: () => void;
}

interface Message {
  id: number;
  text: string;
  sender: 'user' | 'ai';
  timestamp: Date;
}

export function QAPage({ onBack }: QAPageProps) {
  const [messages, setMessages] = useState<Message[]>([
    {
      id: 1,
      text: "Xin chào! Tôi là AI Tư vấn Tâm Linh của Đồ Cúng Việt Nam. Tôi có thể giúp bạn:\n\n🏠 Phong thủy nhà ở, văn phòng\n🙏 Nghi lễ truyền thống\n🕯️ Cách thắp hương, cầu nguyện\n✨ Ý nghĩa các vật phẩm tâm linh\n📅 Chọn ngày tốt\n\nBạn có câu hỏi gì về tâm linh không?",
      sender: 'ai',
      timestamp: new Date()
    }
  ]);
  
  const [inputText, setInputText] = useState('');
  const [isTyping, setIsTyping] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const quickQuestions = [
    "Cách bày trí bàn thờ gia tiên",
    "Ngày nào thắp hương tốt nhất?",
    "Ý nghĩa của việc thắp 3 nén hương",
    "Cách chọn hướng nhà theo phong thủy",
    "Nghi lễ cưới hỏi truyền thống",
    "Cách cầu an cho gia đình"
  ];

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const getAIResponse = (userMessage: string): string => {
    const message = userMessage.toLowerCase();
    
    if (message.includes('bàn thờ') || message.includes('gia tiên')) {
      return "🏠 **Cách bày trí bàn thờ gia tiên:**\n\n✅ **Vị trí:**\n• Đặt ở nơi cao nhất, trang trọng nhất trong nhà\n• Hướng ra cửa chính hoặc hướng Nam\n• Tránh dưới gầm cầu thang, đối diện toilet\n\n🕯️ **Bày trí:**\n• Tượng/ảnh tổ tiên ở giữa\n• 2 bên đặt đèn cầy hoặc đèn thờ\n• Bình hoa tươi, chén nước sạch\n• Lư hương ở trước\n\n⚠️ **Lưu ý:**\n• Luôn giữ sạch sẽ, trang nghiêm\n• Thay nước, hoa đều đặn\n• Thắp hương vào các ngày rằm, mùng 1";
    }
    
    if (message.includes('thắp hương') || message.includes('hương')) {
      return "🕯️ **Về thắp hương cầu nguyện:**\n\n🔢 **Số lượng nén hương:**\n• 1 nén: Cầu cho bản thân\n• 3 nén: Kính tặng Tam Bảo (Phật-Pháp-Tăng)\n• 5 nén: Cầu cho ngũ hành cân bằng\n• 9 nén: Cầu cho chín thế hệ tổ tiên\n\n⏰ **Thời gian tốt:**\n• 6:00 sáng - năng lượng trong lành\n• 12:00 trưa - dương khí sung mãn\n• 18:00 chiều - chuyển giao âm dương\n\n🙏 **Cách thắp:**\n• Tĩnh tâm, rửa tay sạch\n• Thắp từ nến/lửa thiêng\n• Cắm thẳng, không nghiêng\n• Niệm Phật khi thắp";
    }
    
    if (message.includes('phong thủy') || message.includes('hướng nhà')) {
      return "🧭 **Phong thủy và hướng nhà:**\n\n🏠 **Hướng nhà tốt:**\n• **Đông Nam**: Tài lộc, thịnh vượng\n• **Nam**: Danh tiếng, sự nghiệp\n• **Tây Nam**: Tình duyên, gia đình\n• **Đông**: Sức khỏe, sinh khí\n\n⚖️ **Ngũ hành tương sinh:**\n• Kim → Thủy → Mộc → Hỏa → Thổ\n• Chọn hướng hợp mệnh của gia chủ\n\n🚫 **Tránh:**\n• Cửa chính đối diện cửa sau\n• Nhà ở cuối ngõ cụt\n• Gần nghĩa trang, bệnh viện\n• Dưới gầm cầu, cột điện\n\n💡 Bạn có thể chia sẻ năm sinh để tôi tư vấn hướng nhà phù hợp!";
    }
    
    if (message.includes('nghi lễ') || message.includes('cưới hỏi')) {
      return "💒 **Nghi lễ truyền thống:**\n\n👰 **Cưới hỏi:**\n• Lễ dạm ngõ: Hỏi ý định\n• Lễ nạp tài: Đưa sính lễ\n• Lễ thành hôn: Cưới chính thức\n• Lễ về nhà: Dâu về thăm\n\n🎁 **Sính lễ truyền thống:**\n• Trầu cau (100 cặp)\n• Rượu nếp (2 chai)\n• Bánh phu thê\n• Thịt heo, gà luộc\n• Hoa quả 5 loại\n• Vàng bạc, tiền mặt\n\n📅 **Chọn ngày tốt:**\n• Tránh tháng 3, 7, 9 âm lịch\n• Chọn ngày hợp tuổi cô dâu, chú rể\n• Tham khảo lịch vạn niên\n\nBạn có cần tư vấn về nghi lễ cụ thể nào không?";
    }

    if (message.includes('cầu an') || message.includes('cầu nguyện')) {
      return "🙏 **Cách cầu an cho gia đình:**\n\n🕯️ **Tại nhà:**\n• Thắp hương trước bàn thờ gia tiên\n• Cúng hoa quả, nước sạch\n• Niệm lời cầu nguyện chân thành\n• Thời gian: 6h sáng hoặc 18h chiều\n\n🏛️ **Tại đền chùa:**\n• Chọn chùa linh thiêng gần nhà\n• Chuẩn bị hoa, quả, tiền công đức\n• Thắp hương, lễ Phật\n• Quỳ xuống cầu nguyện thành tâm\n\n📿 **Lời cầu nguyện mẫu:**\n'Con kính lạy Phật, xin phù hộ cho gia đình con luôn bình an, khỏe mạnh, hạnh phúc. Xin cho con có đủ sức khỏe và trí tuệ để chăm sóc gia đình, làm những việc thiện.'\n\n💡 Điều quan tr��ng nhất là tấm lòng thành!";
    }

    // Generic helpful response
    return `Cảm ơn bạn đã hỏi về "${userMessage}". \n\nTôi có thể hỗ trợ bạn về:\n\n🏠 **Phong thủy**: Hướng nhà, bày trí nội thất\n🙏 **Nghi lễ**: Cưới hỏi, tang lễ, lễ giỗ\n🕯️ **Thắp hương**: Cách thức, thời gian, ý nghĩa\n✨ **Vật phẩm**: Ý nghĩa tượng Phật, bùa hộ mệnh\n📅 **Chọn ngày**: Ngày tốt cho các việc quan trọng\n\nBạn muốn tìm hiểu về chủ đề nào cụ thể? Hoặc có thể chọn một trong các câu hỏi gợi ý bên dưới! 😊`;
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

    // Simulate AI thinking time
    setTimeout(() => {
      const aiResponse: Message = {
        id: Date.now() + 1,
        text: getAIResponse(inputText),
        sender: 'ai',
        timestamp: new Date()
      };
      
      setMessages(prev => [...prev, aiResponse]);
      setIsTyping(false);
    }, 1500 + Math.random() * 1000);
  };

  const handleQuickQuestion = (question: string) => {
    setInputText(question);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-indigo-50 to-purple-50">
      {/* Header */}
      <div className="bg-gradient-to-r from-indigo-600 to-purple-600 text-white sticky top-0 z-40">
        <div className="max-w-4xl mx-auto px-4 py-4">
          <div className="flex items-center justify-between mb-4">
            <button
              onClick={onBack}
              className="text-indigo-100 hover:text-white"
            >
              ← Quay lại
            </button>
            <h1 className="text-xl flex items-center gap-2">
              <Bot className="w-6 h-6" />
              Hỏi đáp AI Tâm Linh
            </h1>
            <div className="w-6"></div>
          </div>
          
          <div className="text-center">
            <p className="text-indigo-100 text-sm mb-2">Tư vấn 24/7 - Giải đáp mọi thắc mắc về tâm linh</p>
            <div className="flex items-center justify-center gap-4 text-sm">
              <Badge className="bg-green-500/20 text-green-100 border-green-400">
                <div className="w-2 h-2 bg-green-400 rounded-full mr-1 animate-pulse"></div>
                AI Trực tuyến
              </Badge>
              <span className="flex items-center gap-1 text-indigo-200">
                <MessageCircle className="w-4 h-4" />
                Phản hồi trong 2 giây
              </span>
            </div>
          </div>
        </div>
      </div>

      <div className="max-w-4xl mx-auto px-4 py-6">
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
          {/* Sidebar - Quick Questions */}
          <div className="lg:col-span-1">
            <Card className="p-4 sticky top-24">
              <h3 className="mb-4 flex items-center gap-2 text-indigo-800">
                <Sparkles className="w-5 h-5" />
                Câu hỏi thường gặp
              </h3>
              <div className="space-y-2">
                {quickQuestions.map((question, index) => (
                  <button
                    key={index}
                    onClick={() => handleQuickQuestion(question)}
                    className="w-full text-left p-3 text-sm bg-indigo-50 hover:bg-indigo-100 rounded-lg transition-colors border border-indigo-200 hover:border-indigo-300"
                  >
                    {question}
                  </button>
                ))}
              </div>

              {/* Stats */}
              <div className="mt-6 pt-4 border-t border-indigo-200">
                <h4 className="text-indigo-800 mb-3">Thống kê hôm nay</h4>
                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span className="text-gray-600">Câu hỏi đã giải đáp</span>
                    <span className="text-indigo-700">247</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Người đang hỏi</span>
                    <span className="text-indigo-700">12</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-600">Độ hài lòng</span>
                    <span className="text-indigo-700 flex items-center gap-1">
                      98% <Heart className="w-3 h-3 fill-current text-red-500" />
                    </span>
                  </div>
                </div>
              </div>
            </Card>
          </div>

          {/* Main Chat Area */}
          <div className="lg:col-span-3">
            <Card className="h-[600px] flex flex-col">
              {/* Chat Header */}
              <div className="p-4 border-b border-gray-200 bg-gradient-to-r from-indigo-50 to-purple-50">
                <div className="flex items-center gap-3">
                  <div className="w-10 h-10 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-full flex items-center justify-center">
                    <Bot className="w-6 h-6 text-white" />
                  </div>
                  <div>
                    <h3 className="text-indigo-900">AI Tư vấn Tâm Linh</h3>
                    <p className="text-sm text-indigo-600">Sẵn sàng giải đáp thắc mắc của bạn</p>
                  </div>
                </div>
              </div>

              {/* Messages */}
              <div className="flex-1 overflow-y-auto p-4 space-y-4">
                {messages.map((message) => (
                  <div key={message.id} className={`flex gap-3 ${message.sender === 'user' ? 'justify-end' : 'justify-start'}`}>
                    {message.sender === 'ai' && (
                      <div className="w-8 h-8 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-full flex items-center justify-center flex-shrink-0">
                        <Bot className="w-4 h-4 text-white" />
                      </div>
                    )}
                    <div className={`max-w-[80%] p-4 rounded-lg ${
                      message.sender === 'user'
                        ? 'bg-indigo-600 text-white'
                        : 'bg-gray-100 text-gray-800'
                    }`}>
                      <div className="whitespace-pre-line text-sm leading-relaxed">{message.text}</div>
                      <div className={`text-xs mt-2 opacity-70 flex items-center gap-1 ${
                        message.sender === 'user' ? 'text-indigo-100' : 'text-gray-500'
                      }`}>
                        <Clock className="w-3 h-3" />
                        {message.timestamp.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })}
                      </div>
                    </div>
                    {message.sender === 'user' && (
                      <div className="w-8 h-8 bg-gradient-to-br from-gray-300 to-gray-400 rounded-full flex items-center justify-center flex-shrink-0">
                        <User className="w-4 h-4 text-gray-700" />
                      </div>
                    )}
                  </div>
                ))}
                
                {isTyping && (
                  <div className="flex gap-3 justify-start">
                    <div className="w-8 h-8 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-full flex items-center justify-center">
                      <Bot className="w-4 h-4 text-white" />
                    </div>
                    <div className="bg-gray-100 p-4 rounded-lg">
                      <div className="flex gap-1">
                        {[1, 2, 3].map((i) => (
                          <div
                            key={i}
                            className="w-2 h-2 bg-indigo-400 rounded-full animate-bounce"
                            style={{ animationDelay: `${i * 0.2}s` }}
                          />
                        ))}
                      </div>
                    </div>
                  </div>
                )}
                <div ref={messagesEndRef} />
              </div>

              {/* Input */}
              <div className="p-4 border-t border-gray-200">
                <div className="flex gap-3">
                  <Input
                    value={inputText}
                    onChange={(e) => setInputText(e.target.value)}
                    onKeyPress={(e) => e.key === 'Enter' && sendMessage()}
                    placeholder="Nhập câu hỏi về tâm linh, phong thủy..."
                    className="flex-1"
                    disabled={isTyping}
                  />
                  <Button
                    onClick={sendMessage}
                    disabled={!inputText.trim() || isTyping}
                    className="bg-indigo-600 hover:bg-indigo-700 text-white px-6"
                  >
                    <Send className="w-4 h-4" />
                  </Button>
                </div>
                
                <div className="mt-2 text-xs text-gray-500 text-center">
                  💡 Hãy hỏi về phong thủy, nghi lễ, thắp hương, cầu nguyện... Tôi sẽ tư vấn chi tiết!
                </div>
              </div>
            </Card>
          </div>
        </div>
      </div>
    </div>
  );
}

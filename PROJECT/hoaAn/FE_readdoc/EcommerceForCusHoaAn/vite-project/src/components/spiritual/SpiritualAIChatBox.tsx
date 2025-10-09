import { useState, useRef, useEffect } from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { MessageCircle, Send, X, Minimize2, Maximize2, Bot, User, Flower } from 'lucide-react';

interface SpiritualAIChatBoxProps {
  isVisible: boolean;
  onToggle: () => void;
}

interface Message {
  id: number;
  text: string;
  sender: 'user' | 'ai';
  timestamp: Date;
  suggestions?: string[];
}

export function SpiritualAIChatBox({ isVisible, onToggle }: SpiritualAIChatBoxProps) {
  const [messages, setMessages] = useState<Message[]>([
    {
      id: 1,
      text: "Chào bạn! Tôi là AI Trợ lý Tâm Linh. Tôi có thể giúp bạn tìm hiểu về phong thủy, tử vi, cách thắp hương cầu nguyện, hoặc trả lời các câu hỏi về tâm linh. Bạn muốn hỏi gì ạ?",
      sender: 'ai',
      timestamp: new Date(),
      suggestions: [
        "Cách thắp hương đúng cách",
        "Tìm hiểu về phong thủy nhà ở",
        "Ý nghĩa của việc niệm Phật",
        "Làm thế nào để tĩnh tâm?"
      ]
    }
  ]);
  const [inputText, setInputText] = useState('');
  const [isMinimized, setIsMinimized] = useState(false);
  const [isTyping, setIsTyping] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const spiritualResponses = {
    "thắp hương": "🙏 Thắp hương là nghi lễ thiêng liêng trong văn hóa Việt Nam:\n\n• Trước khi thắp, hãy rửa tay sạch sẽ và tĩnh tâm\n• Thắp 1 hoặc 3 nén hương (số lẻ mang ý nghĩa tốt lành)\n• Cầm hương bằng hai tay, cúi đầu tôn kính\n• Niệm lời cầu nguyện chân thành từ trái tim\n• Cắm hương vào lư, để hương cháy hết tự nhiên\n\nHương khói mang lời cầu nguyện của bạn lên trời cao! ✨",
    
    "phong thủy": "🏠 Phong thủy là nghệ thuật sắp xếp không gian sống hài hòa:\n\n• **Ngũ hành**: Kim, Mộc, Thủy, Hỏa, Thổ cân bằng nhau\n• **Cửa chính**: Hướng tốt theo tuổi chủ nhà\n• **Phòng ngủ**: Đầu giường tựa vào tường vững chắc\n• **Bếp**: Không đối diện cửa chính\n• **Cây xanh**: Đặt ở góc Đông Nam (góc Tài Lộc)\n\nBạn muốn tư vấn phong thủy cho không gian nào cụ thể? 🌿",
    
    "niệm phật": "🙏 Niệm Phật là pháp môn tu tập thanh tịnh tâm hồn:\n\n• **Nam Mô A Di Đà Phật**: Cầu sinh về Cực Lạc\n• **Nam Mô Quan Thế Âm Bồ Tát**: Cầu bình an, từ bi\n• **Nam Mô Địa Tạng Vương Bồ Tát**: Cầu siêu độ, giải nghiệp\n\n**Cách niệm**: Tĩnh tâm, niệm chậm rãi, chân thành. Có thể niệm thầm trong lòng hoặc niệm to.\n\n**Lợi ích**: Tăng cường năng lượng tích cực, giảm stress, mang lại bình an nội tâm 🌸",
    
    "tĩnh tâm": "🧘 Tĩnh tâm là nền tảng của mọi tu tập tâm linh:\n\n**Phương pháp đơn giản**:\n• Ngồi thẳng lưng, thở sâu 3 lần\n• Tập trung vào hơi thở vào-ra\n• Khi tâm tán loạn, nhẹ nhàng đưa về hơi thở\n• Bắt đầu 5-10 phút/ngày\n\n**Thời gian tốt**: Sáng sớm (5-7h) hoặc tối (19-21h)\n**Địa điểm**: Nơi yên tĩnh, sạch sẽ\n\nTĩnh tâm giúp bạn kết nối với bản ngã cao nhất! 🌟",
    
    "cầu nguyện": "🙏 Cầu nguyện là cách kết nối với năng lượng thiêng liêng:\n\n**Các loại cầu nguyện**:\n• **Sức khỏe**: \"Con cầu cho gia đình luôn mạnh khỏe, bình an\"\n• **Sự nghiệp**: \"Con mong công việc thuận lợi, gặp nhiều may mắn\"\n• **Tình yêu**: \"Con cầu cho tình yêu viên mãn, hạnh phúc\"\n• **Bình an**: \"Con cầu thế giới hòa bình, không khổ đau\"\n\n**Lời khuyên**: Cầu nguyện với tâm chân thành, không cầu xin để làm hại người khác 🕯️"
  };

  const quickResponses = [
    "Hãy kể thêm về vấn đề này",
    "Bạn có thể giải thích rõ hơn được không?",
    "Điều gì khiến bạn quan tâm đến vấn đề này?",
    "Bạn đã thử phương pháp nào chưa?"
  ];

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const getAIResponse = (userMessage: string): string => {
    const message = userMessage.toLowerCase();
    
    // Check for keywords in spiritual responses
    for (const [keyword, response] of Object.entries(spiritualResponses)) {
      if (message.includes(keyword)) {
        return response;
      }
    }
    
    // Default responses based on common patterns
    if (message.includes('xin chào') || message.includes('hello')) {
      return "Chào bạn! Tôi rất vui được hỗ trợ bạn về các vấn đề tâm linh. Bạn có câu hỏi gì không? 🙏";
    }
    
    if (message.includes('cảm ơn')) {
      return "Rất vui được giúp đỡ bạn! Chúc bạn luôn bình an và hạnh phúc. Nếu có thêm câu hỏi, đừng ngần ngại hỏi tôi nhé! 🌸";
    }
    
    if (message.includes('tôi buồn') || message.includes('stress') || message.includes('lo âu')) {
      return "Tôi hiểu bạn đang gặp khó khăn. Hãy thử:\n\n• Thở sâu 10 lần, thả lỏng cơ thể\n• Niệm \"Nam Mô Quan Thế Âm Bồ Tát\" để tĩnh tâm\n• Nghe nhạc thiền hoặc tiếng chuông chùa\n• Viết ra những điều bạn biết ơn\n\nMọi khó khăn đều sẽ qua đi. Hãy kiên nhẫn và từ ái với chính mình 💚";
    }
    
    // Generic helpful response
    return `Tôi hiểu bạn đang quan tâm về "${userMessage}". Đây là chủ đề rất thú vị trong tâm linh! Bạn có thể chia sẻ thêm chi tiết để tôi hỗ trợ bạn tốt hơn được không? 🌟\n\nHoặc bạn có thể hỏi tôi về:\n• Cách thắp hương cầu nguyện\n• Phong thủy nhà ở, văn phòng\n• Các pháp môn tu tập\n• Cách tĩnh tâm, thiền định`;
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
        timestamp: new Date(),
        suggestions: Math.random() > 0.5 ? [
          "Tìm hiểu thêm về chủ đề này",
          "Câu hỏi khác về tâm linh",
          "Hướng dẫn thực hành cụ thể"
        ] : undefined
      };
      
      setMessages(prev => [...prev, aiResponse]);
      setIsTyping(false);
    }, 1500 + Math.random() * 1000);
  };

  const handleSuggestionClick = (suggestion: string) => {
    setInputText(suggestion);
  };

  if (!isVisible) {
    return (
      <div className="fixed bottom-4 right-4 z-50">
        <Button
          onClick={onToggle}
          className="w-14 h-14 bg-gradient-to-br from-amber-600 to-orange-600 hover:from-amber-700 hover:to-orange-700 text-white rounded-full shadow-lg hover:shadow-xl transition-all duration-300 animate-pulse"
        >
          <MessageCircle className="w-6 h-6" />
        </Button>
      </div>
    );
  }

  return (
    <div className="fixed bottom-4 right-4 z-50">
      <Card className={`w-80 bg-white shadow-2xl border-amber-200 transition-all duration-300 ${
        isMinimized ? 'h-16' : 'h-96'
      }`}>
        {/* Header */}
        <div className="flex items-center justify-between p-4 bg-gradient-to-r from-amber-600 to-orange-600 text-white rounded-t-lg">
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 bg-white/20 rounded-full flex items-center justify-center">
              <Flower className="w-4 h-4" />
            </div>
            <div>
              <h3 className="text-sm">AI Tâm Linh</h3>
              <p className="text-xs text-orange-100">Trợ lý phong thủy & tâm linh</p>
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
            <div className="h-64 overflow-y-auto p-4 space-y-3">
              {messages.map((message) => (
                <div key={message.id} className={`flex gap-2 ${message.sender === 'user' ? 'justify-end' : 'justify-start'}`}>
                  {message.sender === 'ai' && (
                    <div className="w-8 h-8 bg-gradient-to-br from-amber-100 to-orange-200 rounded-full flex items-center justify-center flex-shrink-0">
                      <Bot className="w-4 h-4 text-amber-700" />
                    </div>
                  )}
                  <div className={`max-w-[70%] p-3 rounded-lg text-sm ${
                    message.sender === 'user'
                      ? 'bg-amber-600 text-white'
                      : 'bg-gray-100 text-gray-800'
                  }`}>
                    <div className="whitespace-pre-line">{message.text}</div>
                    {message.suggestions && (
                      <div className="mt-2 space-y-1">
                        {message.suggestions.map((suggestion, index) => (
                          <button
                            key={index}
                            onClick={() => handleSuggestionClick(suggestion)}
                            className="block w-full text-left p-2 bg-white/20 hover:bg-white/30 rounded text-xs transition-colors"
                          >
                            {suggestion}
                          </button>
                        ))}
                      </div>
                    )}
                  </div>
                  {message.sender === 'user' && (
                    <div className="w-8 h-8 bg-gradient-to-br from-blue-100 to-blue-200 rounded-full flex items-center justify-center flex-shrink-0">
                      <User className="w-4 h-4 text-blue-700" />
                    </div>
                  )}
                </div>
              ))}
              
              {isTyping && (
                <div className="flex gap-2 justify-start">
                  <div className="w-8 h-8 bg-gradient-to-br from-amber-100 to-orange-200 rounded-full flex items-center justify-center">
                    <Bot className="w-4 h-4 text-amber-700" />
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

            {/* Input */}
            <div className="p-4 border-t border-gray-200">
              <div className="flex gap-2">
                <Input
                  value={inputText}
                  onChange={(e) => setInputText(e.target.value)}
                  onKeyPress={(e) => e.key === 'Enter' && sendMessage()}
                  placeholder="Hỏi về phong thủy, tâm linh..."
                  className="flex-1 text-sm"
                  disabled={isTyping}
                />
                <Button
                  onClick={sendMessage}
                  disabled={!inputText.trim() || isTyping}
                  size="sm"
                  className="bg-amber-600 hover:bg-amber-700 text-white"
                >
                  <Send className="w-4 h-4" />
                </Button>
              </div>
            </div>
          </>
        )}
      </Card>
    </div>
  );
}

import { useState, useRef, useEffect } from 'react';
import { Card } from '../ui/card';
import { Button } from '../ui/button';
import { Slider } from '../ui/slider';
import { Play, Pause, SkipBack, SkipForward, Volume2, Heart, Download, Share, Repeat, Shuffle } from 'lucide-react';

interface AudioChantingPageProps {
  onBack: () => void;
}

export function AudioChantingPage({ onBack }: AudioChantingPageProps) {
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTrack, setCurrentTrack] = useState(0);
  const [progress, setProgress] = useState(0);
  const [volume, setVolume] = useState(70);
  const [isRepeat, setIsRepeat] = useState(false);
  const [isShuffle, setIsShuffle] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [isLiked, setIsLiked] = useState(false);
  const progressInterval = useRef<NodeJS.Timeout>();

  const sutras = [
    {
      id: 1,
      title: "Kinh Pháp Hoa",
      artist: "Thầy Thích Trí Quang",
      duration: "45:30",
      category: "Kinh điển",
      description: "Kinh Pháp Hoa là một trong những bộ kinh quan trọng nhất của Phật giáo",
      listeners: "12,456"
    },
    {
      id: 2,
      title: "Thần chú Quan Âm",
      artist: "Ni sư Diệu Âm",
      duration: "15:20",
      category: "Thần chú",
      description: "Thần chú cầu nguyện với Bồ tát Quan Thế Âm",
      listeners: "8,923"
    },
    {
      id: 3,
      title: "Kinh Địa Tạng",
      artist: "Thầy Thích Minh An",
      duration: "38:15",
      category: "Kinh điển",
      description: "Kinh về lòng từ bi và sự cứu độ của Bồ tát Địa Tạng",
      listeners: "15,234"
    },
    {
      id: 4,
      title: "Nam Mô A Di Đà Phật",
      artist: "Tăng đoàn Thiền viện",
      duration: "60:00",
      category: "Niệm Phật",
      description: "Niệm Phật A Di Đà để cầu sinh về Cực Lạc",
      listeners: "25,678"
    },
    {
      id: 5,
      title: "Kinh Tâm",
      artist: "Thầy Thích Nhật Từ",
      duration: "8:45",
      category: "Kinh điển",
      description: "Bát Nhã Tâm Kinh - tinh hoa của trí tuệ Phật",
      listeners: "9,567"
    }
  ];

  const categories = ["Tất cả", "Kinh điển", "Thần chú", "Niệm Phật", "Thiền"];
  const [selectedCategory, setSelectedCategory] = useState("Tất cả");

  const filteredSutras = selectedCategory === "Tất cả" 
    ? sutras 
    : sutras.filter(sutra => sutra.category === selectedCategory);

  const current = sutras[currentTrack];

  // Simulate audio playback
  useEffect(() => {
    if (isPlaying) {
      progressInterval.current = setInterval(() => {
        setProgress(prev => {
          const newProgress = prev + 1;
          setCurrentTime(newProgress);
          if (newProgress >= 100) {
            handleNext();
            return 0;
          }
          return newProgress;
        });
      }, 1000);
    } else {
      if (progressInterval.current) {
        clearInterval(progressInterval.current);
      }
    }

    return () => {
      if (progressInterval.current) {
        clearInterval(progressInterval.current);
      }
    };
  }, [isPlaying]);

  const handlePlayPause = () => {
    setIsPlaying(!isPlaying);
  };

  const handleNext = () => {
    if (isShuffle) {
      const randomIndex = Math.floor(Math.random() * sutras.length);
      setCurrentTrack(randomIndex);
    } else {
      setCurrentTrack((prev) => (prev + 1) % sutras.length);
    }
    setProgress(0);
    setCurrentTime(0);
  };

  const handlePrevious = () => {
    setCurrentTrack((prev) => (prev - 1 + sutras.length) % sutras.length);
    setProgress(0);
    setCurrentTime(0);
  };

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  };

  const getDurationInSeconds = (duration: string) => {
    const [mins, secs] = duration.split(':').map(Number);
    return mins * 60 + secs;
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-orange-50 via-amber-50 to-yellow-50">
      {/* Header */}
      <div className="bg-gradient-to-r from-orange-700 to-amber-700 text-white">
        <div className="max-w-md mx-auto px-4 py-6">
          <div className="flex items-center justify-between mb-4">
            <button
              onClick={onBack}
              className="text-orange-100 hover:text-white"
            >
              ← Quay lại
            </button>
            <h1 className="text-lg">Nghe Kinh Phật</h1>
            <button className="text-orange-100 hover:text-white">
              <Share className="w-5 h-5" />
            </button>
          </div>
          
          <div className="text-center">
            <p className="text-orange-100 text-sm">Tụng niệm - Thiền định - Tĩnh tâm</p>
          </div>
        </div>
      </div>

      <div className="max-w-md mx-auto px-4 py-6 space-y-6">
        {/* Now Playing Card */}
        <Card className="p-6 bg-gradient-to-br from-amber-100 to-orange-100 border-amber-200">
          <div className="text-center mb-6">
            <div className="w-32 h-32 bg-gradient-to-br from-amber-300 to-orange-400 rounded-full mx-auto mb-4 flex items-center justify-center shadow-lg">
              <div className={`text-4xl ${isPlaying ? 'animate-pulse' : ''}`}>🙏</div>
            </div>
            <h2 className="text-amber-900 mb-1">{current.title}</h2>
            <p className="text-amber-700 text-sm mb-2">{current.artist}</p>
            <div className="flex items-center justify-center gap-4 text-xs text-amber-600">
              <span>{current.category}</span>
              <span>•</span>
              <span>{current.listeners} người nghe</span>
            </div>
          </div>

          {/* Progress */}
          <div className="mb-4">
            <Slider
              value={[progress]}
              onValueChange={(value) => setProgress(value[0])}
              max={100}
              step={1}
              className="w-full"
            />
            <div className="flex justify-between text-xs text-amber-700 mt-1">
              <span>{formatTime(currentTime)}</span>
              <span>{current.duration}</span>
            </div>
          </div>

          {/* Controls */}
          <div className="flex items-center justify-center gap-4 mb-4">
            <button
              onClick={() => setIsShuffle(!isShuffle)}
              className={`p-2 rounded-full ${isShuffle ? 'bg-amber-200 text-amber-800' : 'text-amber-600 hover:bg-amber-200'}`}
            >
              <Shuffle className="w-4 h-4" />
            </button>
            
            <button
              onClick={handlePrevious}
              className="p-3 bg-amber-200 rounded-full text-amber-800 hover:bg-amber-300"
            >
              <SkipBack className="w-5 h-5" />
            </button>
            
            <button
              onClick={handlePlayPause}
              className="p-4 bg-amber-600 rounded-full text-white hover:bg-amber-700 shadow-lg"
            >
              {isPlaying ? <Pause className="w-6 h-6" /> : <Play className="w-6 h-6" />}
            </button>
            
            <button
              onClick={handleNext}
              className="p-3 bg-amber-200 rounded-full text-amber-800 hover:bg-amber-300"
            >
              <SkipForward className="w-5 h-5" />
            </button>
            
            <button
              onClick={() => setIsRepeat(!isRepeat)}
              className={`p-2 rounded-full ${isRepeat ? 'bg-amber-200 text-amber-800' : 'text-amber-600 hover:bg-amber-200'}`}
            >
              <Repeat className="w-4 h-4" />
            </button>
          </div>

          {/* Volume & Actions */}
          <div className="flex items-center gap-4">
            <Volume2 className="w-4 h-4 text-amber-600" />
            <Slider
              value={[volume]}
              onValueChange={(value) => setVolume(value[0])}
              max={100}
              step={1}
              className="flex-1"
            />
            <div className="flex gap-2">
              <button
                onClick={() => setIsLiked(!isLiked)}
                className={`p-2 rounded-full ${isLiked ? 'text-red-500' : 'text-amber-600 hover:text-red-500'}`}
              >
                <Heart className={`w-4 h-4 ${isLiked ? 'fill-current' : ''}`} />
              </button>
              <button className="p-2 rounded-full text-amber-600 hover:text-amber-800">
                <Download className="w-4 h-4" />
              </button>
            </div>
          </div>
        </Card>

        {/* Current Track Description */}
        <Card className="p-4">
          <h3 className="text-gray-800 mb-2">Về bài tụng này</h3>
          <p className="text-gray-600 text-sm">{current.description}</p>
        </Card>

        {/* Categories */}
        <div>
          <h3 className="text-gray-800 mb-3">Danh mục</h3>
          <div className="flex gap-2 overflow-x-auto pb-2">
            {categories.map((category) => (
              <button
                key={category}
                onClick={() => setSelectedCategory(category)}
                className={`px-4 py-2 rounded-full text-sm whitespace-nowrap ${
                  selectedCategory === category
                    ? 'bg-amber-600 text-white'
                    : 'bg-white border border-amber-300 text-amber-700 hover:bg-amber-50'
                }`}
              >
                {category}
              </button>
            ))}
          </div>
        </div>

        {/* Playlist */}
        <Card className="p-4">
          <h3 className="text-gray-800 mb-4">Danh sách phát</h3>
          <div className="space-y-3">
            {filteredSutras.map((sutra, index) => (
              <div
                key={sutra.id}
                onClick={() => {
                  setCurrentTrack(sutras.findIndex(s => s.id === sutra.id));
                  setProgress(0);
                  setCurrentTime(0);
                }}
                className={`flex items-center gap-3 p-3 rounded-lg cursor-pointer transition-colors ${
                  currentTrack === sutras.findIndex(s => s.id === sutra.id)
                    ? 'bg-amber-100 border border-amber-300'
                    : 'hover:bg-gray-50'
                }`}
              >
                <div className="w-12 h-12 bg-gradient-to-br from-amber-200 to-orange-300 rounded-lg flex items-center justify-center">
                  <span className="text-lg">🎵</span>
                </div>
                <div className="flex-1">
                  <h4 className={`text-sm ${
                    currentTrack === sutras.findIndex(s => s.id === sutra.id) ? 'text-amber-800' : 'text-gray-800'
                  }`}>
                    {sutra.title}
                  </h4>
                  <p className="text-xs text-gray-600">{sutra.artist} • {sutra.duration}</p>
                  <div className="flex items-center gap-2 text-xs text-gray-500 mt-1">
                    <span className="px-2 py-0.5 bg-gray-100 rounded-full">{sutra.category}</span>
                    <span>{sutra.listeners} lượt nghe</span>
                  </div>
                </div>
                {currentTrack === sutras.findIndex(s => s.id === sutra.id) && isPlaying && (
                  <div className="flex gap-1">
                    {[1, 2, 3].map((i) => (
                      <div
                        key={i}
                        className="w-1 bg-amber-600 rounded-full animate-pulse"
                        style={{
                          height: `${Math.random() * 16 + 8}px`,
                          animationDelay: `${i * 0.2}s`
                        }}
                      />
                    ))}
                  </div>
                )}
              </div>
            ))}
          </div>
        </Card>

        {/* Benefits */}
        <Card className="p-4 bg-gradient-to-r from-green-50 to-emerald-50 border-green-200">
          <h3 className="text-green-800 mb-3">Lợi ích của việc tụng kinh</h3>
          <div className="space-y-2 text-sm text-green-700">
            <div className="flex items-center gap-2">
              <div className="w-1.5 h-1.5 bg-green-600 rounded-full" />
              <span>Tĩnh tâm, giảm stress và lo âu</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-1.5 h-1.5 bg-green-600 rounded-full" />
              <span>Tăng cường khả năng tập trung</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-1.5 h-1.5 bg-green-600 rounded-full" />
              <span>Phát triển lòng từ bi và trí tuệ</span>
            </div>
            <div className="flex items-center gap-2">
              <div className="w-1.5 h-1.5 bg-green-600 rounded-full" />
              <span>Tạo năng lượng tích cực</span>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}

// components/calendar/CalendarHeader.tsx
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { ChevronLeft, ChevronRight, ArrowLeft, Filter } from 'lucide-react';

const monthNames = ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6',
  'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'];

interface Props {
  currentDate: Date;
  viewMode: 'month' | 'week' | 'day';
  selectedCategory: string;
  onViewModeChange: (mode: 'month' | 'week' | 'day') => void;
  onCategoryChange: (cat: string) => void;
  onNavigateBack: () => void;
  onNavigateMonth: (dir: 'prev' | 'next') => void;
  onToday: () => void;
}

export default function CalendarHeader({
  currentDate,
  viewMode,
  selectedCategory,
  onViewModeChange,
  onCategoryChange,
  onNavigateBack,
  onNavigateMonth,
  onToday
}: Props) {
  return (
    <section className="bg-gradient-to-r from-amber-900 to-red-800 text-white py-8">
      <div className="max-w-6xl mx-auto px-4">
        <div className="flex items-center gap-4 mb-6">
          <Button variant="outline" size="icon" onClick={onNavigateBack}
            className="border-white/30 text-white hover:bg-white/10">
            <ArrowLeft className="w-4 h-4" />
          </Button>
          <div>
            <h1 className="text-3xl md:text-4xl">Lịch âm đầy đủ</h1>
            <p className="text-yellow-100">Xem các ngày tốt, lễ hội và nghi lễ theo truyền thống</p>
          </div>
        </div>

        <div className="flex flex-wrap items-center gap-4">
          <div className="text-white text-sm font-medium">
            {monthNames[currentDate.getMonth()]} {currentDate.getFullYear()}
          </div>

          <div className="flex bg-white/10 rounded-lg p-1">
            {(['month', 'week', 'day'] as const).map((mode) => (
              <Button key={mode} variant={viewMode === mode ? 'default' : 'ghost'}
                size="sm" onClick={() => onViewModeChange(mode)}
                className={`text-white ${viewMode === mode ? 'bg-white/20' : ''}`}>
                {mode === 'month' ? 'Tháng' : mode === 'week' ? 'Tuần' : 'Ngày'}
              </Button>
            ))}
          </div>

          <Select value={selectedCategory} onValueChange={onCategoryChange}>
            <SelectTrigger className="w-48 border-white/30 text-white bg-white/10">
              <Filter className="w-4 h-4 mr-2" />
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">Tất cả</SelectItem>
              <SelectItem value="gia-tien">Gia tiên</SelectItem>
              <SelectItem value="phat-giao">Phật giáo</SelectItem>
              <SelectItem value="phong-thuy">Phong thủy</SelectItem>
              <SelectItem value="le-hoi">Lễ hội</SelectItem>
            </SelectContent>
          </Select>

          <div className="ml-auto flex items-center gap-2">
            <Button variant="outline" size="icon" onClick={() => onNavigateMonth('prev')}
              className="border-white/30 hover:bg-white/10">
              <ChevronLeft className="w-4 h-4" />
            </Button>
            <Button variant="outline" onClick={onToday}
              className="border-white/30 hover:bg-white/10">
              Hôm nay
            </Button>
            <Button variant="outline" size="icon" onClick={() => onNavigateMonth('next')}
              className="border-white/30 hover:bg-white/10">
              <ChevronRight className="w-4 h-4" />
            </Button>
          </div>
        </div>
      </div>
    </section>
  );
}
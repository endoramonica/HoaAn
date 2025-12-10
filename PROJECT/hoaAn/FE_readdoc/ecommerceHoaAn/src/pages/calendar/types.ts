export interface CalendarEvent {
    id: string;
    title: string;
    lunarDate: string;
    solarDate: string;
    type: 'festival' | 'ceremony' | 'good-day' | 'bad-day';
    category: 'gia-tien' | 'phat-giao' | 'phong-thuy' | 'le-hoi';
    description: string;
    color: string;
    recommendations?: string[];
}

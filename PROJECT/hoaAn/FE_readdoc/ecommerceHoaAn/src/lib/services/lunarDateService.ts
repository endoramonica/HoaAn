// src/lib/services/lunarDateService.ts
import axios from 'axios';

export interface LunarDateResponse {
    day: number;
    month: number;
    year: number;
    date: string;
    heavenlyStems: string;
    earthlyBranches: string;
    sexagenaryCycle: string;
}

export interface LunarDateApiResponse {
    data: LunarDateResponse;
    code: string;
}

const LUNAR_API_BASE = 'https://open.oapi.vn/date';

/**
 * Convert solar date to lunar date
 * @param day - Day (1-31)
 * @param month - Month (1-12)
 * @param year - Year (e.g., 2025)
 */
export async function convertToLunar(
    day: number,
    month: number,
    year: number
): Promise<LunarDateResponse> {
    try {
        const response = await axios.post<LunarDateApiResponse>(
            `${LUNAR_API_BASE}/convert-to-lunar`,
            { day, month, year },
            {
                headers: { 'Content-Type': 'application/json' },
                timeout: 10000,
            }
        );

        if (response.data.code !== 'success') {
            throw new Error(`API returned code: ${response.data.code}`);
        }

        return response.data.data;
    } catch (error) {
        console.error('Error converting to lunar date:', error);
        throw new Error('Không thể chuyển đổi ngày. Vui lòng thử lại.');
    }
}

/**
 * Convert lunar date to solar date
 * @param day - Day (1-30)
 * @param month - Month (1-12)
 * @param year - Year (e.g., 2025)
 */
export async function convertToSolar(
    day: number,
    month: number,
    year: number
): Promise<LunarDateResponse> {
    try {
        const response = await axios.post<LunarDateApiResponse>(
            `${LUNAR_API_BASE}/convert-to-solar`,
            { day, month, year },
            {
                headers: { 'Content-Type': 'application/json' },
                timeout: 10000,
            }
        );

        if (response.data.code !== 'success') {
            throw new Error(`API returned code: ${response.data.code}`);
        }

        return response.data.data;
    } catch (error) {
        console.error('Error converting to solar date:', error);
        throw new Error('Không thể chuyển đổi ngày. Vui lòng thử lại.');
    }
}

/**
 * Format lunar date for display
 */
export function formatLunarDate(lunarData: LunarDateResponse): string {
    return `${lunarData.day}/${lunarData.month}/${lunarData.year} (${lunarData.sexagenaryCycle})`;
}

/**
 * Format solar date for display
 */
export function formatSolarDate(date: Date): string {
    return date.toLocaleDateString('vi-VN');
}

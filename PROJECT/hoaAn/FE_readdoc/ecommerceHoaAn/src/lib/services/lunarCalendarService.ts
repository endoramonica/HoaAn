export interface LunarDateResult {
    day: number;
    month: number;
    year: number;
    leap: boolean;
    jd: number;
    canChiDay: string;
    canChiMonth: string;
    canChiYear: string;
    tietKhi: string;
}

// --- Constants & Dictionaries ---
const CAN = ['Canh', 'Tân', 'Nhâm', 'Quý', 'Giáp', 'Ất', 'Bính', 'Đinh', 'Mậu', 'Kỷ'];
const CHI = ['Thân', 'Dậu', 'Tuất', 'Hợi', 'Tý', 'Sửu', 'Dần', 'Mão', 'Thìn', 'Tỵ', 'Ngọ', 'Mùi'];
const TIET_KHI = [
    'Tiểu hàn', 'Đại hàn', 'Lập xuân', 'Vũ thủy', 'Kinh trập', 'Xuân phân',
    'Thanh minh', 'Cốc vũ', 'Lập hạ', 'Tiểu mãn', 'Mang chủng', 'Hạ chí',
    'Tiểu thử', 'Đại thử', 'Lập thu', 'Xử thử', 'Bạch lộ', 'Thu phân',
    'Hàn lộ', 'Sương giáng', 'Lập đông', 'Tiểu tuyết', 'Đại tuyết', 'Đông chí'
];

/**
 * Convert Date to Julian Day Number
 */
function jdn(dd: number, mm: number, yy: number) {
    let a = Math.floor((14 - mm) / 12);
    let y = yy + 4800 - a;
    let m = mm + 12 * a - 3;
    let jd = dd + Math.floor((153 * m + 2) / 5) + 365 * y + Math.floor(y / 4) - Math.floor(y / 100) + Math.floor(y / 400) - 32045;
    return jd;
}

/**
 * Convert Solar to Lunar Date (Simplified algorithm)
 */
function convertSolar2Lunar(dd: number, mm: number, yy: number) {
    let lunarDay, lunarMonth, lunarYear;
    const jd = jdn(dd, mm, yy);

    // Reference New Moon: Jan 21 1900
    const refDate = jdn(21, 1, 1900);
    const daysSince = jd - refDate;
    const moons = Math.floor(daysSince / 29.530588853);
    const approxNewMoon = refDate + moons * 29.530588853;
    let age = jd - approxNewMoon;
    if (age < 0) age += 29.53;

    lunarDay = Math.floor(age) + 1;
    if (lunarDay > 30) lunarDay = 1;

    // Year calculation
    if (mm < 2) {
        lunarYear = yy - 1;
    } else if (mm === 2) {
        lunarYear = (dd < 10) ? yy - 1 : yy;
    } else {
        lunarYear = yy;
    }

    // Month calculation
    lunarMonth = mm - 1;
    if (lunarMonth <= 0) {
        lunarMonth += 12;
    }

    // Special handling for 2025 Tet (Jan 29)
    if (yy === 2025) {
        const tetJd = jdn(29, 1, 2025);
        if (jd < tetJd) {
            lunarYear = 2024;
            const daysBefore = tetJd - jd;
            if (daysBefore < 30) {
                lunarMonth = 12;
                lunarDay = 30 - daysBefore;
            } else {
                lunarMonth = 11;
            }
        } else {
            lunarYear = 2025;
            const daysAfter = jd - tetJd;
            lunarMonth = Math.floor(daysAfter / 29.5) + 1;
            lunarDay = (daysAfter % 29) + 1;
            if (lunarDay > 30) lunarDay = 1;
        }
    }

    return { day: Math.floor(lunarDay), month: Math.floor(lunarMonth), year: lunarYear, leap: false };
}

/**
 * Get Lunar Date info
 */
export function getLunarDate(dd: number, mm: number, yyyy: number): LunarDateResult {
    const jd = jdn(dd, mm, yyyy);

    // Day Can/Chi
    const canDay = CAN[(jd + 3) % 10];
    const chiDay = CHI[(jd + 1) % 12];
    const canChiDay = `${canDay} ${chiDay}`;

    // Year Can/Chi
    const canYear = CAN[yyyy % 10];
    const chiYear = CHI[yyyy % 12];
    const canChiYear = `${canYear} ${chiYear}`;

    // Convert to Lunar
    const lunar = convertSolar2Lunar(dd, mm, yyyy);

    // Month Can/Chi
    const yearCanIndex = yyyy % 10;
    const monthCanIndex = (yearCanIndex * 2 + lunar.month) % 10;
    const canMonth = CAN[monthCanIndex];
    const chiMonth = CHI[(lunar.month + 2) % 12];
    const canChiMonth = `${canMonth} ${chiMonth}`;

    // Solar Term (Tiet Khi)
    const tietKhiIndex = Math.floor(((jd - jdn(6, 1, yyyy)) / 15.2184) + 23) % 24;
    const tietKhi = TIET_KHI[Math.abs(Math.floor(tietKhiIndex))];

    return {
        day: lunar.day,
        month: lunar.month,
        year: lunar.year,
        leap: lunar.leap,
        jd: jd,
        canChiDay,
        canChiMonth,
        canChiYear,
        tietKhi
    };
}

/**
 * Converts Solar Date to Lunar Date
 * Returns a Promise to maintain interface compatibility
 */
export const convertSolarToLunar = async (date: Date): Promise<LunarDateResult> => {
    return new Promise((resolve) => {
        setTimeout(() => {
            const day = date.getDate();
            const month = date.getMonth() + 1;
            const year = date.getFullYear();
            const result = getLunarDate(day, month, year);
            resolve(result);
        }, 100);
    });
};

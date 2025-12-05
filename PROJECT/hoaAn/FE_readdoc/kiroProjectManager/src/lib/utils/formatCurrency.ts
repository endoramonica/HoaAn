/**
 * Format a number as Vietnamese Dong (VND) currency
 * @param amount - The amount to format
 * @returns Formatted currency string (e.g., "1.000.000 ₫")
 */
export function formatCurrency(amount: number): string {
    if (typeof amount !== 'number' || isNaN(amount)) {
        return '0 ₫';
    }

    // Format with Vietnamese locale
    const formatted = new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    }).format(amount);

    return formatted;
}

/**
 * Format a number as VND without the currency symbol
 * @param amount - The amount to format
 * @returns Formatted number string (e.g., "1.000.000")
 */
export function formatNumber(amount: number): string {
    if (typeof amount !== 'number' || isNaN(amount)) {
        return '0';
    }

    return new Intl.NumberFormat('vi-VN').format(amount);
}

/**
 * Parse a formatted VND string back to a number
 * @param formattedAmount - The formatted currency string
 * @returns The numeric value
 */
export function parseCurrency(formattedAmount: string): number {
    if (!formattedAmount) return 0;

    // Remove currency symbol and dots, then parse
    const cleaned = formattedAmount.replace(/[₫\s.]/g, '').replace(',', '.');
    const parsed = parseFloat(cleaned);

    return isNaN(parsed) ? 0 : parsed;
}

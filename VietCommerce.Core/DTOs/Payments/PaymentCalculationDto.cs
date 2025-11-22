namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// DTO tính toán chi tiết đơn hàng
/// </summary>
public class PaymentCalculationDto
{
    public decimal SubTotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }

    // Breakdown details
    public string? VoucherApplied { get; set; }
    public decimal? VoucherDiscount { get; set; }
    public bool FreeShipping { get; set; }
    public string? ShippingMethod { get; set; }
}
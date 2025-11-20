// ShiftDto.cs + Requests
using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.DTOs.HRM;

public class ShiftDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal OpeningCash { get; set; }
    public decimal? ClosingCash { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalTransactions { get; set; }
    public ShiftStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class OpenShiftRequest
{
    public Guid StoreId { get; set; }
    public decimal OpeningCash { get; set; }
    public string? Notes { get; set; }
    
}

public class CloseShiftRequest
{
    public decimal ClosingCash { get; set; }
    public string? Notes { get; set; }
    
}

public class UpdateShiftRequest
{
    public string? Notes { get; set; }
    public ShiftStatus Status { get; set; }
}
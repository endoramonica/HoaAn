using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Entities.Logistics;

namespace VietCommerce.Core.DTOs.Logistics
{
    // ===================================
    // MAIN DTO
    // ===================================
    public class StockTransferDto
    {
        public Guid Id { get; set; }
        public string FromWarehouse { get; set; } = string.Empty;
        public string ToWarehouse { get; set; } = string.Empty;
        public StockTransferStatus Status { get; set; }
        public Guid? RequestedBy { get; set; }
        public string? RequestedByName { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Transfer Items
        public List<TransferItemDto> TransferItems { get; set; } = new();

        // Calculated fields
        public int TotalItems => TransferItems?.Count ?? 0;
        public int TotalQuantity => TransferItems?.Sum(x => x.Quantity) ?? 0;
    }

    // ===================================
    // TRANSFER ITEM DTO
    // ===================================
    public class TransferItemDto
    {
        public Guid Id { get; set; }
        public Guid StockTransferId { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductSKU { get; set; }
        public int Quantity { get; set; }
        public int? Received { get; set; }
        public string? Notes { get; set; }
    }

    // ===================================
    // CREATE REQUEST
    // ===================================
    public class CreateStockTransferRequest
    {
        [Required(ErrorMessage = "FromWarehouse is required")]
        [MaxLength(200)]
        public string FromWarehouse { get; set; } = string.Empty;

        [Required(ErrorMessage = "ToWarehouse is required")]
        [MaxLength(200)]
        public string ToWarehouse { get; set; } = string.Empty;

        [Required(ErrorMessage = "RequestedBy is required")]
        public Guid RequestedBy { get; set; }

        public Guid? SupplierId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "At least one transfer item is required")]
        [MinLength(1, ErrorMessage = "At least one transfer item is required")]
        public List<CreateTransferItemRequest> Items { get; set; } = new();
    }

    public class CreateTransferItemRequest
    {
        [Required(ErrorMessage = "ProductId is required")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    // ===================================
    // UPDATE STATUS REQUEST
    // ===================================
    public class UpdateStockTransferStatusRequest
    {
        [Required(ErrorMessage = "Status is required")]
        public StockTransferStatus Status { get; set; }

        public Guid? ApprovedBy { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        // For Delivered status - update received quantities
        public List<UpdateReceivedQuantityRequest>? ReceivedItems { get; set; }
    }

    public class UpdateReceivedQuantityRequest
    {
        [Required]
        public Guid TransferItemId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ReceivedQuantity { get; set; }
    }

    // ===================================
    // CANCEL REQUEST
    // ===================================
    public class CancelStockTransferRequest
    {
        [Required(ErrorMessage = "Reason is required")]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;

        public Guid? CancelledBy { get; set; }
    }

    // ===================================
    // FILTERS
    // ===================================
    public class StockTransferFilters
    {
        public StockTransferStatus? Status { get; set; }

        public string? FromWarehouse { get; set; }

        public string? ToWarehouse { get; set; }

        public Guid? RequestedBy { get; set; }

        public Guid? ApprovedBy { get; set; }

        public Guid? SupplierId { get; set; }

        // Date range filters
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }

        public DateTime? DeliveryFrom { get; set; }
        public DateTime? DeliveryTo { get; set; }

        // Search
        [MaxLength(200)]
        public string? SearchTerm { get; set; }
    }

    // ===================================
    // SUMMARY DTO (for dashboard/reports)
    // ===================================
    public class StockTransferSummaryDto
    {
        public int TotalTransfers { get; set; }
        public int PendingCount { get; set; }
        public int InTransitCount { get; set; }
        public int DeliveredCount { get; set; }
        public int CancelledCount { get; set; }
        public int TotalItemsTransferred { get; set; }
    }
}
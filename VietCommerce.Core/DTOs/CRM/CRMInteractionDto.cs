using System;
using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Entities.CRM;

namespace VietCommerce.Core.DTOs.CRM;

/// <summary>
/// DTO for CRM Interaction details
/// </summary>
public class CRMInteractionDto
{
    public Guid Id { get; set; }

    // Customer Info
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }

    // Interaction Info
    public CRMInteractionType Type { get; set; }
    public string TypeText => Type.ToString();

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public CRMInteractionStatus Status { get; set; }
    public string StatusText => Status.ToString();

    public DateTime? FollowUpDate { get; set; }

    // Audit Info
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? UpdatedByName { get; set; }
}

/// <summary>
/// DTO for CRM Interaction list view
/// </summary>
public class CRMInteractionListDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public CRMInteractionType Type { get; set; }
    public string TypeText => Type.ToString();
    public string Title { get; set; } = string.Empty;
    public CRMInteractionStatus Status { get; set; }
    public string StatusText => Status.ToString();
    public DateTime? FollowUpDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByName { get; set; }
}

/// <summary>
/// Request for creating new interaction
/// </summary>
public class CreateInteractionRequest
{
    [Required(ErrorMessage = "Customer ID is required")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Interaction type is required")]
    public CRMInteractionType Type { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string Description { get; set; } = string.Empty;

    public DateTime? FollowUpDate { get; set; }

    [Required(ErrorMessage = "Status is required")]
    public CRMInteractionStatus Status { get; set; } = CRMInteractionStatus.Pending;
}

/// <summary>
/// Request for updating interaction
/// </summary>
public class UpdateInteractionRequest
{
    public CRMInteractionType? Type { get; set; }

    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; set; }

    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }

    public DateTime? FollowUpDate { get; set; }

    public CRMInteractionStatus? Status { get; set; }
}

/// <summary>
/// Filters for CRM Interaction queries
/// </summary>
public class CRMInteractionFilters
{
    /// <summary>
    /// Filter by customer ID
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Filter by interaction type
    /// </summary>
    public CRMInteractionType? Type { get; set; }

    /// <summary>
    /// Filter by interaction status
    /// </summary>
    public CRMInteractionStatus? Status { get; set; }

    /// <summary>
    /// Search by Title or Description (partial match, case-insensitive)
    /// </summary>
    [MaxLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter interactions created on or after this date
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Filter interactions created on or before this date
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Filter by follow-up date range start
    /// </summary>
    public DateTime? FollowUpFromDate { get; set; }

    /// <summary>
    /// Filter by follow-up date range end
    /// </summary>
    public DateTime? FollowUpToDate { get; set; }

    /// <summary>
    /// Filter by creator user ID
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Filter interactions with pending follow-ups
    /// </summary>
    public bool? HasPendingFollowUp { get; set; }

    /// <summary>
    /// Validate filters
    /// </summary>
    public bool IsValid(out string errorMessage)
    {
        if (FromDate.HasValue && ToDate.HasValue && FromDate > ToDate)
        {
            errorMessage = "FromDate must be less than or equal to ToDate";
            return false;
        }

        if (FollowUpFromDate.HasValue && FollowUpToDate.HasValue && FollowUpFromDate > FollowUpToDate)
        {
            errorMessage = "FollowUpFromDate must be less than or equal to FollowUpToDate";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
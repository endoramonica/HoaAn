// File: VietCommerce.Application/Extensions/PaginationExtensions.cs
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Extensions;

public static class PaginationExtensions
{
    public static PaginatedResponse<T> ToResponse<T>(this PaginatedResult<T> result)
    {
        return new PaginatedResponse<T>(
            items: result.Items,
            pageNumber: result.PageNumber,
            pageSize: result.PageSize,
            totalItems: result.TotalItems
        );
    }
}
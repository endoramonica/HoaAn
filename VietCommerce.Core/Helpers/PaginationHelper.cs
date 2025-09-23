using VietCommerce.Core.Models;

namespace VietCommerce.Core.Helpers;

public static class PaginationHelper
{
    public static PaginatedResult<T> CreatePaginatedResult<T>(IEnumerable<T> items, int pageNumber, int pageSize, int totalItems)
    {
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        return new PaginatedResult<T>(items, pageNumber, pageSize, totalItems, totalPages);
    }
}
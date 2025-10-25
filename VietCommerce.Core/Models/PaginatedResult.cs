namespace VietCommerce.Core.Models;
public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public PaginatedResult()
    {
        Items = Enumerable.Empty<T>();
    }
    public PaginatedResult(IEnumerable<T> items, int pageNumber, int pageSize, int totalItems, int totalPages = 0)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = totalPages > 0 ? totalPages : (int)Math.Ceiling((double)totalItems / pageSize);
    }
}

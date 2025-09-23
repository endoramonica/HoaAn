namespace VietCommerce.Core.DTOs.Products;

public class ProductListDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedDate { get; set; }
}
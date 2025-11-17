using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Enums.Products;

namespace VietCommerce.Core.Entities.Products;

public class ProductImage : AuditableEntity // BaseEntity -> AuditableEntity để có CreatedAt, UpdatedAt
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;   // Đường dẫn vật lý trên server
    public int DisplayOrder { get; set; } = 0; // thứ tự hiển thị tăng dần

    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;        // URL truy cập file

    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }              // Thumbnail nếu là ảnh

    public MediaTypeEnum MediaType { get; set; } = MediaTypeEnum.Image;        // Loại media (ảnh, video, v.v.)

    public bool IsMain { get; set; } = false;             // Ảnh chính của product

    // Navigation properties
    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;
}

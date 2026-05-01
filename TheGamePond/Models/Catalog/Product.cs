using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheGamePond.Models.Catalog;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(160)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(180)]
    public string Slug { get; set; } = string.Empty;

    [StringLength(80)]
    public string Sku { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Barcode { get; set; }

    [StringLength(80)]
    public string Platform { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Franchise { get; set; }

    [StringLength(80)]
    public string Condition { get; set; } = string.Empty;

    [StringLength(1200)]
    public string? Description { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal CostPrice { get; set; }

    [Column(TypeName = "numeric(10,2)")]
    public decimal SalePrice { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public int ProductCategoryId { get; set; }

    public ProductCategory? Category { get; set; }

    public InventoryItem? InventoryItem { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    public ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();
}

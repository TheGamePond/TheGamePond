using System.ComponentModel.DataAnnotations;

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

    [StringLength(4000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(64)]
    public string Sku { get; set; } = string.Empty;

    [StringLength(64)]
    public string? Barcode { get; set; }

    [StringLength(80)]
    public string? Platform { get; set; }

    [StringLength(120)]
    public string? Franchise { get; set; }

    public ProductCondition Condition { get; set; } = ProductCondition.Good;

    public ProductStatus Status { get; set; } = ProductStatus.Draft;

    public decimal? CostPrice { get; set; }

    public decimal SalePrice { get; set; }

    public int CategoryId { get; set; }

    public ProductCategory? Category { get; set; }

    public InventoryItem? InventoryItem { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    public ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}

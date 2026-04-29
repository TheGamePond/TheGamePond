using System.ComponentModel.DataAnnotations;
using TheGamePond.Models.Catalog;

namespace TheGamePond.Models.Admin;

public class ProductFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [StringLength(160)]
    public string Name { get; set; } = string.Empty;

    [StringLength(4000)]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "SKU")]
    [StringLength(64)]
    public string Sku { get; set; } = string.Empty;

    [StringLength(64)]
    public string? Barcode { get; set; }

    [StringLength(80)]
    public string? Platform { get; set; }

    [StringLength(120)]
    public string? Franchise { get; set; }

    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    public ProductCondition Condition { get; set; } = ProductCondition.Good;

    public ProductStatus Status { get; set; } = ProductStatus.Draft;

    [Display(Name = "Cost price")]
    [Range(0, 999999.99)]
    public decimal? CostPrice { get; set; }

    [Required]
    [Display(Name = "Sale price")]
    [Range(0.01, 999999.99)]
    public decimal SalePrice { get; set; }

    [Display(Name = "Quantity on hand")]
    [Range(0, 999999)]
    public int QuantityOnHand { get; set; }

    [Display(Name = "Low-stock threshold")]
    [Range(0, 999999)]
    public int LowStockThreshold { get; set; } = 1;

    [Display(Name = "Location code")]
    [StringLength(80)]
    public string? LocationCode { get; set; }
}

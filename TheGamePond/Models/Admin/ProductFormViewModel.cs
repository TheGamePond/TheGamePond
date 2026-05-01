using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TheGamePond.Models.Admin;

public class ProductFormViewModel
{
    public int? Id { get; set; }

    [Required]
    [StringLength(160)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Sku { get; set; } = string.Empty;

    [StringLength(80)]
    public string? Barcode { get; set; }

    [Required]
    [StringLength(80)]
    public string Platform { get; set; } = string.Empty;

    [StringLength(120)]
    public string? Franchise { get; set; }

    [Required]
    [StringLength(80)]
    public string Condition { get; set; } = string.Empty;

    [StringLength(1200)]
    public string? Description { get; set; }

    [Display(Name = "Category")]
    [Range(1, int.MaxValue, ErrorMessage = "Choose a category.")]
    public int ProductCategoryId { get; set; }

    [Display(Name = "Cost price")]
    [Range(0, 999999.99)]
    public decimal CostPrice { get; set; }

    [Display(Name = "Sale price")]
    [Range(0.01, 999999.99)]
    public decimal SalePrice { get; set; }

    [Display(Name = "Quantity on hand")]
    [Range(0, 100000)]
    public int QuantityOnHand { get; set; }

    [Display(Name = "Low-stock threshold")]
    [Range(0, 100000)]
    public int LowStockThreshold { get; set; } = 1;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public IFormFile? ImageUpload { get; set; }

    public string? ExistingImagePath { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = Array.Empty<SelectListItem>();
}

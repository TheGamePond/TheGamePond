using System.ComponentModel.DataAnnotations;
using TheGamePond.Models.Catalog;

namespace TheGamePond.Models.Admin;

public class StockAdjustmentViewModel
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public int CurrentQuantity { get; set; }

    [Display(Name = "Quantity change")]
    [Range(-100000, 100000)]
    public int QuantityDelta { get; set; }

    [Required]
    public StockAdjustmentReason Reason { get; set; } = StockAdjustmentReason.Correction;

    [StringLength(500)]
    public string? Notes { get; set; }

    public int NewQuantity => CurrentQuantity + QuantityDelta;
}

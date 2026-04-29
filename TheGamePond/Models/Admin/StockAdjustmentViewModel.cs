using System.ComponentModel.DataAnnotations;
using TheGamePond.Models.Catalog;

namespace TheGamePond.Models.Admin;

public class StockAdjustmentViewModel
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Sku { get; set; } = string.Empty;

    public int CurrentQuantity { get; set; }

    [Required]
    [Display(Name = "Quantity change")]
    [Range(-999999, 999999)]
    public int QuantityDelta { get; set; }

    [Display(Name = "New quantity")]
    public int NewQuantity => CurrentQuantity + QuantityDelta;

    public StockAdjustmentReason Reason { get; set; } = StockAdjustmentReason.Correction;

    [StringLength(500)]
    public string? Notes { get; set; }
}

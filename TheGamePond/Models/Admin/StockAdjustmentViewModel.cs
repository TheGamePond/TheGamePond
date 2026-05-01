using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.Admin;

public class StockAdjustmentViewModel
{
    public int ProductId { get; set; }

    [Display(Name = "Quantity change")]
    [Range(-100000, 100000)]
    public int QuantityChange { get; set; }

    [Required]
    [StringLength(120)]
    public string Reason { get; set; } = string.Empty;
}

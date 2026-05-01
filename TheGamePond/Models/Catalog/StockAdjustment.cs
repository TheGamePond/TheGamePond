using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.Catalog;

public class StockAdjustment
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public int QuantityDelta { get; set; }

    public int QuantityAfter { get; set; }

    public StockAdjustmentReason Reason { get; set; } = StockAdjustmentReason.Correction;

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(450)]
    public string? CreatedByUserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

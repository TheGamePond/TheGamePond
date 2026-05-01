using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.Catalog;

public class StockAdjustment
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public int QuantityChange { get; set; }

    public int QuantityAfter { get; set; }

    [StringLength(120)]
    public string Reason { get; set; } = string.Empty;

    [StringLength(450)]
    public string? AdjustedByUserId { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

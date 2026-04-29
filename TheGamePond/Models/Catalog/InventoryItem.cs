using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.Catalog;

public class InventoryItem
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public int QuantityOnHand { get; set; }

    public int LowStockThreshold { get; set; } = 1;

    [StringLength(80)]
    public string? LocationCode { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }
}

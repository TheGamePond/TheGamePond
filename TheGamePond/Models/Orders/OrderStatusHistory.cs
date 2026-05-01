using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.Orders;

public class OrderStatusHistory
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order? Order { get; set; }

    public OrderStatus FromStatus { get; set; }

    public OrderStatus ToStatus { get; set; }

    [StringLength(450)]
    public string? ChangedByUserId { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

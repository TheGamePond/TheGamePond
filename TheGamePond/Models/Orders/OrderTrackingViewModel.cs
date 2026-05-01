namespace TheGamePond.Models.Orders;

public class OrderTrackingViewModel
{
    public string OrderNumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;

    public OrderStatus Status { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public string? TrackingNumber { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? PaidAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public decimal Total { get; set; }

    public IReadOnlyList<OrderItem> Items { get; set; } = new List<OrderItem>();

    public IReadOnlyList<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
}

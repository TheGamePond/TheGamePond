using TheGamePond.Models.Orders;

namespace TheGamePond.Models.Checkout;

public class OrderConfirmationViewModel
{
    public string OrderNumber { get; set; } = string.Empty;

    public OrderStatus Status { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public decimal Total { get; set; }

    public IReadOnlyList<OrderItem> Items { get; set; } = new List<OrderItem>();
}

namespace TheGamePond.Models.Orders;

public enum OrderStatus
{
    PendingPayment = 1,
    PaymentReceived = 2,
    Processing = 3,
    Packed = 4,
    Shipped = 5,
    Delivered = 6,
    Cancelled = 7,
    Refunded = 8
}

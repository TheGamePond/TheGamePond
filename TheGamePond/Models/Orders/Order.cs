using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.Orders;

public class Order
{
    public int Id { get; set; }

    [Required]
    [StringLength(32)]
    public string OrderNumber { get; set; } = string.Empty;

    public OrderStatus Status { get; set; } = OrderStatus.PendingPayment;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    [StringLength(80)]
    public string PaymentProvider { get; set; } = string.Empty;

    [StringLength(200)]
    public string? PaymentSessionId { get; set; }

    [Required]
    [StringLength(120)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(180)]
    public string CustomerEmail { get; set; } = string.Empty;

    [StringLength(40)]
    public string? CustomerPhone { get; set; }

    [Required]
    [StringLength(180)]
    public string ShippingAddressLine1 { get; set; } = string.Empty;

    [StringLength(180)]
    public string? ShippingAddressLine2 { get; set; }

    [Required]
    [StringLength(90)]
    public string ShippingCity { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string ShippingState { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string ShippingPostalCode { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string ShippingCountry { get; set; } = "United States";

    [StringLength(1000)]
    public string? CustomerNotes { get; set; }

    public decimal Subtotal { get; set; }

    public decimal ShippingTotal { get; set; }

    public decimal TaxTotal { get; set; }

    public decimal Total { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? PaidAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public ICollection<PaymentEvent> PaymentEvents { get; set; } = new List<PaymentEvent>();
}

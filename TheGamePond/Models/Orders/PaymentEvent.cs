using System.ComponentModel.DataAnnotations;

namespace TheGamePond.Models.Orders;

public class PaymentEvent
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order? Order { get; set; }

    [Required]
    [StringLength(80)]
    public string Provider { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string ProviderEventId { get; set; } = string.Empty;

    [StringLength(200)]
    public string? ProviderSessionId { get; set; }

    public PaymentEventType EventType { get; set; }

    public bool IsVerified { get; set; }

    public bool WasProcessed { get; set; }

    [StringLength(2000)]
    public string? RawPayload { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

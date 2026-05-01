using TheGamePond.Models.Orders;

namespace TheGamePond.Services.Payments;

public class PaymentWebhookResult
{
    public bool IsVerified { get; set; }

    public string Provider { get; set; } = string.Empty;

    public string ProviderEventId { get; set; } = string.Empty;

    public string? ProviderSessionId { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public PaymentEventType EventType { get; set; }

    public string? RawPayload { get; set; }
}

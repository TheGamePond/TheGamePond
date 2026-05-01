using System.Text.Json;
using TheGamePond.Models.Orders;

namespace TheGamePond.Services.Payments;

public class LocalTestPaymentGateway : IPaymentGateway
{
    public const string Provider = "local-test-gateway";

    public string ProviderName => Provider;

    public Task<PaymentCheckoutSession> CreateCheckoutSessionAsync(PaymentCheckoutRequest request)
    {
        var sessionId = $"local_{Guid.NewGuid():N}";
        var redirectUrl = $"/Checkout/TestGateway/{Uri.EscapeDataString(request.OrderNumber)}?sessionId={Uri.EscapeDataString(sessionId)}";

        return Task.FromResult(new PaymentCheckoutSession
        {
            Provider = ProviderName,
            ProviderSessionId = sessionId,
            RedirectUrl = redirectUrl
        });
    }

    public async Task<PaymentWebhookResult> VerifyWebhookAsync(HttpRequest request)
    {
        var signature = request.Headers["X-GamePond-Test-Signature"].ToString();
        var rawPayload = await new StreamReader(request.Body).ReadToEndAsync();

        if (!string.Equals(signature, "local-test", StringComparison.Ordinal))
        {
            return new PaymentWebhookResult
            {
                Provider = ProviderName,
                IsVerified = false,
                RawPayload = rawPayload
            };
        }

        var payload = JsonSerializer.Deserialize<LocalPaymentWebhookPayload>(
            rawPayload,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (payload is null)
        {
            return new PaymentWebhookResult
            {
                Provider = ProviderName,
                IsVerified = false,
                RawPayload = rawPayload
            };
        }

        return new PaymentWebhookResult
        {
            Provider = ProviderName,
            IsVerified = true,
            ProviderEventId = payload.EventId,
            ProviderSessionId = payload.SessionId,
            OrderNumber = payload.OrderNumber,
            EventType = payload.Succeeded ? PaymentEventType.PaymentSucceeded : PaymentEventType.PaymentFailed,
            RawPayload = rawPayload
        };
    }

    private sealed class LocalPaymentWebhookPayload
    {
        public string EventId { get; set; } = string.Empty;

        public string SessionId { get; set; } = string.Empty;

        public string OrderNumber { get; set; } = string.Empty;

        public bool Succeeded { get; set; }
    }
}

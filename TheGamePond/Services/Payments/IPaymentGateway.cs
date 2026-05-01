namespace TheGamePond.Services.Payments;

public interface IPaymentGateway
{
    string ProviderName { get; }

    Task<PaymentCheckoutSession> CreateCheckoutSessionAsync(PaymentCheckoutRequest request);

    Task<PaymentWebhookResult> VerifyWebhookAsync(HttpRequest request);
}

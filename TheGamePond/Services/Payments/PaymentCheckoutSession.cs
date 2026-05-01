namespace TheGamePond.Services.Payments;

public class PaymentCheckoutSession
{
    public string Provider { get; set; } = string.Empty;

    public string ProviderSessionId { get; set; } = string.Empty;

    public string RedirectUrl { get; set; } = string.Empty;
}

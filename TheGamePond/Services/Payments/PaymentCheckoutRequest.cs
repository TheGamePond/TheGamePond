namespace TheGamePond.Services.Payments;

public class PaymentCheckoutRequest
{
    public int OrderId { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = "USD";

    public string CustomerEmail { get; set; } = string.Empty;

    public string SuccessUrl { get; set; } = string.Empty;

    public string CancelUrl { get; set; } = string.Empty;
}

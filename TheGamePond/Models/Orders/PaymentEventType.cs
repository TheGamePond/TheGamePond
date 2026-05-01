namespace TheGamePond.Models.Orders;

public enum PaymentEventType
{
    CheckoutSessionCreated = 1,
    PaymentSucceeded = 2,
    PaymentFailed = 3,
    PaymentCancelled = 4,
    WebhookReceived = 5
}

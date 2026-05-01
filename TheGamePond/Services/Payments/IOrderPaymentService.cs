using TheGamePond.Models.Orders;

namespace TheGamePond.Services.Payments;

public interface IOrderPaymentService
{
    Task<bool> MarkOrderPaidAsync(Order order, PaymentWebhookResult paymentResult);
}

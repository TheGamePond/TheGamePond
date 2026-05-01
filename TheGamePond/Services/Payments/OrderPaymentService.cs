using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models.Catalog;
using TheGamePond.Models.Orders;

namespace TheGamePond.Services.Payments;

public class OrderPaymentService : IOrderPaymentService
{
    private readonly ApplicationDbContext _context;

    public OrderPaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> MarkOrderPaidAsync(Order order, PaymentWebhookResult paymentResult)
    {
        var existingEvent = await _context.PaymentEvents
            .AnyAsync(item => item.Provider == paymentResult.Provider && item.ProviderEventId == paymentResult.ProviderEventId);

        if (existingEvent)
        {
            return false;
        }

        var paymentEvent = new PaymentEvent
        {
            OrderId = order.Id,
            Provider = paymentResult.Provider,
            ProviderEventId = paymentResult.ProviderEventId,
            ProviderSessionId = paymentResult.ProviderSessionId,
            EventType = paymentResult.EventType,
            IsVerified = paymentResult.IsVerified,
            RawPayload = paymentResult.RawPayload
        };

        _context.PaymentEvents.Add(paymentEvent);

        if (!paymentResult.IsVerified || paymentResult.EventType != PaymentEventType.PaymentSucceeded)
        {
            order.PaymentStatus = PaymentStatus.Failed;
            order.UpdatedAt = DateTimeOffset.UtcNow;
            paymentEvent.WasProcessed = true;
            await _context.SaveChangesAsync();
            return false;
        }

        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            paymentEvent.WasProcessed = true;
            await _context.SaveChangesAsync();
            return false;
        }

        foreach (var item in order.Items)
        {
            var inventory = await _context.InventoryItems
                .Include(inventoryItem => inventoryItem.Product)
                .FirstOrDefaultAsync(inventoryItem => inventoryItem.ProductId == item.ProductId);

            if (inventory is null || inventory.QuantityOnHand < item.Quantity)
            {
                order.PaymentStatus = PaymentStatus.Failed;
                order.UpdatedAt = DateTimeOffset.UtcNow;
                paymentEvent.WasProcessed = true;
                paymentEvent.RawPayload = $"{paymentResult.RawPayload}\nStock validation failed for product {item.ProductId}.";
                await _context.SaveChangesAsync();
                return false;
            }
        }

        foreach (var item in order.Items)
        {
            var inventory = await _context.InventoryItems
                .FirstAsync(inventoryItem => inventoryItem.ProductId == item.ProductId);

            inventory.QuantityOnHand -= item.Quantity;
            inventory.UpdatedAt = DateTimeOffset.UtcNow;

            _context.StockAdjustments.Add(new StockAdjustment
            {
                ProductId = item.ProductId,
                QuantityDelta = -item.Quantity,
                QuantityAfter = inventory.QuantityOnHand,
                Reason = StockAdjustmentReason.ManualSale,
                Notes = $"Order {order.OrderNumber} paid"
            });
        }

        order.Status = OrderStatus.PaymentReceived;
        order.PaymentStatus = PaymentStatus.Paid;
        order.PaidAt = DateTimeOffset.UtcNow;
        order.UpdatedAt = DateTimeOffset.UtcNow;
        paymentEvent.WasProcessed = true;

        await _context.SaveChangesAsync();
        return true;
    }
}

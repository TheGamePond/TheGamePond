using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Services.Payments;

namespace TheGamePond.Controllers;

[ApiController]
[Route("Payments/Webhooks")]
public class PaymentWebhookController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IOrderPaymentService _orderPaymentService;

    public PaymentWebhookController(
        ApplicationDbContext context,
        IPaymentGateway paymentGateway,
        IOrderPaymentService orderPaymentService)
    {
        _context = context;
        _paymentGateway = paymentGateway;
        _orderPaymentService = orderPaymentService;
    }

    [HttpPost("local-test-gateway")]
    public async Task<IActionResult> LocalTestGateway()
    {
        var result = await _paymentGateway.VerifyWebhookAsync(Request);

        if (!result.IsVerified)
        {
            return Unauthorized();
        }

        var order = await _context.Orders
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.OrderNumber == result.OrderNumber);

        if (order is null)
        {
            return NotFound();
        }

        await _orderPaymentService.MarkOrderPaidAsync(order, result);
        return Ok();
    }
}

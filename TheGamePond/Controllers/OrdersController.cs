using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models.Orders;

namespace TheGamePond.Controllers;

[Route("Orders")]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("Lookup")]
    public IActionResult Lookup(string? orderNumber = null)
    {
        return View(new OrderLookupViewModel { OrderNumber = orderNumber ?? string.Empty });
    }

    [HttpPost("Lookup")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lookup(OrderLookupViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var orderNumber = model.OrderNumber.Trim();
        var customerEmail = model.CustomerEmail.Trim();
        var order = await FindCustomerOrderAsync(orderNumber, customerEmail);

        if (order is null)
        {
            ModelState.AddModelError(string.Empty, "We could not find an order with that order number and email address.");
            return View(model);
        }

        HttpContext.Session.SetString(CreateLookupSessionKey(order.OrderNumber), customerEmail);

        return RedirectToAction(nameof(Details), new { orderNumber = order.OrderNumber });
    }

    [HttpGet("{orderNumber}")]
    public async Task<IActionResult> Details(string orderNumber)
    {
        var customerEmail = HttpContext.Session.GetString(CreateLookupSessionKey(orderNumber));

        if (string.IsNullOrWhiteSpace(customerEmail))
        {
            return RedirectToAction(nameof(Lookup), new { orderNumber });
        }

        var order = await FindCustomerOrderAsync(orderNumber, customerEmail);

        if (order is null)
        {
            return RedirectToAction(nameof(Lookup));
        }

        return View(new OrderTrackingViewModel
        {
            OrderNumber = order.OrderNumber,
            CustomerName = order.CustomerName,
            Status = order.Status,
            PaymentStatus = order.PaymentStatus,
            TrackingNumber = order.TrackingNumber,
            CreatedAt = order.CreatedAt,
            PaidAt = order.PaidAt,
            UpdatedAt = order.UpdatedAt,
            Total = order.Total,
            Items = order.Items.OrderBy(item => item.ProductName).ToList(),
            StatusHistory = order.StatusHistory.OrderByDescending(history => history.CreatedAt).ToList()
        });
    }

    private async Task<Order?> FindCustomerOrderAsync(string orderNumber, string customerEmail)
    {
        var normalizedOrderNumber = orderNumber.Trim();
        var normalizedEmail = customerEmail.Trim().ToUpperInvariant();

        return await _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Include(order => order.StatusHistory)
            .FirstOrDefaultAsync(order =>
                order.OrderNumber == normalizedOrderNumber &&
                order.CustomerEmail.ToUpper() == normalizedEmail);
    }

    private static string CreateLookupSessionKey(string orderNumber)
    {
        return $"VerifiedOrderLookup:{orderNumber.Trim()}";
    }
}

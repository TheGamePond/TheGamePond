using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TheGamePond.Data;
using TheGamePond.Models.Admin;
using TheGamePond.Models.Orders;

namespace TheGamePond.Controllers;

[Authorize(Roles = $"{AppRoles.Owner},{AppRoles.Admin},{AppRoles.Staff}")]
[Route("Admin/Orders")]
public class AdminOrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminOrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(OrderStatus? status = null)
    {
        var ordersQuery = _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .AsQueryable();

        if (status.HasValue)
        {
            ordersQuery = ordersQuery.Where(order => order.Status == status.Value);
        }

        ViewData["Status"] = status;

        var orders = await ordersQuery
            .OrderByDescending(order => order.CreatedAt)
            .Take(100)
            .ToListAsync();

        return View(orders);
    }

    [HttpGet("{orderNumber}")]
    public async Task<IActionResult> Details(string orderNumber)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(item => item.Items)
            .Include(item => item.PaymentEvents.OrderByDescending(paymentEvent => paymentEvent.CreatedAt))
            .Include(item => item.StatusHistory.OrderByDescending(history => history.CreatedAt))
            .FirstOrDefaultAsync(item => item.OrderNumber == orderNumber);

        if (order is null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost("{orderNumber}/Status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(string orderNumber, OrderStatusUpdateViewModel model)
    {
        if (!string.Equals(orderNumber, model.OrderNumber, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest();
        }

        var order = await _context.Orders.FirstOrDefaultAsync(item => item.OrderNumber == orderNumber);

        if (order is null)
        {
            return NotFound();
        }

        if (!CanTransition(order.Status, model.NextStatus))
        {
            TempData["StatusMessage"] = $"Cannot move order from {order.Status} to {model.NextStatus}.";
            return RedirectToAction(nameof(Details), new { orderNumber });
        }

        var previousStatus = order.Status;
        order.Status = model.NextStatus;
        order.TrackingNumber = NormalizeOptional(model.TrackingNumber);
        order.StaffNotes = NormalizeOptional(model.StaffNotes);
        order.UpdatedAt = DateTimeOffset.UtcNow;

        if (model.NextStatus == OrderStatus.Refunded)
        {
            order.PaymentStatus = PaymentStatus.Refunded;
        }

        if (model.NextStatus == OrderStatus.Cancelled && order.PaymentStatus == PaymentStatus.Pending)
        {
            order.PaymentStatus = PaymentStatus.Cancelled;
        }

        _context.OrderStatusHistory.Add(new OrderStatusHistory
        {
            OrderId = order.Id,
            FromStatus = previousStatus,
            ToStatus = model.NextStatus,
            ChangedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Notes = NormalizeOptional(model.StaffNotes)
        });

        await _context.SaveChangesAsync();
        TempData["StatusMessage"] = $"Order moved to {model.NextStatus}.";

        return RedirectToAction(nameof(Details), new { orderNumber });
    }

    [HttpPost("{orderNumber}/Fulfillment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateFulfillment(string orderNumber, OrderStatusUpdateViewModel model)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(item => item.OrderNumber == orderNumber);

        if (order is null)
        {
            return NotFound();
        }

        order.TrackingNumber = NormalizeOptional(model.TrackingNumber);
        order.StaffNotes = NormalizeOptional(model.StaffNotes);
        order.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        TempData["StatusMessage"] = "Fulfillment details saved.";

        return RedirectToAction(nameof(Details), new { orderNumber });
    }

    public static IReadOnlyList<OrderStatus> GetAllowedNextStatuses(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.PendingPayment => new[] { OrderStatus.Cancelled },
            OrderStatus.PaymentReceived => new[] { OrderStatus.Processing, OrderStatus.Cancelled, OrderStatus.Refunded },
            OrderStatus.Processing => new[] { OrderStatus.Packed, OrderStatus.Cancelled, OrderStatus.Refunded },
            OrderStatus.Packed => new[] { OrderStatus.Shipped, OrderStatus.Cancelled, OrderStatus.Refunded },
            OrderStatus.Shipped => new[] { OrderStatus.Delivered, OrderStatus.Refunded },
            OrderStatus.Delivered => new[] { OrderStatus.Refunded },
            _ => Array.Empty<OrderStatus>()
        };
    }

    private static bool CanTransition(OrderStatus currentStatus, OrderStatus nextStatus)
    {
        return GetAllowedNextStatuses(currentStatus).Contains(nextStatus);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
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
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
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
            .FirstOrDefaultAsync(item => item.OrderNumber == orderNumber);

        if (order is null)
        {
            return NotFound();
        }

        return View(order);
    }
}

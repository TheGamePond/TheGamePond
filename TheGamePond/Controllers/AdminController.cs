using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models.Admin;
using TheGamePond.Models.Catalog;
using TheGamePond.Models.TradeIns;

namespace TheGamePond.Controllers;

[Authorize(Roles = $"{AppRoles.Owner},{AppRoles.Admin},{AppRoles.Staff}")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var lowStockProducts = await _context.Products
            .Include(product => product.InventoryItem)
            .AsNoTracking()
            .Where(product =>
                product.Status == ProductStatus.Active &&
                product.InventoryItem != null &&
                product.InventoryItem.QuantityOnHand <= product.InventoryItem.LowStockThreshold)
            .OrderBy(product => product.InventoryItem!.QuantityOnHand)
            .ThenBy(product => product.Name)
            .Take(5)
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            ProductCount = await _context.Products.CountAsync(),
            ActiveProductCount = await _context.Products.CountAsync(product => product.Status == ProductStatus.Active),
            DraftProductCount = await _context.Products.CountAsync(product => product.Status == ProductStatus.Draft),
            LowStockCount = await _context.Products.CountAsync(product =>
                product.Status == ProductStatus.Active &&
                product.InventoryItem != null &&
                product.InventoryItem.QuantityOnHand <= product.InventoryItem.LowStockThreshold),
            OpenTradeInCount = await _context.TradeInRequests.CountAsync(request =>
                request.Status == TradeInRequestStatus.Submitted ||
                request.Status == TradeInRequestStatus.UnderReview ||
                request.Status == TradeInRequestStatus.OfferSent),
            LowStockProducts = lowStockProducts
        };

        return View(model);
    }
}

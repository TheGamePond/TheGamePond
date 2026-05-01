using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models.Admin;

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
                product.IsActive &&
                product.InventoryItem != null &&
                product.InventoryItem.QuantityOnHand <= product.InventoryItem.LowStockThreshold)
            .OrderBy(product => product.InventoryItem!.QuantityOnHand)
            .ThenBy(product => product.Name)
            .Take(5)
            .ToListAsync();

        var model = new AdminDashboardViewModel
        {
            ProductCount = await _context.Products.CountAsync(),
            ActiveProductCount = await _context.Products.CountAsync(product => product.IsActive),
            DraftProductCount = await _context.Products.CountAsync(product => !product.IsActive),
            LowStockCount = await _context.Products.CountAsync(product =>
                product.IsActive &&
                product.InventoryItem != null &&
                product.InventoryItem.QuantityOnHand <= product.InventoryItem.LowStockThreshold),
            LowStockProducts = lowStockProducts
        };

        return View(model);
    }
}

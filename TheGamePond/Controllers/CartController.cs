using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models.Cart;
using TheGamePond.Models.Catalog;

namespace TheGamePond.Controllers;

[Route("Cart")]
public class CartController : Controller
{
    private const string CartSessionKey = "TheGamePond.Cart";

    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var model = await BuildCartViewModelAsync();
        return View(model);
    }

    [HttpPost("Add/{slug}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string slug, int quantity = 1)
    {
        var product = await _context.Products
            .Include(item => item.InventoryItem)
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Slug == slug && product.Status == ProductStatus.Active);

        if (product is null)
        {
            return NotFound();
        }

        var availableQuantity = product.InventoryItem?.QuantityOnHand ?? 0;

        if (availableQuantity <= 0)
        {
            TempData["StatusMessage"] = "That item is sold out.";
            return RedirectToAction("Details", "Shop", new { slug });
        }

        var cart = GetCartItems();
        var requestedQuantity = Math.Max(1, quantity);
        var existingItem = cart.FirstOrDefault(item => item.ProductSlug == slug);

        if (existingItem is null)
        {
            cart.Add(new CartSessionItem
            {
                ProductSlug = slug,
                Quantity = Math.Min(requestedQuantity, availableQuantity)
            });
        }
        else
        {
            existingItem.Quantity = Math.Min(existingItem.Quantity + requestedQuantity, availableQuantity);
        }

        SaveCartItems(cart);
        TempData["StatusMessage"] = "Added to cart.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string slug, int quantity)
    {
        var cart = GetCartItems();
        var existingItem = cart.FirstOrDefault(item => item.ProductSlug == slug);

        if (existingItem is null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (quantity <= 0)
        {
            cart.Remove(existingItem);
            SaveCartItems(cart);
            TempData["StatusMessage"] = "Item removed.";
            return RedirectToAction(nameof(Index));
        }

        var availableQuantity = await _context.Products
            .Where(product => product.Slug == slug && product.Status == ProductStatus.Active)
            .Select(product => product.InventoryItem == null ? 0 : product.InventoryItem.QuantityOnHand)
            .FirstOrDefaultAsync();

        if (availableQuantity <= 0)
        {
            cart.Remove(existingItem);
            TempData["StatusMessage"] = "That item is no longer available.";
        }
        else
        {
            existingItem.Quantity = Math.Min(quantity, availableQuantity);
            TempData["StatusMessage"] = existingItem.Quantity == quantity
                ? "Cart updated."
                : "Cart updated to match available stock.";
        }

        SaveCartItems(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Remove/{slug}")]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(string slug)
    {
        var cart = GetCartItems();
        var existingItem = cart.FirstOrDefault(item => item.ProductSlug == slug);

        if (existingItem is not null)
        {
            cart.Remove(existingItem);
            SaveCartItems(cart);
            TempData["StatusMessage"] = "Item removed.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<CartViewModel> BuildCartViewModelAsync()
    {
        var cart = GetCartItems();

        if (cart.Count == 0)
        {
            return new CartViewModel();
        }

        var slugs = cart.Select(item => item.ProductSlug).ToList();
        var products = await _context.Products
            .Include(item => item.InventoryItem)
            .Include(item => item.Images)
            .AsNoTracking()
            .Where(product => slugs.Contains(product.Slug) && product.Status == ProductStatus.Active)
            .ToListAsync();

        var resolvedItems = new List<CartItemViewModel>();
        var changed = false;

        foreach (var cartItem in cart.ToList())
        {
            var product = products.FirstOrDefault(item => item.Slug == cartItem.ProductSlug);
            var availableQuantity = product?.InventoryItem?.QuantityOnHand ?? 0;

            if (product is null || availableQuantity <= 0)
            {
                cart.Remove(cartItem);
                changed = true;
                continue;
            }

            if (cartItem.Quantity > availableQuantity)
            {
                cartItem.Quantity = availableQuantity;
                changed = true;
            }

            var primaryImage = product.Images
                .OrderByDescending(image => image.IsPrimary)
                .ThenBy(image => image.SortOrder)
                .FirstOrDefault();

            resolvedItems.Add(new CartItemViewModel
            {
                ProductSlug = product.Slug,
                Name = product.Name,
                Sku = product.Sku,
                Platform = product.Platform,
                ImagePath = primaryImage?.ImagePath,
                ImageAltText = primaryImage?.AltText,
                UnitPrice = product.SalePrice,
                Quantity = cartItem.Quantity,
                QuantityOnHand = availableQuantity
            });
        }

        if (changed)
        {
            SaveCartItems(cart);
        }

        return new CartViewModel
        {
            Items = resolvedItems
        };
    }

    private List<CartSessionItem> GetCartItems()
    {
        var json = HttpContext.Session.GetString(CartSessionKey);

        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<CartSessionItem>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<CartSessionItem>>(json) ?? new List<CartSessionItem>();
        }
        catch (JsonException)
        {
            return new List<CartSessionItem>();
        }
    }

    private void SaveCartItems(List<CartSessionItem> items)
    {
        if (items.Count == 0)
        {
            HttpContext.Session.Remove(CartSessionKey);
            return;
        }

        HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(items));
    }
}

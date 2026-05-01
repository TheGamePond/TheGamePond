using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models.Cart;
using TheGamePond.Models.Checkout;
using TheGamePond.Models.Orders;
using TheGamePond.Services.Cart;
using TheGamePond.Services.Payments;

namespace TheGamePond.Controllers;

[Route("Checkout")]
public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICartSessionService _cartSession;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IOrderPaymentService _orderPaymentService;

    public CheckoutController(
        ApplicationDbContext context,
        ICartSessionService cartSession,
        IPaymentGateway paymentGateway,
        IOrderPaymentService orderPaymentService)
    {
        _context = context;
        _cartSession = cartSession;
        _paymentGateway = paymentGateway;
        _orderPaymentService = orderPaymentService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var cart = await BuildCartViewModelAsync();

        if (cart.IsEmpty)
        {
            TempData["StatusMessage"] = "Add an item before checkout.";
            return RedirectToAction("Index", "Cart");
        }

        return View(new CheckoutViewModel { Cart = cart });
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutViewModel model)
    {
        var cart = await BuildCartViewModelAsync();
        model.Cart = cart;

        if (cart.IsEmpty)
        {
            TempData["StatusMessage"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var products = await LoadCartProductsAsync(_cartSession.GetItems());
        var stockError = ValidateStock(cart, products);

        if (!string.IsNullOrWhiteSpace(stockError))
        {
            TempData["StatusMessage"] = stockError;
            return RedirectToAction("Index", "Cart");
        }

        var order = await CreateOrderAsync(model, cart, products);
        var session = await _paymentGateway.CreateCheckoutSessionAsync(new PaymentCheckoutRequest
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            Amount = order.Total,
            CustomerEmail = order.CustomerEmail,
            SuccessUrl = Url.ActionLink(nameof(Confirmation), "Checkout", new { orderNumber = order.OrderNumber }) ?? string.Empty,
            CancelUrl = Url.ActionLink("Index", "Cart") ?? string.Empty
        });

        order.PaymentProvider = session.Provider;
        order.PaymentSessionId = session.ProviderSessionId;
        _context.PaymentEvents.Add(new PaymentEvent
        {
            OrderId = order.Id,
            Provider = session.Provider,
            ProviderEventId = $"session-created-{session.ProviderSessionId}",
            ProviderSessionId = session.ProviderSessionId,
            EventType = PaymentEventType.CheckoutSessionCreated,
            IsVerified = true,
            WasProcessed = true
        });

        await _context.SaveChangesAsync();

        return Redirect(session.RedirectUrl);
    }

    [HttpGet("TestGateway/{orderNumber}")]
    public async Task<IActionResult> TestGateway(string orderNumber, string sessionId)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.OrderNumber == orderNumber && item.PaymentSessionId == sessionId);

        if (order is null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost("TestGateway/{orderNumber}/Complete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteTestGateway(string orderNumber, string sessionId)
    {
        var order = await _context.Orders
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.OrderNumber == orderNumber && item.PaymentSessionId == sessionId);

        if (order is null)
        {
            return NotFound();
        }

        var result = new PaymentWebhookResult
        {
            Provider = LocalTestPaymentGateway.Provider,
            IsVerified = true,
            ProviderEventId = $"local-payment-{sessionId}",
            ProviderSessionId = sessionId,
            OrderNumber = order.OrderNumber,
            EventType = PaymentEventType.PaymentSucceeded,
            RawPayload = JsonSerializer.Serialize(new
            {
                order.OrderNumber,
                SessionId = sessionId,
                Succeeded = true,
                Source = "local test gateway"
            })
        };

        var paid = await _orderPaymentService.MarkOrderPaidAsync(order, result);

        if (paid)
        {
            _cartSession.SaveItems(new List<CartSessionItem>());
        }

        return RedirectToAction(nameof(Confirmation), new { orderNumber = order.OrderNumber });
    }

    [HttpGet("Confirmation/{orderNumber}")]
    public async Task<IActionResult> Confirmation(string orderNumber)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.OrderNumber == orderNumber);

        if (order is null)
        {
            return NotFound();
        }

        return View(new OrderConfirmationViewModel
        {
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            PaymentStatus = order.PaymentStatus,
            Total = order.Total,
            Items = order.Items.OrderBy(item => item.ProductName).ToList()
        });
    }

    private async Task<Order> CreateOrderAsync(CheckoutViewModel model, CartViewModel cart, IReadOnlyList<TheGamePond.Models.Catalog.Product> products)
    {
        var order = new Order
        {
            OrderNumber = await CreateOrderNumberAsync(),
            CustomerName = model.CustomerName.Trim(),
            CustomerEmail = model.CustomerEmail.Trim(),
            CustomerPhone = NormalizeOptional(model.CustomerPhone),
            ShippingAddressLine1 = model.ShippingAddressLine1.Trim(),
            ShippingAddressLine2 = NormalizeOptional(model.ShippingAddressLine2),
            ShippingCity = model.ShippingCity.Trim(),
            ShippingState = model.ShippingState.Trim(),
            ShippingPostalCode = model.ShippingPostalCode.Trim(),
            ShippingCountry = model.ShippingCountry.Trim(),
            CustomerNotes = NormalizeOptional(model.CustomerNotes),
            Subtotal = cart.Subtotal,
            ShippingTotal = 0,
            TaxTotal = 0,
            Total = cart.Subtotal
        };

        foreach (var cartItem in cart.Items)
        {
            var product = products.First(item => item.Slug == cartItem.ProductSlug);
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Sku = product.Sku,
                ProductSlug = product.Slug,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                LineTotal = cartItem.LineTotal
            });
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    private async Task<CartViewModel> BuildCartViewModelAsync()
    {
        var cart = _cartSession.GetItems();

        if (cart.Count == 0)
        {
            return new CartViewModel();
        }

        var products = await LoadCartProductsAsync(cart);
        var items = new List<CartItemViewModel>();
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

            items.Add(new CartItemViewModel
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
            _cartSession.SaveItems(cart);
        }

        return new CartViewModel { Items = items };
    }

    private async Task<List<TheGamePond.Models.Catalog.Product>> LoadCartProductsAsync(IReadOnlyList<CartSessionItem> cart)
    {
        var slugs = cart.Select(item => item.ProductSlug).ToList();

        return await _context.Products
            .Include(item => item.InventoryItem)
            .Include(item => item.Images)
            .Where(product => slugs.Contains(product.Slug) && product.Status == TheGamePond.Models.Catalog.ProductStatus.Active)
            .ToListAsync();
    }

    private static string? ValidateStock(CartViewModel cart, IReadOnlyList<TheGamePond.Models.Catalog.Product> products)
    {
        foreach (var item in cart.Items)
        {
            var product = products.FirstOrDefault(product => product.Slug == item.ProductSlug);
            var availableQuantity = product?.InventoryItem?.QuantityOnHand ?? 0;

            if (product is null || availableQuantity <= 0)
            {
                return $"{item.Name} is no longer available.";
            }

            if (item.Quantity > availableQuantity)
            {
                return $"{item.Name} only has {availableQuantity} available.";
            }
        }

        return null;
    }

    private async Task<string> CreateOrderNumberAsync()
    {
        var datePrefix = DateTimeOffset.UtcNow.ToString("yyyyMMdd");

        for (var attempt = 0; attempt < 10; attempt++)
        {
            var candidate = $"TGP-{datePrefix}-{Random.Shared.Next(1000, 9999)}";
            var exists = await _context.Orders.AnyAsync(order => order.OrderNumber == candidate);

            if (!exists)
            {
                return candidate;
            }
        }

        return $"TGP-{datePrefix}-{Guid.NewGuid():N}"[..22];
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

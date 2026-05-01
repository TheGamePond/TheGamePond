using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TheGamePond.Data;
using TheGamePond.Models.Admin;
using TheGamePond.Models.Catalog;

namespace TheGamePond.Controllers;

[Authorize(Roles = $"{AppRoles.Owner},{AppRoles.Admin},{AppRoles.Staff}")]
[Route("Admin/Products")]
public class AdminProductsController : Controller
{
    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;

    public AdminProductsController(ApplicationDbContext dbContext, IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _environment = environment;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? search = null, bool lowStock = false)
    {
        var productsQuery = _dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Include(product => product.InventoryItem)
            .Include(product => product.Images)
            .OrderBy(product => product.Name)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var trimmedSearch = search.Trim();
            productsQuery = productsQuery.Where(product =>
                product.Name.Contains(trimmedSearch) ||
                product.Sku.Contains(trimmedSearch) ||
                (product.Barcode != null && product.Barcode.Contains(trimmedSearch)));
        }

        if (lowStock)
        {
            productsQuery = productsQuery.Where(product =>
                product.InventoryItem != null &&
                product.InventoryItem.QuantityOnHand <= product.InventoryItem.LowStockThreshold);
        }

        ViewData["Search"] = search;
        ViewData["LowStock"] = lowStock;

        return View(await productsQuery.ToListAsync());
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        return View(await BuildFormViewModelAsync(new ProductFormViewModel()));
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        await ValidateProductFormAsync(model);

        if (!ModelState.IsValid)
        {
            return View(await BuildFormViewModelAsync(model));
        }

        var product = new Product
        {
            Name = model.Name.Trim(),
            Slug = await CreateUniqueSlugAsync(model.Name),
            Sku = model.Sku.Trim(),
            Barcode = NormalizeOptional(model.Barcode),
            Platform = model.Platform.Trim(),
            Franchise = NormalizeOptional(model.Franchise),
            Condition = model.Condition.Trim(),
            Description = NormalizeOptional(model.Description),
            ProductCategoryId = model.ProductCategoryId,
            CostPrice = model.CostPrice,
            SalePrice = model.SalePrice,
            IsActive = model.IsActive,
            InventoryItem = new InventoryItem
            {
                QuantityOnHand = model.QuantityOnHand,
                LowStockThreshold = model.LowStockThreshold
            }
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        if (model.ImageUpload is not null)
        {
            var imagePath = await SaveProductImageAsync(model.ImageUpload);
            _dbContext.ProductImages.Add(new ProductImage
            {
                ProductId = product.Id,
                FilePath = imagePath,
                AltText = product.Name,
                IsPrimary = true
            });
        }

        if (model.QuantityOnHand > 0)
        {
            _dbContext.StockAdjustments.Add(new StockAdjustment
            {
                ProductId = product.Id,
                QuantityChange = model.QuantityOnHand,
                QuantityAfter = model.QuantityOnHand,
                Reason = "Initial stock",
                AdjustedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            });
        }

        await _dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Product created.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .Include(item => item.Category)
            .Include(item => item.InventoryItem)
            .Include(item => item.Images)
            .Include(item => item.StockAdjustments.OrderByDescending(adjustment => adjustment.CreatedAt))
            .FirstOrDefaultAsync(item => item.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpGet("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _dbContext.Products
            .Include(item => item.InventoryItem)
            .Include(item => item.Images)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        var model = new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Barcode = product.Barcode,
            Platform = product.Platform,
            Franchise = product.Franchise,
            Condition = product.Condition,
            Description = product.Description,
            ProductCategoryId = product.ProductCategoryId,
            CostPrice = product.CostPrice,
            SalePrice = product.SalePrice,
            QuantityOnHand = product.InventoryItem?.QuantityOnHand ?? 0,
            LowStockThreshold = product.InventoryItem?.LowStockThreshold ?? 1,
            IsActive = product.IsActive,
            ExistingImagePath = product.Images.FirstOrDefault(image => image.IsPrimary)?.FilePath
        };

        return View(await BuildFormViewModelAsync(model));
    }

    [HttpPost("{id:int}/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        await ValidateProductFormAsync(model);

        if (!ModelState.IsValid)
        {
            return View(await BuildFormViewModelAsync(model));
        }

        var product = await _dbContext.Products
            .Include(item => item.InventoryItem)
            .Include(item => item.Images)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        product.Name = model.Name.Trim();
        product.Slug = await CreateUniqueSlugAsync(model.Name, product.Id);
        product.Sku = model.Sku.Trim();
        product.Barcode = NormalizeOptional(model.Barcode);
        product.Platform = model.Platform.Trim();
        product.Franchise = NormalizeOptional(model.Franchise);
        product.Condition = model.Condition.Trim();
        product.Description = NormalizeOptional(model.Description);
        product.ProductCategoryId = model.ProductCategoryId;
        product.CostPrice = model.CostPrice;
        product.SalePrice = model.SalePrice;
        product.IsActive = model.IsActive;
        product.UpdatedAt = DateTimeOffset.UtcNow;

        product.InventoryItem ??= new InventoryItem { ProductId = product.Id };
        var originalQuantity = product.InventoryItem.QuantityOnHand;
        product.InventoryItem.QuantityOnHand = model.QuantityOnHand;
        product.InventoryItem.LowStockThreshold = model.LowStockThreshold;
        product.InventoryItem.UpdatedAt = DateTimeOffset.UtcNow;

        if (originalQuantity != model.QuantityOnHand)
        {
            _dbContext.StockAdjustments.Add(new StockAdjustment
            {
                ProductId = product.Id,
                QuantityChange = model.QuantityOnHand - originalQuantity,
                QuantityAfter = model.QuantityOnHand,
                Reason = "Manual count update",
                AdjustedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            });
        }

        if (model.ImageUpload is not null)
        {
            var imagePath = await SaveProductImageAsync(model.ImageUpload);
            foreach (var existingImage in product.Images)
            {
                existingImage.IsPrimary = false;
            }

            _dbContext.ProductImages.Add(new ProductImage
            {
                ProductId = product.Id,
                FilePath = imagePath,
                AltText = product.Name,
                IsPrimary = true
            });
        }

        await _dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Product updated.";

        return RedirectToAction(nameof(Details), new { id = product.Id });
    }

    [HttpPost("{id:int}/AdjustStock")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdjustStock(int id, StockAdjustmentViewModel model)
    {
        if (id != model.ProductId)
        {
            return BadRequest();
        }

        if (model.QuantityChange == 0)
        {
            ModelState.AddModelError(nameof(model.QuantityChange), "Enter a non-zero stock change.");
        }

        var product = await _dbContext.Products
            .Include(item => item.InventoryItem)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            TempData["StatusMessage"] = "Stock adjustment was not saved. Check the quantity and reason.";
            return RedirectToAction(nameof(Details), new { id });
        }

        product.InventoryItem ??= new InventoryItem { ProductId = product.Id };
        var adjustedQuantity = product.InventoryItem.QuantityOnHand + model.QuantityChange;

        if (adjustedQuantity < 0)
        {
            TempData["StatusMessage"] = "Stock cannot be adjusted below zero.";
            return RedirectToAction(nameof(Details), new { id });
        }

        product.InventoryItem.QuantityOnHand = adjustedQuantity;
        product.InventoryItem.UpdatedAt = DateTimeOffset.UtcNow;
        product.UpdatedAt = DateTimeOffset.UtcNow;

        _dbContext.StockAdjustments.Add(new StockAdjustment
        {
            ProductId = product.Id,
            QuantityChange = model.QuantityChange,
            QuantityAfter = adjustedQuantity,
            Reason = model.Reason.Trim(),
            AdjustedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        await _dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Stock adjusted.";

        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task<ProductFormViewModel> BuildFormViewModelAsync(ProductFormViewModel model)
    {
        model.Categories = await _dbContext.ProductCategories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
            .ToListAsync();

        return model;
    }

    private async Task ValidateProductFormAsync(ProductFormViewModel model)
    {
        if (model.ImageUpload is not null && !AllowedImageTypes.Contains(model.ImageUpload.ContentType))
        {
            ModelState.AddModelError(nameof(model.ImageUpload), "Upload a JPG, PNG, or WebP image.");
        }

        var skuExists = await _dbContext.Products
            .AnyAsync(product => product.Sku == (model.Sku ?? string.Empty).Trim() && product.Id != model.Id);

        if (skuExists)
        {
            ModelState.AddModelError(nameof(model.Sku), "SKU must be unique.");
        }
    }

    private async Task<string> SaveProductImageAsync(IFormFile upload)
    {
        var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "products");
        Directory.CreateDirectory(uploadsRoot);

        var extension = Path.GetExtension(upload.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(uploadsRoot, fileName);

        await using var stream = System.IO.File.Create(absolutePath);
        await upload.CopyToAsync(stream);

        return $"/uploads/products/{fileName}";
    }

    private async Task<string> CreateUniqueSlugAsync(string value, int? productId = null)
    {
        var baseSlug = CreateSlug(value);
        var slug = baseSlug;
        var suffix = 2;

        while (await SlugExistsAsync(slug, productId))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }

    private async Task<bool> SlugExistsAsync(string slug, int? productId)
    {
        var products = _dbContext.Products.Where(product => product.Slug == slug);

        if (productId.HasValue)
        {
            products = products.Where(product => product.Id != productId.Value);
        }

        return await products.AnyAsync();
    }

    private static string CreateSlug(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        var characters = normalized.Select(character => char.IsLetterOrDigit(character) ? character : '-').ToArray();
        var slug = string.Join('-', new string(characters).Split('-', StringSplitOptions.RemoveEmptyEntries));

        return string.IsNullOrWhiteSpace(slug) ? "product" : slug;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

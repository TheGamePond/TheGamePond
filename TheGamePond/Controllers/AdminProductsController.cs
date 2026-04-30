using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models.Admin;
using TheGamePond.Models.Catalog;

namespace TheGamePond.Controllers;

[Authorize(Roles = $"{AppRoles.Owner},{AppRoles.Admin},{AppRoles.Staff}")]
[Route("Admin/Products")]
public class AdminProductsController : Controller
{
    private const long MaxImageBytes = 4 * 1024 * 1024;
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AdminProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, int? categoryId, ProductStatus? status)
    {
        var products = _context.Products
            .Include(product => product.Category)
            .Include(product => product.InventoryItem)
            .Include(product => product.Images)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchText = search.Trim();
            products = products.Where(product =>
                product.Name.Contains(searchText) ||
                product.Sku.Contains(searchText) ||
                (product.Barcode != null && product.Barcode.Contains(searchText)) ||
                (product.Platform != null && product.Platform.Contains(searchText)));
        }

        if (categoryId.HasValue)
        {
            products = products.Where(product => product.CategoryId == categoryId.Value);
        }

        if (status.HasValue)
        {
            products = products.Where(product => product.Status == status.Value);
        }

        var model = new ProductIndexViewModel
        {
            Search = search,
            CategoryId = categoryId,
            Status = status,
            Categories = await GetActiveCategoriesAsync(),
            Products = await products
                .OrderBy(product => product.Name)
                .ToListAsync()
        };

        return View(model);
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        await PopulateCategoryOptionsAsync();
        ViewData["FormTitle"] = "Create Product";
        ViewData["SubmitLabel"] = "Create product";

        return View(new ProductFormViewModel());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        await ValidateProductFormAsync(model);
        ValidateImageUpload(model);

        if (!ModelState.IsValid)
        {
            await PopulateCategoryOptionsAsync();
            ViewData["FormTitle"] = "Create Product";
            ViewData["SubmitLabel"] = "Create product";
            return View(model);
        }

        var product = new Product
        {
            Name = model.Name.Trim(),
            Slug = await CreateUniqueSlugAsync(model.Name),
            Description = model.Description?.Trim(),
            Sku = NormalizeSku(model.Sku),
            Barcode = NormalizeOptional(model.Barcode),
            Platform = NormalizeOptional(model.Platform),
            Franchise = NormalizeOptional(model.Franchise),
            CategoryId = model.CategoryId,
            Condition = model.Condition,
            Status = model.Status,
            CostPrice = model.CostPrice,
            SalePrice = model.SalePrice,
            InventoryItem = new InventoryItem
            {
                QuantityOnHand = model.QuantityOnHand,
                LowStockThreshold = model.LowStockThreshold,
                LocationCode = NormalizeOptional(model.LocationCode)
            }
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        await AddUploadedImageAsync(product, model);

        if (model.QuantityOnHand > 0)
        {
            _context.StockAdjustments.Add(new StockAdjustment
            {
                ProductId = product.Id,
                QuantityDelta = model.QuantityOnHand,
                QuantityAfter = model.QuantityOnHand,
                Reason = StockAdjustmentReason.InitialStock,
                Notes = "Initial product creation stock.",
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            });

        }

        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Product created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products
            .Include(item => item.InventoryItem)
            .Include(item => item.Images)
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        await PopulateCategoryOptionsAsync();
        ViewData["FormTitle"] = "Edit Product";
        ViewData["SubmitLabel"] = "Save changes";

        return View(ToFormModel(product));
    }

    [HttpPost("{id:int}/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }

        await ValidateProductFormAsync(model, id);
        ValidateImageUpload(model);

        if (!ModelState.IsValid)
        {
            model.ExistingImages = await GetImageSummariesAsync(id);
            await PopulateCategoryOptionsAsync();
            ViewData["FormTitle"] = "Edit Product";
            ViewData["SubmitLabel"] = "Save changes";
            return View(model);
        }

        var product = await _context.Products
            .Include(item => item.InventoryItem)
            .Include(item => item.Images)
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        var oldQuantity = product.InventoryItem?.QuantityOnHand ?? 0;
        var newQuantity = model.QuantityOnHand;

        product.Name = model.Name.Trim();
        product.Slug = await CreateUniqueSlugAsync(model.Name, id);
        product.Description = model.Description?.Trim();
        product.Sku = NormalizeSku(model.Sku);
        product.Barcode = NormalizeOptional(model.Barcode);
        product.Platform = NormalizeOptional(model.Platform);
        product.Franchise = NormalizeOptional(model.Franchise);
        product.CategoryId = model.CategoryId;
        product.Condition = model.Condition;
        product.Status = model.Status;
        product.CostPrice = model.CostPrice;
        product.SalePrice = model.SalePrice;
        product.UpdatedAt = DateTimeOffset.UtcNow;

        product.InventoryItem ??= new InventoryItem { ProductId = product.Id };
        product.InventoryItem.QuantityOnHand = newQuantity;
        product.InventoryItem.LowStockThreshold = model.LowStockThreshold;
        product.InventoryItem.LocationCode = NormalizeOptional(model.LocationCode);
        product.InventoryItem.UpdatedAt = DateTimeOffset.UtcNow;

        var quantityDelta = newQuantity - oldQuantity;

        if (quantityDelta != 0)
        {
            _context.StockAdjustments.Add(new StockAdjustment
            {
                ProductId = product.Id,
                QuantityDelta = quantityDelta,
                QuantityAfter = newQuantity,
                Reason = StockAdjustmentReason.Correction,
                Notes = "Admin product edit quantity adjustment.",
                CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            });
        }

        await AddUploadedImageAsync(product, model);
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Product updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/Stock")]
    public async Task<IActionResult> Stock(int id)
    {
        var product = await _context.Products
            .Include(item => item.InventoryItem)
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        return View(new StockAdjustmentViewModel
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Sku = product.Sku,
            CurrentQuantity = product.InventoryItem?.QuantityOnHand ?? 0
        });
    }

    [HttpPost("{id:int}/Stock")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Stock(int id, StockAdjustmentViewModel model)
    {
        if (model.ProductId != id)
        {
            return BadRequest();
        }

        var product = await _context.Products
            .Include(item => item.InventoryItem)
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        var currentQuantity = product.InventoryItem?.QuantityOnHand ?? 0;
        model.ProductName = product.Name;
        model.Sku = product.Sku;
        model.CurrentQuantity = currentQuantity;

        if (model.QuantityDelta == 0)
        {
            ModelState.AddModelError(nameof(StockAdjustmentViewModel.QuantityDelta), "Enter a positive or negative quantity change.");
        }

        if (model.NewQuantity < 0)
        {
            ModelState.AddModelError(nameof(StockAdjustmentViewModel.QuantityDelta), "Stock cannot go below zero.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        product.InventoryItem ??= new InventoryItem { ProductId = product.Id };
        product.InventoryItem.QuantityOnHand = model.NewQuantity;
        product.InventoryItem.UpdatedAt = DateTimeOffset.UtcNow;

        _context.StockAdjustments.Add(new StockAdjustment
        {
            ProductId = product.Id,
            QuantityDelta = model.QuantityDelta,
            QuantityAfter = model.NewQuantity,
            Reason = model.Reason,
            Notes = NormalizeOptional(model.Notes),
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Stock adjusted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/Images/{imageId:int}/Primary")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPrimaryImage(int id, int imageId)
    {
        var product = await _context.Products
            .Include(item => item.Images)
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        var primaryImage = product.Images.FirstOrDefault(image => image.Id == imageId);

        if (primaryImage is null)
        {
            return NotFound();
        }

        foreach (var image in product.Images)
        {
            image.IsPrimary = image.Id == primaryImage.Id;
        }

        product.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

        TempData["StatusMessage"] = "Primary image updated.";
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost("{id:int}/Images/{imageId:int}/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        var product = await _context.Products
            .Include(item => item.Images)
            .FirstOrDefaultAsync(product => product.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        var imageToDelete = product.Images.FirstOrDefault(image => image.Id == imageId);

        if (imageToDelete is null)
        {
            return NotFound();
        }

        var deletedImagePath = imageToDelete.ImagePath;
        var shouldPromotePrimaryImage = imageToDelete.IsPrimary;
        _context.ProductImages.Remove(imageToDelete);

        if (shouldPromotePrimaryImage)
        {
            var nextPrimaryImage = product.Images
                .Where(image => image.Id != imageToDelete.Id)
                .OrderBy(image => image.SortOrder)
                .ThenBy(image => image.Id)
                .FirstOrDefault();

            if (nextPrimaryImage is not null)
            {
                nextPrimaryImage.IsPrimary = true;
            }
        }

        product.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
        DeleteImageFileIfExists(deletedImagePath);

        TempData["StatusMessage"] = "Image deleted.";
        return RedirectToAction(nameof(Edit), new { id });
    }

    private async Task ValidateProductFormAsync(ProductFormViewModel model, int? productId = null)
    {
        if (!await _context.ProductCategories.AnyAsync(category => category.Id == model.CategoryId && category.IsActive))
        {
            ModelState.AddModelError(nameof(ProductFormViewModel.CategoryId), "Choose an active category.");
        }

        var sku = NormalizeSku(model.Sku);

        if (await _context.Products.AnyAsync(product => product.Sku == sku && product.Id != productId))
        {
            ModelState.AddModelError(nameof(ProductFormViewModel.Sku), "That SKU is already in use.");
        }
    }

    private async Task<IReadOnlyList<ProductCategory>> GetActiveCategoriesAsync()
    {
        return await _context.ProductCategories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .ToListAsync();
    }

    private async Task PopulateCategoryOptionsAsync()
    {
        ViewBag.CategoryOptions = (await GetActiveCategoriesAsync())
            .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
            .ToList();
    }

    private void ValidateImageUpload(ProductFormViewModel model)
    {
        if (model.ImageUpload is null)
        {
            return;
        }

        if (model.ImageUpload.Length == 0)
        {
            ModelState.AddModelError(nameof(ProductFormViewModel.ImageUpload), "Choose a non-empty image file.");
            return;
        }

        if (model.ImageUpload.Length > MaxImageBytes)
        {
            ModelState.AddModelError(nameof(ProductFormViewModel.ImageUpload), "Image must be 4 MB or smaller.");
        }

        var extension = Path.GetExtension(model.ImageUpload.FileName);

        if (!AllowedImageExtensions.Contains(extension))
        {
            ModelState.AddModelError(nameof(ProductFormViewModel.ImageUpload), "Use a JPG, PNG, or WEBP image.");
        }
    }

    private async Task AddUploadedImageAsync(Product product, ProductFormViewModel model)
    {
        if (model.ImageUpload is null)
        {
            return;
        }

        var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "products", product.Id.ToString());
        Directory.CreateDirectory(uploadsRoot);

        var extension = Path.GetExtension(model.ImageUpload.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadsRoot, fileName);

        await using (var stream = System.IO.File.Create(filePath))
        {
            await model.ImageUpload.CopyToAsync(stream);
        }

        var imagePath = $"/uploads/products/{product.Id}/{fileName}";
        var hasPrimaryImage = product.Images.Any(image => image.IsPrimary);

        product.Images.Add(new ProductImage
        {
            ImagePath = imagePath,
            AltText = product.Name,
            IsPrimary = !hasPrimaryImage,
            SortOrder = product.Images.Count + 1
        });
    }

    private async Task<IReadOnlyList<ProductImageSummaryViewModel>> GetImageSummariesAsync(int productId)
    {
        return await _context.ProductImages
            .AsNoTracking()
            .Where(image => image.ProductId == productId)
            .OrderByDescending(image => image.IsPrimary)
            .ThenBy(image => image.SortOrder)
            .Select(image => new ProductImageSummaryViewModel
            {
                Id = image.Id,
                ImagePath = image.ImagePath,
                AltText = image.AltText,
                IsPrimary = image.IsPrimary
            })
            .ToListAsync();
    }

    private async Task<string> CreateUniqueSlugAsync(string value, int? productId = null)
    {
        var baseSlug = CreateSlug(value);
        var slug = baseSlug;
        var suffix = 2;

        while (await _context.Products.AnyAsync(product => product.Slug == slug && product.Id != productId))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }

    private static ProductFormViewModel ToFormModel(Product product)
    {
        return new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku,
            Barcode = product.Barcode,
            Platform = product.Platform,
            Franchise = product.Franchise,
            CategoryId = product.CategoryId,
            Condition = product.Condition,
            Status = product.Status,
            CostPrice = product.CostPrice,
            SalePrice = product.SalePrice,
            QuantityOnHand = product.InventoryItem?.QuantityOnHand ?? 0,
            LowStockThreshold = product.InventoryItem?.LowStockThreshold ?? 1,
            LocationCode = product.InventoryItem?.LocationCode,
            ExistingImages = product.Images
                .OrderByDescending(image => image.IsPrimary)
                .ThenBy(image => image.SortOrder)
                .Select(image => new ProductImageSummaryViewModel
                {
                    Id = image.Id,
                    ImagePath = image.ImagePath,
                    AltText = image.AltText,
                    IsPrimary = image.IsPrimary
                })
                .ToList()
        };
    }

    private static string CreateSlug(string value)
    {
        var slug = Regex.Replace(value.Trim().ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? "product" : slug;
    }

    private static string NormalizeSku(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private void DeleteImageFileIfExists(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return;
        }

        var relativePath = imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, relativePath));
        var webRoot = Path.GetFullPath(_environment.WebRootPath);

        if (!webRoot.EndsWith(Path.DirectorySeparatorChar))
        {
            webRoot += Path.DirectorySeparatorChar;
        }

        if (!fullPath.StartsWith(webRoot, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(fullPath))
        {
            return;
        }

        try
        {
            System.IO.File.Delete(fullPath);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}

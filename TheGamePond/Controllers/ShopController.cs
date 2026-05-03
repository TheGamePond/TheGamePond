using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models.Catalog;
using TheGamePond.Models.Storefront;

namespace TheGamePond.Controllers;

[Route("Shop")]
public class ShopController : Controller
{
    private readonly ApplicationDbContext _context;

    public ShopController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? search, string? category, string? sort)
    {
        var selectedSort = NormalizeSort(sort);
        var categories = await _context.ProductCategories
            .AsNoTracking()
            .Where(item => item.IsActive)
            .OrderBy(item => item.SortOrder)
            .ThenBy(item => item.Name)
            .ToListAsync();

        var products = _context.Products
            .Include(product => product.Category)
            .Include(product => product.InventoryItem)
            .Include(product => product.Images)
            .AsNoTracking()
            .Where(product => product.Status == ProductStatus.Active);

        if (!string.IsNullOrWhiteSpace(category))
        {
            var categorySlug = category.Trim();
            products = products.Where(product => product.Category != null && product.Category.Slug == categorySlug);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchText = search.Trim();
            products = products.Where(product =>
                product.Name.Contains(searchText) ||
                product.Sku.Contains(searchText) ||
                (product.Platform != null && product.Platform.Contains(searchText)) ||
                (product.Franchise != null && product.Franchise.Contains(searchText)));
        }

        products = selectedSort switch
        {
            "name" => products.OrderBy(product => product.Name),
            "price-low" => products.OrderBy(product => product.SalePrice).ThenBy(product => product.Name),
            "price-high" => products.OrderByDescending(product => product.SalePrice).ThenBy(product => product.Name),
            _ => products.OrderByDescending(product => product.CreatedAt).ThenBy(product => product.Name)
        };

        var productList = await products.ToListAsync();
        var selectedCategoryName = categories.FirstOrDefault(item => item.Slug == category)?.Name;

        var model = new ShopIndexViewModel
        {
            Search = search,
            Category = category,
            Sort = selectedSort,
            SelectedCategoryName = selectedCategoryName,
            Categories = categories,
            Products = productList.Select(ToCardModel).ToList()
        };

        return View(model);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Details(string slug, string? search, string? category, string? sort)
    {
        var selectedSort = NormalizeSort(sort);
        var product = await _context.Products
            .Include(item => item.Category)
            .Include(item => item.InventoryItem)
            .Include(item => item.Images)
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Slug == slug && product.Status == ProductStatus.Active);

        if (product is null)
        {
            return NotFound();
        }

        return View(ToDetailModel(product, search, category, selectedSort));
    }

    private static ShopProductCardViewModel ToCardModel(Product product)
    {
        var primaryImage = product.Images
            .OrderByDescending(image => image.IsPrimary)
            .ThenBy(image => image.SortOrder)
            .FirstOrDefault();

        return new ShopProductCardViewModel
        {
            Name = product.Name,
            Slug = product.Slug,
            Sku = product.Sku,
            Platform = product.Platform,
            Franchise = product.Franchise,
            CategoryName = product.Category?.Name,
            Condition = product.Condition,
            SalePrice = product.SalePrice,
            QuantityOnHand = product.InventoryItem?.QuantityOnHand ?? 0,
            PrimaryImagePath = primaryImage?.ImagePath,
            PrimaryImageAltText = primaryImage?.AltText
        };
    }

    private static ProductDetailViewModel ToDetailModel(
        Product product,
        string? shopSearch,
        string? shopCategory,
        string shopSort)
    {
        return new ProductDetailViewModel
        {
            Name = product.Name,
            Slug = product.Slug,
            Sku = product.Sku,
            Description = product.Description,
            Platform = product.Platform,
            Franchise = product.Franchise,
            CategoryName = product.Category?.Name,
            Condition = product.Condition,
            SalePrice = product.SalePrice,
            QuantityOnHand = product.InventoryItem?.QuantityOnHand ?? 0,
            ShopSearch = shopSearch,
            ShopCategory = shopCategory,
            ShopSort = shopSort,
            Images = product.Images
                .OrderByDescending(image => image.IsPrimary)
                .ThenBy(image => image.SortOrder)
                .Select(image => new ShopProductImageViewModel
                {
                    ImagePath = image.ImagePath,
                    AltText = image.AltText,
                    IsPrimary = image.IsPrimary
                })
                .ToList()
        };
    }

    private static string NormalizeSort(string? sort)
    {
        return sort switch
        {
            "name" => "name",
            "price-low" => "price-low",
            "price-high" => "price-high",
            _ => "newest"
        };
    }
}

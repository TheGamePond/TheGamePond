using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheGamePond.Data;
using TheGamePond.Models.Catalog;

namespace TheGamePond.Controllers;

[Authorize(Roles = $"{AppRoles.Owner},{AppRoles.Admin},{AppRoles.Staff}")]
[Route("Admin/Categories")]
public class AdminCategoriesController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public AdminCategoriesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var categories = await _dbContext.ProductCategories
            .AsNoTracking()
            .Include(category => category.Products)
            .OrderBy(category => category.Name)
            .ToListAsync();

        return View(categories);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View(new ProductCategory());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCategory category)
    {
        category.Name = (category.Name ?? string.Empty).Trim();
        category.Slug = CreateSlug(category.Slug, category.Name);
        category.Description = string.IsNullOrWhiteSpace(category.Description) ? null : category.Description.Trim();

        await ValidateSlugAsync(category);

        if (!ModelState.IsValid)
        {
            return View(category);
        }

        _dbContext.ProductCategories.Add(category);
        await _dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Category created.";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("{id:int}/Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _dbContext.ProductCategories.FindAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost("{id:int}/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductCategory category)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }

        category.Name = (category.Name ?? string.Empty).Trim();
        category.Slug = CreateSlug(category.Slug, category.Name);
        category.Description = string.IsNullOrWhiteSpace(category.Description) ? null : category.Description.Trim();

        await ValidateSlugAsync(category);

        if (!ModelState.IsValid)
        {
            return View(category);
        }

        _dbContext.ProductCategories.Update(category);
        await _dbContext.SaveChangesAsync();
        TempData["StatusMessage"] = "Category updated.";

        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateSlugAsync(ProductCategory category)
    {
        var slugExists = await _dbContext.ProductCategories
            .AnyAsync(existingCategory => existingCategory.Slug == category.Slug && existingCategory.Id != category.Id);

        if (slugExists)
        {
            ModelState.AddModelError(nameof(category.Slug), "Slug must be unique.");
        }
    }

    private static string CreateSlug(string? slug, string name)
    {
        var source = string.IsNullOrWhiteSpace(slug) ? name : slug;
        var normalized = source.Trim().ToLowerInvariant();
        var characters = normalized.Select(character => char.IsLetterOrDigit(character) ? character : '-').ToArray();

        return string.Join('-', new string(characters).Split('-', StringSplitOptions.RemoveEmptyEntries));
    }
}
